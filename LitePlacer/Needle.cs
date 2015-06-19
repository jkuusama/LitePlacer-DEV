using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using LitePlacer.Properties;

namespace LitePlacer {
    public class NeedleClass {
        public struct NeedlePoint {
            public double Angle;
            public double X;  // X offset from nominal, in mm's, at angle
            public double Y;

            public PartLocation ToPartLocation() {
                return new PartLocation(X, Y, Angle);
            }

        }


        public List<NeedlePoint> CalibrationPoints = new List<NeedlePoint>();

        private static FormMain MainForm;

        public NeedleClass(FormMain MainF) {
            MainForm = MainF;
            Calibrated = false;
            CalibrationPoints.Clear();
        }

        // private bool probingMode;
        public void ProbingMode(bool set, bool JSON) {
            if (set) {
                if (JSON) {
                    // set in JSON mode
                    CNC_Write("{\"zsn\",0}");
                    Thread.Sleep(150);
                    CNC_Write("{\"zsx\",1}");
                    Thread.Sleep(150);
                    CNC_Write("{\"zzb\",0}");
                    Thread.Sleep(150);
                    // probingMode = true;
                } else {
                    // set in text mode
                    CNC_Write("$zsn=0");
                    Thread.Sleep(50);
                    CNC_Write("$zsx=1");
                    Thread.Sleep(50);
                    CNC_Write("$zzb=0");
                    Thread.Sleep(50);
                    // probingMode = true;
                }
            } else {
                if (JSON) {
                    // clear in JSON mode
                    CNC_Write("{\"zsn\",3}");
                    Thread.Sleep(50);
                    CNC_Write("{\"zsx\",2}");
                    Thread.Sleep(50);
                    CNC_Write("{\"zzb\",2}");
                    Thread.Sleep(50);
                    // probingMode = false;
                } else {
                    // clear in text mode
                    CNC_Write("$zsn=3");
                    Thread.Sleep(50);
                    CNC_Write("$zsx=2");
                    Thread.Sleep(50);
                    CNC_Write("$zzb=2");
                    Thread.Sleep(50);
                    // probingMode = false;
                }
            }

        }


        public bool Calibrated { get; set; }

        public bool CorrectedPosition_m(double angle, out double X, out double Y) {
            if (!Calibrated) {
                MainForm.DisplayText("Needle not calibrated - calibrating now", Color.Red);
                MainForm.TestNeedleRecognition_button_Click(null, null);
                if (!Calibrated) {
                    MainForm.ShowSimpleMessageBox("Needle not calibrated and calibration attempt failed");
                    X = 0; Y = 0;
                    return false;
                }
            }

            while (angle < 0) angle = angle + 360.0;
            while (angle > 360.0) angle = angle - 360.0;

            // since we are not going to check the last point (which is the cal. value for 360)
            // in the for loop,we check that now
            if (angle > 359.98) {
                X = CalibrationPoints[0].X;
                Y = CalibrationPoints[0].Y;
                return true;
            };

            for (int i = 0; i < CalibrationPoints.Count; i++) {
                if (Math.Abs(angle - CalibrationPoints[i].Angle) < 1.0) {
                    X = CalibrationPoints[i].X;
                    Y = CalibrationPoints[i].Y;
                    return true;
                }
                if ((angle > CalibrationPoints[i].Angle) && (angle < CalibrationPoints[i + 1].Angle) &&
                    (Math.Abs(angle - CalibrationPoints[i + 1].Angle) > 1.0)) {
                    // angle is between CalibrationPoints[i] and CalibrationPoints[i+1], and is not == CalibrationPoints[i+1]
                    double fract = (angle - CalibrationPoints[i + 1].Angle) / (CalibrationPoints[i + 1].Angle - CalibrationPoints[i].Angle);
                    X = CalibrationPoints[i].X + fract * (CalibrationPoints[i + 1].X - CalibrationPoints[i].X);
                    Y = CalibrationPoints[i].Y + fract * (CalibrationPoints[i + 1].Y - CalibrationPoints[i].Y);
                    return true;
                }
            }
            MainForm.ShowMessageBox(
                "Needle Calibration value read: value not found",
                "Sloppy programmer error",
                MessageBoxButtons.OK);
            X = 0;
            Y = 0;
            return false;
        }


        public bool Calibrate(double Tolerance) {
            //setup camera
            MainForm.cameraView.SetUpCameraFunctionSet("needle");

            // we are already @ upcamera position
            MainForm.ZUp_button_Click(null, null); // move needle up
            MainForm.GotoUpCamPosition_button_Click(null, null);
            MainForm.Cnc.CNC_Z_m(Settings.Default.General_ZtoPCB - 1.0);

            CalibrationPoints.Clear();   // Presumably user changed the needle, and calibration is void no matter if we succeed here
            Calibrated = false;

            for (int i = 0; i <= 3600; i = i + 225) {
                NeedlePoint Point = new NeedlePoint();
                Point.Angle = i / 10.0;

                if (!MainForm.Cnc.CNC_A_m(Point.Angle)) return false;
                //detect average of 3 measurements
                var circle = VideoDetection.GetClosestAverageCircle(MainForm.cameraView.upVideoProcessing, Tolerance, 3);

                if (circle == null) {
                    MainForm.ShowSimpleMessageBox("Needle Calibration: Can't see needle at angle " + Point.Angle + " - aborting");
                    return false;
                }

                circle.ToMMResolution();
                Point.X = circle.X;
                Point.Y = circle.Y;

                // display point
                var pt = circle.Clone().ToScreenResolution().ToPartLocation().ToPointF();
                MainForm.DisplayText("circle @ " + circle + "\tx @ " + pt.X + "," + pt.Y);
                MainForm.cameraView.downVideoProcessing.MarkA.Add(pt);

                CalibrationPoints.Add(Point);
            }
            Calibrated = true;

            MainForm.ZUp_button_Click(null, null); //move up
            MainForm.cameraView.SetUpCameraDefaults();


            return true;
        }

        public bool Move_m(PartLocation p) {
            return Move_m(p.X, p.Y, p.A);
        }

        public PartLocation NeedleOffset {
            get { return new PartLocation(Settings.Default.DownCam_NeedleOffsetX, Settings.Default.DownCam_NeedleOffsetY); }
            set {
                Settings.Default.DownCam_NeedleOffsetY = value.Y;
                Settings.Default.DownCam_NeedleOffsetX = value.X;
            }
        }

        public bool Move_m(double X, double Y, double A) {
            double dX;
            double dY;
            if (!CorrectedPosition_m(A, out dX, out dY)) return false;


            var loc = new PartLocation(X, Y, A);
            var wobble = new PartLocation(dX, dY, 0);

            var dest = loc - wobble + NeedleOffset;

            MainForm.DisplayText("== NEEDLE MOVE ==", Color.ForestGreen);
            MainForm.DisplayText(String.Format("pos {0} + offset {1} - wobble {2} = {3}", loc, NeedleOffset, wobble, dest), Color.ForestGreen);

            return Global.Instance.cnc.CNC_XYA_m(dest);
        }



        // =================================================================================
        // CNC interface functions
        // =================================================================================
        private bool CNC_Write(string s) {
            return Global.Instance.cnc.CNC_Write_m(s);
        }
    }
}

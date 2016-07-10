using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LitePlacer
{
    class NeedleClass
    {
        public struct NeedlePoint
        {
            public double Angle;
            public double X;  // X offset from nominal, in mm's, at angle
            public double Y;
        }

        public List<NeedlePoint> CalibrationPoints = new List<NeedlePoint>();
        public NeedlePoint CalibrationPointsCenter = new NeedlePoint();

        private Camera Cam;
        private CNC Cnc;
        private static FormMain MainForm;

        public NeedleClass(Camera MyCam, CNC MyCnc, FormMain MainF)
        {
            MainForm = MainF;
            Calibrated = false;
            Cam = MyCam;
            Cnc = MyCnc;
            CalibrationPoints.Clear();
        }

        // private bool probingMode;
        public void ProbingMode(bool set, bool JSON)
        {
            int wait= 150;
            if(set)
            {
                if(JSON)
                {
                    // set in JSON mode
                    CNC_Write("{\"zsn\",0}");
                    Thread.Sleep(wait);
                    CNC_Write("{\"zsx\",1}");
                    Thread.Sleep(wait);
                    CNC_Write("{\"zzb\",0}");
                    Thread.Sleep(wait);
                    // probingMode = true;
                }
                else
                {
                    // set in text mode
                    CNC_Write("$zsn=0");
                    Thread.Sleep(wait);
                    CNC_Write("$zsx=1");
                    Thread.Sleep(wait);
                    CNC_Write("$zzb=0");
                    Thread.Sleep(wait);
                    // probingMode = true;
                }
            }            
            else
            {
                if (JSON)
                {
                    // clear in JSON mode
                    CNC_Write("{\"zsn\",3}");
                    Thread.Sleep(wait);
                    CNC_Write("{\"zsx\",2}");
                    Thread.Sleep(wait);
                    CNC_Write("{\"zzb\",2}");
                    Thread.Sleep(wait);
                    // probingMode = false;
                }
                else
                {
                    // clear in text mode
                    CNC_Write("$zsn=3");
                    Thread.Sleep(wait);
                    CNC_Write("$zsx=2");
                    Thread.Sleep(wait);
                    CNC_Write("$zzb=2");
                    Thread.Sleep(wait);
                    // probingMode = false;
                }
            }

        }


        public bool Calibrated { get; set; }

        public bool CorrectedPosition_m(double angle, out double X, out double Y)
        {
            if (Properties.Settings.Default.Placement_OmitNeedleCalibration)
            {
                X = Properties.Settings.Default.DownCam_NeedleOffsetX;
                Y = Properties.Settings.Default.DownCam_NeedleOffsetY;
                return true;
            };

            if (!Calibrated)
            {
                DialogResult dialogResult = MainForm.ShowMessageBox(
                    "Needle not calibrated. Calibrate now?",
                    "Needle not calibrated", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    X = Properties.Settings.Default.DownCam_NeedleOffsetX;
                    Y = Properties.Settings.Default.DownCam_NeedleOffsetY;
                    return false;
                };
                double CurrX = Cnc.CurrentX;
                double CurrY = Cnc.CurrentY;
                double CurrA = Cnc.CurrentA;
                if(!MainForm.CalibrateNeedle_m())
                {
                    X = Properties.Settings.Default.DownCam_NeedleOffsetX;
                    Y = Properties.Settings.Default.DownCam_NeedleOffsetY;
                    return false;
                }
                if (!MainForm.CNC_XYA_m(CurrX, CurrY, CurrA))
                {
                    X = Properties.Settings.Default.DownCam_NeedleOffsetX;
                    Y = Properties.Settings.Default.DownCam_NeedleOffsetY;
                    return false;
                }
            };
            double VirtualUpCamCncX = Properties.Settings.Default.UpCam_PositionX - Properties.Settings.Default.DownCam_NeedleOffsetX;
            double VirtualUpCamCncY = Properties.Settings.Default.UpCam_PositionY - Properties.Settings.Default.DownCam_NeedleOffsetY;

            while (angle < 0)
            {
                angle = angle + 360.0;
            };
            while (angle > 360.0)
            {
                angle = angle - 360.0;
            }
            // since we are not going to check the last point (which is the cal. value for 360)
            // in the for loop,we check that now
            if (angle > 359.98)
            {
                X = CalibrationPoints[0].X - VirtualUpCamCncX;
                Y = CalibrationPoints[0].Y - VirtualUpCamCncY;
                return true;
            };

            for (int i = 0; i < CalibrationPoints.Count; i++)
            {
                if (Math.Abs(angle - CalibrationPoints[i].Angle) < 1.0)
                {
                    X = CalibrationPoints[i].X - VirtualUpCamCncX;
                    Y = CalibrationPoints[i].Y - VirtualUpCamCncY;
					return true;
                }
                if ((angle > CalibrationPoints[i].Angle)
                    &&
                    (angle < CalibrationPoints[i + 1].Angle)
                    &&
                    (Math.Abs(angle - CalibrationPoints[i + 1].Angle) > 1.0))
                {
                    // angle is between CalibrationPoints[i] and CalibrationPoints[i+1], and is not == CalibrationPoints[i+1]
                    double fract = (angle - CalibrationPoints[i+1].Angle) / (CalibrationPoints[i+1].Angle - CalibrationPoints[i].Angle);
                    X = CalibrationPoints[i].X + fract * (CalibrationPoints[i + 1].X - CalibrationPoints[i].X) - VirtualUpCamCncX;
                    Y = CalibrationPoints[i].Y + fract * (CalibrationPoints[i + 1].Y - CalibrationPoints[i].Y) - VirtualUpCamCncY;
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


        public bool Calibrate(double Tolerance)
        {
            if (Properties.Settings.Default.Placement_OmitNeedleCalibration)
            {
                return true;
            };

            CalibrationPoints.Clear();   // Presumably user changed the needle, and calibration is void no matter if we succeed here
            Calibrated = false;
            if (!Cam.IsRunning())
            {
                MainForm.ShowMessageBox(
                    "Attempt to calibrate needle, camera is not running.",
                    "Camera not running",
                    MessageBoxButtons.OK);
                return false;
            }

			double X = 0;
			double Y = 0;
            double Dist;
			int res = 0; ;
            for (int i = 0; i <= 3600; i = i + 225)
            {
                NeedlePoint Point = new NeedlePoint();
                Point.Angle = Convert.ToDouble(i) / 10.0;
				if (!CNC_A_m(Point.Angle))
				{
					return false;
				}
				for (int tries = 0; tries < 10; tries++)
				{
    				Thread.Sleep(10);
                    Application.DoEvents(); // Give video processing a chance to catch a new frame
                    res = Cam.GetClosestCircle(out X, out Y, Tolerance);
					if (res != 0)
					{
                        X *= Properties.Settings.Default.UpCam_XmmPerPixel;
                        Y *= Properties.Settings.Default.UpCam_YmmPerPixel;
                        Dist = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
                        if (Dist < 0.01) break;
                        //Not close enough yet, move closer
                        CNC_XY_m(Cnc.CurrentX + X, Cnc.CurrentY + Y);
					}

				}
                if (res == 0)
                {
                    MainForm.ShowMessageBox(
                        "Needle Calibration: Can't find needle",
                        "No Circle found",
                        MessageBoxButtons.OK);
                    return false;
                }
                //if (res > 1)
                //{
                //    MessageBox.Show(
                //        "Needle Calibration: Ambiguous regognition result",
                //        "Too macy circles in focus",
                //        MessageBoxButtons.OK);
                //    return false;
                //}

                // Big Change!! Now recording camera location in CNC domain when needle is centered
                // The difference between this and the virtual camera position gives the true offset
                Point.X = Cnc.CurrentX + X;
                Point.Y = Cnc.CurrentY + Y;
				// MainForm.DisplayText("A: " + Point.Angle.ToString("0.000") + ", X: " + Point.X.ToString("0.000") + ", Y: " + Point.Y.ToString("0.000"));
                CalibrationPoints.Add(Point);
            }
            /*********************************************************************************
             * Here we are going to calculate the center of the calibration points.  It will
             * be the center of the line connecting each two points that are 180 degrees apart.
             * We are assuming that the points are on even spacing, cover 360 degrees and that
             * there are an even number of points.  For example, 16 points with 22.5 degree
             * spacing will work.  In the ideal world all of the centers will be at the same
             * point.  But to account for some variation we will take the average of all the
             * centers.  To do that we will sum all of the points in the following loop and
             * divide the sum by the loop count.
             * 
             * This point will be used for following calibration cycles to center the wobble
             * on the up camera.
             *
             ********************************************************************************/
            // There are half as many lines as there are points
            int HalfCount = CalibrationPoints.Count / 2;
            CalibrationPointsCenter.X = 0;
            CalibrationPointsCenter.Y = 0;
            for (int i = 0; i < HalfCount; i++)
            {
                //The following block could be used if one is anal about validating the situation
                //if(CalibrationPoints[i + HalfCount].Angle - CalibrationPoints[i ].Angle != 180)
                //{
                //    Calibrated = false;
                //    return false;
                //}
                CalibrationPointsCenter.X += (CalibrationPoints[i].X + CalibrationPoints[i + HalfCount].X) / 2;
                CalibrationPointsCenter.Y += (CalibrationPoints[i].Y + CalibrationPoints[i + HalfCount].Y) / 2;
            }
            CalibrationPointsCenter.X /= HalfCount;
            CalibrationPointsCenter.Y /= HalfCount;
            Calibrated = true;
            return true;
        }

        public bool Move_m(double X, double Y, double A)
        {
            double dX;
            double dY;
			MainForm.DisplayText("Needle.Move_m(): X= " + X.ToString() + ", Y= " + Y.ToString() + ", A= " + A.ToString());
			if (!CorrectedPosition_m(A, out dX, out dY))
			{
				return false;
			};
            return CNC_XYA(X + dX, Y + dY, A);
        }



        // =================================================================================
        // CNC interface functions
        // =================================================================================
        
        private bool CNC_A_m(double A)
        {
			return MainForm.CNC_A_m(A);
        }

        private bool CNC_XY_m(double X, double Y)
        {
			return MainForm.CNC_XY_m(X, Y);
        }

        private bool CNC_XYA(double X, double Y, double A)
        {
			return MainForm.CNC_XYA_m(X, Y, A);
        }

        private bool CNC_Write(string s)
        {
			return MainForm.CNC_Write_m(s);
        }
    }
}

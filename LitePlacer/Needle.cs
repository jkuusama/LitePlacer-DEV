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
            if(set)
            {
                if(JSON)
                {
                    // set in JSON mode
                    CNC_Write("{\"zsn\",0}"); 
                    Thread.Sleep(150);
                    CNC_Write("{\"zsx\",1}"); 
                    Thread.Sleep(150);
                    CNC_Write("{\"zzb\",0}"); 
                    Thread.Sleep(150);
                    // probingMode = true;
                }
                else
                {
                    // set in text mode
                    CNC_Write("$zsn=0");
                    Thread.Sleep(50);
                    CNC_Write("$zsx=1");
                    Thread.Sleep(50);
                    CNC_Write("$zzb=0");
                    Thread.Sleep(50);
                    // probingMode = true;
                }
            }            
            else
            {
                if (JSON)
                {
                    // clear in JSON mode
                    CNC_Write("{\"zsn\",3}");
                    Thread.Sleep(50);
                    CNC_Write("{\"zsx\",2}");
                    Thread.Sleep(50);
                    CNC_Write("{\"zzb\",2}");
                    Thread.Sleep(50);
                    // probingMode = false;
                }
                else
                {
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

        public bool CorrectedPosition_m(double angle, out double X, out double Y)
        {
            if (!Calibrated)
            {
                MainForm.ShowMessageBox(
					"CorrectedPosition() call, needle not calibrated.",
					"Sloppy programmer error",
					MessageBoxButtons.OK);
				X = 0;
				Y = 0;
				return false;
			}
            else
            {
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
                    X = CalibrationPoints[0].X;
                    Y = CalibrationPoints[0].Y;
                    return true;
                };

                for (int i = 0; i < CalibrationPoints.Count; i++)
                {
                    if (Math.Abs(angle - CalibrationPoints[i].Angle) < 1.0)
                    {
                        X = CalibrationPoints[i].X;
                        Y = CalibrationPoints[i].Y;
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
        }


        public bool Calibrate(double Tolerance)
        {
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
			int res = 0; ;
            for (int i = 0; i <= 3600; i = i + 225)
            {
                NeedlePoint Point = new NeedlePoint();
                Point.Angle = Convert.ToDouble(i) / 10.0;
                if (!MainForm.CNC_A_m(Point.Angle))
				{
					return false;
				}
				for (int tries = 0; tries < 10; tries++)
				{
    				Thread.Sleep(100);
					res = Cam.GetClosestCircle(out X, out Y, Tolerance);
					if (res != 0)
					{
						break;
					}

					if (tries >= 9)
					{
                        MainForm.ShowMessageBox(
							"Needle calibration: Can't see Needle",
							"No Circle found",
							MessageBoxButtons.OK);
						return false;
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

                Point.X = X * Properties.Settings.Default.UpCam_XmmPerPixel;
                Point.Y = Y * Properties.Settings.Default.UpCam_YmmPerPixel;
				// MainForm.DisplayText("A: " + Point.Angle.ToString("0.000") + ", X: " + Point.X.ToString("0.000") + ", Y: " + Point.Y.ToString("0.000"));
                CalibrationPoints.Add(Point);
            }
            Calibrated = true;
            return true;
        }

        public bool Move_m(PartLocation p) {
            return Move_m(p.X, p.Y, p.A);
        }

        public bool Move_m(double X, double Y, double A)        {
            double dX;
            double dY;
			if (!CorrectedPosition_m(A, out dX, out dY))
			{
				return false;
			};
            double Xoff = Properties.Settings.Default.DownCam_NeedleOffsetX;
            double Yoff = Properties.Settings.Default.DownCam_NeedleOffsetY;

            var loc = new PartLocation(X, Y, A);
            var wobble = new PartLocation(dX, dY, 0);
            var needle_offset = new PartLocation(Xoff, Yoff, 0);
            var dest = loc+wobble+needle_offset;
            MainForm.DisplayText("== NEEDLE MOVE ==", System.Drawing.Color.ForestGreen);
            MainForm.DisplayText(String.Format("pos {0} + offset {1} + wobble {2} = {3}", loc, needle_offset, wobble, dest), System.Drawing.Color.ForestGreen); 
        
            return MainForm.CNC_XYA_m(dest);
        }



        // =================================================================================
        // CNC interface functions
        // =================================================================================
        private bool CNC_Write(string s)  {
			return MainForm.CNC_Write_m(s);
        }
    }
}

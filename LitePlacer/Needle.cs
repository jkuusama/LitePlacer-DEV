using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace LitePlacer
{
    [Serializable()]
    class NeedleClass
    {
        [Serializable()]
        public struct NeedlePoint
        {
            public double Angle;
            public double X;  // X offset from nominal, in mm's, at angle
            public double Y;
        }

		public List<NeedlePoint> CalibrationPoints = new List<NeedlePoint>();   // what we use
        // to calibrate nozzles, we can store and restore calibration points and their validity here.
        public List<NeedlePoint>[] CalibrationPointsArr = new List<NeedlePoint>[Properties.Settings.Default.Nozzles_maximum];
        public bool[] CalibratedArr = new bool[Properties.Settings.Default.Nozzles_maximum];

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

        // =================================================================================
        // store and restore

        private object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }

        public void Store(int nozzle)
        {
            MainForm.DisplayText("Stored calibration for nozzle " + nozzle.ToString());
            CalibrationPointsArr[nozzle] = (List<NeedlePoint>)DeepClone(CalibrationPoints);
            CalibratedArr[nozzle] = Calibrated;
        }

        public void UseCalibration(int nozzle)
        {
            MainForm.DisplayText("Using calibration for nozzle " + nozzle.ToString());
            CalibrationPoints = (List<NeedlePoint>)DeepClone(CalibrationPointsArr[nozzle]);
            Calibrated = CalibratedArr[nozzle];
        }

        // =================================================================================
        // save and load from disk
        // =================================================================================

        public void SaveCalibration(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            MainForm.DisplayText("Saving nozzle calibration data");
            formatter.Serialize(stream, CalibrationPointsArr);
            MainForm.DisplayText("Saving nozzle calibration validity data");
            formatter.Serialize(stream, CalibratedArr);
            stream.Close();
        }

        public void LoadCalibration(string filename)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.Open(filename, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                MainForm.DisplayText("Loading nozzle calibration data");
                CalibrationPointsArr = (List<NeedlePoint>[])formatter.Deserialize(stream);
                MainForm.DisplayText("Loading nozzle calibration validity data");
                CalibratedArr = (bool[])formatter.Deserialize(stream);
                stream.Close();
            }
            else
            {
                MainForm.DisplayText("No nozzle calibration data");
                for (int i = 0; i < CalibratedArr.Length; i++)
                {
                    CalibratedArr[i] = false;
                }
            }
        }


        // =================================================================================
        // private bool probingMode;
        public void ProbingMode(bool set, bool JSON)
        {
            int wait= 250;
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
                X = 0.0;
                Y = 0.0;
                return true;
            };

            if (!Calibrated)
            {
                DialogResult dialogResult = MainForm.ShowMessageBox(
                    "Needle not calibrated. Calibrate now?",
                    "Needle not calibrated", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    X = 0.0;
                    Y = 0.0;
                    return false;
                };
                double CurrX = Cnc.CurrentX;
                double CurrY = Cnc.CurrentY;
                double CurrA = Cnc.CurrentA;
                if(!MainForm.CalibrateNeedle_m())
                {
                    X = 0;
                    Y = 0;
                    return false;
                }
                if (!MainForm.CNC_XYA_m(CurrX, CurrY, CurrA))
                {
                    X = 0;
                    Y = 0;
                    return false;
                }
            };

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
                    // asked the exact calibrated value
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
                    // linear interpolation:
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


        public bool Calibrate()
        {
            if (Properties.Settings.Default.Placement_OmitNeedleCalibration)
            {
                return true;
            };

            CalibrationPoints.Clear();   // Presumably calibration is void no matter if we succeed here
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
            double Maxdistance = Properties.Settings.Default.Nozzles_CalibrationDistance / Properties.Settings.Default.UpCam_XmmPerPixel;
            double radius = 0;
            int res = 0;
            // I goes in .1 of degrees. Makes sense to have the increase so, that multiplies of 45 are hit
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
                    res = Cam.GetSmallestCircle(out X, out Y, out radius, Maxdistance);
                    if (res != 0)
                    {
                        break;
                    }

                    Thread.Sleep(100);
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
                Point.X = X * Properties.Settings.Default.UpCam_XmmPerPixel;
                Point.Y = Y * Properties.Settings.Default.UpCam_YmmPerPixel;
                // MainForm.DisplayText("A: " + Point.Angle.ToString("0.000") + ", X: " + Point.X.ToString("0.000") + ", Y: " + Point.Y.ToString("0.000"));
                CalibrationPoints.Add(Point);
            }
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
            double Xoff = Properties.Settings.Default.DownCam_NeedleOffsetX;
            double Yoff = Properties.Settings.Default.DownCam_NeedleOffsetY;
            return CNC_XYA(X + Xoff + dX, Y + Yoff + dY, A);
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

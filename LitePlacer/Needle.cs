using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

namespace LitePlacer
{
    [Serializable()]
    class NozzleClass
    {
        [Serializable()]
        public struct NozzlePoint
        {
            public double Angle;
            public double X;  // X offset from nominal, in mm's, at angle
            public double Y;
        }

		public List<NozzlePoint> CalibrationPoints = new List<NozzlePoint>();   // what we use
        // to calibrate nozzles, we can store and restore calibration points and their validity here.
        public List<NozzlePoint>[] CalibrationPointsArr;
        public bool[] CalibratedArr;

        private Camera Cam;
        private CNC Cnc;
        private static FormMain MainForm;

        public NozzleClass(Camera MyCam, CNC MyCnc, FormMain MainF)
        {
            MainForm = MainF;
            Calibrated = false;
            Cam = MyCam;
            Cnc = MyCnc;
            CalibrationPointsArr = new List<NozzlePoint>[MainForm.Setting.Nozzles_maximum];
            CalibrationPoints.Clear();
            CalibratedArr = new bool[MainForm.Setting.Nozzles_maximum];
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
            CalibrationPointsArr[nozzle] = (List<NozzlePoint>)DeepClone(CalibrationPoints);
            CalibratedArr[nozzle] = Calibrated;
        }

        public void UseCalibration(int nozzle)
        {
            if (CalibratedArr[nozzle])
            {
            MainForm.DisplayText("Using calibration for nozzle " + nozzle.ToString());
            CalibrationPoints = (List<NozzlePoint>)DeepClone(CalibrationPointsArr[nozzle]);
            }
            Calibrated = CalibratedArr[nozzle];
        }

        // =================================================================================
        // save and load from disk
        // =================================================================================

        public bool SaveCalibration(string filename)
        {
            try
            {
                Stream stream = File.Open(filename, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                MainForm.DisplayText("Saving nozzle calibration data");
                formatter.Serialize(stream, CalibrationPointsArr);
                MainForm.DisplayText("Saving nozzle calibration validity data");
                formatter.Serialize(stream, CalibratedArr);
                stream.Flush();
                stream.Close();
                return true;
            }
            catch (System.Exception excep)
            {
                MainForm.DisplayText("Saving nozzle calibration data failed: " + excep.Message);
                return false;
            }
        }

        public void LoadCalibration(string filename)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.Open(filename, FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MainForm.DisplayText("Loading nozzle calibration data");
                    CalibrationPointsArr = (List<NozzlePoint>[])formatter.Deserialize(stream);
                    MainForm.DisplayText("Loading nozzle calibration validity data");
                    CalibratedArr = (bool[])formatter.Deserialize(stream);
                    stream.Flush();
                    stream.Close();
                }
                catch (Exception)
                {
                    stream.Close();
                    MainForm.DisplayText("No nozzle calibration data");
                    for (int i = 0; i < CalibratedArr.Length; i++)
                    {
                        CalibratedArr[i] = false;
                    }
                }
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
 
        public bool Calibrated { get; set; }

        public bool CorrectedPosition_m(double angle, out double X, out double Y)
        {
            if (MainForm.Setting.Placement_OmitNozzleCalibration)
            {
                X = 0.0;
                Y = 0.0;
                return true;
            };

            if (!Calibrated)
            {
                DialogResult dialogResult = MainForm.ShowMessageBox(
                    "Nozzle not calibrated. Calibrate now?",
                    "Nozzle not calibrated", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    X = 0.0;
                    Y = 0.0;
                    return false;
                };
                double CurrX = Cnc.CurrentX;
                double CurrY = Cnc.CurrentY;
                double CurrA = Cnc.CurrentA;
                if(!MainForm.CalibrateNozzle_m())
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
                "Nozzle Calibration value read: value not found",
                "Sloppy programmer error",
                MessageBoxButtons.OK);
            X = 0;
            Y = 0;
			return false;
        }


        public bool Calibrate()
        {
            CalibrationPoints.Clear();   // Presumably calibration is void no matter if we succeed here
            Calibrated = false;
            if (!Cam.IsRunning())
            {
                MainForm.ShowMessageBox(
                    "Attempt to calibrate Nozzle, camera is not running.",
                    "Camera not running",
                    MessageBoxButtons.OK);
                return false;
            }

            double X = 0;
            double Y = 0;
            double Maxdistance = MainForm.Setting.Nozzles_CalibrationDistance / MainForm.Setting.UpCam_XmmPerPixel;
            double radius = 0;
            int res = 0;
            // I goes in .1 of degrees. Makes sense to have the increase so, that multiplies of 45 are hit
            for (int i = 0; i <= 3600; i = i + 225)
            {
                NozzlePoint Point = new NozzlePoint();
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
                            "Nozzle calibration: Can't see Nozzle",
                            "No Circle found",
                            MessageBoxButtons.OK);
                        return false;
                    }
                }
                if (res == 0)
                {
                    MainForm.ShowMessageBox(
                        "Nozzle Calibration: Can't find Nozzle",
                        "No Circle found",
                        MessageBoxButtons.OK);
                    return false;
                }
                Point.X = X * MainForm.Setting.UpCam_XmmPerPixel;
                Point.Y = Y * MainForm.Setting.UpCam_YmmPerPixel;
                // MainForm.DisplayText("A: " + Point.Angle.ToString("0.000") + ", X: " + Point.X.ToString("0.000") + ", Y: " + Point.Y.ToString("0.000"));
                CalibrationPoints.Add(Point);
            }
            Calibrated = true;
            if (MainForm.Setting.Nozzles_Enabled)
            {
                Store(MainForm.Setting.Nozzles_current);
            };

            return true;
        }

        public bool Move_m(double X, double Y, double A)
        {
            double dX;
            double dY;
			MainForm.DisplayText("Nozzle.Move_m(): X= " + X.ToString() + ", Y= " + Y.ToString() + ", A= " + A.ToString());
			if (!CorrectedPosition_m(A, out dX, out dY))
			{
				return false;
			};
            double Xoff = MainForm.Setting.DownCam_NozzleOffsetX;
            double Yoff = MainForm.Setting.DownCam_NozzleOffsetY;
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

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

namespace LitePlacer
{
#pragma warning disable CA1031 // Do not catch general exception types (see MainForm.cs beginning)

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

        public List<List<NozzlePoint>> CalibrationData;  // calibration data for all nozzles:
                // CalibrationPointsArr[nozzlenumber] is a list of the calibration points for this nozzle
        public List<bool> Calibrated;  // if this particular nozzle is calibrated

#pragma warning disable CA2235 // Mark all non-serializable fields
        private Camera Cam;
        private CNC Cnc;
#pragma warning restore CA2235 // Mark all non-serializable fields
        private static FormMain MainForm;

        public NozzleClass(Camera MyCam, CNC MyCnc, FormMain MainF)
        {
            MainForm = MainF;
            Cam = MyCam;
            Cnc = MyCnc;
            CalibrationData = new List<List<NozzlePoint>>();
            Calibrated = new List<bool>();
        }

        // =================================================================================
        // Add remove
        // =================================================================================

        public void Add()
        {
            List<NozzlePoint> L = new List<NozzlePoint>();
            CalibrationData.Add(L);
            Calibrated.Add(false);
        }

        public void Remove()
        {
            CalibrationData.RemoveAt(CalibrationData.Count - 1);
            Calibrated.RemoveAt(Calibrated.Count - 1);
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
                formatter.Serialize(stream, CalibrationData);
                MainForm.DisplayText("Saving nozzle calibration validity data");
                formatter.Serialize(stream, Calibrated);
                stream.Flush();
                stream.Close();
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (System.Exception excep)
            {
                MainForm.DisplayText("Saving nozzle calibration data failed: " + excep.Message);
                return false;
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        public void LoadCalibration(string filename)
        {
            if (File.Exists(filename))
            {
                Stream stream = File.Open(filename, FileMode.Open);
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MainForm.DisplayText("Loading nozzle calibration data");
                    CalibrationData = (List<List<NozzlePoint>>)formatter.Deserialize(stream);
                    MainForm.DisplayText("Loading nozzle calibration validity data");
                    Calibrated = (List<bool>)formatter.Deserialize(stream);
                    stream.Flush();
                    stream.Close();
                }
                catch (Exception)
                {
                    stream.Close();
                    MainForm.DisplayText("Loading nozzle calibration data failed");
                    for (int i = 0; i < Calibrated.Count; i++)
                    {
                        Calibrated[i] = false;
                    }
                }
            }
            else
            {
                MainForm.DisplayText("Nozzle calibration data file not found");
                for (int i = 0; i < Calibrated.Count; i++)
                {
                    Calibrated[i] = false;
                }
            }
        }


        // =================================================================================

        public bool GetPositionCorrection_m(double angle, out double X, out double Y)
        {
            if (MainForm.Setting.Placement_OmitNozzleCalibration)
            {
                X = 0.0;
                Y = 0.0;
                return true;
            };

            if (!Calibrated[MainForm.Setting.Nozzles_current])
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
                // Do calibration: Save current position, ..
                double CurrX = Cnc.CurrentX;
                double CurrY = Cnc.CurrentY;
                double CurrA = Cnc.CurrentA;
                // calibrate, ..
                if(!MainForm.CalibrateNozzle_m())
                {
                    X = 0;
                    Y = 0;
                    return false;
                }
                // and return to the previous position
                if (!MainForm.CNC_XYA_m(CurrX, CurrY, CurrA))
                {
                    X = 0;
                    Y = 0;
                    return false;
                }
            };

            // move angle to 0..360
            while (angle < 0)
            {
                angle = angle + 360.0;
            };
            while (angle > 360.0)
            {
                angle = angle - 360.0;
            }
            // If angle is 360, return the value for 0
            if (angle > 359.98)
            {
                X = CalibrationData[MainForm.Setting.Nozzles_current][0].X;
                Y = CalibrationData[MainForm.Setting.Nozzles_current][0].Y;
                return true;
            };

            List<NozzlePoint> Points = CalibrationData[MainForm.Setting.Nozzles_current];
            for (int i = 0; i < Points.Count; i++)
            {
                if (Math.Abs(angle - Points[i].Angle) < 1.0)
                {
                    // we had a calibration value for exact angle asked
                    X = Points[i].X;
                    Y = -Points[i].Y;
					return true;
                }
                if ((angle > Points[i].Angle)
                    &&
                    (angle < Points[i + 1].Angle)
                    &&
                    (Math.Abs(angle - Points[i + 1].Angle) > 1.0))
                {
                    // we are between points, and next point is not what was asked either: 
                    // linear interpolation:
                    double fract = (angle - Points[i+1].Angle) / (Points[i+1].Angle - Points[i].Angle);
                    X = Points[i].X + fract * (Points[i + 1].X - Points[i].X);
                    Y = Points[i].Y + fract * (Points[i + 1].Y - Points[i].Y);
                    Y = -Y;
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
            if (!Cam.IsRunning())
            {
                MainForm.DisplayText("Attempt to calibrate Nozzle, camera is not running. \n\r"
                    + "Using old data, if it exsists.", System.Drawing.KnownColor.DarkRed, true);
                return false;
            }

            Calibrated[MainForm.Setting.Nozzles_current] = false;
            CalibrationData[MainForm.Setting.Nozzles_current].Clear();
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
                    if (Cam.Measure(out Point.X, out Point.Y, out int err, true))
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
                MainForm.DisplayText("A: " + Point.Angle.ToString("0.000") + ", X: " + Point.X.ToString("0.000") + ", Y: " + Point.Y.ToString("0.000"));
                CalibrationData[MainForm.Setting.Nozzles_current].Add(Point);
            }
            Calibrated[MainForm.Setting.Nozzles_current] = true;
            return true;
        }


        // =================================================================================
        // Nozzle.Move(): Takes machine to position, where nozzle, whewn lowered down, eds up in the coordinates specified
        // Correction is applied, if enabled.

        public bool Move_m(double X, double Y, double A)
        {
            double dX;
            double dY;
			MainForm.DisplayText("Nozzle.Move_m(): X= " + X.ToString(CultureInfo.InvariantCulture)
                + ", Y= " + Y.ToString(CultureInfo.InvariantCulture) + ", A= " + A.ToString(CultureInfo.InvariantCulture));
			if (!GetPositionCorrection_m(A, out dX, out dY))
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

        private bool CNC_XYA(double X, double Y, double A)
        {
			return MainForm.CNC_XYA_m(X, Y, A);
        }

    }
}

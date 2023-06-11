using System;
using System.Threading;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using AForge.Math.Geometry;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace LitePlacer
{
    public class Marlinclass
    {
        FormMain MainForm;
        CNC Cnc;
        SerialComm Com;

        public Marlinclass(FormMain MainF, CNC C, SerialComm ser)
        {
            MainForm = MainF;
            Cnc = C;
            Com = ser;
        }


        public int RegularMoveTimeout { get; set; } // in ms

        // =================================================================================
        #region Communications

        public bool JustConnected()
        {
            if (!MainForm.SetMarlinXAxisParameters()) return false;
            if (!MainForm.SetMarlinYAxisParameters()) return false;
            if (!MainForm.SetMarlinZAxisParameters()) return false;
            if (!MainForm.SetMarlinAmotorParameters()) return false;
            if (!SetMachineSizeX()) return false;
            if (!SetMachineSizeY()) return false;
            return true;
        }


        // ===================================================================
        // Read & write
        // ===================================================================

        private Queue<string> ReceivedLines=new Queue<string>();

        public bool ExpectingResponse = false; // to make note about unexpected messages
        bool ResponseWanted = false;
        private bool WriteBusy = false;
        public bool LogCommunication = true;   // real-time monitoring would flood the log window

        // ===================================================================
        // Write_m
        // Normal write, waits until "ok" response is received

        public bool Write_m(string cmd, int Timeout = 500)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + cmd + " discarded, com not open");
                ReceivedLines.Clear();
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + cmd + " discarded, error state on");
                ReceivedLines.Clear();
                return false;
            }

            Timeout = Timeout / 2;
            int i = 0;
            WriteBusy = true;
            if (LogCommunication)
            {
                MainForm.DisplayText("==> " + cmd, KnownColor.Blue);
            }
            if (!Com.Write(cmd))
            {
                MainForm.ShowMessageBox(
                    "Marlin.Write_m: Write failed on cmd " + cmd,
                    "Write failed",
                    MessageBoxButtons.OK);
                ReceivedLines.Clear();
                return false;
            }
            while (WriteBusy)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    MainForm.ShowMessageBox(
                        "Marlin.Write_m: Timeout on command " + cmd,
                        "Timeout",
                        MessageBoxButtons.OK);
                    ReceivedLines.Clear();
                    return false;
                }
            }
            if (!ResponseWanted)
            {
                ReceivedLines.Clear();
            }
            return true;
        }


        // ===================================================================
        // GetResponse
        // Writes a command, returns a response. Failed write returns empty response.
        // Response is everything, including "ok"
        // Line separator is \n
        public List<string> GetResponse_m(string cmd, out bool success, int Timeout = 500)
        {
            List<string> response = new List<string>();

            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + cmd + " discarded, com not open");
                ReceivedLines.Clear();
                success = false;
                return response;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + cmd + " discarded, error state on");
                ReceivedLines.Clear();
                success = false;
                return response;
            }

            ExpectingResponse = true;
            ResponseWanted = true;
            success = Write_m(cmd, Timeout);
            ResponseWanted = false;
            ExpectingResponse = false;
            lock (ReceivedLines)
            {
                response=ReceivedLines.ToList();
                ReceivedLines.Clear();
            }
            return response;
        }


        // ===================================================================

        // Position info is stored here on start of the move, and PositionUpdateRequired
        // is set. If set, "ok" message updates the UI.
        public void LineReceived(string line)
        {
            // This is called from Cnc.LineReceived (called from SerialComm dataReceived),
            // and runs in a separate thread than UI            
            if (line == "ok")
            {
                if (LogCommunication)
                {
                    MainForm.DisplayText("<== " + line);
                }
                WriteBusy = false;
                return;
            }
            lock (ReceivedLines)
            {
                ReceivedLines.Enqueue(line);
            }
            if (LogCommunication)
            {
                if (!ExpectingResponse)
                {
                    MainForm.DisplayText("<=! " + line, KnownColor.DarkRed);
                    // MainForm.DisplayText("*** Marlin() - unsoliticed message", KnownColor.DarkRed, true);
                }
                else
                {
                    MainForm.DisplayText("<== " + line);
                }
            }

        }

        #endregion Communications

        // =================================================================================
        // Movement, position:
        #region Movement

        private bool SetXposition(string pos)
        {
            double val;
            if (!double.TryParse(pos.Replace(',', '.'), out val))
            {
                MainForm.ShowMessageBox(
                    "Marlin.SetXposition() called with bad value " + pos,
                    "BUG",
                    MessageBoxButtons.OK);
                return false;
            }
            Cnc.SetCurrentX(val);
            if (!Write_m("G92 X" + pos))
            {
                MainForm.ShowMessageBox(
                    "Marlin G92 X" + pos + " failed",
                    "comm err?",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool SetYposition(string pos)
        {
            double val;
            if (!double.TryParse(pos.Replace(',', '.'), out val))
            {
                MainForm.ShowMessageBox(
                    "Marlin.SetYposition() called with bad value " + pos,
                    "BUG",
                    MessageBoxButtons.OK);
                return false;
            }
            Cnc.SetCurrentY(val);
            if (!Write_m("G92 Y" + pos))
            {
                MainForm.ShowMessageBox(
                    "Marlin G92 Y" + pos + " failed",
                    "comm err?",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool SetZposition(string pos)
        {
            double val;
            if (!double.TryParse(pos.Replace(',', '.'), out val))
            {
                MainForm.ShowMessageBox(
                    "Marlin.SetZposition() called with bad value " + pos,
                    "BUG",
                    MessageBoxButtons.OK);
                return false;
            }
            Cnc.SetCurrentZ(val);
            if (!Write_m("G92 Z" + pos))
            {
                MainForm.ShowMessageBox(
                    "Marlin G92 Z" + pos + " failed",
                    "comm err?",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool SetAposition(string pos)
        {
            double val;
            if (!double.TryParse(pos.Replace(',', '.'), out val))
            {
                MainForm.ShowMessageBox(
                    "Marlin.SetAposition() called with bad value " + pos,
                    "BUG",
                    MessageBoxButtons.OK);
                return false;
            }
            Cnc.SetCurrentA(val);
            if (!Write_m("G92 A" + pos))
            {
                MainForm.ShowMessageBox(
                    "Marlin G92 A" + pos + " failed",
                    "comm err?",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }


        public void SetPosition(string Xstr, string Ystr, string Zstr, string Astr)
        {
            if (Xstr != "")
            {
                SetXposition(Xstr);
            };
            if (Ystr != "")
            {
                SetYposition(Ystr);
            };
            if (Zstr != "")
            {
                SetZposition(Zstr);
            };
            if (Astr != "")
            {
                SetAposition(Astr);
            };
        }



        public void CancelJog()
        {
            MainForm.ShowMessageBox("Unimplemented Marlin function CancelJog", "Unimplemented function", MessageBoxButtons.OK);
        }



        public void Jog(string Speed, string X, string Y, string Z, string A)
        {
            MainForm.ShowMessageBox("Unimplemented Marlin function Jog", "Unimplemented function", MessageBoxButtons.OK);
        }

        // =================================================================================
        // homing

        private bool HomingTimeout_m(out int TimeOut, string axis)
        {
            double Speed;
            double size;
            TimeOut = 0;
            switch (axis)
            {
                case "X":
                    Speed = MainForm.Setting.Marlin_XHomingSpeed;
                    size = MainForm.Setting.General_MachineSizeX;
                    break;

                case "Y":
                    Speed = MainForm.Setting.Marlin_YHomingSpeed;
                    size = MainForm.Setting.General_MachineSizeY;
                    break;

                case "Z":
                    Speed = MainForm.Setting.Marlin_ZHomingSpeed;
                    size = 100.0;
                    break;

                default:
                    return false;
            }

            Speed = Speed / 60;  // Was mm/min, now in mm / second
            Double MaxTime = (size / Speed) * 1.2 + 4;
            // in seconds for the machine size and some (1.2 to allow acceleration, + 4 for the operarations at end stop
            TimeOut = (int)MaxTime * 1000;  // to ms
            return true;
        }


        public bool Home_m(string axis)
        {
            double HomingSpeed = 0;
            double HomingBackoff = 0;
            string BackoffSpeedStr = MainForm.Setting.CNC_SmallMovementSpeed.ToString();
            int timeout;

            switch (axis)
            {
                case "X":
                    HomingSpeed = MainForm.Setting.Marlin_XHomingSpeed;
                    HomingBackoff = MainForm.Setting.Marlin_XHomingBackoff;
                    MainForm.Update_Xposition();
                    break;
                case "Y":
                    HomingSpeed = MainForm.Setting.Marlin_YHomingSpeed;
                    HomingBackoff = MainForm.Setting.Marlin_YHomingBackoff;
                    MainForm.Update_Yposition();
                    break;
                case "Z":
                    HomingSpeed = MainForm.Setting.Marlin_ZHomingSpeed;
                    HomingBackoff = MainForm.Setting.Marlin_ZHomingBackoff;
                    MainForm.Update_Zposition();
                    break;
                default:
                    MainForm.ShowMessageBox("Unimplemented Marlin function Home_m: axis " + axis,
                        "Unimplemented function", MessageBoxButtons.OK);
                    break;
            }
            if (!HomingTimeout_m(out timeout, axis))
            {
                return false;
            }


            string cmd = "G1 H1 " + axis + "-999999 F" + HomingSpeed.ToString();
            if (!Write_m(cmd, timeout))
            {
                MainForm.ShowMessageBox(
                    "Homing operation mechanical step failed, CNC issue",
                    "Homing failed",
                    MessageBoxButtons.OK);
                return false;
            }
            cmd = "G1 " + axis + HomingBackoff.ToString() + " F" + BackoffSpeedStr;
            if (!Write_m(cmd, RegularMoveTimeout))
            {
                MainForm.ShowMessageBox(
                    "Homing operation mechanical step failed, CNC issue",
                    "Homing failed",
                    MessageBoxButtons.OK);
                return false;
            }
            bool res = true;
            switch (axis)
            {
                case "X":
                    res= SetXposition("0.0");
                    break;
                case "Y":
                    res = SetYposition("0.0");
                    break;
                case "Z":
                    res = SetZposition("0.0");
                    break;
                default:
                    MainForm.ShowMessageBox("Unimplemented Marlin function Home_m: axis " + axis,
                        "Unimplemented function", MessageBoxButtons.OK);
                    break;
            }
            if (!res)
            {
                MainForm.DisplayText("*** Homing operation post moves position set failed", KnownColor.DarkRed, true);
                return false;
            }

            MainForm.DisplayText("Homing " + axis + " done.");
            return true;
        }


        public bool XYA(double X, double Y, double A, double speed, string MoveType)
        {
            string command;
            if (MoveType == "G1")
            {
                command = "G1 F" + speed.ToString() +
                    " X" + X.ToString("0.000", CultureInfo.InvariantCulture) +
                    " Y" + Y.ToString("0.000", CultureInfo.InvariantCulture) +
                    " A" + A.ToString("0.000", CultureInfo.InvariantCulture);
            }
            else
            {
                command = "G0 " +
                    " X" + X.ToString("0.000", CultureInfo.InvariantCulture) +
                    " Y" + Y.ToString("0.000", CultureInfo.InvariantCulture) +
                    " A" + A.ToString("0.000", CultureInfo.InvariantCulture);
            }
            if (!Write_m(command, RegularMoveTimeout))
            {
                return false;
            }
            Cnc.SetCurrentX(X);
            Cnc.SetCurrentY(Y);
            Cnc.SetCurrentA(A);
            return true;
        }


        public bool A(double A, double speed, string MoveType)
        {
            string command;
            if (MoveType == "G1")
            {
                command = "G1 F" + speed.ToString() +
                    " A" + A.ToString("0.000", CultureInfo.InvariantCulture);
            }
            else
            {
                command = "G0 " +
                    " A" + A.ToString("0.000", CultureInfo.InvariantCulture);
            }
            if (!Write_m(command, RegularMoveTimeout))
            {
                return false;
            }
            Cnc.SetCurrentA(A);
            return true;
        }


        public bool Z(double Z, double speed, string MoveType)
        {
            string command;
            if (MoveType == "G1")
            {
                command = "G1 F" + speed.ToString() +
                    " Z" + Z.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                command = "G0 " +
                    " Z" + Z.ToString(CultureInfo.InvariantCulture);
            }
            if (!Write_m(command, RegularMoveTimeout))
            {
                return false;
            }
            Cnc.SetCurrentZ(Z);
            return true;
        }


        #endregion Movement


        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public bool SetMachineSizeX()
        {
            int MaxSixe = (int)Math.Round(MainForm.Setting.General_MachineSizeX) + 3;
            int MinSize = (int)Math.Round(MainForm.Setting.General_NegativeX);
            return Write_m("M208 X-" + MinSize.ToString() + ":" + MaxSixe.ToString());
        }

        public bool SetMachineSizeY()
        {
            int MaxSixe = (int)Math.Round(MainForm.Setting.General_MachineSizeY) + 3;
            int MinSize = (int)Math.Round(MainForm.Setting.General_NegativeY);
            return Write_m("M208 Y-" + MinSize.ToString() + ":" + MaxSixe.ToString());
        }


        public void DisableZswitches()
        {
            MainForm.ShowMessageBox("Unimplemented Marlin function DisableZswitches", "Unimplemented function", MessageBoxButtons.OK);
        }



        public void EnableZswitches()
        {
            MainForm.ShowMessageBox("Unimplemented Marlin function EnableZswitches", "Unimplemented function", MessageBoxButtons.OK);
        }

        public bool GetEndStopStatuses(out List<int> Statuses, bool show)
        {
            List<int> stats = new List<int> { 0, -1, 0, -1, 0, -1, 0, -1 };
            // Xmin, Xmax, Ymin, Ymax, Zmin, Zmax. 1= not active, 0= active, -1= not present
            // FOR NOW: only min switches
            List<string> response = new List<string>(GetResponse_m("m119", out bool success));
            if (!success)
            {
                Statuses = stats;
                return false;
            }
            if (response.Count!=4)
            {
                MainForm.DisplayText("Marlin.GetEndStopStatuses: Unexpected response:", KnownColor.DarkRed, true);
                foreach (string s in response)
                {
                    MainForm.DisplayText(s, KnownColor.DarkRed, true);
                }
                Statuses = stats;
                return false;
            }
            if (response[1].Contains("open")) stats[0] = 1;
            if (response[2].Contains("open")) stats[2] = 1;
            if (response[3].Contains("open")) stats[4] = 1;
            Statuses = stats;
            return true;
        }

        public bool Nozzle_ProbeDown(double backoff)
        {
            MainForm.ShowMessageBox("Unimplemented Marlin function Nozzle_ProbeDown", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }



        public void MotorPowerOn()
        {
            MainForm.DisplayText("MotorPowerOn(), Marlin");
            Write_m("M17");
        }



        public void MotorPowerOff()
        {
            MainForm.DisplayText("MotorPowerOff(), Marlin");
            Write_m("M18");
        }



        public void VacuumOn()
        {
            MainForm.DisplayText("VacuumOn(), Marlin");
            Write_m("M42 P7 S1");
        }



        public void VacuumOff()
        {
            MainForm.DisplayText("VacuumOff(), Marlin");
            Write_m("M42 P8 S0");
        }


        public void PumpOn()
        {
            MainForm.DisplayText("PumpOn(), Marlin");
            Write_m("M42 P7 S1");
        }



        public void PumpOff()
        {
            MainForm.DisplayText("PumpOff(), Marlin");
            Write_m("M42 P7 S0");
        }

        #endregion Features


    }
}

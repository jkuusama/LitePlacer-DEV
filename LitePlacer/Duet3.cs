using System;
using System.Threading;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LitePlacer
{
    public class Duet3class
    {
        FormMain MainForm;
        CNC Cnc;
        SerialComm Com;

        public Duet3class(FormMain MainF, CNC C, SerialComm ser)
        {
            MainForm = MainF;
            Cnc = C;
            Com = ser;
        }


        public int RegularMoveTimeout { get; set; } // in ms

        // =================================================================================
        #region Communications

        public bool CheckIdentity()
        {
            string resp = GetResponse_m("M115", 200, false);
            if (resp.Contains("Duet 3"))
            {
                MainForm.DisplayText("Duet 3 board found.");
                return true;
            }
            return false;
        }


        public bool JustConnected()
        {
            if (!MainForm.SetDuet3XmotorParameters()) return false;
            if (!MainForm.SetDuet3YmotorParameters()) return false;
            if (!MainForm.SetDuet3ZmotorParameters()) return false;
            if (!MainForm.SetDuet3AmotorParameters()) return false;
            return true;
        }


        // ===================================================================
        // Read & write
        // ===================================================================

        private bool LineAvailable = false;
        private string ReceivedLine = "";
        private bool WriteBusy = false;

        // so that we don't need to write lock... so many times
        private void ClearReceivedLine()
        {
            lock (ReceivedLine)
            {
                ReceivedLine = "";
            }
        }

        // ===================================================================
        // Write_m
        // Normal write, waits until "ok" response is received

        public bool Write_m(string cmd, int Timeout = 250)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + cmd + " discarded, com not open");
                ClearReceivedLine();
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + cmd + " discarded, error state on");
                ClearReceivedLine();
                return false;
            }

            Timeout = Timeout / 2;
            int i = 0;
            WriteBusy = true;
            bool WriteOk = Com.Write(cmd);
            while (WriteBusy)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    MainForm.ShowMessageBox(
                        "Duet3.Write_m: Timeout on command " + cmd,
                        "Timeout",
                        MessageBoxButtons.OK);
                    ClearReceivedLine();
                    return false;
                }
            }
            return WriteOk;
        }


        // ===================================================================
        // GetResponse
        // Writes a command, returns a response. Failed write returns empty response.
        public string GetResponse_m(string cmd, int Timeout = 250, bool report = true)
        {
            string line;

            if (!Com.IsOpen)
            {
                if (report)
                {
                    MainForm.DisplayText("###" + cmd + " discarded, com not open");
                }
                ClearReceivedLine();
                return "";
            }
            if (Cnc.ErrorState)
            {
                if (report)
                {
                    MainForm.DisplayText("###" + cmd + " discarded, error state on");
                }
                ClearReceivedLine();
                return "";
            }

            Timeout = Timeout / 2;
            int i = 0;
            LineAvailable = false;
            Com.Write(cmd);
            while (!LineAvailable)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    if (report)
                    {
                        MainForm.ShowMessageBox(
                            "Duet3.Write_m: Timeout on command " + cmd,
                            "Timeout",
                            MessageBoxButtons.OK);
                    }
                    ClearReceivedLine();
                    return "";
                }
            }
            lock (ReceivedLine)
            {
                line = ReceivedLine;
            }
            return line;
        }


        // ===================================================================

        public void LineReceived(string line)
        {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            MainForm.DisplayText("<== " + line);
            if (line == "ok\n")
            {
                WriteBusy = false;
                return;
            }
            lock (ReceivedLine)
            {
                ReceivedLine = line;
                LineAvailable = true;
            }
        }


        // ===================================================================
        // For operations that don't give response
        // Caller does waiting, if needed.
        public bool RawWrite(string command)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + command + " discarded, com not open");
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + command + " discarded, error state on");
                return false;
            }
            return Com.Write(command);
        }


        #endregion Communications

            // =================================================================================
            // Movement, position:
            #region Movement

        public void SetPosition(string Xstr, string Ystr, string Zstr, string Astr)
        {
            string Pos = "G92";
            if (Xstr != "")
            {
                Pos = Pos + " X" + Xstr;
            };
            if (Ystr != "")
            {
                Pos = Pos + " Y" + Ystr;
            };
            if (Zstr != "")
            {
                Pos = Pos + " Z" + Zstr;
            };
            if (Astr != "")
            {
                Pos = Pos + " A" + Astr;
            };
            Write_m(Pos);
        }



        public void CancelJog()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function CancelJog", "Unimplemented function", MessageBoxButtons.OK);
        }



        public void Jog(string Speed, string X, string Y, string Z, string A)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function Jog", "Unimplemented function", MessageBoxButtons.OK);
        }


        public bool Home_m(string axis)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function Home_m", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        public bool XYA(double X, double Y, double A, double speed, string MoveType)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function XYA", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        public bool A(double A, double speed, string MoveType)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function A", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        public bool Z(double Z, double speed, string MoveType)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function Z", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        #endregion Movement


        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public bool SetMachineSizeX(int Xsize)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function SetMachineSizeX", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }

        public bool SetMachineSizeY(int Xsize)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function SetMachineSizeY", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        public void DisableZswitches()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function DisableZswitches", "Unimplemented function", MessageBoxButtons.OK);
        }



        public void EnableZswitches()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function EnableZswitches", "Unimplemented function", MessageBoxButtons.OK);
        }


        public bool SetZ_SwitchClearance(double val)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function SetZ_SwitchClearance", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }




        public bool GetZ_LatchBackoff(out double val) 
        {
            val = 0.0;
            MainForm.ShowMessageBox("Unimplemented Duet3 function GetZ_LatchBackoff", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }

        public bool SetZ_LatchBackoff(double val)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function SetZ_LatchBackoff", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }



        public bool GetZ_ZeroBackoff(out double val) 
        {
            val = 0.0;
            MainForm.ShowMessageBox("Unimplemented Duet3 function GetZ_ZeroBackoff", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }

        public bool SetZ_ZeroBackoff(double val)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function SetZ_ZeroBackoff", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }


        public void ProbingMode(bool set)
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function ProbingMode", "Unimplemented function", MessageBoxButtons.OK);
        }


        public bool Nozzle_ProbeDown()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function Nozzle_ProbeDown", "Unimplemented function", MessageBoxButtons.OK);
            return false;
        }



        public void MotorPowerOn()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function MotorPowerOn", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("MotorPowerOn(), Duet3");
        }



        public void MotorPowerOff()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function MotorPowerOff", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("MotorPowerOff(), Duet3");
        }



        public void VacuumOn()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function VacuumOn", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("VacuumOn(), Duet3");
        }



        public void VacuumOff()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function VacuumOff", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("VacuumOff(), Duet3");
        }


        public void PumpOn()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function PumpOn", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("PumpOn(), Duet3");
        }



        public void PumpOff()
        {
            MainForm.ShowMessageBox("Unimplemented Duet3 function PumpOff", "Unimplemented function", MessageBoxButtons.OK);
            MainForm.DisplayText("PumpOff(), Duet3");
        }

        #endregion Features


    }
}

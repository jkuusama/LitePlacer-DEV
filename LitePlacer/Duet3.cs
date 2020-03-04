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
            Write_m("M115");
            string resp = ReadLine();
            if (resp.Contains("Duet 3"))
            {
                MainForm.DisplayText("Duet 3 board found.");
                return true;
            }
            return false;
        }


        public bool JustConnected()
        {
            return true;
        }


        // ===================================================================
        // Read & write
        // ===================================================================

        private bool LineAvailable = false;
        private string ReceivedLine = "";

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
        // Normal write, waits until there is response available

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

            LineAvailable = false;
            Timeout = Timeout / 2;
            int i = 0;
            bool WriteOk = Com.Write(cmd);
            while (!LineAvailable)
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
        // ReadLine
        // Normal read, assumes there is response available
        public string ReadLine()
        {
            string line;
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
        }



        public void Jog(string Speed, string X, string Y, string Z, string A)
        {

        }


        public bool Home_m(string axis)
        {
            return false;
        }


        public bool XYA(double X, double Y, double A, double speed, string MoveType)
        {
            return false;
        }


        public bool A(double A, double speed, string MoveType)
        {
            return false;
        }


        public bool Z(double Z, double speed, string MoveType)
        {
            return false;
        }


        #endregion Movement




        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public bool SetMachineSizeX(int Xsize)
        {
            return false;
        }

        public bool SetMachineSizeY(int Xsize)
        {
            return false;
        }



        public void DisableZswitches()
        {

        }



        public void EnableZswitches()
        {

        }




        public void ProbingMode(bool set)
        {

        }


        public bool Nozzle_ProbeDown()
        {

            return false;
        }



        public void MotorPowerOn()
        {
            MainForm.DisplayText("MotorPowerOn(), Duet3");
        }



        public void MotorPowerOff()
        {
            MainForm.DisplayText("MotorPowerOff(), Duet3");
        }



        public void VacuumOn()
        {
            MainForm.DisplayText("VacuumOn(), Duet3");
            string command = "";
            if (MainForm.Setting.General_VacuumOutputInverted)
            {
                command = "";
            }
            if (!Cnc.VacuumIsOn)
            {
                if (RawWrite(command))
                {
                    Cnc.VacuumIsOn = true;
                    Thread.Sleep(MainForm.Setting.General_PickupVacuumTime);
                }
            }
            MainForm.Vacuum_checkBox.Checked = Cnc.VacuumIsOn;
        }



        public void VacuumOff()
        {
            MainForm.DisplayText("VacuumOff(), Duet3");
            string command = "";
            if (MainForm.Setting.General_VacuumOutputInverted)
            {
                command = "";
            }
            if (Cnc.VacuumIsOn)
            {
                if (RawWrite(command))
                {
                    Cnc.VacuumIsOn = false;
                    Thread.Sleep(MainForm.Setting.General_PickupReleaseTime);
                }
            }
            MainForm.Vacuum_checkBox.Checked = Cnc.VacuumIsOn;
        }


        public void PumpOn()
        {
            MainForm.DisplayText("PumpOn(), Duet3");
            string command = "";
            if (MainForm.Setting.General_PumpOutputInverted)
            {
                command = "";
            }
            MainForm.DisplayText("PumpOn(), TinyG");
            if (!Cnc.PumpIsOn)
            {
                if (RawWrite(command))
                {
                    Thread.Sleep(500);  // this much to develop vacuum
                    Cnc.PumpIsOn = true;
                }
            }
            MainForm.Pump_checkBox.Checked = Cnc.PumpIsOn;
        }



        public void PumpOff()
        {
            MainForm.DisplayText("PumpOff(), Duet3");
            string command = "";
            if (MainForm.Setting.General_PumpOutputInverted)
            {
                command = "";
            }
            if (Cnc.PumpIsOn)
            {
                if (RawWrite(command))
                {
                    Thread.Sleep(50);
                    Cnc.PumpIsOn = false;
                }
            }
            MainForm.Pump_checkBox.Checked = Cnc.PumpIsOn;
        }

        #endregion Features


    }
}

using System;
using System.Threading;
using System.Windows.Forms;

using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LitePlacer
{
    public class Duet3class
    {
        private FormMain MainForm;
        private CNC Cnc;
        private SerialComm Com;

        public Duet3class(FormMain MainF, CNC C)
        {
            MainForm = MainF;
            Cnc = C;
            Com = new SerialComm(C, MainF);
        }

        // =================================================================================
        // Communications to hardware, status of the link
        #region Communications

        public bool Connected { get; set; }
        public bool ErrorState { get; set; }



        public bool Connect(string name)
        {
            if (Com.IsOpen)
            {
                MainForm.DisplayText("Already connected to serial port " + name + ": already open");
                Cnc.Connected = true;
            }
            else
            {
                Com.Open(name);
                if (!Com.IsOpen)
                {
                    MainForm.DisplayText("Connecting to serial port " + name + " failed.");
                    RaiseError();
                    return false;
                }
                else
                {
                    MainForm.DisplayText("Connected to serial port " + name);
                }
            }
            // At this point, we are either connected to a board, or returned
            // Is it a right board?
            if (!CheckIdentity())
            {
                RaiseError();
                return false;
            }
            Cnc.Connected = true;
            Cnc.ErrorState = false;
            MainForm.UpdateCncConnectionStatus();
            return true;
        }

        // =========================
        bool CheckIdentity()
        {
            MainForm.DisplayText("Finding board type:");
            string resp;
            LineWanted = true;
            Write_m("M115");
            if (!ReadLine(out resp, 250))
            {
                MainForm.DisplayText("No success.");
                return false;
            }
            if (resp.Contains("Duet 3"))
            {
                MainForm.DisplayText("Duet 3 board found.");
                return true;
            }
            return false;
        }


        public bool JustConnected()
        {
            return false;
        }



        public void Close()
        {
            Com.Close();
            Cnc.ErrorState = false;
            Cnc.Connected = false;
            Cnc.Homing = false;
            MainForm.UpdateCncConnectionStatus();
        }


        public bool Write_m(string command, int Timeout = 250)
        {
                return false;
        }



        // For operations that cause conflicts with event firings or don't give response
        // Caller does waiting, if needed.
        public bool RawWrite(string command)
        {

            return false;
        }


        // Write, that doesn't care what we think of the communication link status
        public void ForceWrite(string command)
        {
            Com.Write(command);
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



        #endregion
        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public void ProbingMode(bool set)
        {

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






        // ===============================
        public void RaiseError()
        {
            ErrorState = true;
        }



        // ===============================
        // Read single line:

        private bool LineWanted = false;
        private string LineOut;

        public bool ReadLine(out string line, int TimeOut)
        {
            LineWanted = true;
            TimeOut = TimeOut / 5;
            while (!LineWanted)
            {
                Thread.Sleep(5);
                Application.DoEvents();
                TimeOut--;
                if (TimeOut < 0)
                {
                    LineWanted = false;
                    line = "";
                    return false;
                }
            }
            line = LineOut;
            return true;
        }



        public void InterpretLine(string line)
        {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            MainForm.DisplayText("<== " + line);

            // In some cases, the caller wants to look at the line directly:
            if (LineWanted)
            {
                LineOut = line;
                LineWanted = false;
                return;
            }


        }
    }
}

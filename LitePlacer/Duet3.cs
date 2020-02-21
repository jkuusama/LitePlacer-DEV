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
            string resp;
            resp = ReadLineDirectly("M115");
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


        public string ReadLineDirectly(string command)
        {
            return "";
        }


        public bool Write_m(string command, int Timeout = 250)
        {
            MainForm.DisplayText("Duet3 write not implemented!***", KnownColor.DarkRed, true);
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

        // =================================================================================
        // Movement


        // ===============================
        // Read single line:

        private bool LineWanted = false;
        private string LineOut;

        public bool ReadLine(out string line, int TimeOut)
        {
            LineWanted = true;
            TimeOut = TimeOut / 5;
            while (LineWanted)
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

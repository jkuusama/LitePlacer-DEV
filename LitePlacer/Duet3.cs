using System.Threading;
using System.Windows.Forms;

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
            Com = new SerialComm(MainF, InterpretLine);
        }

        // =========================================================================================
        // home
        public bool Home_m(string axis)
        {
            return false;
        }

        // =========================================================================================      
        // set position

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
            Write(Pos);
        }

        // =========================================================================================
        // probing
        public void ProbingMode(bool set)
        {
        }

        public bool Nozzle_ProbeDown_m()
        {
            return false;
        }

        // =========================================================================================
        // motor power
        public void MotorPowerOn()
        {
            // Can't currently be done without moving the machine.
        }

        public void MotorPowerOff()
        {
            Write("M18");
        }

        // =========================================================================================
        // vacuum
        public void VacuumOn()
        {
        }

        public void VacuumOff()
        {
        }

        // =========================================================================================
        // Pump
        public void PumpOn()
        {
        }

        public void PumpOff()
        {
        }

        // =========================================================================================
        // jog
        public void CancelJog()
        {
        }

        public void Jog(string Speed, string X, string Y, string Z, string A)
        {

        }

        // =========================================================================================
        // status

        public bool Homing { get; set; }  // Homing is much slower than other operations, we need longer timeouts.

        void Error()
        {
            Cnc.ErrorState = true;
            Cnc.Connected = false;
            Homing = false;
            MainForm.UpdateCncConnectionStatus();
        }


        // =========================================================================================
        // Low level communications:

        // =========================
        // Open (but let's call it connect, as Open already has too many meanings)
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
                    Error();
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
                Error();
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
            Write("M115");
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
            MainForm.DisplayText("Unknown board/device.");
            return false;
        }

        // =========================
        public void Close()
        {
            Com.Close();
            Cnc.ErrorState = false;
            Cnc.Connected = false;
            MainForm.UpdateCncConnectionStatus();
        }

        // =========================
        // low level writes:
        bool Write(string command)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + command + " discarded, com not open");
                _readyEvent.Set();
                Cnc.Connected = false;
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + command + " discarded, error state on");
                _readyEvent.Set();
                return false;
            }
            _readyEvent.Reset();
            bool res = Com.Write(command);
            _readyEvent.Wait();
            if (!res)
            {
                Error();
            }
            return res;
        }

        public bool RawWrite(string command)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + command + " discarded, com not open");
                Cnc.Connected = false;
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + command + " discarded, error state on");
                return false;
            }
            bool res = Com.Write(command);
            if (!res)
            {
                Error();
            }
            return res;
        }

        public void ForceWrite(string command)
        {
            Com.Write(command);
        }

        // =========================================================================================


        // =========================
        public bool JustConnected()
        {
            return false;
        }

        // =========================================================================================

        // =================================================================================
        // Read:
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

            // .....
        }

    }
}
using System.Drawing;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;
using System;
using System.Web.Script.Serialization;

namespace LitePlacer
{
    public class TinyGclass
    {
        private FormMain MainForm;
        private CNC Cnc;
        private SerialComm Com;

        public TinyGclass(FormMain MainF, CNC C)
        {
            MainForm = MainF;
            Cnc = C;
            Com = new SerialComm(MainF, InterpretLine);
        }

        // =========================================================================================
        // home
        public bool Home_m(string axis)
        {
            int timeout;
            if (!HomingTimeout_m(out timeout, axis))
            {
                return false;
            }
            CNC_HomingTimeout = timeout;

            DisplayText("Homing axis " + axis + ", timeout value: " + CNC_HomingTimeout.ToString(CultureInfo.InvariantCulture));

            Cnc.Homing = true;
            if (!Write_m("{\"gc\":\"G28.2 " + axis + "0\"}"))
            {
                MainForm.ShowMessageBox(
                    "Homing operation mechanical step failed, CNC issue",
                    "Homing failed",
                    MessageBoxButtons.OK);
                Cnc.Homing = false;
                return false;
            }
            Cnc.Homing = false;
            MainForm.DisplayText("Homing " + axis + " done.");
            return true;
        }

        // =========================================================================================      
        // set position

        public void SetPosition(string Xstr, string Ystr, string Zstr, string Astr)
        {
            string Pos = "{\"gc\":\"G28.3";
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
            RawWrite(Pos + "\"}");
        }

        // =========================================================================================
        // probing
        public void ProbingMode(bool set)
        {
            int wait = 250;
            double b = MainForm.Setting.General_ZprobingHysteresis;
            string backoff = b.ToString("0.00", CultureInfo.InvariantCulture);

            if (set)
            {
                MainForm.DisplayText("Probing mode on, TinyG");
                MainForm.CNC_Write_m("{\"zsn\",0}");
                Thread.Sleep(wait);
                MainForm.CNC_Write_m("{\"zsx\",1}");
                Thread.Sleep(wait);
                MainForm.CNC_Write_m("{\"zzb\"," + backoff + "}");
                Thread.Sleep(wait);
            }
            else
            {
                MainForm.DisplayText("Probing mode off, TinyG");
                MainForm.CNC_Write_m("{\"zsn\",3}");
                Thread.Sleep(wait);
                MainForm.CNC_Write_m("{\"zsx\",2}");
                Thread.Sleep(wait);
                MainForm.CNC_Write_m("{\"zzb\",2}");
                Thread.Sleep(wait);
            }
        }

        public bool Nozzle_ProbeDown_m()
        {
            int timeout;
            if (!HomingTimeout_m(out timeout, "Z"))
            {
                return false;
            }
            CNC_HomingTimeout = timeout;
            DisplayText("Probing Z, timeout value: " + CNC_HomingTimeout.ToString(CultureInfo.InvariantCulture));

            Cnc.ProbingMode(true);
            Cnc.Homing = true;
            if (!CNC_Write_m("{\"gc\":\"G28.4 Z0\"}", 4000))
            {
                Cnc.Homing = false;
                Cnc.ProbingMode(false);
                return false;
            }
            Cnc.Homing = false;
            Cnc.ProbingMode(false);
            return true;
        }

        // =========================================================================================
        // motor power
        public void MotorPowerOff()
        {
            MainForm.DisplayText("MotorPowerOff(), TinyG");
            TimerDone = true;
            Write_m("{\"md\":\"\"}");
        }

        public void MotorPowerOn()
        {
            MainForm.DisplayText("MotorPowerOn(), TinyG");
            Write_m("{\"me\":\"\"}");
            MainForm.ResetMotorTimer();
        }

        // =========================================================================================
        // vacuum
        public void VacuumOn()
        {
            Write_m("{\"gc\":\"M08\"}");
        }

        public void VacuumOff()
        {
            Write_m("{\"gc\":\"M09\"}");
        }

        // =========================================================================================
        // Pump
        public void PumpOn()
        {
            Write_m("{\"gc\":\"M03\"}");
        }

        public void PumpOff()
        {
            Write_m("{\"gc\":\"M05\"}");
        }

        // =========================================================================================
        // jog
        public void CancelJog()
        {
            RawWrite("!%");
        }

        public void Jog(string Speed, string X, string Y, string Z, string A)
        {
            if (X != "" && Y != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " X" + X + " Y" + Y + "\"}");
            }
            else if (X != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " X" + "\"}");
            }
            else if (Y != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " Y" + "\"}");
            }
            else if (Z == "")
            {

            }
            else if ((X != "") && (Y != "") && (Z == "") && (A == ""))
            {

            }

        }
        // =========================================================================================
        // status

        public bool Homing { get; set; }  // Homing is much slower than other operations, we need longer timeouts.

        void Error()
        {
            Cnc.ErrorState = true;
            Cnc.Connected = false;
            Homing = false;
            _readyEvent.Set();
            MainForm.UpdateCncConnectionStatus();
        }

        // =========================================================================================
        // Low level communications

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
            if (resp.Contains("\"r\":{ },"))
            {
                MainForm.DisplayText("TinyG board found.");
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
            Homing = false;
            _readyEvent.Set();
            MainForm.UpdateCncConnectionStatus();
        }

        // =========================
        // low level writes:
        bool Write(string command)
        {
            if (!Com.IsOpen)
            {
                MainForm.DisplayText("###" + command + " discarded, com not open (readyevent set)");
                _readyEvent.Set();
                Cnc.Connected = false;
                return false;
            }
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("###" + command + " discarded, error state on (readyevent set)");
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
        bool BlockingWriteDone = false;
        bool WriteOk = true;
        private void CNC_BlockingWrite_thread(string cmd)
        {
            Cnc_ReadyEvent.Reset();
            WriteOk = Write(cmd);
            Cnc_ReadyEvent.Wait();
            BlockingWriteDone = true;
        }

        public bool Write_m(string s, int Timeout = 250)
        {
            if (Cnc.ErrorState)
            {
                MainForm.DisplayText("### " + s + " ignored, cnc is in error state", KnownColor.DarkRed);
                return false;
            };

            BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingWrite_thread(s));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            if (Homing)
            {
                Timeout = CNC_HomingTimeout * 1000 / 2;
            };
            while (!BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    Cnc_ReadyEvent.Set();  // terminates the CNC_BlockingWrite_thread
                    MainForm.ShowMessageBox(
                        "Debug: CNC_BlockingWrite: Timeout on command " + s,
                        "Timeout",
                        MessageBoxButtons.OK);
                    BlockingWriteDone = true;
                    MainForm.JoggingBusy = false;
                    Error();
                    MainForm.SetValidMeasurement_checkBox(false);
                    return false;
                }
            }
            if (!WriteOk)
            {
                MainForm.SetValidMeasurement_checkBox(false);
            }
            return (WriteOk);
        }

        // =========================
        public bool JustConnected()
        {
            RawWrite("\x11");  // Xon
            Thread.Sleep(50);   // TinyG wakeup

            // get initial position
            if (!Write_m("{sr:n}"))
            {
                return false;
            }
            if (Cnc.Controlboard == CNC.ControlBoardType.TinygHW)
            {
                MainForm.DisplayText("Reading TinyG settings:");
                if (!LoopTinyGParameters())
                {
                    return false;
                }
            }

            // Do settings that need to be done always
            Cnc.ProbingMode(false);

            Write_m("{\"me\":\"\"}");  // motor power on
            MainForm.SetMotorPower_checkBox(true);
            return true;
        }

        // =========================
        // Sends the calls that will result to messages that update the values shown on UI

        private bool LoopTinyGParameters()
        {

            foreach (var parameter in MainForm.TinyGBoard.GetType().GetProperties())
            {
                // The motor parameters are <motor number><parameter>, such as 1ma, 1sa, 1tr etc.
                // These are not valid parameter names, so Motor1ma, motor1sa etc are used.
                // to retrieve the values, we remove the "Motor"
                string Name = parameter.Name;
                if (Name.StartsWith("Motor", StringComparison.Ordinal))
                {
                    Name = Name.Substring(5);
                }
                if (!Write_m("{\"" + Name + "\":\"\"}"))
                {
                    return false;
                };
                //Thread.Sleep(500);
            }
            return true;
        }

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

            if (line.Contains("SYSTEM READY"))
            {
                Error();
                MainForm.ShowMessageBox(
                    "TinyG Reset.",
                    "System Reset",
                    MessageBoxButtons.OK);
                MainForm.SetMotorPower_checkBox(false);
                MainForm.UpdateCncConnectionStatus();
                return;
            }

            if (line.StartsWith("{\"r\":{\"msg", StringComparison.Ordinal))
            {
                line = line.Substring(13);
                int i = line.IndexOf('"');
                line = line.Substring(0, i);
                MainForm.ShowMessageBox(
                    "TinyG Message:",
                    line,
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"er\":", StringComparison.Ordinal))
            {
                if (line.Contains("File not open") && IgnoreError)
                {
                    MainForm.DisplayText("### Ignored file not open error ###");
                    return;
                };
                Error();
                MainForm.ShowMessageBox(
                    "TinyG error. Review situation and restart if needed.",
                    "TinyG Error",
                    MessageBoxButtons.OK);
                return;
            }


            if (line.StartsWith("{\"r\":{}", StringComparison.Ordinal))
            {
                // ack for g code command
                return;
            }

            /* Special homing handling is not needed in this firmware version
            if (Homing)
            {
                if (line.StartsWith("{\"sr\":"))
                {
                    // Status report
                    NewStatusReport(line);
				}

                if (line.Contains("\"home\":1"))
                {
                    _readyEvent.Set();
                    MainForm.DisplayText("ReadyEvent home");
                }
                return; 
            }
            */

            if (line.StartsWith("tinyg [mm] ok>", StringComparison.Ordinal))
            {
                // MainForm.DisplayText("ReadyEvent ok");
                _readyEvent.Set();
                return;
            }


            if (line.StartsWith("{\"sr\":", StringComparison.Ordinal))
            {
                // Status report
                NewStatusReport(line);
                if (line.Contains("\"stat\":3"))
                {
                    MainForm.DisplayText("ReadyEvent stat");
                    MainForm.ResetMotorTimer();
                    _readyEvent.Set();
                }
                return;
            }

            if (line.StartsWith("{\"r\":{\"sr\"", StringComparison.Ordinal))
            {
                // Status enquiry response, remove the wrapper:
                line = line.Substring(5);
                int i = line.IndexOf("}}", StringComparison.Ordinal);
                line = line.Substring(0, i + 2);
                NewStatusReport(line);
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent r:sr");
                return;
            }

            if (line.StartsWith("{\"r\":{\"me", StringComparison.Ordinal) || line.StartsWith("{\"r\":{\"md", StringComparison.Ordinal))
            {
                // response to motor power on/off commands
                _readyEvent.Set();
                return;
            }

            if (line.StartsWith("{\"r\":", StringComparison.Ordinal))
            {
                // response to setting a setting or reading motor settings for saving them
                ParameterValue(line);  // <========= causes UI update
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent r");
                return;
            }

            if (line.StartsWith("{\"r\":{\"sys\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent sys group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"x\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                /*
                // remove the wrapper: 
                line = line.Substring(5);
                int i = line.IndexOf("}}");
                line = line.Substring(0, i + 2);
                MainForm.TinyGSetting.TinyG_x = line; 
                */
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent x group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"y\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent y group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"z\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent z group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"a\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent a group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"1\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m1 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"2\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m2 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"3\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m3 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"4\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                _readyEvent.Set();
                MainForm.DisplayText("ReadyEvent m4 group (depreciated)");
                return;
            }

        }  // end InterpretLine()

        public void ParameterValue(string line)
        {
            // line format is {"r":{"<parameter>":<value>},"f":[<some numbers>]}
            line = line.Substring(7);  // line: <parameter>":<value>},"f":[<some numbers>]}
            string parameter = line.Split(':')[0];            // line: <parameter>"
            parameter = parameter.Substring(0, parameter.Length - 1);     // remove the "
            line = line.Substring(line.IndexOf(':') + 1);   //line: <value>},"f":[<some numbers>]}
            line = line.Substring(0, line.IndexOf('}'));    // line is now the value
            MainForm.ValueUpdater(parameter, line);

        }
        // =================================================================================
        // TinyG JSON stuff
        // =================================================================================

        // =================================================================================
        // Status report


        [Serializable]
        internal class StatusReport
        {
            public Sr sr { get; set; }
        }

        StatusReport Status { get; set; }
        public void NewStatusReport(string line)
        {
            //MainForm.DisplayText("NewStatusReport: " + line);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Status = serializer.Deserialize<StatusReport>(line);
        }

        [Serializable]
        internal class Sr
        {
            // mpox, posy, ...: Position
            // NOTE: Some firmware versions use mpox, mpoy,... some use posx, posy, ... 
            // This should be reflected in the public variable names
            private double _posx = 0;
            public double posx // <======================== here
            {
                get { return _posx; }
                set
                {
                    _posx = value;
                    CNC.setCurrX(_posx);
                }
            }

            private double _posy = 0;
            public double posy // <======================== and here
            {
                get { return _posy; }
                set
                {
                    _posy = value;
                    CNC.setCurrY(_posy);
                }
            }

            private double _posz = 0;
            public double posz // <======================== and here
            {
                get { return _posz; }
                set
                {
                    _posz = value;
                    CNC.setCurrZ(_posz);
                }
            }

            private double _posa = 0;
            public double posa // <======================== and here
            {
                get { return _posa; }
                set
                {
                    _posa = value;
                    CNC.setCurrA(_posa);
                }
            }

        }

    }
}
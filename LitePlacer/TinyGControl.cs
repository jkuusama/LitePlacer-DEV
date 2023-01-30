using System;
using System.Windows.Forms;
using System.Threading;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Globalization;
using System.Drawing;

namespace LitePlacer
{
    public class TinyGclass
    {
        FormMain MainForm;
        CNC Cnc;
        SerialComm Com;

        public TinyGclass(FormMain MainF, CNC C, SerialComm ser)
        {
            MainForm = MainF;
            Cnc = C;
            Com = ser;
        }

        static ManualResetEvent ReadyEvent = new ManualResetEvent(false);

        public int RegularMoveTimeout { get; set; } // in ms

        // =================================================================================

        #region Communications
        public bool JustConnected()
        {
            MainForm.DisplayText("Reading TinyG settings:");
            if (!LoopTinyGParameters())
            {
                return false;
            }
            // Do settings that need to be done always
            if (!Write_m("{\"me\":\"\"}"))      // motor power on
            {
                return false;
            }
            // Check aam parameter and if not 1, force it
            //     TODO: Find out the real problem (issue #176), don't hide it!
            if (MainForm.TinyGBoard.Aam != "1")
            {
                if (!Write_m("{\"aam\":1}"))      // aam mode
                {
                    return false;
                }
            }
            MainForm.SetMotorPower_checkBox(true);
            return true;
        }



        public void Close()
        {
            Com.Close();
            Cnc.ErrorState = false;
            Cnc.Connected = false;
            Cnc.Homing = false;
            ReadyEvent.Set();
            MainForm.UpdateCncConnectionStatus();
        }


        // =================================================================================
        // Write_m():
        // Sends a command to the board, doesn't return until the response is handled
        // by LineReceived() (which sets ReadyEvent), or operation times out

        bool BlockingWriteDone = false;
        bool WriteOk = true;

        private void BlockingWrite_thread(string cmd)
        {
            ReadyEvent.Reset();
            WriteOk = Com.Write(cmd);
            ReadyEvent.WaitOne();
            BlockingWriteDone = true;
        }

        public bool Write_m(string cmd, int Timeout= 250, bool report = true)
        {
            if (!Com.IsOpen)
            {
                if (report)
                {
                    MainForm.DisplayText("### " + cmd + " discarded, com not open (readyevent set)");
                }
                ReadyEvent.Set();
                Cnc.Connected = false;
                return false;
            }
            if (Cnc.ErrorState)
            {
                if (report)
                {
                    MainForm.DisplayText("### " + cmd + " discarded, error state on (readyevent set)");
                }
                ReadyEvent.Set();
                return false;
            }

            BlockingWriteDone = false;
            Thread t = new Thread(() => BlockingWrite_thread(cmd));
            t.IsBackground = true;
            t.Name = "TinyGwrite";
            t.Start();

            Timeout = Timeout / 2;
            int i = 0;
            while (!BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    ReadyEvent.Set();  // terminates the CNC_BlockingWrite_thread
                    if (report)
                    {
                        MainForm.ShowMessageBox(
                            "TinyG.Write_m: Timeout on command " + cmd,
                            "Timeout",
                            MessageBoxButtons.OK);
                    }
                    BlockingWriteDone = true;
                    return false;
                }
            }
            return (WriteOk);
        }



        // For operations that cause conflicts with event firings or don't give response
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
                RawWrite("{\"gc\":\"G1 F" + Speed + " X" + X + "\"}");
            }
            else if (Y != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " Y" + Y + "\"}");
            }
            else if (Z != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " Z" + Z + "\"}");
            }
            else if (A != "")
            {
                RawWrite("{\"gc\":\"G1 F" + Speed + " A" + A + "\"}");
            }
            else
            {
                MainForm.DisplayText("***Jog, no axis", KnownColor.DarkRed, true);
            }
        }



        private bool HomingTimeout_m(out int TimeOut, string axis)
        {
            string SpeedStr = "0";
            double size;
            TimeOut = 0;
            switch (axis)
            {
                case "X":
                    SpeedStr = MainForm.xsv_maskedTextBox.Text;
                    size = MainForm.Setting.General_MachineSizeX;
                    break;

                case "Y":
                    SpeedStr = MainForm.ysv_maskedTextBox.Text;
                    size = MainForm.Setting.General_MachineSizeY;
                    break;

                case "Z":
                    SpeedStr = MainForm.zsv_maskedTextBox.Text;
                    size = 100.0;
                    break;

                default:
                    return false;
            }

            double Speed;
            if (!double.TryParse(SpeedStr.Replace(',', '.'), out Speed))
            {
                MainForm.ShowMessageBox(
                    "Bad data in " + axis + " homing speed",
                    "Data error",
                    MessageBoxButtons.OK);
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
            int timeout;
            if (!HomingTimeout_m(out timeout, axis))
            {
                return false;
            }

            MainForm.DisplayText("Homing axis " + axis + ", timeout value: " + timeout.ToString(CultureInfo.InvariantCulture));

            if (!Write_m("{\"gc\":\"G28.2 " + axis + "0\"}", timeout))
            {
                MainForm.ShowMessageBox(
                    "Homing operation mechanical step failed, CNC issue",
                    "Homing failed",
                    MessageBoxButtons.OK);
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
            return Write_m("{\"gc\":\"" + command + "\"}", RegularMoveTimeout);
        }

        public bool A(double A, double speed, string MoveType)
        {
            string command;
            if (MoveType == "G1")
            {
                command = "G1 F" + speed.ToString() +
                    " A" + A.ToString("0.000",CultureInfo.InvariantCulture);
            }
            else
            {
                command = "G0 " +
                    " A" + A.ToString("0.000", CultureInfo.InvariantCulture);
            }
            return Write_m("{\"gc\":\"" + command + "\"}", RegularMoveTimeout);
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
            return Write_m("{\"gc\":\"" + command + "\"}", RegularMoveTimeout);
        }

        #endregion Movement



        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public bool SetMachineSizeX(int Xsize)
        {
            bool res = Write_m("{\"xtm\":" + Xsize.ToString(CultureInfo.InvariantCulture) + "}", 100);
            Thread.Sleep(50);
            return res;
        }

        public bool SetMachineSizeY(int Ysize)
        {
            bool res = Write_m("{\"ytm\":" + Ysize.ToString(CultureInfo.InvariantCulture) + "}", 100);
            Thread.Sleep(50);
            return res;
        }


        public void DisableZswitches()
        {
            Write_m("{\"zsn\":0}", 100);
            Thread.Sleep(50);
            Write_m("{\"zsx\":0}", 100);
            Thread.Sleep(50);
        }


        public void EnableZswitches()
        {
            Write_m("{\"zsn\":3}", 100);
            Thread.Sleep(50);
            Write_m("{\"zsx\":2}", 100);
            Thread.Sleep(50);
        }


        public bool Nozzle_ProbeDown(double backoff)
        {
            MainForm.DisplayText("Probing, TinyG");
            Write_m("{\"zsn\",0}", 150);
            Thread.Sleep(50);
            Write_m("{\"zsx\",1}",150);
            Thread.Sleep(50);
            Write_m("{\"zzb\",0}", 150);
            Thread.Sleep(50);

            if (!Write_m("{\"gc\":\"G28.4 Z0\"}", RegularMoveTimeout))
            {
                return false;
            }
            Write_m("{\"zzb\",2}", 150);
            Thread.Sleep(50);
            EnableZswitches();
            if (!MainForm.CNC_Z_m(Cnc.CurrentZ - backoff))
            {
                return false;
            }
            return true;
        }


        public void MotorPowerOn()
        {
            MainForm.DisplayText("MotorPowerOn(), TinyG");
            Write_m("{\"me\":\"\"}");
            MainForm.ResetMotorTimer();
        }



        public void MotorPowerOff()
        {
            MainForm.DisplayText("MotorPowerOff(), TinyG");
            MainForm.TimerDone = true;
            Write_m("{\"md\":\"\"}");
        }



        public void VacuumOn()
        {
            MainForm.DisplayText("VacuumOn(), TinyG");
            RawWrite("{\"gc\":\"M08\"}");
        }



        public void VacuumOff()
        {
            MainForm.DisplayText("VacuumOff(), TinyG");
            RawWrite("{\"gc\":\"M09\"}");
        }



        private void BugWorkaround()
        {
            // see https://www.synthetos.com/topics/file-not-open-error/#post-7194
            // Summary: In some cases, we need a dummy move.
            MainForm.CNC_A_m(Cnc.CurrentA - 0.01);
            MainForm.CNC_A_m(Cnc.CurrentA + 0.01);
        }



        public void PumpOn()
        {
            MainForm.DisplayText("PumpOn(), TinyG");
            RawWrite("{\"gc\":\"M03\"}");
        }



        public void PumpOff()
        {
            MainForm.DisplayText("PumpOff(), TinyG");
            RawWrite("{\"gc\":\"M05\"}");
        }

        #endregion Features


        // =================================================================================
        // Board settings
        #region settings

        public bool LoopTinyGParameters()
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

        #endregion


        // ===============================
        // Read single line:

        private bool LineWanted = false;
        private string LineOut;

        public string ReadLineDirectly(string command, bool report=true)
        {
            LineWanted = true;
            if (Write_m(command, 250, report))
            {
                return LineOut;
            }
            LineWanted = false;
            return "";
        }


        public void LineReceived(string line)
        {
            // This is called from SerialComm dataReceived, and runs in a separate thread than UI            
            MainForm.DisplayText("<== " + line);

            // In some cases, the caller wants to look at the line directly:
            if (LineWanted)
            {
                LineOut = line;
                LineWanted = false;
                ReadyEvent.Set();
                return;
            }

            if (line.Contains("SYSTEM READY"))
            {
                Cnc.RaiseError();
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
                if (line.Contains("File not open"))
                {
                    MainForm.DisplayText("### File not open error ignored. This is TinyG quirk, not a real error.");
                    return;
                };
                Cnc.RaiseError();
                if (line.Contains("Limit"))
                {
                    MainForm.ShowMessageBox(
                        "Limit switch hit. Reset the TinyG and reconnect. Check job status if needed.",
                        "TinyG Error",
                        MessageBoxButtons.OK);
                    return;
                }
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
                ReadyEvent.Set();
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
                    ReadyEvent.Set();
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
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent r:sr");
                return;
            }

            if (line.StartsWith("{\"r\":{\"me", StringComparison.Ordinal) || line.StartsWith("{\"r\":{\"md", StringComparison.Ordinal))
            {
                // response to motor power on/off commands
                ReadyEvent.Set();
                return;
            }

            if (line.StartsWith("{\"r\":", StringComparison.Ordinal))
            {
                // response to setting a setting or reading motor settings for saving them
                ParameterValue(line);  // <========= causes UI update
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent r");
                return;
            }

            if (line.StartsWith("{\"r\":{\"sys\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
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
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent x group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"y\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent y group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"z\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent z group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"a\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent a group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"1\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent m1 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"2\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent m2 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"3\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent m3 group (depreciated)");
                return;
            }

            if (line.StartsWith("{\"r\":{\"4\":", StringComparison.Ordinal))
            {
                // response to reading settings for saving them
                ReadyEvent.Set();
                MainForm.DisplayText("ReadyEvent m4 group (depreciated)");
                return;
            }

        }  // end LineReceived()
        public string GetParameterValue(string line)
        {
            // line format is {"r":{"<parameter>":<value>},"f":[<some numbers>]}
            line = line.Substring(7);  // line: <parameter>":<value>},"f":[<some numbers>]}
            line = line.Substring(line.IndexOf(':') + 1);   //line: <value>},"f":[<some numbers>]}
            line = line.Substring(0, line.IndexOf('}'));    // line is now the value
            return line;
        }
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
                    CNC.MainForm.Cnc.SetCurrentX(_posx);
                    CNC.MainForm.Update_Xposition();
                }
            }

            private double _posy = 0;
            public double posy // <======================== and here
            {
                get { return _posy; }
                set
                {
                    _posy = value;
                    CNC.MainForm.Cnc.SetCurrentY(_posy);
                    CNC.MainForm.Update_Yposition();
                }
            }

            private double _posz = 0;
            public double posz // <======================== and here
            {
                get { return _posz; }
                set
                {
                    _posz = value;
                    CNC.MainForm.Cnc.SetCurrentZ(_posz);
                    CNC.MainForm.Update_Zposition();
                }
            }

            private double _posa = 0;
            public double posa // <======================== and here
            {
                get { return _posa; }
                set
                {
                    _posa = value;
                    CNC.MainForm.Cnc.SetCurrentA(_posa);
                    CNC.MainForm.Update_Aposition();
                }
            }

        }



    }
}

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

/*
CNC class handles communication with the control board. Most calls are just passed to a supported board.
(For now, there are only two supported boards, so most routines are
    - if board is Duet3, call Duet3.routine
    - else if board is Tinyg, call TinyG.routine
    - else report and return failure )

Here are templates for that:

        public void ()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.###(), board in error state.", KnownColor.DarkRed, true);
                return;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        public bool ()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.###(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.())
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.())
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else
            {
                MainForm.DisplayText("*** Cnc.(), unknown board.", KnownColor.DarkRed, true);
                Connected = false;
                ErrorState = true;
                return false;
            }
        }


*/

namespace LitePlacer
{

    public class CNC
    {
        public static FormMain MainForm;
        private TinyGclass TinyG;
        public Duet3class Duet3;

        public enum ControlBoardType { TinyG, Duet3, other, unknown };
        public ControlBoardType Controlboard { get; set; } = ControlBoardType.unknown;

        public bool SlackCompensation { get; set; }
        public double SlackCompensationDistance { get; set; }

        public bool SlackCompensationA { get; set; }
        private double SlackCompensationDistanceA = 5.0;


        public bool SlowXY { get; set; }
        public double NozzleSpeedXY { get; set; }

        public bool SlowZ { get; set; }
        public double NozzleSpeedZ { get; set; }

        public bool SlowA { get; set; }
        public double NozzleSpeedA { get; set; }

        public bool Homing { get; set; }

        SerialComm Com;


        // =================================================================================
        public CNC(FormMain MainF)
        {
            MainForm = MainF;
            SlowXY = false;
            SlowZ = false;
            SlowA = false;
            Com = new SerialComm(this, MainF);
            TinyG = new TinyGclass(MainForm, this, Com);
            Duet3 = new Duet3class(MainForm, this, Com);
        }

        // =================================================================================
        // Timeout
        private double regTimeout = 10;

        public double RegularMoveTimeout // in seconds
        { 
            get
            {
                return (regTimeout);
            }
            set
            {
                regTimeout = value;
                TinyG.RegularMoveTimeout = (int)value*1000;  // in ms
                Duet3.RegularMoveTimeout = (int)value * 1000;  // in ms
            }
        }

        // =================================================================================
        // Square compensation, current position:
        #region Position

        // Square correction:
        // The machine will be only approximately square. Fortunately, the squareness is easy to measure with camera.
        // User measures correction value, that we apply to movements and reads.
        // For example, correction value is +0.002, meaning that for every unit of +Y movement, 
        // the machine actually also unintentionally moves 0.002 units to +X. 
        // Therefore, for each movement when the user wants to go to (X, Y),
        // we really go to (X - 0.002*Y, Y)

        // CurrentX, CurrentXY are the corrected values that user and MainForm sees and uses.
        // These reflect a square machine.
        // TrueX/Y is what the control board actually uses.

        // CurrentZ, CurrentA are the values that user and MainForm sees and uses; no correction at the moment

        public static double SquareCorrection { get; set; }

        private static double CurrX;
        private static double _trueX;

        public double TrueX
        {
            get
            {
                return (_trueX);
            }
            set
            {
                _trueX = value;
            }
        }

        public double CurrentX
        {
            get
            {
                return (CurrX);
            }
            set
            {
                CurrX = value;
            }
        }

        public static void setCurrX(double x)
        {
            _trueX = x;
            CurrX = x - CurrY * SquareCorrection;
            //MainForm.DisplayText("CNC.setCurrX: x= " + x.ToString() + ", CurrX= " + CurrX.ToString() + ", CurrY= " + CurrY.ToString());
        }



        private static double CurrY;
        public double CurrentY
        {
            get
            {
                return (CurrY);
            }
            set
            {
                CurrY = value;
            }
        }

        public static void setCurrY(double y)
        {
            CurrY = y;
            CurrX = _trueX - CurrY * SquareCorrection;
            //MainForm.DisplayText("CNC.setCurrY: "+ y.ToString()+ " CurrX= " + CurrX.ToString());
        }


        private static double CurrZ;
        public double CurrentZ
        {
            get
            {
                return (CurrZ);
            }
            set
            {
                CurrZ = value;
            }
        }
        public static void setCurrZ(double z)
        {
            CurrZ = z;
        }


        private static double CurrA;
        public double CurrentA
        {
            get
            {
                return (CurrA);
            }
            set
            {
                CurrA = value;
            }
        }
        public static void setCurrA(double a)
        {
            CurrA = a;
        }


        public void SetPosition(string X, string Y, string Z, string A)
        {
            if ((X + Y + Z + A) == "")
            {
                MainForm.DisplayText("*** Cnc.SetPosition(), no coordinates.", KnownColor.DarkRed, true);
                return;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.SetPosition(X, Y, Z, A);
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.SetPosition(X, Y, Z, A);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.SetPosition(), unknown board.", KnownColor.DarkRed, true);
            }
        }



        public void CancelJog()
        {
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.CancelJog();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.CancelJog();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.CancelJog(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        public void Jog(string Speed, string X, string Y, string Z, string A)
        {
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.Jog(Speed, X, Y, Z, A);
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.Jog(Speed, X, Y, Z, A);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.Jog(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        #endregion Position

        // =================================================================================
        #region Communications

        public bool Connected { get; set; } // If connectged to a serial port
        public string Port { get; set; }  // valid only if connected
        public bool ErrorState { get; set; }    // Cnc is in error or not connected

        public void RaiseError()
        {
            ErrorState = true;
            // Connected = false;
            // Com.Close();
            MainForm.ValidMeasurement_checkBox.Checked = false;
            MainForm.UpdateCncConnectionStatus();
        }

        public void InterpretLine(string line)
        {
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.LineReceived(line);
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.InterpretLine(line);
            }
            else if (line.Contains("\", \"msg\":\"SYSTEM READY\"}"))     // TinyG reset message
            {
                TinyG.InterpretLine(line);  // This will give the user a message
            }
            else 
            {
                // MainForm.DisplayText("*** Cnc.InterpretLine(), unknown board.", KnownColor.DarkRed, true);
                MainForm.DisplayText("Line: " + line, KnownColor.Black, true);
            }
        }


        bool OpenPort(string port)
        {
            if (Com.IsOpen)
            {
                MainForm.DisplayText("Already connected to serial port " + port + ": already open");
                Port = port;
                Connected = true;
            }
            else
            {
                Com.Open(port);
                if (!Com.IsOpen)
                {
                    MainForm.DisplayText("Connecting to serial port " + port + " failed.");
                    RaiseError();
                    Connected = false;
                    return false;
                }
                else
                {
                    MainForm.DisplayText("Connected to serial port " + port);
                }
            }
            Port = port;
            Connected = true;
            return true;
        }


        public bool Connect(string port)
        {
            ErrorState = false;
            Connected = false;

            if (!OpenPort(port))
            {
                return false;
            }
            Connected = true;
            Port = port;
            Controlboard = ControlBoardType.Duet3;      // to direct the response to correct module
            if (Duet3.CheckIdentity())
            {
                MainForm.Motors_label.Text = "Axes setup (Duet3 board):";
                MainForm.Duet3Motors_tabControl.Visible = true;
                ErrorState = false;
                return true;
            }
            else
            {
                Com.Close();
                Connected = false;
            }

            // Not Duet. TinyG?
            if (!OpenPort(port))
            {
                return false;
            }
            Connected = true;
            Controlboard = ControlBoardType.TinyG;
            if (TinyG.CheckIdentity())
            {
                MainForm.Motors_label.Text = "Axes setup (TinyG board):";
                MainForm.TinyGMotors_tabControl.Visible = true;
                ErrorState = false;
                return true;
            }
            Controlboard = ControlBoardType.unknown;
            MainForm.DisplayText("*** Cnc.Connect(), did not find a supported board.", KnownColor.DarkRed, true);
            RaiseError();
            return false;
        }


        public bool JustConnected()
        {
            // Called after a control board connection is estabished.
            // Set sleep time according to slowest board supported.
            // TinyG: 200 (ms)
            // Duet 3: ??
            Thread.Sleep(200);

            if (Controlboard == ControlBoardType.Duet3)
            {
                if (!Duet3.JustConnected())
                {
                    RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (!TinyG.JustConnected())
                {
                    RaiseError();
                    return false;
                }
            }
            else
            {
                MainForm.DisplayText("**Unknown/unsupported control board.");
                RaiseError();
                return false;
            }
            // Do settings that need to be done always
            return true;
        }


        public void Close()
        {
            Com.Close();
            ErrorState = false;
            Connected = false;
            Homing = false;
        }


        // Sends a command to control board, doesn't return until the response is handled
        public bool Write_m (string command, int Timeout = 250)
        {
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.Write_m(command, Timeout))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.Write_m(command, Timeout))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            MainForm.DisplayText("*** Cnc.Write(), unknown board.", KnownColor.DarkRed, true);
            Connected = false;
            ErrorState = true;
            return false;
        }


        // For operations that cause conflicts with event firings or don't give response
        // Caller does waiting, if needed.
        public bool RawWrite(string command)
        {
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.RawWrite(command))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.RawWrite(command))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            MainForm.DisplayText("*** Cnc.RawWrite(), unknown board.", KnownColor.DarkRed, true);
            Connected = false;
            ErrorState = true;
            return false;
        }



        // ===================================================================
        // Write, that doesn't care what we think of the board or communication link status
        public void ForceWrite(string command)
        {
            Com.Write(command);
        }

        #endregion Communications

        // =================================================================================
        // Hardware features: probing, pump, vacuum, motor power
        #region Features

        public bool SetMachineSizeX(int Xsize)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.SetMachineSizeX(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.SetMachineSizeX(Xsize))
                {
                    return true;
                }
                else
                {
                    // RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.SetMachineSizeX(Xsize))
                {
                    return true;
                }
                else
                {
                    // RaiseError();
                    return false;
                }
            }
            else
            {
                MainForm.DisplayText("*** Cnc.SetMachineSizeX(), unknown board.", KnownColor.DarkRed, true);
                Connected = false;
                ErrorState = true;
                return false;
            }
        }


        public bool SetMachineSizeY(int Ysize)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.SetMachineSizeY(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.SetMachineSizeY(Ysize))
                {
                    return true;
                }
                else
                {
                    // RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.SetMachineSizeY(Ysize))
                {
                    return true;
                }
                else
                {
                    // RaiseError();
                    return false;
                }
            }
            else
            {
                MainForm.DisplayText("*** Cnc.SetMachineSizeY(), unknown board.", KnownColor.DarkRed, true);
                Connected = false;
                ErrorState = true;
                return false;
            }
        }

        // =============================================================================================

        public void DisableZswitches()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.DisableZswitches(), board in error state.", KnownColor.DarkRed, true);
                return;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.DisableZswitches();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.DisableZswitches();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.DisableZswitches(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        public void EnableZswitches()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.EnableZswitches(), board in error state.", KnownColor.DarkRed, true);
                return;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.EnableZswitches();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.EnableZswitches();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.EnableZswitches(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        public bool Nozzle_ProbeDown(double backoff)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Nozzle_ProbeDown(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.Nozzle_ProbeDown(backoff))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.Nozzle_ProbeDown(backoff))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            MainForm.DisplayText("*** Cnc.Nozzle_ProbeDown(), unknown board.", KnownColor.DarkRed, true);
            Connected = false;
            ErrorState = true;
            return false;
        }


        // =============================================================================================

        public void MotorPowerOn()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.MotorPowerOn(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.MotorPowerOn();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.MotorPowerOn();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.MotorPowerOn(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        public void MotorPowerOff()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.MotorPowerOff(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.MotorPowerOff();
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.MotorPowerOff();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.MotorPowerOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        public bool VacuumIsOn = false;

        public void VacuumDefaultSetting()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.VacuumDefaultSetting(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            Vacuum_Off();
        }


        public void Vacuum_On()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.VacuumOn(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.VacuumOn();
                VacuumIsOn = true;
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.VacuumOn();
                VacuumIsOn = true;
            }
            else
            {
                MainForm.DisplayText("*** Cnc.VacuumOn(), unknown board.", KnownColor.DarkRed, true);
                return;
            }
            MainForm.Vacuum_checkBox.Checked = true;
        }

        public void Vacuum_Off()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.VacuumOff(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.VacuumOff();
                VacuumIsOn = false;
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.VacuumOff();
                VacuumIsOn = false;
            }
            else
            {
                MainForm.DisplayText("*** Cnc.VacuumOff(), unknown board.", KnownColor.DarkRed, true);
                return;
            }
            MainForm.Vacuum_checkBox.Checked = false;
        }


        public bool PumpIsOn { get; set; } = false;

        public void PumpDefaultSetting()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.PumpDefaultSetting(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            Pump_Off();
        }


        public void Pump_On()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.PumpOn(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.PumpOn();
                PumpIsOn = true;
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.PumpOn();
                PumpIsOn = true;
            }
            else
            {
                MainForm.DisplayText("*** Cnc.PumpOn(), unknown board.", KnownColor.DarkRed, true);
                return;
            }
            MainForm.Pump_checkBox.Checked = false;
        }

        public void Pump_Off()
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.PumpOff(), board in error state.", KnownColor.DarkRed, true);
                return;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                Duet3.PumpOff();
                PumpIsOn = false;
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                TinyG.PumpOff();
                PumpIsOn = false;
            }
            else
            {
                MainForm.DisplayText("*** Cnc.PumpOff(), unknown board.", KnownColor.DarkRed, true);
                return;
            }
            MainForm.Pump_checkBox.Checked = false;
        }

        #endregion Features

        // =================================================================================
        // Movement
        #region Movement

        private decimal SmallMoveSpeed_dec = 250;
        private double SmallMoveSpeed = 250;

        public decimal SmallMovementSpeed
        {
            get
            {
                return (SmallMoveSpeed_dec);
            }
            set
            {
                SmallMoveSpeed_dec = value;
                SmallMoveSpeed = (double)value;
            }
        }


        public bool Home_m(string axis)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Home_m(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.Home_m(axis))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.Home_m(axis))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            };

            MainForm.DisplayText("*** Cnc.Home_m(), unknown board.", KnownColor.DarkRed, true);
            Connected = false;
            ErrorState = true;
            return false;
        }

        // =================================================================================
        // The execute_xxx pass the move commands to control board, with all the higer level logic
        // already handled (such as speed in different situations, slack compensation, square compensation etc)


        public bool Execute_XYA(double X, double Y, double A, double speed, string MoveType)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Execute_XYA(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.XYA(X, Y, A, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.XYA(X, Y, A, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else
            {
                Connected = false;
                ErrorState = true;
                MainForm.DisplayText("*** Cnc.Execute_XYA(), unknown board.", KnownColor.DarkRed, true);
                return false;
            }
        }


        public bool Execute_A(double A, double speed, string MoveType)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Execute_A(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.A(A, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.A(A, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else
            {
                Connected = false;
                ErrorState = true;
                MainForm.DisplayText("*** Cnc.Execute_A(), unknown board.", KnownColor.DarkRed, true);
                return false;
            }
        }


        public bool Execute_Z(double Z, double speed, string MoveType)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Execute_Z(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }
            if (Controlboard == ControlBoardType.Duet3)
            {
                if (Duet3.Z(Z, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else if (Controlboard == ControlBoardType.TinyG)
            {
                if (TinyG.Z(Z, speed, MoveType))
                {
                    return true;
                }
                else
                {
                    RaiseError();
                    return false;
                }
            }
            else
            {
                Connected = false;
                ErrorState = true;
                MainForm.DisplayText("*** Cnc.Execute_Z(), unknown board.", KnownColor.DarkRed, true);
                return false;
            }
        }


        // =================================================================================
        // Main XYA move command
        public bool XYA(double X, double Y, double A)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.XYA(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            bool CompensateXY = false;
            bool CompensateA = false;

            //if ((SlackCompensation) && ((CurrentX > X) || (CurrentY > Y)))
            if (SlackCompensation)
            {
                CompensateXY = true;
            }

            if ((SlackCompensationA) && (CurrentA > Math.Abs(A - SlackCompensationDistanceA)))
            {
                CompensateA = true;
            }

            if ((!CompensateXY) && (!CompensateA))
            {
                return XYA_move(X, Y, A);
            }
            else if ((CompensateXY) && (!CompensateA))
            {
                if (!XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, A))
                {
                    return false;
                }
                return XYA_move(X, Y, A);
            }
            else if ((!CompensateXY) && (CompensateA))
            {
                if (!XYA_move(X, Y, A - SlackCompensationDistanceA))
                {
                    return false;
                }
                return A_move(A);
            }
            else
            {
                if (!XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, A - SlackCompensationDistanceA))
                {
                    return false;
                }
                return XYA_move(X, Y, A);
            }
        }



        // =================================================================================
        // Main A move command
        public bool A(double A)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.A(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            if (Math.Abs(A - CurrentA) < 0.0009)
            {
                MainForm.DisplayText(" -- zero A movement command --");
                return true;   // already there
            }
            if ((SlackCompensationA) && (CurrentA > (A - SlackCompensationDistanceA)))
            {
                if (!A_move(A - SlackCompensationDistanceA))
                {
                    return false;
                }
                return A_move(A);
            }
            else
            {
                return A_move(A);
            }
        }

        // =================================================================================
        // Main Z move command
        public bool Z(double Z)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.Z(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            double speed = 0;
            string MoveType = "G0";
            double dZ = Math.Abs(Z - CurrentZ);
            if (dZ < 0.005)
            {
                MainForm.DisplayText(" -- zero Z movement command --");
                return true;   // already there
            }
            if (SlowZ)
            {
                speed = NozzleSpeedZ;
                MoveType = "G1";
            }
            return Execute_Z(Z, speed, MoveType);
        }

        // =================================================================================
        // helper functions for XYA, XY A and Z moves

        private bool XYA_move(double X, double Y, double Am)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.XYA_move(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            // double speed = (double)Properties.Settings.Default.CNC_SmallMovementSpeed;
            // string MoveType = "G0";
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            double dA = Math.Abs(Am - CurrentA);
            if ((dX < 0.0005) && (dY < 0.0005) && (dA < 0.0005))
            {
                MainForm.DisplayText(" -- zero XYA movement command --", KnownColor.Gray);
                return true;   // already there
            }

            // It doesn't hurt to do only A when needed, and because of TinyG bug, is a must:
            if ((dX < 0.0005) && (dY < 0.0005))
            {
                return A_move(Am);
            };
            X = X + SquareCorrection * Y;
            X = Math.Round(X, 3);
            if ((dX < 1.0) && (dY < 1.0))
            {
                // small movement
                // First do XY move, then A. This works always.
                // (small moves and fast settings can sometimes cause problems)
                if (!Execute_XYA(X, Y, CurrentA, SmallMoveSpeed, "G1"))
                {
                    return false;
                }
                return A_move(Am);
            }
            else
            {
                // normal case, large move
                // Possibilities:
                // SlowA, SlowXY
                // SlowA, FastXY
                // FastA, SlowXY
                // Fast A, Fast XY
                // To avoid side effects, we'll separate a and xy for first three cases.

                // Normal case first:
                if (!SlowA && !SlowXY)
                {
                    // Fast A, Fast XY
                    return Execute_XYA(X, Y, Am, 0, "G0");  // on G0, speed doesn't matter
                }

                // Do A first, then XY
                if (dA < 0.0005)
                {
                    MainForm.DisplayText(" -- XYA command, A already there --", KnownColor.Gray);
                }
                else
                {
                    if (!A_move(Am))
                    {
                        return false;
                    }
                }
                // A done, we know XY is slow and large
                return Execute_XYA(X, Y, Am, NozzleSpeedXY, "G1");
            }
        }


        private bool A_move(double A)
        {
            if (ErrorState)
            {
                MainForm.DisplayText("*** Cnc.A_move(), board in error state.", KnownColor.DarkRed, true);
                return false;
            }

            double dA = Math.Abs(A - CurrentA);
            if (dA<0.0009)
            {
                MainForm.DisplayText(" -- A_move, already there --", KnownColor.Gray);
                return (true);  // Already there
            }
            double speed = 0;
            string MoveType = "G0";
            if (SlowA)
            {
                speed = NozzleSpeedA;
                MoveType = "G1";
            }
            return Execute_A(A, speed, MoveType);
        }


        #endregion Movement

        // =================================================================================

 

        // =================================================================================

    }  // end Class CNC
}
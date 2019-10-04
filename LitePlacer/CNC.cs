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

Here is a template for that:

        public void ()
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.();
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.(), unknown board.", KnownColor.DarkRed, true);
            }
        }

*/

namespace LitePlacer
{

    public class CNC
    {
        static FormMain MainForm;
        public TinyGclass TinyG;
        public Duet3class Duet3;

        public enum ControlBoardType { TinygHW, Duet3HW, unknownHW };
        public ControlBoardType Controlboard { get; set; } = ControlBoardType.unknownHW;

        static ManualResetEventSlim _readyEvent = new ManualResetEventSlim(false);

        public bool SlackCompensation { get; set; }
        public double SlackCompensationDistance { get; set; }

        public bool SlackCompensationA { get; set; }
        private double SlackCompensationDistanceA = 5.0;

        public string SmallMovementString { get; set; } = "G1 F200 ";

        public bool SlowXY { get; set; }
        public double SlowSpeedXY { get; set; }

        public bool SlowZ { get; set; }
        public double SlowSpeedZ { get; set; }

        public bool SlowA { get; set; }
        public double SlowSpeedA { get; set; }


        public CNC(FormMain MainF)
        {
            MainForm = MainF;
            SlowXY = false;
            SlowZ = false;
            SlowA = false;
        }

        // =================================================================================
        // =========================================================================================
        // home
        public bool Home_m(string axis)
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                return Duet3.Home_m(axis);
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                return TinyG.Home_m(axis);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.Home_m(" + axis + "), unknown board.", KnownColor.DarkRed, true);
                return false;
            }
        }

        // =================================================================================
        public void SetPosition(string X, string Y, string Z, string A)
        {
            if ((X + Y + Z + A) == "")
            {
                MainForm.DisplayText("*** Cnc.SetPosition(), no coordinates.", KnownColor.DarkRed, true);
                return;
            }

            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.SetPosition(X, Y, Z, A);
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.SetPosition(X, Y, Z, A);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.SetPosition(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        // =================================================================================
        public void ProbingMode(bool set)
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.ProbingMode(set);
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.ProbingMode(set);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.ProbingMode(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        public bool Nozzle_ProbeDown_m()
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                return Duet3.Nozzle_ProbeDown_m();
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                return TinyG.Nozzle_ProbeDown_m();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.Nozzle_ProbeDown_m(), unknown board.", KnownColor.DarkRed, true);
                return false;
            }
        }

        // =================================================================================
        // Square compensation:
        // The machine will be only approximately square. Fortunately, the squareness is easy to measure with camera.
        // User measures correction value, that we apply to movements and reads.
        // For example, correction value is +0.002, meaning that for every unit of +Y movement, 
        // the machine actually also unintentionally moves 0.002 units to +X. 
        // Therefore, for each movement when the user wants to go to (X, Y),
        // we really go to (X - 0.002*Y, Y)

        // CurrentX/Y is the corrected value that user sees and uses, and reflects a square machine
        // TrueX/Y is what the TinyG actually uses.

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
            MainForm.ValueUpdater("posx", x.ToString("0.000", CultureInfo.InvariantCulture));
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
            MainForm.ValueUpdater("posy", y.ToString("0.000", CultureInfo.InvariantCulture));
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
            MainForm.ValueUpdater("posz", z.ToString("0.000", CultureInfo.InvariantCulture));
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
            CNC.MainForm.ValueUpdater("posa", a.ToString("0.000", CultureInfo.InvariantCulture));
        }

        public void XY(double X, double Y)
        {
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004))
            {
                MainForm.DisplayText(" -- zero XY movement command --", KnownColor.Gray);
                MainForm.DisplayText("ReadyEvent: zero movement command", KnownColor.Gray);
                _readyEvent.Set();
                return;   // already there
            }
            //if ((SlackCompensation) && ((CurrentX > X) || (CurrentY > Y)))
            if (SlackCompensation)
            {
                XY_move(X - SlackCompensationDistance, Y - SlackCompensationDistance);
                XY_move(X, Y);
            }
            else
            {
                XY_move(X, Y);
            }
        }

        private void XY_move(double X, double Y)
        {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            if ((dX < 0.004) && (dY < 0.004))
            {
                MainForm.DisplayText(" -- zero XY movement command --", KnownColor.Gray);
                MainForm.DisplayText("ReadyEvent: zero movement command", KnownColor.Gray);
                _readyEvent.Set();
                return;   // already there
            }
            X = X + SquareCorrection * Y;
            X = Math.Round(X, 3);
            if ((dX < 1) && (dY < 1))
            {
                // Small move
                if (SlowXY)
                {
                    if ((double)MainForm.Setting.CNC_SmallMovementSpeed > SlowSpeedXY)
                    {
                        command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                       " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        command = "G1 F" + SlowSpeedXY.ToString(CultureInfo.InvariantCulture)
                                + " X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                   " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                // large move
                if (SlowXY)
                {
                    command = "G1 F" + SlowSpeedXY.ToString(CultureInfo.InvariantCulture)
                            + " X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                }
            }
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void XYA(double X, double Y, double Am)
        {
            bool CompensateXY = false;
            bool CompensateA = false;

            //if ((SlackCompensation) && ((CurrentX > X) || (CurrentY > Y)))
            if (SlackCompensation)
            {
                CompensateXY = true;
            }

            if ((SlackCompensationA) && (CurrentA > (Am - SlackCompensationDistanceA)))
            {
                CompensateA = true;
            }


            if ((!CompensateXY) && (!CompensateA))
            {
                XYA_move(X, Y, Am);
            }
            else if ((CompensateXY) && (!CompensateA))
            {
                XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, Am);
                XY_move(X, Y);
            }
            else if ((!CompensateXY) && (CompensateA))
            {
                XYA_move(X, Y, Am - SlackCompensationDistanceA);
                A_move(Am);
            }
            else
            {
                XYA_move(X - SlackCompensationDistance, Y - SlackCompensationDistance, Am - SlackCompensationDistanceA);
                XYA_move(X, Y, Am);
            }
        }

        private void XYA_move(double X, double Y, double Am)
        {
            string command;
            double dX = Math.Abs(X - CurrentX);
            double dY = Math.Abs(Y - CurrentY);
            double dA = Math.Abs(Am - CurrentA);
            if ((dX < 0.004) && (dY < 0.004) && (dA < 0.01))
            {
                MainForm.DisplayText(" -- zero XYA movement command --", KnownColor.Gray);
                MainForm.DisplayText("ReadyEvent: zero movement command", KnownColor.Gray);
                _readyEvent.Set();
                return;   // already there
            }

            X = X + SquareCorrection * Y;
            if ((dX < 1.0) && (dY < 1.0))
            {
                // small movement
                // First do XY move, then A. This works always.
                // (small moves and fast settings can sometimes cause problems)
                if ((dX < 0.004) && (dY < 0.004))
                {
                    MainForm.DisplayText(" -- XYA command, XY already there --", KnownColor.Gray);
                }
                else
                {
                    if (SlowXY)
                    {
                        if ((double)Properties.Settings.Default.CNC_SmallMovementSpeed > SlowSpeedXY)
                        {
                            command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                           " Y" + Y.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            command = "G1 F" + SlowSpeedXY.ToString(CultureInfo.InvariantCulture)
                                    + " X" + X.ToString(CultureInfo.InvariantCulture) + " Y" + Y.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        command = SmallMovementString + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                                       " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    }
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }

                // then A:
                if (dA < 0.01)
                {
                    MainForm.DisplayText(" -- XYA command, XY already there --", KnownColor.Gray);
                }
                else
                {
                    if (SlowA)
                    {
                        command = "G1 F" + SlowSpeedA.ToString(CultureInfo.InvariantCulture) + " A" + Am.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        command = "G0 A" + Am.ToString(CultureInfo.InvariantCulture);
                    }
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
            }
            else
            {
                // normal case, large move
                // Possibilities:
                // SlowA, SlowXY
                // SlowA, FastXY
                // FastA, SlowXY
                // Fast A, Fast XY
                // To avoid side effects, we'll separate a and xy for first three cases
                if (SlowA || (!SlowA && SlowXY))
                {
                    // Do A first, then XY
                    if (dA < 0.01)
                    {
                        MainForm.DisplayText(" -- XYA command, XY already there --", KnownColor.Gray);
                    }
                    else
                    {
                        if (SlowA)
                        {
                            command = "G1 F" + SlowSpeedA.ToString(CultureInfo.InvariantCulture) + " A" + Am.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            command = "G0 A" + Am.ToString(CultureInfo.InvariantCulture);
                        }
                        _readyEvent.Reset();
                        Com.Write("{\"gc\":\"" + command + "\"}");
                        _readyEvent.Wait();
                    }
                    // A done, we know XY is slow and large
                    command = "G1 F" + SlowSpeedXY.ToString(CultureInfo.InvariantCulture) +
                                " X" + X.ToString(CultureInfo.InvariantCulture) +
                                " Y" + Y.ToString(CultureInfo.InvariantCulture);
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
                else
                {
                    // Fast A, Fast XY
                    command = "G0 " + "X" + X.ToString(CultureInfo.InvariantCulture) +
                                     " Y" + Y.ToString(CultureInfo.InvariantCulture) +
                                     " A" + Am.ToString(CultureInfo.InvariantCulture);
                    _readyEvent.Reset();
                    Com.Write("{\"gc\":\"" + command + "\"}");
                    _readyEvent.Wait();
                }
            }
        }


        public void Z(double Z)
        {
            string command = "G0 Z" + Z.ToString(CultureInfo.InvariantCulture);
            double dZ = Math.Abs(Z - CurrentZ);
            if (dZ < 0.005)
            {
                MainForm.DisplayText(" -- zero Z movement command --");
                MainForm.DisplayText("ReadyEvent: zero movement command");
                _readyEvent.Set();
                return;   // already there
            }
            if (dZ < 1.1)
            {
                if (SlowZ)
                {
                    if ((double)MainForm.Setting.CNC_SmallMovementSpeed > SlowSpeedZ)
                    {
                        command = "G1 F" + SlowSpeedZ.ToString(CultureInfo.InvariantCulture) + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        command = "G1 F" + MainForm.Setting.CNC_SmallMovementSpeed.ToString(CultureInfo.InvariantCulture) + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
            else
            {
                if (SlowZ)
                {
                    command = "G1 F" + SlowSpeedZ.ToString(CultureInfo.InvariantCulture) + " Z" + Z.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    command = "G0 Z" + Z.ToString(CultureInfo.InvariantCulture);
                }
            }
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        public void A(double A)
        {
            if (Math.Abs(A - CurrentA) < 0.01)
            {
                MainForm.DisplayText(" -- zero A movement command --");
                _readyEvent.Set();
                return;   // already there
            }
            if ((SlackCompensationA) && (CurrentA > (A - SlackCompensationDistanceA)))
            {
                A_move(A - SlackCompensationDistanceA);
                A_move(A);
            }
            else
            {
                A_move(A);
            }
        }
        private void A_move(double A)
        {
            string command;
            if (SlowA)
            {
                command = "G1 F" + SlowSpeedA.ToString(CultureInfo.InvariantCulture) + " A" + A.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                command = "G0 A" + A.ToString(CultureInfo.InvariantCulture);
            }
            _readyEvent.Reset();
            Com.Write("{\"gc\":\"" + command + "\"}");
            _readyEvent.Wait();
        }

        // =================================================================================

        // =================================================================================
        public void CancelJog()
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.CancelJog();
            }
            else if (Controlboard == ControlBoardType.TinygHW)
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
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.Jog(Speed, X, Y, Z, A);
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.Jog(Speed, X, Y, Z, A);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.Jog(), unknown board.", KnownColor.DarkRed, true);
            }

        }
        // =================================================================================
        public void MotorPowerOn()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.MotorPowerOn();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.MotorPowerOn();
            }
            else
            {
                MainForm.DisplayText("*** MotorPowerOn(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        public void MotorPowerOff()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.MotorPowerOff();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.MotorPowerOff();
            }
            else
            {
                MainForm.DisplayText("*** MotorPowerOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }


        // =========================================================================================
        // vacuum
        public void VacuumOn()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.VacuumOn();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.VacuumOn();
            }
            else
            {
                MainForm.DisplayText("*** VacuumOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        public void VacuumOff()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.VacuumOff();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.VacuumOff();
            }
            else
            {
                MainForm.DisplayText("*** VacuumOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        // =========================================================================================
        // Pump
        public void PumpOn()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.PumpOn();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.PumpOn();
            }
            else
            {
                MainForm.DisplayText("*** PumpOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        public void PumpOff()
        {
            if (Controlboard == CNC.ControlBoardType.Duet3HW)
            {
                Duet3.PumpOff();
            }
            else if (Controlboard == CNC.ControlBoardType.TinygHW)
            {
                TinyG.PumpOff();
            }
            else
            {
                MainForm.DisplayText("*** PumpOff(), unknown board.", KnownColor.DarkRed, true);
            }
        }





        // =========================================================================================
        // Communication
        public ManualResetEventSlim ReadyEvent
        {
            get
            {
                return _readyEvent;
            }
        }

        public bool Connected { get; set; }
        public bool ErrorState { get; set; }


        public void Close()
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.Close();
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.Close();
            }
            else
            {
                MainForm.DisplayText("*** Cnc.Close(), unknown board.", KnownColor.DarkRed, true);
            }
        }

        // =================================================================================
        public bool Connect(String name)
        {
            if (Duet3.Connect(name))
            {
                Controlboard = ControlBoardType.Duet3HW;
                return true;
            }

            if (TinyG.Connect(name))
            {
                Controlboard = ControlBoardType.TinygHW;
                return true;
            }

            MainForm.DisplayText("*** Cnc.Connect(), did not find a supported board.", KnownColor.DarkRed, true);
            return false;
        }

        public bool JustConnected()
        {
            // Called after a control board conenction is estabished.

            // Set sleep time according to slowest board supported.
            // TinyG: 200 (ms)
            // Duet 3: ??
            Thread.Sleep(200);
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                return Duet3.JustConnected();
            }

            if (Controlboard == ControlBoardType.TinygHW)
            {
                return TinyG.JustConnected();
            }
            MainForm.DisplayText("**Unknown/unsupported control board.");
            return false;
        }

        // =================================================================================
        // RawWrite: Attempt write, regardless of error status

        public void RawWrite(string str)
        {
            if (Controlboard == ControlBoardType.Duet3HW)
            {
                Duet3.RawWrite(str);
            }
            else if (Controlboard == ControlBoardType.TinygHW)
            {
                TinyG.RawWrite(str);
            }
            else
            {
                MainForm.DisplayText("*** Cnc.RawWrite(" + str + "), unknown board.", KnownColor.DarkRed, true);
            }
        }


    }  // end Class CNC
}
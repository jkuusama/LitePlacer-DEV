using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
//using System.Web.Script.Serialization;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AForge.Imaging;
using System.Windows.Media;
using MathNet.Numerics;
using HomographyEstimation;

using System.Text.RegularExpressions;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu;


namespace LitePlacer{
    public partial class FormMain : Form {
        public double _z_offset = 0;  // this is how far from zero the z-head should be to speed-up movements
        public double z_offset {
            get { return _z_offset; }
            set {
                if (value < 0) value = 0; 
                if (value > 20) {
                    ShowSimpleMessageBox("Attempted to set z_offset > 20mm - too dangerous, setting to 20mm");
                    value = 20;
                }
                // adjust where we are if a new value was entered and we were at the old position
                if (_z_offset != value && Cnc.CurrentZ == _z_offset) CNC_Z_m(value);
                _z_offset = value;
            }
        }
 


        private void CNC_Park() {
            CNC_Z_m(0);
            CNC_XY_m(GeneralParkLocation);
        }

        private bool CNC_Home_m(string axis) {
            Cnc.Homing = true; //one shot
            if (!CNC_Write_m("{\"gc\":\"G28.2 " + axis + "0\"}")) {
                ShowSimpleMessageBox("Homing operation mechanical step failed, CNC issue");
                return false;
            }
            DisplayText("Homing " + axis + " done.", Color.DarkSeaGreen);
            return true;
        }

        // =================================================================================
        // CNC_Write_m
        // Sends a command to CNC, doesn't return until the response is handled
        // by the CNC class. (See _readyEvent )
        // =================================================================================
        private const int CNC_MoveTimeout = 3000; // timeout for X,Y,Z,A movements; 2x ms. (3000= 6s timeout)

        private void CNC_RawWrite(string s) {
            // This for operations that cause conflicts with event firings. Caller does waiting, if needed.
            Cnc.RawWrite(s);
        }

        bool CNC_BlockingWriteDone = false;
        bool CNC_WriteOk = true;
        private void CNC_BlockingWrite_thread(string cmd) {
            Cnc_ReadyEvent.Reset();
            CNC_WriteOk = Cnc.Write(cmd);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_Write_m(string s, int Timeout = 250) {
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingWrite_thread(s));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            if (Cnc.Homing) {
                Timeout = 10000;
                Cnc.Homing = false;
            };
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout) {
                    Cnc_ReadyEvent.Set();  // terminates the CNC_BlockingWrite_thread
                    ShowMessageBox(
                        "Debug: CNC_BlockingWrite: Timeout on command " + s,
                        "Timeout",
                        MessageBoxButtons.OK);
                    CNC_BlockingWriteDone = true;
                    JoggingBusy = false;

                    return false;
                }
            }
            return (CNC_WriteOk);
        }

        private bool CNC_MoveIsSafe_m(PartLocation p) {
            if ((p.X < -3.0) || (p.X > setting.General_MachineSizeX) || (p.Y < -3.0) || (p.Y > setting.General_MachineSizeY)) {
                ShowSimpleMessageBox("Attempt to move outside safe limits "+p);
                return false;
            }
            if (CNC_NeedleIsDown_m()) {
                ZGuardOn();
                CNC_Z_m(0);
            }
            return true;
        }



        private bool _Zguard = true;
        private void ZGuardOn() {
            _Zguard = true;
        }
        private void ZGuardOff() {
            _Zguard = false;
        }

        private bool CNC_NeedleIsDown_m() {
            if ((Cnc.CurrentZ > z_offset+1) && _Zguard) {
                DisplayText("Needle down error.");
               /* ShowMessageBox(
                   "Attempt to move while needle is down.",
                   "Danger to Needle",
                   MessageBoxButtons.OK);*/
                return true;
            }
            return false;
        }

        private void CNC_BlockingXY_thread(double X, double Y) {
            Cnc_ReadyEvent.Reset();
            Cnc.XY(X, Y);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        private void CNC_BlockingXYA_thread(double X, double Y, double A) {
            Cnc_ReadyEvent.Reset();
            Cnc.XYA(X, Y, A);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_XYA_m(double X, double Y, double A) { return CNC_XY_m(new PartLocation(X, Y, A), true); }
        public bool CNC_XY_m(double X, double Y) { return CNC_XY_m(new PartLocation(X, Y), false); }
        public bool CNC_XY_m(PartLocation loc) { return CNC_XY_m(loc, false); }
        public bool CNC_XYA_m(PartLocation loc) { return CNC_XY_m(loc, true); }

        public bool CNC_XY_m(PartLocation loc, bool MoveAngle) {
            if (MoveAngle) DisplayText("CNC_XYA_m, x: " + loc);
            else DisplayText("CNC_XY_m, x: " + loc);

            if (AbortPlacement) {
                AbortPlacement = false;  // one shot
                ShowSimpleMessageBox("Operation aborted");
                return false;
            }

            if (!CNC_MoveIsSafe_m(loc)) return false;

            if (!Cnc.Connected) {
                ShowSimpleMessageBox("CNC_XY: Cnc not connected");
                return false;
            }

            CNC_BlockingWriteDone = false;
            Thread t;
            if (MoveAngle) {
                t = new Thread(() => CNC_BlockingXYA_thread(loc.X, loc.Y, loc.A));
            } else {
                t = new Thread(() => CNC_BlockingXY_thread(loc.X, loc.Y));
            }
            t.IsBackground = true;
            t.Start();
            int i = 0;

            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Cnc.Connected) {
                ShowMessageBox(
                           "CNC_XY: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                Cnc.Close();
                UpdateCncConnectionStatus();
            }
            DisplayText("CNC_XY_m ok");
            return (Cnc.Connected);
        }


        private void CNC_BlockingZ_thread(double Z) {
            Cnc_ReadyEvent.Reset();
            Cnc.Z(Z);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_Z_m(double Z) {
            if (Z == 0) Z = z_offset; //consider this height = zero

            if (AbortPlacement) {
                AbortPlacement = false;  // one shot
                ShowSimpleMessageBox("Operation aborted");
                return false;
            }

            if (!Cnc.Connected) {
                ShowSimpleMessageBox("CNC_XY: Cnc not connected");
                return false;
            }

            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingZ_thread(Z));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }
            if ((i > CNC_MoveTimeout) || !Cnc.Connected) {
                ShowSimpleMessageBox("CNC_Z: Timeout / Cnc connection cut!");
                Cnc.Close();
            }
            return (Cnc.Connected);
        }

        private void CNC_BlockingA_thread(double A) {
            if (Properties.Settings.Default.CNC_SlackCompensation) {
                Cnc_ReadyEvent.Reset();
                Cnc.A(A - 10); //this is slack compensation for the angle
                Cnc_ReadyEvent.Wait();
            }
            Cnc_ReadyEvent.Reset();
            Cnc.A(A);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_A_m(double A) {
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingA_thread(A));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            if (!Cnc.Connected) {
                ShowMessageBox(
                    "CNC_A: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }
            while (!CNC_BlockingWriteDone) {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout) {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Cnc.Connected) {
                ShowMessageBox(
                           "CNC_A: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                Cnc.Close();
                UpdateCncConnectionStatus();
            }
            return (Cnc.Connected);
        }


    }
}

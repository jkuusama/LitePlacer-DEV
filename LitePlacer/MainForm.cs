#define TINYG_SHORTUNITS
// Some firmvare versions use units in millions, some don't. If not, comment out the above line.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Drawing;
using System.Reflection;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Timers;


namespace LitePlacer {

    public partial class FormMain : Form {
        public CNC Cnc;
        public CameraView cameraView;
        NeedleClass Needle;
        TapesClass Tapes;
        CAD Cad;


        public bool JoggingBusy {
            get { return Cnc.JoggingBusy; }
            set { Cnc.JoggingBusy = value; }
        }

        public bool AbortPlacement {
            get { return Cnc.AbortPlacement; }
            set { Cnc.AbortPlacement = value; }
        }

        // =================================================================================
        // General and "global" functions 
        // =================================================================================
        #region General

        // Note about thread guards: The prologue "if(InvokeRequired) {something long}" at a start of a function, 
        // makes the function safe to call from another thread.
        // See http://stackoverflow.com/questions/661561/how-to-update-the-gui-from-another-thread-in-c, 
        // "MajesticRa"'s answer near the bottom of first page



        #region MessageDialogs
        // =================================================================================
        // Thread safe dialog box:
        // (see http://stackoverflow.com/questions/559252/does-messagebox-show-automatically-marshall-to-the-ui-thread )

        public DialogResult ShowMessageBox(String message, String header, MessageBoxButtons buttons) {
            if (InvokeRequired) {
                return (DialogResult)Invoke(new PassStringStringReturnDialogResultDelegate(ShowMessageBox), message, header, buttons);
            }
            if (buttons.Equals(MessageBoxButtons.OK)) {
                return ShowSimpleMessageBox(message);
            }
            return MessageBox.Show(this, message, header, buttons);
        }
        public delegate DialogResult PassStringStringReturnDialogResultDelegate(String s1, String s2, MessageBoxButtons buttons);



        // This block of code will queue error messages that are sent within
        // mTimer.Interval and show them all at once - as a simpler way of handling
        // multiple error messages vs. the _m() method -- lets you be a lazier coder
        // without annoying the user.  Also provides a sort of 'error stack trace' 
        // which is useful in debugging
        List<string> errorMessages = new List<string>();
        public System.Timers.Timer mtimer;
   
        public void setupQueuedMessages() { 
            mtimer = new System.Timers.Timer();
            mtimer.Elapsed += DisplayQueuedMessages;
            mtimer.Interval = 500;
        }

        public void DisplayQueuedMessages(object source, ElapsedEventArgs args){
            mtimer.Enabled = false;
            ShowSimpleMessageBox(null);
        }

        public DialogResult ShowSimpleMessageBox(String message) {
            if (InvokeRequired) {
                return (DialogResult)Invoke(new PassStringDelegate(ShowSimpleMessageBox), message);
            }
            //null message to terminate
            if (message==null)  {
                errorMessages.Reverse();
                string msg = String.Join("\n",errorMessages);
                errorMessages.Clear();
                if (!IsDisposed)
                    return MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK);
            }
            errorMessages.Add(message);
            mtimer.Stop();
            mtimer.Enabled = true;
            mtimer.Start();
            return DialogResult.OK;
        }
        public delegate DialogResult PassStringDelegate(String s1);
        #endregion

        // =================================================================================


        // We need some functions both in JSON and in text mode:
        public const bool JSON = true;
        public const bool TextMode = false;



        // This event is raised in the CNC class, and we'll wait for it when we want to continue only after TinyG has stabilized

        public FormMain() {
            InitializeComponent();
            Global.Instance.mainForm = this;
            setupQueuedMessages();
        }

        private void LightPlacerFormsSetup() {
            cameraView.SetDownCameraDefaults();
            UpdateCncConnectionStatus();
            Z0toPCB_BasicTab_label.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture);
            Z_Backoff_label.Text = Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture);
            SizeXMax_textBox.Text = Properties.Settings.Default.General_MachineSizeX.ToString();
            SizeYMax_textBox.Text = Properties.Settings.Default.General_MachineSizeY.ToString();

            ParkLocationX_textBox.Text = Global.GeneralParkLocation.X.ToString();
            ParkLocationY_textBox.Text = Global.GeneralParkLocation.Y.ToString();
            SquareCorrection_textBox.Text = Properties.Settings.Default.CNC_SquareCorrection.ToString();
            VacuumTime_textBox.Text = Properties.Settings.Default.General_PickupVacuumTime.ToString();
            VacuumRelease_textBox.Text = Properties.Settings.Default.General_PickupReleaseTime.ToString();
            SmallMovement_numericUpDown.Value = Properties.Settings.Default.CNC_SmallMovementSpeed;


            // Does this machine have any ports? (Maybe not, if TinyG is powered down.)
            RefreshPortList();
            if (comboBoxSerialPorts.Items.Count == 0) {
                return;
            };

            // At least there are some ports. Show the default port, if it is still there:
            bool found = false;
            int i = 0;
            foreach (var item in comboBoxSerialPorts.Items) {
                if (item.ToString() == Properties.Settings.Default.CNC_SerialPort) {
                    found = true;
                    comboBoxSerialPorts.SelectedIndex = i;
                    break;
                }
                i++;
            }
            if (found) {
                // Yes, the default port is still there, show it
                comboBoxSerialPorts.SelectedIndex = i;
            } else {
                // show the first available port
                comboBoxSerialPorts.SelectedIndex = 0;
                return;
            }

            //--------------------------------------------
            NeedleOffset_label.Visible = false;

            double f;
            f = Properties.Settings.Default.DownCam_XmmPerPixel * cameraView.downVideoProcessing.box.Width;
            DownCameraBoxX_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            DownCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            f = Properties.Settings.Default.DownCam_YmmPerPixel * cameraView.downVideoProcessing.box.Height;
            DownCameraBoxY_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            DownCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";

            f = Properties.Settings.Default.UpCam_XmmPerPixel * cameraView.upVideoProcessing.box.Width;
            UpCameraBoxX_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            UpCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_XmmPerPixel.ToString("0.000", CultureInfo.InvariantCulture) + "mm/pixel)";
            f = Properties.Settings.Default.UpCam_YmmPerPixel * cameraView.upVideoProcessing.box.Height;
            UpCameraBoxY_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            UpCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_YmmPerPixel.ToString("0.000", CultureInfo.InvariantCulture) + "mm/pixel)";

            // Threshold_numericUpDown.Value = Properties.Settings.Default.Downcam_OpticalHomingThreshold;
            JigX_textBox.Text = Properties.Settings.Default.General_JigOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
            JigY_textBox.Text = Properties.Settings.Default.General_JigOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
            PickupCenterX_textBox.Text = Properties.Settings.Default.General_PickupCenterX.ToString("0.00", CultureInfo.InvariantCulture);
            PickupCenterY_textBox.Text = Properties.Settings.Default.General_PickupCenterY.ToString("0.00", CultureInfo.InvariantCulture);
            NeedleOffsetX_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
            NeedleOffsetY_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
            // Z0toPCB_CamerasTab_label.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture) + " mm";

            UpcamPositionX_textBox.Text = Properties.Settings.Default.UpCam_PositionX.ToString("0.00", CultureInfo.InvariantCulture);
            UpcamPositionY_textBox.Text = Properties.Settings.Default.UpCam_PositionY.ToString("0.00", CultureInfo.InvariantCulture);

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
            if (Cnc.Connected) {
                PumpDefaultSetting();
                VacuumDefaultSetting();
                Cnc.CNC_Write_m("{\"md\":\"\"}");  // motor power off
                Cnc.Close();
            }
            Properties.Settings.Default.Save();
            Tapes.SaveAll();
            cameraView.Shutdown();
        }

        // =================================================================================
        // Get and save settings from old version if necessary
        // http://blog.johnsworkshop.net/automatically-upgrading-user-settings-after-an-application-version-change/

        private void Do_Upgrade() {
            try {
                if (Properties.Settings.Default.General_UpgradeRequired) {
                    DisplayText("Updating from previous version");
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.General_UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
            } catch (SettingsPropertyNotFoundException) {
                DisplayText("Updating from previous version (through ex)");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.General_UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

        }
        // =================================================================================

        private void Form1_Load(object sender, EventArgs e) {
            Size = new Size(1280, 960);
            DisplayText("Application Start");

            // show the camera view stuff right away
            cameraView = new CameraView();
            cameraView.Show();

            Do_Upgrade(); 

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Cnc = new CNC(this);

            CNC.SquareCorrection = Properties.Settings.Default.CNC_SquareCorrection;

            Needle = new NeedleClass(this);
            Tapes = new TapesClass(Tapes_dataGridView, Needle, Cnc, this);
            Cad = new CAD(this);

            Global.Instance.cnc = Cnc;
            Global.Instance.needle = Needle;

            //setup table bindings
            JobData_GridView.DataSource = Cad.JobData;
            CadData_GridView.DataSource = Cad.ComponentData;
            Tapes_dataGridView.DataSource = Tapes.tapeObjs;
            OriginalPartOrientation.DataSource = Enum.GetNames(typeof(Orientation));
            OriginalTapeOrientation.DataSource = Enum.GetNames(typeof(Orientation));
            Type.DataSource = new BindingSource { DataSource = Tapes.TapeTypes };
            PartType.DataSource = Enum.GetNames(typeof(ComponentType));           
            

            KeyPreview = true;
            RemoveCursorNavigation(Controls);
            //this.KeyDown += new KeyEventHandler(My_KeyDown);
            KeyUp += My_KeyUp;

            cameraView.SetDownCameraDefaults();
            cameraView.SetUpCameraDefaults();

            Zlb_label.Text = "";
            Zlb_label.Visible = false;

            // setup a bunch of stuff on the forms
            LightPlacerFormsSetup();

            zoffset_textbox.Text = Properties.Settings.Default.z_offset.ToString();
            // template based fudical locating RN
            cb_useTemplate.Checked = Properties.Settings.Default.use_template;
            fiducialTemlateMatch_textBox.Text = Properties.Settings.Default.template_threshold.ToString();
            fiducial_designator_regexp_textBox.Text = Properties.Settings.Default.fiducial_designator_regexp;
        }

        private void ShowBuildNumber() {
            // see http://stackoverflow.com/questions/1600962/displaying-the-build-date

            var version = Assembly.GetEntryAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)
            DisplayText("Version: " + version + ", build date: " + buildDateTime);
        }

        private void FormMain_Shown(object sender, EventArgs e) {
            LabelTestButtons();
            ShowBuildNumber();

            tabControlPages.SelectedTab = tabPageBasicSetup;

            Cnc.SlackCompensation = Properties.Settings.Default.CNC_SlackCompensation;
            SlackCompensation_checkBox.Checked = Properties.Settings.Default.CNC_SlackCompensation;
            Cnc.SmallMovementString = "G1 F" + Properties.Settings.Default.CNC_SmallMovementSpeed + " ";

            ZTestTravel_textBox.Text = Properties.Settings.Default.General_ZTestTravel.ToString();

            UpdateCncConnectionStatus();
            if (Cnc.Connected) {
                Thread.Sleep(200); // Give TinyG time to wake up
                Cnc.CNC_RawWrite("\x11");  // Xon
                Thread.Sleep(50);
                UpdateWindowValues_m();
            }

        }




        // =================================================================================
        // Forcing a DataGridview display update
        // Ugly hack if you ask me, but MS didn't give us any other reliable way...
        private void Update_GridView(DataGridView Grid) {
            Grid.Invalidate();
            Grid.Update();
        }

        #endregion

        // =================================================================================
        // Jogging
        // =================================================================================
        #region Jogging

        // see https://github.com/synthetos/TinyG/wiki/TinyG-Feedhold-and-Resume


        List<Keys> JoggingKeys = new List<Keys>
        {
	        Keys.Up,
	        Keys.Down,
	        Keys.Left,
            Keys.Right,
            Keys.F5,
            Keys.F6,
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12
	    };

        // To make sure we get to see all keydown events:
        private void RemoveCursorNavigation(System.Windows.Forms.Control.ControlCollection controls) {
            foreach (System.Windows.Forms.Control ctrl in controls) {
                ctrl.PreviewKeyDown += My_KeyDown;
                RemoveCursorNavigation(ctrl.Controls);
            }
        }


        public void My_KeyUp(object sender, KeyEventArgs e) {
            if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right)) {
                JoggingBusy = false;
                //DisplayText("arrow key up");
                e.Handled = true;
                e.SuppressKeyPress = true;
                Cnc.RawWrite("!%");
            }
        }

        public void My_KeyDown(object sender, PreviewKeyDownEventArgs e) {
            //DisplayText("My_KeyDown: " + e.KeyCode.ToString());
            if (!JoggingKeys.Contains(e.KeyCode)) {
                return;
            }

            e.IsInputKey = true;
            string Movestr = "{\"gc\":\"G1 F2000 ";

            if (JoggingBusy) {
                return;
            }

            if (!Cnc.Connected) {
                return;
            }

            if (e.KeyCode == Keys.Up) {
                JoggingBusy = true;
                //DisplayText("up");
                //return;
                Cnc.RawWrite(Movestr + "Y" + Properties.Settings.Default.General_MachineSizeY + "\"}");
            } else if (e.KeyCode == Keys.Down) {
                JoggingBusy = true;
                //DisplayText("down");
                //return;
                Cnc.RawWrite(Movestr + "Y0\"}");
            } else if (e.KeyCode == Keys.Left) {
                JoggingBusy = true;
                //DisplayText("left");
                //return;
                Cnc.RawWrite(Movestr + "X0\"}");
            } else if (e.KeyCode == Keys.Right) {
                JoggingBusy = true;
                //DisplayText("right");
                //return;
                Cnc.RawWrite(Movestr + "X" + Properties.Settings.Default.General_MachineSizeX + "\"}");
            }
            Jog(sender, e);
        }


        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void Jog(object sender, PreviewKeyDownEventArgs e) {
            if (JoggingBusy) {
                return;
            }

            if (!Cnc.Connected) {
                return;
            }

            double Mag = 0.0;
            if ((e.Alt) && (e.Shift)) {
                Mag = 100.0;
            } else if ((e.Alt) && (e.Control)) {
                Mag = 4.0;
            } else if (e.Alt) {
                Mag = 10.0;
            } else if (e.Shift) {
                Mag = 1.0;
            } else if (e.Control) {
                Mag = 0.01;
            } else {
                Mag = 0.1;
            };

            // move right
            if (e.KeyCode == Keys.F5) {
                JoggingBusy = true;
                Cnc.CNC_XY_m(Cnc.CurrentX - Mag, Cnc.CurrentY);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move left
            if (e.KeyCode == Keys.F6) {
                JoggingBusy = true;
                Cnc.CNC_XY_m(Cnc.CurrentX + Mag, Cnc.CurrentY);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move away
            if (e.KeyCode == Keys.F7) {
                JoggingBusy = true;
                Cnc.CNC_XY_m(Cnc.CurrentX, Cnc.CurrentY + Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move closer
            if (e.KeyCode == Keys.F8) {
                JoggingBusy = true;
                Cnc.CNC_XY_m(Cnc.CurrentX, Cnc.CurrentY - Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            };

            // rotate ccw
            if (e.KeyCode == Keys.F9) {
                JoggingBusy = true;
                if ((Mag > 99) && (Mag < 101)) {
                    Mag = 90.0;
                }
                Cnc.CNC_A_m(Cnc.CurrentA + Mag);

                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // rotate cw
            if (e.KeyCode == Keys.F10) {
                JoggingBusy = true;
                if ((Mag > 99) && (Mag < 101)) {
                    Mag = 90.0;
                }
                Cnc.CNC_A_m(Cnc.CurrentA - Mag);

                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move up
            if (e.KeyCode == Keys.F11) {
                JoggingBusy = true;
                Cnc.CNC_Z_m(Cnc.CurrentZ - Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move down
            if ((e.KeyCode == Keys.F12) && (Mag < 50)) {
                JoggingBusy = true;
                Cnc.CNC_Z_m(Cnc.CurrentZ + Mag);
                JoggingBusy = false;
                e.IsInputKey = true;
            }

        }

        #endregion

        // =================================================================================
        // CNC interface functions
        // =================================================================================
        #region CNC interface functions

        private bool VacuumIsOn;

        private void VacuumDefaultSetting() {
            VacuumOff();
        }

        private void VacuumOn() {
            if (!VacuumIsOn) {
                DisplayText("VacuumOn()");
                Cnc.CNC_RawWrite("{\"gc\":\"M08\"}");
                VacuumIsOn = true;
                Vacuum_checkBox.Checked = true;
                Thread.Sleep(Properties.Settings.Default.General_PickupVacuumTime);
            }
        }

        private void VacuumOff() {
            if (VacuumIsOn) {
                DisplayText("VacuumOff()");
                Cnc.CNC_RawWrite("{\"gc\":\"M09\"}");
                VacuumIsOn = false;
                Vacuum_checkBox.Checked = false;
                Thread.Sleep(Properties.Settings.Default.General_PickupReleaseTime);
            }
        }

        private bool PumpIsOn;
        private void PumpDefaultSetting() {
            PumpOff();
        }

        private void BugWorkaround() {
            // see https://www.synthetos.com/topics/file-not-open-error/#post-7194
            // Summary: In some cases, we need a dummy move.
            bool slackSave = Cnc.SlackCompensation;
            Cnc.SlackCompensation = false;
            Cnc.CNC_XY_m(Cnc.CurrentX - 0.5, Cnc.CurrentY - 0.5);
            Cnc.CNC_XY_m(Cnc.CurrentX + 0.5, Cnc.CurrentY + 0.5);
            Cnc.SlackCompensation = slackSave;
        }

        private void PumpOn() {
            if (!PumpIsOn) {
                //Cnc.CNC_RawWrite("M03");
                Cnc.CNC_RawWrite("{\"gc\":\"M03\"}");
                Pump_checkBox.Checked = true;
                Thread.Sleep(500);  // this much to develop vacuum
                BugWorkaround();
                PumpIsOn = true;
            }
        }

        private void PumpOff() {
            if (PumpIsOn) {
                //CNC_RawWrite("M05");
                Cnc.CNC_RawWrite("{\"gc\":\"M05\"}");
                Thread.Sleep(50);
                BugWorkaround();
                Pump_checkBox.Checked = false;
                PumpIsOn = false;
            }
        }

        private bool Needle_ProbeDown_m() {
            Needle.ProbingMode(true, JSON);
            //CNC_Write_m("G28.4 Z0", 4000);
            if (!Cnc.CNC_Write_m("{\"gc\":\"G28.4 Z0\"}", 10000)) {
                Needle.ProbingMode(false, JSON);
                return false;
            }
            Needle.ProbingMode(false, JSON);
            return true;
        }

        // moved logic from this function into Needle.Calibrate
        private bool CalibrateNeedle_m() {
            if (Needle.Calibrate(4.0 / Properties.Settings.Default.UpCam_XmmPerPixel)) {
                for (int i = 0; i < Needle.CalibrationPoints.Count; i++)
                    DisplayText(Needle.CalibrationPoints[i].ToPartLocation().ToString(), Color.Purple);
                Needle.Calibrated = true;
                return true;
            }
            ShowSimpleMessageBox("Needle Calibration Failed");
            Needle.Calibrated = false;
            return false;
        }


        /// <summary>
        /// Will return the actual location of the part
        /// </summary>
        public PartLocation FindPositionOfClosest(Shapes.ShapeTypes type, double FindTolerance, double MoveTolerance) {
            double X, Y;
            if (!GoToLocation_m(type, FindTolerance, MoveTolerance, out X, out Y)) return null;
            return Cnc.XYLocation + new PartLocation(X, Y);
        }


        // =====================================================================
        // This routine finds an accurate location of a shpe that downcamera is looking at.
        // Used in homing and locating fiducials.
        // Tolerances in mm; find: how far from center to accept a circle, move: how close to go (set small to ensure view from straight up)
        // At return, the camera is located on top of the circle.
        // X and Y are set to remainding error (true position: currect + error)
        // =====================================================================
        public bool GoToLocation_m(Shapes.ShapeTypes type, double FindTolerance, double MoveTolerance, out double X, out double Y) {
            return GoToLocation_m(cameraView.downVideoProcessing, type, FindTolerance, MoveTolerance, out X, out Y);
        }


        public bool GoToLocation_m(VideoProcessing vp, Shapes.ShapeTypes type, double FindTolerance, double MoveTolerance, out double X, out double Y) {
            DisplayText("GoToLocation_m(" + type + "), FindTolerance: " + FindTolerance + ", MoveTolerance: " + MoveTolerance, Color.Orange);

            X = 0; Y = 0;

            //move up to 8 times
            PartLocation location = null;
            for (int i = 0; i < 8; i++) {
                var thing = VideoDetection.FindClosest(vp, type, FindTolerance, 8);

                if (thing == null) {
                    ShowSimpleMessageBox("Optical positioning: Can't find " + type);
                    return false;
                }

                thing.ToMMResolution();
                DisplayText("Optical positioning, round " + i + ", offset= " + thing + "  dist/tol = " + thing.ToPartLocation().VectorLength() + "/" + FindTolerance);
                Console.WriteLine("Optical positioning, round " + i + ", offset= " + thing + "  dist/tol = " + thing.ToPartLocation().VectorLength() + "/" + FindTolerance);

                // If we are further than move tolerance, go there, else end loop
                if (thing.ToPartLocation().VectorLength() > MoveTolerance) {
                    Console.WriteLine("\tmoving " + thing.ToPartLocation().VectorLength() + " > " + MoveTolerance);
                    Cnc.CNC_XY_m(Cnc.XYLocation + thing.ToPartLocation());
                } else {
                    location = thing.ToPartLocation();
                    break;
                }
            }


            if (location == null) {
                ShowSimpleMessageBox("Optical positioning: Process is unstable, result is unreliable!");
                return false;
            }

            X = location.X;
            Y = location.Y;
            return true;
        }




        private bool OpticalHoming_m() {
            DisplayText("Optical homing");
            cameraView.SetDownCameraFunctionSet("homing");
            double X;
            double Y;
            // Find within 20mm, goto within 0.05
            if (!GoToLocation_m(Shapes.ShapeTypes.Circle, 20.0, 0.05, out X, out Y)) {
                return false;
            }
            X = -X;
            Y = -Y;
            // Cnc.CNC_RawWrite("G28.3 X" + X.ToString("0.000") + " Y" + Y.ToString("0.000"));
            Cnc.CNC_RawWrite("{\"gc\":\"G28.3 X" + X.ToString("0.000") + " Y" + Y.ToString("0.000") + "\"}");
            Thread.Sleep(50);

            Cnc.CurrentX = X;
            Cnc.CurrentY = Y;
            Update_xpos(X.ToString("F3"));
            Update_ypos(Y.ToString("F3"));

            DisplayText("Optical homing OK.");
            cameraView.SetDownCameraFunctionSet("");
            return true;
        }


        private bool MechanicalHoming_m() {
            Needle.ProbingMode(false, JSON);
            DisplayText("Home Z");
            return Cnc.CNC_Home_m("ZXYA");
        }


        private void OpticalHome_button_Click(object sender, EventArgs e) {
            if (!MechanicalHoming_m()) return;
            OpticalHoming_m();
        }

        #endregion

        // =================================================================================
        // Up/Down camera setup page functions
        // =================================================================================
        #region Camera setup pages functions

        // =================================================================================
        // Common
        // =================================================================================




        // =================================================================================

        // =================================================================================
        /*gooz        private void Cam_pictureBox_MouseClick(object sender, MouseEventArgs e)
                {
                    if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
                    {
                        // Cntrl-click
                        double X = Convert.ToDouble(e.X) / Convert.ToDouble(Cam_pictureBox.Size.Width);
                        X = X * Properties.Settings.Default.General_MachineSizeX;
                        double Y = Convert.ToDouble(Cam_pictureBox.Size.Height - e.Y) / Convert.ToDouble(Cam_pictureBox.Size.Height);
                        Y = Y * Properties.Settings.Default.General_MachineSizeY;
                        Cnc.CNC_XY_m(X, Y);
                    }
                    else if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
                    {
                        PickColor(e.X, e.Y);
                    }
                    else
                    {
                        int X = e.X - Cam_pictureBox.Size.Width / 2;  // X= diff from center
                        int Y = e.Y - Cam_pictureBox.Size.Height / 2;
                        double Xmm, Ymm;
                        if (DownCamera.IsRunning())
                        {
                            Xmm = Convert.ToDouble(X) * Properties.Settings.Default.DownCam_XmmPerPixel;
                            Ymm = Convert.ToDouble(Y) * Properties.Settings.Default.DownCam_YmmPerPixel;
                            if (DownCamera.Zoom)  // if zoomed for display
                            {
                                Xmm = Xmm / DownCamera.ZoomFactor;
                                Ymm = Ymm / DownCamera.ZoomFactor;
                            };
                            Xmm = Xmm / DownCamera.GetDisplayZoom();	// Might also be zoomed for processing
                            Ymm = Ymm / DownCamera.GetDisplayZoom();
                        }
                        else if (UpCamera.IsRunning())
                        {
                            Xmm = Convert.ToDouble(X) * Properties.Settings.Default.UpCam_XmmPerPixel;
                            Ymm = Convert.ToDouble(Y) * Properties.Settings.Default.UpCam_YmmPerPixel;
                            if (UpCamera.Zoom)
                            {
                                Xmm = -Xmm / UpCamera.ZoomFactor;
                                Ymm = -Ymm / UpCamera.ZoomFactor;
                            }
                            Xmm = Xmm / UpCamera.GetDisplayZoom();	// Might also be zoomed for processing
                            Ymm = Ymm / UpCamera.GetDisplayZoom();
                        }
                        else
                        {
                            return;
                        };

                        Cnc.CNC_XY_m(Cnc.CurrentX + Xmm, Cnc.CurrentY - Ymm);
                    }
                }
         */

        // =================================================================================


        // =================================================================================
        //gooz todo
        /*

            */
        // =================================================================================
        // get the devices         



        // =================================================================================

        // =================================================================================
        // DownCam specific functions
        // =================================================================================

        // =================================================================================
        private void GotoPCB0_button_Click(object sender, EventArgs e) {
            Cnc.CNC_XY_m(Properties.Settings.Default.General_JigOffsetX, Properties.Settings.Default.General_JigOffsetY);
        }

        // =================================================================================
        private void SetPCB0_button_Click(object sender, EventArgs e) {
            JigX_textBox.Text = Cnc.CurrentX.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_JigOffsetX = Cnc.CurrentX;
            JigY_textBox.Text = Cnc.CurrentY.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_JigOffsetY = Cnc.CurrentY;
        }


        // =================================================================================
        private void GotoPickupCenter_button_Click(object sender, EventArgs e) {
            Cnc.CNC_XY_m(Properties.Settings.Default.General_PickupCenterX, Properties.Settings.Default.General_PickupCenterY);
        }

        // =================================================================================
        private void SetPickupCenter_button_Click(object sender, EventArgs e) {
            PickupCenterX_textBox.Text = Cnc.CurrentX.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_PickupCenterX = Cnc.CurrentX;
            PickupCenterY_textBox.Text = Cnc.CurrentY.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_PickupCenterY = Cnc.CurrentY;
            PickupCenterX_textBox.ForeColor = Color.Black;
            PickupCenterY_textBox.ForeColor = Color.Black;
        }

        // =================================================================================
        private static int SetNeedleOffset_stage;
        private static double NeedleOffsetMarkX;
        private static double NeedleOffsetMarkY;


        private void Offset2Method_button_Click(object sender, EventArgs e) {
            Cnc.ZGuardOff();
            cameraView.SetDownCameraDefaults();
            switch (SetNeedleOffset_stage) {

                case 0:
                    SetNeedleOffset_stage = 1;
                    Offset2Method_button.Text = "Next";
                    Cnc.CNC_A_m(0.0);
                    NeedleOffset_label.Visible = true;
                    NeedleOffset_label.Text = "Jog needle to a point on a PCB, then click \"Next\"";
                    break;

                case 1:
                    SetNeedleOffset_stage = 2;
                    NeedleOffsetMarkX = Cnc.CurrentX;
                    NeedleOffsetMarkY = Cnc.CurrentY;
                    Cnc.Zup();
                    Cnc.CNC_XY_m(Cnc.CurrentX - 75.0, Cnc.CurrentY - 25.0);
                    cameraView.downVideoProcessing.DrawCross = true;
                    NeedleOffset_label.Text = "Jog camera above the same point, \n\rthen click \"Next\"";
                    break;

                case 2:
                    SetNeedleOffset_stage = 0;
                    Properties.Settings.Default.DownCam_NeedleOffsetX = NeedleOffsetMarkX - Cnc.CurrentX;
                    Properties.Settings.Default.DownCam_NeedleOffsetY = NeedleOffsetMarkY - Cnc.CurrentY;
                    Properties.Settings.Default.Save();
                    NeedleOffsetX_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
                    NeedleOffsetY_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
                    NeedleOffset_label.Visible = false;
                    NeedleOffset_label.Text = "   ";
                    ShowMessageBox(
                        "Now, jog the needle above the up camera,\n\rtake needle down, jog it to the image center\n\rand set Up Camera location",
                        "Done here",
                        MessageBoxButtons.OK);
                    cameraView.upVideoProcessing.SetFunctionsList("needle");
                    Offset2Method_button.Text = "Start";
                    Cnc.Zup();
                    Cnc.ZGuardOn();
                    break;
            }
        }


        /// <summary>
        /// This will set the up cam position taking into account the needle offset (if just calibrated) 
        /// so that if you go to this location, the down camera is exactly here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetUpCamPosition_button_Click(object sender, EventArgs e) {
            UpcamPositionX_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Properties.Settings.Default.UpCam_PositionX = Cnc.CurrentX;
            UpcamPositionY_textBox.Text = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            Properties.Settings.Default.UpCam_PositionY = Cnc.CurrentY;
            DisplayText("True position (with needle offset):");
            DisplayText("X: " + (Cnc.CurrentX - Properties.Settings.Default.DownCam_NeedleOffsetX));
            DisplayText("Y: " + (Cnc.CurrentY - Properties.Settings.Default.DownCam_NeedleOffsetY));
        }


        public void GotoUpCamPosition_button_Click(object sender, EventArgs e) {
            Cnc.CNC_XY_m(Properties.Settings.Default.UpCam_PositionX, Properties.Settings.Default.UpCam_PositionY);
        }


        // =================================================================================

        public void TestNeedleRecognition_button_Click(object sender, EventArgs e) {
            var p = Cnc.XYLocation;
            // setup camera
            cameraView.SetUpCameraDefaults();
            cameraView.upVideoProcessing.FindCircles = true;
            CalibrateNeedle_m();

            //revert to previous state
            Cnc.CNC_XYA_m(p);
            cameraView.SetDownCameraDefaults();
        }

        #endregion  Up/Down Camera setup pages functions

        // =================================================================================
        // Basic setup page functions
        // =================================================================================
        #region Basic setup page functions



        private void BasicSetupTab_End() {
            Cnc.ZGuardOn();
        }

        private void buttonRefreshPortList_Click(object sender, EventArgs e) {
            RefreshPortList();
        }

        private void RefreshPortList() {
            comboBoxSerialPorts.Items.Clear();
            foreach (string s in SerialPort.GetPortNames()) {
                comboBoxSerialPorts.Items.Add(s);
            }
            if (comboBoxSerialPorts.Items.Count == 0) {
                labelSerialPortStatus.Text = "No serial ports found. Is TinyG powered on?";
            } else {
                // show the first available port
                comboBoxSerialPorts.SelectedIndex = 0;
            }
        }


        public void UpdateCncConnectionStatus() {
            if (InvokeRequired) { Invoke(new Action(UpdateCncConnectionStatus)); return; }

            if (Cnc.Connected) {
                buttonConnectSerial.Text = "Reset Conn.";
                labelSerialPortStatus.Text = "Connected";
                labelSerialPortStatus.ForeColor = Color.Black;
            } else {
                buttonConnectSerial.Text = "Connect";
                labelSerialPortStatus.Text = "Not connected";
                labelSerialPortStatus.ForeColor = Color.Red;
            }
        }

        private void buttonConnectSerial_Click(object sender, EventArgs e) {
            if (comboBoxSerialPorts.SelectedItem == null) {
                return;
            };

            if (Cnc.Connected) {
                Cnc.Close();
                Thread.Sleep(250);
            } else {
                if (Cnc.Connect(comboBoxSerialPorts.SelectedItem.ToString())) {
                    Properties.Settings.Default.CNC_SerialPort = comboBoxSerialPorts.SelectedItem.ToString();
                    UpdateWindowValues_m();
                }
            }
            UpdateCncConnectionStatus();
        }



        // TinyG communication monitor textbox  
        public void DisplayText(string txt, Color color) {
            // XXX need to add robust mechanism to only show desired debugging messages
            //if (color.Equals(Color.Gray)) return;
            try {
                if (InvokeRequired) { Invoke(new Action<string, Color>(DisplayText), txt, color); return; }
                txt = txt.Replace("\n", "");
                // TinyG sends \n, textbox needs \r\n. (TinyG could be set to send \n\r, which does not work with textbox.)
                // Adding end of line here saves typing elsewhere
                txt = txt + "\r\n";
                if (SerialMonitor_richTextBox.Text.Length > 1000000) {
                    SerialMonitor_richTextBox.Text = SerialMonitor_richTextBox.Text.Substring(SerialMonitor_richTextBox.Text.Length - 10000);
                }
                SerialMonitor_richTextBox.AppendText(txt, color);
                SerialMonitor_richTextBox.ScrollToCaret();
            } catch {
            }
        }

        public void DisplayText(string txt) {
            DisplayText(txt, SerialMonitor_richTextBox.ForeColor);
        }


        private void textBoxSendtoTinyG_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r') {
                Cnc.RawWrite(textBoxSendtoTinyG.Text);
                textBoxSendtoTinyG.Clear();
            }
        }

        // Sends the calls that will result to messages that update the values shown
        string[] tinyg_commands = new string[] {"sr","xjm","xvm","xsv","xsn","xjh","xsx","1mi","1sa","1tr","yjm",
                "yvm","ysn","ysx","yjh","ysv","2mi","2sa","2tr","zjm","zvm","zsn","zsx","zjh","zsv","3mi","3sa",
                "3tr","ajm","avm","4mi","4sa","4tr"};

        private bool UpdateWindowValues_m() {
            foreach (var c in tinyg_commands) {
                if (!Cnc.CNC_Write_m("{\"" + c + "\":\"\"}")) return false;
            }
            //RN - needed to change angle of rotation of needle to match stuff
            if (!Cnc.CNC_Write_m("{\"4po\":1}")) return false;

            // Do settings that need to be done always
            Cnc.IgnoreError = true;
            Needle.ProbingMode(false, JSON);
            //PumpDefaultSetting();
            //VacuumDefaultSetting();
            //Thread.Sleep(100);
            //Vacuum_checkBox.Checked = true;
            //Cnc.IgnoreError = false;

            // RN
            //Cnc.CNC_Write_m("{\"me\":\"\"}");  // motor power on -  wait till we actually send a command it should power itself on
            //MotorPower_checkBox.Checked = true;
            return true;
        }

        // Called from CNC class when UI need updating
        public void ValueUpdater(string item, string value) {
            if (InvokeRequired) { Invoke(new Action<string, string>(ValueUpdater), item, value); return; }

            switch (item) {
                case "posx": Update_xpos(value);
                    break;
                case "posy": Update_ypos(value);
                    break;
                case "posz": update_field("zpos",value);
                    break;
                case "posa": update_field("apos",value);
                    break;

                case "xjm": Update_xjm(value);
                    break;
                case "yjm": Update_yjm(value);
                    break;
                case "zjm": Update_zjm(value);
                    break;
                case "ajm": Update_ajm(value);
                    break;

                case "xjh": Update_xjh(value);
                    break;
                case "yjh": Update_yjh(value);
                    break;
                case "zjh": Update_zjh(value);
                    break;

                case "xsv": Update_xsv(value);
                    break;
                case "ysv": Update_ysv(value);
                    break;
                case "zsv": Update_zsv(value);
                    break;

                case "xsn": Update_xsn(value);
                    break;
                case "ysn": Update_ysn(value);
                    break;
                case "zsn": Update_zsn(value);
                    break;

                case "xsx": Update_xsx(value);
                    break;
                case "ysx": Update_ysx(value);
                    break;
                case "zsx": Update_zsx(value);
                    break;

                case "xvm": Update_xvm(value);
                    break;
                case "yvm": Update_yvm(value);
                    break;
                case "zvm": Update_zvm(value);
                    break;
                case "avm": Update_avm(value);
                    break;

                case "1mi": Update_1mi(value);
                    break;
                case "2mi": Update_2mi(value);
                    break;
                case "3mi": Update_3mi(value);
                    break;
                case "4mi": Update_4mi(value);
                    break;

                case "1tr": update_field("tr1",value);
                    break;
                case "2tr": update_field("tr2",value);
                    break;
                case "3tr": update_field("tr3",value);
                    break;
                case "4tr": update_field("tr4",value);
                    break;

                case "1sa": Update_1sa(value);
                    break;
                case "2sa": Update_2sa(value);
                    break;
                case "3sa": Update_3sa(value);
                    break;
                case "4sa": Update_4sa(value);
                    break;

                default:
                    break;
            }
        }

        // =========================================================================
        // Thread-safe update functions and value setting fuctions

        // =========================================================================
        #region jm
        // *jm: jerk maximum
        // *jm update
        private void Update_xjm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjm), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_yjm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjm), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_zjm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjm), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_ajm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ajm), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            ajm_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *jm setting
        private void xjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            xjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {

#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            yjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {

#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            zjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {

#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                zjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void ajm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            ajm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {

#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                ajm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        #endregion

        // =========================================================================
        #region jh
        // *jh: jerk homing
        // *jh update

        private void Update_xjh(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjh), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjh_maskedTextBox.Text = val.ToString();
        }

        private void Update_yjh(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjh), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjh_maskedTextBox.Text = val.ToString();
        }

        private void Update_zjh(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjh), value); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjh_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *jh setting

        private void xjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            xjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {

#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjh_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            yjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjh_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            zjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
#if (TINYG_SHORTUNITS)
                Cnc.CNC_Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "}");
#else
                Cnc.CNC_Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                zjh_maskedTextBox.ForeColor = Color.Black;
            }
        }

        #endregion

        // =========================================================================
        #region sv
        // *sv: search velocity
        // * update

        private void Update_xsv(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xsv), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            xsv_maskedTextBox.Text = val.ToString();
        }

        private void Update_ysv(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ysv), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            ysv_maskedTextBox.Text = val.ToString();
        }

        private void Update_zsv(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zsv), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            zsv_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *sv setting

        private void xsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            xsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"xsv\":" + xsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                xsv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void ysv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            ysv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"ysv\":" + ysv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                ysv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            zsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"zsv\":" + zsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zsv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        #endregion

        // =========================================================================
        #region sn
        // *sn: Negative limit switch
        // *sn update

        private void Update_xsn(string value) {
            switch (value) {
                case "0":
                    Xhome_checkBox.Checked = false;
                    Xlim_checkBox.Checked = false;
                    break;
                case "1":
                    Xhome_checkBox.Checked = true;
                    Xlim_checkBox.Checked = false;
                    break;
                case "2":
                    Xhome_checkBox.Checked = false;
                    Xlim_checkBox.Checked = true;
                    break;
                case "3":
                    Xhome_checkBox.Checked = true;
                    Xlim_checkBox.Checked = true;
                    break;
            }
        }

        private void Update_ysn(string value) {
            switch (value) {
                case "0":
                    Yhome_checkBox.Checked = false;
                    Ylim_checkBox.Checked = false;
                    break;
                case "1":
                    Yhome_checkBox.Checked = true;
                    Ylim_checkBox.Checked = false;
                    break;
                case "2":
                    Yhome_checkBox.Checked = false;
                    Ylim_checkBox.Checked = true;
                    break;
                case "3":
                    Yhome_checkBox.Checked = true;
                    Ylim_checkBox.Checked = true;
                    break;
            }
        }

        private void Update_zsn(string value) {
            switch (value) {
                case "0":
                    Zhome_checkBox.Checked = false;
                    Zlim_checkBox.Checked = false;
                    break;
                case "1":
                    Zhome_checkBox.Checked = true;
                    Zlim_checkBox.Checked = false;
                    break;
                case "2":
                    Zhome_checkBox.Checked = false;
                    Zlim_checkBox.Checked = true;
                    break;
                case "3":
                    Zhome_checkBox.Checked = true;
                    Zlim_checkBox.Checked = true;
                    break;
            }
        }

        // =========================================================================
        // *sn setting

        private void Xhome_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"xsn\":" + i + "}");
            Thread.Sleep(50);
        }

        private void Xlim_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"xsn\":" + i + "}");
            Thread.Sleep(50);
        }

        private void Yhome_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"ysn\":" + i + "}");
            Thread.Sleep(50);
        }

        private void Ylim_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"ysn\":" + i + "}");
            Thread.Sleep(50);
        }

        private void Zhome_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"zsn\":" + i + "}");
            Thread.Sleep(50);
        }

        private void Zlim_checkBox_Click(object sender, EventArgs e) {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            Cnc.CNC_Write_m("{\"zsn\":" + i + "}");
            Thread.Sleep(50);
        }

        #endregion

        // =========================================================================
        #region sx
        // *sx: Maximum limit switch
        // *sx update

        private void Update_xsx(string value) {
            if (value == "2") {
                Xmax_checkBox.Checked = true;
            } else {
                Xmax_checkBox.Checked = false;
            }
        }

        private void Update_ysx(string value) {
            if (value == "2") {
                Ymax_checkBox.Checked = true;
            } else {
                Ymax_checkBox.Checked = false;
            }
        }

        private void Update_zsx(string value) {
            if (value == "2") {
                Zmax_checkBox.Checked = true;
            } else {
                Zmax_checkBox.Checked = false;
            }
        }

        // =========================================================================
        // *sx setting

        private void Xmax_checkBox_Click(object sender, EventArgs e) {
            if (Xmax_checkBox.Checked) {
                Cnc.CNC_Write_m("{\"xsx\":2}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"xsx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Ymax_checkBox_Click(object sender, EventArgs e) {
            if (Ymax_checkBox.Checked) {
                Cnc.CNC_Write_m("{\"ysx\":2}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"ysx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Zmax_checkBox_Click(object sender, EventArgs e) {
            if (Zmax_checkBox.Checked) {
                Cnc.CNC_Write_m("{\"zsx\":2}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"zsx\":0}");
                Thread.Sleep(50);
            }
        }

        #endregion

        // =========================================================================
        #region vm
        // *vm: Velocity maximum
        // *vm update

        private void Update_xvm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xvm), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            xvm_maskedTextBox.Text = val.ToString();
        }

        private void Update_yvm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yvm), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            yvm_maskedTextBox.Text = val.ToString();
        }

        private void Update_zvm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zvm), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            zvm_maskedTextBox.Text = val.ToString();
        }


        private void Update_avm(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_avm), value); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0) {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            avm_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *vm setting

        private void xvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            xvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"xvm\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.CNC_Write_m("{\"xfr\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                xvm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            yvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"yvm\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.CNC_Write_m("{\"yfr\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                yvm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            zvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"zvm\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                Cnc.CNC_Write_m("{\"zfr\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zvm_maskedTextBox.ForeColor = Color.Black;
                int peek = Convert.ToInt32(zvm_maskedTextBox.Text);
                Properties.Settings.Default.CNC_ZspeedMax = peek;
            }
        }

        private void avm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            avm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                Cnc.CNC_Write_m("{\"avm\":" + avm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.CNC_Write_m("{\"afr\":" + avm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                avm_maskedTextBox.ForeColor = Color.Black;
                int peek = Convert.ToInt32(avm_maskedTextBox.Text);
                Properties.Settings.Default.CNC_AspeedMax = peek;
            }
        }

        #endregion

        // =========================================================================
        #region mi
        // *mi: microstepping
        // *mi update

        private void Update_1mi(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1mi), value); return; }

            int val = Convert.ToInt32(value);
            mi1_maskedTextBox.Text = val.ToString();
        }

        private void Update_2mi(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2mi), value); return; }

            int val = Convert.ToInt32(value);
            mi2_maskedTextBox.Text = val.ToString();
        }

        private void Update_3mi(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3mi), value); return; }

            int val = Convert.ToInt32(value);
            mi3_maskedTextBox.Text = val.ToString();
        }

        private void Update_4mi(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4mi), value); return; }

            int val = Convert.ToInt32(value);
            mi4_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *mi setting

        private void mi1_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            mi1_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if ((mi1_maskedTextBox.Text == "1") || (mi1_maskedTextBox.Text == "2")
                    || (mi1_maskedTextBox.Text == "4") || (mi1_maskedTextBox.Text == "8")) {
                    Cnc.CNC_Write_m("{\"1mi\":" + mi1_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi1_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private void mi2_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            mi2_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if ((mi2_maskedTextBox.Text == "1") || (mi2_maskedTextBox.Text == "2")
                    || (mi2_maskedTextBox.Text == "4") || (mi2_maskedTextBox.Text == "8")) {
                    Cnc.CNC_Write_m("{\"2mi\":" + mi2_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi2_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }


        private void mi3_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            mi3_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if ((mi3_maskedTextBox.Text == "1") || (mi3_maskedTextBox.Text == "2")
                    || (mi3_maskedTextBox.Text == "4") || (mi3_maskedTextBox.Text == "8")) {
                    Cnc.CNC_Write_m("{\"3mi\":" + mi3_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi3_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private void mi4_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e) {
            mi4_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if ((mi4_maskedTextBox.Text == "1") || (mi4_maskedTextBox.Text == "2")
                    || (mi4_maskedTextBox.Text == "4") || (mi4_maskedTextBox.Text == "8")) {
                    Cnc.CNC_Write_m("{\"4mi\":" + mi4_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi4_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        // =========================================================================
        #region tr



        // =========================================================================
        // *tr setting
        private void tr1_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            double val;
            tr1_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if (double.TryParse(tr1_textBox.Text, out val)) {
                    Cnc.CNC_Write_m("{\"1tr\":" + tr1_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr1_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr2_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            double val;
            tr2_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if (double.TryParse(tr2_textBox.Text, out val)) {
                    Cnc.CNC_Write_m("{\"2tr\":" + tr2_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr2_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr3_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            double val;
            tr3_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if (double.TryParse(tr3_textBox.Text, out val)) {
                    Cnc.CNC_Write_m("{\"3tr\":" + tr3_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr3_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr4_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            double val;
            tr4_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r') {
                if (double.TryParse(tr1_textBox.Text, out val)) {
                    Cnc.CNC_Write_m("{\"4tr\":" + tr4_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr4_textBox.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        // =========================================================================
        #region sa
        // *sa: Step angle, 0.9 or 1.8
        // *sa update

        private void Update_1sa(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1sa), value); return; }

            if ((value == "0.90") || (value == "0.900")) {
                m1deg09_radioButton.Checked = true;
            } else if ((value == "1.80") || (value == "1.800")) {
                m1deg18_radioButton.Checked = true;
            }
        }

        private void Update_2sa(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2sa), value); return; }

            if ((value == "0.90") || (value == "0.900")) {
                m2deg09_radioButton.Checked = true;
            } else if ((value == "1.80") || (value == "1.800")) {
                m2deg18_radioButton.Checked = true;
            }
        }

        private void Update_3sa(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3sa), value); return; }

            if ((value == "0.90") || (value == "0.900")) {
                m3deg09_radioButton.Checked = true;
            } else if ((value == "1.80") || (value == "1.800")) {
                m3deg18_radioButton.Checked = true;
            }
        }

        private void Update_4sa(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4sa), value); return; }

            if ((value == "0.90") || (value == "0.900")) {
                m4deg09_radioButton.Checked = true;
            } else if ((value == "1.80") || (value == "1.800")) {
                m4deg18_radioButton.Checked = true;
            }
        }

        // =========================================================================
        // *sa setting

        private void m1deg09_radioButton_Click(object sender, EventArgs e) {
            if (m1deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m1deg18_radioButton_Click(object sender, EventArgs e) {
            if (m1deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }


        private void m2deg09_radioButton_Click(object sender, EventArgs e) {
            if (m2deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m2deg18_radioButton_Click(object sender, EventArgs e) {
            if (m2deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg09_radioButton_Click(object sender, EventArgs e) {
            if (m3deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg18_radioButton_Click(object sender, EventArgs e) {
            if (m3deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg09_radioButton_Click(object sender, EventArgs e) {
            if (m4deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg18_radioButton_Click(object sender, EventArgs e) {
            if (m4deg09_radioButton.Checked) {
                Cnc.CNC_Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            } else {
                Cnc.CNC_Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        #endregion

        // =========================================================================
        #region mpo
        // mpo*: Position
        // * update

        private void update_field(string field, string value) {
            if (InvokeRequired) { Invoke(new Action<string,string>(update_field), value, field); return; }
            var x = this.GetType().GetField(field + "_textBox", BindingFlags.NonPublic | BindingFlags.Instance);
            if (x == null) throw new Exception("bad map for field " + field);
            var textbox = (TextBox)x.GetValue(this);
            if (textbox == null) throw new Exception("texbox is null");
            textbox.Text = value;
        }

        private void Update_xpos(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xpos), value); return; }
            TrueX_label.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void Update_ypos(string value) {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ypos), value); return; }
            ypos_textBox.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
        }


        #endregion

        // =========================================================================


        // =========================================================================




        private void TestX_button_Click(object sender, EventArgs e) {
            if (!Cnc.CNC_XY_m(0.0, Cnc.CurrentY))
                return;
            if (!Cnc.CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, Cnc.CurrentY))
                return;
            if (!Cnc.CNC_XY_m(0.0, Cnc.CurrentY))
                return;
        }

        private void TestY_thread() {
            if (!Cnc.CNC_XY_m(Cnc.CurrentX, 0))
                return;
            if (!Cnc.CNC_XY_m(Cnc.CurrentX, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!Cnc.CNC_XY_m(Cnc.CurrentX, 0))
                return;
        }

        private void TestY_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestY_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestXYA_thread() {
            if (!Cnc.CNC_XYA_m(0, 0, 0))
                return;
            if (!Cnc.CNC_XYA_m(Properties.Settings.Default.General_MachineSizeX, Properties.Settings.Default.General_MachineSizeY, 360.0))
                return;
            if (!Cnc.CNC_XYA_m(0, 0, 0))
                return;
        }

        private void TestXYA_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestXYA_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestXY_thread() {
            if (!Cnc.CNC_XY_m(0, 0))
                return;
            if (!Cnc.CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!Cnc.CNC_XY_m(0, 0))
                return;
        }

        private void TestXY_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestXY_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestYX_thread() {
            if (!Cnc.CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, 0))
                return;
            if (!Cnc.CNC_XY_m(0, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!Cnc.CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, 0))
                return;
        }

        private void TestYX_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestYX_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void HomeX_button_Click(object sender, EventArgs e) {
            Cnc.CNC_Home_m("X");
        }

        private void HomeXY_button_Click(object sender, EventArgs e) {
            if (!Cnc.CNC_Home_m("X"))
                return;
            Cnc.CNC_Home_m("Y");
        }

        private void HomeY_button_Click(object sender, EventArgs e) {
            Cnc.CNC_Home_m("Y");
        }

        private void HomeZ_button_Click(object sender, EventArgs e) {
            Needle.ProbingMode(false, JSON);
            Cnc.CNC_Home_m("Z");
        }

        private void TestZ_thread() {
            if (!Cnc.Zup())
                return;
            if (!Cnc.CNC_Z_m(Properties.Settings.Default.General_ZTestTravel))
                return;
            if (!Cnc.Zup())
                return;
        }

        private void TestZ_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestZ_thread());
            t.IsBackground = true;
            t.Start();
        }


        private void TestA_thread() {
            if (!Cnc.CNC_A_m(0))
                return;
            if (!Cnc.CNC_A_m(360))
                return;
            if (!Cnc.CNC_A_m(0))
                return;
        }

        private void TestA_button_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => TestA_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void Homebutton_Click(object sender, EventArgs e) {
            if (!Cnc.CNC_Home_m("Z"))
                return;
            if (!Cnc.CNC_Home_m("X"))
                return;
            if (!Cnc.CNC_Home_m("Y"))
                return;
            Cnc.CNC_A_m(0);
        }



        private void MotorPowerOff() {
            Cnc.CNC_Write_m("{\"md\":\"\"}");
        }

        private void MotorPowerOn() {
            Cnc.CNC_Write_m("{\"me\":\"\"}");
        }

        private void MotorPower_checkBox_Click(object sender, EventArgs e) {
            if (MotorPower_checkBox.Checked) {
                MotorPowerOn();
            } else {
                MotorPowerOff();
            }
        }

        private void Pump_checkBox_Click(object sender, EventArgs e) {
            if (Pump_checkBox.Checked) {
                PumpOn();
            } else {
                PumpOff();
            }
        }


        private void Vacuum_checkBox_Click(object sender, EventArgs e) {
            if (Vacuum_checkBox.Checked) {
                VacuumOn();
            } else {
                VacuumOff();
            }
        }

        private void SlackCompensation_checkBox_Click(object sender, EventArgs e) {
            if (SlackCompensation_checkBox.Checked) {
                Cnc.SlackCompensation = true;
                Properties.Settings.Default.CNC_SlackCompensation = true;
            } else {
                Cnc.SlackCompensation = false;
                Properties.Settings.Default.CNC_SlackCompensation = false;
            }
        }

        private void BuiltInSettings_button_Click(object sender, EventArgs e) {
            DialogResult dialogResult = ShowMessageBox(
                "All your current settings on TinyG will be lost. Are you sure?",
                "Confirm Loading Built-In settings", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) {
                return;
            };

            Thread t = new Thread(_BuiltInSettings);
            t.Start();
        }

        private void _BuiltInSettings() {
            // global
            // TODO: exeption, exeption handling here
            Cnc.CNC_Write_m("{\"st\":0}");   // switches NO type            
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"mt\":300}");   // motor timeout 5min
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"jv\":3}");   // JSON verbosity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"tv\":1}");   // text verbosity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"qv\":2}");   // queue verbosity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"sv\":1}");   // Status report verbosity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"si\":200}");   // Status report interval
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ec\":0}");   // send LF only
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ee\":0}");   // echo off
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"gun\":1}");   // use mm's
            Thread.Sleep(50);
            // Cnc.CNC_RawWrite("f2000");   // default feed rate (important thing that it is not 0)
            Cnc.CNC_RawWrite("{\"gc\":\"f2000\"}");   // default feed rate (important thing that it is not 0)
            Thread.Sleep(50);

            // Motor 1
            Cnc.CNC_Write_m("{\"1ma\":0}");   // map 1 to X
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"1sa\":0.9}");   // 0.9 deg. per step
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"1tr\":40.0}");   // 40mm per rev.
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"1mi\":8}");   // microstepping
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"1po\":0}");   // normal polarity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"1pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 2
            Cnc.CNC_Write_m("{\"2ma\":1}");   // map 2 to Y
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"2sa\":0.9}");   // 0.9 deg. per step
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"2tr\":40.0}");   // 40mm per rev.
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"2mi\":8}");   // microstepping
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"2po\":0}");   // normal polarity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"2pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 3
            Cnc.CNC_Write_m("{\"3ma\":2}");   // map 3 to Z
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"3sa\":1.8}");   // 1.8 deg. per step
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"3tr\":8.0}");   // 8mm per rev.
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"3mi\":8}");   // microstepping
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"3po\":0}");   // normal polarity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"3pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 4
            Cnc.CNC_Write_m("{\"4ma\":3}");   // map 4 to A
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"4sa\":0.9}");   // 1.8 deg. per step
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"4tr\":160.0}");   // 80 deg. per rev.
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"4mi\":8}");   // microstepping
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"4po\":0}");   // normal polarity
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"4pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // X
            Cnc.CNC_Write_m("{\"xam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xvm\":10000}");   // max velocity (proto 20000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xfr\":10000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xtm\":600}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            Cnc.CNC_Write_m("{\"xjm\":1000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xjh\":2000}");   // homing jerk (== xjm)
            Thread.Sleep(50);
#else
            Cnc.CNC_Write_m("{\"xjm\":1000000000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xjh\":2000000000}");   // homing jerk (== xjm)
            Thread.Sleep(50);
#endif
            Cnc.CNC_Write_m("{\"xjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xsn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xsx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xsv\":2000}");   // homing speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xlv\":100}");   // latch speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xlb\":8}");   // latch backup
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"xzb\":2}");   // zero backup
            Thread.Sleep(50);

            // Y
            Cnc.CNC_Write_m("{\"yam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"yvm\":10000}");   // max velocity (proto 20000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"yfr\":10000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ytm\":400}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            Cnc.CNC_Write_m("{\"yjm\":1000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"yjh\":2000}");   // homing jerk (== yjm)
            Thread.Sleep(50);
#else
            Cnc.CNC_Write_m("{\"yjm\":1000000000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"yjh\":2000000000}");   // homing jerk (== yjm)
            Thread.Sleep(50);
#endif
            Cnc.CNC_Write_m("{\"yjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ysn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ysx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ysv\":2000}");   // homing speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ylv\":100}");   // latch speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ylb\":8}");   // latch backup
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"yzb\":2}");   // zero backup
            Thread.Sleep(50);

            // Z
            Cnc.CNC_Write_m("{\"zam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zvm\":5000}");   // max velocity (proto 10000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zfr\":2000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ztm\":80}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            Cnc.CNC_Write_m("{\"zjm\":500}");   // max jerk (proto 1000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zjh\":500}");   // homing jerk (== zjm)
            Thread.Sleep(50);
#else
            Cnc.CNC_Write_m("{\"zjm\":500000000}");   // max jerk (proto 1000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zjh\":500000000}");   // homing jerk (== zjm)
            Thread.Sleep(50);
#endif
            Cnc.CNC_Write_m("{\"zjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zsn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zsx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zsv\":1000}");   // homing speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zlv\":100}");   // latch speed
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zlb\":4}");   // latch backup
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"zzb\":2}");   // zero backup
            Thread.Sleep(50);

            // A
            Cnc.CNC_Write_m("{\"aam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"avm\":50000}");   // max velocity (proto 50000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"afr\":200000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"atm\":400}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            Cnc.CNC_Write_m("{\"ajm\":5000}");   // max jerk (proto 5000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ajh\":5000}");   // homing jerk (== ajm)
            Thread.Sleep(50);
#else
            Cnc.CNC_Write_m("{\"ajm\":5000000000}");   // max jerk (proto 5000)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"ajh\":5000000000}");   // homing jerk (== ajm)
            Thread.Sleep(50);
#endif
            Cnc.CNC_Write_m("{\"ajd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"asn\":0}");   // disable switches, no homing an A
            Thread.Sleep(50);
            Cnc.CNC_Write_m("{\"asx\":0}");   // disable switches, no homing an A
            Thread.Sleep(50);
            // No need to touch A homing parameters

            UpdateWindowValues_m();
            ShowMessageBox(
                "Default settings written.",
                "Default settings written",
                MessageBoxButtons.OK);
        }


        private void SaveSettings_button_Click(object sender, EventArgs e) {

            Cnc.CNC_Write_m("{\"sys\":\"\"}");
            Cnc.CNC_Write_m("{\"x\":\"\"}");
            Cnc.CNC_Write_m("{\"y\":\"\"}");
            Cnc.CNC_Write_m("{\"z\":\"\"}");
            Cnc.CNC_Write_m("{\"a\":\"\"}");
            Cnc.CNC_Write_m("{\"1\":\"\"}");
            Cnc.CNC_Write_m("{\"2\":\"\"}");
            Cnc.CNC_Write_m("{\"3\":\"\"}");
            Cnc.CNC_Write_m("{\"4\":\"\"}");

            // And save
            Properties.Settings.Default.Save();
            DisplayText("Settings saved.");
            Properties.Settings.Default.TinyG_settings_saved = true;
            DisplayText("sys:");
            DisplayText(Properties.Settings.Default.TinyG_sys);
            DisplayText("x:");
            DisplayText(Properties.Settings.Default.TinyG_x);
            DisplayText("y:");
            DisplayText(Properties.Settings.Default.TinyG_y);
            DisplayText("m1:");
            DisplayText(Properties.Settings.Default.TinyG_m1);
            // save tape stuff
            Tapes.SaveAll();
        }


        private void DefaultSettings_button_Click(object sender, EventArgs e) {
            if (!Properties.Settings.Default.TinyG_settings_saved) {
                ShowMessageBox(
                "You don't have saved User Default settings.",
                "No Saved settings", MessageBoxButtons.OK);
                return;
            }

            DialogResult dialogResult = ShowMessageBox(
                "All your current settings on TinyG will be lost. Are you sure?",
                "Confirm Loading Saved settings", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No) {
                return;
            }

            DisplayText("Start of DefaultSettings()");
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_sys);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_x);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_y);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_z);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_a);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_m1);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_m2);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_m3);
            Thread.Sleep(150);
            Cnc.CNC_Write_m(Properties.Settings.Default.TinyG_m4);
            Thread.Sleep(150);
            UpdateWindowValues_m();
            ShowMessageBox(
                "Settings restored.",
                "Saved settings restored",
                MessageBoxButtons.OK);
        }

        private static int SetProbing_stage;
        private void SetProbing_button_Click(object sender, EventArgs e) {
            Cnc.ZGuardOff();
            switch (SetProbing_stage) {
                case 0:
                    Zlb_label.Text = "Put a regular height PCB under the needle, \n\rthen click \"Next\"";
                    Zlb_label.Visible = true;
                    SetProbing_button.Text = "Next";
                    SetProbing_stage = 1;
                    break;

                case 1:
                    Needle.ProbingMode(true, JSON);
                    //CNC_Write_m("G28.4 Z0", 2000);
                    Cnc.CNC_Write_m("{\"gc\":\"G28.4 Z0\"}", 2000);
                    Properties.Settings.Default.General_ZtoPCB = Cnc.CurrentZ;
                    Zlb_label.Text = "Jog Z axis until the needle just barely touches the PCB\nThen click \"Next\"";
                    SetProbing_stage = 2;
                    break;

                case 2:
                    Properties.Settings.Default.General_ProbingBackOff = Properties.Settings.Default.General_ZtoPCB - Cnc.CurrentZ;
                    Properties.Settings.Default.General_ZtoPCB = Cnc.CurrentZ;
                    Needle.ProbingMode(false, JSON);
                    SetProbing_button.Text = "Start";
                    Zlb_label.Text = "";
                    Zlb_label.Visible = false;
                    Cnc.CNC_Home_m("Z");
                    ShowMessageBox(
                       "Probing Backoff set successfully.\n" +
                            "PCB surface: " + Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture) +
                            "\nBackoff:  " + Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture),
                       "Done",
                       MessageBoxButtons.OK);
                    SetProbing_stage = 0;
                    Z0toPCB_BasicTab_label.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture);
                    Z_Backoff_label.Text = Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture);
                    Cnc.ZGuardOn();
                    break;
            }
        }


        #endregion
        // =================================================================================
        // Run job page functions
        // =================================================================================
        #region Job page functions
        private PartLocation JobOffset = new PartLocation();
        private double JobOffsetX;
        private double JobOffsetY;
        private PartLocation JigOffset {
            get { return new PartLocation(Properties.Settings.Default.General_JigOffsetX, Properties.Settings.Default.General_JigOffsetY); }
        }






        // =================================================================================
        // CAD data and Job data load and save functions
        // =================================================================================
        private string CadDataFileName = "";
        private string JobFileName = "";




        private bool LoadCadData_m() {
            String[] AllLines;

            // read in CAD data (.csv file)
            if (CAD_openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    bool result;
                    CadDataFileName = CAD_openFileDialog.FileName;
                    AllLines = File.ReadAllLines(CadDataFileName);
                    if (Path.GetExtension(CAD_openFileDialog.FileName) == ".pos") {
                        result = Cad.ParseKiCadData_m(AllLines);
                    } else {
                        result = Cad.ParseCadData_m(AllLines, false);
                    }
                    return result;
                } catch (Exception ex) {
                    Cursor.Current = Cursors.Default;
                    ShowMessageBox(
                        "Error in file, Msg: " + ex.Message,
                        "Can't read CAD file",
                        MessageBoxButtons.OK);
                    CadData_GridView.Rows.Clear();
                    CadDataFileName = "--";
                    return false;
                };
            }
            return false;
        }

        // =================================================================================
        private bool LoadJobData_m(string filename) {
            try {
                Cad.JobData.Clear();
                Cad.JobData.AddRange(Global.DeSerialization<SortableBindingList<JobData>>(filename));
                JobData_GridView.DataSource = Cad.JobData;
            } catch (Exception ex) {
                ShowSimpleMessageBox("Can't Load Job File "+Path.GetFileName(filename)+" : "+ex);
                JobData_GridView.Rows.Clear();
                CadDataFileName = "--";
                return false;
            };
            return true;
        }



        public int GetGridRow(DataGridView grid) {
            return grid.CurrentCell.RowIndex;
        }

        private void Up_button_Click(object sender, EventArgs e) {
            var row = GetGridRow(JobData_GridView);
            if (row != -1) Global.MoveItem(Cad.JobData, row, -1);
            JobData_GridView.CurrentCell = JobData_GridView[0, row - 1];
        }

        private void Down_button_Click(object sender, EventArgs e) {
            var row = GetGridRow(JobData_GridView);
            if (row != -1) Global.MoveItem(Cad.JobData, row, +1);
            JobData_GridView.CurrentCell = JobData_GridView[0, row + 1];
        }

        private void DeleteComponentGroup_button_Click(object sender, EventArgs e) {
            var row = GetGridRow(JobData_GridView);
            Cad.JobData.RemoveAt(row);
        }


        private void NewRow_button_Click(object sender, EventArgs e) {
            int row = GetGridRow(JobData_GridView);
            if (row != -1) Cad.JobData.Insert(row, new JobData());
            else Cad.JobData.Add(new JobData());
        }

        // =================================================================================
        // JobData editing
        // =================================================================================
        private void JobData_GridView_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (JobData_GridView.CurrentCell.ColumnIndex == 3) {
                // For method, show a form with explanation texts
                MethodSelectionForm MethodDialog = new MethodSelectionForm();
                MethodDialog.ShowCheckBox = false;
                MethodDialog.ShowDialog(this);
                string id = "";
                if (MethodDialog.SelectedMethod == "Place") {
                    id = SelectTape("Select tape for " + JobData_GridView.Rows[JobData_GridView.CurrentCell.RowIndex].Cells[2].Value);
                }
                if (MethodDialog.SelectedMethod != "") {
                    foreach (DataGridViewCell cell in JobData_GridView.SelectedCells) {
                        JobData_GridView.Rows[cell.RowIndex].Cells[3].Value = MethodDialog.SelectedMethod;
                        if (id != "") JobData_GridView.Rows[cell.RowIndex].Cells[4].Value = id;
                    }
                }
                Update_GridView(JobData_GridView);
                return;
            };

            if (JobData_GridView.CurrentCell.ColumnIndex == 4) {
                // For method parameter, show the tape selection form if method is "place" 
                int row = JobData_GridView.CurrentCell.RowIndex;
                var x = JobData_GridView.Rows[row].Cells[3].Value;
                if (x != null && x.Equals("Place")) {
                    JobData_GridView.Rows[row].Cells[4].Value = SelectTape("Select tape for ");
                    //+ JobData_GridView.Rows[row].Cells["ComponentType"].Value.ToString());
                    Update_GridView(JobData_GridView);
                }
            }
        }


        /*  // If components are edited, update count automatically
          private void JobData_GridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
          {
              if (JobData_GridView.CurrentCell.ColumnIndex == 4)
              {
                  // components
                  List<String> Line = CAD.SplitCSV(JobData_GridView.CurrentCell.Value.ToString(), ',');
                  int row = JobData_GridView.CurrentCell.RowIndex;
                  JobData_GridView.Rows[row].Cells["ComponentCount"].Value = Line.Count.ToString();
                  Update_GridView(JobData_GridView);
              }
          }
          */

        // =================================================================================
        // Do someting to a group of components:
        // =================================================================================
        // Several rows are selected at Job data:

        private void PlaceThese_button_Click(object sender, EventArgs e) {
            if (!PrepareToPlace_m()) {
                ShowSimpleMessageBox("Placement operation failed, nothing done.");
                return;
            }

            var selectedCount = JobData_GridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedCount == 0) {
                CleanupPlacement(true);
                ShowSimpleMessageBox("Nothing selected");
                return;
            }

            List<PhysicalComponent> to_place = new List<PhysicalComponent>();
            for (int i = 0; i < selectedCount; i++) {
                var job = (JobData)JobData_GridView.SelectedRows[i].DataBoundItem;
                to_place.AddRange(job.GetComponents());
            }

            // ... so that we can put next row label in place:
            //    NextGroup_label.Text = JobData_GridView.Rows[FirstRow].Cells["ComponentType"].Value.ToString() + " (" +
            //            JobData_GridView.Rows[FirstRow].Cells["ComponentCount"].Value.ToString() + " pcs.)";


            if (!PlaceComponents(to_place)) {
                ShowSimpleMessageBox("Placement operation failed. Review job status.");
                CleanupPlacement(false);
                return;
            }

            CleanupPlacement(true);
        }


        // =================================================================================
        // This routine places selected component(s) from CAD data grid view:
        private void PlaceOne_button_Click(object sender, EventArgs e) {
            if (!PrepareToPlace_m()) {
                ShowSimpleMessageBox("Placement operation failed, nothing done.");
                return;
            }

            var selectedCount = CadData_GridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedCount == 0) {
                CleanupPlacement(true);
                ShowSimpleMessageBox("Nothing selected");
                return;
            }

            List<PhysicalComponent> to_place = new List<PhysicalComponent>();
            for (int i = 0; i < selectedCount; i++) {
                var comp = (PhysicalComponent)CadData_GridView.SelectedRows[i].DataBoundItem;
                to_place.Add(comp);
            }

            // ... so that we can put next row label in place:
            //    NextGroup_label.Text = JobData_GridView.Rows[FirstRow].Cells["ComponentType"].Value.ToString() + " (" +
            //            JobData_GridView.Rows[FirstRow].Cells["ComponentCount"].Value.ToString() + " pcs.)";


            if (!PlaceComponents(to_place)) {
                ShowSimpleMessageBox("Placement operation failed. Review job status.");
                CleanupPlacement(false);
                return;
            }

            CleanupPlacement(true);
        }

        // =================================================================================
        // This routine places the [index] row from Job data grid view:
        private bool PlaceComponents(List<PhysicalComponent> components) {
            DisplayText("Placing " + string.Join(",", components.Select(x => x.Designator)));

            foreach (var component in components) {
                if (component.JobData.Method.Equals("?")) {
                    MethodSelectionForm MethodDialog = new MethodSelectionForm();
                    MethodDialog.ShowCheckBox = true;
                    // MethodDialog.HeaderString = CurrentGroup_label.Text;
                    MethodDialog.ShowDialog(this);

                    switch (MethodDialog.SelectedMethod) {
                        case "Place":
                        case "Place Fast":
                            var NewID = SelectTape("Select tape for " + component.Footprint);
                            component.Method = MethodDialog.SelectedMethod;
                            component.MethodParameters = NewID;
                            break;
                        case "": //user pressed x
                            return false;
                        case "none":
                            break;
                        case "Ignore":
                            component.Method = "Ignore";
                            component.MethodParameters = "";
                            break;
                        case "Abort":
                            AbortPlacement = true;
                            component.Method = "Ignore";
                            component.MethodParameters = "";
                            break;
                        default:
                            component.JobData.Method = MethodDialog.SelectedMethod;
                            break;
                    }
                    MethodDialog.Dispose();
                }
            }

            foreach (var component in components) {
                if (!PlaceComponent(component)) return false;
            }
            return true;
        }


        // =================================================================================
        // All rows:
        private void PlaceAll_button_Click(object sender, EventArgs e) {
            JobData_GridView.SelectAll();
            PlaceThese_button_Click(null, null);
        }


        // =================================================================================
        // PlaceComponent_m()
        // This routine does the actual placement of a single component.
        // Component is the component name (Such as R15); based to this, we'll find the coordinates from CAD data
        // GroupRow is the row index to Job data grid view.
        // =================================================================================

        private bool PlaceComponent(PhysicalComponent comp) {
            DisplayText("PlaceComponent_m: Component: " + comp.Designator);
            if (comp.IsFiducial) return true; //skip fiducials


            if ((comp.Method == "LoosePart") || (comp.Method == "Place") || (comp.Method == "Place Fast")) {
                PlacedComponent_label.Text = comp.Designator;
                PlacedComponent_label.Update();
                PlacedValue_label.Text = comp.Footprint;
                PlacedValue_label.Update();
                PlacedX_label.Text = comp.machine.X.ToString();
                PlacedX_label.Update();
                PlacedY_label.Text = comp.machine.Y.ToString();
                PlacedY_label.Update();
                PlacedRotation_label.Text = comp.machine.A.ToString("F2");
                PlacedRotation_label.Update();
                MachineCoords_label.Text = "( " + comp.machine + ")";
                MachineCoords_label.Update();
            };

            if (CheckAbort()) return false;

            // Component is at CadData_GridView.Rows[CADdataRow]. 
            // What to do to it is at  JobData_GridView.Rows[GroupRow].

            switch (comp.Method) {
                case "?":
                    ShowMessageBox(
                        "Method is ? at run time",
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                    return false;

                case "Pause":
                    ShowMessageBox(
                        "Job pause, click OK to continue.",
                        "Pause",
                        MessageBoxButtons.OK);
                    return true;

                case "LoosePart":
                    if (!PlacePart_m(true, comp.IsFirstInRow, comp)) return false;
                    break;
                case "Place Fast":
                case "Place":
                    if (!PlacePart_m(false, comp.IsFirstInRow, comp)) return false;
                    break;

                case "Change needle":
                    if (!ChangeNeedle_m()) return false;
                    break;

                case "Recalibrate":
                    if (!PrepareToPlace_m()) return false;
                    break;

                case "Ignore":
                    return true;

                case "Fiducials":
                    return true;

                default:
                    ShowSimpleMessageBox("No way to handle method " + comp.Method);
                    return false;
            }

            return true;
        }



        // =================================================================================
        private bool ChangeNeedle_m() {
            Cnc.CNC_Write_m("{\"zsn\":0}");
            Cnc.CNC_Write_m("{\"zsx\":0}");
            Thread.Sleep(50);
            PumpOff();
            MotorPowerOff();
            ShowMessageBox(
                "Change Needle now, press OK when done",
                "Needle change pause",
                MessageBoxButtons.OK);
            MotorPowerOn();
            Zlim_checkBox.Checked = true;
            Zhome_checkBox.Checked = true;
            Needle.Calibrated = false;
            ValidMeasurement_checkBox.Checked = false;
            Cnc.CNC_Write_m("{\"zsn\":3}");
            Cnc.CNC_Write_m("{\"zsx\":2}");
            if (!MechanicalHoming_m()) {
                return false;
            }
            if (!OpticalHoming_m()) {
                return false;
            }
            if (!CalibrateNeedle_m()) {
                return false;
            }
            if (!BuildMachineCoordinateData_m()) {
                return false;
            }
            PumpOn();
            return true;
        }

        // =================================================================================
        // This is called before any placement is done:
        // =================================================================================
        private bool PrepareToPlace_m() {
            if (Cad.JobData.Count == 0) {
                ShowMessageBox(
                    "No Job loaded.",
                    "No Job",
                    MessageBoxButtons.OK
                );
                return false;
            }

            if (!Needle.Calibrated) {
                //CurrentGroup_label.Text = "Calibrating needle";
                if (!CalibrateNeedle_m()) {
                    //   CurrentGroup_label.Text = "--";
                    return false;
                }
            }

            //  CurrentGroup_label.Text = "Measuring PCB";
            if (!BuildMachineCoordinateData_m()) {
                //    CurrentGroup_label.Text = "--";
                return false;
            }

            AbortPlacement = false;
            PlaceThese_button.Capture = false;
            PlaceAll_button.Capture = false;
            JobData_GridView.ReadOnly = true;
            PumpOn();
            return true;
        }  // end PrepareToPlace_m

        // =================================================================================
        // This cleans up the UI after placement operations
        // =================================================================================
        private void CleanupPlacement(bool success) {
            PlacedComponent_label.Text = "--";
            PlacedValue_label.Text = "--";
            PlacedX_label.Text = "--";
            PlacedY_label.Text = "--";
            PlacedRotation_label.Text = "--";
            MachineCoords_label.Text = "( -- )";
            JobData_GridView.ReadOnly = false;
            PumpDefaultSetting();
            VacuumDefaultSetting();
            if (success) {
                Cnc.CNC_Park();  // if fail, it helps debugging if machine stays still
            }
        }


        // =================================================================================
        // PickUpPart_m(): Picks next part from the tape
        private bool PickUpPart_m(string TapeID) {
            DisplayText("PickUpPart_m(), tape id: " + TapeID);
            var tape = Tapes.GetTapeObjByID(TapeID);
            if (tape == null) return false;

            // Go to part location:
            VacuumOff();
            if (!Tapes.GotoNextPart_m(TapeID)) return false;

            // Pick it up:
            if (! tape.IsPickupZSet) {
                if (!Needle_ProbeDown_m()) return false;
                tape.PickupZ = Cnc.CurrentZ;
                DisplayText("PickUpPart_m(): Probing Z= " + Cnc.CurrentZ);
            } else {
                double Z = tape.PickupZ + 0.5; //not sure why the .5 is there - increased pressure?
                DisplayText("PickUpPart_m(): Part pickup, Z" + Z);
                if (!Cnc.CNC_Z_m(Z))  return false;
            }

            VacuumOn();
            DisplayText("PickUpPart_m(): needle up");
            if (!Cnc.Zup()) return false;

            return true;
        }

        // =================================================================================
        // PutPartDown_m(): Puts part down at this position. 
        // If placement Z isn't known already, updates the tape info.
        private bool PutPartDown_m(string TapeID) {
            var tape = Tapes.GetTapeObjByID(TapeID);
            if (tape == null) return false;

            if (!tape.IsPlaceZSet) {
                DisplayText("PutPartDown_m(): Probing placement Z");
                if (!Needle_ProbeDown_m()) return false;
                tape.PlaceZ = Cnc.CurrentZ;
                DisplayText("PutPartDown_m(): placement Z= " + Cnc.CurrentZ);
            } else {
                double Z = tape.PlaceZ;
                DisplayText("PlacePart_m(): Part down, Z" + Z);
                if (!Cnc.CNC_Z_m(Z))  return false;
            }
            
            DisplayText("PlacePart_m(): Needle up.");
            VacuumOff();
            if (!Cnc.Zup()) return false;
            return true;
        }

        // =================================================================================
        // 
        private bool PutLoosePartDown_m(bool Probe) {
            if (Probe) {
                DisplayText("PutLoosePartDown_m(): Probing placement Z");
                if (!Needle_ProbeDown_m()) {
                    return false;
                }
                LoosePartPlaceZ = Cnc.CurrentZ;
                DisplayText("PutLoosePartDown_m(): placement Z= " + Cnc.CurrentZ);
            } else {
                if (!Cnc.CNC_Z_m(LoosePartPlaceZ)) {
                    return false;
                }
            }
            DisplayText("PutLoosePartDown_m(): Needle up.");
            VacuumOff();
            if (!Cnc.Zup())  // back up
            {
                return false;
            }
            return true;
        }
        // =================================================================================
        // Actual placement 
        // =================================================================================
        private double LoosePartPickupZ;
        private double LoosePartPlaceZ;

        private bool PickUpLoosePart_m(bool Probe, PhysicalComponent comp) {
            if (!Cnc.CNC_XY_m(Properties.Settings.Default.General_PickupCenterX, Properties.Settings.Default.General_PickupCenterY)) {
                return false;
            }

            // ask for it
            string ComponentType = comp.Footprint;
            DialogResult dialogResult = ShowMessageBox(
                "Put one " + ComponentType + " to the pickup location.",
                "Placing " + comp.Designator,
                MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel) {
                return false;
            }

            // Find component
            double X = 0;
            double Y = 0;
            double A = 0.0;
            cameraView.SetDownCameraFunctionSet("component");
            // If we don't get a look from straight up (more than 2mm off) we need to re-measure
            for (int i = 0; i < 2; i++) {
                // measure 5 averages, component must be 8.0mm from its place
                int count = VideoDetection.MeasureClosestComponentInPx(out X, out Y, out A, cameraView.downVideoProcessing, (8.0 / Properties.Settings.Default.DownCam_XmmPerPixel), 5);
                if (count == 0) {
                    ShowMessageBox(
                        "Could not see component",
                        "No component",
                        MessageBoxButtons.OK);
                    return false;
                }
                X = X * Properties.Settings.Default.DownCam_XmmPerPixel;
                Y = -Y * Properties.Settings.Default.DownCam_YmmPerPixel;
                DisplayText("PickUpLoosePart_m(): measurement " + i + ", X: " + X + ", Y: " + Y + ", A: " + A);
                if ((Math.Abs(X) < 2.0) && (Math.Abs(Y) < 2.0)) {
                    break;
                }
                if (!Cnc.CNC_XY_m(Cnc.CurrentX + X, Cnc.CurrentY + Y)) {
                    return false;
                }
            }
            Needle.Move_m(Cnc.CurrentX + X, Cnc.CurrentY + Y, A);
            // pick it up
            if (Probe) {
                DisplayText("PickUpLoosePart_m(): Probing pickup Z");
                if (!Needle_ProbeDown_m()) {
                    return false;
                }
                LoosePartPickupZ = Cnc.CurrentZ;
                DisplayText("PickUpLoosePart_m(): Probing Z= " + Cnc.CurrentZ);
            } else {
                DisplayText("PickUpLoosePart_m(): Part pickup, Z" + LoosePartPickupZ);
                if (!Cnc.CNC_Z_m(LoosePartPickupZ)) {
                    return false;
                }
            }
            VacuumOn();
            DisplayText("PickUpLoosePart_m(): needle up");
            if (!Cnc.Zup()) {
                return false;
            }
            if (AbortPlacement) {
                AbortPlacement = false;
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool CheckAbort() {
            if (AbortPlacement) {
                AbortPlacement = false;
                ShowSimpleMessageBox("Aborting");
                return true;
            }
            return false;
        }

        private bool PlacePart_m(bool LoosePart, bool resetTapeZ, PhysicalComponent comp) {
            if (CheckAbort()) return false;

            DisplayText("PlacePart_m, Component: " + comp.Designator);
            if (CheckAbort()) return false;
            if (resetTapeZ && !LoosePart && !Tapes.ClearHeights_m(comp.MethodParameters)) return false;

            // Pickup:
            if (CheckAbort()) return false;
            if (LoosePart && !PickUpLoosePart_m(resetTapeZ, comp)) return false;
            if (!PickUpPart_m(comp.MethodParameters)) return false;

            // Take the part to position:
            if (CheckAbort()) return false;
            DisplayText("PlacePart_m: goto placement position " + comp.machine, Color.Blue);
            if (!Needle.Move_m(comp.machine)) return false;

            // Place it:
            if (CheckAbort()) return false;
            if (LoosePart && !PutLoosePartDown_m(resetTapeZ)) return false;
            if (!PutPartDown_m(comp.MethodParameters)) return false;

            return true;
        }

        // =================================================================================
        // PlacePartWithUpCamHelp_m(): Like basic PlacePart_m(),
        // but shows the part to upcam for additional correction values
        private bool PlacePartWithUpCamHelp_m(int CADdataRow, string Component, string TapeID, double X, double Y, double A) {
            var tape = Tapes.GetTapeObjByID(TapeID);
            if (tape == null) return false;
            // Pick the part
            if (!PickUpPart_m(TapeID)) {
                return false;
            }
            // take it to camera
            if (!Needle.Move_m(
                Properties.Settings.Default.UpCam_PositionX - Properties.Settings.Default.DownCam_NeedleOffsetX,
                Properties.Settings.Default.UpCam_PositionY - Properties.Settings.Default.DownCam_NeedleOffsetY, A)) {
                // VacuumOff();  if the above failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                return false;
            }
            // Get placement Z
            double Z;
            if (!tape.IsPlaceZSet) {
                // Don't know it yet, use PCB height -1mm
                Z = Properties.Settings.Default.General_ZtoPCB - 1.0;
            } else {
                Z = tape.PlaceZ;
            }
            DisplayText("PlacePart_m(): Part down at Upcam");
            if (!Cnc.CNC_Z_m(Z)) {
                return false;
            };
            double dX = 0;
            double dY = 0;
            double dA;
            if (!GetCorrentionForPartAtNeedle(out dX, out dY, out dA)) {
                return false;
            }
            if (!Cnc.Zup()) {
                return false;
            };
            // Take the part to position:
            if (!Needle.Move_m(X + dX, Y + dY, A + dA)) {
                // VacuumOff();  if the above failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                return false;
            }
            // Place it:
            if (!PutPartDown_m(TapeID)) {
                // VacuumOff();  if this failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                return false;
            }
            return true;
        }

        // =================================================================================
        // GetCorrentionForPartAtNeedle():
        // takes a look from Upcam, sets the correction values for the part at needle
        private bool GetCorrentionForPartAtNeedle(out double dX, out double dY, out double dA) {

            dX = 0;
            dY = 0;
            dA = 0;

            cameraView.upVideoProcessing.SetFunctionsList("component");
            bool GoOn = false;
            bool result = false;
            while (!GoOn) {
                if (MeasureUpCamComponent(3.0, out dX, out dY, out dA)) {
                    result = true;
                    GoOn = true;
                } else {
                    DialogResult dialogResult = ShowMessageBox(
                        "Did not get correction values from camera.\n Abort job / Retry / Place anyway?",
                        "Did not see component",
                        MessageBoxButtons.AbortRetryIgnore
                    );
                    if (dialogResult == DialogResult.Abort) {
                        AbortPlacement = true;
                        result = false;
                        GoOn = true;
                    } else if (dialogResult == DialogResult.Retry) {
                        GoOn = false;
                    } else {
                        // ignore
                        result = true;
                        GoOn = true;
                    }
                }
            };
            cameraView.upVideoProcessing.ClearFunctionsList();
            return result;
        }

        private bool MeasureUpCamComponent(double Tolerance, out double dX, out double dY, out double dA) {
            dX = 0; dY = 0; dA = 0;
            // make 5 measurements and return the closest component detected each time
            List<Shapes.Component> components = new List<Shapes.Component>();
            for (int i = 0; i < 5; i++) components.Add(
                VideoDetection.GetClosest(
                VideoDetection.FindComponents(cameraView.upVideoProcessing)));

            if (components.Count == 0) return false;

            var c = VideoDetection.AverageLocation(components).ToMMResolution();
            dX = c.X;
            dY = c.Y;
            dA = c.A;


            DisplayText("Component measurement:");
            DisplayText("X: " + dX.ToString("0.000", CultureInfo.InvariantCulture) + " (" + components.Count + " results out of 5)");
            DisplayText("Y: " + dY.ToString("0.000", CultureInfo.InvariantCulture));
            return true;
        }

        // =================================================================================
        // BuildMachineCoordinateData_m routine builds the machine coordinates data 
        // based on fiducials true (machine coord) location.
        // =================================================================================

        // =================================================================================
        // FindFiducials_m():
        // Finds the fiducials from job data (which by now, exists).
        // Sets FiducialsRow to indicate the row in JobData_GridView


        private PhysicalComponent FindDesignator(string designator) {
            var y = Cad.ComponentData.Where(x => x.Designator.Equals(designator)).ToArray();
            if (y.Length == 0) return null;
            if (y.Length > 1) throw new Exception("More than one entry with designator " + designator);
            return y[0];
        }



        // =================================================================================
        // MeasureFiducial_m():
        // Takes the parameter nominal location and measures its physical location.
        // Assumes measurement parameters already set.

        private bool MeasureFiducial_m(ref PhysicalComponent fid) {
            Cnc.CNC_XY_m(fid.nominal + JobOffset + JigOffset);

            // If more than 3mm off here, not good.
            double X, Y;

            // what type of fiducail are we interested in?
            Shapes.ShapeTypes type;
            if (Properties.Settings.Default.use_template) {
                type = Shapes.ShapeTypes.Fiducial;  //template based mathing
            } else {
                type = Shapes.ShapeTypes.Circle;    //circle based matching
            }

            if (!GoToLocation_m(type, 3, 0.1, out X, out Y)) {
                ShowMessageBox(
                    "Finding fiducial: Can't regognize fiducial " + fid.Designator,
                    "No Circle found",
                    MessageBoxButtons.OK);
                return false;
            }

            fid.X_machine = Cnc.CurrentX + X;
            fid.Y_machine = Cnc.CurrentY + Y;
            // For user confidence, show it:
            for (int i = 0; i < 50; i++) {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            return true;
        }

        // =================================================================================
        // BuildMachineCoordinateData_m():
        private bool BuildMachineCoordinateData_m() {
            if (ValidMeasurement_checkBox.Checked) return true;

            // Get ready for position measurements
            DisplayText("SetFiducialsMeasurement");
            cameraView.SetDownCameraFunctionSet("fiducial");

            // populate fiducal data
            PhysicalComponent[] Fiducials = Cad.ComponentData.Where(x => x.IsFiducial).ToArray();
            if (Fiducials.Length < 2) {
                ShowSimpleMessageBox("Only " + Fiducials.Length + " fiducials set - not able to calibrate machine coordinates");
                return false;
            }

            // measure the actual data
            for (int i = 0; i < Fiducials.Length; i++) {
                if (!MeasureFiducial_m(ref Fiducials[i])) return false;
            }

            // Find the homographic tranformation from CAD data (fiducials.nominal) to measured machine coordinates
            // (fiducials.machine):

            // RN - setup lists of points and do a least squares fit of a afine transform
            List<PartLocation> nominalLocations = new List<PartLocation>(Fiducials.Length);
            List<PartLocation> measuredLocations = new List<PartLocation>(Fiducials.Length);
            foreach (PhysicalComponent t in Fiducials){
                nominalLocations.Add(t.nominal);
                measuredLocations.Add(t.machine);
            }
            LeastSquaresMapping lsm = new LeastSquaresMapping(nominalLocations, measuredLocations);
            var error = lsm.RMSError();
            //    if (error > 1) { //some arbitrary thershold
            //       ShowSimpleMessageBox("Fiducial fit high RMS diff value - aborting (" + error + ")");
            //       return false;
            //    }


            // verify that no fiducial moved more then 0.4mm
            if (lsm.MaxFiducialMovement() > 0.4) {
                DisplayText(" ** A fiducial moved more than 0.4mm from its measured location");
                DisplayText(" ** when applied the same calculations than regular componets.");
                DisplayText(" ** (Maybe the camera picked a via instead of a fiducial?)");
                DisplayText(" ** Placement data is likely not good.");
                DialogResult dialogResult = ShowMessageBox(
                    "Nominal to machine trasnformation seems to be off. (See log window)",
                    "Cancel operation?",
                    MessageBoxButtons.OKCancel
                );
                if (dialogResult == DialogResult.Cancel) return false;
            }


            //apply mapping
            foreach (var component in Cad.ComponentData.Where(x => !x.IsFiducial).ToArray()) {
                component.machine = lsm.Map(component.nominal);
            }

            // Refresh UI:
            Update_GridView(CadData_GridView);

            // Done! 
            ValidMeasurement_checkBox.Checked = true;
            cameraView.SetDownCameraFunctionSet("");
            return true;
        }// end BuildMachineCoordinateData_m

        // =================================================================================
        // BuildMachineCoordinateData_m functions end
        // =================================================================================


        // =================================================================================
        private void PausePlacement_button_Click(object sender, EventArgs e) {
            DialogResult dialogResult = ShowMessageBox(
                "Placement Paused. Continue? (Cancel aborts)",
                "Placement Paused",
                MessageBoxButtons.OKCancel
            );
            if (dialogResult == DialogResult.Cancel) {
                AbortPlacement = true;
            }

        }

        // =================================================================================
        private void AbortPlacement_button_Click(object sender, EventArgs e) {
            AbortPlacement = true;
        }



        // =================================================================================
        private void ShowNominal_button_Click(object sender, EventArgs e) {
            PhysicalComponent component = (PhysicalComponent)CadData_GridView.CurrentRow.DataBoundItem;
            Cnc.CNC_XY_m(component.nominal + JobOffset + JigOffset);
        }

        // =================================================================================
        // Checks what is needed to check before doing something for a single component selected at "CAD data" table. 
        // If succesful, sets X, Y to component machine coordinates.
        private bool PrepareSingleComponentOperation(out double X, out double Y) {
            X = 0.0;
            Y = 0.0;

            DataGridViewCell cell = CadData_GridView.CurrentCell;
            if (cell == null) {
                return false;  // no component selected
            }
            if (cell.OwningRow.Index < 0) {
                return false;  // header row
            }
            if (cell.OwningRow.Cells["X_machine"].Value.ToString() == "Nan") {
                DialogResult dialogResult = ShowMessageBox(
                    "Component locations not yet measured. Measure now?",
                    "Measure now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No) {
                    return false;
                }
                if (!BuildMachineCoordinateData_m()) {
                    return false;
                }
            }

            if (!double.TryParse(cell.OwningRow.Cells["X_machine"].Value.ToString(), out X)) {
                ShowMessageBox(
                    "Bad data at X_machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }

            if (!double.TryParse(cell.OwningRow.Cells["Y_machine"].Value.ToString(), out Y)) {
                ShowMessageBox(
                    "Bad data at Y_machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        // =================================================================================
        private void ShowMachine_button_Click(object sender, EventArgs e) {
            double X;
            double Y;

            if (!PrepareSingleComponentOperation(out X, out Y)) {
                return;
            }

            DataGridViewCell cell = CadData_GridView.CurrentCell;
            Cnc.CNC_XY_m(X, Y);
            //bool KnownComponent = ShowFootPrint_m(cell.OwningRow.Index);
            //ShowMessageBox(
            //    "This is " + cell.OwningRow.Cells["Component"].Value.ToString() + " location",
            //    "Locate Component",
            //    MessageBoxButtons.OK);
            //if (KnownComponent)
            //{
            //    DownCamera.DrawBox = false;
            //}
        }


        // =================================================================================
        private void ReMeasure_button_Click(object sender, EventArgs e) {
            ValidMeasurement_checkBox.Checked = false;
            ValidMeasurement_checkBox.Checked = BuildMachineCoordinateData_m();
            // Cnc.CNC_Park();
        }



        #endregion


        // =================================================================================
        // Test functions
        // =================================================================================
        #region test functions

        // =================================================================================
        private void LabelTestButtons() {
            Test1_button.Text = "Pickup this";
            Test2_button.Text = "Place here";
            Test3_button.Text = "Probe (n.c.)";
            Test4_button.Text = "Needle to cam";
            Test5_button.Text = "Probe down";
            Test6_button.Text = "Needle up";
        }

        // test 1

        private void Test1_button_Click(object sender, EventArgs e) {
            double Xmark = Cnc.CurrentX;
            double Ymark = Cnc.CurrentY;
            DisplayText("test 1: Pick up this (probing)");
            PumpOn();
            VacuumOff();
            if (!Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA)) {
                PumpOff();
                return;
            }
            Needle_ProbeDown_m();
            VacuumOn();
            Cnc.Zup();  // pick up
            Cnc.CNC_XY_m(Xmark, Ymark);
        }

        // =================================================================================
        // test 2

        // static int test2_state = 0;
        private void Test2_button_Click(object sender, EventArgs e) {
            double Xmark = Cnc.CurrentX;
            double Ymark = Cnc.CurrentY;
            DisplayText("test 2: Place here (probing)");
            if (!Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA)) {
                return;
            }
            Needle_ProbeDown_m();
            VacuumOff();
            Cnc.Zup();  // back up
            Cnc.CNC_XY_m(Xmark, Ymark);  // show results
        }

        // =================================================================================
        // test 3
        private void Test3_button_Click(object sender, EventArgs e) {
            Xmark = Cnc.CurrentX;
            Ymark = Cnc.CurrentY;
            Needle.ProbingMode(true, JSON);
            Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA);
            //Cnc.CNC_XY_m((Cnc.CurrentX + Properties.Settings.Default.DownCam_NeedleOffsetX), (Cnc.CurrentY + Properties.Settings.Default.DownCam_NeedleOffsetY));
            Needle_ProbeDown_m();
        }


        // =================================================================================
        // test 4

        private void Test4_button_Click(object sender, EventArgs e) {
            double xp = Properties.Settings.Default.UpCam_PositionX;
            double yp = Properties.Settings.Default.UpCam_PositionY;

            double xo = Properties.Settings.Default.DownCam_NeedleOffsetX;
            double yo = Properties.Settings.Default.DownCam_NeedleOffsetY;

            Needle.Move_m(xp - xo, yp - yo, Cnc.CurrentA);
        }

        // =================================================================================
        // test 5

        private double Xmark;
        private double Ymark;
        private void Test5_button_Click(object sender, EventArgs e) {
            Xmark = Cnc.CurrentX;
            Ymark = Cnc.CurrentY;
            Needle.ProbingMode(true, JSON);
            Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA);
            Needle_ProbeDown_m();
        }

        // =================================================================================
        // test 6
        private void Test6_button_Click(object sender, EventArgs e) {
            DisplayText("test 6: Needle up");
            Needle.ProbingMode(false, JSON);
            Cnc.Zup();  // go up
            Cnc.CNC_XY_m(Xmark, Ymark);

            // ShowMessageBox test
            //    Cnc_ReadyEvent.Reset();
            //    Thread t = new Thread(() => test_thread());
            //    t.IsBackground = true;
            //    t.Start();
            //    while (!Cnc_ReadyEvent.IsSet)
            //    {
            //        Thread.Sleep(20);
            //        Application.DoEvents();
            //    }
            //    ShowMessageBox(
            //       "Done",
            //       "test_6",
            //       MessageBoxButtons.OK);
            //}

            //private void test_thread()
            //{
            //    ShowMessageBox(
            //        "err",
            //        "t1",
            //        MessageBoxButtons.OK);
            //    Cnc_ReadyEvent.Set();

        }

        #endregion


        public void ZDown_button_Click(object sender, EventArgs e) {
            Cnc.Zdown();
        }

        public void ZUp_button_Click(object sender, EventArgs e) {
            Cnc.Zup();
        }





        // =================================================================================

        public Shapes.Thing DebugCirclesDownCamera(double Tolerance) {
            var circle = VideoDetection.GetClosestCircle(cameraView.downVideoProcessing, Tolerance);
            if (circle != null) {
                circle.ToMMResolution();
                DisplayText("Circle Detected @ " + circle, Color.Blue);
                return circle;
            }
            DisplayText("No Circle Detected", Color.Red);
            return null;
        }

        // =================================================================================




        /*     private void DebugComponents_Camera(double Tolerance, Camera Cam, double mmPerPixel)
             {
                 double X = 0;
                 double Y = 0;
                 double A = 0.0;
                 int count = MeasureClosestComponentInPx(out X, out Y, out A, Cam, Tolerance, 5);
                 if (count == 0)
                 {
                     DisplayText("No results");
                     return;
                 }
                 X = X * mmPerPixel;
                 Y = -Y * mmPerPixel;
                 DisplayText("Component measurement:");
                 DisplayText("X: " + X.ToString("0.000", CultureInfo.InvariantCulture) + " (" + count.ToString() + " results out of 5)");
                 DisplayText("Y: " + Y.ToString("0.000", CultureInfo.InvariantCulture));
                 DisplayText("A: " + A.ToString("0.000", CultureInfo.InvariantCulture));
             }
             */
        /*     private void DebugCirclesUpCamera(double Tolerance) {
                 var circle = UpCamera.videoDetection.GetClosestCircle(Tolerance);
                 if (circle != null) DisplayText("Circle Found @ " + circle, Color.Blue);
                 else DisplayText("No Circle Found", Color.Red);
             }*/


        // =================================================================================

        private void AddTape_button_Click(object sender, EventArgs e) {
            Tapes.AddTapeObject(0);
        }


        private void SmallMovement_numericUpDown_ValueChanged(object sender, EventArgs e) {
            Properties.Settings.Default.CNC_SmallMovementSpeed = SmallMovement_numericUpDown.Value;
            Cnc.SmallMovementString = "G1 F" + Properties.Settings.Default.CNC_SmallMovementSpeed + " ";
        }

    
        private void TapeUp_button_Click(object sender, EventArgs e) {
            int row = GetGridRow(Tapes_dataGridView);
            Global.MoveItem(Tapes.tapeObjs, row, -1);
        }

        private void TapeDown_button_Click(object sender, EventArgs e) {
            int row = GetGridRow(Tapes_dataGridView);
            Global.MoveItem(Tapes.tapeObjs, row, +1);
        }

        private void DeleteTape_button_Click(object sender, EventArgs e) {
            if (Tapes_dataGridView.SelectedCells.Count != 1) return;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            Tapes_dataGridView.Rows.RemoveAt(row);
        }

        private void TapeGoTo_button_Click(object sender, EventArgs e) {
            if (Tapes_dataGridView.SelectedCells.Count != 1) return;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            Cnc.CNC_XY_m(Tapes.GetTapeObjByIndex(row).FirstHole);
        }

        private void TapeSet1_button_Click(object sender, EventArgs e) {
            if (Tapes_dataGridView.SelectedCells.Count != 1) return;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            // Calibrate the selected tape
            Tapes.CalibrateTape(Tapes.GetTapeObjByIndex(row));
        }

        // =======================  Configure settings based on checkboxes ============================





        // ==========================================================================================================
        // Video processign functions lists control
        // ==========================================================================================================
        #region VideoProcessingFunctionsLists





        // ==========================================================================================================
        // Parameter labels and control widgets:
        // ==========================================================================================================
        // Sharing the labels and some controls so I don't need to duplicate so much code:




        #endregion


        // ==========================================================================================================
        // Tapes_dataGridView_CellClick(): 
        // If the click is on a button column, resets the tape. 
        private void Tapes_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e) {
            // Ignore clicks that are not on button cell  IdColumn
            if ((e.RowIndex < 0) || (e.ColumnIndex != Tapes_dataGridView.Columns["SelectButtonColumn"].Index)) {
                return;
            }
            Tapes.Reset(e.RowIndex);
            Update_GridView(Tapes_dataGridView);
        }


        // ==========================================================================================================
        // SelectTape(): 
        // Displays a dialog with buttons for all defined tapes, returns the ID of the tape.
        // used in placement when the user selects the tape to be used in runtime.
        // In this case, we remove the regular Tapes_dataGridView_CellClick() handler; the TapeDialog
        // changes the button text to "select" (and back to "reset" on return) and uses its own handler.
        private string SelectTape(string header) {
            Point loc = Tapes_dataGridView.Location;
            Size size = Tapes_dataGridView.Size;
            DataGridView Grid = Tapes_dataGridView;
            TapeSelectionForm TapeDialog = new TapeSelectionForm(Tapes);
            TapeDialog.HeaderString = header;
            Tapes_dataGridView.CellClick -= Tapes_dataGridView_CellClick;
            Controls.Remove(Tapes_dataGridView);

            TapeDialog.ShowDialog(this);

            string ID = TapeDialog.ID;  // get the result
            DisplayText("Selected tape: " + ID);

            Tapes_tabPage.Controls.Add(Tapes_dataGridView);
            Tapes_dataGridView = Grid;
            Tapes_dataGridView.Location = loc;
            Tapes_dataGridView.Size = size;

            TapeDialog.Dispose();
            Tapes_dataGridView.CellClick += Tapes_dataGridView_CellClick;
            return ID;
        }

        // XXX GOOZ I might have broken this
        private void ResetOneTape_button_Click(object sender, EventArgs e) {
            var rows = JobData_GridView.SelectedRows;
            if (rows.Count != 1) return;
            Tapes.GetTapeObjByID(rows[0].Cells[3].Value.ToString()).SetPart(1);
        }

        private void ResetAllTapes_button_Click(object sender, EventArgs e) {
            Tapes.ClearAll();
        }

        private void SetPartNo_button_Click(object sender, EventArgs e) {
            int no = 0;
            if (!int.TryParse(NextPart_TextBox.Text, out no)) {
                return;
            }
            DataGridViewRow Row = Tapes_dataGridView.Rows[Tapes_dataGridView.CurrentCell.RowIndex];
            Row.Cells["Next_Column"].Value = no.ToString();
        }




/*
        // will go to the nearest hole to the next part and display what it will pick up gooz
        private void Tape_GoToNext_button_Click(object sender, EventArgs e) {
            if (Tapes_dataGridView.SelectedCells.Count != 1) return;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;

            TapeObj to = new TapeObj(Tapes.GetTapeRow(row));
            Tapes.SetCurrentTapeMeasurement_m(to.Type); //setup tape type to measure

            // move to closest hole to the part we are looking for
            Cnc.CNC_XY_m(to.GetNearestCurrentPartHole());
            var hole = CenterCameraOnCircle();

            // move camera on top of the part, and then move from there to the part to pick up
            var offset = to.GetCurrentPartLocation() - to.GetNearestCurrentPartHole();
            Cnc.CNC_XY_m(hole + offset);

            // clear marks
            // DownCamera.MarkA.Clear();
            // DownCamera.MarkB.Clear();

            // mark location
            // DownCamera.MarkB.Add((to.GetCurrentPartLocation() - hole).ToPixels().ToPointF());

        }
*/

        private void Tape_resetZs_button_Click(object sender, EventArgs e) {
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            Tapes.GetTapeObjByIndex(row).PlaceZ = -1;
            Tapes.GetTapeObjByIndex(row).PickupZ = -1;
        }


        private void ChangeNeedle_button_Click(object sender, EventArgs e) {
            ChangeNeedle_m();
        }

        private void ZTestTravel_textBox_TextChanged(object sender, EventArgs e) {
            double val;
            if (double.TryParse(ZTestTravel_textBox.Text, out val)) {
                Properties.Settings.Default.General_ZTestTravel = val;
            }

        }


        /*
        // some stuff for testing
        private void tape_ViewComponents_button_Click(object sender, EventArgs e) {
            var DownCamera = cameraView.downVideoProcessing;
            // current index
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            if (row == -1) return;

            TapeObj to = new TapeObj(Tapes.GetTapeRow(row));

            // move to first hole
            Cnc.CNC_XY_m(to.FirstHole.X, to.FirstHole.Y);

            cameraView.SetDownCameraDefaults();
            // show current hole, next hole, and first 5 parts
            for (int i = 0; i < 3; i++) {
                var hole = to.GetHoleLocation(i) - to.GetHoleLocation(0);
                DownCamera.MarkA.Add(Shapes.Thing.Convert(DownCamera, hole, Shapes.PointMode.MM, Shapes.PointMode.Screen).ToPointF());
            }

            for (int i = to.CurrentPartIndex(); i < to.CurrentPartIndex() + 5; i++) {
                var part = to.GetPartLocation(i) - to.GetHoleLocation(0);
                DownCamera.MarkB.Add(Shapes.Thing.Convert(DownCamera, part, Shapes.PointMode.MM, Shapes.PointMode.Screen).ToPointF());
            }



        }
        */
        /// <summary>
        /// Center camera on nearest hole and return hole location
        /// </summary>
        public PartLocation CenterCameraOnCircle() {
            double X, Y;

            if (!GoToLocation_m(Shapes.ShapeTypes.Circle, 1.8, 0.5, out X, out Y)) return null;
            return new PartLocation(Cnc.CurrentX + X, Cnc.CurrentY + Y);
        }

        /// <summary>
        /// test to see how well we pick up the parts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pickup_next_button_Click(object sender, EventArgs e) {
            if (Tapes_dataGridView.SelectedCells.Count != 1) return;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            var name = Tapes.GetTapeObjByIndex(row).ID;

            if (!Tapes.GotoNextPart_m(name)) return;

            PumpOn();
            VacuumOff();
            Needle_ProbeDown_m();
            VacuumOn();
            Cnc.Zup();  // pick up

        }

        private void button1_Click(object sender, EventArgs e) {
            // set angle to zero
            Cnc.CNC_A_m(0);
        }

        private void button2_Click(object sender, EventArgs e) {
            var loc = Cnc.XYALocation;
            Cnc.Zup(); //needle up
            for (int i = 0; i <= 360; i += 90) {
                Needle.Move_m(loc); // move to target
                Cnc.ZGuardOff();
                Cnc.CNC_Z_m(Properties.Settings.Default.General_ZtoPCB);//move down
                Thread.Sleep(1000); //wait 1 second
                Cnc.ZGuardOn(); //Needle up
                Cnc.Zup();

                loc.A = i;
            }

        }

        private void MultiCalibrate_button_Click(object sender, EventArgs e) {
            ((Button)sender).BackColor = Color.Yellow;
            if (!MechanicalHoming_m()) return;
            if (!OpticalHoming_m()) return;
            TestNeedleRecognition_button_Click(null, null);
            ReMeasure_button_Click(null, null);
            Cnc.CNC_XYA_m(0, 0, 0); //go back home
            ((Button)sender).BackColor = Color.Chartreuse;


        }



        int tape_index;
        private void view_nextParts_button_Click(object sender, EventArgs e) {
            cameraView.SetDownCameraFunctionSet("");

            var list = Tapes.GetListOfTapeIDs();
            if (tape_index == list.Count() - 1) tape_index = 0;

            var tape = Tapes.GetTapeObjByID(list[tape_index]);
            DisplayText("Examining Tape " + list[tape_index] + " NextPart=" + tape.CurrentPartIndex(), Color.Green);

            Cnc.CNC_XY_m(tape.GetCurrentPartLocation());
       //     tape.myRow["IdColumn"].Style.BackColor = Color.Yellow;
         //   tape.myRow["Next_Column"].Selected = true;
            tape_index++;
            //  DownCamera.AddMarkupText(new VideoTextMarkup(DownCamera, new PartLocation(0,0), tape.ID + " #" + tape.CurrentPartIndex()));

            // Thread.Sleep(5000);
            //  DownCamera.ClearMarkupText();

        }

        private void needle_calibration_test_button_Click(object sender, EventArgs e) {
            AutoCalibration.DoNeedleErrorMeasurement(cameraView.upVideoProcessing);
        }

        private void mechHome_button_Click(object sender, EventArgs e) {
            MechanicalHoming_m();
        }

        private void OptHome_button_Click(object sender, EventArgs e) {
            OpticalHoming_m();
        }



        private void DownCamera_Calibration_button_Click(object sender, EventArgs e) {
            var ret = AutoCalibration.UpCamera_Calibration(cameraView, 1d);
            // update values
            Properties.Settings.Default.UpCam_XmmPerPixel = ret.X;
            Properties.Settings.Default.UpCam_YmmPerPixel = ret.Y;
            UpCameraBoxXmmPerPixel_label.Text = "(" + ret.X.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            UpCameraBoxYmmPerPixel_label.Text = "(" + ret.Y.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            UpCameraBoxX_textBox.Text = (ret.X * cameraView.downVideoProcessing.box.Width).ToString("0.000", CultureInfo.InvariantCulture);
            UpCameraBoxY_textBox.Text = (ret.Y * cameraView.downVideoProcessing.box.Height).ToString("0.000", CultureInfo.InvariantCulture);

        }


        private void EndEditModeForTapeSelection(Object sender, EventArgs e) {
            var dgv = (DataGridView)sender;
             if (dgv.CurrentCell.GetType() == typeof(DataGridViewComboBoxCell) && (dgv.IsCurrentCellDirty))
                 dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void loadCADFileToolStripMenuItem_Click(object sender, EventArgs e) {
            ValidMeasurement_checkBox.Checked = false;
            if (LoadCadData_m()) {
                // Read in job data (.lpj file), if exists
                string ext = Path.GetExtension(CadDataFileName);
                JobFileName = CadDataFileName.Replace(ext, ".lpj");
                if (File.Exists(JobFileName)) {
                    if (!LoadJobData_m(JobFileName)) {
                        ShowMessageBox(
                            "Attempt to read in existing Job Data file failed. Job data automatically created, review situation!",
                            "Job Data load error",
                            MessageBoxButtons.OK);
                        Cad.AutoFillJobEntry();
                    }
                } else {
                    // If not, build job data ourselves.
                    Cad.AutoFillJobEntry();
                }
            } else {
                // CAD data load failed, clear to false data
                CadData_GridView.Rows.Clear();
                CadDataFileName = "--";
            }
        }

        private void loadJobFileToolStripMenuItem_Click(object sender, EventArgs e) {
            if (Job_openFileDialog.ShowDialog() == DialogResult.OK) {
                JobFileName = Job_openFileDialog.FileName;
                LoadJobData_m(JobFileName);
                Cad.CopyComponentsFromJob(); // repopulate component data based on saved file
                ValidMeasurement_checkBox.Checked = false;
            }
        }

        private void saveJobFileToolStripMenuItem_Click(object sender, EventArgs e) {
            Job_saveFileDialog.Filter = "LitePlacer Job files (*.lpj)|*.lpj|All files (*.*)|*.*";
            if (Job_saveFileDialog.ShowDialog() == DialogResult.OK) {
                Global.Serialization(Cad.JobData, Job_saveFileDialog.FileName);
                JobFileName = Job_saveFileDialog.FileName;
            }
        }

  


    }	// end of: 	public partial class FormMain : Form

    // allows additionl of color info to displayText
    public static class RichTextBoxExtensions {
        public static void AppendText(this RichTextBox box, string text, Color color) {
            if (color != box.ForeColor) {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;
            } else {
                box.AppendText(text);
            }
        }



    }

}	// end of: namespace LitePlacer
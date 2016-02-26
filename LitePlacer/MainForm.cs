#define TINYG_SHORTUNITS
// Some firmvare versions use units in millions, some don't. If not, comment out the above line.

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
using System.Drawing.Imaging;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Net;

using MathNet.Numerics;
using HomographyEstimation;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;


namespace LitePlacer
{
    // Note: For function success/failure, I use bool return code. (instead of C# exceptions; a philosophical debate, let's not go there.)
    // The naming convention is xxx_m() for functions that have already displayed an error message to user. If a function only
    // calls _m functions, it can consider itself a _m function. 

    public partial class FormMain : Form
    {
        CNC Cnc;
        Camera DownCamera;
        Camera UpCamera;
        NeedleClass Needle;
        TapesClass Tapes;

        // =================================================================================
        // General and "global" functions 
        // =================================================================================
        #region General

        // =================================================================================
        // Note about thread guards: The prologue "if(InvokeRequired) {something long}" at a start of a function, 
        // makes the function safe to call from another thread.
        // See http://stackoverflow.com/questions/661561/how-to-update-the-gui-from-another-thread-in-c, 
        // "MajesticRa"'s answer near the bottom of first page


        // =================================================================================
        // Thread safe dialog box:
        // (see http://stackoverflow.com/questions/559252/does-messagebox-show-automatically-marshall-to-the-ui-thread )

        public DialogResult ShowMessageBox(String message, String header, MessageBoxButtons buttons)
        {
            if (this.InvokeRequired)
            {
                return (DialogResult)this.Invoke(new PassStringStringReturnDialogResultDelegate(ShowMessageBox), message, header, buttons);
            }
            return MessageBox.Show(this, message, header, buttons);
        }
        public delegate DialogResult PassStringStringReturnDialogResultDelegate(String s1, String s2, MessageBoxButtons buttons);

        // =================================================================================


        // We need some functions both in JSON and in text mode:
        public const bool JSON = true;
        public const bool TextMode = false;



        private static ManualResetEventSlim Cnc_ReadyEvent = new ManualResetEventSlim(false);
        // This event is raised in the CNC class, and we'll wait for it when we want to continue only after TinyG has stabilized

        public FormMain()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(MouseWheel_event);
        }

        // =================================================================================
        public bool StartingUp = false; // we want to react to some changes, but not during startup data load (which is counts as a change)

        private void Form1_Load(object sender, EventArgs e)
        {
            StartingUp = true;
            this.Size = new Size(1280, 900);
            DisplayText("Application Start");

            Do_Upgrade();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Cnc = new CNC(this);
            Cnc_ReadyEvent = Cnc.ReadyEvent;
            CNC.SquareCorrection = Properties.Settings.Default.CNC_SquareCorrection;
            DownCamera = new Camera(this);
            UpCamera = new Camera(this);
            Needle = new NeedleClass(UpCamera, Cnc, this);
            Tapes = new TapesClass(Tapes_dataGridView, CustomTapes_dataGridView, Needle, DownCamera, Cnc, this);

            // Setup error handling for Tapes_dataGridView
            // This is necessary, because programmatically changing a combobox cell value raises this error. (@MS: booooo!)
            Tapes_dataGridView.DataError += new DataGridViewDataErrorEventHandler(Tapes_dataGridView_DataError);

            this.KeyPreview = true;
            RemoveCursorNavigation(this.Controls);
            // this.KeyDown += new KeyEventHandler(My_KeyDown);
            this.KeyUp += new KeyEventHandler(My_KeyUp);

            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            int i = path.LastIndexOf('\\');
            path = path.Remove(i + 1);
            // LoadDataGrid(path + "LitePlacer.ComponentData", ComponentData_dataGridView);
            LoadDataGrid(path + "LitePlacer.TapesData", Tapes_dataGridView);
            LoadDataGrid(path + "LitePlacer.CustomTapes", CustomTapes_dataGridView);
            Tapes.AddCustomTapesToTapes();
            Tapes.AddWidthValues();

            // 
            LoadDataGrid(path + "LitePlacer.HomingFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref Homing_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.FiducialsFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref Fiducials_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.ComponentsFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref Components_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.PaperTapeFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref PaperTape_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.BlackTapeFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref BlackTape_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.ClearTapeFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref ClearTape_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.SnapshotFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref DowncamSnapshot_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.NeedleFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref Needle_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.UpCamComponentsFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref UpCamComponents_dataGridView, false);

            LoadDataGrid(path + "LitePlacer.UpCamSnapshotFunctions", Temp_dataGridView);
            DataGridViewCopy(Temp_dataGridView, ref UpcamSnapshot_dataGridView, false);

            SetProcessingFunctions(Display_dataGridView);
            SetProcessingFunctions(Homing_dataGridView);
            SetProcessingFunctions(Fiducials_dataGridView);
            SetProcessingFunctions(Components_dataGridView);
            SetProcessingFunctions(PaperTape_dataGridView);
            SetProcessingFunctions(BlackTape_dataGridView);
            SetProcessingFunctions(ClearTape_dataGridView);
            SetProcessingFunctions(DowncamSnapshot_dataGridView);
            SetProcessingFunctions(Needle_dataGridView);
            SetProcessingFunctions(UpCamComponents_dataGridView);
            SetProcessingFunctions(UpcamSnapshot_dataGridView);

            Bookmark1_button.Text = Properties.Settings.Default.General_Mark1Name;
            Bookmark2_button.Text = Properties.Settings.Default.General_Mark2Name;
            Bookmark3_button.Text = Properties.Settings.Default.General_Mark3Name;
            Bookmark4_button.Text = Properties.Settings.Default.General_Mark4Name;
            Bookmark5_button.Text = Properties.Settings.Default.General_Mark5Name;
            Bookmark6_button.Text = Properties.Settings.Default.General_Mark6Name;
            Mark1_textBox.Text = Properties.Settings.Default.General_Mark1Name;
            Mark2_textBox.Text = Properties.Settings.Default.General_Mark2Name;
            Mark3_textBox.Text = Properties.Settings.Default.General_Mark3Name;
            Mark4_textBox.Text = Properties.Settings.Default.General_Mark4Name;
            Mark5_textBox.Text = Properties.Settings.Default.General_Mark5Name;
            Mark6_textBox.Text = Properties.Settings.Default.General_Mark6Name;

            // The components tab is more a distraction than useful.
            // To add data, comment out the next line.
            tabControlPages.TabPages.Remove(Components_tabPage);

            Zlb_label.Text = "";
            Zlb_label.Visible = false;

            Display_dataGridView.DataError += new
                DataGridViewDataErrorEventHandler(Display_dataGridView_DataError);
        }

        // ==============================================================================================
        // New software release checks

        private string BuildDate()
        {
            // see http://stackoverflow.com/questions/1600962/displaying-the-build-date
            var version = Assembly.GetEntryAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)
            return buildDateTime.ToString();
        }

        private bool UpdateAvailable(out string UpdateDescription)
        {
            try
            {
                var url = "http://www.liteplacer.com/Downloads/release.txt";
                UpdateDescription = (new WebClient()).DownloadString(url);
                string UpdateDate = "";
                for (int i = 0; i < UpdateDescription.Length; i++)
                {
                    if (UpdateDescription[i] == '\n')
                    {
                        break;
                    }
                    UpdateDate += UpdateDescription[i];
                }
                string BuildDateText = "Build date " + BuildDate().Substring(0, 10);
                BuildDateText = BuildDateText.Trim();
                if (UpdateDate != BuildDateText)
                {
                    return true;
                }
            }
            catch
            {
                DisplayText("Could not read http://www.liteplacer.com/Downloads/release.txt, update info not available.");
                UpdateDescription = "";
                return false;
            }
            return false;
        }

        private bool CheckForUpdate()
        {
            string UpdateText;
            if (UpdateAvailable(out UpdateText))
            {
                ShowMessageBox(
                    "There is software update available:\n\r" + UpdateText,
                    "Update available",
                    MessageBoxButtons.OK);
                return true;
            }
            return false;
        }

        private void CheckNow_button_Click(object sender, EventArgs e)
        {
            if (!CheckForUpdate())
            {
                ShowMessageBox(
                    "The software is up to date",
                    "Up to date",
                    MessageBoxButtons.OK);
            }
        }

        // ==============================================================================================
        // For diagnostics, log button presses. (Customers tend to send log window contents,
        // but the window does not log user actions without this.)
        // http://stackoverflow.com/questions/17949390/log-all-button-clicks-in-win-forms-app

        public void AttachButtonLogging(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (var control in controls.Cast<System.Windows.Forms.Control>())
            {
                if (control is Button)
                {
                    Button button = (Button)control;
                    button.MouseDown += LogButtonClick; // MoseDown comes before mouse click, we want this to fire first)
                }
                else
                {
                    AttachButtonLogging(control.Controls);
                }
            }
        }

        private void LogButtonClick(object sender, EventArgs eventArgs)
        {

            Button button = sender as Button;
            DisplayText(button.Text.ToString(), KnownColor.DarkGreen);
        }
        // ==============================================================================================

        private string LastTabPage = "";

        // ==============================================================================================
        private void FormMain_Shown(object sender, EventArgs e)
        {
            LabelTestButtons();
            AttachButtonLogging(this.Controls);

            DisplayText("Version: " + Assembly.GetEntryAssembly().GetName().Version.ToString() + ", build date: " + BuildDate());
            CheckForUpdate_checkBox.Checked = Properties.Settings.Default.General_CheckForUpdates;
            if (CheckForUpdate_checkBox.Checked)
            {
                CheckForUpdate();
            }

            OmitNeedleCalibration_checkBox.Checked = Properties.Settings.Default.Placement_OmitNeedleCalibration;
            SkipMeasurements_checkBox.Checked = Properties.Settings.Default.Placement_SkipMeasurements;

            DownCamZoom_checkBox.Checked = Properties.Settings.Default.DownCam_Zoom;
            DownCamera.Zoom = Properties.Settings.Default.DownCam_Zoom;
            DownCamZoomFactor_textBox.Text = Properties.Settings.Default.DownCam_Zoomfactor.ToString("0.0", CultureInfo.InvariantCulture);
            DownCamera.ZoomFactor = Properties.Settings.Default.DownCam_Zoomfactor;

            UpCamZoom_checkBox.Checked = Properties.Settings.Default.UpCam_Zoom;
            UpCamera.Zoom = Properties.Settings.Default.UpCam_Zoom;
            UpCamZoomFactor_textBox.Text = Properties.Settings.Default.UpCam_Zoomfactor.ToString("0.0", CultureInfo.InvariantCulture);
            UpCamera.ZoomFactor = Properties.Settings.Default.UpCam_Zoomfactor;

            RobustFast_checkBox.Checked = Properties.Settings.Default.Cameras_RobustSwitch;
            KeepActive_checkBox.Checked = Properties.Settings.Default.Cameras_KeepActive;
            if (KeepActive_checkBox.Checked)
            {
                RobustFast_checkBox.Enabled = false;
            }
            else
            {
                RobustFast_checkBox.Enabled = true;
            }
            RobustFast_checkBox.Checked = Properties.Settings.Default.Cameras_RobustSwitch;

            StartCameras();
            tabControlPages.SelectedTab = tabPageBasicSetup;
            LastTabPage = "tabPageBasicSetup";

            Cnc.SlackCompensation = Properties.Settings.Default.CNC_SlackCompensation;
            SlackCompensation_checkBox.Checked = Properties.Settings.Default.CNC_SlackCompensation;
            Cnc.SlackCompensationA = Properties.Settings.Default.CNC_SlackCompensationA;
            SlackCompensationA_checkBox.Checked = Properties.Settings.Default.CNC_SlackCompensationA;
            Cnc.SmallMovementString = "G1 F" + Properties.Settings.Default.CNC_SmallMovementSpeed + " ";

            MouseScroll_checkBox.Checked = Properties.Settings.Default.CNC_EnableMouseWheelJog;
            NumPadJog_checkBox.Checked = Properties.Settings.Default.CNC_EnableNumPadJog;

            ZTestTravel_textBox.Text = Properties.Settings.Default.General_ZTestTravel.ToString();
            ShadeGuard_textBox.Text = Properties.Settings.Default.General_ShadeGuard_mm.ToString();

            Z0_textBox.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture);
            BackOff_textBox.Text = Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture);
            PlacementDepth_textBox.Text = Properties.Settings.Default.Placement_Depth.ToString("0.00", CultureInfo.InvariantCulture);

            UpdateCncConnectionStatus();
            if (Cnc.Connected)
            {
                Thread.Sleep(200); // Give TinyG time to wake up
                CNC_RawWrite("\x11");  // Xon
                Thread.Sleep(50);
                //CNC_RawWrite("{\"js\":1}");  // strict JSON syntax
                //Thread.Sleep(150);
                //CNC_RawWrite("{\"ec\":0}");  // send LF only
                //Thread.Sleep(150);
                UpdateWindowValues_m();
            }
 
            DisableLog_checkBox.Checked = Properties.Settings.Default.General_MuteLogging;
            StartingUp = false;
        }

        // =================================================================================

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CNC_EnableMouseWheelJog = MouseScroll_checkBox.Checked;
            Properties.Settings.Default.CNC_EnableNumPadJog = NumPadJog_checkBox.Checked;
            Properties.Settings.Default.General_CheckForUpdates = CheckForUpdate_checkBox.Checked;
            Properties.Settings.Default.General_MuteLogging = DisableLog_checkBox.Checked;

            Properties.Settings.Default.Save();
            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            int i = path.LastIndexOf('\\');
            path = path.Remove(i + 1);
            SaveDataGrid(path + "LitePlacer.ComponentData", ComponentData_dataGridView);
            SaveDataGrid(path + "LitePlacer.TapesData", Tapes_dataGridView);
            SaveDataGrid(path + "LitePlacer.CustomTapes", CustomTapes_dataGridView);

            DataGridViewCopy(Homing_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.HomingFunctions", Temp_dataGridView);

            DataGridViewCopy(Fiducials_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.FiducialsFunctions", Temp_dataGridView);

            DataGridViewCopy(Components_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.ComponentsFunctions", Temp_dataGridView);

            DataGridViewCopy(PaperTape_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.PaperTapeFunctions", Temp_dataGridView);

            DataGridViewCopy(BlackTape_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.BlackTapeFunctions", Temp_dataGridView);

            DataGridViewCopy(ClearTape_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.ClearTapeFunctions", Temp_dataGridView);

            DataGridViewCopy(DowncamSnapshot_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.SnapshotFunctions", Temp_dataGridView);

            DataGridViewCopy(Needle_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.NeedleFunctions", Temp_dataGridView);

            DataGridViewCopy(UpCamComponents_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.UpCamComponentsFunctions", Temp_dataGridView);

            DataGridViewCopy(UpcamSnapshot_dataGridView, ref Temp_dataGridView, false);
            SaveDataGrid(path + "LitePlacer.UpCamSnapshotFunctions", Temp_dataGridView);

            if (Cnc.Connected)
            {
                PumpIsOn = true;        // so it will be turned off, no matter what we think the status
                PumpOff_NoWorkaround();
                VacuumDefaultSetting();
                CNC_Write_m("{\"md\":\"\"}");  // motor power off
            }
            Cnc.Close();

            if (DownCamera.IsRunning())
            {
                DownCamera.Close();
                Thread.Sleep(200);   // camera feed needs a frame to close.
            }
            if (UpCamera.IsRunning())
            {
                UpCamera.Close();
                Thread.Sleep(200);   // camera feed needs a frame to close.
            }
            Thread.Sleep(200);
        }

        // =================================================================================
        // Get and save settings from old version if necessary
        // http://blog.johnsworkshop.net/automatically-upgrading-user-settings-after-an-application-version-change/

        private void Do_Upgrade()
        {
            try
            {
                if (Properties.Settings.Default.General_UpgradeRequired)
                {
                    DisplayText("Updating from previous version");
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.General_UpgradeRequired = false;
                    Properties.Settings.Default.Save();
                }
            }
            catch (SettingsPropertyNotFoundException)
            {
                DisplayText("Updating from previous version (through ex)");
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.General_UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

        }

        // =================================================================================
        public bool SetupCamerasPageVisible = false;

        private void tabControlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupCamerasPageVisible = false;
            switch (tabControlPages.SelectedTab.Name)
            {
                case "RunJob_tabPage":
                    RunJob_tabPage_Begin();
                    LastTabPage = "RunJob_tabPage";
                    break;
                case "tabPageBasicSetup":
                    BasicSetupTab_Begin();
                    LastTabPage = "tabPageBasicSetup";
                    break;
                case "tabPageSetupCameras":
                    SetupCamerasPageVisible = true;
                    tabPageSetupCameras_Begin();
                    LastTabPage = "tabPageSetupCameras";
                    break;
                case "Tapes_tabPage":
                    Tapes_tabPage_Begin();
                    LastTabPage = "Tapes_tabPage";
                    break;
            }
        }

        private void tabControlPages_Selecting(object sender, TabControlCancelEventArgs e)
        {
            switch (LastTabPage)
            {
                case "RunJob_tabPage":
                    RunJob_tabPage_End();
                    break;
                case "tabPageBasicSetup":
                    BasicSetupTab_End();
                    break;
                case "tabPageSetupCameras":
                    tabPageSetupCameras_End();
                    break;
                case "Tapes_tabPage":
                    Tapes_tabPage_End();
                    break;
            }
        }

        // =================================================================================
        // Saving and restoring data tables (Note: Not job files)
        // =================================================================================

        // Reading ver2 format allows changing the data grid itself at a software update, 
        // adding and removing columns, and still read in a saved file from previous software version.

        private int Ver2FormatID = 20000001;
        public bool LoadingDataGrid = false;  // to avoid problems with cell value changed event and unfilled grids


        public void LoadDataGrid(string FileName, DataGridView dgv)
        {
            try
            {
                bool Ver2 = false;
                LoadingDataGrid = true;
                int first;

                // Does version 2 files exist?
                Ver2 = (File.Exists(FileName+"_v2"));
                if(Ver2)
                {
                    FileName = FileName + "_v2";
                }

                if (!File.Exists(FileName))
                {
                    return;
                }
                
                using (BinaryReader br = new BinaryReader(File.Open(FileName, FileMode.Open)))
                {
                    dgv.Rows.Clear();
                    if (Ver2)
                    {
                        DisplayText("Reading v2 format file " + FileName);
                        first = br.ReadInt32();
                    }
                    else
                    {
                        DisplayText("Reading v1 format file " + FileName);
                    }

                    int cols = br.ReadInt32();
                    int rows = br.ReadInt32();

                    if (dgv.AllowUserToAddRows)
                    {
                        // There is an empty row in the bottom that is visible for manual add.
                        // It is saved in the file. It is automatically added, so we don't want to add it again.
                        rows = rows - 1;
                    }
                    // read headers;
                    List<string> Headers = new List<string>();

                    if (Ver2)
	                {
                        for (int j = 0; j < cols; ++j)
                        {
                            Headers.Add(br.ReadString());
                        }
	                }
                    else
                    {
                        Headers = Addv1Headers(FileName);
                    }

                    // read data
                    int i_out;
                    for (int i = 0; i < rows; ++i)
                    {
                        dgv.Rows.Add();
                        for (int j = 0; j < cols; ++j)
                        {
                            if (MapHeaders(dgv, Headers, j, out i_out))
                            {
                                if (br.ReadBoolean())
                                {
                                    dgv.Rows[i].Cells[i_out].Value = br.ReadString();
                                }
                                else br.ReadBoolean();
                                //if ((dgv.Rows[i].Cells[i_out].Value == null) && (dgv.Rows[i].Cells[i_out].ValueType == typeof(DataGridViewTextBoxColumn)))
                                //{
                                //    dgv.Rows[i].Cells[i_out].Value = "--";
                                //}
                                //if (dgv.Rows[i].Cells[i_out].Value.ToString() == "")
                                //{
                                //    dgv.Rows[i].Cells[i_out].Value = "--";
                                //}
                            }
                            else
                            {
                                // column is removed: dummy read, discard the data
                                if (br.ReadBoolean())
                                {
                                    br.ReadString();
                                }
                                else br.ReadBoolean();
                            }
                        }
                    }
                }
                LoadingDataGrid = false;
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
                LoadingDataGrid = false;
            }
        }

        private bool MapHeaders(DataGridView Grid, List<string> Headers, int i_in, out int i_out)
        {
            // i_in= column index of the read-in data
            // sets i_out to the column index of the currect header in Grid
            // returns if match was found
            i_out = -1;
            string label = Headers[i_in];
            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                if (Grid.Columns[i].Name == label)
                {
                    i_out = i;
                    return true;
                }
            }
            return false;
        }



        public void SaveDataGrid(string FileName, DataGridView dgv)
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(FileName+"_v2", FileMode.Create)))
                {
                    bw.Write(Ver2FormatID);
                    bw.Write(dgv.Columns.Count);
                    bw.Write(dgv.Rows.Count);
                    LoadingDataGrid = false;

                    // write headers
                    foreach (DataGridViewColumn column in dgv.Columns)
                    {
                        bw.Write(column.Name);
                    }

                    foreach (DataGridViewRow dgvR in dgv.Rows)
                    {
                        for (int j = 0; j < dgv.Columns.Count; ++j)
                        {
                            object val = dgvR.Cells[j].Value;
                            if (val == null)
                            {
                                bw.Write(false);
                                bw.Write(false);
                            }
                            else
                            {
                                bw.Write(true);
                                bw.Write(val.ToString());
                            }
                        }
                    }
                }
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
            }
        }

        // =================================================================================
        // To be able to change columns and read in old format data file, we need to manually set the old
        // format headers, since I didn't have the insight to write them to the file forom beginning.
        // this routine does it, called from LoadDataGrid()

        public List<string> Addv1Headers(string filename)
        {
            List<string> Headers = new List<string>();
            int dot = filename.LastIndexOf('.');
            string filetype = filename.Substring(dot);

            switch (filetype)
            {
                case ".TapesData":
                    Headers.Add("SelectButtonColumn");
                    Headers.Add("IdColumn");
                    Headers.Add("OrientationColumn");
                    Headers.Add("RotationColumn");
                    Headers.Add("WidthColumn");
                    Headers.Add("TypeColumn");
                    Headers.Add("Next_Column");
                    Headers.Add("Tray_Column");
                    Headers.Add("X_Column");
                    Headers.Add("Y_Column");
                    Headers.Add("PickupZ_Column");
                    Headers.Add("PlaceZ_Column");
                    Headers.Add("NextX_Column");
                    Headers.Add("NextY_column");
                break;

                case ".CustomTapes":
                    Headers.Add("Name_Column");
                    Headers.Add("PitchColumn");
                    Headers.Add("UsesLocationMarks_Column");
                    Headers.Add("PartOffsetX_Column");
                    Headers.Add("PartOffsetY_Column");
                break;

                case ".ComponentData":
                    Headers.Add("PartialName_column");
                    Headers.Add("SizeX_column");
                    Headers.Add("SizeY_column");
                break;

                case ".HomingFunctions":
                case ".FiducialsFunctions":
                case ".ComponentsFunctions":
                case ".PaperTapeFunctions":
                case ".BlackTapeFunctions":
                case ".ClearTapeFunctions":
                case ".SnapshotFunctions":
                case ".NeedleFunctions":
                case ".UpCamComponentsFunctions":
                case ".UpCamSnapshotFunctions":
                    Headers.Add("Funct_column");
                    Headers.Add("Enabled_column");
                    Headers.Add("Int1_column");
                    Headers.Add("Double1_column");
                    Headers.Add("R_column");
                    Headers.Add("G_column");
                    Headers.Add("B_column");
                break;


                default:
                    ShowMessageBox(
                        "*** Header description for " + filetype + " file missing. Programmer's error. ***",
                        "Sloppy programmer",
                        MessageBoxButtons.OK);
                break;
            }

            return Headers;
        }

        // =================================================================================
        // This routine reads in old format file
        public void LoadDataGrid_V1(string FileName, DataGridView dgv)
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    return;
                }
                LoadingDataGrid = true;
                dgv.Rows.Clear();
                using (BinaryReader bw = new BinaryReader(File.Open(FileName, FileMode.Open)))
                {
                    int Cols = bw.ReadInt32();
                    int Rows = bw.ReadInt32();
                    string debug = "foo";
                    if (dgv.AllowUserToAddRows)
                    {
                        // There is an empty row in the bottom that is visible for manual add.
                        // It is saved in the file. It is automatically added, so we don't want to add it also.
                        // It is not there when rows are added only programmatically, so we need to do it here.
                        Rows = Rows - 1;
                    }
                    for (int i = 0; i < Rows; ++i)
                    {
                        dgv.Rows.Add();
                        for (int j = 0; j < Cols; ++j)
                        {
                            if (bw.ReadBoolean())
                            {
                                debug = bw.ReadString();
                                dgv.Rows[i].Cells[j].Value = "";
                                dgv.Rows[i].Cells[j].Value = debug;
                            }
                            else bw.ReadBoolean();
                            if (dgv.Rows[i].Cells[j].Value == null)
                            {
                                dgv.Rows[i].Cells[j].Value = "--";
                            }
                            if (dgv.Rows[i].Cells[j].Value.ToString() == "")
                            {
                                dgv.Rows[i].Cells[j].Value = "--";
                            }
                        }
                    }
                }
                LoadingDataGrid = false;
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
                LoadingDataGrid = false;
            }
        }

        // =================================================================================
        // Scrolling & arranging datagridviews...
        // =================================================================================
        private void DataGrid_Up_button(DataGridView Grid)
        {
            int row = Grid.CurrentCell.RowIndex;
            int col = Grid.CurrentCell.ColumnIndex;
            if (row == 0)
            {
                return;
            }
            if (Grid.SelectedCells.Count != 1)
            {
                return;
            }
            DataGridViewRow temp = Grid.Rows[row];
            Grid.Rows.Remove(Grid.Rows[row]);
            Grid.Rows.Insert(row - 1, temp);
            Grid.ClearSelection();
            Grid.CurrentCell = Grid[col, row - 1];
            HandleGridScrolling(true, Grid);
        }

        private void DataGrid_Down_button(DataGridView Grid)
        {
            int row = Grid.CurrentCell.RowIndex;
            int col = Grid.CurrentCell.ColumnIndex;
            int i = Grid.RowCount;
            if (row == Grid.RowCount - 1)
            {
                return;
            }
            if (Grid.SelectedCells.Count != 1)
            {
                return;
            }
            DataGridViewRow temp = Grid.Rows[row];
            Grid.Rows.Remove(Grid.Rows[row]);
            Grid.Rows.Insert(row + 1, temp);
            Grid.ClearSelection();
            Grid.CurrentCell = Grid[col, row + 1];
            Grid.Rows[row + 1].Cells[col].Selected = true;
            HandleGridScrolling(false, Grid);
        }

        private void HandleGridScrolling(bool Up, DataGridView Grid)
        {
            // Makes sure the selected row is visible
            int VisibleRows = Grid.DisplayedRowCount(false);
            int FirstVisibleRow = Grid.FirstDisplayedCell.RowIndex;
            int LastVisibleRow = (FirstVisibleRow + VisibleRows) - 1;
            int SelectedRow = Grid.CurrentCell.RowIndex;

            if (VisibleRows >= Grid.RowCount)
            {
                return;
            }

            if (Up)
            {
                if (FirstVisibleRow == 0)
                {
                    return;
                }
                if (SelectedRow < FirstVisibleRow + 2)
                {
                    if (SelectedRow > 2)
                    {
                        Grid.FirstDisplayedScrollingRowIndex = SelectedRow - 2;
                    }
                    else
                    {
                        Grid.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
            }
            else
            {
                if (LastVisibleRow == Grid.RowCount)
                {
                    return;
                }
                if (SelectedRow > LastVisibleRow - 3)
                {
                    if (SelectedRow < Grid.RowCount - 3)
                    {
                        Grid.FirstDisplayedScrollingRowIndex = SelectedRow - VisibleRows + 3;
                    }
                    else
                    {
                        Grid.FirstDisplayedScrollingRowIndex =
                            Grid.RowCount - VisibleRows;
                    }
                }
            }
        }

        // =================================================================================
        // Forcing a DataGridview display update
        // Ugly hack if you ask me, but MS didn't give us any other reliable way...
        private void Update_GridView(DataGridView Grid)
        {
            Grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            BindingSource bs = new BindingSource(); // create a BindingSource
            bs.DataSource = Grid.DataSource;  // copy jobdata to bs
            DataTable dat = (DataTable)(bs.DataSource);  // make a datatable from bs
            Grid.DataSource = dat;  // and copy datatable data to jobdata, forcing redraw
            Grid.RefreshEdit();
            Grid.Refresh();
        }

        #endregion General

        // =================================================================================
        // Jogging
        // =================================================================================
        #region Jogging

        // see https://github.com/synthetos/TinyG/wiki/TinyG-Feedhold-and-Resume

        // =================================================================================
        private void MouseWheel_event(object sender, MouseEventArgs e)
        {
            if (!MouseScroll_checkBox.Checked)
            {
                return;
            }

            double Mag = 0.0;
            if (e.Delta < 0)
            {
                Mag = -1.0;
            }
            else if (e.Delta > 0)
            {
                Mag = 1.0;
            }
            else
            {
                return;
            }
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Shift)
            {
                Mag = Mag * 10.0;
            }
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                Mag = Mag / 10.0;
            }

            JoggingBusy = true;
            CNC_A_m(Cnc.CurrentA + Mag);
            if (DownCamera.Draw_Snapshot)
            {
                DownCamera.RotateSnapshot(Cnc.CurrentA);
                while (DownCamera.rotating)
                {
                    Thread.Sleep(10);
                }
            }
            JoggingBusy = false;
        }


        public bool JoggingBusy = false;
        List<Keys> JoggingKeys = new List<Keys>()
	    {
	        Keys.NumPad1,   // down + left
	        Keys.NumPad2,   // down
	        Keys.NumPad3,   // down + right
	        Keys.NumPad4,   // left
            Keys.NumPad6,   // right
	        Keys.NumPad7,   // up + left
	        Keys.NumPad8,   // up
 	        Keys.NumPad9,   // up + right
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
        // TODO: we don't; datagrids steal arrow keys
        private void RemoveCursorNavigation(System.Windows.Forms.Control.ControlCollection controls)
        {
            foreach (System.Windows.Forms.Control ctrl in controls)
            {
                ctrl.PreviewKeyDown += new PreviewKeyDownEventHandler(My_KeyDown);
                RemoveCursorNavigation(ctrl.Controls);
            }
        }


        public void My_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.NumPad1) || (e.KeyCode == Keys.NumPad2) || (e.KeyCode == Keys.NumPad3) ||
                (e.KeyCode == Keys.NumPad4) || (e.KeyCode == Keys.NumPad6) ||
                (e.KeyCode == Keys.NumPad7) || (e.KeyCode == Keys.NumPad8) || (e.KeyCode == Keys.NumPad9))
            {
                if (!NumPadJog_checkBox.Checked)
                {
                    return;
                }
                if ((ActiveControl is TextBox) || (ActiveControl is NumericUpDown) || (ActiveControl is MaskedTextBox))
                {
                    return;
                };
                JoggingBusy = false;
                e.Handled = true;
                e.SuppressKeyPress = true;
                Cnc.RawWrite("!%");
            }
        }

        static bool EnterKeyHit = true;
        static string Movestr;

        public void My_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //DisplayText("My_KeyDown: " + e.KeyCode.ToString());

            if (e.KeyCode == Keys.Enter)
            {
                EnterKeyHit = true;
                return;
            }

            if (e.KeyCode == Keys.F4)
            {
                Demo_button.Visible = !Demo_button.Visible;
                StopDemo_button.Visible = !StopDemo_button.Visible;
                return;
            }


            if (!JoggingKeys.Contains(e.KeyCode))
            {
                return;
            }

            if ((e.KeyCode == Keys.NumPad1) || (e.KeyCode == Keys.NumPad2) || (e.KeyCode == Keys.NumPad3) ||
                (e.KeyCode == Keys.NumPad4) || (e.KeyCode == Keys.NumPad6) ||
                (e.KeyCode == Keys.NumPad7) || (e.KeyCode == Keys.NumPad8) || (e.KeyCode == Keys.NumPad9))
            {
                if (!NumPadJog_checkBox.Checked)
                {
                    return;
                }
                if ((ActiveControl is TextBox) || (ActiveControl is NumericUpDown) || (ActiveControl is MaskedTextBox))
                {
                    return;
                }
            }

            if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
            {
                Movestr = "{\"gc\":\"G1 F" + AltJogSpeed_numericUpDown.Value.ToString() + " ";
            }
            else if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                Movestr = "{\"gc\":\"G1 F" + CtlrJogSpeed_numericUpDown.Value.ToString() + " ";
            }
            else
            {
                Movestr = "{\"gc\":\"G1 F" + NormalJogSpeed_numericUpDown.Value.ToString() + " ";
            }


            e.IsInputKey = true;

            if (JoggingBusy)
            {
                return;
            }

            if (!Cnc.Connected)
            {
                return;
            }

            if (e.KeyCode == Keys.NumPad1)
            {
                JoggingBusy = true;
                //DisplayText("up");
                //return;
                Cnc.RawWrite(Movestr + "X0 Y0\"}");
            }
            else if (e.KeyCode == Keys.NumPad2)
            {
                JoggingBusy = true;
                //DisplayText("down");
                //return;
                Cnc.RawWrite(Movestr + "Y0\"}");
            }
            else if (e.KeyCode == Keys.NumPad3)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "Y0" + "X" + Properties.Settings.Default.General_MachineSizeX.ToString() + "\"}");
            }
            else if (e.KeyCode == Keys.NumPad4)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "X0\"}");
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "X" + Properties.Settings.Default.General_MachineSizeX.ToString() + "\"}");
            }
            else if (e.KeyCode == Keys.NumPad7)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "X0" + "Y" + Properties.Settings.Default.General_MachineSizeY.ToString() + "\"}");
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "Y" + Properties.Settings.Default.General_MachineSizeY.ToString() + "\"}");
            }
            else if (e.KeyCode == Keys.NumPad9)
            {
                JoggingBusy = true;
                Cnc.RawWrite(Movestr + "X" + Properties.Settings.Default.General_MachineSizeX.ToString() + "Y" + Properties.Settings.Default.General_MachineSizeY.ToString() + "\"}");
            }
            else
            {
                Jog(sender, e);
            }
        }


        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void Jog(object sender, PreviewKeyDownEventArgs e)
        {

            if (JoggingBusy)
            {
                return;
            }

            if (!Cnc.Connected)
            {
                return;
            }

            double Mag = 0.0;
            if ((e.Alt) && (e.Shift))
            {
                Mag = 100.0;
            }
            else if ((e.Alt) && (e.Control))
            {
                Mag = 4.0;
            }
            else if (e.Alt)
            {
                Mag = 10.0;
            }
            else if (e.Shift)
            {
                Mag = 1.0;
            }
            else if (e.Control)
            {
                Mag = 0.01;
            }
            else
            {
                Mag = 0.1;
            };

            // move right
            if (e.KeyCode == Keys.F5)
            {
                JoggingBusy = true;
                CNC_XY_m(Cnc.CurrentX - Mag, Cnc.CurrentY);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move left
            if (e.KeyCode == Keys.F6)
            {
                JoggingBusy = true;
                CNC_XY_m(Cnc.CurrentX + Mag, Cnc.CurrentY);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move away
            if (e.KeyCode == Keys.F7)
            {
                JoggingBusy = true;
                CNC_XY_m(Cnc.CurrentX, Cnc.CurrentY + Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move closer
            if (e.KeyCode == Keys.F8)
            {
                JoggingBusy = true;
                CNC_XY_m(Cnc.CurrentX, Cnc.CurrentY - Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            };

            // rotate ccw
            if (e.KeyCode == Keys.F9)
            {
                JoggingBusy = true;
                if ((Mag > 99) && (Mag < 101))
                {
                    Mag = 90.0;
                }
                CNC_A_m(Cnc.CurrentA + Mag);
                if (DownCamera.Draw_Snapshot)
                {
                    DownCamera.RotateSnapshot(Cnc.CurrentA);
                    while (DownCamera.rotating)
                    {
                        Thread.Sleep(10);
                    }
                }
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // rotate cw
            if (e.KeyCode == Keys.F10)
            {
                JoggingBusy = true;
                if ((Mag > 99) && (Mag < 101))
                {
                    Mag = 90.0;
                }
                CNC_A_m(Cnc.CurrentA - Mag);
                if (DownCamera.Draw_Snapshot)
                {
                    DownCamera.RotateSnapshot(Cnc.CurrentA);
                    while (DownCamera.rotating)
                    {
                        Thread.Sleep(10);
                    }
                }
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move up
            if (e.KeyCode == Keys.F11)
            {
                JoggingBusy = true;
                CNC_Z_m(Cnc.CurrentZ - Mag);
                e.IsInputKey = true;
                JoggingBusy = false;
                return;
            }

            // move down
            if ((e.KeyCode == Keys.F12) && (Mag < 50))
            {
                JoggingBusy = true;
                CNC_Z_m(Cnc.CurrentZ + Mag);
                JoggingBusy = false;
                e.IsInputKey = true;
                return;
            }

        }

        // =================================================================================
        // Picturebox mouse functions

        private void BoxTo_mms(out double Xmm, out double Ymm, int MouseX, int MouseY, PictureBox Box)
        {
            // Input is mouse position inside picture box.
            // Output is mouse position in mm's from image center (machine position)
            Xmm = 0.0;
            Ymm = 0.0;
            int X = MouseX - Box.Size.Width / 2;  // X= diff from center
            int Y = MouseY - Box.Size.Height / 2;
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
                    Xmm = Xmm / UpCamera.ZoomFactor;
                    Ymm = Ymm / UpCamera.ZoomFactor;
                }
                Xmm = -Xmm / UpCamera.GetDisplayZoom();	// Might also be zoomed for processing
                Ymm = -Ymm / UpCamera.GetDisplayZoom();
            }
            else
            {
                DisplayText("No camera running");
            };
            DisplayText("BoxTo_mms: MouseX: " + MouseX.ToString() + ", X: " + X.ToString() + ", Xmm: " + Xmm.ToString());
            DisplayText("BoxTo_mms: MouseY: " + MouseY.ToString() + ", Y: " + Y.ToString() + ", Ymm: " + Ymm.ToString());
        }


        bool PictureBox_MouseDragged = false;

        private void General_pictureBox_MouseMove(PictureBox Box, int MouseX, int MouseY)
        {
            if (MouseButtons == MouseButtons.Left)
            {
                DisplayText("X: " + MouseX.ToString() + ", Y: " + MouseY.ToString());
            }
        }

        private void General_pictureBox_MouseClick(PictureBox Box, int MouseX, int MouseY)
        {
            double Xmm, Ymm;

            if (PictureBox_MouseDragged)
            {
                PictureBox_MouseDragged = false;
                return;
            }
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Control)
            {
                // Cntrl-click
                double X = Convert.ToDouble(MouseX) / Convert.ToDouble(Box.Size.Width);
                X = X * Properties.Settings.Default.General_MachineSizeX;
                double Y = Convert.ToDouble(Box.Size.Height - MouseY) / Convert.ToDouble(Box.Size.Height);
                Y = Y * Properties.Settings.Default.General_MachineSizeY;
                CNC_XY_m(X, Y);
            }

            else
            {
                BoxTo_mms(out Xmm, out Ymm, MouseX, MouseY, Box);
                if (UpCamera.IsRunning())
                {
                    //Xmm = -Xmm;
                    //Ymm = -Ymm;
                }
                CNC_XY_m(Cnc.CurrentX + Xmm, Cnc.CurrentY - Ymm);
            }
        }



        #endregion Jogging

        // =================================================================================
        // CNC interface functions
        // =================================================================================
        #region CNC interface functions

        private bool VacuumIsOn = false;

        private void VacuumDefaultSetting()
        {
            VacuumOff();
        }

        private void VacuumOn()
        {
            if (!VacuumIsOn)
            {
                DisplayText("VacuumOn()");
                CNC_RawWrite("{\"gc\":\"M08\"}");
                VacuumIsOn = true;
                Vacuum_checkBox.Checked = true;
                Thread.Sleep(Properties.Settings.Default.General_PickupVacuumTime);
            }
        }

        private void VacuumOff()
        {
            if (VacuumIsOn)
            {
                DisplayText("VacuumOff()");
                CNC_RawWrite("{\"gc\":\"M09\"}");
                VacuumIsOn = false;
                Vacuum_checkBox.Checked = false;
                Thread.Sleep(Properties.Settings.Default.General_PickupReleaseTime);
            }
        }

        private bool PumpIsOn = false;
        private void PumpDefaultSetting()
        {
            PumpOff();
        }

        private void BugWorkaround()
        {
            // see https://www.synthetos.com/topics/file-not-open-error/#post-7194
            // Summary: In some cases, we need a dummy move.
            bool slackSave = Cnc.SlackCompensation;
            Cnc.SlackCompensation = false;
            CNC_XY_m(Cnc.CurrentX - 0.5, Cnc.CurrentY - 0.5);
            CNC_XY_m(Cnc.CurrentX + 0.5, Cnc.CurrentY + 0.5);
            Cnc.SlackCompensation = slackSave;
        }

        private void PumpOn()
        {
            if (!PumpIsOn)
            {
                //CNC_RawWrite("M03");
                CNC_RawWrite("{\"gc\":\"M03\"}");
                Pump_checkBox.Checked = true;
                Thread.Sleep(500);  // this much to develop vacuum
                BugWorkaround();
                PumpIsOn = true;
            }
        }

        private void PumpOff()
        {
            if (PumpIsOn)
            {
                //CNC_RawWrite("M05");
                CNC_RawWrite("{\"gc\":\"M05\"}");
                Thread.Sleep(50);
                BugWorkaround();
                Pump_checkBox.Checked = false;
                PumpIsOn = false;
            }
        }

        private void PumpOff_NoWorkaround()
        // For error situations where we don't want to do the dance
        {
            if (PumpIsOn)
            {
                //CNC_RawWrite("M05");
                CNC_RawWrite("{\"gc\":\"M05\"}");
                Thread.Sleep(50);
                Pump_checkBox.Checked = false;
                PumpIsOn = false;
            }
        }


        private bool Needle_ProbeDown_m()
        {
            if (!HomingTimeout_m(out CNC_HomingTimeout, "Z"))
            {
                return false;
            }

            DisplayText("Probing Z, timeout value: " + CNC_HomingTimeout.ToString());

            Needle.ProbingMode(true, JSON);
            Cnc.Homing = true;
            if (!CNC_Write_m("{\"gc\":\"G28.4 Z0\"}", 4000))
            {
                Cnc.Homing = false;
                Needle.ProbingMode(false, JSON);
                return false;
            }
            Cnc.Homing = false;
            Needle.ProbingMode(false, JSON);
            return true;
        }

        public bool CalibrateNeedle_m()
        {
            if (Properties.Settings.Default.Placement_OmitNeedleCalibration)
            {
                return true;
            };

            double MarkX = Cnc.CurrentX;
            double MarkY = Cnc.CurrentY;
            double MarkZ = Cnc.CurrentZ;
            double MarkA = Cnc.CurrentA;

            bool UpCamWasRunning = false;
            if (UpCamera.Active)
            {
                UpCamWasRunning = true;
            }
            SelectCamera(UpCamera);
            if (!UpCamera.IsRunning())
            {
                ShowMessageBox(
                    "Up camera not running, can't calibrate needle.",
                    "Needle calibration failed.",
                    MessageBoxButtons.OK);
                return false;
            }
            UpCamera.PauseProcessing = true;

            // take needle up
            bool result = true;
            result &= CNC_Z_m(0.0);

            // take needle to camera
            result &= CNC_XY_m(Properties.Settings.Default.UpCam_PositionX, Properties.Settings.Default.UpCam_PositionY);
            result &= CNC_Z_m(Properties.Settings.Default.General_ZtoPCB - 1.0); // Average small component height 1mm (?)


            // measure the values
            SetNeedleMeasurement();
            result &= Needle.Calibrate(4.0 / Properties.Settings.Default.UpCam_XmmPerPixel);  // have to find the tip within 4mm of center

            // take needle up
            result &= CNC_Z_m(0.0);

            // restore position
            // result &= CNC_XYA_m(MarkX, MarkY, MarkA);

            UpCamera.PauseProcessing = false;
            if (!UpCamWasRunning)
            {
                SelectCamera(DownCamera);
            }
            if (result)
            {
                for (int i = 0; i < Needle.CalibrationPoints.Count; i++)
                {
                    DisplayText("A: " + Needle.CalibrationPoints[i].Angle.ToString("0.000") +
                        ", X: " + Needle.CalibrationPoints[i].X.ToString("0.000") +
                        ", Y: " + Needle.CalibrationPoints[i].Y.ToString("0.000"));
                }
            }
            else
            {
                ShowMessageBox(
                    "Needle calibration failed.",
                    "Needle calibration failed.",
                    MessageBoxButtons.OK);
            }
            return (result);
        }

        private void CNC_Park()
        {
            CNC_Z_m(0);
            CNC_XY_m(Properties.Settings.Default.General_ParkX, Properties.Settings.Default.General_ParkY);
        }

        private bool HomingTimeout_m(out int TimeOut, string axis)
        {
            string speed = "0";
            double size;
            TimeOut = 0;
            switch (axis)
            {
                case "X":
                    speed = xsv_maskedTextBox.Text;
                    size = Properties.Settings.Default.General_MachineSizeX;
                    break;

                case "Y":
                    speed = ysv_maskedTextBox.Text;
                    size = Properties.Settings.Default.General_MachineSizeY;
                    break;

                case "Z":
                    speed = zsv_maskedTextBox.Text;
                    size = 100.0;
                    break;

                default:
                    return false;
            }

            double MaxTime;
            if (!double.TryParse(speed, out MaxTime))
            {
                ShowMessageBox(
                    "Bad data in " + axis + " homing speed",
                    "Data error",
                    MessageBoxButtons.OK);
                return false;
            }

            MaxTime = MaxTime / 60;  // Now in seconds/mm
            MaxTime = (size / MaxTime) * 1.2; // in seconds for the machine size and some 
            TimeOut = (int)MaxTime;
            return true;
        }

        private bool CNC_Home_m(string axis)
        {
            if (!HomingTimeout_m(out CNC_HomingTimeout, axis))
            {
                return false;
            }
            DisplayText("Homing axis " + axis + ", timeout value: " + CNC_HomingTimeout.ToString());

            Cnc.Homing = true;
            if (!CNC_Write_m("{\"gc\":\"G28.2 " + axis + "0\"}"))
            {
                ShowMessageBox(
                    "Homing operation mechanical step failed, CNC issue",
                    "Homing failed",
                    MessageBoxButtons.OK);
                Cnc.Homing = false;
                return false;
            }
            Cnc.Homing = false;
            DisplayText("Homing " + axis + " done.");
            return true;
        }

        // =================================================================================
        // CNC_Write_m
        // Sends a command to CNC, doesn't return until the response is handled
        // by the CNC class. (See _readyEvent )
        // =================================================================================
        private const int CNC_MoveTimeout = 3000; // timeout for X,Y,Z,A movements; 2x ms. (3000= 6s timeout)
        public int CNC_HomingTimeout = 16;  // in seconds

        private void CNC_RawWrite(string s)
        {
            // This for operations that cause conflicts with event firings. Caller does waiting, if needed.
            Cnc.RawWrite(s);
        }

        bool CNC_BlockingWriteDone = false;
        bool CNC_WriteOk = true;
        private void CNC_BlockingWrite_thread(string cmd)
        {
            Cnc_ReadyEvent.Reset();
            CNC_WriteOk = Cnc.Write(cmd);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_Write_m(string s, int Timeout = 250)
        {
            if (Cnc.ErrorState)
            {
                DisplayText("### " + s + " ignored, cnc is in error state", KnownColor.Red);
                return false;
            };

            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingWrite_thread(s));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            if (Cnc.Homing)
            {
                Timeout = CNC_HomingTimeout * 1000 / 2;
            };
            while (!CNC_BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > Timeout)
                {
                    Cnc_ReadyEvent.Set();  // terminates the CNC_BlockingWrite_thread
                    ShowMessageBox(
                        "Debug: CNC_BlockingWrite: Timeout on command " + s,
                        "Timeout",
                        MessageBoxButtons.OK);
                    CNC_BlockingWriteDone = true;
                    JoggingBusy = false;
                    Cnc.Error();
                    ValidMeasurement_checkBox.Checked = false;
                    return false;
                }
            }
            if (!CNC_WriteOk)
            {
                ValidMeasurement_checkBox.Checked = false;
            }
            return (CNC_WriteOk);
        }

        private bool CNC_MoveIsSafeX_m(double X)
        {
            if ((X < -3.0) || (X > Properties.Settings.Default.General_MachineSizeX))
            {
                ShowMessageBox(
                    "Attempt to move outside safe limits (X " + X.ToString("0.000", CultureInfo.InvariantCulture) + ")",
                    "Limits corossed",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool CNC_MoveIsSafeY_m(double Y)
        {
            if ((Y < -3.0) || (Y > Properties.Settings.Default.General_MachineSizeY))
            {
                ShowMessageBox(
                    "Attempt to move outside safe limits (Y " + Y.ToString("0.000", CultureInfo.InvariantCulture) + ")",
                    "Limits corssed",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private bool _Zguard = true;
        private void ZGuardOn()
        {
            _Zguard = true;
        }
        private void ZGuardOff()
        {
            _Zguard = false;
        }

        private bool CNC_NeedleIsDown_m()
        {
            if ((Cnc.CurrentZ > 5) && _Zguard)
            {
                DisplayText("Needle down error.");
                ShowMessageBox(
                   "Attempt to move while needle is down.",
                   "Danger to Needle",
                   MessageBoxButtons.OK);
                return true;
            }
            return false;
        }

        private void CNC_BlockingXY_thread(double X, double Y)
        {
            Cnc_ReadyEvent.Reset();
            Cnc.XY(X, Y);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        private void CNC_BlockingXYA_thread(double X, double Y, double A)
        {
            Cnc_ReadyEvent.Reset();
            Cnc.XYA(X, Y, A);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_XY_m(double X, double Y)
        {
            DisplayText("CNC_XY_m, x: " + X.ToString() + ", y: " + Y.ToString());
            if (CNC_NeedleIsDown_m())
            {
                return false;
            }
            if (Cnc.ErrorState)
            {
                DisplayText("### Cnc in error state, ignored", KnownColor.Red);
                return false;
            }
            if (AbortPlacement)
            {
                AbortPlacement = false;  // one shot
                ShowMessageBox(
                           "Operation aborted",
                           "Operation aborted",
                           MessageBoxButtons.OK);
                return false;
            }

            if (!CNC_MoveIsSafeX_m(X))
            {
                return false;
            }

            if (!CNC_MoveIsSafeY_m(Y))
            {
                return false;
            }

            if (!Cnc.Connected)
            {
                ShowMessageBox(
                    "CNC_XY: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingXY_thread(X, Y));
            t.IsBackground = true;
            t.Start();
            int i = 0;

            while (!CNC_BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout)
                {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Cnc.Connected)
            {
                ShowMessageBox(
                           "CNC_XY: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                CncError();
            }
            DisplayText("CNC_XY_m ok");
            return (!Cnc.ErrorState);
        }

        public bool CNC_XYA_m(double X, double Y, double A)
        {
            DisplayText("CNC_XYA_m, x: " + X.ToString() + ", y: " + Y.ToString() + ", a: " + A.ToString());
            if (CNC_NeedleIsDown_m())
            {
                return false;
            }
            if (Cnc.ErrorState)
            {
                DisplayText("### Cnc in error state, ignored", KnownColor.Red);
                return false;
            }
            if (AbortPlacement)
            {
                AbortPlacement = false;  // one shot
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            }

            if (!CNC_MoveIsSafeX_m(X))
            {
                return false;
            }

            if (!CNC_MoveIsSafeY_m(Y))
            {
                return false;
            }

            if (!Cnc.Connected)
            {
                ShowMessageBox(
                    "CNC_XYA: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }

            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingXYA_thread(X, Y, A));
            t.IsBackground = true;
            t.Start();
            int i = 0;

            while (!CNC_BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout)
                {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Cnc.Connected)
            {
                ShowMessageBox(
                           "CNC_XYA: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                CncError();
            }
            return (Cnc.Connected);
        }


        private void CNC_BlockingZ_thread(double Z)
        {
            Cnc_ReadyEvent.Reset();
            Cnc.Z(Z);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        private bool CNC_Z_m(double Z)
        {
            if (AbortPlacement)
            {
                AbortPlacement = false;  // one shot
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            }
            if (!Cnc.Connected)
            {
                ShowMessageBox(
                    "CNC_Z: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }

            if (Cnc.ErrorState)
            {
                ShowMessageBox(
                    "CNC_Z: Cnc in error state",
                    "Cncin error state",
                    MessageBoxButtons.OK);
                return false;
            }

            if (Z>(Properties.Settings.Default.General_ZtoPCB + 1.6))
            {
                DialogResult dialogResult = ShowMessageBox(
                    "The operation seems to take needle below table surface. Continue?",
                    "Z below table", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return false;
                };

            }

            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingZ_thread(Z));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            while (!CNC_BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout)
                {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }
            if ((i > CNC_MoveTimeout) || !Cnc.Connected)
            {
                ShowMessageBox(
                           "CNC_Z: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                CncError();
            }
            return (Cnc.Connected);
        }

        private void CNC_BlockingA_thread(double A)
        {
            Cnc_ReadyEvent.Reset();
            Cnc.A(A);
            Cnc_ReadyEvent.Wait();
            CNC_BlockingWriteDone = true;
        }

        public bool CNC_A_m(double A)
        {
            DisplayText("CNC_A_m, a: " + A.ToString());
            if (Cnc.ErrorState)
            {
                DisplayText("### Cnc in error state, ignored", KnownColor.Red);
                return false;
            }
            if (!Cnc.Connected)
            {
                ShowMessageBox(
                    "CNC_A: Cnc not connected",
                    "Cnc not connected",
                    MessageBoxButtons.OK);
                return false;
            }
            CNC_BlockingWriteDone = false;
            Thread t = new Thread(() => CNC_BlockingA_thread(A));
            t.IsBackground = true;
            t.Start();
            int i = 0;
            while (!CNC_BlockingWriteDone)
            {
                Thread.Sleep(2);
                Application.DoEvents();
                i++;
                if (i > CNC_MoveTimeout)
                {
                    Cnc_ReadyEvent.Set();   // causes CNC_Blocking_thread to exit
                }
            }

            CNC_BlockingWriteDone = true;
            if ((i > CNC_MoveTimeout) && Cnc.Connected)
            {
                ShowMessageBox(
                           "CNC_A: Timeout / Cnc connection cut!",
                           "Timeout",
                           MessageBoxButtons.OK);
                CncError();
            }
            return (Cnc.Connected);
        }


        // =====================================================================
        // This routine finds an accurate location of a circle that downcamera is looking at.
        // Used in homing and locating fiducials.
        // Tolerances in mm; find: how far from center to accept a circle, move: how close to go (set small to ensure view from straight up)
        // At return, the camera is located on top of the circle.
        // X and Y are set to remainding error (true position: currect + error)
        // =====================================================================

        public bool GoToCircleLocation_m(double FindTolerance, double MoveTolerance, out double X, out double Y)
        {
            DisplayText("GoToCircleLocation_m(), FindTolerance: " + FindTolerance.ToString() + ", MoveTolerance: " + MoveTolerance.ToString());
            SelectCamera(DownCamera);
            X = 100;
            Y = 100;
            FindTolerance = FindTolerance / Properties.Settings.Default.DownCam_XmmPerPixel;
            if (!DownCamera.IsRunning())
            {
                ShowMessageBox(
                    "Attempt to find circle, downcamera is not running.",
                    "Camera not running",
                    MessageBoxButtons.OK);
                return false;
            }
            int count = 0;
            int res = 0;
            int tries = 0;
            bool ProcessingStateSave = DownCamera.PauseProcessing;
            DownCamera.PauseProcessing = true;
            do
            {
                // Measure circle location
                for (tries = 0; tries < 8; tries++)
                {
                    res = DownCamera.GetClosestCircle(out X, out Y, FindTolerance);
                    if (res != 0)
                    {
                        break;
                    }
                    Thread.Sleep(80); // next frame + vibration damping
                    if (tries >= 7)
                    {
                        DisplayText("Failed in 8 tries.");
                        ShowMessageBox(
                            "Optical positioning: Can't find Circle",
                            "No Circle found",
                            MessageBoxButtons.OK);
                        DownCamera.PauseProcessing = ProcessingStateSave;
                        return false;
                    }
                }
                X = X * Properties.Settings.Default.DownCam_XmmPerPixel;
                Y = -Y * Properties.Settings.Default.DownCam_YmmPerPixel;
                DisplayText("Optical positioning, round " + count.ToString() + ", dX= " + X.ToString() + ", dY= " + Y.ToString() + ", tries= " + tries.ToString());
                // If we are further than move tolerance, go there
                if ((Math.Abs(X) > MoveTolerance) || (Math.Abs(Y) > MoveTolerance))
                {
                    CNC_XY_m(Cnc.CurrentX + X, Cnc.CurrentY + Y);
                }
                count++;
            }  // repeat this until we didn't need to move
            while ((count < 8)
                && ((Math.Abs(X) > MoveTolerance)
                || (Math.Abs(Y) > MoveTolerance)));

            DownCamera.PauseProcessing = ProcessingStateSave;
            if (count >= 7)
            {
                ShowMessageBox(
                    "Optical positioning: Process is unstable, result is unreliable!",
                    "Count exeeded",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }


        private bool OpticalHoming_m()
        {
            DisplayText("Optical homing");
            SetHomingMeasurement();
            double X;
            double Y;
            // Find within 20mm, goto within 0.05
            if (!GoToCircleLocation_m(20.0, 0.05, out X, out Y))
            {
                return false;
            }
            X = -X;
            Y = -Y;
            // CNC_RawWrite("G28.3 X" + X.ToString("0.000") + " Y" + Y.ToString("0.000"));
            CNC_RawWrite("{\"gc\":\"G28.3 X" + X.ToString("0.000") + " Y" + Y.ToString("0.000") + "\"}");
            Thread.Sleep(50);
            Cnc.CurrentX = 0.0;
            Cnc.CurrentY = 0.0;
            Update_xpos("0.00");
            Update_ypos("0.00");
            DisplayText("Optical homing OK.");
            return true;
        }

        private bool MechanicalHoming_m()
        {
            Needle.ProbingMode(false, JSON);
            if (!CNC_Home_m("Z"))
            {
                return false;
            };
            DisplayText("Move Z");
            if (!CNC_Z_m(Properties.Settings.Default.General_ShadeGuard_mm))		// make room for shade
            {
                return false;
            };
            if (!CNC_Home_m("Y"))
            {
                return false;
            };
            if (!CNC_Home_m("X"))
            {
                return false;
            };
            DisplayText("Move A");
            if (!CNC_A_m(0))
            {
                return false;
            };
            if (Properties.Settings.Default.General_ShadeGuard_mm > 0.0)
            {
                ZGuardOff();
                if (!CNC_XY_m(10, 10))
                {
                    ZGuardOn();
                    return false;
                };
                DisplayText("Z back up Z");  // Z back up
                if (!CNC_Z_m(0))
                {
                    ZGuardOn();
                    return false;
                };
                ZGuardOn();
            };

            return true;
        }


        private void OpticalHome_button_Click(object sender, EventArgs e)
        {
            ValidMeasurement_checkBox.Checked = false;
            if (!MechanicalHoming_m())
            {
                return;
            }
            OpticalHoming_m();
        }

        #endregion CNC interface functions

        // =================================================================================
        // Up/Down camera setup page functions
        // =================================================================================
        #region Camera setup pages functions

        // =================================================================================
        // Common
        // =================================================================================

        private void StartCameras()
        {
            // Called at startup. 
            DownCamera.Close();
            UpCamera.Close();
            SetDownCameraDefaults();
            SetUpCameraDefaults();
            if (KeepActive_checkBox.Checked)
            {
                StartUpCamera_m();
                UpCamera.Active = false;
                StartDownCamera_m();
            }
            SelectCamera(DownCamera);

        }


        private void RobustFast_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Cameras_RobustSwitch = RobustFast_checkBox.Checked;
        }


        private void SelectCamera(Camera cam)
        {
            if (KeepActive_checkBox.Checked)
            {
                DownCamera.Active = false;
                UpCamera.Active = false;
                cam.Active = true;
                if (cam == DownCamera)
                {
                    DisplayText("DownCamera activated");
                }
                else
                {
                    DisplayText("UpCamera activated");
                }
                return;
            };

            if (Properties.Settings.Default.Cameras_RobustSwitch)
            {
                if (UpCamera.IsRunning())
                {
                    UpCamera.Close();
                };
                if (DownCamera.IsRunning())
                {
                    DownCamera.Close();
                };
            }
            if (cam == DownCamera)
            {
                if (UpCamera.IsRunning())
                {
                    UpCamera.Close();
                };
                StartDownCamera_m();
            }
            else
            {
                if (DownCamera.IsRunning())
                {
                    DownCamera.Close();
                };
                StartUpCamera_m();
            }
        }


        // =================================================================================
        private bool StartDownCamera_m()
        {
            UpCamera.Active = false;
            DownCamera.Active = true;
            if (DownCamera.IsRunning())
            {
                DisplayText("DownCamera already running");
                return true;
            };
            if (Properties.Settings.Default.DownCam_index == -1)
            {
                // Very first runs, no attempt to connect cameras yet. This is ok.
                return true;
            };

            if (!DownCamera.Start("DownCamera", Properties.Settings.Default.DownCam_index))
            {
                ShowMessageBox(
                    "Problem Starting down camera.",
                    "Down Camera problem",
                    MessageBoxButtons.OK
                );
                DownCamStatus_label.Text = "Not Connected";
                return false;
            };
            DownCamStatus_label.Text = "Active";
            UpCamStatus_label.Text = "Not Active";
            return true;
        }

        // ====
        private bool StartUpCamera_m()
        {
            DownCamera.Active = false;
            UpCamera.Active = true;
            if (UpCamera.IsRunning())
            {
                DisplayText("UpCamera already running");
                return true;
            };
            if (Properties.Settings.Default.UpCam_index == -1)
            {
                // Very first runs, no attempt to connect cameras yet. This is ok.
                return true;
            };

            if (!UpCamera.Start("UpCamera", Properties.Settings.Default.UpCam_index))
            {
                ShowMessageBox(
                    "Problem Starting up camera.",
                    "Up Camera problem",
                    MessageBoxButtons.OK
                );
                UpCamStatus_label.Text = "Not Connected";
                return false;
            };
            UpCamera.Active = true;
            UpCamStatus_label.Text = "Active";
            DownCamStatus_label.Text = "Not Active";
            return true;
        }

        // =================================================================================

        // =================================================================================

        private void Cam_pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Control.ModifierKeys == Keys.Alt)
            {
                PickColor(e.X, e.Y);
            }
            else
            {
                General_pictureBox_MouseClick(Cam_pictureBox, e.X, e.Y);
            }
        }

        // =================================================================================

        private void SetDownCameraDefaults()
        {
            DownCamera.BoxSizeX = 200;
            DownCamera.BoxSizeY = 200;
            DownCamera.BoxRotationDeg = 0;
            DownCamera.ImageBox = Cam_pictureBox;
            DownCamera.Mirror = false;
            DownCamera.ClearDisplayFunctionsList();
            DownCamera.SnapshotColor = Properties.Settings.Default.DownCam_SnapshotColor;
            // Draws
            DownCamera.DrawCross = true;
            DownCameraDrawCross_checkBox.Checked = true;
            DownCamera.DrawDashedCross = false;
            DownCameraDrawDashedCross_checkBox.Checked = false;
            DownCamera.Draw_Snapshot = false;
            // Finds:
            DownCamera.FindCircles = false;
            DownCamFindCircles_checkBox.Checked = false;
            DownCamera.FindRectangles = false;
            DownCamFindRectangles_checkBox.Checked = false;
            DownCamera.FindComponent = false;
            DownCam_FindComponents_checkBox.Checked = false;
            DownCamera.TestAlgorithm = false;
            ImageTest_checkBox.Checked = false;
            Overlay_checkBox.Checked = false;
            DownCamera.DrawBox = false;
            DownCamera.DrawArrow = false;
            DownCameraDrawBox_checkBox.Checked = false;
        }
        // ====
        private void SetUpCameraDefaults()
        {
            UpCamera.BoxSizeX = 200;
            UpCamera.BoxSizeY = 200;
            UpCamera.BoxRotationDeg = 0;
            UpCamera.ImageBox = Cam_pictureBox;
            UpCamera.Mirror = true;
            UpCamera.ClearDisplayFunctionsList();
            UpCamera.SnapshotColor = Properties.Settings.Default.UpCam_SnapshotColor;
            // Draws
            UpCamera.DrawCross = true;
            UpCameraDrawCross_checkBox.Checked = true;
            UpCamera.DrawDashedCross = false;
            UpCameraDrawDashedCross_checkBox.Checked = false;
            UpCamera.Draw_Snapshot = false;
            // Finds:
            UpCamera.FindCircles = false;
            UpCamera.FindRectangles = false;
            UpCamera.FindComponent = false;
            UpCamera.TestAlgorithm = false;
            UpCamera.DrawBox = false;
            UpCamera.DrawArrow = false;
            UpCameraDrawBox_checkBox.Checked = false;
        }

        // =================================================================================
        private void tabPageSetupCameras_Begin()
        {
            SetDownCameraDefaults();
            SetUpCameraDefaults();
            NeedleOffset_label.Visible = false;
            ClearEditTargets();

            double f;
            f = Properties.Settings.Default.DownCam_XmmPerPixel * DownCamera.BoxSizeX;
            DownCameraBoxX_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            DownCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            f = Properties.Settings.Default.DownCam_YmmPerPixel * DownCamera.BoxSizeY;
            DownCameraBoxY_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            DownCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";

            f = Properties.Settings.Default.UpCam_XmmPerPixel * UpCamera.BoxSizeX;
            UpCameraBoxX_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            UpCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_XmmPerPixel.ToString("0.000", CultureInfo.InvariantCulture) + "mm/pixel)";
            f = Properties.Settings.Default.UpCam_YmmPerPixel * UpCamera.BoxSizeY;
            UpCameraBoxY_textBox.Text = f.ToString("0.00", CultureInfo.InvariantCulture);
            UpCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_YmmPerPixel.ToString("0.000", CultureInfo.InvariantCulture) + "mm/pixel)";

            JigX_textBox.Text = Properties.Settings.Default.General_JigOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
            JigY_textBox.Text = Properties.Settings.Default.General_JigOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
            PickupCenterX_textBox.Text = Properties.Settings.Default.General_PickupCenterX.ToString("0.00", CultureInfo.InvariantCulture);
            PickupCenterY_textBox.Text = Properties.Settings.Default.General_PickupCenterY.ToString("0.00", CultureInfo.InvariantCulture);
            NeedleOffsetX_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
            NeedleOffsetY_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
            Z0toPCB_CamerasTab_label.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture) + " mm";

            UpcamPositionX_textBox.Text = Properties.Settings.Default.UpCam_PositionX.ToString("0.00", CultureInfo.InvariantCulture);
            UpcamPositionY_textBox.Text = Properties.Settings.Default.UpCam_PositionY.ToString("0.00", CultureInfo.InvariantCulture);

            DownCamera.SideMarksX = Properties.Settings.Default.General_MachineSizeX / 100;
            DownCamera.SideMarksY = Properties.Settings.Default.General_MachineSizeY / 100;
            DownCameraDrawTicks_checkBox.Checked = Properties.Settings.Default.DownCam_DrawTicks;

            DowncamSnapshot_ColorBox.BackColor = Properties.Settings.Default.DownCam_SnapshotColor;
            UpcamSnapshot_ColorBox.BackColor = Properties.Settings.Default.UpCam_SnapshotColor;

            Display_dataGridView.Rows.Clear();
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
            getDownCamList();
            getUpCamList();
            SelectCamera(DownCamera);
        }

        // =================================================================================
        // get the devices         
        private void getDownCamList()
        {
            List<string> Devices = DownCamera.GetDeviceList();
            DownCam_comboBox.Items.Clear();
            if (Devices.Count != 0)
            {
                for (int i = 0; i < Devices.Count; i++)
                {
                    DownCam_comboBox.Items.Add(Devices[i]);
                }
            }
            else
            {
                DownCam_comboBox.Items.Add("----");
                DownCamStatus_label.Text = "No Cam";
            }
            if ((Devices.Count >= Properties.Settings.Default.DownCam_index) && (Properties.Settings.Default.DownCam_index > 0))
            {
                DownCam_comboBox.SelectedIndex = Properties.Settings.Default.DownCam_index;
            }
            else
            {
                DownCam_comboBox.SelectedIndex = 0;  // default to first
            }
            DisplayText("DownCam_comboBox.SelectedIndex= " + DownCam_comboBox.SelectedIndex.ToString());
        }

        // ====
        private void getUpCamList()
        {
            List<string> Devices = UpCamera.GetDeviceList();
            UpCam_comboBox.Items.Clear();
            int d = Properties.Settings.Default.UpCam_index;
            if (Devices.Count != 0)
            {
                for (int i = 0; i < Devices.Count; i++)
                {
                    UpCam_comboBox.Items.Add(Devices[i]);
                }
            }
            else
            {
                UpCam_comboBox.Items.Add("----");
                UpCamStatus_label.Text = "No Cam";
            }
            if ((Devices.Count >= Properties.Settings.Default.UpCam_index) && (Properties.Settings.Default.UpCam_index > 0))
            {
                DisplayText("UpCam_comboBox.SelectedIndex= " + Properties.Settings.Default.UpCam_index.ToString());
                UpCam_comboBox.SelectedIndex = Properties.Settings.Default.UpCam_index;
            }
            else
            {
                DisplayText("UpCam_comboBox.SelectedIndex= 0");
                UpCam_comboBox.SelectedIndex = 0;  // default to first
            }

        }


        // =================================================================================
        private void tabPageSetupCameras_End()
        {
            ZGuardOn();
        }

        // =================================================================================
        private void RefreshDownCameraList_button_Click(object sender, EventArgs e)
        {
            getDownCamList();
        }

        // ====
        private void RefreshUpCameraList_button_Click(object sender, EventArgs e)
        {
            getUpCamList();
        }

        // =================================================================================

        private void SetCurrentCameraParameters()
        {
            Camera CurrentCam;
            if (UpCamera.IsRunning())
            {
                CurrentCam = UpCamera;
            }
            else if (DownCamera.IsRunning())
            {
                CurrentCam = DownCamera;
            }
            else
            {
                return;
            };
            double val;
            if (UpCamera.IsRunning())
            {
                CurrentCam.Zoom = UpCamZoom_checkBox.Checked;
                if (double.TryParse(UpCamZoomFactor_textBox.Text, out val))
                {
                    UpCamera.ZoomFactor = val;
                }
            }
            else
            {
                CurrentCam.Zoom = DownCamZoom_checkBox.Checked;
                if (double.TryParse(DownCamZoomFactor_textBox.Text, out val))
                {
                    DownCamera.ZoomFactor = val;
                }
            }
        }

        // =================================================================================

        private void ConnectDownCamera_button_Click(object sender, EventArgs e)
        {
            DisplayText("DownCam_comboBox.SelectedIndex= " + DownCam_comboBox.SelectedIndex.ToString());
            Properties.Settings.Default.DownCam_index = DownCam_comboBox.SelectedIndex;
            SelectCamera(DownCamera);
            Properties.Settings.Default.DowncamMoniker = DownCamera.MonikerString;
            if (DownCamera.IsRunning())
            {
                SetCurrentCameraParameters();
            }
            else
            {
                ShowMessageBox(
                    "Problem starting this camera",
                    "Problem starting camera",
                    MessageBoxButtons.OK);
            }
        }

        // ====
        private void ConnectUpCamera_button_Click(object sender, EventArgs e)
        {
            DisplayText("UpCam_comboBox.SelectedIndex= " + UpCam_comboBox.SelectedIndex.ToString());
            Properties.Settings.Default.UpCam_index = UpCam_comboBox.SelectedIndex;
            SelectCamera(UpCamera);
            Properties.Settings.Default.UpcamMoniker = UpCamera.MonikerString;
            if (UpCamera.IsRunning())
            {
                SetCurrentCameraParameters();
            }
            else
            {
                ShowMessageBox(
                    "Problem starting this camera",
                    "Problem starting camera",
                    MessageBoxButtons.OK);
            }
        }

        // =================================================================================
        private void DownCameraDrawDashedCross_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCameraDrawDashedCross_checkBox.Checked)
            {
                DownCamera.DrawDashedCross = true;
            }
            else
            {
                DownCamera.DrawDashedCross = false;
            }
        }

        // ====
        private void UpCameraDrawDashedCross_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCameraDrawDashedCross_checkBox.Checked)
            {
                UpCamera.DrawDashedCross = true;
            }
            else
            {
                UpCamera.DrawDashedCross = false;
            }
        }


        // =================================================================================
        private void DownCameraDrawTicks_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCameraDrawTicks_checkBox.Checked)
            {
                DownCamera.DrawSidemarks = true;
                Properties.Settings.Default.DownCam_DrawTicks = true;
            }
            else
            {
                DownCamera.DrawSidemarks = false;
                Properties.Settings.Default.DownCam_DrawTicks = false;
            }
        }

        // =================================================================================
        private void DownCameraDrawCross_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCameraDrawCross_checkBox.Checked)
            {
                DownCamera.DrawCross = true;
            }
            else
            {
                DownCamera.DrawCross = false;
            }
        }

        // ====
        private void UpCameraDrawCross_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCameraDrawCross_checkBox.Checked)
            {
                UpCamera.DrawCross = true;
            }
            else
            {
                UpCamera.DrawCross = false;
            }
        }

        // =================================================================================
        private void DownCameraDrawBox_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCameraDrawBox_checkBox.Checked)
            {
                DownCamera.DrawBox = true;
            }
            else
            {
                DownCamera.DrawBox = false;
            }
        }

        // ====
        private void UpCameraDrawBox_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCameraDrawBox_checkBox.Checked)
            {
                UpCamera.DrawBox = true;
            }
            else
            {
                UpCamera.DrawBox = false;
            }
        }

        // =================================================================================
        private void DownCameraBoxX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                DownCameraUpdateXmmPerPixel();
            }
        }

        private void DownCameraBoxX_textBox_Leave(object sender, EventArgs e)
        {
            DownCameraUpdateXmmPerPixel();
        }


        private void DownCameraUpdateXmmPerPixel()
        {
            double val;
            if (double.TryParse(DownCameraBoxX_textBox.Text, out val))
            {
                Properties.Settings.Default.DownCam_XmmPerPixel = val / DownCamera.BoxSizeX;
                DownCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // ====
        private void UpCameraBoxX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                UpCameraUpdateXmmPerPixel();
            }
        }

        private void UpCameraBoxX_textBox_Leave(object sender, EventArgs e)
        {
            UpCameraUpdateXmmPerPixel();
        }

        private void UpCameraUpdateXmmPerPixel()
        {
            double val;
            if (double.TryParse(UpCameraBoxX_textBox.Text, out val))
            {
                Properties.Settings.Default.UpCam_XmmPerPixel = val / UpCamera.BoxSizeX;
                UpCameraBoxXmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // =================================================================================
        private void DownCameraBoxY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                DownCameraUpdateYmmPerPixel();
            }
        }

        private void DownCameraBoxY_textBox_Leave(object sender, EventArgs e)
        {
            DownCameraUpdateYmmPerPixel();
        }

        private void DownCameraUpdateYmmPerPixel()
        {
            double val;
            if (double.TryParse(DownCameraBoxY_textBox.Text, out val))
            {
                Properties.Settings.Default.DownCam_YmmPerPixel = val / DownCamera.BoxSizeY;
                DownCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.DownCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // ====
        private void UpCameraBoxY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                UpCameraUpdateYmmPerPixel();
            }
        }

        private void UpCameraBoxY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpCameraBoxY_textBox.Text, out val))
            {
                UpCameraUpdateYmmPerPixel();
            }
        }

        private void UpCameraUpdateYmmPerPixel()
        {
            double val;
            if (double.TryParse(UpCameraBoxY_textBox.Text, out val))
            {
                Properties.Settings.Default.UpCam_YmmPerPixel = val / UpCamera.BoxSizeY;
                UpCameraBoxYmmPerPixel_label.Text = "(" + Properties.Settings.Default.UpCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }



        // =================================================================================
        private void DownCamZoom_checkBox_Click(object sender, EventArgs e)
        {
            if (DownCamZoom_checkBox.Checked)
            {
                DownCamera.Zoom = true;
                Properties.Settings.Default.DownCam_Zoom = true;
            }
            else
            {
                DownCamera.Zoom = false;
                Properties.Settings.Default.DownCam_Zoom = false;
            }
        }
        // ====
        private void UpCamZoom_checkBox_Click(object sender, EventArgs e)
        {
            if (UpCamZoom_checkBox.Checked)
            {
                UpCamera.Zoom = true;
                Properties.Settings.Default.UpCam_Zoom = true;
            }
            else
            {
                UpCamera.Zoom = false;
                Properties.Settings.Default.UpCam_Zoom = false;
            }
        }

        // =================================================================================
        private void DownCamZoomFactor_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(DownCamZoomFactor_textBox.Text, out val))
                {
                    DownCamera.ZoomFactor = val;
                    Properties.Settings.Default.DownCam_Zoomfactor = val;
                }
            }
        }

        private void DownCamZoomFactor_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(DownCamZoomFactor_textBox.Text, out val))
            {
                DownCamera.ZoomFactor = val;
                Properties.Settings.Default.DownCam_Zoomfactor = val;
            }
        }

        // ====
        private void UpCamZoomFactor_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(UpCamZoomFactor_textBox.Text, out val))
                {
                    UpCamera.ZoomFactor = val;
                    Properties.Settings.Default.UpCam_Zoomfactor = val;
                }
            }
        }

        private void UpCamZoomFactor_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpCamZoomFactor_textBox.Text, out val))
            {
                UpCamera.ZoomFactor = val;
                Properties.Settings.Default.UpCam_Zoomfactor = val;
            }
        }


        // ==========================================================================================================

        private void DownCamFindCircles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCamFindCircles_checkBox.Checked)
            {
                DownCamera.FindCircles = true;
            }
            else
            {
                DownCamera.FindCircles = false;
            }
        }

        private void UpCamFindCircles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCamFindCircles_checkBox.Checked)
            {
                UpCamera.FindCircles = true;
            }
            else
            {
                UpCamera.FindCircles = false;
            }
        }

        private void DownCamFindRectangles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCamFindRectangles_checkBox.Checked)
            {
                DownCamera.FindRectangles = true;
            }
            else
            {
                DownCamera.FindRectangles = false;
            }

        }

        private void DownCam_FindComponents_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DownCam_FindComponents_checkBox.Checked)
            {
                DownCamera.FindComponent = true;
            }
            else
            {
                DownCamera.FindComponent = false;
            }
        }

        private void UpCam_FindComponents_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCam_FindComponents_checkBox.Checked)
            {
                UpCamera.FindComponent = true;
            }
            else
            {
                UpCamera.FindComponent = false;
            }
        }


        // =================================================================================
        // DownCam specific functions
        // =================================================================================

        // =================================================================================
        private void ImageTest_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageTest_checkBox.Checked)
            {
                DownCamera.TestAlgorithm = true;
            }
            else
            {
                DownCamera.TestAlgorithm = false;
            }
        }

        // =================================================================================
        private void Overlay_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Overlay_checkBox.Checked)
            {
                DownCamera.Draw_Snapshot = true;
            }
            else
            {
                DownCamera.Draw_Snapshot = false;
            }
        }

        // =================================================================================
        private void JigX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JigX_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_JigOffsetX = val;
                }
            }
        }

        private void JigX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JigX_textBox.Text, out val))
            {
                Properties.Settings.Default.General_JigOffsetX = val;
            }
        }

        // =================================================================================
        private void JigY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JigY_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_JigOffsetY = val;
                }
            }
        }

        private void JigY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JigY_textBox.Text, out val))
            {
                Properties.Settings.Default.General_JigOffsetY = val;
            }
        }

        // =================================================================================
        private void GotoPCB0_button_Click(object sender, EventArgs e)
        {
            CNC_XY_m(Properties.Settings.Default.General_JigOffsetX, Properties.Settings.Default.General_JigOffsetY);
        }

        // =================================================================================
        private void SetPCB0_button_Click(object sender, EventArgs e)
        {
            JigX_textBox.Text = Cnc.CurrentX.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_JigOffsetX = Cnc.CurrentX;
            JigY_textBox.Text = Cnc.CurrentY.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_JigOffsetY = Cnc.CurrentY;
        }



        // =================================================================================
        private void PickupCenterX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            PickupCenterX_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(PickupCenterX_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_PickupCenterX = val;
                    PickupCenterX_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void PickupCenterX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(PickupCenterX_textBox.Text, out val))
            {
                Properties.Settings.Default.General_PickupCenterX = val;
                PickupCenterX_textBox.ForeColor = Color.Black;
            }
        }

        // =================================================================================
        private void PickupCenterY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            PickupCenterY_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(PickupCenterY_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_PickupCenterY = val;
                    PickupCenterY_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void PickupCenterY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(PickupCenterY_textBox.Text, out val))
            {
                Properties.Settings.Default.General_PickupCenterY = val;
                PickupCenterY_textBox.ForeColor = Color.Black;
            }
        }

        // =================================================================================
        private void GotoPickupCenter_button_Click(object sender, EventArgs e)
        {
            CNC_XY_m(Properties.Settings.Default.General_PickupCenterX, Properties.Settings.Default.General_PickupCenterY);
        }

        // =================================================================================
        private void SetPickupCenter_button_Click(object sender, EventArgs e)
        {
            PickupCenterX_textBox.Text = Cnc.CurrentX.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_PickupCenterX = Cnc.CurrentX;
            PickupCenterY_textBox.Text = Cnc.CurrentY.ToString("0.00", CultureInfo.InvariantCulture);
            Properties.Settings.Default.General_PickupCenterY = Cnc.CurrentY;
            PickupCenterX_textBox.ForeColor = Color.Black;
            PickupCenterY_textBox.ForeColor = Color.Black;
        }

        // =================================================================================
        // Needle calibration

        private static int SetNeedleOffset_stage = 0;
        private static double NeedleOffsetMarkX = 0.0;
        private static double NeedleOffsetMarkY = 0.0;

        private void ZDown_button_Click(object sender, EventArgs e)
        {
            ZGuardOff();
            CNC_Z_m(Properties.Settings.Default.General_ZtoPCB);
        }

        private void ZUp_button_Click(object sender, EventArgs e)
        {
            ZGuardOn();
            CNC_Z_m(0);
        }

        private void NeedleOffsetX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(NeedleOffsetX_textBox.Text, out val))
                {
                    Properties.Settings.Default.DownCam_NeedleOffsetX = val;
                }
            }
        }

        private void NeedleOffsetX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(NeedleOffsetX_textBox.Text, out val))
            {
                Properties.Settings.Default.DownCam_NeedleOffsetX = val;
            }
        }

        private void NeedleOffsetY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(NeedleOffsetY_textBox.Text, out val))
                {
                    Properties.Settings.Default.DownCam_NeedleOffsetY = val;
                }
            }
        }

        private void NeedleOffsetY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(NeedleOffsetY_textBox.Text, out val))
            {
                Properties.Settings.Default.DownCam_NeedleOffsetY = val;
            }
        }


        private void Offset2Method_button_Click(object sender, EventArgs e)
        {
            ZGuardOff();
            SelectCamera(DownCamera);
            SetCurrentCameraParameters();
            switch (SetNeedleOffset_stage)
            {
                case 0:
                    SetNeedleOffset_stage = 1;
                    Offset2Method_button.Text = "Next";
                    CNC_A_m(0.0);
                    NeedleOffset_label.Visible = true;
                    NeedleOffset_label.Text = "Jog needle to a point on a PCB, then click \"Next\"";
                    break;

                case 1:
                    SetNeedleOffset_stage = 2;
                    NeedleOffsetMarkX = Cnc.CurrentX;
                    NeedleOffsetMarkY = Cnc.CurrentY;
                    CNC_Z_m(0);
                    CNC_XY_m(Cnc.CurrentX - 75.0, Cnc.CurrentY - 29.0);
                    DownCamera.DrawCross = true;
                    NeedleOffset_label.Text = "Jog camera above the same point, \n\rthen click \"Next\"";
                    break;

                case 2:
                    SetNeedleOffset_stage = 0;
                    Properties.Settings.Default.DownCam_NeedleOffsetX = NeedleOffsetMarkX - Cnc.CurrentX;
                    Properties.Settings.Default.DownCam_NeedleOffsetY = NeedleOffsetMarkY - Cnc.CurrentY;
                    NeedleOffsetX_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetX.ToString("0.00", CultureInfo.InvariantCulture);
                    NeedleOffsetY_textBox.Text = Properties.Settings.Default.DownCam_NeedleOffsetY.ToString("0.00", CultureInfo.InvariantCulture);
                    NeedleOffset_label.Visible = false;
                    NeedleOffset_label.Text = "   ";
                    ShowMessageBox(
                        "Now, jog the needle above the up camera,\n\rtake needle down, jog it to the image center\n\rand set Up Camera location",
                        "Done here",
                        MessageBoxButtons.OK);
                    SelectCamera(UpCamera);
                    SetNeedleMeasurement();
                    Offset2Method_button.Text = "Start";
                    CNC_Z_m(0.0);
                    ZGuardOn();
                    break;
            }
        }


        // =================================================================================
        // UpCam specific functions
        // =================================================================================

        private void UpcamPositionX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double val;
                if (double.TryParse(UpcamPositionX_textBox.Text, out val))
                {
                    Properties.Settings.Default.UpCam_PositionX = val;
                }
            }
        }

        private void UpcamPositionX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpcamPositionX_textBox.Text, out val))
            {
                Properties.Settings.Default.UpCam_PositionX = val;
            }
        }

        private void UpcamPositionY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double val;
                if (double.TryParse(UpcamPositionY_textBox.Text, out val))
                {
                    Properties.Settings.Default.UpCam_PositionY = val;
                }
            }
        }

        private void UpcamPositionY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpcamPositionY_textBox.Text, out val))
            {
                Properties.Settings.Default.UpCam_PositionY = val;
            }
        }

        private void SetUpCamPosition_button_Click(object sender, EventArgs e)
        {
            UpcamPositionX_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Properties.Settings.Default.UpCam_PositionX = Cnc.CurrentX;
            UpcamPositionY_textBox.Text = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            Properties.Settings.Default.UpCam_PositionY = Cnc.CurrentY;
            DisplayText("True position (with needle offset):");
            DisplayText("X: " + (Cnc.CurrentX - Properties.Settings.Default.DownCam_NeedleOffsetX).ToString());
            DisplayText("Y: " + (Cnc.CurrentY - Properties.Settings.Default.DownCam_NeedleOffsetY).ToString());
        }

        private void GotoUpCamPosition_button_Click(object sender, EventArgs e)
        {
            CNC_XY_m(Properties.Settings.Default.UpCam_PositionX, Properties.Settings.Default.UpCam_PositionY);
        }

        #endregion Up/Down Camera setup pages functions

        // =================================================================================
        // Basic setup page functions
        // =================================================================================
        #region Basic setup page functions

        private void BasicSetupTab_Begin()
        {
            SetDownCameraDefaults();

            UpCamera.Active = false;
            DownCamera.Active = false;

            UpdateCncConnectionStatus();
            SizeXMax_textBox.Text = Properties.Settings.Default.General_MachineSizeX.ToString();
            SizeYMax_textBox.Text = Properties.Settings.Default.General_MachineSizeY.ToString();

            ParkLocationX_textBox.Text = Properties.Settings.Default.General_ParkX.ToString();
            ParkLocationY_textBox.Text = Properties.Settings.Default.General_ParkY.ToString();
            SquareCorrection_textBox.Text = Properties.Settings.Default.CNC_SquareCorrection.ToString();
            VacuumTime_textBox.Text = Properties.Settings.Default.General_PickupVacuumTime.ToString();
            VacuumRelease_textBox.Text = Properties.Settings.Default.General_PickupReleaseTime.ToString();
            SmallMovement_numericUpDown.Value = Properties.Settings.Default.CNC_SmallMovementSpeed;

            CtlrJogSpeed_numericUpDown.Value = Properties.Settings.Default.CNC_CtlrJogSpeed;
            NormalJogSpeed_numericUpDown.Value = Properties.Settings.Default.CNC_NormalJogSpeed;
            AltJogSpeed_numericUpDown.Value = Properties.Settings.Default.CNC_AltJogSpeed;

            // Does this machine have any ports? (Maybe not, if TinyG is powered down.)
            RefreshPortList();
            if (comboBoxSerialPorts.Items.Count == 0)
            {
                return;
            };

            // At least there are some ports. Show the default port, if it is still there:
            bool found = false;
            int i = 0;
            foreach (var item in comboBoxSerialPorts.Items)
            {
                if (item.ToString() == Properties.Settings.Default.CNC_SerialPort)
                {
                    found = true;
                    comboBoxSerialPorts.SelectedIndex = i;
                    break;
                }
                i++;
            }
            if (found)
            {
                // Yes, the default port is still there, show it
                comboBoxSerialPorts.SelectedIndex = i;
            }
            else
            {
                // show the first available port
                comboBoxSerialPorts.SelectedIndex = 0;
                return;
            }
        }

        private void BasicSetupTab_End()
        {
            ZGuardOn();
        }

        private void buttonRefreshPortList_Click(object sender, EventArgs e)
        {
            RefreshPortList();
        }

        private void RefreshPortList()
        {
            comboBoxSerialPorts.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBoxSerialPorts.Items.Add(s);
            }
            if (comboBoxSerialPorts.Items.Count == 0)
            {
                labelSerialPortStatus.Text = "No serial ports found. Is TinyG powered on?";
            }
            else
            {
                // show the first available port
                comboBoxSerialPorts.SelectedIndex = 0;
                return;
            }
        }

        public void CncError()
        {
            Cnc.ErrorState = true;
            UpdateCncConnectionStatus();
        }

        public void UpdateCncConnectionStatus()
        {
            if (InvokeRequired) { Invoke(new Action(UpdateCncConnectionStatus)); return; }

            if (Cnc.Connected)
            {
                if (Cnc.ErrorState)
                {
                    buttonConnectSerial.Text = "Clear Err.";
                    labelSerialPortStatus.Text = "ERROR";
                    labelSerialPortStatus.ForeColor = Color.Red;
                    ValidMeasurement_checkBox.Checked = false;
                }
                else
                {
                    buttonConnectSerial.Text = "Close";
                    labelSerialPortStatus.Text = "Connected";
                    labelSerialPortStatus.ForeColor = Color.Black;
                }
            }
            else
            {
                buttonConnectSerial.Text = "Connect";
                labelSerialPortStatus.Text = "Not connected";
                labelSerialPortStatus.ForeColor = Color.Red;
                ValidMeasurement_checkBox.Checked = false;
            }
        }

        private void buttonConnectSerial_Click(object sender, EventArgs e)
        {
            if (comboBoxSerialPorts.SelectedItem == null)
            {
                return;
            };

            if (!Cnc.Connected)
            {
                // reconnect
                if (Cnc.Connect(comboBoxSerialPorts.SelectedItem.ToString()))
                {
                    Properties.Settings.Default.CNC_SerialPort = comboBoxSerialPorts.SelectedItem.ToString();
                    if (!UpdateWindowValues_m())
                    {
                        CncError();
                    }
                }
            }
            else if (Cnc.ErrorState)
            {
                // Attempt to clear the error
                Cnc.ErrorState = false;
                if (!UpdateWindowValues_m())
                {
                    CncError();
                }
            }
            else
            {
                // Close connection
                Cnc.Close();
                Thread.Sleep(250);
            }
            UpdateCncConnectionStatus();
        }


        // =================================================================================
        // Logging textbox

        Color AppCol = Color.Black;
        public void DisplayText(string txt, KnownColor col = KnownColor.Black, bool force= false)
        {
            if (DisableLog_checkBox.Checked && !force)
            {
                return;
            }
            // intermediate step to get the invoke... work with calls with one or two parameters
            AppCol = Color.FromName(col.ToString());
            DisplayTxt(txt);
        }

        public void DisplayTxt(string txt)
        {
            try
            {
                if (InvokeRequired) { Invoke(new Action<string>(DisplayTxt), new[] { txt }); return; }
                txt = txt.Replace("\n", "");
                // TinyG sends \n, textbox needs \r\n. (TinyG could be set to send \n\r, which does not work with textbox.)
                // Adding end of line here saves typing elsewhere
                txt = txt + "\r\n";
                if (SerialMonitor_richTextBox.Text.Length > 1000000)
                {
                    SerialMonitor_richTextBox.Text = SerialMonitor_richTextBox.Text.Substring(SerialMonitor_richTextBox.Text.Length - 10000);
                }
                SerialMonitor_richTextBox.AppendText(txt, AppCol);
                SerialMonitor_richTextBox.ScrollToCaret();
            }
            catch
            {
            }
        }


        private void textBoxSendtoTinyG_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                Cnc.ForceWrite(textBoxSendtoTinyG.Text);
                textBoxSendtoTinyG.Clear();
            }
        }

        // Sends the calls that will result to messages that update the values shown
        private bool UpdateWindowValues_m()
        {
            if (!CNC_Write_m("{\"sr\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"xjm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"xvm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"xsv\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"xsn\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"xjh\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"xsx\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"1mi\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"1sa\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"1tr\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"yjm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"yvm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"ysn\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"ysx\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"yjh\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"ysv\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"2mi\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"2sa\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"2tr\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"zjm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"zvm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"zsn\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"zsx\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"zjh\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"zsv\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"3mi\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"3sa\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"3tr\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"ajm\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"avm\":\"\"}"))
            {
                return false;
            };

            if (!CNC_Write_m("{\"4mi\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"4sa\":\"\"}"))
            {
                return false;
            };
            if (!CNC_Write_m("{\"4tr\":\"\"}"))
            {
                return false;
            };

            // Do settings that need to be done always
            Cnc.IgnoreError = true;
            Needle.ProbingMode(false, JSON);
            //PumpDefaultSetting();
            //VacuumDefaultSetting();
            //Thread.Sleep(100);
            //Vacuum_checkBox.Checked = true;
            //Cnc.IgnoreError = false;
            CNC_Write_m("{\"me\":\"\"}");  // motor power on
            MotorPower_checkBox.Checked = true;
            return true;
        }

        // Called from CNC class when UI need updating
        public void ValueUpdater(string item, string value)
        {
            if (InvokeRequired) { Invoke(new Action<string, string>(ValueUpdater), new[] { item, value }); return; }

            switch (item)
            {
                case "posx": Update_xpos(value);
                    break;
                case "posy": Update_ypos(value);
                    break;
                case "posz": Update_zpos(value);
                    break;
                case "posa": Update_apos(value);
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

                case "1tr": Update_1tr(value);
                    break;
                case "2tr": Update_2tr(value);
                    break;
                case "3tr": Update_3tr(value);
                    break;
                case "4tr": Update_4tr(value);
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
        private void Update_xjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_yjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_zjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjm_maskedTextBox.Text = val.ToString();
        }

        private void Update_ajm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ajm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            ajm_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *jm setting
        private void xjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                zjm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void ajm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ajm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "000000}");
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

        private void Update_xjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjh_maskedTextBox.Text = val.ToString();
        }

        private void Update_yjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjh_maskedTextBox.Text = val.ToString();
        }

        private void Update_zjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjh_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *jh setting

        private void xjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjh_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjh_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
#if (TINYG_SHORTUNITS)
                CNC_Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "}");
#else
                CNC_Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "000000}");
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

        private void Update_xsv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xsv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            xsv_maskedTextBox.Text = val.ToString();
        }

        private void Update_ysv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ysv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            ysv_maskedTextBox.Text = val.ToString();
        }

        private void Update_zsv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zsv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            zsv_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *sv setting

        private void xsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"xsv\":" + xsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                xsv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void ysv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ysv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"ysv\":" + ysv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                ysv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"zsv\":" + zsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zsv_maskedTextBox.ForeColor = Color.Black;
            }
        }

        #endregion

        // =========================================================================
        #region sn
        // *sn: Negative limit switch
        // *sn update

        private void Update_xsn(string value)
        {
            switch (value)
            {
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

        private void Update_ysn(string value)
        {
            switch (value)
            {
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

        private void Update_zsn(string value)
        {
            switch (value)
            {
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

        private void Xhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            CNC_Write_m("{\"xsn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        private void Xlim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            CNC_Write_m("{\"xsn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        private void Yhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            CNC_Write_m("{\"ysn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        private void Ylim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            CNC_Write_m("{\"ysn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        private void Zhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            CNC_Write_m("{\"zsn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        private void Zlim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            CNC_Write_m("{\"zsn\":" + i.ToString() + "}");
            Thread.Sleep(50);
        }

        #endregion

        // =========================================================================
        #region sx
        // *sx: Maximum limit switch
        // *sx update

        private void Update_xsx(string value)
        {
            if (value == "2")
            {
                Xmax_checkBox.Checked = true;
            }
            else
            {
                Xmax_checkBox.Checked = false;
            }
        }

        private void Update_ysx(string value)
        {
            if (value == "2")
            {
                Ymax_checkBox.Checked = true;
            }
            else
            {
                Ymax_checkBox.Checked = false;
            }
        }

        private void Update_zsx(string value)
        {
            if (value == "2")
            {
                Zmax_checkBox.Checked = true;
            }
            else
            {
                Zmax_checkBox.Checked = false;
            }
        }

        // =========================================================================
        // *sx setting

        private void Xmax_checkBox_Click(object sender, EventArgs e)
        {
            if (Xmax_checkBox.Checked)
            {
                CNC_Write_m("{\"xsx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"xsx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Ymax_checkBox_Click(object sender, EventArgs e)
        {
            if (Ymax_checkBox.Checked)
            {
                CNC_Write_m("{\"ysx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"ysx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Zmax_checkBox_Click(object sender, EventArgs e)
        {
            if (Zmax_checkBox.Checked)
            {
                CNC_Write_m("{\"zsx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"zsx\":0}");
                Thread.Sleep(50);
            }
        }

        #endregion

        // =========================================================================
        #region vm
        // *vm: Velocity maximum
        // *vm update

        private void Update_xvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            xvm_maskedTextBox.Text = val.ToString();
        }

        private void Update_yvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            yvm_maskedTextBox.Text = val.ToString();
        }

        private void Update_zvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            zvm_maskedTextBox.Text = val.ToString();
        }


        private void Update_avm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_avm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value);
            val = val / 1000;
            avm_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *vm setting

        private void xvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"xvm\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                CNC_Write_m("{\"xfr\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                xvm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void yvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"yvm\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                CNC_Write_m("{\"yfr\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                yvm_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private void zvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"zvm\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                CNC_Write_m("{\"zfr\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zvm_maskedTextBox.ForeColor = Color.Black;
                int peek = Convert.ToInt32(zvm_maskedTextBox.Text);
                Properties.Settings.Default.CNC_ZspeedMax = peek;
            }
        }

        private void avm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            avm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                CNC_Write_m("{\"avm\":" + avm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                CNC_Write_m("{\"afr\":" + avm_maskedTextBox.Text + "000}");
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

        private void Update_1mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1mi), new[] { value }); return; }

            int val = Convert.ToInt32(value);
            mi1_maskedTextBox.Text = val.ToString();
        }

        private void Update_2mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2mi), new[] { value }); return; }

            int val = Convert.ToInt32(value);
            mi2_maskedTextBox.Text = val.ToString();
        }

        private void Update_3mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3mi), new[] { value }); return; }

            int val = Convert.ToInt32(value);
            mi3_maskedTextBox.Text = val.ToString();
        }

        private void Update_4mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4mi), new[] { value }); return; }

            int val = Convert.ToInt32(value);
            mi4_maskedTextBox.Text = val.ToString();
        }

        // =========================================================================
        // *mi setting

        private void mi1_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            mi1_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if ((mi1_maskedTextBox.Text == "1") || (mi1_maskedTextBox.Text == "2")
                    || (mi1_maskedTextBox.Text == "4") || (mi1_maskedTextBox.Text == "8"))
                {
                    CNC_Write_m("{\"1mi\":" + mi1_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi1_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private void mi2_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            mi2_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if ((mi2_maskedTextBox.Text == "1") || (mi2_maskedTextBox.Text == "2")
                    || (mi2_maskedTextBox.Text == "4") || (mi2_maskedTextBox.Text == "8"))
                {
                    CNC_Write_m("{\"2mi\":" + mi2_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi2_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }


        private void mi3_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            mi3_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if ((mi3_maskedTextBox.Text == "1") || (mi3_maskedTextBox.Text == "2")
                    || (mi3_maskedTextBox.Text == "4") || (mi3_maskedTextBox.Text == "8"))
                {
                    CNC_Write_m("{\"3mi\":" + mi3_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi3_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private void mi4_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            mi4_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if ((mi4_maskedTextBox.Text == "1") || (mi4_maskedTextBox.Text == "2")
                    || (mi4_maskedTextBox.Text == "4") || (mi4_maskedTextBox.Text == "8"))
                {
                    CNC_Write_m("{\"4mi\":" + mi4_maskedTextBox.Text + "}");
                    Thread.Sleep(50);
                    mi4_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        #endregion

        // =========================================================================
        #region tr
        // *tr: Travel per revolution
        // *tr update

        private void Update_1tr(string value)
        {
            tr1_textBox.Text = value;
        }

        private void Update_2tr(string value)
        {
            tr2_textBox.Text = value;
        }

        private void Update_3tr(string value)
        {
            tr3_textBox.Text = value;

        }

        private void Update_4tr(string value)
        {
            tr4_textBox.Text = value;
        }

        // =========================================================================
        // *tr setting
        private void tr1_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr1_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr1_textBox.Text, out val))
                {
                    CNC_Write_m("{\"1tr\":" + tr1_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr1_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr2_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr2_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr2_textBox.Text, out val))
                {
                    CNC_Write_m("{\"2tr\":" + tr2_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr2_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr3_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr3_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr3_textBox.Text, out val))
                {
                    CNC_Write_m("{\"3tr\":" + tr3_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr3_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void tr4_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr4_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr1_textBox.Text, out val))
                {
                    CNC_Write_m("{\"4tr\":" + tr4_textBox.Text + "}");
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

        private void Update_1sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m1deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m1deg18_radioButton.Checked = true;
            }
        }

        private void Update_2sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m2deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m2deg18_radioButton.Checked = true;
            }
        }

        private void Update_3sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m3deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m3deg18_radioButton.Checked = true;
            }
        }

        private void Update_4sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m4deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m4deg18_radioButton.Checked = true;
            }
        }

        // =========================================================================
        // *sa setting

        private void m1deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m1deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m1deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m1deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }


        private void m2deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m2deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m2deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m2deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m3deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m3deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m4deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m4deg09_radioButton.Checked)
            {
                CNC_Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                CNC_Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        #endregion

        // =========================================================================
        #region mpo
        // mpo*: Position
        // * update
        private void Update_xpos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xpos), new[] { value }); return; }
            TrueX_label.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void Update_ypos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ypos), new[] { value }); return; }
            ypos_textBox.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void Update_zpos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zpos), new[] { value }); return; }
            zpos_textBox.Text = value;
        }

        private void Update_apos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_apos), new[] { value }); return; }
            apos_textBox.Text = value;
        }

        #endregion

        // =========================================================================
        #region Machine_Size

        private void SizeXMax_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SizeXMax_textBox.ForeColor = Color.Red;
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SizeXMax_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_MachineSizeX = val;
                    SizeXMax_textBox.ForeColor = Color.Black;
                    DownCamera.SideMarksX = Properties.Settings.Default.General_MachineSizeX / 100;
                }
            }
        }

        private void SizeXMax_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SizeXMax_textBox.Text, out val))
            {
                Properties.Settings.Default.General_MachineSizeX = val;
                SizeXMax_textBox.ForeColor = Color.Black;
                DownCamera.SideMarksX = Properties.Settings.Default.General_MachineSizeX / 100;

            }
        }

        private void SizeYMax_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            SizeYMax_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SizeYMax_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_MachineSizeY = val;
                    SizeYMax_textBox.ForeColor = Color.Black;
                    DownCamera.SideMarksY = Properties.Settings.Default.General_MachineSizeY / 100;
                }
            }
        }

        private void SizeYMax_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SizeYMax_textBox.Text, out val))
            {
                Properties.Settings.Default.General_MachineSizeY = val;
                SizeYMax_textBox.ForeColor = Color.Black;
                DownCamera.SideMarksY = Properties.Settings.Default.General_MachineSizeY / 100;
            }
        }


        #endregion

        // =========================================================================
        #region park_location

        private void ParkLocationX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(ParkLocationX_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_ParkX = val;
                }
            }
        }

        private void ParkLocationX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ParkLocationX_textBox.Text, out val))
            {
                Properties.Settings.Default.General_ParkX = val;
            }
        }

        private void ParkLocationY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(ParkLocationY_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_ParkY = val;
                }
            }
        }

        private void ParkLocationY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ParkLocationY_textBox.Text, out val))
            {
                Properties.Settings.Default.General_ParkY = val;
            }
        }

        private void Park_button_Click(object sender, EventArgs e)
        {
            CNC_Park();
        }

        #endregion

        // =========================================================================

        private void reset_button_Click(object sender, EventArgs e)
        {
            Cnc.Write("\x18");
        }

        private void TestX_thread()
        {
            //if (!CNC_XY_m(0.0, Cnc.CurrentY))
            //    return;
            //if (!CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, Cnc.CurrentY))
            //    return;
            //if (!CNC_XY_m(0.0, Cnc.CurrentY))
            //    return;
        }

        private void TestX_button_Click(object sender, EventArgs e)
        {
            if (!CNC_XY_m(0.0, Cnc.CurrentY))
                return;
            if (!CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, Cnc.CurrentY))
                return;
            if (!CNC_XY_m(0.0, Cnc.CurrentY))
                return;
            //Thread t = new Thread(() => TestX_thread());
            //t.IsBackground = true;
            //t.Start();
        }

        private void TestY_thread()
        {
            if (!CNC_XY_m(Cnc.CurrentX, 0))
                return;
            if (!CNC_XY_m(Cnc.CurrentX, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!CNC_XY_m(Cnc.CurrentX, 0))
                return;
        }

        private void TestY_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestY_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestXYA_thread()
        {
            if (!CNC_XYA_m(0, 0, 0))
                return;
            if (!CNC_XYA_m(Properties.Settings.Default.General_MachineSizeX, Properties.Settings.Default.General_MachineSizeY, 360.0))
                return;
            if (!CNC_XYA_m(0, 0, 0))
                return;
        }

        private void TestXYA_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestXYA_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestXY_thread()
        {
            if (!CNC_XY_m(0, 0))
                return;
            if (!CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!CNC_XY_m(0, 0))
                return;
        }

        private void TestXY_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestXY_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void TestYX_thread()
        {
            if (!CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, 0))
                return;
            if (!CNC_XY_m(0, Properties.Settings.Default.General_MachineSizeY))
                return;
            if (!CNC_XY_m(Properties.Settings.Default.General_MachineSizeX, 0))
                return;
        }

        private void TestYX_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestYX_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void HomeX_button_Click(object sender, EventArgs e)
        {
            CNC_Home_m("X");
        }

        private void HomeXY_button_Click(object sender, EventArgs e)
        {
            if (!CNC_Home_m("X"))
                return;
            CNC_Home_m("Y");
        }

        private void HomeY_button_Click(object sender, EventArgs e)
        {
            CNC_Home_m("Y");
        }

        private void HomeZ_button_Click(object sender, EventArgs e)
        {
            Needle.ProbingMode(false, JSON);
            CNC_Home_m("Z");
        }

        private void TestZ_thread()
        {
            if (!CNC_Z_m(0))
                return;
            if (!CNC_Z_m(Properties.Settings.Default.General_ZTestTravel))
                return;
            if (!CNC_Z_m(0))
                return;
        }

        private void TestZ_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestZ_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void ZTestTravel_textBox_TextChanged(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ZTestTravel_textBox.Text, out val))
            {
                Properties.Settings.Default.General_ZTestTravel = val;
            }
        }

        private void ShadeGuard_textBox_TextChanged(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ShadeGuard_textBox.Text, out val))
            {
                Properties.Settings.Default.General_ShadeGuard_mm = val;
            }
        }


        private void TestA_thread()
        {
            if (!CNC_A_m(0))
                return;
            if (!CNC_A_m(360))
                return;
            if (!CNC_A_m(0))
                return;
        }

        private void TestA_button_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => TestA_thread());
            t.IsBackground = true;
            t.Start();
        }

        private void Homebutton_Click(object sender, EventArgs e)
        {
            if (!CNC_Home_m("Z"))
                return;
            if (!CNC_Home_m("X"))
                return;
            if (!CNC_Home_m("Y"))
                return;
            CNC_A_m(0);
        }



        private void MotorPowerOff()
        {
            CNC_Write_m("{\"md\":\"\"}");
        }

        private void MotorPowerOn()
        {
            CNC_Write_m("{\"me\":\"\"}");
        }

        private void MotorPower_checkBox_Click(object sender, EventArgs e)
        {
            if (MotorPower_checkBox.Checked)
            {
                MotorPowerOn();
            }
            else
            {
                MotorPowerOff();
            }
        }

        private void Pump_checkBox_Click(object sender, EventArgs e)
        {
            if (Pump_checkBox.Checked)
            {
                PumpOn();
            }
            else
            {
                PumpOff();
            }
        }

        private void Vacuum_checkBox_Click(object sender, EventArgs e)
        {
            if (Vacuum_checkBox.Checked)
            {
                VacuumOn();
            }
            else
            {
                VacuumOff();
            }
        }

        private void VacuumTime_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int val;
            VacuumTime_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(VacuumTime_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_PickupVacuumTime = val;
                    VacuumTime_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void VacuumTime_textBox_Leave(object sender, EventArgs e)
        {
            int val;
            if (int.TryParse(VacuumTime_textBox.Text, out val))
            {
                Properties.Settings.Default.General_PickupVacuumTime = val;
                VacuumTime_textBox.ForeColor = Color.Black;
            }
        }

        private void VacuumRelease_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int val;
            VacuumRelease_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(VacuumRelease_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_PickupReleaseTime = val;
                    VacuumRelease_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void VacuumRelease_textBox_Leave(object sender, EventArgs e)
        {
            int val;
            if (int.TryParse(VacuumRelease_textBox.Text, out val))
            {
                Properties.Settings.Default.General_PickupReleaseTime = val;
                VacuumRelease_textBox.ForeColor = Color.Black;
            }
        }


        private void SlackCompensation_checkBox_Click(object sender, EventArgs e)
        {
            if (SlackCompensation_checkBox.Checked)
            {
                Cnc.SlackCompensation = true;
                Properties.Settings.Default.CNC_SlackCompensation = true;
            }
            else
            {
                Cnc.SlackCompensation = false;
                Properties.Settings.Default.CNC_SlackCompensation = false;
            }
        }

        private void BuiltInSettings_button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = ShowMessageBox(
                "All your current settings on TinyG will be lost. Are you sure?",
                "Confirm Loading Built-In settings", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            };

            Thread t = new Thread(_BuiltInSettings);
            t.Start();
        }

        private void _BuiltInSettings()
        {
            // global
            // TODO: exeption, exeption handling here
            CNC_Write_m("{\"st\":0}");   // switches NO type            
            Thread.Sleep(50);
            CNC_Write_m("{\"mt\":300}");   // motor timeout 5min
            Thread.Sleep(50);
            CNC_Write_m("{\"jv\":3}");   // JSON verbosity
            Thread.Sleep(50);
            CNC_Write_m("{\"tv\":1}");   // text verbosity
            Thread.Sleep(50);
            CNC_Write_m("{\"qv\":2}");   // queue verbosity
            Thread.Sleep(50);
            CNC_Write_m("{\"sv\":1}");   // Status report verbosity
            Thread.Sleep(50);
            CNC_Write_m("{\"si\":200}");   // Status report interval
            Thread.Sleep(50);
            CNC_Write_m("{\"ec\":0}");   // send LF only
            Thread.Sleep(50);
            CNC_Write_m("{\"ee\":0}");   // echo off
            Thread.Sleep(50);
            CNC_Write_m("{\"gun\":1}");   // use mm's
            Thread.Sleep(50);
            // CNC_RawWrite("f2000");   // default feed rate (important thing that it is not 0)
            CNC_RawWrite("{\"gc\":\"f2000\"}");   // default feed rate (important thing that it is not 0)
            Thread.Sleep(50);

            // Motor 1
            CNC_Write_m("{\"1ma\":0}");   // map 1 to X
            Thread.Sleep(50);
            CNC_Write_m("{\"1sa\":0.9}");   // 0.9 deg. per step
            Thread.Sleep(50);
            CNC_Write_m("{\"1tr\":40.0}");   // 40mm per rev.
            Thread.Sleep(50);
            CNC_Write_m("{\"1mi\":8}");   // microstepping
            Thread.Sleep(50);
            CNC_Write_m("{\"1po\":0}");   // normal polarity
            Thread.Sleep(50);
            CNC_Write_m("{\"1pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 2
            CNC_Write_m("{\"2ma\":1}");   // map 2 to Y
            Thread.Sleep(50);
            CNC_Write_m("{\"2sa\":0.9}");   // 0.9 deg. per step
            Thread.Sleep(50);
            CNC_Write_m("{\"2tr\":40.0}");   // 40mm per rev.
            Thread.Sleep(50);
            CNC_Write_m("{\"2mi\":8}");   // microstepping
            Thread.Sleep(50);
            CNC_Write_m("{\"2po\":0}");   // normal polarity
            Thread.Sleep(50);
            CNC_Write_m("{\"2pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 3
            CNC_Write_m("{\"3ma\":2}");   // map 3 to Z
            Thread.Sleep(50);
            CNC_Write_m("{\"3sa\":1.8}");   // 1.8 deg. per step
            Thread.Sleep(50);
            CNC_Write_m("{\"3tr\":8.0}");   // 8mm per rev.
            Thread.Sleep(50);
            CNC_Write_m("{\"3mi\":8}");   // microstepping
            Thread.Sleep(50);
            CNC_Write_m("{\"3po\":0}");   // normal polarity
            Thread.Sleep(50);
            CNC_Write_m("{\"3pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // Motor 4
            CNC_Write_m("{\"4ma\":3}");   // map 4 to A
            Thread.Sleep(50);
            CNC_Write_m("{\"4sa\":0.9}");   // 1.8 deg. per step
            Thread.Sleep(50);
            CNC_Write_m("{\"4tr\":160.0}");   // 80 deg. per rev.
            Thread.Sleep(50);
            CNC_Write_m("{\"4mi\":8}");   // microstepping
            Thread.Sleep(50);
            CNC_Write_m("{\"4po\":0}");   // normal polarity
            Thread.Sleep(50);
            CNC_Write_m("{\"4pm\":2}");   // keep powered (for "mt" seconds after movement)
            Thread.Sleep(50);

            // X
            CNC_Write_m("{\"xam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            CNC_Write_m("{\"xvm\":10000}");   // max velocity (proto 20000)
            Thread.Sleep(50);
            CNC_Write_m("{\"xfr\":10000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            CNC_Write_m("{\"xtm\":600}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            CNC_Write_m("{\"xjm\":1000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            CNC_Write_m("{\"xjh\":2000}");   // homing jerk (== xjm)
            Thread.Sleep(50);
#else
            CNC_Write_m("{\"xjm\":1000000000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            CNC_Write_m("{\"xjh\":2000000000}");   // homing jerk (== xjm)
            Thread.Sleep(50);
#endif
            CNC_Write_m("{\"xjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            CNC_Write_m("{\"xsn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"xsx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"xsv\":2000}");   // homing speed
            Thread.Sleep(50);
            CNC_Write_m("{\"xlv\":100}");   // latch speed
            Thread.Sleep(50);
            CNC_Write_m("{\"xlb\":8}");   // latch backup
            Thread.Sleep(50);
            CNC_Write_m("{\"xzb\":2}");   // zero backup
            Thread.Sleep(50);

            // Y
            CNC_Write_m("{\"yam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            CNC_Write_m("{\"yvm\":10000}");   // max velocity (proto 20000)
            Thread.Sleep(50);
            CNC_Write_m("{\"yfr\":10000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            CNC_Write_m("{\"ytm\":400}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            CNC_Write_m("{\"yjm\":1000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            CNC_Write_m("{\"yjh\":2000}");   // homing jerk (== yjm)
            Thread.Sleep(50);
#else
            CNC_Write_m("{\"yjm\":1000000000}");   // max jerk (proto 2000)
            Thread.Sleep(50);
            CNC_Write_m("{\"yjh\":2000000000}");   // homing jerk (== yjm)
            Thread.Sleep(50);
#endif
            CNC_Write_m("{\"yjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            CNC_Write_m("{\"ysn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"ysx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"ysv\":2000}");   // homing speed
            Thread.Sleep(50);
            CNC_Write_m("{\"ylv\":100}");   // latch speed
            Thread.Sleep(50);
            CNC_Write_m("{\"ylb\":8}");   // latch backup
            Thread.Sleep(50);
            CNC_Write_m("{\"yzb\":2}");   // zero backup
            Thread.Sleep(50);

            // Z
            CNC_Write_m("{\"zam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            CNC_Write_m("{\"zvm\":5000}");   // max velocity (proto 10000)
            Thread.Sleep(50);
            CNC_Write_m("{\"zfr\":2000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            CNC_Write_m("{\"ztm\":80}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            CNC_Write_m("{\"zjm\":500}");   // max jerk (proto 1000)
            Thread.Sleep(50);
            CNC_Write_m("{\"zjh\":500}");   // homing jerk (== zjm)
            Thread.Sleep(50);
#else
            CNC_Write_m("{\"zjm\":500000000}");   // max jerk (proto 1000)
            Thread.Sleep(50);
            CNC_Write_m("{\"zjh\":500000000}");   // homing jerk (== zjm)
            Thread.Sleep(50);
#endif
            CNC_Write_m("{\"zjd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            CNC_Write_m("{\"zsn\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"zsx\":0}");   // disable switches (!)
            Thread.Sleep(50);
            CNC_Write_m("{\"zsv\":1000}");   // homing speed
            Thread.Sleep(50);
            CNC_Write_m("{\"zlv\":100}");   // latch speed
            Thread.Sleep(50);
            CNC_Write_m("{\"zlb\":4}");   // latch backup
            Thread.Sleep(50);
            CNC_Write_m("{\"zzb\":2}");   // zero backup
            Thread.Sleep(50);

            // A
            CNC_Write_m("{\"aam\":1}");   // mormal axis mode
            Thread.Sleep(50);
            CNC_Write_m("{\"avm\":50000}");   // max velocity (proto 50000)
            Thread.Sleep(50);
            CNC_Write_m("{\"afr\":200000}");   // max feed rate (must be !=0)
            Thread.Sleep(50);
            CNC_Write_m("{\"atm\":400}");   // max homing travel
            Thread.Sleep(50);
#if (TINYG_SHORTUNITS)
            CNC_Write_m("{\"ajm\":5000}");   // max jerk (proto 5000)
            Thread.Sleep(50);
            CNC_Write_m("{\"ajh\":5000}");   // homing jerk (== ajm)
            Thread.Sleep(50);
#else
            CNC_Write_m("{\"ajm\":5000000000}");   // max jerk (proto 5000)
            Thread.Sleep(50);
            CNC_Write_m("{\"ajh\":5000000000}");   // homing jerk (== ajm)
            Thread.Sleep(50);
#endif
            CNC_Write_m("{\"ajd\":0.01}");   // junction deviation (default)
            Thread.Sleep(50);
            CNC_Write_m("{\"asn\":0}");   // disable switches, no homing an A
            Thread.Sleep(50);
            CNC_Write_m("{\"asx\":0}");   // disable switches, no homing an A
            Thread.Sleep(50);
            // No need to touch A homing parameters

            UpdateWindowValues_m();
            ShowMessageBox(
                "Default settings written.",
                "Default settings written",
                MessageBoxButtons.OK);
        }


        private void SaveSettings_button_Click(object sender, EventArgs e)
        {

            CNC_Write_m("{\"sys\":\"\"}");
            CNC_Write_m("{\"x\":\"\"}");
            CNC_Write_m("{\"y\":\"\"}");
            CNC_Write_m("{\"z\":\"\"}");
            CNC_Write_m("{\"a\":\"\"}");
            CNC_Write_m("{\"1\":\"\"}");
            CNC_Write_m("{\"2\":\"\"}");
            CNC_Write_m("{\"3\":\"\"}");
            CNC_Write_m("{\"4\":\"\"}");

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
        }


        private void DefaultSettings_button_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.TinyG_settings_saved)
            {
                ShowMessageBox(
                "You don't have saved User Default settings.",
                "No Saved settings", MessageBoxButtons.OK);
                return;
            }

            DialogResult dialogResult = ShowMessageBox(
                "All your current settings on TinyG will be lost. Are you sure?",
                "Confirm Loading Saved settings", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            DisplayText("Start of DefaultSettings()");
            /*
            DisplayText("sys: " + Properties.Settings.Default.TinyG_sys);
            DisplayText("x: " + Properties.Settings.Default.TinyG_x);
            DisplayText("y: " + Properties.Settings.Default.TinyG_y);
            DisplayText("z: " + Properties.Settings.Default.TinyG_z);
            DisplayText("a: " + Properties.Settings.Default.TinyG_a);
            DisplayText("1: " + Properties.Settings.Default.TinyG_m1);
            DisplayText("2: " + Properties.Settings.Default.TinyG_m2);
            DisplayText("3: " + Properties.Settings.Default.TinyG_m3);
            DisplayText("4: " + Properties.Settings.Default.TinyG_m4");
            */
            CNC_Write_m(Properties.Settings.Default.TinyG_sys);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_x);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_y);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_z);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_a);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_m1);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_m2);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_m3);
            Thread.Sleep(150);
            CNC_Write_m(Properties.Settings.Default.TinyG_m4);
            Thread.Sleep(150);
            UpdateWindowValues_m();
            ShowMessageBox(
                "Settings restored.",
                "Saved settings restored",
                MessageBoxButtons.OK);
        }

        private static int SetProbing_stage = 0;

        private void CancelProbing_button_Click(object sender, EventArgs e)
        {
            SetProbing_stage = 0;
            ZGuardOn();
            CancelProbing_button.Visible = false;
            Zlb_label.Visible = false;
        }

        private void SetProbing_button_Click(object sender, EventArgs e)
        {
            ZGuardOff();
            switch (SetProbing_stage)
            {
                case 0:
                    CancelProbing_button.Visible = true;
                    Zlb_label.Text = "Put a regular height PCB under the needle, \n\rthen click \"Next\"";
                    Zlb_label.Visible = true;
                    SetProbing_button.Text = "Next";
                    SetProbing_stage = 1;
                    break;

                case 1:
                    if (!Needle_ProbeDown_m())
                    {
                        Zlb_label.Text = "";
                        Zlb_label.Visible = false;
                        SetProbing_stage = 0;
                        CancelProbing_button.Visible = false;
                        return;
                    }
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
                    CNC_Home_m("Z");
                    ShowMessageBox(
                       "Probing Backoff set successfully.\n" +
                            "PCB surface: " + Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture) +
                            "\nBackoff:  " + Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture),
                       "Done",
                       MessageBoxButtons.OK);
                    SetProbing_stage = 0;
                    Z0_textBox.Text = Properties.Settings.Default.General_ZtoPCB.ToString("0.00", CultureInfo.InvariantCulture);
                    BackOff_textBox.Text = Properties.Settings.Default.General_ProbingBackOff.ToString("0.00", CultureInfo.InvariantCulture);

                    ZGuardOn();
                    CancelProbing_button.Visible = false;
                    break;
            }
        }


        private void SetMark1_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark1X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark1Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark1Name = Mark1_textBox.Text;
            Bookmark1_button.Text = Properties.Settings.Default.General_Mark1Name;
        }

        private void SetMark2_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark2X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark2Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark2Name = Mark2_textBox.Text;
            Bookmark2_button.Text = Properties.Settings.Default.General_Mark2Name;
        }

        private void SetMark3_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark3X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark3Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark3Name = Mark3_textBox.Text;
            Bookmark3_button.Text = Properties.Settings.Default.General_Mark3Name;
        }

        private void SetMark4_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark4X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark4Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark4Name = Mark4_textBox.Text;
            Bookmark4_button.Text = Properties.Settings.Default.General_Mark4Name;
        }

        private void SetMark5_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark5X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark5Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark5Name = Mark5_textBox.Text;
            Bookmark5_button.Text = Properties.Settings.Default.General_Mark5Name;
        }

        private void SetMark6_button_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.General_Mark6X = Cnc.CurrentX;
            Properties.Settings.Default.General_Mark6Y = Cnc.CurrentY;
            Properties.Settings.Default.General_Mark6Name = Mark6_textBox.Text;
            Bookmark6_button.Text = Properties.Settings.Default.General_Mark6Name;
        }

        private void Bookmark1_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark1X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark1Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark1X, Properties.Settings.Default.General_Mark1Y);
        }

        private void Bookmark2_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark2X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark2Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark2X, Properties.Settings.Default.General_Mark2Y);
        }

        private void Bookmark3_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark3X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark3Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark3X, Properties.Settings.Default.General_Mark3Y);
        }

        private void Bookmark4_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark4X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark4Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark4X, Properties.Settings.Default.General_Mark4Y);
        }

        private void Bookmark5_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark5X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark5Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark5X, Properties.Settings.Default.General_Mark5Y);
        }

        private void Bookmark6_button_Click(object sender, EventArgs e)
        {
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                Properties.Settings.Default.General_Mark6X = Cnc.CurrentX;
                Properties.Settings.Default.General_Mark6Y = Cnc.CurrentY;
                return;
            };
            CNC_XY_m(Properties.Settings.Default.General_Mark6X, Properties.Settings.Default.General_Mark6Y);
        }

        private void SmallMovement_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CNC_SmallMovementSpeed = SmallMovement_numericUpDown.Value;
            Cnc.SmallMovementString = "G1 F" + Properties.Settings.Default.CNC_SmallMovementSpeed.ToString() + " ";
        }

        private void SquareCorrection_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SquareCorrection_textBox.Text, out val))
            {
                Properties.Settings.Default.CNC_SquareCorrection = val;
                CNC.SquareCorrection = val;
            }
        }

        private void SquareCorrection_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SquareCorrection_textBox.Text, out val))
                {
                    Properties.Settings.Default.CNC_SquareCorrection = val;
                    CNC.SquareCorrection = val;
                }
            }
        }

        private void CtlrJogSpeed_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CNC_CtlrJogSpeed = (int)CtlrJogSpeed_numericUpDown.Value;

        }

        private void NormalJogSpeed_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CNC_NormalJogSpeed = (int)NormalJogSpeed_numericUpDown.Value;
        }

        private void AltJogSpeed_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.CNC_AltJogSpeed = (int)AltJogSpeed_numericUpDown.Value;
        }


        #endregion Basic setup page functions

        // =================================================================================
        // Run job page functions
        // =================================================================================
        #region Job page functions

        private double JobOffsetX;
        private double JobOffsetY;

        private class PhysicalComponent
        {
            public string Designator { get; set; }
            public string Footprint { get; set; }
            public double X_nominal { get; set; }
            public double Y_nominal { get; set; }
            public double Rotation { get; set; }
            public double X_machine { get; set; }
            public double Y_machine { get; set; }
            public string Method { get; set; }
            public string MethodParameter { get; set; }
        }

        private void Placement_pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            General_pictureBox_MouseClick(Placement_pictureBox, e.X, e.Y);
        }


        private void RunJob_tabPage_Begin()
        {
            SetDownCameraDefaults();
            DownCamera.ImageBox = Placement_pictureBox;
            SelectCamera(DownCamera);
            if (!DownCamera.IsRunning())
            {
                ShowMessageBox(
                    "Problem starting down camera. Please fix before continuing.",
                    "Down Camera problem",
                    MessageBoxButtons.OK
                );
            }
            DownCamera.DrawCross = true;
            DownCamera.BoxSizeX = 200;
            DownCamera.BoxSizeY = 200;
            DownCamera.BoxRotationDeg = 0;
            DownCamera.TestAlgorithm = false;
            DownCamera.Draw_Snapshot = false;
            DownCamera.FindCircles = false;
            DownCamera.DrawDashedCross = false;
        }

        private void RunJob_tabPage_End()
        {
        }

        // =================================================================================
        // CAD data and Job data load and save functions
        // =================================================================================
        private string CadDataFileName = "";
        private string JobFileName = "";

        private bool LoadCadData_m()
        {
            String[] AllLines;

            // read in CAD data (.csv file)
            if (CAD_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bool result;
                    CadDataFileName = CAD_openFileDialog.FileName;
                    CadFileName_label.Text = Path.GetFileName(CadDataFileName);
                    CadFilePath_label.Text = Path.GetDirectoryName(CadDataFileName);
                    AllLines = File.ReadAllLines(CadDataFileName);
                    if (Path.GetExtension(CAD_openFileDialog.FileName) == ".pos")
                    {
                        result = ParseKiCadData_m(AllLines);
                    }
                    else
                    {
                        result = ParseCadData_m(AllLines, false);
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    ShowMessageBox(
                        "Error in file, Msg: " + ex.Message,
                        "Can't read CAD file",
                        MessageBoxButtons.OK);
                    CadData_GridView.Rows.Clear();
                    CadFileName_label.Text = "--";
                    CadFilePath_label.Text = "--";
                    CadDataFileName = "--";
                    return false;
                };
            }
            else
            {
                return false;
            }
        }

        // =================================================================================
        private bool LoadJobData_m(string JobFileName)
        {
            String[] AllLines;
            try
            {
                AllLines = File.ReadAllLines(JobFileName);
                JobFileName_label.Text = Path.GetFileName(JobFileName);
                JobFilePath_label.Text = Path.GetDirectoryName(JobFileName);
                ParseJobData(AllLines);
            }
            catch (Exception ex)
            {
                ShowMessageBox(
                    "Error in file, Msg: " + ex.Message,
                    "Can't read job file (" + JobFileName + ")",
                    MessageBoxButtons.OK);
                JobData_GridView.Rows.Clear();
                JobFileName_label.Text = "--";
                JobFilePath_label.Text = "--";
                CadDataFileName = "--";
                return false;
            };
            return true;
        }

        // =================================================================================
        private void LoadCadData_button_Click(object sender, EventArgs e)
        {
            ValidMeasurement_checkBox.Checked = false;
            if (LoadCadData_m())
            {
                // Read in job data (.lpj file), if exists
                string ext = Path.GetExtension(CadDataFileName);
                JobFileName = CadDataFileName.Replace(ext, ".lpj");
                if (File.Exists(JobFileName))
                {
                    if (!LoadJobData_m(JobFileName))
                    {
                        ShowMessageBox(
                            "Attempt to read in existing Job Data file failed. Job data automatically created, review situation!",
                            "Job Data load error",
                            MessageBoxButtons.OK);
                        FillJobData_GridView();
                        int dummy;
                        FindFiducials_m(out dummy);  // don't care of the result, just trying to find fids
                    }
                }
                else
                {
                    // If not, build job data ourselves.
                    FillJobData_GridView();
                    int dummy;
                    FindFiducials_m(out dummy);  // don't care of the result, just trying to find fids
                    JobFileName_label.Text = "--";
                    JobFilePath_label.Text = "--";
                }
            }
            else
            {
                // CAD data load failed, clear to false data
                CadData_GridView.Rows.Clear();
                CadFileName_label.Text = "--";
                CadFilePath_label.Text = "--";
                CadDataFileName = "--";
            }
        }

        // =================================================================================
        private void SaveCadData_button_Click(object sender, EventArgs e)
        {
            Stream SaveStream;
            string OutLine;

            if (CadData_GridView.RowCount < 1)
            {
                ShowMessageBox(
                    "No Data",
                    "No Data",
                    MessageBoxButtons.OK
                );
                return;
            }

            Job_saveFileDialog.Filter = "CSV placement files (*.csv)|*.csv|All files (*.*)|*.*";

            if (Job_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((SaveStream = Job_saveFileDialog.OpenFile()) != null)
                {
                    // "Component","Component Type","X, nom.","Y, nom.","Rotation","X, machine","Y, machine","Rotation, machine"
                    using (StreamWriter f = new StreamWriter(SaveStream))
                    {
                        // Write header
                        OutLine = "\"Component\",\"Value\",\"Footprint\",\"X\",\"Y\",\"Rotation\"";
                        f.WriteLine(OutLine);
                        // write data
                        foreach (DataGridViewRow Row in CadData_GridView.Rows)
                        {
                            OutLine = "\"" + Row.Cells["Component"].Value.ToString() + "\"";
                            string temp = Row.Cells["Value_Footprint"].Value.ToString();
                            int i = temp.IndexOf('|');
                            OutLine += ",\"" + temp.Substring(0, i - 2) + "\"";
                            OutLine += ",\"" + temp.Substring(i + 3, temp.Length - i - 3) + "\"";
                            OutLine += ",\"" + Row.Cells["X_nominal"].Value.ToString() + "\"";
                            OutLine += ",\"" + Row.Cells["Y_nominal"].Value.ToString() + "\"";
                            OutLine += ",\"" + Row.Cells["Rotation"].Value.ToString() + "\"";
                            f.WriteLine(OutLine);
                        }
                    }
                    SaveStream.Close();
                }
                CadDataFileName = Job_saveFileDialog.FileName;
                CadFileName_label.Text = Path.GetFileName(CadDataFileName);
                CadFilePath_label.Text = Path.GetDirectoryName(CadDataFileName);
            }
        }

        private void JobDataLoad_button_Click(object sender, EventArgs e)
        {
            if (Job_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                JobFileName = Job_openFileDialog.FileName;
                LoadJobData_m(JobFileName);
                ValidMeasurement_checkBox.Checked = false;
            }
        }

        // =================================================================================
        private void JobDataSave_button_Click(object sender, EventArgs e)
        {
            Stream SaveStream;
            string OutLine;

            if (JobData_GridView.RowCount < 1)
            {
                ShowMessageBox(
                    "No Data",
                    "Empty Job",
                    MessageBoxButtons.OK
                );
                return;
            }

            Job_saveFileDialog.Filter = "LitePlacer Job files (*.lpj)|*.lpj|All files (*.*)|*.*";

            if (Job_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((SaveStream = Job_saveFileDialog.OpenFile()) != null)
                {
                    using (StreamWriter f = new StreamWriter(SaveStream))
                    {
                        // for each row in job datagrid,
                        for (int i = 0; i < JobData_GridView.RowCount; i++)
                        {
                            OutLine = "\"" + JobData_GridView.Rows[i].Cells["ComponentCount"].Value.ToString() + "\"";
                            OutLine += ",\"" + JobData_GridView.Rows[i].Cells["ComponentType"].Value.ToString() + "\"";
                            OutLine += ",\"" + JobData_GridView.Rows[i].Cells["GroupMethod"].Value.ToString() + "\"";
                            OutLine += ",\"" + JobData_GridView.Rows[i].Cells["MethodParamAllComponents"].Value.ToString() + "\"";
                            OutLine += ",\"" + JobData_GridView.Rows[i].Cells["ComponentList"].Value.ToString() + "\"";
                            f.WriteLine(OutLine);
                        }
                    }
                    SaveStream.Close();
                }
                JobFileName = Job_saveFileDialog.FileName;
                JobFileName_label.Text = Path.GetFileName(JobFileName);
                JobFilePath_label.Text = Path.GetDirectoryName(JobFileName);
            }
        }


        // =================================================================================
        private void ParseJobData(String[] AllLines)
        {
            int LineIndex = 0;
            JobData_GridView.Rows.Clear();
            // ComponentCount ComponentType GroupMethod MethodParamAllComponents ComponentList
            foreach (string s in AllLines)
            {
                List<String> Line = SplitCSV(AllLines[LineIndex++], ',');
                JobData_GridView.Rows.Add();
                int Last = JobData_GridView.RowCount - 1;
                JobData_GridView.Rows[Last].Cells["ComponentCount"].Value = Line[0];
                JobData_GridView.Rows[Last].Cells["ComponentType"].Value = Line[1];
                JobData_GridView.Rows[Last].Cells["GroupMethod"].Value = Line[2];
                JobData_GridView.Rows[Last].Cells["MethodParamAllComponents"].Value = Line[3];
                JobData_GridView.Rows[Last].Cells["ComponentList"].Value = Line[4];
            }
            JobData_GridView.ClearSelection();
        }

        // =================================================================================
        // JobData_GridView and CadData_GridView handling
        // JobData_GridView has the components of same type grouped to one component type per line.
        // Public, as it is called from panelize process, too.
        // =================================================================================
        public void FillJobData_GridView()
        {
            string CurrentComponentType = "";
            int ComponentCount = 0;
            JobData_GridView.Rows.Clear();
            int TypeRow;

            foreach (DataGridViewRow InRow in CadData_GridView.Rows)
            {
                CurrentComponentType = InRow.Cells["Value_Footprint"].Value.ToString();
                TypeRow = -1;
                // Have we seen this component type already?
                foreach (DataGridViewRow JobRow in JobData_GridView.Rows)
                {
                    if (JobRow.Cells["ComponentType"].Value.ToString() == CurrentComponentType)
                    {
                        TypeRow = JobRow.Index;
                        break;
                    }
                }
                if (TypeRow == -1)
                {
                    // No, create a new row
                    JobData_GridView.Rows.Add();
                    DataGridViewRow OutRow = JobData_GridView.Rows[JobData_GridView.RowCount - 1];
                    OutRow.Cells["ComponentCount"].Value = "1";
                    OutRow.Cells["ComponentType"].Value = CurrentComponentType;
                    OutRow.Cells["GroupMethod"].Value = "?";
                    OutRow.Cells["MethodParamAllComponents"].Value = "--";
                    OutRow.Cells["ComponentList"].Value = InRow.Cells["Component"].Value.ToString();
                }
                else
                {
                    // Yes, increment component count and add component name to list
                    ComponentCount = Convert.ToInt32(JobData_GridView.Rows[TypeRow].Cells["ComponentCount"].Value.ToString());
                    ComponentCount++;
                    JobData_GridView.Rows[TypeRow].Cells["ComponentCount"].Value = ComponentCount.ToString();
                    // and add component name to list
                    string CurrentComponentList = JobData_GridView.Rows[TypeRow].Cells["ComponentList"].Value.ToString();
                    CurrentComponentList = CurrentComponentList + ',' + InRow.Cells["Component"].Value.ToString();
                    JobData_GridView.Rows[TypeRow].Cells["ComponentList"].Value = CurrentComponentList;
                }
            }
        }


        private void Up_button_Click(object sender, EventArgs e)
        {
            DataGrid_Up_button(JobData_GridView);
        }

        private void Down_button_Click(object sender, EventArgs e)
        {
            DataGrid_Down_button(JobData_GridView);
        }

        private void DeleteComponentGroup_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in JobData_GridView.SelectedCells)
            {
                if (oneCell.Selected)
                    JobData_GridView.Rows.RemoveAt(oneCell.RowIndex);
            }
        }

        private DataGridViewRow ClipBoardRow;

        private void CopyRow_button_Click(object sender, EventArgs e)
        {
            ClipBoardRow = JobData_GridView.CurrentRow;
        }

        private void PasteRow_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < JobData_GridView.ColumnCount; i++)
            {
                JobData_GridView.CurrentRow.Cells[i].Value = ClipBoardRow.Cells[i].Value;
            }
        }


        private void NewRow_button_Click(object sender, EventArgs e)
        {
            int index = JobData_GridView.CurrentRow.Index;
            JobData_GridView.Rows.Insert(index);
            JobData_GridView.Rows[index].Cells["ComponentCount"].Value = "--";
            JobData_GridView.Rows[index].Cells["ComponentType"].Value = "--";
            JobData_GridView.Rows[index].Cells["GroupMethod"].Value = "?";
            JobData_GridView.Rows[index].Cells["MethodParamAllComponents"].Value = "--";
            JobData_GridView.Rows[index].Cells["ComponentList"].Value = "--";
        }

        private void AddCadDataRow_button_Click(object sender, EventArgs e)
        {
            int index = CadData_GridView.CurrentRow.Index;
            CadData_GridView.Rows.Insert(index);
        }

        private void RebuildJobData_button_Click(object sender, EventArgs e)
        {
            // TO DO: Error checking here
            FillJobData_GridView();
            int dummy;
            FindFiducials_m(out dummy);  // don't care of the result, just trying to find fids
            JobFileName_label.Text = "--";
            JobFilePath_label.Text = "--";

        }

        private void DeleteCadDataRow_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in CadData_GridView.SelectedCells)
            {
                if (oneCell.Selected)
                    CadData_GridView.Rows.RemoveAt(oneCell.RowIndex);
            }
        }

        private void CopyCadDataRow_button_Click(object sender, EventArgs e)
        {
            ClipBoardRow = CadData_GridView.CurrentRow;
        }

        private void PasteCadDataRow_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < CadData_GridView.ColumnCount; i++)
            {
                CadData_GridView.CurrentRow.Cells[i].Value = ClipBoardRow.Cells[i].Value;
            }
        }

        // =================================================================================
        // JobData editing
        // =================================================================================
        private void JobData_GridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (JobData_GridView.CurrentCell.ColumnIndex == 2)
            {
                // For method, show a form with explanation texts
                MethodSelectionForm MethodDialog = new MethodSelectionForm();
                MethodDialog.ShowCheckBox = false;
                MethodDialog.ShowDialog(this);
                if (MethodDialog.SelectedMethod != "")
                {
                    foreach (DataGridViewCell cell in JobData_GridView.SelectedCells)
                    {
                        JobData_GridView.Rows[cell.RowIndex].Cells[2].Value = MethodDialog.SelectedMethod;
                    }
                }
                Update_GridView(JobData_GridView);
                return;
            };

            if (JobData_GridView.CurrentCell.ColumnIndex == 3)
            {
                // For method parameter, show the tape selection form if method is "place" 
                int row = JobData_GridView.CurrentCell.RowIndex;
                if ((JobData_GridView.Rows[row].Cells["GroupMethod"].Value.ToString() == "Place") ||
                     (JobData_GridView.Rows[row].Cells["GroupMethod"].Value.ToString() == "Place Fast"))
                {
                    JobData_GridView.Rows[row].Cells["MethodParamAllComponents"].Value = SelectTape("Select tape for "
                        + JobData_GridView.Rows[row].Cells["ComponentType"].Value.ToString());
                    Update_GridView(JobData_GridView);
                    return;
                }
            }
        }

        // If components are edited, update count automatically
        private void JobData_GridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (JobData_GridView.CurrentCell.ColumnIndex == 4)
            {
                // components
                List<String> Line = SplitCSV(JobData_GridView.CurrentCell.Value.ToString(), ',');
                int row = JobData_GridView.CurrentCell.RowIndex;
                JobData_GridView.Rows[row].Cells["ComponentCount"].Value = Line.Count.ToString();
                Update_GridView(JobData_GridView);
            }
        }


        // =================================================================================
        // Do someting to a group of components:
        // =================================================================================
        // Several rows are selected at Job data:

        private void PlaceThese_button_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null; // User might need press enter during the process, which would run this again...
            if (!PrepareToPlace_m())
            {
                ShowMessageBox(
                    "Placement operation failed, nothing done.",
                    "Placement failed",
                    MessageBoxButtons.OK
                 );
                return;
            }

            DataGridViewRow Row;
            bool DoRow = false;
            // Get first row #...
            int FirstRow;
            for (FirstRow = 0; FirstRow < JobData_GridView.RowCount; FirstRow++)
            {
                Row = JobData_GridView.Rows[FirstRow];
                DoRow = false;
                foreach (DataGridViewCell oneCell in Row.Cells)
                {
                    if (oneCell.Selected)
                    {
                        DoRow = true;
                        break;
                    }
                }
                if (DoRow)
                    break;
            }
            if (!DoRow)
            {
                CleanupPlacement(true);
                ShowMessageBox(
                    "Nothing selected, nothing done.",
                    "Done",
                    MessageBoxButtons.OK);
                return;
            };
            // ... so that we can put next row label in place:
            NextGroup_label.Text = JobData_GridView.Rows[FirstRow].Cells["ComponentType"].Value.ToString() + " (" +
                    JobData_GridView.Rows[FirstRow].Cells["ComponentCount"].Value.ToString() + " pcs.)";

            // We know there is something to do, NextGroup_label is updated. Place all rows:
            for (int CurrentRow = 0; CurrentRow < JobData_GridView.RowCount; CurrentRow++)
            {
                // Find the Row we need to do now:
                Row = JobData_GridView.Rows[CurrentRow];
                DoRow = false;
                foreach (DataGridViewCell oneCell in Row.Cells)
                {
                    if (oneCell.Selected)
                    {
                        DoRow = true;
                        break;
                    }
                }

                if (!DoRow)
                {
                    continue;
                };
                // Handle labels:
                PreviousGroup_label.Text = CurrentGroup_label.Text;
                CurrentGroup_label.Text = NextGroup_label.Text;
                // See if there is something more to do, so the "Next" label is corrently updated:
                bool NotLast = false;
                int NextRow;
                for (NextRow = CurrentRow + 1; NextRow < JobData_GridView.RowCount; NextRow++)
                {
                    foreach (DataGridViewCell someCell in JobData_GridView.Rows[NextRow].Cells)
                    {
                        if (someCell.Selected)
                        {
                            NotLast = true;
                            break;
                        }
                    }
                    if (NotLast)
                        break;
                }
                if (NotLast)
                {
                    NextGroup_label.Text = JobData_GridView.Rows[NextRow].Cells["ComponentType"].Value.ToString() + " (" +
                                            JobData_GridView.Rows[NextRow].Cells["ComponentCount"].Value.ToString() + " pcs.)";
                }
                else
                {
                    NextGroup_label.Text = "--";
                };

                // Labels are updated, place the row:
                if (!PlaceRow_m(CurrentRow))
                {
                    ShowMessageBox(
                        "Placement operation failed. Review job status.",
                        "Placement failed",
                        MessageBoxButtons.OK);
                    CleanupPlacement(false);
                    return;
                }
            };

            CleanupPlacement(true);
            ShowMessageBox(
                "Selected components succesfully placed.",
                "Done",
                MessageBoxButtons.OK);
        }


        // =================================================================================
        // This routine places selected component(s) from CAD data grid view:
        private void PlaceOne_button_Click(object sender, EventArgs e)
        {
            // is something actually selected?
            if (CadData_GridView.SelectedCells.Count == 0)
            {
                return;
            };

            // Preparations:
            if (!PrepareToPlace_m())
            {
                ShowMessageBox(
                    "Placement operation failed, nothing done.",
                    "Placement failed",
                    MessageBoxButtons.OK
                 );
                return;
            };

            // if a cell is selected on a row, place that component:
            bool DoRow = false;
            bool ok = true;
            DataGridViewRow CadRow;
            for (int CadRowNo = 0; CadRowNo < CadData_GridView.RowCount; CadRowNo++)
            {
                CadRow = CadData_GridView.Rows[CadRowNo];
                DoRow = false;
                foreach (DataGridViewCell oneCell in CadRow.Cells)
                {
                    if (oneCell.Selected)
                    {
                        DoRow = true;
                        break;
                    }
                }
                if (!DoRow)
                {
                    continue;
                };
                // Found something to do. Find the row from Job data
                string component = CadRow.Cells["Component"].Value.ToString();
                string componentlist;
                int JobRowNo;
                for (JobRowNo = 0; JobRowNo < JobData_GridView.RowCount; JobRowNo++)
                {
                    componentlist = JobData_GridView.Rows[JobRowNo].Cells["ComponentList"].Value.ToString();

                    if ((componentlist.StartsWith(component))
                        || (componentlist.Contains("," + component + ","))
                        || (componentlist.EndsWith(component))
                        )
                    {
                        break;
                    }
                }
                if (JobRowNo >= JobData_GridView.RowCount)
                {
                    PumpOff();
                    ShowMessageBox(
                        "Did not find " + component + " from Job data.",
                        "Job data error",
                        MessageBoxButtons.OK);
                    return;
                }
                // Make a copy of it to the end of the Job data grid view
                JobData_GridView.Rows.Add();
                int LastRowNo = JobData_GridView.Rows.Count - 1;
                for (int i = 0; i < JobData_GridView.ColumnCount; i++)
                {
                    JobData_GridView.Rows[LastRowNo].Cells[i].Value = JobData_GridView.Rows[JobRowNo].Cells[i].Value;
                }

                // Make the copy row to have only one component on it
                JobData_GridView.Rows[LastRowNo].Cells["ComponentCount"].Value = "1";
                JobData_GridView.Rows[LastRowNo].Cells["ComponentList"].Value = component;
                // Update labels
                PreviousGroup_label.Text = CurrentGroup_label.Text;
                CurrentGroup_label.Text = JobData_GridView.Rows[LastRowNo].Cells["ComponentType"].Value.ToString() + " (1 pcs.)";

                // Place that row
                ok = PlaceRow_m(LastRowNo);
                // delete the row
                JobData_GridView.Rows.RemoveAt(LastRowNo);
                if (!ok)
                {
                    break;
                }
            }
            CleanupPlacement(ok);
            Update_GridView(JobData_GridView);
        }

        // =================================================================================
        // This routine places the [index] row from Job data grid view:
        private bool PlaceRow_m(int RowNo)
        {
            DisplayText("PlaceRow_m(" + RowNo.ToString() + ")", KnownColor.Blue);
            // Select the row and keep it visible
            JobData_GridView.Rows[RowNo].Selected = true;
            HandleGridScrolling(false, JobData_GridView);

            string[] Components;  // This array has the list of components to place:
            string NewID = "";
            bool RestoreRow = false;
            // RestoreRow: If we'll replace the jobdata row with new data at the end. 
            // There isn't any new data, unless method is "?".

            // If the Method is "?", ask the user what to do:
            DataGridViewRow tempRow = (DataGridViewRow)JobData_GridView.Rows[RowNo].Clone();
            if (JobData_GridView.Rows[RowNo].Cells["GroupMethod"].Value.ToString() == "?")
            {
                // We'll now get new data from the user. By default, we don't mess with the data we have
                // So, take a copy of current row
                RestoreRow = true;
                for (int i = 0; i < tempRow.Cells.Count; i++)
                {
                    tempRow.Cells[i].Value = JobData_GridView.Rows[RowNo].Cells[i].Value;
                }

                // Display Method selection form:
                bool UserHasNotDecided = false;
                string NewMethod;
                MethodSelectionForm MethodDialog = new MethodSelectionForm();
                do
                {
                    MethodDialog.ShowCheckBox = true;
                    MethodDialog.HeaderString = CurrentGroup_label.Text;
                    MethodDialog.ShowDialog(this);
                    if (Properties.Settings.Default.Placement_UpdateJobGridAtRuntime)
                    {
                        RestoreRow = false;
                    };
                    NewMethod = MethodDialog.SelectedMethod;
                    if ((NewMethod == "Place") || (NewMethod == "Place Fast"))
                    {
                        // show the tape selection dialog
                        NewID = SelectTape("Select tape for " + JobData_GridView.Rows[RowNo].Cells["ComponentType"].Value.ToString());
                        if (!Properties.Settings.Default.Placement_UpdateJobGridAtRuntime)
                        {
                            RestoreRow = true;   // In case user unselected it at the tape selection dialog
                        };

                        if (NewID == "none")
                        {
                            UserHasNotDecided = true; // User did select "Place", but didn't select TapeNumber, we'll ask again.
                        }
                        else if (NewID == "Ignore")
                        {
                            NewMethod = "Ignore";
                            NewID = "";
                        }
                        else if (NewID == "Abort")
                        {
                            NewMethod = "Ignore";
                            NewID = "";
                            AbortPlacement = true;
                            RestoreRow = true;		// something went astray, keep method at "?"
                        }
                    }

                } while (UserHasNotDecided);
                if (NewMethod == "")
                {
                    return false;   // user pressed x
                }
                // Put the values to JobData_GridView
                JobData_GridView.Rows[RowNo].Cells["GroupMethod"].Value = NewMethod;
                JobData_GridView.Rows[RowNo].Cells["MethodParamAllComponents"].Value = NewID;
                Update_GridView(JobData_GridView);
                MethodDialog.Dispose();
            }
            // Method is now selected, even if it was ?. If user quits the operation, PlaceComponent_m() notices.

            // The place operation does not necessarily have any components for it (such as a needle change).
            // Make sure there is valid data at ComponentList anyway.
            if (JobData_GridView.Rows[RowNo].Cells["ComponentList"].Value == null)
            {
                JobData_GridView.Rows[RowNo].Cells["ComponentList"].Value = "--";
            };
            if (JobData_GridView.Rows[RowNo].Cells["ComponentList"].Value.ToString() == "--")
            {
                Components = new string[] { "--" };
            }
            else
            {
                Components = JobData_GridView.Rows[RowNo].Cells["ComponentList"].Value.ToString().Split(',');
            };
            bool ReturnValue = true;

            // Prepare for placement
            bool FirstInRow = true;
            if (JobData_GridView.Rows[RowNo].Cells["GroupMethod"].Value.ToString() == "Place Fast")
            {
                string TapeID = JobData_GridView.Rows[RowNo].Cells["MethodParamAllComponents"].Value.ToString();
                int count;
                if (!int.TryParse(JobData_GridView.Rows[RowNo].Cells["ComponentCount"].Value.ToString(), out count))
                {
                    ShowMessageBox(
                        "Bad data at component count",
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                    return false;
                }
                if (!Tapes.PrepareForFastPlacement_m(TapeID, count))
                {
                    return false;
                }
                Tapes.FastParametersOk = true;
            }
            // Place parts:
            foreach (string Component in Components)
            {
                if (!PlaceComponent_m(Component, RowNo, FirstInRow))
                {
                    JobData_GridView.Rows[RowNo].Selected = false;
                    ReturnValue = false;
                    Tapes.FastParametersOk = false;
                    break;
                };
                FirstInRow = false;
            };
            Tapes.FastParametersOk = false;

            // restore the row if needed
            if (RestoreRow)
            {
                for (int i = 0; i < tempRow.Cells.Count; i++)
                {
                    JobData_GridView.Rows[RowNo].Cells[i].Value = tempRow.Cells[i].Value;
                }
                Update_GridView(JobData_GridView);
            };

            JobData_GridView.Rows[RowNo].Selected = false;
            return ReturnValue;
        }


        // =================================================================================
        // All rows:
        private void PlaceAll_button_Click(object sender, EventArgs e)
        {

            if (!PrepareToPlace_m())
            {
                ShowMessageBox(
                    "Placement operation failed, nothing done.",
                    "Placement failed",
                    MessageBoxButtons.OK
                 );
                return;
            }
            PreviousGroup_label.Text = "--";
            CurrentGroup_label.Text = "--";
            NextGroup_label.Text = JobData_GridView.Rows[0].Cells["ComponentType"].Value.ToString() + " (" +
                JobData_GridView.Rows[0].Cells["ComponentCount"].Value.ToString() + " pcs.)";

            for (int i = 0; i < JobData_GridView.RowCount; i++)
            {
                PreviousGroup_label.Text = CurrentGroup_label.Text;
                CurrentGroup_label.Text = NextGroup_label.Text;
                if (i < (JobData_GridView.RowCount - 1))
                {
                    NextGroup_label.Text = JobData_GridView.Rows[i + 1].Cells["ComponentType"].Value.ToString() + " (" +
                        JobData_GridView.Rows[i + 1].Cells["ComponentCount"].Value.ToString() + " pcs.)";
                }
                else
                {
                    NextGroup_label.Text = "--";
                };

                if (!PlaceRow_m(i))
                {
                    break;
                }
            }

            CleanupPlacement(true);
            ShowMessageBox(
                "All components placed.",
                "Done",
                MessageBoxButtons.OK);
        }


        // =================================================================================

        // =================================================================================
        // ComponentDataValidates_m():
        // Checks that CAD data for a component is good. (Used by PlaceComponent_m() )

        private bool ComponentDataValidates_m(string Component, int CADdataRow, int GroupRow)
        {
            // Component exist. Footprint, X, Y and rotation data should be there:
            if (JobData_GridView.Rows[GroupRow].Cells["ComponentType"].Value == null)
            {
                ShowMessageBox(
                        "Component " + Component + ": No Footprint",
                        "Missing Data",
                        MessageBoxButtons.OK);
                return false;
            }

            if (CadData_GridView.Rows[CADdataRow].Cells["X_nominal"].Value == null)
            {
                ShowMessageBox(
                        "Component " + Component + ": No X data",
                        "Missing Data",
                        MessageBoxButtons.OK);
                return false;
            }

            if (CadData_GridView.Rows[CADdataRow].Cells["Y_nominal"].Value == null)
            {
                ShowMessageBox(
                        "Component " + Component + ": No Y data",
                        "Missing Data",
                        MessageBoxButtons.OK);
                return false;
            }

            if (CadData_GridView.Rows[CADdataRow].Cells["Rotation"].Value == null)
            {
                ShowMessageBox(
                        "Component " + Component + ": No Rotation data",
                        "Missing Data",
                        MessageBoxButtons.OK);
                return false;
            }
            else
            {
                double Rotation;
                if (!double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Rotation"].Value.ToString(), out Rotation))
                {
                    ShowMessageBox(
                        "Component " + Component + ": Bad Rotation data",
                        "Conversion Error",
                        MessageBoxButtons.OK);
                    return false;
                }
            }

            // Machine coordinate data should be also there, properly formatted:
            double X;
            double Y;
            double A;
            if ((!double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["X_machine"].Value.ToString(), out X))
                ||
                (!double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Y_machine"].Value.ToString(), out Y)))
            {
                ShowMessageBox(
                    "Component " + Component + ", bad machine coordinate",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }
            if (!double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Rotation_machine"].Value.ToString(), out A))
            {
                ShowMessageBox(
                    "Bad data at Rotation, machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            };

            // Even if component is not specified, Method data should be there:
            if (JobData_GridView.Rows[GroupRow].Cells["GroupMethod"].Value == null)
            {
                ShowMessageBox(
                        "Component " + Component + ", No method data",
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                return false;
            }
            return true;
        } // end of ComponentDataValidates_m()


        // =================================================================================
        // PlaceComponent_m()
        // This routine does the actual placement of a single component.
        // Component is the component name (Such as R15); based to this, we'll find the coordinates from CAD data
        // GroupRow is the row index to Job data grid view.
        // =================================================================================

        private bool PlaceComponent_m(string Component, int GroupRow, bool FirstInRow)
        {
            DisplayText("PlaceComponent_m: Component: " + Component + ", Row:" + GroupRow.ToString(), KnownColor.Blue);
            // Skip fiducials
            if (JobData_GridView.Rows[GroupRow].Cells["GroupMethod"].Value.ToString() == "Fiducials")
            {
                DisplayText("PlaceComponent_m(): Skip fiducials");
                return true;
            }
            // If component is specified, find its row in CAD data:
            int CADdataRow = -1;
            if (Component != "--")
            {
                foreach (DataGridViewRow Row in CadData_GridView.Rows)
                {
                    if (Row.Cells["Component"].Value.ToString() == Component)
                    {
                        CADdataRow = Row.Index;
                        break;
                    }
                }
                if (CADdataRow < 0)
                {
                    ShowMessageBox(
                        "Component " + Component + " data not found.",
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                    return false;
                }
            }

            // Validate data:
            string Footprint = "n/a";
            string Xstr = "n/a";
            string Ystr = "n/a";
            string RotationStr = "n/a";
            double Rotation = Double.NaN;
            double X_machine = 0;
            double Y_machine = 0;
            double A_machine = 0;
            string Method = "";
            string MethodParameter = "";

            if (Component != "--") // if component exists:
            {
                // check data consistency
                if (!ComponentDataValidates_m(Component, CADdataRow, GroupRow))
                {
                    return false;
                }
                // and fill values:
                Footprint = JobData_GridView.Rows[GroupRow].Cells["ComponentType"].Value.ToString();
                Xstr = CadData_GridView.Rows[CADdataRow].Cells["X_nominal"].Value.ToString();
                Ystr = CadData_GridView.Rows[CADdataRow].Cells["Y_nominal"].Value.ToString();
                RotationStr = CadData_GridView.Rows[CADdataRow].Cells["Rotation"].Value.ToString();
                double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Rotation"].Value.ToString(), out Rotation);
                double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["X_machine"].Value.ToString(), out X_machine);
                double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Y_machine"].Value.ToString(), out Y_machine);
                double.TryParse(CadData_GridView.Rows[CADdataRow].Cells["Rotation_machine"].Value.ToString(), out A_machine);
            }

            Method = JobData_GridView.Rows[GroupRow].Cells["GroupMethod"].Value.ToString();
            // Even if component is not specified, Method data should be there:
            // it is not an error if method does not have parameters.
            if (JobData_GridView.Rows[GroupRow].Cells["MethodParamAllComponents"].Value != null)
            {
                MethodParameter = JobData_GridView.Rows[GroupRow].Cells["MethodParamAllComponents"].Value.ToString();
            };

            // Data is now validated, all variables have values that check out. Place the component.
            // Update "Now placing" labels:
            if ((Method == "LoosePart") || (Method == "Place") || (Method == "Place Fast"))
            {
                PlacedComponent_label.Text = Component;
                PlacedComponent_label.Update();
                PlacedValue_label.Text = Footprint;
                PlacedValue_label.Update();
                PlacedX_label.Text = Xstr;
                PlacedX_label.Update();
                PlacedY_label.Text = Ystr;
                PlacedY_label.Update();
                PlacedRotation_label.Text = RotationStr;
                PlacedRotation_label.Update();
                MachineCoords_label.Text = "( " +
                    CadData_GridView.Rows[CADdataRow].Cells["X_machine"].Value.ToString() + ", " +
                    CadData_GridView.Rows[CADdataRow].Cells["Y_machine"].Value.ToString() + " )";
                MachineCoords_label.Update();
            };

            if (AbortPlacement)
            {
                AbortPlacement = false;  // one shot
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            }

            // Component is at CadData_GridView.Rows[CADdataRow]. 
            // What to do to it is at  JobData_GridView.Rows[GroupRow].

            int time;
            switch (Method)
            {
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

                case "DownCam Snapshot":
                case "UpCam Snapshot":
                case "LoosePart":
                case "Place Fast":
                case "Place":
                    if (Component == "--")
                    {
                        ShowMessageBox(
                            "Attempt to \"place\" non-existing component(\"--\")",
                            "Data error",
                            MessageBoxButtons.OK);
                        return false;
                    }
                    if (!PlacePart_m(CADdataRow, GroupRow, X_machine, Y_machine, A_machine, FirstInRow))
                        return false;
                    break;

                case "Change needle":
                    if (!ChangeNeedle_m())
                        return false;
                    break;

                case "Recalibrate":
                    ValidMeasurement_checkBox.Checked = false;
                    if (!PrepareToPlace_m())
                        return false;
                    break;

                case "Ignore":
                    // Optionally, method parameter specifies pause time (used in debugging, I can't think of a real-life use case for that)
                    if (int.TryParse(JobData_GridView.Rows[GroupRow].Cells["MethodParamAllComponents"].Value.ToString(), out time))
                    {
                        for (int z = 0; z < time / 5; z++)
                        {
                            Application.DoEvents();  // keeping video running
                            Thread.Sleep(5);
                        }
                    };
                    return true;  // To next row...
                // break;

                case "Fiducials":
                    return true;  // To next row...
                // break; 


                // Used this for debugging, but is unreachable now
                case "Locate":
                    // Shows the component and its footprint(if known)...
                    CNC_XY_m(X_machine, Y_machine);
                    if (!ShowFootPrint_m(CADdataRow))
                        return false;

                    // ... either for the time specified in method parameter
                    if (int.TryParse(JobData_GridView.Rows[GroupRow].Cells["MethodParamAllComponents"].Value.ToString(), out time))
                    {
                        for (int z = 0; z < time / 5; z++)
                        {
                            Application.DoEvents();  // keeping video running
                            Thread.Sleep(5);
                        }
                    }
                    else
                    {
                        // If no time specified, show a dialog:
                        ShowMessageBox(
                            "This is " + Component,
                            "Locate Component",
                            MessageBoxButtons.OK);
                    }
                    DownCamera.DrawBox = false;
                    break;

                default:
                    ShowMessageBox(
                        "No code for method " + JobData_GridView.Rows[GroupRow].Cells["GroupMethod"].Value.ToString(),
                        "Lazy programmer error",
                        MessageBoxButtons.OK);
                    return false;
                // break;
            }
            return true;
        }

        public bool AbortPlacement = false;


        // =================================================================================
        private bool ChangeNeedle_m()
        {
            CNC_Write_m("{\"zsn\":0}");
            CNC_Write_m("{\"zsx\":0}");
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
            CNC_Write_m("{\"zsn\":3}");
            CNC_Write_m("{\"zsx\":2}");
            if (!MechanicalHoming_m())
            {
                return false;
            }
            if (!OpticalHoming_m())
            {
                return false;
            }
            if (!CalibrateNeedle_m())
            {
                return false;
            }
            if (!BuildMachineCoordinateData_m())
            {
                return false;
            }
            PumpOn();
            return true;
        }

        // =================================================================================
        // This is called before any placement is done:
        // =================================================================================
        private bool PrepareToPlace_m()
        {
            if (JobData_GridView.RowCount < 1)
            {
                ShowMessageBox(
                    "No Job loaded.",
                    "No Job",
                    MessageBoxButtons.OK
                );
                return false;
            }

            // Tapes.ClearAll();

            if (!Needle.Calibrated || !ValidMeasurement_checkBox.Checked)
            {
                CurrentGroup_label.Text = "Calibrating needle";
                if (!CalibrateNeedle_m())
                {
                    CurrentGroup_label.Text = "--";
                    return false;
                }
            }
            if(!ValidMeasurement_checkBox.Checked)
            {
                CurrentGroup_label.Text = "Measuring PCB";
                if (!BuildMachineCoordinateData_m())
                {
                    CurrentGroup_label.Text = "--";
                    return false;
                }
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
        private void CleanupPlacement(bool success)
        {
            PlacedComponent_label.Text = "--";
            PlacedValue_label.Text = "--";
            PlacedX_label.Text = "--";
            PlacedY_label.Text = "--";
            PlacedRotation_label.Text = "--";
            MachineCoords_label.Text = "( -- )";
            PreviousGroup_label.Text = "--";
            CurrentGroup_label.Text = "--";
            NextGroup_label.Text = "--";
            JobData_GridView.ReadOnly = false;
            PumpDefaultSetting();
            if (success)
            {
                CNC_Park();  // if fail, it helps debugging if machine stays still
            }
            VacuumDefaultSetting();
        }


        // =================================================================================
        // PickUpThis_m(): Actual pickup, assumes needle is on top of the part
        private bool PickUpThis_m(int TapeNumber)
        {
            string Z_str = Tapes_dataGridView.Rows[TapeNumber].Cells["PickupZ_Column"].Value.ToString();
            if (Z_str == "--")
            {
                DisplayText("PickUpPart_m(): Probing pickup Z", KnownColor.Blue);
                if (!Needle_ProbeDown_m())
                {
                    return false;
                }
                double Zpickup = Cnc.CurrentZ - Properties.Settings.Default.General_ProbingBackOff + Properties.Settings.Default.Placement_Depth;
                Tapes_dataGridView.Rows[TapeNumber].Cells["PickupZ_Column"].Value = Zpickup.ToString();
                DisplayText("PickUpPart_m(): Probed Z= " + Cnc.CurrentZ.ToString());
            }
            else
            {
                double Z;
                if (!double.TryParse(Z_str, out Z))
                {
                    ShowMessageBox(
                        "Bad pickup Z data at Tape #" + TapeNumber.ToString(),
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                    return false;
                };
                // Z += 0.5;
                DisplayText("PickUpPart_m(): Part pickup, Z" + Z.ToString(), KnownColor.Blue);
                if (!CNC_Z_m(Z))
                {
                    return false;
                }
            }
            VacuumOn();
            DisplayText("PickUpPart_m(): needle up");
            if (!CNC_Z_m(0))
            {
                return false;
            }
            return true;
        }

        // ========================================================================================
        public bool PickUpPartFast_m(int TapeNum)
        {
            if (!Tapes.FastParametersOk)
            {
                ShowMessageBox(
                    "FastParameters not ok",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }

            // find the part location and go there:
            double PartX = 0.0;
            double PartY = 0.0;
            double A = 0.0;

            if (!Tapes.GetPartLocationFromHolePosition_m(TapeNum, Tapes.FastXpos, Tapes.FastYpos, out PartX, out PartY, out A))
            {
                ShowMessageBox(
                    "Can't find tape hole",
                    "Tape error",
                    MessageBoxButtons.OK
                );
            }

            // Now, PartX, PartY, A tell the position of the part. For test: get camera there:  instead of Needle.Move_m(PartX, PartY, A
            if (!Needle.Move_m(PartX, PartY, A))
            {
                return false;
            }
            if (!PickUpThis_m(TapeNum))
            {
                return false;
            }

            Tapes.IncrementTape_Fast(TapeNum);
            return true;
        }

        // =================================================================================
        // PickUpPartWithHoleMeasurement_m(): Picks next part from the tape, measuring the hole
        private bool PickUpPartWithHoleMeasurement_m(int TapeNumber)
        {
            // If this succeeds, we update next hole location at the end, but these values are measured eat start
            double HoleX = 0;
            double HoleY = 0;
            DisplayText("PickUpPart_m(), tape no: " + TapeNumber.ToString());
            // Go to part location:
            VacuumOff();
            if (!Tapes.GotoNextPartByMeasurement_m(TapeNumber, out HoleX, out HoleY))
            {
                return false;
            }
            // Pick it up:
            if (!PickUpThis_m(TapeNumber))
            {
                return false;
            }

            if (!Tapes.IncrementTape(TapeNumber, HoleX, HoleY))
            {
                return false;
            }

            return true;
        }

        // =================================================================================
        // PutPartDown_m(): Puts part down at this position. 
        // If placement Z isn't known already, updates the tape info.
        private bool PutPartDown_m(int TapeNum)
        {
            string Z_str = Tapes_dataGridView.Rows[TapeNum].Cells["PlaceZ_Column"].Value.ToString();
            if (Z_str == "--")
            {
                DisplayText("PutPartDown_m(): Probing placement Z", KnownColor.Blue);
                if (!Needle_ProbeDown_m())
                {
                    return false;
                };
                double Zplace = Cnc.CurrentZ - Properties.Settings.Default.General_ProbingBackOff + Properties.Settings.Default.Placement_Depth;
                Tapes_dataGridView.Rows[TapeNum].Cells["PlaceZ_Column"].Value = Zplace.ToString();
                DisplayText("PutPartDown_m(): Probed placement Z= " + Cnc.CurrentZ.ToString());
            }
            else
            {
                double Z;
                if (!double.TryParse(Z_str, out Z))
                {
                    ShowMessageBox(
                        "Bad place Z data at Tape #" + TapeNum,
                        "Sloppy programmer error",
                        MessageBoxButtons.OK);
                    return false;
                };
                DisplayText("PlacePart_m(): Part down, Z" + Z.ToString(), KnownColor.Blue);
                if (!CNC_Z_m(Z))
                {
                    return false;
                }
            }
            //ShowMessageBox(
            //    "Debug: Needle down at component." + Component,
            //    "Debug",
            //    MessageBoxButtons.OK);
            DisplayText("PlacePart_m(): Needle up.");
            VacuumOff();
            if (!CNC_Z_m(0))  // back up
            {
                return false;
            }
            //ShowMessageBox(
            //    "Debug: Needle up.",
            //    "Debug",
            //    MessageBoxButtons.OK);
            return true;
        }

        // =================================================================================
        // 
        private bool PutLoosePartDown_m(bool Probe)
        {
            if (Probe)
            {
                DisplayText("PutLoosePartDown_m(): Probing placement Z");
                if (!Needle_ProbeDown_m())
                {
                    return false;
                }
                LoosePartPlaceZ = Cnc.CurrentZ - Properties.Settings.Default.General_ProbingBackOff + Properties.Settings.Default.Placement_Depth;
                DisplayText("PutLoosePartDown_m(): probed Z= " + Cnc.CurrentZ.ToString());
                DisplayText("PutLoosePartDown_m(): placement Z= " + LoosePartPlaceZ.ToString());
            }
            else
            {
                if (!CNC_Z_m(LoosePartPlaceZ))
                {
                    return false;
                }
            }
            DisplayText("PutLoosePartDown_m(): Needle up.");
            VacuumOff();
            if (!CNC_Z_m(0))  // back up
            {
                return false;
            }
            return true;
        }
        // =================================================================================
        // Actual placement 
        // =================================================================================
        private double LoosePartPickupZ = 0.0;
        private double LoosePartPlaceZ = 0.0;

        private double ReduceRotation(double rot)
        {
            // takes a rotation value, rotates it to +- 45degs.
            while (rot > 45.01)
            {
                rot = rot - 90;
            };
            while (rot < -45.01)
            {
                rot = rot + 90;
            }
            return rot;
        }

        private int MeasureClosestComponentInPx(out double X, out double Y, out double A, Camera Cam, double Tolerance, int averages)
        {
            X = 0;
            double Xsum = 0;
            Y = 0;
            double Ysum = 0;
            A = 0.0;
            double Asum = 0.0;
            int count = 0;
            for (int i = 0; i < averages; i++)
            {
                if (Cam.GetClosestComponent(out X, out Y, out A, Tolerance) > 0)
                {
                    //DisplayText("GetClosestComponent, X: " + X.ToString() + ", Y: " + Y.ToString() + ", A: " + A.ToString()+
                    //    ", red: "+ReduceRotation(A).ToString());
                    count++;
                    Xsum += X;
                    Ysum += Y;
                    Asum += ReduceRotation(A);
                }
            }
            if (count == 0)
            {
                return 0;
            }
            X = Xsum / (float)count;
            Y = Ysum / (float)count;
            A = -Asum / (float)count;
            return count;
        }

        private bool PickUpLoosePart_m(bool Probe, bool Snapshot, int CADdataRow, string Component)
        {
            DisplayText("PickUpLoosePart_m: " + Probe.ToString() + ", " + Snapshot.ToString() + ", " + CADdataRow.ToString() + ", " + Component, KnownColor.Blue);
            if (!CNC_XY_m(Properties.Settings.Default.General_PickupCenterX, Properties.Settings.Default.General_PickupCenterY))
            {
                return false;
            }

            // ask for it 
            string ComponentType = CadData_GridView.Rows[CADdataRow].Cells["Value_Footprint"].Value.ToString();
            DialogResult dialogResult = ShowMessageBox(
                "Put one " + ComponentType + " to the pickup location.",
                "Placing " + Component,
                MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.Cancel)
            {
                return false;
            }

            // Find component
            double X = 0;
            double Y = 0;
            double A = 0.0;
            SetComponentsMeasurement();
            // If we don't get a look from straight up (more than 2mm off) we need to re-measure
            for (int i = 0; i < 2; i++)
            {
                // measure 5 averages, component must be 8.0mm from its place
                int count = MeasureClosestComponentInPx(out X, out Y, out A, DownCamera, (8.0 / Properties.Settings.Default.DownCam_XmmPerPixel), 5);
                if (count == 0)
                {
                    ShowMessageBox(
                        "Could not see component",
                        "No component",
                        MessageBoxButtons.OK);
                    return false;
                }
                X = X * Properties.Settings.Default.DownCam_XmmPerPixel;
                Y = -Y * Properties.Settings.Default.DownCam_YmmPerPixel;
                DisplayText("PickUpLoosePart_m(): measurement " + i.ToString() + ", X: " + X.ToString() + ", Y: " + Y.ToString() + ", A: " + A.ToString());
                if ((Math.Abs(X) < 2.0) && (Math.Abs(Y) < 2.0))
                {
                    break;
                }
                else
                {
                    if (!CNC_XY_m(Cnc.CurrentX + X, Cnc.CurrentY + Y))
                    {
                        return false;
                    }
                }
            }
            // go exactly on top of component for user confidence and for snapshot to be at right place
            if (Snapshot)
            {
                if (!CNC_XY_m(Cnc.CurrentX + X, Cnc.CurrentY + Y))
                {
                    return false;
                }
                DownCamera.SnapshotRotation = A;
                DownCamera.BuildMeasurementFunctionsList(DowncamSnapshot_dataGridView);
                DownCamera.TakeSnapshot();
                DownCamera.Draw_Snapshot = true;
                X = 0.0;
                Y = 0.0;
            };

            if (!Needle.Move_m(Cnc.CurrentX + X, Cnc.CurrentY + Y, A))
            {
                return false;
            }
            // pick it up
            if (Probe)
            {
                DisplayText("PickUpLoosePart_m(): Probing pickup Z");
                if (!Needle_ProbeDown_m())
                {
                    DownCamera.Draw_Snapshot = true;
                    return false;
                }
                LoosePartPickupZ = Cnc.CurrentZ - Properties.Settings.Default.General_ProbingBackOff + Properties.Settings.Default.Placement_Depth;
                DisplayText("PickUpLoosePart_m(): Probed Z= " + Cnc.CurrentZ.ToString());
                DisplayText("PickUpLoosePart_m(): Pickup Z= " + LoosePartPickupZ.ToString());
            }
            else
            {
                DisplayText("PickUpLoosePart_m(): Part pickup, Z" + LoosePartPickupZ.ToString());
                if (!CNC_Z_m(LoosePartPickupZ))
                {
                    DownCamera.Draw_Snapshot = true;
                    return false;
                }
            }
            VacuumOn();
            DisplayText("PickUpLoosePart_m(): needle up");
            if (!CNC_Z_m(0))
            {
                DownCamera.Draw_Snapshot = true;
                return false;
            }
            if (AbortPlacement)
            {
                AbortPlacement = false;
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        // ========================================================================================
        // PlacePart_m(): This routine places a single component.
        // Component is at CadData_GridView.Rows[CADdataRow]. 
        // Tape ID and method are at JobDataRow.Rows[JobDataRow]. 
        // It should go to X, Y, A
        // Data is validated already.

        private bool PlacePart_m(int CADdataRow, int JobDataRow, double X, double Y, double A, bool FirstInRow)
        {
            if (AbortPlacement)
            {
                AbortPlacement = false;
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                    MessageBoxButtons.OK);
                return false;
            };
            string id = JobData_GridView.Rows[JobDataRow].Cells["MethodParamAllComponents"].Value.ToString();
            string Method = JobData_GridView.Rows[JobDataRow].Cells["GroupMethod"].Value.ToString();

            int TapeNum = 0;
            string Component = CadData_GridView.Rows[CADdataRow].Cells["Component"].Value.ToString();
            DisplayText("PlacePart_m, Component: " + Component + ", CAD data row: " + CADdataRow.ToString(), KnownColor.Blue);

            // Preparing:
            switch (Method)
            {
                case "Place":
                case "Place Fast":
                    if (!Tapes.IdValidates_m(id, out TapeNum))
                    {
                        return false;
                    }
                    // First component in a row: We don't necessarily know the correct pickup and placement height, even though there
                    // might be values from previous runs. (PCB and needle might have changed.)
                    if (FirstInRow && MeasureZs_checkBox.Checked)
                    {
                        // Clear heights
                        Tapes_dataGridView.Rows[TapeNum].Cells["PickupZ_Column"].Value = "--";
                        Tapes_dataGridView.Rows[TapeNum].Cells["PlaceZ_Column"].Value = "--";
                    };
                    break;

                default:
                    // Other methods don't use tapes, nothing to do here.
                    break;
            };

            // Pickup:
            switch (Method)
            {
                case "Place":
                    if (!PickUpPartWithHoleMeasurement_m(TapeNum))
                    {
                        return false;
                    }
                    break;

                case "Place Fast":
                    if (!PickUpPartFast_m(TapeNum))
                    {
                        return false;
                    }
                    break;

                case "LoosePart":
                    if (!PickUpLoosePart_m(FirstInRow, false, CADdataRow, Component))
                    {
                        return false;
                    }
                    break;
                case "DownCam Snapshot":
                case "UpCam Snapshot":
                    if (!PickUpLoosePart_m(FirstInRow, true, CADdataRow, Component))
                    {
                        return false;
                    }
                    break;

                default:
                    ShowMessageBox(
                        "Unknown method at placement time: ",
                        "Operation aborted.",
                        MessageBoxButtons.OK);
                    return false;
                // break;
            };

            // Take the part to position. With snapshot, we want to fine tune it here:
            if (Method == "UpCam Snapshot")
            {
                DisplayText("PlacePart_m: take Upcam snapshot");
                // Take part to upcam
                if (!CNC_XYA_m(Properties.Settings.Default.UpCam_PositionX, Properties.Settings.Default.UpCam_PositionY, 0.0))
                {
                    return false;
                };
                if (!CNC_Z_m(LoosePartPickupZ))
                {
                    return false;
                };
                // take snapshot
                SelectCamera(UpCamera);
                UpCam_TakeSnapshot();
                SelectCamera(DownCamera);
                if (!CNC_Z_m(0.0))
                {
                    DownCamera.Draw_Snapshot = false;
                    return false;
                };
            }

            if ((Method == "DownCam Snapshot") || (Method == "UpCam Snapshot"))
            {
                DisplayText("PlacePart_m: Snapshot place");
                // Take cam to where we think part is going. This might not be exactly right.
                DownCamera.RotateSnapshot(A);
                if (!CNC_XYA_m(X, Y, A))
                {
                    // VacuumOff();  if the above failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                    DownCamera.Draw_Snapshot = false;
                    return false;
                };
                // Wait for enter key press. Before enter, user jogs the part and the image to right place
                EnterKeyHit = false;
                do
                {
                    Application.DoEvents();
                    Thread.Sleep(10);
                    if (AbortPlacement)
                    {
                        AbortPlacement = false;
                        ShowMessageBox(
                            "Operation aborted.",
                            "Operation aborted.",
                            MessageBoxButtons.OK);
                        return false;
                    }
                } while (!EnterKeyHit);
                DownCamera.Draw_Snapshot = false;
                X = Cnc.CurrentX;
                Y = Cnc.CurrentY;
                A = Cnc.CurrentA;
            };

            // Take the part to position:
            DisplayText("PlacePart_m: goto placement position");
            if (!Needle.Move_m(X, Y, A))
            {
                // VacuumOff();  if the above failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                DownCamera.Draw_Snapshot = false;
                UpCamera.Draw_Snapshot = false;
                return false;
            }

            // Place it:
            if (AbortPlacement)
            {
                AbortPlacement = false;
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                MessageBoxButtons.OK);
                DownCamera.Draw_Snapshot = false;
                UpCamera.Draw_Snapshot = false;
                return false;
            }

            switch (Method)
            {
                case "LoosePart":
                case "DownCam Snapshot":
                case "UpCam Snapshot":
                    DownCamera.Draw_Snapshot = false;
                    UpCamera.Draw_Snapshot = false;
                    if (!PutLoosePartDown_m(FirstInRow))
                    {
                        // VacuumOff();  if this failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                        return false;
                    }
                    break;

                default:
                    if (!PutPartDown_m(TapeNum))
                    {
                        // VacuumOff();  if this failed CNC seems to be down; low chances that VacuumOff() would go thru either. 
                        DownCamera.Draw_Snapshot = false;
                        UpCamera.Draw_Snapshot = false;
                        return false;
                    }
                    break;
            };
            if (AbortPlacement)
            {
                AbortPlacement = false;
                ShowMessageBox(
                    "Operation aborted.",
                    "Operation aborted.",
                MessageBoxButtons.OK);
                DownCamera.Draw_Snapshot = false;
                UpCamera.Draw_Snapshot = false;
                return false;
            }
            return true;
        }


        // =================================================================================
        // GetCorrentionForPartAtNeedle():
        // takes a look from Upcam, sets the correction values for the part at needle
        private bool GetCorrentionForPartAtNeedle(out double dX, out double dY, out double dA)
        {
            SelectCamera(UpCamera);
            dX = 0;
            dY = 0;
            dA = 0;

            if (!UpCamera.IsRunning())
            {
                SelectCamera(DownCamera);
                return false;
            }
            SetUpCamComponentsMeasurement();
            bool GoOn = false;
            bool result = false;
            while (!GoOn)
            {
                if (MeasureUpCamComponent(3.0, out dX, out dY, out dA))
                {
                    result = true;
                    GoOn = true;
                }
                else
                {
                    DialogResult dialogResult = ShowMessageBox(
                        "Did not get correction values from camera.\n Abort job / Retry / Place anyway?",
                        "Did not see component",
                        MessageBoxButtons.AbortRetryIgnore
                    );
                    if (dialogResult == DialogResult.Abort)
                    {
                        AbortPlacement = true;
                        result = false;
                        GoOn = true;
                    }
                    else if (dialogResult == DialogResult.Retry)
                    {
                        GoOn = false;
                    }
                    else
                    {
                        // ignore
                        result = true;
                        GoOn = true;
                    }
                }
            };
            SelectCamera(DownCamera);
            return result;
        }

        private bool MeasureUpCamComponent(double Tolerance, out double dX, out double dY, out double dA)
        {
            double X = 0;
            double Xsum = 0;
            double Y = 0;
            double Ysum = 0;
            int count = 0;
            dX = 0;
            dY = 0;
            dA = 0;
            for (int i = 0; i < 5; i++)
            {
                if (UpCamera.GetClosestComponent(out X, out Y, out dA, Tolerance * Properties.Settings.Default.UpCam_XmmPerPixel) > 0)
                {
                    count++;
                    Xsum += X;
                    Ysum += Y;
                }
            };
            if (count == 0)
            {
                return false;
            }
            X = Xsum / Properties.Settings.Default.UpCam_XmmPerPixel;
            dX = X / (float)count;
            Y = -Y / Properties.Settings.Default.UpCam_XmmPerPixel;
            dY = Y / (float)count;
            DisplayText("Component measurement:");
            DisplayText("X: " + X.ToString("0.000", CultureInfo.InvariantCulture) + " (" + count.ToString() + " results out of 5)");
            DisplayText("Y: " + Y.ToString("0.000", CultureInfo.InvariantCulture));
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

        public bool FindFiducials_m(out int FiducialsRow)
        {
            // I) Find fiducials nominal placement data
            // Ia) Are fiducials indicated and only once?
            bool FiducialsFound = false;
            FiducialsRow = 0;

            for (int i = 0; i < JobData_GridView.RowCount; i++)
            {
                if (JobData_GridView.Rows[i].Cells["GroupMethod"].Value.ToString() == "Fiducials")
                {
                    if (FiducialsFound)
                    {
                        ShowMessageBox(
                            "Fiducials selected twice in Methods. Please fix!",
                            "Double Fiducials",
                            MessageBoxButtons.OK);
                        return false;
                    }
                    else
                    {
                        FiducialsFound = true;
                        FiducialsRow = i;
                    }
                }
            }
            if (!FiducialsFound)
            {
                // Ib) OriginalFiducials not pointed out yet. Find them automatically.
                // OriginalFiducials designators are FI*** or FID*** where *** is a number.
                string Fids;
                bool FidsOnThisRow = false;
                for (int i = 0; i < JobData_GridView.RowCount; i++)
                {
                    Fids = JobData_GridView.Rows[i].Cells["ComponentList"].Value.ToString();
                    if (Fids.StartsWith("FI", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (Char.IsDigit(Fids[2]))
                        {
                            FidsOnThisRow = true;
                        }
                        else if (Fids.StartsWith("FID", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Char.IsDigit(Fids[3]))
                            {
                                FidsOnThisRow = true;
                            }
                        }
                    }
                    if (FidsOnThisRow)
                    {
                        if (FiducialsFound)
                        {
                            ShowMessageBox(
                                "Two group of components starting with FI*. Please indicate Fiducials",
                                "Double Fiducials",
                                MessageBoxButtons.OK);
                            return false;
                        }
                        else
                        {
                            FiducialsFound = true;
                            FiducialsRow = i;
                            FidsOnThisRow = false;
                        }
                    }
                }
                // If fids were found, the row is unique. Mark it:
                if (FiducialsFound)
                {
                    JobData_GridView.Rows[FiducialsRow].Cells["GroupMethod"].Value = "Fiducials";
                }
            }
            if (!FiducialsFound)
            {
                ShowMessageBox(
                    "Fiducials definitions not found.",
                    "Fiducials not defined",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        // =================================================================================
        // MeasureFiducial_m():
        // Takes the parameter nominal location and measures its physical location.
        // Assumes measurement parameters already set.

        private bool MeasureFiducial_m(ref PhysicalComponent fid)
        {
            CNC_XY_m(fid.X_nominal + JobOffsetX + Properties.Settings.Default.General_JigOffsetX,
                     fid.Y_nominal + JobOffsetY + Properties.Settings.Default.General_JigOffsetY);
            // If more than 3mm off here, not good.
            double X;
            double Y;
            if (!GoToCircleLocation_m(3, 0.1, out X, out Y))
            {
                ShowMessageBox(
                    "Finding fiducial: Can't regognize fiducial " + fid.Designator,
                    "No Circle found",
                    MessageBoxButtons.OK);
                return false;
            }
            fid.X_machine = Cnc.CurrentX + X;
            fid.Y_machine = Cnc.CurrentY + Y;
            // For user confidence, show it:
            for (int i = 0; i < 50; i++)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            return true;
        }

        // =================================================================================
        // ValidateCADdata_m(): Checks, that supplied nominal values are good numbers:
        private bool ValidateCADdata_m()
        {
            double x;
            double y;
            double r;
            foreach (DataGridViewRow Row in CadData_GridView.Rows)
            {
                if (!double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out x))
                {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " X coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                if (!double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out y))
                {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " Y coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                if (!double.TryParse(Row.Cells["Rotation"].Value.ToString(), out r))
                {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " rotation data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                // DisplayText(Row.Cells["Component"].Value.ToString() + ": x= " + x.ToString() + ", y= " + y.ToString() + ", r= " + r.ToString());
            }
            return true;
        }


        // =================================================================================
        // BuildMachineCoordinateData_m():
        private bool BuildMachineCoordinateData_m()
        {
            double X_nom = 0;
            double Y_nom = 0;
            if (Properties.Settings.Default.Placement_SkipMeasurements)
            {
                foreach (DataGridViewRow Row in CadData_GridView.Rows)
                {
                    // Cad data is validated.
                    double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out X_nom);
                    double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out Y_nom);
                    X_nom += Properties.Settings.Default.General_JigOffsetX;
                    Y_nom += Properties.Settings.Default.General_JigOffsetY;
                    Row.Cells["X_machine"].Value = X_nom.ToString("0.000", CultureInfo.InvariantCulture);
                    Row.Cells["Y_machine"].Value = Y_nom.ToString("0.000", CultureInfo.InvariantCulture);

                    Row.Cells["Rotation_machine"].Value = Row.Cells["Rotation"].Value;
                }
                // Refresh UI:
                Update_GridView(CadData_GridView);
                return true;
            };

            if (ValidMeasurement_checkBox.Checked)
            {
                return true;
            }
            // Check, that our data is good:
            if (!ValidateCADdata_m())
                return false;

            // Find the fiducials form CAD data:
            int FiducialsRow = 0;
            if (!FindFiducials_m(out FiducialsRow))
            {
                return false;
            }
            // OriginalFiducials are at JobData_GridView.Rows[FiducialsRow]
            string[] FiducialDesignators = JobData_GridView.Rows[FiducialsRow].Cells["ComponentList"].Value.ToString().Split(',');
            // Are there at least two?
            if (FiducialDesignators.Length < 2)
            {
                ShowMessageBox(
                    "Only one fiducial found.",
                    "Too few fiducials",
                    MessageBoxButtons.OK);
                return false;
            }
            // Get ready for position measurements
            DisplayText("SetFiducialsMeasurement");
            SetFiducialsMeasurement();
            // Move them to our array, checking the data:
            PhysicalComponent[] Fiducials = new PhysicalComponent[FiducialDesignators.Length];  // store the data here
            // double X_nom = 0;
            // double Y_nom = 0;
            for (int i = 0; i < FiducialDesignators.Length; i++)  // for each fiducial in our OriginalFiducials array,
            {
                Fiducials[i] = new PhysicalComponent();
                Fiducials[i].Designator = FiducialDesignators[i];
                // find the fiducial in CAD data.
                foreach (DataGridViewRow Row in CadData_GridView.Rows)
                {
                    if (Row.Cells["Component"].Value.ToString() == FiducialDesignators[i]) // If this is the fiducial we want,
                    {
                        // Get its nominal position (value already checked).
                        double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out X_nom);
                        double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out Y_nom);
                        break;
                    }
                }
                Fiducials[i].X_nominal = X_nom;
                Fiducials[i].Y_nominal = Y_nom;
                // And measure it's true location:
                if (!MeasureFiducial_m(ref Fiducials[i]))
                {
                    return false;
                }
                // We could put the machine data in place at this point. However, 
                // we don't, as if the algorithms below are correct, the data will not change more than measurement error.
                // During development, that is a good checkpoint.
            }

            // Find the homographic tranformation from CAD data (fiducials.nominal) to measured machine coordinates
            // (fiducials.machine):
            Transform transform = new Transform();
            HomographyEstimation.Point[] nominals = new HomographyEstimation.Point[Fiducials.Length];
            HomographyEstimation.Point[] measured = new HomographyEstimation.Point[Fiducials.Length];
            // build point data arrays:
            for (int i = 0; i < Fiducials.Length; i++)
            {
                nominals[i].X = Fiducials[i].X_nominal;
                nominals[i].Y = Fiducials[i].Y_nominal;
                nominals[i].W = 1.0;
                measured[i].X = Fiducials[i].X_machine;
                measured[i].Y = Fiducials[i].Y_machine;
                measured[i].W = 1.0;
            }
            // find the tranformation
            bool res = transform.Estimate(nominals, measured, ErrorMetric.Transfer, 450, 450);  // the PCBs are smaller than 450mm
            if (!res)
            {
                ShowMessageBox(
                    "Transform estimation failed.",
                    "Data error",
                    MessageBoxButtons.OK);
                return false;
            }
            // Analyze the transform: Displacement is for debug. We could also calculate X & Y stretch and shear, but why bother.
            // Find out the displacement in the transform (where nominal origin ends up):
            HomographyEstimation.Point Loc, Loc2;
            Loc.X = 0.0;
            Loc.Y = 0.0;
            Loc.W = 1.0;
            Loc = transform.TransformPoint(Loc);
            Loc = Loc.NormalizeHomogeneous();
            DisplayText("Transform:");
            DisplayText("dX= " + (Loc.X).ToString());
            DisplayText("dY= " + Loc.Y.ToString());
            // We do need rotation. Find out by rotatíng a unit vector:
            Loc2.X = 1.0;
            Loc2.Y = 0.0;
            Loc2.W = 1.0;
            Loc2 = transform.TransformPoint(Loc2);
            Loc2 = Loc2.NormalizeHomogeneous();
            DisplayText("dX= " + Loc2.X.ToString());
            DisplayText("dY= " + Loc2.Y.ToString());
            double angle = Math.Asin(Loc2.Y - Loc.Y) * 180.0 / Math.PI; // in degrees
            DisplayText("angle= " + angle.ToString());

            // Calculate machine coordinates of all components:
            foreach (DataGridViewRow Row in CadData_GridView.Rows)
            {
                // build a point from CAD data values
                double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out Loc.X);
                double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out Loc.Y);
                Loc.W = 1;
                // transform it
                Loc = transform.TransformPoint(Loc);
                Loc = Loc.NormalizeHomogeneous();
                // store calculated location values
                Row.Cells["X_machine"].Value = Loc.X.ToString("0.000", CultureInfo.InvariantCulture);
                Row.Cells["Y_machine"].Value = Loc.Y.ToString("0.000", CultureInfo.InvariantCulture);
                // handle rotation
                double rot;
                double.TryParse(Row.Cells["Rotation"].Value.ToString(), out rot);
                rot += angle;
                while (rot > 360.0)
                {
                    rot -= 360.0;
                }
                while (rot < 0.0)
                {
                    rot += 360.0;
                }
                Row.Cells["Rotation_machine"].Value = rot.ToString("0.0000", CultureInfo.InvariantCulture);

            }
            // Refresh UI:
            Update_GridView(CadData_GridView);

            // For debug, compare fiducials true measured locations and the locations.
            // Also, if a fiducials moves more than 0.5mm, something is off (maybe there
            // was a via too close to a fid, and we picked that for a measurement). Warn the user!
            bool DataOk = true;
            double dx, dy;
            for (int i = 0; i < Fiducials.Length; i++)
            {
                Loc.X = Fiducials[i].X_nominal;
                Loc.Y = Fiducials[i].Y_nominal;
                Loc.W = 1.0;
                Loc = transform.TransformPoint(Loc);
                Loc = Loc.NormalizeHomogeneous();
                dx = Math.Abs(Loc.X - Fiducials[i].X_machine);
                dy = Math.Abs(Loc.Y - Fiducials[i].Y_machine);
                DisplayText(Fiducials[i].Designator +
                    ": x_meas= " + Fiducials[i].X_machine.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", x_calc= " + Loc.X.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", dx= " + dx.ToString("0.000", CultureInfo.InvariantCulture) +
                    ": y_meas= " + Fiducials[i].Y_machine.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", y_calc= " + Loc.Y.ToString("0.000", CultureInfo.InvariantCulture) +
                    ", dy= " + dy.ToString("0.000", CultureInfo.InvariantCulture));
                if ((Math.Abs(dx) > 0.4) || (Math.Abs(dy) > 0.4))
                {
                    DataOk = false;
                }
            };
            if (!DataOk)
            {
                DisplayText(" ** A fiducial moved more than 0.4mm from its measured location");
                DisplayText(" ** when applied the same calculations than regular componets.");
                DisplayText(" ** (Maybe the camera picked a via instead of a fiducial?)");
                DisplayText(" ** Placement data is likely not good.");
                DialogResult dialogResult = ShowMessageBox(
                    "Nominal to machine trasnformation seems to be off. (See log window)",
                    "Cancel operation?",
                    MessageBoxButtons.OKCancel
                );
                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
            }
            // Done! 
            ValidMeasurement_checkBox.Checked = true;
            return true;
        }// end BuildMachineCoordinateData_m

        // =================================================================================
        // BuildMachineCoordinateData_m functions end
        // =================================================================================


        // =================================================================================
        private void PausePlacement_button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = ShowMessageBox(
                "Placement Paused. Continue? (Cancel aborts)",
                "Placement Paused",
                MessageBoxButtons.OKCancel
            );
            if (dialogResult == DialogResult.Cancel)
            {
                AbortPlacement = true;
            }

        }

        private void ChangeNeedle_button_Click(object sender, EventArgs e)
        {
            ChangeNeedle_m();
        }


        // =================================================================================
        private void AbortPlacement_button_Click(object sender, EventArgs e)
        {
            AbortPlacement = true;
        }


        // =================================================================================
        private void JobOffsetX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JobOffsetX_textBox.Text, out val))
                {
                    JobOffsetX = val;
                }
            }
        }

        private void JobOffsetX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JobOffsetX_textBox.Text, out val))
            {
                JobOffsetX = val;
            }
        }

        // =================================================================================
        private void JobOffsetY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JobOffsetY_textBox.Text, out val))
                {
                    JobOffsetY = val;
                }
            }
        }

        private void JobOffsetY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JobOffsetY_textBox.Text, out val))
            {
                JobOffsetY = val;
            }
        }

        // =================================================================================
        private void ShowNominal_button_Click(object sender, EventArgs e)
        {
            double X;
            double Y;
            double A;

            DataGridViewCell cell = CadData_GridView.CurrentCell;
            if (cell == null)
            {
                return;  // no component selected
            }
            if (cell.OwningRow.Index < 0)
            {
                return;  // header row
            }
            if (!double.TryParse(cell.OwningRow.Cells["X_nominal"].Value.ToString(), out X))
            {
                DialogResult dialogResult = ShowMessageBox(
                    "Bad data at X_nominal",
                    "Bad data",
                    MessageBoxButtons.OK);
                return;
            }

            if (!double.TryParse(cell.OwningRow.Cells["Y_nominal"].Value.ToString(), out Y))
            {
                DialogResult dialogResult = ShowMessageBox(
                    "Bad data at Y_nominal",
                    "Bad data",
                    MessageBoxButtons.OK);
                return;
            }

            if (!double.TryParse(cell.OwningRow.Cells["Rotation"].Value.ToString(), out A))
            {
                DialogResult dialogResult = ShowMessageBox(
                    "Bad data at Rotation",
                    "Bad data",
                    MessageBoxButtons.OK);
                return;
            }

            CNC_XY_m(X + JobOffsetX + Properties.Settings.Default.General_JigOffsetX,
                Y + JobOffsetY + Properties.Settings.Default.General_JigOffsetY);
            DownCamera.ArrowAngle = A;
            DownCamera.DrawArrow = true;

            //ShowMessageBox(
            //    "This is " + cell.OwningRow.Cells["Component"].Value.ToString() + " approximate (nominal) location",
            //    "Locate Component",
            //    MessageBoxButtons.OK);
        }

        private void ShowNominal_button_Leave(object sender, EventArgs e)
        {
            DownCamera.DrawArrow = false;
        }


        // =================================================================================
        // Checks what is needed to check before doing something for a single component selected at "CAD data" table. 
        // If succesful, sets X, Y to component machine coordinates.
        private bool PrepareSingleComponentOperation(out double X, out double Y)
        {
            X = 0.0;
            Y = 0.0;

            DataGridViewCell cell = CadData_GridView.CurrentCell;
            if (cell == null)
            {
                return false;  // no component selected
            }
            if (cell.OwningRow.Index < 0)
            {
                return false;  // header row
            }
            if (cell.OwningRow.Cells["X_machine"].Value.ToString() == "Nan")
            {
                DialogResult dialogResult = ShowMessageBox(
                    "Component locations not yet measured. Measure now?",
                    "Measure now?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return false;
                }
                if (!BuildMachineCoordinateData_m())
                {
                    return false;
                }
            }

            if (!double.TryParse(cell.OwningRow.Cells["X_machine"].Value.ToString(), out X))
            {
                ShowMessageBox(
                    "Bad data at X_machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }

            if (!double.TryParse(cell.OwningRow.Cells["Y_machine"].Value.ToString(), out Y))
            {
                ShowMessageBox(
                    "Bad data at Y_machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        // =================================================================================
        private void ShowMachine_button_Click(object sender, EventArgs e)
        {
            double X;
            double Y;
            double A;

            if (!PrepareSingleComponentOperation(out X, out Y))
            {
                return;
            }
            CNC_XY_m(X, Y);
            if (!double.TryParse(CadData_GridView.CurrentCell.OwningRow.Cells["Rotation_machine"].Value.ToString(), out A))
            {
                ShowMessageBox(
                    "Bad data at Rotation_machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return;
            }
            DownCamera.ArrowAngle = A;
            DownCamera.DrawArrow = true;

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

        private void ShowMachine_button_Leave(object sender, EventArgs e)
        {
            DownCamera.DrawArrow = false;
        }


        // =================================================================================
        private bool MeasurePositionErrorByFiducial_m(double X, double Y, out double errX, out double errY)
        {

            // find nearest fiducial
            int FiducialsRow = 0;
            errX = 0;
            errY = 0;
            foreach (DataGridViewRow Row in JobData_GridView.Rows)
            {
                if (Row.Cells["GroupMethod"].Value.ToString() == "Fiducials")
                {
                    FiducialsRow = Row.Index;
                    break;
                }
            }
            if (FiducialsRow == 0)
            {
                ShowMessageBox(
                    "ResetPositionByFiducial: Fiducials not indicated",
                    "Missing data",
                    MessageBoxButtons.OK);
                return false;
            }
            string[] FiducialDesignators = JobData_GridView.Rows[FiducialsRow].Cells["ComponentList"].Value.ToString().Split(',');
            double ShortestDistance = 10000;
            double X_fid = Double.NaN;
            double Y_fid = Double.NaN;
            double X_shortest = Double.NaN;
            double Y_shortest = Double.NaN;
            int i;
            for (i = 0; i < FiducialDesignators.Count(); i++)
            {
                foreach (DataGridViewRow Row in CadData_GridView.Rows)
                {
                    if (Row.Cells["Component"].Value.ToString() == FiducialDesignators[i])
                    {
                        if (!double.TryParse(Row.Cells["X_Machine"].Value.ToString(), out X_fid))
                        {
                            ShowMessageBox(
                                "Problem with " + FiducialDesignators[i] + "X machine coordinate data",
                                "Bad data",
                                MessageBoxButtons.OK);
                            return false;
                        };
                        if (!double.TryParse(Row.Cells["Y_Machine"].Value.ToString(), out Y_fid))
                        {
                            ShowMessageBox(
                                "Problem with " + FiducialDesignators[i] + "Y machine coordinate data",
                                "Bad data",
                                MessageBoxButtons.OK);
                            return false;
                        };
                        break;
                    }  // end "if this is the row of the current fiducial, ...
                }
                // This is the fid we want. It is measured to be at X_fid, Y_fid
                if (double.IsNaN(X_fid) || double.IsNaN(Y_fid))
                {
                    ShowMessageBox(
                        "Machine coord data for fiducial " + FiducialDesignators[i] + "not found",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                }
                double dX = X_fid - X;
                double dY = Y_fid - Y;
                double Distance = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

                if (Distance < ShortestDistance)
                {
                    X_shortest = X_fid;
                    Y_shortest = Y_fid;
                    ShortestDistance = Distance;
                }
            }
            if (double.IsNaN(X_shortest) || double.IsNaN(Y_shortest))
            {
                ShowMessageBox(
                    "Problem finding nearest fiducial",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            };

            // go there
            CNC_XY_m(X_shortest, Y_shortest);
            SetFiducialsMeasurement();

            for (int tries = 0; tries < 5; tries++)
            {
                // 3mm max. error
                int res = DownCamera.GetClosestCircle(out errX, out errY, 3.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
                if (res != 0)
                {
                    break;
                }
                if (tries >= 4)
                {
                    ShowMessageBox(
                        "Finding fiducial: Can't regognize fiducial " + FiducialDesignators[i],
                        "No Circle found",
                        MessageBoxButtons.OK);
                    return false;
                }
            }
            errX = errX * Properties.Settings.Default.DownCam_XmmPerPixel;
            errY = -errY * Properties.Settings.Default.DownCam_YmmPerPixel;
            // and err_ now tell how much we are off.
            return true;
        }

        // =================================================================================
        private void ReMeasure_button_Click(object sender, EventArgs e)
        {
            ValidMeasurement_checkBox.Checked = false;
            BuildMachineCoordinateData_m();
            ValidMeasurement_checkBox.Checked = true;
            // CNC_Park();
        }

        private void TestNeedleRecognition_button_Click(object sender, EventArgs e)
        {
            double X = Cnc.CurrentX;
            double Y = Cnc.CurrentY;
            CalibrateNeedle_m();
            CNC_XYA_m(X, Y, 0.0);
        }


        // =================================================================================
        private bool ShowFootPrint_m(int Row)
        {
            // Turns on drawing a box of component outline to Downcamera image, if component size is known
            string FootPrint = CadData_GridView.Rows[Row].Cells["Value_Footprint"].Value.ToString();
            double sizeX = 0.0;
            double sizeY = 0.0;
            bool found = false;

            foreach (DataGridViewRow SizeRow in ComponentData_dataGridView.Rows)
            {
                if (SizeRow.Cells["PartialName_column"].Value != null)
                {
                    if (FootPrint.Contains(SizeRow.Cells["PartialName_column"].Value.ToString()))
                    {
                        if (!double.TryParse(SizeRow.Cells["SizeX_column"].Value.ToString(), out sizeX))
                        {
                            ShowMessageBox(
                                "Bad data at " + SizeRow.Cells["PartialName_column"].Value.ToString() + ", SizeX",
                                "Sloppy programmer error",
                                MessageBoxButtons.OK);
                            return false;
                        }
                        if (!double.TryParse(SizeRow.Cells["SizeY_column"].Value.ToString(), out sizeY))
                        {
                            ShowMessageBox(
                                "Bad data at " + SizeRow.Cells["PartialName_column"].Value.ToString() + ", SizeY",
                                "Sloppy programmer error",
                                MessageBoxButtons.OK);
                            return false;
                        }
                        found = true;
                        break;
                    }
                }
            }
            if (!found)
            {
                return true;  // ok if there is no data on component
            }

            double rot;
            if (!double.TryParse(CadData_GridView.Rows[Row].Cells["Rotation_machine"].Value.ToString(), out rot))
            {
                ShowMessageBox(
                    "Bad data at Rotation, machine",
                    "Sloppy programmer error",
                    MessageBoxButtons.OK);
                return false;
            }

            DownCamera.BoxSizeX = (int)Math.Round((sizeX) / Properties.Settings.Default.DownCam_XmmPerPixel);
            DownCamera.BoxSizeY = (int)Math.Round((sizeY) / Properties.Settings.Default.DownCam_YmmPerPixel);
            DownCamera.BoxRotationDeg = rot;
            DownCamera.DrawBox = true;
            return true;
        }

        private void Demo_button_Click(object sender, EventArgs e)
        {
            DemoThread = new Thread(() => DemoWork());
            DemoRunning = true;
            CNC_Z_m(0.0);
            DemoThread.IsBackground = true;
            DemoThread.Start();
        }

        private void StopDemo_button_Click(object sender, EventArgs e)
        {
            DemoRunning = false;
        }

        private bool DemoRunning = false;
        private Thread DemoThread;

        private void DemoWork()
        {
            double PCB_X = Properties.Settings.Default.General_JigOffsetX + Properties.Settings.Default.DownCam_NeedleOffsetX;
            double PCB_Y = Properties.Settings.Default.General_JigOffsetY + Properties.Settings.Default.DownCam_NeedleOffsetY;
            double HoleX;
            double HoleY;
            double PartX;
            double PartY;
            Random rnd = new Random();
            int a;
            int b;
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            CultureInfo culture = CultureInfo.InvariantCulture;
            string s = Tapes_dataGridView.Rows[0].Cells["X_Column"].Value.ToString();
            if (!double.TryParse(s, style, culture, out HoleX))
            {
                ShowMessageBox(
                    "Bad X data at Tape 1",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return;
            }
            s = Tapes_dataGridView.Rows[0].Cells["Y_Column"].Value.ToString();
            if (!double.TryParse(s, style, culture, out HoleY))
            {
                ShowMessageBox(
                    "Bad Y data at Tape 1",
                    "Tape data error",
                    MessageBoxButtons.OK
                );
                return;
            }

            while (DemoRunning)
            {
                // Simulate fast measurement. Last hole:
                if (!CNC_XY_m(HoleX + 6 * 4.0, HoleY)) goto err;
                Thread.Sleep(400);
                if (!DemoRunning) return;
                // First hole:
                if (!CNC_XY_m(HoleX, HoleY)) goto err;
                Thread.Sleep(400);
                if (!DemoRunning) return;
                // components
                for (int i = 0; i < 6; i++)
                {
                    // needle to part:
                    PartX = HoleX + i * 4 - 2 + Properties.Settings.Default.DownCam_NeedleOffsetX;
                    PartY = HoleY + 3.5 + Properties.Settings.Default.DownCam_NeedleOffsetY;
                    if (!CNC_XY_m(PartX, PartY)) goto err;
                    if (!DemoRunning) return;
                    // Pick up
                    if (!CNC_Z_m(Properties.Settings.Default.General_ZtoPCB)) goto err;
                    Thread.Sleep(Properties.Settings.Default.General_PickupVacuumTime);
                    if (!CNC_Z_m(0.0)) goto err;
                    if (!DemoRunning) return;
                    // goto position
                    a = rnd.Next(10, 60);
                    b = rnd.Next(10, 60);
                    if (!CNC_XY_m(PCB_X + a, PCB_Y + b)) goto err;
                    if (!DemoRunning) return;
                    // place
                    if (!CNC_Z_m(Properties.Settings.Default.General_ZtoPCB - 0.5)) goto err;
                    Thread.Sleep(Properties.Settings.Default.General_PickupReleaseTime);
                    if (!CNC_Z_m(0.0)) goto err;
                    if (!DemoRunning) return;
                }
            }
        err:
            DemoRunning = false;
        }

        // =================================================================================
        // Panelizing
        // =================================================================================
        private void Panelize_button_Click(object sender, EventArgs e)
        {
            PanelizeForm PanelizeDialog = new PanelizeForm(this);
            PanelizeDialog.CadData = CadData_GridView;
            PanelizeDialog.JobData = JobData_GridView;
            PanelizeDialog.ShowDialog(this);
            if (PanelizeDialog.OK == true)
            {
                DisplayText("Panelize ok.");
                Update_GridView(CadData_GridView);
                Update_GridView(JobData_GridView);
            }
        }


        #endregion Job page functions

        // =================================================================================
        //CAD data reading functions: Tries to understand different pick and place file formats
        // =================================================================================
        #region CAD data reading functions

        // =================================================================================
        // CADdataToMMs_m(): Data was in inches, convert to mms

        private bool CADdataToMMs_m()
        {
            double val;
            foreach (DataGridViewRow Row in CadData_GridView.Rows)
            {
                if (!double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out val))
                {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " X coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                Row.Cells["X_nominal"].Value = Math.Round((val / 2.54), 3).ToString();
                if (!double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out val))
                {
                    ShowMessageBox(
                        "Problem with " + Row.Cells["Component"].Value.ToString() + " Y coordinate data",
                        "Bad data",
                        MessageBoxButtons.OK);
                    return false;
                };
                Row.Cells["Y_nominal"].Value = Math.Round((val / 2.54), 3).ToString();
            }
            return true;
        }

        // =================================================================================
        // ParseKiCadData_m()
        // =================================================================================
        private bool ParseKiCadData_m(String[] AllLines)
        {
            // Convert KiCad data to regular CSV
            int i = 0;
            bool inches = false;
            // Skip headers until find one starting with "## "
            while (!(AllLines[i].StartsWith("## ")))
            {
                i++;
            };

            // inches vs mms
            if (AllLines[i++].Contains("inches"))
            {
                inches = true;
            }
            i++; // skip the "Side" line
            List<string> KiCadLines = new List<string>();
            KiCadLines.Add(AllLines[i++].Substring(2));  // add header, skip the "# " start
            // add rest of the lines
            while (!(AllLines[i]).StartsWith("## End"))
            {
                KiCadLines.Add(AllLines[i++]);
            };
            // parse the data
            string[] KicadArr = KiCadLines.ToArray();
            if (!ParseCadData_m(KicadArr, true))
            {
                return false;
            };
            // convert to mm'f if needed
            if (inches)
            {
                return (CADdataToMMs_m());
            }
            else
            {
                return true;
            }
        }

        // =================================================================================
        // ParseCadData_m(): main function called from file open
        // =================================================================================

        // =================================================================================
        // FindDelimiter_m(): Tries to find the difference with comma and semicolon separated files  
        bool FindDelimiter_m(String Line, out char delimiter)
        {
            int commas = 0;
            foreach (char c in Line)
            {
                if (c == ',')
                {
                    commas++;
                }
            };
            int semicolons = 0;
            foreach (char c in Line)
            {
                if (c == ';')
                {
                    semicolons++;
                }
            };
            if ((commas == 0) && (semicolons > 4))
            {
                delimiter = ';';
                return true;
            };
            if ((semicolons == 0) && (commas > 4))
            {
                delimiter = ',';
                return true;
            };

            ShowMessageBox(
                "File header parse fail",
                "Data format error",
                MessageBoxButtons.OK
            );
            delimiter = ',';
            return false;
        }

        private bool ParseCadData_m(String[] AllLines, bool KiCad)
        {
            int ComponentIndex;
            int ValueIndex;
            int FootPrintIndex;
            int X_Nominal_Index;
            int Y_Nominal_Index;
            int RotationIndex;
            int LayerIndex = -1;
            bool LayerDataPresent = false;
            int i;
            int LineIndex = 0;

            // Parse header. Skip empty lines and comment lines (starting with # or "//")
            foreach (string s in AllLines)
            {
                if (s == "")
                {
                    LineIndex++;
                    continue;
                }
                if (s[0] == '#')
                {
                    LineIndex++;
                    continue;
                };
                if ((s.Length > 1) && (s[0] == '/') && (s[1] == '/'))
                {
                    LineIndex++;
                    continue;
                };
                break;
            };

            char delimiter;
            if (KiCad)
            {
                delimiter = ' ';
            }
            else
            {
                if (!FindDelimiter_m(AllLines[0], out delimiter))
                {
                    return false;
                };
            }

            List<String> Headers = SplitCSV(AllLines[LineIndex++], delimiter);

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Designator") ||
                    (Headers[i] == "designator") ||
                    (Headers[i] == "Part") ||
                    (Headers[i] == "part") ||
                    (Headers[i] == "RefDes") ||
                    (Headers[i] == "Ref") ||
                    (Headers[i] == "Component") ||
                    (Headers[i] == "component")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component/Designator/Name not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            ComponentIndex = i;

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Value") ||
                    (Headers[i] == "value") ||
                    (Headers[i] == "Val") ||
                    (Headers[i] == "val") ||
                    (Headers[i] == "Comment") ||
                    (Headers[i] == "comment")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component value/comment not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            ValueIndex = i;

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Footprint") ||
                    (Headers[i] == "footprint") ||
                    (Headers[i] == "Package") ||
                    (Headers[i] == "package") ||
                    (Headers[i] == "Pattern") ||
                    (Headers[i] == "pattern")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component footprint/pattern not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            FootPrintIndex = i;

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "X") ||
                    (Headers[i] == "x") ||
                    (Headers[i] == "X (mm)") ||
                    (Headers[i] == "x (mm)") ||
                    (Headers[i] == "Center-X(mm)") ||
                    (Headers[i] == "PosX") ||
                    (Headers[i] == "Mid X") ||
                    (Headers[i] == "mid x")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component X not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            X_Nominal_Index = i;

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Y") ||
                    (Headers[i] == "y") ||
                    (Headers[i] == "Y (mm)") ||
                    (Headers[i] == "y (mm)") ||
                    (Headers[i] == "Center-Y(mm)") ||
                    (Headers[i] == "PosY") ||
                    (Headers[i] == "Mid Y") ||
                    (Headers[i] == "mid y")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component Y not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            Y_Nominal_Index = i;

            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Rotation") ||
                    (Headers[i] == "rotation") ||
                    (Headers[i] == "Rot") ||
                    (Headers[i] == "rot") ||
                    (Headers[i] == "Rotate")
                  )
                {
                    break;
                }
            }
            if (i >= Headers.Count)
            {
                ShowMessageBox("Component rotation not found in header line", "Syntax error", MessageBoxButtons.OK);
                return false;
            }
            RotationIndex = i;


            for (i = 0; i < Headers.Count; i++)
            {
                if ((Headers[i] == "Layer") ||
                    (Headers[i] == "layer") ||
                    (Headers[i] == "Side") ||
                    (Headers[i] == "side") ||
                    (Headers[i] == "TB") ||
                    (Headers[i] == "tb")
                  )
                {
                    LayerIndex = i;
                    LayerDataPresent = true;
                    break;
                }
            }

            // clear and rebuild the data tables
            CadData_GridView.Rows.Clear();
            JobData_GridView.Rows.Clear();
            foreach (DataGridViewColumn column in JobData_GridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;   // disable manual sort
            }

            // Parse data
            List<String> Line;
            string peek;

            for (i = LineIndex; i < AllLines.Count(); i++)   // for each component
            {
                if (i == 5)
                {
                    peek = AllLines[i];
                }
                // Skip: empty lines and comment lines (starting with # or "//")
                if (
                        (AllLines[i] == "")  // empty lines
                        ||
                        (AllLines[i] == "\"\"")  // empty lines ("")
                        ||
                        (AllLines[i][0] == '#')  // comment lines starting with #
                        ||
                        ((AllLines[i].Length > 1) && (AllLines[i][0] == '/') && (AllLines[i][1] == '/'))  // // comment lines starting with //
                    )
                {
                    continue;
                }

                Line = SplitCSV(AllLines[i], delimiter);
                // If layer is indicated and the component is not on this layer, skip it
                if (LayerDataPresent)
                {
                    if (Bottom_checkBox.Checked)
                    {
                        if ((Line[LayerIndex] == "Top") ||
                            (Line[LayerIndex] == "top") ||
                            (Line[LayerIndex] == "F.Cu") ||
                            (Line[LayerIndex] == "T") ||
                            (Line[LayerIndex] == "t"))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if ((Line[LayerIndex] == "Bottom") ||
                            (Line[LayerIndex] == "bottom") ||
                            (Line[LayerIndex] == "B") ||
                            (Line[LayerIndex] == "b") ||
                            (Line[LayerIndex] == "B.Cu") ||
                            (Line[LayerIndex] == "Bot") ||
                            (Line[LayerIndex] == "bot"))
                        {
                            continue;
                        }
                    }
                }
                CadData_GridView.Rows.Add();
                int Last = CadData_GridView.RowCount - 1;
                CadData_GridView.Rows[Last].Cells["Component"].Value = Line[ComponentIndex];
                CadData_GridView.Rows[Last].Cells["Value_Footprint"].Value = Line[ValueIndex] + "  |  " + Line[FootPrintIndex];
                CadData_GridView.Rows[Last].Cells["Rotation"].Value = Line[RotationIndex];

                if (LayerDataPresent)
                {
                    if (Bottom_checkBox.Checked)
                    {
                        CadData_GridView.Rows[Last].Cells["X_nominal"].Value = "-" + Line[X_Nominal_Index].Replace("mm", "");
                        double rot;
                        if (!double.TryParse(CadData_GridView.Rows[Last].Cells["Rotation"].Value.ToString(), out rot))
                        {
                            DialogResult dialogResult = ShowMessageBox(
                                "Bad data at Rotation",
                                "Bad data",
                                MessageBoxButtons.OK);
                            return false;
                        }
                        rot = -rot + 180;
                        CadData_GridView.Rows[Last].Cells["Rotation"].Value= rot.ToString();
                    }
                    else
                    {
                        CadData_GridView.Rows[Last].Cells["X_nominal"].Value = Line[X_Nominal_Index].Replace("mm", "");
                    }
                }
                else
                {
                    CadData_GridView.Rows[Last].Cells["X_nominal"].Value = Line[X_Nominal_Index].Replace("mm", "");
                }
                CadData_GridView.Rows[Last].Cells["Y_nominal"].Value = Line[Y_Nominal_Index].Replace("mm", "");
                CadData_GridView.Rows[Last].Cells["X_nominal"].Value = CadData_GridView.Rows[Last].Cells["X_nominal"].Value.ToString().Replace(",", ".");
                CadData_GridView.Rows[Last].Cells["Y_nominal"].Value = CadData_GridView.Rows[Last].Cells["Y_nominal"].Value.ToString().Replace(",", ".");
                CadData_GridView.Rows[Last].Cells["X_Machine"].Value = "Nan";   // will be set later 
                CadData_GridView.Rows[Last].Cells["Y_Machine"].Value = "Nan";
                CadData_GridView.Rows[Last].Cells["Rotation_machine"].Value = "Nan";
            }   // end "for each component..."

            // Disable manual sorting
            foreach (DataGridViewColumn column in CadData_GridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            CadData_GridView.ClearSelection();
            // Check, that our data is good:
            if (!ValidateCADdata_m())
                return false;

            return true;
        }   // end ParseCadData

        // =================================================================================
        // Helper function for ParseCadData() (and some others, hence public)

        public List<String> SplitCSV(string InputLine, char delimiter)
        {
            // input lines can be "xxx","xxxx","xx"; output is array: xxx  xxxxx  xx
            // or xxx,xxxx,xx; output is array: xxx  xxxx  xx
            // or xxx,"xx,xx",xxxx; output is array: xxx  xx,xx  xxxx

            List<String> Tokens = new List<string>();
            string Line = InputLine;
            while (Line != "")
            {
                // skip the delimiter(s)
                while (Line[0] == delimiter)
                {
                    if (Line.Length < 2)
                    {
                        ShowMessageBox(
                           "Unexpected end of line on " + InputLine,
                           "Line parsing failed",
                           MessageBoxButtons.OK);
                        return (Tokens);
                    }
                    Line = Line.Substring(1);
                };
                // add token
                if (Line[0] == '"')
                {
                    if (Line.Length < 2)
                    {
                        ShowMessageBox(
                           "Unexpected end of line on " + InputLine,
                           "Line parsing failed",
                           MessageBoxButtons.OK);
                        return (Tokens);
                    }
                    // token is "xxx"
                    Line = Line.Substring(1);   // skip the first "
                    Tokens.Add(Line.Substring(0, Line.IndexOf('"')));
                    Line = Line.Substring(Line.IndexOf('"') + 1);
                }
                else
                {
                    // token does not have "" 's
                    if (Line.IndexOf(delimiter) < 0)
                    {
                        Tokens.Add(Line);   // last element
                        Line = "";
                    }
                    else
                    {
                        Tokens.Add(Line.Substring(0, Line.IndexOf(delimiter)));
                        Line = Line.Substring(Line.IndexOf(delimiter));
                    }
                }
            }
            // remove leading spaces
            for (int i = 0; i < Tokens.Count; i++)
            {
                Tokens[i] = Tokens[i].Trim();
            }
            return (Tokens);
        }

        #endregion  CAD data reading functions

        // =================================================================================
        // Tape Positions page functions
        // =================================================================================
        #region Tape Positions page functions

        // This is set up at Form1_Load()
        void Tapes_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Ugly, but MS raises this error when programmatically changing a combobox cell value
            // or when it is not set at design time (and we will put custom tape names in)
        }

        private void Tapes_tabPage_Begin()
        {
            foreach (DataGridViewRow row in Tapes_dataGridView.Rows)
            {
                row.HeaderCell.Value = row.Index.ToString();
                row.Cells["SelectButtonColumn"].Value = "Reset";
            }
            SetDownCameraDefaults();
            SelectCamera(DownCamera);
            DownCamera.ImageBox = Tapes_pictureBox;
        }

        private void Tapes_tabPage_End()
        {
            ZGuardOn();
        }

        private void AddTape_button_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection SelectedRows = Tapes_dataGridView.SelectedRows;
            int index = 0;
            if (SelectedRows.Count == 0)
            {
                // add to the end
                Tapes_dataGridView.Rows.Insert(Tapes_dataGridView.Rows.Count);
                index = Tapes_dataGridView.Rows.Count - 1;
            }
            else
            {
                // replace current
                index = Tapes_dataGridView.SelectedRows[0].Index;
                Tapes_dataGridView.Rows.RemoveAt(index);
                Tapes_dataGridView.Rows.Insert(index);
            };
            // Add data
            // SelectButtonColumn: On main form, resets tape to position 1.
            // The gridView is moved to selection dialog on job run time. There the SelectButton selects that tape.
            Tapes_dataGridView.Rows[index].Cells["SelectButtonColumn"].Value = "Reset";
            // IdColumn: User settable name for the tape
            Tapes_dataGridView.Rows[index].Cells["IdColumn"].Value = index.ToString();
            // X_Column, Y_Column: Originally set approximate location for the first hole
            Tapes_dataGridView.Rows[index].Cells["X_Column"].Value = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Tapes_dataGridView.Rows[index].Cells["Y_Column"].Value = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            // OrientationColumn: Which way the tape is set. It is the direction to go for next part
            Tapes_dataGridView.Rows[index].Cells["OrientationColumn"].Value = "+X";
            // RotationColumn: Which way the parts are rotated on the tape. if 0, parts form +Y oriented tape
            // correspont to 0deg. on teh PCB, tape.e. the placement operation does not rotate them.
            Tapes_dataGridView.Rows[index].Cells["RotationColumn"].Value = "0deg.";
            // WidthColumn: sets the width of the tape and the distance from one part to next. 
            // From EIA-481, we get the part location from the hole location.
            Tapes_dataGridView.Rows[index].Cells["WidthColumn"].Value = "8/4mm";
            // TypeColumn: used in hole recognition
            Tapes_dataGridView.Rows[index].Cells["TypeColumn"].Value = "Paper (White)";
            // Next_Column tells the part number of next part. 
            // NextX, NextY tell the approximate hole location for the next part. Incremented when a part is picked up.
            Tapes_dataGridView.Rows[index].Cells["Next_Column"].Value = "1";
            Tapes_dataGridView.Rows[index].Cells["NextX_Column"].Value = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Tapes_dataGridView.Rows[index].Cells["NextY_Column"].Value = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            // PickupZ_Column, PlaceZ_Column: The Z values are measured when first part is placed. Picking up and
            // placing the next parts will then be faster.
            Tapes_dataGridView.Rows[index].Cells["PickupZ_Column"].Value = "--";
            Tapes_dataGridView.Rows[index].Cells["PlaceZ_Column"].Value = "--";
            Tapes_dataGridView.Rows[index].Cells["Tray_Column"].Value = "--";
        }

        private void TapeUp_button_Click(object sender, EventArgs e)
        {
            DataGrid_Up_button(Tapes_dataGridView);
        }

        private void TapeDown_button_Click(object sender, EventArgs e)
        {
            DataGrid_Down_button(Tapes_dataGridView);
        }

        private void DeleteTape_button_Click(object sender, EventArgs e)
        {
            if (Tapes_dataGridView.RowCount > 0)
            {
                Tapes_dataGridView.Rows.RemoveAt(Tapes_dataGridView.CurrentCell.RowIndex);
            }
        }

        private void TapeGoTo_button_Click(object sender, EventArgs e)
        {
            if (Tapes_dataGridView.SelectedCells.Count != 1)
            {
                return;
            };
            double X;
            double Y;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            if (!double.TryParse(Tapes_dataGridView.Rows[row].Cells["X_Column"].Value.ToString(), out X))
            {
                return;
            }
            if (!double.TryParse(Tapes_dataGridView.Rows[row].Cells["Y_Column"].Value.ToString(), out Y))
            {
                return;
            }
            CNC_XY_m(X, Y);
        }

        private void TapeSet1_button_Click(object sender, EventArgs e)
        {
            if (Tapes_dataGridView.SelectedCells.Count != 1)
            {
                return;
            };
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            Tapes_dataGridView.Rows[row].Cells["X_Column"].Value = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Tapes_dataGridView.Rows[row].Cells["Y_Column"].Value = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            // fix #22 update next coordinates when setting hole 1
            Tapes_dataGridView.Rows[row].Cells["Next_Column"].Value = "1";
            Tapes_dataGridView.Rows[row].Cells["NextX_Column"].Value = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            Tapes_dataGridView.Rows[row].Cells["NextY_Column"].Value = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);

        }


        // ==========================================================================================================
        // Tapes_dataGridView_CellClick(): 
        // If the click is on a button column, resets the tape. 
        private void Tapes_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cell  IdColumn
            if ((e.RowIndex < 0) || (e.ColumnIndex != Tapes_dataGridView.Columns["SelectButtonColumn"].Index))
            {
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
        private string SelectTape(string header)
        {
            System.Drawing.Point loc = Tapes_dataGridView.Location;
            Size size = Tapes_dataGridView.Size;
            DataGridView Grid = Tapes_dataGridView;
            TapeSelectionForm TapeDialog = new TapeSelectionForm(Grid);
            TapeDialog.HeaderString = header;
            Tapes_dataGridView.CellClick -= new DataGridViewCellEventHandler(Tapes_dataGridView_CellClick);
            this.Controls.Remove(Tapes_dataGridView);

            TapeDialog.ShowDialog(this);

            string ID = TapeDialog.ID;  // get the result
            DisplayText("Selected tape: " + ID);

            Tapes_tabPage.Controls.Add(Tapes_dataGridView);
            Tapes_dataGridView = Grid;
            Tapes_dataGridView.Location = loc;
            Tapes_dataGridView.Size = size;

            TapeDialog.Dispose();
            Tapes_dataGridView.CellClick += new DataGridViewCellEventHandler(Tapes_dataGridView_CellClick);
            return ID;
        }

        private void ResetOneTape_button_Click(object sender, EventArgs e)
        {
            for (int CurrentRow = 0; CurrentRow < JobData_GridView.RowCount; CurrentRow++)
            {
                DataGridViewRow Row = JobData_GridView.Rows[CurrentRow];
                bool DoRow = false;
                foreach (DataGridViewCell oneCell in Row.Cells)
                {
                    if (oneCell.Selected)
                    {
                        DoRow = true;
                        break;
                    }
                }

                if (!DoRow)
                {
                    continue;
                };
                // Reset this component's tape:
                string ID = JobData_GridView.Rows[CurrentRow].Cells[3].Value.ToString();
                int TapeNo = 0;
                if (Tapes.IdValidates_m(ID, out TapeNo))
                {
                    // fix #22 reset next coordinates
                    Tapes.Reset(TapeNo);
                }
            }
        }

        private void ResetAllTapes_button_Click(object sender, EventArgs e)
        {
            Tapes.ClearAll();
        }

        private void SetPartNo_button_Click(object sender, EventArgs e)
        {
            int no = 0;
            if (!int.TryParse(NextPart_TextBox.Text, out no))
            {
                return;
            }
            DataGridViewRow Row = Tapes_dataGridView.Rows[Tapes_dataGridView.CurrentCell.RowIndex];
            Row.Cells["Next_Column"].Value = no.ToString();
            Row.Cells["NextX_Column"].Value = Cnc.CurrentX.ToString();
            Row.Cells["NextY_Column"].Value = Cnc.CurrentY.ToString();
        }

        private void Tape_GoToNext_button_Click(object sender, EventArgs e)
        {
            if (Tapes_dataGridView.SelectedCells.Count != 1)
            {
                return;
            };
            double X;
            double Y;
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            if (!double.TryParse(Tapes_dataGridView.Rows[row].Cells["NextX_Column"].Value.ToString(), out X))
            {
                return;
            }
            if (!double.TryParse(Tapes_dataGridView.Rows[row].Cells["NextY_Column"].Value.ToString(), out Y))
            {
                return;
            }
            CNC_XY_m(X, Y);
        }

        private void Tape_resetZs_button_Click(object sender, EventArgs e)
        {
            int row = Tapes_dataGridView.CurrentCell.RowIndex;
            Tapes_dataGridView.Rows[row].Cells["PickupZ_Column"].Value = "--";
            Tapes_dataGridView.Rows[row].Cells["PlaceZ_Column"].Value = "--";
        }


        private void SaveAllTapes_button_Click(object sender, EventArgs e)
        {
            TapesAll_saveFileDialog.Filter = "LitePlacer Tapes files (*.tapes)|*.tapes|All files (*.*)|*.*";

            if (TapesAll_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveDataGrid(TapesAll_saveFileDialog.FileName, Tapes_dataGridView);
            }
        }

        private void LoadAllTapes_button_Click(object sender, EventArgs e)
        {
            TapesAll_openFileDialog.Filter = "LitePlacer Tapes files (*.tapes)|*.tapes|All files (*.*)|*.*";

            if (TapesAll_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadDataGrid(TapesAll_openFileDialog.FileName, Tapes_dataGridView);
                Tapes.AddWidthValues();
            }
        }

        private void HoleTest_button_Click(object sender, EventArgs e)
        {
            int PartNum = 0;
            int TapeNum = 0;
            DataGridViewRow Row = Tapes_dataGridView.Rows[Tapes_dataGridView.CurrentCell.RowIndex];
            if (!int.TryParse(HoleTest_maskedTextBox.Text, out PartNum))
            {
                if (!int.TryParse(Row.Cells["Next_Column"].Value.ToString(), out PartNum))
                {
                    return;
                }
            }
            string Id = Row.Cells["IdColumn"].Value.ToString();
            double X = 0.0;
            double Y = 0.0;
            if (!Tapes.IdValidates_m(Id, out TapeNum))
            {
                return;
            }
            if (Tapes.GetPartHole_m(TapeNum, PartNum, out X, out Y))
            {
                CNC_XY_m(X, Y);
            }
        }

        private void ShowPart_button_Click(object sender, EventArgs e)
        {
            int PartNum = 0;
            int TapeNum = 0;
            DataGridViewRow Row = Tapes_dataGridView.Rows[Tapes_dataGridView.CurrentCell.RowIndex];
            if (!int.TryParse(HoleTest_maskedTextBox.Text, out PartNum))
            {
                if (!int.TryParse(Row.Cells["Next_Column"].Value.ToString(), out PartNum))
                {
                    return;
                }
            }
            string Id = Row.Cells["IdColumn"].Value.ToString();
            double X = 0.0;
            double Y = 0.0;
            if (!Tapes.IdValidates_m(Id, out TapeNum))
            {
                return;
            }
            if (!Tapes.GetPartHole_m(TapeNum, PartNum, out X, out Y))
            {
                return;
            }
            double pX = 0.0;
            double pY = 0.0;
            double A = 0.0;
            // Tapes.GetPartLocationFromHolePosition_m uses the next column from Tapes_dataGridView.
            // Set it temporarily, but remember what was there:
            string temp = Row.Cells["Next_Column"].Value.ToString();
            Row.Cells["Next_Column"].Value = PartNum.ToString();
            if (Tapes.GetPartLocationFromHolePosition_m(TapeNum, X, Y, out pX, out pY, out A))
            {
                CNC_XY_m(pX, pY);
            }
            DownCamera.ArrowAngle = A;
            DownCamera.DrawArrow = true;

            Row.Cells["Next_Column"].Value = temp.ToString();
        }

        private void ShowPart_button_Leave(object sender, EventArgs e)
        {
            DownCamera.DrawArrow = false;
        }



        // =================================================================================
        // Trays:

        private void SaveTray_button_Click(object sender, EventArgs e)
        {
            TapesAll_saveFileDialog.Filter = "LitePlacer Tapes files (*.tapes)|*.tapes|All files (*.*)|*.*";

            if (TapesAll_saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            // Get current tray ID
            int CurrRow = Tapes_dataGridView.CurrentCell.RowIndex;
            string TrayID = Tapes_dataGridView.Rows[CurrRow].Cells["Tray_Column"].Value.ToString();

            // Copy the tapes with this ID to Clipboard datagridview:
            DataGridView ClipBoard_dgw = new DataGridView();
            ClipBoard_dgw.AllowUserToAddRows = false;  // this prevents an empty row in the end
            foreach (DataGridViewColumn col in Tapes_dataGridView.Columns)
            {
                ClipBoard_dgw.Columns.Add(col.Clone() as DataGridViewColumn);
            }
            DataGridViewRow NewRow = new DataGridViewRow();
            foreach (DataGridViewRow row in Tapes_dataGridView.Rows)
            {
                if (row.Cells["Tray_Column"].Value.ToString() == TrayID)
                {
                    NewRow = (DataGridViewRow)row.Clone();
                    int intColIndex = 0;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        NewRow.Cells[intColIndex].Value = cell.Value;
                        intColIndex++;
                    }
                    ClipBoard_dgw.Rows.Add(NewRow);
                }
            }

            // Save the Clipboard
            SaveDataGrid(TapesAll_saveFileDialog.FileName, ClipBoard_dgw);
        }

        private void LoadTrayFromFile(string FileName)
        {
            DataGridView ClipBoard_dgw = new DataGridView();
            ClipBoard_dgw.AllowUserToAddRows = false;  // this prevents an empty row in the end
            foreach (DataGridViewColumn col in Tapes_dataGridView.Columns)
            {
                ClipBoard_dgw.Columns.Add(new DataGridViewColumn(col.CellTemplate));
            }
            LoadDataGrid(FileName, ClipBoard_dgw);
            DataGridViewRow NewRow = new DataGridViewRow();
            foreach (DataGridViewRow row in ClipBoard_dgw.Rows)
            {
                NewRow = (DataGridViewRow)row.Clone();
                int intColIndex = 0;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    NewRow.Cells[intColIndex].Value = cell.Value;
                    intColIndex++;
                }
                Tapes_dataGridView.Rows.Add(NewRow);
            }
            Update_GridView(Tapes_dataGridView);
        }

        private void LoadTray_button_Click(object sender, EventArgs e)
        {
            TapesAll_openFileDialog.Filter = "LitePlacer Tapes files (*.tapes)|*.tapes|All files (*.*)|*.*";

            if (TapesAll_openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            LoadTrayFromFile(TapesAll_openFileDialog.FileName);
        }

        private void DeleteTray(string TrayID, int col_index)
        {
            // removes a tray from Tapes_dataGridView
            // Can't modify the gridview and iterate through it at the same time, so:

            // Copy current to clipboard
            DataGridView ClipBoard_dgw = new DataGridView();
            ClipBoard_dgw.AllowUserToAddRows = false;  // this prevents an empty row in the end
            // create columns
            foreach (DataGridViewColumn col in Tapes_dataGridView.Columns)
            {
                ClipBoard_dgw.Columns.Add(new DataGridViewColumn(col.CellTemplate));
            }
            // copy rows
            DataGridViewRow NewRow = new DataGridViewRow();
            foreach (DataGridViewRow row in Tapes_dataGridView.Rows)
            {
                NewRow = (DataGridViewRow)row.Clone();
                int intColIndex = 0;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    NewRow.Cells[intColIndex].Value = cell.Value;
                    intColIndex++;
                }
                ClipBoard_dgw.Rows.Add(NewRow);
            }

            Tapes_dataGridView.Rows.Clear();   // Clear existing
            // Copy back if needed
            foreach (DataGridViewRow row in ClipBoard_dgw.Rows)
            {
                NewRow = (DataGridViewRow)row.Clone();
                int intColIndex = 0;
                if (row.Cells[col_index].Value.ToString() != TrayID)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        NewRow.Cells[intColIndex].Value = cell.Value;
                        intColIndex++;
                    }
                    Tapes_dataGridView.Rows.Add(NewRow);
                }
            }
        }

        private void ReplaceTray_button_Click(object sender, EventArgs e)
        {
            TapesAll_openFileDialog.Filter = "LitePlacer Tapes files (*.tapes)|*.tapes|All files (*.*)|*.*";

            if (TapesAll_openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            // Get current tray ID
            int CurrRow = Tapes_dataGridView.CurrentCell.RowIndex;
            string TrayID = Tapes_dataGridView.Rows[CurrRow].Cells["Tray_Column"].Value.ToString();
            int col = Tapes_dataGridView.Rows[CurrRow].Cells["Tray_Column"].ColumnIndex;
            DeleteTray(TrayID, col);
            LoadTrayFromFile(TapesAll_openFileDialog.FileName);
        }

        private void ReloadTray_button_Click(object sender, EventArgs e)
        {
            // Get current tray ID
            int CurrRow = Tapes_dataGridView.CurrentCell.RowIndex;
            string TrayID = Tapes_dataGridView.Rows[CurrRow].Cells["Tray_Column"].Value.ToString();
            foreach (DataGridViewRow row in Tapes_dataGridView.Rows)
            {
                if (row.Cells["Tray_Column"].Value.ToString() == TrayID)
                {
                    row.Cells["Next_Column"].Value = 1;
                }
            }
        }

        // =================================================================================
        // Custom tapes:

        private void SaveCustomTapes_button_Click(object sender, EventArgs e)
        {
            TapesAll_saveFileDialog.Filter = "LitePlacer Custom Tapes files (*.customtapes)|*.customtapes|All files (*.*)|*.*";

            if (TapesAll_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveDataGrid(TapesAll_saveFileDialog.FileName, CustomTapes_dataGridView);
            }
        }

        private void LoadCustomTapes_button_Click(object sender, EventArgs e)
        {
            TapesAll_openFileDialog.Filter = "LitePlacer Custom Tapes files (*.customtapes)|*.customtapes|All files (*.*)|*.*";

            if (TapesAll_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadDataGrid(TapesAll_openFileDialog.FileName, CustomTapes_dataGridView);
            }
        }

        private void DeleteCustomTape_button_Click(object sender, EventArgs e)
        {
            if (CustomTapes_dataGridView.RowCount > 1)
            {
                CustomTapes_dataGridView.Rows.RemoveAt(CustomTapes_dataGridView.CurrentCell.RowIndex);
            }
            // ReloadTapes();
            Tapes.AddCustomTapesToTapes();
        }

        private void CustomTapeUp_button_Click(object sender, EventArgs e)
        {
            DataGrid_Up_button(CustomTapes_dataGridView);
            Tapes.AddCustomTapesToTapes();
        }

        private void CustomTapeDown_button_Click(object sender, EventArgs e)
        {
            DataGrid_Down_button(CustomTapes_dataGridView);
            Tapes.AddCustomTapesToTapes();
        }

        private void UseCustomTapes_button_Click(object sender, EventArgs e)
        {
            Tapes.AddCustomTapesToTapes();
        }



        #endregion  TapeNumber Positions page functions

        // =================================================================================
        // Test functions
        // =================================================================================
        #region test functions

        // =================================================================================
        private void LabelTestButtons()
        {
            Test1_button.Text = "Pickup this";
            Test2_button.Text = "Place here";
            Test3_button.Text = "Probe (n.c.)";
            Test4_button.Text = "Needle to cam";
            Test5_button.Text = "Probe down";
            Test6_button.Text = "Needle up";
        }

        // test 1

        private void Test1_button_Click(object sender, EventArgs e)
        {
            double Xmark = Cnc.CurrentX;
            double Ymark = Cnc.CurrentY;
            DisplayText("test 1: Pick up this (probing)");
            PumpOn();
            VacuumOff();
            if (!Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA))
            {
                PumpOff_NoWorkaround();
                return;
            }
            if (!Needle_ProbeDown_m())
            {
                return;
            }
            VacuumOn();
            CNC_Z_m(0);  // pick up
            CNC_XY_m(Xmark, Ymark);
        }

        // =================================================================================
        // test 2

        // static int test2_state = 0;
        private void Test2_button_Click(object sender, EventArgs e)
        {
            double Xmark = Cnc.CurrentX;
            double Ymark = Cnc.CurrentY;
            DisplayText("test 2: Place here (probing)");
            if (!Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA))
            {
                return;
            }
            Needle_ProbeDown_m();
            VacuumOff();
            CNC_Z_m(0);  // back up
            CNC_XY_m(Xmark, Ymark);  // show results
        }

        // =================================================================================
        // test 3
        private void Test3_button_Click(object sender, EventArgs e)
        {
            Xmark = Cnc.CurrentX;
            Ymark = Cnc.CurrentY;
            CNC_XY_m((Cnc.CurrentX + Properties.Settings.Default.DownCam_NeedleOffsetX), (Cnc.CurrentY + Properties.Settings.Default.DownCam_NeedleOffsetY));
            Needle_ProbeDown_m();
        }


        // =================================================================================
        // test 4

        private void Test4_button_Click(object sender, EventArgs e)
        {
            double xp = Properties.Settings.Default.UpCam_PositionX;
            double xo = Properties.Settings.Default.DownCam_NeedleOffsetX;
            double yp = Properties.Settings.Default.UpCam_PositionY;
            double yo = Properties.Settings.Default.DownCam_NeedleOffsetY;
            Needle.Move_m(xp - xo, yp - yo, Cnc.CurrentA);
        }

        // =================================================================================
        // test 5

        private double Xmark;
        private double Ymark;
        private void Test5_button_Click(object sender, EventArgs e)
        {
            Xmark = Cnc.CurrentX;
            Ymark = Cnc.CurrentY;
            if (!Needle.Move_m(Cnc.CurrentX, Cnc.CurrentY, Cnc.CurrentA))
            {
                return;
            }
            Needle.ProbingMode(true, JSON);
            Needle_ProbeDown_m();
        }

        // =================================================================================
        // test 6
        private void Test6_button_Click(object sender, EventArgs e)
        {
            DisplayText("test 6: Needle up");
            CNC_Z_m(0);  // go up
            CNC_XY_m(Xmark, Ymark);

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

        #endregion test functions

        // ==========================================================================================================
        // Measurement boxes (Homing, Needle, OriginalFiducials, Tapes etc)
        // ==========================================================================================================
        #region Measurementboxes

        // ==========================================================================================================
        // Some helpers:

        public void DataGridViewCopy(DataGridView FromGr, ref DataGridView ToGr, bool print = true)
        {
            DataGridViewRow NewRow;
            ToGr.Rows.Clear();

            foreach (DataGridViewRow Row in FromGr.Rows)
            {
                NewRow = (DataGridViewRow)Row.Clone();
                for (int i = 0; i < Row.Cells.Count; i++)
                {
                    NewRow.Cells[i].Value = Row.Cells[i].Value;
                }
                ToGr.Rows.Add(NewRow);
            }
            if (print)
            {
                // Print results:
                string DebugStr = FromGr.Name + " => " + ToGr.Name + ":";
                DebugStr = DebugStr.Replace("_dataGridView", "");
                if (ToGr.Rows.Count == 0)
                {
                    DisplayText(DebugStr + "( )");
                }
                else
                {
                    foreach (DataGridViewRow Row in ToGr.Rows)
                    {
                        DebugStr += "( ";
                        for (int i = 0; i < Row.Cells.Count; i++)
                        {
                            if (Row.Cells[i].Value == null)
                            {
                                DebugStr += "--, ";
                            }
                            else
                            {
                                DebugStr += Row.Cells[i].Value.ToString() + ", ";
                            }
                        }
                        DebugStr += ")\n";
                        DebugStr = DebugStr.Replace(", )\n", " )");
                        DisplayText(DebugStr);
                    }
                }
                ClearEditTargets();
            }
        }

        // ==========================================================================================================
        // DownCam:


        // ==========================================================================================================

        private void DebugCirclesDownCamera(double Tolerance)
        {
            double X, Y;
            if (DownCamera.GetClosestCircle(out X, out Y, Tolerance) > 0)
            {
                X = X * Properties.Settings.Default.DownCam_XmmPerPixel;
                Y = -Y * Properties.Settings.Default.DownCam_YmmPerPixel;
                DisplayText("X: " + X.ToString("0.000", CultureInfo.InvariantCulture));
                DisplayText("Y: " + Y.ToString("0.000", CultureInfo.InvariantCulture));
            }
            else
            {
                DisplayText("No results.");
            }
        }

        // ==========================================================================================================
        // Snapshot:
        private void UpCam_SnapshotToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref UpcamSnapshot_dataGridView);
        }

        private void UpCam_SnapshotToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(UpcamSnapshot_dataGridView, ref Display_dataGridView);
            UpCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void UpCam_TakeSnapshot_button_Click(object sender, EventArgs e)
        {
            UpCam_TakeSnapshot();
        }

        private void UpCam_TakeSnapshot()
        {
            SelectCamera(UpCamera);
            DisplayText("UpCam_TakeSnapshot()");
            UpCamera.SnapshotRotation = Cnc.CurrentA;
            UpCamera.BuildMeasurementFunctionsList(UpcamSnapshot_dataGridView);
            UpCamera.TakeSnapshot();

            DownCamera.SnapshotOriginalImage = new Bitmap(UpCamera.SnapshotImage);
            DownCamera.SnapshotImage = new Bitmap(UpCamera.SnapshotImage);

            // We need a copy of the snapshot to scale it, in 24bpp format. See http://stackoverflow.com/questions/2016406/converting-bitmap-pixelformats-in-c-sharp
            Bitmap Snapshot24bpp = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(Snapshot24bpp))
            {
                gr.DrawImage(UpCamera.SnapshotOriginalImage, new Rectangle(0, 0, 640, 480));
            }
            // scale:
            double Xscale = Properties.Settings.Default.UpCam_XmmPerPixel / Properties.Settings.Default.DownCam_XmmPerPixel;
            double Yscale = Properties.Settings.Default.UpCam_YmmPerPixel / Properties.Settings.Default.DownCam_YmmPerPixel;
            double zoom = UpCamera.GetMeasurementZoom();
            Xscale = Xscale / zoom;
            Yscale = Yscale / zoom;
            int SnapshotSizeX = (int)(Xscale * 640);
            int SnapshotSizeY = (int)(Yscale * 480);
            // SnapshotSize is the size (in pxls) of upcam snapshot, scaled so that it draws in correct size on downcam image.
            ResizeNearestNeighbor RezFilter = new ResizeNearestNeighbor(SnapshotSizeX, SnapshotSizeY);
            Bitmap ScaledShot = RezFilter.Apply(Snapshot24bpp);  // and this is the scaled image
            // Mirror:
            Mirror MirrFilter = new Mirror(false, true);
            MirrFilter.ApplyInPlace(ScaledShot);

            // Clear DownCam image
            Graphics DownCamGr = Graphics.FromImage(DownCamera.SnapshotImage);
            DownCamGr.Clear(Color.Black);
            // Embed the ScaledShot to it. Upper left corner of the embedding is:
            int X = 320 - SnapshotSizeX / 2;
            int Y = 240 - SnapshotSizeY / 2;
            DownCamGr.DrawImage(ScaledShot, X, Y, SnapshotSizeX, SnapshotSizeY);
            DownCamera.SnapshotImage.MakeTransparent(Color.Black);
            // DownCam Snapshot is ok, copy it to original too
            DownCamera.SnapshotOriginalImage = new Bitmap(DownCamera.SnapshotImage);

            DownCamera.SnapshotRotation = Cnc.CurrentA;
        }

        private void UpcamSnapshot_ColorBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Show the color dialog.
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                UpcamSnapshot_ColorBox.BackColor = colorDialog1.Color;
                Properties.Settings.Default.UpCam_SnapshotColor = colorDialog1.Color;
                UpCamera.SnapshotColor = colorDialog1.Color;
            }
        }


        private void DownCam_SnapshotToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref DowncamSnapshot_dataGridView);
        }

        private void DownCam_SnapshotToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(DowncamSnapshot_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void DownCam_TakeSnapshot_button_Click(object sender, EventArgs e)
        {
            DownCamera.SnapshotRotation = Cnc.CurrentA;
            DownCamera.BuildMeasurementFunctionsList(DowncamSnapshot_dataGridView);
            DownCamera.TakeSnapshot();
        }

        private void DowncamSnapshot_ColorBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Show the color dialog.
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                DowncamSnapshot_ColorBox.BackColor = colorDialog1.Color;
                Properties.Settings.Default.DownCam_SnapshotColor = colorDialog1.Color;
                DownCamera.SnapshotColor = colorDialog1.Color;
            }
        }


        // ==========================================================================================================
        // Homing:
        private void HomingToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref Homing_dataGridView);
        }

        private void HomingToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Homing_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void HomingMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetHomingMeasurement();
                // Big tolerance for manual debug
                DebugCirclesDownCamera(20.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }

        private void SetHomingMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(Homing_dataGridView);
        }

        // ==========================================================================================================
        // OriginalFiducials:
        private void FiducialsToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref Fiducials_dataGridView);
        }

        private void FiducialsToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Fiducials_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void SetFiducialsMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(Fiducials_dataGridView);
            Thread.Sleep(100);   // for automatic camera gain to have an effect
        }

        private void FiducialsMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetFiducialsMeasurement();
                DebugCirclesDownCamera(20.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }

        // ==========================================================================================================
        // Components:

        private void DebugComponents_Camera(double Tolerance, Camera Cam, double mmPerPixel)
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


        private void ComponentsToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref Components_dataGridView);
        }

        private void ComponentsToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Components_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }


        private void SetComponentsMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(Components_dataGridView);
        }

        private void ComponentsMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetComponentsMeasurement();
                DebugComponents_Camera(
                    10.0 / Properties.Settings.Default.DownCam_XmmPerPixel,
                    DownCamera,
                    Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }


        // ==========================================================================================================
        // Paper TapeNumber:
        private void PaperTapeToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref PaperTape_dataGridView);
        }

        private void PaperTapeToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(PaperTape_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        public void SetPaperTapeMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(PaperTape_dataGridView);
        }

        private void PaperTapeMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetPaperTapeMeasurement();
                DebugCirclesDownCamera(20.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }

        // ==========================================================================================================
        // Clear Plastic TapeNumber:
        private void ClearTapeToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref ClearTape_dataGridView);
        }

        private void ClearTapeToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(ClearTape_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        public void SetClearTapeMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(ClearTape_dataGridView);
        }

        private void ClearTapeMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetClearTapeMeasurement();
                DebugCirclesDownCamera(20.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }

        // ==========================================================================================================
        // Black Plastic TapeNumber:
        private void BlackTapeToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref BlackTape_dataGridView);
        }

        private void BlackTapeToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(BlackTape_dataGridView, ref Display_dataGridView);
            DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        public void SetBlackTapeMeasurement()
        {
            DownCamera.BuildMeasurementFunctionsList(BlackTape_dataGridView);
        }

        private void BlackTapeMeasure_button_Click(object sender, EventArgs e)
        {
            if (DownCamera.IsRunning())
            {
                SetBlackTapeMeasurement();
                DebugCirclesDownCamera(20.0 / Properties.Settings.Default.DownCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Down camera is not running.");
            }
        }

        // ==========================================================================================================
        // UpCam:
        // Needle:
        private void NeedleToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref Needle_dataGridView);
        }

        private void NeedleToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Needle_dataGridView, ref Display_dataGridView);
            UpCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void SetNeedleMeasurement()
        {
            UpCamera.BuildMeasurementFunctionsList(Needle_dataGridView);
        }

        private void NeedleMeasure_button_Click(object sender, EventArgs e)
        {
            if (UpCamera.IsRunning())
            {
                SetNeedleMeasurement();
                // Manual debug, big tolerance
                DebugCirclesUpCamera(20.0 / Properties.Settings.Default.UpCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Up camera is not running.");
            }
        }

        private void DebugCirclesUpCamera(double Tolerance)
        {
            double X, Y;
            double Xpx, Ypx;
            if (UpCamera.GetClosestCircle(out X, out Y, Tolerance) > 0)
            {
                Xpx = X * UpCamera.GetMeasurementZoom();
                Ypx = Y * UpCamera.GetMeasurementZoom();
                DisplayText("X: " + Xpx.ToString() + "pixels, Y: " + Ypx.ToString() + "pixels");
                X = X * Properties.Settings.Default.UpCam_XmmPerPixel;
                Y = -Y * Properties.Settings.Default.UpCam_YmmPerPixel;
                DisplayText("X: " + X.ToString("0.000", CultureInfo.InvariantCulture));
                DisplayText("Y: " + Y.ToString("0.000", CultureInfo.InvariantCulture));
            }
            else
            {
                DisplayText("No results.");
            }
        }


        // ==========================================================================================================
        // Components:
        private void UpCamComponentsToHere_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(Display_dataGridView, ref UpCamComponents_dataGridView);
        }

        private void UpCamComponentsToDisplay_button_Click(object sender, EventArgs e)
        {
            DataGridViewCopy(UpCamComponents_dataGridView, ref Display_dataGridView);
            UpCamera.BuildDisplayFunctionsList(Display_dataGridView);
        }

        private void SetUpCamComponentsMeasurement()
        {
            UpCamera.BuildMeasurementFunctionsList(UpCamComponents_dataGridView);
        }

        private void UpCamComponentsMeasure_button_Click(object sender, EventArgs e)
        {
            if (UpCamera.IsRunning())
            {
                SetUpCamComponentsMeasurement();
                DebugComponents_Camera(
                    10.0 / Properties.Settings.Default.UpCam_XmmPerPixel,
                    UpCamera,
                    Properties.Settings.Default.UpCam_XmmPerPixel);
            }
            else
            {
                DisplayText("Up camera is not running.");
            }
        }


        #endregion  Measurementboxes

        // ==========================================================================================================
        // Video processing functions lists control
        // ==========================================================================================================
        #region VideoProcessingFunctionsLists

        private void UpdateDisplayFunctions()
        {
            if (DownCamera.IsRunning())
            {

                DownCamera.BuildDisplayFunctionsList(Display_dataGridView);
            }
            if (UpCamera.IsRunning())
            {

                UpCamera.BuildDisplayFunctionsList(Display_dataGridView);
            }
        }

        // ==========================================================================================================
        // Add, delete and move rows:

        private void AddCamFunction_button_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection SelectedRows = Display_dataGridView.SelectedRows;
            int index = 0;
            if (Display_dataGridView.Rows.Count == 0)
            {
                // Grid is empty:
                Display_dataGridView.Rows.Insert(0);
            }
            else
            {
                // insert at end
                Display_dataGridView.Rows.Insert(Display_dataGridView.Rows.Count);
                index = Display_dataGridView.Rows.Count - 1;
            };
            Display_dataGridView.Rows[index].Cells[1].Value = "false";	// This will be tested later, and can't be null
        }

        private void DeleteCamFunction_button_Click(object sender, EventArgs e)
        {
            int DoomedRow = Display_dataGridView.CurrentCell.RowIndex;
            Display_dataGridView.ClearSelection();
            Display_dataGridView.Rows.RemoveAt(DoomedRow);
            UpdateDisplayFunctions();
        }

        private void CamFunctionUp_button_Click(object sender, EventArgs e)
        {
            DataGrid_Up_button(Display_dataGridView);
        }

        private void CamFunctionDown_button_Click(object sender, EventArgs e)
        {
            DataGrid_Down_button(Display_dataGridView);
        }

        private void CamFunctionsClear_button_Click(object sender, EventArgs e)
        {
            Display_dataGridView.Rows.Clear();
            UpdateDisplayFunctions();
            ClearEditTargets();
        }

        // ==========================================================================================================
        // Display_dataGridView functions:
        enum Display_dataGridViewColumns { Function, Active, Int, Double, R, G, B };
        // NOTE: a verbatim copy is at camera.cs. If you edit this, edit camera.cs as well!

        private void Display_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            int FunctionCol = (int)Display_dataGridViewColumns.Function;
            int ActiveCol = (int)Display_dataGridViewColumns.Active;
            if ((Display_dataGridView.CurrentCell.ColumnIndex != FunctionCol)
                &&
                (Display_dataGridView.CurrentCell.ColumnIndex != ActiveCol))
            {
                return;  // parameter changes hadled by the parameter edit widgets
            };

            // Return if not dirty; otherwise the stuff is executed twice (once when it changes, once when it becomes clean)
            if (!Display_dataGridView.IsCurrentCellDirty)
            {
                return;
            }
            if (SetupCamerasPageVisible)
            {
                // react immediately to changes
                Update_GridView(Display_dataGridView);
                if (Display_dataGridView.CurrentRow.Cells[ActiveCol].Value.ToString() == "True")
                {
                    SetEditTargets();
                }
                else
                {
                    ClearEditTargets();
                }
                UpdateDisplayFunctions();
            }
        }

        private void Display_dataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (Display_dataGridView.CurrentCell != null)
            {
                SetEditTargets();
            }
        }

        // ==========================================================================================================
        // Parameter labels and control widgets:
        // ==========================================================================================================
        // Sharing the labels and some controls so I don't need to duplicate so much code:

        private void CamerasSetUp_tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Move labels and value setting widgets to current page
            TabPage Page = DownCamera_tabPage;
            switch (CamerasSetUp_tabControl.SelectedTab.Name)
            {
                case "DownCamera_tabPage":
                    Page = DownCamera_tabPage;
                    break;
                case "UpCamera_tabPage":
                    Page = UpCamera_tabPage;
                    break;
            }
            Display_dataGridView.Parent = Page;
            Parameter_int_label.Parent = Page;
            Parameter_Int_numericUpDown.Parent = Page;
            Parameter_double_label.Parent = Page;
            Parameter_double_textBox.Parent = Page;
            AddCamFunction_button.Parent = Page;
            DeleteCamFunction_button.Parent = Page;
            CamFunctionUp_button.Parent = Page;
            CamFunctionDown_button.Parent = Page;
            CamFunctionsClear_button.Parent = Page;
            Color_label.Parent = Page;
            Color_Box.Parent = Page;
            R_label.Parent = Page;
            R_numericUpDown.Parent = Page;
            G_label.Parent = Page;
            G_numericUpDown.Parent = Page;
            B_label.Parent = Page;
            B_numericUpDown.Parent = Page;
            ColorHelp_label.Parent = Page;
            RobustFast_checkBox.Parent = Page;
            KeepActive_checkBox.Parent = Page;
        }

        private void ClearEditTargets()
        {
            Parameter_int_label.Text = "--";
            Parameter_Int_numericUpDown.Enabled = false;
            Parameter_Int_numericUpDown.Value = 0;
            Parameter_double_label.Text = "--";
            Parameter_double_textBox.Enabled = false;
            Color_label.Text = "--";
            Color_Box.Enabled = false;
            R_label.Text = "--";
            R_numericUpDown.Enabled = false;
            G_label.Text = "--";
            G_numericUpDown.Enabled = false;
            B_label.Text = "--";
            B_numericUpDown.Enabled = false;
            ColorHelp_label.Visible = false;
        }

        private void ClearParameterValuesExcept(int row, int keep)
        {
            for (int i = 2; i < Display_dataGridView.Columns.Count; i++)
            {
                if (i != keep)
                {
                    Display_dataGridView.Rows[row].Cells[i].Value = null;
                }
            }
        }

        private void DoRGBparameters(int row)
        {
            Parameter_int_label.Text = "Range";
            Parameter_Int_numericUpDown.Enabled = true;
            Parameter_double_textBox.Enabled = false;
            int DoubleCol = (int)Display_dataGridViewColumns.Double;
            Display_dataGridView.Rows[row].Cells[DoubleCol].Value = null;
            R_label.Text = "R";
            R_numericUpDown.Enabled = true;
            G_label.Text = "G";
            G_numericUpDown.Enabled = true;
            B_label.Text = "B";
            B_numericUpDown.Enabled = true;
            ColorHelp_label.Visible = true;

            int par_i;
            int IntCol = (int)Display_dataGridViewColumns.Int;
            if (Display_dataGridView.Rows[row].Cells[IntCol].Value == null)
            {
                Parameter_Int_numericUpDown.Value = 10;
                Display_dataGridView.Rows[row].Cells[IntCol].Value = "10";
            }
            else if (!int.TryParse(Display_dataGridView.Rows[row].Cells[IntCol].Value.ToString(), out par_i))
            {
                Parameter_Int_numericUpDown.Value = 10;
                Display_dataGridView.Rows[row].Cells[IntCol].Value = "10";
            }
            else
            {
                Parameter_Int_numericUpDown.Value = par_i;
            }

            SetColorBoxColor(row);


        }

        private void SetGridRowColor(int row)
        {
            // Didn't find good color definition from dataviewgrid, so we'll copy the box color to grid.
            int R_col = (int)Display_dataGridViewColumns.R;
            int G_col = (int)Display_dataGridViewColumns.G;
            int B_col = (int)Display_dataGridViewColumns.B;

            Color BoxColor = Color_Box.BackColor;
            Display_dataGridView.Rows[row].Cells[R_col].Value = BoxColor.R;
            Display_dataGridView.Rows[row].Cells[G_col].Value = BoxColor.G;
            Display_dataGridView.Rows[row].Cells[B_col].Value = BoxColor.B;
        }

        private void SetColorBoxColor(int row)
        {
            byte R = 128;
            byte G = 128;
            byte B = 128;
            byte temp;
            int R_col = (int)Display_dataGridViewColumns.R;
            int G_col = (int)Display_dataGridViewColumns.G;
            int B_col = (int)Display_dataGridViewColumns.B;

            if (Display_dataGridView.Rows[row].Cells[R_col].Value == null)
            {
                SetGridRowColor(row);
                return;
            }
            else
            {
                if (byte.TryParse(Display_dataGridView.Rows[row].Cells[R_col].Value.ToString(), out temp))
                {
                    R = temp;
                }
                else
                {
                    SetGridRowColor(row);
                    return;
                }
            };

            if (Display_dataGridView.Rows[row].Cells[G_col].Value == null)
            {
                SetGridRowColor(row);
                return;
            }
            else
            {
                if (byte.TryParse(Display_dataGridView.Rows[row].Cells[G_col].Value.ToString(), out temp))
                {
                    G = temp;
                }
                else
                {
                    SetGridRowColor(row);
                    return;
                }
            };

            if (Display_dataGridView.Rows[row].Cells[B_col].Value == null)
            {
                SetGridRowColor(row);
                return;
            }
            else
            {
                if (byte.TryParse(Display_dataGridView.Rows[row].Cells[B_col].Value.ToString(), out temp))
                {
                    B = temp;
                }
                else
                {
                    SetGridRowColor(row);
                    return;
                }
            };
            // have not returned, so set box color:
            Color_Box.BackColor = Color.FromArgb(R, G, B);
        }

        private void SetProcessingFunctions(DataGridView Grid)
        {
            // To add a video processing fuction:
            // Add the name to here.
            // Add the name to Camera.cs, BuildFunctionsList()
            // Add the actual function (take example of any function already referred from BuildFunctionsList()

            DataGridViewComboBoxColumn comboboxColumn =
                (DataGridViewComboBoxColumn)Grid.Columns[(int)Display_dataGridViewColumns.Function];
            comboboxColumn.Items.Clear();
            comboboxColumn.Items.AddRange("Threshold", "Histogram", "Grayscale", "Invert", "Edge detect",
                "Noise reduction", "Kill color", "Keep color", "Meas. zoom");
        }

        private void SetEditTargets()
        {
            int row = Display_dataGridView.CurrentCell.RowIndex;
            int col = Display_dataGridView.CurrentCell.ColumnIndex;
            int FunctCol = (int)Display_dataGridViewColumns.Function;
            int ActiveCol = (int)Display_dataGridViewColumns.Active;
            int par_i;
            int IntCol = (int)Display_dataGridViewColumns.Int;
            double par_d;
            int DoubleCol = (int)Display_dataGridViewColumns.Double;

            ClearEditTargets();
            if (Display_dataGridView.Rows[row].Cells[FunctCol].Value == null)
            {
                return;
            };
            if (Display_dataGridView.Rows[row].Cells[ActiveCol].Value == null)
            {
                return;
            };
            if (Display_dataGridView.Rows[row].Cells[ActiveCol].Value.ToString() == "False")
            {
                return;
            };
            switch (Display_dataGridView.Rows[row].Cells[FunctCol].Value.ToString())
            {
                // switch by the selected algorithm: 
                case "Histogram":
                    ClearParameterValuesExcept(row, -1);
                    return;		// no parameters

                case "Grayscale":
                    ClearParameterValuesExcept(row, -1);
                    return;		// no parameters

                case "Invert":
                    ClearParameterValuesExcept(row, -1);
                    return;		// no parameters

                case "Edge detect":
                    ClearParameterValuesExcept(row, 2);
                    if (Display_dataGridView.Rows[row].Cells[IntCol].Value == null)
                    {
                        Parameter_Int_numericUpDown.Value = 1;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "1";
                    }
                    else if (!int.TryParse(Display_dataGridView.Rows[row].Cells[2].Value.ToString(), out par_i))
                    {
                        Parameter_Int_numericUpDown.Value = 1;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "1";
                    }
                    else
                    {
                        Parameter_Int_numericUpDown.Value = par_i;
                    }
                    Parameter_Int_numericUpDown.Enabled = true;
                    Parameter_int_label.Text = "Type (1..4)";
                    UpdateDisplayFunctions();
                    return;		// no parameters

                case "Noise reduction":
                    ClearParameterValuesExcept(row, 2);
                    if (Display_dataGridView.Rows[row].Cells[IntCol].Value == null)
                    {
                        Parameter_Int_numericUpDown.Value = 1;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "1";
                    }
                    else if (!int.TryParse(Display_dataGridView.Rows[row].Cells[2].Value.ToString(), out par_i))
                    {
                        Parameter_Int_numericUpDown.Value = 1;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "1";
                    }
                    else
                    {
                        Parameter_Int_numericUpDown.Value = par_i;
                    }
                    Parameter_Int_numericUpDown.Enabled = true;
                    Parameter_int_label.Text = "Type (1..3)";
                    UpdateDisplayFunctions();
                    return;		// no parameters

                case "Kill color":
                    DoRGBparameters(row);
                    Color_label.Text = "Color to kill:";
                    UpdateDisplayFunctions();
                    return;

                case "Keep color":
                    Color_label.Text = "Color to keep:";
                    DoRGBparameters(row);
                    UpdateDisplayFunctions();
                    return;

                case "Meas. zoom":
                    // one double parameter
                    ClearParameterValuesExcept(row, 3);
                    if (Display_dataGridView.Rows[row].Cells[DoubleCol].Value == null)
                    {
                        Parameter_double_textBox.Text = "2.0";
                        Display_dataGridView.Rows[row].Cells[DoubleCol].Value = "2.0";
                    }
                    else if (!double.TryParse(Display_dataGridView.Rows[row].Cells[3].Value.ToString(), out par_d))
                    {
                        Parameter_double_textBox.Text = "2.0";
                        Display_dataGridView.Rows[row].Cells[DoubleCol].Value = "2.0";
                    }
                    else
                    {
                        Parameter_double_textBox.Text = par_d.ToString("0.0");
                    }
                    Parameter_double_textBox.Enabled = true;
                    Parameter_double_label.Text = "Zoom factor";
                    UpdateDisplayFunctions();
                    break;

                case "Threshold":
                    // one int parameter
                    ClearParameterValuesExcept(row, 2);

                    if (Display_dataGridView.Rows[row].Cells[IntCol].Value == null)
                    {
                        Parameter_Int_numericUpDown.Value = 100;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "100";
                    }
                    else if (!int.TryParse(Display_dataGridView.Rows[row].Cells[2].Value.ToString(), out par_i))
                    {
                        Parameter_Int_numericUpDown.Value = 100;
                        Display_dataGridView.Rows[row].Cells[IntCol].Value = "100";
                    }
                    else
                    {
                        Parameter_Int_numericUpDown.Value = par_i;
                    }
                    Parameter_Int_numericUpDown.Enabled = true;
                    Parameter_int_label.Text = "Threshold";
                    UpdateDisplayFunctions();
                    break;

                default:
                    return;
                // break;
            }
        }

        private void Parameter_Int_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!Parameter_Int_numericUpDown.Enabled)
            {
                return;		// so that it can be cleared safely
            }

            int row = Display_dataGridView.CurrentCell.RowIndex;
            int IntCol = (int)Display_dataGridViewColumns.Int;
            Display_dataGridView.Rows[row].Cells[IntCol].Value = Parameter_Int_numericUpDown.Value.ToString();
            UpdateDisplayFunctions();
        }

        private void Parameter_double_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            int DoubleCol = (int)Display_dataGridViewColumns.Double;
            int row = Display_dataGridView.CurrentCell.RowIndex;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Parameter_double_textBox.Text, out val))
                {
                    Display_dataGridView.Rows[row].Cells[DoubleCol].Value = Parameter_double_textBox.Text;
                    UpdateDisplayFunctions();
                }
            }

        }

        private void Parameter_double_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            int DoubleCol = (int)Display_dataGridViewColumns.Double;
            int row = Display_dataGridView.CurrentCell.RowIndex;
            if (double.TryParse(Parameter_double_textBox.Text, out val))
            {
                Display_dataGridView.Rows[row].Cells[DoubleCol].Value = Parameter_double_textBox.Text;
                UpdateDisplayFunctions();
            }

        }

        private void R_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!R_numericUpDown.Enabled)
            {
                return;
            }
            int row = Display_dataGridView.CurrentCell.RowIndex;
            int R_col = (int)Display_dataGridViewColumns.R;
            Display_dataGridView.Rows[row].Cells[R_col].Value = R_numericUpDown.Value.ToString();
            SetColorBoxColor(row);
            UpdateDisplayFunctions();
        }

        private void G_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!G_numericUpDown.Enabled)
            {
                return;
            }
            int row = Display_dataGridView.CurrentCell.RowIndex;
            int G_col = (int)Display_dataGridViewColumns.G;
            Display_dataGridView.Rows[row].Cells[G_col].Value = G_numericUpDown.Value.ToString();
            SetColorBoxColor(row);
            UpdateDisplayFunctions();
        }

        private void B_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!B_numericUpDown.Enabled)
            {
                return;
            }
            int row = Display_dataGridView.CurrentCell.RowIndex;
            int B_col = (int)Display_dataGridViewColumns.B;
            Display_dataGridView.Rows[row].Cells[B_col].Value = B_numericUpDown.Value.ToString();
            SetColorBoxColor(row);
            UpdateDisplayFunctions();
        }


        private void PickColor(int X, int Y)
        {
            if (X < 2)
                X = 2;
            if (X > (Cam_pictureBox.Width - 2))
                X = Cam_pictureBox.Width - 2;
            if (Y < 2)
                Y = 2;
            if (Y > (Cam_pictureBox.Height - 2))
                X = Cam_pictureBox.Height - 2;

            int Rsum = 0;
            int Gsum = 0;
            int Bsum = 0;
            byte R = 0;
            byte G = 0;
            byte B = 0;
            Color pixelColor;
            int deb = 0;
            Bitmap img = (Bitmap)Cam_pictureBox.Image;
            for (int ix = X - 2; ix <= X + 2; ix++)
            {
                for (int iy = Y - 2; iy <= Y + 2; iy++)
                {
                    pixelColor = img.GetPixel(ix, iy);
                    Rsum += pixelColor.R;
                    Gsum += pixelColor.G;
                    Bsum += pixelColor.B;
                    deb++;
                }
            }
            R = (byte)(Rsum / 25);
            G = (byte)(Gsum / 25);
            B = (byte)(Bsum / 25);
            R_numericUpDown.Value = R;
            G_numericUpDown.Value = G;
            B_numericUpDown.Value = B;
            Color_Box.BackColor = Color.FromArgb(R, G, B);
            img.Dispose();
        }

        private void Display_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            DisplayText("PaperTape_dataGridView values:");
            string msg = "";
            foreach (DataGridViewRow Row in PaperTape_dataGridView.Rows)
            {
                for (int i = 0; i < Row.Cells.Count; i++)
                {
                    if (Row.Cells[i].Value == null)
                    {
                        msg += "null:";
                    }
                    else
                    {
                        msg += Row.Cells[i].Value.ToString() + ":";
                    }
                }
                DisplayText(msg);
            }
            DisplayText("Display_dataGridView values:");
            msg = "";
            foreach (DataGridViewRow Row in Display_dataGridView.Rows)
            {
                for (int i = 0; i < Row.Cells.Count; i++)
                {
                    if (Row.Cells[i].Value == null)
                    {
                        msg += "null:";
                    }
                    else
                    {
                        msg += Row.Cells[i].Value.ToString() + ":";
                    }
                }
                DisplayText("msg");
            }

        }
        #endregion

        private void UpCamOverlay_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpCamOverlay_checkBox.Checked)
            {
                UpCamera.Draw_Snapshot = true;
            }
            else
            {
                UpCamera.Draw_Snapshot = false;
            }
        }

        private void Tapes_pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            General_pictureBox_MouseClick(Tapes_pictureBox, e.X, e.Y);
        }

        private void Placement_pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            General_pictureBox_MouseMove(Tapes_pictureBox, e.X, e.Y);
        }

        private void SlackCompensationA_checkBox_Click(object sender, EventArgs e)
        {
            if (SlackCompensationA_checkBox.Checked)
            {
                Cnc.SlackCompensationA = true;
                Properties.Settings.Default.CNC_SlackCompensationA = true;
            }
            else
            {
                Cnc.SlackCompensationA = false;
                Properties.Settings.Default.CNC_SlackCompensationA = false;
            }
        }

        private void Z0_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            Z0_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Z0_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_ZtoPCB = val;
                    Z0_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void BackOff_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            BackOff_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(BackOff_textBox.Text, out val))
                {
                    Properties.Settings.Default.General_ProbingBackOff = val;
                    BackOff_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void PlacementDepth_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            PlacementDepth_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(PlacementDepth_textBox.Text, out val))
                {
                    Properties.Settings.Default.Placement_Depth = val;
                    PlacementDepth_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void OmitNeedleCalibration_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Placement_OmitNeedleCalibration = OmitNeedleCalibration_checkBox.Checked;

        }

        private void SkipMeasurements_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Placement_SkipMeasurements = SkipMeasurements_checkBox.Checked;
        }

        private void KeepActive_checkBox_Click(object sender, EventArgs e)
        {
            // We handle checked state ourselves to avoid automatic call to StartCameras at startup
            // (Changing Checked activatges CheckedChanged event
            if (KeepActive_checkBox.Checked)
            {
                // KeepActive_checkBox.Checked = true;
                RobustFast_checkBox.Enabled = false;
                StartCameras();
            }
            else
            {
                // KeepActive_checkBox.Checked = false;
                RobustFast_checkBox.Enabled = true;
            }
            Properties.Settings.Default.Cameras_KeepActive = KeepActive_checkBox.Checked;
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void Goto_button_Click(object sender, EventArgs e)
        {
            double X;
            double Y;
            double Z;
            double A;
            if (!double.TryParse(GotoX_textBox.Text, out X))
            {
                return;
            }
            if (!double.TryParse(GotoY_textBox.Text, out Y))
            {
                return;
            }
            if (!double.TryParse(GotoZ_textBox.Text, out Z))
            {
                return;
            }
            if (!double.TryParse(GotoA_textBox.Text, out A))
            {
                return;
            }
            if (Math.Abs(Z) < 0.01)  // allow raising Z and move at one go
            {
                if (!CNC_Z_m(Z))
                {
                    return;
                }
            };
            // Move X, Y, A if needed
            if (!((Math.Abs(X - Cnc.CurrentX) < 0.01) && (Math.Abs(Y - Cnc.CurrentY) < 0.01) && (Math.Abs(A - Cnc.CurrentA) < 0.01)))
            {
                // Allow raise Z, goto and low Z:
                if (!(Math.Abs(Z) < 0.01))
                {
                    if (!CNC_Z_m(0))
                    {
                        return;
                    }
                }
                if (!CNC_XYA_m(X, Y, A))
                {
                    return;
                }
                if (!(Math.Abs(Z - Cnc.CurrentZ) < 0.01))
                {
                    if (!CNC_Z_m(Z))
                    {
                        return;
                    }
                };
            }
            // Move Z if needed
            if (!(Math.Abs(Z - Cnc.CurrentZ) < 0.01))
            {
                if (!CNC_Z_m(Z))
                {
                    return;
                }
            }
        }

        // fix #22 calculate new next coordinates if column was changed
        private void Tapes_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (StartingUp || LoadingDataGrid)
            {
                return;
            }

            // only update next coordinates if corresponding column has been changed and rowIndex > 0
            if (e.ColumnIndex != 6 || e.RowIndex < 0)
            {
                return;
            }

            int NextNo = 1;

            if (!int.TryParse(Tapes_dataGridView.Rows[e.RowIndex].Cells["Next_Column"].Value.ToString(), out NextNo))
            {
                ShowMessageBox(
                    "Bad data in Next",
                    "Data error",
                    MessageBoxButtons.OK);
                return;
            }

            if (Tapes != null)
            {
                Tapes.UpdateNextCoordinates(e.RowIndex, NextNo);
            }

        }

        private void DisableLog_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DisableLog_checkBox.Checked)
            {
                DisplayText("** Logging disabled **", KnownColor.Black, true);
            }
        }

    }	// end of: 	public partial class FormMain : Form

    // allows additionl of color info to displayText 
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            if (color != box.ForeColor)
            {
                box.SelectionStart = box.TextLength;
                box.SelectionLength = 0;
                box.SelectionColor = color;
                box.AppendText(text);
                box.SelectionColor = box.ForeColor;
            }
            else
            {
                box.AppendText(text);
            }
        }
    }


}	// end of: namespace LitePlacer
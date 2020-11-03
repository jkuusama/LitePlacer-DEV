//#define TRANSFER

/* 

 Using the standard method (Settings.settings file, Setting.xxx) was a mistake:
 No easy recovery, impossible to move installation from one machine to another, to name a few.
 This class resolves the issues. See
 http://stackoverflow.com/questions/453161/best-practice-to-save-application-settings-in-a-windows-forms-application,
 Trevor's answer (second answer from top). Also, to get a nice formatting (each parameter on its own line), 
 Newtonsoft.Json library is used. http://www.newtonsoft.com/json

 If you uncomment the first line, you get a version that transfers the settings from old version to new.

 */



using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Configuration;
using System;

namespace LitePlacer
{
    // =================================================================================
    // 
    // =================================================================================
    public abstract class AppSettingsBase
    {
        public abstract int SettingsVersion { get; set; }

        protected abstract AppSettingsBase Read(string jsonString);
        /* convert from previous versions like this:
        {
            AppSettingsBase settingsBase = JsonConvert.DeserializeObject<AppSettingsBase>(jsonString);
            switch (settingsBase?.SettingsVersion)
            {
                default:
                    return JsonConvert.DeserializeObject<AppSettingsV2>(jsonString);
            }
        }
        */
    }

    public class AppSettings : AppSettingsBase
    {
#pragma warning disable CA1031 // Do not catch general exception types (see MainForm.cs beginning)

        static private FormMain MainForm;

        public override int SettingsVersion { get; set; } = 0;
        public bool General_SafeFilesAtClosing { get; set; } = true;

        protected override AppSettingsBase Read(string jsonString)
        {
            return JsonConvert.DeserializeObject<AppSettings>(jsonString);
        }

        public static void InitDisplayText(FormMain MainF)
        {
            MainForm = MainF;
        }

        public bool Save(string FileName)
        {
            try
            {
                MainForm?.DisplayText("Writing " + FileName);
                File.WriteAllText(FileName, JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (Exception excep)
            {
                MainForm?.DisplayText("Application settings save failed: " + excep.Message);
                return false;
            }
        }

        public AppSettings Load(string FileName)
        {
            try
            {

#if TRANSFER
                TransferSettings(FileName);  // does not return
                // in case it does:
                MainForm.ShowMessageBox(
                   "TransferSettings(FileName) returned!\n" +
                   "Please report this issue to juha@kuusama.com.",
                   "Sloppy progrfammer error",
                   MessageBoxButtons.OK);
                Application.Exit();
#endif

                if (File.Exists(FileName))
                {
                    MainForm?.DisplayText("Reading " + FileName);
                    AppSettings settings = (AppSettings)Read(File.ReadAllText(FileName));

                    // JsonConvert.DeserializeObject can return null if the setings file is corrupt,
                    // catch this here and inform the operator before we cause a NullReferenceException in Form1_Load.
                    if (settings == null)
                    {
                        throw new Exception($"Couldn't load {FileName}. File exists but is corrupt.");
                    }
                    return settings;
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show(
                       "SAVED SETTINGS FILE NOT FOUND. \n\n" +
                       "If this is the first time you are running this program, this is expected. " +
                       "Click OK, startup continues and the program uses built-in default values.\n\n" +
                       "If you are updating from an earlier version, click cancel now, " +
                       "then run the settings transfer program." +
                       "Please see https://www.liteplacer.com/downloads/ for instructions and download link.",
                       "Settings file not found", MessageBoxButtons.OKCancel);
                    if (dialogResult == DialogResult.Cancel)
                    {
                        Environment.Exit(0);
                    }
                    return this;
                }
            }
            catch (Exception excep)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "Problem loading application settings:\n" + excep.Message +
                    " \n\rExit program without saving any data? \n\r" +
                   "If \"Yes\", you lose changes since last start.\n\r" +
                   "If \"No\", continue with default settings. Your old settings will be\n\r" +
                   "overwritten at program end.",
                   "Data save problem", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return this;
                }
                General_SafeFilesAtClosing = false;
                Application.Exit();
                return this;    // to avoid compile error
            }
        }

#if TRANSFER

        private void CopySettings(string FileName)
        {
            try
            {
                Properties.Settings.Default.Upgrade();
                MySettings s = new MySettings();
                s.Cameras_KeepActive = Properties.Settings.Default.Cameras_KeepActive;
                s.Cameras_RobustSwitch = Properties.Settings.Default.Cameras_RobustSwitch;
                s.CNC_AltJogSpeed = Properties.Settings.Default.CNC_AltJogSpeed;
                s.CNC_AspeedMax = Properties.Settings.Default.CNC_AspeedMax;
                s.CNC_CtlrJogSpeed = Properties.Settings.Default.CNC_CtlrJogSpeed;
                s.CNC_EnableMouseWheelJog = Properties.Settings.Default.CNC_EnableMouseWheelJog;
                s.CNC_EnableNumPadJog = Properties.Settings.Default.CNC_EnableNumPadJog;
                s.CNC_NormalJogSpeed = Properties.Settings.Default.CNC_NormalJogSpeed;
                s.CNC_SerialPort = Properties.Settings.Default.CNC_SerialPort;
                s.CNC_SlackCompensation = Properties.Settings.Default.CNC_SlackCompensation;
                s.CNC_SlackCompensationA = Properties.Settings.Default.CNC_SlackCompensationA;
                s.CNC_SmallMovementSpeed = Properties.Settings.Default.CNC_SmallMovementSpeed;
                s.CNC_SquareCorrection = Properties.Settings.Default.CNC_SquareCorrection;
                s.CNC_ZspeedMax = Properties.Settings.Default.CNC_ZspeedMax;
                s.DownCam_DrawTicks = Properties.Settings.Default.DownCam_DrawTicks;
                s.DownCam_index = Properties.Settings.Default.DownCam_index;
                s.Downcam_InvertedHomeMark = Properties.Settings.Default.Downcam_InvertedHomeMark;
                s.DownCam_MeasurementDelay = Properties.Settings.Default.DownCam_MeasurementDelay;
                s.DownCam_NozzleOffsetX = Properties.Settings.Default.DownCam_NozzleOffsetX;
                s.DownCam_NozzleOffsetY = Properties.Settings.Default.DownCam_NozzleOffsetY;
                s.DownCam_SnapshotColor = Properties.Settings.Default.DownCam_SnapshotColor;
                s.DownCam_XmmPerPixel = Properties.Settings.Default.DownCam_XmmPerPixel;
                s.DownCam_YmmPerPixel = Properties.Settings.Default.DownCam_YmmPerPixel;
                s.DownCam_Zoom = Properties.Settings.Default.DownCam_Zoom;
                s.DownCam_Zoomfactor = Properties.Settings.Default.DownCam_Zoomfactor;
                s.DowncamMoniker = Properties.Settings.Default.DowncamMoniker;
                s.General_BelowPCB_Allowance = Properties.Settings.Default.General_BelowPCB_Allowance;
                s.General_CheckForUpdates = Properties.Settings.Default.General_CheckForUpdates;
                s.General_JigOffsetX = Properties.Settings.Default.General_JigOffsetX;
                s.General_JigOffsetY = Properties.Settings.Default.General_JigOffsetY;
                s.General_MachineSizeX = Properties.Settings.Default.General_MachineSizeX;
                s.General_MachineSizeY = Properties.Settings.Default.General_MachineSizeY;
                s.General_Mark1Name = Properties.Settings.Default.General_Mark1Name;
                s.General_Mark1X = Properties.Settings.Default.General_Mark1X;
                s.General_Mark1Y = Properties.Settings.Default.General_Mark1Y;
                s.General_Mark2Name = Properties.Settings.Default.General_Mark2Name;
                s.General_Mark2X = Properties.Settings.Default.General_Mark2X;
                s.General_Mark2Y = Properties.Settings.Default.General_Mark2Y;
                s.General_Mark3Name = Properties.Settings.Default.General_Mark3Name;
                s.General_Mark3X = Properties.Settings.Default.General_Mark3X;
                s.General_Mark3Y = Properties.Settings.Default.General_Mark3Y;
                s.General_Mark4Name = Properties.Settings.Default.General_Mark4Name;
                s.General_Mark4X = Properties.Settings.Default.General_Mark4X;
                s.General_Mark4Y = Properties.Settings.Default.General_Mark4Y;
                s.General_Mark5Name = Properties.Settings.Default.General_Mark5Name;
                s.General_Mark5X = Properties.Settings.Default.General_Mark5X;
                s.General_Mark5Y = Properties.Settings.Default.General_Mark5Y;
                s.General_Mark6Name = Properties.Settings.Default.General_Mark6Name;
                s.General_Mark6X = Properties.Settings.Default.General_Mark6X;
                s.General_Mark6Y = Properties.Settings.Default.General_Mark6Y;
                s.General_MediumSpeed = Properties.Settings.Default.General_MediumSpeed;
                s.General_MuteLogging = Properties.Settings.Default.General_MuteLogging;
                s.General_ParkX = Properties.Settings.Default.General_ParkX;
                s.General_ParkY = Properties.Settings.Default.General_ParkY;
                s.General_PickupCenterX = Properties.Settings.Default.General_PickupCenterX;
                s.General_PickupCenterY = Properties.Settings.Default.General_PickupCenterY;
                s.General_PickupReleaseTime = Properties.Settings.Default.General_PickupReleaseTime;
                s.General_PickupVacuumTime = Properties.Settings.Default.General_PickupVacuumTime;
                s.General_PlacementBackOff = Properties.Settings.Default.General_PlacementBackOff;
                s.General_ShadeGuard_mm = Properties.Settings.Default.General_ShadeGuard_mm;
                s.General_UpgradeRequired = Properties.Settings.Default.General_UpgradeRequired;
                s.General_ZprobingHysteresis = Properties.Settings.Default.General_ZprobingHysteresis;
                s.General_ZTestTravel = Properties.Settings.Default.General_ZTestTravel;
                s.General_ZtoPCB = Properties.Settings.Default.General_ZtoPCB;
                s.Job_Xoffset = Properties.Settings.Default.Job_Xoffset;
                s.Job_Yoffset = Properties.Settings.Default.Job_Yoffset;
                s.Nozzles_AfullSpeed = Properties.Settings.Default.Nozzles_AfullSpeed;
                s.Nozzles_Aspeed = Properties.Settings.Default.Nozzles_Aspeed;
                s.Nozzles_CalibrationDistance = Properties.Settings.Default.Nozzles_CalibrationDistance;
                s.Nozzles_CalibrationMaxSize = Properties.Settings.Default.Nozzles_CalibrationMaxSize;
                s.Nozzles_CalibrationMinSize = Properties.Settings.Default.Nozzles_CalibrationMinSize;
                s.Nozzles_count = Properties.Settings.Default.Nozzles_count;
                s.Nozzles_current = Properties.Settings.Default.Nozzles_current;
                s.Nozzles_default = Properties.Settings.Default.Nozzles_default;
                s.Nozzles_Enabled = Properties.Settings.Default.Nozzles_Enabled;
                s.Nozzles_FirstMoveFullSpeed = Properties.Settings.Default.Nozzles_FirstMoveFullSpeed;
                s.Nozzles_FirstMoveSlackCompensation = Properties.Settings.Default.Nozzles_FirstMoveSlackCompensation;
                s.Nozzles_LastMoveFullSpeed = Properties.Settings.Default.Nozzles_LastMoveFullSpeed;
                s.Nozzles_maximum = Properties.Settings.Default.Nozzles_maximum;
                s.Nozzles_Timeout = Properties.Settings.Default.Nozzles_Timeout;
                s.Nozzles_XYfullSpeed = Properties.Settings.Default.Nozzles_XYfullSpeed;
                s.Nozzles_XYspeed = Properties.Settings.Default.Nozzles_XYspeed;
                s.Nozzles_ZfullSpeed = Properties.Settings.Default.Nozzles_ZfullSpeed;
                s.Nozzles_Zspeed = Properties.Settings.Default.Nozzles_Zspeed;
                s.Panel_UseBoardFids = Properties.Settings.Default.Panel_UseBoardFids;
                s.Panel_XFirstOffset = Properties.Settings.Default.Panel_XFirstOffset;
                s.Panel_XIncrement = Properties.Settings.Default.Panel_XIncrement;
                s.Panel_XRepeats = Properties.Settings.Default.Panel_XRepeats;
                s.Panel_YFirstOffset = Properties.Settings.Default.Panel_YFirstOffset;
                s.Panel_YIncrement = Properties.Settings.Default.Panel_YIncrement;
                s.Panel_YRepeats = Properties.Settings.Default.Panel_YRepeats;
                s.Placement_Depth = Properties.Settings.Default.Placement_Depth;
                s.Placement_FiducialConfirmation = Properties.Settings.Default.Placement_FiducialConfirmation;
                s.Placement_FiducialsType = Properties.Settings.Default.Placement_FiducialsType;
                s.Placement_FiducialTolerance = Properties.Settings.Default.Placement_FiducialTolerance;
                s.Placement_OmitNozzleCalibration = Properties.Settings.Default.Placement_OmitNozzleCalibration;
                s.Placement_SkipMeasurements = Properties.Settings.Default.Placement_SkipMeasurements;
                s.Placement_UpdateJobGridAtRuntime = Properties.Settings.Default.Placement_UpdateJobGridAtRuntime;
                s.UpCam_index = Properties.Settings.Default.UpCam_index;
                s.UpCam_MeasurementDelay = Properties.Settings.Default.UpCam_MeasurementDelay;
                s.UpCam_PositionX = Properties.Settings.Default.UpCam_PositionX;
                s.UpCam_PositionY = Properties.Settings.Default.UpCam_PositionY;
                s.UpCam_SnapshotColor = Properties.Settings.Default.UpCam_SnapshotColor;
                s.UpCam_XmmPerPixel = Properties.Settings.Default.UpCam_XmmPerPixel;
                s.UpCam_YmmPerPixel = Properties.Settings.Default.UpCam_YmmPerPixel;
                s.UpCam_Zoom = Properties.Settings.Default.UpCam_Zoom;
                s.UpCam_Zoomfactor = Properties.Settings.Default.UpCam_Zoomfactor;
                s.UpcamMoniker = Properties.Settings.Default.UpcamMoniker;

                if (Save(s, FileName))
                {
                    MainForm.ShowMessageBox(
                        "New format settings file created.\n" +
                        "You can now update the software.",
                        "Settings saved",
                        MessageBoxButtons.OK);
                }
            }
            catch (System.Exception excep)
            {
                MainForm.ShowMessageBox(
                    "Problem saving the new format file:\n" + excep.Message + "\n",
                    "Problem in save",
                    MessageBoxButtons.OK);
            }
            Environment.Exit(0);
        }

        private void TransferSettings(string FileName)
        {
            // This routine gets settings from old version and creates the file

            DialogResult dialogResult = MessageBox.Show(
                "This version of the LitePlacer software only transfers application settings from old format to new.\n" +
                "For more information, please see link in /downloads page." + "Continue?",
                "Copy settings from old version to new version",
                MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.No)
            {
                Environment.Exit(0);
                // Application.Exit();
            }

            if (File.Exists(FileName))
            {
                MainForm.ShowMessageBox(
                   "New format application settings file found, nothing done.\n" +
                   "You can now update the software.",
                   "Settings already transferred",
                   MessageBoxButtons.OK);
                Environment.Exit(0);
            }
            else
            {
                CopySettings(FileName);
            }
        }
#endif

    }
}

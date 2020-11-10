using Newtonsoft.Json;
using System.Collections.Generic;

namespace LitePlacer
{
    // =================================================================================
    // The actual settings
    // =================================================================================

    public partial class AppSettingsV3 : AppSettings
    {
        override public AppSettingsBase Read(string jsonString)
        {
            //read baseversion so version is zero if there is no version saved in the file
            AppSettings settingsBase = new AppSettings();
            settingsBase = (AppSettings)settingsBase.Read(jsonString);

            //create default settings
            AppSettingsV3 settingsV3 = new AppSettingsV3();

            if (settingsBase.SettingsVersion >= 3)
            {
                //read all available propertys
                settingsV3 = JsonConvert.DeserializeObject<AppSettingsV3>(jsonString);
                if (settingsV3 == null)
                    settingsV3 = new AppSettingsV3();

                //JsonConvert keeps default List elements. If there are at least 2 additional cameras
                //delete the two default ones
                if (settingsV3.Cameras_Settings.Count > 2)
                    settingsV3.Cameras_Settings.RemoveAt(0);
                if (settingsV3.Cameras_Settings.Count > 2)
                    settingsV3.Cameras_Settings.RemoveAt(0);
            }
            else if (settingsBase.SettingsVersion < 3)
            {
                //let previous version handle load as far as it goes
                //newer version should only call previous Version and let it convert recursively from oldest to newest
                AppSettingsV2 settingsV2 = new AppSettingsV2();
                settingsV2 = (AppSettingsV2)settingsV2.Read(jsonString);

                //convert to json and load all available [converted] data
                string tmpJson = JsonConvert.SerializeObject(settingsV2);
                settingsV3 = JsonConvert.DeserializeObject<AppSettingsV3>(tmpJson);
                settingsV3.SettingsVersion = 3;

                //convert old data from previous Version
                settingsV3.Cameras_Settings[0].DesiredX = settingsV2.DownCam_DesiredX;
                settingsV3.Cameras_Settings[0].DesiredY = settingsV2.DownCam_DesiredY;
                settingsV3.Cameras_Settings[0].DrawBox = settingsV2.DownCam_DrawBox;
                settingsV3.Cameras_Settings[0].DrawCross = settingsV2.DownCam_DrawCross;
                settingsV3.Cameras_Settings[0].DrawGrid = settingsV2.DownCam_DrawGrid;
                settingsV3.Cameras_Settings[0].DrawSidemarks = settingsV2.DownCam_DrawSidemarks;
                settingsV3.Cameras_Settings[0].DrawTicks = settingsV2.DownCam_DrawTicks;
                settingsV3.Cameras_Settings[0].MirrorX = settingsV2.DownCam_MirrorX;
                settingsV3.Cameras_Settings[0].MirrorY = settingsV2.DownCam_MirrorY;
                settingsV3.Cameras_Settings[0].Moniker = settingsV2.DowncamMoniker;
                settingsV3.Cameras_Settings[0].Name = settingsV2.Downcam_Name;
                settingsV3.Cameras_Settings[0].OnHead = settingsV2.DownCam_OnHead;
                settingsV3.Cameras_Settings[0].SnapshotColor = settingsV2.DownCam_SnapshotColor;
                settingsV3.Cameras_Settings[0].XmmPerPixel = settingsV2.DownCam_XmmPerPixel;
                settingsV3.Cameras_Settings[0].YmmPerPixel = settingsV2.DownCam_YmmPerPixel;
                settingsV3.Cameras_Settings[0].Zoom = settingsV2.DownCam_Zoom;
                settingsV3.Cameras_Settings[0].ZoomFactor = settingsV2.DownCam_Zoomfactor;

                settingsV3.Cameras_Settings[1].DesiredX = settingsV2.UpCam_DesiredX;
                settingsV3.Cameras_Settings[1].DesiredY = settingsV2.UpCam_DesiredY;
                settingsV3.Cameras_Settings[1].DrawBox = settingsV2.UpCam_DrawBox;
                settingsV3.Cameras_Settings[1].DrawCross = settingsV2.UpCam_DrawCross;
                settingsV3.Cameras_Settings[1].DrawGrid = settingsV2.UpCam_DrawGrid;
                settingsV3.Cameras_Settings[1].DrawSidemarks = settingsV2.UpCam_DrawSidemarks;
                settingsV3.Cameras_Settings[1].DrawTicks = settingsV2.UpCam_DrawTicks;
                settingsV3.Cameras_Settings[1].MirrorX = settingsV2.UpCam_MirrorX;
                settingsV3.Cameras_Settings[1].MirrorY = settingsV2.UpCam_MirrorY;
                settingsV3.Cameras_Settings[1].Moniker = settingsV2.UpcamMoniker;
                settingsV3.Cameras_Settings[1].Name = settingsV2.Upcam_Name;
                settingsV3.Cameras_Settings[1].OnHead = settingsV2.UpCam_OnHead;
                settingsV3.Cameras_Settings[1].SnapshotColor = settingsV2.UpCam_SnapshotColor;
                settingsV3.Cameras_Settings[1].XmmPerPixel = settingsV2.UpCam_XmmPerPixel;
                settingsV3.Cameras_Settings[1].YmmPerPixel = settingsV2.UpCam_YmmPerPixel;
                settingsV3.Cameras_Settings[1].Zoom = settingsV2.UpCam_Zoom;
                settingsV3.Cameras_Settings[1].ZoomFactor = settingsV2.UpCam_Zoomfactor;
            }
            return settingsV3;
        }

        public class CameraSettings
        {
            public bool DrawBox { get; set; } = true;           // Draws a box on the image that is used for scale setting
            public bool DrawCross { get; set; } = true;         // If crosshair cursor is drawn
            public bool DrawGrid { get; set; } = false;         // Draws aiming grid for parts alignment
            public bool DrawSidemarks { get; set; } = true;     // If marks on the side of the image are drawn
            public bool DrawTicks { get; set; } = true;
            public bool MirrorX { get; set; } = false;
            public bool MirrorY { get; set; } = false;
            public System.Drawing.Color SnapshotColor { get; set; } = System.Drawing.Color.White;
            public double XmmPerPixel { get; set; } = 0.1;
            public double YmmPerPixel { get; set; } = 0.1;
            public bool Zoom { get; set; } = false;              // If image is zoomed or not
            public double ZoomFactor { get; set; } = 1.5;        // If it is, this much
            public string Moniker { get; set; } = "unconnected";
            public string Name { get; set; } = "";
            public int DesiredX { get; set; } = 1280;
            public int DesiredY { get; set; } = 1024;
            public bool OnHead { get; set; } = true;
        }

        public override int SettingsVersion { get; set; } = 3;
        public bool Cam_ShowPixels { get; set; } = false;
        public bool Cameras_KeepActive { get; set; } = false;
        public bool Cameras_RobustSwitch { get; set; } = false;

        public int CNC_AltJogSpeed { get; set; } = 4000;
        public int CNC_AspeedMax { get; set; } = 4000;
        public int CNC_CtlrJogSpeed { get; set; } = 4000;
        public bool CNC_EnableMouseWheelJog { get; set; } = true;
        public bool CNC_EnableNumPadJog { get; set; } = true;
        public int CNC_NormalJogSpeed { get; set; } = 1000;
        public string CNC_SerialPort { get; set; } = "";
        public bool CNC_SlackCompensation { get; set; } = false;
        public double SlackCompensationDistance { get; set; } = 0.4;
        public bool CNC_SlackCompensationA { get; set; } = false;
        public decimal CNC_SmallMovementSpeed { get; set; } = 150;
        public double CNC_SquareCorrection { get; set; } = 0;
        public int CNC_ZspeedMax { get; set; } = 1000;
        public double CNC_RegularMoveTimeout { get; set; } = 10.0;

        public double General_BelowPCB_Allowance { get; set; } = 3;
        public bool General_CheckForUpdates { get; set; } = false;
        public double General_JigOffsetX { get; set; } = 10;
        public double General_JigOffsetY { get; set; } = 10;
        public double General_MachineSizeX { get; set; } = 590;
        public double General_MachineSizeY { get; set; } = 370;
        public string General_Mark1Name { get; set; } = "Mark1";
        public double General_Mark1X { get; set; } = 0;
        public double General_Mark1Y { get; set; } = 0;
        public double General_Mark1A { get; set; } = 0;
        public string General_Mark2Name { get; set; } = "Mark2";
        public double General_Mark2X { get; set; } = 0;
        public double General_Mark2Y { get; set; } = 0;
        public double General_Mark2A { get; set; } = 0;
        public string General_Mark3Name { get; set; } = "Mark3";
        public double General_Mark3X { get; set; } = 0;
        public double General_Mark3Y { get; set; } = 0;
        public double General_Mark3A { get; set; } = 0;
        public string General_Mark4Name { get; set; } = "Mark4";
        public double General_Mark4X { get; set; } = 0;
        public double General_Mark4Y { get; set; } = 0;
        public double General_Mark4A { get; set; } = 0;
        public string General_Mark5Name { get; set; } = "Mark5";
        public double General_Mark5X { get; set; } = 0;
        public double General_Mark5Y { get; set; } = 0;
        public double General_Mark5A { get; set; } = 0;
        public string General_Mark6Name { get; set; } = "Mark6";
        public double General_Mark6X { get; set; } = 0;
        public double General_Mark6Y { get; set; } = 0;
        public double General_Mark6A { get; set; } = 0;
        public string General_MediumSpeed { get; set; } = "50000";
        public bool General_MuteLogging { get; set; } = false;
        public double General_ParkX { get; set; } = 0;
        public double General_ParkY { get; set; } = 0;
        public double General_PickupCenterX { get; set; } = 0;
        public double General_PickupCenterY { get; set; } = 0;
        public int General_PickupReleaseTime { get; set; } = 150;
        public int General_PickupVacuumTime { get; set; } = 250;
        public double General_PlacementBackOff { get; set; } = 0.2;
        public double General_ShadeGuard_mm { get; set; } = 0;
        public bool General_UpgradeRequired { get; set; } = true;
        public double General_ZprobingHysteresis { get; set; } = 0.2;
        public double General_ZTestTravel { get; set; } = 20;
        public double General_ZtoPCB { get; set; } = 0;
        public double Job_Xoffset { get; set; } = 0;
        public double Job_Yoffset { get; set; } = 0;
        public bool General_VigorousHoming { get; set; } = false;
        public bool General_PumpOutputInverted { get; set; } = false;
        public bool General_VacuumOutputInverted { get; set; } = false;

        public bool Nozzles_AfullSpeed { get; set; } = true;
        public double Nozzles_Aspeed { get; set; } = 500;
        public double Nozzles_CalibrationDistance { get; set; } = 4;
        public double Nozzles_CalibrationMaxSize { get; set; } = 2;
        public double Nozzles_CalibrationMinSize { get; set; } = 0;
        public int Nozzles_count { get; set; } = 6;
        public int Nozzles_current { get; set; } = 1;
        public int Nozzles_default { get; set; } = 1;
        public bool Nozzles_Enabled { get; set; } = false;
        public bool Nozzles_FirstMoveFullSpeed { get; set; } = true;
        public bool Nozzles_FirstMoveSlackCompensation { get; set; } = true;
        public bool Nozzles_LastMoveFullSpeed { get; set; } = false;
        public int Nozzles_maximum { get; set; } = 10;
        public int Nozzles_Timeout { get; set; } = 5000;
        public bool Nozzles_XYfullSpeed { get; set; } = false;
        public double Nozzles_XYspeed { get; set; } = 500;
        public bool Nozzles_ZfullSpeed { get; set; } = false;
        public double Nozzles_Zspeed { get; set; } = 500;
        public double Nozzles_WarningTreshold { get; set; } = 1.0;

        public bool Panel_UseBoardFids { get; set; } = true;
        public double Panel_XFirstOffset { get; set; } = 0;
        public double Panel_XIncrement { get; set; } = 0;
        public int Panel_XRepeats { get; set; } = 0;
        public double Panel_YFirstOffset { get; set; } = 0;
        public double Panel_YIncrement { get; set; } = 0;
        public int Panel_YRepeats { get; set; } = 0;

        public double Placement_Depth { get; set; } = 1;
        public bool Placement_FiducialConfirmation { get; set; } = false;
        public int Placement_FiducialsType { get; set; } = 0;
        public bool Placement_OmitNozzleCalibration { get; set; } = false;
        public bool Placement_SkipMeasurements { get; set; } = false;
        public bool Placement_UpdateJobGridAtRuntime { get; set; } = false;

        public double DownCam_NozzleOffsetX { get; set; } = 84;
        public double DownCam_NozzleOffsetY { get; set; } = 27;
        public double UpCam_PositionX { get; set; } = 2;
        public double UpCam_PositionY { get; set; } = 3;

        public List<CameraSettings> Cameras_Settings { get; } = new List<CameraSettings>() {
            new CameraSettings() {
                 DrawBox  = true,
         DrawCross  = true,
        DrawGrid  = false,
         DrawSidemarks  = true,
         DrawTicks  = true,
         MirrorX  = false,
         MirrorY  = false,
         SnapshotColor  = System.Drawing.Color.White,
         XmmPerPixel  = 0.1,
         YmmPerPixel  = 0.1,
         Zoom  = false,
         ZoomFactor  = 1.5,
         Moniker  = "unconnected",
         Name  = "",
         DesiredX  = 1280,
         DesiredY  = 1024,
         OnHead  = true
    },
            new CameraSettings() {
                 DrawBox  = true,
         DrawCross  = true,
        DrawGrid  = false,
         DrawSidemarks  = true,
         DrawTicks  = true,
         MirrorX  = false,
         MirrorY  = false,
         SnapshotColor  = System.Drawing.Color.White,
         XmmPerPixel  = 0.05,
         YmmPerPixel  = 0.05,
         Zoom  = false,
         ZoomFactor  = 1.5,
         Moniker  = "unconnected",
         Name  = "",
         DesiredX  = 1280,
         DesiredY  = 1024,
         OnHead  = false
    }
        };
    }

    // =================================================================================
    // Board Settings
    // =================================================================================

    public partial class AppSettingsV3
    {
        public double Duet3_Xspeed { get; set; } = 200; // mm/s
        public double Duet3_Yspeed { get; set; } = 200; // mm/s
        public double Duet3_Zspeed { get; set; } = 50; // mm/s
        public double Duet3_Aspeed { get; set; } = 200; // mm/s

        public double Duet3_Xacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Yacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Zacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Aacc { get; set; } = 1000; // mm/s^2

        public double Duet3_XTravelPerRev { get; set; } = 40;   // mm
        public double Duet3_YTravelPerRev { get; set; } = 40;   // mm
        public double Duet3_ZTravelPerRev { get; set; } = 8;    // mm
        public double Duet3_ATravelPerRev { get; set; } = 160;  // deg

        public double Duet3_XDegPerStep { get; set; } = 0.9;   // mm
        public double Duet3_YDegPerStep { get; set; } = 0.9;   // mm
        public double Duet3_ZDegPerStep { get; set; } = 1.8;   // mm
        public double Duet3_ADegPerStep { get; set; } = 0.9;   // mm

        public int Duet3_XMicroStep { get; set; } = 16;
        public int Duet3_YMicroStep { get; set; } = 16;
        public int Duet3_ZMicroStep { get; set; } = 16;
        public int Duet3_AMicroStep { get; set; } = 16;

        public bool Duet3_XInterpolate { get; set; } = true;
        public bool Duet3_YInterpolate { get; set; } = true;
        public bool Duet3_ZInterpolate { get; set; } = true;
        public bool Duet3_AInterpolate { get; set; } = true;

        public int Duet3_XCurrent { get; set; } = 1100; //mA
        public int Duet3_YCurrent { get; set; } = 1200; //mA
        public int Duet3_ZCurrent { get; set; } = 1300; //mA
        public int Duet3_ACurrent { get; set; } = 400; //mA
    }
}

using System.Windows.Forms;

namespace LitePlacer
{
    public partial class FormMain
    {
        private void InitCameras()
        {
            // Called at startup. 
            foreach (Camera camera in Cameras)
            {
                camera.Close();
            }
            SetDownCameraDefaults();
            SetUpCameraDefaults();
            if (Setting.Cameras_KeepActive)
            {
                foreach (Camera camera in Cameras)
                {
                    camera.Start();
                }
            }
            SelectedCam = DownCamera;
        }

        private void SelectCamera(Camera cam)
        {
            if (cam.Settings.Moniker == AppSettingsV3.CameraSettings.DefaultString)
            {
                DisplayText("Selecting, no camera");
                return;
            }

            cam.ClearDisplayFunctionsList();
            cam.BuildDisplayFunctionsList(VideoAlgorithms?.CurrentAlgorithm);

            CamCross_CheckBox.DataBindings.Clear();
            CamCross_CheckBox.DataBindings.Add("Checked", cam.Settings, nameof(cam.Settings.DrawCross), false, DataSourceUpdateMode.OnPropertyChanged);
            CamGrid_CheckBox.DataBindings.Clear();
            CamGrid_CheckBox.DataBindings.Add("Checked", cam.Settings, nameof(cam.Settings.DrawGrid), false, DataSourceUpdateMode.OnPropertyChanged);
            CamZoom_CheckBox.DataBindings.Clear();
            CamZoom_CheckBox.DataBindings.Add("Checked", cam.Settings, nameof(cam.Settings.Zoom), false, DataSourceUpdateMode.OnPropertyChanged);
            CamZoomFactor_numericUpDown.DataBindings.Clear();
            CamZoomFactor_numericUpDown.DataBindings.Add("Value", cam.Settings, nameof(cam.Settings.ZoomFactor), false, DataSourceUpdateMode.OnPropertyChanged);
            CamZoomFactor_numericUpDown.DataBindings.Add("Enabled", cam.Settings, nameof(cam.Settings.Zoom), false, DataSourceUpdateMode.OnPropertyChanged);
            CamPauseDisplay_checkBox.DataBindings.Clear();
            CamPauseDisplay_checkBox.DataBindings.Add("Checked", cam, nameof(cam.PauseDisplay), false, DataSourceUpdateMode.OnPropertyChanged);
            CamFind_CheckBox.DataBindings.Clear();
            CamFind_CheckBox.DataBindings.Add("Checked", cam, nameof(cam.FindObjects), false, DataSourceUpdateMode.OnPropertyChanged);
            CamOverlay_trackBar.DataBindings.Clear();
            CamOverlay_trackBar.DataBindings.Add("Value", cam, nameof(cam.OverlayRatio), false, DataSourceUpdateMode.OnPropertyChanged);

            if (!Setting.Cameras_KeepActive)
            {
                foreach (Camera camera in Cameras)
                {
                    if (Setting.Cameras_RobustSwitch || camera != cam)
                        camera.Close();
                }
            }

            cam.Start();

            foreach (Camera camera in Cameras)
            {
                camera.Displayed = camera == cam;
            }
        }

        private void PauseCameras()
        {
            foreach (Camera camera in Cameras)
            {
                camera.Displayed = false;
            }
        }

        private void ResumeCameras()
        {
            foreach (Camera camera in Cameras)
            {
                camera.Displayed = camera == SelectedCam;
            }
        }

        private void SetDownCameraDefaults()
        {
            DownCamera.ImageBox = Cam_pictureBox;
            DownCamera.BoxSizeX = 200;
            DownCamera.BoxSizeY = 200;
            DownCamera.BoxRotationDeg = 0;
            DownCamera.ClearDisplayFunctionsList();
            // Draws
            /*
            DownCamera.DrawCross = true;
            DownCamera.DrawDashedCross = false;
            DownCamera.DrawGrid = false;
            DownCamera.Draw_Snapshot = false;
            */
            // Finds:
            DownCamera.FindCircles = false;
            DownCamera.FindRectangles = false;
            DownCamera.FindComponent = false;
            DownCamera.TestAlgorithm = false;
            DownCamera.Settings.DrawBox = false;
            DownCamera.DrawArrow = false;

            DownCamera.SideMarksX = Setting.General_MachineSizeX / 100;
            DownCamera.SideMarksY = Setting.General_MachineSizeY / 100;
        }

        // ====
        private void SetUpCameraDefaults()
        {
            UpCamera.ImageBox = Cam_pictureBox;
            UpCamera.BoxSizeX = 200;
            UpCamera.BoxSizeY = 200;
            UpCamera.BoxRotationDeg = 0;
            UpCamera.ClearDisplayFunctionsList();
            // Draws
            UpCamera.Settings.DrawCross = true;
            UpCamera.Settings.DrawGrid = false;
            UpCamera.DrawDashedCross = false;
            UpCamera.Draw_Snapshot = false;
            // Finds:
            UpCamera.FindCircles = false;
            UpCamera.FindRectangles = false;
            UpCamera.FindComponent = false;
            UpCamera.TestAlgorithm = false;
            UpCamera.Settings.DrawBox = false;
            UpCamera.DrawArrow = false;

            UpCamera.SideMarksX = Setting.General_MachineSizeX / 100;
            UpCamera.SideMarksY = Setting.General_MachineSizeY / 100;
        }
    }
}
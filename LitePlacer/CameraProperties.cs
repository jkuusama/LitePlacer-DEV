using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;


namespace LitePlacer
{
    public partial class CameraProperties : Form
    {
        // links to main program
        Camera Cam;
        public FormMain MainForm { get; set; }

        // Values, as they were when entering this dialog
        int ExposureDefault;
        CameraControlFlags ExposureDefaultFlags;
        int IrisDefault;
        CameraControlFlags IrisDefaultFlags;
        int FocusDefault;
        CameraControlFlags FocusDefaultFlags;

        bool DefaultValuesRead = false; // If the above are valid
        bool ValuesChanged = false;     // If they are changed by the user


        public CameraProperties(FormMain main, Camera _cam)
        {
            InitializeComponent();
            MainForm = main;
            Cam = _cam;
        }


        private bool CheckCameraStatus()
        {
            if (!Cam.IsRunning())
            {
                MainForm.DisplayText("CameraProperties, CheckCameraStatus: cam not running");
                EnableControls(false);
                return false;
            }
            else
            {
                MainForm.DisplayText("CameraProperties, CheckCameraStatus: updating values");
                EnableControls(true);
                if (!DefaultValuesRead)
                {
                    ReadDefaultValues();
                }
                return true;
            }
        }


        private void CameraProperties_Shown(object sender, EventArgs e)
        {
            MainForm.DisplayText("CameraProperties: loading form");

            if (!CheckCameraStatus())
            {
                return;
            }
            else
            {
                EnableControls(true);
                ReadDefaultValues();
            }
        }


        private void EnableControls(bool val)
        {
            Exposure_trackBar.Enabled = val;
            ExposureAuto_checkBox.Enabled = val;
            Focus_trackBar.Enabled = val;
            FocusAuto_checkBox.Enabled = val;
            Iris_trackBar.Enabled = val;
            IrisAuto_checkBox.Enabled = val;
            if (!val)
            {
                Status_label.Text = (Cam.Name + " not running.");
            }
            else
            {
                if (Cam.Name == "DownCamera")
                {
                    Status_label.Text = ("DownCamera is running");
                    Resolution_label.Text = MainForm.DownCamUsedResolution_label.Text;
                    Fps_label.Text = MainForm.DownCameraFps_label.Text;
                }
                else
                {
                    Status_label.Text = ("UpCamera is running");
                    Resolution_label.Text = MainForm.UpCamUsedResolution_label.Text;
                    Fps_label.Text = MainForm.UpCameraFps_label.Text;
                }
            }
        }


        private void ReadDefaultValues()
        {
            if (!Cam.IsRunning())
            {
                MainForm.DisplayText("CameraProperties, ReadCameraValues: cam not running");
                EnableControls(false);
                return;
            }

            int val;
            int minValue;
            int maxValue;
            int stepSize;
            int defaultValue;
            CameraControlFlags flags;

            Cam.VideoSource.GetCameraPropertyRange(CameraControlProperty.Exposure, out minValue,
                out maxValue, out stepSize, out defaultValue, out flags);
            Exposure_trackBar.Minimum = minValue;
            Exposure_trackBar.Maximum = maxValue;
            Exposure_trackBar.SmallChange = stepSize;

            Cam.VideoSource.GetCameraProperty(CameraControlProperty.Exposure, out val, out flags);
            Exposure_trackBar.Value = val;
            ExposureValue_label.Text = val.ToString();
            ExposureDefault = val;
            ExposureDefaultFlags = flags;
            if (flags == CameraControlFlags.None)
            {
                ExposureAuto_checkBox.Enabled = false;
                Exposure_trackBar.Enabled = true;
                MainForm.DisplayText("Exposure: " + val.ToString() + ", range " + minValue.ToString() +
                    " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: None");
            }
            else
            {
                ExposureAuto_checkBox.Enabled = true;
                if (flags == CameraControlFlags.Auto)
                {
                    ExposureAuto_checkBox.Checked = true;
                    Exposure_trackBar.Enabled = false;
                    MainForm.DisplayText("Exposure: " + val.ToString() + ", range " + minValue.ToString() +
                       " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Auto");
                }
                else
                {
                    ExposureAuto_checkBox.Checked = false;
                    Exposure_trackBar.Enabled = true;
                    MainForm.DisplayText("Exposure: " + val.ToString() + ", range " + minValue.ToString() +
                        " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Manual");
                }
            }

            Cam.VideoSource.GetCameraPropertyRange(CameraControlProperty.Focus, out minValue,
                out maxValue, out stepSize, out defaultValue, out flags);
            Focus_trackBar.Minimum = minValue;
            Focus_trackBar.Maximum = maxValue;
            Focus_trackBar.SmallChange = stepSize;

            Cam.VideoSource.GetCameraProperty(CameraControlProperty.Focus, out val, out flags);
            Focus_trackBar.Value = val;
            FocusValue_label.Text = val.ToString();
            FocusDefault = val;
            FocusDefaultFlags = flags;
            if (flags == CameraControlFlags.None)
            {
                FocusAuto_checkBox.Enabled = false;
                Focus_trackBar.Enabled = true;
                MainForm.DisplayText("Focus: " + val.ToString() + ", range " + minValue.ToString() +
                    " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: None");
            }
            else
            {
                FocusAuto_checkBox.Enabled = true;
                if (flags == CameraControlFlags.Auto)
                {
                    FocusAuto_checkBox.Checked = true;
                    Focus_trackBar.Enabled = false;
                    MainForm.DisplayText("Focus: " + val.ToString() + ", range " + minValue.ToString() +
                        " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Auto");
                }
                else
                {
                    FocusAuto_checkBox.Checked = false;
                    Focus_trackBar.Enabled = true;
                    MainForm.DisplayText("Focus: " + val.ToString() + ", range " + minValue.ToString() +
                        " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Manual");
                }
            }

            Cam.VideoSource.GetCameraPropertyRange(CameraControlProperty.Iris, out minValue,
                out maxValue, out stepSize, out defaultValue, out flags);
            Iris_trackBar.Minimum = minValue;
            Iris_trackBar.Maximum = maxValue;
            Iris_trackBar.SmallChange = stepSize;

            Cam.VideoSource.GetCameraProperty(CameraControlProperty.Iris, out val, out flags);
            Iris_trackBar.Value = val;
            IrisValue_label.Text = val.ToString();
            IrisDefault = val;
            IrisDefaultFlags = flags;
            if (flags == CameraControlFlags.None)
            {
                IrisAuto_checkBox.Enabled = false;
                Iris_trackBar.Enabled = true;
                MainForm.DisplayText("Iris: " + val.ToString() + ", range " + minValue.ToString() +
                    " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: None");
            }
            else
            {
                IrisAuto_checkBox.Enabled = true;
                if (flags == CameraControlFlags.Auto)
                {
                    IrisAuto_checkBox.Checked = true;
                    Iris_trackBar.Enabled = false;
                    MainForm.DisplayText("Iris: " + val.ToString() + ", range " + minValue.ToString() +
                        " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Auto");
                }
                else
                {
                    IrisAuto_checkBox.Checked = false;
                    Iris_trackBar.Enabled = true;
                    MainForm.DisplayText("Iris: " + val.ToString() + ", range " + minValue.ToString() +
                        " - " + maxValue.ToString() + ", step " + stepSize.ToString() + ", flags: Manual");
                }
            }

            DefaultValuesRead = true;
        }


        private void RestoreValues()
        {
            MainForm.DisplayText("Camera Properties, Restore values");
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Exposure, (int)ExposureDefault, ExposureDefaultFlags);
            MainForm.DisplayText("Exposure: "+ ExposureDefault.ToString());
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Focus, (int)FocusDefault, FocusDefaultFlags);
            MainForm.DisplayText("Focus: " + FocusDefault.ToString());
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Iris, (int)IrisDefault, IrisDefaultFlags);
            MainForm.DisplayText("Iris: " + IrisDefault.ToString());
        }

        // ============== Buttons ===========================

        private void CheckStatus_button_Click(object sender, EventArgs e)
        {
            MainForm.DisplayText("CameraProperties: CheckStatus_button");
            CheckCameraStatus();
        }


        private void OK_button_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void Cancel_button_Click(object sender, EventArgs e)
        {
            if (ValuesChanged && DefaultValuesRead)
            {
                RestoreValues();
            }
            Close();
        }


        // ===================== Exposure =========================================

        private void Exposure_trackBar_Scroll(object sender, EventArgs e)
        {
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Exposure, Exposure_trackBar.Value, CameraControlFlags.Manual);
            ValuesChanged = true;
            ExposureValue_label.Text = Exposure_trackBar.Value.ToString();
        }


        private void ExposureAuto_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int val = Exposure_trackBar.Value;
            if (ExposureAuto_checkBox.Checked)
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Exposure, val, CameraControlFlags.Auto);
                Exposure_trackBar.Enabled = false;
            }
            else
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Exposure, val, CameraControlFlags.Manual);
                Exposure_trackBar.Enabled = true;
            }
            ValuesChanged = true;
        }

        private void Iris_trackBar_Scroll(object sender, EventArgs e)
        {
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Iris, Iris_trackBar.Value, CameraControlFlags.Manual);
            ValuesChanged = true;
            IrisValue_label.Text = Iris_trackBar.Value.ToString();
        }

        private void IrisAuto_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int val = Iris_trackBar.Value;
            if (IrisAuto_checkBox.Checked)
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Iris, val, CameraControlFlags.Auto);
                Iris_trackBar.Enabled = false;
            }
            else
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Iris, val, CameraControlFlags.Manual);
                Iris_trackBar.Enabled = true;
            }
            ValuesChanged = true;
        }

        private void Focus_trackBar_Scroll(object sender, EventArgs e)
        {
            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Focus, Focus_trackBar.Value, CameraControlFlags.Manual);
            ValuesChanged = true;
            FocusValue_label.Text = Focus_trackBar.Value.ToString();
        }

        private void FocusAuto_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int val = Focus_trackBar.Value;
            if (FocusAuto_checkBox.Checked)
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Focus, val, CameraControlFlags.Auto);
                Focus_trackBar.Enabled = false;
            }
            else
            {
                Cam.VideoSource.SetCameraProperty(CameraControlProperty.Focus, val, CameraControlFlags.Manual);
                Focus_trackBar.Enabled = true;
            }
            ValuesChanged = true;
        }
    }
}

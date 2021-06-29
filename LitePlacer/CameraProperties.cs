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
        Camera Cam;
        public FormMain MainForm { get; set; }

        public CameraProperties(FormMain main, Camera _cam)
        {
            InitializeComponent();
            MainForm = main;
            Cam = _cam;
        }

        bool DefaultValuesRead = false;

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
                ReadCameraValues();
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
                ReadCameraValues();
            }
        }


        private void EnableControls(bool val)
        {
            Exposure_trackBar.Enabled = val;
            ExposureAuto_checkBox.Enabled = val;
            ApplyExposure_button.Enabled = val;
            if (!val)
            {
                Status_label.Text = (Cam.Name + " not running.");
            }
            else
            {
                if (Cam.Name == "DownCamera")
                {
                    Status_label.Text = ("DownCamera is running, " + MainForm.DownCamUsedResolution_label.Text +
                    ", " + MainForm.DownCameraFps_label.Text) ;
                }
                else
                {
                    Status_label.Text = ("DownCamera is running, " + MainForm.DownCamUsedResolution_label.Text +
                    ", " + MainForm.DownCameraFps_label.Text);
                }
            }
        }


        private void ReadCameraValues()
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
            if (flags == CameraControlFlags.None)
            {
                ExposureAuto_checkBox.Enabled = false;
            }
            else
            {
                ExposureAuto_checkBox.Enabled = true;
                if (flags == CameraControlFlags.Auto)
                {
                    ExposureAuto_checkBox.Checked = true;
                }
                else
                {
                    ExposureAuto_checkBox.Checked = false;
                }
            }

        }



        // ============== Buttons ===========================

        private void CheckStatus_button_Click(object sender, EventArgs e)
        {
            MainForm.DisplayText("CameraProperties: CheckStatus_button");
            CheckCameraStatus();
        }


        private void ApplyExposure_button_Click(object sender, EventArgs e)
        {
            if (!DefaultValuesRead)
            {
                MainForm.DisplayText("CameraProperties, ApplyExposure_button: DefaultValues not read");
                return;
            }

            CameraControlFlags flags;
            if (!ExposureAuto_checkBox.Enabled)
            {
                flags = CameraControlFlags.None;
            }
            else
            {
                if (ExposureAuto_checkBox.Checked)
                {
                    flags = CameraControlFlags.Auto;
                }
                else
                {
                    flags = CameraControlFlags.Manual;
                }
            }
            int val = Exposure_trackBar.Value;

            Cam.VideoSource.SetCameraProperty(CameraControlProperty.Exposure, val, flags);
        }




    }
}

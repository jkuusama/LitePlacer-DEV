using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using System.Drawing;
using System.Drawing.Imaging;



namespace LitePlacer
{
	public partial class FormMain : Form
	{
        const int NoOfImages = 10;
        Bitmap[] Images = new Bitmap[NoOfImages];
        string[] ImageFilenames = new string[NoOfImages];

        static int ImageNumber = 0;

        private void InitStoredImages()
        {
            for (int i = 0; i < NoOfImages; i++)
            {
                Images[i] = null;
                ImageFilenames[i] = "no image";
            }
            LeftArrowImage_button.Visible = false;
            ImageNumber_label.Text = "1";
            StoredImageFilename_label.Text = "1: no image";
        }


        private void UseStoredImage()
        {
            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
            }
            if (UseStoredImage_checkBox.Checked)
            {
                if (Images[ImageNumber] == null)
                {
                    DisplayText("No image loaded at image position " + ImageNumber.ToString());
                    return;
                }
                if (Cam.PauseProcessing || !Cam.IsRunning())
                {
                    DisplayText("Camera is not active");
                    return;
                }
                Cam.Paused = false;
                Cam.PauseProcessing = true;
                while (!Cam.Paused)
                {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                Cam.ExternalImage = (Bitmap)Images[ImageNumber].Clone();
                Cam.UseExternalImage = true;
                Cam.PauseProcessing = false;
                return;
            }
            else
            {
                Cam.UseExternalImage = false;
            }
        }


        private void UseStoredImage_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            UseStoredImage();
        }


        private void LeftArrowImage_button_Click(object sender, EventArgs e)
        {
            ImageNumber--;
            UpdateImageLabels();
            UseStoredImage();
        }

        private void RightArrowImage_button_Click(object sender, EventArgs e)
        {
            ImageNumber++;
            UpdateImageLabels();
            UseStoredImage();
        }


        private void UpdateImageLabels()
        {
            if (ImageNumber == 0)
            {
                LeftArrowImage_button.Visible = false;
            }
            else
            {
                LeftArrowImage_button.Visible = true;
            }
            if (ImageNumber == 9)
            {
                RightArrowImage_button.Visible = false;
            }
            else
            {
                RightArrowImage_button.Visible = true;
            }
            ImageNumber_label.Text = ImageNumber.ToString();
            StoredImageFilename_label.Text = ImageNumber.ToString() + ": " + ImageFilenames[ImageNumber];

        }


        private void StoredImageMeasureDelay_button_Click(object sender, EventArgs e)
        {
            if (!CheckPositionConfidence()) return;
            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
            }
            if (Cam== DownCamera)
            {
                if (!CNC_XYA_m(0.0, 0.0, Cnc.CurrentA))
                {
                    return;
                }
            }
            else
            {
                if (!CNC_XYA_m(Setting.UpCam_PositionX, Setting.UpCam_PositionY, Cnc.CurrentA))
                {
                    return;
                }
                if (!CNC_Z_m(Setting.General_Z0toPCB - 0.5))
                {
                    return;
                }
            }
            Cam.MeasurementFunctions = null;
            Stopwatch stopwatch = new Stopwatch();
            Bitmap debug;
            stopwatch.Start();
            long[] times = new long[NoOfImages];
            for (int i = 0; i < NoOfImages; i++)
            {
                debug= Cam.GetMeasurementFrame();
                Images[i] = (Bitmap)debug.Clone();
                times[i] = stopwatch.ElapsedMilliseconds;
            }
            stopwatch.Stop();
            for (int i = 0; i < NoOfImages; i++)
            {
                ImageFilenames[i] = "captured frame at " + times[i].ToString() + "ms";
            }
            DisplayText("Done.");
        }



        private void StoredImageSetDelay_button_Click(object sender, EventArgs e)
        {
            if (DownCam_radioButton.Checked)
            {
                DownCamera.MeasurementDelay = ImageNumber;
            }
            else
            {
                UpCamera.MeasurementDelay = ImageNumber;
            }
        }

        private void StoredImageSnapshot_button_Click(object sender, EventArgs e)
        {

        }

        private void StoredImageLoadOne_button_Click(object sender, EventArgs e)
        {

        }

        private void StoredImageLoadAll_button_Click(object sender, EventArgs e)
        {

        }

        private void StoredImageSaveOne_button_Click(object sender, EventArgs e)
        {

        }

        private void StoredImageSaveAll_button_Click(object sender, EventArgs e)
        {

        }

    }
}

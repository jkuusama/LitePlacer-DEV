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
        const int NoOfImages = 2;
        static Bitmap[] Images = new Bitmap[NoOfImages];
        static string[] ImageFilenames = new string[NoOfImages];

        static int ImageNumber = 0;

        private void InitStoredImages()
        {
            for (int i = 0; i < NoOfImages; i++)
            {
                Images[i] = null;
                ImageFilenames[i] = "no image";
            }
            LeftArrowImage_button.Visible = false;
            ImageNumber_label.Text = "0";
            StoredImageFilename_label.Text = "0: no image";
        }


        private void EnterStoredImagetab()
        {
            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
            }
            MeasurementDelay_label.Text = "Current delay: " + Cam.MeasurementDelay.ToString();
            UpdateImageLabels();
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
                bool PauseSave = Cam.PauseProcessing;
                Cam.Paused = false;
                Cam.PauseProcessing = true;
                while (!Cam.Paused)
                {
                    Thread.Sleep(10);
                    Application.DoEvents();
                }
                Bitmap debug = Images[ImageNumber];

                Cam.ExternalImage = debug;
                Cam.UseExternalImage = true;
                Cam.PauseProcessing = PauseSave;
                return;
            }
            else
            {
                Bitmap debug = Images[ImageNumber];
                Cam.Dummy = debug;
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
            Bitmap debug = Images[ImageNumber];
            ImageNumber_label.Text = ImageNumber.ToString();
            StoredImageFilename_label.Text = ImageNumber.ToString() + ": " + ImageFilenames[ImageNumber];

        }


        private void StoredImageMeasureDelay_button_Click(object sender, EventArgs e)
        {
            // if (!CheckPositionConfidence()) return;

            // which camera?
            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
            }

            // initialize the images
            for (int i = 0; i < NoOfImages; i++)
            {
                if (Images[i]!=null)
                {
                    Images[i].Dispose();
                }
                Images[i] = new Bitmap(Cam.FrameSizeX, Cam.FrameSizeY, PixelFormat.Format24bppRgb);
            }

            Cam.MeasurementFunctions = null;
            Stopwatch stopwatch = new Stopwatch();
            long[] times = new long[NoOfImages];

            /*
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
            */

            int DelaySave = Cam.MeasurementDelay;
            Cam.MeasurementDelay = 0;
            stopwatch.Start();
            for (int i = 0; i < NoOfImages; i++)
            {
                Cam.GetMeasurementFrame(ref Images[i]);
                times[i] = stopwatch.ElapsedMilliseconds;
            }
            stopwatch.Stop();
            Cam.MeasurementDelay = DelaySave;

            Bitmap debug;
            for (int i = 0; i < NoOfImages; i++)
            {
                debug = Images[i];
                ImageFilenames[i] = "captured frame at " + times[i].ToString() + "ms";
            }
            DisplayText("Done.");
        }



        private void StoredImageSetDelay_button_Click(object sender, EventArgs e)
        {
            if (DownCam_radioButton.Checked)
            {
                DownCamera.MeasurementDelay = ImageNumber;
                Setting.DownCam_MeasurementDelay = ImageNumber;
                MeasurementDelay_label.Text = "Current delay: " + DownCamera.MeasurementDelay.ToString();
            }
            else
            {
                UpCamera.MeasurementDelay = ImageNumber;
                Setting.UpCam_MeasurementDelay = ImageNumber;
                MeasurementDelay_label.Text = "Current delay: " + UpCamera.MeasurementDelay.ToString();
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

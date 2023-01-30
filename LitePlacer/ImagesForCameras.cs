using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

using System.Drawing;
using System.Drawing.Imaging;



namespace LitePlacer
{
	public partial class FormMain : Form
	{
        const int MaxDelay = 20;
        static Bitmap[] Images = new Bitmap[MaxDelay];
        static string[] ImageFilenames = new string[MaxDelay];

        static int FramesToWait = 0;

        private void InitStoredImages()
        {
            for (int i = 0; i < MaxDelay; i++)
            {
                Images[i] = null;
                ImageFilenames[i] = "no image";
            }
            // LeftArrowImage_button.Visible = false;
            StoredImageFilename_label.Text = "0: no image";
        }


        private void EnterStoredImagetab()
        {
            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
            }
            FramesToWait = Cam.MeasurementDelay;
            ImageNumber_label.Text = Cam.MeasurementDelay.ToString();

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
                if (Images[FramesToWait] == null)
                {
                    DisplayText("No image loaded at image position " + FramesToWait.ToString());
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
                Bitmap debug = Images[FramesToWait];

                Cam.ExternalImage = debug;
                Cam.UseExternalImage = true;
                Cam.PauseProcessing = PauseSave;
                return;
            }
            else
            {
                Bitmap debug = Images[FramesToWait];
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
            if (FramesToWait>0)
            {
            FramesToWait--;
            }
            ImageNumber_label.Text = FramesToWait.ToString();

            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
                Setting.UpCam_MeasurementDelay = FramesToWait;
            }
            else
            {
            Setting.DownCam_MeasurementDelay = FramesToWait;
            }
            Cam.MeasurementDelay = FramesToWait;
        }

        private void RightArrowImage_button_Click(object sender, EventArgs e)
        {
            if (FramesToWait < MaxDelay-1)
            {
                FramesToWait++;
            }
            ImageNumber_label.Text = FramesToWait.ToString();

            Camera Cam = DownCamera;
            if (UpCam_radioButton.Checked)
            {
                Cam = UpCamera;
                Setting.UpCam_MeasurementDelay = FramesToWait;
            }
            else
            {
                Setting.DownCam_MeasurementDelay = FramesToWait;
            }

            Cam.MeasurementDelay = FramesToWait;
        }


        private void UpdateImageLabels()
        {
            if (FramesToWait == 0)
            {
                LeftArrowImage_button.Visible = false;
            }
            else
            {
                LeftArrowImage_button.Visible = true;
            }
            if (FramesToWait == 9)
            {
                RightArrowImage_button.Visible = false;
            }
            else
            {
                RightArrowImage_button.Visible = true;
            }
            Bitmap debug = Images[FramesToWait];
            ImageNumber_label.Text = FramesToWait.ToString();
            StoredImageFilename_label.Text = FramesToWait.ToString() + ": " + ImageFilenames[FramesToWait];

        }

        private bool MeasureOneDelay(out double X, out double Y)
        {
            X = 0.0;
            Y = 0.0;
            Thread.Sleep(200);
            if (!CNC_XYA_m(10.0, 10.0, 0))
            {
                return false;
            }
            Thread.Sleep(800);
            if (!CNC_XYA_m(0.0, 0.0, 0))
            {
                return false;
            }
            if (!DownCamera.Measure(out X, out Y, out double A, false))
            {
                X = -100;   // fake result to indicate failed measurement
            }
            return true;
        }

        private void MeasureCameraDelay_button_Click(object sender, EventArgs e)
        {
            if (!CheckPositionConfidence()) return;
            if (UpCam_radioButton.Checked)
            {
                DisplayText("For now, measurememnt works only with down camera.");
                return;
            }


            VideoAlgorithmsCollection.FullAlgorithmDescription HomeAlg = new VideoAlgorithmsCollection.FullAlgorithmDescription();
            if (!VideoAlgorithms.FindAlgorithm("Homing", out HomeAlg))
            {
                DisplayText("*** Homing algorithm not found - programming error or corrupt data file!", KnownColor.Red, true);
                return;
            }
            DownCamera.BuildMeasurementFunctionsList(HomeAlg.FunctionList);
            DownCamera.MeasurementParameters = HomeAlg.MeasurementParameters;
            if (!CNC_XYA_m(0.0, 0.0, 0))
            {
                return;
            }

            double[] Xs = new double[20];
            double[] Ys = new double[20];
            int DelayStore = DownCamera.MeasurementDelay;
            for (int i = 0; i < MaxDelay-1; i++)
            {
                DownCamera.MeasurementDelay = i;
                if (!MeasureOneDelay(out Xs[i], out Ys[i]))
                {
                    DownCamera.MeasurementDelay = DelayStore;
                    DisplayText("Operation failed.");
                    break;
                }
            }
            DisplayText("Results:");
            DisplayText("Del| X       | Y");

            for (int i = 0; i < MaxDelay - 1; i++)
            {
                if (Xs[i] < -99)
                {
                    DisplayText(String.Format("{0,2}", i) + "  |    --- |    ---");
                }
                else
                {
                    DisplayText(String.Format("{0,2}", i) + " | " + String.Format("{0,7:0.000}", Xs[i])
                        + " | " + String.Format("{0,7:0.000}", Ys[i]));
                }
            }
            DownCamera.MeasurementDelay = DelayStore;


            /*
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
            */
        }



        private void StoredImageSetDelay_button_Click(object sender, EventArgs e)
        {
            if (DownCam_radioButton.Checked)
            {
                DownCamera.MeasurementDelay = FramesToWait;
                Setting.DownCam_MeasurementDelay = FramesToWait;
            }
            else
            {
                UpCamera.MeasurementDelay = FramesToWait;
                Setting.UpCam_MeasurementDelay = FramesToWait;
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

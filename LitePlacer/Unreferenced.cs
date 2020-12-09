using AForge;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

//This File doesnt get compiled

namespace LitePlacer
{
    partial class FormMain
    {
        /*private void ProcessDisplay_checkBox_Checked_Change()
        {
            selectedCam.Overlay = OverlayPictures_checkBox.Checked;
            if (ProcessDisplay_checkBox.Checked)
            {
                UpdateVideoProcessing();
            }
            else
            {
                StopVideoProcessing();
            }
        }*/

        // =================================================================================
        // We need "goto" to different features, currently circles, rectangles or both
        public enum FeatureType { Circle, Rectangle, Both };

        // =================================================================================
        private void Demo_button_Click(object sender, EventArgs e)
        {
            DemoThread = new Thread(() => DemoWork());
            DemoRunning = true;
            CNC_Z_m(0.0);
            DemoThread.IsBackground = true;
            DemoThread.Start();
        }

        // ==============================================================================================
        // =================================================================================
        // Get and save settings from old version if necessary
        // http://blog.johnsworkshop.net/automatically-upgrading-user-settings-after-an-application-version-change/
        /*
        private void Do_Upgrade()
        {
            try
            {
                if (Setting.General_UpgradeRequired)
                {
                    DisplayText("Updating from previous version");
                    Setting.Upgrade();
                    Setting.General_UpgradeRequired = false;
                    Setting.Save();
                }
            }
            catch (SettingsPropertyNotFoundException)
            {
                DisplayText("Updating from previous version (through ex)");
                Setting.Upgrade();
                Setting.General_UpgradeRequired = false;
                Setting.Save();
            }

        }
        */
        // =================================================================================

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
                            if (string.IsNullOrEmpty(dgv.Rows[i].Cells[j].Value.ToString()))
                            {
                                dgv.Rows[i].Cells[j].Value = "--";
                            }
                        }
                    }
                    //bw.Close();
                }
                LoadingDataGrid = false;
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message);
                LoadingDataGrid = false;
            }
        }

        // ==========================================================================================================
        // DownCam:

        // ==========================================================================================================
        // Snapshot:
        /*
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
            double Xscale = Setting.UpCam_XmmPerPixel / Setting.DownCam_XmmPerPixel;
            double Yscale = Setting.UpCam_YmmPerPixel / Setting.DownCam_YmmPerPixel;
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
                Setting.UpCam_SnapshotColor = colorDialog1.Color;
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
                Setting.DownCam_SnapshotColor = colorDialog1.Color;
                DownCamera.SnapshotColor = colorDialog1.Color;
            }
        }

        */

        private void StopDemo_button_Click(object sender, EventArgs e)
        {
            DemoRunning = false;
        }

        private bool DemoRunning = false;
        private Thread DemoThread;

        private void DemoWork()
        {

            while (DemoRunning)
            {

            }
        }

        private void SetColorBoxColor(int row)
        {
            // xxx Color_Box.BackColor = Color.FromArgb(R, G, B);
        }

        // =================================================================================
        // GetCorrentionForPartAtNozzle():
        // takes a look from Upcam, sets the correction values for the part at Nozzle
        private bool GetCorrentionForPartAtNozzle(out double dX, out double dY, out double dA)
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
            // xxx SetUpCamComponentsMeasurement();
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
                        AbortPlacementShown = true;
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
                if (UpCamera.GetClosestComponent(out X, out Y, out dA, Tolerance * UpCamera.Settings.XmmPerPixel) > 0)
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
            X = Xsum / UpCamera.Settings.XmmPerPixel;
            dX = X / (float)count;
            Y = -Y / UpCamera.Settings.XmmPerPixel;
            dY = Y / (float)count;
            DisplayText("Component measurement:");
            DisplayText("X: " + X.ToString("0.000", CultureInfo.InvariantCulture)
                + " (" + count.ToString(CultureInfo.InvariantCulture) + " results out of 5)");
            DisplayText("Y: " + Y.ToString("0.000", CultureInfo.InvariantCulture));
            return true;
        }
    }

    partial class Camera
    {
        public void SignalToStop()      // Asks nicely
        {
            VideoSource.SignalToStop();
        }

        public void NakedStop()         // Tries to force it (but still doesn't always work, just like with children)
        {
            VideoSource.Stop();
        }

        public void DisplayPropertyPage()
        {
            VideoSource.DisplayPropertyPage(IntPtr.Zero);
        }

        enum DataGridViewColumns { Function, Active, Int, Double, R, G, B };

        // ===========
        public int Threshold { get; set; }                  // Threshold for all the "draw" functions
        public bool GrayScale { get; set; }                 // If image is converted to grayscale 

        // ===========
        private List<IntPoint> ScaleOutline(double scale, List<IntPoint> Outline)
        {
            List<IntPoint> Result = new List<IntPoint>();
            foreach (var p in Outline)
            {
                Result.Add(new IntPoint((int)(p.X * scale), (int)(p.Y * scale)));
            }
            return Result;
        }

        // ===========
        private List<AForge.Point> ScaleOutline(double scale, List<AForge.Point> Outline)
        {
            List<AForge.Point> Result = new List<AForge.Point>();
            foreach (var p in Outline)
            {
                Result.Add(new AForge.Point((float)(p.X * scale), (float)(p.Y * scale)));
            }
            return Result;
        }

        // ===========
        public List<Shapes.Component> GetMeasurementComponents()
        {
            // No filtering! (tech. debt, maybe)
            Bitmap image = GetMeasurementFrame();
            List<Shapes.Component> Components = FindComponentsFunct(image);
            image.Dispose();
            return Components;
        }

        // ===========
        public int GetClosestRectangle(out double X, out double Y, double MaxDistance)
        // Sets X, Y position of the closest circle to the frame center in pixels, return value is number of circles found
        {
            List<Shapes.Rectangle> Rectangles = GetMeasurementRectangles(MaxDistance);
            X = 0.0;
            Y = 0.0;
            if (Rectangles.Count == 0)
            {
                return (0);
            }
            // Find the closest
            int closest = FindClosestRectangle(Rectangles);
            double zoom = GetMeasurementZoom();
            X = (Rectangles[closest].Center.X - FrameCenterX);
            Y = (Rectangles[closest].Center.Y - FrameCenterY);
            X = X / zoom;
            Y = Y / zoom;
            return (Rectangles.Count);
        }

        // ===========
        private Bitmap TestAlgorithmFunct(Bitmap frame)
        {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
            return (frame);
        }

        // ===========
        private Bitmap Draw_SnapshotFunct(Bitmap image)
        {
            if (rotating)
            {
                return (image);
            }
            overlaying = true;
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(SnapshotImage, new System.Drawing.Point(0, 0));
            g.Dispose();
            overlaying = false;
            return (image);
        }

        // ===========
        /*private void DrawSidemarksFunct(Graphics g)
        {
            // default values used when show pixels is off: 
            // Draw from frame edges inwards, using ticksize that gets zoomed down
            int TickSize = (FrameSizeX / 640) * 8;
            int XstartUp = FrameSizeY;  // values used when drawing along X axis
            int XstartBot = 0;
            int YstartLeft = 0;         // values used when drawing along Y axis
            int YstartRight = FrameSizeX;
            int Xinc = Convert.ToInt32(YstartRight / SideMarksX);    // sidemarks: 10cm on machine
            int Yinc = Convert.ToInt32(XstartUp / SideMarksY);

            if (ImageBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                // Show pixels is on, draw to middle of the image
                TickSize = 8;
                XstartUp = (FrameSizeY / 2) + 240;
                XstartBot = (FrameSizeY / 2) - 240;
                YstartLeft = (FrameSizeX / 2) - 320;
                YstartRight = (FrameSizeX / 2) + 320;
                Xinc = Convert.ToInt32(640 / SideMarksX);
                Yinc = Convert.ToInt32(480 / SideMarksY);
            }

            Pen pen = new Pen(Color.Red, 2);
            Graphics g = Graphics.FromImage(img);
            int X = YstartLeft + Xinc;
            while (X < YstartRight)
            {
                g.DrawLine(pen, X, XstartUp, X, XstartUp - TickSize);
                g.DrawLine(pen, X, XstartBot, X, XstartBot + TickSize);
                X += Xinc;
            }

            int Y = XstartBot + Yinc;
            while (Y < XstartUp)
            {
                g.DrawLine(pen, YstartLeft, Y, YstartLeft + TickSize, Y);
                g.DrawLine(pen, YstartRight, Y, YstartRight - TickSize, Y);
                Y += Yinc;
            }
        }*/
    }
}
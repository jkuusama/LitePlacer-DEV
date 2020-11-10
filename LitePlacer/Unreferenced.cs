using AForge;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LitePlacer
{
    partial class FormMain
    {

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
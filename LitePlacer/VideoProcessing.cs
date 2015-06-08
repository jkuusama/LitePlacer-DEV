using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu;

namespace LitePlacer {

    public delegate void AForge_op(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B);
    class AForgeFunction {
            public AForge_op func { get; set; }
            public int parameter_int { get; set; }				// general parameters. Some functions take one int,
            public double parameter_double { get; set; }		// some take a float,
            public int R { get; set; }				// and some need R, B, G values.
            public int G { get; set; }
            public int B { get; set; }
        }



    public class VideoProcessing {

        private List<AForgeFunction> FunctionList = new List<AForgeFunction>();
        private List<AForgeFunction> NewFunctionList = null;
        private VideoDetection videoDetection;
        private Camera cam;
        public bool ApplyVideoMarkup = true;

        public VideoProcessing(Camera camera) {
            this.cam = camera;
            this.videoDetection = camera.videoDetection;
        }


        // And calls xx_measure() funtion. (Any function doing measurement from video frames.)
        // The xxx_measure funtion calls GetMeasurementFrame() function, that takes a frame form the stream, 
        // processes it with the MeasurementFunctions list and returns the processed frame:

        public void ProcessFrame(ref Bitmap displayFrame, ref Bitmap measurementFrame) {
            // if we have an updated list, then apply it the next itteration
            if (NewFunctionList != null) {
                FunctionList = NewFunctionList;
                NewFunctionList = null;
            }

            // apply video processing functions
            if (FunctionList != null) {
                foreach (AForgeFunction f in FunctionList) {
                    f.func(ref displayFrame, f.parameter_int, f.parameter_double, f.R, f.B, f.G);
                }
            }

            // flip the measurement displayFrame so it looks the same as the display displayFrame XXX not sure why this is (?)
            if (cam.IsUpCamera()) new Mirror(false, true).ApplyInPlace(displayFrame);

            if (cam.CopyFrame) {
                // this frame is wher we measure, pre-markup and pre-extra-zoom
                measurementFrame = (Bitmap)displayFrame.Clone();
                cam.CopyFrame = false;
            }

            //further modify the image
            if (ApplyVideoMarkup) {
                if (cam.FindCircles) DrawCirclesFunct(displayFrame);
                if (cam.FindRectangles) displayFrame = DrawRectanglesFunct(displayFrame);
                if (cam.FindFiducial) DrawFiducialFunct(ref displayFrame);
                if (cam.FindComponent) displayFrame = DrawComponentsFunct(displayFrame);
                if (cam.Draw_Snapshot) displayFrame = Draw_SnapshotFunct(displayFrame);
              //  if (cam.IsUpCamera()) displayFrame = MirrorFunct(displayFrame);
                if (cam.DrawBox) DrawBoxFunct(displayFrame);
                if (cam.MarkA.Count > 0) DrawMarks(ref displayFrame, cam.MarkA, Color.Blue, 20);
                if (cam.MarkB.Count > 0) DrawMarks(ref displayFrame, cam.MarkB, Color.Red, 20);
                // Thing after this point are affected by the zoom
                if (cam.Zoom) ZoomFunct(ref displayFrame, cam.ZoomFactor);
                if (cam.Draw1mmGrid) DrawGridFunct(ref displayFrame);
                if (cam.DrawCross) DrawCrossFunct(ref displayFrame);
                if (cam.DrawSidemarks) DrawSidemarksFunct(ref displayFrame);
                if (cam.DrawDashedCross) DrawDashedCrossFunct(displayFrame);
                if (cam.MarkupText.Count > 0) DrawMarkupText(ref displayFrame, cam.MarkupText);
            }

            if (cam.TakeSnapshot) {
                TakeSnapshot_funct(displayFrame);
                cam.TakeSnapshot = false;
            };

        } 


        // =========================================================
        // Snapshot handling
        // =========================================================

        // repeated rotations destroy the image. We'll store the original here and rotate only once.
        Bitmap SnapshotOriginalImage = new Bitmap(640, 480);

        Bitmap SnapshotImage = new Bitmap(640, 480);

        private void TakeSnapshot_funct(Bitmap img) {
            Bitmap image = Grayscale.CommonAlgorithms.RMY.Apply(img);

            // find edges
            SobelEdgeDetector EdgeFilter = new SobelEdgeDetector();
            EdgeFilter.ApplyInPlace(image);
            // back to color format
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();
            image = RGBfilter.Apply(image);
            // get rid of grays
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            filter.CenterColor.Red = 20;
            filter.CenterColor.Green = 20;
            filter.CenterColor.Blue = 20;
            filter.FillOutside = false;
            filter.Radius = 200;
            filter.ApplyInPlace(image);

            Color peek;
            for (int y = 0; y < image.Height; y++) {
                for (int x = 0; x < image.Width; x++) {
                    peek = image.GetPixel(x, y);
                    if (peek.R != 0) {
                        image.SetPixel(x, y, Color.Blue);
                    }
                }
            }

            image.MakeTransparent(Color.Black);
            SnapshotImage = image;
            SnapshotOriginalImage = image;
        }


        // =========================================================
        public bool rotating = false;
        private bool overlaying = false;

        private Bitmap Draw_SnapshotFunct(Bitmap image) {
            if (rotating) {
                return (image);
            }
            overlaying = true;
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(SnapshotImage, new System.Drawing.Point(0, 0));
            g.Dispose();
            overlaying = false;
            return (image);
        }

        // =========================================================
        public void RotateSnapshot(double deg) {
            while (overlaying) {
                Thread.Sleep(10);
            }
            rotating = true;
            // Convert to 24 bpp RGB Image
            Rectangle dimensions = new Rectangle(0, 0, SnapshotOriginalImage.Width, SnapshotOriginalImage.Height);
            Bitmap Snapshot24b = new Bitmap(SnapshotOriginalImage.Width, SnapshotOriginalImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(Snapshot24b)) {
                gr.DrawImage(SnapshotOriginalImage, dimensions);
            }

            RotateNearestNeighbor filter = new RotateNearestNeighbor(deg - cam.SnapshotRotation, true);
            Snapshot24b = filter.Apply(Snapshot24b);
            // convert back to 32b, to have transparency
            Snapshot24b.MakeTransparent(Color.Black);
            SnapshotImage = Snapshot24b;
            rotating = false;
        }



        // Since the measured image might be zoomed in, we need the value, so that we can convert to real measurements (public for debug)
        public double GetZoom() {
            double zoom = 1.0;
            foreach (var x in FunctionList.Where(x => x.func == Meas_ZoomFunc).Select(x => x.parameter_double).ToList()) {
                zoom *= x;
            }
            return zoom;
        }

        enum DataGridViewColumns { Function, Active, Int, Double, R, G, B };
        public void UpdateFunctionList(DataGridView Grid) {
            List<AForgeFunction> NewList = new List<AForgeFunction>();
            int temp_i;
            double temp_d;
            int FunctionCol = (int)DataGridViewColumns.Function;
            int ActiveCol = (int)DataGridViewColumns.Active;
            int IntCol = (int)DataGridViewColumns.Int;
            int DoubleCol = (int)DataGridViewColumns.Double;
            int R_col = (int)DataGridViewColumns.R;
            int G_col = (int)DataGridViewColumns.G;
            int B_col = (int)DataGridViewColumns.B;

            NewList.Clear();
            cam.MainForm.DisplayText("BuildFunctionsList:");

            foreach (DataGridViewRow Row in Grid.Rows) {
                AForgeFunction f = new AForgeFunction();
                // newly created rows are not complete yet
                if (Row.Cells[FunctionCol].Value == null) {
                    continue;
                }
                if (Row.Cells[ActiveCol].Value == null) {
                    continue;
                }
                // skip inactive rows
                if (Row.Cells[ActiveCol].Value.ToString() == "False") {
                    continue;
                }

                if (Row.Cells[ActiveCol].Value.ToString() == "false") {
                    continue;
                }

                switch (Row.Cells[FunctionCol].Value.ToString()) {
                    case "Grayscale":
                        f.func = GrayscaleFunc;
                        break;

                    case "Contrast scretch":
                        f.func = Contrast_scretchFunc;
                        break;

                    case "Kill color":
                        f.func = KillColor_Func;
                        break;

                    case "Keep color":
                        f.func = KeepColor_Func;
                        break;

                    case "Invert":
                        f.func = InvertFunct;
                        break;

                    case "Meas. zoom":
                        f.func = Meas_ZoomFunc;
                        break;

                    case "Edge detect":
                        f.func = Edge_detectFunc;
                        break;

                    case "Noise reduction":
                        f.func = NoiseReduction_Funct;
                        break;

                    case "Threshold":
                        f.func = ThresholdFunct;
                        break;

                    case "Histogram":
                        f.func = HistogramFunct;
                        break;

                    default:
                        continue;
                    // break; 
                }
                string msg = Row.Cells[FunctionCol].Value.ToString();
                msg += " / ";
                if (Row.Cells[IntCol].Value != null) {
                    int.TryParse(Row.Cells[IntCol].Value.ToString(), out temp_i);
                    f.parameter_int = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[DoubleCol].Value != null) {
                    double.TryParse(Row.Cells[DoubleCol].Value.ToString(), out temp_d);
                    f.parameter_double = temp_d;
                    msg += temp_d.ToString();
                }
                msg += " / ";
                if (Row.Cells[R_col].Value != null) {
                    int.TryParse(Row.Cells[R_col].Value.ToString(), out temp_i);
                    f.R = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[G_col].Value != null) {
                    int.TryParse(Row.Cells[G_col].Value.ToString(), out temp_i);
                    f.G = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                if (Row.Cells[B_col].Value != null) {
                    int.TryParse(Row.Cells[B_col].Value.ToString(), out temp_i);
                    f.B = temp_i;
                    msg += temp_i.ToString();
                }
                msg += " / ";
                NewList.Add(f);
                cam.MainForm.DisplayText(msg);
            };
            // update list when video is not being processed
            NewFunctionList = NewList;
        }


        public void ClearFunctionsList() {
            NewFunctionList = new List<AForgeFunction>();
        }



        // ==========================================================================================================
        // Functions compatible with lists:
        // ==========================================================================================================
        // Note, that each function needs to keep the image in RGB, otherwise drawing fill fail 

        // ========================================================= 

        private void NoiseReduction_Funct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);	// Make gray
            switch (par_int) {
                case 1:
                    BilateralSmoothing Bil_filter = new BilateralSmoothing();
                    Bil_filter.KernelSize = 7;
                    Bil_filter.SpatialFactor = 10;
                    Bil_filter.ColorFactor = 30;
                    Bil_filter.ColorPower = 0.5;
                    Bil_filter.ApplyInPlace(frame);
                    break;

                case 2:
                    Median M_filter = new Median();
                    M_filter.ApplyInPlace(frame);
                    break;

                case 3:
                    Mean Meanfilter = new Mean();
                    // apply the filter
                    Meanfilter.ApplyInPlace(frame);
                    break;

                default:
                    Median Median_filter = new Median();
                    Median_filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();	// back to color format
            frame = RGBfilter.Apply(frame);
        }

        // =========================================================
        private void Edge_detectFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);	// Make gray
            switch (par_int) {
                case 1:
                    SobelEdgeDetector SobelFilter = new SobelEdgeDetector();
                    SobelFilter.ApplyInPlace(frame);
                    break;

                case 2:
                    DifferenceEdgeDetector DifferenceFilter = new DifferenceEdgeDetector();
                    DifferenceFilter.ApplyInPlace(frame);
                    break;

                case 3:
                    HomogenityEdgeDetector HomogenityFilter = new HomogenityEdgeDetector();
                    HomogenityFilter.ApplyInPlace(frame);
                    break;

                case 4:
                    // can we not have references to canny in the code. gives me ptsd flashbacks
                    CannyEdgeDetector Cannyfilter = new CannyEdgeDetector();
                    // apply the filter
                    Cannyfilter.ApplyInPlace(frame);
                    break;

                default:
                    HomogenityEdgeDetector filter = new HomogenityEdgeDetector();
                    filter.ApplyInPlace(frame);
                    break;
            }
            GrayscaleToRGB RGBfilter = new GrayscaleToRGB();	// back to color format
            frame = RGBfilter.Apply(frame);
        }

        // =========================================================
        private void InvertFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void HistogramFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            // create filter
            HistogramEqualization filter = new HistogramEqualization();
            // process image
            filter.ApplyInPlace(frame);
        }


        // =========================================================
        private void KillColor_Func(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            // create filter
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            // set center colol and radius
            filter.CenterColor = new RGB((byte)par_R, (byte)par_G, (byte)par_B);
            filter.Radius = (short)par_int;
            filter.FillOutside = false;
            // apply the filter
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void KeepColor_Func(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            // create filter
            EuclideanColorFiltering filter = new EuclideanColorFiltering();
            // set center colol and radius
            filter.CenterColor = new RGB((byte)par_R, (byte)par_G, (byte)par_B);
            filter.Radius = (short)par_int;
            filter.FillOutside = true;
            // apply the filter
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void ThresholdFunct(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Threshold filter = new Threshold(par_int);
            filter.ApplyInPlace(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(frame);
        }


        // ========================================================= Contrast_scretchFunc
        private void GrayscaleFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            Grayscale toGrFilter = new Grayscale(0.2125, 0.7154, 0.0721);       // create grayscale filter (BT709)
            Bitmap fr = toGrFilter.Apply(frame);
            GrayscaleToRGB toColFilter = new GrayscaleToRGB();
            frame = toColFilter.Apply(fr);
        }

        private void Contrast_scretchFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            ContrastStretch filter = new ContrastStretch();
            // process image
            filter.ApplyInPlace(frame);
        }

        // =========================================================
        private void Meas_ZoomFunc(ref Bitmap frame, int par_int, double par_d, int par_R, int par_G, int par_B) {
            ZoomFunct(ref frame, par_d);
        }

        private Bitmap DrawComponentsFunct(Bitmap bitmap) {
            List<Shapes.Component> Components = videoDetection.FindComponents(bitmap);

            Graphics g = Graphics.FromImage(bitmap);
            Pen OrangePen = new Pen(Color.DarkOrange, 1);
            Pen RedPen = new Pen(Color.DarkRed, 2);
            Pen BluePen = new Pen(Color.Blue, 2);
            Shapes.Component Component;
            System.Drawing.Point p1 = new System.Drawing.Point();
            System.Drawing.Point p2 = new System.Drawing.Point();

            for (int i = 0, n = Components.Count; i < n; i++) {
                Component = Components[i];

                // move Component.Longest start to ComponentCenter, draw it
                float dx = Component.Center.X - Component.Longest.Start.X;
                float dy = Component.Center.Y - Component.Longest.Start.Y;
                p1.X = (int)Math.Round(Component.Longest.Start.X + dx);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y + dy);
                p2.X = (int)Math.Round(Component.Longest.End.X + dx);
                p2.Y = (int)Math.Round(Component.Longest.End.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                // move Component.Longest end to ComponentCenter, draw Component.Longest
                dx = Component.Center.X - Component.Longest.End.X;
                dy = Component.Center.Y - Component.Longest.End.Y;
                p1.X = (int)Math.Round(Component.Longest.Start.X + dx);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y + dy);
                p2.X = (int)Math.Round(Component.Longest.End.X + dx);
                p2.Y = (int)Math.Round(Component.Longest.End.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                //  move Normal start to ComponentCenter, draw it
                dx = Component.Center.X - Component.NormalStart.X;
                dy = Component.Center.Y - Component.NormalStart.Y;
                p1.X = (int)Math.Round(Component.NormalStart.X + dx);
                p1.Y = (int)Math.Round(Component.NormalStart.Y + dy);
                p2.X = (int)Math.Round(Component.NormalEnd.X + dx);
                p2.Y = (int)Math.Round(Component.NormalEnd.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                //  move Component.Normal end to ComponentCenter, draw it
                dx = Component.Center.X - Component.NormalEnd.X;
                dy = Component.Center.Y - Component.NormalEnd.Y;
                p1.X = (int)Math.Round(Component.NormalStart.X + dx);
                p1.Y = (int)Math.Round(Component.NormalStart.Y + dy);
                p2.X = (int)Math.Round(Component.NormalEnd.X + dx);
                p2.Y = (int)Math.Round(Component.NormalEnd.Y + dy);
                g.DrawLine(RedPen, p1, p2);

                // draw outline
                g.DrawPolygon(OrangePen, ToPointsArray(Component.Outline));

                // draw Component.Longest
                p1.X = (int)Math.Round(Component.Longest.Start.X);
                p1.Y = (int)Math.Round(Component.Longest.Start.Y);
                p2.X = (int)Math.Round(Component.Longest.End.X);
                p2.Y = (int)Math.Round(Component.Longest.End.Y);
                g.DrawLine(BluePen, p1, p2);

            }
            return (bitmap);
        }


        // =========================================================
        private void DrawCirclesFunct(Bitmap bitmap) {
            List<Shapes.Circle> Circles = videoDetection.FindCircles(bitmap);

            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(Color.DarkOrange, 2);

            for (int i = 0, n = Circles.Count; i < n; i++) {
                Circles[i].ToRawResolution();
                g.DrawEllipse(pen,
                    (float)(Circles[i].X - Circles[i].Radius), (float)(Circles[i].Y - Circles[i].Radius),
                    (float)(Circles[i].Radius * 2), (float)(Circles[i].Radius * 2));
            }
        }


        // ==========================================================================================================

        // flip Y
        private Bitmap MirrorFunct(Bitmap frame) {
            new Mirror(false, true).ApplyInPlace(frame);
            return frame;
        }


        // =========================================================
        private Bitmap TestAlgorithmFunct(Bitmap frame) {
            frame = Grayscale.CommonAlgorithms.RMY.Apply(frame);
            Invert filter = new Invert();
            filter.ApplyInPlace(frame);
            return (frame);
        }

        // =========================================================
        private void ZoomFunct(ref Bitmap frame, double Factor) {
            if (Factor < 0.1) return;

            int centerX = frame.Width / 2;
            int centerY = frame.Height / 2;
            int OrgSizeX = frame.Width;
            int OrgSizeY = frame.Height;

            int fromX = centerX - (int)(centerX / Factor);
            int fromY = centerY - (int)(centerY / Factor);
            int SizeX = (int)(OrgSizeX / Factor);
            int SizeY = (int)(OrgSizeY / Factor);
            Crop CrFilter = new Crop(new Rectangle(fromX, fromY, SizeX, SizeY));
            frame = CrFilter.Apply(frame);
            ResizeBilinear RBfilter = new ResizeBilinear(OrgSizeX, OrgSizeY);
            frame = RBfilter.Apply(frame);
        }

        public void DrawMarkupText(ref Bitmap image, List<VideoTextMarkup> texts) {
            lock(texts) {
                foreach (var x in texts) {
                    x.Draw(ref image);
                }
            } 
        }
        


        // =========================================================
        /// <summary>
        /// This function will display markings at specified locations on the image offset from the center of the image
        /// </summary>
        public void DrawMarks(ref Bitmap image, List<PointF> points, Color color, int size) {
            try {
                using (Image<Bgr, Byte> img = new Image<Bgr, byte>(image)) {
                    foreach (var pt in points) {
                        PointF p = new PointF(pt.X + cam.FrameCenterX, cam.FrameCenterY-pt.Y);
                        var cross = new Cross2DF(p, size, size);
                        img.Draw(cross, new Bgr(color), 2);
                    }
                    image = img.ToBitmap();
                }
            } catch {

            }
        }

        // =========================================================
        private void DrawFiducialFunct(ref Bitmap image) {
            cam.MarkA.Clear();
            var fids = videoDetection.FindTemplates(image);
            foreach (var f in fids) f.ToScreenResolution();
            cam.MarkA.AddRange(fids.Select(x => x.ToPartLocation().ToPointF()).ToArray());
        }





        public System.Drawing.PointF[] ToRotatedRectangle(Shapes.Rectangle s) {
            s.ToRawResolution();
            System.Drawing.PointF[] p = new System.Drawing.PointF[5];
            p[0].X = (float)s.Left; p[0].Y = (float)s.Top;
            p[1].X = (float)s.Right; p[1].Y = (float)s.Top;
            p[2].X = (float)s.Right; p[2].Y = (float)s.Bottom;
            p[3].X = (float)s.Left; p[3].Y = (float)s.Bottom;
            p[4].X = (float)s.Left; p[4].Y = (float)s.Top;

            // roate about center
            double angle = s.A * Math.PI / 180d;
            PartLocation center = new PartLocation(s);
            for (int i = 0; i < p.Length; i++) {
                PartLocation pp = new PartLocation(p[i]) - center; //shift to zero
                pp = pp.Rotate(angle) + center;
                p[i] = pp.ToPointF();
            }

            return p;
        }


        private Bitmap DrawRectanglesFunct(Bitmap image) {
            var rects = cam.videoDetection.FindRectangles(image);
            if (rects.Count == 0) return image;
            Graphics g = Graphics.FromImage(image);
           
            
            foreach (var rect in rects) {
            //    g.DrawRectangle(new Pen(Color.Red), rect.ToDrawingRectangle());
                g.DrawLines(new Pen(Color.Red), ToRotatedRectangle(rect));
            }
            
            var rect2 = cam.videoDetection.GetSmallestCenteredRectangle(rects);
            //g.DrawRectangle(new Pen(Color.Green, 5f), rect2.ToDrawingRectangle());
            if (rect2 != null) {
                g.DrawLines(new Pen(Color.Green, 5f), ToRotatedRectangle(rect2));
                g.DrawRectangle(new Pen(Color.Green, 5f), new Rectangle((int)rect2.X, (int)rect2.Y, 10, 10));
            }
            return image;
        }

        // =========================================================
        private Bitmap DrawRectanglesFunct_OLD(Bitmap image) {

            // step 1 - turn background to black (done)

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 3;
            blobCounter.MinWidth = 3;
            blobCounter.ProcessImage(image);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // step 3 - check objects' type and do what you do:
            Graphics g = Graphics.FromImage(image);
            Pen pen = new Pen(Color.DarkOrange, 2);

            for (int i = 0, n = blobs.Length; i < n; i++) {
                SimpleShapeChecker ShapeChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                List<IntPoint> cornerPoints;

                // fine tune ShapeChecker
                ShapeChecker.AngleError = 15;  // default 7
                ShapeChecker.LengthError = 0.3F;  // default 0.1 (10%)
                ShapeChecker.MinAcceptableDistortion = 0.9F;  // in pixels, default 0.5 
                ShapeChecker.RelativeDistortionLimit = 0.2F;  // default 0.03 (3%)

                // use the Outline checker to extract the corner points
                if (ShapeChecker.IsQuadrilateral(edgePoints, out cornerPoints)) {
                    // only do things if the corners form a rectangle
                    if (ShapeChecker.CheckPolygonSubType(cornerPoints) == PolygonSubType.Rectangle) {
                        List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(edgePoints);
                        g.DrawPolygon(pen, ToPointsArray(corners));
                    }
                }
            }
            return (image);
        }


        // =========================================================
        private void DrawDashedCrossFunct(Bitmap img) {
            Pen pen = new Pen(Color.SlateGray, 1);
            Graphics g = Graphics.FromImage(img);
            int step = cam.FrameSizeY / 40;
            int i = step / 2;
            while (i < cam.FrameSizeY) {
                g.DrawLine(pen, cam.FrameCenterX, i, cam.FrameCenterX, i + step);
                i = i + 2 * step;
            }
            step = cam.FrameSizeX / 40;
            i = step / 2;
            while (i < cam.FrameSizeX) {
                g.DrawLine(pen, i, cam.FrameCenterY, i + step, cam.FrameCenterY);
                i = i + 2 * step;
            }
        }

        // =========================================================

        private void DrawGridFunct(ref Bitmap img) {  // i get out of memory errors here all the time - not sure why 
            // so protecting execution this way.
            try {
                Pen pen = new Pen(Color.Green, 1);
                Graphics g = Graphics.FromImage(img);

                var xscale = Properties.Settings.Default.DownCam_XmmPerPixel / cam.GetMeasurementZoom();
                var yscale = Properties.Settings.Default.DownCam_YmmPerPixel / cam.GetMeasurementZoom();
                var xlines = cam.FrameSizeX / 2 * xscale;
                var ylines = cam.FrameSizeY / 2 * yscale;

                for (int i = 0; i < xlines; i++) {
                    g.DrawLine(pen, cam.FrameCenterX + (int)(i / xscale), cam.FrameSizeY, cam.FrameCenterX + (int)(i / xscale), 0);
                    g.DrawLine(pen, cam.FrameCenterX + (int)(-i / xscale), cam.FrameSizeY, cam.FrameCenterX + (int)(-i / xscale), 0);
                }
                for (int i = 0; i < ylines; i++) {
                    g.DrawLine(pen, 0, cam.FrameCenterY + (int)(i / yscale), cam.FrameSizeX, cam.FrameCenterY + (int)(i / yscale));
                    g.DrawLine(pen, 0, cam.FrameCenterY + (int)(-i / yscale), cam.FrameSizeX, cam.FrameCenterY + (int)(-i / yscale));
                }
            } catch (Exception e) {
                Console.WriteLine("DrawGridFunct Error: " + e);
            }
        }
        // =========================================================

        public void DrawCrossFunct(ref Bitmap img) {  // i get out of memory errors here all the time - not sure why 
            // so protecting execution this way.
            try {
                Pen pen = new Pen(Color.Red, 1);
                Graphics g = Graphics.FromImage(img);
                g.DrawLine(pen, cam.FrameCenterX, 0, cam.FrameCenterX, cam.FrameSizeY);
                g.DrawLine(pen, 0, cam.FrameCenterY, cam.FrameSizeX, cam.FrameCenterY);
            } catch (Exception e) {
                Console.WriteLine("DrawCrossFunct Error: " + e);
            }
        }

        // =========================================================

        private void DrawSidemarksFunct(ref Bitmap img) {
            Pen pen = new Pen(Color.Red, 2);
            Graphics g = Graphics.FromImage(img);
            int Xinc = Convert.ToInt32(cam.FrameSizeX / cam.SideMarksX);
            int X = Xinc;
            int tick = 6;
            while (X < cam.FrameSizeX) {
                g.DrawLine(pen, X, cam.FrameSizeY, X, cam.FrameSizeY - tick);
                g.DrawLine(pen, X, 0, X, tick);
                X += Xinc;
            }
            int Yinc = Convert.ToInt32(cam.FrameSizeY / cam.SideMarksY);
            int Y = Yinc;
            while (Y < cam.FrameSizeY) {
                g.DrawLine(pen, cam.FrameSizeX, Y, cam.FrameSizeX - tick, Y);
                g.DrawLine(pen, 0, Y, tick, Y);
                Y += Yinc;
            }
        }

   


        // =========================================================
        private void DrawBoxFunct(Bitmap img) {

            Pen pen = new Pen(Color.Red, 1);
            Graphics g = Graphics.FromImage(img);

            g.DrawLine(pen, cam.BoxPoints[0].X + cam.FrameCenterX, cam.BoxPoints[0].Y + cam.FrameCenterY, cam.BoxPoints[1].X + cam.FrameCenterX, cam.BoxPoints[1].Y + cam.FrameCenterY);
            g.DrawLine(pen, cam.BoxPoints[1].X + cam.FrameCenterX, cam.BoxPoints[1].Y + cam.FrameCenterY, cam.BoxPoints[2].X + cam.FrameCenterX, cam.BoxPoints[2].Y + cam.FrameCenterY);
            g.DrawLine(pen, cam.BoxPoints[2].X + cam.FrameCenterX, cam.BoxPoints[2].Y + cam.FrameCenterY, cam.BoxPoints[3].X + cam.FrameCenterX, cam.BoxPoints[3].Y + cam.FrameCenterY);
            g.DrawLine(pen, cam.BoxPoints[3].X + cam.FrameCenterX, cam.BoxPoints[3].Y + cam.FrameCenterY, cam.BoxPoints[0].X + cam.FrameCenterX, cam.BoxPoints[0].Y + cam.FrameCenterY);
        }

        // =========================================================
        // Convert list of AForge.NET's points to array of .NET points
        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points) {
            return points.Select(x => new System.Drawing.Point(x.X, x.Y)).ToArray();
        }

    }
}

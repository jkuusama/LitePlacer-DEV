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
    public class VideoDetection {
        private Camera camera;

        public VideoDetection(Camera camera) {
            this.camera = camera;
        }
                   

        /************** These functions are intelligent about their resolution/offset and can be changed dynamically *****/
        
        /// <summary>
        /// Finds circles - zoom is NOT compensated for in returned results
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>Graphic coordinates with zoom not accounted for for found circles</returns>
        public List<Shapes.Circle> FindCircles(Bitmap bitmap) {
            // locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            List<Shapes.Circle> Circles = new List<Shapes.Circle>();

            for (int i = 0, n = blobs.Length; i < n; i++) {
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                AForge.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius)) {
                    if (radius > 3) {  // filter out some noise
                        var circle = new Shapes.Circle(center.X, center.Y, radius);
                        Circles.Add(circle);
                    }
                }
            }
            SetCamera(Circles);
            return (Circles);
        }

        public List<Shapes.Rectangle> FindRectangles(Bitmap frame) {
            List<Shapes.Rectangle> rects = new List<Shapes.Rectangle>();

            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(frame)) {

              //  double cannyThresholdLinking = 120.0;
              //  double cannyThreshold = 180.0;

                //Convert the image to grayscale and filter out the noise
                Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();
                //Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);

                using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                    for ( Contour<System.Drawing.Point> contours = gray.FindContours(
                          Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                          Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST,
                          storage);
                          contours != null;
                          contours = contours.HNext) {
                        Contour<System.Drawing.Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                        if (currentContour.Area > 250) {//only consider contours with area greater than 250
                            if (currentContour.Total == 4) { //The contour has 4 vertices.
                                #region determine if all the angles in the contour are within [80, 100] degree
                                bool isRectangle = true;
                                System.Drawing.Point[] pts = currentContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int i = 0; i < edges.Length; i++) {
                                    double angle = Math.Abs(
                                        edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                                    if (angle < 80 || angle > 100) {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                #endregion

                                if (isRectangle) {
                                    var box = currentContour.GetMinAreaRect();
                                    Shapes.Rectangle r = new Shapes.Rectangle(box.center.X, box.center.Y, box.angle);                                 
                                    r.Height = box.size.Height;
                                    r.Width = box.size.Width;
                                    r.camera = this.camera;                                    
                                    rects.Add(r);
                                }

                            }
                        }
                 }
            }
            return rects;
        }

        //gooz
        public List<Shapes.Circle> FindCircles() {
            camera.MainForm.DisplayText("FindCircles()", System.Drawing.Color.Orange);

            Bitmap frame = camera.GetMeasurementFrame();
            var circles = FindCircles(frame);
            List<PointF> t = new List<PointF>();
            foreach (var circle in circles) {
                circle.ToScreenResolution();
                t.Add(circle.ToPartLocation().ToPointF());
            }
            camera.videoProcessing.DrawMarks(ref frame, t, Color.Green, 30);
            camera.videoProcessing.DrawCrossFunct(ref frame);
            frame.Save(@"c:\findCircles.bmp");
            return circles;
        }





        /// <summary>
        /// This function will take a bitmap image and find the locations of a template image in the file for a given threhsold
        /// </summary>
        /// <param name="image">Bitmap of image to look at</param>
        /// <param name="template_filename">File containg template image</param>
        /// <returns>imagefinder object</returns>
        public List<Shapes.Fiducal> FindTemplates(Bitmap image, string template_filename, double threshold) {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(image);

            // template plus 90deg roated version of the template
            Image<Bgr, Byte> templ1 = new Image<Bgr, byte>(template_filename);
            Image<Bgr, Byte> templ2 = new Image<Bgr, byte>(templ1.Height, templ1.Width);
            CvInvoke.cvTranspose(templ1.Ptr, templ2.Ptr);

            ImageFinder imageFinder = new ImageFinder(img, threshold);
            imageFinder.FindImage(templ1);
            imageFinder.FindImage(templ2);

            var ret = imageFinder.Points.Select(x => new Shapes.Fiducal(x.X, x.Y)).ToList();
            SetCamera(ret);
            return ret;
        }
        public List<Shapes.Fiducal> FindTemplates(string template_filename, double threshold) {
            return FindTemplates(camera.GetMeasurementFrame(), template_filename, threshold);
        }

        /// <summary>
        /// Will use default settings for finding the template
        /// </summary>
        /// <param name="image">the image to search</param>
        /// <returns>list of fiducary points</returns>
        public List<Shapes.Fiducal> FindTemplates(Bitmap image) {
            return FindTemplates(image, Properties.Settings.Default.template_file, Properties.Settings.Default.template_threshold);
        }
        public List<Shapes.Fiducal> FindTemplates() {
            return FindTemplates(camera.GetMeasurementFrame());
        }

        public List<Shapes.Component> FindComponents(Bitmap bitmap) {
            // Locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 8;
            blobCounter.MinWidth = 8;
            blobCounter.ProcessImage(bitmap);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // create convex hull searching algorithm
            GrahamConvexHull hullFinder = new GrahamConvexHull();
            ClosePointsMergingOptimizer optimizer1 = new ClosePointsMergingOptimizer();
            FlatAnglesOptimizer optimizer2 = new FlatAnglesOptimizer();

            List<Shapes.Component> Components = new List<Shapes.Component>();

            // process each blob
            foreach (Blob blob in blobs) {
                List<IntPoint> leftPoints, rightPoints, edgePoints = new List<IntPoint>();
                if ((blob.Rectangle.Height > 400) && (blob.Rectangle.Width > 600)) {
                    break;	// The whole image could be a blob, discard that
                }
                // get blob's edge points
                blobCounter.GetBlobsLeftAndRightEdges(blob,
                    out leftPoints, out rightPoints);

                edgePoints.AddRange(leftPoints);
                edgePoints.AddRange(rightPoints);

                // blob's convex hull
                List<IntPoint> Outline = hullFinder.FindHull(edgePoints);
                optimizer1.MaxDistanceToMerge = 4;
                optimizer2.MaxAngleToKeep = 170F;
                Outline = optimizer2.OptimizeShape(Outline);
                Outline = optimizer1.OptimizeShape(Outline);

                // find Longest line segment
                float dist = 0;
                LineSegment Longest = new LineSegment(Outline[0], Outline[1]);
                LineSegment line;
                dist = Longest.Length;
                int LongestInd = 0;
                for (int i = 1; i < Outline.Count; i++) {
                    if (i != Outline.Count - 1) {
                        line = new LineSegment(Outline[i], Outline[i + 1]);
                    } else {
                        // last iteration
                        if (Outline[i] == Outline[0]) {
                            break;
                        }
                        line = new LineSegment(Outline[i], Outline[0]);
                    }
                    if (line.Length > dist) {
                        Longest = line;
                        dist = line.Length;
                        LongestInd = i;
                    }
                }
                // Get the center point of it
                AForge.Point LongestCenter = new AForge.Point();
                LongestCenter.X = (float)Math.Round((Longest.End.X - Longest.Start.X) / 2.0 + Longest.Start.X);
                LongestCenter.Y = (float)Math.Round((Longest.End.Y - Longest.Start.Y) / 2.0 + Longest.Start.Y);
                AForge.Point NormalStart = new AForge.Point();
                AForge.Point NormalEnd = new AForge.Point();
                // Find normal: 
                // start= longest.start rotated +90deg relative to center
                // end= longest.end rotated -90deg and relative to center
                // If you rotate point (px, py) around point (ox, oy) by angle theta you'll get:
                // p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
                // p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
                // cos90 = 0, sin90= 1 => 
                // p'x= -(py-oy) + ox= oy-py+ox, p'y= (px-ox)+ oy
                NormalStart.X = LongestCenter.Y - Longest.Start.Y + LongestCenter.X;
                NormalStart.Y = (Longest.Start.X - LongestCenter.X) + LongestCenter.Y;
                // cos-90=0, sin-90= -1 =>
                // p'x= (py-oy) + ox
                // p'y= -(px-ox)+oy= ox-px+oy
                NormalEnd.X = (Longest.Start.Y - LongestCenter.Y) + LongestCenter.X;
                NormalEnd.Y = LongestCenter.X - Longest.Start.X + LongestCenter.Y;
                // Make line out of the points
                Line Normal = Line.FromPoints(NormalStart, NormalEnd);

                // Find the furthest intersection to the normal (skip the Longest)
                AForge.Point InterSection = new AForge.Point();
                AForge.Point Furthest = new AForge.Point();
                bool FurhtestAssinged = false;
                LineSegment seg;
                dist = 0;
                for (int i = 0; i < Outline.Count; i++) {
                    if (i == LongestInd) {
                        continue;
                    }
                    if (i != Outline.Count - 1) {
                        seg = new LineSegment(Outline[i], Outline[i + 1]);
                    } else {
                        // last iteration
                        if (Outline[i] == Outline[0]) {
                            break;
                        }
                        seg = new LineSegment(Outline[i], Outline[0]);
                    }
                    if (seg.GetIntersectionWith(Normal) == null) {
                        continue;
                    }
                    InterSection = (AForge.Point)seg.GetIntersectionWith(Normal);
                    if (InterSection.DistanceTo(LongestCenter) > dist) {
                        Furthest = InterSection;
                        FurhtestAssinged = true;
                        dist = InterSection.DistanceTo(LongestCenter);
                    }
                }
                // Check, if there is a edge point that is close to the normal even further
                AForge.Point fPoint = new AForge.Point();
                for (int i = 0; i < Outline.Count; i++) {
                    fPoint.X = Outline[i].X;
                    fPoint.Y = Outline[i].Y;
                    if (Normal.DistanceToPoint(fPoint) < 1.5) {
                        if (fPoint.DistanceTo(LongestCenter) > dist) {
                            Furthest = fPoint;
                            FurhtestAssinged = true;
                            dist = fPoint.DistanceTo(LongestCenter);
                        }
                    }
                }
                AForge.Point ComponentCenter = new AForge.Point();
                if (FurhtestAssinged) {
                    // Find the midpoint of LongestCenter and Furthest: This is the centerpoint of component
                    ComponentCenter.X = (float)Math.Round((LongestCenter.X - Furthest.X) / 2.0 + Furthest.X);
                    ComponentCenter.Y = (float)Math.Round((LongestCenter.Y - Furthest.Y) / 2.0 + Furthest.Y);
                    // Alignment is the angle of longest
                    double Alignment;
                    if (Math.Abs(Longest.End.X - Longest.Start.X) < 0.001) {
                        Alignment = 0;
                    } else {
                        Alignment = Math.Atan((Longest.End.Y - Longest.Start.Y) / (Longest.End.X - Longest.Start.X));
                        Alignment = Alignment * 180.0 / Math.PI; // in deg.
                    }
                    Components.Add(new Shapes.Component(ComponentCenter, Alignment, Outline, Longest, NormalStart, NormalEnd));
                }
            }
            SetCamera(Components);
            return Components;
        }
        public List<Shapes.Component> FindComponents() {
            return FindComponents(camera.GetMeasurementFrame());
        }
        // standard paradigms



        public Shapes.Fiducal GetClosestTemplate(double tolerance) {
            return GetClosest(GetWithin(FindTemplates(), tolerance));
        }

       
        public Shapes.Circle GetClosestCircle(double tolerance) {
            camera.MainForm.DisplayText("GetClosestCircle(" + tolerance + ")", Color.Orange);
           /* var circles = FindCircles();
            foreach (var c in circles) Console.WriteLine("found circle @ " + c);
            circles = GetWithin(circles, tolerance);
            foreach (var c in circles) Console.WriteLine("tolerance found circle @ " + c);
            var circle = GetClosest(circles);
            Console.WriteLine("closest @ " + circle);
            return circle; */
            return GetClosest( GetWithin( FindCircles(), tolerance) );
        }

        public Shapes.Circle GetClosestAverageCircle(double tolerance, double retries) {
            List<Shapes.Circle> circles = new List<Shapes.Circle>();
            while (retries-- > 0) circles.Add(GetClosestCircle(tolerance));
            return AverageLocation(circles);
        }



        
        // Sorting Routines ================================

        public void SetCamera<T>(List<T> list) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            if (this.camera == null) throw new Exception("SetCamera / VideoDetection doesn't have vlid camera object set");
            foreach (var x in list) x.camera = this.camera;
        }

        public int GetCountWithin<T>(List<T> list, double distanceMM) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            return GetWithin(list, distanceMM).Count;
        }
            
        public List<T> GetWithin<T>(List<T> list, double distanceMM) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            foreach (var x in list) {
                x.ToMMResolution();
              //  Console.WriteLine("circle "+x+" distance = "+x.DistanceFrom(new PartLocation(0,0)));
            }

            var ret = list.Where(x => x.DistanceFrom(new PartLocation(0, 0)) <= distanceMM).ToList();
            foreach (var x in ret) {
             //   Console.WriteLine("circle "+x+" within tolerance "+distanceMM);
            }
            return ret;

        }

        public Shapes.Rectangle GetSmallestCenteredRectangle(List<Shapes.Rectangle> list) {
            list.RemoveAll(x => x == null);
            if (list == null || list.Count == 0) return null;
           // remove entries not containing the center point

            foreach (var x in list) x.ToScreenResolution();
            list.RemoveAll( x => (!(x.Left < 0 && x.Right > 0 && x.Top > 0 && x.Bottom < 0)));
            if (list.Count == 0) return null;
            return list.Aggregate((c, d) => c.Area() < d.Area() ? c : d);
        }

        public T GetClosest<T>(List<T> list) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            if (list == null || list.Count == 0) return null;
            foreach (var x in list) x.ToMMResolution();
            PartLocation centerPoint = new PartLocation(0, 0);
            return list.Aggregate((c, d) => c.DistanceFrom(centerPoint) < d.DistanceFrom(centerPoint) ? c : d);
        }

        /// <summary>
        /// This assumes that the list is all repeated measurements of the same part - this will return one part with the average value for X,Y
        /// </summary>
        public T AverageLocation<T>(List<T> list) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            if (list.Count == 0) return null;
            double xx = 0, yy = 0, aa = 0;
            foreach (var x in list) { xx += x.X; yy += x.Y; aa += x.A; }
            list[0].X = xx / list.Count;
            list[0].Y = yy / list.Count;
            list[0].A = aa / list.Count;
            return list[0];
        }

    }
}

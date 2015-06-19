using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AForge;
using AForge.Imaging;
using AForge.Math.Geometry;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using LitePlacer.Properties;
using Point = AForge.Point;

namespace LitePlacer {
    public class VideoDetection {

        /************** These functions are intelligent about their resolution/offset and can be changed dynamically *****/


        public static List<Shapes.Circle> FindCircles(VideoProcessing vp) {
            return FindCircles(vp, vp.GetMeasurementFrame());
        }


        /// <summary>
        /// Finds circles - zoom is NOT compensated for in returned results
        /// </summary>
        /// <returns>Graphic coordinates with zoom not accounted for for found circles</returns>
        public static List<Shapes.Circle> FindCircles(VideoProcessing vp, Bitmap frame) {
            // locating objects
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;
            blobCounter.ProcessImage(frame);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            List<Shapes.Circle> Circles = new List<Shapes.Circle>();

            for (int i = 0, n = blobs.Length; i < n; i++) {
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius)) {
                    if (radius > 3) {  // filter out some noise
                        var circle = new Shapes.Circle(center.X, center.Y, radius);
                        Circles.Add(circle);
                    }
                }
            }
            SetVideoProcessing(Circles, vp);
            return (Circles);
        }

        public static List<Shapes.Rectangle> FindRectangles(VideoProcessing vp) {
            return FindRectangles(vp, vp.GetMeasurementFrame());
        }
        public static List<Shapes.Rectangle> FindRectangles(VideoProcessing vp, Bitmap frame) {
            
            List<Shapes.Rectangle> rects = new List<Shapes.Rectangle>();

            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(frame)) {

              //  double cannyThresholdLinking = 120.0;
              //  double cannyThreshold = 180.0;

                //Convert the image to grayscale and filter out the noise
                Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();
                //Image<Gray, Byte> cannyEdges = gray.Canny(cannyThreshold, cannyThresholdLinking);

                using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                    for ( Contour<System.Drawing.Point> contours = gray.FindContours(
                          CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                          RETR_TYPE.CV_RETR_LIST,
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
                                    rects.Add(r);
                                }

                            }
                        }
                 }
            }
            SetVideoProcessing(rects, vp);
            return rects;
        }

        //gooz






        /// <summary>
        /// This function will take a bitmap image and find the locations of a template image in the file for a given threhsold
        /// </summary>
        /// <param name="image">Bitmap of image to look at</param>
        /// <param name="template_filename">File containg template image</param>
        /// <returns>imagefinder object</returns>
        public static List<Shapes.Fiducal> FindTemplates(VideoProcessing vp, Bitmap image, string template_filename, double threshold) {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(image);

            // template plus 90deg roated version of the template
            Image<Bgr, Byte> templ1 = new Image<Bgr, byte>(template_filename);
            Image<Bgr, Byte> templ2 = new Image<Bgr, byte>(templ1.Height, templ1.Width);
            CvInvoke.cvTranspose(templ1.Ptr, templ2.Ptr);

            ImageFinder imageFinder = new ImageFinder(img, threshold);
            imageFinder.FindImage(templ1);
            imageFinder.FindImage(templ2);

            var ret = imageFinder.Points.Select(x => new Shapes.Fiducal(x.X, x.Y)).ToList();

            SetVideoProcessing(ret, vp);
            return ret;
        }


        /// <summary>
        /// Will use default settings for finding the template
        /// </summary>
        /// <param name="image">the image to search</param>
        /// <returns>list of fiducary points</returns>
        public static List<Shapes.Fiducal> FindTemplates(VideoProcessing vp) {
            return FindTemplates(vp, vp.GetMeasurementFrame(), Settings.Default.template_file, Settings.Default.template_threshold);
        }

        public static List<Shapes.Fiducal> FindTemplates(VideoProcessing vp, Bitmap image) {
            return FindTemplates(vp, image, Settings.Default.template_file, Settings.Default.template_threshold);
        }

        public static List<Shapes.Component> FindComponents(VideoProcessing vp) {
            return FindComponents(vp, vp.GetMeasurementFrame());
        }
        public static List<Shapes.Component> FindComponents(VideoProcessing vp, Bitmap bitmap) {
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
                Point LongestCenter = new Point();
                LongestCenter.X = (float)Math.Round((Longest.End.X - Longest.Start.X) / 2.0 + Longest.Start.X);
                LongestCenter.Y = (float)Math.Round((Longest.End.Y - Longest.Start.Y) / 2.0 + Longest.Start.Y);
                Point NormalStart = new Point();
                Point NormalEnd = new Point();
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
                Point InterSection = new Point();
                Point Furthest = new Point();
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
                    InterSection = (Point)seg.GetIntersectionWith(Normal);
                    if (InterSection.DistanceTo(LongestCenter) > dist) {
                        Furthest = InterSection;
                        FurhtestAssinged = true;
                        dist = InterSection.DistanceTo(LongestCenter);
                    }
                }
                // Check, if there is a edge point that is close to the normal even further
                Point fPoint = new Point();
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
                Point ComponentCenter = new Point();
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
            SetVideoProcessing(Components, vp);
            return Components;
        }




       
        public static Shapes.Circle GetClosestCircle(VideoProcessing vp, double tolerance) {
            Global.Instance.DisplayText("GetClosestCircle(" + tolerance + ")", Color.Orange);
            return GetClosest( GetWithin( FindCircles(vp), tolerance) );
        }

        public static Shapes.Circle GetClosestAverageCircle(VideoProcessing vp, double tolerance, double retries) {
            List<Shapes.Circle> circles = new List<Shapes.Circle>();
            while (retries-- > 0) circles.Add(GetClosestCircle(vp, tolerance));
            return AverageLocation(circles);
        }



        
        // Sorting Routines ================================

        public static void SetVideoProcessing<T>(List<T> list, VideoProcessing vp) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            foreach (var x in list) x.videoProcessing = vp;
        }

        public static int GetCountWithin<T>(List<T> list, double distanceMM) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            return GetWithin(list, distanceMM).Count;
        }
            
        public static List<T> GetWithin<T>(List<T> list, double distanceMM) where T : Shapes.Thing {
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

        public static Shapes.Rectangle GetSmallestCenteredRectangle(List<Shapes.Rectangle> list) {
            list.RemoveAll(x => x == null);
            if (list == null || list.Count == 0) return null;
           // remove entries not containing the center point

            foreach (var x in list) x.ToScreenResolution();
            list.RemoveAll( x => (!(x.Left < 0 && x.Right > 0 && x.Top > 0 && x.Bottom < 0)));
            if (list.Count == 0) return null;
            return list.Aggregate((c, d) => c.Area() < d.Area() ? c : d);
        }

        public static T GetClosest<T>(List<T> list) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            if (list == null || list.Count == 0) return null;
            foreach (var x in list) x.ToMMResolution();
            PartLocation centerPoint = new PartLocation(0, 0);
            return list.Aggregate((c, d) => c.DistanceFrom(centerPoint) < d.DistanceFrom(centerPoint) ? c : d);
        }

        /// <summary>
        /// This assumes that the list is all repeated measurements of the same part - this will return one part with the average value for X,Y
        /// </summary>
        public static T AverageLocation<T>(List<T> list) where T : Shapes.Thing {
            list.RemoveAll(x => x == null);
            if (list.Count == 0) return null;
            double xx = 0, yy = 0, aa = 0;
            foreach (var x in list) { xx += x.X; yy += x.Y; aa += x.A; }
            list[0].X = xx / list.Count;
            list[0].Y = yy / list.Count;
            list[0].A = aa / list.Count;
            return list[0];
        }


        // higher levels tuff
        public static int MeasureClosestComponentInPx(out double X, out double Y, out double A, VideoProcessing vp, double Tolerance, int averages) {
            X = 0;
            double Xsum = 0;
            Y = 0;
            double Ysum = 0;
            A = 0.0;
            double Asum = 0.0;

            List<Shapes.Component> components = new List<Shapes.Component>();
            for (int i = 0; i < 5; i++) components.Add(
                GetClosest(
                GetWithin( FindComponents(vp), Tolerance )
                ));
            int count = components.Count;
            if (count == 0) return 0;
            foreach (var c in components) {
                Xsum += c.X;
                Ysum += c.Y;
                Asum += Global.ReduceRotation(c.A);
            }

            X = Xsum / count;
            Y = Ysum / count;
            A = -Asum / count;
            return count;
        }



        public static Shapes.Thing FindClosest(VideoProcessing vp, Shapes.ShapeTypes type, double FindTolerance, int retries) {
            while (retries-- > 0) {
                for (int i = 0; i < 10; i++) vp.GetMeasurementFrame(); //skip 10 frames
                Shapes.Thing thing = null;
                switch (type) {
                    case Shapes.ShapeTypes.Circle:
                        thing = GetClosestCircle(vp,FindTolerance);
                        break;
                    case Shapes.ShapeTypes.Fiducial:
                        var things = FindTemplates(vp);
                        thing = GetClosest( GetWithin( things, FindTolerance));
                        break;
                    case Shapes.ShapeTypes.Rectangle:
                        thing = GetSmallestCenteredRectangle(FindRectangles(vp));
                        break;
                    default:
                        Global.Instance.DisplayText("detection of " + type + " not yet supported", Color.Red);
                        break;
                }
                if (thing != null) return thing;
            }
            return null;
        }



    }
}

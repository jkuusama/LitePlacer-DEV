using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;

// based on post http://www.emgu.com/forum/viewtopic.php?f=7&t=3056&p=10417&hilit=template#p10417

namespace LitePlacer {
    public class ImageFinder {
        private List<Rectangle> rectangles;
        public Stopwatch stopwatch; //meaures time to find fiducical

        public List<Point> Points;
        public Image<Bgr, Byte> BaseImage { get; set; }
        public Image<Bgr, Byte> SubImage { get; set; }
        public Image<Bgr, Byte> ResultImage { get; set; }
        public Bitmap ResultBitmap {
            get { return ResultImage.ToBitmap(); }
        }
        public double Threashold { get; set; }

        public List<Rectangle> Rectangles {
            get { return rectangles; }
        }

        public ImageFinder(Image<Bgr, Byte> baseImage, double threashold) {
            rectangles = new List<Rectangle>();
            stopwatch = new Stopwatch();
            BaseImage = baseImage;
            Threashold = threashold;
            Points = new List<Point>();
        }

        public ImageFinder(Image<Bgr, Byte> baseImage) {
            rectangles = new List<Rectangle>();
            stopwatch = new Stopwatch();
            BaseImage = baseImage;
            Threashold = 0.9;
            Points = new List<Point>();
        }

        public void MarkLocationsOnImage() {
            ResultImage = BaseImage.Copy();
            foreach (var pt in this.Points) {
                ResultImage.Draw(new Cross2DF((PointF)pt, 10, 10), new Bgr(Color.Red), 2);
            }
        }

        public void FindImage(Image<Bgr, Byte> subImage) {
            SubImage = subImage;
            FindImage();
        }

        public void FindImage() {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            Image<Bgr, Byte> imgSrc = BaseImage.Copy();

            // FindImage all occurences of imgFind
            while (true) {
                using (Image<Gray, float> result = imgSrc.MatchTemplate(SubImage, Emgu.CV.CvEnum.TM_TYPE.CV_TM_CCOEFF_NORMED)) {

                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                    // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
                    if (maxValues[0] > Threashold) {
                        // This is a match. Do something with it, for example draw a rectangle around it.
                        Rectangle match = new Rectangle(maxLocations[0], SubImage.Size);
                        Points.Add(new Point(maxLocations[0].X + SubImage.Size.Width / 2,
                                             maxLocations[0].Y + SubImage.Size.Height / 2));

                        // Fill the drawing with red in order to ellimate this as a source.
                        imgSrc.Draw(match, new Bgr(Color.Red), -1);

                        // Add the found rectangle to the results.
                        rectangles.Add(match);
                    } else {
                        break;
                    }
                }
            }

            imgSrc.Dispose();
            stopwatch.Stop();
            
        }




    }
}

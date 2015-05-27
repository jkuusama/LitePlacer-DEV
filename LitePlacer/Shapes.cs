using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;
using AForge.Math.Geometry;

namespace LitePlacer
{
	public class Shapes
	{
        public enum ShapeTypes {
            Thing,
            Circle,
            Fiducial
        };

		public class Component
		{
			public AForge.Point Center { get; set; }	// Centerpoint of component
			public double Alignment { get; set; }		// angle of component
			public List<IntPoint> Outline { get; set; }
			public LineSegment Longest { get; set; }	// Longest line segment in Outline (needed in drawing, avoid calculating twice)
			public AForge.Point NormalStart { get; set; }  // (needed in drawing, avoid calculating twice)
			public AForge.Point NormalEnd { get; set; }		// (needed in drawing, avoid calculating twice)

			public Component(AForge.Point centr, double alignmnt, List<IntPoint> outln, 
							 LineSegment lngst, AForge.Point Nstart, AForge.Point Nend)
			{
				Center = centr;
				Alignment = alignmnt;
				Outline = outln;
				Longest = lngst;
				NormalStart = Nstart;
				NormalEnd = Nend;
			}
		}

        /// <summary>
        ///  Generic class for an object that is located someplace
        /// </summary>
        public class Thing {
            public double X { get; set; }
            public double Y { get; set; }

            private bool _pixelMode = true;
            private double zoom = 1d;

            public void SetPixelMode() {
                if (_pixelMode) return;
                X /= Properties.Settings.Default.DownCam_XmmPerPixel / zoom;
                Y /= Properties.Settings.Default.DownCam_YmmPerPixel / zoom;
                _pixelMode = true;
            }

            public void SetMMMode(double zoom) {
                if (!_pixelMode) return;
                X *= Properties.Settings.Default.DownCam_XmmPerPixel / zoom;
                Y *= Properties.Settings.Default.DownCam_YmmPerPixel / zoom;
                this.zoom = zoom;
                _pixelMode = false;
            }

            public PartLocation ToPartLocation() {
                return new PartLocation(X, Y);
            }

            public AForge.Point Center { 
                get { return new AForge.Point((float)this.X, (float)this.Y); }
                set { X = value.X; Y = value.Y; }
            }
            public Thing(double x, double y) {
                X = x;
                Y = y;
            }

            public double DistanceFrom(Thing thing) {
                return DistanceFrom(thing.X, thing.Y);
            }

            public double DistanceFrom(AForge.Point point) {
                return DistanceFrom(point.X, point.Y);
            }

            public double DistanceFrom(double x, double y) {
                return Math.Sqrt(Math.Pow((x - X), 2) + Math.Pow((y - Y), 2));
            }

            public Point VectorFrom(double x, double y) {
                return new Point((float)(X - x), (float)(Y - y));
            }

            /// <summary>
            /// Distance from this Thing to x,y divided by zoom
            /// </summary>
            public Point VectorFrom(double x, double y, double zoom) {
                return new Point((float)((X - x)/zoom), (float)((Y - y)/zoom));
            }

        }

		public class Circle : Thing {
			public double Radius { get; set; }

			public Circle(double x, double y, double r) : base(x,y) {
				Radius = r;
			}
            public override string ToString() {
                return String.Format("({0},{1} R={2})", X, Y, Radius);
            }
		}

        public class Fiducal : Thing {
            public Fiducal(double x, double y) : base(x, y) { }
        }


	}


}

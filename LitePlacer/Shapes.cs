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
            Fiducial,
            Component,
            Rectangle
        };

        public enum PointMode {
            Raw, //The camera geometry
            Screen, //same zoom as raw, but centered on the screen with the y axis flipped (table dimensions)
            MM, //A physical distance from the camera center offset (table dimentions) 
            ScreenUnzoomed //same as screen, but with the zoom removed (for mmPerPixel calculations)
        };

        

        /// <summary>
        ///  Generic class for an object that is located someplace
        /// </summary>
        public class Thing {
            public double X { get; set; }
            public double Y { get; set; }
            public double A { get; set; } 
            public double Radians { 
                get { return A * Math.PI / 180d; } 
                set { A = value * 180d / Math.PI; }
            }

            protected PointMode pointMode = PointMode.Raw;
            public Camera camera; //what camera was used to aquire the 
            
            public PartLocation ToPartLocation() {;
                return new PartLocation(X, Y);
            }

            public AForge.Point Center { 
                get { return new AForge.Point((float)this.X, (float)this.Y); }
                set { X = value.X; Y = value.Y; }
            }

            public Thing(PartLocation p, PointMode mode) {
                X = p.X;
                Y = p.Y;
                A = p.A;
                pointMode = mode;
            }

            public Thing(double x, double y) {
                X = x;
                Y = y;
            }

            public Thing(double x, double y, double a) {
                X=x; Y=y; A=a;
            }

            public static PartLocation Convert(Camera cam, PartLocation p, PointMode fromMode, PointMode toMode) {
                Thing t = new Thing(p, fromMode);
                t.camera = cam;
                switch (toMode) {
                    case PointMode.MM: return t.ToMMResolution().ToPartLocation(); 
                    case PointMode.Raw: return t.ToRawResolution().ToPartLocation(); 
                    case PointMode.Screen: return t.ToScreenResolution().ToPartLocation(); 
                    case PointMode.ScreenUnzoomed: return t.ToScreenUnzoomedResolution().ToPartLocation();
                }
                return null;
            }





            public double DistanceFrom(PartLocation p) {
                return DistanceFrom(p.X, p.Y);
            }

 
            public double DistanceFrom(double x, double y) {
                return Math.Sqrt(Math.Pow((x - X), 2) + Math.Pow((y - Y), 2));
            }

            /// <summary>
            /// This is the resolution and zoom of things we see on the screen.
            /// The image is simply shifted and flipped
            /// </summary>
            /// <param name="camera"></param>
            /// <returns></returns>
            public virtual Thing ToScreenResolution() {
                var zoom = camera.GetMeasurementZoom();
                switch (pointMode) {
                    case PointMode.ScreenUnzoomed:                        
                        X *= zoom;
                        Y *= zoom;
                        break;
                    case PointMode.Raw:                        
                        X = (X - camera.FrameCenterX);
                        Y = (camera.FrameCenterY - Y); //flip Y axis to switch from graphic coordinates to table coordinates
                        break;
                    case PointMode.MM:                        
                        // convert to unzoomed pixels
                        X = X / camera.XmmPerPixel * zoom; 
                        Y = Y / camera.YmmPerPixel * zoom;
                        // apply appropriate zoom
                        break;
                }
                pointMode = PointMode.Screen;
                return this;
            }
                        
            /// <summary>
            /// This is the MM representation of what we see on the table
            /// </summary>
            /// <param name="camera"></param>
            /// <returns></returns>
            public virtual Thing ToMMResolution() {
                var zoom = camera.GetMeasurementZoom();
                switch (pointMode) {
                    case PointMode.ScreenUnzoomed:
                        X *= camera.XmmPerPixel ;
                        Y *= camera.YmmPerPixel ;
                        break;
                    case PointMode.Raw:
                        this.ToScreenResolution();
                        X *= camera.XmmPerPixel / zoom;
                        Y *= camera.YmmPerPixel / zoom;
                        break;
                    case PointMode.Screen:
                        X *= camera.XmmPerPixel / zoom;
                        Y *= camera.YmmPerPixel / zoom;
                        break;
                }
                pointMode = PointMode.MM;
                return this;
            }

            public virtual Thing ToRawResolution() {
                if (camera == null) throw new Exception("Camera Not Set For Object " + this);
                var zoom = camera.GetMeasurementZoom();
                switch (pointMode) {
                    case PointMode.ScreenUnzoomed:
                        X *= zoom;
                        Y *= zoom;
                        pointMode = PointMode.Screen;
                        return ToRawResolution();
                    case PointMode.Screen:
                        X = X + camera.FrameCenterX;
                        Y = camera.FrameCenterY - Y;
                        break;
                    case PointMode.MM:
                        this.ToScreenResolution();
                        X = X + camera.FrameCenterX;
                        Y = camera.FrameCenterY - Y;
                        break;
                }
                pointMode = PointMode.Raw;
                return this;
            }

            public virtual Thing ToScreenUnzoomedResolution() {
                var zoom = camera.GetMeasurementZoom();
                switch (pointMode) {
                    case PointMode.Screen:
                        X /= zoom;
                        Y /= zoom;
                        break;
                    case PointMode.Raw:
                        this.ToScreenResolution();
                        X /= zoom;
                        Y /= zoom;
                        break;
                    case PointMode.MM:
                        this.ToScreenResolution();
                        X /= zoom;
                        Y /= zoom;
                        break;
                }
                pointMode = PointMode.ScreenUnzoomed;
                return this;
            }


            public virtual Thing Clone() {
                var x = new Thing(X, Y, A);
                x.camera = this.camera;
                x.pointMode = this.pointMode;
                return x;
            }
            

        }

        /****************************************************************************************/
        public class Component : Thing {

            public double Alignment { get { return A; } set { A = value; } }		// angle of component
            public List<IntPoint> Outline { get; set; }
            public LineSegment Longest { get; set; }	// Longest line segment in Outline (needed in drawing, avoid calculating twice)
            public AForge.Point NormalStart { get; set; }  // (needed in drawing, avoid calculating twice)
            public AForge.Point NormalEnd { get; set; }		// (needed in drawing, avoid calculating twice)

            public Component(AForge.Point centr, double alignmnt, List<IntPoint> outln,
                             LineSegment lngst, AForge.Point Nstart, AForge.Point Nend)
                : base(centr.X, centr.Y) {
                Alignment = alignmnt;
                Outline = outln;
                Longest = lngst;
                NormalStart = Nstart;
                NormalEnd = Nend;
            }
        }



        /****************************************************************************************/
        /// <summary>
        /// This thing is a rectangle. The corrdinates represent the center
        /// </summary>
        public class Rectangle : Thing {
            public double Width {get;set;} //this will always be in raw mode so valid for raw & screen
            public double Height {get;set;}
            public double Left { get { return (X - Width/2); } }
            public double Right { get { return (X+Width/2); }}
            public double Top { get {
                if (pointMode== PointMode.Raw) return (Y-Width/2);
                else return (Y+Width/2);
            }}
            public double Bottom { get { 
                if (pointMode == PointMode.Raw) return (Y+Width/2);
                else return (Y-Width/2);
            }}


            public Rectangle(double x, double y) : base(x,y) {}
            public Rectangle(double x, double y, double height, double width) : base(x,y) {Height=height;Width=width;}
            public Rectangle(double x, double y, System.Drawing.Size s) : base(x,y) {Height= s.Height; Width=s.Width;}
            public Rectangle(PartLocation p) : base(p.X,p.Y) {}
            public Rectangle(PartLocation p, System.Drawing.Size s) : base(p.X,p.Y) { Height= s.Height; Width=s.Width;}
            public Rectangle(double x, double y, double angle) : base(x, y, angle) { }

            public double Area() {
                return Height * Width;
            }

            /// <summary>
            /// will return the angle offset from 90 with positive angles being counterclockwise
            /// </summary>
            public double AngleOffsetFrom90() {
                double angle = A;
                //rotate till we are within 45 degrees
                while (Math.Abs( angle ) > 45 )  angle += (angle<0) ? 90 : -90;
                //invert to give us the positive counterclockwise angle
                return -1*angle;
            }
                    
                    
            public override string ToString() {
                return String.Format("(Rect@{0:F2},{1:F2} A={4:F2} H={2:F2} W={3:F2})", X, Y, Height, Width,A);
            }

            public System.Drawing.Rectangle ToDrawingRectangle() {
                this.ToRawResolution();
                return new System.Drawing.Rectangle((int)(X - Width / 2), (int)(Y - Height / 2), (int)Width, (int)Height);
            }

        }


        /****************************************************************************************/
		public class Circle : Thing {
			public double Radius { get; set; }
            
			public Circle(double x, double y, double r) : base(x,y) {
				Radius = r;
                pointMode = PointMode.Raw;
			}

            public override string ToString() {
                return String.Format("({0:F2},{1:F2} R={2}pixels)", X, Y, Radius);
            }
		}

        public class Fiducal : Thing {
            public Fiducal(double x, double y) : base(x, y) { }
        }


	}


}

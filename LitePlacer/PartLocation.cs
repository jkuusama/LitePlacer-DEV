using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using MathNet.Numerics.LinearRegression;

namespace LitePlacer {
    public class PartLocation {
        public double X { get; set; }
        public double Y { get; set; }
        private double _A;
        public double A { get { return (_A % 360); } set { _A = (value % 360); } }

        public PhysicalComponent physicalComponent; //link back


        public PartLocation() { }
        public PartLocation(Shapes.Thing thing) {X = thing.X; Y = thing.Y; A = thing.A;}
        public PartLocation(PartLocation p) {X = p.X; Y = p.Y; A = p.A;}
        public PartLocation(PointF p) {X = p.X; Y = p.Y;}
        public PartLocation(double x, double y) {X = x; Y = y; A = 0;}
        public PartLocation(double x, double y, double a) {X = x; Y = y; A = a;}
        public PartLocation(Matrix<double> matrix) {
            if (matrix.Height == 2 && matrix.Width == 1)
                Y = matrix[1, 0];
            else if (matrix.Height == 1 && matrix.Width == 2)
                Y = matrix[0, 1];
            else throw new Exception("Cannot convert " + matrix.Height + "X" + matrix.Width + " matrix to PartLocation");
            X = matrix[0, 0];
        }

        /* modification methods */
        public PartLocation FlipX() { X = -1 * X; return this; }
        public PartLocation FlipY() { Y = -1 * Y; return this; }
        public PartLocation OffsetBy(double x, double y, double a) { X += x; Y += y; A += a; return this; }
        public PartLocation OffsetBy(double x, double y) { X += x; Y += y; return this; }
        public PartLocation OffsetBy(PartLocation p) { X += p.X; Y += p.Y; A += p.A; return this;}

        

        public double ToRadians() {return Math.Atan2(Y, X);  }
        public double ToDegrees() {return ToRadians() * 180 / Math.PI;   }
        public Matrix<double> ToMatrix() { var m = new Matrix<double>(2, 1); m[0, 0] = X; m[1, 0] = Y; return m; }

        public double DistanceTo(PartLocation p) {  return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2));   }

        public override string ToString() {
            return String.Format("({0},{1},{2})", X, Y, A);
        }

        /// <summary>
        /// This will rotate the X,Y vector by radians and leave A alone.
        /// It will return itself
        /// </summary>
        public PartLocation Rotate(double radians) {
            var x2 = X * Math.Cos(radians) - Y * Math.Sin(radians);
            var y2 = X * Math.Sin(radians) + Y * Math.Cos(radians);
            X = x2;
            Y = y2;
            return this;
        }

        public static PartLocation operator +(PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X + p2.X, p1.Y + p2.Y, p1.A + p2.A);
        }
        public static PartLocation operator -(PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X - p2.X, p1.Y - p2.Y, p1.A - p2.A);
        }
        public static PartLocation operator +(PartLocation p1, PointF p2) {
            return new PartLocation(p1.X + p2.X, p1.Y + p2.Y, p1.A);
        }

        public static PartLocation operator -(PartLocation p1, PointF p2) {
            return new PartLocation(p1.X - p2.X, p1.Y - p2.Y, p1.A);
        }

        /// <summary>
        /// This will multiply the X & Y Components with each other, an return p1's A
        /// </summary>
        public static PartLocation operator *(PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X * p2.X, p1.Y * p2.Y, p1.A);
        }

        /// <summary>
        /// This will divide the X & Y Components with each other, an return p1's A
        /// </summary>
        public static PartLocation operator /(PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X / p2.X, p1.Y / p2.Y, p1.A);
        }

        public static PartLocation operator *(double scalar, PartLocation p) {
            return new PartLocation(scalar * p.X, scalar * p.Y, p.A);
        }
        /// <summary>
        /// (scalar / p.X, scalar / P.Y, p.A)
        /// </summary>
        public static PartLocation operator /(double scalar, PartLocation p) {
            return new PartLocation(scalar / p.X, scalar / p.Y, p.A);
        }

        public double VectorLength() {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public static double MeasureSlope(PartLocation[] list) {
            // Fit  to linear regression // y:x->a+b*x
            Double[] Xs = list.Select(xx => xx.X).ToArray();
            Double[] Ys = list.Select(xx => xx.Y).ToArray();
            Tuple<double, double> result = SimpleRegression.Fit(Xs, Ys);
            //x.a = result.Item1; //this should be as close to zero as possible if things worked correctly
            return result.Item2; //this represents the slope of the tape            
        }

        public PointF ToPointF() {
            return new PointF((float)X, (float)Y);
        }
        public Point ToPoint() {
            return new Point((int)X, (int)Y);
        }

        /// <summary>
        /// Returns a new list of points offset by X andd Y
        /// </summary>
        public static List<PartLocation> Offset(List<PartLocation> list, PartLocation offset_by) {
            var newlist = new List<PartLocation>(list.Count);
            foreach (var x in list) {
                newlist.Add(new PartLocation(x.X+offset_by.X, x.Y+offset_by.Y, x.A+offset_by.A));
            }
            return newlist;
        }

        /// <summary>
        /// will return the mean of the x,y,z coordinates
        /// </summary>
        public static PartLocation Average(List<PartLocation> list) {
            double xx = 0, yy = 0, aa = 0;
            foreach (var p in list) {
                xx += p.X;
                yy += p.Y;
                aa += p.A;
            }
            int c = list.Count;
            return new PartLocation(xx / c, yy / c, aa / c);
        }

        /// <summary>
        /// Returns the absolute value of the maximum of the x,y,z entries
        /// </summary>
        public static PartLocation MaxValues(List<PartLocation> list) {
            double xx = 0, yy = 0, aa = 0;
            foreach (var p in list) {
                xx = (Math.Abs(p.X) > xx) ? Math.Abs(p.X) : xx;
                yy = (Math.Abs(p.Y) > yy) ? Math.Abs(p.Y) : yy;
                aa = (Math.Abs(p.A) > aa) ? Math.Abs(p.A) : aa;
            }
            return new PartLocation(xx, yy, aa);
        }
    }


  

}

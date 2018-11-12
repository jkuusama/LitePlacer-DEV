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
        public class Shape
        {
            public AForge.Point Center { get; set; }    // Centerpoint of a shape
            public double Angle { get; set; }       // Rotation of a shape
            // Note: Either we have rotation on a circle (that doesn't have a meaning) or we need to complicate code to
            // make the difference. I chose the former, so angle is here.
        }


        public class Component: Shape
        {
            public List<IntPoint> Outline { get; set; }
            public LineSegment Longest { get; set; }    // Longest line segment in Outline (needed in drawing, avoid calculating twice)
            public AForge.Point NormalStart { get; set; }  // (needed in drawing, avoid calculating twice)
            public AForge.Point NormalEnd { get; set; }     // (needed in drawing, avoid calculating twice)

            public Component(AForge.Point centr, double angl, List<IntPoint> outln,
                             LineSegment lngst, AForge.Point Nstart, AForge.Point Nend)
            {
                Center = centr;
                Angle = angl;
                Outline = outln;
                Longest = lngst;
                NormalStart = Nstart;
                NormalEnd = Nend;
            }

        }

        public class Rectangle : Shape
        {
            public List<IntPoint> Corners { get; set; }
            public double LongsideLenght { get; }
            public double ShortSideLenght { get; }

            public Rectangle(List<IntPoint> corners)
            {
                Corners = corners;
                AForge.Point C = new AForge.Point();
                double x0 = corners[0].X;
                double y0 = corners[0].Y;
                double x1 = corners[1].X;
                double y1 = corners[1].Y;
                double x2 = corners[2].X;
                double y2 = corners[2].Y;


                C.X = (float)(((x2 - x0) / 2.0) + x0);
                C.Y = (float)(((y2 - y0) / 2.0) + y0);
                Center = C;
                double dist1 = new LineSegment(corners[0], corners[1]).Length;
                double dist2 = new LineSegment(corners[1], corners[2]).Length;
                double A = Math.Atan(Math.Abs((y1 - y0) / (x1 - x0)));
                A = A * 180.0 / Math.PI; // in deg.

                if (dist1>dist2)
                {
                    LongsideLenght = dist1;
                    ShortSideLenght = dist2;
                    // note: x0 is lowest, cor0..cor1 is long side. Angle is 0..90
                    Angle = A;
                }
                else
                {
                    LongsideLenght = dist2;
                    ShortSideLenght = dist1;
                    // corn0..corn1 is short side. Angle is 0..-90
                    Angle = A - 90.0;
                }
            }
        }

        public class Circle : Shape
        {
			public double Radius { get; set; }

			public Circle(AForge.Point centr, double r)
			{
                Center = centr;
                Radius = r;
			}
		}

	}


}

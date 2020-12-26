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
            public double Angle { get; set; }       // Rotation of a shape; zero is parallel to X axis
            // Note: Either we have rotation on a circle (that doesn't have a meaning) or we need to complicate code to
            // make the difference. I chose the former, so angle is here.
            public double Xsize;
            public double Ysize;
        }


        public class LitePlacerShapeComponent : Shape
        {
    #pragma warning disable CA2227 // Collection properties should be read only
            public List<IntPoint> Outline { get; set; }  // Compiler does not see tht this is indeed used(?), need to disable the warning
#pragma warning restore CA2227
            public LineSegment Longest { get; set; }    // Longest line segment in Outline (needed in drawing, avoid calculating twice)
            public AForge.Point NormalStart { get; set; }  // (needed in drawing, avoid calculating twice)
            public AForge.Point NormalEnd { get; set; }     // (needed in drawing, avoid calculating twice)

            public LitePlacerShapeComponent(AForge.Point centr, double angl, List<IntPoint> outln,
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

            public Rectangle(List<IntPoint> cornersIn)
            {
                // corners are in clockwise order, but not necessarily
                // in order. Rearrange so, that x0 is lowest:
                // Find lowest x0
                double lowest= cornersIn[0].X;
                int lowIndex = 0;
                for (int i = 1; i < 4; i++)
                {
                    if (cornersIn[i].X< lowest)
                    {
                        lowIndex = i;
                        lowest = cornersIn[i].X;
                    }
                }
                Corners = new List<IntPoint>();
                for (int i = 0; i < 4; i++)
                {
                    Corners.Add(cornersIn[lowIndex++]);
                    if (lowIndex>3)
                    {
                        lowIndex = 0;
                    }
                }

                double x0 = Corners[0].X;
                double y0 = Corners[0].Y;
                double x1 = Corners[1].X;
                double y1 = Corners[1].Y;
                double x2 = Corners[2].X;
                double y2 = Corners[2].Y;
                double x3 = Corners[3].X;
                double y3 = Corners[3].Y;


                AForge.Point C = new AForge.Point();
                C.X = (float)(((x2 - x0) / 2.0) + x0);
                C.Y = (float)(((y2 - y0) / 2.0) + y0);
                Center = C;

                if (Math.Abs(x0-x1)<0.00001)    // x0 == x1
                {
                    Angle = 0.0;
                    Xsize = Math.Abs(x2 - x1);
                    Ysize = Math.Abs(y0 - y1);
                    return;
                }

                double A;
                A = Math.Atan((x1 - x0) / (y1 - y0));
                A = A * 180.0 / Math.PI; // in deg.
                if (A<-45.0)
                {
                    A = A + 90.0;
                }
                Angle = A; // -45 < A < 45

                // if A < 0: Xsize = c[1] to c[2], Ysize = c[0] to c[1]
                // if A > 0: Xsize = c[0] to c[1], Ysize= c[1] to c[2]

                double c0toc1 = Math.Sqrt(((x0 - x1) * (x0 - x1)) + ((y0 - y1) * (y0 - y1)));
                double c1toc2 = Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));

                if (A< 0.0)
                {
                    Xsize = c1toc2;
                    Ysize = c0toc1;
                }
                else
                {
                    Xsize = c0toc1;
                    Ysize = c1toc2;
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
                Angle = 0.0;
                Xsize = r * 2.0;
                Ysize = r * 2.0;
            }
        }

        public class Component : Shape
            // for now, equivalent to rectangle, but we'll see if this needs to change...
        {
            public Rectangle BoundingBox { get; set; }

            public Component(List<IntPoint> corners)
            {
                BoundingBox = new Rectangle(corners);
                Angle = BoundingBox.Angle;
                Center = BoundingBox.Center;
                Xsize = BoundingBox.Xsize;
                Ysize = BoundingBox.Ysize;

            }
        }

    }


}

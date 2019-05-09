using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;
using AForge.Math.Geometry;

namespace LitePlacer
{
    public class LitePlacerShapeComponent
    {
        public AForge.Point Center { get; set; }    // Centerpoint of component
        public double Alignment { get; set; }       // angle of component
#pragma warning disable CA2227 // Collection properties should be read only
        public List<IntPoint> Outline { get; set; }  // Compiler does not see tht this is undeed used(?), need to disable the warning
#pragma warning restore CA2227 // Collection properties should be read only
        public LineSegment Longest { get; set; }    // Longest line segment in Outline (needed in drawing, avoid calculating twice)
        public AForge.Point NormalStart { get; set; }  // (needed in drawing, avoid calculating twice)
        public AForge.Point NormalEnd { get; set; }     // (needed in drawing, avoid calculating twice)

        public LitePlacerShapeComponent(AForge.Point centr, double alignmnt, List<IntPoint> outln,
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

    public class LitePlacerShapeRectangle
    {
        public AForge.Point Center { get; set; }    // Centerpoint
        public float Alignment { get; set; }       // angle
#pragma warning disable CA2227 // Collection properties should be read only
        public List<IntPoint> Corners { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public LitePlacerShapeRectangle(AForge.Point centr, float alignmnt, List<IntPoint> crnrs)
        {
            Center = centr;
            Alignment = alignmnt;
            Corners = crnrs;
        }
    }

    public class LitePlacerShapeCircle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; }

        public LitePlacerShapeCircle(double x, double y, double r)
        {
            X = x;
            Y = y;
            Radius = r;
        }
    }

}


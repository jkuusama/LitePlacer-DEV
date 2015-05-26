using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;


// http://www.yageo.com/exep/pages/download/literatures/UPY-C_GEN_15.pdf

namespace LitePlacer {
    public enum TapeType { White, Black, Clear }
    public enum TapeDefault { T0402, T0603, T0806 }

    public class PartLocation {
        public double X;
        public double Y;
        public double A;

        public PartLocation() {}
        public PartLocation(PartLocation p) {
            X = p.X; Y = p.Y; A = p.A;
        }
        public PartLocation(PointF p) {
            X=p.X; Y=p.Y;
        }
        public PartLocation(double x, double y) {
            X=x; Y=y; A=0;
        }
        public PartLocation(double x, double y, double a) {
            X=x; Y=y; A=a;
        }

        public double ToRadians() {
            return Math.Atan2(Y, X);
        }

        public PartLocation ToPixels() {
            return ToPixels(1d);
        }

        public PartLocation ToPixels(double zoom) {
            return new PartLocation(
                X / Properties.Settings.Default.DownCam_XmmPerPixel * zoom,
                Y / Properties.Settings.Default.DownCam_YmmPerPixel * zoom * -1,
                A);
        }
        
        /// <summary>
        /// This will rotate the X,Y vector by radians and leave A alone.
        /// It will return itself
        /// </summary>
        public  PartLocation Rotate(double radians) {
            var x2 = X * Math.Cos(radians) - Y * Math.Sin(radians);
            var y2 = X * Math.Sin(radians) + Y * Math.Cos(radians);
            X = x2;
            Y = y2;
            return this;
        }

        public static PartLocation operator + (PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X+p2.X, p1.Y+p2.Y, p1.A+p2.A);
        }
        public static PartLocation operator -(PartLocation p1, PartLocation p2) {
            return new PartLocation(p1.X - p2.X, p1.Y- p2.Y, p1.A - p2.A);
        }
        public static PartLocation operator + (PartLocation p1, PointF p2) {
            return new PartLocation(p1.X+p2.X, p1.Y+p2.Y, p1.A);
        }

        public static PartLocation operator - (PartLocation p1, PointF p2) {
            return new PartLocation(p1.X-p2.X, p1.Y-p2.Y, p1.A);
        }

        public static PartLocation operator *(double scalar, PartLocation p) {
            return new PartLocation(scalar * p.X, scalar * p.Y, p.A);
        }

        public PointF ToPointF() {
            return new PointF((float)X,(float)Y);
        }

    }
    
    public class TapeObj {
        public TapeType Type;
        public double HoleDiameter; //Do
        public double HolePitch; // P0
        public double PartPitch; // P1
        public double TapeWidth; //W
        public double HoleToPartSpacingY { get { return TapeWidth/2-.5; }} // F
        public double HoleToPartSpacingX; //P2
        public PartLocation FirstHole;
        public double PickupZ, PlaceZ;
        public int row;
        public bool isPickupZSet = false;
        public bool isPlaceZSet = false;

        // these 2 are used as directional vectors
        private PartLocation _TapeOrientation;
        private PartLocation _PartOrientation;
        public string TapeOrientation {
            get { return VectorToOrientation(_TapeOrientation); }
            set { _TapeOrientation = OrienationToVector(value); }
        }
        public string PartOrientation {
            get { return VectorToOrientation(_PartOrientation); }
            set { _PartOrientation = OrienationToVector(value); }
        }

        private int currentPart = 0;

        /// <summary>
        /// If given a reference to a DataGridViewCellCollection, it will
        /// auto-populate itself with what's in there and throw an exception 
        /// if it can't parse something
        /// </summary>
        /// <param name="rowData"></param>
        public TapeObj(DataGridViewCellCollection rowData) {
            HolePitch = 4d;
            HoleToPartSpacingX = -2d; // first part is to the "left" of the hole
            HoleDiameter = 1.5d;
            
            try {
                PartOrientation = rowData["RotationColumn"].Value.ToString();
                TapeOrientation = rowData["OrientationColumn"].Value.ToString();
                FirstHole = new PartLocation(float.Parse(rowData["X_Column"].Value.ToString()),
                                      float.Parse(rowData["Y_Column"].Value.ToString()));
                string tape_size = rowData["WidthColumn"].Value.ToString();
                string[] sizes = tape_size.Split(new string[] { "/", "mm" }, StringSplitOptions.RemoveEmptyEntries);
                TapeWidth = double.Parse(sizes[0]);
                PartPitch = double.Parse(sizes[1]);
                currentPart = int.Parse(rowData["Next_Column"].Value.ToString()); //XXX might need to subtract 1?
                switch (rowData["TypeColumn"].Value.ToString()) {
                    case "Paper (White)": Type = TapeType.White; break;
                    case "Black Plastic": Type = TapeType.Black; break;
                    case "Clear Plastic": Type = TapeType.Clear; break;
                    default:
                        throw new Exception("Unable to parse tape type");
                }
            } catch (Exception e) {
                throw new Exception("Tape: Unable to parse row data : "+e.ToString());
            }

            try {
                PlaceZ = double.Parse(rowData["PlaceZ_Column"].Value.ToString());
                isPlaceZSet = true;
            } catch { }

            try {
                PickupZ = double.Parse(rowData["PickupZ_Column"].Value.ToString());
                isPickupZSet = true;
            } catch { }

        }


        public TapeObj ( TapeDefault x ) {
            // pretty standard settings that can be overridden
            PartPitch = 2d;
            TapeWidth = 8d;
            HolePitch = 4d;
            HoleToPartSpacingX = -2d; // first part is to the "left" of the hole
            HoleDiameter = 1.5d;

            switch (x) {
                case TapeDefault.T0402:
                    PartPitch = 2d;
                    break;
                default:
                    PartPitch = 4d;
                    break;
            }
        }

        public int CurrentPartIndex() {
            return currentPart;
        }
        public void NextPart() {
            currentPart++;
        }
        public void SetPart(int part) {
            currentPart = part;
        }


        // PART //
        public PartLocation GetCurrentPartLocation() {
            return GetPartLocation(currentPart);
        }
        public PartLocation GetPartLocation(int componentNumber) {
            PartLocation offset = new PartLocation(componentNumber * PartPitch + HoleToPartSpacingX, HoleToPartSpacingY);
            // add the vector to the part rotated by the tape orientation
            PartLocation part = new PartLocation(FirstHole) + offset.Rotate(_TapeOrientation.ToRadians());
            part.A =_PartOrientation.ToRadians() * 180d / Math.PI;

            return part;
        }

        // HOLES //
        public PartLocation GetHoleLocation(int holeNumber) {
            return new PartLocation(FirstHole) + new PartLocation(holeNumber * HolePitch, 0).Rotate(_TapeOrientation.ToRadians());
        }

        public PartLocation GetNearestCurrentPartHole() {
            double distanceX = currentPart * PartPitch + HoleToPartSpacingX;
            int holeNumber = (int)(distanceX / HolePitch);
            return GetHoleLocation(holeNumber);
        }


        private PartLocation OrienationToVector(string orientation) {
            switch (orientation) {
                case "0deg.":   case "+X": return new PartLocation(1,0);
                case "90deg.":  case "+Y": return new PartLocation(0,1);
                case "180deg.": case "-X": return new PartLocation(-1,0);
                case "270deg.": case "-Y": return new PartLocation(0,-1);
                default:   throw new Exception("Invalid Orientation");
            }
        }

        private string VectorToOrientation(PartLocation x) {
            if (x.X == 1) return "+X";
            if (x.X == -1) return "-X";
            if (x.Y == 1) return "+Y";
            if (x.Y == -1) return "-Y";
            throw new Exception("Invalid Vector");
        }


    }
}


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
    public enum TapeDefault { T0402, T0603, T0806, QFN }

   
    
    public class TapeObj {
        public TapeType Type;
        public string ID; // name 
        public double HoleDiameter; //Do
        public double HolePitch; // P0
        public double PartPitch; // P1
        public double TapeWidth; //W
        public double HoleToPartSpacingY { get { return TapeWidth/2-.5; }} // F
        public double HoleToPartSpacingX; //P2
        public PartLocation FirstHole;
        public double _PickupZ, _PlaceZ;
        public int row;
        public bool isPickupZSet = false;
        public bool isPlaceZSet = false;
        public bool isFullyCalibrated = false;
        public double a; //coeffients of equation defining location of tape a + b*x
        private double _b;
        private int currentPart = 0;
        public DataGridViewCellCollection myRow;
        

        // these 2 are used as directional vectors
        private PartLocation _TapeOrientation;
        private PartLocation _PartOrientation;
        public string TapeOrientation {
            get { return (_TapeOrientation.ToRadians() * 180 / Math.PI).ToString() + " deg" ; }
            set {
                if (isFullyCalibrated) throw new Exception("Fully Calibrated Tape - updating TapeOrientation is not advised");
                _TapeOrientation = OrienationToVector(value); 
            }

        }

        public PartLocation PartOrientationVector {
            get { return _PartOrientation; }
            set { _PartOrientation = value; }
        }
        public string PartOrientation {
            get { return VectorToOrientation(_PartOrientation); }
            set { _PartOrientation = OrienationToVector(value); }
        }
        public string PickupZ {
            get { return (isPickupZSet) ? _PickupZ.ToString() : "--"; }
            set {
                    try {
                        _PickupZ = double.Parse(value);
                        isPickupZSet = true;
                    } catch {
                        isPickupZSet = false;
                    }
                }       
        }
        public string PlaceZ {
            get { return (isPlaceZSet) ? _PlaceZ.ToString() : "--"; }
            set {
                try {
                    _PlaceZ = double.Parse(value);
                    isPlaceZSet = true;
                } catch {
                    isPlaceZSet = false;
                }
            }
        }
        /// <summary>
        /// This is the slope of the line for the tape and is more accurate than the intial settings
        /// </summary>
        public double b {
            set {
                _b = value;
                _TapeOrientation = new PartLocation(1, value);
            }
            get {
                return _b;
            }
        }




        /// <summary>
        /// If given a reference to a DataGridViewCellCollection, it will
        /// auto-populate itself with what's in there and throw an exception 
        /// if it can't parse something
        /// </summary>
        /// <param name="rowData">DataGridRow with the data</param>
        /// <param name="rowIndex">The index from the DataGridRow</param>
        public TapeObj(DataGridViewCellCollection rowData, int rowIndex) : this(rowData) {
            row = rowIndex;
        }

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
            myRow = rowData;
            ReParse();
        }

        /// <summary>
        /// These values are user defined and not super critical and should be updated every time we 
        /// need to do anything that can be effected by changes in these values
        /// </summary>
        public void UpdateValues() {
            if (myRow == null) throw new Exception("my row dissapeared");
            PartOrientation = myRow["RotationColumn"].Value.ToString();
            string tape_size = myRow["WidthColumn"].Value.ToString();
            string[] sizes = tape_size.Split(new string[] { "/", "mm" }, StringSplitOptions.RemoveEmptyEntries);
            TapeWidth = double.Parse(sizes[0]);
            PartPitch = double.Parse(sizes[1]);
            currentPart = int.Parse(myRow["Next_Column"].Value.ToString()); //XXX might need to subtract 1?
Console.WriteLine("ID " + this.ID + " Next=" + currentPart);
            switch (myRow["TypeColumn"].Value.ToString()) {
                case "Paper (White)": Type = TapeType.White; break;
                case "Black Plastic": Type = TapeType.Black; break;
                case "Clear Plastic": Type = TapeType.Clear; break;
                default:
                    throw new Exception("Unable to parse tape type");
            }
        }

        public void ReParse() {
            try {
                isFullyCalibrated = false;
                ID = myRow["IdColumn"].Value.ToString();
                TapeOrientation = myRow["OrientationColumn"].Value.ToString();
                FirstHole = new PartLocation(float.Parse(myRow["X_Column"].Value.ToString()),
                                      float.Parse(myRow["Y_Column"].Value.ToString()));
                UpdateValues();
            } catch (Exception e) {
                throw new Exception("Tape: Unable to parse row data : " + e.ToString());
            }

            try {
                _PlaceZ = double.Parse(myRow["PlaceZ_Column"].Value.ToString());
                isPlaceZSet = true;
            } catch { }

            try {
                _PickupZ = double.Parse(myRow["PickupZ_Column"].Value.ToString());
                isPickupZSet = true;
            } catch { }

            try { b=double.Parse(myRow["Slope_Column"].Value.ToString()); } catch {}
            try { HolePitch = double.Parse(myRow["HolePitch_Column"].Value.ToString()); } catch {}
            try { isFullyCalibrated = bool.Parse(myRow["IsCalibrated_Column"].Value.ToString()); } catch {}


        }

        public void PushChangesToRow() {
            if (myRow == null) throw new Exception("my row dissapeared");
            myRow["IsCalibrated_Column"].Value = true;
            myRow["Slope_Column"].Value = b.ToString();
            myRow["HolePitch_Column"].Value = HolePitch.ToString();
            myRow["X_Column"].Value = FirstHole.X.ToString();
            myRow["Y_Column"].Value = FirstHole.Y.ToString();
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
            UpdateValues();
            return currentPart;
        }
        public void NextPart() {
            UpdateValues();
            SetPart(currentPart + 1);
        }
        public void SetPart(int part) {
            currentPart = part;
            if (myRow == null) throw new Exception("my row dissapeared");
            myRow["Next_Column"].Value = currentPart.ToString();
        }

        

        // PART //
        public PartLocation GetCurrentPartLocation() {
            UpdateValues();
            return GetPartLocation(currentPart);
        }

        // Part Orientation = orientation if tape is perfectly oriented the way it is oriented
        // deltaOrientation = how far offf the tape is from it's stated orientation
        public PartLocation GetPartLocation(int componentNumber) {
            UpdateValues(); //pull in changes (?)
            PartLocation offset = new PartLocation(componentNumber * PartPitch + HoleToPartSpacingX, HoleToPartSpacingY);
            // add the vector to the part rotated by the tape orientation
            PartLocation part = new PartLocation(FirstHole) + offset.Rotate(_TapeOrientation.ToRadians());
            // add deviation from expected orientation to part orientation
            var originalOrientation = OrienationToVector(myRow["OrientationColumn"].Value.ToString());
            var deltaOrientation = _TapeOrientation.ToRadians() - originalOrientation.ToRadians();
            part.A = (_PartOrientation.ToRadians() + deltaOrientation) * 180d / Math.PI;
            
            return part;
        }


        // HOLES //
        public PartLocation GetHoleLocation(int holeNumber) {
            UpdateValues();
            return new PartLocation(FirstHole) + new PartLocation(holeNumber * HolePitch, 0).Rotate(_TapeOrientation.ToRadians());
        }

        public PartLocation GetNearestCurrentPartHole() {
            UpdateValues();
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


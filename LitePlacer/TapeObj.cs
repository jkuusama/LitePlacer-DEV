using System;
using System.ComponentModel;

// http://www.yageo.com/exep/pages/download/literatures/UPY-C_GEN_15.pdf

namespace LitePlacer {
    public enum ComponentType { T0402, T0603, T0806, QFN }
    public enum Orientation {PosX,PosY,NegX,NegY}

    [Serializable]
    public class TapeObj : INotifyPropertyChanged {

        private string __tapeType = "Paper";
        public string TapeType { get { return __tapeType; } set { __tapeType = value; notify("TapeType"); } } //tapetype
        private string __partType = "T0402";
        public string PartType { get { return __partType; } set { __partType = value; ToDefaults(_PartType); notify("PartType"); } } //tapedefault

        public ComponentType _PartType { get { return ToTapeDefault(PartType); } }

        private string _ID;
        public string ID { get { return _ID; } set { _ID = value; notify("ID"); } } // name 

        public double HoleDiameter {get;  set;} //Do

        private double _HolePitch;
        public double HolePitch { get { return _HolePitch; } set { _HolePitch = value; notify("HolePitch");} } // P0

        private double _PartPitch;
        public double PartPitch { get { return _PartPitch; } set { _PartPitch = value; notify("PartPitch"); } }

        private double _HoleToPartSpacingX;
        public double HoleToPartSpacingX { get { return _HoleToPartSpacingX; } set { _HoleToPartSpacingX = value; notify("HoleToPartSpacingX"); } }

        private double _HoleToPartSpacyingY;
        public double HoleToPartSpacyingY { get { return _HoleToPartSpacyingY; } set { _HoleToPartSpacyingY = value; notify("HoleToPartSpacyingY"); } }

        private double _PickupZ;
        public double PickupZ { get { return _PickupZ; } set { _PickupZ = value; notify("PickupZ"); } }

        private double _PlaceZ;
        public double PlaceZ { get { return _PlaceZ; } set { _PlaceZ = value; notify("PlaceZ"); } }

        private double _Slope;
        public double Slope { get { return _Slope; } set { _Slope = value; notify("Slope"); } }

        private double _TapeWidth;
        public double TapeWidth { get { return _TapeWidth; } set { _TapeWidth = value; notify("TapeWidth"); notify("HoleToPartSpacingY"); } }

        public double HoleToPartSpacingY { get { return TapeWidth / 2 - .5; } } // F
        public PartLocation FirstHole;

        public bool IsPickupZSet { get { return (_PickupZ == -1) ?false : true; } }
        public bool IsPlaceZSet { get { return (_PlaceZ == -1) ?false : true; } }
        public bool IsFullyCalibrated = false;
        public double a; //coeffients of equation defining location of tape a + b*x
        private double _b;
        private int currentPart;

        public void Reset() {
            TapeType = "Paper";
            currentPart = 1;
            PickupZ = -1;
            PlaceZ = -1;
            OriginalTapeOrientation = "PositiveX";
            OriginalPartOrientation = "PositiveX";
            b = 0;
            PartType = "T0402"; //will reset defaults
        }

        // these are the actual anglees of the part - represted as slopes
        private PartLocation _TapeOrientation;
        // this is the user defined direction of the part
        // if tape is orthogonal to axis, then part orientation refers to the orientation of the part at that time

        public PartLocation OriginalTapeOrientationVector { get { return OrientationToVector(OriginalTapeOrientation); } }
        public PartLocation OriginalPartOrientationVector { get { return OrientationToVector(OriginalPartOrientation); } }
        public String OriginalTapeOrientation {get;set;}
        public String OriginalPartOrientation {get;set;}


       
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
        /// <param name="rowData"></param>
        public TapeObj() : this(ComponentType.T0402) {
            HolePitch = 4d;
            HoleToPartSpacingX = -2d; // first part is to the "left" of the hole
            HoleDiameter = 1.5d;
        }

        public void ToDefaults(ComponentType x) {
            PartPitch = 2d;
            TapeWidth = 8d;
            HolePitch = 4d;
            HoleToPartSpacingX = -2d; // first part is to the "left" of the hole
            HoleDiameter = 1.5d;
            PickupZ = -1;
            PlaceZ = -1;

            switch (x) {
                case ComponentType.T0402:
                    PartPitch = 2d;
                    break;
                default:
                    PartPitch = 4d;
                    break;
            }
        }

        public TapeObj(ComponentType x) {
            ToDefaults(x);            
        }

        public int CurrentPartIndex() {
            return currentPart;
        }
        public void NextPart() {
            SetPart(currentPart + 1);
        }
        public void SetPart(int part) {
            currentPart = part;
        }

        public PartLocation GetCurrentPartLocation() {
            return GetPartLocation(currentPart);
        }

        // Part Orientation = orientation if tape is perfectly oriented the way it is oriented
        // deltaOrientation = how far offf the tape is from it's stated orientation
        public PartLocation GetPartLocation(int componentNumber) {
            PartLocation offset = new PartLocation(componentNumber * PartPitch + HoleToPartSpacingX, HoleToPartSpacingY);
            // add the vector to the part rotated by the tape orientation
            PartLocation part = new PartLocation(FirstHole) + offset.Rotate(_TapeOrientation.ToRadians());
            // add deviation from expected orientation to part orientation
            var deltaOrientation = _TapeOrientation.ToRadians() - OriginalTapeOrientationVector.ToRadians();
            part.A = (OriginalPartOrientationVector.ToRadians() + deltaOrientation) * 180d / Math.PI;

            return part;
        }


        // HOLES //
        public PartLocation GetHoleLocation(int holeNumber) {;
            return new PartLocation(FirstHole) + new PartLocation(holeNumber * HolePitch, 0).Rotate(_TapeOrientation.ToRadians());
        }

        public PartLocation GetNearestCurrentPartHole() {
            double distanceX = currentPart * PartPitch + HoleToPartSpacingX;
            int holeNumber = (int)(distanceX / HolePitch);
            return GetHoleLocation(holeNumber);
        }

        private PartLocation OrientationToVector(string o) {
            return OrientationToVector((Orientation)Enum.Parse(typeof(Orientation), o));
        }


        private ComponentType ToTapeDefault(string s) {
            return (ComponentType)Enum.Parse(typeof(ComponentType), s);
        }

        private PartLocation OrientationToVector(Orientation orientation) {
            switch (orientation) {
                case Orientation.PosX: return new PartLocation(1, 0);
                case Orientation.PosY: return new PartLocation(0, 1);
                case Orientation.NegX: return new PartLocation(-1, 0);
                case Orientation.NegY: return new PartLocation(0, -1);
                default: throw new Exception("Invalid Orientation");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void notify(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString() {
            return ID;
        }

    }
}


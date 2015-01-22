using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitePlacer
{

    enum slot_type { FullAuto, ShowFirst, FullManual };
    enum slot_rotation { Deg0, Deg90, Deg180, Deg270 };

    class Slot
    {

        public Slot()
        {
            //assign initial values
            Xfirst = Double.NaN;
            Yfirst = Double.NaN;
            Xsecond = Double.NaN;
            Ysecond = Double.NaN;
            Xlast = Double.NaN;
            Ylast = Double.NaN;
            Xdelta = Double.NaN;
            Ydelta = Double.NaN;
            Capacity = 0;
            Name = "New Slot";
            SlotType = slot_type.FullAuto;
            Zpickup = 0.0;
            ZpickupAuto = true;
            Zplace = 0.0;
            ZplaceAuto = true;
            Zspeed = Properties.Settings.Default.CNC_ZspeedMax;
            RotationSpeed = Properties.Settings.Default.CNC_AspeedMax;
            Rotation = slot_rotation.Deg0;
            CurrentLocation = 0;
        }

        public double Xfirst { get; set; }  // coordinates of the first component
        public double Yfirst { get; set; }
        public double Xsecond { get; set; }  // coordinates of the second component
        public double Ysecond { get; set; }
        public double Xlast { get; set; }  // coordinates of the last component
        public double Ylast { get; set; }
        public double Xdelta { get; set; }  // movenent difference form one component to the next
        public double Ydelta { get; set; }
        public int Capacity { get; set; }  // how many components fit
        public int CurrentLocation { get; set; }  // 1 to Capacity
        public string Name { get; set; }  // user set descriptive name
        public slot_type SlotType { get; set; }
        // FullAuto: The set values are used directly
        // ShowFirst: Manually point the first component. Delta values are used to locate parts (
        //             (So that the user doesn't need to aim the strip so accurately
        // FullManual: First, 2nd and last are all pointed out at use time
        public slot_rotation Rotation { get; set; }  // nominal rotation of the part in slot (0, 90, 180 or 270)
        public double Zpickup { get; set; }  // Z movement for pick-up
        public bool ZpickupAuto { get; set; }  // if first pickup Z should be probed
        public double Zplace { get; set; }  // Z movement for place
        public bool ZplaceAuto { get; set; }  // if first place Z should be probed
        public double Zspeed { get; set; }  // Z speed for pickup (so that components are not shaken off the strip)
        public double RotationSpeed { get; set; }  // set so, that the inertia does not make the compoent to rotate on the needle

        public int CalculateValues()
        {
            if (Double.IsNaN(Xfirst) || Double.IsNaN(Xsecond) || Double.IsNaN(Xlast))
            {
                // user has not set all values, can't calculate. X&Y are set together, no need to check Y's.
                return (-1);
            }
            double Pitch = Math.Sqrt(Math.Abs(Xfirst - Xsecond) * Math.Abs(Xfirst - Xsecond)
                + Math.Abs(Yfirst - Ysecond) * Math.Abs(Yfirst - Ysecond));
            double TotalLenght = Math.Sqrt(Math.Abs(Xfirst - Xlast) * Math.Abs(Xfirst - Xlast)
                + Math.Abs(Yfirst - Ylast) * Math.Abs(Yfirst - Ylast));
            Capacity = (int)Math.Round(TotalLenght / Math.Round(Pitch)) + 1;
            Xdelta = (Xlast - Xfirst) / Convert.ToDouble(Capacity - 1);
            Ydelta = (Ylast - Yfirst) / Convert.ToDouble(Capacity - 1);
            return 0;
        }

        public double TrueRotation()
        {
            double rot = 0;
            if (Rotation == slot_rotation.Deg0)
            {
                rot = 0.0;
            }
            else if (Rotation == slot_rotation.Deg90)
            {
                rot = 90.0;
            }
            else if (Rotation == slot_rotation.Deg180)
            {
                rot = 180.0;
            }
            else
            {
                rot = 270.0;
            }
            return Math.Atan2((Xlast - Xfirst), (Ylast - Yfirst)) * (180.0 / Math.PI) + rot; ;
            // true rotation (== (angle in rad) * to degrees) + nominal
        }
            

        public int GetPart(out double X, out double Y)
        {
            if (CurrentLocation == 0)
            {
                X = Double.NaN;
                Y = Double.NaN;
                return (-1);  // not loaded
            }
            if (CurrentLocation > Capacity)
            {
                X = Double.NaN;
                Y = Double.NaN;
                return (-2);  // out of stuff
            }
            X = Xfirst + (CurrentLocation - 1) * Xdelta;
            Y = Yfirst + (CurrentLocation - 1) * Ydelta;
            CurrentLocation = CurrentLocation + 1;
            return 0;
        }

    }
}

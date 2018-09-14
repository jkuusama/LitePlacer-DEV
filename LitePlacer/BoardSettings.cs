using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LitePlacer
{
    public class BoardSettings
    {
        // These parameters on both TinyG and qQuintic:
        public class CommonSettings
        {
            // ==========  System values  ==========
            public string st = "0";     // switch type, [0=NO,1=NC]
            public string mt = "300";   // motor idle timeout, in seconds
            public string jv = "3";     // json verbosity, [0=silent,1=footer,2=messages,3=configs,4=linenum,5=verbose]
            public string js = "1";     // json serialize style [0=relaxed,1=strict]
            public string tv = "1";     // text verbosity [0=silent,1=verbose]
            public string qv = "2";     // queue report verbosity [0=off,1=single,2=triple]
            public string sv = "1";     // status report verbosity [0=off,1=filtered,2=verbose]
            public string si = "200";   // status interval, in ms
            public string gun = "1";    // default gcode units mode [0=G20,1=G21] (1=mm)

            //        public string _Feedrate = "f2000";   // should not be 0

            // ========== motor 1 ==========
            public string motor1ma = "0";        // map to axis [0=X,1=Y,2=Z...]
            public string motor1sa = "0.900";    // step angle, deg
            public string motor1tr = "40.0000";  // travel per revolution, mm
            public string motor1mi = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string motor1po = "0";        // motor polarity [0=normal,1=reverse]
            public string motor1pm = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 2 ==========
            public string motor2ma = "1";        // map to axis [0=X,1=Y,2=Z...]
            public string motor2sa = "0.900";    // step angle, deg
            public string motor2tr = "40.0000";  // travel per revolution, mm
            public string motor2mi = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string motor2po = "0";        // motor polarity [0=normal,1=reverse]
            public string motor2pm = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 3 ==========
            public string motor3ma = "2";        // map to axis [0=X,1=Y,2=Z...]
            public string motor3sa = "1.800";    // step angle, deg
            public string motor3tr = "8.0000";  // travel per revolution, mm
            public string motor3mi = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string motor3po = "0";        // motor polarity [0=normal,1=reverse]
            public string motor3pm = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 4 ==========
            public string motor4ma = "3";        // map to axis [0=X,1=Y,2=Z...]
            public string motor4sa = "0.900";    // step angle, deg
            public string motor4tr = "160.0000";  // travel per revolution, mm
            public string motor4mi = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string motor4po = "0";        // motor polarity [0=normal,1=reverse]
            public string motor4pm = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== X axis ==========
            public string xam = "1";        // x axis mode, 1=standard
            public string xvm = "10000";    // x velocity maximum, mm/min
            public string xfr = "10000";    // x feedrate maximum, mm/min
            public string xtn = "0";        // x travel minimum, mm
            public string xtm = "600";      // x travel maximum, mm
            public string xjm = "1000";     // x jerk maximum, mm/min^3 * 1 million
            public string xjh = "2000";     // x jerk homing, mm/min^3 * 1 million
            public string xsv = "2000";     // x search velocity, mm/min
            public string xlv = "100";      // x latch velocity, mm/min
            public string xlb = "8";        // x latch backoff, mm
            public string xzb = "2";        // x zero backoff, mm

            // ========== Y axis ==========
            public string yam = "1";        // y axis mode, 1=standard
            public string yvm = "10000";    // y velocity maximum, mm/min
            public string yfr = "10000";    // y feedrate maximum, mm/min
            public string ytn = "0";        // y travel minimum, mm
            public string ytm = "400";      // y travel mayimum, mm
            public string yjm = "1000";     // y jerk maximum, mm/min^3 * 1 million
            public string yjh = "2000";     // y jerk homing, mm/min^3 * 1 million
            public string ysv = "2000";     // y search velocity, mm/min
            public string ylv = "100";      // y latch velocity, mm/min
            public string ylb = "8";        // y latch backoff, mm
            public string yzb = "2";        // y zero backoff, mm

            // ========== Z axis ==========
            public string zam = "1";        // z axis mode, 1=standard
            public string zvm = "5000";     // z velocity maximum, mm/min
            public string zfr = "2000";     // z feedrate maximum, mm/min
            public string ztn = "0";        // z travel minimum, mm
            public string ztm = "80";       // z travel mazimum, mm
            public string zjm = "500";      // z jerk mazimum, mm/min^3 * 1 million
            public string zjh = "500";      // z jerk homing, mm/min^3 * 1 million
            public string zsv = "1000";     // z search velocity, mm/min
            public string zlv = "100";      // z latch velocity, mm/min
            public string zlb = "4";        // z latch backoff, mm
            public string zzb = "2";        // z zero backoff, mm

            // ========== A axis ==========
            public string aam = "1";        // a axis mode, 1=standard
            public string avm = "50000";    // a velocity maximum, mm/min
            public string afr = "200000";   // a feedrate maximum, mm/min
            public string atn = "0";        // a travel minimum, mm
            public string atm = "600";      // a travel maximum, mm
            public string ajm = "5000";     // a jerk maximum, mm/min^3 * 1 million
            public string ajh = "5000";     // a jerk homing, mm/min^3 * 1 million
            public string asv = "2000";     // a search velocity, mm/min

        }

        public class TinyG : CommonSettings
        {
            public string ec = "0";     // expand LF to CRLF on TX [0=off,1=on]
            public string ee = "0";     // enable echo [0=off,1=on]
            public string ex = "1";     // enable flow control [0=off,1=XON/XOFF, 2=RTS/CTS]

            // homing and limit default values are disabled, otherwise problem correction is difficult
            public string xsn = "0";   // x switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string xsx = "0";   // x switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string ysn = "0";   // y switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string ysx = "0";   // y switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string zsn = "0";   // z switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string zsx = "0";   // z switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string asn = "0";   // a switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string asx = "0";   // a switch max [0=off,1=homing,2=limit,3=limit+homing];
        }

        public class qQuintic : CommonSettings
        {
            public string motor1pl = "0.600";    // motor power level [0.000=minimum, 1.000=maximum]
            public string motor2pl = "0.600";    // motor power level [0.000=minimum, 1.000=maximum]
            public string motor3pl = "0.600";    // motor power level [0.000=minimum, 1.000=maximum]
            public string motor4pl = "0.100";    // motor power level [0.000=minimum, 1.000=maximum]
            public string motor5pl = "0.000";    // motor power level [0.000=minimum, 1.000=maximum]

            public string motor5ma = "4";        // map to axis [0=X,1=Y,2=Z...]
            public string motor5pm = "0";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            public string xhi = "0";     // x homing input [input 1-N or 0 to disable homing this axis]
            public string xhd = "0";     // x homing direction [0=search-to-negative, 1=search-to-positive]
            public string yhi = "0";     // y homing input [input 1-N or 0 to disable homing this axis]
            public string yhd = "0";     // y homing direction [0=search-to-negative, 1=search-to-positive]
            public string zhi = "0";     // z homing input [input 1-N or 0 to disable homing this axis]
            public string zhd = "0";     // z homing direction [0=search-to-negative, 1=search-to-positive]
            public string ahi = "0";     // a homing input [input 1-N or 0 to disable homing this axis]
            public string bhi = "0";     // b homing input [input 1-N or 0 to disable homing this axis]
        }


        public static FormMain MainForm;

        // Board settings file: Text file, starting with 8 characters board name and \n\r, 
        // then the setings in Json format
        static public bool Save(TinyG TinyGSettings, qQuintic qQuinticSettings, string FileName)
        {
            try
            {
                if (MainForm.Cnc.Controlboard == CNC.ControlBoardType.TinyG)
                {
                    MainForm.DisplayText("Writing TinyG settings file: " + FileName);
                    File.WriteAllText(FileName, "TinyG   \n\r" + JsonConvert.SerializeObject(TinyGSettings, Formatting.Indented));
                }
                else if (MainForm.Cnc.Controlboard == CNC.ControlBoardType.qQuintic)
                {
                    MainForm.DisplayText("Writing qQuintic settings file: " + FileName);
                    File.WriteAllText(FileName, "qQuintic\n\r" + JsonConvert.SerializeObject(qQuinticSettings, Formatting.Indented));
                }
                else
                {
                    MainForm.DisplayText("Skipping writing board settings file; board type unknown");
                }
                return true;
            }
            catch (System.Exception excep)
            {
                MainForm.DisplayText("Saving settings to " + FileName + " failed:\n" + excep.Message, KnownColor.DarkRed);
                return false;
            }
        }

        static public bool Load(ref TinyG TinyGSettings, ref qQuintic qQuinticSettings, string FileName)
        {
            string content= "";
            try
            {
                string name = FileName;
                if (File.Exists(name))
                {
                    MainForm.DisplayText("Reading " + name);
                    content = File.ReadAllText(name);
                }
                else
                {
                    MainForm.DisplayText("Settings file " + name + " not found, using default values.");
                    return false;
                }
                if (content.Length<10)
                {
                    MainForm.ShowMessageBox(
                       "Problem loading application settings: File is too short.\nUsing built in defaults.",
                       "Settings not loaded",
                       MessageBoxButtons.OK);
                    return false;
                }
                string boardType = content.Substring(0, 10);
                content = content.Remove(0, 10);
                if (boardType == "TinyG   \n\r")
                {
                    TinyGSettings = JsonConvert.DeserializeObject<TinyG>(content);
                }
                else if (boardType == "qQuintic\n\r")
                {
                    qQuinticSettings = JsonConvert.DeserializeObject<qQuintic>(content
                        );
                }
                else
                {
                    MainForm.ShowMessageBox(
                       "Unknown baord type " + boardType + "\nUsing built in defaults.",
                       "Settings not loaded",
                       MessageBoxButtons.OK);
                    return false;
                }
                return true;
            }
            catch (System.Exception excep)
            {
                MainForm.ShowMessageBox(
                    "Problem loading application settings:\n" + excep.Message + "\nUsing built in defaults.",
                    "Settings not loaded",
                    MessageBoxButtons.OK);
                return false;
            }
        }
    }

}

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
#pragma warning disable CA1031 // Do not catch general exception types (see MainForm.cs beginning)

    public class CommonBoardSettings
    {
        // ==========  System values  ==========
        public string Sys_st { get; set; } = "0";     // switch type, [0=NO,1=NC]
            public string Sys_mt { get; set; } = "300";   // motor idle timeout, in seconds
            public string Sys_jv { get; set; } = "3";     // json verbosity, [0=silent,1=footer,2=messages,3=configs,4=linenum,5=verbose]
            public string Sys_js { get; set; } = "1";     // json serialize style [0=relaxed,1=strict]
            public string Sys_tv { get; set; } = "1";     // text verbosity [0=silent,1=verbose]
            public string Sys_qv { get; set; } = "2";     // queue report verbosity [0=off,1=single,2=triple]
            public string Sys_sv { get; set; } = "1";     // status report verbosity [0=off,1=filtered,2=verbose]
            public string Sys_si { get; set; } = "200";   // status interval, in ms
            public string Sys_gun { get; set; } = "1";    // default gcode units mode [0=G20,1=G21] (1=mm)

            //        public string _Feedrate = "f2000";   // should not be 0

            // ========== motor 1 ==========
            public string Motor1ma { get; set; } = "0";        // map to axis [0=X,1=Y,2=Z...]
            public string Motor1sa { get; set; } = "0.900";    // step angle, deg
            public string Motor1tr { get; set; } = "40.0000";  // travel per revolution, mm
            public string Motor1mi { get; set; } = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string Motor1po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
            public string Motor1pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 2 ==========
            public string Motor2ma { get; set; } = "1";        // map to axis [0=X,1=Y,2=Z...]
            public string Motor2sa { get; set; } = "0.900";    // step angle, deg
            public string Motor2tr { get; set; } = "40.0000";  // travel per revolution, mm
            public string Motor2mi { get; set; } = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string Motor2po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
            public string Motor2pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 3 ==========
            public string Motor3ma { get; set; } = "2";        // map to axis [0=X,1=Y,2=Z...]
            public string Motor3sa { get; set; } = "1.800";    // step angle, deg
            public string Motor3tr { get; set; } = "8.0000";  // travel per revolution, mm
            public string Motor3mi { get; set; } = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string Motor3po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
            public string Motor3pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== motor 4 ==========
            public string Motor4ma { get; set; } = "3";        // map to axis [0=X,1=Y,2=Z...]
            public string Motor4sa { get; set; } = "0.900";    // step angle, deg
            public string Motor4tr { get; set; } = "160.0000";  // travel per revolution, mm
            public string Motor4mi { get; set; } = "8";        // microsteps [1,2,4,8], qQuintic [1,2,4,8,16,32]
            public string Motor4po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
            public string Motor4pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

            // ========== X axis ==========
            public string Xam { get; set; } = "1";        // x axis mode, 1=standard
            public string Xvm { get; set; } = "10000";    // x velocity maximum, mm/min
            public string Xfr { get; set; } = "10000";    // x feedrate maximum, mm/min
            public string Xtn { get; set; } = "0";        // x travel minimum, mm
            public string Xtm { get; set; } = "600";      // x travel maximum, mm
            public string Xjm { get; set; } = "1000";     // x jerk maximum, mm/min^3 * 1 million
            public string Xjh { get; set; } = "2000";     // x jerk homing, mm/min^3 * 1 million
            public string Xsv { get; set; } = "2000";     // x search velocity, mm/min
            public string Xlv { get; set; } = "100";      // x latch velocity, mm/min
            public string Xlb { get; set; } = "8";        // x latch backoff, mm
            public string Xzb { get; set; } = "2";        // x zero backoff, mm

            // ========== Y axis ==========
            public string Yam { get; set; } = "1";        // y axis mode, 1=standard
            public string Yvm { get; set; } = "10000";    // y velocity maximum, mm/min
            public string Yfr { get; set; } = "10000";    // y feedrate maximum, mm/min
            public string Ytn { get; set; } = "0";        // y travel minimum, mm
            public string Ytm { get; set; } = "400";      // y travel mayimum, mm
            public string Yjm { get; set; } = "1000";     // y jerk maximum, mm/min^3 * 1 million
            public string Yjh { get; set; } = "2000";     // y jerk homing, mm/min^3 * 1 million
            public string Ysv { get; set; } = "2000";     // y search velocity, mm/min
            public string Ylv { get; set; } = "100";      // y latch velocity, mm/min
            public string Ylb { get; set; } = "8";        // y latch backoff, mm
            public string Yzb { get; set; } = "2";        // y zero backoff, mm

            // ========== Z axis ==========
            public string Zam { get; set; } = "1";        // z axis mode, 1=standard
            public string Zvm { get; set; } = "5000";     // z velocity maximum, mm/min
            public string Zfr { get; set; } = "2000";     // z feedrate maximum, mm/min
            public string Ztn { get; set; } = "0";        // z travel minimum, mm
            public string Ztm { get; set; } = "80";       // z travel mazimum, mm
            public string Zjm { get; set; } = "500";      // z jerk mazimum, mm/min^3 * 1 million
            public string Zjh { get; set; } = "500";      // z jerk homing, mm/min^3 * 1 million
            public string Zsv { get; set; } = "1000";     // z search velocity, mm/min
            public string Zlv { get; set; } = "100";      // z latch velocity, mm/min
            public string Zlb { get; set; } = "4";        // z latch backoff, mm
            public string Zzb { get; set; } = "2";        // z zero backoff, mm

            // ========== A axis ==========
            public string Aam { get; set; } = "1";        // a axis mode, 1=standard
            public string Avm { get; set; } = "50000";    // a velocity maximum, mm/min
            public string Afr { get; set; } = "200000";   // a feedrate maximum, mm/min
            public string Atn { get; set; } = "0";        // a travel minimum, mm
            public string Atm { get; set; } = "600";      // a travel maximum, mm
            public string Ajm { get; set; } = "5000";     // a jerk maximum, mm/min^3 * 1 million
            public string Ajh { get; set; } = "5000";     // a jerk homing, mm/min^3 * 1 million
            public string Asv { get; set; } = "2000";     // a search velocity, mm/min

        }

    public class TinyGSettings : CommonBoardSettings
    {
        public string TG_ec { get; set; } = "0";     // expand LF to CRLF on TX [0=off,1=on]
            public string TG_ee { get; set; } = "0";     // enable echo [0=off,1=on]
            public string TG_ex { get; set; } = "1";     // enable flow control [0=off,1=XON/XOFF, 2=RTS/CTS]

            // homing and limit default values are disabled, otherwise problem correction is difficult
            public string Xsn { get; set; } = "0";   // x switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string Xsx { get; set; } = "0";   // x switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string Ysn { get; set; } = "0";   // y switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string Ysx { get; set; } = "0";   // y switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string Zsn { get; set; } = "0";   // z switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string Zsx { get; set; } = "0";   // z switch max [0=off,1=homing,2=limit,3=limit+homing];
            public string Asn { get; set; } = "0";   // a switch min [0=off,1=homing,2=limit,3=limit+homing];
            public string Asx { get; set; } = "0";   // a switch max [0=off,1=homing,2=limit,3=limit+homing];
        }

    public static class BoardSettings
    {

        static public FormMain MainForm { get; set; }

        // Board settings file: Text file, starting with 8 characters board name and \n\r, 
        // then the setings in Json format
        static public bool Save(TinyGSettings TinyGSettings, string FileName)
        {
            try
            {
                if (MainForm.Cnc.Controlboard == CNC.ControlBoardType.TinygHW)
                {
                    MainForm.DisplayText("Writing TinyG settings file: " + FileName);
                    File.WriteAllText(FileName, "TinyG   \n\r" + JsonConvert.SerializeObject(TinyGSettings, Formatting.Indented));
                }
                else
                {
                    MainForm.DisplayText("Skipping writing board settings file; board type unknown");
                }
                return true;
            }
            catch (Exception excep)
            {
                MainForm.DisplayText("Saving settings to " + FileName + " failed:\n" + excep.Message, KnownColor.DarkRed);
                return false;
            }
        }

        static public bool Load(ref TinyGSettings TinyGSettings, string FileName)
        {
            string content = "";
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
                if (content.Length < 10)
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
                    TinyGSettings = JsonConvert.DeserializeObject<TinyGSettings>(content);
                }
                else
                {
                    MainForm.ShowMessageBox(
                       "Unknown board type " + boardType + "\nUsing built in defaults.",
                       "Settings not loaded",
                       MessageBoxButtons.OK);
                    return false;
                }
                return true;
            }
            catch (System.Exception excep)
            {
                MainForm.ShowMessageBox(
                    "Problem loading board settings:\n" + excep.Message + "\nUsing built in defaults.",
                    "Settings not loaded",
                    MessageBoxButtons.OK);
                return false;
            }
        }
    }

}

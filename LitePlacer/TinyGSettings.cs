#define TINYG_SHORTUNITS
// Some firmware versions use units in millions, some don't. If not, comment out the above line.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LitePlacer
{
#pragma warning disable CA1031 // Do not catch general exception types (see MainForm.cs beginning)
    // This file has things that are related to control board settings, UI, their storage and retrieval.
    // 
    // TinyG related items were written first. Looking back now, the implementation is not very good.
    // It works, but for Duet 3, I'll handle this differently.
    //

    public partial class FormMain : Form
    {
        // =================================================================================
        // TinyG stuff
        // =================================================================================

        // Called from TinyG class when TinyG related UI need updating

        public void ValueUpdater(string item, string value)
        {
            if (InvokeRequired) { Invoke(new Action<string, string>(ValueUpdater), new[] { item, value }); return; }
            // DisplayText("ValueUpdater: item= " + item + ", value= " + value);

            switch (item)
            {
                // ==========  position values  ==========
                // now handled at Sr class
                case "posx":
                    DisplayText("*** ValueUpdater posx", KnownColor.DarkRed, true);                    
                    break;
                case "posy":
                    DisplayText("*** ValueUpdater posy", KnownColor.DarkRed, true);
                    break;
                case "posz":
                    DisplayText("*** ValueUpdater posz", KnownColor.DarkRed, true);
                    break;
                case "posa":
                    DisplayText("*** ValueUpdater posa", KnownColor.DarkRed, true);
                    break;

                // ==========  System values  ==========
                case "st":     // switch type, [0=NO,1=NC]
                    break;
                case "mt":   // motor idle timeout, in seconds
                    Update_mt(value);
                    break;
                case "jv":     // json verbosity, [0=silent,1=footer,2=messages,3=configs,4=linenum,5=verbose]
                    break;
                case "js":     // json serialize style [0=relaxed,1=strict]
                    break;
                case "tv":     // text verbosity [0=silent,1=verbose]
                    break;
                case "qv":     // queue report verbosity [0=off,1=single,2=triple]
                    break;
                case "sv":     // status report verbosity [0=off,1=filtered,2=verbose
                    break;
                case "si":   // status interval, in ms
                    break;
                case "gun":    // default gcode units mode [0=G20,1=G21] (1=mm)
                    break;

                // ========== motor 1 ==========
                case "1ma":        // map to axis [0=X,1=Y,2=Z...]
                    TinyGBoard.Motor1ma = value;
                    break;
                case "1sa":    // step angle, deg
                    TinyGBoard.Motor1ma = value;
                    Update_1sa(value);
                    break;
                case "1tr":  // travel per revolution, mm
                    TinyGBoard.Motor1tr = value;
                    Update_1tr(value);
                    break;
                case "1mi":        // microsteps [1,2,4,8]
                    TinyGBoard.Motor1mi = value;
                    Update_1mi(value);
                    break;
                case "1po":        // motor polarity [0=normal,1=reverse]
                    TinyGBoard.Motor1po = value;
                    break;
                case "1pm":        // power management [0=disabled,1=always on,2=in cycle,3=when moving]
                    TinyGBoard.Motor1pm = value;
                    break;
                case "1pl":    // motor power level [0.000=minimum, 1.000=maximum]
                    break;

                // ========== motor 2 ==========
                case "2ma":        // map to axis [0=X,1=Y,2=Z...]
                    TinyGBoard.Motor2ma = value;
                    break;
                case "2sa":    // step angle, deg
                    TinyGBoard.Motor2sa = value;
                    Update_2sa(value);
                    break;
                case "2tr":  // travel per revolution, mm
                    TinyGBoard.Motor2tr = value;
                    Update_2tr(value);
                    break;
                case "2mi":        // microsteps [1,2,4,8]
                    TinyGBoard.Motor2mi = value;
                    Update_2mi(value);
                    break;
                case "2po":        // motor polarity [0=normal,1=reverse]
                    TinyGBoard.Motor2po = value;
                    break;
                case "2pm":        // power management [0=disabled,1=always on,2=in cycle,3=when moving]
                    TinyGBoard.Motor2pm = value;
                    break;
                case "2pl":    // motor power level [0.000=minimum, 1.000=maximum]
                    break;

                // ========== motor 3 ==========
                case "3ma":        // map to axis [0=X,1=Y,2=Z...]
                    TinyGBoard.Motor3ma = value;
                    break;
                case "3sa":    // step angle, deg
                    TinyGBoard.Motor3sa = value;
                    Update_3sa(value);
                    break;
                case "3tr":  // travel per revolution, mm
                    TinyGBoard.Motor3tr = value;
                    Update_3tr(value);
                    break;
                case "3mi":        // microsteps [1,2,4,8]
                    TinyGBoard.Motor3mi = value;
                    Update_3mi(value);
                    break;
                case "3po":        // motor polarity [0=normal,1=reverse]
                    TinyGBoard.Motor3po = value;
                    break;
                case "3pm":        // power management [0=disabled,1=always on,2=in cycle,3=when moving]
                    TinyGBoard.Motor3pm = value;
                    break;
                case "3pl":    // motor power level [0.000=minimum, 1.000=maximum]
                    break;

                // ========== motor 4 ==========
                case "4ma":        // map to axis [0=X,1=Y,2=Z...]
                    TinyGBoard.Motor4ma = value;
                    break;
                case "4sa":    // step angle, deg
                    TinyGBoard.Motor4sa = value;
                    Update_4sa(value);
                    break;
                case "4tr":  // travel per revolution, mm
                    TinyGBoard.Motor4tr = value;
                    Update_4tr(value);
                    break;
                case "4mi":        // microsteps [1,2,4,8]
                    TinyGBoard.Motor4mi = value;
                    Update_4mi(value);
                    break;
                case "4po":        // motor polarity [0=normal,1=reverse]
                    TinyGBoard.Motor4po = value;
                    break;
                case "4pm":        // power management [0=disabled,1=always on,2=in cycle,3=when moving]
                    TinyGBoard.Motor4pm = value;
                    break;
                case "4pl":    // motor power level [0.000=minimum, 1.000=maximum]
                    break;

                // ========== X axis ==========
                case "xam":        // x axis mode, 1=standard
                    TinyGBoard.Xam = value;
                    break;
                case "xvm":    // x velocity maximum, mm/min
                    Update_xvm(value);
                    TinyGBoard.Xvm = value;
                    break;
                case "xfr":    // x feedrate maximum, mm/mi
                    TinyGBoard.Xfr = value;
                    break;
                case "xtn":        // x travel minimum, mm
                    TinyGBoard.Xtn = value;
                    break;
                case "xtm":      // x travel maximum, mm
                    TinyGBoard.Xtm = value;
                    break;
                case "xjm":     // x jerk maximum, mm/min^3 * 1 million
                    TinyGBoard.Xjm = value;
                    Update_xjm(value);
                    break;
                case "xjh":     // x jerk homing, mm/min^3 * 1 million
                    TinyGBoard.Xjh = value;
                    Update_xjh(value);
                    break;
                case "xsv":     // x search velocity, mm/min
                    TinyGBoard.Xsv = value;
                    Update_xsv(value);
                    break;
                case "xlv":      // x latch velocity, mm/min
                    TinyGBoard.Xlv = value;
                    break;
                case "xlb":        // x latch backoff, mm
                    TinyGBoard.Xlb = value;
                    break;
                case "xzb":        // x zero backoff, mm
                    TinyGBoard.Xzb = value;
                    break;

                // ========== Y axis ==========
                case "yam":        // y axis mode, 1=standard
                    TinyGBoard.Yam = value;
                    break;
                case "yvm":    // y velocity maximum, mm/min
                    Update_yvm(value);
                    TinyGBoard.Yvm = value;
                    break;
                case "yfr":    // y feedrate maximum, mm/min
                    TinyGBoard.Yfr = value;
                    break;
                case "ytn":        // y travel minimum, mm
                    TinyGBoard.Ytn = value;
                    break;
                case "ytm":      // y travel mayimum, mm
                    TinyGBoard.Ytm = value;
                    break;
                case "yjm":     // y jerk maximum, mm/min^3 * 1 million
                    TinyGBoard.Yjm = value;
                    Update_yjm(value);
                    break;
                case "yjh":     // y jerk homing, mm/min^3 * 1 million
                    TinyGBoard.Yjh = value;
                    Update_yjh(value);
                    break;
                case "ysv":     // y search velocity, mm/min
                    TinyGBoard.Ysv = value;
                    Update_ysv(value);
                    break;
                case "ylv":      // y latch velocity, mm/min
                    TinyGBoard.Ylv = value;
                    break;
                case "ylb":        // y latch backoff, mm
                    TinyGBoard.Ylb = value;
                    break;
                case "yzb":        // y zero backoff, mm
                    TinyGBoard.Yzb = value;
                    break;

                // ========== Z axis ==========
                case "zam":        // z axis mode, 1=standard
                    TinyGBoard.Zam = value;
                    break;
                case "zvm":     // z velocity maximum, mm/min
                    Update_zvm(value);
                    TinyGBoard.Zvm = value;
                    break;
                case "zfr":     // z feedrate maximum, mm/min
                    TinyGBoard.Zfr = value;
                    break;
                case "ztn":        // z travel minimum, mm
                    TinyGBoard.Ztn = value;
                    break;
                case "ztm":       // z travel mazimum, mm
                    TinyGBoard.Ztm = value;
                    break;
                case "zjm":      // z jerk mazimum, mm/min^3 * 1 million
                    TinyGBoard.Zjm = value;
                    Update_zjm(value);
                    break;
                case "zjh":      // z jerk homing, mm/min^3 * 1 million
                    TinyGBoard.Zjh = value;
                    Update_zjh(value);
                    break;
                case "zsv":     // z search velocity, mm/min
                    TinyGBoard.Zsv = value;
                    Update_zsv(value);
                    break;
                case "zlv":      // z latch velocity, mm/min
                    TinyGBoard.Zlv = value;
                    break;

                case "zlb":        // z latch backoff, mm
                        TinyGBoard.Zlb = value;
                    break;

                case "zzb":        // z zero backoff, mm
                        TinyGBoard.Zzb = value;
                    break;

                // ========== A axis ==========
                case "aam":        // a axis mode, 1=standard
                    TinyGBoard.Aam = value;
                    break;
                case "avm":    // a velocity maximum, mm/min
                    TinyGBoard.Avm = value;
                    Update_avm(value);
                    break;
                case "afr":   // a feedrate maximum, mm/min
                    TinyGBoard.Afr = value;
                    break;
                case "atn":        // a travel minimum, mm
                    TinyGBoard.Atn = value;
                    break;
                case "atm":      // a travel maximum, mm
                    TinyGBoard.Atm = value;
                    break;
                case "ajm":     // a jerk maximum, mm/min^3 * 1 million
                    TinyGBoard.Ajm = value;
                    Update_ajm(value);
                    break;
                case "ajh":     // a jerk homing, mm/min^3 * 1 million
                    TinyGBoard.Ajh = value;
                    break;
                case "asv":     // a search velocity, mm/min
                    TinyGBoard.Asv = value;
                    break;

                // ========== TinyG switch values ==========
                case "xsn":   // x switch min [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Xsn = value;
                    Update_xsn(value);
                    break;
                case "xsx":   // x switch max [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Xsx = value;
                    Update_xsx(value);
                    break;
                case "ysn":   // y switch min [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Ysn = value;
                    Update_ysn(value);
                    break;
                case "ysx":   // y switch max [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Ysx = value;
                    Update_ysx(value);
                    break;
                case "zsn":   // z switch min [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Zsn = value;
                    Update_zsn(value);
                    break;
                case "zsx":   // z switch max [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Zsx = value;
                    Update_zsx(value);
                    break;
                case "asn":   // a switch min [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Asn = value;
                    break;
                case "asx":   // a switch max [0=off,1=homing,2=limit,3=limit+homing];
                    TinyGBoard.Asx = value;
                    break;

                // Hardware platform
                case "hp":
                    Update_hp(value);
                    break;

                default:
                    DisplayText("ValueUpdater, no operation(" + item + ", " + value + ")");
                    break;
            }
        }

        // =========================================================================
        // Thread-safe update functions and value setting fuctions
        // =========================================================================
        #region hp  // hardware platform

        private void Update_hp(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_hp), new[] { value }); return; }

            if (value == "1")
            {
                Setting.Controlboard = ControlBoardType.TinyG;
                DisplayText("TinyG board found.");
            }
            else
            {
                ShowMessageBox(
                  "Unknown control board",
                  "Unknown control board, hp value = " + value,
                  MessageBoxButtons.OK);
            }
        }

        #endregion

        // =========================================================================
        #region jm  // *jm: jerk maximum
        // *jm update
        private void Update_xjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_yjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_zjm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_ajm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ajm), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            ajm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================================
        // *jm setting
        private void xjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"xjm\":" + xjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void yjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"yjm\":" + yjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void zjm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zjm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"zjm\":" + zjm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                zjm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void ajm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ajm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"ajm\":" + ajm_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                ajm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion

        // =========================================================================
        #region jh  // *jh: jerk homing
        // *jh update

        private void Update_xjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            xjh_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_yjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            yjh_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_zjh(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zjh), new[] { value }); return; }

#if (TINYG_SHORTUNITS)
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
#else
            double val = Convert.ToDouble(value) / 1000000;
#endif
            zjh_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================================
        // *jh setting

        private void xjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {

#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"xjh\":" + xjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                xjh_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void yjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"yjh\":" + yjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                yjh_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void zjh_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zjh_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
#if (TINYG_SHORTUNITS)
                Cnc.Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "}");
#else
                Cnc.Write_m("{\"zjh\":" + zjh_maskedTextBox.Text + "000000}");
#endif
                Thread.Sleep(50);
                zjh_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion

        // =========================================================================
        #region sv  // *sv: search velocity
        // * update

        private void Update_xsv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xsv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            xsv_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_ysv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ysv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            ysv_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_zsv(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zsv), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            zsv_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================================
        // *sv setting

        private void xsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"xsv\":" + xsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                xsv_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void ysv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ysv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"ysv\":" + ysv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                ysv_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void zsv_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zsv_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"zsv\":" + zsv_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zsv_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion

        // =========================================================================
        #region sn  // *sn: Negative limit switch
        // *sn update

        private void Update_xsn(string value)
        {
            switch (value)
            {
                case "0":
                    Xhome_checkBox.Checked = false;
                    Xlim_checkBox.Checked = false;
                    break;
                case "1":
                    Xhome_checkBox.Checked = true;
                    Xlim_checkBox.Checked = false;
                    break;
                case "2":
                    Xhome_checkBox.Checked = false;
                    Xlim_checkBox.Checked = true;
                    break;
                case "3":
                    Xhome_checkBox.Checked = true;
                    Xlim_checkBox.Checked = true;
                    break;
            }
        }

        private void Update_ysn(string value)
        {
            switch (value)
            {
                case "0":
                    Yhome_checkBox.Checked = false;
                    Ylim_checkBox.Checked = false;
                    break;
                case "1":
                    Yhome_checkBox.Checked = true;
                    Ylim_checkBox.Checked = false;
                    break;
                case "2":
                    Yhome_checkBox.Checked = false;
                    Ylim_checkBox.Checked = true;
                    break;
                case "3":
                    Yhome_checkBox.Checked = true;
                    Ylim_checkBox.Checked = true;
                    break;
            }
        }

        private void Update_zsn(string value)
        {
            switch (value)
            {
                case "0":
                    Zhome_checkBox.Checked = false;
                    Zlim_checkBox.Checked = false;
                    break;
                case "1":
                    Zhome_checkBox.Checked = true;
                    Zlim_checkBox.Checked = false;
                    break;
                case "2":
                    Zhome_checkBox.Checked = false;
                    Zlim_checkBox.Checked = true;
                    break;
                case "3":
                    Zhome_checkBox.Checked = true;
                    Zlim_checkBox.Checked = true;
                    break;
            }
        }

        // =========================================================================
        // *sn setting

        private void Xhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"xsn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        private void Xlim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Xlim_checkBox.Checked) i = 2;
            if (Xhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"xsn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        private void Yhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"ysn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        private void Ylim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Ylim_checkBox.Checked) i = 2;
            if (Yhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"ysn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        private void Zhome_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"zsn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        private void Zlim_checkBox_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (Zlim_checkBox.Checked) i = 2;
            if (Zhome_checkBox.Checked) i++;
            Cnc.Write_m("{\"zsn\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
            Thread.Sleep(50);
        }

        #endregion

        // =========================================================================
        #region sx  // *sx: Maximum limit switch
        // *sx update

        private void Update_xsx(string value)
        {
            if (value == "2")
            {
                Xmax_checkBox.Checked = true;
            }
            else
            {
                Xmax_checkBox.Checked = false;
            }
        }

        private void Update_ysx(string value)
        {
            if (value == "2")
            {
                Ymax_checkBox.Checked = true;
            }
            else
            {
                Ymax_checkBox.Checked = false;
            }
        }

        private void Update_zsx(string value)
        {
            if (value == "2")
            {
                Zmax_checkBox.Checked = true;
            }
            else
            {
                Zmax_checkBox.Checked = false;
            }
        }

        // =========================================================================
        // *sx setting

        private void Xmax_checkBox_Click(object sender, EventArgs e)
        {
            if (Xmax_checkBox.Checked)
            {
                Cnc.Write_m("{\"xsx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"xsx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Ymax_checkBox_Click(object sender, EventArgs e)
        {
            if (Ymax_checkBox.Checked)
            {
                Cnc.Write_m("{\"ysx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"ysx\":0}");
                Thread.Sleep(50);
            }
        }

        private void Zmax_checkBox_Click(object sender, EventArgs e)
        {
            if (Zmax_checkBox.Checked)
            {
                Cnc.Write_m("{\"zsx\":2}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"zsx\":0}");
                Thread.Sleep(50);
            }
        }

        #endregion

        // =========================================================================
        #region vm  // *vm: Velocity maximum
        // *vm update

        private void Update_xvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            val = val / 1000;
            xvm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_yvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_yvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            val = val / 1000;
            yvm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_zvm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zvm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            zvm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }


        private void Update_avm(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_avm), new[] { value }); return; }

            int ind = value.IndexOf('.');   // Cut off the decimal portion, otherwise convert fails in some non-US cultures 
            if (ind > 0)
            {
                value = value.Substring(0, ind);
            };
            double val = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            val = val / 1000;
            avm_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================================
        // *vm setting

        private void Xvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            xvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"xvm\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.Write_m("{\"xfr\":" + xvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                xvm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Yvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            yvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"yvm\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.Write_m("{\"yfr\":" + yvm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                yvm_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Zvm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            zvm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"zvm\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                Cnc.Write_m("{\"zfr\":" + zvm_maskedTextBox.Text + "}");
                Thread.Sleep(50);
                zvm_maskedTextBox.ForeColor = Color.Black;
                int peek = Convert.ToInt32(zvm_maskedTextBox.Text);
                Setting.CNC_ZspeedMax = peek;
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Avm_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            avm_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                Cnc.Write_m("{\"avm\":" + avm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                Cnc.Write_m("{\"afr\":" + avm_maskedTextBox.Text + "000}");
                Thread.Sleep(50);
                avm_maskedTextBox.ForeColor = Color.Black;
                int peek = Convert.ToInt32(avm_maskedTextBox.Text);
                Setting.CNC_AspeedMax = peek;
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion

        // =========================================================================
        #region mi  // *mi: microstepping
        // *mi update

        private void Update_1mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1mi), new[] { value }); return; }

            int val = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            mi1_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_2mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2mi), new[] { value }); return; }

            int val = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            mi2_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_3mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3mi), new[] { value }); return; }

            int val = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            mi3_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        private void Update_4mi(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4mi), new[] { value }); return; }

            int val = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            mi4_maskedTextBox.Text = val.ToString(CultureInfo.InvariantCulture);
        }

        // =========================================================================
        // *mi setting

        private void Microstep_KeyPress(MaskedTextBox box, int BoxNo, KeyPressEventArgs e)
        {
            box.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                List<String> GoodValues = new List<string> { "1", "2", "4", "8" };
                if (GoodValues.Contains(box.Text))
                {
                    Cnc.Write_m("{\"" + BoxNo.ToString(CultureInfo.InvariantCulture) + "mi\":" + box.Text + "}");
                    Thread.Sleep(50);
                    box.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }
        private void mi1_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Microstep_KeyPress(mi1_maskedTextBox, 1, e);
        }

        private void mi2_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Microstep_KeyPress(mi2_maskedTextBox, 2, e);
        }


        private void mi3_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Microstep_KeyPress(mi3_maskedTextBox, 3, e);
        }

        private void mi4_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Microstep_KeyPress(mi4_maskedTextBox, 4, e);
        }

        #endregion

        // =========================================================================
        #region tr  // *tr: Travel per revolution
        // *tr update

        private void Update_1tr(string value)
        {
            tr1_textBox.Text = value;
        }

        private void Update_2tr(string value)
        {
            tr2_textBox.Text = value;
        }

        private void Update_3tr(string value)
        {
            tr3_textBox.Text = value;

        }

        private void Update_4tr(string value)
        {
            tr4_textBox.Text = value;
        }

        // =========================================================================
        // *tr setting
        private void tr1_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr1_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr1_textBox.Text.Replace(',', '.'), out val))
                {
                    Cnc.Write_m("{\"1tr\":" + tr1_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr1_textBox.ForeColor = Color.Black;
                }
            }
            e.Handled = true;   // supress the ding sound
        }

        private void tr2_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr2_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr2_textBox.Text.Replace(',', '.'), out val))
                {
                    Cnc.Write_m("{\"2tr\":" + tr2_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr2_textBox.ForeColor = Color.Black;
                }
            }
            e.Handled = true;   // supress the ding sound
        }

        private void tr3_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr3_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr3_textBox.Text.Replace(',', '.'), out val))
                {
                    Cnc.Write_m("{\"3tr\":" + tr3_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr3_textBox.ForeColor = Color.Black;
                }
            }
            e.Handled = true;   // supress the ding sound
        }

        private void tr4_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            tr4_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(tr1_textBox.Text.Replace(',', '.'), out val))
                {
                    Cnc.Write_m("{\"4tr\":" + tr4_textBox.Text + "}");
                    Thread.Sleep(50);
                    tr4_textBox.ForeColor = Color.Black;
                }
            }
            e.Handled = true;   // supress the ding sound
        }

        #endregion

        // =========================================================================
        #region sa  // *sa: Step angle, 0.9 or 1.8
        // *sa update

        private void Update_1sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_1sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m1deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m1deg18_radioButton.Checked = true;
            }
        }

        private void Update_2sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_2sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m2deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m2deg18_radioButton.Checked = true;
            }
        }

        private void Update_3sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_3sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m3deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m3deg18_radioButton.Checked = true;
            }
        }

        private void Update_4sa(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_4sa), new[] { value }); return; }

            if ((value == "0.90") || (value == "0.900"))
            {
                m4deg09_radioButton.Checked = true;
            }
            else if ((value == "1.80") || (value == "1.800"))
            {
                m4deg18_radioButton.Checked = true;
            }
        }

        // =========================================================================
        // *sa setting

        private void m1deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m1deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m1deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m1deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"1sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"1sa\":1.8}");
                Thread.Sleep(50);
            }
        }


        private void m2deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m2deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m2deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m2deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"2sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"2sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m3deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m3deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m3deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"3sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"3sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg09_radioButton_Click(object sender, EventArgs e)
        {
            if (m4deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        private void m4deg18_radioButton_Click(object sender, EventArgs e)
        {
            if (m4deg09_radioButton.Checked)
            {
                Cnc.Write_m("{\"4sa\":0.9}");
                Thread.Sleep(50);
            }
            else
            {
                Cnc.Write_m("{\"4sa\":1.8}");
                Thread.Sleep(50);
            }
        }

        #endregion


        // =========================================================================
        #region Save_and_Load


        // Board settings file: Text file, starting with 8 characters board name and \n\r, 
        // then the setings in Json format
        private bool SaveTinyGSettings(TinyGSettings TinyGSettings, string FileName)
        {
            try
            {
                DisplayText("Writing TinyG settings file: " + FileName);
                File.WriteAllText(FileName, "TinyG   \n\r" + JsonConvert.SerializeObject(TinyGSettings, Formatting.Indented));
                DisplayText("Done.");
                return true;
            }
            catch (Exception excep)
            {
                DisplayText("*** Saving settings to " + FileName + " failed:\n" + excep.Message, KnownColor.DarkRed, true);
                return false;
            }
        }


        private bool LoadTinyGsettings(ref TinyGSettings TinyGSettings, string FileName)
        {
            string content = "";
            try
            {
                string name = FileName;
                if (File.Exists(name))
                {
                    DisplayText("Reading " + name);
                    content = File.ReadAllText(name);
                }
                else
                {
                    DisplayText("Settings file " + name + " not found, using default values.");
                    return false;
                }
                if (content.Length < 10)
                {
                    ShowMessageBox(
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
                    ShowMessageBox(
                       "Unknown / wrong board type (" + boardType + ") in file, board settings not changed.",
                       "Settings not loaded",
                       MessageBoxButtons.OK);
                    return false;
                }
                return true;
            }
            catch (System.Exception excep)
            {
                ShowMessageBox(
                    "Problem loading board settings:\n" + excep.Message + "\nBoard settings not changed.",
                    "Settings not loaded",
                    MessageBoxButtons.OK);
                return false;
            }
        }


        private void WriteAllTinyGSettings_m()
        {
            if (StartingUp)
            {
                return;
            }
            string path = GetPath();
            bool res = true;
            DialogResult dialogResult;
            if (Setting.Controlboard == ControlBoardType.TinyG)
            {
                dialogResult = ShowMessageBox(
                   "Settings currently stored on board of your TinyG will be overwritten,\n" +
                   " with conservative values. Current values will be permanently lost\n" +
                   "if you haven't stored a backup copy. Continue?",
                   "Overwrite current settings?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
                res = WriteTinyGSettings();
            }
            if (!res)
            {
                DisplayText("Writing settings failed.");
                ShowMessageBox(
                    "Problem writing board settings. Board is in undefined state, fix the problem before continuing!",
                    "Settings not loaded",
                    MessageBoxButtons.OK);
            }
        }

        private bool WriteOneTinyGSetting(string setting, string value, bool delay)
        {
            string dbg = "{\"" + setting + "\":" + value + "}";
            //string dbg = "$" + setting + "=" + value;
            DisplayText("write: " + dbg);
            if (!Cnc.Write_m(dbg))
            {
                return false;
            };
            if (delay)
            {
                Thread.Sleep(100);
            }
            return true;
        }


        private bool WriteTinyGSettings()
        {

            DisplayText("Writing settings to TinyG board.");
            foreach (PropertyInfo pi in TinyGBoard.GetType().GetProperties())
            {
                // The motor parameters are <motor number><parameter>, such as 1ma, 1sa, 1tr etc.
                // These are not valid parameter names, so Motor1ma, motor1sa etc are used.
                // to retrieve the values, we remove the "Motor"
                string name = pi.Name;
                if (name.StartsWith("Motor", StringComparison.Ordinal))
                {
                    name = name.Substring(5);
                }
                string value = pi.GetValue(TinyGBoard, null).ToString();
                if (!WriteOneTinyGSetting(name, value, true))
                {
                    return false;
                };
                DisplayText(" Wrote parameter: " + name + ", value: " + value);
            }
            return true;

        }



        #endregion
    }



    public class TinyGSettings
    {
        // ==========  System values  ==========
        public string st { get; set; } = "0";     // switch type, [0=NO,1=NC]
        public string mt { get; set; } = "300";   // motor idle timeout, in seconds
        public string jv { get; set; } = "3";     // json verbosity, [0=silent,1=footer,2=messages,3=configs,4=linenum,5=verbose]
        public string js { get; set; } = "1";     // json serialize style [0=relaxed,1=strict]
        public string tv { get; set; } = "1";     // text verbosity [0=silent,1=verbose]
        public string qv { get; set; } = "2";     // queue report verbosity [0=off,1=single,2=triple]
        public string sv { get; set; } = "1";     // status report verbosity [0=off,1=filtered,2=verbose]
        public string si { get; set; } = "200";   // status interval, in ms
        public string gun { get; set; } = "1";    // default gcode units mode [0=G20,1=G21] (1=mm)

        //        public string _Feedrate = "f2000";   // should not be 0

        // ========== motor 1 ==========
        public string Motor1ma { get; set; } = "0";        // map to axis [0=X,1=Y,2=Z...]
        public string Motor1sa { get; set; } = "0.900";    // step angle, deg
        public string Motor1tr { get; set; } = "40.0000";  // travel per revolution, mm
        public string Motor1mi { get; set; } = "8";        // microsteps [1,2,4,8]]
        public string Motor1po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
        public string Motor1pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

        // ========== motor 2 ==========
        public string Motor2ma { get; set; } = "1";        // map to axis [0=X,1=Y,2=Z...]
        public string Motor2sa { get; set; } = "0.900";    // step angle, deg
        public string Motor2tr { get; set; } = "40.0000";  // travel per revolution, mm
        public string Motor2mi { get; set; } = "8";        // microsteps [1,2,4,8]
        public string Motor2po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
        public string Motor2pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

        // ========== motor 3 ==========
        public string Motor3ma { get; set; } = "2";        // map to axis [0=X,1=Y,2=Z...]
        public string Motor3sa { get; set; } = "1.800";    // step angle, deg
        public string Motor3tr { get; set; } = "8.0000";  // travel per revolution, mm
        public string Motor3mi { get; set; } = "8";        // microsteps [1,2,4,8]
        public string Motor3po { get; set; } = "0";        // motor polarity [0=normal,1=reverse]
        public string Motor3pm { get; set; } = "2";        // power management [0=disabled,1=always on,2=in cycle,3=when moving]

        // ========== motor 4 ==========
        public string Motor4ma { get; set; } = "3";        // map to axis [0=X,1=Y,2=Z...]
        public string Motor4sa { get; set; } = "0.900";    // step angle, deg
        public string Motor4tr { get; set; } = "160.0000";  // travel per revolution, mm
        public string Motor4mi { get; set; } = "8";        // microsteps [1,2,4,8]]
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
        public string Zlb { get; set; } = "10";        // z latch backoff, mm
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

        public string ec { get; set; } = "0";     // expand LF to CRLF on TX [0=off,1=on]
        public string ee { get; set; } = "0";     // enable echo [0=off,1=on]
        public string ex { get; set; } = "1";     // enable flow control [0=off,1=XON/XOFF, 2=RTS/CTS]

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
}

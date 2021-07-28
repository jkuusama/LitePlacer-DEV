#define TINYG_SHORTUNITS
// Some firmware versions use units in millions, some don't. If not, comment out the above line.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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

    public partial class MySettings
    {
        public double Duet3_Xspeed { get; set; } = 200; // mm/s
        public double Duet3_Yspeed { get; set; } = 200; // mm/s
        public double Duet3_Zspeed { get; set; } = 50; // mm/s
        public double Duet3_Aspeed { get; set; } = 200; // mm/s

        public double Duet3_Xacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Yacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Zacc { get; set; } = 1000; // mm/s^2
        public double Duet3_Aacc { get; set; } = 1000; // mm/s^2

        public double Duet3_XTravelPerRev { get; set; } = 40;   // mm
        public double Duet3_YTravelPerRev { get; set; } = 40;   // mm
        public double Duet3_ZTravelPerRev { get; set; } = 8;    // mm
        public double Duet3_ATravelPerRev { get; set; } = 160;  // deg

        public double Duet3_XDegPerStep { get; set; } = 0.9;   // mm
        public double Duet3_YDegPerStep { get; set; } = 0.9;   // mm
        public double Duet3_ZDegPerStep { get; set; } = 1.8;   // mm
        public double Duet3_ADegPerStep { get; set; } = 0.9;   // mm

        public int Duet3_XMicroStep { get; set; } = 16;
        public int Duet3_YMicroStep { get; set; } = 16;
        public int Duet3_ZMicroStep { get; set; } = 16;
        public int Duet3_AMicroStep { get; set; } = 16;

        public bool Duet3_XInterpolate { get; set; } = true;
        public bool Duet3_YInterpolate { get; set; } = true;
        public bool Duet3_ZInterpolate { get; set; } = true;
        public bool Duet3_AInterpolate { get; set; } = true;

        public int Duet3_XCurrent { get; set; } = 1100; //mA
        public int Duet3_YCurrent { get; set; } = 1200; //mA
        public int Duet3_ZCurrent { get; set; } = 1300; //mA
        public int Duet3_ACurrent { get; set; } = 400; //mA

    }


    public partial class FormMain : Form
    {
        // =================================================================================
        // Duet 3 stuff
        // (TinyG related stuff in the end of this file)
        // =================================================================================
        // We can go directly to business. No need to go trough cnc class in motor settins,
        // as they are visible only when the corresponding board is already found.
        // Duet3 is owned by Cnc, so we do Cnc.Duet3.xxx(), not Cnc.xxx()

        #region Duet3

        // =================================================================================
        // X motor
        // =================================================================================
        #region Duet3Xmotor

        private bool SettingDuet3XmotorParameters = false;


        public bool SetDuet3XmotorParameters()
        {
            SettingDuet3XmotorParameters = true;

            Duet3Xspeed_maskedTextBox.Text = Setting.Duet3_Xspeed.ToString();
            if (!SetDuet3Xspeed(Setting.Duet3_Xspeed)) return false;

            Duet3Xacceleration_maskedTextBox.Text = Setting.Duet3_Xacc.ToString();
            if (!SetDuet3Xacc(Setting.Duet3_Xacc)) return false;

            Duet3Xmicrosteps_maskedTextBox.Text = Setting.Duet3_XMicroStep.ToString();
            if (Setting.Duet3_XDegPerStep > 1.0)
            {
                Duet3Xdeg09_radioButton.Checked = true;
                Duet3Xdeg18_radioButton.Checked = false;
            }
            else
            {
                Duet3Xdeg09_radioButton.Checked = false;
                Duet3Xdeg18_radioButton.Checked = true;

            }
            Duet3Xinterpolate_checkBox.Checked = Setting.Duet3_XInterpolate;
            Duet3XtravelPerRev_textBox.Text = Setting.Duet3_XTravelPerRev.ToString();
            if (!SetDuet3Xstepping()) return false;

            Duet3XCurrent_maskedTextBox.Text = Setting.Duet3_XCurrent.ToString();
            SetDuet3Xcurr(Setting.Duet3_XCurrent);

            SettingDuet3XmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void Duet3Xspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3Xspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Xspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_Xspeed = speed;
                    SetDuet3Xspeed(speed);
                    Duet3Xspeed_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Xspeed(double speed)
        {
            speed = speed * 60; // speed is set in mm/min, but reported in mm/s.
            return Cnc.Duet3.Write_m("M203 X" + speed.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // acceleration
        private void Duet3Xacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            Duet3Xacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Xacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Duet3_Xacc = acc;
                    SetDuet3Xacc(acc);
                }
                Duet3Xacceleration_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private bool SetDuet3Xacc(double acc)
        {
            return Cnc.Duet3.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetDuet3Xstepping()
        {
            string i;
            if (Setting.Duet3_XInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Duet3.Write_m("M350 X" + Setting.Duet3_XMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Duet3_XMicroStep * 360.0 / Setting.Duet3_XDegPerStep;
            steps = steps / Setting.Duet3_XTravelPerRev;
            return Cnc.Duet3.Write_m("M92 X" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void Duet3Xmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            Duet3Xmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3Xmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ( (usteps>1) && (usteps<=256) &&
                        ((usteps & (usteps - 1)) == 0))   
                    {
                        Setting.Duet3_XMicroStep = usteps;
                        SetDuet3Xstepping();
                        Duet3Xmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
            }
        }


        // =================================================================================
        // interpolate
        private void Duet3Xinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Duet3_XInterpolate = Duet3Xinterpolate_checkBox.Checked;
            if (!SettingDuet3XmotorParameters)
            {
                SetDuet3Xstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void Duet3Xdeg09_radioButton_Click(object sender, EventArgs e)
        {
            Duet3XDegChange();
        }

        private void Duet3Xdeg18_radioButton_Click(object sender, EventArgs e)
        {
            Duet3XDegChange();
        }

        private void Duet3XDegChange()
        {
            if (Duet3Xdeg09_radioButton.Checked)
            {
                Setting.Duet3_XDegPerStep = 0.9;
            }
            else
            {
                Setting.Duet3_XDegPerStep = 0.9;
            }
            SetDuet3Xstepping();
        }

        // =================================================================================
        // travel per revolution
        private void Duet3XtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            Duet3XtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3XtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Duet3_XTravelPerRev = travel;
                    SetDuet3Xstepping();
                    Duet3XtravelPerRev_textBox.ForeColor = Color.Black;
                }
            }
        }


        // =================================================================================
        // motor current
        private void Duet3XCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            Duet3XCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3XCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Duet3_XCurrent = curr;
                    SetDuet3Xcurr(curr);
                    Duet3XCurrent_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Xcurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 X" + curr.ToString());
        }

        #endregion

        // =================================================================================
        // Y motor
        // =================================================================================
        #region Duet3Ymotor

        private bool SettingDuet3YmotorParameters = false;


        public bool SetDuet3YmotorParameters()
        {
            SettingDuet3YmotorParameters = true;

            Duet3Yspeed_maskedTextBox.Text = Setting.Duet3_Yspeed.ToString();
            if (!SetDuet3Yspeed(Setting.Duet3_Yspeed)) return false;

            Duet3Yacceleration_maskedTextBox.Text = Setting.Duet3_Yacc.ToString();
            if (!SetDuet3Yacc(Setting.Duet3_Yacc)) return false;

            Duet3Ymicrosteps_maskedTextBox.Text = Setting.Duet3_YMicroStep.ToString();
            if (Setting.Duet3_YDegPerStep > 1.0)
            {
                Duet3Ydeg09_radioButton.Checked = true;
                Duet3Ydeg18_radioButton.Checked = false;
            }
            else
            {
                Duet3Ydeg09_radioButton.Checked = false;
                Duet3Ydeg18_radioButton.Checked = true;

            }
            Duet3Yinterpolate_checkBox.Checked = Setting.Duet3_YInterpolate;
            Duet3YtravelPerRev_textBox.Text = Setting.Duet3_YTravelPerRev.ToString();
            if (!SetDuet3Ystepping()) return false;

            Duet3YCurrent_maskedTextBox.Text = Setting.Duet3_YCurrent.ToString();
            SetDuet3Ycurr(Setting.Duet3_YCurrent);

            SettingDuet3YmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void Duet3Yspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3Yspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Yspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_Yspeed = speed;
                    SetDuet3Yspeed(speed);
                    Duet3Yspeed_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Yspeed(double speed)
        {
            speed = speed * 60; // speed is set in mm/min, but reported in mm/s.
            return Cnc.Duet3.Write_m("M203 Y" + speed.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // acceleration
        private void Duet3Yacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            Duet3Yacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Yacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Duet3_Yacc = acc;
                    SetDuet3Yacc(acc);
                }
                Duet3Yacceleration_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private bool SetDuet3Yacc(double acc)
        {
            return Cnc.Duet3.Write_m("M201 Y" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetDuet3Ystepping()
        {
            string i;
            if (Setting.Duet3_YInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Duet3.Write_m("M350 Y" + Setting.Duet3_YMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Duet3_YMicroStep * 360.0 / Setting.Duet3_YDegPerStep;
            steps = steps / Setting.Duet3_YTravelPerRev;
            return Cnc.Duet3.Write_m("M92 Y" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void Duet3Ymicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            Duet3Ymicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3Ymicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Duet3_YMicroStep = usteps;
                        SetDuet3Ystepping();
                        Duet3Ymicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
            }
        }


        // =================================================================================
        // interpolate
        private void Duet3Yinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Duet3_YInterpolate = Duet3Yinterpolate_checkBox.Checked;
            if (!SettingDuet3YmotorParameters)
            {
                SetDuet3Ystepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void Duet3Ydeg09_radioButton_Click(object sender, EventArgs e)
        {
            Duet3YDegChange();
        }

        private void Duet3Ydeg18_radioButton_Click(object sender, EventArgs e)
        {
            Duet3YDegChange();
        }

        private void Duet3YDegChange()
        {
            if (Duet3Ydeg09_radioButton.Checked)
            {
                Setting.Duet3_YDegPerStep = 0.9;
            }
            else
            {
                Setting.Duet3_YDegPerStep = 0.9;
            }
            SetDuet3Ystepping();
        }

        // =================================================================================
        // travel per revolution
        private void Duet3YtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            Duet3YtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3YtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Duet3_YTravelPerRev = travel;
                    SetDuet3Ystepping();
                    Duet3YtravelPerRev_textBox.ForeColor = Color.Black;
                }
            }
        }


        // =================================================================================
        // motor current
        private void Duet3YCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            Duet3YCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3YCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Duet3_YCurrent = curr;
                    SetDuet3Ycurr(curr);
                    Duet3YCurrent_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Ycurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 Y" + curr.ToString());
        }

        #endregion

        // =================================================================================
        // Z motor
        // =================================================================================
        #region Duet3Zmotor

        private bool SettingDuet3ZmotorParameters = false;


        public bool SetDuet3ZmotorParameters()
        {
            SettingDuet3ZmotorParameters = true;

            Duet3Zspeed_maskedTextBox.Text = Setting.Duet3_Zspeed.ToString();
            if (!SetDuet3Zspeed(Setting.Duet3_Zspeed)) return false;

            Duet3Zacceleration_maskedTextBox.Text = Setting.Duet3_Zacc.ToString();
            if (!SetDuet3Zacc(Setting.Duet3_Zacc)) return false;

            Duet3Zmicrosteps_maskedTextBox.Text = Setting.Duet3_ZMicroStep.ToString();
            if (Setting.Duet3_ZDegPerStep > 1.0)
            {
                Duet3Zdeg09_radioButton.Checked = true;
                Duet3Zdeg18_radioButton.Checked = false;
            }
            else
            {
                Duet3Zdeg09_radioButton.Checked = false;
                Duet3Zdeg18_radioButton.Checked = true;

            }
            Duet3Zinterpolate_checkBox.Checked = Setting.Duet3_ZInterpolate;
            Duet3ZtravelPerRev_textBox.Text = Setting.Duet3_ZTravelPerRev.ToString();
            if (!SetDuet3Zstepping()) return false;

            Duet3ZCurrent_maskedTextBox.Text = Setting.Duet3_ZCurrent.ToString();
            SetDuet3Zcurr(Setting.Duet3_ZCurrent);

            SettingDuet3ZmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void Duet3Zspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3Zspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Zspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_Zspeed = speed;
                    SetDuet3Zspeed(speed);
                    Duet3Zspeed_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Zspeed(double speed)
        {
            speed = speed * 60; // speed is set in mm/min, but reported in mm/s.
            return Cnc.Duet3.Write_m("M203 Z" + speed.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // acceleration
        private void Duet3Zacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            Duet3Zacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Zacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Duet3_Zacc = acc;
                    SetDuet3Zacc(acc);
                }
                Duet3Zacceleration_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private bool SetDuet3Zacc(double acc)
        {
            return Cnc.Duet3.Write_m("M201 Z" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetDuet3Zstepping()
        {
            string i;
            if (Setting.Duet3_ZInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Duet3.Write_m("M350 Z" + Setting.Duet3_ZMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Duet3_ZMicroStep * 360.0 / Setting.Duet3_ZDegPerStep;
            steps = steps / Setting.Duet3_ZTravelPerRev;
            return Cnc.Duet3.Write_m("M92 Z" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void Duet3Zmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            Duet3Zmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3Zmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Duet3_ZMicroStep = usteps;
                        SetDuet3Zstepping();
                        Duet3Zmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
            }
        }


        // =================================================================================
        // interpolate
        private void Duet3Zinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Duet3_ZInterpolate = Duet3Zinterpolate_checkBox.Checked;
            if (!SettingDuet3ZmotorParameters)
            {
                SetDuet3Zstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void Duet3Zdeg09_radioButton_Click(object sender, EventArgs e)
        {
            Duet3ZDegChange();
        }

        private void Duet3Zdeg18_radioButton_Click(object sender, EventArgs e)
        {
            Duet3ZDegChange();
        }

        private void Duet3ZDegChange()
        {
            if (Duet3Zdeg09_radioButton.Checked)
            {
                Setting.Duet3_ZDegPerStep = 0.9;
            }
            else
            {
                Setting.Duet3_ZDegPerStep = 0.9;
            }
            SetDuet3Zstepping();
        }

        // =================================================================================
        // travel per revolution
        private void Duet3ZtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            Duet3ZtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3ZtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Duet3_ZTravelPerRev = travel;
                    SetDuet3Zstepping();
                    Duet3ZtravelPerRev_textBox.ForeColor = Color.Black;
                }
            }
        }


        // =================================================================================
        // motor current
        private void Duet3ZCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            Duet3ZCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3ZCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Duet3_ZCurrent = curr;
                    SetDuet3Zcurr(curr);
                    Duet3ZCurrent_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Zcurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 Z" + curr.ToString());
        }

        #endregion

        // =================================================================================
        // A motor
        // =================================================================================
        #region Duet3Amotor

        private bool SettingDuet3AmotorParameters = false;


        public bool SetDuet3AmotorParameters()
        {
            SettingDuet3AmotorParameters = true;

            Duet3Aspeed_maskedTextBox.Text = Setting.Duet3_Aspeed.ToString();
            if (!SetDuet3Aspeed(Setting.Duet3_Aspeed)) return false;

            Duet3Aacceleration_maskedTextBox.Text = Setting.Duet3_Aacc.ToString();
            if (!SetDuet3Aacc(Setting.Duet3_Aacc)) return false;

            Duet3Amicrosteps_maskedTextBox.Text = Setting.Duet3_AMicroStep.ToString();
            if (Setting.Duet3_ADegPerStep > 1.0)
            {
                Duet3Adeg09_radioButton.Checked = true;
                Duet3Adeg18_radioButton.Checked = false;
            }
            else
            {
                Duet3Adeg09_radioButton.Checked = false;
                Duet3Adeg18_radioButton.Checked = true;

            }
            Duet3Ainterpolate_checkBox.Checked = Setting.Duet3_AInterpolate;
            Duet3AtravelPerRev_textBox.Text = Setting.Duet3_ATravelPerRev.ToString();
            if (!SetDuet3Astepping()) return false;

            Duet3ACurrent_maskedTextBox.Text = Setting.Duet3_ACurrent.ToString();
            SetDuet3Acurr(Setting.Duet3_ACurrent);

            SettingDuet3AmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void Duet3Aspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3Aspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Aspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_Aspeed = speed;
                    SetDuet3Aspeed(speed);
                    Duet3Aspeed_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Aspeed(double speed)
        {
            speed = speed * 60; // speed is set in mm/min, but reported in mm/s.
            return Cnc.Duet3.Write_m("M203 A" + speed.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // acceleration
        private void Duet3Aacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            Duet3Aacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3Aacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Duet3_Aacc = acc;
                    SetDuet3Aacc(acc);
                }
                Duet3Aacceleration_maskedTextBox.ForeColor = Color.Black;
            }
        }

        private bool SetDuet3Aacc(double acc)
        {
            return Cnc.Duet3.Write_m("M201 A" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetDuet3Astepping()
        {
            string i;
            if (Setting.Duet3_AInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Duet3.Write_m("M350 A" + Setting.Duet3_AMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Duet3_AMicroStep * 360.0 / Setting.Duet3_ADegPerStep;
            steps = steps / Setting.Duet3_ATravelPerRev;
            return Cnc.Duet3.Write_m("M92 A" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void Duet3Amicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            Duet3Amicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3Amicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Duet3_AMicroStep = usteps;
                        SetDuet3Astepping();
                        Duet3Amicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
            }
        }


        // =================================================================================
        // interpolate
        private void Duet3Ainterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Duet3_AInterpolate = Duet3Ainterpolate_checkBox.Checked;
            if (!SettingDuet3AmotorParameters)
            {
                SetDuet3Astepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void Duet3Adeg09_radioButton_Click(object sender, EventArgs e)
        {
            Duet3ADegChange();
        }

        private void Duet3Adeg18_radioButton_Click(object sender, EventArgs e)
        {
            Duet3ADegChange();
        }

        private void Duet3ADegChange()
        {
            if (Duet3Adeg09_radioButton.Checked)
            {
                Setting.Duet3_ADegPerStep = 0.9;
            }
            else
            {
                Setting.Duet3_ADegPerStep = 0.9;
            }
            SetDuet3Astepping();
        }

        // =================================================================================
        // travel per revolution
        private void Duet3AtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            Duet3AtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3AtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Duet3_ATravelPerRev = travel;
                    SetDuet3Astepping();
                    Duet3AtravelPerRev_textBox.ForeColor = Color.Black;
                }
            }
        }


        // =================================================================================
        // motor current
        private void Duet3ACurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            Duet3ACurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(Duet3ACurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Duet3_ACurrent = curr;
                    SetDuet3Acurr(curr);
                    Duet3ACurrent_maskedTextBox.ForeColor = Color.Black;
                }
            }
        }

        private bool SetDuet3Acurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 A" + curr.ToString());
        }

        #endregion

        #endregion Duet3



        // =================================================================================
        // TinyG stuff
        // =================================================================================
        #region TinyG
        // =================================================================================

        // Called from TinyG class when TinyG related UI need updating
        public void ValueUpdater(string item, string value)
        {
            if (InvokeRequired) { Invoke(new Action<string, string>(ValueUpdater), new[] { item, value }); return; }
            // DisplayText("ValueUpdater: item= " + item + ", value= " + value);

            switch (item)
            {
                // ==========  position values  ==========
                case "posx":
                    Update_xpos(value);
                    break;
                case "posy":
                    Update_ypos(value);
                    break;
                case "posz":
                    Update_zpos(value);
                    break;
                case "posa":
                    Update_apos(value);
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
                Cnc.Controlboard = CNC.ControlBoardType.TinyG;
                DisplayText("TinyG board found.");
            }
            else
            {
                Cnc.Controlboard = CNC.ControlBoardType.other;
                DisplayText("Unknown control board.");
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
        #region mpo  // mpo*: Position
        // * update
        private void Update_xpos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_xpos), new[] { value }); return; }
            TrueX_label.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            //DisplayText("Update_xpos: " + Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture));
        }

        private void Update_ypos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_ypos), new[] { value }); return; }
            ypos_textBox.Text = value;
            xpos_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            //DisplayText("Update_ypos, x: " + Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture));
        }

        private void Update_zpos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_zpos), new[] { value }); return; }
            zpos_textBox.Text = value;
        }

        private void Update_apos(string value)
        {
            if (InvokeRequired) { Invoke(new Action<string>(Update_apos), new[] { value }); return; }
            apos_textBox.Text = value;
        }

        #endregion


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


    // =================================================================================
    // Really, TinyG board settings

    public static class BoardSettings
    {

        static public FormMain MainForm { get; set; }

        // Board settings file: Text file, starting with 8 characters board name and \n\r, 
        // then the setings in Json format
        static public bool Save(TinyGSettings TinyGSettings, string FileName)
        {
            try
            {
                if (MainForm.Cnc.Controlboard == CNC.ControlBoardType.TinyG)
                {
                    MainForm.DisplayText("Writing TinyG settings file: " + FileName);
                    File.WriteAllText(FileName, "TinyG   \n\r" + JsonConvert.SerializeObject(TinyGSettings, Formatting.Indented));
                    MainForm.DisplayText("Done.");
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

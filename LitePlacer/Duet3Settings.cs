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
    // This file has things that are related to Duet 3 control board settings, UI, their storage and retrieval.
    //

    // For settings, see AppSettings.cs (VS bug prevents having the settings here.
    public partial class FormMain
    {
        // =================================================================================
        // We can go directly to business. No need to go trough cnc class in motor settings,
        // as they are visible only when the corresponding board is already found.
        // Duet3 is owned by Cnc, so we do Cnc.Duet3.xxx(), not Cnc.xxx()

        // =================================================================================
        // X motor
        // =================================================================================
        #region Duet3Xmotor

        private bool SettingDuet3XmotorParameters = false;
        public bool SetDuet3XmotorParameters()
        {
            SettingDuet3XmotorParameters = true;    // to not trigger checkbox related events
            Duet3Xspeed_maskedTextBox.Text = Setting.Duet3_Xspeed.ToString();
            if (!SetDuet3Xspeed(Setting.Duet3_Xspeed)) return false;

            Duet3Xacceleration_maskedTextBox.Text = Setting.Duet3_Xacc.ToString();
            if (!SetDuet3Xacc(Setting.Duet3_Xacc)) return false;

            Duet3Xmicrosteps_maskedTextBox.Text = Setting.Duet3_XMicroStep.ToString();
            if (Setting.Duet3_XDegPerStep < 1.0)
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
            if (!SetDuet3Xstepping())
            {
                SettingDuet3XmotorParameters = false;
                return false;
            }
            Duet3XCurrent_maskedTextBox.Text = Setting.Duet3_XCurrent.ToString();
            SetDuet3Xcurr(Setting.Duet3_XCurrent);
            Duet3XhomingSpeed_maskedTextBox.Text = Setting.Duet3_XHomingSpeed.ToString();
            Duet3XHomingBackoff_maskedTextBox.Text = Setting.Duet3_XHomingBackoff.ToString();
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Xspeed(double speed)
        {
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Xcurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 X" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void Duet3XhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3XhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3XhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_XHomingSpeed = speed;
                    Duet3XhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Duet3XHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            Duet3XHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3XHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Duet3_XHomingBackoff = backoff;
                    Duet3XHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion Duet3Xmotor

        // =================================================================================
        // Y motor
        // =================================================================================
        #region Duet3Ymotor

        private bool SettingDuet3YmotorParameters = false;

        public bool SetDuet3YmotorParameters()
        {
            SettingDuet3YmotorParameters = true;    // to not trigger checkbox related events
            Duet3Yspeed_maskedTextBox.Text = Setting.Duet3_Yspeed.ToString();
            if (!SetDuet3Yspeed(Setting.Duet3_Yspeed)) return false;

            Duet3Yacceleration_maskedTextBox.Text = Setting.Duet3_Yacc.ToString();
            if (!SetDuet3Yacc(Setting.Duet3_Yacc)) return false;

            Duet3Ymicrosteps_maskedTextBox.Text = Setting.Duet3_YMicroStep.ToString();
            if (Setting.Duet3_YDegPerStep < 1.0)
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
            if (!SetDuet3Ystepping())
            {
                SettingDuet3YmotorParameters = false;
                return false;
            }
            Duet3YCurrent_maskedTextBox.Text = Setting.Duet3_YCurrent.ToString();
            SetDuet3Ycurr(Setting.Duet3_YCurrent);
            Duet3YhomingSpeed_maskedTextBox.Text = Setting.Duet3_YHomingSpeed.ToString();
            Duet3YHomingBackoff_maskedTextBox.Text = Setting.Duet3_YHomingBackoff.ToString();
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Yspeed(double speed)
        {
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Ycurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 Y" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void Duet3YhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3YhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3YhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_YHomingSpeed = speed;
                    Duet3YhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Duet3YHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            Duet3YHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3YHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Duet3_YHomingBackoff = backoff;
                    Duet3YHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }
        #endregion Duet3Ymotor

        // =================================================================================
        // Z motor
        // =================================================================================

        #region Duet3Zmotor

        private bool SettingDuet3ZmotorParameters = false;
        public bool SetDuet3ZmotorParameters()
        {
            SettingDuet3ZmotorParameters = true;    // to not trigger checkbox related events
            Duet3Zspeed_maskedTextBox.Text = Setting.Duet3_Zspeed.ToString();
            if (!SetDuet3Zspeed(Setting.Duet3_Zspeed)) return false;

            Duet3Zacceleration_maskedTextBox.Text = Setting.Duet3_Zacc.ToString();
            if (!SetDuet3Zacc(Setting.Duet3_Zacc)) return false;

            Duet3Zmicrosteps_maskedTextBox.Text = Setting.Duet3_ZMicroStep.ToString();
            if (Setting.Duet3_ZDegPerStep < 1.0)
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
            if (!SetDuet3Zstepping())
            {
                SettingDuet3ZmotorParameters = false;
                return false;
            }
            Duet3ZCurrent_maskedTextBox.Text = Setting.Duet3_ZCurrent.ToString();
            SetDuet3Zcurr(Setting.Duet3_ZCurrent);
            Duet3ZhomingSpeed_maskedTextBox.Text = Setting.Duet3_ZHomingSpeed.ToString();
            Duet3ZHomingBackoff_maskedTextBox.Text = Setting.Duet3_ZHomingBackoff.ToString();
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Zspeed(double speed)
        {
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Zcurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 Z" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void Duet3ZhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            Duet3ZhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3ZhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Duet3_ZHomingSpeed = speed;
                    Duet3ZhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void Duet3ZHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            Duet3ZHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Duet3ZHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Duet3_ZHomingBackoff = backoff;
                    Duet3ZHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion Duet3Zmotor

        // =================================================================================
        // A motor
        // =================================================================================

        #region Duet3Amotor

        private bool SettingDuet3AmotorParameters = false;
        public bool SetDuet3AmotorParameters()
        {
            SettingDuet3AmotorParameters = true;    // to not trigger checkbox related events
            Duet3Aspeed_maskedTextBox.Text = Setting.Duet3_Aspeed.ToString();
            if (!SetDuet3Aspeed(Setting.Duet3_Aspeed)) return false;

            Duet3Aacceleration_maskedTextBox.Text = Setting.Duet3_Aacc.ToString();
            if (!SetDuet3Aacc(Setting.Duet3_Aacc)) return false;

            Duet3Amicrosteps_maskedTextBox.Text = Setting.Duet3_AMicroStep.ToString();
            if (Setting.Duet3_ADegPerStep < 1.0)
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
            if (!SetDuet3Astepping())
            {
                SettingDuet3AmotorParameters = false;
                return false;
            }
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Aspeed(double speed)
        {
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
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
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetDuet3Acurr(int curr)
        {
            return Cnc.Duet3.Write_m("M906 A" + curr.ToString());
        }


        #endregion Duet3Amotor


    }
}

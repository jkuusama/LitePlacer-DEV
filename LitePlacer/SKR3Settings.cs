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
    // This file has things that are related to SKR 3 control board settings, UI, their storage and retrieval.
    //

    // For settings, see AppSettings.cs; VS bug prevents having the settings here.
    public partial class FormMain
    {
        // =================================================================================
        // We can go directly to business. No need to go trough cnc class in motor settings,
        // as they are visible only when the corresponding board is already found.
        // SKR3 is owned by Cnc, so we do Cnc.SKR3.xxx(), not Cnc.xxx()

        // =================================================================================
        // X motor
        // =================================================================================
        #region SKR3Xmotor

        private bool SettingSKR3XmotorParameters = false;
        public bool SetSKR3XmotorParameters()
        {
            SettingSKR3XmotorParameters = true;    // to not trigger checkbox related events
            SKR3Xspeed_maskedTextBox.Text = Setting.SKR3_Xspeed.ToString();
            if (!SetSKR3Xspeed(Setting.SKR3_Xspeed)) return false;

            SKR3Xacceleration_maskedTextBox.Text = Setting.SKR3_Xacc.ToString();
            if (!SetSKR3Xacc(Setting.SKR3_Xacc)) return false;

            SKR3Xmicrosteps_maskedTextBox.Text = Setting.SKR3_XMicroStep.ToString();
            if (Setting.SKR3_XDegPerStep < 1.0)
            {
                SKR3Xdeg09_radioButton.Checked = true;
                SKR3Xdeg18_radioButton.Checked = false;
            }
            else
            {
                SKR3Xdeg09_radioButton.Checked = false;
                SKR3Xdeg18_radioButton.Checked = true;

            }
            SKR3Xinterpolate_checkBox.Checked = Setting.SKR3_XInterpolate;
            SKR3XtravelPerRev_textBox.Text = Setting.SKR3_XTravelPerRev.ToString();
            if (!SetSKR3Xstepping())
            {
                SettingSKR3XmotorParameters = false;
                return false;
            }
            SKR3XCurrent_maskedTextBox.Text = Setting.SKR3_XCurrent.ToString();
            SetSKR3Xcurr(Setting.SKR3_XCurrent);
            SKR3XhomingSpeed_maskedTextBox.Text = Setting.SKR3_XHomingSpeed.ToString();
            SKR3XHomingBackoff_maskedTextBox.Text = Setting.SKR3_XHomingBackoff.ToString();
            SettingSKR3XmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void SKR3Xspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3Xspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Xspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_Xspeed = speed;
                    SetSKR3Xspeed(speed);
                    SKR3Xspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Xspeed(double speed)
        {
            return Cnc.SKR3.Write_m("M203 X" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void SKR3Xacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            SKR3Xacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Xacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.SKR3_Xacc = acc;
                    SetSKR3Xacc(acc);
                }
                SKR3Xacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Xacc(double acc)
        {
            return Cnc.SKR3.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetSKR3Xstepping()
        {
            string i;
            if (Setting.SKR3_XInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.SKR3.Write_m("M350 X" + Setting.SKR3_XMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.SKR3_XMicroStep * 360.0 / Setting.SKR3_XDegPerStep;
            steps = steps / Setting.SKR3_XTravelPerRev;
            return Cnc.SKR3.Write_m("M92 X" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void SKR3Xmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            SKR3Xmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3Xmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ( (usteps>1) && (usteps<=256) &&
                        ((usteps & (usteps - 1)) == 0))   
                    {
                        Setting.SKR3_XMicroStep = usteps;
                        SetSKR3Xstepping();
                        SKR3Xmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void SKR3Xinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.SKR3_XInterpolate = SKR3Xinterpolate_checkBox.Checked;
            if (!SettingSKR3XmotorParameters)
            {
                SetSKR3Xstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void SKR3Xdeg09_radioButton_Click(object sender, EventArgs e)
        {
            SKR3XDegChange();
        }

        private void SKR3Xdeg18_radioButton_Click(object sender, EventArgs e)
        {
            SKR3XDegChange();
        }

        private void SKR3XDegChange()
        {
            if (SKR3Xdeg09_radioButton.Checked)
            {
                Setting.SKR3_XDegPerStep = 0.9;
            }
            else
            {
                Setting.SKR3_XDegPerStep = 0.9;
            }
            SetSKR3Xstepping();
        }

        // =================================================================================
        // travel per revolution
        private void SKR3XtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            SKR3XtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3XtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.SKR3_XTravelPerRev = travel;
                    SetSKR3Xstepping();
                    SKR3XtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void SKR3XCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            SKR3XCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3XCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.SKR3_XCurrent = curr;
                    SetSKR3Xcurr(curr);
                    SKR3XCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Xcurr(int curr)
        {
            return Cnc.SKR3.Write_m("M906 X" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void SKR3XhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3XhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3XhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_XHomingSpeed = speed;
                    SKR3XhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void SKR3XHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            SKR3XHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3XHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.SKR3_XHomingBackoff = backoff;
                    SKR3XHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion SKR3Xmotor

        // =================================================================================
        // Y motor
        // =================================================================================
        #region SKR3Ymotor

        private bool SettingSKR3YmotorParameters = false;

        public bool SetSKR3YmotorParameters()
        {
            SettingSKR3YmotorParameters = true;    // to not trigger checkbox related events
            SKR3Yspeed_maskedTextBox.Text = Setting.SKR3_Yspeed.ToString();
            if (!SetSKR3Yspeed(Setting.SKR3_Yspeed)) return false;

            SKR3Yacceleration_maskedTextBox.Text = Setting.SKR3_Yacc.ToString();
            if (!SetSKR3Yacc(Setting.SKR3_Yacc)) return false;

            SKR3Ymicrosteps_maskedTextBox.Text = Setting.SKR3_YMicroStep.ToString();
            if (Setting.SKR3_YDegPerStep < 1.0)
            {
                SKR3Ydeg09_radioButton.Checked = true;
                SKR3Ydeg18_radioButton.Checked = false;
            }
            else
            {
                SKR3Ydeg09_radioButton.Checked = false;
                SKR3Ydeg18_radioButton.Checked = true;

            }
            SKR3Yinterpolate_checkBox.Checked = Setting.SKR3_YInterpolate;
            SKR3YtravelPerRev_textBox.Text = Setting.SKR3_YTravelPerRev.ToString();
            if (!SetSKR3Ystepping())
            {
                SettingSKR3YmotorParameters = false;
                return false;
            }
            SKR3YCurrent_maskedTextBox.Text = Setting.SKR3_YCurrent.ToString();
            SetSKR3Ycurr(Setting.SKR3_YCurrent);
            SKR3YhomingSpeed_maskedTextBox.Text = Setting.SKR3_YHomingSpeed.ToString();
            SKR3YHomingBackoff_maskedTextBox.Text = Setting.SKR3_YHomingBackoff.ToString();
            SettingSKR3YmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void SKR3Yspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3Yspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Yspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_Yspeed = speed;
                    SetSKR3Yspeed(speed);
                    SKR3Yspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Yspeed(double speed)
        {
            return Cnc.SKR3.Write_m("M203 Y" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void SKR3Yacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            SKR3Yacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Yacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.SKR3_Yacc = acc;
                    SetSKR3Yacc(acc);
                }
                SKR3Yacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Yacc(double acc)
        {
            return Cnc.SKR3.Write_m("M201 Y" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetSKR3Ystepping()
        {
            string i;
            if (Setting.SKR3_YInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.SKR3.Write_m("M350 Y" + Setting.SKR3_YMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.SKR3_YMicroStep * 360.0 / Setting.SKR3_YDegPerStep;
            steps = steps / Setting.SKR3_YTravelPerRev;
            return Cnc.SKR3.Write_m("M92 Y" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void SKR3Ymicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            SKR3Ymicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3Ymicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.SKR3_YMicroStep = usteps;
                        SetSKR3Ystepping();
                        SKR3Ymicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void SKR3Yinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.SKR3_YInterpolate = SKR3Yinterpolate_checkBox.Checked;
            if (!SettingSKR3YmotorParameters)
            {
                SetSKR3Ystepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void SKR3Ydeg09_radioButton_Click(object sender, EventArgs e)
        {
            SKR3YDegChange();
        }

        private void SKR3Ydeg18_radioButton_Click(object sender, EventArgs e)
        {
            SKR3YDegChange();
        }

        private void SKR3YDegChange()
        {
            if (SKR3Ydeg09_radioButton.Checked)
            {
                Setting.SKR3_YDegPerStep = 0.9;
            }
            else
            {
                Setting.SKR3_YDegPerStep = 0.9;
            }
            SetSKR3Ystepping();
        }

        // =================================================================================
        // travel per revolution
        private void SKR3YtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            SKR3YtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3YtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.SKR3_YTravelPerRev = travel;
                    SetSKR3Ystepping();
                    SKR3YtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void SKR3YCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            SKR3YCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3YCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.SKR3_YCurrent = curr;
                    SetSKR3Ycurr(curr);
                    SKR3YCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Ycurr(int curr)
        {
            return Cnc.SKR3.Write_m("M906 Y" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void SKR3YhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3YhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3YhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_YHomingSpeed = speed;
                    SKR3YhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void SKR3YHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            SKR3YHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3YHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.SKR3_YHomingBackoff = backoff;
                    SKR3YHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }
        #endregion SKR3Ymotor

        // =================================================================================
        // Z motor
        // =================================================================================

        #region SKR3Zmotor

        private bool SettingSKR3ZmotorParameters = false;
        public bool SetSKR3ZmotorParameters()
        {
            SettingSKR3ZmotorParameters = true;    // to not trigger checkbox related events
            SKR3Zspeed_maskedTextBox.Text = Setting.SKR3_Zspeed.ToString();
            if (!SetSKR3Zspeed(Setting.SKR3_Zspeed)) return false;

            SKR3Zacceleration_maskedTextBox.Text = Setting.SKR3_Zacc.ToString();
            if (!SetSKR3Zacc(Setting.SKR3_Zacc)) return false;

            SKR3Zmicrosteps_maskedTextBox.Text = Setting.SKR3_ZMicroStep.ToString();
            if (Setting.SKR3_ZDegPerStep < 1.0)
            {
                SKR3Zdeg09_radioButton.Checked = true;
                SKR3Zdeg18_radioButton.Checked = false;
            }
            else
            {
                SKR3Zdeg09_radioButton.Checked = false;
                SKR3Zdeg18_radioButton.Checked = true;

            }
            SKR3Zinterpolate_checkBox.Checked = Setting.SKR3_ZInterpolate;
            SKR3ZtravelPerRev_textBox.Text = Setting.SKR3_ZTravelPerRev.ToString();
            if (!SetSKR3Zstepping())
            {
                SettingSKR3ZmotorParameters = false;
                return false;
            }
            SKR3ZCurrent_maskedTextBox.Text = Setting.SKR3_ZCurrent.ToString();
            SetSKR3Zcurr(Setting.SKR3_ZCurrent);
            SKR3ZhomingSpeed_maskedTextBox.Text = Setting.SKR3_ZHomingSpeed.ToString();
            SKR3ZHomingBackoff_maskedTextBox.Text = Setting.SKR3_ZHomingBackoff.ToString();
            SettingSKR3ZmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void SKR3Zspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3Zspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Zspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_Zspeed = speed;
                    SetSKR3Zspeed(speed);
                    SKR3Zspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Zspeed(double speed)
        {
            return Cnc.SKR3.Write_m("M203 Z" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void SKR3Zacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            SKR3Zacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Zacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.SKR3_Zacc = acc;
                    SetSKR3Zacc(acc);
                }
                SKR3Zacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Zacc(double acc)
        {
            return Cnc.SKR3.Write_m("M201 Z" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetSKR3Zstepping()
        {
            string i;
            if (Setting.SKR3_ZInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.SKR3.Write_m("M350 Z" + Setting.SKR3_ZMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.SKR3_ZMicroStep * 360.0 / Setting.SKR3_ZDegPerStep;
            steps = steps / Setting.SKR3_ZTravelPerRev;
            return Cnc.SKR3.Write_m("M92 Z" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void SKR3Zmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            SKR3Zmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3Zmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.SKR3_ZMicroStep = usteps;
                        SetSKR3Zstepping();
                        SKR3Zmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void SKR3Zinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.SKR3_ZInterpolate = SKR3Zinterpolate_checkBox.Checked;
            if (!SettingSKR3ZmotorParameters)
            {
                SetSKR3Zstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void SKR3Zdeg09_radioButton_Click(object sender, EventArgs e)
        {
            SKR3ZDegChange();
        }

        private void SKR3Zdeg18_radioButton_Click(object sender, EventArgs e)
        {
            SKR3ZDegChange();
        }

        private void SKR3ZDegChange()
        {
            if (SKR3Zdeg09_radioButton.Checked)
            {
                Setting.SKR3_ZDegPerStep = 0.9;
            }
            else
            {
                Setting.SKR3_ZDegPerStep = 0.9;
            }
            SetSKR3Zstepping();
        }

        // =================================================================================
        // travel per revolution
        private void SKR3ZtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            SKR3ZtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3ZtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.SKR3_ZTravelPerRev = travel;
                    SetSKR3Zstepping();
                    SKR3ZtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void SKR3ZCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            SKR3ZCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3ZCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.SKR3_ZCurrent = curr;
                    SetSKR3Zcurr(curr);
                    SKR3ZCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Zcurr(int curr)
        {
            return Cnc.SKR3.Write_m("M906 Z" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void SKR3ZhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3ZhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3ZhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_ZHomingSpeed = speed;
                    SKR3ZhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void SKR3ZHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            SKR3ZHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3ZHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.SKR3_ZHomingBackoff = backoff;
                    SKR3ZHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion SKR3Zmotor

        // =================================================================================
        // A motor
        // =================================================================================

        #region SKR3Amotor

        private bool SettingSKR3AmotorParameters = false;
        public bool SetSKR3AmotorParameters()
        {
            SettingSKR3AmotorParameters = true;    // to not trigger checkbox related events
            SKR3Aspeed_maskedTextBox.Text = Setting.SKR3_Aspeed.ToString();
            if (!SetSKR3Aspeed(Setting.SKR3_Aspeed)) return false;

            SKR3Aacceleration_maskedTextBox.Text = Setting.SKR3_Aacc.ToString();
            if (!SetSKR3Aacc(Setting.SKR3_Aacc)) return false;

            SKR3Amicrosteps_maskedTextBox.Text = Setting.SKR3_AMicroStep.ToString();
            if (Setting.SKR3_ADegPerStep < 1.0)
            {
                SKR3Adeg09_radioButton.Checked = true;
                SKR3Adeg18_radioButton.Checked = false;
            }
            else
            {
                SKR3Adeg09_radioButton.Checked = false;
                SKR3Adeg18_radioButton.Checked = true;

            }
            SKR3Ainterpolate_checkBox.Checked = Setting.SKR3_AInterpolate;
            SKR3AtravelPerRev_textBox.Text = Setting.SKR3_ATravelPerRev.ToString();
            if (!SetSKR3Astepping())
            {
                SettingSKR3AmotorParameters = false;
                return false;
            }
            SKR3ACurrent_maskedTextBox.Text = Setting.SKR3_ACurrent.ToString();
            SetSKR3Acurr(Setting.SKR3_ACurrent);
            SettingSKR3AmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void SKR3Aspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            SKR3Aspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Aspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.SKR3_Aspeed = speed;
                    SetSKR3Aspeed(speed);
                    SKR3Aspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Aspeed(double speed)
        {
            return Cnc.SKR3.Write_m("M203 A" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void SKR3Aacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            SKR3Aacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3Aacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.SKR3_Aacc = acc;
                    SetSKR3Aacc(acc);
                }
                SKR3Aacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Aacc(double acc)
        {
            return Cnc.SKR3.Write_m("M201 A" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetSKR3Astepping()
        {
            string i;
            if (Setting.SKR3_AInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.SKR3.Write_m("M350 A" + Setting.SKR3_AMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.SKR3_AMicroStep * 360.0 / Setting.SKR3_ADegPerStep;
            steps = steps / Setting.SKR3_ATravelPerRev;
            return Cnc.SKR3.Write_m("M92 A" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void SKR3Amicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            SKR3Amicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3Amicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.SKR3_AMicroStep = usteps;
                        SetSKR3Astepping();
                        SKR3Amicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void SKR3Ainterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.SKR3_AInterpolate = SKR3Ainterpolate_checkBox.Checked;
            if (!SettingSKR3AmotorParameters)
            {
                SetSKR3Astepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void SKR3Adeg09_radioButton_Click(object sender, EventArgs e)
        {
            SKR3ADegChange();
        }

        private void SKR3Adeg18_radioButton_Click(object sender, EventArgs e)
        {
            SKR3ADegChange();
        }

        private void SKR3ADegChange()
        {
            if (SKR3Adeg09_radioButton.Checked)
            {
                Setting.SKR3_ADegPerStep = 0.9;
            }
            else
            {
                Setting.SKR3_ADegPerStep = 0.9;
            }
            SetSKR3Astepping();
        }

        // =================================================================================
        // travel per revolution
        private void SKR3AtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            SKR3AtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SKR3AtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.SKR3_ATravelPerRev = travel;
                    SetSKR3Astepping();
                    SKR3AtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void SKR3ACurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            SKR3ACurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(SKR3ACurrent_maskedTextBox.Text, out curr))
                {
                    Setting.SKR3_ACurrent = curr;
                    SetSKR3Acurr(curr);
                    SKR3ACurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetSKR3Acurr(int curr)
        {
            return Cnc.SKR3.Write_m("M906 A" + curr.ToString());
        }


        #endregion SKR3Amotor


    }
}

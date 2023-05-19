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
        // Marlin is owned by Cnc, so we do Cnc.Marlin.xxx(), not Cnc.xxx()

        // =================================================================================
        // X motor
        // =================================================================================
        #region MarlinXmotor

        private bool SettingMarlinXmotorParameters = false;
        public bool SetMarlinXmotorParameters()
        {
            SettingMarlinXmotorParameters = true;    // to not trigger checkbox related events
            MarlinXspeed_maskedTextBox.Text = Setting.Marlin_Xspeed.ToString();
            if (!SetMarlinXspeed(Setting.Marlin_Xspeed)) return false;

            MarlinXacceleration_maskedTextBox.Text = Setting.Marlin_Xacc.ToString();
            if (!SetMarlinXacc(Setting.Marlin_Xacc)) return false;

            MarlinXmicrosteps_maskedTextBox.Text = Setting.Marlin_XMicroStep.ToString();
            if (Setting.Marlin_XDegPerStep < 1.0)
            {
                MarlinXdeg09_radioButton.Checked = true;
                MarlinXdeg18_radioButton.Checked = false;
            }
            else
            {
                MarlinXdeg09_radioButton.Checked = false;
                MarlinXdeg18_radioButton.Checked = true;

            }
            MarlinXinterpolate_checkBox.Checked = Setting.Marlin_XInterpolate;
            MarlinXtravelPerRev_textBox.Text = Setting.Marlin_XTravelPerRev.ToString();
            if (!SetMarlinXstepping())
            {
                SettingMarlinXmotorParameters = false;
                return false;
            }
            MarlinXCurrent_maskedTextBox.Text = Setting.Marlin_XCurrent.ToString();
            SetMarlinXcurr(Setting.Marlin_XCurrent);
            MarlinXhomingSpeed_maskedTextBox.Text = Setting.Marlin_XHomingSpeed.ToString();
            MarlinXHomingBackoff_maskedTextBox.Text = Setting.Marlin_XHomingBackoff.ToString();
            SettingMarlinXmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinXspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinXspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinXspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Xspeed = speed;
                    SetMarlinXspeed(speed);
                    MarlinXspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinXspeed(double speed)
        {
            return Cnc.Marlin.Write_m("M203 X" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void MarlinXacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            MarlinXacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinXacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Xacc = acc;
                    SetMarlinXacc(acc);
                }
                MarlinXacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinXacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinXstepping()
        {
            string i;
            if (Setting.Marlin_XInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Marlin.Write_m("M350 X" + Setting.Marlin_XMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Marlin_XMicroStep * 360.0 / Setting.Marlin_XDegPerStep;
            steps = steps / Setting.Marlin_XTravelPerRev;
            return Cnc.Marlin.Write_m("M92 X" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void MarlinXmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            MarlinXmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinXmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ( (usteps>1) && (usteps<=256) &&
                        ((usteps & (usteps - 1)) == 0))   
                    {
                        Setting.Marlin_XMicroStep = usteps;
                        SetMarlinXstepping();
                        MarlinXmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinXinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_XInterpolate = MarlinXinterpolate_checkBox.Checked;
            if (!SettingMarlinXmotorParameters)
            {
                SetMarlinXstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void MarlinXdeg09_radioButton_Click(object sender, EventArgs e)
        {
            MarlinXDegChange();
        }

        private void MarlinXdeg18_radioButton_Click(object sender, EventArgs e)
        {
            MarlinXDegChange();
        }

        private void MarlinXDegChange()
        {
            if (MarlinXdeg09_radioButton.Checked)
            {
                Setting.Marlin_XDegPerStep = 0.9;
            }
            else
            {
                Setting.Marlin_XDegPerStep = 0.9;
            }
            SetMarlinXstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinXtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            MarlinXtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinXtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Marlin_XTravelPerRev = travel;
                    SetMarlinXstepping();
                    MarlinXtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void MarlinXCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            MarlinXCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinXCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Marlin_XCurrent = curr;
                    SetMarlinXcurr(curr);
                    MarlinXCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinXcurr(int curr)
        {
            return Cnc.Marlin.Write_m("M906 X" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void MarlinXhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinXhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinXhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_XHomingSpeed = speed;
                    MarlinXhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinXHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            MarlinXHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinXHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Marlin_XHomingBackoff = backoff;
                    MarlinXHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion MarlinXmotor

        // =================================================================================
        // Y motor
        // =================================================================================
        #region MarlinYmotor

        private bool SettingMarlinYmotorParameters = false;

        public bool SetMarlinYmotorParameters()
        {
            SettingMarlinYmotorParameters = true;    // to not trigger checkbox related events
            MarlinYspeed_maskedTextBox.Text = Setting.Marlin_Yspeed.ToString();
            if (!SetMarlinYspeed(Setting.Marlin_Yspeed)) return false;

            MarlinYacceleration_maskedTextBox.Text = Setting.Marlin_Yacc.ToString();
            if (!SetMarlinYacc(Setting.Marlin_Yacc)) return false;

            MarlinYmicrosteps_maskedTextBox.Text = Setting.Marlin_YMicroStep.ToString();
            if (Setting.Marlin_YDegPerStep < 1.0)
            {
                MarlinYdeg09_radioButton.Checked = true;
                MarlinYdeg18_radioButton.Checked = false;
            }
            else
            {
                MarlinYdeg09_radioButton.Checked = false;
                MarlinYdeg18_radioButton.Checked = true;

            }
            MarlinYinterpolate_checkBox.Checked = Setting.Marlin_YInterpolate;
            MarlinYtravelPerRev_textBox.Text = Setting.Marlin_YTravelPerRev.ToString();
            if (!SetMarlinYstepping())
            {
                SettingMarlinYmotorParameters = false;
                return false;
            }
            MarlinYCurrent_maskedTextBox.Text = Setting.Marlin_YCurrent.ToString();
            SetMarlinYcurr(Setting.Marlin_YCurrent);
            MarlinYhomingSpeed_maskedTextBox.Text = Setting.Marlin_YHomingSpeed.ToString();
            MarlinYHomingBackoff_maskedTextBox.Text = Setting.Marlin_YHomingBackoff.ToString();
            SettingMarlinYmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinYspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinYspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinYspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Yspeed = speed;
                    SetMarlinYspeed(speed);
                    MarlinYspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinYspeed(double speed)
        {
            return Cnc.Marlin.Write_m("M203 Y" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void MarlinYacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            MarlinYacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinYacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Yacc = acc;
                    SetMarlinYacc(acc);
                }
                MarlinYacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinYacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 Y" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinYstepping()
        {
            string i;
            if (Setting.Marlin_YInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Marlin.Write_m("M350 Y" + Setting.Marlin_YMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Marlin_YMicroStep * 360.0 / Setting.Marlin_YDegPerStep;
            steps = steps / Setting.Marlin_YTravelPerRev;
            return Cnc.Marlin.Write_m("M92 Y" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void MarlinYmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            MarlinYmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinYmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Marlin_YMicroStep = usteps;
                        SetMarlinYstepping();
                        MarlinYmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinYinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_YInterpolate = MarlinYinterpolate_checkBox.Checked;
            if (!SettingMarlinYmotorParameters)
            {
                SetMarlinYstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void MarlinYdeg09_radioButton_Click(object sender, EventArgs e)
        {
            MarlinYDegChange();
        }

        private void MarlinYdeg18_radioButton_Click(object sender, EventArgs e)
        {
            MarlinYDegChange();
        }

        private void MarlinYDegChange()
        {
            if (MarlinYdeg09_radioButton.Checked)
            {
                Setting.Marlin_YDegPerStep = 0.9;
            }
            else
            {
                Setting.Marlin_YDegPerStep = 0.9;
            }
            SetMarlinYstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinYtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            MarlinYtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinYtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Marlin_YTravelPerRev = travel;
                    SetMarlinYstepping();
                    MarlinYtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void MarlinYCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            MarlinYCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinYCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Marlin_YCurrent = curr;
                    SetMarlinYcurr(curr);
                    MarlinYCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinYcurr(int curr)
        {
            return Cnc.Marlin.Write_m("M906 Y" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void MarlinYhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinYhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinYhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_YHomingSpeed = speed;
                    MarlinYhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinYHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            MarlinYHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinYHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Marlin_YHomingBackoff = backoff;
                    MarlinYHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }
        #endregion MarlinYmotor

        // =================================================================================
        // Z motor
        // =================================================================================

        #region MarlinZmotor

        private bool SettingMarlinZmotorParameters = false;
        public bool SetMarlinZmotorParameters()
        {
            SettingMarlinZmotorParameters = true;    // to not trigger checkbox related events
            MarlinZspeed_maskedTextBox.Text = Setting.Marlin_Zspeed.ToString();
            if (!SetMarlinZspeed(Setting.Marlin_Zspeed)) return false;

            MarlinZacceleration_maskedTextBox.Text = Setting.Marlin_Zacc.ToString();
            if (!SetMarlinZacc(Setting.Marlin_Zacc)) return false;

            MarlinZmicrosteps_maskedTextBox.Text = Setting.Marlin_ZMicroStep.ToString();
            if (Setting.Marlin_ZDegPerStep < 1.0)
            {
                MarlinZdeg09_radioButton.Checked = true;
                MarlinZdeg18_radioButton.Checked = false;
            }
            else
            {
                MarlinZdeg09_radioButton.Checked = false;
                MarlinZdeg18_radioButton.Checked = true;

            }
            MarlinZinterpolate_checkBox.Checked = Setting.Marlin_ZInterpolate;
            MarlinZtravelPerRev_textBox.Text = Setting.Marlin_ZTravelPerRev.ToString();
            if (!SetMarlinZstepping())
            {
                SettingMarlinZmotorParameters = false;
                return false;
            }
            MarlinZCurrent_maskedTextBox.Text = Setting.Marlin_ZCurrent.ToString();
            SetMarlinZcurr(Setting.Marlin_ZCurrent);
            MarlinZhomingSpeed_maskedTextBox.Text = Setting.Marlin_ZHomingSpeed.ToString();
            MarlinZHomingBackoff_maskedTextBox.Text = Setting.Marlin_ZHomingBackoff.ToString();
            SettingMarlinZmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinZspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinZspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinZspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Zspeed = speed;
                    SetMarlinZspeed(speed);
                    MarlinZspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinZspeed(double speed)
        {
            return Cnc.Marlin.Write_m("M203 Z" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void MarlinZacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            MarlinZacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinZacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Zacc = acc;
                    SetMarlinZacc(acc);
                }
                MarlinZacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinZacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 Z" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinZstepping()
        {
            string i;
            if (Setting.Marlin_ZInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Marlin.Write_m("M350 Z" + Setting.Marlin_ZMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Marlin_ZMicroStep * 360.0 / Setting.Marlin_ZDegPerStep;
            steps = steps / Setting.Marlin_ZTravelPerRev;
            return Cnc.Marlin.Write_m("M92 Z" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void MarlinZmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            MarlinZmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinZmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Marlin_ZMicroStep = usteps;
                        SetMarlinZstepping();
                        MarlinZmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinZinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_ZInterpolate = MarlinZinterpolate_checkBox.Checked;
            if (!SettingMarlinZmotorParameters)
            {
                SetMarlinZstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void MarlinZdeg09_radioButton_Click(object sender, EventArgs e)
        {
            MarlinZDegChange();
        }

        private void MarlinZdeg18_radioButton_Click(object sender, EventArgs e)
        {
            MarlinZDegChange();
        }

        private void MarlinZDegChange()
        {
            if (MarlinZdeg09_radioButton.Checked)
            {
                Setting.Marlin_ZDegPerStep = 0.9;
            }
            else
            {
                Setting.Marlin_ZDegPerStep = 0.9;
            }
            SetMarlinZstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinZtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            MarlinZtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinZtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Marlin_ZTravelPerRev = travel;
                    SetMarlinZstepping();
                    MarlinZtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void MarlinZCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            MarlinZCurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinZCurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Marlin_ZCurrent = curr;
                    SetMarlinZcurr(curr);
                    MarlinZCurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinZcurr(int curr)
        {
            return Cnc.Marlin.Write_m("M906 Z" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void MarlinZhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinZhomingSpeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinZhomingSpeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_ZHomingSpeed = speed;
                    MarlinZhomingSpeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinZHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double backoff;
            MarlinZHomingBackoff_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinZHomingBackoff_maskedTextBox.Text.Replace(',', '.'), out backoff))
                {
                    Setting.Marlin_ZHomingBackoff = backoff;
                    MarlinZHomingBackoff_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        #endregion MarlinZmotor

        // =================================================================================
        // A motor
        // =================================================================================

        #region MarlinAmotor

        private bool SettingMarlinAmotorParameters = false;
        public bool SetMarlinAmotorParameters()
        {
            SettingMarlinAmotorParameters = true;    // to not trigger checkbox related events
            MarlinAspeed_maskedTextBox.Text = Setting.Marlin_Aspeed.ToString();
            if (!SetMarlinAspeed(Setting.Marlin_Aspeed)) return false;

            MarlinAacceleration_maskedTextBox.Text = Setting.Marlin_Aacc.ToString();
            if (!SetMarlinAacc(Setting.Marlin_Aacc)) return false;

            MarlinAmicrosteps_maskedTextBox.Text = Setting.Marlin_AMicroStep.ToString();
            if (Setting.Marlin_ADegPerStep < 1.0)
            {
                MarlinAdeg09_radioButton.Checked = true;
                MarlinAdeg18_radioButton.Checked = false;
            }
            else
            {
                MarlinAdeg09_radioButton.Checked = false;
                MarlinAdeg18_radioButton.Checked = true;

            }
            MarlinAinterpolate_checkBox.Checked = Setting.Marlin_AInterpolate;
            MarlinAtravelPerRev_textBox.Text = Setting.Marlin_ATravelPerRev.ToString();
            if (!SetMarlinAstepping())
            {
                SettingMarlinAmotorParameters = false;
                return false;
            }
            MarlinACurrent_maskedTextBox.Text = Setting.Marlin_ACurrent.ToString();
            SetMarlinAcurr(Setting.Marlin_ACurrent);
            SettingMarlinAmotorParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinAspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double speed;
            MarlinAspeed_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinAspeed_maskedTextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Aspeed = speed;
                    SetMarlinAspeed(speed);
                    MarlinAspeed_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinAspeed(double speed)
        {
            return Cnc.Marlin.Write_m("M203 A" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void MarlinAacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double acc;
            MarlinAacceleration_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinAacceleration_maskedTextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Aacc = acc;
                    SetMarlinAacc(acc);
                }
                MarlinAacceleration_maskedTextBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinAacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 A" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinAstepping()
        {
            string i;
            if (Setting.Marlin_AInterpolate)
            {
                i = " i1";
            }
            else
            {
                i = " i0";
            }
            if (!Cnc.Marlin.Write_m("M350 A" + Setting.Marlin_AMicroStep.ToString().Replace(',', '.') + i)) return false;
            // steps per rev= usteps * 360/(step_angle)
            // steps per mm = steps per rev / travel per rev
            double steps = Setting.Marlin_AMicroStep * 360.0 / Setting.Marlin_ADegPerStep;
            steps = steps / Setting.Marlin_ATravelPerRev;
            return Cnc.Marlin.Write_m("M92 A" + steps.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // microsteps
        private void MarlinAmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            int usteps;
            MarlinAmicrosteps_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinAmicrosteps_maskedTextBox.Text.Replace(',', '.'), out usteps))
                {
                    if ((usteps > 1) && (usteps <= 256) &&
                        ((usteps & (usteps - 1)) == 0))
                    {
                        Setting.Marlin_AMicroStep = usteps;
                        SetMarlinAstepping();
                        MarlinAmicrosteps_maskedTextBox.ForeColor = Color.Black;
                    }
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinAinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_AInterpolate = MarlinAinterpolate_checkBox.Checked;
            if (!SettingMarlinAmotorParameters)
            {
                SetMarlinAstepping();
            }
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void MarlinAdeg09_radioButton_Click(object sender, EventArgs e)
        {
            MarlinADegChange();
        }

        private void MarlinAdeg18_radioButton_Click(object sender, EventArgs e)
        {
            MarlinADegChange();
        }

        private void MarlinADegChange()
        {
            if (MarlinAdeg09_radioButton.Checked)
            {
                Setting.Marlin_ADegPerStep = 0.9;
            }
            else
            {
                Setting.Marlin_ADegPerStep = 0.9;
            }
            SetMarlinAstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinAtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double travel;
            MarlinAtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(MarlinAtravelPerRev_textBox.Text.Replace(',', '.'), out travel))
                {
                    Setting.Marlin_ATravelPerRev = travel;
                    SetMarlinAstepping();
                    MarlinAtravelPerRev_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // motor current
        private void MarlinACurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int curr;
            MarlinACurrent_maskedTextBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(MarlinACurrent_maskedTextBox.Text, out curr))
                {
                    Setting.Marlin_ACurrent = curr;
                    SetMarlinAcurr(curr);
                    MarlinACurrent_maskedTextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinAcurr(int curr)
        {
            return Cnc.Marlin.Write_m("M906 A" + curr.ToString());
        }


        #endregion MarlinAmotor


    }
}

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
        // doing our own mask, as masked text box doesn't get backspace or delete
        bool IsNumberOrEditKey(int e)
        {
            if ((e >= 0x60) && (e <= 0x069)) return true;   // number
            if ((e >= 0x30) && (e <= 0x039)) return true;   // number
            if (e == 0x08) return true;  // backspace
            if (e == 0x2e) return true;  // delete
            if (e == 0xbe) return true;  // .
            if (e == 0x6e) return true;  // numpad .
            if (e == 0xbc) return true;  // ,
            if (e == 0x0d) return true;  // enter
            return false;
        }

        bool IsPowerOfTwo(string s, out int i)
        {
            // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            i = 0;
            if (int.TryParse(s.Replace(',', '.'), out i))   // is integer
            {
                if ((i > 1) && (i <= 256) && ((i & (i - 1)) == 0))   // is pow of 2    
                {
                    return true;
                }
            }
            return false;
        }
        // =================================================================================
        // X 
        // =================================================================================
        #region MarlinX

        private bool SettingMarlinXAxisParameters = false;
        public bool SetMarlinXAxisParameters()  // when connection is established
        {
            SettingMarlinXAxisParameters = true;    // to not trigger checkbox related events
            MarlinXspeed_TextBox.Text = Setting.Marlin_Xspeed.ToString();
            if (!SetMarlinXspeed(Setting.Marlin_Xspeed)) return false;

            MarlinXacceleration_TextBox.Text = Setting.Marlin_Xacc.ToString();
            if (!SetMarlinXacc(Setting.Marlin_Xacc)) return false;

            MarlinXhomingSpeed_TextBox.Text = Setting.Marlin_XHomingSpeed.ToString();
            MarlinXHomingAcceleration_TextBox.Text = Setting.Marlin_XHomingAcc.ToString();
            MarlinXHomingBackoff_textBox.Text = Setting.Marlin_XHomingBackoff.ToString();

            //MarlinXmicrosteps_textBox.Text = Setting.Marlin_XMicroStep.ToString();
            MarlinXmicrosteps_textBox.Text = "16";
            //MarlinXinterpolate_checkBox.Checked = Setting.Marlin_XInterpolate;
            MarlinXinterpolate_checkBox.Checked = true;

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
            MarlinXtravelPerRev_textBox.Text = Setting.Marlin_XTravelPerRev.ToString();
            if (!SetMarlinXstepping())
            {
                SettingMarlinXAxisParameters = false;
                return false;
            }
            MarlinXCurrent_textBox.Text = Setting.Marlin_XCurrent.ToString();
            SetMarlinXcurr(Setting.Marlin_XCurrent);
            SettingMarlinXAxisParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinXspeed_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double speed;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXspeed_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXspeed_TextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Xspeed = speed;
                    SetMarlinXspeed(speed);
                    MarlinXspeed_TextBox.ForeColor = Color.Black;
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
        private void MarlinXacceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double acc;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXacceleration_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXacceleration_TextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Xacc = acc;
                    SetMarlinXacc(acc);
                    MarlinXacceleration_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinXacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // homing

        private void MarlinXhomingSpeed_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double speed;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXhomingSpeed_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXhomingSpeed_TextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_XHomingSpeed = speed;  // Nothing done here, the value is used at homing
                    MarlinXhomingSpeed_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinXHomingAcceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double acc;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXHomingAcceleration_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXHomingAcceleration_TextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_XHomingAcc = acc;
                    MarlinXHomingAcceleration_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinXHomingBackoff_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double Backoff;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXHomingBackoff_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXHomingBackoff_textBox.Text.Replace(',', '.'), out Backoff))
                {
                    Setting.Marlin_XHomingBackoff = Backoff;
                    MarlinXHomingBackoff_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // microsteps
        private void MarlinXmicrosteps_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            int usteps;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXHomingBackoff_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (!IsPowerOfTwo(MarlinXmicrosteps_textBox.Text, out usteps))
                {
                    return;
                }
                Setting.Marlin_XMicroStep = usteps;
                SetMarlinXstepping();
                MarlinXmicrosteps_textBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinXinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_XInterpolate = MarlinXinterpolate_checkBox.Checked;
            if (!SettingMarlinXAxisParameters)
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
                Setting.Marlin_XDegPerStep = 1.8;
            }
            SetMarlinXstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinXtravelPerRev_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double travel;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
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
        private void MarlinXCurrent_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double curr;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinXCurrent_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinXCurrent_textBox.Text.Replace(',', '.'), out curr))
                {
                    Setting.Marlin_XCurrent = curr;
                    SetMarlinXcurr(curr);
                    MarlinXCurrent_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound

            }
        }
        private bool SetMarlinXcurr(double curr)
        {
            return Cnc.Marlin.Write_m("M906 X" + ((int)curr).ToString());
        }

        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinXstepping()
        {
            double steps = 360.0 / Setting.Marlin_XDegPerStep;  // steps per revolution
            steps = Setting.Marlin_XTravelPerRev / steps;       // whole steps per mm
            steps = steps * Setting.Marlin_XMicroStep;
            return Cnc.Marlin.Write_m("M92 X" + steps.ToString());
        }
        #endregion MarlinX


        // =================================================================================
        // Y 
        // =================================================================================
        #region MarlinY
        private bool SettingMarlinYAxisParameters = false;
        public bool SetMarlinYAxisParameters()  // when connection is established
        {
            SettingMarlinYAxisParameters = true;    // to not trigger checkbox related events
            MarlinYspeed_TextBox.Text = Setting.Marlin_Yspeed.ToString();
            if (!SetMarlinYspeed(Setting.Marlin_Yspeed)) return false;

            MarlinYacceleration_TextBox.Text = Setting.Marlin_Yacc.ToString();
            if (!SetMarlinYacc(Setting.Marlin_Yacc)) return false;

            MarlinYhomingSpeed_TextBox.Text = Setting.Marlin_YHomingSpeed.ToString();
            MarlinYHomingAcceleration_TextBox.Text = Setting.Marlin_YHomingAcc.ToString();
            MarlinYHomingBackoff_textBox.Text = Setting.Marlin_YHomingBackoff.ToString();

            //MarlinYmicrosteps_textBox.Text = Setting.Marlin_YMicroStep.ToString();
            MarlinYmicrosteps_textBox.Text = "16";
            //MarlinYinterpolate_checkBox.Checked = Setting.Marlin_YInterpolate;
            MarlinYinterpolate_checkBox.Checked = true;

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
            MarlinYtravelPerRev_textBox.Text = Setting.Marlin_YTravelPerRev.ToString();
            if (!SetMarlinYstepping())
            {
                SettingMarlinYAxisParameters = false;
                return false;
            }
            MarlinYCurrent_textBox.Text = Setting.Marlin_YCurrent.ToString();
            SetMarlinYcurr(Setting.Marlin_YCurrent);
            SettingMarlinYAxisParameters = false;
            return true;
        }


        // =================================================================================
        // speed
        private void MarlinYspeed_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double speed;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYspeed_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYspeed_TextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_Yspeed = speed;
                    SetMarlinYspeed(speed);
                    MarlinYspeed_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinYspeed(double speed)
        {
            return Cnc.Marlin.Write_m("M203 X" + speed.ToString().Replace(',', '.'));
        }

        // =================================================================================
        // acceleration
        private void MarlinYacceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double acc;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYacceleration_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYacceleration_TextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_Yacc = acc;
                    SetMarlinYacc(acc);
                    MarlinYacceleration_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private bool SetMarlinYacc(double acc)
        {
            return Cnc.Marlin.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
        }


        // =================================================================================
        // homing

        private void MarlinYhomingSpeed_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double speed;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYhomingSpeed_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYhomingSpeed_TextBox.Text.Replace(',', '.'), out speed))
                {
                    Setting.Marlin_YHomingSpeed = speed;  // Nothing done here, the value is used at homing
                    MarlinYhomingSpeed_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinYHomingAcceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            double acc;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYHomingAcceleration_TextBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYHomingAcceleration_TextBox.Text.Replace(',', '.'), out acc))
                {
                    Setting.Marlin_YHomingAcc = acc;
                    MarlinYHomingAcceleration_TextBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        private void MarlinYHomingBackoff_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double Backoff;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYHomingBackoff_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYHomingBackoff_textBox.Text.Replace(',', '.'), out Backoff))
                {
                    Setting.Marlin_YHomingBackoff = Backoff;
                    MarlinYHomingBackoff_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // microsteps
        private void MarlinYmicrosteps_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            int usteps;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYHomingBackoff_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (!IsPowerOfTwo(MarlinYmicrosteps_textBox.Text, out usteps))
                {
                    return;
                }
                Setting.Marlin_YMicroStep = usteps;
                SetMarlinYstepping();
                MarlinYmicrosteps_textBox.ForeColor = Color.Black;
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // interpolate
        private void MarlinYinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Setting.Marlin_YInterpolate = MarlinYinterpolate_checkBox.Checked;
            if (!SettingMarlinYAxisParameters)
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
                Setting.Marlin_YDegPerStep = 1.8;
            }
            SetMarlinYstepping();
        }

        // =================================================================================
        // travel per revolution
        private void MarlinYtravelPerRev_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double travel;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYtravelPerRev_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
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
        private void MarlinYCurrent_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            double curr;
            if (!IsNumberOrEditKey(e.KeyValue))
            {
                e.SuppressKeyPress = true;
                return;
            }

            MarlinYCurrent_textBox.ForeColor = Color.Red;
            if (e.KeyValue == 0x0d) // enter  
            {
                if (double.TryParse(MarlinYCurrent_textBox.Text.Replace(',', '.'), out curr))
                {
                    Setting.Marlin_YCurrent = curr;
                    SetMarlinYcurr(curr);
                    MarlinYCurrent_textBox.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound

            }
        }
        private bool SetMarlinYcurr(double curr)
        {
            return Cnc.Marlin.Write_m("M906 X" + ((int)curr).ToString());
        }

        // =================================================================================
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        private bool SetMarlinYstepping()
        {
            double steps = 360.0 / Setting.Marlin_YDegPerStep;  // steps per revolution
            steps = Setting.Marlin_YTravelPerRev / steps;       // whole steps per mm
            steps = steps * Setting.Marlin_YMicroStep;
            return Cnc.Marlin.Write_m("M92 X" + steps.ToString());
        }
        #endregion MarlinY


        // =================================================================================
        // Z 
        // =================================================================================

        #region MarlinZ
                private bool SettingMarlinZAxisParameters = false;
                public bool SetMarlinZAxisParameters()  // when connection is established
                {
                    SettingMarlinZAxisParameters = true;    // to not trigger checkbox related events
                    MarlinZspeed_TextBox.Text = Setting.Marlin_Zspeed.ToString();
                    if (!SetMarlinZspeed(Setting.Marlin_Zspeed)) return false;

                    MarlinZacceleration_TextBox.Text = Setting.Marlin_Zacc.ToString();
                    if (!SetMarlinZacc(Setting.Marlin_Zacc)) return false;

                    MarlinZhomingSpeed_TextBox.Text = Setting.Marlin_ZHomingSpeed.ToString();
                    MarlinZHomingAcceleration_TextBox.Text = Setting.Marlin_ZHomingAcc.ToString();
                    MarlinZHomingBackoff_textBox.Text = Setting.Marlin_ZHomingBackoff.ToString();

                    //MarlinZmicrosteps_textBox.Text = Setting.Marlin_ZMicroStep.ToString();
                    MarlinZmicrosteps_textBox.Text = "16";
                    //MarlinZinterpolate_checkBox.Checked = Setting.Marlin_ZInterpolate;
                    MarlinZinterpolate_checkBox.Checked = true;

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
                    MarlinZtravelPerRev_textBox.Text = Setting.Marlin_ZTravelPerRev.ToString();
                    if (!SetMarlinZstepping())
                    {
                        SettingMarlinZAxisParameters = false;
                        return false;
                    }
                    MarlinZCurrent_textBox.Text = Setting.Marlin_ZCurrent.ToString();
                    SetMarlinZcurr(Setting.Marlin_ZCurrent);
                    SettingMarlinZAxisParameters = false;
                    return true;
                }


                // =================================================================================
                // speed
                private void MarlinZspeed_TextBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double speed;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZspeed_TextBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZspeed_TextBox.Text.Replace(',', '.'), out speed))
                        {
                            Setting.Marlin_Zspeed = speed;
                            SetMarlinZspeed(speed);
                            MarlinZspeed_TextBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound
                    }
                }

                private bool SetMarlinZspeed(double speed)
                {
                    return Cnc.Marlin.Write_m("M203 X" + speed.ToString().Replace(',', '.'));
                }

                // =================================================================================
                // acceleration
                private void MarlinZacceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double acc;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZacceleration_TextBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZacceleration_TextBox.Text.Replace(',', '.'), out acc))
                        {
                            Setting.Marlin_Zacc = acc;
                            SetMarlinZacc(acc);
                            MarlinZacceleration_TextBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound
                    }
                }

                private bool SetMarlinZacc(double acc)
                {
                    return Cnc.Marlin.Write_m("M201 X" + acc.ToString().Replace(',', '.'));
                }


                // =================================================================================
                // homing

                private void MarlinZhomingSpeed_TextBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double speed;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZhomingSpeed_TextBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZhomingSpeed_TextBox.Text.Replace(',', '.'), out speed))
                        {
                            Setting.Marlin_ZHomingSpeed = speed;  // Nothing done here, the value is used at homing
                            MarlinZhomingSpeed_TextBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound
                    }
                }

                private void MarlinZHomingAcceleration_TextBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double acc;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZHomingAcceleration_TextBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZHomingAcceleration_TextBox.Text.Replace(',', '.'), out acc))
                        {
                            Setting.Marlin_ZHomingAcc = acc;
                            MarlinZHomingAcceleration_TextBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound
                    }
                }

                private void MarlinZHomingBackoff_textBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double Backoff;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZHomingBackoff_textBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZHomingBackoff_textBox.Text.Replace(',', '.'), out Backoff))
                        {
                            Setting.Marlin_ZHomingBackoff = Backoff;
                            MarlinZHomingBackoff_textBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound
                    }
                }

                // =================================================================================
                // microsteps
                private void MarlinZmicrosteps_textBox_KeyDown(object sender, KeyEventArgs e)
                {
                    int usteps;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZHomingBackoff_textBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (!IsPowerOfTwo(MarlinZmicrosteps_textBox.Text, out usteps))
                        {
                            return;
                        }
                        Setting.Marlin_ZMicroStep = usteps;
                        SetMarlinZstepping();
                        MarlinZmicrosteps_textBox.ForeColor = Color.Black;
                        e.Handled = true;   // supress the ding sound
                    }
                }

                // =================================================================================
                // interpolate
                private void MarlinZinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
                {
                    Setting.Marlin_ZInterpolate = MarlinZinterpolate_checkBox.Checked;
                    if (!SettingMarlinZAxisParameters)
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
                        Setting.Marlin_ZDegPerStep = 1.8;
                    }
                    SetMarlinZstepping();
                }

                // =================================================================================
                // travel per revolution
                private void MarlinZtravelPerRev_textBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double travel;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZtravelPerRev_textBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
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
                private void MarlinZCurrent_textBox_KeyDown(object sender, KeyEventArgs e)
                {
                    double curr;
                    if (!IsNumberOrEditKey(e.KeyValue))
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }

                    MarlinZCurrent_textBox.ForeColor = Color.Red;
                    if (e.KeyValue == 0x0d) // enter  
                    {
                        if (double.TryParse(MarlinZCurrent_textBox.Text.Replace(',', '.'), out curr))
                        {
                            Setting.Marlin_ZCurrent = curr;
                            SetMarlinZcurr(curr);
                            MarlinZCurrent_textBox.ForeColor = Color.Black;
                        }
                        e.Handled = true;   // supress the ding sound

                    }
                }
                private bool SetMarlinZcurr(double curr)
                {
                    return Cnc.Marlin.Write_m("M906 X" + ((int)curr).ToString());
                }

                // =================================================================================
                // Stepping, depends on microsteps, degrees per step and travel per revolution
                private bool SetMarlinZstepping()
                {
                    double steps = 360.0 / Setting.Marlin_ZDegPerStep;  // steps per revolution
                    steps = Setting.Marlin_ZTravelPerRev / steps;       // whole steps per mm
                    steps = steps * Setting.Marlin_ZMicroStep;
                    return Cnc.Marlin.Write_m("M92 X" + steps.ToString());
                }
        #endregion MarlinZ
      
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
            SetMarlinAcurr((int)Setting.Marlin_ACurrent);
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

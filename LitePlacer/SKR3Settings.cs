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
        // Startup  
        public void SKR3Settings_Load()
        {
            // Load settings
            // Get values for the masked text boxes
            if (!MaskedTextBox_GetValue(SKR3Xspeed_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Xacceleration_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Yspeed_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Yacceleration_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Zspeed_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Zacceleration_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Aspeed_maskedTextBox))
                return;
            if (!MaskedTextBox_GetValue(SKR3Aacceleration_maskedTextBox))
                return;
        }
// test if git sees this change...
        private bool MaskedTextBox_GetValue(MaskedTextBox Box)
        {
            string response = Cnc.SKR3.GetResponse_m("$" + Box.Tag);
            int ix = response.IndexOf('=');
            if (ix > 0)
            {
                response = response.Substring(ix + 1).Trim();
                Box.Text = decimal.Parse(response).ToString("G29");
                return true;
            }
            else
            {
                DisplayText("Cound not get value for parameter $" + Box.Tag + "response: " + response);
                Box.Text = "***";
                Box.ForeColor = Color.Red;
                return false;
            }
        }


        private void Double_maskedTextBox_KeyPress(MaskedTextBox Box, KeyPressEventArgs e)
        {
            double val;
            Box.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(Box.Text.Replace(',', '.'), out val))
                {
                    Cnc.SKR3.Write_m("$" + Box.Tag + "=" + val.ToString().Replace(',', '.'));
                    Box.ForeColor = Color.Black;
                }
                e.Handled = true;   // supress the ding sound
            }
        }

        // =================================================================================
        // X motor
        // =================================================================================
        #region SKR3_Xmotor

        // speed
        private void SKR3Xspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Xspeed_maskedTextBox, e);
        }

        // acceleration
        private void SKR3Xacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Xacceleration_maskedTextBox, e);
        }

        // Check for power of 2: // https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
        // microsteps
        private void SKR3Xmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // interpolate
        private void SKR3Xinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
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
            }
            else
            {
            }
        }

        // travel per revolution
        private void SKR3XtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // motor current
        private void SKR3XCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // homing

        private void SKR3XhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void SKR3XHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        #endregion

        // =================================================================================
        // Y motor
        // =================================================================================
        #region SKR3Ymotor

        // speed
        private void SKR3Yspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Yspeed_maskedTextBox, e);
        }

        // acceleration
        private void SKR3Yacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Yacceleration_maskedTextBox, e);
        }
        // Stepping, depends on microsteps, degrees per step and travel per revolution
        // microsteps
        private void SKR3Ymicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // interpolate
        private void SKR3Yinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
        }

        // =================================================================================
        // 0.9 or 1.8 deg. per step?
        private void SKR3Ydeg09_radioButton_Click(object sender, EventArgs e)
        {
        }

        private void SKR3Ydeg18_radioButton_Click(object sender, EventArgs e)
        {
        }

        private void SKR3YDegChange()
        {
            if (SKR3Ydeg09_radioButton.Checked)
            {
            }
            else
            {
            }
        }

        // =================================================================================
        // travel per revolution
        private void SKR3YtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // =================================================================================
        // motor current
        private void SKR3YCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private bool SetSKR3Ycurr(int curr)
        {
            return Cnc.SKR3.Write_m("M906 Y" + curr.ToString());
        }

        // =================================================================================
        // homing

        private void SKR3YhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void SKR3YHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        #endregion SKR3Ymotor

        // =================================================================================
        // Z motor
        // =================================================================================

        #region SKR3Zmotor

        // speed
        private void SKR3Zspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Zspeed_maskedTextBox, e);
        }

        // acceleration
        private void SKR3Zacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Zacceleration_maskedTextBox, e);
        }

        // microsteps
        private void SKR3Zmicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // interpolate
        private void SKR3Zinterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
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
        }

        // =================================================================================
        // travel per revolution
        private void SKR3ZtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // =================================================================================
        // motor current
        private void SKR3ZCurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
 
        // homing
        private void SKR3ZhomingSpeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void SKR3ZHomingBackoff_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        #endregion SKR3Zmotor

        // =================================================================================
        // A motor
        // =================================================================================

        #region SKR3Amotor

        // speed
        private void SKR3Aspeed_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Aspeed_maskedTextBox, e);
        }

        // acceleration
        private void SKR3Aacceleration_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Double_maskedTextBox_KeyPress(SKR3Aacceleration_maskedTextBox, e);
        }

        // microsteps
        private void SKR3Amicrosteps_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // interpolate
        private void SKR3Ainterpolate_checkBox_CheckedChanged(object sender, EventArgs e)
        {
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
            }
            else
            {
            }
        }

        // =================================================================================
        // travel per revolution
        private void SKR3AtravelPerRev_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        // =================================================================================
        // motor current
        private void SKR3ACurrent_maskedTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        #endregion SKR3Amotor

    }
}

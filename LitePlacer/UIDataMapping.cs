using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LitePlacer.Properties;
//using System.Web.Script.Serialization;


namespace LitePlacer {
    public partial class FormMain : Form {

        Settings setting= Settings.Default;

        private void VacuumTime_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int val;
            VacuumTime_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(VacuumTime_textBox.Text, out val))
                {
                    setting.General_PickupVacuumTime = val;
                    VacuumTime_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void VacuumTime_textBox_Leave(object sender, EventArgs e)
        {
            int val;
            if (int.TryParse(VacuumTime_textBox.Text, out val))
            {
                setting.General_PickupVacuumTime = val;
                VacuumTime_textBox.ForeColor = Color.Black;
            }
        }

        private void VacuumRelease_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int val;
            VacuumRelease_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (int.TryParse(VacuumRelease_textBox.Text, out val))
                {
                    setting.General_PickupReleaseTime = val;
                    VacuumRelease_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void VacuumRelease_textBox_Leave(object sender, EventArgs e)
        {
            int val;
            if (int.TryParse(VacuumRelease_textBox.Text, out val))
            {
                setting.General_PickupReleaseTime = val;
                VacuumRelease_textBox.ForeColor = Color.Black;
            }
        }

        private void NeedleOffsetX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(NeedleOffsetX_textBox.Text, out val))
                {
                    setting.DownCam_NeedleOffsetX = val;
                }
            }
        }

        private void NeedleOffsetX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(NeedleOffsetX_textBox.Text, out val))
            {
                setting.DownCam_NeedleOffsetX = val;
            }
        }

        private void NeedleOffsetY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(NeedleOffsetY_textBox.Text, out val))
                {
                    setting.DownCam_NeedleOffsetY = val;
                }
            }
        }

        private void NeedleOffsetY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(NeedleOffsetY_textBox.Text, out val))
            {
                setting.DownCam_NeedleOffsetY = val;
            }
        }

    private void SquareCorrection_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SquareCorrection_textBox.Text, out val))
            {
                setting.CNC_SquareCorrection = val;
                CNC.SquareCorrection = val;
            }
        }

        private void SquareCorrection_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SquareCorrection_textBox.Text, out val))
                {
                    setting.CNC_SquareCorrection = val;
                    CNC.SquareCorrection = val;
                }
            }
        }

        // =================================================================================
        private void DownCameraBoxX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                DownCameraUpdateXmmPerPixel();
            }
        }

        private void DownCameraBoxX_textBox_Leave(object sender, EventArgs e)
        {
            DownCameraUpdateXmmPerPixel();
        }


        private void DownCameraUpdateXmmPerPixel()
        {
            double val;
            if (double.TryParse(DownCameraBoxX_textBox.Text, out val))
            {
                setting.DownCam_XmmPerPixel = val / cameraView.downVideoProcessing.box.Width;
                DownCameraBoxXmmPerPixel_label.Text = "(" + setting.DownCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // ====
        private void UpCameraBoxX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                UpCameraUpdateXmmPerPixel();
            }
        }

        private void UpCameraBoxX_textBox_Leave(object sender, EventArgs e)
        {
            UpCameraUpdateXmmPerPixel();
        }

        private void UpCameraUpdateXmmPerPixel()        {
            double val;
            if (double.TryParse(UpCameraBoxX_textBox.Text, out val))            {
                setting.UpCam_XmmPerPixel = val / cameraView.upVideoProcessing.box.Width;
                UpCameraBoxXmmPerPixel_label.Text = "(" + setting.UpCam_XmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // =================================================================================
        private void DownCameraBoxY_textBox_KeyPress(object sender, KeyPressEventArgs e)        {
            if (e.KeyChar == '\r') DownCameraUpdateYmmPerPixel();
        }

        private void DownCameraBoxY_textBox_Leave(object sender, EventArgs e)        {
            DownCameraUpdateYmmPerPixel();
        }

        private void DownCameraUpdateYmmPerPixel()        {
            double val;
            if (double.TryParse(DownCameraBoxY_textBox.Text, out val)) {
                setting.DownCam_YmmPerPixel = val / cameraView.downVideoProcessing.box.Height;
                DownCameraBoxYmmPerPixel_label.Text = "(" + setting.DownCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }

        // ====
        private void UpCameraBoxY_textBox_KeyPress(object sender, KeyPressEventArgs e)        {
            if (e.KeyChar == '\r')  UpCameraUpdateYmmPerPixel();
        }

        private void UpCameraBoxY_textBox_Leave(object sender, EventArgs e){
                UpCameraUpdateYmmPerPixel();
        }

        private void UpCameraUpdateYmmPerPixel()        {
            double val;
            if (double.TryParse(UpCameraBoxY_textBox.Text, out val))
            {
                setting.UpCam_YmmPerPixel = val / cameraView.upVideoProcessing.box.Height;
                UpCameraBoxYmmPerPixel_label.Text = "(" + setting.UpCam_YmmPerPixel.ToString("0.0000", CultureInfo.InvariantCulture) + "mm/pixel)";
            }
        }



   


  // =================================================================================
        private void JigX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JigX_textBox.Text, out val))
                {
                    setting.General_JigOffsetX = val;
                }
            }
        }

        private void JigX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JigX_textBox.Text, out val))
            {
                setting.General_JigOffsetX = val;
            }
        }

        // =================================================================================
        private void JigY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JigY_textBox.Text, out val))
                {
                    setting.General_JigOffsetY = val;
                }
            }
        }

        private void JigY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JigY_textBox.Text, out val))
            {
                setting.General_JigOffsetY = val;
            }
        }

        private void PickupCenterX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            PickupCenterX_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(PickupCenterX_textBox.Text, out val))
                {
                    setting.General_PickupCenterX = val;
                    PickupCenterX_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void PickupCenterX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(PickupCenterX_textBox.Text, out val))
            {
                setting.General_PickupCenterX = val;
                PickupCenterX_textBox.ForeColor = Color.Black;
            }
        }
        private void PickupCenterY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            PickupCenterY_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(PickupCenterY_textBox.Text, out val))
                {
                    setting.General_PickupCenterY = val;
                    PickupCenterY_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void PickupCenterY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(PickupCenterY_textBox.Text, out val))
            {
                setting.General_PickupCenterY = val;
                PickupCenterY_textBox.ForeColor = Color.Black;
            }
        }

        // =================================================================================
        // UpCam specific functions
        // =================================================================================

        private void UpcamPositionX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double val;
                if (double.TryParse(UpcamPositionX_textBox.Text, out val))
                {
                    setting.UpCam_PositionX = val;
                }
            }
        }

        private void UpcamPositionX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpcamPositionX_textBox.Text, out val))
            {
                setting.UpCam_PositionX = val;
            }
        }

        private void UpcamPositionY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                double val;
                if (double.TryParse(UpcamPositionY_textBox.Text, out val))
                {
                    setting.UpCam_PositionY = val;
                }
            }
        }

        private void UpcamPositionY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(UpcamPositionY_textBox.Text, out val))
            {
                setting.UpCam_PositionY = val;
            }
        }
      #region Machine_Size

        private void SizeXMax_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SizeXMax_textBox.ForeColor = Color.Red;
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SizeXMax_textBox.Text, out val))
                {
                    setting.General_MachineSizeX = val;
                    SizeXMax_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void SizeXMax_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SizeXMax_textBox.Text, out val))
            {
                setting.General_MachineSizeX = val;
                SizeXMax_textBox.ForeColor = Color.Black;

            }
        }

        private void SizeYMax_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            SizeYMax_textBox.ForeColor = Color.Red;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(SizeYMax_textBox.Text, out val))
                {
                    setting.General_MachineSizeY = val;
                    SizeYMax_textBox.ForeColor = Color.Black;
                }
            }
        }

        private void SizeYMax_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(SizeYMax_textBox.Text, out val))
            {
                setting.General_MachineSizeY = val;
                SizeYMax_textBox.ForeColor = Color.Black;
            }
        }


        #endregion

        // =========================================================================
        #region park_location

        private void ParkLocationX_textBox_KeyPress(object sender, KeyPressEventArgs e)        {
            double val;
            if (e.KeyChar == '\r')            {
                if (double.TryParse(ParkLocationX_textBox.Text, out val))  {
                    setting.General_ParkX = val;
                }
            }
        }

        private void ParkLocationX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ParkLocationX_textBox.Text, out val))
            {
                setting.General_ParkX = val;
            }
        }

        private void ParkLocationY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(ParkLocationY_textBox.Text, out val))
                {
                    setting.General_ParkY = val;
                }
            }
        }

        private void ParkLocationY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(ParkLocationY_textBox.Text, out val))
            {
                setting.General_ParkY = val;
            }
        }

        private void Park_button_Click(object sender, EventArgs e)        {
            Cnc.CNC_Park();
        }

        #endregion
  // =================================================================================
        private void JobOffsetX_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JobOffsetX_textBox.Text, out val))
                {
                    JobOffsetX = val;
                    JobOffset.X = val;
                }
            }
        }

        private void JobOffsetX_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JobOffsetX_textBox.Text, out val))
            {
                JobOffsetX = val;
                JobOffset.Y = val;
            }
        }

        // =================================================================================
        private void JobOffsetY_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            double val;
            if (e.KeyChar == '\r')
            {
                if (double.TryParse(JobOffsetY_textBox.Text, out val))
                {
                    JobOffsetY = val;
                }
            }
        }

        private void JobOffsetY_textBox_Leave(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(JobOffsetY_textBox.Text, out val))
            {
                JobOffsetY = val;
            }
        }

        // Template Based Fiducal Finding Settings - RN 5/23
        #region Template Based Fiducal Settings
        private void fiducialTemlateMatch_textBox_Leave(object sender, EventArgs e) {
            fiducialTemlateMatch_textBox_KeyPress(sender, new KeyPressEventArgs('\r'));
        }

        
        private void fiducial_designator_regexp_textBox_TextChanged(object sender, EventArgs e) {
            setting.fiducial_designator_regexp = ((TextBox)sender).Text;
            setting.Save();
        }


        /// <summary>
        /// If the use template checkbox is toggled, save the state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_useTemplate_CheckedChanged(object sender, EventArgs e) {
            setting.use_template = ((CheckBox)sender).Checked;
            setting.Save();
        }

        private void button_setTemplate_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg (*.jpg)|*.jpg|png (*.png)|*.png|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK) {
                setting.template_file = ofd.FileName;
                setting.Save();
            }
        }

        private void fiducialTemlateMatch_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) {
                e.Handled = true;
            }

            // save
            double val;
            if (e.KeyChar == '\r' && double.TryParse(((TextBox)sender).Text, out val)) {
                setting.template_threshold = val;
                setting.Save();
            }

        }

        private void z_offset_textbox_keypress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1)) {
                e.Handled = true;
            }

            // save
            double val;
            if (e.KeyChar == '\r' && double.TryParse(((TextBox)sender).Text, out val)) {
                Cnc.z_offset = val; //setter has some intelligence to prevent bad values
                setting.z_offset = Cnc.z_offset;
                setting.Save();
            }

        }


        #endregion

    }
}

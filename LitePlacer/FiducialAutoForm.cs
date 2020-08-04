using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LitePlacer
{
    public partial class FiducialAutoForm : Form
    {
        public enum FiducialAutoResult
        {
            None, OK, Retry, Cancel, Manual
        }

        public bool dialogFinished = false;
        private FiducialAutoResult dialogResult = FiducialAutoResult.None;

        new public FiducialAutoResult DialogResult { get => dialogResult; set => dialogResult = value; }

        public FiducialAutoForm()
        {
            InitializeComponent();
        }

        public FiducialAutoForm(bool validMeasuredLocation)
        {
            InitializeComponent();
            if (!validMeasuredLocation)
            {
                buttonConfirm.Enabled = false;
                label1.Text = "Can't regognize fiducial. Retry or manually move the maschine\r\n" +
                              "to the fiducial and confirm with \"Manual\".";
            }
            else
            {
                label1.Text = "Fiducial location OK?\r\n\r\nAlternatively to specify a manual location move the masc" +
                            "hine\r\nto the desired position and confirm with \"Manual\".";
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = FiducialAutoResult.OK;
            this.Close();
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            this.DialogResult = FiducialAutoResult.Retry;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = FiducialAutoResult.Cancel;
            this.Close();
        }

        private void buttonManual_Click(object sender, EventArgs e)
        {
            this.DialogResult = FiducialAutoResult.Manual;
            this.Close();
        }

        private void FiducialAutoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dialogFinished = true;
        }
    }
}

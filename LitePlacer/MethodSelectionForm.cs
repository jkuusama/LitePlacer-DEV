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
	public partial class MethodSelectionForm : Form
	{
        public bool ShowCheckBox { get; set; } = false;
        public string HeaderString { get; set; } = "";

        static FormMain MainForm;

        public MethodSelectionForm(FormMain MainF)
		{
			InitializeComponent();
            MainForm = MainF;
        }

        private void MethodSelectionForm_Load(object sender, EventArgs e)
		{
			UpdateJobGrid_checkBox.Visible = ShowCheckBox;
			if (ShowCheckBox)
			{
				// The form is raised at run time, because method was ?. That can't be selected again.
				Question_button.Enabled = false;
				this.Text = "Place " + HeaderString;
			}
			UpdateJobGrid_checkBox.Checked = MainForm.Setting.Placement_UpdateJobGridAtRuntime;
		}

		private void UpdateJobGrid_checkBox_CheckedChanged(object sender, EventArgs e)
		{
            MainForm.Setting.Placement_UpdateJobGridAtRuntime = UpdateJobGrid_checkBox.Checked;
		}

		private void Question_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "?";
			this.Close();
		}

		private void ChangeNozzle_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Change Nozzle";
			this.Close();
		}

		private void Recalibrate_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Recalibrate";
			this.Close();
		}

		private void Ignore_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Ignore";
			this.Close();
		}

		private void Fiducials_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Fiducials";
			this.Close();
		}

		private void Pause_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Pause";
			this.Close();
		}

		private void Place_button_Click(object sender, EventArgs e)
		{
			 MainForm.SelectedMethod = "Place";
			this.Close();
		}

		private void ManualUpCam_button_Click(object sender, EventArgs e)
		{
             MainForm.SelectedMethod = "UpCam Snapshot";
            this.Close();
		}

        private void LoosePart_button_Click(object sender, EventArgs e)
        {
             MainForm.SelectedMethod = "LoosePart";
            this.Close();
        }

        private void PlaceFast_button_Click(object sender, EventArgs e)
        {
             MainForm.SelectedMethod = "Place Fast";
            this.Close();
        }

        private void ManualDownCam_button_Click(object sender, EventArgs e)
        {
             MainForm.SelectedMethod = "DownCam Snapshot";
            this.Close();
        }

        private void PlaceAssisted_button_Click(object sender, EventArgs e)
        {
             MainForm.SelectedMethod = "Place Assisted";
            this.Close();
        }

        private void LoosePartAssisted_button_Click(object sender, EventArgs e)
        {
             MainForm.SelectedMethod = "LoosePart Assisted";
            this.Close();
        }
	}
}

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
	public partial class TapeSelectionForm : Form
	{
        static FormMain MainForm;

        public DataGridView Grid;
		public string ID = "none";
        public string Nozzle;
        public string HeaderString = "";

		const int ButtonWidth = 75;
		const int ButtonHeight = 23;
		const int SideGap = 12;
		const int ButtonGap = 6;
		Size ButtonSize = new Size(ButtonWidth, ButtonHeight);

        private Size GridSizeSave= new Size();

		public TapeSelectionForm(DataGridView grd, FormMain MainF)
		{
            MainForm = MainF;
            InitializeComponent();
			Grid = grd;
            GridSizeSave = Grid.Size;
            Nozzle = MainForm.Setting.Nozzles_default.ToString();
            // this.Size = new Size(10 * ButtonWidth + 9*ButtonGap + 2 * SideGap+20, 133);  // 20?? 404; 480
            this.Controls.Add(Grid);
			for (int i = 0; i < Grid.RowCount; i++)
			{
				Grid.Rows[i].Cells["SelectButton_Column"].Value="Select";
			}
			Grid.Columns["SelectButton_Column"].Visible = true;
			Grid.Location = new Point(15, 59);
			Grid.Size = new Size(800, 480);
			// Add a CellClick handler to handle clicks in the button column.
			Grid.CellClick += new DataGridViewCellEventHandler(Grid_CellClick);
		}

		private void CloseForm()
		{
            for (int i = 0; i < Grid.RowCount; i++)
            {
                Grid.Rows[i].Cells["SelectButton_Column"].Value = "Reset";
            }
            Grid.Size = GridSizeSave;
			Grid.CellClick -= new DataGridViewCellEventHandler(Grid_CellClick);
			this.Close();
		}

		private void TapeSelectionForm_Load(object sender, EventArgs e)
		{
            this.Text = HeaderString;
            UpdateJobData_checkBox.Checked = MainForm.Setting.Placement_UpdateJobGridAtRuntime;
		}


		private void AbortJob_button_Click(object sender, EventArgs e)
		{
			ID = "Abort";
			CloseForm();
		}

		private void UpdateJobData_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			MainForm.Setting.Placement_UpdateJobGridAtRuntime = UpdateJobData_checkBox.Checked;
		}

		private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore clicks that are not on button cell  Id_Column
			if ((e.RowIndex < 0) || (e.ColumnIndex != Grid.Columns["SelectButton_Column"].Index))
			{
				return;
			}
			ID = Grid.Rows[e.RowIndex].Cells["Id_Column"].Value.ToString();
            if (Grid.Rows[e.RowIndex].Cells["Nozzle_Column"].Value == null)
            {
                MainForm.ShowMessageBox(
                    "Warning: This tape has no nozzle defined, using default value",
                    "No nozzle defined",
                    MessageBoxButtons.OK);
                Grid.Rows[e.RowIndex].Cells["Nozzle_Column"].Value = MainForm.Setting.Nozzles_default.ToString();
            }
            CloseForm();
        }

		private void Ignore_button_Click(object sender, EventArgs e)
		{
			ID = "Ignore";
			CloseForm();
        }
	}
}

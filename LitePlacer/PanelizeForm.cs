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
    public partial class PanelizeForm : Form
    {
        public static FormMain MainForm;
        public DataGridView CadData;
        public DataGridView JobData;

        public PanelizeForm(FormMain MainF)
        {
            MainForm = MainF;
            InitializeComponent();
        }

        public bool OK = false;

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            if (!Panelize())
            {
                return;
            }
            OK = true;
            this.Close();
        }

        // =================================================================================
        private double XFirstOffset = double.NaN;
        private double YFirstOffset = double.NaN;

        private int XRepeats = 0;
        private int YRepeats = 0;

        private double XIncrement = double.NaN;
        private double YIncrement = double.NaN;


        // =================================================================================
        // ValidateData: Check, that the Panelize process has all the data and the data is good
        // =================================================================================
        private bool ValidateData()
        {
            double val;
            int intval;

            if (double.TryParse(XFirstOffset_textBox.Text, out val))
            {
                XFirstOffset = val;
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in Offset to lower left board, X",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (double.TryParse(YFirstOffset_textBox.Text, out val))
            {
                YFirstOffset = val;
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in Offset to lower left board, Y",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (int.TryParse(XRepeats_textBox.Text, out intval))
            {
                XRepeats = intval;
                if (intval < 1)
                {
                    MainForm.ShowMessageBox(
                        "Invalid value in X repeats",
                        "Invalid value",
                        MessageBoxButtons.OK);
                    return false;
                }
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in X repeats",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (int.TryParse(YRepeats_textBox.Text, out intval))
            {
                YRepeats = intval;
                if (intval < 1)
                {
                    MainForm.ShowMessageBox(
                        "Invalid value in Y repeats",
                        "Invalid value",
                        MessageBoxButtons.OK);
                    return false;
                }
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in Y repeats",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (double.TryParse(XIncrement_textBox.Text, out val))
            {
                XIncrement = val;
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in X increment",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (double.TryParse(YIncrement_textBox.Text, out val))
            {
                YIncrement = val;
            }
            else
            {
                MainForm.ShowMessageBox(
                    "Invalid value in Y increment",
                    "Invalid value",
                    MessageBoxButtons.OK);
                return false;
            }

            if (UseBoardFids_checkBox.Checked)
            {
                // Check, that the fiducials are correctly indicated already
                bool found = false;
                foreach (DataGridViewRow Row in JobData.Rows)
                {
                    if (Row.Cells["GroupMethod"].Value.ToString() == "Fiducials")
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                MainForm.ShowMessageBox(
                    "Board fiducials not found or indicated",
                    "No fiducials",
                    MessageBoxButtons.OK);
                return false;
                }
            }
            else
            {
                // Check, that the data grid view has good data:
                // Less than three is error
                if (PanelFiducials_dataGridView.RowCount < 3)
                {
                MainForm.ShowMessageBox(
                    "Need data for at least three (four preferred) fiducials",
                    "Not enough fiducials",
                    MessageBoxButtons.OK);
                return false;
                }
                // Three is warning
                if (PanelFiducials_dataGridView.RowCount == 3)
                {
                    DialogResult dialogResult = MainForm.ShowMessageBox(
                        "Only three fiducials, results might be inaccurate. Continue?",
                        "Three fiducials",
                        MessageBoxButtons.OKCancel
                    );
                    if (dialogResult == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                // more than three is ok
                // X and Y columns need to have good data, too (Label_column just some data):
                bool OK = true;
                foreach (DataGridViewRow Row in PanelFiducials_dataGridView.Rows)
                {
                    if (Row.Cells["Label_column"].Value == null)
                    {
                        OK = false;
                        break;
                    }
                    if (Row.Cells["X_column"].Value == null)
                    {
                        OK = false;
                        break;
                    }
                    if (!double.TryParse(Row.Cells["X_column"].Value.ToString(), out val))
                    {
                        OK = false;
                        break;
                    }
                    if (Row.Cells["Y_column"].Value == null)
                    {
                        OK = false;
                        break;
                    }
                    if (!double.TryParse(Row.Cells["Y_column"].Value.ToString(), out val))
                    {
                        OK = false;
                        break;
                    }
                }
                if (!OK)
                {
                    MainForm.ShowMessageBox(
                        "Error data in fiducials table",
                        "No fiducials",
                        MessageBoxButtons.OK);
                    return false;
                }
            }
            return true;
        }

        // =================================================================================
        // Panelize: Builds multiple copies to CAD data table. 
        // =================================================================================
        private bool Panelize()
        {
            if (!ValidateData())
            {
                return false;
            };

            // Fill CAD data with panelized values:
            // Take a copy of current CAD data grid...
            DataGridView CadData_copy = new DataGridView();
            CadData_copy.AllowUserToAddRows = false;  // this prevents an empty row in the end
            for (int i = 0; i < CadData.Columns.Count; i++)
            {
                CadData_copy.Columns.Add(CadData.Columns[i].Clone() as DataGridViewColumn);
                CadData_copy.Columns[i].HeaderText = CadData.Columns[i].HeaderText;                
            }
            CadData_copy.Name = "CadData_copy";
            MainForm.DataGridViewCopy(CadData, ref CadData_copy, false);

            // ... and clear existing
            CadData.Rows.Clear();

            // For each component in the copy, multiply it:
            string Component = "";
            double val;
            bool OK = true;
            foreach (DataGridViewRow Row in CadData_copy.Rows)
            {
                // Panels are (row, column):
                // ...
                // (2,1), (2,2), (2,3) ...
                // (1,1), (1,2), (1,3) ...
                for (int PanelRow = 1; PanelRow <= YRepeats; PanelRow++)
                {
                    for (int PanelColumn = 1; PanelColumn <= XRepeats; PanelColumn++)
                    {
                        CadData.Rows.Add();
                        int Last = CadData.RowCount - 1;
                        // Component:
                        Component = Row.Cells["Component"].Value.ToString() + "_" + PanelRow.ToString() + PanelColumn.ToString();
                        CadData.Rows[Last].Cells["Component"].Value = Component;
                        // Value_Footprint:
                        CadData.Rows[Last].Cells["Value_Footprint"].Value = Row.Cells["Value_Footprint"].Value;
                        // X_nominal:
                        if (!double.TryParse(Row.Cells["X_nominal"].Value.ToString(), out val))
                        {
                            OK = false;
                            Component = Row.Cells["Component"].Value.ToString();
                            break;
                        }
                        val = XFirstOffset + (double)(PanelColumn - 1) * XIncrement + val;
                        CadData.Rows[Last].Cells["X_nominal"].Value = val;
                        // Y_nominal:
                        if (!double.TryParse(Row.Cells["Y_nominal"].Value.ToString(), out val))
                        {
                            OK = false;
                            Component = Row.Cells["Component"].Value.ToString();
                            break;
                        }
                        val = YFirstOffset + (double)(PanelRow - 1) * YIncrement + val;
                        CadData.Rows[Last].Cells["Y_nominal"].Value = val;
                        // Rotation:
                        CadData.Rows[Last].Cells["Rotation"].Value = Row.Cells["Rotation"].Value;
                        CadData.Rows[Last].Cells["X_Machine"].Value = "Nan";   // will be set later 
                        CadData.Rows[Last].Cells["Y_Machine"].Value = "Nan";
                        CadData.Rows[Last].Cells["Rotation_machine"].Value = "Nan";
                    }
                }
            }
            if (!OK)
            {
                MainForm.DataGridViewCopy(CadData_copy, ref CadData, false);
                MainForm.ShowMessageBox(
                    "Error in " + Component + " data.",
                    "No fiducials",
                    MessageBoxButtons.OK);
                return false;

            }
            // Build Job data
            MainForm.FillJobData_GridView();
            // Fix fiducials

            return true;
        }
    }
}

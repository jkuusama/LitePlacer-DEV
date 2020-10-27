using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Globalization;

namespace LitePlacer
{
    public partial class PanelizeForm : Form
    {
        public static FormMain MainForm { get; set; }
        public DataGridView CadData { get; set; }
        public DataGridView JobData { get; set; }

        // =================================================================================
        private double XFirstOffset = double.NaN;
        private double YFirstOffset = double.NaN;

        private int XRepeats = 0;
        private int YRepeats = 0;

        private double XIncrement = double.NaN;
        private double YIncrement = double.NaN;
        // =================================================================================

        public const string PANELFIDUCIALS_DATAFILE = "LitePlacer.PanelFids";


        public PanelizeForm(FormMain MainF)
        {
            MainForm = MainF;
            InitializeComponent();
        }

        private void PanelizeForm_Load(object sender, EventArgs e)
        {
            string path = MainForm.GetPath();
            MainForm.LoadDataGrid(path + PANELFIDUCIALS_DATAFILE, PanelFiducials_dataGridView, FormMain.DataTableType.PanelFiducials);

            XFirstOffset = MainForm.Setting.Panel_XFirstOffset;
            XFirstOffset_textBox.Text = XFirstOffset.ToString("0.00", CultureInfo.InvariantCulture);
            YFirstOffset = MainForm.Setting.Panel_YFirstOffset;
            YFirstOffset_textBox.Text = YFirstOffset.ToString("0.00", CultureInfo.InvariantCulture);
            XRepeats = MainForm.Setting.Panel_XRepeats;
            XRepeats_textBox.Text = XRepeats.ToString(CultureInfo.InvariantCulture);
            YRepeats = MainForm.Setting.Panel_YRepeats;
            YRepeats_textBox.Text = YRepeats.ToString(CultureInfo.InvariantCulture);
            XIncrement = MainForm.Setting.Panel_XIncrement;
            XIncrement_textBox.Text = XIncrement.ToString("0.00", CultureInfo.InvariantCulture);
            YIncrement = MainForm.Setting.Panel_YIncrement;
            YIncrement_textBox.Text = YIncrement.ToString("0.00", CultureInfo.InvariantCulture);
            UseBoardFids_checkBox.Checked = MainForm.Setting.Panel_UseBoardFids;
        }

        public bool OK { get; set; } = false;

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
            string path = MainForm.GetPath();
            MainForm.SaveDataGrid(path + PANELFIDUCIALS_DATAFILE, PanelFiducials_dataGridView);
            MainForm.Setting.Panel_XFirstOffset = XFirstOffset;
            MainForm.Setting.Panel_YFirstOffset = YFirstOffset;
            MainForm.Setting.Panel_XRepeats = XRepeats;
            MainForm.Setting.Panel_YRepeats = YRepeats;
            MainForm.Setting.Panel_XIncrement = XIncrement;
            MainForm.Setting.Panel_YIncrement = YIncrement;
            MainForm.Setting.Panel_UseBoardFids = UseBoardFids_checkBox.Checked;
            this.Close();
        }

        // =================================================================================
        // ValidateData: Check, that the Panelize process has all the data and the data is good
        // =================================================================================
        private bool ValidateData()
        {
            double val;
            int intval;

            if (double.TryParse(XFirstOffset_textBox.Text.Replace(',', '.'), out val))
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

            if (double.TryParse(YFirstOffset_textBox.Text.Replace(',', '.'), out val))
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

            if (int.TryParse(YRepeats_textBox.Text.Replace(',', '.'), out intval))
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

            if (double.TryParse(XIncrement_textBox.Text.Replace(',', '.'), out val))
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

            if (double.TryParse(YIncrement_textBox.Text.Replace(',', '.'), out val))
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
                    if (Row.Cells["JobdataMethodColumn"].Value.ToString() == "Fiducials")
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
                // X and Y columns need to have good data, too; others if empty, fill:
                bool OK = true;
                DataGridViewRow Row;
                int i = 0;
                for (i = 0; i < PanelFiducials_dataGridView.RowCount - 1; i++)
                {
                    Row = PanelFiducials_dataGridView.Rows[i];
                    if (Row.Cells["Designator_column"].Value == null)
                    {
                        Row.Cells["Designator_column"].Value = "Fid" + i.ToString(CultureInfo.InvariantCulture);
                    }
                    if (Row.Cells["Xpanelize_Column"].Value == null)
                    {
                        OK = false;
                        break;
                    }
                    if (!double.TryParse(Row.Cells["Xpanelize_Column"].Value.ToString().Replace(',', '.'), out val))
                    {
                        OK = false;
                        break;
                    }
                    if (Row.Cells["Ypanelize_Column"].Value == null)
                    {
                        OK = false;
                        break;
                    }
                    if (!double.TryParse(Row.Cells["Ypanelize_Column"].Value.ToString().Replace(',', '.'), out val))
                    {
                        OK = false;
                        break;
                    }
                }
                if (!OK)
                {
                    MainForm.ShowMessageBox(
                        "Bad data in fiducials table, line " + i.ToString(CultureInfo.InvariantCulture),
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

        // =================================================================================
        // Helper: FindExistingFiducials: 
        // Returns the row number from job data that has the fiducials, -1 if there are none
        private int FindExistingFiducials()
        {
            int FiducialIndex = -1;
            foreach (DataGridViewRow Row in JobData.Rows)
            {
                if (Row.Cells["JobdataMethodColumn"].Value.ToString() == "Fiducials")
                {
                    FiducialIndex = Row.Index;
                    break;
                }
            }
            return FiducialIndex;
        }

        // =================================================================================
        // Helper: distance, calculates distance of two points
        private double Distance(double X1, double Y1, double X2, double Y2)
        {
            return Math.Sqrt((X1 - X2) * (X1 - X2) + (Y1 - Y2) * (Y1 - Y2));
        }

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
                        Component = Row.Cells["CADdataComponentColumn"].Value.ToString() + "_"
                            + PanelRow.ToString(CultureInfo.InvariantCulture) + "_" + PanelColumn.ToString(CultureInfo.InvariantCulture);
                        CadData.Rows[Last].Cells["CADdataComponentColumn"].Value = Component;
                        // Value:
                        CadData.Rows[Last].Cells["CADdataValueColumn"].Value = Row.Cells["CADdataValueColumn"].Value;
                        // Footprint:
                        CadData.Rows[Last].Cells["CADdataFootprintColumn"].Value = Row.Cells["CADdataFootprintColumn"].Value;
                        // X_nominal:
                        if (!double.TryParse(Row.Cells["CADdataXnominalColumn"].Value.ToString().Replace(',', '.'), out val))
                        {
                            OK = false;
                            Component = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            break;
                        }
                        val = XFirstOffset + (double)(PanelColumn - 1) * XIncrement + val;
                        CadData.Rows[Last].Cells["CADdataXnominalColumn"].Value = val;
                        // Y_nominal:
                        if (!double.TryParse(Row.Cells["CADdataYnominalColumn"].Value.ToString().Replace(',', '.'), out val))
                        {
                            OK = false;
                            Component = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            break;
                        }
                        val = YFirstOffset + (double)(PanelRow - 1) * YIncrement + val;
                        CadData.Rows[Last].Cells["CADdataYnominalColumn"].Value = val;
                        // Rotation:
                        CadData.Rows[Last].Cells["CADdataRotationNominalColumn"].Value = Row.Cells["CADdataRotationNominalColumn"].Value;
                        CadData.Rows[Last].Cells["CADdataXmachineColumn"].Value = "Nan";   // will be set later 
                        CadData.Rows[Last].Cells["CADdataYmachineColumn"].Value = "Nan";
                        CadData.Rows[Last].Cells["CADdataRotationMachineColumn"].Value = "Nan";
                    }
                }
            }
            if (!OK)
            {
                DataGridView Grid = CadData;
                MainForm.DataGridViewCopy(CadData_copy, ref Grid, false);
                CadData = Grid;
                MainForm.ShowMessageBox(
                    "Error in " + Component + " data.",
                    "Fiducial data error",
                    MessageBoxButtons.OK);
                return false;

            }


            // Build Job data:
            int FiducialIndex = FindExistingFiducials();
            if (!UseBoardFids_checkBox.Checked)  // if using user defined fiducials:
            {
                // Remove existing:
                if (FiducialIndex >= 0)
                {
                    string[] OriginalFids = JobData.Rows[FiducialIndex].Cells["JobdataComponentsColumn"].Value.ToString().Split(',');
                    foreach (string CurrentFiducial in OriginalFids)
                    {
                        // loop from bottom, so we can modify the collection we are looping through:
                        for (int i = CadData.RowCount - 1; i >= 0; i--)
                        {
                            if (CadData.Rows[i].Cells["CADdataComponentColumn"].Value.ToString().Contains(CurrentFiducial))
                            {
                                CadData.Rows.RemoveAt(i);
                            }

                        }
                    }
                }
                // Add user defined fiducials to Cad data
                for (int i = 0; i < PanelFiducials_dataGridView.RowCount - 1; i++)
                {
                    CadData.Rows.Add();
                    int Last = CadData.RowCount - 1;
                    DataGridViewRow Row = PanelFiducials_dataGridView.Rows[i];
                    CadData.Rows[Last].Cells["CADdataComponentColumn"].Value = Row.Cells["Designator_column"].Value.ToString();
                    CadData.Rows[Last].Cells["CADdataValueColumn"].Value = "Fiducial";
                    CadData.Rows[Last].Cells["CADdataFootprintColumn"].Value = "On panel";
                    CadData.Rows[Last].Cells["CADdataXnominalColumn"].Value = Row.Cells["Xpanelize_Column"].Value.ToString();
                    CadData.Rows[Last].Cells["CADdataYnominalColumn"].Value = Row.Cells["Ypanelize_Column"].Value.ToString();
                    CadData.Rows[Last].Cells["CADdataRotationNominalColumn"].Value = "0.0";
                    CadData.Rows[Last].Cells["CADdataXmachineColumn"].Value = "Nan";   // will be set later 
                    CadData.Rows[Last].Cells["CADdataYmachineColumn"].Value = "Nan";
                    CadData.Rows[Last].Cells["CADdataRotationMachineColumn"].Value = "Nan";
                }
                // Build Job data
                MainForm.FillJobData_GridView();
                int FidRow;
                if (!MainForm.FindFiducials_m(out FidRow)) 
                {
                    return false;
                }

                return true;  // and we're done.
            }

            // Here, we are using the fiducials data from the individual subboards.
            // (We know they are indicated already)
            string[] OriginalFiducials = JobData.Rows[FiducialIndex].Cells["JobdataComponentsColumn"].Value.ToString().Split(',');
            // Build the job:
            MainForm.FillJobData_GridView();

            // Our job now has multiples of each board fiducials, we only want to use four that are furthest apart
            string LowLeft = "_";
            string LowRight = "_";
            string HighLeft = "_";
            string HighRight = "_";
            double LowLeftX = 0;
            double LowLeftY = 0;
            double LowRightX = 0;
            double LowRightY = 0;
            double HighLeftX = 0;
            double HighLeftY = 0;
            double HighRightX = 0;
            double HighRightY = 0;
            bool first = true;
            double X = 0.0;
            double Y = 0.0;
            double m10 = 10000;
            double CurrDist;
            double ThisDist;
            double tempX;
            double tempY;


            // For each fiducial in the list,
            foreach (string CurrentFiducial in OriginalFiducials)
            {
                // Find its nominal coordinates:
                // find the fiducial in CAD data.
                foreach (DataGridViewRow Row in CadData.Rows)
                {
                    if (Row.Cells["CADdataComponentColumn"].Value.ToString().Contains(CurrentFiducial))
                    {
                        // Get its nominal position (value already checked, but need to check again for compiler).
                        tempX = 0.0;
                        if (double.TryParse(Row.Cells["CADdataXnominalColumn"].Value.ToString().Replace(',', '.'), out tempX))
                        {
                            X = tempX;
                        }
                        tempY = 0.0;
                        if (double.TryParse(Row.Cells["CADdataYnominalColumn"].Value.ToString().Replace(',', '.'), out Y))
                        {
                            Y = tempY;
                        }
                        // We don't want to be fooled by negative coordinates. Therefore, we'll place the fid 10m up and right
                        X = X + m10;
                        Y = Y + m10;
                        if ((X < 0) || (Y < 0))
                        {
                            // if 10m is not enough, then ??
                            MainForm.ShowMessageBox(
                                "Problem with data. " + Row.Cells["CADdataComponentColumn"].Value.ToString()
                                + " X: " + X.ToString(CultureInfo.InvariantCulture)
                                + ", Y: " + Y.ToString(CultureInfo.InvariantCulture) + "?",
                                "Fiducials data error",
                                MessageBoxButtons.OK);
                            return false;
                        }

                        // First fid that we find is the lowest and rightest, highest and lowest, for now:
                        if (first)
                        {
                            tempX = 0.0;
                            if (double.TryParse(Row.Cells["CADdataXnominalColumn"].Value.ToString().Replace(',', '.'), out tempX))
                            {
                                X = tempX;
                            }
                            tempY = 0.0;
                            if (double.TryParse(Row.Cells["CADdataYnominalColumn"].Value.ToString().Replace(',', '.'), out Y))
                            {
                                Y = tempY;
                            }
                            LowLeft = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            LowLeftX = X;
                            LowLeftY = Y;
                            LowRight = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            LowRightX = X;
                            LowRightY = Y;
                            HighLeft = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            HighLeftX = X;
                            HighLeftY = Y;
                            HighRight = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            HighRightX = X;
                            HighRightY = Y;
                            first = false;
                            continue;
                        }

                        CurrDist = Distance(LowLeftX, LowLeftY, 0, 0);
                        ThisDist = Distance(X, Y, 0, 0);
                        if (ThisDist < CurrDist)
                        {
                            LowLeft = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            LowLeftX = X;
                            LowLeftY = Y;
                        }

                        CurrDist = Distance(HighLeftX, HighLeftY, 0, 2 * m10);
                        ThisDist = Distance(X, Y, 0, 2 * m10);
                        if (ThisDist < CurrDist)
                        {
                            HighLeft = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            HighLeftX = X;
                            HighLeftY = Y;
                        }

                        CurrDist = Distance(LowRightX, LowRightY, 2 * m10, 0);
                        ThisDist = Distance(X, Y, 2 * m10, 0);
                        if (ThisDist < CurrDist)
                        {
                            LowRight = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            LowRightX = X;
                            LowRightY = Y;
                        }

                        CurrDist = Distance(HighRightX, HighRightY, 2 * m10, 2 * m10);
                        ThisDist = Distance(X, Y, 2 * m10, 2 * m10);
                        if (ThisDist < CurrDist)
                        {
                            HighRight = Row.Cells["CADdataComponentColumn"].Value.ToString();
                            HighRightX = X;
                            HighRightY = Y;
                        }
                    }
                }
            }

            // Are all four updated? If not, placement is not going to work
            // Likely, one fiducial on board, one dimensional panel
            if (String.Equals(LowLeft, "_", StringComparison.Ordinal) ||
                String.Equals(HighLeft, "_", StringComparison.Ordinal) ||
                String.Equals(LowRight, "_", StringComparison.Ordinal) ||
                String.Equals(HighRight, "_", StringComparison.Ordinal))
            {
                MainForm.ShowMessageBox(
                    "Current fiducials don't spread out.",
                    "Fiducials data error",
                    MessageBoxButtons.OK);
                return false;
            }
            JobData.Rows[FiducialIndex].Cells["JobdataComponentsColumn"].Value = LowLeft + "," + HighLeft + "," + LowRight + "," + HighRight;
            JobData.Rows[FiducialIndex].Cells["JobdataCountColumn"].Value = "4";
            return true;
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LitePlacer
{
    public partial class TapeEditForm : Form
    {
        public DataGridView TapesDataGrid { get; set; }
        public DataGridViewRow Row { get; set; }
        public int TapeRowNo { get; set; }
        public FormMain MainForm { get; set; }
        public bool CreatingNew { get; set; }
        Camera Cam;
        CNC Cnc;

        // The parameters of tapes, most taken care by this dialog

        // string ID: Name of the tape (ex: 0805, 10k)
        // enum Direction: which way the components increase (values: PlusX, PlusY, MinusX, MinusY)
        // double Rotation: Tells how much a part is rotated on the tape (manufacturers load parts on tapes in any orientation they wish).
        //      a part on a tape with PlusY orientation, 0deg. rotation in CAD data, is rotated this much when placed
        //      values 0, 90, 180 and 270 degrees
        // int nozzle: uses this nozzle for these parts (if nozzles are used)
        // double pitch: distance from one part to next
        // double offsetX, OffsetY: distance from location mark (hole, other) to the part (measured in Plus Y orientation)
        //      used for finding the part when optical recognition of holes are used
        //      standard values through a drop down dialog, custom values by manual edit

        // enum Type: what values to use in optical hole recognition (values: Paper, Black, Clear (last two are plastic tapes));
        //      used to determine which optical filter set to use in hole recognition
        // bool MeasureHole: if part is located by optical recognition of tape hole; if not, coordinates are used directly
        // bool MeasurePart: if part position is optically measured
        // int Capacity: how many components can be on a fully loaded tape (0: user is prompted to place the component; <0: infinite (feeders))
        // int Next: the next component used in placement
        // int Tray: Some users are using part holder trays, allowing loading/unloading a bunch of tapes at one time. This allows the same for tapes data.
        // double X, double Y: coordinates of first hole/part
        // double NextX, NextY: coordinates for the hole/part next used (most cases, automatically maintained)
        // double PickupZ, PlacementZ: Z values used for pickup/place operation
        // bool PickupZvalid, PlacementZvalid: if the values are valid (if not, they are measured when used)

        public TapeEditForm(CNC _cnc, Camera _cam)
        {
            InitializeComponent();
            Cnc = _cnc;
            Cam = _cam;
        }

        // =================================================================================

        private bool DrawCross = true;
        private void TapeEditForm_Load(object sender, EventArgs e)
        {
            //Row = TapesDataGrid.Rows[TapeRowNo];
            if (Row.Cells["Id_Column"].Value != null)
            {
                ID_textBox.Text = Row.Cells["Id_Column"].Value.ToString();
            }
            if (Row.Cells["Orientation_Column"].Value != null)
            {
                TapeOrientation_comboBox.SelectedItem = Row.Cells["Orientation_Column"].Value;
            }
            if (Row.Cells["Rotation_Column"].Value != null)
            {
                TapeRotation_comboBox.SelectedItem = Row.Cells["Rotation_Column"].Value;
            }
            if (MainForm.Setting.Nozzles_Enabled)
            {
                Nozzle_numericUpDown.Maximum = MainForm.Setting.Nozzles_count;
                if (Row.Cells["Nozzle_Column"].Value != null)
                {
                    int nozzle;
                    if (int.TryParse(Row.Cells["Nozzle_Column"].Value.ToString(), out nozzle))
                    {
                        if (nozzle == 0)
                        {
                            nozzle = MainForm.Setting.Nozzles_default;   // default nozzle = 0 (!?)
                        }
                        Nozzle_numericUpDown.Value = nozzle;
                    }
                    else
                    {
                        Nozzle_numericUpDown.Value = MainForm.Setting.Nozzles_default;
                    }
                }
                else
                {
                    if (MainForm.Setting.Nozzles_default == 0)
                    {
                        Nozzle_numericUpDown.Value = 1;
                    }
                    else
                    {
                        Nozzle_numericUpDown.Value = MainForm.Setting.Nozzles_default;
                    }
                }
            }
            else
            {
                Nozzle_numericUpDown.Enabled = false;
                MainForm.DefaultNozzle_label.Text = "--";
            }

            if (Row.Cells["Width_Column"].Value != null)
            {
                TapeWidth_comboBox.Text = Row.Cells["Width_Column"].Value.ToString();
            }

            if (Row.Cells["Pitch_Column"].Value != null)
            {
                TapePitch_textBox.Text = Row.Cells["Pitch_Column"].Value.ToString();
                ValidateDouble(TapePitch_textBox);
            }

            if (Row.Cells["OffsetX_Column"].Value != null)
            {
                TapeOffsetX_textBox.Text = Row.Cells["OffsetX_Column"].Value.ToString();
                ValidateDouble(TapePitch_textBox);
            }

            if (Row.Cells["OffsetY_Column"].Value != null)
            {
                TapeOffsetY_textBox.Text = Row.Cells["OffsetY_Column"].Value.ToString();
                ValidateDouble(TapeOffsetY_textBox); 
            }
            /*
            if (Row.Cells["Capacity_Column"].Value != null)
            {
                Capacity_textBox.Text = Row.Cells["Capacity_Column"].Value.ToString();

            }
            */
            if (Row.Cells["NextPart_Column"].Value != null)
            {
                NextPart_textBox.Text = Row.Cells["NextPart_Column"].Value.ToString();

            }

            if (Row.Cells["FirstX_Column"].Value != null)
            {
                FirstX_textBox.Text = Row.Cells["FirstX_Column"].Value.ToString();
            }
            if (Row.Cells["FirstY_Column"].Value != null)
            {
                FirstY_textBox.Text = Row.Cells["FirstY_Column"].Value.ToString();
            }

            if (Row.Cells["LastX_Column"].Value != null)
            {
                LastX_textBox.Text = Row.Cells["LastX_Column"].Value.ToString();
            }
            if (Row.Cells["LastY_Column"].Value != null)
            {
                LastY_textBox.Text = Row.Cells["LastY_Column"].Value.ToString();
            }
            if (Row.Cells["RotationDirect_Column"].Value != null)
            {
                RotationDirect_textBox.Text = Row.Cells["RotationDirect_Column"].Value.ToString();
            }

            if (Row.Cells["Z_Pickup_Column"].Value != null)
            {
                PickupZ_textBox.Text = Row.Cells["Z_Pickup_Column"].Value.ToString();
            }
            if (Row.Cells["Z_Place_Column"].Value != null)
            {
                PlacementZ_textBox.Text = Row.Cells["Z_Place_Column"].Value.ToString();
            }
            if (Row.Cells["TrayID_Column"].Value != null)
            {
                TrayID_textBox.Text = Row.Cells["TrayID_Column"].Value.ToString();
            }
            MainForm.DownCameraRotationFollowsA = true;
            DrawCross = Cam.DrawCross;
            if (Row.Cells["CoordinatesForParts_Column"].Value != null)
            {
                if (Row.Cells["CoordinatesForParts_Column"].Value.ToString() == "True")
                {
                    CoordinatesForParts_checkBox.Checked = true;
                    DrawGrid_checkBox.Enabled = true;
                    DrawGrid_checkBox.Checked = false;
                    Cam.DrawGrid = false;
                    double val;
                    if (double.TryParse(RotationDirect_textBox.Text.Replace(',', '.'), out val))
                    {
                        // MainForm.CNC_A_m(val);
                    }
                    EnableLastItems();
                }
                else
                {
                    CoordinatesForParts_checkBox.Checked = false;
                    DrawGrid_checkBox.Enabled = false;
                    DrawGrid_checkBox.Checked = false;
                    EnableLastItems();
                }
            }
            else
            {
                CoordinatesForParts_checkBox.Checked = false;
                DrawGrid_checkBox.Enabled = false;
                DrawGrid_checkBox.Checked = false;
                EnableLastItems();
            }

            if (Row.Cells["UseNozzleCoordinates_Column"].Value != null)
            {
                if (Row.Cells["UseNozzleCoordinates_Column"].Value.ToString() == "True")
                {
                    UseNozzleCoordinates_checkBox.Checked = true;
                }
                else
                {
                    CoordinatesForParts_checkBox.Checked = false;
                }
            }
            else
            {
                CoordinatesForParts_checkBox.Checked = false;
            }

            // add videoalgorithms to the selection box
            Type_comboBox.Items.Clear();
            string CurrentAlg = Row.Cells["Type_Column"].Value.ToString();
            int CurrentItem = 0;
            for (int i = 1; i < MainForm.VideoAlgorithms.AllAlgorithms.Count; i++)
            {
                Type_comboBox.Items.Add(MainForm.VideoAlgorithms.AllAlgorithms[i].Name);
                if (MainForm.VideoAlgorithms.AllAlgorithms[i].Name.ToString() == CurrentAlg)
                {
                    CurrentItem = i;
                }
            }
            if (CurrentItem!=0)
            {
                Type_comboBox.SelectedIndex = CurrentItem-1;
            }
            else
            {
                Type_comboBox.SelectedIndex = 1;
            }
        }

        private void EnableLastItems()
        {
            LastX_textBox.Enabled = CoordinatesForParts_checkBox.Checked;
            LastY_textBox.Enabled = CoordinatesForParts_checkBox.Checked;
            GetLastPosition_button.Enabled = CoordinatesForParts_checkBox.Checked;
            LastX_label.Enabled = CoordinatesForParts_checkBox.Checked;
            LastY_label.Enabled = CoordinatesForParts_checkBox.Checked;
            // Cam.DrawGrid = CoordinatesForParts_checkBox.Checked;
        }

        // =================================================================================
        private void ValidateError(string item)
        {
            MainForm.ShowMessageBox(
                "Invalid data at " + item,
                "Data error",
                MessageBoxButtons.OK);

        }

        private void TapeEditOK_button_Click(object sender, EventArgs e)
        {
            // Check that everythign has valid data
            if (String.IsNullOrWhiteSpace(ID_textBox.Text))
            {
                ValidateError("ID");
                return;
            }
            if (!ValidateInt(NextPart_textBox))
            {
                ValidateError("Next part");
                return;
            }
            /*
            if (!ValidateInt(Capacity_textBox))
            {
                ValidateError("Capacity");
                return;
            }
            */
            if (!ValidateDouble(TapePitch_textBox))
            {
                ValidateError("Part pitch");
                return;
            }
            if (!ValidateDouble(TapeOffsetX_textBox))
            {
                ValidateError("Offset X");
                return;
            }
            if (!ValidateDouble(TapeOffsetY_textBox))
            {
                ValidateError("Offset Y");
                return;
            }
            if (!ValidateDouble(FirstX_textBox))
            {
                ValidateError("First X");
                return;
            }
            if (!ValidateDouble(FirstY_textBox))
            {
                ValidateError("First Y");
                return;
            }
            if (!ValidateDouble(RotationDirect_textBox))
            {
                ValidateError("Rotation");
                return;
            }
            if (PickupZ_textBox.Text != "--")
            {
                if (!ValidateDouble(PickupZ_textBox))
                {
                    ValidateError("Pickup Z");
                    return;
                }
            }
            if (PlacementZ_textBox.Text != "--")
            {
                if (!ValidateDouble(PlacementZ_textBox))
                {
                    ValidateError("Placement Z");
                    return;
                }
            }
            if (!ValidateDouble(LastX_textBox) && LastX_textBox.Enabled)
            {
                ValidateError("Last X");
                return;
            }
            if (!ValidateDouble(LastY_textBox) && LastY_textBox.Enabled)
            {
                ValidateError("Last Y");
                return;
            }
            // Ok, we can close
            Row.Cells["Id_Column"].Value = ID_textBox.Text;
            Row.Cells["Orientation_Column"].Value = TapeOrientation_comboBox.SelectedItem;
            Row.Cells["Rotation_Column"].Value = TapeRotation_comboBox.SelectedItem;
            Row.Cells["Nozzle_Column"].Value = Nozzle_numericUpDown.Value.ToString(CultureInfo.InvariantCulture);
            Row.Cells["NextPart_Column"].Value = NextPart_textBox.Text;
            // Row.Cells["Capacity_Column"].Value = Capacity_textBox.Text;
            Row.Cells["Width_Column"].Value = TapeWidth_comboBox.Text;
            Row.Cells["Type_Column"].Value = Type_comboBox.Text; 
            Row.Cells["TrayID_Column"].Value = TrayID_textBox.Text;
            Row.Cells["Pitch_Column"].Value = TapePitch_textBox.Text;
            Row.Cells["OffsetX_Column"].Value = TapeOffsetX_textBox.Text;
            Row.Cells["OffsetY_Column"].Value = TapeOffsetY_textBox.Text;
            Row.Cells["FirstX_Column"].Value = FirstX_textBox.Text;
            Row.Cells["FirstY_Column"].Value = FirstY_textBox.Text;
            Row.Cells["RotationDirect_Column"].Value = RotationDirect_textBox.Text;
            Row.Cells["Z_Pickup_Column"].Value = PickupZ_textBox.Text;
            Row.Cells["Z_Place_Column"].Value = PlacementZ_textBox.Text;
            Row.Cells["CoordinatesForParts_Column"].Value = CoordinatesForParts_checkBox.Checked;
            Row.Cells["UseNozzleCoordinates_Column"].Value = UseNozzleCoordinates_checkBox.Checked;
            Row.Cells["LastX_Column"].Value = LastX_textBox.Text;
            Row.Cells["LastY_Column"].Value = LastY_textBox.Text;
            MainForm.Update_GridView(TapesDataGrid);
            MainForm.DownCameraRotationFollowsA = false;
            Cam.DrawGrid = false;
            Cam.DrawCross = DrawCross;
            Close();
        }

        private void TapeEditCancel_button_Click(object sender, EventArgs e)
        {
            MainForm.DownCameraRotationFollowsA = false;
            Cam.DrawGrid = false;
            Cam.DrawCross = DrawCross;
            if (CreatingNew)
            {
                TapesDataGrid.Rows.RemoveAt(TapeRowNo);
                MainForm.Update_GridView(TapesDataGrid);
            }
            Close();
        }

        private void TapeWidth_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TapeWidth_comboBox.Text == "custom")
            {
                return;
            }
            double Xoff;
            double Yoff;
            double pitch;
            MainForm.TapeWidthStringToValues(TapeWidth_comboBox.SelectedItem.ToString(), out Xoff, out Yoff, out pitch);
            TapeOffsetX_textBox.Text = Xoff.ToString(CultureInfo.InvariantCulture);
            TapeOffsetX_textBox.ForeColor = Color.Black;    // in case there are erroneous edits left
            TapeOffsetY_textBox.Text = Yoff.ToString(CultureInfo.InvariantCulture);
            TapeOffsetY_textBox.ForeColor = Color.Black;
            TapePitch_textBox.Text = pitch.ToString(CultureInfo.InvariantCulture);
            TapePitch_textBox.ForeColor = Color.Black;
        }

        private bool ValidateDouble(TextBox box)
        {
            double val;
            if (double.TryParse(box.Text.Replace(',', '.'), out val))
            {
                box.ForeColor = Color.Black;
                return true;
            }
            else
            {
                box.ForeColor = Color.Red;
                return false;
            }
        }

        private bool ValidateInt(TextBox box)
        {
            int val;
            if (int.TryParse(box.Text, out val))
            {
                box.ForeColor = Color.Black;
                return true;
            }
            else
            {
                box.ForeColor = Color.Red;
                return false;
            }
        }

        private void TapePitch_textBox_TextChanged(object sender, EventArgs e)
        {
            if (!TapePitch_textBox.Focused)
            {
                return;
            }
            TapeWidth_comboBox.Text = "custom";
            ValidateDouble(TapePitch_textBox);
        }

        private void Capacity_textBox_TextChanged(object sender, EventArgs e)
        {
            // ValidateInt(Capacity_textBox);
        }

        private void NextPart_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateInt(NextPart_textBox);
        }

        private void FirstY_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateDouble(FirstY_textBox);
        }

        private void FirstX_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateDouble(FirstX_textBox);
        }


        private void PickupZ_textBox_TextChanged(object sender, EventArgs e)
        {
            if (PickupZ_textBox.Text== "--")
            {
                PickupZ_textBox.ForeColor = Color.Black;
            }
            else
            {
                ValidateDouble(PickupZ_textBox);
            }
        }

        private void PlacementZ_textBox_TextChanged(object sender, EventArgs e)
        {
            if (PlacementZ_textBox.Text == "--")
            {
                PlacementZ_textBox.ForeColor = Color.Black;
            }
            else
            {
                ValidateDouble(PlacementZ_textBox);
            }
        }

        private void ResetPickupZ_button_Click(object sender, EventArgs e)
        {
            PickupZ_textBox.Text = "--";
        }

        private void ResetPlacementZ_button_Click(object sender, EventArgs e)
        {
            PlacementZ_textBox.Text = "--";
        }

        private void ResetTrayID_button_Click(object sender, EventArgs e)
        {
            TrayID_textBox.Text = "--";
        }

        private void GetFirstPosition_button_Click(object sender, EventArgs e)
        {
            FirstX_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            FirstY_textBox.Text = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
            RotationDirect_textBox.Text = Cnc.CurrentA.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void GetLastPosition_button_Click(object sender, EventArgs e)
        {
            LastX_textBox.Text = Cnc.CurrentX.ToString("0.000", CultureInfo.InvariantCulture);
            LastY_textBox.Text = Cnc.CurrentY.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void GetPickupZ_button_Click(object sender, EventArgs e)
        {
            PickupZ_textBox.Text = Cnc.CurrentZ.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void GetPlacementZ_button_Click(object sender, EventArgs e)
        {
            PlacementZ_textBox.Text = Cnc.CurrentZ.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void CoordinatesForParts_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            EnableLastItems();
            if (CoordinatesForParts_checkBox.Checked)
            {
                DrawGrid_checkBox.Enabled = true;
                Cam.DrawGrid = DrawGrid_checkBox.Checked;
            }
            else
            {
                DrawGrid_checkBox.Enabled = false;
                Cam.DrawGrid = false;
                Cam.DrawCross = DrawCross;
            }
        }

        private void ACorrection_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateDouble(RotationDirect_textBox);
        }

        private void LastX_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateDouble(LastX_textBox);
        }

        private void LastY_textBox_TextChanged(object sender, EventArgs e)
        {
            ValidateDouble(LastY_textBox);
        }

        private void GetACorrection_button_Click(object sender, EventArgs e)
        {
            RotationDirect_textBox.Text = Cnc.CurrentA.ToString("0.000", CultureInfo.InvariantCulture);
        }

        private void DrawGrid_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Cam.DrawGrid = DrawGrid_checkBox.Checked;
            if (!Cam.DrawGrid)
            {
                Cam.DrawCross = DrawCross;
            }
        }
    }
}

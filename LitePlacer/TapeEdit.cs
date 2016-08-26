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
    public partial class TapeEditForm : Form
    {
        public DataGridView TapesDataGrid;
        public DataGridViewRow Row;
        public int TapeRowNo;

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

        // enum Type: what values to use in optical hole regognition (values: Paper, Black, Clear (last two are plastic tapes));
        //      used to determine which optical filter set to use in hole regognition
        // bool MeasureHole: if part is located by optical regognition of tape hole; if not, coordinates are used directly
        // bool MeasurePart: if part position is optically measured
        // int Capacity: how many components can be on a fully loaded tape (0: user is prompted to place the component; <0: infinite (feeders))
        // int Next: the next component used in placement
        // int Tray: Some users are using part holder trays, allowing loading/unloading a bunch of tapes at one time. This allows the same for tapes data.
        // double X, double Y: coordinates of first hole/part
        // double NextX, NextY: coordinates for the hole/part next used (most cases, automatically maintained)
        // double PickupZ, PlacementZ: Z values used for pickup/place operation
        // bool PickupZvalid, PlacementZvalid: if the values are valid (if not, they are measured when used)

        public TapeEditForm()
        {
            InitializeComponent();
        }

        private void TapeEditForm_Load(object sender, EventArgs e)
        {
            Row = TapesDataGrid.Rows[TapeRowNo];
            ID_textBox.Text = Row.Cells["IdColumn"].Value.ToString();
            TapeOrientation_comboBox.SelectedItem = Row.Cells["OrientationColumn"].Value;
            TapeRotation_comboBox.SelectedItem = Row.Cells["RotationColumn"].Value;
            Nozzle_numericUpDown.Maximum = Properties.Settings.Default.Nozzles_count;
            if (Row.Cells["NozzleColumn"].Value!=null)
            {
                int nzl;
                if (int.TryParse(Row.Cells["NozzleColumn"].Value.ToString(), out nzl))
                {
                    Nozzle_numericUpDown.Value = nzl;
                }
                else
                {
                    Nozzle_numericUpDown.Value = Properties.Settings.Default.Nozzles_default;
                }
            }
            else
            {
                Nozzle_numericUpDown.Value = Properties.Settings.Default.Nozzles_default;
            }
        }

        private void TapeEditOK_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ID_textBox_TextChanged(object sender, EventArgs e)
        {
            Row.Cells["IdColumn"].Value = ID_textBox.Text;
        }

        private void TapeOrientation_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Row.Cells["OrientationColumn"].Value = TapeOrientation_comboBox.SelectedItem;
        }

        private void TapeRotation_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Row.Cells["RotationColumn"].Value = TapeRotation_comboBox.SelectedItem;
        }

        private void Nozzle_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Row.Cells["NozzleColumn"].Value = Nozzle_numericUpDown.Value.ToString();
        }

        private void TapeWidth_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TapeWidth_comboBox.SelectedItem.ToString())
            {
                case "8/2mm":
                    TapePitch_textBox.Text = "2.0";
                    TapeOffsetX_textBox.Text = "3.5";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "8/4mm":
                    TapePitch_textBox.Text = "4.0";
                    TapeOffsetX_textBox.Text = "3.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "12/4mm":
                    TapePitch_textBox.Text = "4.0";
                    TapeOffsetX_textBox.Text = "5.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "12/8mm":
                    TapePitch_textBox.Text = "8.0";
                    TapeOffsetX_textBox.Text = "5.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "16/4mm":
                    TapePitch_textBox.Text = "4.0";
                    TapeOffsetX_textBox.Text = "7.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "16/8mm":
                    TapePitch_textBox.Text = "8.0";
                    TapeOffsetX_textBox.Text = "7.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "16/12mm":
                    TapePitch_textBox.Text = "12.0";
                    TapeOffsetX_textBox.Text = "7.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "24/4mm":
                    TapePitch_textBox.Text = "4.0";
                    TapeOffsetX_textBox.Text = "11.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "24/8mm":
                    TapePitch_textBox.Text = "8.0";
                    TapeOffsetX_textBox.Text = "11.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "24/12mm":
                    TapePitch_textBox.Text = "12.0";
                    TapeOffsetX_textBox.Text = "11.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "24/16mm":
                    TapePitch_textBox.Text = "16.0";
                    TapeOffsetX_textBox.Text = "11.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "24/20mm":
                    TapePitch_textBox.Text = "20.0";
                    TapeOffsetX_textBox.Text = "11.50";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/4mm":
                    TapePitch_textBox.Text = "4.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/8mm":
                    TapePitch_textBox.Text = "8.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/12mm":
                    TapePitch_textBox.Text = "12.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/16mm":
                    TapePitch_textBox.Text = "16.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/20mm":
                    TapePitch_textBox.Text = "20.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/24mm":
                    TapePitch_textBox.Text = "24.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/28mm":
                    TapePitch_textBox.Text = "28.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                case "32/32mm":
                    TapePitch_textBox.Text = "32.0";
                    TapeOffsetX_textBox.Text = "14.20";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
                default:
                    TapePitch_textBox.Text = "0.0";
                    TapeOffsetX_textBox.Text = "0.0";
                    TapeOffsetY_textBox.Text = "2.0";
                    break;
            }
        }
    }
}

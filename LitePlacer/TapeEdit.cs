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
        public FormMain MainForm;

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

        // =================================================================================
        private void TapeEditForm_Load(object sender, EventArgs e)
        {
            Row = TapesDataGrid.Rows[TapeRowNo];
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
            Nozzle_numericUpDown.Maximum = Properties.Settings.Default.Nozzles_count;
            if (Row.Cells["Nozzle_Column"].Value!=null)
            {
                int nozzle;
                if (int.TryParse(Row.Cells["Nozzle_Column"].Value.ToString(), out nozzle))
                {
                    Nozzle_numericUpDown.Value = nozzle;
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
            if (Row.Cells["Capacity_Column"].Value != null)
            {
            }
            if (Row.Cells["Type_Column"].Value != null)
            {
            }
            if (Row.Cells["TrayID_Column"].Value != null)
            {
            }
            if (Row.Cells["Rotation_Column"].Value != null)
            {
            }
            if (Row.Cells["FirstX_Column"].Value != null)
            {
            }
            if (Row.Cells["FirstY_Column"].Value != null)
            {
            }
            if (Row.Cells["Z_Pickup_Column"].Value != null)
            {
            }
            if (Row.Cells["Z_Place_Column"].Value != null)
            {
            }
        }

        // =================================================================================
        private void TapeEditOK_button_Click(object sender, EventArgs e)
        {
            Row.Cells["Id_Column"].Value = ID_textBox.Text;
            Row.Cells["Rotation_Column"].Value = TapeRotation_comboBox.SelectedItem;
            Row.Cells["Orientation_Column"].Value = TapeOrientation_comboBox.SelectedItem;
            Row.Cells["Nozzle_Column"].Value = Nozzle_numericUpDown.Value.ToString();
            Row.Cells["Pitch_Column"].Value = TapePitch_textBox.Text;
            Row.Cells["OffsetX_Column"].Value = TapeOffsetX_textBox.Text;
            Row.Cells["OffsetY_Column"].Value = TapeOffsetY_textBox.Text;
            this.Close();
        }


        private void TapeWidth_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            double Xoff;
            double Yoff;
            double pitch;
            MainForm.TapeWidthStringToValues(TapeWidth_comboBox.SelectedItem.ToString(), out Xoff, out Yoff, out pitch);
            TapeOffsetX_textBox.Text = Xoff.ToString();
            TapeOffsetY_textBox.Text = Yoff.ToString();
            TapePitch_textBox.Text = pitch.ToString();
        }

        private void TapeEditCancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

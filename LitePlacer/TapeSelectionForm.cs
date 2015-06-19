using System;
using System.Drawing;
using System.Windows.Forms;
using LitePlacer.Properties;

namespace LitePlacer {
    public partial class TapeSelectionForm : Form {
        public DataGridView Grid;
        public string ID = "none";
        public string HeaderString = "";

        const int ButtonWidth = 75;
        const int ButtonHeight = 23;
        const int SideGap = 12;
        const int ButtonGap = 6;
        Size ButtonSize = new Size(ButtonWidth, ButtonHeight);

        private Size GridSizeSave;

        public TapeSelectionForm(DataGridView grd) {
            InitializeComponent();
            Grid = grd;
            GridSizeSave = Grid.Size;
            // this.Size = new Size(10 * ButtonWidth + 9*ButtonGap + 2 * SideGap+20, 133);  // 20?? 404; 480
            Controls.Add(Grid);
            for (int i = 0; i < Grid.RowCount; i++) {
                Grid.Rows[i].Cells[0].Value = "Select";
            }
            Grid.Columns[0].Visible = true;
            Grid.Location = new Point(15, 59);
            Grid.Size = new Size(800, 480);
            // Add a CellClick handler to handle clicks in the button column.
            Grid.CellClick += Grid_CellClick;
        }

        private void CloseForm() {
            for (int i = 0; i < Grid.RowCount; i++) {
                Grid.Rows[i].Cells[0].Value = "Reset";
            }
            Grid.Size = GridSizeSave;
            Grid.CellClick -= Grid_CellClick;
            Close();
        }

        private void TapeSelectionForm_Load(object sender, EventArgs e) {
            Text = HeaderString;
            UpdateJobData_checkBox.Checked = Settings.Default.Placement_UpdateJobGridAtRuntime;
        }


        private void AbortJob_button_Click(object sender, EventArgs e) {
            ID = "Abort";
            CloseForm();
        }

        private void UpdateJobData_checkBox_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Placement_UpdateJobGridAtRuntime = UpdateJobData_checkBox.Checked;
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e) {
            // Ignore clicks that are not on button cell  IdColumn
            if ((e.RowIndex < 0) || (e.ColumnIndex != Grid.Columns[0].Index)) {
                return;
            }
            ID = Grid.Rows[e.RowIndex].Cells[1].Value.ToString();
            CloseForm();
        }

        private void Ignore_button_Click(object sender, EventArgs e) {
            ID = "Ignore";
            CloseForm();
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;
using LitePlacer.Properties;

namespace LitePlacer {
    public partial class TapeSelectionForm : Form {
        public string ID = "none";
        public string HeaderString = "";
        private readonly TapesClass tapes;
        
        public TapeSelectionForm(TapesClass tapes) {
            InitializeComponent();
            this.tapes = tapes;
            dataGridView_tapeSelect.DataSource = tapes.tapeObjs;
            var grid = dataGridView_tapeSelect;
            grid.CellClick += Grid_CellClick;
        }

        private void TapeSelectionForm_Load(object sender, EventArgs e) {
            Text = HeaderString;
            UpdateJobData_checkBox.Checked = Settings.Default.Placement_UpdateJobGridAtRuntime;
        }


        private void AbortJob_button_Click(object sender, EventArgs e) {
            ID = "Abort";
            Close();
        }

        private void UpdateJobData_checkBox_CheckedChanged(object sender, EventArgs e) {
            Settings.Default.Placement_UpdateJobGridAtRuntime = UpdateJobData_checkBox.Checked;
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e) {
            // Ignore clicks that are not on button cell  IdColumn
            var Grid = dataGridView_tapeSelect;
            if ((e.RowIndex < 0) || (e.ColumnIndex != Grid.Columns[0].Index)) {
                return;
            }
            ID = tapes.tapeObjs[e.RowIndex].ID;
            Close();
            
        }

        private void Ignore_button_Click(object sender, EventArgs e) {
            ID = "Ignore";
            Close();
        }
    }
}

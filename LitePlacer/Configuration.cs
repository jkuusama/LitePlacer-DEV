using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
//using System.Web.Script.Serialization;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AForge.Imaging;
using System.Windows.Media;
using MathNet.Numerics;
using HomographyEstimation;

using System.Text.RegularExpressions;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu;

namespace LitePlacer {
    public partial class FormMain : Form {

        Properties.Settings setting = Properties.Settings.Default;

        public PartLocation GeneralParkLocation {
            get { return new PartLocation(setting.General_ParkX, setting.General_ParkY); }
            set { setting.General_ParkX = value.X; setting.General_ParkY = value.Y; }
        }



        public void SaveConfiguration() {
            setting.Save();
            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            int i = path.LastIndexOf('\\');
            path = path.Remove(i + 1);
            SaveDataGrid(path + "LitePlacer.ComponentData", ComponentData_dataGridView);
            SaveDataGrid(path + "LitePlacer.TapesData", Tapes_dataGridView);
            SaveDataGrid(path + "LitePlacer.HomingFunctions", Homing_dataGridView);
            SaveDataGrid(path + "LitePlacer.FiducialsFunctions", Fiducials_dataGridView);
            SaveDataGrid(path + "LitePlacer.ComponentsFunctions", Components_dataGridView);
            SaveDataGrid(path + "LitePlacer.PaperTapeFunctions", PaperTape_dataGridView);
            SaveDataGrid(path + "LitePlacer.BlackTapeFunctions", BlackTape_dataGridView);
            SaveDataGrid(path + "LitePlacer.ClearTapeFunctions", ClearTape_dataGridView);
            SaveDataGrid(path + "LitePlacer.NeedleFunctions", Needle_dataGridView);
            SaveDataGrid(path + "LitePlacer.UpCamComponentsFunctions", UpCamComponents_dataGridView);
        }

        public void LoadConfiguration() {
            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            int i = path.LastIndexOf('\\');
            path = path.Remove(i + 1);
            LoadDataGrid(path + "LitePlacer.ComponentData", ComponentData_dataGridView);
            LoadDataGrid(path + "LitePlacer.TapesData", Tapes_dataGridView);
            Tapes.ParseAll(); // Convert Table To Object List
            LoadDataGrid(path + "LitePlacer.HomingFunctions", Homing_dataGridView);
            LoadDataGrid(path + "LitePlacer.FiducialsFunctions", Fiducials_dataGridView);
            LoadDataGrid(path + "LitePlacer.ComponentsFunctions", Components_dataGridView);
            LoadDataGrid(path + "LitePlacer.PaperTapeFunctions", PaperTape_dataGridView);
            LoadDataGrid(path + "LitePlacer.BlackTapeFunctions", BlackTape_dataGridView);
            LoadDataGrid(path + "LitePlacer.ClearTapeFunctions", ClearTape_dataGridView);
            LoadDataGrid(path + "LitePlacer.NeedleFunctions", Needle_dataGridView);
            LoadDataGrid(path + "LitePlacer.UpCamComponentsFunctions", UpCamComponents_dataGridView);
            SetProcessingFunctions(Display_dataGridView);
            SetProcessingFunctions(Homing_dataGridView);
            SetProcessingFunctions(Fiducials_dataGridView);
            SetProcessingFunctions(Components_dataGridView);
            SetProcessingFunctions(PaperTape_dataGridView);
            SetProcessingFunctions(BlackTape_dataGridView);
            SetProcessingFunctions(ClearTape_dataGridView);
            SetProcessingFunctions(Needle_dataGridView);
            SetProcessingFunctions(UpCamComponents_dataGridView);

            Bookmark1_button.Text = setting.General_Mark1Name;
            Bookmark2_button.Text = setting.General_Mark2Name;
            Bookmark3_button.Text = setting.General_Mark3Name;
            Bookmark4_button.Text = setting.General_Mark4Name;
            Bookmark5_button.Text = setting.General_Mark5Name;
          //  Bookmark6_button.Text = setting.General_Mark6Name;
            Mark1_textBox.Text = setting.General_Mark1Name;
            Mark2_textBox.Text = setting.General_Mark2Name;
            Mark3_textBox.Text = setting.General_Mark3Name;
            Mark4_textBox.Text = setting.General_Mark4Name;
            Mark5_textBox.Text = setting.General_Mark5Name;
          //  Mark6_textBox.Text = setting.General_Mark6Name;


            zoffset_textbox.Text = setting.z_offset.ToString();
            // template based fudical locating RN
            cb_useTemplate.Checked = setting.use_template;
            FindFiducials_cb.Enabled = setting.use_template;
            fiducialTemlateMatch_textBox.Text = setting.template_threshold.ToString();
            fiducial_designator_regexp_textBox.Text = setting.fiducial_designator_regexp;

        }

        // =================================================================================
        // Saving and restoring data tables (Note: Not job files)
        // =================================================================================
        private void SaveDataGrid(string FileName, DataGridView dgv) {
            try {
                using (BinaryWriter bw = new BinaryWriter(File.Open(FileName, FileMode.Create))) {
                    bw.Write(dgv.Columns.Count);
                    bw.Write(dgv.Rows.Count);
                    foreach (DataGridViewRow dgvR in dgv.Rows) {
                        for (int j = 0; j < dgv.Columns.Count; ++j) {
                            object val = dgvR.Cells[j].Value;
                            if (val == null) {
                                bw.Write(false);
                                bw.Write(false);
                            } else {
                                bw.Write(true);
                                bw.Write(val.ToString());
                            }
                        }
                    }
                }
            } catch (System.Exception excep) {
                MessageBox.Show(excep.Message);
            }
        }

        private void LoadDataGrid(string FileName, DataGridView dgv) {
            try {
                if (!File.Exists(FileName)) {
                    return;
                }
                dgv.Rows.Clear();
                using (BinaryReader bw = new BinaryReader(File.Open(FileName, FileMode.Open))) {
                    int n = bw.ReadInt32();
                    int m = bw.ReadInt32();
                    if (dgv.AllowUserToAddRows) {
                        // There is an empty row in the bottom that is visible for manual add.
                        // It is saved in the file. It is automatically added, so we don't want to add it also.
                        // It is not there when rows are added only programmatically, so we need to do it here.
                        m = m - 1;
                    }
                    for (int i = 0; i < m; ++i) {
                        dgv.Rows.Add();
                        for (int j = 0; j < n; ++j) {
                            if (bw.ReadBoolean()) {
                                dgv.Rows[i].Cells[j].Value = bw.ReadString();
                            } else bw.ReadBoolean();
                        }
                    }
                }
            } catch (System.Exception excep) {
                MessageBox.Show(excep.Message);
            }
        }



    }
}

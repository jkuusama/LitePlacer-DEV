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
using System.Drawing.Imaging;
using System.Reflection;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Input;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

using AForge;
using AForge.Video;
using Newtonsoft.Json;


namespace LitePlacer
{
    public partial class FormMain : Form
    {
        enum Functions_dataGridViewColumns : int { FunctionColumn, ActiveColumn };
        public List<string> KnownFunctions = new List<string> {"Threshold", "Histogram", "Grayscale", "Invert", "Edge detect",
                "Noise reduction", "Kill color", "Keep color", "Blur", "Gaussian blur", "Meas. zoom"};

        public VideoAlgorithmsCollection VideoAlgorithms;

        // =====================================================================================
        // interface to main form:
        Camera cam;

        private void Algorithms_tabPage_Begin()
        {
            SetDownCameraDefaults();
            SetUpCameraDefaults();
            if (UpCamera.Active)
            {
                UpCam_radioButton.Checked = true;
            }
            else
            {
                DownCam_radioButton.Checked = true;
            }
            AlgorithmsTab_RestoreBehaviour();
            ProcessDisplay_checkBox_Checked_Change();
        }

        private void Algorithms_tabPage_End()
        {
        }

        public void InitVideoAlgorithmsUI()
        {
            DownCam_radioButton.Checked = true; // default to Downcamera
            Functions_dataGridView.Rows.Clear();
            DataGridViewComboBoxColumn comboboxColumn =
                 (DataGridViewComboBoxColumn)Functions_dataGridView.Columns[(int)Functions_dataGridViewColumns.FunctionColumn];
            comboboxColumn.Items.Clear();
            comboboxColumn.DataSource = KnownFunctions;

            VideoAlgorithms = new VideoAlgorithmsCollection();
            LoadVideoAlgorithms(VideoAlgorithms); // causes updating of Functions_dataGridView and Function parameters
        }

        // =====================================================================================
        #region select, draw and find boxes

        private void AlgorithmsTab_RestoreBehaviour()
        {
            // called on tab load and camera change
            FindCircles_checkBox.Checked = cam.FindCircles;
            FindRectangles_checkBox.Checked = cam.FindRectangles;
            FindComponents_checkBox.Checked = cam.FindComponent;
            if (ProcessDisplay_checkBox.Checked)
            {
                UpdateVideoProcessing();
            }
            else
            {
                StopVideoProcessing();
            }

        }

        // camera change
        private void DownCam_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            cam = DownCamera;
            SelectCamera(DownCamera);
            AlgorithmsTab_RestoreBehaviour();
        }

        private void UpCam_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            cam = UpCamera;
            SelectCamera(UpCamera);
            AlgorithmsTab_RestoreBehaviour();
        }

        // =====================================================================================
        // find boxes
        private void FindCircles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            cam.FindCircles = FindCircles_checkBox.Checked;
        }

        private void FindRectangles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            cam.FindRectangles = FindRectangles_checkBox.Checked;
        }

        private void FindComponents_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            cam.FindComponent = FindComponents_checkBox.Checked;
        }

        #endregion select, draw and find boxes
        // =====================================================================================

        // =====================================================================================
        #region Algorithms Load and Save

        // Load:
        private bool AlgorithmChange = false;   // to prevent parameters etc updates in the middle of process

        private void AlgorithmsLoad_button_Click(object sender, EventArgs e)
        {
            LoadVideoAlgorithms(VideoAlgorithms);
        }

        private void LoadVideoAlgorithms(VideoAlgorithmsCollection Collection)
        {
            AlgorithmChange = true;
            string path = Application.StartupPath + '\\';
            string FileName = path + "LitePlacer.VideoAlgorithms";
            if (File.Exists(FileName))
            {
                DisplayText("LoadVideoAlgorithms from " + FileName);
                List<VideoAlgorithmsCollection.FullAlgorithmDescription> NewList = new List<VideoAlgorithmsCollection.FullAlgorithmDescription>();
                NewList = JsonConvert.DeserializeObject<List<VideoAlgorithmsCollection.FullAlgorithmDescription>>(File.ReadAllText(FileName));
                Collection.AllAlgorithms = NewList;
            }
            else
            {
                LoadOldVideoAlgorithms(path, Collection);
            }
            // fill Algorithm_comboBox
            Algorithm_comboBox.Items.Clear();
            foreach (VideoAlgorithmsCollection.FullAlgorithmDescription Algorithm in Collection.AllAlgorithms)
            {
                Algorithm_comboBox.Items.Add(Algorithm.Name);
            }
            ClearFunctionParameters();
            Algorithm_comboBox.SelectedIndex = 0;
            Functions_dataGridView.CurrentCell = null;
            AlgorithmChange = false;
        }


        private void LoadOldVideoAlgorithms(string path, VideoAlgorithmsCollection Collection)
        {
            // For now (maybe for good?), build an placeholder list
            List<VideoAlgorithmsCollection.FullAlgorithmDescription> NewList = new List<VideoAlgorithmsCollection.FullAlgorithmDescription>();
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Homing" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Fiducials" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Paper tape" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Black tape" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Clear tape" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Components" });
            NewList.Add(new VideoAlgorithmsCollection.FullAlgorithmDescription { Name = "Nozzle tip" });
            Collection.AllAlgorithms = NewList;
        }

        // ========= Save

        private bool SaveVideoAlgorithms(string FileName, VideoAlgorithmsCollection Collection)
        {
            try
            {
                DisplayText("SaveVideoAlgorithms to " + FileName);
                File.WriteAllText(FileName, JsonConvert.SerializeObject(VideoAlgorithms.AllAlgorithms, Formatting.Indented));
                return true;
            }
            catch (System.Exception excep)
            {
                DisplayText("Saving Video algorithms failed. " + excep.Message);
                return false;
            }
        }

        private void AlgorithmsSave_button_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + '\\';
            SaveVideoAlgorithms(path + "LitePlacer.VideoAlgorithms", VideoAlgorithms);
        }

        #endregion Algorithms Load and Save
        // =====================================================================================

        // =====================================================================================
        #region Current video algorithm

        private void Algorithm_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlgorithmChange = true;
            string AlgorithmName = Algorithm_comboBox.SelectedItem.ToString();
            // DisplayText("Algorithm_comboBox_SelectedIndexChanged(), func: " + AlgorithmName);
            VideoAlgorithms.SelectedAlgorithmChanged(AlgorithmName);
            FillFunctionTable(AlgorithmName);
            FillMeasurementValues(AlgorithmName);
            ClearFunctionParameters();
            Functions_dataGridView.CurrentCell = null;
            AlgorithmChange = false;
            if (ProcessDisplay_checkBox.Checked)
            {
                UpdateVideoProcessing();
            }

        }

        bool ChangeYwithX = false;

        private void FillMeasurementValues(string ListName)
        {
            // User changed the current algorithm. This function fills the measurement value boxes
            MeasurementParametersClass values = VideoAlgorithms.CurrentAlgorithm.MeasurementParameters;
            SearchRound_checkBox.Checked = values.SearchRounds;
            SearchRectangles_checkBox.Checked = values.SearchRectangles;
            SearchComponents_checkBox.Checked = values.SearchComponents;
            // for some reason(??), the order of things to set are not honored. A workaround:
            ChangeYwithX = false;
            Xmin_textBox.Text = values.Xmin.ToString("0.00", CultureInfo.InvariantCulture);
            Xmax_textBox.Text = values.Xmax.ToString("0.00", CultureInfo.InvariantCulture);
            Ymin_textBox.Text = values.Ymin.ToString("0.00", CultureInfo.InvariantCulture);
            Ymax_textBox.Text = values.Ymax.ToString("0.00", CultureInfo.InvariantCulture);
            XmaxDistance_textBox.Text = values.XUniqueDistance.ToString("0.00", CultureInfo.InvariantCulture);
            YmaxDistance_textBox.Text = values.XUniqueDistance.ToString("0.00", CultureInfo.InvariantCulture);
            ChangeYwithX = true;
        }
        // =====================================================================================
        // Add, Remove, Duplicate, Rename

        // ==================
        // helpers
        public string GetName(string StartName, bool rename)
        {
            AlgorithmNameForm GetNameDialog = new AlgorithmNameForm(StartName);
            GetNameDialog.Algorithms = VideoAlgorithms.AllAlgorithms;
            GetNameDialog.Renaming = rename;
            GetNameDialog.StartPosition = FormStartPosition.CenterParent;
            GetNameDialog.ShowDialog(this);
            if (GetNameDialog.OK)
            {
                return GetNameDialog.NewName;
            }
            else
            {
                return null;
            }
        }

        private bool FindLocation(string AlgorithmName, out int loc)
        {
            // returns the index of the named algorithm in AllAlgorithms
            for (int i = 0; i < VideoAlgorithms.AllAlgorithms.Count; i++)
            {
                if (VideoAlgorithms.AllAlgorithms[i].Name == AlgorithmName)
                {
                    loc = i;
                    return true;
                }
            }
            loc = -1;
            return false;
        }

        public static T DeepClone<T>(T obj)
        {
            string clone = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return JsonConvert.DeserializeObject<T>(clone);
        }

        // ==================
        // Button clicks

        private void AddAlgorithm_button_Click(object sender, EventArgs e)
        {
            string NewName = GetName("", false);
            if (NewName == null)
            {
                DisplayText("Add algorithm canceled");
                return;
            }
            DisplayText("Add algorithm " + NewName);
            VideoAlgorithmsCollection.FullAlgorithmDescription Alg = new VideoAlgorithmsCollection.FullAlgorithmDescription();
            Alg.Name = NewName;
            VideoAlgorithms.AllAlgorithms.Add(Alg);
            Algorithm_comboBox.Items.Add(NewName);
            Algorithm_comboBox.SelectedIndex = Algorithm_comboBox.Items.Count - 1;
        }

        private void RemoveAlgorithm_button_Click(object sender, EventArgs e)
        {
            int pos = 0;
            if (!FindLocation(Algorithm_comboBox.SelectedItem.ToString(), out pos))
            {
                DisplayText("Remove algorithm, algorithm not found!");
                return;
            }
            VideoAlgorithms.AllAlgorithms.RemoveAt(pos);
            Algorithm_comboBox.Items.RemoveAt(Algorithm_comboBox.SelectedIndex);
            AlgorithmChange = true;
            ClearFunctionParameters();
            Algorithm_comboBox.SelectedIndex = 0;
            Functions_dataGridView.CurrentCell = null;
            AlgorithmChange = false;
            UpdateVideoProcessing();
        }

        private void DuplicateAlgorithm_button_Click(object sender, EventArgs e)
        {
            int loc;
            if (!FindLocation(Algorithm_comboBox.SelectedItem.ToString(), out loc))
            {
                DisplayText("Duplicate algorithm, algorithm not found!");
                return;
            }
            DisplayText("Duplicate algorithm " + VideoAlgorithms.AllAlgorithms[loc].Name);
            VideoAlgorithmsCollection.FullAlgorithmDescription Alg =
                DeepClone<VideoAlgorithmsCollection.FullAlgorithmDescription>(VideoAlgorithms.AllAlgorithms[loc]);
            Alg.Name = VideoAlgorithms.AllAlgorithms[loc].Name + "(duplicate)";
            VideoAlgorithms.AllAlgorithms.Add(Alg);
            Algorithm_comboBox.Items.Add(Alg.Name);
            Algorithm_comboBox.SelectedIndex = Algorithm_comboBox.Items.Count - 1;
            // User propably doesn't want the name+(duplicate), so let's click the rename button automatically
            RenameAlgorithm_button_Click(sender, e);
        }

        private void RenameAlgorithm_button_Click(object sender, EventArgs e)
        {
            if (Algorithm_comboBox.SelectedIndex == 0)
            {
                DisplayText("Homing can't be renamed.");
                return;
            }
            string OldName = Algorithm_comboBox.SelectedItem.ToString();
            string NewName = GetName(OldName, true);
            if (NewName == null)
            {
                DisplayText("Rename algorithm canceled");
                return;
            }
            DisplayText("Rename algorithm to " + NewName);
            int AlgPos;
            if (!FindLocation(OldName, out AlgPos))
            {
                DisplayText("***Rename algorithm, algorithm not found!");
                return;
            }
            VideoAlgorithms.AllAlgorithms[AlgPos].Name = NewName;
            int NamePos = Algorithm_comboBox.SelectedIndex;
            Algorithm_comboBox.Items.RemoveAt(NamePos);
            Algorithm_comboBox.Items.Insert(NamePos, NewName);
            Algorithm_comboBox.SelectedIndex = NamePos;

            // Rename algorithm names if already in use
            // Fiducials on the job table
            foreach (DataGridViewRow row in JobData_GridView.Rows)
            {
                if (row.Cells["GroupMethod"].Value.ToString() == "Fiducials")
                {
                    if (row.Cells["MethodParamAllComponents"].Value.ToString() == OldName)
                    {
                        row.Cells["MethodParamAllComponents"].Value = null;
                        DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
                        BuildAlgorithmsCombox(out c);
                        row.Cells["MethodParamAllComponents"] = c;
                        row.Cells["MethodParamAllComponents"].Value = NewName;
                    }
                }
            }
            // Tapes data table
            foreach (DataGridViewRow row in Tapes_dataGridView.Rows)
            {
                string CurrentValue = row.Cells["Type_Column"].Value.ToString();
                row.Cells["Type_Column"].Value = null;
                DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
                BuildAlgorithmsCombox(out c);
                row.Cells["Type_Column"] = c;
                if (VideoAlgorithms.AlgorithmExists(CurrentValue))
                {
                    row.Cells["Type_Column"].Value = CurrentValue;
                }
                else
                {
                    // if CurrentValue does not exist anymore, the name was changed
                    if (CurrentValue != OldName)
                    {
                        DisplayText("*** Rename algorithm: Assert failed!");  // or something else is off
                    }
                    row.Cells["Type_Column"].Value = NewName;
                }
            }
            // Nozzles

        }

        private void ProcessDisplay_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            ProcessDisplay_checkBox_Checked_Change();
        }

        private void ProcessDisplay_checkBox_Checked_Change()
        {
            if (ProcessDisplay_checkBox.Checked)
            {
                UpdateVideoProcessing();
            }
            else
            {
                StopVideoProcessing();
            }
        }

        #endregion Current video algorithm
        // =====================================================================================

        // =====================================================================================
        #region Functions and parameters

        // =====================================================================================
        // Buttons:

        private void AddFunction_button_Click(object sender, EventArgs e)
        {
            // adding a row will change current cell; we don't want false update of function parameters
            AlgorithmChange = true;
            DataGridViewSelectedRowCollection SelectedRows = Functions_dataGridView.SelectedRows;
            int index = 0;
            if (Functions_dataGridView.Rows.Count == 0)
            {
                // grid is empty:
                Functions_dataGridView.Rows.Insert(0);
            }
            else
            {
                // insert at end
                Functions_dataGridView.Rows.Insert(Functions_dataGridView.Rows.Count);
                index = Functions_dataGridView.Rows.Count - 1;
            };
            Functions_dataGridView.CurrentCell = null;
            AlgorithmChange = false;
        }

        // ===================
        private void RemoveFunction_button_Click(object sender, EventArgs e)
        {
            // If function exists, remove it.
            if (VideoAlgorithms.CurrentFunctionIndex >= 0)
            {
                bool WasActive =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].Active;
                VideoAlgorithms.CurrentAlgorithm.FunctionList.RemoveAt(VideoAlgorithms.CurrentFunctionIndex);
                VideoAlgorithms.CurrentFunctionIndex = -1;
                // We don't want false update of function parameters
                AlgorithmChange = true;
                FillFunctionTable(VideoAlgorithms.CurrentAlgorithm.Name);
                ClearFunctionParameters();
                Functions_dataGridView.CurrentCell = null;
                AlgorithmChange = false;
                if (WasActive)
                {
                    UpdateVideoProcessing();
                }
            }
        }

        // ===================
        private void MoveUp_button_Click(object sender, EventArgs e)
        {
            if (Functions_dataGridView.CurrentCell == null)
            {
                DisplayText("Move up, cell=null");
                return;
            }
            int OldPos = Functions_dataGridView.CurrentCell.RowIndex;
            if (OldPos == 0)
            {
                DisplayText("Move up, at top (row==0)");
                return;
            }
            MoveFunction(OldPos, OldPos - 1);
        }

        // ===================
        private void MoveDown_button_Click(object sender, EventArgs e)
        {
            if (Functions_dataGridView.CurrentCell == null)
            {
                DisplayText("Move down, cell=null");
                return;
            }
            int OldPos = Functions_dataGridView.CurrentCell.RowIndex;
            if (OldPos >= Functions_dataGridView.RowCount - 1)
            {
                DisplayText("Move down, at bottom)");
                return;
            }
            MoveFunction(OldPos, OldPos + 1);
        }

        // ===================
        private void MoveFunction(int OldPos, int NewPos)
        {
            // Re-arranges functions in the UI
            AForgeFunctionDefinition funct = VideoAlgorithms.CurrentAlgorithm.FunctionList[OldPos];
            VideoAlgorithms.CurrentAlgorithm.FunctionList.RemoveAt(OldPos);
            VideoAlgorithms.CurrentAlgorithm.FunctionList.Insert(NewPos, funct);
            int col = Functions_dataGridView.CurrentCell.ColumnIndex;
            AlgorithmChange = true;
            FillFunctionTable(VideoAlgorithms.CurrentAlgorithm.Name);
            ClearFunctionParameters();
            Functions_dataGridView.CurrentCell = null; // to force the change event at last statement
            AlgorithmChange = false;
            Functions_dataGridView.CurrentCell = Functions_dataGridView[col, NewPos];
            if (funct.Active)
            {
                UpdateVideoProcessing();
            }
        }

        // ===================
        private void Algorithm_Measure_button_Click(object sender, EventArgs e)
        {
            Camera cam = UpCamera;
            double Xmult = Setting.UpCam_XmmPerPixel;
            double Ymult = Setting.UpCam_YmmPerPixel;
            if (DownCam_radioButton.Checked)
            {
                cam = DownCamera;
                Xmult = Setting.DownCam_XmmPerPixel;
                Ymult = Setting.DownCam_YmmPerPixel;
            }
            cam.BuildMeasurementFunctionsList(VideoAlgorithms.CurrentAlgorithm.FunctionList);
            cam.MeasurementParameters = VideoAlgorithms.CurrentAlgorithm.MeasurementParameters;
            cam.Measure(out double X, out double Y, out int err, Xmult, Ymult, true);
        }


        // =====================================================================================
        // Functions_dataGridView 

        // =====================================================================================
        // Helper functions:

        public void BuildAlgorithmsCombox(out DataGridViewComboBoxCell Cout)
        {
            DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
            for (int i = 1; i < VideoAlgorithms.AllAlgorithms.Count; i++)
            {
                c.Items.Add(VideoAlgorithms.AllAlgorithms[i].Name);
            }
            Cout = c;
        }


        void FillFunctionTable(string AlgorithmName)
        {
            // User changed the current algorithm or deleted a fuction. 
            // This function (re-)fills Algorithms_dataGridView function column
            Functions_dataGridView.Rows.Clear();
            int row = 0;
            AForgeFunctionDefinition func = new AForgeFunctionDefinition();

            for (int i = 0; i < VideoAlgorithms.CurrentAlgorithm.FunctionList.Count; i++)
            {
                func = VideoAlgorithms.CurrentAlgorithm.FunctionList[i];
                if (KnownFunctions.Contains(func.Name))
                {
                    int index = KnownFunctions.IndexOf(func.Name);
                    Functions_dataGridView.Rows.Add();
                    Functions_dataGridView.Rows[row].Cells[(int)Functions_dataGridViewColumns.FunctionColumn].Value =
                        func.Name;
                    Functions_dataGridView.Rows[row].Cells[(int)Functions_dataGridViewColumns.ActiveColumn].Value =
                        func.Active;
                    row++;
                }
            }
            Update_GridView(Functions_dataGridView);
        }

        // =====================================================================================
        // Grid cell events:

        private void Functions_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (AlgorithmChange)
            {
                //DisplayText("Functions_dataGridView_CellValueChanged(), AlgorithmChange");
                return;
            }
            if (Functions_dataGridView.CurrentCell == null)
            {
                //DisplayText("Functions_dataGridView_CellValueChanged(), cell=null");
                return;
            }
            int row = Functions_dataGridView.CurrentCell.RowIndex;
            int col = Functions_dataGridView.CurrentCell.ColumnIndex;
            //DisplayText("Functions_dataGridView_CellValueChanged(), " + row.ToString() + ", " + col.ToString());
            int FunctCol = (int)Functions_dataGridViewColumns.FunctionColumn;
            int ActiveCol = (int)Functions_dataGridViewColumns.ActiveColumn;

            if (Functions_dataGridView.Rows[row].Cells[FunctCol].Value == null)
            {
                //DisplayText("value: null");
                return;
            };
            //DisplayText("value: " + Functions_dataGridView.Rows[row].Cells[FunctCol].Value.ToString());
            Update_GridView(Functions_dataGridView);

            if (col == FunctCol)
            {
                // Function column changed
                string FunctionName = Functions_dataGridView.Rows[row].Cells[FunctCol].Value.ToString();
                VideoAlgorithms.CurrentFunctionIndex = row;
                VideoAlgorithms.CurrentAlgorithm.FunctionList[row].Name = FunctionName;
                SetFunctionDefaultParameters(FunctionName);
                VideoAlgorithms.CurrentAlgorithm.FunctionList[row].Active = false;  // newly selected function is inactive by default
                UpdateParameterTargets(FunctionName);
                // No need to update video processing
            }
            else
            {
                // active column changed
                VideoAlgorithms.CurrentAlgorithm.FunctionList[row].Active =
                    (bool)Functions_dataGridView.Rows[row].Cells[ActiveCol].Value;
                UpdateVideoProcessing();
            }
        }

        private void Functions_dataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            if (AlgorithmChange)
            {
                DisplayText("Functions_dataGridView_CurrentCellChanged(), AlgorithmChange");
                return;
            }
            if (Functions_dataGridView.CurrentCell == null)
            {
                DisplayText("Functions_dataGridView_CurrentCellChanged(), cell=null");
                return;
            }
            int row = Functions_dataGridView.CurrentCell.RowIndex;
            int col = Functions_dataGridView.CurrentCell.ColumnIndex;
            DisplayText("Functions_dataGridView_CurrentCellChanged(), " + row.ToString() + ", " + col.ToString());
            VideoAlgorithms.CurrentFunctionIndex = row;
            if (Functions_dataGridView.Rows[row].Cells[(int)Functions_dataGridViewColumns.FunctionColumn].Value == null)
            {
                // user is creating new function and has not yet selected the value
                return;
            }
            UpdateParameterTargets(Functions_dataGridView.Rows[row].Cells[(int)Functions_dataGridViewColumns.FunctionColumn].Value.ToString());
        }

        private void Functions_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Functions_dataGridView.CurrentCell == null)
            {
                DisplayText("Functions_dataGridView_CurrentCellDirtyStateChanged(), cell=null");
                return;
            }

            int row = Functions_dataGridView.CurrentCell.RowIndex;
            int col = Functions_dataGridView.CurrentCell.ColumnIndex;

            DisplayText("Functions_dataGridView_CurrentCellDirtyStateChanged: "
                + row.ToString() + ", " + col.ToString());
            // Return if not dirty; otherwise the stuff is executed twice (once when it changes, once when it becomes clean)
            if (!Functions_dataGridView.IsCurrentCellDirty)
            {
                return;
            }
            Update_GridView(Functions_dataGridView);
        }


        // =====================================================================================
        // Functions parameters 

        private void SetFunctionDefaultParameters(string FunctionName)
        {
            // The function is just created. Function is 
            AForgeFunctionDefinition funct = VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex];
            switch (FunctionName)
            {
                // switch by the selected algorithm:  
                case "Blur":
                    break;		// no parameters

                case "Histogram":
                    break;		// no parameters

                case "Grayscale":
                    break;		// no parameters

                case "Invert":
                    break;		// no parameters

                case "Edge detect":
                    // Type, maximum difference of processing pixel with neighboring pixels in 8 direction
                    funct.parameterInt = 3;
                    break;

                case "Kill color":
                    // radius and color
                    funct.parameterInt = 10;
                    funct.R = 128;
                    funct.G = 128;
                    funct.B = 128;
                    break;

                case "Keep color":
                    // radius and color
                    funct.parameterInt = 10;
                    funct.R = 128;
                    funct.G = 128;
                    funct.B = 128;
                    break;

                case "Meas. zoom":
                    funct.parameterDouble = 2.0;
                    break;

                case "Gaussian blur":
                    funct.parameterDouble = 2.0;
                    break;

                case "Threshold":
                    funct.parameterInt = 128;
                    break;

                default:
                    break;
            }
            return;
        }

        private void UpdateParameterTargets(string Name)
        {
            // This function is called when user changes a function in Algorithms_dataGridView
            // The function updates the parameter editing boxes
            // make all inactive
            ClearFunctionParameters();
            // activate those that this function needs, with description text, 
            // and put in default values
            switch (Name)
            {
                // switch by the selected algorithm:  
                case "Blur":
                    // no parameters
                    FunctionExplanation_textBox.Text = "Blurs the image, reducing the effects of camera noise " +
                        "and possible imperfections in the target outline";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                case "Histogram":
                    FunctionExplanation_textBox.Text = "Increases contrast in the image";
                    FunctionExplanation_textBox.Visible = true;
                    break;		// no parameters

                case "Grayscale":
                    FunctionExplanation_textBox.Text = "Converts the image to grayscale";
                    FunctionExplanation_textBox.Visible = true;
                    break;		// no parameters

                case "Invert":
                    FunctionExplanation_textBox.Text = "Inverts the image; the detection functions are looking for" +
                        " white image on black background.";
                    FunctionExplanation_textBox.Visible = true;
                    break;		// no parameters

                case "Edge detect":
                    // single int parameter, 1..4
                    EnableInt(1, 4, "Operator type:");
                    FunctionExplanation_textBox.Text = "Finds edges in the image:\r\n" +
                        "1: Using Sobel operator.\r\n" +
                        "2: Calculating maximum difference between pixels in 4 directions around the processing pixel.\r\n" +
                        "3: Calculating maximum difference of processing pixel with neighboring pixels in 8 direction.\r\n" +
                        "4: Applying Canny edge detector";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                case "Kill color":
                    // int and RGB parameter
                    EnableInt(0, 255, "Radius:");
                    EnableRGB("Color to remove");
                    FunctionExplanation_textBox.Text = "Removes color that is inside of RGB sphere " +
                        "with specified center color and radius.";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                case "Keep color":
                    // int and RGB parameter
                    EnableInt(0, 255, "Radius:");
                    EnableRGB("Color to keep:");
                    FunctionExplanation_textBox.Text = "Removes color that is outside of RGB sphere " +
                        "with specified center color and radius.";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                case "Meas. zoom":
                    // one double parameter
                    EnableDouble("Zoom factor:");
                    FunctionExplanation_textBox.Text = "Enlargens the image that is used for measurements.";
                    FunctionExplanation_textBox.Visible = true;

                    break;

                case "Gaussian blur":
                    // one double parameter
                    EnableDouble("Sigma:");
                    FunctionExplanation_textBox.Text = "Another method to blur the image: gaussian blur with kernel size of 11.";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                case "Threshold":
                    // one int parameter
                    EnableInt(0, 255, "Threshold:");
                    FunctionExplanation_textBox.Text = "Makes the image black and white.";
                    FunctionExplanation_textBox.Visible = true;
                    break;

                default:
                    break;
            }
            return;

            // local functions for UpdateParameterTargets():
            void EnableInt(int min, int max, string label)
            {
                IntParameter_numericUpDown.Minimum = min;
                IntParameter_numericUpDown.Maximum = max;
                IntParameter_numericUpDown.Value =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].parameterInt;
                IntParameter_numericUpDown.Visible = true;
                IntParameter_label.Text = label;
                IntParameter_label.Visible = true;
            }

            void EnableDouble(string label)
            {
                DoubleParameter_textBox.Text =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].parameterDouble.ToString();
                DoubleParameter_textBox.Visible = true;
                DoubleParameter_label.Text = label;
                DoubleParameter_label.Visible = true;
            }

            void EnableRGB(string label)
            {
                R_label.Visible = true;
                G_label.Visible = true;
                B_label.Visible = true;
                R_numericUpDown.Visible = true;
                G_numericUpDown.Visible = true;
                B_numericUpDown.Visible = true;
                R_numericUpDown.Value =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].R;
                G_numericUpDown.Value =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].G;
                B_numericUpDown.Value =
                    VideoAlgorithms.CurrentAlgorithm.FunctionList[VideoAlgorithms.CurrentFunctionIndex].B;
                RGBParameter_label.Text = label;
                RGBParameter_label.Visible = true;
            }
        }       // UpdateParameterTargets() end



        private void ClearFunctionParameters()
        {
            IntParameter_label.Text = "--";
            IntParameter_label.Visible = false;
            IntParameter_numericUpDown.Visible = false;

            DoubleParameter_label.Text = "--";
            DoubleParameter_label.Visible = false;
            DoubleParameter_textBox.Text = "";
            DoubleParameter_textBox.Visible = false;

            RGBParameter_label.Text = "--";
            RGBParameter_label.Visible = false;
            R_numericUpDown.Visible = false;
            G_numericUpDown.Visible = false;
            B_numericUpDown.Visible = false;
            R_label.Visible = false;
            G_label.Visible = false;
            B_label.Visible = false;

            FunctionExplanation_textBox.Text = "";
            FunctionExplanation_textBox.Visible = false;
        }

        private void UpdateVideoProcessing()
        {
            // User changed something, that (potentially) affects the current video processing
            // Note, that there might not be a current algorithm
            if (!ProcessDisplay_checkBox.Checked)
            {
                return;
            }
            if (VideoAlgorithms.CurrentAlgorithm == null)
            {
                DisplayText("UpdateVideoProcessing(), no current algorithm");
                return;
            }
            // Pass CurrentAlgorithm to camera
            DisplayText("UpdateVideoProcessing()");
            if (DownCam_radioButton.Checked)
            {
                DownCamera.BuildDisplayFunctionsList(VideoAlgorithms.CurrentAlgorithm.FunctionList);
            }
            if (UpCam_radioButton.Checked)
            {
                UpCamera.BuildDisplayFunctionsList(VideoAlgorithms.CurrentAlgorithm.FunctionList);
            }
        }

        private void StopVideoProcessing()
        {
            DisplayText("StopVideoProcessing()");
            if (DownCam_radioButton.Checked)
            {
                DownCamera.ClearDisplayFunctionsList();
            }
            if (UpCam_radioButton.Checked)
            {
                UpCamera.ClearDisplayFunctionsList();
            }
        }


        // =====================================================================================
        // Functions parameter changes:
        private void IntParameter_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentFunction_NewInt((int)IntParameter_numericUpDown.Value);
            UpdateVideoProcessing();
        }

        private void DoubleParameter_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(DoubleParameter_textBox);
            if (double.TryParse(DoubleParameter_textBox.Text, out val))
            {
                Ymin_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentFunction_NewDouble(val);
                UpdateVideoProcessing();
            }
            else
            {
                Ymin_textBox.ForeColor = Color.Red;
            }
        }

        private void R_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentFunction_NewR((int)R_numericUpDown.Value);
            UpdateVideoProcessing();
        }

        private void G_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentFunction_NewR((int)G_numericUpDown.Value);
            UpdateVideoProcessing();
        }

        private void B_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentFunction_NewR((int)B_numericUpDown.Value);
            UpdateVideoProcessing();
        }

        private void Functions_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            DisplayText("Functions_dataGridView_DataError ");
        }
        #endregion Functions and parameters
        // =====================================================================================

        // =====================================================================================
        #region search, size and distance

        // =====================================================================================
        // search boxes
        private void SearchRound_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.SearchRounds = SearchRound_checkBox.Checked;
        }

        private void SearchRectangles_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.SearchRectangles = SearchRectangles_checkBox.Checked;
        }

        private void SearchComponents_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.SearchComponents = SearchComponents_checkBox.Checked;
        }

        private void ShowPixels_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowPixels_checkBox.Checked) 
            {
                Cam_pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                Cam_pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            Setting.Cam_ShowPixels = ShowPixels_checkBox.Checked;
        }

        // =====================================================================================
        // Textbox values and interaction
        void CommasToPoints(TextBox box)
        {
            int pos = box.SelectionStart;
            box.Text = box.Text.Replace(',', '.');
            box.SelectionStart = pos;
        }

        private void Xmin_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(Xmin_textBox);
            if (double.TryParse(Xmin_textBox.Text, out val))
            {
                if (ChangeYwithX)
                {
                    Ymin_textBox.Text = Xmin_textBox.Text;
                }
                Xmin_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.Xmin = val;
            }
            else
            {
                Xmin_textBox.ForeColor = Color.Red;
            }
        }

        private void Xmax_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(Xmax_textBox);
            if (double.TryParse(Xmax_textBox.Text, out val))
            {
                if (ChangeYwithX)
                {
                    Ymax_textBox.Text = Xmax_textBox.Text;
                }
                Xmax_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.Xmax = val;
            }
            else
            {
                Xmax_textBox.ForeColor = Color.Red;
            }
        }

        private void Ymin_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(Ymin_textBox);
            if (double.TryParse(Ymin_textBox.Text, out val))
            {
                Ymin_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.Ymin = val;
            }
            else
            {
                Ymin_textBox.ForeColor = Color.Red;
            }
        }

        private void Ymax_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(Ymax_textBox);
            if (double.TryParse(Ymax_textBox.Text, out val))
            {
                Ymax_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.Ymax = val;
            }
            else
            {
                Ymax_textBox.ForeColor = Color.Red;
            }
        }

        private void XmaxDistance_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(XmaxDistance_textBox);
            if (double.TryParse(XmaxDistance_textBox.Text, out val))
            {
                if (ChangeYwithX)
                {
                    YmaxDistance_textBox.Text = XmaxDistance_textBox.Text;
                }
                XmaxDistance_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.XUniqueDistance = val;
            }
            else
            {
                XmaxDistance_textBox.ForeColor = Color.Red;
            }
        }

        private void YmaxDistance_textBox_TextChanged(object sender, EventArgs e)
        {
            double val = 0.0;
            CommasToPoints(YmaxDistance_textBox);
            if (double.TryParse(YmaxDistance_textBox.Text, out val))
            {
                YmaxDistance_textBox.ForeColor = Color.Black;
                VideoAlgorithms.CurrentAlgorithm.MeasurementParameters.YUniqueDistance = val;
            }
            else
            {
                YmaxDistance_textBox.ForeColor = Color.Red;
            }
        }
        #endregion search, size and distance
        // =====================================================================================

    }

}

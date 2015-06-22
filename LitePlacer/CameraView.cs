using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;

namespace LitePlacer {
    public delegate void PictureBoxClickDelegate(double x, double y);

    public partial class CameraView : Form {
        public VideoCapture upVideoCapture;
        public VideoCapture downVideoCapture;
        public VideoProcessing upVideoProcessing;
        public VideoProcessing downVideoProcessing;

        private AForgeFunctionSet upSet, downSet;
        private BindingList<AForgeFunction> currentUpBinding = new BindingList<AForgeFunction>();
        private BindingList<AForgeFunction> currentDownBinding = new BindingList<AForgeFunction>();
        public PictureBoxClickDelegate upClickDelegate, downClickDelegate;

        public void Cleanup(object sender, System.ComponentModel.CancelEventArgs e) {
            // stop forwarding frames
            downVideoCapture.FrameCaptureDelegates.Clear();
            upVideoCapture.FrameCaptureDelegates.Clear();
            Thread.Sleep(100);
            // shut down cameras
            if (downVideoCapture.IsRunning()) downVideoCapture.NoWaitClose();
            if (upVideoCapture.IsRunning()) upVideoCapture.NoWaitClose();
        }

        public CameraView() {
            InitializeComponent();
            this.Closing +=new CancelEventHandler(Cleanup);

            //setup video processing
            // load the different filter blocks
            upSet = new AForgeFunctionSet("UP");
            downSet = new AForgeFunctionSet("DOWN");
            HideFilters(); //start hidden

            //start video capture
            upVideoCapture = new VideoCapture(CameraType.UpCamera);
            upVideoProcessing = new VideoProcessing(upVideoCapture);
            upVideoCapture.FrameCaptureDelegates.Add(UpCameraVideoProcessingCallback);
            upVideoProcessing.SetFunctionsList(currentUpBinding);

            downVideoCapture = new VideoCapture(CameraType.DownCamera);
            downVideoProcessing = new VideoProcessing(downVideoCapture);
            downVideoCapture.FrameCaptureDelegates.Add(DownCameraVideoProcessingCallback);
            downVideoProcessing.SetFunctionsList(currentDownBinding);


            //fill combobox // todo - restore from defaults 
            UpCamera_FilterSet.DataSource = new BindingSource { DataSource = upSet.GetNames() };
            DownCamera_FilterSet.DataSource = new BindingSource { DataSource = downSet.GetNames() };
            var videoSources = VideoCapture.GetVideoDeviceList();
            UpCam_ComboBox.DataSource = new BindingSource { DataSource =  videoSources};
            DownCam_ComboBox.DataSource = new BindingSource { DataSource = videoSources};

            //load saved values
            var s = Properties.Settings.Default;
            if (s.DownCam_index > 0 && s.DownCam_index <= videoSources.Count+1) {
                DownCam_ComboBox.SelectedIndex = s.DownCam_index-1;
                downVideoCapture.Start(DownCam_ComboBox.SelectedIndex);
            }

            if (s.UpCam_index > 0 && s.UpCam_index <= videoSources.Count+1 && s.UpCam_index != s.DownCam_index ) {
                UpCam_ComboBox.SelectedIndex = s.UpCam_index-1;
                upVideoCapture.Start(UpCam_ComboBox.SelectedIndex);
            }


            //bind editor values
            uFilter_dataGridView.DataSource = currentUpBinding;
            dFilter_dataGridView.DataSource = currentDownBinding;
            methodDataGridViewTextBoxColumn.DataSource = Enum.GetValues(typeof(AForgeMethod));
            methodDataGridViewTextBoxColumn1.DataSource = Enum.GetValues(typeof(AForgeMethod));

           
        }



        private void UpCameraVideoProcessingCallback(Bitmap frame) {
            frame = upVideoProcessing.ProcessFrame(frame); //apply filters, etc
            frame = upVideoProcessing.ApplyMarkup(frame); //draw lines, etc
            SetFrame(frame, "UpCamera");
        }

        private void DownCameraVideoProcessingCallback(Bitmap frame) {
            frame = downVideoProcessing.ProcessFrame(frame); //apply filters, etc
            frame = downVideoProcessing.ApplyMarkup(frame); //draw lines, etc
            SetFrame(frame, "DownCamera");
        }

        //xxx think this through
        public void SetDownCameraFunctionSet(string name) {
            if (name.Equals("")) {
                currentDownBinding = new BindingList<AForgeFunction>();
                return;
            }
            var s = downSet.GetSet(name);
            if (s != null) currentDownBinding = s;
        }

        public void SetUpCameraFunctionSet(string name) {
            if (name.Equals("")) {
                currentUpBinding = new BindingList<AForgeFunction>();
                return;
            }
            var s = upSet.GetSet(name);
            if (s != null) currentUpBinding = s;
        }


        private void Restart_Button_Click(object sender, EventArgs e) {
            if (DownCam_ComboBox.SelectedIndex == UpCam_ComboBox.SelectedIndex) {
                Global.Instance.DisplayText("Up cam can't be the same as downcam");
                return;
            }

            if (downVideoCapture.IsRunning()) downVideoCapture.Close();
            if (upVideoCapture.IsRunning()) upVideoCapture.Close();
            
            upVideoCapture.Start(UpCam_ComboBox.SelectedIndex);
            downVideoCapture.Start(DownCam_ComboBox.SelectedIndex);
            //save
            Properties.Settings.Default.DownCam_index = DownCam_ComboBox.SelectedIndex + 1;
            Properties.Settings.Default.UpCam_index = UpCam_ComboBox.SelectedIndex + 1;
            Properties.Settings.Default.Save();
        }

        //forward mouse clicks
        private void PictureBox_MouseClick(object sender, MouseEventArgs e) {
            //MouseEventArgs me = (MouseEventArgs)e;
            //Point coordinates = me.Location;
            var pictureBox = (PictureBox)sender;
            if (pictureBox.Equals(UpCamera_PictureBox) && (upClickDelegate != null))
                upClickDelegate(e.X - pictureBox.Size.Width / 2, pictureBox.Size.Height / 2 - e.Y);
            if (pictureBox.Equals(DownCamera_PictureBox) && (downClickDelegate != null))
                downClickDelegate(e.X - pictureBox.Size.Width / 2, pictureBox.Size.Height / 2 - e.Y);
        }

        public void HideFilters() { Width = 672; }
        public void ShowFilters() { Width = 1041; }

        //delegates to handle updating the pictureframes
        public delegate void PassBitmapDelegate(Bitmap frame, string cam);
        public void SetFrame(Bitmap frame, string cam) {
            if (this.IsDisposed) return;
            if (InvokeRequired) {
                Invoke(new PassBitmapDelegate(SetFrame), frame, cam);
                return;
            }
            switch (cam) {
                case "UpCamera":
                    if (UpCamera_PictureBox.Image != null) UpCamera_PictureBox.Image.Dispose();
                    UpCamera_PictureBox.Image = frame;
                    break;
                case "DownCamera":
                    if (DownCamera_PictureBox.Image != null) DownCamera_PictureBox.Image.Dispose();
                    DownCamera_PictureBox.Image = frame;
                    break;
            }
        }

        private void showFilters_button_Click(object sender, EventArgs e) {
            var b = (Button)sender;
            if (Width > 1000) {
                HideFilters();
                b.Text = "Show Filters";
            } else {
                ShowFilters();
                b.Text = "Hide Filters";
            }
        }


        private int SelectedRow(DataGridView grid) {
            if (grid.SelectedCells.Count == 1)
                return grid.CurrentCell.RowIndex;
            return -1;
        }

        private void FilterEditorButtonAction(object sender, EventArgs e) {
            var b = (Button)sender;
            string buttonName = b.Name;
            var target = (buttonName[0] == 'u') ? currentUpBinding : currentDownBinding;
            var row = (buttonName[0] == 'u') ? SelectedRow(uFilter_dataGridView) : SelectedRow(dFilter_dataGridView);

            switch (buttonName.Remove(0, 1)) {
                case "AddButton":
                    target.Add(new AForgeFunction());
                    break;
                case "DeleteButton":
                    if (row != -1) target.RemoveAt(row);
                    break;
                case "MoveUpButton":
                    if (row != -1) Global.MoveItem(target, row, -1);
                    break;
                case "MoveDownButton":
                    if (row != -1) Global.MoveItem(target, row, +1);
                    break;
                case "ClearButton":
                    target.Clear();
                    break;
            }

        }

                    

        /* Filter Set Management */
        /*************************/
        private void DownCamera_FilterSet_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            var list = downSet.GetSet(cb.SelectedItem.ToString());
            if (list != null) {
                currentDownBinding = list; //local copy
                dFilter_dataGridView.DataSource = currentDownBinding;
                downVideoProcessing.SetFunctionsList(currentDownBinding);
                Console.WriteLine("new downcamera set = "+currentDownBinding.GetHashCode());
            }
        }

        private void UpCamea_FilterSet_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            var list = upSet.GetSet(cb.SelectedItem.ToString());
            if (list != null) {
                currentUpBinding = list; //local copy
                uFilter_dataGridView.DataSource = currentUpBinding;
                downVideoProcessing.SetFunctionsList(currentUpBinding);
            }
        }

        private void DownCameraFilterSave_button_Click(object sender, EventArgs e) {
            downSet.Save(DownCamera_FilterSet.SelectedText, currentDownBinding);
        }

        private void UpCameraFilterSave_button_Click(object sender, EventArgs e) {
            upSet.Save(UpCamera_FilterSet.SelectedText, currentUpBinding);
        }




        // bunch of annoying functions
        #region checkboxMapping
        private void DownCameraDrawCross_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.DrawCross = ((CheckBox)sender).Checked;
        }
        private void DownCameraDrawBox_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.DrawBox = ((CheckBox)sender).Checked;
        }
        private void DownCameraDrawTicks_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.DrawSidemarks = ((CheckBox)sender).Checked;
        }
        private void DownCamFindCircles_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.FindCircles = ((CheckBox)sender).Checked;
        }
        private void DownCamFindRectangles_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.FindRectangles = ((CheckBox)sender).Checked;
        }
        private void DownCam_FindComponents_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.FindComponent = ((CheckBox)sender).Checked;
        }
        private void DownCamera_drawGrid_checkBox_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.Draw1mmGrid = ((CheckBox)sender).Checked;
        }
        private void DownCamera_FindFiducials_cb_CheckedChanged(object sender, EventArgs e) {
            downVideoProcessing.FindFiducial = ((CheckBox)sender).Checked;
        }

        private void UpCam_DrawCross_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.DrawCross = ((CheckBox)sender).Checked;
        }
        private void UpCan_DrawBox_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.DrawBox = ((CheckBox)sender).Checked;
        }
        private void UpCam_DrawDashedBox_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.DrawDashedCross = ((CheckBox)sender).Checked;
        }
        private void UpCam_FindCircles_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.FindCircles = ((CheckBox)sender).Checked;
        }
        private void UpCam_FindComponents_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.FindComponent = ((CheckBox)sender).Checked;
        }
        private void UpCam_FindRectangles_CheckedChanged(object sender, EventArgs e) {
            upVideoProcessing.FindRectangles = ((CheckBox)sender).Checked;
        }
        #endregion


        public void SetDownCameraDefaults() {
            downVideoProcessing.Reset();
        }
        public void SetUpCameraDefaults() {
            upVideoProcessing.Reset();
        }

        public void UpdateCheckboxes() {
            DownCameraDrawCross_checkBox.Checked = downVideoProcessing.DrawCross;
            DownCameraDrawBox_checkBox.Checked = downVideoProcessing.DrawBox;
            DownCameraDrawTicks_checkBox.Checked = downVideoProcessing.DrawSidemarks;
            DownCamFindCircles_checkBox.Checked = downVideoProcessing.FindCircles;
            DownCamFindRectangles_checkBox.Checked = downVideoProcessing.FindRectangles;
            DownCam_FindComponents_checkBox.Checked = downVideoProcessing.FindComponent;
            DownCamera_drawGrid_checkBox.Checked = downVideoProcessing.Draw1mmGrid;
            DownCamera_FindFiducials_cb.Checked = downVideoProcessing.FindFiducial;
            UpCam_DrawCross.Checked = upVideoProcessing.DrawCross;
            UpCan_DrawBox.Checked = upVideoProcessing.DrawBox;
            UpCam_DrawDashedBox.Checked = upVideoProcessing.DrawDashedCross;
            UpCam_FindCircles.Checked = upVideoProcessing.FindCircles;
            UpCam_FindComponents.Checked = upVideoProcessing.FindComponent;
            UpCam_FindRectangles.Checked = upVideoProcessing.FindRectangles;
        }


        // if selected, then the next click is fowarded to GetDownRGB then 
        // restored to whatever it was
        PictureBoxClickDelegate oldUpDelegate, oldDownDelegate;
        private void SelectColor_Click(object sender, EventArgs e) {
            oldDownDelegate = downClickDelegate;
            oldUpDelegate = upClickDelegate;
            downClickDelegate = GetDownRGB;
            upClickDelegate = GetUpRGB;
        }

        private void GetDownRGB(double x, double y) {
            var frame = downVideoCapture.GetFrame();
            var row = SelectedRow(dFilter_dataGridView);
            if (row != -1) {
                Color color = frame.GetPixel((int)x, (int)y);
                currentDownBinding[row].R = color.R;
                currentDownBinding[row].G = color.G;
                currentDownBinding[row].B = color.B;
            }
            downClickDelegate = oldDownDelegate;
            upClickDelegate = oldUpDelegate;
        }


        private void GetUpRGB(double x, double y) {
            var frame = downVideoCapture.GetFrame();
            var row = SelectedRow(dFilter_dataGridView);
            if (row != -1) {
                Color color = frame.GetPixel((int)x, (int)y);
                currentUpBinding[row].R = color.R;
                currentUpBinding[row].G = color.G;
                currentUpBinding[row].B = color.B;
            }
            downClickDelegate = oldDownDelegate;
            upClickDelegate = oldUpDelegate;
        }

        public void Shutdown() {
            if (upVideoCapture.IsRunning()) upVideoCapture.Close();
            if (downVideoCapture.IsRunning()) downVideoCapture.Close();
            Thread.Sleep(200);
        }

        //commit changes to combobox or checkbox instantly
        private void EndEditMode(Object sender, EventArgs e) {
            var dgv = (DataGridView)sender;
            if (dgv.CurrentCell.GetType() == typeof(DataGridViewComboBoxCell) ||
                dgv.CurrentCell.GetType() == typeof(DataGridViewCheckBoxCell)) {
                if (dgv.IsCurrentCellDirty) dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }



        private void dFilter_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            e.Cancel = true;
        }

     /*   private void DownCam_ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Properties.Settings.Default.DownCam_index = DownCam_ComboBox.SelectedIndex+1;
            Properties.Settings.Default.Save();
        }

        private void UpCam_ComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Properties.Settings.Default.UpCam_index = UpCam_ComboBox.SelectedIndex+1;
            Properties.Settings.Default.Save();
        }*/



    }
}
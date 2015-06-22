using System.ComponentModel;
using System.Windows.Forms;

namespace LitePlacer {
    partial class CameraView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.DownCamera_PictureBox = new System.Windows.Forms.PictureBox();
            this.UpCamera_PictureBox = new System.Windows.Forms.PictureBox();
            this.UpCam_ComboBox = new System.Windows.Forms.ComboBox();
            this.DownCam_ComboBox = new System.Windows.Forms.ComboBox();
            this.Restart_Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dFilter_dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uFilter_dataGridView = new System.Windows.Forms.DataGridView();
            this.parameter_double = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DownCamera_drawGrid_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawCross_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCamera_FindFiducials_cb = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawBox_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawTicks_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCamFindCircles_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCam_FindComponents_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCamFindRectangles_checkBox = new System.Windows.Forms.CheckBox();
            this.UpCam_FindComponents = new System.Windows.Forms.CheckBox();
            this.UpCam_FindRectangles = new System.Windows.Forms.CheckBox();
            this.UpCam_FindCircles = new System.Windows.Forms.CheckBox();
            this.UpCam_DrawDashedBox = new System.Windows.Forms.CheckBox();
            this.UpCan_DrawBox = new System.Windows.Forms.CheckBox();
            this.UpCam_DrawCross = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dAddButton = new System.Windows.Forms.Button();
            this.dDeleteButton = new System.Windows.Forms.Button();
            this.dMoveUpButton = new System.Windows.Forms.Button();
            this.dMoveDownButton = new System.Windows.Forms.Button();
            this.dClearButton = new System.Windows.Forms.Button();
            this.uClearButton = new System.Windows.Forms.Button();
            this.uMoveDownButton = new System.Windows.Forms.Button();
            this.uMoveUpButton = new System.Windows.Forms.Button();
            this.uDeleteButton = new System.Windows.Forms.Button();
            this.uAddButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.showFilters_button = new System.Windows.Forms.Button();
            this.UpCamera_FilterSet = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DownCamera_FilterSet = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.DownCameraFilterSave_button = new System.Windows.Forms.Button();
            this.UpCameraFilterSave_button = new System.Windows.Forms.Button();
            this.SelectColor = new System.Windows.Forms.Button();
            this.enabledDataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.methodDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.rDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aForgeFunctionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.methodDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.rDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DownCamera_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpCamera_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dFilter_dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uFilter_dataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aForgeFunctionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // DownCamera_PictureBox
            // 
            this.DownCamera_PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DownCamera_PictureBox.Location = new System.Drawing.Point(12, 16);
            this.DownCamera_PictureBox.Name = "DownCamera_PictureBox";
            this.DownCamera_PictureBox.Size = new System.Drawing.Size(640, 480);
            this.DownCamera_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DownCamera_PictureBox.TabIndex = 0;
            this.DownCamera_PictureBox.TabStop = false;
            this.DownCamera_PictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseClick);
            // 
            // UpCamera_PictureBox
            // 
            this.UpCamera_PictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UpCamera_PictureBox.Location = new System.Drawing.Point(12, 516);
            this.UpCamera_PictureBox.Name = "UpCamera_PictureBox";
            this.UpCamera_PictureBox.Size = new System.Drawing.Size(320, 240);
            this.UpCamera_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.UpCamera_PictureBox.TabIndex = 1;
            this.UpCamera_PictureBox.TabStop = false;
            this.UpCamera_PictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseClick);
            // 
            // UpCam_ComboBox
            // 
            this.UpCam_ComboBox.FormattingEnabled = true;
            this.UpCam_ComboBox.Location = new System.Drawing.Point(413, 726);
            this.UpCam_ComboBox.Name = "UpCam_ComboBox";
            this.UpCam_ComboBox.Size = new System.Drawing.Size(166, 21);
            this.UpCam_ComboBox.TabIndex = 2;
          //  this.UpCam_ComboBox.SelectedIndexChanged += new System.EventHandler(this.UpCam_ComboBox_SelectedIndexChanged);
            // 
            // DownCam_ComboBox
            // 
            this.DownCam_ComboBox.FormattingEnabled = true;
            this.DownCam_ComboBox.Location = new System.Drawing.Point(413, 698);
            this.DownCam_ComboBox.Name = "DownCam_ComboBox";
            this.DownCam_ComboBox.Size = new System.Drawing.Size(166, 21);
            this.DownCam_ComboBox.TabIndex = 3;
           // this.DownCam_ComboBox.SelectedIndexChanged += new System.EventHandler(this.DownCam_ComboBox_SelectedIndexChanged);
            // 
            // Restart_Button
            // 
            this.Restart_Button.Location = new System.Drawing.Point(590, 697);
            this.Restart_Button.Name = "Restart_Button";
            this.Restart_Button.Size = new System.Drawing.Size(62, 48);
            this.Restart_Button.TabIndex = 4;
            this.Restart_Button.Text = "(Re)Start Cameras";
            this.Restart_Button.UseVisualStyleBackColor = true;
            this.Restart_Button.Click += new System.EventHandler(this.Restart_Button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(365, 729);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "UpCam";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(351, 701);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "DownCam";
            // 
            // dFilter_dataGridView
            // 
            this.dFilter_dataGridView.AllowUserToAddRows = false;
            this.dFilter_dataGridView.AutoGenerateColumns = false;
            this.dFilter_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dFilter_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enabledDataGridViewCheckBoxColumn,
            this.methodDataGridViewTextBoxColumn,
            this.dataGridViewTextBoxColumn1,
            this.rDataGridViewTextBoxColumn,
            this.gDataGridViewTextBoxColumn,
            this.bDataGridViewTextBoxColumn});
            this.dFilter_dataGridView.DataSource = this.aForgeFunctionBindingSource;
            this.dFilter_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dFilter_dataGridView.Location = new System.Drawing.Point(661, 47);
            this.dFilter_dataGridView.MultiSelect = false;
            this.dFilter_dataGridView.Name = "dFilter_dataGridView";
            this.dFilter_dataGridView.RowHeadersVisible = false;
            this.dFilter_dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dFilter_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dFilter_dataGridView.Size = new System.Drawing.Size(355, 200);
            this.dFilter_dataGridView.TabIndex = 45;
            this.dFilter_dataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this.EndEditMode);
            this.dFilter_dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dFilter_dataGridView_DataError);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "parameter_double";
            this.dataGridViewTextBoxColumn1.HeaderText = "Value";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 50;
            // 
            // uFilter_dataGridView
            // 
            this.uFilter_dataGridView.AllowUserToAddRows = false;
            this.uFilter_dataGridView.AutoGenerateColumns = false;
            this.uFilter_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.uFilter_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.enabledDataGridViewCheckBoxColumn1,
            this.methodDataGridViewTextBoxColumn1,
            this.parameter_double,
            this.rDataGridViewTextBoxColumn1,
            this.gDataGridViewTextBoxColumn1,
            this.bDataGridViewTextBoxColumn1});
            this.uFilter_dataGridView.DataSource = this.aForgeFunctionBindingSource;
            this.uFilter_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.uFilter_dataGridView.Location = new System.Drawing.Point(658, 402);
            this.uFilter_dataGridView.MultiSelect = false;
            this.uFilter_dataGridView.Name = "uFilter_dataGridView";
            this.uFilter_dataGridView.RowHeadersVisible = false;
            this.uFilter_dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uFilter_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.uFilter_dataGridView.Size = new System.Drawing.Size(355, 200);
            this.uFilter_dataGridView.TabIndex = 46;
            // 
            // parameter_double
            // 
            this.parameter_double.DataPropertyName = "parameter_double";
            this.parameter_double.HeaderText = "Value";
            this.parameter_double.Name = "parameter_double";
            this.parameter_double.Width = 50;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DownCamera_drawGrid_checkBox);
            this.groupBox1.Controls.Add(this.DownCameraDrawCross_checkBox);
            this.groupBox1.Controls.Add(this.DownCamera_FindFiducials_cb);
            this.groupBox1.Controls.Add(this.DownCameraDrawBox_checkBox);
            this.groupBox1.Controls.Add(this.DownCameraDrawTicks_checkBox);
            this.groupBox1.Controls.Add(this.DownCamFindCircles_checkBox);
            this.groupBox1.Controls.Add(this.DownCam_FindComponents_checkBox);
            this.groupBox1.Controls.Add(this.DownCamFindRectangles_checkBox);
            this.groupBox1.Location = new System.Drawing.Point(664, 278);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(355, 90);
            this.groupBox1.TabIndex = 135;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Down Camera";
            // 
            // DownCamera_drawGrid_checkBox
            // 
            this.DownCamera_drawGrid_checkBox.AutoSize = true;
            this.DownCamera_drawGrid_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCamera_drawGrid_checkBox.Location = new System.Drawing.Point(257, 19);
            this.DownCamera_drawGrid_checkBox.Name = "DownCamera_drawGrid_checkBox";
            this.DownCamera_drawGrid_checkBox.Size = new System.Drawing.Size(98, 17);
            this.DownCamera_drawGrid_checkBox.TabIndex = 152;
            this.DownCamera_drawGrid_checkBox.Text = "Draw 1mm Grid";
            this.DownCamera_drawGrid_checkBox.UseVisualStyleBackColor = true;
            this.DownCamera_drawGrid_checkBox.CheckedChanged += new System.EventHandler(this.DownCamera_drawGrid_checkBox_CheckedChanged);
            // 
            // DownCameraDrawCross_checkBox
            // 
            this.DownCameraDrawCross_checkBox.AutoSize = true;
            this.DownCameraDrawCross_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraDrawCross_checkBox.Location = new System.Drawing.Point(6, 19);
            this.DownCameraDrawCross_checkBox.Name = "DownCameraDrawCross_checkBox";
            this.DownCameraDrawCross_checkBox.Size = new System.Drawing.Size(80, 17);
            this.DownCameraDrawCross_checkBox.TabIndex = 145;
            this.DownCameraDrawCross_checkBox.Text = "Draw Cross";
            this.DownCameraDrawCross_checkBox.UseVisualStyleBackColor = true;
            this.DownCameraDrawCross_checkBox.CheckedChanged += new System.EventHandler(this.DownCameraDrawCross_checkBox_CheckedChanged);
            // 
            // DownCamera_FindFiducials_cb
            // 
            this.DownCamera_FindFiducials_cb.AutoSize = true;
            this.DownCamera_FindFiducials_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCamera_FindFiducials_cb.Location = new System.Drawing.Point(257, 42);
            this.DownCamera_FindFiducials_cb.Name = "DownCamera_FindFiducials_cb";
            this.DownCamera_FindFiducials_cb.Size = new System.Drawing.Size(90, 17);
            this.DownCamera_FindFiducials_cb.TabIndex = 151;
            this.DownCamera_FindFiducials_cb.Text = "Find Fiducials";
            this.DownCamera_FindFiducials_cb.UseVisualStyleBackColor = true;
            this.DownCamera_FindFiducials_cb.CheckedChanged += new System.EventHandler(this.DownCamera_FindFiducials_cb_CheckedChanged);
            // 
            // DownCameraDrawBox_checkBox
            // 
            this.DownCameraDrawBox_checkBox.AutoSize = true;
            this.DownCameraDrawBox_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraDrawBox_checkBox.Location = new System.Drawing.Point(6, 42);
            this.DownCameraDrawBox_checkBox.Name = "DownCameraDrawBox_checkBox";
            this.DownCameraDrawBox_checkBox.Size = new System.Drawing.Size(72, 17);
            this.DownCameraDrawBox_checkBox.TabIndex = 146;
            this.DownCameraDrawBox_checkBox.Text = "Draw Box";
            this.DownCameraDrawBox_checkBox.UseVisualStyleBackColor = true;
            this.DownCameraDrawBox_checkBox.CheckedChanged += new System.EventHandler(this.DownCameraDrawBox_checkBox_CheckedChanged);
            // 
            // DownCameraDrawTicks_checkBox
            // 
            this.DownCameraDrawTicks_checkBox.AutoSize = true;
            this.DownCameraDrawTicks_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCameraDrawTicks_checkBox.Location = new System.Drawing.Point(6, 65);
            this.DownCameraDrawTicks_checkBox.Name = "DownCameraDrawTicks_checkBox";
            this.DownCameraDrawTicks_checkBox.Size = new System.Drawing.Size(103, 17);
            this.DownCameraDrawTicks_checkBox.TabIndex = 150;
            this.DownCameraDrawTicks_checkBox.Text = "Draw Tickmarks";
            this.DownCameraDrawTicks_checkBox.UseVisualStyleBackColor = true;
            this.DownCameraDrawTicks_checkBox.CheckedChanged += new System.EventHandler(this.DownCameraDrawTicks_checkBox_CheckedChanged);
            // 
            // DownCamFindCircles_checkBox
            // 
            this.DownCamFindCircles_checkBox.AutoSize = true;
            this.DownCamFindCircles_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCamFindCircles_checkBox.Location = new System.Drawing.Point(144, 19);
            this.DownCamFindCircles_checkBox.Name = "DownCamFindCircles_checkBox";
            this.DownCamFindCircles_checkBox.Size = new System.Drawing.Size(80, 17);
            this.DownCamFindCircles_checkBox.TabIndex = 147;
            this.DownCamFindCircles_checkBox.Text = "Find Circles";
            this.DownCamFindCircles_checkBox.UseVisualStyleBackColor = true;
            this.DownCamFindCircles_checkBox.CheckedChanged += new System.EventHandler(this.DownCamFindCircles_checkBox_CheckedChanged);
            // 
            // DownCam_FindComponents_checkBox
            // 
            this.DownCam_FindComponents_checkBox.AutoSize = true;
            this.DownCam_FindComponents_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCam_FindComponents_checkBox.Location = new System.Drawing.Point(144, 65);
            this.DownCam_FindComponents_checkBox.Name = "DownCam_FindComponents_checkBox";
            this.DownCam_FindComponents_checkBox.Size = new System.Drawing.Size(108, 17);
            this.DownCam_FindComponents_checkBox.TabIndex = 149;
            this.DownCam_FindComponents_checkBox.Text = "Find Components";
            this.DownCam_FindComponents_checkBox.UseVisualStyleBackColor = true;
            this.DownCam_FindComponents_checkBox.CheckedChanged += new System.EventHandler(this.DownCam_FindComponents_checkBox_CheckedChanged);
            // 
            // DownCamFindRectangles_checkBox
            // 
            this.DownCamFindRectangles_checkBox.AutoSize = true;
            this.DownCamFindRectangles_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownCamFindRectangles_checkBox.Location = new System.Drawing.Point(144, 42);
            this.DownCamFindRectangles_checkBox.Name = "DownCamFindRectangles_checkBox";
            this.DownCamFindRectangles_checkBox.Size = new System.Drawing.Size(103, 17);
            this.DownCamFindRectangles_checkBox.TabIndex = 148;
            this.DownCamFindRectangles_checkBox.Text = "Find Rectangles";
            this.DownCamFindRectangles_checkBox.UseVisualStyleBackColor = true;
            this.DownCamFindRectangles_checkBox.CheckedChanged += new System.EventHandler(this.DownCamFindRectangles_checkBox_CheckedChanged);
            // 
            // UpCam_FindComponents
            // 
            this.UpCam_FindComponents.AutoSize = true;
            this.UpCam_FindComponents.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCam_FindComponents.Location = new System.Drawing.Point(144, 42);
            this.UpCam_FindComponents.Name = "UpCam_FindComponents";
            this.UpCam_FindComponents.Size = new System.Drawing.Size(108, 17);
            this.UpCam_FindComponents.TabIndex = 76;
            this.UpCam_FindComponents.Text = "Find Components";
            this.UpCam_FindComponents.UseVisualStyleBackColor = true;
            this.UpCam_FindComponents.CheckedChanged += new System.EventHandler(this.UpCam_FindComponents_CheckedChanged);
            // 
            // UpCam_FindRectangles
            // 
            this.UpCam_FindRectangles.AutoSize = true;
            this.UpCam_FindRectangles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCam_FindRectangles.Location = new System.Drawing.Point(144, 65);
            this.UpCam_FindRectangles.Name = "UpCam_FindRectangles";
            this.UpCam_FindRectangles.Size = new System.Drawing.Size(103, 17);
            this.UpCam_FindRectangles.TabIndex = 75;
            this.UpCam_FindRectangles.Text = "Find Rectangles";
            this.UpCam_FindRectangles.UseVisualStyleBackColor = true;
            this.UpCam_FindRectangles.CheckedChanged += new System.EventHandler(this.UpCam_FindRectangles_CheckedChanged);
            // 
            // UpCam_FindCircles
            // 
            this.UpCam_FindCircles.AutoSize = true;
            this.UpCam_FindCircles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCam_FindCircles.Location = new System.Drawing.Point(144, 19);
            this.UpCam_FindCircles.Name = "UpCam_FindCircles";
            this.UpCam_FindCircles.Size = new System.Drawing.Size(80, 17);
            this.UpCam_FindCircles.TabIndex = 74;
            this.UpCam_FindCircles.Text = "Find Circles";
            this.UpCam_FindCircles.UseVisualStyleBackColor = true;
            this.UpCam_FindCircles.CheckedChanged += new System.EventHandler(this.UpCam_FindCircles_CheckedChanged);
            // 
            // UpCam_DrawDashedBox
            // 
            this.UpCam_DrawDashedBox.AutoSize = true;
            this.UpCam_DrawDashedBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCam_DrawDashedBox.Location = new System.Drawing.Point(6, 65);
            this.UpCam_DrawDashedBox.Name = "UpCam_DrawDashedBox";
            this.UpCam_DrawDashedBox.Size = new System.Drawing.Size(120, 17);
            this.UpCam_DrawDashedBox.TabIndex = 73;
            this.UpCam_DrawDashedBox.Text = "Draw Dashed Cross";
            this.UpCam_DrawDashedBox.UseVisualStyleBackColor = true;
            this.UpCam_DrawDashedBox.CheckedChanged += new System.EventHandler(this.UpCam_DrawDashedBox_CheckedChanged);
            // 
            // UpCan_DrawBox
            // 
            this.UpCan_DrawBox.AutoSize = true;
            this.UpCan_DrawBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCan_DrawBox.Location = new System.Drawing.Point(6, 42);
            this.UpCan_DrawBox.Name = "UpCan_DrawBox";
            this.UpCan_DrawBox.Size = new System.Drawing.Size(72, 17);
            this.UpCan_DrawBox.TabIndex = 72;
            this.UpCan_DrawBox.Text = "Draw Box";
            this.UpCan_DrawBox.UseVisualStyleBackColor = true;
            this.UpCan_DrawBox.CheckedChanged += new System.EventHandler(this.UpCan_DrawBox_CheckedChanged);
            // 
            // UpCam_DrawCross
            // 
            this.UpCam_DrawCross.AutoSize = true;
            this.UpCam_DrawCross.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpCam_DrawCross.Location = new System.Drawing.Point(6, 19);
            this.UpCam_DrawCross.Name = "UpCam_DrawCross";
            this.UpCam_DrawCross.Size = new System.Drawing.Size(80, 17);
            this.UpCam_DrawCross.TabIndex = 71;
            this.UpCam_DrawCross.Text = "Draw Cross";
            this.UpCam_DrawCross.UseVisualStyleBackColor = true;
            this.UpCam_DrawCross.CheckedChanged += new System.EventHandler(this.UpCam_DrawCross_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.UpCam_FindComponents);
            this.groupBox2.Controls.Add(this.UpCam_FindRectangles);
            this.groupBox2.Controls.Add(this.UpCam_FindCircles);
            this.groupBox2.Controls.Add(this.UpCam_DrawDashedBox);
            this.groupBox2.Controls.Add(this.UpCan_DrawBox);
            this.groupBox2.Controls.Add(this.UpCam_DrawCross);
            this.groupBox2.Location = new System.Drawing.Point(658, 635);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(355, 86);
            this.groupBox2.TabIndex = 135;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Up Camera";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(661, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 136;
            this.label3.Text = "Down Camera";
            // 
            // dAddButton
            // 
            this.dAddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dAddButton.Location = new System.Drawing.Point(661, 252);
            this.dAddButton.Name = "dAddButton";
            this.dAddButton.Size = new System.Drawing.Size(60, 23);
            this.dAddButton.TabIndex = 144;
            this.dAddButton.Text = "Add";
            this.dAddButton.UseVisualStyleBackColor = true;
            this.dAddButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // dDeleteButton
            // 
            this.dDeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dDeleteButton.Location = new System.Drawing.Point(734, 252);
            this.dDeleteButton.Name = "dDeleteButton";
            this.dDeleteButton.Size = new System.Drawing.Size(60, 23);
            this.dDeleteButton.TabIndex = 145;
            this.dDeleteButton.Text = "Delete";
            this.dDeleteButton.UseVisualStyleBackColor = true;
            this.dDeleteButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // dMoveUpButton
            // 
            this.dMoveUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dMoveUpButton.Location = new System.Drawing.Point(807, 252);
            this.dMoveUpButton.Name = "dMoveUpButton";
            this.dMoveUpButton.Size = new System.Drawing.Size(60, 23);
            this.dMoveUpButton.TabIndex = 146;
            this.dMoveUpButton.Text = "Move Up";
            this.dMoveUpButton.UseVisualStyleBackColor = true;
            this.dMoveUpButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // dMoveDownButton
            // 
            this.dMoveDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dMoveDownButton.Location = new System.Drawing.Point(880, 252);
            this.dMoveDownButton.Name = "dMoveDownButton";
            this.dMoveDownButton.Size = new System.Drawing.Size(60, 23);
            this.dMoveDownButton.TabIndex = 147;
            this.dMoveDownButton.Text = "Down";
            this.dMoveDownButton.UseVisualStyleBackColor = true;
            this.dMoveDownButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // dClearButton
            // 
            this.dClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dClearButton.Location = new System.Drawing.Point(953, 252);
            this.dClearButton.Name = "dClearButton";
            this.dClearButton.Size = new System.Drawing.Size(60, 23);
            this.dClearButton.TabIndex = 148;
            this.dClearButton.Text = "Clear";
            this.dClearButton.UseVisualStyleBackColor = true;
            this.dClearButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // uClearButton
            // 
            this.uClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uClearButton.Location = new System.Drawing.Point(950, 607);
            this.uClearButton.Name = "uClearButton";
            this.uClearButton.Size = new System.Drawing.Size(60, 23);
            this.uClearButton.TabIndex = 153;
            this.uClearButton.Text = "Clear";
            this.uClearButton.UseVisualStyleBackColor = true;
            this.uClearButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // uMoveDownButton
            // 
            this.uMoveDownButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uMoveDownButton.Location = new System.Drawing.Point(877, 607);
            this.uMoveDownButton.Name = "uMoveDownButton";
            this.uMoveDownButton.Size = new System.Drawing.Size(60, 23);
            this.uMoveDownButton.TabIndex = 152;
            this.uMoveDownButton.Text = "Down";
            this.uMoveDownButton.UseVisualStyleBackColor = true;
            this.uMoveDownButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // uMoveUpButton
            // 
            this.uMoveUpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uMoveUpButton.Location = new System.Drawing.Point(804, 607);
            this.uMoveUpButton.Name = "uMoveUpButton";
            this.uMoveUpButton.Size = new System.Drawing.Size(60, 23);
            this.uMoveUpButton.TabIndex = 151;
            this.uMoveUpButton.Text = "Move Up";
            this.uMoveUpButton.UseVisualStyleBackColor = true;
            this.uMoveUpButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // uDeleteButton
            // 
            this.uDeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uDeleteButton.Location = new System.Drawing.Point(731, 607);
            this.uDeleteButton.Name = "uDeleteButton";
            this.uDeleteButton.Size = new System.Drawing.Size(60, 23);
            this.uDeleteButton.TabIndex = 150;
            this.uDeleteButton.Text = "Delete";
            this.uDeleteButton.UseVisualStyleBackColor = true;
            this.uDeleteButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // uAddButton
            // 
            this.uAddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uAddButton.Location = new System.Drawing.Point(658, 607);
            this.uAddButton.Name = "uAddButton";
            this.uAddButton.Size = new System.Drawing.Size(60, 23);
            this.uAddButton.TabIndex = 149;
            this.uAddButton.Text = "Add";
            this.uAddButton.UseVisualStyleBackColor = true;
            this.uAddButton.Click += new System.EventHandler(this.FilterEditorButtonAction);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(658, 386);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 154;
            this.label4.Text = "Up Camera";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 500);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 155;
            this.label5.Text = "Up Camera";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 156;
            this.label6.Text = "Down Camera";
            // 
            // showFilters_button
            // 
            this.showFilters_button.Location = new System.Drawing.Point(574, 502);
            this.showFilters_button.Name = "showFilters_button";
            this.showFilters_button.Size = new System.Drawing.Size(78, 45);
            this.showFilters_button.TabIndex = 157;
            this.showFilters_button.Text = "Show Filters";
            this.showFilters_button.UseVisualStyleBackColor = true;
            this.showFilters_button.Click += new System.EventHandler(this.showFilters_button_Click);
            // 
            // UpCamera_FilterSet
            // 
            this.UpCamera_FilterSet.FormattingEnabled = true;
            this.UpCamera_FilterSet.Location = new System.Drawing.Point(809, 378);
            this.UpCamera_FilterSet.Name = "UpCamera_FilterSet";
            this.UpCamera_FilterSet.Size = new System.Drawing.Size(152, 21);
            this.UpCamera_FilterSet.TabIndex = 158;
            this.UpCamera_FilterSet.SelectedIndexChanged += new System.EventHandler(this.UpCamea_FilterSet_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(746, 386);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 159;
            this.label7.Text = "Active Filter";
            // 
            // DownCamera_FilterSet
            // 
            this.DownCamera_FilterSet.FormattingEnabled = true;
            this.DownCamera_FilterSet.Location = new System.Drawing.Point(810, 23);
            this.DownCamera_FilterSet.Name = "DownCamera_FilterSet";
            this.DownCamera_FilterSet.Size = new System.Drawing.Size(152, 21);
            this.DownCamera_FilterSet.TabIndex = 160;
            this.DownCamera_FilterSet.SelectedIndexChanged += new System.EventHandler(this.DownCamera_FilterSet_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(746, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 13);
            this.label8.TabIndex = 161;
            this.label8.Text = "Active Filter";
            // 
            // DownCameraFilterSave_button
            // 
            this.DownCameraFilterSave_button.Location = new System.Drawing.Point(966, 22);
            this.DownCameraFilterSave_button.Name = "DownCameraFilterSave_button";
            this.DownCameraFilterSave_button.Size = new System.Drawing.Size(50, 23);
            this.DownCameraFilterSave_button.TabIndex = 162;
            this.DownCameraFilterSave_button.Text = "Save";
            this.DownCameraFilterSave_button.UseVisualStyleBackColor = true;
            this.DownCameraFilterSave_button.Click += new System.EventHandler(this.DownCameraFilterSave_button_Click);
            // 
            // UpCameraFilterSave_button
            // 
            this.UpCameraFilterSave_button.Location = new System.Drawing.Point(963, 377);
            this.UpCameraFilterSave_button.Name = "UpCameraFilterSave_button";
            this.UpCameraFilterSave_button.Size = new System.Drawing.Size(50, 23);
            this.UpCameraFilterSave_button.TabIndex = 163;
            this.UpCameraFilterSave_button.Text = "Save";
            this.UpCameraFilterSave_button.UseVisualStyleBackColor = true;
            this.UpCameraFilterSave_button.Click += new System.EventHandler(this.UpCameraFilterSave_button_Click);
            // 
            // SelectColor
            // 
            this.SelectColor.Location = new System.Drawing.Point(491, 502);
            this.SelectColor.Name = "SelectColor";
            this.SelectColor.Size = new System.Drawing.Size(77, 45);
            this.SelectColor.TabIndex = 164;
            this.SelectColor.Text = "Select Color";
            this.SelectColor.UseVisualStyleBackColor = true;
            this.SelectColor.Click += new System.EventHandler(this.SelectColor_Click);
            // 
            // enabledDataGridViewCheckBoxColumn1
            // 
            this.enabledDataGridViewCheckBoxColumn1.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn1.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn1.Name = "enabledDataGridViewCheckBoxColumn1";
            this.enabledDataGridViewCheckBoxColumn1.Width = 50;
            // 
            // methodDataGridViewTextBoxColumn1
            // 
            this.methodDataGridViewTextBoxColumn1.DataPropertyName = "Method";
            this.methodDataGridViewTextBoxColumn1.HeaderText = "Method";
            this.methodDataGridViewTextBoxColumn1.Name = "methodDataGridViewTextBoxColumn1";
            this.methodDataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.methodDataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.methodDataGridViewTextBoxColumn1.Width = 150;
            // 
            // rDataGridViewTextBoxColumn1
            // 
            this.rDataGridViewTextBoxColumn1.DataPropertyName = "R";
            this.rDataGridViewTextBoxColumn1.HeaderText = "R";
            this.rDataGridViewTextBoxColumn1.Name = "rDataGridViewTextBoxColumn1";
            this.rDataGridViewTextBoxColumn1.Width = 35;
            // 
            // gDataGridViewTextBoxColumn1
            // 
            this.gDataGridViewTextBoxColumn1.DataPropertyName = "G";
            this.gDataGridViewTextBoxColumn1.HeaderText = "G";
            this.gDataGridViewTextBoxColumn1.Name = "gDataGridViewTextBoxColumn1";
            this.gDataGridViewTextBoxColumn1.Width = 35;
            // 
            // bDataGridViewTextBoxColumn1
            // 
            this.bDataGridViewTextBoxColumn1.DataPropertyName = "B";
            this.bDataGridViewTextBoxColumn1.HeaderText = "B";
            this.bDataGridViewTextBoxColumn1.Name = "bDataGridViewTextBoxColumn1";
            this.bDataGridViewTextBoxColumn1.Width = 35;
            // 
            // aForgeFunctionBindingSource
            // 
            this.aForgeFunctionBindingSource.DataSource = typeof(LitePlacer.AForgeFunction);
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            this.enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            this.enabledDataGridViewCheckBoxColumn.Width = 50;
            // 
            // methodDataGridViewTextBoxColumn
            // 
            this.methodDataGridViewTextBoxColumn.DataPropertyName = "Method";
            this.methodDataGridViewTextBoxColumn.HeaderText = "Method";
            this.methodDataGridViewTextBoxColumn.Name = "methodDataGridViewTextBoxColumn";
            this.methodDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.methodDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.methodDataGridViewTextBoxColumn.Width = 150;
            // 
            // rDataGridViewTextBoxColumn
            // 
            this.rDataGridViewTextBoxColumn.DataPropertyName = "R";
            this.rDataGridViewTextBoxColumn.HeaderText = "R";
            this.rDataGridViewTextBoxColumn.Name = "rDataGridViewTextBoxColumn";
            this.rDataGridViewTextBoxColumn.Width = 35;
            // 
            // gDataGridViewTextBoxColumn
            // 
            this.gDataGridViewTextBoxColumn.DataPropertyName = "G";
            this.gDataGridViewTextBoxColumn.HeaderText = "G";
            this.gDataGridViewTextBoxColumn.Name = "gDataGridViewTextBoxColumn";
            this.gDataGridViewTextBoxColumn.Width = 35;
            // 
            // bDataGridViewTextBoxColumn
            // 
            this.bDataGridViewTextBoxColumn.DataPropertyName = "B";
            this.bDataGridViewTextBoxColumn.HeaderText = "B";
            this.bDataGridViewTextBoxColumn.Name = "bDataGridViewTextBoxColumn";
            this.bDataGridViewTextBoxColumn.Width = 35;
            // 
            // CameraView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1025, 762);
            this.Controls.Add(this.SelectColor);
            this.Controls.Add(this.UpCameraFilterSave_button);
            this.Controls.Add(this.DownCameraFilterSave_button);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.DownCamera_FilterSet);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.UpCamera_FilterSet);
            this.Controls.Add(this.showFilters_button);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.uClearButton);
            this.Controls.Add(this.uMoveDownButton);
            this.Controls.Add(this.uMoveUpButton);
            this.Controls.Add(this.uDeleteButton);
            this.Controls.Add(this.uAddButton);
            this.Controls.Add(this.dClearButton);
            this.Controls.Add(this.dMoveDownButton);
            this.Controls.Add(this.dMoveUpButton);
            this.Controls.Add(this.dDeleteButton);
            this.Controls.Add(this.dAddButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.uFilter_dataGridView);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.dFilter_dataGridView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Restart_Button);
            this.Controls.Add(this.DownCam_ComboBox);
            this.Controls.Add(this.UpCam_ComboBox);
            this.Controls.Add(this.UpCamera_PictureBox);
            this.Controls.Add(this.DownCamera_PictureBox);
            this.Name = "CameraView";
            this.Text = "CameraView";
            ((System.ComponentModel.ISupportInitialize)(this.DownCamera_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpCamera_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dFilter_dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uFilter_dataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aForgeFunctionBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox DownCamera_PictureBox;
        private PictureBox UpCamera_PictureBox;
        private ComboBox UpCam_ComboBox;
        private ComboBox DownCam_ComboBox;
        private Button Restart_Button;
        private Label label1;
        private Label label2;
        private DataGridView dFilter_dataGridView;
        private DataGridView uFilter_dataGridView;
        private GroupBox groupBox1;
        private CheckBox DownCamera_drawGrid_checkBox;
        private CheckBox DownCameraDrawCross_checkBox;
        private CheckBox DownCamera_FindFiducials_cb;
        private CheckBox DownCameraDrawBox_checkBox;
        private CheckBox DownCameraDrawTicks_checkBox;
        private CheckBox DownCamFindCircles_checkBox;
        private CheckBox DownCam_FindComponents_checkBox;
        private CheckBox DownCamFindRectangles_checkBox;
        private CheckBox UpCam_FindComponents;
        private CheckBox UpCam_FindRectangles;
        private CheckBox UpCam_FindCircles;
        private CheckBox UpCam_DrawDashedBox;
        private CheckBox UpCan_DrawBox;
        private CheckBox UpCam_DrawCross;
        private GroupBox groupBox2;
        private Label label3;
        private Button dAddButton;
        private Button dDeleteButton;
        private Button dMoveUpButton;
        private Button dMoveDownButton;
        private Button dClearButton;
        private Button uClearButton;
        private Button uMoveDownButton;
        private Button uMoveUpButton;
        private Button uDeleteButton;
        private Button uAddButton;
        private Label label4;
        private BindingSource aForgeFunctionBindingSource;
        private Label label5;
        private Label label6;
        private Button showFilters_button;
        private ComboBox UpCamera_FilterSet;
        private Label label7;
        private ComboBox DownCamera_FilterSet;
        private Label label8;
        private Button DownCameraFilterSave_button;
        private Button UpCameraFilterSave_button;
        private Button SelectColor;
        private DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private DataGridViewComboBoxColumn methodDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn rDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn gDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn bDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn1;
        private DataGridViewComboBoxColumn methodDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn parameter_double;
        private DataGridViewTextBoxColumn rDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn gDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn bDataGridViewTextBoxColumn1;
    }
}
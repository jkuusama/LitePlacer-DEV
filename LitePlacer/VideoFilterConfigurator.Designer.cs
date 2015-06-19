namespace LitePlacer {
    partial class VideoFilterConfigurator {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.Display_dataGridView = new System.Windows.Forms.DataGridView();
            this.methodDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.enabledDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.parameterintDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.aForgeFunctionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.CamFunctionsClear_button = new System.Windows.Forms.Button();
            this.CamFunctionDown_button = new System.Windows.Forms.Button();
            this.CamFunctionUp_button = new System.Windows.Forms.Button();
            this.DeleteCamFunction_button = new System.Windows.Forms.Button();
            this.AddCamFunction_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.drawGrid_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawCross_checkBox = new System.Windows.Forms.CheckBox();
            this.FindFiducials_cb = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawBox_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCameraDrawTicks_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCamFindCircles_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCam_FindComponents_checkBox = new System.Windows.Forms.CheckBox();
            this.DownCamFindRectangles_checkBox = new System.Windows.Forms.CheckBox();
            this.FilterSetSelectorComboBox = new System.Windows.Forms.ComboBox();
            this.button_saveSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Display_dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aForgeFunctionBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Display_dataGridView
            // 
            this.Display_dataGridView.AllowUserToAddRows = false;
            this.Display_dataGridView.AutoGenerateColumns = false;
            this.Display_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Display_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.methodDataGridViewTextBoxColumn,
            this.enabledDataGridViewCheckBoxColumn,
            this.parameterintDataGridViewTextBoxColumn,
            this.rDataGridViewTextBoxColumn,
            this.gDataGridViewTextBoxColumn,
            this.bDataGridViewTextBoxColumn});
            this.Display_dataGridView.DataSource = this.aForgeFunctionBindingSource;
            this.Display_dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Display_dataGridView.Location = new System.Drawing.Point(107, 48);
            this.Display_dataGridView.Name = "Display_dataGridView";
            this.Display_dataGridView.RowHeadersVisible = false;
            this.Display_dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Display_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Display_dataGridView.Size = new System.Drawing.Size(478, 139);
            this.Display_dataGridView.TabIndex = 44;
            // 
            // methodDataGridViewTextBoxColumn
            // 
            this.methodDataGridViewTextBoxColumn.DataPropertyName = "Method";
            this.methodDataGridViewTextBoxColumn.HeaderText = "Method";
            this.methodDataGridViewTextBoxColumn.Name = "methodDataGridViewTextBoxColumn";
            this.methodDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.methodDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // enabledDataGridViewCheckBoxColumn
            // 
            this.enabledDataGridViewCheckBoxColumn.DataPropertyName = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.HeaderText = "Enabled";
            this.enabledDataGridViewCheckBoxColumn.Name = "enabledDataGridViewCheckBoxColumn";
            this.enabledDataGridViewCheckBoxColumn.Width = 50;
            // 
            // parameterintDataGridViewTextBoxColumn
            // 
            this.parameterintDataGridViewTextBoxColumn.DataPropertyName = "parameter_int";
            this.parameterintDataGridViewTextBoxColumn.HeaderText = "Mode";
            this.parameterintDataGridViewTextBoxColumn.Name = "parameterintDataGridViewTextBoxColumn";
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
            // aForgeFunctionBindingSource
            // 
            this.aForgeFunctionBindingSource.DataSource = typeof(LitePlacer.AForgeFunction);
            // 
            // CamFunctionsClear_button
            // 
            this.CamFunctionsClear_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CamFunctionsClear_button.Location = new System.Drawing.Point(12, 164);
            this.CamFunctionsClear_button.Name = "CamFunctionsClear_button";
            this.CamFunctionsClear_button.Size = new System.Drawing.Size(75, 23);
            this.CamFunctionsClear_button.TabIndex = 133;
            this.CamFunctionsClear_button.Text = "Clear";
            this.CamFunctionsClear_button.UseVisualStyleBackColor = true;
            // 
            // CamFunctionDown_button
            // 
            this.CamFunctionDown_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CamFunctionDown_button.Location = new System.Drawing.Point(12, 135);
            this.CamFunctionDown_button.Name = "CamFunctionDown_button";
            this.CamFunctionDown_button.Size = new System.Drawing.Size(75, 23);
            this.CamFunctionDown_button.TabIndex = 132;
            this.CamFunctionDown_button.Text = "Move Down";
            this.CamFunctionDown_button.UseVisualStyleBackColor = true;
            // 
            // CamFunctionUp_button
            // 
            this.CamFunctionUp_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CamFunctionUp_button.Location = new System.Drawing.Point(12, 106);
            this.CamFunctionUp_button.Name = "CamFunctionUp_button";
            this.CamFunctionUp_button.Size = new System.Drawing.Size(75, 23);
            this.CamFunctionUp_button.TabIndex = 131;
            this.CamFunctionUp_button.Text = "Move Up";
            this.CamFunctionUp_button.UseVisualStyleBackColor = true;
            // 
            // DeleteCamFunction_button
            // 
            this.DeleteCamFunction_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteCamFunction_button.Location = new System.Drawing.Point(12, 77);
            this.DeleteCamFunction_button.Name = "DeleteCamFunction_button";
            this.DeleteCamFunction_button.Size = new System.Drawing.Size(75, 23);
            this.DeleteCamFunction_button.TabIndex = 130;
            this.DeleteCamFunction_button.Text = "Delete";
            this.DeleteCamFunction_button.UseVisualStyleBackColor = true;
            // 
            // AddCamFunction_button
            // 
            this.AddCamFunction_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddCamFunction_button.Location = new System.Drawing.Point(12, 48);
            this.AddCamFunction_button.Name = "AddCamFunction_button";
            this.AddCamFunction_button.Size = new System.Drawing.Size(75, 23);
            this.AddCamFunction_button.TabIndex = 129;
            this.AddCamFunction_button.Text = "Add";
            this.AddCamFunction_button.UseVisualStyleBackColor = true;
            this.AddCamFunction_button.Click += new System.EventHandler(this.AddCamFunction_button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.drawGrid_checkBox);
            this.groupBox1.Controls.Add(this.DownCameraDrawCross_checkBox);
            this.groupBox1.Controls.Add(this.FindFiducials_cb);
            this.groupBox1.Controls.Add(this.DownCameraDrawBox_checkBox);
            this.groupBox1.Controls.Add(this.DownCameraDrawTicks_checkBox);
            this.groupBox1.Controls.Add(this.DownCamFindCircles_checkBox);
            this.groupBox1.Controls.Add(this.DownCam_FindComponents_checkBox);
            this.groupBox1.Controls.Add(this.DownCamFindRectangles_checkBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 234);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 138);
            this.groupBox1.TabIndex = 134;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Down Camera";
            // 
            // drawGrid_checkBox
            // 
            this.drawGrid_checkBox.AutoSize = true;
            this.drawGrid_checkBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.drawGrid_checkBox.Location = new System.Drawing.Point(144, 88);
            this.drawGrid_checkBox.Name = "drawGrid_checkBox";
            this.drawGrid_checkBox.Size = new System.Drawing.Size(98, 17);
            this.drawGrid_checkBox.TabIndex = 152;
            this.drawGrid_checkBox.Text = "Draw 1mm Grid";
            this.drawGrid_checkBox.UseVisualStyleBackColor = true;
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
            // 
            // FindFiducials_cb
            // 
            this.FindFiducials_cb.AutoSize = true;
            this.FindFiducials_cb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FindFiducials_cb.Location = new System.Drawing.Point(6, 88);
            this.FindFiducials_cb.Name = "FindFiducials_cb";
            this.FindFiducials_cb.Size = new System.Drawing.Size(90, 17);
            this.FindFiducials_cb.TabIndex = 151;
            this.FindFiducials_cb.Text = "Find Fiducials";
            this.FindFiducials_cb.UseVisualStyleBackColor = true;
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
            // 
            // FilterSetSelectorComboBox
            // 
            this.FilterSetSelectorComboBox.FormattingEnabled = true;
            this.FilterSetSelectorComboBox.Location = new System.Drawing.Point(12, 12);
            this.FilterSetSelectorComboBox.Name = "FilterSetSelectorComboBox";
            this.FilterSetSelectorComboBox.Size = new System.Drawing.Size(165, 21);
            this.FilterSetSelectorComboBox.TabIndex = 136;
            this.FilterSetSelectorComboBox.SelectedIndexChanged += new System.EventHandler(this.FilterSetSelectorComboBox_SelectedIndexChanged);
            // 
            // button_saveSettings
            // 
            this.button_saveSettings.Location = new System.Drawing.Point(189, 12);
            this.button_saveSettings.Name = "button_saveSettings";
            this.button_saveSettings.Size = new System.Drawing.Size(75, 24);
            this.button_saveSettings.TabIndex = 142;
            this.button_saveSettings.Text = "Save Settings";
            this.button_saveSettings.UseVisualStyleBackColor = true;
            this.button_saveSettings.Click += new System.EventHandler(this.button_saveSettings_Click);
            // 
            // VideoFilterConfigurator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 406);
            this.Controls.Add(this.button_saveSettings);
            this.Controls.Add(this.FilterSetSelectorComboBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.CamFunctionsClear_button);
            this.Controls.Add(this.CamFunctionDown_button);
            this.Controls.Add(this.CamFunctionUp_button);
            this.Controls.Add(this.DeleteCamFunction_button);
            this.Controls.Add(this.AddCamFunction_button);
            this.Controls.Add(this.Display_dataGridView);
            this.Name = "VideoFilterConfigurator";
            this.Text = "VideoFilterConfigurator";
            ((System.ComponentModel.ISupportInitialize)(this.Display_dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aForgeFunctionBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView Display_dataGridView;
        private System.Windows.Forms.Button CamFunctionsClear_button;
        private System.Windows.Forms.Button CamFunctionDown_button;
        private System.Windows.Forms.Button CamFunctionUp_button;
        private System.Windows.Forms.Button DeleteCamFunction_button;
        private System.Windows.Forms.Button AddCamFunction_button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox drawGrid_checkBox;
        private System.Windows.Forms.CheckBox DownCameraDrawCross_checkBox;
        private System.Windows.Forms.CheckBox FindFiducials_cb;
        private System.Windows.Forms.CheckBox DownCameraDrawBox_checkBox;
        private System.Windows.Forms.CheckBox DownCameraDrawTicks_checkBox;
        private System.Windows.Forms.CheckBox DownCamFindCircles_checkBox;
        private System.Windows.Forms.CheckBox DownCam_FindComponents_checkBox;
        private System.Windows.Forms.CheckBox DownCamFindRectangles_checkBox;
        private System.Windows.Forms.ComboBox FilterSetSelectorComboBox;
        private System.Windows.Forms.Button button_saveSettings;
        private System.Windows.Forms.BindingSource aForgeFunctionBindingSource;
        private System.Windows.Forms.DataGridViewComboBoxColumn methodDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterintDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterdoubleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn gDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn bDataGridViewTextBoxColumn;

    }
}
namespace LitePlacer
{
    partial class PanelizeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.XRepeats_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.YRepeats_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.XIncrement_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.YIncrement_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PanelFiducials_dataGridView = new System.Windows.Forms.DataGridView();
            this.UseBoardFids_checkBox = new System.Windows.Forms.CheckBox();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.XFirstOffset_textBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.YFirstOffset_textBox = new System.Windows.Forms.TextBox();
            this.Designator_Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Xpanelize_Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ypanelize_Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PanelFiducials_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // XRepeats_textBox
            // 
            this.XRepeats_textBox.Location = new System.Drawing.Point(280, 18);
            this.XRepeats_textBox.Name = "XRepeats_textBox";
            this.XRepeats_textBox.Size = new System.Drawing.Size(38, 20);
            this.XRepeats_textBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X repeats:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(219, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y repeats:";
            // 
            // YRepeats_textBox
            // 
            this.YRepeats_textBox.Location = new System.Drawing.Point(280, 44);
            this.YRepeats_textBox.Name = "YRepeats_textBox";
            this.YRepeats_textBox.Size = new System.Drawing.Size(38, 20);
            this.YRepeats_textBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(324, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "X increment:";
            // 
            // XIncrement_textBox
            // 
            this.XIncrement_textBox.Location = new System.Drawing.Point(396, 18);
            this.XIncrement_textBox.Name = "XIncrement_textBox";
            this.XIncrement_textBox.Size = new System.Drawing.Size(38, 20);
            this.XIncrement_textBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Y increment:";
            // 
            // YIncrement_textBox
            // 
            this.YIncrement_textBox.Location = new System.Drawing.Point(396, 44);
            this.YIncrement_textBox.Name = "YIncrement_textBox";
            this.YIncrement_textBox.Size = new System.Drawing.Size(38, 20);
            this.YIncrement_textBox.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PanelFiducials_dataGridView);
            this.groupBox1.Controls.Add(this.UseBoardFids_checkBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(375, 234);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fiducials:";
            // 
            // PanelFiducials_dataGridView
            // 
            this.PanelFiducials_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.PanelFiducials_dataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.PanelFiducials_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PanelFiducials_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Designator_Column,
            this.Xpanelize_Column,
            this.Ypanelize_Column});
            this.PanelFiducials_dataGridView.Location = new System.Drawing.Point(6, 42);
            this.PanelFiducials_dataGridView.Name = "PanelFiducials_dataGridView";
            this.PanelFiducials_dataGridView.RowHeadersVisible = false;
            this.PanelFiducials_dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.PanelFiducials_dataGridView.Size = new System.Drawing.Size(363, 186);
            this.PanelFiducials_dataGridView.TabIndex = 9;
            // 
            // UseBoardFids_checkBox
            // 
            this.UseBoardFids_checkBox.AutoSize = true;
            this.UseBoardFids_checkBox.Location = new System.Drawing.Point(6, 19);
            this.UseBoardFids_checkBox.Name = "UseBoardFids_checkBox";
            this.UseBoardFids_checkBox.Size = new System.Drawing.Size(230, 17);
            this.UseBoardFids_checkBox.TabIndex = 7;
            this.UseBoardFids_checkBox.Text = "Use fiducials on CAD data, ignore this table";
            this.UseBoardFids_checkBox.UseVisualStyleBackColor = true;
            // 
            // Cancel_button
            // 
            this.Cancel_button.Location = new System.Drawing.Point(260, 311);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 9;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(359, 311);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 8;
            this.OK_button.Text = "OK";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Offset to lower left board, X:";
            // 
            // XFirstOffset_textBox
            // 
            this.XFirstOffset_textBox.Location = new System.Drawing.Point(156, 18);
            this.XFirstOffset_textBox.Name = "XFirstOffset_textBox";
            this.XFirstOffset_textBox.Size = new System.Drawing.Size(38, 20);
            this.XFirstOffset_textBox.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Offset to lower left board, Y:";
            // 
            // YFirstOffset_textBox
            // 
            this.YFirstOffset_textBox.Location = new System.Drawing.Point(156, 44);
            this.YFirstOffset_textBox.Name = "YFirstOffset_textBox";
            this.YFirstOffset_textBox.Size = new System.Drawing.Size(38, 20);
            this.YFirstOffset_textBox.TabIndex = 2;
            // 
            // Designator_Column
            // 
            this.Designator_Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Designator_Column.DividerWidth = 1;
            this.Designator_Column.HeaderText = "Designator";
            this.Designator_Column.Name = "Designator_Column";
            this.Designator_Column.Width = 200;
            // 
            // Xpanelize_Column
            // 
            this.Xpanelize_Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Xpanelize_Column.HeaderText = "X";
            this.Xpanelize_Column.Name = "Xpanelize_Column";
            this.Xpanelize_Column.Width = 80;
            // 
            // Ypanelize_Column
            // 
            this.Ypanelize_Column.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Ypanelize_Column.HeaderText = "Y";
            this.Ypanelize_Column.Name = "Ypanelize_Column";
            this.Ypanelize_Column.Width = 80;
            // 
            // PanelizeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 346);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.YFirstOffset_textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.XFirstOffset_textBox);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.YIncrement_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.XIncrement_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.YRepeats_textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.XRepeats_textBox);
            this.Name = "PanelizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PanelizeForm";
            this.Load += new System.EventHandler(this.PanelizeForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PanelFiducials_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox XRepeats_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox YRepeats_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox XIncrement_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox YIncrement_textBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox UseBoardFids_checkBox;
        private System.Windows.Forms.DataGridView PanelFiducials_dataGridView;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox XFirstOffset_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox YFirstOffset_textBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn Designator_Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Xpanelize_Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ypanelize_Column;
    }
}
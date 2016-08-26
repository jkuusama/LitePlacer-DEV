namespace LitePlacer
{
    partial class TapeEditForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TapeEditForm));
            this.TapeEditOK_button = new System.Windows.Forms.Button();
            this.ID_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.TapeOrientation_comboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TapeRotation_comboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TapeOffsetX_textBox = new System.Windows.Forms.TextBox();
            this.TapePitch_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.Nozzle_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TapeWidth_comboBox = new System.Windows.Forms.ComboBox();
            this.TapeOffsetY_textBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.TapeEditCancel_button = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Nozzle_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TapeEditOK_button
            // 
            this.TapeEditOK_button.Location = new System.Drawing.Point(315, 227);
            this.TapeEditOK_button.Name = "TapeEditOK_button";
            this.TapeEditOK_button.Size = new System.Drawing.Size(75, 23);
            this.TapeEditOK_button.TabIndex = 0;
            this.TapeEditOK_button.Text = "OK";
            this.TapeEditOK_button.UseVisualStyleBackColor = true;
            this.TapeEditOK_button.Click += new System.EventHandler(this.TapeEditOK_button_Click);
            // 
            // ID_textBox
            // 
            this.ID_textBox.Location = new System.Drawing.Point(15, 25);
            this.ID_textBox.Name = "ID_textBox";
            this.ID_textBox.Size = new System.Drawing.Size(100, 20);
            this.ID_textBox.TabIndex = 1;
            this.toolTip1.SetToolTip(this.ID_textBox, "Name of the tape (ex: 0805, 10k)");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ID";
            this.toolTip1.SetToolTip(this.label1, "Name of the tape (ex: 0805, 10k)");
            // 
            // TapeOrientation_comboBox
            // 
            this.TapeOrientation_comboBox.FormattingEnabled = true;
            this.TapeOrientation_comboBox.Items.AddRange(new object[] {
            "+X",
            "-X",
            "+Y",
            "-Y"});
            this.TapeOrientation_comboBox.Location = new System.Drawing.Point(121, 25);
            this.TapeOrientation_comboBox.Name = "TapeOrientation_comboBox";
            this.TapeOrientation_comboBox.Size = new System.Drawing.Size(61, 21);
            this.TapeOrientation_comboBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.TapeOrientation_comboBox, "Which way the part count increases.\r\n+Y: Tape holes on right\r\n-Y: Tape holes on l" +
        "eft\r\n+X: Tape holes down\r\n-X: Tape holes up");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Orientation";
            this.toolTip1.SetToolTip(this.label2, "Which way the part count increases.\r\n+Y: Tape holes on right\r\n-Y: Tape holes on l" +
        "eft\r\n+X: Tape holes down\r\n-X: Tape holes up");
            // 
            // TapeRotation_comboBox
            // 
            this.TapeRotation_comboBox.FormattingEnabled = true;
            this.TapeRotation_comboBox.Items.AddRange(new object[] {
            "0deg.",
            "90deg.",
            "180deg.",
            "270deg."});
            this.TapeRotation_comboBox.Location = new System.Drawing.Point(188, 24);
            this.TapeRotation_comboBox.Name = "TapeRotation_comboBox";
            this.TapeRotation_comboBox.Size = new System.Drawing.Size(61, 21);
            this.TapeRotation_comboBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.TapeRotation_comboBox, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(185, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Rotation";
            this.toolTip1.SetToolTip(this.label3, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Nozzle";
            this.toolTip1.SetToolTip(this.label4, "Manufactures put parts on tapes rotated in any which way.\r\nPart on a tape in +Y o" +
        "rientation (holes to right) and with 0 deg.\r\nrotation in the CAD data, is rotate" +
        "d this much when placed.");
            // 
            // TapeOffsetX_textBox
            // 
            this.TapeOffsetX_textBox.Location = new System.Drawing.Point(435, 24);
            this.TapeOffsetX_textBox.Name = "TapeOffsetX_textBox";
            this.TapeOffsetX_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapeOffsetX_textBox.TabIndex = 9;
            this.toolTip1.SetToolTip(this.TapeOffsetX_textBox, "Distance to part center from location mark (hole). Y is 2.0mm for\r\nregular tapes." +
        "");
            // 
            // TapePitch_textBox
            // 
            this.TapePitch_textBox.Location = new System.Drawing.Point(381, 24);
            this.TapePitch_textBox.Name = "TapePitch_textBox";
            this.TapePitch_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapePitch_textBox.TabIndex = 10;
            this.toolTip1.SetToolTip(this.TapePitch_textBox, "Distance of one part to next");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(432, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Offset X";
            this.toolTip1.SetToolTip(this.label5, "Distance to part center from location mark (hole). Y is 2.0mm for\r\nregular tapes." +
        "");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(378, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Part Pitch";
            this.toolTip1.SetToolTip(this.label6, "Distance of one part to next");
            // 
            // Nozzle_numericUpDown
            // 
            this.Nozzle_numericUpDown.Location = new System.Drawing.Point(255, 24);
            this.Nozzle_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Nozzle_numericUpDown.Name = "Nozzle_numericUpDown";
            this.Nozzle_numericUpDown.Size = new System.Drawing.Size(37, 20);
            this.Nozzle_numericUpDown.TabIndex = 7;
            this.toolTip1.SetToolTip(this.Nozzle_numericUpDown, "Which nozzle to use for these parts");
            this.Nozzle_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TapeWidth_comboBox
            // 
            this.TapeWidth_comboBox.DropDownWidth = 170;
            this.TapeWidth_comboBox.FormattingEnabled = true;
            this.TapeWidth_comboBox.Items.AddRange(new object[] {
            "8/2mm",
            "8/4mm",
            "12/4mm",
            "12/8mm",
            "16/4mm",
            "16/8mm",
            "16/12mm",
            "24/4mm",
            "24/8mm",
            "24/12mm",
            "24/16mm",
            "24/20mm",
            "32/4mm",
            "32/8mm",
            "32/12mm",
            "32/16mm",
            "32/20mm",
            "32/24mm",
            "32/28mm",
            "32/32mm",
            "custom"});
            this.TapeWidth_comboBox.Location = new System.Drawing.Point(298, 23);
            this.TapeWidth_comboBox.Name = "TapeWidth_comboBox";
            this.TapeWidth_comboBox.Size = new System.Drawing.Size(77, 21);
            this.TapeWidth_comboBox.TabIndex = 11;
            this.toolTip1.SetToolTip(this.TapeWidth_comboBox, "Select a standard size");
            this.TapeWidth_comboBox.SelectedIndexChanged += new System.EventHandler(this.TapeWidth_comboBox_SelectedIndexChanged);
            // 
            // TapeOffsetY_textBox
            // 
            this.TapeOffsetY_textBox.Location = new System.Drawing.Point(489, 24);
            this.TapeOffsetY_textBox.Name = "TapeOffsetY_textBox";
            this.TapeOffsetY_textBox.Size = new System.Drawing.Size(48, 20);
            this.TapeOffsetY_textBox.TabIndex = 14;
            this.toolTip1.SetToolTip(this.TapeOffsetY_textBox, "Distance to part center from location mark (hole). Y is 2.0mm for\r\nregular tapes." +
        "");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(486, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Offset Y";
            this.toolTip1.SetToolTip(this.label7, "Distance to part center from location mark (hole). Y is 2.0mm for\r\nregular tapes." +
        "");
            // 
            // TapeEditCancel_button
            // 
            this.TapeEditCancel_button.Location = new System.Drawing.Point(402, 227);
            this.TapeEditCancel_button.Name = "TapeEditCancel_button";
            this.TapeEditCancel_button.Size = new System.Drawing.Size(75, 23);
            this.TapeEditCancel_button.TabIndex = 16;
            this.TapeEditCancel_button.Text = "Cancel";
            this.TapeEditCancel_button.UseVisualStyleBackColor = true;
            this.TapeEditCancel_button.Click += new System.EventHandler(this.TapeEditCancel_button_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(297, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Tape Width";
            this.toolTip1.SetToolTip(this.label8, "Select a standard size");
            // 
            // TapeEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 262);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.TapeEditCancel_button);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.TapeOffsetY_textBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TapeWidth_comboBox);
            this.Controls.Add(this.TapePitch_textBox);
            this.Controls.Add(this.TapeOffsetX_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Nozzle_numericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TapeRotation_comboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TapeOrientation_comboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ID_textBox);
            this.Controls.Add(this.TapeEditOK_button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TapeEditForm";
            this.Text = "Edit Tape Parameters";
            this.Load += new System.EventHandler(this.TapeEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Nozzle_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TapeEditOK_button;
        private System.Windows.Forms.TextBox ID_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox TapeOrientation_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox TapeRotation_comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown Nozzle_numericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TapeOffsetX_textBox;
        private System.Windows.Forms.TextBox TapePitch_textBox;
        private System.Windows.Forms.ComboBox TapeWidth_comboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TapeOffsetY_textBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button TapeEditCancel_button;
        private System.Windows.Forms.Label label8;
    }
}
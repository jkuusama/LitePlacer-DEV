﻿namespace LitePlacer
{
	partial class MethodSelectionForm
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
            this.Question_button = new System.Windows.Forms.Button();
            this.Place_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChangeNozzle_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Recalibrate_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.Ignore_button = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.Fiducials_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.Pause_button = new System.Windows.Forms.Button();
            this.UpdateJobGrid_checkBox = new System.Windows.Forms.CheckBox();
            this.ManualUpCam_button = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.LoosePart_button = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.PlaceFast_button = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.ManualDownCam_button = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.PlaceAssisted_button = new System.Windows.Forms.Button();
            this.LoosePartAssisted_button = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.Method_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Question_button
            // 
            this.Question_button.Location = new System.Drawing.Point(317, 15);
            this.Question_button.Margin = new System.Windows.Forms.Padding(4);
            this.Question_button.Name = "Question_button";
            this.Question_button.Size = new System.Drawing.Size(181, 28);
            this.Question_button.TabIndex = 0;
            this.Question_button.Text = "?";
            this.Question_button.UseVisualStyleBackColor = true;
            this.Question_button.Click += new System.EventHandler(this.Question_button_Click);
            // 
            // Place_button
            // 
            this.Place_button.Location = new System.Drawing.Point(317, 86);
            this.Place_button.Margin = new System.Windows.Forms.Padding(4);
            this.Place_button.Name = "Place_button";
            this.Place_button.Size = new System.Drawing.Size(181, 28);
            this.Place_button.TabIndex = 1;
            this.Place_button.Text = "Place";
            this.Method_toolTip.SetToolTip(this.Place_button, "Slow placement measures tape hole postion\r\nfor each part separately. Use this if " +
        "the vision\r\nmeasurement targets parts, not holes");
            this.Place_button.UseVisualStyleBackColor = true;
            this.Place_button.Click += new System.EventHandler(this.Place_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Decide at runtime";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 92);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Slow placement";
            this.Method_toolTip.SetToolTip(this.label2, "Slow placement measures tape hole postion\r\nfor each part separately. Use this if " +
        "the vision\r\nmeasurement targets parts, not holes");
            // 
            // ChangeNozzle_button
            // 
            this.ChangeNozzle_button.Location = new System.Drawing.Point(317, 229);
            this.ChangeNozzle_button.Margin = new System.Windows.Forms.Padding(4);
            this.ChangeNozzle_button.Name = "ChangeNozzle_button";
            this.ChangeNozzle_button.Size = new System.Drawing.Size(181, 28);
            this.ChangeNozzle_button.TabIndex = 4;
            this.ChangeNozzle_button.Text = "Manual Nozzle change";
            this.ChangeNozzle_button.UseVisualStyleBackColor = true;
            this.ChangeNozzle_button.Click += new System.EventHandler(this.ChangeNozzle_button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(142, 235);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pause for Nozzle change";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(76, 271);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(216, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Re-runs postion calibration routines";
            // 
            // Recalibrate_button
            // 
            this.Recalibrate_button.Location = new System.Drawing.Point(317, 265);
            this.Recalibrate_button.Margin = new System.Windows.Forms.Padding(4);
            this.Recalibrate_button.Name = "Recalibrate_button";
            this.Recalibrate_button.Size = new System.Drawing.Size(181, 28);
            this.Recalibrate_button.TabIndex = 6;
            this.Recalibrate_button.Text = "Recalibrate";
            this.Recalibrate_button.UseVisualStyleBackColor = true;
            this.Recalibrate_button.Click += new System.EventHandler(this.Recalibrate_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(220, 306);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Skip this row";
            // 
            // Ignore_button
            // 
            this.Ignore_button.Location = new System.Drawing.Point(317, 300);
            this.Ignore_button.Margin = new System.Windows.Forms.Padding(4);
            this.Ignore_button.Name = "Ignore_button";
            this.Ignore_button.Size = new System.Drawing.Size(181, 28);
            this.Ignore_button.TabIndex = 8;
            this.Ignore_button.Text = "Ignore";
            this.Ignore_button.UseVisualStyleBackColor = true;
            this.Ignore_button.Click += new System.EventHandler(this.Ignore_button_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(149, 378);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 16);
            this.label6.TabIndex = 11;
            this.label6.Text = "Fiducials are on this row";
            // 
            // Fiducials_button
            // 
            this.Fiducials_button.Location = new System.Drawing.Point(317, 372);
            this.Fiducials_button.Margin = new System.Windows.Forms.Padding(4);
            this.Fiducials_button.Name = "Fiducials_button";
            this.Fiducials_button.Size = new System.Drawing.Size(181, 28);
            this.Fiducials_button.TabIndex = 10;
            this.Fiducials_button.Text = "Fiducials";
            this.Fiducials_button.UseVisualStyleBackColor = true;
            this.Fiducials_button.Click += new System.EventHandler(this.Fiducials_button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(213, 342);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "Waits for user";
            // 
            // Pause_button
            // 
            this.Pause_button.Location = new System.Drawing.Point(317, 336);
            this.Pause_button.Margin = new System.Windows.Forms.Padding(4);
            this.Pause_button.Name = "Pause_button";
            this.Pause_button.Size = new System.Drawing.Size(181, 28);
            this.Pause_button.TabIndex = 12;
            this.Pause_button.Text = "Pause";
            this.Pause_button.UseVisualStyleBackColor = true;
            this.Pause_button.Click += new System.EventHandler(this.Pause_button_Click);
            // 
            // UpdateJobGrid_checkBox
            // 
            this.UpdateJobGrid_checkBox.AutoSize = true;
            this.UpdateJobGrid_checkBox.Location = new System.Drawing.Point(333, 437);
            this.UpdateJobGrid_checkBox.Margin = new System.Windows.Forms.Padding(4);
            this.UpdateJobGrid_checkBox.Name = "UpdateJobGrid_checkBox";
            this.UpdateJobGrid_checkBox.Size = new System.Drawing.Size(132, 20);
            this.UpdateJobGrid_checkBox.TabIndex = 14;
            this.UpdateJobGrid_checkBox.Text = "Update Job Data";
            this.UpdateJobGrid_checkBox.UseVisualStyleBackColor = true;
            this.UpdateJobGrid_checkBox.CheckedChanged += new System.EventHandler(this.UpdateJobGrid_checkBox_CheckedChanged);
            // 
            // ManualUpCam_button
            // 
            this.ManualUpCam_button.Location = new System.Drawing.Point(9, 406);
            this.ManualUpCam_button.Margin = new System.Windows.Forms.Padding(4);
            this.ManualUpCam_button.Name = "ManualUpCam_button";
            this.ManualUpCam_button.Size = new System.Drawing.Size(159, 28);
            this.ManualUpCam_button.TabIndex = 15;
            this.ManualUpCam_button.Text = "Place with UpCam";
            this.ManualUpCam_button.UseVisualStyleBackColor = true;
            this.ManualUpCam_button.Visible = false;
            this.ManualUpCam_button.Click += new System.EventHandler(this.ManualUpCam_button_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 454);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(268, 16);
            this.label8.TabIndex = 16;
            this.label8.Text = "Manually assisted, with up camera snapshot";
            this.label8.Visible = false;
            // 
            // LoosePart_button
            // 
            this.LoosePart_button.Location = new System.Drawing.Point(315, 158);
            this.LoosePart_button.Margin = new System.Windows.Forms.Padding(4);
            this.LoosePart_button.Name = "LoosePart_button";
            this.LoosePart_button.Size = new System.Drawing.Size(181, 28);
            this.LoosePart_button.TabIndex = 17;
            this.LoosePart_button.Text = "Loose Part Pickup";
            this.LoosePart_button.UseVisualStyleBackColor = true;
            this.LoosePart_button.Click += new System.EventHandler(this.LoosePart_button_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(60, 164);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(233, 16);
            this.label9.TabIndex = 18;
            this.label9.Text = "Pickup from loose part pickup position";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(205, 56);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 16);
            this.label10.TabIndex = 20;
            this.label10.Text = "Fast placement";
            this.Method_toolTip.SetToolTip(this.label10, "Fast (normal) placement: Measures first and last hole of the tape,\r\ncalculates in" +
        "termediate part positions from these");
            // 
            // PlaceFast_button
            // 
            this.PlaceFast_button.Location = new System.Drawing.Point(317, 50);
            this.PlaceFast_button.Margin = new System.Windows.Forms.Padding(4);
            this.PlaceFast_button.Name = "PlaceFast_button";
            this.PlaceFast_button.Size = new System.Drawing.Size(181, 28);
            this.PlaceFast_button.TabIndex = 19;
            this.PlaceFast_button.Text = "Place Fast";
            this.Method_toolTip.SetToolTip(this.PlaceFast_button, "Fast (normal) placement: Measures first and last hole of the tape,\r\ncalculates in" +
        "termediate part positions from these");
            this.PlaceFast_button.UseVisualStyleBackColor = true;
            this.PlaceFast_button.Click += new System.EventHandler(this.PlaceFast_button_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(5, 438);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(285, 16);
            this.label11.TabIndex = 22;
            this.label11.Text = "Manually assisted, with down camera snapshot";
            this.label11.Visible = false;
            // 
            // ManualDownCam_button
            // 
            this.ManualDownCam_button.Location = new System.Drawing.Point(176, 406);
            this.ManualDownCam_button.Margin = new System.Windows.Forms.Padding(4);
            this.ManualDownCam_button.Name = "ManualDownCam_button";
            this.ManualDownCam_button.Size = new System.Drawing.Size(159, 28);
            this.ManualDownCam_button.TabIndex = 21;
            this.ManualDownCam_button.Text = "Place with DownCam";
            this.ManualDownCam_button.UseVisualStyleBackColor = true;
            this.ManualDownCam_button.Visible = false;
            this.ManualDownCam_button.Click += new System.EventHandler(this.ManualDownCam_button_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(177, 128);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(125, 16);
            this.label12.TabIndex = 24;
            this.label12.Text = "Assisted placement";
            this.Method_toolTip.SetToolTip(this.label12, "Assisted placement takes part almost to position,\r\nthen wait for you to jog it ex" +
        "actly on target. \r\nTo finish placement, press enter.");
            // 
            // PlaceAssisted_button
            // 
            this.PlaceAssisted_button.Location = new System.Drawing.Point(315, 122);
            this.PlaceAssisted_button.Margin = new System.Windows.Forms.Padding(4);
            this.PlaceAssisted_button.Name = "PlaceAssisted_button";
            this.PlaceAssisted_button.Size = new System.Drawing.Size(181, 28);
            this.PlaceAssisted_button.TabIndex = 23;
            this.PlaceAssisted_button.Text = "Place Assisted";
            this.Method_toolTip.SetToolTip(this.PlaceAssisted_button, "Assisted placement takes part almost to position,\r\nthen wait for you to jog it ex" +
        "actly on target. \r\nTo finish placement, press enter.");
            this.PlaceAssisted_button.UseVisualStyleBackColor = true;
            this.PlaceAssisted_button.Click += new System.EventHandler(this.PlaceAssisted_button_Click);
            // 
            // LoosePartAssisted_button
            // 
            this.LoosePartAssisted_button.Location = new System.Drawing.Point(315, 193);
            this.LoosePartAssisted_button.Margin = new System.Windows.Forms.Padding(4);
            this.LoosePartAssisted_button.Name = "LoosePartAssisted_button";
            this.LoosePartAssisted_button.Size = new System.Drawing.Size(181, 28);
            this.LoosePartAssisted_button.TabIndex = 25;
            this.LoosePartAssisted_button.Text = "Loose Part Assisted";
            this.Method_toolTip.SetToolTip(this.LoosePartAssisted_button, "Picks up loose part, then takes part almost to position,\r\nthen wait for you to jo" +
        "g it exactly on target. \r\nTo finish placement, press enter.\r\n");
            this.LoosePartAssisted_button.UseVisualStyleBackColor = true;
            this.LoosePartAssisted_button.Click += new System.EventHandler(this.LoosePartAssisted_button_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(141, 199);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(159, 16);
            this.label13.TabIndex = 26;
            this.label13.Text = "Place loose part assisted";
            this.Method_toolTip.SetToolTip(this.label13, "Picks up loose part, then takes part almost to position,\r\nthen wait for you to jo" +
        "g it exactly on target. \r\nTo finish placement, press enter.\r\n");
            // 
            // MethodSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 405);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.LoosePartAssisted_button);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.PlaceAssisted_button);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.ManualDownCam_button);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.PlaceFast_button);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.LoosePart_button);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ManualUpCam_button);
            this.Controls.Add(this.UpdateJobGrid_checkBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Pause_button);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Fiducials_button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Ignore_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Recalibrate_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ChangeNozzle_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Place_button);
            this.Controls.Add(this.Question_button);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MethodSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Main Method";
            this.Load += new System.EventHandler(this.MethodSelectionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Question_button;
		private System.Windows.Forms.Button Place_button;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button ChangeNozzle_button;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button Recalibrate_button;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button Ignore_button;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button Fiducials_button;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button Pause_button;
		private System.Windows.Forms.CheckBox UpdateJobGrid_checkBox;
		private System.Windows.Forms.Button ManualUpCam_button;
		private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button LoosePart_button;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button PlaceFast_button;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button ManualDownCam_button;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button PlaceAssisted_button;
        private System.Windows.Forms.Button LoosePartAssisted_button;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ToolTip Method_toolTip;
    }
}
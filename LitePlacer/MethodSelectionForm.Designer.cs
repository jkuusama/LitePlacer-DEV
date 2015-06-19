using System.ComponentModel;
using System.Windows.Forms;

namespace LitePlacer
{
	partial class MethodSelectionForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
            this.Question_button = new System.Windows.Forms.Button();
            this.Place_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ChangeNeedle_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Recalibrate_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.Ignore_button = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.Fiducials_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.Pause_button = new System.Windows.Forms.Button();
            this.PlaceWithCam_button = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.LoosePart_button = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Question_button
            // 
            this.Question_button.Location = new System.Drawing.Point(199, 14);
            this.Question_button.Name = "Question_button";
            this.Question_button.Size = new System.Drawing.Size(109, 23);
            this.Question_button.TabIndex = 0;
            this.Question_button.Text = "?";
            this.Question_button.UseVisualStyleBackColor = true;
            this.Question_button.Click += new System.EventHandler(this.button_Click);
            // 
            // Place_button
            // 
            this.Place_button.Location = new System.Drawing.Point(199, 43);
            this.Place_button.Name = "Place_button";
            this.Place_button.Size = new System.Drawing.Size(109, 23);
            this.Place_button.TabIndex = 1;
            this.Place_button.Text = "Place";
            this.Place_button.UseVisualStyleBackColor = true;
            this.Place_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Dedice at runtime";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Normal placement";
            // 
            // ChangeNeedle_button
            // 
            this.ChangeNeedle_button.Location = new System.Drawing.Point(199, 101);
            this.ChangeNeedle_button.Name = "ChangeNeedle_button";
            this.ChangeNeedle_button.Size = new System.Drawing.Size(109, 23);
            this.ChangeNeedle_button.TabIndex = 4;
            this.ChangeNeedle_button.Text = "Change needle";
            this.ChangeNeedle_button.UseVisualStyleBackColor = true;
            this.ChangeNeedle_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pause for needle change";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Re-runs postion calibration routines";
            // 
            // Recalibrate_button
            // 
            this.Recalibrate_button.Location = new System.Drawing.Point(199, 130);
            this.Recalibrate_button.Name = "Recalibrate_button";
            this.Recalibrate_button.Size = new System.Drawing.Size(109, 23);
            this.Recalibrate_button.TabIndex = 6;
            this.Recalibrate_button.Text = "Recalibrate";
            this.Recalibrate_button.UseVisualStyleBackColor = true;
            this.Recalibrate_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(126, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Skip this row";
            // 
            // Ignore_button
            // 
            this.Ignore_button.Location = new System.Drawing.Point(199, 159);
            this.Ignore_button.Name = "Ignore_button";
            this.Ignore_button.Size = new System.Drawing.Size(109, 23);
            this.Ignore_button.TabIndex = 8;
            this.Ignore_button.Text = "Ignore";
            this.Ignore_button.UseVisualStyleBackColor = true;
            this.Ignore_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(73, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Fiducials are on this row";
            // 
            // Fiducials_button
            // 
            this.Fiducials_button.Location = new System.Drawing.Point(199, 217);
            this.Fiducials_button.Name = "Fiducials_button";
            this.Fiducials_button.Size = new System.Drawing.Size(109, 23);
            this.Fiducials_button.TabIndex = 10;
            this.Fiducials_button.Text = "Fiducials";
            this.Fiducials_button.UseVisualStyleBackColor = true;
            this.Fiducials_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(121, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Waits for user";
            // 
            // Pause_button
            // 
            this.Pause_button.Location = new System.Drawing.Point(199, 188);
            this.Pause_button.Name = "Pause_button";
            this.Pause_button.Size = new System.Drawing.Size(109, 23);
            this.Pause_button.TabIndex = 12;
            this.Pause_button.Text = "Pause";
            this.Pause_button.UseVisualStyleBackColor = true;
            this.Pause_button.Click += new System.EventHandler(this.button_Click);
            // 
            // PlaceWithCam_button
            // 
            this.PlaceWithCam_button.Location = new System.Drawing.Point(199, 256);
            this.PlaceWithCam_button.Name = "PlaceWithCam_button";
            this.PlaceWithCam_button.Size = new System.Drawing.Size(109, 23);
            this.PlaceWithCam_button.TabIndex = 15;
            this.PlaceWithCam_button.Text = "Place with UpCam";
            this.PlaceWithCam_button.UseVisualStyleBackColor = true;
            this.PlaceWithCam_button.Visible = false;
            this.PlaceWithCam_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 261);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(170, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Place, with correction from UpCam";
            this.label8.Visible = false;
            // 
            // LoosePart_button
            // 
            this.LoosePart_button.Location = new System.Drawing.Point(199, 72);
            this.LoosePart_button.Name = "LoosePart_button";
            this.LoosePart_button.Size = new System.Drawing.Size(109, 23);
            this.LoosePart_button.TabIndex = 17;
            this.LoosePart_button.Text = "LoosePart";
            this.LoosePart_button.UseVisualStyleBackColor = true;
            this.LoosePart_button.Click += new System.EventHandler(this.button_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 77);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(186, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Pickup from loose part pickup position";
            // 
            // MethodSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 284);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.LoosePart_button);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.PlaceWithCam_button);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Pause_button);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Fiducials_button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Ignore_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Recalibrate_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ChangeNeedle_button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Place_button);
            this.Controls.Add(this.Question_button);
            this.Name = "MethodSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Method";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Button Question_button;
		private Button Place_button;
		private Label label1;
		private Label label2;
		private Button ChangeNeedle_button;
		private Label label3;
		private Label label4;
		private Button Recalibrate_button;
		private Label label5;
		private Button Ignore_button;
		private Label label6;
		private Button Fiducials_button;
		private Label label7;
        private Button Pause_button;
		private Button PlaceWithCam_button;
		private Label label8;
        private Button LoosePart_button;
        private Label label9;
	}
}
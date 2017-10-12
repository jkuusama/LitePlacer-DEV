namespace LitePlacer
{
	partial class TapeSelectionForm
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
            this.AbortJob_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Ignore_button = new System.Windows.Forms.Button();
            this.UpdateJobData_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // AbortJob_button
            // 
            this.AbortJob_button.Location = new System.Drawing.Point(15, 12);
            this.AbortJob_button.Name = "AbortJob_button";
            this.AbortJob_button.Size = new System.Drawing.Size(75, 23);
            this.AbortJob_button.TabIndex = 0;
            this.AbortJob_button.Text = "Abort Job";
            this.AbortJob_button.UseVisualStyleBackColor = true;
            this.AbortJob_button.Click += new System.EventHandler(this.AbortJob_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select Tape:";
            // 
            // Ignore_button
            // 
            this.Ignore_button.Location = new System.Drawing.Point(96, 12);
            this.Ignore_button.Name = "Ignore_button";
            this.Ignore_button.Size = new System.Drawing.Size(75, 23);
            this.Ignore_button.TabIndex = 4;
            this.Ignore_button.Text = "Ignore these";
            this.Ignore_button.UseVisualStyleBackColor = true;
            this.Ignore_button.Click += new System.EventHandler(this.Ignore_button_Click);
            // 
            // UpdateJobData_checkBox
            // 
            this.UpdateJobData_checkBox.AutoSize = true;
            this.UpdateJobData_checkBox.Location = new System.Drawing.Point(177, 16);
            this.UpdateJobData_checkBox.Name = "UpdateJobData_checkBox";
            this.UpdateJobData_checkBox.Size = new System.Drawing.Size(107, 17);
            this.UpdateJobData_checkBox.TabIndex = 5;
            this.UpdateJobData_checkBox.Text = "Update Job Data";
            this.UpdateJobData_checkBox.UseVisualStyleBackColor = true;
            this.UpdateJobData_checkBox.CheckedChanged += new System.EventHandler(this.UpdateJobData_checkBox_CheckedChanged);
            // 
            // TapeSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 668);
            this.Controls.Add(this.UpdateJobData_checkBox);
            this.Controls.Add(this.Ignore_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AbortJob_button);
            this.Name = "TapeSelectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Tape";
            this.Load += new System.EventHandler(this.TapeSelectionForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button AbortJob_button;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button Ignore_button;
        private System.Windows.Forms.CheckBox UpdateJobData_checkBox;

	}
}
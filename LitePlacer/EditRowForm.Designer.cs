namespace LitePlacer
{
	partial class EditRowForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.EditCount_textBox = new System.Windows.Forms.TextBox();
			this.EditComponentType_textBox = new System.Windows.Forms.TextBox();
			this.EditParameters_textBox = new System.Windows.Forms.TextBox();
			this.EditComponents_textBox = new System.Windows.Forms.TextBox();
			this.OK_button = new System.Windows.Forms.Button();
			this.Cancel_button = new System.Windows.Forms.Button();
			this.Method_textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.WindowText;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "Count";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label2.ForeColor = System.Drawing.SystemColors.WindowText;
			this.label2.Location = new System.Drawing.Point(63, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 32);
			this.label2.TabIndex = 1;
			this.label2.Text = "Component Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.ForeColor = System.Drawing.SystemColors.WindowText;
			this.label3.Location = new System.Drawing.Point(217, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 32);
			this.label3.TabIndex = 2;
			this.label3.Text = "Method";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.ForeColor = System.Drawing.SystemColors.WindowText;
			this.label4.Location = new System.Drawing.Point(321, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(93, 32);
			this.label4.TabIndex = 3;
			this.label4.Text = "Method Parameters";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.ForeColor = System.Drawing.SystemColors.WindowText;
			this.label5.Location = new System.Drawing.Point(414, 9);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 32);
			this.label5.TabIndex = 4;
			this.label5.Text = "Components";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// EditCount_textBox
			// 
			this.EditCount_textBox.Location = new System.Drawing.Point(12, 41);
			this.EditCount_textBox.Name = "EditCount_textBox";
			this.EditCount_textBox.ReadOnly = true;
			this.EditCount_textBox.Size = new System.Drawing.Size(51, 20);
			this.EditCount_textBox.TabIndex = 5;
			// 
			// EditComponentType_textBox
			// 
			this.EditComponentType_textBox.Location = new System.Drawing.Point(63, 41);
			this.EditComponentType_textBox.Name = "EditComponentType_textBox";
			this.EditComponentType_textBox.Size = new System.Drawing.Size(154, 20);
			this.EditComponentType_textBox.TabIndex = 6;
			this.EditComponentType_textBox.TextChanged += new System.EventHandler(this.EditComponentType_textBox_TextChanged);
			// 
			// EditParameters_textBox
			// 
			this.EditParameters_textBox.Location = new System.Drawing.Point(321, 41);
			this.EditParameters_textBox.Name = "EditParameters_textBox";
			this.EditParameters_textBox.Size = new System.Drawing.Size(93, 20);
			this.EditParameters_textBox.TabIndex = 8;
			this.EditParameters_textBox.TextChanged += new System.EventHandler(this.EditParameters_textBox_TextChanged);
			// 
			// EditComponents_textBox
			// 
			this.EditComponents_textBox.Location = new System.Drawing.Point(414, 41);
			this.EditComponents_textBox.Name = "EditComponents_textBox";
			this.EditComponents_textBox.Size = new System.Drawing.Size(93, 20);
			this.EditComponents_textBox.TabIndex = 9;
			this.EditComponents_textBox.TextChanged += new System.EventHandler(this.EditComponents_textBox_TextChanged);
			// 
			// OK_button
			// 
			this.OK_button.Location = new System.Drawing.Point(181, 78);
			this.OK_button.Name = "OK_button";
			this.OK_button.Size = new System.Drawing.Size(75, 23);
			this.OK_button.TabIndex = 10;
			this.OK_button.Text = "OK";
			this.OK_button.UseVisualStyleBackColor = true;
			this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
			// 
			// Cancel_button
			// 
			this.Cancel_button.Location = new System.Drawing.Point(287, 78);
			this.Cancel_button.Name = "Cancel_button";
			this.Cancel_button.Size = new System.Drawing.Size(75, 23);
			this.Cancel_button.TabIndex = 11;
			this.Cancel_button.Text = "Cancel";
			this.Cancel_button.UseVisualStyleBackColor = true;
			this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
			// 
			// Method_textBox
			// 
			this.Method_textBox.BackColor = System.Drawing.SystemColors.Window;
			this.Method_textBox.Location = new System.Drawing.Point(217, 41);
			this.Method_textBox.Name = "Method_textBox";
			this.Method_textBox.ReadOnly = true;
			this.Method_textBox.Size = new System.Drawing.Size(104, 20);
			this.Method_textBox.TabIndex = 12;
			this.Method_textBox.Click += new System.EventHandler(this.Method_textBox_Click);
			// 
			// EditRowForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(516, 125);
			this.Controls.Add(this.Method_textBox);
			this.Controls.Add(this.Cancel_button);
			this.Controls.Add(this.OK_button);
			this.Controls.Add(this.EditComponents_textBox);
			this.Controls.Add(this.EditParameters_textBox);
			this.Controls.Add(this.EditComponentType_textBox);
			this.Controls.Add(this.EditCount_textBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "EditRowForm";
			this.Text = "Edit Job Data Cells";
			this.Shown += new System.EventHandler(this.EditRowFormShown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox EditCount_textBox;
		private System.Windows.Forms.TextBox EditComponentType_textBox;
		private System.Windows.Forms.TextBox EditParameters_textBox;
		private System.Windows.Forms.TextBox EditComponents_textBox;
		private System.Windows.Forms.Button OK_button;
		private System.Windows.Forms.Button Cancel_button;
		private System.Windows.Forms.TextBox Method_textBox;
	}
}
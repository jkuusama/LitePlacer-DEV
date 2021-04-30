
namespace LitePlacer
{
    partial class LatchBackoffForm
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
            this.Fix_button = new System.Windows.Forms.Button();
            this.NoFix_button = new System.Windows.Forms.Button();
            this.DontAsk_checkBox = new System.Windows.Forms.CheckBox();
            this.Messagebox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // Fix_button
            // 
            this.Fix_button.Location = new System.Drawing.Point(12, 82);
            this.Fix_button.Name = "Fix_button";
            this.Fix_button.Size = new System.Drawing.Size(123, 23);
            this.Fix_button.TabIndex = 0;
            this.Fix_button.Text = "Fix";
            this.Fix_button.UseVisualStyleBackColor = true;
            this.Fix_button.Click += new System.EventHandler(this.Fix_button_Click);
            // 
            // NoFix_button
            // 
            this.NoFix_button.Location = new System.Drawing.Point(141, 82);
            this.NoFix_button.Name = "NoFix_button";
            this.NoFix_button.Size = new System.Drawing.Size(123, 23);
            this.NoFix_button.TabIndex = 2;
            this.NoFix_button.Text = "Keep current value";
            this.NoFix_button.UseVisualStyleBackColor = true;
            this.NoFix_button.Click += new System.EventHandler(this.NoFix_button_Click);
            // 
            // DontAsk_checkBox
            // 
            this.DontAsk_checkBox.AutoSize = true;
            this.DontAsk_checkBox.Location = new System.Drawing.Point(270, 86);
            this.DontAsk_checkBox.Name = "DontAsk_checkBox";
            this.DontAsk_checkBox.Size = new System.Drawing.Size(100, 17);
            this.DontAsk_checkBox.TabIndex = 3;
            this.DontAsk_checkBox.Text = "Don\'t ask again";
            this.DontAsk_checkBox.UseVisualStyleBackColor = true;
            this.DontAsk_checkBox.CheckedChanged += new System.EventHandler(this.DontAsk_checkBox_CheckedChanged);
            // 
            // Messagebox
            // 
            this.Messagebox.Location = new System.Drawing.Point(12, 12);
            this.Messagebox.Name = "Messagebox";
            this.Messagebox.Size = new System.Drawing.Size(358, 64);
            this.Messagebox.TabIndex = 4;
            this.Messagebox.Text = "Z axis latch backoff value is rather low; current value is 1.0\nFix now (Change to" +
    " 10mm)?\n\nMore info at https://liteplacer.com/fix-for-tinyg-zlb-parameter";
            this.Messagebox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.Messagebox_LinkClicked);
            // 
            // LatchBackoffForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 114);
            this.Controls.Add(this.Messagebox);
            this.Controls.Add(this.DontAsk_checkBox);
            this.Controls.Add(this.NoFix_button);
            this.Controls.Add(this.Fix_button);
            this.Name = "LatchBackoffForm";
            this.Text = "Latch Backoff value";
            this.Load += new System.EventHandler(this.LatchBackoff_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Fix_button;
        private System.Windows.Forms.Button NoFix_button;
        private System.Windows.Forms.CheckBox DontAsk_checkBox;
        private System.Windows.Forms.RichTextBox Messagebox;
    }
}
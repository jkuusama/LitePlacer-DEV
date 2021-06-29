
namespace LitePlacer
{
    partial class CameraProperties
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
            this.ApplyExposure_button = new System.Windows.Forms.Button();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.Exposure_trackBar = new System.Windows.Forms.TrackBar();
            this.ExposureAuto_checkBox = new System.Windows.Forms.CheckBox();
            this.Status_label = new System.Windows.Forms.Label();
            this.CheckStatus_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Exposure_trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Exposure:";
            // 
            // ApplyExposure_button
            // 
            this.ApplyExposure_button.Location = new System.Drawing.Point(501, 132);
            this.ApplyExposure_button.Name = "ApplyExposure_button";
            this.ApplyExposure_button.Size = new System.Drawing.Size(75, 23);
            this.ApplyExposure_button.TabIndex = 1;
            this.ApplyExposure_button.Text = "Apply";
            this.ApplyExposure_button.UseVisualStyleBackColor = true;
            this.ApplyExposure_button.Click += new System.EventHandler(this.ApplyExposure_button_Click);
            // 
            // Cancel_button
            // 
            this.Cancel_button.Location = new System.Drawing.Point(452, 399);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 2;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(534, 399);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 3;
            this.OK_button.Text = "OK";
            this.OK_button.UseVisualStyleBackColor = true;
            // 
            // Exposure_trackBar
            // 
            this.Exposure_trackBar.Location = new System.Drawing.Point(35, 131);
            this.Exposure_trackBar.Name = "Exposure_trackBar";
            this.Exposure_trackBar.Size = new System.Drawing.Size(338, 45);
            this.Exposure_trackBar.TabIndex = 4;
            this.Exposure_trackBar.Value = 5;
            // 
            // ExposureAuto_checkBox
            // 
            this.ExposureAuto_checkBox.AutoSize = true;
            this.ExposureAuto_checkBox.Location = new System.Drawing.Point(424, 136);
            this.ExposureAuto_checkBox.Name = "ExposureAuto_checkBox";
            this.ExposureAuto_checkBox.Size = new System.Drawing.Size(48, 17);
            this.ExposureAuto_checkBox.TabIndex = 5;
            this.ExposureAuto_checkBox.Text = "Auto";
            this.ExposureAuto_checkBox.UseVisualStyleBackColor = true;
            // 
            // Status_label
            // 
            this.Status_label.AutoSize = true;
            this.Status_label.Location = new System.Drawing.Point(96, 30);
            this.Status_label.Name = "Status_label";
            this.Status_label.Size = new System.Drawing.Size(35, 13);
            this.Status_label.TabIndex = 6;
            this.Status_label.Text = "status";
            // 
            // CheckStatus_button
            // 
            this.CheckStatus_button.Location = new System.Drawing.Point(15, 25);
            this.CheckStatus_button.Name = "CheckStatus_button";
            this.CheckStatus_button.Size = new System.Drawing.Size(75, 23);
            this.CheckStatus_button.TabIndex = 7;
            this.CheckStatus_button.Text = "Check";
            this.CheckStatus_button.UseVisualStyleBackColor = true;
            this.CheckStatus_button.Click += new System.EventHandler(this.CheckStatus_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Status:";
            // 
            // CameraProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CheckStatus_button);
            this.Controls.Add(this.Status_label);
            this.Controls.Add(this.ExposureAuto_checkBox);
            this.Controls.Add(this.Exposure_trackBar);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.ApplyExposure_button);
            this.Controls.Add(this.label1);
            this.Name = "CameraProperties";
            this.Text = "CameraProperties";
            this.Shown += new System.EventHandler(this.CameraProperties_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Exposure_trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ApplyExposure_button;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.TrackBar Exposure_trackBar;
        private System.Windows.Forms.CheckBox ExposureAuto_checkBox;
        private System.Windows.Forms.Label Status_label;
        private System.Windows.Forms.Button CheckStatus_button;
        private System.Windows.Forms.Label label2;
    }
}
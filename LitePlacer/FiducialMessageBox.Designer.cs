namespace LitePlacer
{
    partial class FiducialMessageBox
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
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonManual = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.Location = new System.Drawing.Point(16, 107);
            this.buttonConfirm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(100, 28);
            this.buttonConfirm.TabIndex = 0;
            this.buttonConfirm.Text = "Confirm";
            this.toolTip1.SetToolTip(this.buttonConfirm, "Confirm visually measured location");
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // buttonRetry
            // 
            this.buttonRetry.Location = new System.Drawing.Point(232, 107);
            this.buttonRetry.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(100, 28);
            this.buttonRetry.TabIndex = 1;
            this.buttonRetry.Text = "Retry";
            this.toolTip1.SetToolTip(this.buttonRetry, "Retry visual measurement of location");
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(340, 107);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.buttonCancel, "Cancel measurement routine");
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.MinimumSize = new System.Drawing.Size(391, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 68);
            this.label1.TabIndex = 2;
            this.label1.Text = "Fiducial location OK?\r\n\r\nAlternatively to specify a manual location move the masc" +
    "hine\r\nto the desired position and confirm with \"Manual\"";
            // 
            // buttonManual
            // 
            this.buttonManual.Location = new System.Drawing.Point(124, 107);
            this.buttonManual.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonManual.Name = "buttonManual";
            this.buttonManual.Size = new System.Drawing.Size(100, 28);
            this.buttonManual.TabIndex = 1;
            this.buttonManual.Text = "Manual";
            this.toolTip1.SetToolTip(this.buttonManual, "Confirm with current CNC location\r\nThis is less accurate than the measured locati" +
        "on");
            this.buttonManual.UseVisualStyleBackColor = true;
            this.buttonManual.Click += new System.EventHandler(this.buttonManual_Click);
            // 
            // FiducialAutoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 140);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonManual);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRetry);
            this.Controls.Add(this.buttonConfirm);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(471, 187);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(471, 187);
            this.Name = "FiducialAutoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Confirm Fiducial";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FiducialAutoForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonManual;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
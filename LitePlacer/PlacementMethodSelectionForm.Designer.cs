namespace LitePlacer
{
    partial class PlacementMethodSelectionForm
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
            this.NoMethod_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PlacementMethod_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.UpCamAssist_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NoMethod_button
            // 
            this.NoMethod_button.Location = new System.Drawing.Point(296, 22);
            this.NoMethod_button.Name = "NoMethod_button";
            this.NoMethod_button.Size = new System.Drawing.Size(169, 28);
            this.NoMethod_button.TabIndex = 0;
            this.NoMethod_button.Text = "None";
            this.PlacementMethod_toolTip.SetToolTip(this.NoMethod_button, "Main method has all info needed. Up camera is not used.");
            this.NoMethod_button.UseVisualStyleBackColor = true;
            this.NoMethod_button.Click += new System.EventHandler(this.NoMethod_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "No special handling for placment";
            this.PlacementMethod_toolTip.SetToolTip(this.label1, "Main method has all info needed. Up camera is not used.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(241, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Up looking camera assisted placement";
            this.PlacementMethod_toolTip.SetToolTip(this.label2, "Takes part over the up looking camera,\r\nmeasures how the part is hanging on the n" +
        "ozzle\r\nand uses the result for placement correction.");
            // 
            // UpCamAssist_button
            // 
            this.UpCamAssist_button.Location = new System.Drawing.Point(296, 56);
            this.UpCamAssist_button.Name = "UpCamAssist_button";
            this.UpCamAssist_button.Size = new System.Drawing.Size(169, 28);
            this.UpCamAssist_button.TabIndex = 2;
            this.UpCamAssist_button.Text = "Up Cam Assisted";
            this.PlacementMethod_toolTip.SetToolTip(this.UpCamAssist_button, "Takes part over the up looking camera,\r\nmeasures how the part is hanging on the n" +
        "ozzle\r\nand uses the result for placement correction.");
            this.UpCamAssist_button.UseVisualStyleBackColor = true;
            this.UpCamAssist_button.Click += new System.EventHandler(this.UpCamAssist_button_Click);
            // 
            // PlacementMethodSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 418);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UpCamAssist_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NoMethod_button);
            this.Name = "PlacementMethodSelectionForm";
            this.Text = "Select Placement Method";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button NoMethod_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip PlacementMethod_toolTip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button UpCamAssist_button;
    }
}
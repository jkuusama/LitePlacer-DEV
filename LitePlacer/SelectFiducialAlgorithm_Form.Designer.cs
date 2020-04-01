namespace LitePlacer
{
    partial class SelectFiducialAlgorithm_Form
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
            this.Algorithms_comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Algorithms_comboBox
            // 
            this.Algorithms_comboBox.FormattingEnabled = true;
            this.Algorithms_comboBox.Location = new System.Drawing.Point(12, 12);
            this.Algorithms_comboBox.Name = "Algorithms_comboBox";
            this.Algorithms_comboBox.Size = new System.Drawing.Size(254, 21);
            this.Algorithms_comboBox.TabIndex = 0;
            this.Algorithms_comboBox.SelectedIndexChanged += new System.EventHandler(this.Algorithms_comboBox_SelectedIndexChanged);
            // 
            // SelectFiducialAlgorithm_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 51);
            this.Controls.Add(this.Algorithms_comboBox);
            this.Name = "SelectFiducialAlgorithm_Form";
            this.Text = "Select Algorithm";
            this.Shown += new System.EventHandler(this.SelectFiducialAlgorithm_Form_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox Algorithms_comboBox;
    }
}
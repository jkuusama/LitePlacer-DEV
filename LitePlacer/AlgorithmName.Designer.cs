namespace LitePlacer
{
    partial class AlgorithmNameForm
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
            this.Cancel_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.AlgorithmName_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Error_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Cancel_button
            // 
            this.Cancel_button.Location = new System.Drawing.Point(12, 95);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 1;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(148, 95);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 2;
            this.OK_button.Text = "OK";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // AlgorithmName_textBox
            // 
            this.AlgorithmName_textBox.Location = new System.Drawing.Point(12, 69);
            this.AlgorithmName_textBox.Name = "AlgorithmName_textBox";
            this.AlgorithmName_textBox.Size = new System.Drawing.Size(211, 20);
            this.AlgorithmName_textBox.TabIndex = 0;
            this.AlgorithmName_textBox.TextChanged += new System.EventHandler(this.AlgorithmName_textBox_TextChanged);
            this.AlgorithmName_textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AlgorithmName_textBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // Error_label
            // 
            this.Error_label.AutoSize = true;
            this.Error_label.ForeColor = System.Drawing.Color.Red;
            this.Error_label.Location = new System.Drawing.Point(12, 31);
            this.Error_label.Name = "Error_label";
            this.Error_label.Size = new System.Drawing.Size(104, 13);
            this.Error_label.TabIndex = 4;
            this.Error_label.Text = "Name already exists!";
            this.Error_label.Visible = false;
            // 
            // AlgorithmNameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 132);
            this.Controls.Add(this.Error_label);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AlgorithmName_textBox);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.Cancel_button);
            this.Name = "AlgorithmNameForm";
            this.Text = "New Name";
            this.Shown += new System.EventHandler(this.AlgorithmNameForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.TextBox AlgorithmName_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Error_label;
    }
}
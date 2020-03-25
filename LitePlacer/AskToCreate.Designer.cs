namespace LitePlacer
{
    partial class AskToCreate_Form
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
            this.YesToAll_button = new System.Windows.Forms.Button();
            this.Yes_button = new System.Windows.Forms.Button();
            this.No_button = new System.Windows.Forms.Button();
            this.NoToAll_button = new System.Windows.Forms.Button();
            this.Message_TextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // YesToAll_button
            // 
            this.YesToAll_button.Location = new System.Drawing.Point(12, 58);
            this.YesToAll_button.Name = "YesToAll_button";
            this.YesToAll_button.Size = new System.Drawing.Size(75, 23);
            this.YesToAll_button.TabIndex = 0;
            this.YesToAll_button.Text = "Yes to All";
            this.YesToAll_button.UseVisualStyleBackColor = true;
            this.YesToAll_button.Click += new System.EventHandler(this.YesToAll_button_Click);
            // 
            // Yes_button
            // 
            this.Yes_button.Location = new System.Drawing.Point(93, 58);
            this.Yes_button.Name = "Yes_button";
            this.Yes_button.Size = new System.Drawing.Size(75, 23);
            this.Yes_button.TabIndex = 1;
            this.Yes_button.Text = "Yes";
            this.Yes_button.UseVisualStyleBackColor = true;
            this.Yes_button.Click += new System.EventHandler(this.Yes_button_Click);
            // 
            // No_button
            // 
            this.No_button.Location = new System.Drawing.Point(186, 58);
            this.No_button.Name = "No_button";
            this.No_button.Size = new System.Drawing.Size(75, 23);
            this.No_button.TabIndex = 2;
            this.No_button.Text = "No";
            this.No_button.UseVisualStyleBackColor = true;
            this.No_button.Click += new System.EventHandler(this.No_button_Click);
            // 
            // NoToAll_button
            // 
            this.NoToAll_button.Location = new System.Drawing.Point(267, 58);
            this.NoToAll_button.Name = "NoToAll_button";
            this.NoToAll_button.Size = new System.Drawing.Size(75, 23);
            this.NoToAll_button.TabIndex = 3;
            this.NoToAll_button.Text = "No to All";
            this.NoToAll_button.UseVisualStyleBackColor = true;
            this.NoToAll_button.Click += new System.EventHandler(this.NoToAll_button_Click);
            // 
            // Message_label
            // 
            this.Message_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Message_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Message_TextBox.Location = new System.Drawing.Point(12, 12);
            this.Message_TextBox.Multiline = true;
            this.Message_TextBox.Name = "Message_label";
            this.Message_TextBox.ReadOnly = true;
            this.Message_TextBox.Size = new System.Drawing.Size(330, 35);
            this.Message_TextBox.TabIndex = 4;
            this.Message_TextBox.Text = "Video algorithm xxx does not exist.\r\nCreate an empty algorithm with that name?";
            // 
            // AskToCreate_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 93);
            this.Controls.Add(this.Message_TextBox);
            this.Controls.Add(this.NoToAll_button);
            this.Controls.Add(this.No_button);
            this.Controls.Add(this.Yes_button);
            this.Controls.Add(this.YesToAll_button);
            this.Name = "AskToCreate_Form";
            this.Text = "Add Algorithm?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button YesToAll_button;
        private System.Windows.Forms.Button Yes_button;
        private System.Windows.Forms.Button No_button;
        private System.Windows.Forms.Button NoToAll_button;
        public System.Windows.Forms.TextBox Message_TextBox;
    }
}
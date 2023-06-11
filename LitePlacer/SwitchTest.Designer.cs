namespace LitePlacer
{
    partial class SwitchTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchTest));
            this.label1 = new System.Windows.Forms.Label();
            this.Xmin_textBox = new System.Windows.Forms.TextBox();
            this.Ymin_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Zmin_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Zmax_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Close_button = new System.Windows.Forms.Button();
            this.SwitchStatus_timer = new System.Windows.Forms.Timer(this.components);
            this.Test_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(34, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "X min:";
            // 
            // Xmin_textBox
            // 
            this.Xmin_textBox.BackColor = System.Drawing.Color.Red;
            this.Xmin_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Xmin_textBox.Location = new System.Drawing.Point(148, 42);
            this.Xmin_textBox.Name = "Xmin_textBox";
            this.Xmin_textBox.ReadOnly = true;
            this.Xmin_textBox.Size = new System.Drawing.Size(124, 44);
            this.Xmin_textBox.TabIndex = 1;
            this.Xmin_textBox.TabStop = false;
            this.Xmin_textBox.Text = "On";
            this.Xmin_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Ymin_textBox
            // 
            this.Ymin_textBox.BackColor = System.Drawing.Color.Red;
            this.Ymin_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Ymin_textBox.Location = new System.Drawing.Point(148, 92);
            this.Ymin_textBox.Name = "Ymin_textBox";
            this.Ymin_textBox.ReadOnly = true;
            this.Ymin_textBox.Size = new System.Drawing.Size(124, 44);
            this.Ymin_textBox.TabIndex = 3;
            this.Ymin_textBox.TabStop = false;
            this.Ymin_textBox.Text = "On";
            this.Ymin_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(34, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 37);
            this.label2.TabIndex = 2;
            this.label2.Text = "Y min:";
            // 
            // Zmin_textBox
            // 
            this.Zmin_textBox.BackColor = System.Drawing.Color.Red;
            this.Zmin_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zmin_textBox.Location = new System.Drawing.Point(148, 142);
            this.Zmin_textBox.Name = "Zmin_textBox";
            this.Zmin_textBox.ReadOnly = true;
            this.Zmin_textBox.Size = new System.Drawing.Size(124, 44);
            this.Zmin_textBox.TabIndex = 5;
            this.Zmin_textBox.TabStop = false;
            this.Zmin_textBox.Text = "On";
            this.Zmin_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(34, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 37);
            this.label3.TabIndex = 4;
            this.label3.Text = "Z min:";
            // 
            // Zmax_textBox
            // 
            this.Zmax_textBox.BackColor = System.Drawing.Color.LightGreen;
            this.Zmax_textBox.Enabled = false;
            this.Zmax_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zmax_textBox.Location = new System.Drawing.Point(148, 192);
            this.Zmax_textBox.Name = "Zmax_textBox";
            this.Zmax_textBox.ReadOnly = true;
            this.Zmax_textBox.Size = new System.Drawing.Size(124, 44);
            this.Zmax_textBox.TabIndex = 7;
            this.Zmax_textBox.TabStop = false;
            this.Zmax_textBox.Text = "Off";
            this.Zmax_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Enabled = false;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(26, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 37);
            this.label4.TabIndex = 6;
            this.label4.Text = "Z max:";
            // 
            // Close_button
            // 
            this.Close_button.Location = new System.Drawing.Point(105, 258);
            this.Close_button.Name = "Close_button";
            this.Close_button.Size = new System.Drawing.Size(75, 23);
            this.Close_button.TabIndex = 8;
            this.Close_button.Text = "Close";
            this.Close_button.UseVisualStyleBackColor = true;
            this.Close_button.Click += new System.EventHandler(this.Close_button_Click);
            // 
            // SwitchStatus_timer
            // 
            this.SwitchStatus_timer.Tick += new System.EventHandler(this.SwitchStatus_timer_Tick);
            // 
            // Test_button
            // 
            this.Test_button.Location = new System.Drawing.Point(197, 258);
            this.Test_button.Name = "Test_button";
            this.Test_button.Size = new System.Drawing.Size(75, 23);
            this.Test_button.TabIndex = 9;
            this.Test_button.Text = "Test once";
            this.Test_button.UseVisualStyleBackColor = true;
            this.Test_button.Click += new System.EventHandler(this.Test_button_Click);
            // 
            // SwitchTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 311);
            this.Controls.Add(this.Test_button);
            this.Controls.Add(this.Close_button);
            this.Controls.Add(this.Zmax_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Zmin_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Ymin_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Xmin_textBox);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SwitchTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Switch Test";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Xmin_textBox;
        private System.Windows.Forms.TextBox Ymin_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Zmin_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Zmax_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Close_button;
        private System.Windows.Forms.Timer SwitchStatus_timer;
        private System.Windows.Forms.Button Test_button;
    }
}
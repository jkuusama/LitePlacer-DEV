using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LitePlacer
{
    public partial class NonModalDialog : Form
    {
        static FormMain MainForm; 
        public string HeaderString { get; set; } = "";
        public string MessageString { get; set; } = "";
        public string Button1Txt { get; set; } = "";
        public string Button2Txt { get; set; } = "";
        public string Button3Txt { get; set; } = "";

        public NonModalDialog(FormMain MainF, string Button1Txt, string Button2Txt, string Button3Txt)
        {
            MainForm = MainF;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.NonModalMessageBox_result = Button1Txt;
            MainForm.MessageboxDone = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm.NonModalMessageBox_result = Button2Txt;
            MainForm.MessageboxDone = true;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MainForm.NonModalMessageBox_result = Button3Txt;
            MainForm.MessageboxDone = true;
            this.Close();
        }

        private void NonModalDialog_Load(object sender, EventArgs e)
        {
            CenterToParent();
            this.Text = HeaderString;
            button1.Text = Button1Txt;
            button2.Text = Button2Txt;
            button3.Text = Button3Txt;
            Message_textBox.Text = MessageString;
            if (Button1Txt == "")
            {
                button1.Enabled = false;
                button1.Visible = false;
            }
            if (Button2Txt == "")
            {
                button2.Enabled = false;
                button2.Visible = false;
            }
            if (Button3Txt == "")
            {
                button3.Enabled = false;
                button3.Visible = false;
            }
        }
    }
}

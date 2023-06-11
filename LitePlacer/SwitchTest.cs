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
    public partial class SwitchTest : Form
    {
        public FormMain MainForm { get; set; }

        public SwitchTest(FormMain main)
        {
            InitializeComponent();
            MainForm = main;
            SwitchStatus_timer.Start();
            MainForm.Cnc.Marlin.LogCommunication = false;
            MainForm.DisplayText("Switch Test window open (comms suspended)");
        }

        private void Close_button_Click(object sender, EventArgs e)
        {
            SwitchStatus_timer.Stop();
            MainForm.Cnc.Marlin.LogCommunication = true;
            MainForm.DisplayText("Switch Test window open (comms restored)");
            Close();
        }

        private void SwitchStatus_timer_Tick(object sender, EventArgs e)
        {
            ShowStatuses();
        }

        private void Test_button_Click(object sender, EventArgs e)
        {
            ShowStatuses();
        }

        private void ShowStatuses()
        {
            List<int> Statuses = new List<int>();
            if (MainForm.Cnc.GetEndStopStatuses(out Statuses, false))
            {
                ShowState(Xmin_textBox, Statuses[0]);
                ShowState(Ymin_textBox, Statuses[2]);
                ShowState(Zmin_textBox, Statuses[4]);
                ShowState(Zmax_textBox, Statuses[5]);
            }
            else
            {
                ShowError();
            }
        }

        private void ShowState (TextBox box, int state)
        {
            if (state == 1)
            {
                box.BackColor = Color.LightGreen;
                box.Text = "Off";
            }
            else if (state==0)
            {
                box.BackColor = Color.Red;
                box.Text = "On";
            }
            else
            {
                box.BackColor = Color.LightGray;
                box.Text = "---";
            }
        }

        private void ShowError()
        {
            Xmin_textBox.Text = "Err";
            Xmin_textBox.BackColor = Color.LightGray;
            Ymin_textBox.Text = "Err";
            Ymin_textBox.BackColor = Color.LightGray;
            Zmin_textBox.Text = "Err";
            Zmin_textBox.BackColor = Color.LightGray;
            Zmax_textBox.Text = "Err";
            Zmax_textBox.BackColor = Color.LightGray;
            SwitchStatus_timer.Stop();
        }
    }
}

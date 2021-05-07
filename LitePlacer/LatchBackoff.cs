using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;


namespace LitePlacer
{
    public partial class LatchBackoffForm : Form
    {
        public LatchBackoffForm()
        {
            InitializeComponent();
        }

        public string zlb_now { get; set; }
        public FormMain MainForm { get; set; }

        private void LatchBackoff_Load(object sender, EventArgs e)
        {
            Messagebox.Text = "Z axis latch backoff value is rather low. Current value is: " +
               zlb_now +
               ".\r\nFix now (Change to 10mm)?" + "\r\n\r\n" +
               "More info at https://liteplacer.com/fix-for-tinyg-zlb-parameter/";
        }

        private void Fix_button_Click(object sender, EventArgs e)
        {
            if (!MainForm.Cnc.Write_m("{\"zlb\",10}"))
            {
                MainForm.DisplayText("Latch backoff write failed", KnownColor.DarkRed, true);
                Close();
            }
            if (!MainForm.CNC_Z_m(10))
            {
                MainForm.DisplayText("Making room for Z re-homing failed", KnownColor.DarkRed, true);
                Close();
            }
            if (!MainForm.HomeZ_m())
            {
                MainForm.DisplayText("Z re-homing failed", KnownColor.DarkRed, true);
                Close();
            }
            Close();
        }

        private void NoFix_button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DontAsk_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.Setting.General_ZlbFixAsked = true;
        }

        private void Messagebox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            LaunchWeblink(e.LinkText);
        }

        // from https://stackoverflow.com/questions/321037/links-in-c-sharp-textbox:
        // Performs the actual browser launch to follow link:
        private void LaunchWeblink(string url)
        {
            if (IsHttpURL(url)) Process.Start(url);
        }

        // Simple check to make sure link is valid,
        // can be modified to check for other protocols:
        private bool IsHttpURL(string url)
        {
            return
                ((!string.IsNullOrWhiteSpace(url)) &&
                (url.ToLower().StartsWith("http")));
        }
    }
}

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
    public partial class PlacementMethodSelectionForm : Form
    {
        static FormMain MainForm;
        public PlacementMethodSelectionForm(FormMain MainF)
        {
            InitializeComponent();
            MainForm = MainF;
        }

        private void NoMethod_button_Click(object sender, EventArgs e)
        {
            MainForm.PlacementMethod = "--";
            this.Close();
        }

        private void UpCamAssist_button_Click(object sender, EventArgs e)
        {
            MainForm.PlacementMethod = "Up Cam Assisted";
            this.Close();
        }
    }
}

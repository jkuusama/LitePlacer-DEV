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
    public partial class SelectFiducialAlgorithm_Form : Form
    {
        public static FormMain MainForm { get; set; }

        public SelectFiducialAlgorithm_Form(FormMain MainF)
        {
            MainForm = MainF;
            InitializeComponent();
        }

        private bool OKtoClose = false;

        private void Algorithms_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.SelectFiducialAlgorithm_FormResult = Algorithms_comboBox.SelectedItem.ToString();
            if (OKtoClose)
            {
                Close();
            }
        }

        private void SelectFiducialAlgorithm_Form_Shown(object sender, EventArgs e)
        {
            for (int i = 1; i < MainForm.VideoAlgorithms.AllAlgorithms.Count; i++)
            {
                Algorithms_comboBox.Items.Add(MainForm.VideoAlgorithms.AllAlgorithms[i].Name);
                if (MainForm.SelectFiducialAlgorithm_FormResult == MainForm.VideoAlgorithms.AllAlgorithms[i].Name)
                {
                    Algorithms_comboBox.SelectedIndex = i-1;
                }
            }
            if (Algorithms_comboBox.SelectedItem==null)
            {
                // nothing got selected above
                Algorithms_comboBox.SelectedIndex = 0;
            }
            OKtoClose = true;
        }
    }
}

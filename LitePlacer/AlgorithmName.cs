using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LitePlacer
{
    public partial class AlgorithmNameForm : Form
    {
        public List<VideoAlgorithmsCollection.FullAlgorithmDescription> Algorithms;
        public bool OK = false;
        public bool Renaming = false; // to avoid the already exists error on rename
        public string NewName = "";
        private string IntialName;
          

        public AlgorithmNameForm(string Name)
        {
            InitializeComponent();
            IntialName = Name;
        }


        private void AlgorithmNameForm_Shown(object sender, EventArgs e)
        {
            AlgorithmName_textBox.Text = IntialName;
        }

        private void AlgorithmName_textBox_TextChanged(object sender, EventArgs e)
        {
            if (AlgorithmExists(AlgorithmName_textBox.Text))
            {
                OK_button.Enabled = false;
                Error_label.Visible = true;
            }
            else
            {
                OK_button.Enabled = true;
                Error_label.Visible = false;
                NewName = AlgorithmName_textBox.Text;
            }
        }

        private bool AlgorithmExists(string NewName)
        {
            // Check for validity of an algorithm name:
            // If creating a new one, no copies can exist
            // If renaming, you can't rename to an algorithm that exists,
            // but you can rename to original name

            foreach (VideoAlgorithmsCollection.FullAlgorithmDescription Algorithm in Algorithms)
            {
                if (Algorithm.Name== NewName)
                {
                    if (!Renaming)
                    {
                        return true;    // not renaming: no copies allowed
                    }
                    if (NewName!= IntialName)
                    {
                        return true;    // renaming, but not to the origal name. Bad.
                    }
                }
            }
            return false;
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            OK = true;
            this.Close();
        }

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            OK = false;
            Close();
        }

        private void AlgorithmName_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (OK_button.Enabled)
                {
                    OK = true;
                    Close();
                }
            }
        }

    }
}

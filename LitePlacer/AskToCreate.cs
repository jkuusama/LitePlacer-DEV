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
    public partial class AskToCreate_Form : Form
    {
        public AskToCreate_Form()
        {
            InitializeComponent();
        }

        public bool YesToAll { get; set; } = false;
        public bool Yes { get; set; } = false;
        public bool No { get; set; } = false;
        public bool NoToAll { get; set; } = false;

        private void YesToAll_button_Click(object sender, EventArgs e)
        {
            YesToAll = true;
            Close();
        }

        private void Yes_button_Click(object sender, EventArgs e)
        {
            Yes = true;
            Close();
        }

        private void No_button_Click(object sender, EventArgs e)
        {
            No = true;
            Close();
        }

        private void NoToAll_button_Click(object sender, EventArgs e)
        {
            NoToAll = true;
            Close();
        }
    }
}

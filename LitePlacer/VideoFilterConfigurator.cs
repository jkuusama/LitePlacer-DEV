using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LitePlacer {
    public partial class VideoFilterConfigurator : Form {
        private AForgeFunctionSet sets;
        private BindingList<AForgeFunction> currentBinding = new BindingList<AForgeFunction>();

        public VideoFilterConfigurator(AForgeFunctionSet functions) {
            InitializeComponent();
            sets = functions;

            //fill combobox
            BindingSource bs = new BindingSource();
            bs.DataSource = sets.GetNames();
            FilterSetSelectorComboBox.DataSource = bs;

            //bind editor values
            Display_dataGridView.DataSource = currentBinding;
            methodDataGridViewTextBoxColumn.DataSource = Enum.GetValues(typeof(AForgeMethod));
        }

        private void AddCamFunction_button_Click(object sender, EventArgs e) {
            currentBinding.Add(new AForgeFunction());
        }

        private void FilterSetSelectorComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            var cb = (ComboBox)sender;
            var list = sets.GetSet(cb.SelectedText);
            if (list != null) {
                // make a local copy here
                currentBinding = AForgeFunction.Clone(list); 
            }
        }

        private void button_saveSettings_Click(object sender, EventArgs e) {

        }


    }
}


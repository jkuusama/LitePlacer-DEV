using System;
using System.Windows.Forms;

namespace LitePlacer
{
	public partial class MethodSelectionForm : Form
	{
		public string SelectedMethod = "";
		public bool ShowCheckBox = false;
		public string HeaderString = "";

		public MethodSelectionForm()		{
			InitializeComponent();
		}


		private void button_Click(object sender, EventArgs e)		{
            SelectedMethod = ((Button)sender).Text;
			Close();
		}


	}
}

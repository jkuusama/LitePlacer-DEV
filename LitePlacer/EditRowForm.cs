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
	public partial class EditRowForm : Form
	{
		public EditRowForm()
		{
			InitializeComponent();
		}

		private void EditRowFormShown(object sender, EventArgs e)
		{
			EditComponentType_textBox.Text = ComponentType_text;
			Method_textBox.Text = Method;
			EditParameters_textBox.Text = Parameters_text;
			EditComponents_textBox.Text = Components_text;
			if(Components_text!="")
			{
				List<String> Line = FormMain.SplitCSV(EditComponents_textBox.Text);
				EditCount_textBox.Text = Line.Count.ToString();
			}
			if (ComponentType_text != "")
			{
				EditComponentType_textBox.Focus();
			}
			else if (Parameters_text != "")
			{
				EditParameters_textBox.Focus();
			}
			else if (Components_text != "")
			{
				EditComponents_textBox.Focus();
			}
			else
			{
				Method_textBox.Focus();
			}
		}

		public bool CountEnable
		{
			get { return EditCount_textBox.Enabled; }
			set { EditCount_textBox.Enabled = value; }
		}

		public bool ComponentTypeEnable
		{
			get { return EditComponentType_textBox.Enabled; }
			set { EditComponentType_textBox.Enabled = value; }
		}

		public bool MethodEnable
		{
			get { return Method_textBox.Enabled; }
			set { Method_textBox.Enabled = value; }
		}

		public bool ParametersEnable
		{
			get { return EditParameters_textBox.Enabled; }
			set { EditParameters_textBox.Enabled = value; }
		}

		public bool ComponentsEnable
		{
			get { return EditComponents_textBox.Enabled; }
			set { EditComponents_textBox.Enabled = value; }
		}

		public string Result = "Cancel";

		private void OK_button_Click(object sender, EventArgs e)
		{
			Result = "OK";
			this.Close();
		}

		private void Cancel_button_Click(object sender, EventArgs e)
		{
			Result = "Cancel";
			this.Close();
		}

		public string Count_text = "";

		public string ComponentType_text = "";
		private void EditComponentType_textBox_TextChanged(object sender, EventArgs e)
		{
			ComponentType_text = EditComponentType_textBox.Text;
		}

		public string Parameters_text = "";
		private void EditParameters_textBox_TextChanged(object sender, EventArgs e)
		{
			Parameters_text = EditParameters_textBox.Text;
		}

		public bool CountChanged = false;

		private int ccount= 0;
		public int ComponentCount
		{
			get { return ccount; }
			set { ccount = value; }
		}
		public string Components_text = "";
		private void EditComponents_textBox_TextChanged(object sender, EventArgs e)
		{
			Components_text = EditComponents_textBox.Text;
			List<String> Line = FormMain.SplitCSV(EditComponents_textBox.Text);
			ComponentCount = Line.Count;
			CountChanged = true;
			EditCount_textBox.Text = Line.Count.ToString();
		}


		// Possible values for Method are:
		// 0: "?"
		// 1: "Place"
		// 2: "Change needle"
		// 3: "Recalibrate"
		// 4: "Ignore"
		// 5: "Pause"
		// 6: "Fiducials"

		private string _mstring = "?";
		public string Method
		{
			get { return _mstring; }
			set 
			{
				if ((value == "?") ||
					 (value == "Place") ||
					 (value == "Change needle") ||
					 (value == "Recalibrate") ||
					 (value == "Ignore") ||
					 (value == "Pause") ||
					 (value == "Fiducials"))
				{
					_mstring = value;
					Method_textBox.Text = value;
				}
			}
		}



		private void Method_textBox_Click(object sender, EventArgs e)
		{
			if (!Method_textBox.Enabled)
			{
				return;
			};
			MethodSelectionForm MethodDialog = new MethodSelectionForm();
			MethodDialog.ShowDialog(this);
			Method_textBox.Text = MethodDialog.SelectedMethod;
			Method = MethodDialog.SelectedMethod;
			MethodDialog.Dispose();
		}

		private void EditRowFormShown()
		{

		}


	}
}

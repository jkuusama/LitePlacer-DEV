using System;
using System.Globalization;
using System.Windows.Forms;

namespace LitePlacer
{
    class NumericIntBox : TextBox
    {
        private float value = 0;

        public float Value { get => value; set { this.value = value; OnValueChanged(); } }

        public event EventHandler ValueChanged;

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }

        public NumericIntBox()
        {
            TextChanged += NumericIntBox_TextChanged;
            LostFocus += NumericIntBox_LostFocus;
            KeyDown += NumericIntBox_KeyDown;
        }

        private void NumericIntBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Text = Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void NumericIntBox_LostFocus(object sender, EventArgs e)
        {
            Text = Value.ToString(CultureInfo.InvariantCulture);
        }

        private void NumericIntBox_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out float tmp))
            {
                if (tmp != Value)
                {
                    Value = tmp;
                }
                ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    class NumericFloatBox : TextBox
    {
        private float value = 0;

        public float Value { get => value; set { this.value = value; OnValueChanged(); } }

        public event EventHandler ValueChanged;

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, new EventArgs());
        }

        public NumericFloatBox()
        {
            TextChanged += NumericFloatBox_TextChanged;
            LostFocus += NumericFloatBox_LostFocus;
            KeyDown += NumericFloatBox_KeyDown;
        }

        private void NumericFloatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Text = Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void NumericFloatBox_LostFocus(object sender, EventArgs e)
        {
            Text = Value.ToString(CultureInfo.InvariantCulture);
        }

        private void NumericFloatBox_TextChanged(object sender, EventArgs e)
        {
            if (float.TryParse(Text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float tmp))
            {
                if (tmp != Value)
                {
                    Value = tmp;
                }
                ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}

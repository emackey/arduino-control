using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoControl;

namespace ArduinoControlPanel
{
    public partial class AnalogControl : UserControl
    {
        public ArduinoPin Pin { get; private set; }

        public AnalogControl(ArduinoPin pin)
        {
            Pin = pin;
            InitializeComponent();
            groupBoxAnalog.Text = pin.Name;
            trackBarAnalog.Value = pin.Value;
            textBoxAnalog.Text = pin.Value.ToString();
        }

        private void trackBarAnalog_Scroll(object sender, EventArgs e)
        {
            int value = trackBarAnalog.Value;
            if (Pin.Value != value)
            {
                Pin.Value = value;
                textBoxAnalog.Text = value.ToString();
            }
        }

        private void textBoxAnalog_TextChanged(object sender, EventArgs e)
        {
            int value;
            if (int.TryParse(textBoxAnalog.Text, out value))
            {
                if ((Pin.Value != value) && (value >= 0) && (value <= 255))
                {
                    Pin.Value = value;
                    trackBarAnalog.Value = value;
                }
            }
        }
    }
}

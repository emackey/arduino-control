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
        private Form1 m_parent;

        public AnalogControl(ArduinoPin pin, Form1 parent)
        {
            m_parent = parent;
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
                m_parent.AnalogControlChangedManually();
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
                    m_parent.AnalogControlChangedManually();
                }
            }
        }

        public int PresetValue
        {
            get { return Pin.Value; }
            set
            {
                if ((Pin.Value != value) && (value >= 0) && (value <= 255))
                {
                    Pin.Value = value;
                    trackBarAnalog.Value = value;
                    textBoxAnalog.Text = value.ToString();
                }
            }
        }
    }
}

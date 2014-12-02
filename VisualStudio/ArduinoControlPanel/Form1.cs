using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoControl;

namespace ArduinoControlPanel
{
    public partial class Form1 : Form
    {
        const string NO_CONNECT = "Disconnected";

        private bool m_isUpdatingAvailability;
        private ArduinoPort m_port;

        private delegate void ArduinoPinsDelegate();

        public Form1()
        {
            m_isUpdatingAvailability = false;
            InitializeComponent();

            comboBoxCOM.Items.Add(NO_CONNECT);
            comboBoxCOM.SelectedIndex = 0;
            UpdateSerialAvailability();

            timerSerialAvailability.Start();
        }

        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            var portName = comboBoxCOM.SelectedItem.ToString();

            if (m_port != null)
            {
                if (m_port.PortName.Equals(portName))
                {
                    return;
                }
                ReleasePort();
            }

            if (!portName.Equals(NO_CONNECT))
            {
                m_port = new ArduinoPort(portName, 9600);
                m_port.ArduinoPinsAvailable += OnArduinoPinsAvailable;
            }
        }

        private void OnArduinoPinsAvailable(object sender, EventArgs e)
        {
            // NOTE: For now, this event happens on a subthread, so we must BeginInvoke.
            BeginInvoke(new ArduinoPinsDelegate(OnMainThreadArduinoPinsAvailable));
        }

        private void OnMainThreadArduinoPinsAvailable()
        {
            int index = 0;
            foreach (var pin in m_port.ArduinoPins)
            {
                if (pin.Mode == ArduinoPinMode.AnalogOut)
                {
                    var control = new AnalogControl(pin);
                    int width = control.Size.Width;
                    control.Location = new Point(index * width + 10, 0);
                    control.TabIndex = index + 100;
                    control.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                    control.Size = new Size(width, panelForControls.Size.Height - 5);
                    panelForControls.Controls.Add(control);
                    ++index;
                }
            }
        }

        private void UpdateSerialAvailability()
        {
            if (m_isUpdatingAvailability)
            {
                return;
            }

            m_isUpdatingAvailability = true;
            var availablePorts = SerialPort.GetPortNames().ToList();
            var toRemove = new List<string>();

            foreach (string portName in comboBoxCOM.Items)
            {
                if (!portName.Equals(NO_CONNECT))
                {
                    if (availablePorts.Contains(portName))
                    {
                        availablePorts.Remove(portName);
                    }
                    else
                    {
                        toRemove.Add(portName);
                    }
                }
            }

            foreach (string portName in toRemove)
            {
                if (comboBoxCOM.SelectedItem.ToString().Equals(portName))
                {
                    comboBoxCOM.SelectedIndex = 0;
                }
                comboBoxCOM.Items.Remove(portName);
            }

            comboBoxCOM.Items.AddRange(availablePorts.ToArray());
            m_isUpdatingAvailability = false;
        }

        private void timerSerialAvailability_Tick(object sender, EventArgs e)
        {
            UpdateSerialAvailability();
        }

        private void ReleasePort()
        {
            m_port.ArduinoPinsAvailable -= OnArduinoPinsAvailable;
            panelForControls.Controls.Clear();
            m_port.Dispose();
            m_port = null;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerSerialAvailability.Stop();
            if (m_port != null)
            {
                ReleasePort();
            }
        }
    }
}

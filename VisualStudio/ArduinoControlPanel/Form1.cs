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
using System.Management;

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

            comboBoxCOM.Items.Add(new PortDescription(NO_CONNECT));
            comboBoxCOM.SelectedIndex = 0;
            UpdateSerialAvailability();

            timerSerialAvailability.Start();
        }

        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            var portName = ((PortDescription)comboBoxCOM.SelectedItem).Name;

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
            var availableDescriptions = availablePorts.Select(p => new PortDescription(p)).ToList();
            var toRemove = new List<string>();

            try
            {
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/331a26c1-0f42-4cf1-8adb-32fb09a18953/how-to-get-fullname-from-available-serial-port-com-port-from-device-manager-in-windows-form-with?forum=vbgeneral
                var searcher = new ManagementObjectSearcher("root\\cimv2", "SELECT * FROM Win32_SerialPort");
                var collection = searcher.Get();
                foreach (var queryObject in collection)
                {
                    //var fullObject = queryObject.GetText(TextFormat.Mof);
                    var portName = queryObject["DeviceID"].ToString();
                    if (availablePorts.Contains(portName))
                    {
                        var availableDescription = availableDescriptions.First(p => p.Name.Equals(portName));
                        availableDescription.Description = queryObject["Name"].ToString();
                    }
                }
            }
            catch
            {
            }

            foreach (PortDescription portDescription in comboBoxCOM.Items)
            {
                var portName = portDescription.Name;
                if (!portName.Equals(NO_CONNECT))
                {
                    if (availablePorts.Contains(portName))
                    {
                        var availableDescription = availableDescriptions.First(p => p.Name.Equals(portName));
                        portDescription.Description = availableDescription.Description;
                        availablePorts.Remove(portName);
                        availableDescriptions.Remove(availableDescription);
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
                PortDescription toRemovePort = null;
                foreach (PortDescription portDescription in comboBoxCOM.Items)
                {
                    if (portDescription.Name.Equals(portName))
                    {
                        toRemovePort = portDescription;
                        break;
                    }
                }
                if (toRemove != null)
                {
                    comboBoxCOM.Items.Remove(toRemovePort);
                }
            }

            comboBoxCOM.Items.AddRange(availableDescriptions.ToArray());
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

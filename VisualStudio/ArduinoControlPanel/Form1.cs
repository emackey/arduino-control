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
        const int BaudRate = 19200;  // default is 9600.

        private bool m_isUpdatingAvailability;
        private ArduinoPort m_port;
        private Dictionary<string, Dictionary<string, int>> m_savedPresets;
        private NamesFromSpeech m_namesFromSpeech = null;

        private delegate void ArduinoPinsDelegate();

        public Form1()
        {
            m_savedPresets = new Dictionary<string, Dictionary<string, int>>();
            m_isUpdatingAvailability = false;
            InitializeComponent();

            comboBoxPresets.SelectedIndex = 0;

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
                m_port = new ArduinoPort(portName, BaudRate);
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
                    var control = new AnalogControl(pin, this);
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

        private void comboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboBoxPresets.SelectedIndex;

            if (index == 1)
            {
                var dialog = new NewPreset();
                if ((dialog.ShowDialog() != DialogResult.OK) || (string.IsNullOrWhiteSpace(dialog.PresetName)))
                {
                    comboBoxPresets.SelectedIndex = 0;
                }
                else
                {
                    string name = dialog.PresetName;
                    SavePreset(name);
                    if (comboBoxPresets.Items.Contains(name))
                    {
                        comboBoxPresets.Items.Remove(name);
                    }
                    comboBoxPresets.Items.Add(name);
                    comboBoxPresets.SelectedIndex = comboBoxPresets.Items.Count - 1;
                    buttonSpeech.Enabled = true;
                }
            }
            else if (index > 1)
            {
                LoadPreset(comboBoxPresets.SelectedItem.ToString());
            }
        }

        public void AnalogControlChangedManually()
        {
            if (comboBoxPresets.SelectedIndex != 0)
            {
                comboBoxPresets.SelectedIndex = 0;
            }
        }

        private void SavePreset(string name)
        {
            var preset = new Dictionary<string, int>();
            foreach (var control in panelForControls.Controls)
            {
                var analogControl = control as AnalogControl;
                if (analogControl != null)
                {
                    preset.Add(analogControl.Pin.Name, analogControl.PresetValue);
                }
            }
            if (m_savedPresets.ContainsKey(name))
            {
                m_savedPresets.Remove(name);
            }
            m_savedPresets.Add(name, preset);
            if (m_namesFromSpeech != null)
            {
                m_namesFromSpeech.AddName(name);
            }
        }

        private void LoadPreset(string name)
        {
            Dictionary<string, int> preset;
            if (m_savedPresets.TryGetValue(name, out preset))
            {
                foreach (var control in panelForControls.Controls)
                {
                    var analogControl = control as AnalogControl;
                    if (analogControl != null)
                    {
                        int value;
                        if (preset.TryGetValue(analogControl.Pin.Name, out value))
                        {
                            analogControl.PresetValue = value;
                        }
                    }
                }
            }
        }

        public void onSpeech(string name)
        {
            int pos = comboBoxPresets.Items.IndexOf(name);
            if (pos > 1)
            {
                comboBoxPresets.SelectedIndex = pos;
            }
        }

        private void buttonSpeech_Click(object sender, EventArgs e)
        {
            if (m_namesFromSpeech == null)
            {
                m_namesFromSpeech = new NamesFromSpeech(this, m_savedPresets.Select(p => p.Key).ToList());
                buttonSpeech.Visible = false;
            }
        }
    }
}

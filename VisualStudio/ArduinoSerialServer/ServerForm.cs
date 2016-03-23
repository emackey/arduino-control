using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoSerialServer
{
    public partial class ServerForm : Form
    {
        const string NO_CONNECT = "Disconnected";

        private SerialPort m_serialPort;
        private bool m_isUpdatingAvailability;

        // Delegates for passing messages to the UI thread
        private delegate void AddMessageDelegate(string message);
        private AddMessageDelegate m_addMessage;
        private delegate void SendSerialDataDelegate(string data);
        private SendSerialDataDelegate m_sendSerialData;

        public ServerForm()
        {
            InitializeComponent();

            // Initialize the delagates to point to this instance.
            m_addMessage = new AddMessageDelegate(AddMessageInternal);
            m_sendSerialData = new SendSerialDataDelegate(SendSerialDataInternal);

            comboBoxCOM.Items.Add(new PortDescription(NO_CONNECT));
            comboBoxCOM.SelectedIndex = 0;
            UpdateSerialAvailability();

            timerSerialAvailability.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddMessageInternal(string message)
        {
            textBoxLog.AppendText(message + "\r\n");
        }

        /// <summary>
        /// Other threads may call this to post a message to the log on the UI thread.
        /// </summary>
        /// <param name="message">The message to post</param>
        public void AddMessage(string message)
        {
            this.BeginInvoke(m_addMessage, new object[] { message });
        }

        private void SendSerialDataInternal(string data)
        {
            if (m_serialPort != null)
            {
                m_serialPort.Write(data);
            }
        }

        /// <summary>
        /// Other threads may call this to send data to the serial port.
        /// </summary>
        /// <param name="data">The data to send</param>
        public void SendSerialData(string data)
        {
            this.BeginInvoke(m_sendSerialData, new object[] { data });
        }

        private void ConnectSerialPort(string portName)
        {
            int networkPortNumber;
            try
            {
                networkPortNumber = int.Parse(textBoxPortNumber.Text);
            }
            catch
            {
                MessageBox.Show("Invalid network port number.");
                return;
            }
            m_serialPort = new SerialPort(portName, 9600);
            m_serialPort.DataReceived += SerialPort_DataReceived;
            m_serialPort.Open();
            m_serialPort.DtrEnable = true;

            comboBoxCOM.Enabled = false;
            textBoxPortNumber.Enabled = false;
            timerSerialAvailability.Stop();
            SocketServer.StartListening(this, networkPortNumber);
        }

        void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // CAUTION: This event is fired on a different thread!
            string data = m_serialPort.ReadExisting();
            AddMessage("Received: " + data);
            SocketServer.SendNetworkData(data);
        }

        private void ReleaseSerialPort()
        {
            if (m_serialPort != null)
            {
                m_serialPort.DataReceived -= SerialPort_DataReceived;
                m_serialPort.Dispose();
                m_serialPort = null;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timerSerialAvailability.Stop();
            SocketServer.Shutdown();
            ReleaseSerialPort();
            base.OnClosing(e);
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

        private void comboBoxCOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            var portName = ((PortDescription)comboBoxCOM.SelectedItem).Name;

            if (m_serialPort != null)
            {
                if (m_serialPort.PortName.Equals(portName))
                {
                    return;
                }
                ReleaseSerialPort();
            }

            if (!portName.Equals(NO_CONNECT))
            {
                ConnectSerialPort(portName);
            }
        }
    }
}

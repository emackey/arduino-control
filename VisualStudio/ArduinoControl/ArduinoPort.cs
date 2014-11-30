using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoControl
{
    public class ArduinoPort : SerialPort
    {
        private List<ArduinoPin> m_pins;
        private string m_receivedBytes;

        public event EventHandler PinsAvailable;

        public ArduinoPort(string portName, int baudRate)
            : base(portName, baudRate)
        {
            m_pins = new List<ArduinoPin>();
            m_receivedBytes = String.Empty;

            DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Open();
            Write("@LIST;");
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var pin in m_pins)
            {
                pin.Disconnect();
            }
            m_pins.Clear();
            base.Dispose(disposing);
        }

        protected void OnDataReceived(string data)
        {
            m_receivedBytes += data;

            int pos;
            while ((pos = m_receivedBytes.IndexOf(';')) >= 0)
            {
                string command = m_receivedBytes.Substring(0, pos);
                m_receivedBytes = m_receivedBytes.Substring(pos + 1);

                pos = command.IndexOf('@');
                if (pos >= 0)
                {
                    command = command.Substring(pos + 1);
                    if (command[0] == 'L')
                    {
                        ParseList(command);
                    }
                }
            }
        }

        private static void DataReceivedHandler(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            ArduinoPort port = (ArduinoPort)sender;
            string data = port.ReadExisting();
            port.OnDataReceived(data);
        }

        private void ParseList(string list)
        {
            var pinList = list.Split('|');
            int numStrings = pinList.Length;
            for (int i = 1; i < numStrings; ++i)
            {
                string pinDescription = pinList[i];
                m_pins.Add(new ArduinoPin(this, pinDescription));
            }
            OnPinsAvailable(EventArgs.Empty);
        }

        protected virtual void OnPinsAvailable(EventArgs e)
        {
            if (PinsAvailable != null)
            {
                PinsAvailable(this, e);
            }
        }
    }
}

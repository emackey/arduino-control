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
        public List<ArduinoPin> ArduinoPins { get; private set; }
        public event EventHandler ArduinoPinsAvailable;

        private string m_receivedBytes;
        private bool m_queryOnReset;

        public ArduinoPort(string portName, int baudRate)
            : base(portName, baudRate)
        {
            ArduinoPins = new List<ArduinoPin>();
            m_receivedBytes = String.Empty;

            DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Open();
            DtrEnable = true;
#if LEONARDO
            // Leonardo doesn't auto-reset every time some new PC process opens up the
            // COM port, and it's possible to requery the Leo's running state from a
            // newly launched PC app.
            Write("@LIST;");
            m_queryOnReset = false;
#else
            // On the Uno, auto-reset happens anytime a PC app opens the COM port.
            // We need to wait for the bootloader to finish and the sketch to begin,
            // before sending any queries.
            m_queryOnReset = true;
#endif
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var pin in ArduinoPins)
            {
                pin.Disconnect();
            }
            ArduinoPins.Clear();
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
                    else if (command.Equals("RESET") && m_queryOnReset)
                    {
                        Write("@LIST;");
                    }
                }
            }
        }

        private static void DataReceivedHandler(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            // TODO: This event is fired on a subthread, need to get the
            //       data handed off to the main thread.
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
                ArduinoPins.Add(new ArduinoPin(this, pinDescription));
            }
            OnArduinoPinsAvailable(EventArgs.Empty);
        }

        protected virtual void OnArduinoPinsAvailable(EventArgs e)
        {
            if (ArduinoPinsAvailable != null)
            {
                ArduinoPinsAvailable(this, e);
            }
        }
    }
}

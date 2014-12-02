using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoControl
{
    public enum ArduinoPinMode
    {
        AnalogIn, AnalogOut, DigitalIn, DigitalOut
    }

    public class ArduinoPin
    {
        private ArduinoPort m_port;
        private int m_value;

        public string Name { get; private set; }
        public int Number { get; private set; }
        public ArduinoPinMode Mode { get; private set; }

        public ArduinoPin(ArduinoPort port, string pinDescription)
        {
            m_port = port;

            string modeString = pinDescription.Substring(0, 2);
            switch (modeString)
            {
                case "AI":
                    Mode = ArduinoPinMode.AnalogIn;
                    m_value = 0;
                    break;
                case "AO":
                    Mode = ArduinoPinMode.AnalogOut;
                    m_value = 255;
                    break;
                case "DI":
                    Mode = ArduinoPinMode.DigitalIn;
                    m_value = 0;
                    break;
                case "DO":
                    Mode = ArduinoPinMode.DigitalOut;
                    m_value = 1;
                    break;
                default:
                    throw new Exception("Unknown pin mode");
            }

            int pos = pinDescription.IndexOf('"');
            if (pos < 3)
            {
                throw new Exception("Missing pin name or number");
            }

            Number = int.Parse(pinDescription.Substring(2, pos - 2));

            int pos2 = pinDescription.LastIndexOf('"');
            Name = pinDescription.Substring(pos + 1, pos2 - pos - 1);
        }

        public void Disconnect()
        {
            m_port = null;
        }

        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                switch (Mode)
                {
                    case ArduinoPinMode.AnalogIn:
                        throw new Exception("Can't write to an analog input pin.");

                    case ArduinoPinMode.AnalogOut:
                        if ((value < 0) || (value > 255))
                        {
                            throw new Exception("Analog values must be between 0 and 255, inclusive.");
                        }
                        m_port.Write("A" + Number + ":" + value + ";");
                        break;

                    case ArduinoPinMode.DigitalIn:
                        throw new Exception("Can't write to a digital input pin.");

                    case ArduinoPinMode.DigitalOut:
                        if ((value < 0) || (value > 1))
                        {
                            throw new Exception("Digital values must be 0 or 1.");
                        }
                        m_port.Write("D" + Number + ":" + value + ";");
                        break;
                }
                m_value = value;
            }
        }
    }
}

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
                    break;
                case "AO":
                    Mode = ArduinoPinMode.AnalogOut;
                    break;
                case "DI":
                    Mode = ArduinoPinMode.DigitalIn;
                    break;
                case "DO":
                    Mode = ArduinoPinMode.DigitalOut;
                    break;
                default:
                    throw new Exception("Unknown pin mode");
            }

            int posColon = pinDescription.IndexOf(':');
            if (posColon < 3)
            {
                throw new Exception("Missing pin value or number");
            }

            int posQuote1 = pinDescription.IndexOf('"');
            if (posQuote1 < (posColon + 2))
            {
                throw new Exception("Missing pin name or value");
            }

            Number = int.Parse(pinDescription.Substring(2, posColon - 2));
            m_value = int.Parse(pinDescription.Substring(posColon + 1, posQuote1 - posColon - 1));

            int posQuote2 = pinDescription.LastIndexOf('"');
            Name = pinDescription.Substring(posQuote1 + 1, posQuote2 - posQuote1 - 1);
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
                        m_port.Write("@A" + Number + ":" + value + ";");
                        break;

                    case ArduinoPinMode.DigitalIn:
                        throw new Exception("Can't write to a digital input pin.");

                    case ArduinoPinMode.DigitalOut:
                        if ((value < 0) || (value > 1))
                        {
                            throw new Exception("Digital values must be 0 or 1.");
                        }
                        m_port.Write("@D" + Number + ":" + value + ";");
                        break;
                }
                m_value = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoSerialServer
{
    public class PortDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PortDescription() { }

        public PortDescription(string both)
        {
            Name = both;
            Description = both;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}

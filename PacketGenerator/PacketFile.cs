using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PacketGenerator
{
    [XmlRoot("Packets")]
    public class PacketFile
    {
        [XmlElement("Packet")]
        public List<Packet> Packets { get; set; } = new List<Packet>();
    }

    public class Packet
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = "";
        
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("Field")]
        public List<Field> Fields { get; set; } = new List<Field>();
    }

    public class Field
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = "";
        [XmlAttribute("type")]
        public string Type { get; set; } = "";
    }
}

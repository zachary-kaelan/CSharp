using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections;

namespace WargamingAPI.XMLModels
{
    [XmlType()]
    public class XMLComponents : IXmlSerializable
    {
        public int nextAvailableId { get; set; }

        public XmlSchema GetSchema() => null;

        public void WriteXml(XmlWriter writer) { }

        public void ReadXml(XmlReader reader)
        {

        }
    }

    
}

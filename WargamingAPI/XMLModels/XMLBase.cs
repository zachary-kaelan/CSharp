using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WargamingAPI.XMLModels
{
    public abstract class XMLBase
    {
        public string userString { get; set; }
        public XMLPrice price { get; set; }

        public struct XMLPrice
        {
            [XmlText]
            public string Value { get; set; }
            [XmlElement(ElementName = "gold", IsNullable = true)]
            public string gold { get; set; }
        }

        public struct XMLEffects
        {
            [XmlText]
            public string Value { get; set; }
            public string destruction { get; set; }
            public string explosion { get; set; }
            public string flaming { get; set; }
            [XmlElement(IsNullable = true)]
            public string collision { get; set; }
            public string damagedStateGroup { get; set; }
        }
    }

    public class XMLBaseTechTree : XMLBase
    {
        [XmlIgnore]
        public string name { get; set; }
        [XmlElement(IsNullable = true)]
        public string tags { get; set; }
        public int level { get; set; }
        public double repairCost { get; set; }
        public int maxHealth { get; set; }
        public int weight { get; set; }
        public bool notInShop { get; set; }
    }
}

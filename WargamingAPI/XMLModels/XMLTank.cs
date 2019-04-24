using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WargamingAPI.XMLModels
{
    public struct XMLTank
    {
        //public IEnumerable<KeyValuePair<string, string>> blitz { get; set; }
        //public IEnumerable<KeyValuePair<string, string>> crew { get; set; }
        public bool isUnlocked { get; set; }
        public XMLSpeedLimits speedLimits { get; set; }
        public XMLInvisibility invisibility { get; set; }
        public double repairCost { get; set; }
        public double premiumVehicleXPFactor { get; set; }
        public double crewXpFactor { get; set; }
        public string optDevicePreset { get; set; }
        public double enrichmentBoost { get; set; }

        public struct XMLSpeedLimits
        {
            public double forward { get; set; }
            public int backward { get; set; }
        }

        public struct XMLInvisibility
        {
            public double camoflageNetBonus { get; set; }
        }

        public struct XMLCamouflage
        {
            public double priceFactor { get; set; }
            public string tiling { get; set; }
            [XmlElement(IsNullable = true)]
            public string exclusionMask { get; set; }
        }
    }
}

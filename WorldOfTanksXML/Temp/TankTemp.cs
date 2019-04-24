using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfTanksXML
{
    internal class WoTBObjectTemp
    {
        public string userString { get; protected set; }
        public string description { get; protected set; }
        public string price { get; protected set; }
        public string tags { get; protected set; }
        public int level { get; protected set; }
    }

    internal class ListedTankTemp : WoTBObjectTemp
    {
        public int id { get; protected set; }
        public string shortUserString { get; protected set; }
        public int enrichmentPermanentCost { get; protected set; }
        public string configurationModes { get; protected set; }
    }

    internal sealed class TankTemp
    {
        public Dictionary<string, string>
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WargamingAPI;
using WargamingAPI.Models;

namespace WargamingTesting.BlitzStarsXML
{
    public class BSShot : BSBase
    {
        public double defaultPortion { get; private set; }
        public int speed { get; private set; }
        public double gravity { get; private set; }
        public int maxDistance { get; private set; }
        public double[] piercingPower { get; private set; }
        public ShellType type { get; private set; }
        public double caliber { get; private set; }
        public int damage { get; private set; }
        public int damageDevices { get; private set; }
        public int? price { get; private set; }
    }
}

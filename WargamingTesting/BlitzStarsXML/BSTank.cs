using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingTesting.BlitzStarsXML
{
    public class BSTank : BSBase
    {
        public string type { get; private set; }
        public string[] tags { get; private set; }
        public double speed_forward { get; private set; }
        public int speed_backward { get; private set; }
        public BSTurret[] turretsList { get; private set; }
        public int tank_id { get; private set; }
    }
}

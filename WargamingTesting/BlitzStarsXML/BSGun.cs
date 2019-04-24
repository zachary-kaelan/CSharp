using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingTesting.BlitzStarsXML
{
    public class BSGun : BSBase
    {
        public double reloadTime { get; private set; }
        public double aimingTime { get; private set; }
        public double shotDispersionRadius { get; private set; }
        public double depression { get; private set; }
        public double elevation { get; private set; }
        public int maxAmmo { get; private set; }
        public int weight { get; private set; }
        public BSShot[] shots { get; private set; }
        public BSShotDispersion shotDispersionFactors { get; private set; }

        public struct BSShotDispersion
        {
            public double turretRotation { get; private set; }
            public double afterShot { get; private set; }
            public double whileGunDamaged { get; private set; }
        }
    }
}

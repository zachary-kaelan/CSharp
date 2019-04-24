using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingTesting.BlitzStarsXML
{
    public class BSTurret : BSBase
    {
        public int viewRange { get; private set; }
        public int weight { get; private set; }
        public double rotationSpeed { get; private set; }
        public double traverse_left_arc { get; private set; }
        public double traverse_right_arc { get; private set; }
        public int maxHealth { get; private set; }
        public BSGun[] guns { get; private set; }
    }
}

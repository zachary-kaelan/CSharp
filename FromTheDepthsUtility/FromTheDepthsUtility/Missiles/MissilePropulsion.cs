using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Missiles
{
    public enum MissilePropulsionModule
    {
        ShortRange,
        Variable,
        TorpedoPropeller
    }

    public class MissilePropulsion : IMissileModule
    {
        public byte Health => 2;
        public byte Drag => 10;
        public MissileModuleType Type => MissileModuleType.Propulsion;
        public MissilePropulsionModule PropulsorType { get; private set; }
        public float ThrustPerFuel { get; private set; }
        public ushort FuelBurnedPerSecond { get; private set; }
        public byte Lifetime { get; private set; }
    }
}

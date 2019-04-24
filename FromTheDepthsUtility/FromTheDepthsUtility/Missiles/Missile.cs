using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Missiles
{
    public enum WarheadType
    {
        Thumper
    }

    public class Missile
    {
        public byte ReloadTime { get; private set; }
        public ushort AmmoCost { get; private set; }
        public float Health { get; private set; }
        public float Drag { get; private set; }
        public byte Size { get; private set; }
        public byte LifeTime { get; private set; }
        public int Fuel { get; private set; }
        private float ThrustPerFuel { get; set; }
        private int FuelBurnedPerSecond { get; set; }
        private byte _sizeCounter = 0;
        public WarheadType Warhead { get; private set; }
        private byte _massCounter = 0;

        public Missile(byte size)
        {
            Size = size;
            _massCounter = size;
            ReloadTime = (byte)(size * 2);
            AmmoCost = (ushort)(size * 25);
        }

        public void AddModule(GantryClass gantry, IMissileModule module)
        {
            ++_sizeCounter;
            Drag += (module.Drag / _sizeCounter);
            Health += module.Health;
            var gantryMultiplier = (int)gantry;

            switch(module.Type)
            {
                case MissileModuleType.Propulsion:
                    var propulsor = (MissilePropulsion)module;
                    if (_sizeCounter != Size && propulsor.PropulsorType == MissilePropulsionModule.TorpedoPropeller)
                    {
                        Drag += (3 / _sizeCounter);
                        ++_massCounter;
                    }
                    else if (LifeTime == 0)
                        LifeTime = (byte)(propulsor.Lifetime * gantryMultiplier);
                    ThrustPerFuel += propulsor.ThrustPerFuel;
                    FuelBurnedPerSecond += propulsor.FuelBurnedPerSecond;
                    break;
            }
        }
    }
}

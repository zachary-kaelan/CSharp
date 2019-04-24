using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfTanksXML
{
    public enum LODDistance
    {
        MEDIUM
    }

    internal sealed class GunTemp : ModuleTemp
    {
        public double impulse { get; private set; }

        public RecoilTemp recoil { get; private set; }

        public string effects { get; private set; }
        public string pitchLimits { get; private set; }
        public string turretYawLimits { get; private set; }
        public double rotationSpeed { get; private set; }
        public double reloadTime { get; private set; }
        public int maxAmmo { get; private set; }
        public double aimingTime { get; private set; }

        public ClipTemp clip { get; private set; }
        public ClipTemp burst { get; private set; }

        public double shotDispersionRadius { get; private set; }

        public ShotDispersionFactorsTemp shotDispersionFactors { get; private set; }
        public Dictionary<string, ShotsTemp> shots { get; private set; }

        internal struct RecoilTemp
        {
            public LODDistance lodDist { get; private set; }
            public double amplitude { get; private set; }
            public double backoffTime { get; private set; }
            public double returnTime { get; private set; }
        }

        internal struct ClipTemp
        {
            public int count { get; private set; }
            public int rate { get; private set; }
        }

        internal struct ShotDispersionFactorsTemp
        {
            public double turretRotation { get; private set; }
            public double afterShot { get; private set; }
            public double whileGunDamaged { get; private set; }
        }

        internal struct ShotsTemp
        {
            public double defaultPortion { get; private set; }
            public int speed { get; private set; }
            public double gravity { get; private set; }
            public int maxDistance { get; private set; }
            public string piercingPower { get; private set; }
        }
    }
}

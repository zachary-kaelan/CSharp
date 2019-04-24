using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfTanksXML
{
    public enum ShellKind
    {
        ARMOR_PIERCING,
        ARMOR_PIERCING_CR,
        HIGH_EXPLOSIVE,
        HOLLOW_CHARGE
    }

    public enum ShellIcon
    {
        ap,
        ap_cr,
        he,
        hc
    }

    internal sealed class ShellTemp : ModuleTemp
    {
        public KeyValuePair<string, int> blitz { get; private set; }
        public int id { get; private set; }
        public string icon { get; private set; }
        public ShellKind kind { get; private set; }
        public double caliber { get; private set; }
        public bool isTracer { get; private set; }
        public string effects { get; private set; }
        public int normalizationAngle { get; private set; }
        public int ricochetAngle { get; private set; }
        public double piercingPowerLossFactorByDistance { get; private set; }

        internal struct DamageTemp
        {
            public int armor { get; private set; }
            public int devices { get; private set; }
        }
    }
}

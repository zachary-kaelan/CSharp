using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.AdvancedCannons
{
    public enum Mantlet
    {
        Omni,
        Elevation,
        Omni3x3,
        Elevation3m,
        AntiAir
    }

    public class AdvancedCannon
    {
        public float Gauge { get; private set; }
        public int NumCartridges { get; private set; }
        //private SortedDictionary<>

        public AdvancedCannon(int numBarrels, int numGaugeIncreasers)
        {
            float gauge = 60;
            float gaugeIncrease = 60;
            for (int i = 0; i < numGaugeIncreasers; ++i)
            {
                gauge += gaugeIncrease;
                gaugeIncrease *= 0.98f;
            }

            if (numBarrels > 1)
                gauge /= (20f / (12 - numBarrels));
            Gauge = gauge;

            NumCartridges = Gauge <= 250 ? Math.Min((int)(2 / Gauge), 64) : (int)(1 / Gauge);
        }
    }

}

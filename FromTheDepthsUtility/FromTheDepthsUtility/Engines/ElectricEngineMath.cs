using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Engines
{
    public static class ElectricEngineMath
    {
        // Each cubic meter of battery can store 1000 kJ

        public static float MaxChargeConsumption(float charge, float powerOutput)
        {
            // kW / m^3
            return 0.4f * charge * powerOutput;
            // 40 at full charge.
        }

        public static float Efficiency(float powerOutput)
        {
            return 2f / (1f + powerOutput);
        }

        public static float MaxPowerOutput(float charge, float powerOutput)
        {
            return MaxChargeConsumption(charge, powerOutput) * Efficiency(powerOutput);
        }

        // RTGs cost 375 resources
        // Produce 25 battery power per second per cubic metre

        // b = volume ratio of batteries to RTGs
        // M is the factor between the maximum power output and the sustainable output
        // c = cost ratio of RTGs to batteries

        public static float MaxSustainableOutput(float powerOutput)
        {
            // Per cubic metre of RTG
            return 50f / (1f + powerOutput);
        }

        // Maximize ratio of power output to cost

        public static float CostRatio(float powerOutput, float headroom)
        {
            return (1.25f * (1f + headroom)) / (float)Math.Pow(powerOutput, 2f);
        }

        public static float PowerOutputForRatio(float headroom, float costRatio)
        {
            return (float)Math.Pow((1.25 * (1f + headroom)) / costRatio, 0.5);
        }

        // For M = 1.2, and c = 18.75; powerOutput = 0.28 and battery ratio = 4.2
    }
}

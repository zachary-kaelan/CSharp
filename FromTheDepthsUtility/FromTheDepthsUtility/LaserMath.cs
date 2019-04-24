using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class LaserMath
    {
        public readonly static Dictionary<float, QSwitchInfo> QSwitches = new Dictionary<float, QSwitchInfo>()
        {
            {0, new QSwitchInfo(40, 0.01, 1, 0.14) },
            {1, new QSwitchInfo(1, 0.2, 2, 5.65) },
            {2, new QSwitchInfo(2, 0.1, 2, 4.00) },
            {3, new QSwitchInfo(3, 0.05, 2, 2.45) },
            {4, new QSwitchInfo(4, 0.025, 2, 1.41) }
        };

        // Laser Cavity stores 200 units of laser energy
        // Laser Cavity produces 50 laser energy per second at the cost of 100 power 
        //      (80 power-frames per laser energy)
        // Laser Destabilizer causes the line its a part of to discharge 80% more energy at a time.
        //      Each Laser Destabilizer is 80% as effective as the last
        // Frequency Doublers increase laser AP by 1.
        //      Laser range is multiplied by sqrt(1f + # of frequency doublers)

        // At base, 20% of stored energy is discharged per second
        // Base range is 5.65m per daamage; final is capped at 10km

        // L = sum of destabilizers and cavities with four pumps
        // Q = amount of Q-Switches
        // P = most efficient length of laser cavities within L

        public static float DamagePerSecond(float destabilizers, float cavities, float qswitches)
        {
            float premax = 40f * (
                (cavities / 2f) + 
                Enumerable.Range(
                    1, Convert.ToInt32(destabilizers)
                ).Sum(n => (cavities / 2f) * Math.Pow(0.8, (float)Convert.ToDouble(n)))
            );

            return Math.Max(premax, Math.Pow(qswitches, 0.5) * premax);
        }

        // Field of fire is 15 degrees per Laser Steering Optics, up to a max of 90 with 6.

        public static float Inaccuracy(float SteeringOptics, float Optics)
        {
            return 10f / (SteeringOptics + Optics);
        }

        // Optics are unusable with Laser Missile Defense
        // Inaccuracy of 0.5 degrees, single-block accuracy out to about 115m
        
        public static float AbsorbtionMultiplier(float air, float water, float smoke)
        {
            // air in meters, water in meters, smoke in layers
            //return Math.Pow(Math.E, -1f * ((0.0003 * air) + (0.01 * water))) * Math.Pow(0.1, smoke);
            return Math.Pow(Math.E, -1f * ((0.0003 * air) + (0.01 * water) + (2.3 * smoke)));
        }

        // Sustained damage is limited by energy going into the laser and energy coming out
        // Energy going in, assuming sufficient power, is 50 tmes the number of Laser Pumps
        // Max energy discharged per second is determined by the number of Cavities and Destabilizers
        public static float MaxEnergyPerSecond(float cavities, float destabilizers)
        {
            return 200f * cavities * (1f - Math.Pow(0.8, destabilizers + 1f));
        }

        // Missiles have 100 HP and 1 AP
        // Aim to make each pulse deal 10 damage with AP 3
    }

    public struct QSwitchInfo
    {
        public float PulsesPerSecond { get; set; }
        public float DrainPerPulse { get; set; }
        public float DamagePerEnergy { get; set; }
        public float RangePerEnergy { get; set; }

        public QSwitchInfo(float pulses, float drain, float damage, float range)
        {
            PulsesPerSecond = pulses;
            DrainPerPulse = drain;
            DamagePerEnergy = damage;
            RangePerEnergy = range;
        }
    }
}

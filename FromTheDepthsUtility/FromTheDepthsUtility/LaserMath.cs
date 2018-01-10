using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class LaserMath
    {
        public readonly static Dictionary<double, QSwitchInfo> QSwitches = new Dictionary<double, QSwitchInfo>()
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
        //      Laser range is multiplied by sqrt(1.0 + # of frequency doublers)

        // At base, 20% of stored energy is discharged per second
        // Base range is 5.65m per daamage; final is capped at 10km

        // L = sum of destabilizers and cavities with four pumps
        // Q = amount of Q-Switches
        // P = most efficient length of laser cavities within L

        public static double DamagePerSecond(double destabilizers, double cavities, double qswitches)
        {
            double premax = 40.0 * (
                (cavities / 2.0) + 
                Enumerable.Range(
                    1, Convert.ToInt32(destabilizers)
                ).Sum(n => (cavities / 2.0) * Math.Pow(0.8, Convert.ToDouble(n)))
            );

            return Math.Max(premax, Math.Pow(qswitches, 0.5) * premax);
        }

        // Field of fire is 15 degrees per Laser Steering Optics, up to a max of 90 with 6.

        public static double Inaccuracy(double SteeringOptics, double Optics)
        {
            return 10.0 / (SteeringOptics + Optics);
        }

        // Optics are unusable with Laser Missile Defense
        // Inaccuracy of 0.5 degrees, single-block accuracy out to about 115m
        
        public static double AbsorbtionMultiplier(double air, double water, double smoke)
        {
            // air in meters, water in meters, smoke in layers
            //return Math.Pow(Math.E, -1.0 * ((0.0003 * air) + (0.01 * water))) * Math.Pow(0.1, smoke);
            return Math.Pow(Math.E, -1.0 * ((0.0003 * air) + (0.01 * water) + (2.3 * smoke)));
        }

        // Sustained damage is limited by energy going into the laser and energy coming out
        // Energy going in, assuming sufficient power, is 50 tmes the number of Laser Pumps
        // Max energy discharged per second is determined by the number of Cavities and Destabilizers
        public static double MaxEnergyPerSecond(double cavities, double destabilizers)
        {
            return 200.0 * cavities * (1.0 - Math.Pow(0.8, destabilizers + 1.0));
        }

        // Missiles have 100 HP and 1 AP
        // Aim to make each pulse deal 10 damage with AP 3
    }

    public struct QSwitchInfo
    {
        public double PulsesPerSecond { get; set; }
        public double DrainPerPulse { get; set; }
        public double DamagePerEnergy { get; set; }
        public double RangePerEnergy { get; set; }

        public QSwitchInfo(double pulses, double drain, double damage, double range)
        {
            PulsesPerSecond = pulses;
            DrainPerPulse = drain;
            DamagePerEnergy = damage;
            RangePerEnergy = range;
        }
    }
}

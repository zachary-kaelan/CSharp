using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Engines
{
    public class ElectricEngine
    {
        public Battery[] Batteries { get; set; }

        public ElectricEngine(Battery[] batteries)
        {
            Batteries = batteries;
        }

        public int GetRunTime(float demand)
        {
            int seconds = 0;
            float totalCharge = 0;
            do
            {
                totalCharge = 0;
                for (int i = 0; i < Batteries.Length; ++i)
                {
                    totalCharge += Batteries[i].Tick();
                }
                if (demand < totalCharge)
                {
                    ++seconds;
                    float refund = 1 - (demand / totalCharge);
                    for (int i = 0; i < Batteries.Length; ++i)
                    {
                        Batteries[i].Refund(refund);
                    }
                }
            } while (demand <= totalCharge);
            return seconds;
        }
    }

    public struct Battery
    {
        private float _lastBurn;
        public float Charge;
        public float PercentChargePerSecond;
        public float OutputEfficiency;

        public Battery(float output)
        {
            _lastBurn = 0;
            Charge = 1000;
            PercentChargePerSecond = (0.04f * output);
            OutputEfficiency = 2 / (1 + output);
        }

        public float Tick()
        {
            var chargeBurned = PercentChargePerSecond * Charge;
            _lastBurn = chargeBurned;
            Charge -= chargeBurned;
            return OutputEfficiency * chargeBurned;
        }

        public void Refund(float percentage)
        {
            Charge += _lastBurn * percentage;
        }
    }
}

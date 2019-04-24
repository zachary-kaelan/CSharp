using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3
{
    public struct EvoStats
    {
        public bool Survives { get; set; }
        public float Fertility { get; set; }
        public float Fitness { get; set; }
        public float MinGeneFitness { get; set; }
        public short TotalTimes { get; set; }

        public float GetStat() =>
            Survives ? Fitness : Fertility;

        public float GetMutationMultiplier() =>
            Survives ? 1 - (Fitness * Fitness) : 1;
    }

    public struct EvoStatsComparer : IComparer<KeyValuePair<ushort, EvoStats>>
    {

        public int Compare(EvoStats x, EvoStats y)
        {
            if (x.Survives == y.Survives)
            {
                if (x.Survives)
                {
                    if (x.Fitness >= y.Fitness)
                        return -1;
                    else if (y.Fitness > x.Fitness)
                        return 1;
                    else
                        return 0;
                }
                else
                {
                    if (x.Fertility >= y.Fertility)
                        return -1;
                    else if (y.Fertility > x.Fertility)
                        return 1;
                    else
                        return 0;
                }
            }
            else
                return x.Survives ? -1 : 1;
        }

        public int Compare(KeyValuePair<ushort, EvoStats> x, KeyValuePair<ushort, EvoStats> y) =>
            x.Key == y.Key ? 0 : Compare(x.Value, y.Value);
    }
}

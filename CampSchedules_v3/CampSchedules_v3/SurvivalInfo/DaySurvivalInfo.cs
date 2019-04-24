using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3
{
    public struct DaySurvivalInfo
    {
        public byte NumGenes { get; set; }
        public byte MinTime { get; set; }
        public byte MaxTime { get; set; }
        public byte[] TailTimes { get; set; }
        public byte[] GenesPerTime { get; set; }

        public DaySurvivalInfo(IEnumerable<byte> timeGenesCounts, IEnumerable<byte> tailTimes, byte numGenes)
        {
            if (timeGenesCounts.First() == 0)
            {
                byte minTime = 2;
                tailTimes = tailTimes.Select(t => (byte)(t - minTime));
                MinTime = minTime;
                timeGenesCounts = timeGenesCounts.Skip(MinTime);
            }
            else
                MinTime = 0;
            MaxTime = tailTimes.Last();
            GenesPerTime = timeGenesCounts.Take(MaxTime + 1).ToArray();
            TailTimes = tailTimes.ToArray();
            NumGenes = numGenes;
        }
    }
}

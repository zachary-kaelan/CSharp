using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphoLib;

namespace CampSchedules_v3
{
    public class SurvivalStats
    {
        public float Fertility { get; private set; }
        public float TimesBounded { get; private set; }
        public short TimesTotal { get; private set; }
        public sbyte[] Times { get; private set; }
        public SortedDictionary<byte, byte> RepeatsExcess { get; private set; }

        public SurvivalStats(float fertility, float timesBounded, short timesTotal, sbyte[] times, IDictionary<byte, byte> repeatsExcess)
        {
            Fertility = fertility;
            TimesBounded = timesBounded;
            TimesTotal = timesTotal;
            Times = times;
            RepeatsExcess = new SortedDictionary<byte, byte>(repeatsExcess);
        }
    }
}

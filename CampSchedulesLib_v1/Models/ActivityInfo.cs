using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public class ActivityInfo
    {
        public int MinDorms { get; private set; }
        public int MaxDorms { get; private set; }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        //public int Zone { get; private set; }
        public int Duration { get; private set; }
        //public bool AvailableCounselorsChoice { get; private set; }
        public int Priority { get; private set; }
        public bool ManuallyScheduled { get; private set; }
        //public int StaffRequired { get; private set; }
        public bool IsExclusive { get; private set; }
        public int ExhaustionLevel { get; private set; }
        public bool Repeatable { get; private set; }

        public ActivityInfo(string name, string abbrv, int basePriority, int exhaustionLevel, bool isExclusive = false, bool isManual = false, bool isRepeatable = true, int maxD = 1, int minD = 1, int duration = 1)
        {
            MinDorms = minD;
            MaxDorms = maxD;
            Name = name;
            Abbreviation = abbrv;
            //Zone = zone;
            Duration = duration;
            Priority = basePriority;
            ExhaustionLevel = exhaustionLevel;
            IsExclusive = isExclusive;
            ManuallyScheduled = isManual;
            Repeatable = isRepeatable;
        }
    }
}

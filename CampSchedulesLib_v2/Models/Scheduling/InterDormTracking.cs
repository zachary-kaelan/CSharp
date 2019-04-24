using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class InterDormTracking : Thing, ICloneable
    {
        private int ThisDorm { get; set; }
        public int Priority { get; set; }
        public int Dorm { get; private set; }
        public int Options { get; set; }
        public SortedSet<int> PreviousRepeatableActivities { get; private set; }
        public SortedSet<string> ScheduleHistory { get; private set; }
        public bool AvailableToday { get; set; }

        public InterDormTracking(int thisDorm, int otherDorm, int priority) : base((thisDorm * Schedule.Dorms.Count) + otherDorm, thisDorm + "_" + otherDorm)
        {
            ThisDorm = thisDorm;
            Priority = priority;
            PreviousRepeatableActivities = new SortedSet<int>();
            ScheduleHistory = new SortedSet<string>();
            Options = -1;
            Dorm = otherDorm;
        }

        public bool IncludeInResolving() => AvailableToday && Options > 0;

        public object Clone() =>
            new InterDormTracking(ThisDorm, Dorm, Priority)
            {
                Options = this.Options,
                PreviousRepeatableActivities = this.PreviousRepeatableActivities,
                ScheduleHistory = this.ScheduleHistory,
                AvailableToday = this.AvailableToday
            };
    }
}

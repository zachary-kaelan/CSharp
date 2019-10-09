using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;

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

            if (ThisDorm == Dorm)
                AvailableToday = false;
        }

        public bool IncludeInResolving() => AvailableToday && Options > 0;

        public void RescheduleActivity(bool doneToday)
        {
            if (doneToday)
                AvailableToday = false;

            Debug.Assert(Priority > 0);
        }

        public bool ScheduleActivity(ScheduledActivity scheduledActivity, bool doneToday, bool repeatable, bool multiDorm)
        {
            if (doneToday)
            {
                Options = 0;
                AvailableToday = false;
            }
            else if (!scheduledActivity.HasOther)
                --Options;

            ScheduleHistory.Add(scheduledActivity.Abbreviation);

            bool soloTracker = ThisDorm == Dorm;

            if (repeatable && (!soloTracker || multiDorm))
            {
                if (!PreviousRepeatableActivities.Add(scheduledActivity.Activity))
                    throw new NotImplementedException("Did a repeatable activity twice with the same Dorm.");
            }

            if (!soloTracker && --Priority == 0)
            {
                Options = 0;
                AvailableToday = false;
                return true;
            }

            return false;
        }

        public void ClearFromHistory(ScheduledActivity scheduledActivity, bool repeatable, bool multiDorm)
        {
            bool soloTracker = ThisDorm == Dorm;

            if (!soloTracker)
            {
                ++Priority;
                AvailableToday = true;
            }

            if (repeatable && (!soloTracker || multiDorm))
            {
                if (!PreviousRepeatableActivities.Remove(scheduledActivity.Activity))
                    throw new NotImplementedException("Activity not found in PreviousRepeatableActivities.");
            }

            ScheduleHistory.Remove(scheduledActivity.Abbreviation);
        }

        public object Clone()
        {
            var tracker = new InterDormTracking(ThisDorm, Dorm, Priority)
            {
                Options = this.Options,
                AvailableToday = this.AvailableToday
            };
            tracker.PreviousRepeatableActivities.UnionWith(this.PreviousRepeatableActivities);
            tracker.ScheduleHistory.UnionWith(this.ScheduleHistory);
            return tracker;
        }
    }
}

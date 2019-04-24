using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class ScheduledActivity : Thing
    {
        public int Dorm { get; private set; }
        public int Duration { get; private set; }
        public int Activity { get; private set; }
        public int OtherDorm { get; private set; }
        //private int _blockID;
        public int BlockID { get; set; }
        /*{
            get => _blockID;
            internal set {
                _blockID = value;
                Abbreviation = Abbreviation.Insert(
                    HasOther ? 3 : 6,
                    value.ToString().PadLeft(2, '0') + "_"
                );
            }
        }*/
        public bool HasOther => OtherDorm != -1;
        public string StringExtension { get; set; }
        private static int ID_COUNTER = 0;

        public ScheduledActivity() : base()
        {
            OtherDorm = -1;
        }

        public ScheduledActivity(int dorm, int duration, int activity, int blockID, int other = -1) : 
            base(
                ID_COUNTER, 
                String.Join(
                    "_", (
                        other == -1 ? 
                            new int[] { blockID, dorm, -1, activity, duration } :
                            new int[] { blockID }.Concat(
                                new int[] { dorm, other }.OrderBy(d => d)
                            ).Concat(
                                new int[] { activity, duration }
                            )
                        ).Select(n => n.ToString().PadLeft(2, '0'))
                )
            )
        {
            Dorm = dorm;
            Duration = duration;
            Activity = activity;
            OtherDorm = other;
            BlockID = blockID;
            ++ID_COUNTER;
        }

        internal class StringableScheduled
        {
            public int DormAgeIndex { get; private set; }
            public int OtherDormAgeIndex { get; private set; }
            public string DormEntry { get; private set; }
            public string OtherDormEntry { get; private set; }
            public bool HasOther { get; private set; }
            public int Duration { get; set; }

            public StringableScheduled(ScheduledActivity activity)
            {
                string activityAbbrv = Schedule.Activities[activity.Activity].Abbreviation;
                DormInfo dorm = Schedule.Dorms[activity.Dorm];
                DormAgeIndex = dorm.AgeIndex;
                DormEntry = dorm.Abbreviation + " - " + activityAbbrv;
                HasOther = activity.HasOther;
                if (HasOther)
                {
                    dorm = Schedule.Dorms[activity.OtherDorm];
                    OtherDormAgeIndex = dorm.AgeIndex;
                    OtherDormEntry = dorm.Abbreviation + " - " + activityAbbrv;
                }
                Duration = activity.Duration;
            }
        }
    }
}

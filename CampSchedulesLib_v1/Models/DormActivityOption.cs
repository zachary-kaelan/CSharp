using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public struct DormActivityOption : IComparable
    {
        public Dorm ThisDorm { get; private set; }
        public Dorm OtherDorm { get; private set; }
        public string ActivityAbbrv { get; private set; }
        public double ThisPriority { get; private set; }
        public double OtherPriority { get; private set; }
        public double Priority { get; private set; }
        
        public DormActivityOption(string activityAbbrv, Dorm dorm, double priority)
        {
            ThisDorm = dorm;
            OtherDorm = Dorm.None;
            ActivityAbbrv = activityAbbrv;
            Priority = priority;
            OtherPriority = -1;
            ThisPriority = priority;
        }

        public DormActivityOption(string activityAbbrv, Dorm dorm, double thisPriority, Dorm otherDorm, double otherPriority)
        {
            ThisDorm = dorm;
            OtherDorm = otherDorm;
            ActivityAbbrv = activityAbbrv;
            Priority = (thisPriority + otherPriority) / 2;
            ThisPriority = thisPriority;
            OtherPriority = otherPriority;
        }

        public DormActivityOption(DormActivityOption opt, double priority)
        {
            ThisDorm = opt.ThisDorm;
            OtherDorm = opt.OtherDorm;
            ActivityAbbrv = opt.ActivityAbbrv;
            Priority = priority;
            ThisPriority = opt.OtherPriority;
            OtherPriority = opt.OtherPriority;
        }

        public int CompareTo(object obj)
        {
            return Priority.CompareTo(obj);
        }

        public override int GetHashCode() => GetHashCode(ActivityAbbrv, ThisDorm, OtherDorm);

        public static int GetHashCode(string activityAbbrv, Dorm dorm, Dorm otherDorm) => (int)(dorm | otherDorm) ^ activityAbbrv.GetHashCode();
    }
}

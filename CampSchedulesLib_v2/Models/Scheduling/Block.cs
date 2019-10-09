using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class Block : Thing
    {
        public TimeSpan Start { get; private set; }
        public SortedSet<string> ScheduleHistory { get; private set; }
        public bool IsExcess { get; private set; }
        private static int ID_COUNTER = 0;

        public Block(TimeSpan start, bool isExcess = false) : base(ID_COUNTER, start.Hours.ToString() + "_" + start.Minutes.ToString())
        {
            Start = start;
            ScheduleHistory = new SortedSet<string>();
            IsExcess = isExcess;
            ++ID_COUNTER;
        }
    }
}

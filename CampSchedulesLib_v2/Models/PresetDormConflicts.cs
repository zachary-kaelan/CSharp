using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models
{
    public class PresetDormConflicts
    {
        public SortedSet<int> FreshActivityConflicts { get; set; }
        public SortedSet<int> UnavailableActivities { get; set; }
        public int OtherDorm { get; set; }

        public PresetDormConflicts()
        {
            FreshActivityConflicts = new SortedSet<int>();
            UnavailableActivities = new SortedSet<int>();
            OtherDorm = -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.Scheduling
{
    public class ConflictNode
    {
        public int Dorm { get; private set; }
        public int Activity { get; private set; }
        public bool Conditional { get; private set; }

        public ConflictNode(int dorm, int activity, bool conditional = false)
        {
            Dorm = dorm;
            Activity = activity;
            Conditional = conditional;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public class ScheduledActivity
    {
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
    }
}

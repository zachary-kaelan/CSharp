using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.CSV
{
    public class ManuallyScheduledCSV
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Dorm { get; set; }
        public string Activity { get; set; }
    }
}

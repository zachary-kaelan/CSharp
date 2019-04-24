using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.CSV
{
    public struct BlockCSV
    {
        public DayOfWeek Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool Excess { get; set; }
    }
}

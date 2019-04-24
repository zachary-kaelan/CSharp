using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public class Day
    {
        public DayOfWeek DayOfWeek { get; private set; }
        public Block[] Blocks { get; private set; }
    }
}

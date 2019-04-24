using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models.CSV
{
    public class DormActivityPriorityCSV
    {
        public string ActivityAbbreviation { get; set; }
        public string Dorm { get; set; }
        public int PriorityChange { get; set; }
    }
}

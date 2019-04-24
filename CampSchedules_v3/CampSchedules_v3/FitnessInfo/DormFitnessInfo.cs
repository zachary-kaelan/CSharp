using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampSchedulesLib_v2.Models.Info;

namespace CampSchedules_v3
{
    public class DormFitnessInfo
    {
        public SortedDictionary<byte, byte> ActivityPriorities { get; set; }
        public SortedDictionary<byte, byte> DormPriorities { get; set; }

        public DormFitnessInfo()
        {
            ActivityPriorities = new SortedDictionary<byte, byte>();
            DormPriorities = new SortedDictionary<byte, byte>();
        }
    }
}

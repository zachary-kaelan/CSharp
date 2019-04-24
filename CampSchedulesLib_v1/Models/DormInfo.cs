using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v1.Models
{
    public class DormInfo
    {
        public Dorm Dorm { get; private set; }
        public SpecialActivity AllowedSpecial { get; private set; }
        public bool IsGirl { get; private set; }
        public int NumCampers { get; private set; }
        public string Name { get; private set; }

        public DormInfo(Dorm dorm, string name, bool isGirl = true)
        {
            Dorm = dorm;
            IsGirl = isGirl;
            Name = name;
        }
    }
}

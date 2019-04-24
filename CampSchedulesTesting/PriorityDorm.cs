using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesTesting
{
    public class PriorityDorm
    {
        public string Name { get; private set; }
        public List<string> OtherDorms { get; private set; }
        public List<string> Activities { get; private set; }

        public PriorityDorm() { }

        public PriorityDorm(string name)
        {
            Name = name;
            OtherDorms = new List<string>();
            Activities = new List<string>();
        }
    }
}

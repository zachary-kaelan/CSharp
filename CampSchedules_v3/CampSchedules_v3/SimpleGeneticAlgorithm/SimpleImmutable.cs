using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public struct SimpleImmutable
    {
        public byte Day { get; private set; }
        public byte Time { get; private set; }
        public byte Activity { get; private set; }

        public SimpleImmutable(byte day, byte time, byte activity)
        {
            Day = day;
            Time = time;
            Activity = activity;
        }
    }
}

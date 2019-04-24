using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public struct SimpleFullGene
    {
        public byte Day { get; private set; }
        public byte Time { get; private set; }
        public byte Activity { get; private set; }
        public byte Dorm { get; private set; }
        public byte OtherDorm { get; private set; }

        public SimpleFullGene(byte day, byte time, byte activity, byte dorm, byte otherDorm)
        {
            Day = day;
            Time = time;
            Activity = activity;
            Dorm = dorm;
            OtherDorm = otherDorm;
        }

        public sbyte CompareTo(byte day, byte time)
        {
            if (Day != day)
                return (sbyte)(Day - day);
            else
            {
                if (Time != time)
                    return (sbyte)(Time - time);
                else //if (Equals(other))
                    return 0;

                //return Time - other.Time;
                /*if (Time != other.Time)
                    return Time - other.Time;
                else
                {
                    if (Dorm != other.Dorm)
                        return Dorm - other.Dorm;
                    else
                    {
                        if (OtherDorm != other.OtherDorm)
                    }
                }*/
            }
        }

        public void SerializeWith(FileStream file)
        {
            file.WriteByte(Day);
            file.WriteByte(Time);
            file.WriteByte(Activity);
            file.WriteByte(Dorm);
            file.WriteByte(OtherDorm);
        }
    }
}

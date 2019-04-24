using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedulesLib_v2.Models
{
    public class Thing : IEquatable<Thing>
    {
        public int ID { get; private set; }
        public string Abbreviation { get; protected set; }

        public Thing()
        {

        }

        public Thing(int id, string abbrv)
        {
            ID = id;
            Abbreviation = abbrv;
        }

        public override string ToString() => Abbreviation;

        public bool Equals(Thing other) => ID == other.ID;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    [Flags]
    public enum Sense : byte
    {
        None,
        Blindsight,
        Darkvision,
        Tremorsense = 4,
        Truesight = 8,
        Unknown = 16
    }

    public class MonsterSense
    {
        public Sense Senses { get; set; }
        public byte Range { get; set; }
    }
}

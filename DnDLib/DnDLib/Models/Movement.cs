using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public enum MovementType : byte
    {
        None,
        Walk,
        Swim,
        Burrow,
        Climb,
        Fly
    }

    public class Movement
    {
        public MovementType Type { get; set; }
        public byte Speed { get; set; }
        public string Note { get; set; }
    }
}

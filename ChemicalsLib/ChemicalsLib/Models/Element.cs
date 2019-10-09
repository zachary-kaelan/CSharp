using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalsLib.Models
{
    public struct Element
    {
        // atomic number
        public byte Z { get; set; }
        public byte Count { get; set; }
        public sbyte Charge { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalsLib.Models
{
    public class ElementInfo : InfoBase
    {
        // s-block, p-block, d-block, and f-block
        public char Block { get; set; }
        public MajorElementCategory MajorElementCateogry { get; set; }
        public byte Group { get; set; }
        public byte Period { get; set; }
        public byte AtomicNumber { get; set; }
        public float AtomicWeight { get; set; }
    }
}

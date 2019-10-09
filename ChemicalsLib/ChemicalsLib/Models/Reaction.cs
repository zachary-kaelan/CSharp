using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalsLib.Models
{
    public class Reaction
    {
        public Molecule[] Reactants { get; set; }
        public Molecule[] Agents { get; set; }
        public Molecule[] Products { get; set; }
    }
}

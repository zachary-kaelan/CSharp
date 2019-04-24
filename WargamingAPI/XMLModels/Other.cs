using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.XMLModels
{
    public class XMLEngine : XMLBaseTechTree
    {
        public int power { get; set; }
        public double fireStartingChance { get; set; }
    }

    public class XMLFuelTank : XMLBaseTechTree { }

    public class XMLRadio : XMLBaseTechTree
    {
        public int distance { get; set; }
    }
}

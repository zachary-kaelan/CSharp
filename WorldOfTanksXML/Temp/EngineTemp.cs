using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfTanksXML
{
    internal sealed class EngineTemp : ModuleTemp
    {
        public string sound { get; private set; }
        public int power { get; private set; }
        public double fireStartingChance { get; private set; }
    }
}

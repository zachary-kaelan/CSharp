using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldOfTanksXML
{
    internal class ModuleTemp : WoTBObjectTemp
    {
        public bool notInShop { get; protected set; }       // Only present if true

        public int weight { get; protected set; }
        public int maxHealth { get; protected set; }
        public int maxRegenHealth { get; protected set; }
        public double repairCost { get; protected set; }
    }

    internal class ModulesTemp<T> where T : ModuleTemp
    {
        public int nextAvailableId { get; protected set; }
        public Dictionary<string, int> ids { get; protected set; }
        public Dictionary<string, T> shared { get; protected set; }
    }
}

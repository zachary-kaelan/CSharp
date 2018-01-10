using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UndeadRaces
{
    public static class MiscDefs
    {
        public static readonly ThingDef CorpseFilth = new ThingDef()
        {
            
            filth = new FilthProperties()
            {
                canFilthAttach = true,
                cleaningWorkToReduceThickness = 70f,
                maxThickness = 3,
                rainWashes = true,
                terrainSourced = false,
                allowsFire = false
            },
        };
    }
}

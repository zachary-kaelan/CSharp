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
            defName = "FilthCorpseDebris",
            label = "corpse debris",
            statBases = new List<StatModifier>() {
                new StatModifier()
                {
                    stat = StatDef.Named("Beauty"),
                    value = -25f
                },
                new StatModifier()
                {
                    stat = StatDef.Named("Cleanliness"),
                    value = -15f
                }
            },
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

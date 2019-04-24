using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public class Stage// : ISet<CHara>
    {
        private SortedSet<ushort> Actors;
        private SortedSet<ushort> PossibleVerbs;
        internal delegate void ReactionHandler(ushort srcEntityID, ushort dstEntityID, ushort verbID, float verbIntensity, SortedList<ushort, Reaction> reactions);
        internal event ReactionHandler On_Action;

        public Stage()
        {
            Actors = new SortedSet<ushort>();
            PossibleVerbs = new SortedSet<ushort>(Constants.UniversalVerbs);
        }

        public Stage(IEnumerable<ushort> possibleVerbs) : this()
        {
            PossibleVerbs.UnionWith(possibleVerbs);
        }

        public bool AddToStage(ushort actorID)
        {
            if (Actors.Add(actorID))
            {
                var actor = Constants.CHARACTERS[actorID];
                On_Action += actor.ObserveAction;
                return true;
            }
            return false;
        }

        public bool DoVerb(ushort srcEntityID, ushort dstEntityID, ushort verbID, float verbIntensity = 0.25f)
        {
            // TODO: Targeted actions
            if (Actors.Contains(srcEntityID) && PossibleVerbs.Contains(verbID))
            {
                SortedList<ushort, Reaction> reactions = new SortedList<ushort, Reaction>(Actors.Count);
                On_Action(srcEntityID, dstEntityID, verbID, verbIntensity, reactions);

                // TODO: Reactions handling

                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    internal static class Constants
    {
        // Actions that are available in every context
        public static SortedSet<ushort> UniversalVerbs;

        // So there is only one instance of each character, no copies
        // If access is too slow, can move to Stage instances
        public static Character[] CHARACTERS;
    }
}

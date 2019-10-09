using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphoLib.Verbs;
using ZachLib.Logging;

namespace SapphoLib
{
    internal static class Constants
    {
        static Constants()
        {
            LogManager.AddLog("Sappho", LogType.FolderFilesByDate);
            LogManager.Start(true);
        }

        public const float EMOTIONAL_INSTABILITY = 0.05f;

        // Actions that are available in every context
        public static SortedSet<ushort> UniversalVerbs;
        public static SortedDictionary<ushort, VerbInfo> VERBS_INFO = new SortedDictionary<ushort, VerbInfo>();

        // So there is only one instance of each character, no copies
        // If access is too slow, can move to Stage instances
        // Remember to set the first Actor to be Fate
        public static Actor[] CHARACTERS;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DnD
{
    public class RollTable
    {
        public const string EntriesPat = @"(?:[0-9]* ?[A-Z]|[0-9]+ [a-z]).+(?:\n(?:[a-z]).+)?";
        public const string RemoveUnwantedCharactersPat = @"[^A-Za-z ]";
        public static SpellChecker Checker = new SpellChecker();

        public string Title { get; set; }
        public string[] entries { get; set; }

        public RollTable(Match match)
        {
            this.Title = match.Groups[1].Value + "gp " + match.Groups[2].Value;
            string[] entryMatches = Regex.Matches(
                match.Groups[3].Value, 
                EntriesPat
            ).Cast<Match>().Select(m => m.Value).ToArray();

            this.entries = entryMatches.Select(
                m => Checker.Check(Regex.Replace(m, RemoveUnwantedCharactersPat, "").Trim())
            ).Select(m => m[0].ToString().ToUpper() + m.Substring(1)).ToArray();
        }

        public override string ToString()
        {
            return Title + "\n" + String.Join("\n", entries);
        }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace RiotSharp.StaticDataEndpoint.Rune
{
    /// <summary>
    /// A class representing a reforged rune slot (Static API).
    /// </summary>
    public class ReforgedRuneSlotStatic
    {
        /// <summary>
        /// A list of runes available for use in this slot.
        /// </summary>
        [JsonProperty("runes")]
        public List<ReforgedRuneStatic> Runes { get; set; }
    }
}

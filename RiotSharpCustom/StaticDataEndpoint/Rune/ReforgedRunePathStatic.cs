using Newtonsoft.Json;
using System.Collections.Generic;

namespace RiotSharp.StaticDataEndpoint.Rune
{
    /// <summary>
    /// Class representing a reforged rune (Static API).
    /// </summary>
    public class ReforgedRunePathStatic
    {
        /// <summary>
        /// Path to the icon.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// RunePath's id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Key of this RunePath.
        /// <para>This is diffrent from the Name attribute!
        /// (Name = ingame display name, Key = codebase name</para>
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Name of this RunePath.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}

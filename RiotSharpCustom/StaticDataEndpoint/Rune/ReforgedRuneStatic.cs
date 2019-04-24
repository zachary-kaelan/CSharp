using Newtonsoft.Json;
using System.Collections.Generic;

namespace RiotSharp.StaticDataEndpoint.Rune
{
    /// <summary>
    /// Class representing a reforged rune (Static API).
    /// </summary>
    public class ReforgedRuneStatic
    {
        /// <summary>
        /// Path to the Rune's icon.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Rune's id.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Key of this Rune.
        /// <para>This is diffrent from the Name attribute!
        /// (Name = ingame display name, Key = codebase name</para>
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// Name of this Rune.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A shortened description of the rune.
        /// </summary>
        [JsonProperty("shortDesc")]
        public string ShortDescription { get; set; }

        /// <summary>
        /// The full description of the rune.
        /// </summary>
        [JsonProperty("longDesc")]
        public string Description { get; set; }

        /// <summary>
        /// The name of the path this rune is a part of.
        /// </summary>
        [JsonProperty("runePathName")]
        public string RunePath { get; set; }

        /// <summary>
        /// The id of the path this rune is a part of.
        /// </summary>
        [JsonProperty("runePathId")]
        public int RunePathId { get; set; }


    }
}

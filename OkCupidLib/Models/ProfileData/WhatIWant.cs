using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models.ProfileData
{
    public enum LookingFor
    {
        new_friends,
        casual_sex,
        non_monogamous_relationships,
        long_term_dating,
        short_term_dating
    }

    public enum IWant
    {
        men,
        women,
        everybody
    }

    public class WhatIWant
    {
        public int metric { get; protected set; }
        public int singles_only { get; protected set; }

        public int[] gender_tags { get; protected set; }
        public AgeVice age { get; protected set; }
        public LookingForStrings looking_for { get; protected set; }

        public IWant i_want { get; protected set; }
        public int gentation { get; protected set; }
        public string monogamous { get; protected set; }
        public int near_me { get; protected set; }

        public Sentence sentence { get; protected set; }

        public int distance { get; protected set; }

    }

    public struct AgeVice
    {
        public int max { get; protected set; }
        public int min { get; protected set; } 
    }

    public class LookingForStrings
    {
        public string[] strings { get; protected set; }
        public LookingFor[] api_strings { get; protected set; } 
    }

    public class Sentence
    {
        public string prefix { get; protected set; }
        public string[] pieces { get; protected set; }
        public string sentence { get; protected set; }
    }
}

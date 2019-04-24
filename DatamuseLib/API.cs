using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatamuseLib.Models;
using ZachLib;
using ZachLib.HTTP;
using RestSharp;
using Jil;

namespace DatamuseLib
{
    [Flags]
    public enum APIConstraints
    {
        None = 0,
        MeansLike = 1,
        SoundsLike = 2,
        SpelledLike = 4,
        LeftContext = 8,
        RightContext = 16,
        Topics = 32
    }

    public enum ExternalConstraints
    {
        None = 0,
        Rel_NounsByAdjective = 1,
        Rel_AdjectivesByNoun,
        Rel_Synonyms,
        Rel_Triggers,
        Rel_Antonyms,
        Rel_Specific,
        Rel_General,
        Rel_Comprises,
        Rel_PartOf,
        Rel_FrequentFollower,
        Rel_FrequentPredecessor,
        Rel_Rhymes,
        Rel_ApproxRhymes,
        Rel_Homophones,
        Rel_ConsonantMatch
    }

    [Flags]
    public enum ExtraLexicalKnowledge
    {
        None = 0,
        Definitions = 1,
        PartsOfSpeech = 2,
        SyllableCount = 4,
        Pronunciation = 8,
        WordFrequency = 16
    }

    [Flags]
    public enum PartOfSpeech
    {
        Unknown = 0,
        Noun = 1,
        Verb = 2,
        Adjective = 4,
        Adverb = 8
    }

    public static class API
    {
        private static readonly RestClient CLIENT = new RestClient("https://api.datamuse.com/");
        private const string LEXICAL_FLAGS = "dpsrf";
        private static readonly string[] API_FLAGS = new string[]
        {
            "ml",
            "sl",
            "sp",
            "lc",
            "rc",
            "topics"
        };

        private static readonly string[] REL_FLAGS = new string[]
        {
            null,
            "jja",
            "jjb",
            "syn",
            "trg",
            "ant",
            "spc",
            "gen",
            "com",
            "par",
            "bga",
            "bgb",
            "rhy",
            "nry",
            "hom",
            "cns"
        };

        internal static readonly Dictionary<string, PartOfSpeech> PARTS_OF_SPEECH_DICT = new Dictionary<string, PartOfSpeech>()
        {
            { "n", PartOfSpeech.Noun  },
            { "v", PartOfSpeech.Verb },
            { "adj", PartOfSpeech.Adjective },
            { "adv", PartOfSpeech.Adverb },
            { "u", PartOfSpeech.Unknown }
        };

        private static T Words<T>(params Parameter[] parameters) => 
            JSON.Deserialize<T>(
                CLIENT.Execute(
                    new RestRequest("words").AddOrUpdateParameters(parameters)
                ).Content
            );

        #region Single Parameter Functions
        private static T SingleParameter<T>(string name, string value) =>
            Words<T>(new Parameter() { Name = name, Value = value });

        public static WordScore[] Autocomplete(string query) => 
            JSON.Deserialize<WordScore[]>(
                CLIENT.Execute(
                    new RestRequest("sug").AddParameter("s", query)
                ).Content
            );

        public static MeansLikeScore[] MeansLike(string query) =>
            SingleParameter<MeansLikeScore[]>("ml", query);

        public static SoundsLikeScore[] SoundsLike(string query) =>
            SingleParameter<SoundsLikeScore[]>("sl", query);

        public static WordScore[] SpelledLike(string query) =>
            SingleParameter<WordScore[]>("sp", query);

        public static SoundsLikeScore[] PerfectRhymes(string query) =>
            SingleParameter<SoundsLikeScore[]>("rel_rhy", query);

        public static SoundsLikeScore[] ApproxRhymes(string query) =>
            SingleParameter<SoundsLikeScore[]>("rel_nry", query);
        #endregion

        public static Parameter GetLexicalParam(ExtraLexicalKnowledge flags)
        {
            string str = "";
            int flagsNum = (int)flags;
            for (int i = 4; i >= 0; --i)
            {
                int num = Convert.ToInt32(Math.Pow(2, i));
                if (flagsNum >= num)
                {
                    flagsNum -= num;
                    str += LEXICAL_FLAGS[i];
                }
            }

            return new Parameter()
            {
                Name = "md",
                Value = str,
                Type = ParameterType.GetOrPost
            };
        }

        public static Parameter[] GetAPIParameters(APIConstraints apiFlags, IEnumerator paramsEnumerator)
        {
            List<Parameter> parameters = new List<Parameter>();
            int flagsNum = (int)apiFlags;
            for (int i = 5; i >= 0; --i)
            {
                int num = Convert.ToInt32(Math.Pow(2, i));
                if (flagsNum >= num)
                {
                    flagsNum -= num;
                    paramsEnumerator.MoveNext();
                    parameters.Add(
                        new Parameter()
                        {
                            Name = API_FLAGS[i],
                            Value = paramsEnumerator.Current,
                            Type = ParameterType.GetOrPost
                        }
                    );
                }
            }
            return parameters.ToArray();
        }

        public static Parameter GetRelParam(ExternalConstraints relFlags, string value)
        {
            int flagNum = (int)relFlags;
            return new Parameter()
            {
                Name = "rel_" + REL_FLAGS[flagNum],
                Value = value,
                Type = ParameterType.GetOrPost
            };
        }

        public static FullLexicalModel[] Words(APIConstraints apiFlags, ExternalConstraints relFlags, ExtraLexicalKnowledge lexFlags, params string[] paramValues)
        {
            List<Parameter> parameters = new List<Parameter>();
            if (apiFlags != APIConstraints.None)
                parameters.AddRange(GetAPIParameters(apiFlags, paramValues.GetEnumerator()));
            if (relFlags != ExternalConstraints.None)
                parameters.Add(GetRelParam(relFlags, paramValues.Last()));
            if (lexFlags != ExtraLexicalKnowledge.None)
                parameters.Add(GetLexicalParam(lexFlags));

            var response = CLIENT.Execute(
                new RestRequest(
                    "words",
                    Method.GET
                ).AddOrUpdateParameters(parameters)
            );

            return JSON.Deserialize<FullLexicalModel[]>(response.Content);
        }

        public static FullLexicalModel GetWordInfo(string word, ExtraLexicalKnowledge lexFlags)
        {
            var request = new RestRequest("words", Method.GET);
            request.AddParameter("sp", word, ParameterType.GetOrPost);
            request.AddParameter("qe", "sp", ParameterType.GetOrPost);
            request.AddParameter(GetLexicalParam(lexFlags));
            request.AddParameter("max", 1, ParameterType.GetOrPost);
            return JSON.Deserialize<FullLexicalModel[]>(CLIENT.Execute(request).Content).Single();
        }
    }
}

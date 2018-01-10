using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using RestSharp;

namespace DnD
{
    public class SpellChecker
    {
        public const string ApiKey1 = "11202eef0b2a4e7eabbc72aa5c4e8f9d";
        public const string ApiKey2 = "32ad9d24c7d14e44bebffb4ae64f29c2";
        private RestClient client { get; set; }
        private static string[] wordlist = System.IO.File.ReadAllLines(@"E:\Installations\Programming_Frameworks\DnD\wordlist.txt");

        public SpellChecker()
        {
            client = new RestClient("https://api.cognitive.microsoft.com/bing/v5.0/spellcheck");
            client.AddDefaultHeader("Ocp-Apim-Subscription-Key", ApiKey1);
        }

        public string Check(string toCheck)
        {
            if (!toCheck.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Any(w => !wordlist.Contains(w)))
                return toCheck;

            RestRequest request = new RestRequest(Method.POST);
            request.AddParameter("Text", toCheck, ParameterType.GetOrPost);
            SpellCheck check = JSON.Deserialize<SpellCheck>(client.Execute(request).Content);
            request = null;

            string newString = toCheck;
            foreach(Flagged token in check.flaggedTokens)
            {
                newString = newString.Replace(token.token, token.suggestions.First().suggestion);
            }

            return newString;
        }
    }

    public struct SpellCheck
    {
        public string _type { get; set; }
        public Flagged[] flaggedTokens { get; set; }
    }

    public struct Flagged
    {
        public int offset { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public Suggestion[] suggestions { get; set; }
    }

    public struct Suggestion
    {
        public string suggestion { get; set; }
        public string score { get; set; }
    }
}

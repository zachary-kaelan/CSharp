using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace DnD
{
    public enum SpellcheckMode { Proof, Spell}

    class SpellCheckerClient
    {
        const string Key1 = "11202eef0b2a4e7eabbc72aa5c4e8f9d";
        const string Key2 = "32ad9d24c7d14e44bebffb4ae64f29c2";
        public RestClient client { get; set; }

        public SpellCheckerClient()
        {
            client = new RestClient("https://api.cognitive.microsoft.com/bing/v5.0/spellcheck/");
            client.AddDefaultHeader("Ocp-Apim-Subscription-Key", Key1);
            //client.AddDefaultParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);
        }

        public string Check(string str, SpellcheckMode mode = SpellcheckMode.Proof)
        {
            RestRequest request = new RestRequest(mode.ToString());
            request.AddParameter("Text", str, ParameterType.QueryString);

        }
    }

    public struct SpellCheckerResponse
    {

    }
}

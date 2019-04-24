using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using RestSharp;

namespace Trello_API
{
    public static class API
    {
        static API()
        {
            CLIENT.AddDefaultParameter("key", API_KEY, ParameterType.GetOrPost);
            CLIENT.AddDefaultParameter("token", TOKEN, ParameterType.GetOrPost);
        }

        private const string API_KEY = "6ad2a7060e02053585f2293d44a383ce";
        private const string TOKEN = "16757f5153330c31fce067d70a1241820dfcfdd9f471319d4b86c94f9dcc311b";
        private const string TEAM_ID = "5b1e90038abff201b18394a4";
        private static RestClient CLIENT = new RestClient("https://api.trello.com/1/");

        public static void Initialize()
        {
            var response = CLIENT.Execute(new RestRequest("organizations/" + TEAM_ID, Method.GET));
            var json = JSON.DeserializeDynamic(response.Content);
            foreach(var id in json.idBoards)
            {
                
            }
        }
    }
}

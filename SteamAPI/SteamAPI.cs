using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using Jil;

namespace SteamAPI
{
    public static class SteamAPI
    {
        private static readonly RestClient client = new RestClient("https://api.steampowered.com/<interface>/<method>/v<version>/");

        static SteamAPI()
        {
            client.AddDefaultParameter("key", "9318DC68A16FBA37FE6AFF4A4EF260C4", ParameterType.GetOrPost);
        }

        private static RestRequest EncodeArray(string resource, params KeyValuePair<string, string>[] keyValues)
        {
            RestRequest request = new RestRequest(resource, Method.POST);
            int count = keyValues.Length;
            request.AddQueryParameter("count", count.ToString());
            for(int i = 0; i < count; ++i)
            {
                
            }
        }

        public static void GetAllApps()
        {
            RestRequest request = new RestRequest(Method.GET);
            request.AddUrlSegment("interface", "ISteamApps");
            request.AddUrlSegment("method", "GetAppList");
            request.AddUrlSegment("version", "v2");
            IEnumerable<App> apps = JSON.DeserializeDynamic(client.Execute(request).Content).applist.apps;


        }
    }
}

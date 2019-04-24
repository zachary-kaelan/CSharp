using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;

namespace PPWebLib
{
    public static class ReportWriter
    {
        private static readonly Lazy<RestClient> _client = new Lazy<RestClient>(
            () => Initialize()
        );

        internal static RestClient CLIENT { get => _client.Value; }

        private static RestClient Initialize()
        {
            RestClient client = new RestClient("http://reportwriter.pestpac.com/")
            {
                CookieContainer = PestPac.cookies,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299",
                FollowRedirects = true
            };

            string resource = PestPac.INSECURE_CLIENT.Execute(
                new RestRequest(
                    "PestPacReportWriter.asp",
                    Method.GET
                ).AddOrUpdateParameter(
                    "Action",
                    "CreateReport",
                    ParameterType.GetOrPost
                )
            ).Headers.First(p => p.Name == "Location").Value.ToString();

            string mainText = client.Execute(
                new RestRequest(
                    resource, 
                    Method.GET
                )
            ).Content;

            ParseMainText(mainText);
            return client;
        }

        private static void ParseMainText(string mainText)
        {

        }
    }
}

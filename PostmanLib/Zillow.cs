using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Jil;
using PestPac.Model;
using PostmanLib.Properties;
using PPLib;
using RestSharp;
using ZachLib;
using ZachLib.HTTP;
using ZachLib.Logging;

namespace PostmanLib
{
    public static class Zillow
    {
        static Zillow()
        {
            LogManager.AddLog(
                new Log(LogType.FolderFilesByDate, "Zillow", EntryType.ERROR)
            );
        }

        private static readonly string[] ZWSIDs = new string[]
        {
            "X1-ZWz191nio6qoln_82rpe",
            "X1-ZWz1923it7mpe3_6o78e",
            "X1-ZWz18z150ys74b_5cfwc"
        };

        public static readonly Dictionary<string, string> UserFields = new Dictionary<string, string>()
        {
            {"useCode", "Home Type"},
            {"bedrooms", "Bedrooms"},
            {"bathrooms", "Bathrooms"},
            {"finishedSqFt", "Square Ft"},
            {"lotSizeSqFt", "Lot SqFt"},
            {"yearBuilt", "Year Built"},
            {"taxAssessment", "TaxAssess"},
            {"zpid", "ZillowPID"},
            {"amount", "Home Value"},
            {"lastSoldDate", "LastSoldOn"},
            {"lastSoldPrice", "LastPrice"}
        };

        private static string ZWSID = ZWSIDs.First();
        private static int curZWSID = 0;
        private static readonly CookieContainer COOKIES = new CookieContainer();

        private static RestClient SearchClient = new RestClient("http://www.zillow.com/webservice/")
        {
            CookieContainer = COOKIES
        };
        private static RestClient DetailsClient = new RestClient("https://www.zillow.com/homes/")
        {
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36"
        };

        public static bool TryGetDeepSearchResults(string address, string citystatezip, out HomeFacts homefacts)
        {
            var response = SearchClient.Execute<HomeFacts>(
                new RestRequest(
                    "GetDeepSearchResults.htm",
                    Method.GET
                ){
                    RootElement = "result"
                }.AddOrUpdateParameters(
                    new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>("address", address),
                        new KeyValuePair<string, object>("citystatezip", citystatezip)
                    }
                )
            );

            if (response.IsSuccessful)
            {
                homefacts = response.Data;
                return true;
            }
            else
            {
                homefacts = new HomeFacts();
                bool contentShortMessage = response.Content.Length <= 20;
                LogManager.Enqueue(
                    new LogUpdate(
                        "Zillow",
                        address + " - " + citystatezip,
                        new object[]
                        {
                            response.StatusCode,
                            contentShortMessage ? 
                                response.Content : 
                                response.StatusDescription
                        },
                        new object[]
                        {
                            !contentShortMessage ?
                            response.Content :
                            (object)response
                        }
                    )
                );
                return false;
            }
        }
    }

    public struct HomeFacts
    {
        public string useCode { get; set; }
        public string bedrooms { get; set; }
        public string bathrooms { get; set; }
        public string finishedSqFt { get; set; }
        public string lotSizeSqFt { get; set; }
        public string yearBuilt { get; set; }
        public string taxAssessment { get; set; }
        public string lastSoldDate { get; set; }
        public string lastSoldPrice { get; set; }
        public string zpid { get; set; }
        public string amount { get; set; }
    }
}

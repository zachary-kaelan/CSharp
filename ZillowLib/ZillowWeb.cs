using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using ZachLib;
using ZachLib.HTTP;
using RGX.Zillow;

namespace ZillowLib
{
    public static class ZillowWeb
    {
        static ZillowWeb()
        {
            CLIENT.RemoveDefaultParameter("Accept");
            /*COOKIES.Add(new Cookie("_ga", "GA1.2.779303067.1535065909", "/", ".zillow.com"));
            COOKIES.Add(new Cookie("abtest", "3|DFdpzJz9HG27vZFGug", "/", ".zillow.com"));
            COOKIES.Add(new Cookie("zgsession", "1|83682181-c54f-4b1e-957b-3c38afbec9fe", "/", ".zillow.com"));
            COOKIES.Add(new Cookie("zguid", "23|%24b11ff2a7-2f1f-48f4-a17d-1865acb4abfd", "/", ".zillow.com"));*/
        }

        private static CookieContainer COOKIES = new CookieContainer();
        private static readonly RestClient CLIENT = new RestClient("https://www.zillow.com/")
        {
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36",
            FollowRedirects = true
        };
        private static readonly RegionID RGX_REGIONID = new RegionID();

        private static readonly Dictionary<string, string> REGION_SEARCH_PARAMS = new Dictionary<string, string>()
        {
            { "formids", "TextField,TextField_0,SubmitButton" },
            { "form:homevalues/HomeValues", "ZH4sIAAAAAAAAALWQQWvCQBCFJwcvlojgX9irwYYWLz2pl0IJQioWPE030zCa7C6TxG3_U_9a_0NjhVJEPGg9Pt7wvXnv8ws6_gUAugE8qILNplJTNNawxuKplZH-q6qUZMuaorVlM2MhXVv52DkwcCQVVzVlc8wpacpXEueTS7HhLza1UjvfBwiFcrbmcTZHwdL5VQBTtQtdCrr2PFINq2frEtzGo9u7Cxss_ol-WIQCSE9xMSvZtGLScJG19rk5PYCb_WA_c9UQoqP3odHDeHw_in0ZwOLUH07sGxd0xifH98yvlnfYfHOlhY_2-gbaFCUnSQMAAA~~" },
            { "reservedids", "form:homevalues/HomeValues" },
            { "submitmode", "" },
            { "submitname", "" },
            { "TextField", "" },
            { "homevalues/HomeValues/$geo$GeoSearchBar.$Form_SubmitButton_submitflag", "1" }
        };
        public static string GetRegionID(string search)
        {
            RestRequest request = new RestRequest("homevalues/HomeValues,$geo$GeoSearchBar.$Form.sdirect", Method.POST);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddParameters(REGION_SEARCH_PARAMS);
            request.AddParameter("TextField_0", search);
            string regionID = RGX_REGIONID.Match(CLIENT.Execute(request).Content).Groups[1].Value;
            if (String.IsNullOrWhiteSpace(regionID))
            {
                Console.WriteLine("CAPTCHA REQUIRED, press enter when completed... ");
                Console.ReadLine();
                return GetRegionID(search);
            }
            else
                return regionID;
        }

        public static TimeseriesResponse GetTimeseries(string regionID)
        {
            RestRequest request = new RestRequest("ajax/homevalues/data/timeseries.json", Method.GET);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddParameter("r", regionID, ParameterType.QueryString);
            request.AddParameter("m", "zhvi_plus_forecast", ParameterType.QueryString);
            request.AddParameter("dt", 1, ParameterType.QueryString);
            return JSON.Deserialize<Dictionary<string, TimeseriesResponse>>(CLIENT.Execute(request).Content).Values.First();
        }
    }

    public struct Point
    {
        public long x { get; private set; }
        public int y { get; private set; }
    }

    public class TimeseriesResponse
    {
        public string update_date { get; private set; }
        public Point[] data { get; private set; }
    }
}

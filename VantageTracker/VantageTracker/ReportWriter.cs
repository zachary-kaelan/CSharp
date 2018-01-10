using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Net;
using RestSharp;

namespace VantageTracker
{
    public class ReportWriter
    {
        public const string MainCodePat = "&quot;TempPath&quot;:&quot;(.+?)&quot;,";

        private string mainCode { get; set; }
        private RestClient client { get; set; }
        public readonly CookieContainer Cookies = new CookieContainer();
        public ReportWriter(CookieContainer cookies, string startingURL)
        {
            client = new RestClient("http://reportwriter.pestpac.com/");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            this.Cookies = cookies;
            client.CookieContainer = Cookies;
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("Host", "reportwriter.pestpac.com");

            /*string key = client.Execute(
                new RestRequest(
                    startingURL.Replace(
                        client.BaseUrl.ToString(), "")
                    , Method.GET
                )
            ).Headers.Single(h => h.Name == "Location").Value.ToString();

            string finalURL = client.Execute(
                new RestRequest(
                    key, Method.GET)
            ).Headers.Single(h => h.Name == "Location").Value.ToString();*/

            client.FollowRedirects = true;
            string content = client.Execute(
                new RestRequest(startingURL, Method.GET)
            ).Content;

            mainCode = Regex.Match(content, MainCodePat).Groups[1].Value;
            client.AddDefaultParameter("t", mainCode, ParameterType.QueryString);
        }

        public void CreateReport(bool isExpress = true)
        {
            RestRequest request = new RestRequest("eWebReports/wrajax/", Method.POST);
            request.AddParameter("application/json", "{\"args\":[{\"className\":\"\",\"value\":\"" + (isExpress ? "express" : "standard") + "\",\"isArray\":false},{\"className\":\"\",\"value\":\"Zach Testing\",\"isArray\":false},{\"className\":\"\",\"value\":false,\"isArray\":false},{\"className\":\"Settings\",\"value\":\"{\\\"BrowserType\\\":\\\"Chrome\\\",\\\"Language\\\":\\\"en-us,en-us-getting-started,en-us-tooltips\\\",\\\"SessionNum\\\":\\\"1\\\",\\\"ShowErrorDetail\\\":true,\\\"IsAdmin\\\":false,\\\"DevicePixelRatio\\\":1,\\\"IsExceptionServerEvent\\\":false}\",\"isArray\":false}]}", ParameterType.RequestBody);

        }
    }
}

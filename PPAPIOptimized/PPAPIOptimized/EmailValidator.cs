using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using Jil;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace PPAPIOptimized
{
    public enum EmailType { Valid, IncorrectSyntax, Invalid }

    public static class EmailValidator
    {
        private static readonly CookieContainer Cookies = new CookieContainer();

        private static RestClient gmailClient = new RestClient("https://accounts.google.com/InputValidator?resource=SignUp&service=mail");
        private const string gmailJSONTemplate = "{\"input01\":{\"Input\":\"GmailAddress\",\"GmailAddress\":\"{0}\"},\"Locale\":\"en\"}";

        private static RestClient yahooClient = new RestClient("https://login.yahoo.com/");

        private static Dictionary<string, Dictionary<string, string>> defaultQueries = new Dictionary<string, Dictionary<string, string>>()
        {
            {"yahoo.com", new Dictionary<string, string>()
                {
                    {"browser-fp-data", "{\"language\":\"en-US\",\"color_depth\":24,\"resolution\":{\"w\":1920,\"h\":1080},\"available_resolution\":{\"w\":1920,\"h\":1040},\"timezone_offset\":240,\"session_storage\":1,\"local_storage\":1,\"indexed_db\":1,\"open_database\":1,\"cpu_class\":\"unknown\",\"navigator_platform\":\"Win32\",\"do_not_track\":\"1\",\"canvas\":\"canvas winding:yes~canvas\",\"webgl\":1,\"adblock\":0,\"has_lied_languages\":0,\"has_lied_resolution\":0,\"has_lied_os\":0,\"has_lied_browser\":0,\"touch_support\":{\"points\":0,\"event\":0,\"start\":0},\"plugins\":{\"count\":4,\"hash\":\"661c5820a590770da622de5b5297c0bd\"},\"fonts\":{\"count\":49,\"hash\":\"73a5ce890bdadb0295b20ba41e66f0ff\"},\"ts\":{\"serve\":1505396249520,\"render\":1505396248914}}"},
                    {"specId", "yidReg"},
                    {"cacheStored", "true"},
                    {"crumb", "FhtevSboXsc"},
                    {"acrumb", "kqjbooS2"},
                    {"c", ""},
                    {"sessionIndex", ""},
                    {"done", "https://www.yahoo.com"},
                    {"googleIdToken", ""},
                    {"authCode", ""},
                    {"tos0", "yahoo_freereg|us|en-US"},
                    {"tos1", "yahoo_comms_atos|us|en-US"},
                    {"firstName", ""},
                    {"lastName", ""},
                    {"password", ""},
                    {"intCountryCode", "US"},
                    {"phone", ""},
                    {"mm", ""},
                    {"dd", ""},
                    {"yyyy", ""},
                    {"freeformGender", ""}
                }
            }
        };

        private static Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            {"yahoo.com", "https://login.yahoo.com/account/module/create?validateField=yid"}
        };

        public static HttpWebRequest defaultReq(string domain)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(urls[domain]);
            request.CookieContainer = Cookies;
            switch(domain)
            {
                case "yahoo.com":
                    request.Accept = "*/*";
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Referer = "https://login.yahoo.com/account/create?specId=yidReg";
                    request.Headers.Add("Origin", "https://login.yahoo.com");
                    request.KeepAlive = true;
                    request.Host = "login.yahoo.com";
                    return request;
                default:
                    throw new Exception();
            }
        }

        public static string POST(string domain, Dictionary<string, string> form)
        {
            foreach(KeyValuePair<string, string> kv in defaultQueries[domain])
            {
                if (!form.ContainsKey(kv.Key))
                    form.Add(kv.Key, kv.Value);
            }

            HttpWebRequest request = defaultReq(domain);
            string response = null;

            StringBuilder sb = new StringBuilder("&");
            foreach(KeyValuePair<string, string> prm in form)
            {
                sb.Append(HttpUtility.UrlEncode(prm.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(prm.Value));
                sb.Append("&");
            }

            byte[] data = Encoding.ASCII.GetBytes(sb.ToString(0, sb.Length - 1));
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            do
            {
                response = new StreamReader(
                    ((HttpWebResponse)request.GetResponse())
                    .GetResponseStream()
                ).ReadToEnd();
            } while (response.Contains("error"));

            return response;
        }

        public static void Setup()
        {
            yahooClient.FollowRedirects = false;
            yahooClient.CookieContainer = Cookies;
            yahooClient.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            yahooClient.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            yahooClient.Execute(new RestRequest(Method.GET));
            yahooClient.Execute(new RestRequest("account/create?specId=yidReg", Method.GET));

            /*yahooClient.AddDefaultParameter("browser-fp-data", "{\"language\":\"en-US\",\"color_depth\":24,\"resolution\":{\"w\":1920,\"h\":1080},\"available_resolution\":{\"w\":1920,\"h\":1040},\"timezone_offset\":240,\"session_storage\":1,\"local_storage\":1,\"indexed_db\":1,\"open_database\":1,\"cpu_class\":\"unknown\",\"navigator_platform\":\"Win32\",\"do_not_track\":\"1\",\"canvas\":\"canvas winding:yes~canvas\",\"webgl\":1,\"adblock\":0,\"has_lied_languages\":0,\"has_lied_resolution\":0,\"has_lied_os\":0,\"has_lied_browser\":0,\"touch_support\":{\"points\":0,\"event\":0,\"start\":0},\"plugins\":{\"count\":4,\"hash\":\"661c5820a590770da622de5b5297c0bd\"},\"fonts\":{\"count\":49,\"hash\":\"73a5ce890bdadb0295b20ba41e66f0ff\"},\"ts\":{\"serve\":1505396249520,\"render\":1505396248914}}", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("specId", "yidReg", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("cacheStored", "true", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("crumb", "FhtevSboXsc", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("acrumb", "kqjbooS2", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("c", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("sessionIndex", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("done", "https://www.yahoo.com", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("googleIdToken", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("authCode", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("tos0", "yahoo_freereg|us|en-US", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("tos1", "yahoo_comms_atos|us|en-US", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("firstName", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("lastName", "", ParameterType.GetOrPost);
            //yahooClient.AddDefaultParameter("yid", "neddy67", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("password", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("intCountryCode", "US", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("phone", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("mm", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("dd", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("yyyy", "", ParameterType.GetOrPost);
            yahooClient.AddDefaultParameter("freeformGender", "", ParameterType.GetOrPost);

            yahooClient.AddDefaultHeader("Origin", "https://login.yahoo.com");
            yahooClient.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            yahooClient.AddDefaultHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            yahooClient.AddDefaultHeader("Referer", "https://login.yahoo.com/account/create?specId=yidReg");
            yahooClient.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");*/
            //yahooClient.AddDefaultHeader("Accept", "*/*");

            /*yahooClient.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");

            RestRequest request = new RestRequest("account/module/create", Method.POST);
            request.AddQueryParameter("validateField", "yid");
            request.AddParameter("yid", "neddy67", ParameterType.GetOrPost);
            yahooClient.FollowRedirects = true;
            var response = yahooClient.Execute(request);*/
        }

        public static async Task<EmailType> VerifyEmail(string address)
        {
            string[] components = address.Split('@');
            RestRequest request = new RestRequest(Method.POST);

            switch (components[1])
            {
                case "gmail.com":
                    request.RequestFormat = DataFormat.Json;
                    request.AddParameter("application/json", gmailJSONTemplate.Replace("{0}", components[0]), ParameterType.RequestBody);
                    GmailResponse response = await Task.Factory.StartNew<GmailResponse>(() => JSON.Deserialize<GmailResponse>(JSON.Deserialize<Dictionary<string, object>>(gmailClient.Execute(request).Content)["input01"].ToString()));
                    if (response.ErrorMessage != null && response.ErrorMessage.Contains("username"))
                        return EmailType.Valid;
                    return EmailType.Invalid;
            }

            return EmailType.IncorrectSyntax;
        }
    }

    public struct GmailResponse
    {
        public string Valid;
        public string ErrorMessage;
        public Dictionary<string, string> Errors;
        public string[] ErrorData;
    }

    public static class RequestExt
    {
        public static HttpWebRequest Clone(this HttpWebRequest req)
        {
            return req;
        }
    }
}

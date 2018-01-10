using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace Snapchat_API
{
    class Client
    {
        private readonly CookieContainer Cookies = new CookieContainer();

        private readonly Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            {"base", "https://www.instagram.com"},
            {"ajax", "https://www.instagram.com/ajax/bz"}
        };

        public HttpWebRequest defaultReq(string url)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US, en;q=0.8");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            request.Headers.Add("Origin", urls["base"]);
            //request.Host = "https://myvantagetracker.com";
            request.AllowAutoRedirect = false;
            request.CookieContainer = this.Cookies;
            request.KeepAlive = true;
            //request.Connection = "keep-alive";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");

            return request;
            //request.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
        }

        public string POST(string URL, Dictionary<string, string> form)
        {
            HttpWebRequest request = this.defaultReq(urls[URL]);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string response = null;

            do
            {
                HttpWebRequest req = request.Clone();
                form[form.Keys.Single(k => k.Contains("token"))] = this.GetToken(tokens[URL]);
                req.Referer = this.referer;
                StringBuilder sb = new StringBuilder();
                foreach (var prm in form)
                {
                    if (sb.Length != 0)
                        sb.Append("&");
                    sb.Append(HttpUtility.UrlEncode(prm.Key));
                    sb.Append("=");
                    sb.Append(HttpUtility.UrlEncode(prm.Value));
                }

                var data = Encoding.ASCII.GetBytes(sb.ToString());
                request.ContentLength = data.Length;
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                response = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();
            } while (response.Contains("error"));
            return response;
        }

        public string GetToken(string URL)
        {
            HttpWebRequest request = this.defaultReq(URL);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = String.IsNullOrEmpty(this.referer) ? URL : this.referer;
            this.referer = URL;

            return Regex.Match(
                    new StreamReader(new GZipStream(
                        ((HttpWebResponse)request.GetResponse())
                        .GetResponseStream(), CompressionMode.Decompress))
                    .ReadToEnd(),
                    "token.+?value=\"(.+?)\"").Groups[1]
            .Value;
        }
    }
}

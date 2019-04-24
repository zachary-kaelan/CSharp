using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Extensions;
using Jil;
using ZachLib;

namespace ImagesDownloader
{
    class Program
    {
        public static readonly Regex RGX_VSCO_JSON = new Regex(@"<script>\s+window[^{]+({[^\n]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(50));
        public static readonly Regex RGX_VSCO_LINKS = new Regex("responsiveUrl\":\"([^\"]{11,})", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
        public static readonly RestClient CLIENT_VSCO = new RestClient("https://vsco.co/ajxp/eo05qnocc0cd5ci9dob2c1kuj2/2.0/")
        {
            FollowRedirects = true
        };
        public static readonly RestClient CLIENT_VSCO_IMAGES = new RestClient("im.vsco.co/");

        static void Main(string[] args)
        {
            var content = CLIENT_VSCO.Execute(
                new RestRequest(
                    "medias",
                    Method.GET
                ).AddOrUpdateParameters(
                    new Dictionary<string, string>()
                    {
                        {"site_id", "9735876" },
                        {"page", "1" },
                        {"size","250" }
                    }
                )
            ).Content;

            Dictionary<string, object>[] media = (Dictionary<string, object>[])JSON.Deserialize<Dictionary<string, object>>(content
               
            )["media"];

            Uri[] urls = media.Select(m => new Uri(m["responsive_url"].ToString())).ToArray();
            media = null;

            foreach(var url in urls)
            {
                CLIENT_VSCO_IMAGES.DownloadData(new RestRequest(url.PathAndQuery, Method.GET)).SaveAs(@"C:\Program Files\Microsoft Games\Mahjong\en-US\resources\assets\Stalking\Casey Hascup\VSCO\" + url.Segments.Last());
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}

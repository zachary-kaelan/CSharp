using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jil;
using RestSharp;

namespace DnDScrapingLib
{
    public static class DnDBeyond
    {
        static DnDBeyond()
        {
            _cookies.Add(new Cookie("__cfduid", "df993d0f4e536acd19cc247b6f91a8fa11564664899", "/", "/"));
            _cookies.Add(new Cookie("ResponsiveSwitch.DesktopMode", "1", "/", "/"));
            _cookies.Add(new Cookie("sublevel", "ANON", "/", "/"));
            _cookies.Add(new Cookie("Preferences", "undefined", "/", "/"));
            _cookies.Add(new Cookie("__extfc", "1", "/", "/"));
            _cookies.Add(new Cookie("LoginState", "c20a100c-6dd0-4c9a-8fe7-75f454acc740", "/", "/"));
            _cookies.Add(new Cookie("_attrg", "null", "/", "/"));
            _cookies.Add(new Cookie("_attrb", "%22f25ff6b4-9430-42e4-8b6b-ba803f88f3ba%22", "/", "/"));
            _cookies.Add(new Cookie("G_ENABLED_IDPS", "google", "/", "/"));
            _cookies.Add(new Cookie("Preferences.TimeZoneID", "1", "/", "/"));
            _cookies.Add(new Cookie("CobaltSession", "eyJhbGciOiJkaXIiLCJlbmMiOiJBMTI4Q0JDLUhTMjU2In0..Rs1MJNif6P-2QR8pcq4iLg.CkHT8bgEFVvcb2l1kbu_MVyzS8QI-YCz3C-mqisusn-SlpHHxJ1U-xEUbJf5KHSY.KuJEP8sW-I7xXHSrlgcKbg", "/", "/"));
            _cookies.Add(new Cookie("User.ID", "105583383", "/", "/"));
            _cookies.Add(new Cookie("User.Username", "Zephandrypus", "/", "/"));
            _cookies.Add(new Cookie("Preferences.Language", "1", "/", "/"));
            _cookies.Add(new Cookie("_attru", "%22105583383%22", "/", "/"));
            _cookies.Add(new Cookie("_ga", "GA1.2.45107987.1564664950", "/", "/"));
            _cookies.Add(new Cookie("_gid", "GA1.2.579829833.1564664950", "/", "/"));
            _cookies.Add(new Cookie("cdmgeo", "us", "/", "/"));
            _cookies.Add(new Cookie("_fbp", "fb.1.1564664950550.860262062", "/", "/"));
            _cookies.Add(new Cookie("__gads", "ID=602fe27f35873b89:T=1564667042:S=ALNI_MbFWuxfNIHm764GdLFiBwp6iraBMg", "/", "/"));
            _cookies.Add(new Cookie("ddb.toast.monster.homebrew-create-copy.hide-toast", "true", "/", "/"));
            _cookies.Add(new Cookie("WarningNotification.Lock", "1", "/", "/"));
            _cookies.Add(new Cookie("AWSELB", "17A593B6CA59C3C4856B812F84CD401A582EF08337A17B659D8E27E0358104ABF15CD902B284A9F525C1AA0DF220CB30AEE9DCF62B803D58969BA5E2872B9984581B5ED2", "/", "/"));
            _cookies.Add(new Cookie("_gat_UA-26524418-48", "1", "/", "/"));

        }

        private static readonly CookieContainer _cookies = new CookieContainer();
        private static readonly RestClient _client = new RestClient("https://www.dndbeyond.com/")
        {
            CookieContainer = _cookies,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36"
        };

        public static void GetMonsterInfo(uint id)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(_client.Execute(new RestRequest("homebrew/creation/create-monster")).Content);
            var node = doc.DocumentNode.SelectSingleNode("./html/body/div[2]/div/div[3]/div/section/div/div/div/form");

            RestRequest request = new RestRequest("homebrew/creations/create-monster", Method.POST);
            request.AddQueryParameter("security-token", node.ChildNodes[0].GetAttributeValue("value", null));
            request.AddQueryParameter(node.ChildNodes[1].Name, node.ChildNodes[1].GetAttributeValue("value", null));
            request.AddQueryParameter("monster-type", "");
            request.AddQueryParameter("monster", id.ToString());

            doc.LoadHtml(_client.Execute(request).Content);
            node = doc.DocumentNode.SelectSingleNode("./html/body/div[2]/div/div[3]/div/section/div/div/form/div[2]");
        }
    }
}

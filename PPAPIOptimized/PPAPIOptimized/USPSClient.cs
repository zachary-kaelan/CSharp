using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RestSharp;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Management;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace PPAPIOptimized
{
    public class USPSClient
    {
        public const string baseURL = "http://production.shippingapis.com/";
        public const string fullURL = baseURL + "ShippingAPI.dll";
        public const string USERID = "284INSIG3782";
        private static readonly string ZipLookupTemplate =
            HttpUtility.UrlEncode(
                "<ZipCodeLookupRequest USERID=\"") + "{0}" + HttpUtility.UrlEncode("\">" +
                    "<Address ID = '0'>" +
                        "<FirmName></FirmName>" +
                        "<Address1></Address1>" +
                        "<Address2>") + "{1}" + HttpUtility.UrlEncode("</Address2>" + 
                        "<City>") + "{2}" + HttpUtility.UrlEncode("</City>" + 
                        "<State>") + "{3}" + HttpUtility.UrlEncode("</State>" +
                        "<Zip5>") + "{4}" + HttpUtility.UrlEncode("</Zip5>" +
                    "</Address>" + 
                "</ZipCodeLookupRequest>"
            );

        private static readonly string CityStateLookupTemplate =
            HttpUtility.UrlEncode(
                "<CityStateLookupRequest USERID=\"") + "{0}" + HttpUtility.UrlEncode("\">" +
                    "<ZipCode ID='0'>" +
                        "<Zip5>") + "{1}" + HttpUtility.UrlEncode("</Zip5>" +
                    "</ZipCode>" + 
                "</CityStateLookupRequest>"
            );

        private static readonly string VerifyTemplate =
            HttpUtility.UrlEncode(
                "<AddressValidateRequest USERID=\"") + "{0}" + HttpUtility.UrlEncode("\">" + // Param 1, UPS User ID
                    "<Revision>1</Revision>" +
                    "<Address ID = \"0\">" +
                        "<Address1></Address1>" +
                        "<Address2>") + "{1}" + HttpUtility.UrlEncode("</Address2>" +        // Param 2, Address
                        "<City></City>" +
                        "<State></State>" +
                        "<Zip5>") + "{2}" + HttpUtility.UrlEncode("</Zip5>" +                // Param 3, Zip Code
                        "<Zip4></Zip4>" +
                    "</Address>" +
                "</AddressValidateRequest>"
            );

        public RestClient client { get; set; }
        //private RestClient googleClient { get; set; }
        private readonly CookieContainer cookies = new CookieContainer();

        //public static XmlSerializer ZipSerializer = new XmlSerializer(typeof(string), new XmlRootAttribute("Zip4"));
        private static readonly XmlSerializer FixInfoSerializer = new XmlSerializer(typeof(ZipInfo), new XmlRootAttribute("ZipCode"));
        private static readonly XmlSerializer ValidationSerializer = new XmlSerializer(typeof(VerifyResponse), new XmlRootAttribute("Address"));
        private static readonly XmlDocument Document = new XmlDocument();

        public USPSClient()
        {
            client = new RestClient(baseURL);
            client.CookieContainer = cookies;
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            client.FollowRedirects = true;

            client.Proxy = new WebProxy("127.0.0.1", 9666);
            //client.Proxy = new IPSProxy("HTTPS", client);
            //xmlTool.Culture = CultureInfo.CurrentCulture;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //googleClient = new RestClient("http://maps.googleapis.com/maps/api/");
            //var statuses = NetworkInterface.GetAllNetworkInterfaces().GroupBy(i => i.OperationalStatus);
            
        }

        public string FullUpdate(ref Dictionary<string, string> info)
        {
            bool missingCityState = ((info.TryGetValue("City", out string city) && String.IsNullOrWhiteSpace(city)) || (info.TryGetValue("State", out string state) && String.IsNullOrWhiteSpace(state)));
            if (info.TryGetValue("Zip", out string zip) && missingCityState)
            {
                Document.Load(
                    fullURL + "?API=CityStateLookup&XML=" + HttpUtility.UrlEncode(
                        String.Format(
                            CityStateLookupTemplate,
                            USERID, zip
                        )
                    )
                );


            }

            return "";
        }

        public string GetZipExt(Dictionary<string, string> info)
        {
            string XML = String.Format(
                ZipLookupTemplate, USERID,
                info["Address"], info["City"],
                info["State"], info["Zip"]
            );

            RestRequest request = new RestRequest("ShippingAPI.dll", Method.GET);
            request.AddQueryParameter("API", "ZipCodeLookup");
            request.AddQueryParameter("XML", XML);

            string response = this.client.Execute(request).Content;
            request = null;
            XML = null;

            var elements = XDocument.Parse(response).Descendants("Zip4");
            if (elements.Count() == 0)
                return info["Zip"];
            string zip4 = elements.First().Value;
            response = null;
            elements = null;
            return info["Zip"] + "-" + zip4;
        }

        public KeyValuePair<string,string> FixInfo(string zip)
        {
            string XML = String.Format(
                CityStateLookupTemplate,
                USERID, zip
            );

            RestRequest request = new RestRequest("ShippingAPI.dll", Method.GET);
            request.AddQueryParameter("API", "CityStateLookup");
            request.AddQueryParameter("XML", XML);
            string response = this.client.Execute(request).Content;
            request = null;
            XML = null;

            XmlReader reader = XmlReader.Create(new StringReader(response));
            reader.ReadToDescendant("ZipCode");
            reader = reader.ReadSubtree();
            ZipInfo addr = (ZipInfo)FixInfoSerializer.Deserialize(reader);

            reader.Close();
            reader = null;
            response = null;
            return new KeyValuePair<string, string>(addr.City, addr.State);
        }

        public VerifyResponse AddressValidation(string address, string zip)
        {
            string XML = String.Format(
                VerifyTemplate,
                USERID,
                address, zip
            );

            RestRequest request = new RestRequest("ShippingAPI.dll", Method.GET);
            request.AddQueryParameter("API", "Verify");
            request.AddQueryParameter("XML", XML);
            string response = this.client.Execute(request).Content;

            XmlReader reader = XmlReader.Create(new StringReader(response));
            reader.ReadToDescendant("Address");
            reader = reader.ReadSubtree();
            VerifyResponse verified = (VerifyResponse)ValidationSerializer.Deserialize(reader);

            reader.Close();
            reader = null;
            response = null;
            return verified;
        }
    }

    public struct IPSProxy : IWebProxy
    {
        private const string UPSBase = "http://production.shippingapis.com/_bpb/1/";
        private const string UPSExt = "_bpb/1/";
        private static Uri BaseURL = new Uri("https://bpb.opendns.com/");
        private static Uri UPSBaseURL = new Uri(UPSBase);
        private const string GetLocationPattern = "href=\"" + UPSBase + @"(.+?)(?:-|:)(.+?):.+?((?:\/|\?).+?&)amp;(.+?)" + "\"";
        //private const string GetLocationPattern = "href=\"(http[s]?:" + @"\/\/.+?\/)((?:\/?[^" + "\"]*?" + @"\/?)*?\?(?:[^" + "\"&]*?&))amp;(.*?)\"";
        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0);
        public ICredentials Credentials { get; set; }
        private string protocol { get; set; }
        private RestClient client { get; set; }
        private RestClient mainClient { get; set; }
        private CookieContainer Cookies { get; set; }
        private CookieContainer ClientCookies { get; set; }
        private long Exp { get; set; }
        private bool bypassed { get; set; }

        public IPSProxy(string prtcl, RestClient upsClient)
        {
            protocol = prtcl;
            Credentials = new NetworkCredential();
            client = new RestClient(BaseURL);
            Cookies = new CookieContainer();
            client.CookieContainer = Cookies;
            ClientCookies = upsClient.CookieContainer;
            mainClient = new RestClient(UPSBase);
            mainClient.CookieContainer = ClientCookies;
            Exp = 0;
            bypassed = false;
        }

        public Uri GetProxy(Uri destination)
        {
            string host = destination.Host;
            client.FollowRedirects = true;
            /*var responseDebug = */client.Execute(new RestRequest("a/" + host, Method.GET));
            RestRequest bypass = new RestRequest("a/" + host, Method.POST);
            bypass.AddParameter("code", "RLOIB", ParameterType.GetOrPost);
            bypass.AddParameter("bypass-code", "1", ParameterType.GetOrPost);
            client.FollowRedirects = false;
            var response = client.Execute(bypass);
            Match url = Regex.Match(response.Content, GetLocationPattern);
            /*client.BaseUrl = UPSBaseURL;
            var final = client.Execute(
                new RestRequest(
                    url.Groups[1].Value + ":" + url.Groups[2].Value +
                    ":2196619:65184281:-2151741:56/" + url.Groups[3].Value, 
                    Method.GET
            ));//.ResponseUri;
            ClientCookies.Add(Cookies.GetCookies(final.ResponseUri).Cast<Cookie>().Single(c => c.Name == "xodbpb"));*/


            /*Cookie cookie = new Cookie(
                "xodbpb",
                url.Groups[1].Value +
                ":" + url.Groups[2].Value +
                    ":2196619:65184281:-2151741:56",
                "/"
            )
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddSeconds(150).ToUniversalTime(),
                Domain = host
            };
            ClientCookies.Add(cookie);*/

            string ProxiedDestination = url.Groups[1].Value +
                ":" + url.Groups[2].Value +
                ":2196619:65184281:-2151741:56" +
                destination.PathAndQuery;
            mainClient.Execute(new RestRequest(ProxiedDestination, Method.GET));
            bypassed = true;

            //Exp = Convert.ToInt64(url.Groups[2].Value.Split('=')[1]);
            //bypassed = true;
            //client.BaseUrl = BaseURL;
            return destination;
        }

        public bool IsBypassed(Uri host)
        {
            bool temp = bypassed;
            bypassed = false;
            return temp || NetworkInterface.GetAllNetworkInterfaces()
                .Single(i => i.Description == "Realtek USB GbE Family Controller" || i.Description == "Realtek PCIe GBE Family Controller")
                .OperationalStatus == OperationalStatus.Down;
        }
    }

    public struct Bypass : ICredentials
    {
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            return new NetworkCredential();
        }
    }
}

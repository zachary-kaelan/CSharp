using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using RestSharp;
using RestSharp.Deserializers;
using PPLib;

namespace PPAPIOptimized
{
    public class ZillowClient
    {
        //public static StreamWriter logZillow = new StreamWriter(LOGS_PATH + DateTime.Now.ToString(Form1.FILENAME_DATE_FORMAT) + ".txt", true) { AutoFlush = true };
        //public static StreamWriter logMissingAddresses = null;

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

        //public const string PropertyInfoPat = @"(?:hdp-fact-category[^>]+>([^<]+)(?:<[^<]+){2}((?:<li(?:[^<]+<)+?\/li>\s+)*))|(?:(?:hdp-fact(?:-ataglance)?-(?:heading|name)" + "\" >)([^" + @"\s][^<]*)<[^<]+<[^" + "\"]+.[^\"]+value\">([^<]+)<)";
        //public const string PropertyInfoPat = "(?:hdp-fact(?:-ataglance)?-(?:heading|name)\" >)([^" + @"\s][^<]*)<[^<]+<[^" + "\"]+.[^\"]+value\">([^<]+)<";
        public const string LOGS_PATH = @"C:\DocUploads\Logs\Zillow\";
        public const string PropertyInfoPat = "(?:(?:hdp-fact(?:-ataglance)?-(?:heading|name)\">)([^ ][^<]*)<[^<]+)?(?:<[^" + "\"]+.[^\"]+value\">([^<]+)<)";
        public const string AddressLogErrorPat = @"\[[\d:]+\] \[[A-Z]+\]:\tError: no exact match found for input address ~ (.+)";
        // Regex wasn't working because autoformat placed a single extra space that I didn't notice, prior to setting the quotations right
        public string ZWSID { get; set; }
        public int curZWSID { get; set; }
        private readonly CookieContainer Cookies = new CookieContainer();
        public static DotNetXmlDeserializer XMLTool = new DotNetXmlDeserializer() { RootElement="SearcResults"};
        public static XmlSerializer Serializer = new XmlSerializer(typeof(HomeFacts), new XmlRootAttribute("editedFacts"));
        public static XmlSerializer DeepSerializer = new XmlSerializer(typeof(HomeFacts), new XmlRootAttribute("result"));
        public static TimeSpan REGEX_TIMEOUT = TimeSpan.FromMilliseconds(1000);
        public static Regex rgxAlphaNumeric = new Regex("[^A-Za-z0-9]");
        public static Regex rgxNumeric = new Regex("[^0-9]");
        private RestClient SearchClient { get; set; }
        private RestClient DetailsClient { get; set; }
        private LogManager Logger { get; set; }

        public ZillowClient(LogManager logger)
        {
            //Cookies.Add(new Cookie("abtest", "3|DFOTjAcraKnyD7HhsQ") { Domain="www.zillow.com"});
            //Cookies.Add(new Cookie("zguid", "23|%2495fd25c7-d4d2-41e4-b5e0-ba86cfb225f0") { Domain = "www.zillow.com" });
            //Cookies.Add(new Cookie("AWSALB", "9WQXqggXL5y1P0Oug6XDGLOhauE4YLH1TT7XUvqaMs75/vvrzcRfBmY9v7tUoUlkfqs8SVXNuLza0Qf284CMXZAIXfkKWY6Pa6WRe292SuKMJZXzmBA40DxZq0t+") { Domain = "www.zillow.com" });
            this.Logger = logger;

            Logger.AddLogs(LOGS_PATH, LOGS_PATH + "MissingAddresses.txt");
            //logMissingAddresses = new StreamWriter(LOGS_PATH + "MissingAddresses.txt", true);

            ZWSID = ZWSIDs[0];
            this.curZWSID = 0;
            SearchClient = new RestClient("http://www.zillow.com/webservice/");
            SearchClient.CookieContainer = Cookies;
            SearchClient.AddDefaultParameter("zws-id", ZWSID, ParameterType.GetOrPost);

            DetailsClient = new RestClient("https://www.zillow.com/homes/");
            DetailsClient.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            DetailsClient.CookieContainer = Cookies;
        }

        public XDocument GetSearchResults(string address, string citystatezip)
        {
            RestRequest req = new RestRequest("GetSearchResults.htm", Method.GET);
            req.AddParameter("address", address, ParameterType.GetOrPost);
            req.AddParameter("citystatezip", citystatezip, ParameterType.GetOrPost);
            XDocument doc = XDocument.Parse(SearchClient.Execute(req).Content);
            var elements = doc.Descendants("message");

            string addressZip = address + ", " + citystatezip;
            if (elements.Count() == 0)
            {
                Logger.Enqueue(
                    new LogUpdate(
                        "Zillow",
                        EntryType.ERROR,
                        "API CALL FAILED",
                        addressZip
                    )
                );
                return null;
            }
            else if (elements.First().Descendants("limit-warning").Count() != 0)
                UpdateZWSID();
            else if (elements.First().Descendants("code").First().Value != "0")
            {
                Logger.Enqueue(
                    new LogUpdate(
                        "Zillow", 
                        EntryType.ERROR,
                        elements.First().Descendants("text").First().Value,
                        addressZip
                    )
                );

                Logger.Enqueue(
                    new LogUpdate(
                        "MissingAddresses",
                        addressZip
                    )
                );
                
                return null;
            }

            return doc;
        }

        private void UpdateZWSID()
        {
            ++curZWSID;
            if (curZWSID == ZWSIDs.Length)
                throw new Exception("Exceeded rate limit.");
            ZWSID = ZWSIDs[curZWSID];
            SearchClient.RemoveDefaultParameter("zws-id");
            SearchClient.AddDefaultParameter("zws-id", ZWSID, ParameterType.GetOrPost);
        }

        public HomeFacts GetUpdatedPropertyDetails(string zpid)
        {
            RestRequest req = new RestRequest("GetUpdatedPropertyDetails.htm", Method.GET);
            req.AddParameter("zpid", zpid, ParameterType.GetOrPost);
            XmlReader reader = XmlReader.Create(new StringReader(SearchClient.Execute(req).Content));
            reader.ReadToDescendant("editedFacts");
            if (reader.IsEmptyElement || !reader.HasValue)
                return new HomeFacts();
            reader = reader.ReadSubtree();
            return (HomeFacts)Serializer.Deserialize(reader);
        }

        public HomeFacts GetDeepSearchResults(string address, string citystatezip)
        {
            RestRequest req = new RestRequest("GetDeepSearchResults.htm", Method.GET);
            req.AddParameter("address", address, ParameterType.GetOrPost);
            req.AddParameter("citystatezip", citystatezip, ParameterType.GetOrPost);

            string xml = SearchClient.Execute(req).Content;
            req = null;
            XDocument doc = XDocument.Parse(xml);
            var elements = doc.Descendants("message");

            string addressZip = address + ", " + citystatezip;
            if (elements.Count() == 0)
            {
                Logger.Enqueue(
                    new LogUpdate(
                        "Zillow",
                        EntryType.ERROR,
                        "API CALL FAILED",
                        addressZip
                    )
                );
                return new HomeFacts();
            }
            else if (elements.First().Descendants("limit-warning").Count() != 0)
                UpdateZWSID();
            else if (elements.First().Descendants("code").First().Value != "0")
            {
                Logger.Enqueue(
                    new LogUpdate(
                        "Zillow",
                        EntryType.ERROR,
                        elements.First().Descendants("text").First().Value,
                        addressZip
                    )
                );

                Logger.Enqueue(
                    new LogUpdate(
                        "MissingAddresses",
                        addressZip
                    )
                );
                return new HomeFacts();
            }

            XmlReader reader = XmlReader.Create(new StringReader(xml));
            xml = null;
            reader.ReadToDescendant("result");
            reader = reader.ReadSubtree();
            HomeFacts facts = (HomeFacts)DeepSerializer.Deserialize(reader);
            reader.Close();
            reader = null;

            facts.amount = doc.Descendants("amount").First().Value;
            doc = null;

            return facts;
        }

        public static void TryWriteLineAsync(string line, StreamWriter streamWriter = null)
        {
            if (streamWriter == null)
                return;// streamWriter = logZillow;
            while (true)
            {
                try
                {
                    Console.WriteLine(line);
                    streamWriter.WriteLineAsync(line);
                    break;
                }
                catch
                {
                    Thread.Sleep(Form1.gen.Next(50, 150));
                }
            }
        }

        public Dictionary<string, string[]> GetDetails(string address, string citystatezip)
        {
            XDocument doc = null;
            string link = null;
            string homeValue = null;

            try
            {
                doc = GetSearchResults(address, citystatezip);
                link = doc.Descendants("homedetails").First().Value.Substring(34);
                homeValue = doc.Descendants("zestimate").First().Descendants("amount").First().Value;
                doc = null;
            }
            catch
            {
                if (doc.Descendants("code").Last().Value == "508")
                    Logger.Enqueue(
                        new LogUpdate(
                            "Zillow",
                            EntryType.ERROR,
                            "NOT FOUND",
                            address + ", " + citystatezip
                        )
                    );
                else
                    throw new Exception();

                return null;
            }


            link = DetailsClient.Execute(new RestRequest(link, Method.GET)).Content;

            link = link.Substring(link.IndexOf("fios-scroll-tracker"));
            link = link.Substring(0, link.IndexOf("additional-links-left"));

            /*MatchCollection matchesTemp = Regex.Matches(
                link, PropertyInfoPat,
                RegexOptions.IgnoreCase,
                REGEX_TIMEOUT
            );*/

            List<KeyValuePair<string, string>> matches = Regex.Matches(
                link, PropertyInfoPat,
                RegexOptions.None,
                REGEX_TIMEOUT
            ).Cast<Match>().Select(
                m => new KeyValuePair<string, string>(m.Groups[1].Value, m.Groups[2].Value)
            ).ToList();
            link = null;
            matches.Add(new KeyValuePair<string, string>("HomeValue", homeValue));
            homeValue = null;

            int index = matches.FindIndex(m => m.Value.Contains(':') && m.Value.Length > 35);
            if (index >= 0)
            {
                string dictionary = matches[index].Value;
                matches.RemoveAt(index);
                matches.Concat(
                    dictionary.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList().FindAll(f => f.Contains(':'))
                    .Select(f => f.Split(':'))
                    .Select(f => new KeyValuePair<string, string>(f[0], f[1]))
                );
                dictionary = null;
            }

            return matches.GroupBy(
                m => rgxAlphaNumeric.Replace(m.Key, ""),
                m => m.Value
            ).ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(v => v.Length).ToArray()
            );

            
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

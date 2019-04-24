using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using CsvHelper;
using RestSharp;
using ZachLib;
using ZachLib.Logging;
using PostmanLib;
using PestPac.Model;
using PPLib;
using HtmlAgilityPack;

namespace PPWebLib
{
    public static class PestPac
    {
        #region Initialization
        static PestPac()
        {
            LogManager.AddLog("PPInternal", LogType.FolderFilesByDate);
        }

        internal static CookieContainer cookies = new CookieContainer();

        private static readonly Lazy<RestClient> _client = new Lazy<RestClient>(
            () => Initialize()
        );
        private static RestClient _insecure = null;
        private static readonly Lazy<Dictionary<string, string>> _countyIds = new Lazy<Dictionary<string, string>>(
            () => File.ReadAllLines(
                @"C:\DocUploads\Program Files\PPDictionaries\CountyIDs.txt"
            ).Select(
                l => l.Split(
                    new string[] { " :=: " },
                    StringSplitOptions.None
                )
            ).ToDictionary(
                l => l[0],
                l => l[1]
            )
        );
        private static readonly Lazy<Parameter[]> _employeeParameters = new Lazy<Parameter[]>(
            () => Utils.LoadKeyValues(@"E:\Temp\Employees\Parameters.txt", "\t").Select(p => new Parameter() { Name = p.Key, Value = p.Value, Type = ParameterType.GetOrPost }).ToArray()
        );

        private static RestClient CLIENT { get => _client.Value; }
        internal static RestClient INSECURE_CLIENT
        {
            get {
                RestClient tempClient = null;
                if (!_client.IsValueCreated)
                    tempClient = _client.Value;
                return _insecure;
            }
        }
        internal static Dictionary<string, string> CountyIDs { get => _countyIds.Value; }

        #region ClientFunctions
        private static RestRequest loginRequest = new RestRequest("/default.asp?Mode=Login", Method.POST);
        //private static readonly LicenseUsers RGX_LICENSES = new LicenseUsers();
        
        private static RestClient Initialize()
        {
            var client = new RestClient("https://app.pestpac.com")
            {
                CookieContainer = cookies,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36",
                FollowRedirects = false
            };
            _insecure = new RestClient("http://app.pestpac.com/")
            {
                CookieContainer = cookies,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36",
                FollowRedirects = false
            };

            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
            client.AddDefaultHeader("Origin", "https://app.pestpac.com");
            client.AddDefaultHeader("DNT", "1");
            client.RemoveDefaultParameter("Accept");
            _insecure.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            _insecure.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
            _insecure.AddDefaultHeader("Origin", "http://app.pestpac.com");
            _insecure.AddDefaultHeader("DNT", "1");
            _insecure.RemoveDefaultParameter("Accept");
            _insecure.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            cookies.Add(new Cookie("Username", "%7BMGIGCSsGAQQBgjdYA6BVMFMGCisGAQQBgjdYAwGgRTBDAgMCAAACAmYDAgIAwAQI%0D%0A3JkrG5QijIoEEDu9lKj23WAaa7uoCz3jKQcEGBTy5negPcgmb6zF5VBlzZHwP8bq%0D%0ALfy8kQ%3D%3D%0D%0A%7D", "/", "pestpac.com"));
            cookies.Add(new Cookie("Password", "%7BMGIGCSsGAQQBgjdYA6BVMFMGCisGAQQBgjdYAwGgRTBDAgMCAAACAmYDAgIAwAQI%0D%0AUwgI3RC27ZQEEPF0mqtnLLPoD8RD0GLzHAoEGPjbL9D2c%2FTRssuRwKO%2Fx6kiLuwv%0D%0AO80dEQ%3D%3D%0D%0A%7D", "/", "pestpac.com"));
            cookies.Add(new Cookie("CompanyKey", "%7BMFoGCSsGAQQBgjdYA6BNMEsGCisGAQQBgjdYAwGgPTA7AgMCAAACAmYDAgIAwAQI%0D%0Ak14VXzl3HXMEELPiij6SlNbzJVK7AAHvKDYEEMXKlmOzuOsN2Q866zPBRCY%3D%0D%0A%7D", "/", "pestpac.com"));

            loginRequest.AddParameter("CompanyKey", "323480", ParameterType.GetOrPost);
            loginRequest.AddParameter("Password", "I15Zac$0208", ParameterType.GetOrPost);
            loginRequest.AddParameter("RememberMe", "1", ParameterType.GetOrPost);
            loginRequest.AddParameter("RememberedAuth", "0", ParameterType.GetOrPost);
            loginRequest.AddParameter("SavePassword", "1", ParameterType.GetOrPost);
            loginRequest.AddParameter("Username", "zac.johnso", ParameterType.GetOrPost);

            //LicenseCleanupThread.Start();
            //Thread.Sleep(1250);
            Login(client);

            client.Proxy = null;
            _insecure.Proxy = null;
            return client;
        }

        private static void Login(RestClient client)
        {
            client.FollowRedirects = true;
            client.Get(new RestRequest("/"));
            client.Execute(loginRequest);
            client.FollowRedirects = false;
        }

        private static Thread LicenseCleanupThread = new Thread(() => LicenseCleanup());
        #endregion
        #endregion

        public static Stopwatch TIMER = new Stopwatch();

        public static ConcurrentBag<Tuple<long, long>> RequestBuildTimes = new ConcurrentBag<Tuple<long, long>>();
        public static ConcurrentBag<Tuple<long, long>> InitialGetTimes = new ConcurrentBag<Tuple<long, long>>();
        public static ConcurrentBag<Tuple<long, long>> DictExtractionTimes = new ConcurrentBag<Tuple<long, long>>();
        public static ConcurrentBag<Tuple<long, long>> CustomMergeTimes = new ConcurrentBag<Tuple<long, long>>();
        public static ConcurrentBag<Tuple<long, long>> FinalExecuteTimes = new ConcurrentBag<Tuple<long, long>>();

        private const string TIMES_FORMAT = "{0}:\r\n\tMinTicks: {1}\r\n\tMaxTicks: {2}\r\n\tAvgTicks: {3}\r\n\tTotalTicks: {4}\r\n\tMinMS: {5}\r\n\tMaxMS: {6}\r\n\tAvgMS: {7}\r\n\tTotalMS: {8}\r\n";
        public static string GetTimesString()
        {
            StringBuilder sb = new StringBuilder();
            double count = 0;
            long minMS = Int32.MaxValue;
            long maxMS = 0;
            long totalMS = 0;
            long minTicks = 0;
            long maxTicks = 0;
            long totalTicks = 0;
            
            void CountUpTimes(ConcurrentBag<Tuple<long, long>> timesCollection, string collectionName)
            {
                minMS = Int32.MaxValue;
                maxMS = 0;
                totalMS = 0;
                minTicks = Int32.MaxValue;
                maxTicks = 0;
                totalTicks = 0;

                count = timesCollection.Count;
                foreach (var times in timesCollection)
                {
                    if (times.Item1 > maxMS)
                        maxMS = times.Item1;
                    else if (times.Item1 < minMS)
                        minMS = times.Item1;
                    totalMS += times.Item1;

                    if (times.Item2 > maxTicks)
                        maxTicks = times.Item2;
                    else if (times.Item2 < minTicks)
                        minTicks = times.Item2;
                    totalTicks += times.Item2;
                }
                sb.AppendFormat(TIMES_FORMAT, collectionName, minTicks, maxTicks, totalTicks / count, totalTicks, minMS, maxMS, totalMS / count, totalMS);
                
                //timesCollection = null;
            }

            CountUpTimes(RequestBuildTimes, "Request Build Times");
            CountUpTimes(InitialGetTimes, "Initial Get Times");
            CountUpTimes(DictExtractionTimes, "Dict Extraction Times");
            CountUpTimes(CustomMergeTimes, "Custom Merge Times");
            CountUpTimes(FinalExecuteTimes, "Final Execute Times");
            return sb.ToString();
        }

        #region TaxCodes
        public static void UpdateStateTaxCodes(string path, string state)
        {
            CLIENT.RemoveDefaultParameter("X-Requested-With");
            Dictionary<string, string> stateAbbreviations = File.ReadAllLines(
                @"C:\DocUploads\Program Files\PPDictionaries\StateAbbreviations.txt"
            ).Select(
                l => l.Split(
                    new string[] { " - " },
                    StringSplitOptions.None
                )
            ).ToDictionary(
                l => l[0].ToUpper(),
                l => l[1]
            );
            Postman.GetTaxCodes(true, out IEnumerable<PostmanLib.TaxCode> existingCodes);

            StreamReader reader = new StreamReader(path);
            CsvReader csvReader = new CsvReader(reader, Utils.csvConfig);
            var taxcodes = csvReader.GetRecords<TaxRates>().OrderBy(c => c.City.Substring(0, 3)).ThenBy(c => c.City.Length).ToArray();
            csvReader.Dispose();
            csvReader = null;
            reader.Close();
            reader = null;

            string abbreviation = stateAbbreviations[state.ToUpper()];
            stateAbbreviations = null;
            List<KeyValuePair<string, TaxRates>> codes = new List<KeyValuePair<string, TaxRates>>();
            //client.FollowRedirects = true;
            foreach(var taxcode in taxcodes)
            {
                string city = taxcode.City.ToUpper().Trim();
                string code = abbreviation + taxcode.County.Replace(" ", "").Substring(0, 3).ToUpper() + city.Substring(0, 3);
                if (codes.Any(c => c.Key == code))
                {
                    if (city.Contains(" "))
                    {
                        city = city.Split(' ').Last();
                        code += city.First() + city.Last();
                    }
                    else
                        code += city.Substring(city.Length - 3);
                }
                else
                {
                    code += taxcode.City.Replace(" ", "").Substring(0, 3).ToUpper();
                    codes.Add(new KeyValuePair<string, TaxRates>(code, taxcode));
                }
                    
                if (!existingCodes.Any(c => c.Code == code))
                {
                    CreateTaxCode(
                        code,
                        abbreviation + " - " + taxcode.County.TitleCapitalization() + (Utils.COMPARE_INFO.IsSuffix(taxcode.County, "County") ? "" : " County") + " - " + taxcode.City.TitleCapitalization(),
                        taxcode.TotalRate,
                        taxcode.CityRate,
                        taxcode.CountyRate,
                        taxcode.StateRate,
                        taxcode.TransitRate
                    );
                }
            }

            Postman.GetTaxCodesAutoFill(out existingCodes, abbreviation);
            if (existingCodes != null)
            {
                existingCodes = existingCodes.Where(c => c.Active);
                codes = codes.FindAll(c => !existingCodes.Any(ec => ec.Code == c.Key));
            }
                
            //client.FollowRedirects = false;
            var codesTemp = codes.ToArray();
            codes = null;
            foreach(var code in codesTemp)
            {
                CreateTaxCodeAutoFill(
                    abbreviation, 
                    code.Value.County, 
                    code.Value.City, 
                    code.Key
                );
            }

            CLIENT.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
        }

        private const string XML = "<params><database></database><id>{0}</id><table>TaxCode</table></params>";
        public static bool CheckIfTaxCodeInUse(string id)
        {
            var request = new RestRequest("/xml/DeleteLookup.asp", Method.POST);
            request.AddParameter("application/x-www-form-urlencoded", String.Format(XML, id), ParameterType.RequestBody);
            return CLIENT.Execute(request).Content.Contains(">1<");
        }

        public static void DeleteTaxCode(string id)
        {
            CLIENT.Execute(
                new RestRequest(
                    "/lookup/taxCode/detail.asp",
                    Method.POST
                ).AddOrUpdateParameter(
                    "Mode",
                    "Delete",
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "TaxCodeID",
                    id,
                    ParameterType.GetOrPost
                )
            );
        }

        public static void CreateTaxCode(string code, string desc, double taxrate, double city, double county, double state, double transit)
        {
            /*client.Execute(
                new RestRequest(
                    "/lookup/taxCode/detail.asp?Mode=New"
                )
            );*/

            RestRequest request = new RestRequest("/lookup/taxCode/insert.asp", Method.POST);
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Referer", "https://app.pestpac.com/lookup/taxCode/detail.asp?Mode=New");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Accept-Encooding", "gzip, deflate, br");

            //request.AddParameter("Mode", "New", ParameterType.GetOrPost);
            request.AddParameter("OldTaxRate", "0.0", ParameterType.GetOrPost);
            request.AddParameter("TaxCodeID", "", ParameterType.GetOrPost);
            request.AddParameter("GST", "0.0", ParameterType.GetOrPost);
            request.AddParameter("PST", "0.0", ParameterType.GetOrPost);

            request.AddParameter("Code", code, ParameterType.GetOrPost);
            request.AddParameter("Description", desc, ParameterType.GetOrPost);
            request.AddParameter("TaxRate", taxrate.ToString("#.0000"), ParameterType.GetOrPost);
            request.AddParameter("ID_1", city.ToString("#.0000"), ParameterType.GetOrPost);
            request.AddParameter("ID_2", county.ToString("#.0000"), ParameterType.GetOrPost);
            request.AddParameter("ID_3", state.ToString("#.0000"), ParameterType.GetOrPost);
            request.AddParameter("ID_5", transit.ToString("#.0000"), ParameterType.GetOrPost);

            CLIENT.Execute(request);
        }

        public static void CreateTaxCodeAutoFill(string state, string county, string city, string code)
        {
            RestRequest request = new RestRequest("/lookup/taxCodeAutoFill/insert.asp", Method.POST);
            request.AddParameter("AutoFillID", "", ParameterType.GetOrPost);
            request.AddParameter("Zip", "", ParameterType.GetOrPost);

            request.AddParameter("State", state, ParameterType.GetOrPost);
            request.AddParameter("County", county, ParameterType.GetOrPost);
            request.AddParameter("City", city, ParameterType.GetOrPost);
            request.AddParameter("TaxCode", code, ParameterType.GetOrPost);

            Postman.GetTaxCodes(true, out IEnumerable<PostmanLib.TaxCode> taxcodes);
            request.AddParameter("TaxCodeID", taxcodes.Single(t => t.Code == code).TaxCodeID, ParameterType.GetOrPost);
            taxcodes = null;

            if (CountyIDs.TryGetValue(county.ToUpper(), out string countyid))
            {
                request.AddParameter("CountyID", countyid, ParameterType.GetOrPost);
                CLIENT.Execute(request);
            }
            else
                Console.WriteLine("CountyID not found ~ County: " + county + ", City: " + city);
            
        }
        #endregion

        #region ServiceSetups
        #region HelperProperties
        private static XmlSerializer SERVICE_LOOKUP_DESERIALIZER = new XmlSerializer(typeof(ServiceCodeLookup), new XmlRootAttribute("service"));

        private static readonly string XML_GENERATE_FORMAT = "\r\n\t<params>" +
            "\r\n\t<skipmonths>000000000000</skipmonths>" +
            "\r\n\t<lastgenerate>{0}</lastgenerate>" +
            "\r\n\t<scheduleid>{1}</scheduleid>" +
            "\r\n\t<date>" + DateTime.Now.ToString("MM/dd/yyyy") + "</date>" +
            "\r\n\t<database></database>" +
        "\r\n\t</params>\r\n\t";
        private static string GetNextGenerate(string lastGenerate, string scheduleID)
        {
            return ZachRGX.XML_SINGLE_VALUE.Single(
                INSECURE_CLIENT.Execute(
                    new RestRequest(
                        "/xml/getNextGenerate.asp",
                        Method.POST
                    ).AddOrUpdateParameter(
                        "Accept", "application/xml, text/xml, */*; q=0.01", ParameterType.HttpHeader
                    ).AddOrUpdateParameter(
                        "application/x-www-form-urlencoded; charset=UTF-8",
                        String.Format(
                            XML_GENERATE_FORMAT,
                            lastGenerate,
                            scheduleID
                        ), ParameterType.RequestBody
                    )
                ).Content
            ).PadLeft(10, '0');
        }

        private const string XML_INITIAL_FORMAT =
            "\r\n<params>" +
                "\r\n<serviceid>{0}</serviceid>" +
                "\r\n<database></database>" +
                "\r\n<companyid>1</companyid>" +
                "\r\n<branchid>{1}</branchid>" +
                "\r\n<locationstate>{2}</locationstate>" +
            "\r\n</params>\r\n";
        private static readonly string[] UPPER_VARIANTS = new string[]
        {
            "InitialServiceID",
            "InitialServiceCode",
            "InitialServicePrice",
            "InitialDuration",
            //"InitialGLCode",
            //"InitialGLCodeID",
            //"Taxable"
        };

        internal static readonly Dictionary<string, string> Defaults = new Dictionary<string, string>()
        {
            { "ACHAutoBill", "1" },
            //{ "Active", "1" },
            { "ApplicationEquipmentCode1", "" },
            { "ApplicationEquipmentID1", ""},
            { "ApplicationRateCode1", "" },
            { "ApplicationRateID1", "" },
            //{ "AutoBill", "1" },
            { "AutoBillType", "CC" },
            { "BASquareFootage1", "" },
            { "ChemicalCode1", "" },
            { "DilutionFactor2_1", "" },
            { "IncreasePrice", "1" },
            //{ "InitialTaxable", "1" },
            { "LinearFootage1", "" },
            { "MethodOfAppl1", "" },
            { "Qty1", "" },
            { "RouteOptIncludeDay1", "1" },
            { "RouteOptIncludeDay2", "1" },
            { "RouteOptIncludeDay3", "1" },
            { "RouteOptIncludeDay4", "1" },
            { "RouteOptIncludeDay5", "1" },
            { "RouteOptIncludeDay6", "1" },
            { "RouteOptIncludeDay7", "1" },
            { "SetupType", "SO" },
            { "STCubicFootage1", "" },
            { "Target1", "" },
            { "Taxable1", "1" },
            { "UndilutedQty1", "" },
            { "UndilutedUOM1", "" },
            { "UnitOfMeasure1", "" },
        };

        internal static readonly Dictionary<string, string> OverwriteNulls = new Dictionary<string, string>()
        {
            { "AnytimeEndAmPm", "PM" },
            { "AnytimeStartAmPm", "AM" },
            { "DatabaseForNotifications", "Provider=SQLOLEDB.1;Password=XGkLXvRzP!3PP647*5UBQMjYw#c3tBgCS;Persist Security Info=True;User ID=sa;Initial Catalog=PestPac6542;Data Source=PP-AWS-E-SQL19" } ,
            //{ "InitialServiceAmPm", "AM" },
            { "InitialDuration", "00:00" },
            { "InitialServicePrice", "0.00" },
            { "PriceByMeasurementDelete1", "1" },
            { "RouteOptTime1BegAmPm", "AM" },
            { "RouteOptTime1EndAmPm", "PM" },
            { "RouteOptTime2BegAmPm", "AM" },
            { "RouteOptTime2EndAmPm", "PM" },
            { "WorkAmPm", "AM" }
        };
        #endregion

        #region HelperFunctions
        public static Dictionary<string, string> ServiceCodeChanges(string code)
        {
            /*var xml = INSECURE_CLIENT.Execute(
                new RestRequest(
                    "http://app.pestpac.com/autocomplete/getLookupData.asp",
                    Method.POST
                ).AddOrUpdateParameters(
                    new Parameter()
                    {
                        Name = "lookup",
                        Value = "Service",
                        Type = ParameterType.QueryString
                    },
                    new Parameter()
                    {
                        Name = "xml",
                        Value = "1",
                        Type = ParameterType.QueryString
                    },
                    new Parameter()
                    {
                        Name = "xmlCode",
                        Value = code,
                        Type = ParameterType.QueryString
                    }
                )
            ).Content;*/
            XmlReader reader = XmlReader.Create("http://app.pestpac.com/autocomplete/getLookupData.asp?lookup=Service&xml=1&xmlCode=" + HttpUtility.UrlEncode(code));
            reader.Read();
            var lookup = SERVICE_LOOKUP_DESERIALIZER.Deserialize(reader);
            return lookup.ToPropertiesDictionary<ServiceCodeLookup>();
        }

        public static Dictionary<string, string> GetDefaults(string filePath)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(filePath));
            var node = doc.DocumentNode.SelectSingleNode("/html/body");
            doc = null;
            //var xmlNodes = node.SelectNodes("//xml");

            node = node.SelectSingleNode("form[@name='frmDetail']");

            var dict = node.SelectNodes(
                ".//*[(self::input or self::span) and @disabled and @name and @value and (@type='text' or @type='hidden' or not(@type) or ((@type='Checkbox' or @type='checkbox' or @type='Radio' or @type='radio') and @checked))]"
            ).ToDictionary(
                n => n.Attributes["name"].Value,
                n => n.Attributes["value"].Value
            );
            dict.TryAddAll(
                node.SelectNodes(".//select[@disabled and ./*[@selected]]").Select(
                    n => new KeyValuePair<string, string>(
                        n.GetAttributeValue("name", ""),
                        n.SelectNodes("option[@selected and @value]").Last().GetAttributeValue("value", "")
                    )
                )
            );

            return dict;
        }

        private static Dictionary<string, string> GetInitialService(string serviceID, string branchID, string state)
        {
            var dict = ZachRGX.XML_KEY_VALUE.ToObjects<KeyValuePair<string, string>>(
                INSECURE_CLIENT.Execute(
                    new RestRequest(
                        "/xml/getInitialService.asp",
                        Method.POST
                    ).AddOrUpdateParameter(
                        "Accept", "application/xml, text/xml, */*; q=0.01", ParameterType.HttpHeader
                    ).AddOrUpdateParameter(
                        "application/x-www-form-urlencoded; charset=UTF-8",
                        String.Format(
                            XML_INITIAL_FORMAT,
                            serviceID,
                            branchID,
                            state
                        ), ParameterType.RequestBody
                    )
                ).Content
            ).ToDictionary();

            dict = UPPER_VARIANTS.ToDictionary(
                k => k,
                k => dict.TryGetValue(k.ToLower(), out string value) ? value : ""
            );

            dict.Add("OInitialServiceCode", dict["InitialServiceCode"]);

            return dict;
        }

        private static Dictionary<string, string> PPInternalModel(string text)
        {
            TIMER.Start();
            Dictionary<string, string> dict = null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var node = doc.DocumentNode.SelectSingleNode("/html/body");
            doc = null;
            //var xmlNodes = node.SelectNodes("//xml");

            node = node.SelectSingleNode("form[@name='frmDetail']");
            dict = node.SelectNodes(
                ".//*[(self::input or self::span) and @name and @value and (@type='text' or @type='hidden' or not(@type) or ((@type='Checkbox' or @type='checkbox' or @type='Radio' or @type='radio') and @checked))]"
            ).ToDictionary(
                n => n.Attributes["name"].Value,
                n => n.Attributes["value"].Value
            );
            dict.TryAddAll(
                node.SelectNodes(".//textarea").Select(
                    n => new KeyValuePair<string, string>(
                        n.GetAttributeValue("name", ""),
                        n.InnerText
                    )
                ).Concat(
                    node.SelectNodes(".//select[./*[@selected]]").Select(
                        n => new KeyValuePair<string, string>(
                            n.GetAttributeValue("name", ""),
                            n.SelectNodes("option[@selected and @value]").Last().GetAttributeValue("value", "")
                        )
                    )
                )
            );
            TIMER.Stop();
            node = null;
            dict.Append(
                "NextGeneratedDate", 
                GetNextGenerate(
                    dict["LastGeneratedDate"],
                    dict["ScheduleID"]
                )
            );
            //dict.TryAddAll(Defaults);

            /*dict.Merge(
                GetInitialService(
                    dict["ServiceID1"], 
                    dict["SetupBranchID"], 
                    RGX.PPI_LOCATION_STATE.Single(text)
                )
            );*/

            /*foreach(var kv in OverwriteNulls)
            {
                if (!dict.TryGetValue(kv.Key, out string value) || String.IsNullOrWhiteSpace(value) || (int.TryParse(value, out int num) && num == 0))
                    dict.Append(kv);
            }*/

            return dict;
        }
        #endregion

        public static Dictionary<string, string> PPInternalModelTest()
        {
            string str = CLIENT.Execute(
                new RestRequest(
                    "serviceSetup/detail.asp",
                    Method.GET
                ).AddOrUpdateParameters(
                    new Parameter()
                    {
                        Name = "Mode",
                        Value = "Edit",
                        Type = ParameterType.GetOrPost
                    }, new Parameter()
                    {
                        Name = "SetupID",
                        Value = "170652",
                        Type = ParameterType.GetOrPost
                    }
                )
            ).Content;
            UpdateReferer("/serviceSetup/detail.asp?Mode=Edit&SetupID=170652");
            return PPInternalModel(str);
        }

        public static IDictionary<string, string> EditServiceSetup(string id, Dictionary<string, string> fieldChanges, string[] fieldRemoval = null)
        {
            bool doFieldChanges = !(fieldChanges == null || fieldChanges.Count == 0);
#if DEBUG
            var timer = Stopwatch.StartNew();
#endif
            var request = new RestRequest(
                "serviceSetup/detail.asp",
                Method.GET
            ).AddOrUpdateParameters(
                new Parameter()
                {
                    Name = "Mode",
                    Value = "Edit",
                    Type = ParameterType.GetOrPost
                }, new Parameter()
                {
                    Name = "SetupID",
                    Value = id,
                    Type = ParameterType.GetOrPost
                }
            );
#if DEBUG
            timer.Stop();
            RequestBuildTimes.Add(new Tuple<long, long>(timer.ElapsedMilliseconds, timer.ElapsedTicks));
            timer.Restart();
#endif

            CLIENT.AllowMultipleDefaultParametersWithSameName = false;
            string str = CLIENT.Execute(request).Content;
            if (str.StartsWith(" <font face"))
                return null;
            UpdateReferer("/serviceSetup/detail.asp?Mode=Edit&SetupID=" + id);

#if DEBUG
            timer.Stop();
            InitialGetTimes.Add(new Tuple<long, long>(timer.ElapsedMilliseconds, timer.ElapsedTicks));

            timer.Restart();
#endif
            var dict = PPInternalModel(str);
            str = null;
            
            if (doFieldChanges)
            {
                string value = null;
                foreach (var kv in ZILLOW_KEYS)
                {
                    if (fieldChanges.Extract(kv.Key, out value))
                        fieldChanges.Add(kv.Value, value);
                }
            }

#if DEBUG
            timer.Stop();
            DictExtractionTimes.Add(new Tuple<long, long>(timer.ElapsedMilliseconds, timer.ElapsedTicks));

            timer.Restart();
#endif

            if (doFieldChanges)
               CustomMerge(dict, fieldChanges);
            dict.Append("UserID", "4084");
            dict.Append("CompanyID", "1");
            dict.Append("Mode", "Save");
            if (fieldRemoval != null)
            {
                foreach(var field in fieldRemoval)
                {
                    dict.Remove(field);
                }
            }

#if DEBUG
            timer.Stop();
            CustomMergeTimes.Add(new Tuple<long, long>(timer.ElapsedMilliseconds, timer.ElapsedTicks));

            timer.Restart();
#endif

            CLIENT.Execute(
                new RestRequest(
                    "/serviceSetup/detail.asp",
                    Method.POST
                ).AddOrUpdateParameters(dict)
            );
            UpdateReferer();

#if DEBUG
            timer.Stop();
            FinalExecuteTimes.Add(new Tuple<long, long>(timer.ElapsedMilliseconds, timer.ElapsedTicks));
#endif

            return dict;
        }
#endregion

#region ServiceOrders
#region HelperProperties
        private static readonly Lazy<Dictionary<string, string>> _orderConstants = new Lazy<Dictionary<string, string>>(
            () => Utils.LoadDictionary(@"E:\Temp\ServiceOrders\Constants.txt", " - ")
        );
        private static readonly Lazy<Dictionary<string, string>> _orderDefaults = new Lazy<Dictionary<string, string>>(
            () => Utils.LoadDictionary(@"E:\Temp\ServiceOrders\Defaults.txt", " - ")
        );
        public static Dictionary<string, string> PPOrderConstants { get => _orderConstants.Value; }
        public static Dictionary<string, string> PPOrderDefaults { get => _orderDefaults.Value; }
#endregion

        public static Dictionary<string, string> PPOrderInternalModel(string text)
        {
            TIMER.Start();
            Dictionary<string, string> dict = null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var node = doc.DocumentNode.SelectSingleNode("/html/body");
            doc = null;
            //var xmlNodes = node.SelectNodes("//xml");

            node = node.SelectSingleNode("div[@id='content-outer'] | form[@id='frmOrder']");
            dict = node.SelectNodes(
                ".//*[(self::input or self::span) and @name and @value and (@type='text' or @type='hidden' or not(@type) or ((@type='Checkbox' or @type='checkbox' or @type='Radio' or @type='radio') and @checked))]"
            ).ToDictionary(
                n => n.Attributes["name"].Value,
                n => n.Attributes["value"].Value
            );

            var otherTypes = node.SelectNodes(".//textarea").Select(
                n => new KeyValuePair<string, string>(
                    n.GetAttributeValue("name", ""),
                    n.InnerText
                )
            ).Concat(
                node.SelectNodes(".//select[./*[@selected]]").Select(
                    n => new KeyValuePair<string, string>(
                        n.GetAttributeValue("name", ""),
                        n.SelectNodes("option[@selected and @value]").Last().GetAttributeValue("value", "")
                    )
                )
            );
            foreach(var input in otherTypes)
            {
                dict.Add(input.Key, input.Value);
            }
            TIMER.Stop();
            node = null;
            
            return dict;
        }

        public static Dictionary<string, string> PPOrderInternalModelTest(string text)
        {
            Dictionary<string, string> dict = null;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var node = doc.DocumentNode.SelectSingleNode("/html/body");
            doc = null;
            //var xmlNodes = node.SelectNodes("//xml");

            node = node.SelectSingleNode("div[@id='content-outer'] | form[@id='frmOrder']");
            dict = node.SelectNodes(
                ".//*[(self::input or self::span) and @disabled and @name and @value and (@type='text' or @type='hidden' or not(@type) or ((@type='Checkbox' or @type='checkbox' or @type='Radio' or @type='radio') and @checked))]"
            ).ToDictionary(
                n => n.Attributes["name"].Value,
                n => n.Attributes["value"].Value
            );

            var otherTypes = node.SelectNodes(".//select[@disabled and ./*[@selected]]").Select(
                n => new KeyValuePair<string, string>(
                    n.GetAttributeValue("name", ""),
                    n.SelectNodes("option[@selected and @value]").Last().GetAttributeValue("value", "")
                )
            );
            foreach (var input in otherTypes)
            {
                dict.Add(input.Key, input.Value);
            }

            return dict;
        }

        public static bool EditServiceOrder(string id, Dictionary<string, string> fieldChanges, out IRestResponse response)
        {
            string str = CLIENT.Execute(
                new RestRequest(
                    "serviceOrder/detail.asp",
                    Method.GET
                ).AddOrUpdateParameters(
                    new Parameter()
                    {
                        Name = "Mode",
                        Value = "Edit",
                        Type = ParameterType.GetOrPost
                    }, new Parameter()
                    {
                        Name = "OrderID",
                        Value = id,
                        Type = ParameterType.GetOrPost
                    }
                )
            ).Content;
            if (str.StartsWith(" <font face"))
            {
                response = null;
                return true;
            }
            //UpdateReferer("/serviceOrder/detail.asp?Mode=Edit&OrderID=" + id);
            IDictionary<string, string> dict = new SortedDictionary<string, string>(PPOrderInternalModel(str));
            str = null;
            dict = CustomMerge(dict, fieldChanges);
            dict.Append("UserID", "4084");
            dict.Append("CompanyID", "1");
            dict.Append("Mode", "Save");

            response = CLIENT.Execute(
                new RestRequest(
                    "/serviceOrder/detail.asp",
                    Method.POST
                ).AddOrUpdateParameters(dict)
            );
            //UpdateReferer();
            return response.StatusCode == HttpStatusCode.Found;
        }
#endregion

        private const string XML_QUICK_BILLTO_FORMAT = "\r\n<params>" +
            "\r\n<database></database>" +
            "\r\n<companyid>1</companyid>" +
            "\r\n<userid>4084</userid>" +
            "\r\n<billtocode>{0}</billtocode>" +
        "\r\n</params>\r\n";

        private static readonly XmlSerializer XML_QUICK_BILLTO_DESERIALIZER = new XmlSerializer(typeof(QuickBillTo), new XmlRootAttribute("response"));

        private static QuickBillTo GetQuickBillTo(string billToCode)
        {
            var request = new RestRequest("/xml/getQuickBillTo.asp", Method.POST);
            request.AddHeader("Accept", "application/xml, text/xml, */*; q=0.01");
            request.AddParameter(
                "application/x-www-form-urlencoded; charset=UTF-8",
                String.Format(
                    XML_QUICK_BILLTO_FORMAT,
                    billToCode
                ),
                ParameterType.RequestBody
            );
            var response = INSECURE_CLIENT.Execute(request);
            LogManager.Enqueue("PPInternal", EntryType.DEBUG, "Retrived QuickBillTo", billToCode);
            return (QuickBillTo)XML_QUICK_BILLTO_DESERIALIZER.Deserialize(
                new StringReader(response.Content)
            );
        }

        private static readonly IEnumerable<KeyValuePair<string, object>> DEFAULT_ADDINVOICE_PARAMS = new Dictionary<string, object>()
        {
            { "DialogHelperConsolidatedInvoiceNum", "" },
            { "DialogHelperPaymentAmountReadOnly", false },
            { "DialogHelperAdjustment", "|" },
            { "DialogHelperAdjustmentReason", "|" }
        };

        private static void AddInvoiceToPayment(object billToID, object invoiceID, object amount)
        {
            var request = new RestRequest("payment/dialog/InvoiceQuickPaymentEntry.asp", Method.POST);
            request.AddParameters(
                new Dictionary<string, object>()
                {
                    { "DialogHelperBillToID", billToID },
                    { "DialogHelperInvoiceID", invoiceID },
                    { "DialogHelperPaymentAmount", amount },
                    { "DialogHelperInvoicePayment", amount.ToString() + "|" }
                }.Concat(DEFAULT_ADDINVOICE_PARAMS)
            );
            INSECURE_CLIENT.Execute(request);
            LogManager.Enqueue("PPInternal", EntryType.DEBUG, "Added invoice " + invoiceID, billToID + " charged $" + amount);
        }

        public static void ReleasePaymentBatch(string batchID) =>
            INSECURE_CLIENT.Execute(
                new RestRequest(
                    "payment/reviewBatch.asp?Mode=Close&BatchID=" + batchID,
                    Method.GET
                )
            );

        public static List<KeyValuePair<string, string>> GetParameters(string resource, string entityIDName, string entityIDValue)
        {
            RestRequest request = new RestRequest(resource, Method.GET);
            request.AddParameter("Mode", "Edit", ParameterType.GetOrPost);
            request.AddParameter(entityIDName, entityIDValue, ParameterType.GetOrPost);
            var response = INSECURE_CLIENT.Execute(request);
            string text = response.Content;

            List<KeyValuePair<string, string>> dict = new List<KeyValuePair<string, string>>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            text = null;
            var node = doc.DocumentNode.SelectSingleNode("/html/body/form[@name='" + (resource.StartsWith("invoice") ? "frmInvoice" : "frmDetail") + "']");
            doc = null;
            //var xmlNodes = node.SelectNodes("//xml");

            dict.AddRange(
                node.SelectNodes(
                    ".//*[(self::input or self::span) and @name and @value and (@type='text' or @type='hidden' or not(@type) or ((@type='Checkbox' or @type='checkbox' or @type='Radio' or @type='radio') and @checked))]"
                ).Select(
                    n => new KeyValuePair<string, string>(
                        n.Attributes["name"].Value,
                        n.Attributes["value"].Value
                    )
                )
            );
            dict.AddRange(
                node.SelectNodes(".//textarea").Select(
                    n => new KeyValuePair<string, string>(
                        n.GetAttributeValue("name", ""),
                        n.InnerText
                    )
                ).Concat(
                    node.SelectNodes(".//select[./*[@selected]]").Select(
                        n => new KeyValuePair<string, string>(
                            n.GetAttributeValue("name", ""),
                            n.SelectNodes("option[@selected and @value]").Last().GetAttributeValue("value", "")
                        )
                    )
                )
            );
            
            node = null;
            return dict;
        }
        
        public static bool EditEntity(string resource, Dictionary<string, string> priorParams, Dictionary<string, string> fieldChanges, out IRestResponse response)
        {
            var finalParams = CustomMerge(new SortedDictionary<string, string>(priorParams), new SortedDictionary<string, string>(fieldChanges));
            finalParams.Append("Mode", "SaveNewWindow");
            response = INSECURE_CLIENT.Execute(new RestRequest(resource, Method.POST).AddOrUpdateParameters(finalParams));
            return response.StatusCode == HttpStatusCode.Found;
        }

        public static bool CreateTech(string firstName, string lastName, string userName, out IRestResponse response)
        {
            var request = new RestRequest("lookup/employee/insert.asp", Method.POST);
            request.AddParameters(_employeeParameters.Value);
            request.AddParameter("Username", userName, ParameterType.GetOrPost);
            request.AddParameter("FName", firstName, ParameterType.GetOrPost);
            request.AddParameter("LName", lastName, ParameterType.GetOrPost);
            request.AddParameter("WWMUserSettings", String.Format("{{\"emailAddress\":\"\",\"firstName\":\"{0}\",\"lastName\":\"{1}\",\"managerId\":null,\"mobilePhone\":\"\",\"marketingAreas\":{{}},\"performerRoles\":{{}},\"permissions\":[],\"salesRegions\":{{}}}}", firstName, lastName), ParameterType.GetOrPost);

            response = INSECURE_CLIENT.Execute(request);
            return response.StatusCode == HttpStatusCode.Found;
        }

        public static bool DeactivateEmployee(string id, out IRestResponse response)
        {
            var parameters = GetParameters("lookup/employee/detail.asp", "EmployeeID", id);
            parameters.Remove(new KeyValuePair<string, string>("Mode", "Save"));
            parameters.Add(new KeyValuePair<string, string>("Mode", "Deactivate"));
            var request = new RestRequest(
                "lookup/employee/detail.asp",
                Method.POST
            );
            request.AddParameters(parameters);
            parameters = null;
            response = INSECURE_CLIENT.Execute(request);
            return response.StatusCode == HttpStatusCode.Found;
        }

        public static void CreateList()
        {

        }

        private static readonly Dictionary<string, string> ZILLOW_KEYS = new Dictionary<string, string> {
            { "Square Ft", "UserDef14" },
            { "Lot SqFt", "UserDef15" },
            { "Year Built", "UserDef16" },
            { "Home Value", "UserDef17" },
            { "Bathrooms", "UserDef18" },
            { "Bedrooms", "UserDef19" },
            { "Median Inc", "UserDef20" },
            { "ZillowPID", "UserDef21" },
            { "TaxAssess", "UserDef22" },
            { "LastSoldOn", "UserDef23" },
            { "LastPrice", "UserDef56" },
            { "Home Type", "UserDef57" }
        };

        

        public static IDictionary<string, string> GetTestDictionary()
        {
            return ZILLOW_KEYS.Inverse().Concat(Defaults).Concat(OverwriteNulls).Concat(
                new Dictionary<string, string>()
                {
                    { "Active", "0" },
                    { "AutoBill", "0" },
                    { "CallNotify1", "1" },
                    { "EmailNotify1", "1" },
                    { "PrintNotify1", "1" },
                    { "TextNotify1", "1"  },
                    { "WhoToNotify1", "1" },
                    { "AutoBillType", "1" },
                    { "RouteOptIncludeDays", "1" },
                    { "Description1", "1" }
                }
            ).ToDictionary();
        }

        public static IDictionary<string, string> CustomMerge(IDictionary<string, string> mainDict, IDictionary<string, string> mergeDict)
        {
            foreach(var kv in mergeDict)
            {
                if (mainDict.TryGetValue(kv.Key, out string oldValue))
                {
                    string orig = "Orig" + kv.Key;
                    string old = "Old" + kv.Key;
                    string original = "Original" + kv.Key;
                    string O = "O" + kv.Key;
                    if (mainDict.ContainsKey(orig))
                        mainDict[orig] = oldValue;
                    else if (mainDict.ContainsKey(old))
                        mainDict[old] = oldValue;
                    else if (mainDict.ContainsKey(original))
                        mainDict[original] = oldValue;
                    else if (mainDict.ContainsKey(O))
                        mainDict[O] = oldValue;
                    mainDict[kv.Key] = kv.Value;
                }
                else
                    mainDict.Add(kv.Key, kv.Value);
            }
            return mainDict;
        }

        private static void UpdateReferer()
        {
            CLIENT.RemoveDefaultParameter("Referer");
            INSECURE_CLIENT.RemoveDefaultParameter("Referer");
        }

        private static void UpdateReferer(string resource)
        {
            UpdateReferer();
            CLIENT.AddDefaultHeader("Referer", CLIENT.BaseUrl + resource);
            INSECURE_CLIENT.AddDefaultHeader("Referer", INSECURE_CLIENT.BaseUrl + resource);
        }

        private const int SLEEP_TIME = 1000 * 60 * 15;
        private static void LicenseCleanup()
        {
            while(true)
            {
                var users = PPRGX.PPI_LICENSE_USERS.ToObjects<LicenseUser>(
                    CLIENT.Get(
                        new RestRequest("license.asp")
                    ).Content
                ).ToArray();

                DateTime now = DateTime.Now;
                if (users.Any(u => u.LogOut(now)))
                {
                    int count = users.Length;
                    RestRequest request = new RestRequest("/license.asp", Method.POST);
                    request.AddParameter("Mode", "Logout", ParameterType.GetOrPost);
                    request.AddParameter("Users", count, ParameterType.GetOrPost);
                    request.AddParameter("OverrideKey", "", ParameterType.GetOrPost);

                    for (int i = 1; i <= count; ++i)
                    {
                        LicenseUser user = users[i - 1];
                        string numString = i.ToString();

                        request.AddParameter(
                            "SessionUserID" + numString, 
                            user.SessionUserID, 
                            ParameterType.GetOrPost
                        );

                        request.AddParameter(
                            "SessionID" + numString, 
                            user.SessionID, 
                            ParameterType.GetOrPost
                        );

                        if (user.LogOut(now))
                            request.AddParameter("LogOutUser" + numString, 1, ParameterType.GetOrPost);
                    }
                }

                Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}

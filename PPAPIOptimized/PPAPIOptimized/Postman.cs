using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Extensions;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;

using PestPac.Model;
using Jil;
using PPLib;
using PDF = RGX.PDF;
using VantageTracker;

namespace PPAPIOptimized
{
    public enum NoteCode
    {
        GEN,
        FEEDBACK
    };

    public class Postman : IPPAPI
    {
        public delegate void ExecutionHandler(object sender, ExecutionEventArgs e);
        public event ExecutionHandler OnExecute;

        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime expireTime { get; set; }
        public const string compkey = "323480";
        public Dictionary<string, string> abbrvs = new Dictionary<string, string>();
        public Dictionary<string, string> abbrvs2 = new Dictionary<string, string>();

        public VantageTracker.Client universalClient { get; set; }
        public RestClient client { get; set; }
        public RestClient tokenClient { get; set; }
        public USPSClient upsClient { get; set; }

        private static readonly Dictionary<string, string[]> schedules = new Dictionary<string, string[]>()
        {
            {"Q1 JAJO", new string[4] {"Jan", "Apr", "Jul", "Oct"} },
            {"Q2 FMAN", new string[4] {"Feb", "May", "Aug", "Nov"} },
            {"Q3 MJSD", new string[4] {"Mar", "Jun", "Sep", "Dec"} },
            {"MOSQUITOS", new string[4] {"Jun", "Jul", "Aug", "Sep"} }
        };
        
        public static readonly PDF.Filename RGX_FILENAME = new RGX.PDF.Filename();
        public static readonly PDF.INPC.Info RGX_INPC_INFO = new RGX.PDF.INPC.Info();
        public static readonly PDF.INPC.VTNumbers RGX_INPC_VT = new RGX.PDF.INPC.VTNumbers();
        public static readonly PDF.SA.Info RGX_SA_INFO = new RGX.PDF.SA.Info();
        public static readonly PDF.SA.NewInfo RGX_SA_NEWINFO = new RGX.PDF.SA.NewInfo();
        public static readonly PDF.SA.Schedule RGX_SA_SCHEDULE = new RGX.PDF.SA.Schedule();
        
        public static readonly CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public static readonly CompareOptions options = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;
        public static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor };
        public static readonly SortedDictionary<string, string> locationCodes = new SortedDictionary<string, string>(
            PPRGX.FILE_DICT.Matches(
                File.ReadAllText(@"C:\DocUploads\Program Files\PPDictionaries\LocationCodes.txt")
            ).Cast<Match>().ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value)
        );

        public const string LOGS_PATH = @"C:\DocUploads\Logs\Postman\";

        private LogManager Logger { get; set; }

        //public Dictionary<string, List<long>> responseTimes { get; set; }
        //public List<long> overallTimes { get; set; }
        //public Stopwatch[] timers { get; set; }

        public Postman()
        {
            this.upsClient = new USPSClient();
            this.refreshToken = Environment.GetEnvironmentVariable("REFRESH_TOKEN", EnvironmentVariableTarget.Machine);
            this.expireTime = DateTime.Parse(Environment.GetEnvironmentVariable("TOKEN_EXPIRE_TIME", EnvironmentVariableTarget.Machine));

            List<string> abbrvsTemp = new List<string>(File.ReadAllLines(Form1.ProgramFilesPath + "Abbreviations.txt"));
            foreach (string abbrv in abbrvsTemp)
            {
                string[] kv = abbrv.Split(',');
                try
                {
                    this.abbrvs.Add(kv[0], kv[1]);
                    this.abbrvs2.Add(kv[1], kv[0]);
                }
                catch
                {
                    try
                    {
                        if (kv[1].Length < this.abbrvs2[kv[1]].Length)
                            this.abbrvs2[kv[1]] = kv[0];
                        continue;
                    }
                    catch
                    {
                        this.abbrvs2[kv[1]] = kv[0];
                        continue;
                    }
                }
            }

            this.tokenClient = new RestClient("https://is.workwave.com/oauth2/token?scope=openid");
            this.tokenClient.AddDefaultHeader("content-type", "application/x-www-form-urlencoded");
            this.tokenClient.AddDefaultHeader("authorization", "Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh");

            this.client = new RestClient("https://api.workwave.com/pestpac/v1/");
            this.client.CookieContainer = new CookieContainer();
            this.client.AddDefaultHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            this.client.AddDefaultHeader("tenant-id", compkey);
            this.client.AddDefaultHeader("authorization", "Bearer ");
            //this.client.AddDefaultHeader("cache-control", "max-age=7200");

            this.GetToken();
            ServicePointManager.UseNagleAlgorithm = false;
        }

        public Postman(LogManager logger) : this()
        {
            this.Logger = logger;
            logger.AddLog(
                LOGS_PATH
            );
        }

        public Dictionary<string, string> ExtractPDF(ref string filePath)
        {
            //this.timers[0].Start();

            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"Type", ""},
                //{"FirstName", ""},
                {"LastName", ""},
                {"Address", ""},
                {"City", ""},
                {"State", ""},
                {"Zip", ""},
                {"VTID", ""}
            };

            PdfReader r = new PdfReader(filePath);
            FileInfo fileinfo = new FileInfo(filePath);
            string fileName = fileinfo.Name;
            string tempZip = "";
            string newFileName = "";
            if (PPRGX.HAS_FILE_INFO.IsMatch(fileName))
            // If a program was already used to name the PDFs based on their fields.
            {
                dict.Merge(RGX_FILENAME.ToKeyValues(fileName));
                if (dict["Type"] == "INPC" && !dict.ContainsKey("InvoiceID"))
                {
                    dict.Merge(RGX_INPC_VT.ToKeyValues(PdfTextExtractor.GetTextFromPage(r, 1, new SimpleTextExtractionStrategy())));
                    newFileName = InfoToFileName(dict);
                }
            }
            else if (fileName.Contains("INPC") || fileName.Contains("invoice"))
            {
                string text = PdfTextExtractor.GetTextFromPage(r, 1, new SimpleTextExtractionStrategy());
                dict.Merge(RGX_INPC_INFO.ToKeyValues(text));
                dict.Merge(RGX_INPC_VT.ToKeyValues(text));
                dict["Type"] = "INPC";
                text = null;
            }
            else if (r.NumberOfPages <= 3 && fileinfo.Length <= 250000)
            {
                //text = PdfTextExtractor.GetTextFromPage(r, 2, new SimpleTextExtractionStrategy());
                dict.Merge(
                    RGX_SA_INFO.ToKeyValues(
                        PdfTextExtractor.GetTextFromPage(
                            r, 2, new SimpleTextExtractionStrategy()
                        )
                    )
                );

                /*var months = RGX_SA_SCHEDULE.ToDictionaries(text).GroupBy(
                    m => new KeyValuePair<string, string>(m["Price"], m["Tax"]),
                    m => m["Month"]
                ).ToArray();*/

                /*string[] months = null;
                var tempMonths = Regex.Matches(
                    text, SchedulePat, RegexOptions.Multiline
                ).Cast<Match>().Select(
                    m => new
                    {
                        Month = m.Groups[1].Value.Substring(0, 3),
                        Price = m.Groups[2].Value
                    }
                ).ToList();

                if (tempMonths.Count > 12 && tempMonths.Count % 12 == 0)
                    tempMonths = tempMonths.GetRange(0, 12);

                //tempMonths.RemoveAll(m => m.Price[1] == '0');
                months = tempMonths.FindAll(m => m.Price[1] != '0').Select(m => m.Month).ToArray();

                dict["Schedule"] = TryGetSchedule(months);

                months = null;*/
                dict["Type"] = "SA";
            }
            else if (r.NumberOfPages >= 4 || fileinfo.Length > 250000)
            {
                //text = PdfTextExtractor.GetTextFromPage(r, 1, new SimpleTextExtractionStrategy());
                dict.Merge(
                    RGX_SA_NEWINFO.ToKeyValues(
                        PdfTextExtractor.GetTextFromPage(
                            r, 1, new SimpleTextExtractionStrategy()
                        )
                    )
                );

                /*var groupedByPrice = Regex.Matches(
                    text, MonthsPat, RegexOptions.Multiline
                ).Cast<Match>().Select(
                    m => new
                    {
                        Month = m.Groups[1].Value,
                        Price = m.Groups[2].Value
                    }
                ).GroupBy(m => m.Price);

                string[] months = null;

                if (groupedByPrice.Any(g => g.Count() == 4))
                    months = groupedByPrice.Single(
                            p => p.Count() == 4 && p.Key[1] != '0'
                        ).Select(m => m.Month).ToArray();
                else if (groupedByPrice.Sum(g => g.Count()) == 4)
                    months = groupedByPrice.SelectMany(g => g.ToArray()).Select(m => m.Month).ToArray();

                groupedByPrice = null;
                dict["Schedule"] = TryGetSchedule(months);

                months = null;*/
                dict["Type"] = "SA2";
            }
            else
                throw new Exception("PDF type unknown.");
            r.Dispose();
            r.Close();
            r = null;

            if (dict.TryGetValue("Phone", out string phone))
                dict["Phone"] = PPRGX.PHONE.Replace(phone, "$1-$2-$3");

            bool brokenInvoice = dict.ContainsKey("InvoiceID") && !dict.ContainsKey("FirstName");
            dict["Broken"] = brokenInvoice.ToString();
            if (brokenInvoice)
                dict.Merge(
                    FixBrokenInvoice(
                        dict["VTID"], 
                        dict["Branch"], 
                        ref fileName
                    )
                );

            if (String.IsNullOrWhiteSpace(dict["LastName"]) && !String.IsNullOrWhiteSpace(dict["SpouseName"]))
            {
                dict["LastName"] = dict["SpouseName"];
                dict["SpouseName"] = "";
            }

            tempZip = dict["Zip"];
            if (!String.IsNullOrEmpty(tempZip) && !tempZip.Contains('-'))
            {
                tempZip = dict["Zip"];
                if (String.IsNullOrEmpty(dict["City"]) || String.IsNullOrEmpty(dict["State"]))
                {
                    var newInfo = this.upsClient.FixInfo(tempZip);
                    dict["City"] = newInfo.Key;
                    dict["State"] = newInfo.Value;
                }
                dict["Zip"] = this.upsClient.GetZipExt(dict);
            }

            if (!String.IsNullOrEmpty(dict["Type"]) && String.IsNullOrEmpty(newFileName))
                newFileName = InfoToFileName(dict);
            dict.Add("FileName", newFileName);

            /*this.timers[0].Stop();
            this.responseTimes["ExtrPDF"].Add(this.timers[0].ElapsedMilliseconds);
            this.timers[0].Reset();*/

            return dict;
        }

        private Dictionary<string, string> FixBrokenInvoice(string accountID, string branch, ref string fileName)
        {
            Dictionary<string, string> customer = this.universalClient.VT.InvoiceByAccountID(accountID, branch);
            File.Delete(fileName);
            fileName = customer["FileName"];
            int comma = customer["Address"].IndexOf(',');
            customer.Merge(PPRGX.LOC_INFO.ToKeyValues(customer["Address"].Substring(comma + 2)));
            customer["Address"] = customer["Address"].Substring(0, comma);
            return customer;
        }


        private string TryGetSchedule(string[] months)
        {
            try
            {
                /*if (tempMonths.Count > 4)
                    months = tempMonths.GroupBy(
                        m => m.Price
                    ).Single(
                        g => g.Count() != 4
                    ).Select(m => m.Month).ToArray();
                else if (tempMonths.Count == 4)
                    months = tempMonths.Select(
                        m => m.Month
                    ).ToArray();
                else if (tempMonths.Count == 12)
                    dict["Schedule"] = "MONTHLY";
                else if (tempMonths.Count == 1)
                    dict["Schedule"] = "YRLY";

                if (dict["Schedule"] != ""
                    dict["Schedule"] = schedules.Single(
                        s => months.All(
                            m => s.Value.Contains(m)
                        )
                    ).Key;*/

                if (schedules["MOSQUITOS"].All(s => months.Contains(s)))
                    return "MOSQUITOS";
                else if (months.Length >= 4)
                    return schedules.Single(
                        s => months.All(
                            m => s.Value.Contains(m, StringComparer.CurrentCultureIgnoreCase)
                        ) || s.Value.All(m => months.Contains(m))
                    ).Key;
                else if (months.Length == 6)
                    return "BIMONTHLY";
                else if (months.Length == 12)
                    return "MONTHLY";
                else if (months.Length == 1)
                    return "YRLY";
                else if (months.Length == 2)
                    return "SEMIANNUAL";
                else if (months.Length == 3)
                    return "21 DAYS";
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LocationModel> GetInfo(string id, bool isCode = true)
        {
            try
            {
                return await Task.Factory.StartNew<LocationModel>(
                    () => JsonConvert.DeserializeObject<LocationModel>(
                        client.Execute(
                            new RestRequest(
                                (isCode ? "Locations/code/" : "Locations/") + id,
                                Method.GET
                            )
                        ).Content
                    )
                );
            }
            catch
            {
                return new LocationModel(LocationCode: id);
            }
        }

        public static string[] ExtractMatches(Match match)
        {
            if (match == null)
                return new string[] { "", "", "", "", "" };
            var mtches = match.Groups.Cast<Group>().Select(g => (String.IsNullOrEmpty(g.Value) ? "" : g.Value.Trim())).ToList();
            mtches.RemoveAt(0);
            return mtches.Concat(new string[] { "", "" }).ToArray();
        }

        public static string InfoToFileName(Dictionary<string, string> cust)
        {
            StringBuilder sb = new StringBuilder(cust["Type"]);
            sb.Append(" - ");
            sb.Append(cust["FirstName"]);
            if (cust.TryGetValue("SpouseName", out string spouse))
            {
                sb.Append(" and ");
                sb.Append(spouse);
            }
            sb.Append(" ");
            sb.Append(cust["LastName"]);
            sb.Append(" - ");
            if (cust.TryGetValue("InvoiceID", out string invoiceid))
            {
                sb.Append(cust["VTID"]);
                sb.Append(", ");
                sb.Append(invoiceid);
                sb.Append(", ");
                sb.Append(cust["Balance"]);
                sb.Append(" - ");
            }
            if (cust.TryGetValue("Email", out string email))
            {
                sb.Append(email);
                sb.Append(", ");
                sb.Append(cust["Phone"]);
                sb.Append(" - ");
            }
            sb.Append(cust["Address"]);
            sb.Append(", ");
            sb.Append(cust["City"]);
            sb.Append(", ");
            sb.Append(cust["State"]);
            sb.Append(" ");
            sb.Append(cust["Zip"]);
            sb.Append(".pdf");
            return sb.ToString();
        }

        public void GetToken()
        {
            SpinWait.SpinUntil(() => Postman.CheckInternet());
            TokenResponse tokens = new TokenResponse();

            try
            {
                if (DateTime.Compare(expireTime, DateTime.Now) <= 0)
                    throw new Exception("Refresh.");
                RestRequest request = new RestRequest(Method.POST);

                request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
                request.AddParameter("username", "pestpacapi@insightpest.com", ParameterType.GetOrPost);
                request.AddParameter("password", "!Pest6547!", ParameterType.GetOrPost);
                
                //request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=pestpacapi%40insightpest.com&password=!Pest6547!", ParameterType.RequestBody);
                tokens = JSON.Deserialize<TokenResponse>(tokenClient.Execute(request).Content);
            }
            catch
            {
                RestRequest request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "refresh_token", ParameterType.GetOrPost);
                request.AddParameter("refresh_token", this.refreshToken, ParameterType.GetOrPost);

                tokens = JSON.Deserialize<TokenResponse>(tokenClient.Execute(request).Content);
            }
            
            this.accessToken = tokens.access_token;
            this.refreshToken = tokens.refresh_token;
            this.expireTime = DateTime.Now.AddSeconds(tokens.expires_in - 5);

            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + this.accessToken);

            Environment.SetEnvironmentVariable("REFRESH_TOKEN", this.refreshToken, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("TOKEN_EXPIRE_TIME", this.expireTime.ToString("G"), EnvironmentVariableTarget.Machine);
        }


        private struct TokenResponse
        {
            public string scope { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string id_token { get; set; }
            public string access_token { get; set; }
        }

        public List<Dictionary<string,string>> AdvSearch(Dictionary<string, string> query)
        {
            var request = new RestRequest("locations", Method.GET);
            request.AddParameter("q", String.IsNullOrWhiteSpace(query["LastName"]) ? query["FirstName"] : query["LastName"]);

            List<Dictionary<string, string>> res;
            //IRestResponse response;
            try
            {
                //IRestResponse response = client.Execute(request);
                //res = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(response.Content);
                var response = this.client.Execute(request);
                res = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response.Content);
                response = null;
                if (res.Count == 1)
                    return res;
            }
            catch
            {
                
                if (query["LastName"].Any(c => !char.IsLetter(c)))
                    query["LastName"] = query["FirstName"];
                else
                    this.GetToken();
                return this.AdvSearch(query);
            }

            res.RemoveAll(c => compInf.Compare(c["LastName"], query["LastName"], options) != 0 && compInf.Compare(c["FirstName"], query["LastName"], options) != 0);
            if (res.Count == 1)
                return res;

            List<LocationModel> full = new List<LocationModel>();

            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

            int count = res.Count;
            for (int i = 0; i < count; ++i)
            {
                RestRequest req = new RestRequest(res[i]["LocationID"], Method.GET);
                req.AddHeader("Accept", "text/plain, application/json, text/json");
                LocationModel UDF = null;
                try
                {
                    var response = this.client.Execute(req);
                    UDF = (LocationModel)JsonConvert.DeserializeObject(response.Content, typeof(LocationModel), serializerSettings);
                }
                catch
                {
                    this.GetToken();
                    var response = this.client.Execute(req);
                    UDF = (LocationModel)JsonConvert.DeserializeObject(response.Content, typeof(LocationModel), serializerSettings);
                }

                try
                {
                    string VTID = UDF.UserDefinedFields.Single(u => u.Caption == "VT Cust #").Value;
                    if (VTID == "")
                        VTID = null;
                    if (compInf.Compare(res[i]["Phone"], query["Phone"], options) == 0 || compInf.Compare(VTID, query["VTID"], options) == 0)
                        results.Add(res[i]);
                }
                catch
                {

                }
                
            }
            if (results.Count <= 10 && results.Count >= 2 && results.All(r => compInf.Compare(results[0]["Email"], r["Email"], options) == 0 || compInf.Compare(results[0]["Phone"], r["Phone"], options) == 0 || /*compInf.Compare(results[0]["Zip"], r["Zip"], options) == 0 || */compInf.Compare(results[0]["Address"], r["Address"], options) == 0))
                throw new DuplicateWaitObjectException();
            else
                return results;
        }

        #region GetNotes

        public List<NoteListModel> GetNotes(string locID, NoteCode filter, DateTime startdate)
        {
            return this.GetNotes(locID, filter.ToString(), startdate);
        }

        public List<NoteListModel> GetNotes(string locID, NoteCode filter)
        {
            return this.GetNotes(locID, filter.ToString());
        }

        public List<NoteListModel> GetNotes(string locID, string code, DateTime startdate)
        {
            return this.GetNotes(locID).FindAll(
                n => n.NoteCode == code && (
                    !n.NoteDate.HasValue ||
                    startdate.CompareTo(n.NoteDate.Value) <= 0
                )
            );
        }

        public List<NoteListModel> GetNotes(string locID, string code)
        {
            return this.GetNotes(locID).FindAll(n => n.NoteCode == code);
        }

        public List<NoteListModel> GetNotes(string locID)
        {
            TryExecute(new RestRequest("locations/" + locID + "/notes", Method.GET), out string content);
            return JsonConvert.DeserializeObject<List<NoteListModel>>(content);
        }

#endregion

        public bool NoteExists(string locID, string note)
        {
            return GetNotes(locID).Any(n => compInf.IndexOf(n.Note, note, options) != -1 || compInf.IndexOf(note, n.Note, options) != -1);
        }

        public void DownloadServiceReport(string invoiceID, string path)
        {
            client.DownloadData(new RestRequest("ServiceOrders/" + invoiceID + "/inspectionReport")).SaveAs(path);
        }

        public void DownloadDoc(string docID, string path)
        {
            RestRequest request = new RestRequest("documents/" + docID + "/download");
            request.AddParameter("type", "LocationDocument", ParameterType.GetOrPost);
            client.DownloadData(request).SaveAs(path);
            request = null;
        }

        public async Task<List<Dictionary<string, string>>> GetLocID(Dictionary<string, string> query)
        {
            //this.timers[1].Start();
            //var client = new RestClient("https://api.workwave.com/pestpac/v1/locations?q=" + query["LastName"]);
            var request = new RestRequest("locations", Method.GET);
            request.AddParameter("q", String.IsNullOrWhiteSpace(query["LastName"]) ? query["FirstName"] : query["LastName"]);
            
            List<Dictionary<string,string>> res;
            //IRestResponse response;
            try
            {
                var response = await Task.Factory.StartNew(() => this.client.Execute(request));
                res = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response.Content));
                response = null;

                /*this.timers[1].Stop();
                this.responseTimes["GetID"].Add(this.timers[1].ElapsedMilliseconds);
                this.timers[1].Reset();*/
            }
            catch
            {
                //timers[1].Reset();
                if (query["LastName"].Any(c => !char.IsLetter(c)))
                    query["LastName"] = query["FirstName"];
                else
                    this.GetToken();
                return this.GetLocID(query).Result;
            }
            
            request = null;

            return this.SearchResults(res, query);
        }

        private bool CheckForDuplicates(List<Dictionary<string, string>> res)
        {
            try
            {
                return res.Count <= 10 && res.Count >= 2 && res.All(r => compInf.Compare(res[0]["Phone"], r["Phone"], options) == 0 || compInf.Compare(res[0]["Address"], r["Address"], options) == 0 || compInf.Compare(res[0]["Email"], r["Email"], options) == 0/*compInf.Compare(results[0]["Zip"], r["Zip"], options) == 0 || */);
            }
            catch
            {
                return false;
            }
        }

        public List<Dictionary<string,string>> SearchResults(List<Dictionary<string,string>> res, Dictionary<string,string> query)
        {
            List<string> keyOrder = new List<string> { "FirstName", "LastName", /*"State", "City",*/ "Zip", "Address", "Phone", "Email" };
            for (int i = 0; i < keyOrder.Count; i++)
            {
                string val = null;
                if (!query.TryGetValue(keyOrder[i], out val))
                {
                    query.Add(keyOrder[i], "");
                    //query.
                }
                if (val == "")
                {
                    keyOrder.RemoveAt(i);
                    --i;
                }
            }

            res.RemoveAll(
                r => compInf.Compare(query["State"], r["State"]) != 0 && compInf.Compare(query["City"], r["City"]) != 0
            );

            if (res.Count == 1)
                return res;
            else if (CheckForDuplicates(res))
                throw new DuplicateWaitObjectException();

            List<Dictionary<string, string>> results = null;
            string[] temp = query["Address"].Split(' ');
            List<KeyValuePair<string, string>> exts = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < temp.Length; ++i)
            {
                string BigExt;
                string BigExt2;
                if (this.abbrvs.TryGetValue(temp[i].ToUpper(), out BigExt))
                    exts.Add(new KeyValuePair<string, string>(temp[i], BigExt));
                else if (this.abbrvs2.TryGetValue(temp[i].ToUpper(), out BigExt2))
                    exts.Add(new KeyValuePair<string, string>(temp[i], BigExt2));
            }

            double count = Math.Pow(2, exts.Count);
            for (int i = 0; i <= count - 1; i++)
            {
                string vaar = query["Address"];
                string str = Convert.ToString(i, 2).PadLeft(exts.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                        vaar.Replace(exts[j].Key, exts[j].Value);
                }
                results = res.FindAll(c => compInf.Compare(c["Address"], vaar, options) == 0);
                if (results.Count == 1)
                    return results;
                else if (CheckForDuplicates(results))
                    throw new DuplicateWaitObjectException();
            }

            string[] keys = query.Select(q => q.Key).ToArray();

            /*
            foreach(string key in keys)
            {
                query[key] = query[key].ToUpper().Trim();
            }
            res.RemoveAll(c => c["LastName"].ToUpper() != query["LastName"]);
            */
            res.RemoveAll(c => compInf.Compare(c["LastName"], query["LastName"], options) != 0 && compInf.Compare(c["FirstName"], query["LastName"], options) != 0);

            if (res.Count == 1)
                return res;
            else if (CheckForDuplicates(res))
                throw new DuplicateWaitObjectException();

            List<List<Dictionary<string, string>>> resultsList = new List<List<Dictionary<string, string>>>();

            count = Math.Pow(2, keyOrder.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                results = res;
                List<string> searchString = new List<string>();
                string str = Convert.ToString(i, 2).PadLeft(keyOrder.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        //results = results.FindAll(c => c[keyOrder[j]].ToUpper().Trim() == query[keyOrder[j]]);
                        results = results.FindAll(c => compInf.Compare(c[keyOrder[j]], query[keyOrder[j]], options) == 0);
                        searchString.Add(keyOrder[j]);
                    }

                    if (results.Count == 0)
                        break;
                }

                if (results.Count == 0)
                {
                    if (searchString.Count == 1)
                    {
                        keyOrder.Remove(searchString[0]);
                        count /= 2;
                        i -= ((Convert.ToInt32(count) % i) + 1);
                    }
                }
                else if (results.Count == 1)
                    return results;
                else if (CheckForDuplicates(results))
                    throw new DuplicateWaitObjectException();
                else if (results.Count < 25)
                {
                    query["SearchString"] = String.Join(", ", searchString);
                    results.Insert(0, query);
                    resultsList.Add(results);
                }
                else
                    continue;
            }

            if (query.TryGetValue("SpouseName", out _))
            {
                if (compInf.Compare(query["SpouseName"], query["FirstName"], options) != 0 && query["SpouseName"] != "")
                {
                    string bak = query["FirstName"];
                    query["FirstName"] = query["SpouseName"];
                    query["SpouseName"] = "";
                    List<Dictionary<string, string>> results2 = this.GetLocID(query).Result;
                    if (results2 != null)
                    {
                        results2.RemoveAt(0);
                        if (results2.Count == 1)
                            return results2;
                        else if (CheckForDuplicates(results2))
                            throw new DuplicateWaitObjectException();
                    }
                    query["FirstName"] = bak;
                }
                else
                    query["SpouseName"] = "";
            }

            if (resultsList.Count == 0)
            {
                if (res.Count == 0)
                    return null;
                res[0].Add("Missing", "True");
                return res;
            }
            return resultsList.OrderBy(lcs => lcs.Count).ToList()[0];
        }

        public string SetDocRecord(Dictionary<string, string> cust, out string docID, string name = "SA")
        {
            if (this.DocExists(cust["LocationID"], out docID, name)) 
                return "Doc Exists";

            //this.timers[2].Start();
            var request = new RestRequest("documents", Method.POST);
            request.AddParameter("type", "LocationDocument", ParameterType.QueryString);

            LocationDocument locDoc = new LocationDocument(cust["LocationID"], name);

            request.AddParameter("application/json", JsonConvert.SerializeObject(locDoc), ParameterType.RequestBody);
            string response = null;

            try
            {
                return JsonConvert.DeserializeObject<dynamic>(client.Execute(request).Content).DocumentID;
                /*this.timers[2].Stop();
                this.responseTimes["SetRec"].Add(this.timers[2].ElapsedMilliseconds);
                this.timers[2].Reset();*/
            }
            catch
            {
                //this.timers[2].Reset();
                this.GetToken();
                response = this.SetDocRecord(cust, out docID, name);
            }
            return response;
        }

        public Document UploadDoc(string DocID, string filePath)
        {
            //this.timers[4].Start();
            //var client = new RestClient("https://api.workwave.com/pestpac/v1/documents/" + DocID + "/upload?type=LocationDocument");
            var request = new RestRequest("documents/{DocID}/upload", Method.POST);
            request.AddUrlSegment("DocID", DocID);
            request.AlwaysMultipartFormData = true;
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("type", "LocationDocument", ParameterType.QueryString);
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + new FileInfo(filePath).Name + "\"\r\nContent-Type: application/pdf\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            request.AddFile(
                "file", 
                File.ReadAllBytes(filePath), 
                new FileInfo(filePath).Name, 
                "multipart/form-data"
            );

            Document response = null;
            IRestResponse temp = null;
            try
            {
                temp = client.Execute(request);
                response = JsonConvert.DeserializeObject<Document>(temp.Content);
                /*this.timers[4].Stop();
                this.responseTimes["Upload"].Add(this.timers[4].ElapsedMilliseconds);
                this.timers[4].Reset();*/
            }
            catch (Exception e)
            {
                if (temp.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                    throw new HttpRequestException();
                //this.timers[4].Reset();
                this.GetToken();
                response = this.UploadDoc(DocID, filePath);
            }
            return response;
        }

        public bool Patch(string locID, List<PatchOperation> ops, string patchResource = "locations")
        {
            RestRequest request = new RestRequest(
                patchResource + "/" + locID,
                Method.PATCH
            );
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", JsonConvert.SerializeObject(ops), ParameterType.RequestBody);

            HttpStatusCode status = client.Execute(request).StatusCode;
            if (status == HttpStatusCode.Unauthorized)
            {
                this.GetToken();
                return this.Patch(locID, ops);
            }

            return status == HttpStatusCode.OK;
        }

        public static List<PatchOperation> CreatePatch(PatchEntry[] patch, Dictionary<string, string[]> optional, Dictionary<string, string> required = null, string path = "/userdefinedfields/")
        {
            List<PatchOperation> ops = required == null ? new List<PatchOperation>() : required.Select(f => new PatchOperation("replace", path + f.Key, f.Value)).ToList();
            for (int i = 0; i < patch.Length; ++i)
            {
                PatchEntry field = patch[i];
                if (optional.TryGetValue(field.DictionaryKey, out string[] values))
                    ops.Add(
                        new PatchOperation(
                            "replace", 
                            path + field.UserFieldName, 
                            field.ConvertNumeric ? ZillowClient.rgxNumeric.Replace(values.First(), "") : values.First()
                        )
                    );
            }

            return ops;
        }

        public string GetIDFromCode(string code)
        {
            return JSON.DeserializeDynamic(
                client.Execute(
                    new RestRequest(
                        "Locations/code/" + code
                    )
                ).Content
            ).LocationID;
        }

        public string GetTaxCode(string state, string county, string city, string zip)
        {
            int branchID = GetBranchID(zip);
            if (branchID == -1)
                return "NOT FOUND";

            RestRequest request = new RestRequest("lookups/TaxCodes/autoFill", Method.GET);
            request.AddQueryParameter("branchId", branchID.ToString());
            request.AddQueryParameter("state", state);
            request.AddQueryParameter("county", county);
            request.AddQueryParameter("city", city);
            request.AddQueryParameter("zip", zip);
            return JSON.DeserializeDynamic(client.Execute(request).Content).Code;
        }

        public int GetBranchID(string zip)
        {
            RestResponse response = (RestResponse)client.Execute(new RestRequest("lookups/Branches/zip/" + zip, Method.GET));
            return (response.StatusCode != HttpStatusCode.NotFound ? JSON.DeserializeDynamic(response.Content).BranchID : -1);
        }

        public LocationModel CreateLocation(LocationInputModel input)
        {
            RestRequest request = new RestRequest("Locations", Method.POST);
            request.AddQueryParameter("validateAndGeocode", "true");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", JsonConvert.SerializeObject(input), ParameterType.RequestBody);

            if (this.TryExecute(request, out string content))
                return JsonConvert.DeserializeObject<LocationModel>(content);
            else
                throw new Exception("CONTENT - " + content);
        }

        public bool TryExecute(IRestRequest request, out string content)
        {

            RestResponse response = null;
            try
            {
                response = (RestResponse)client.Execute(request);
                content = response.Content;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Logger.Enqueue(
                        new LogUpdate(
                            "Postman",
                            EntryType.HTTP,
                            ((int)response.StatusCode).ToString(),
                            "Getting new token...",
                            request.Resource
                        )
                    );

                    //OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = "Getting new token...", URL = request.Resource });
                    this.GetToken();
                    return this.TryExecute(request, out content);
                }
                else if ((int)response.StatusCode >= 400)
                {
                    Logger.Enqueue(
                        new LogUpdate(
                            "Postman",
                            EntryType.HTTP,
                            ((int)response.StatusCode).ToString(),
                            request.Resource,
                            response.Content
                        )
                    );
                    //OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = response.Content.ToString(), URL = request.Resource });
                    return false;
                }
            }
            catch (Exception e)
            {
                if (!CheckInternet())
                {
                    Logger.Enqueue(
                        new LogUpdate(
                            "Postman",
                            EntryType.NTWRK,
                            "Internet down, waiting..."
                        )
                    );

                    //OnExecute(this, new ExecutionEventArgs() { ResponseCode = "NTWRK", Message = "Internet down, waiting..." });
                    SpinWait.SpinUntil(() => CheckInternet());
                    //OnExecute(this, new ExecutionEventArgs() {ResponseCode = "NTWRK", Message = "Internet back up!" });
                    return this.TryExecute(request, out content);
                }
                else
                {
                    Logger.Enqueue(
                        new LogUpdate(
                            "Postman",
                            EntryType.HTTP,
                            ((int)response.StatusCode).ToString(),
                            request.Resource,
                            e.ToErrString(5)
                        )
                    );

                    //OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = Form1.ExceptionToString(e, 5), URL = request.Resource });
                    content = null;
                }
            }
            
            return (int)response.StatusCode <= 400;
        }

        public bool DocExists(string locID, out string docID, string name = "SA")
        {
            //this.timers[3].Start();
            //var client = new RestClient("https://api.workwave.com/pestpac/v1/Locations/" + locID + @"/documents");
            var request = new RestRequest("Locations/{locID}/documents", Method.GET);
            request.AddUrlSegment("locID", locID);
            request.AddHeader("Accept", "text/plain, application/json");

            List<KeyValuePair<Document,DateTime>> docs = null;
            bool response = false;
            try
            {
                docs = JsonConvert.DeserializeObject<List<Document>>(
                    client.Execute(request).Content
                ).Select(
                    d => new KeyValuePair<Document, DateTime>(
                       d, DateTime.Parse(d.Date)
                    ) 
                ).ToList().FindAll(
                    d => d.Value.CompareTo(DateTime.Now.AddMonths(-3)) >= 0 &&
                    d.Key.FileName != "tempName.pdf"
                );
                if (name == "SA")
                    response = docs.Any(d => d.Key.Name.ToUpper().Contains("SA") || (d.Key.Name.ToUpper().Contains("SERVICE") && d.Key.Name.ToUpper().Contains("AGREEMENT")));
                else
                    response = docs.Any(d => d.Key.Name.ToUpper().Contains("INPC") || d.Key.Name.ToUpper().Contains("INVOICE"));
                List<KeyValuePair<Document, DateTime>> postmanTags = docs.FindAll(d => !String.IsNullOrEmpty(d.Key.Tags) && d.Key.Tags.Contains("Postman"));
                if (postmanTags.Count > 0)
                    docID = postmanTags.OrderBy(d => d.Value).LastOrDefault().Key.DocumentID;
                else
                    docID = null;

                /*this.timers[3].Stop();
                this.responseTimes["Exists"].Add(this.timers[3].ElapsedMilliseconds);
                this.timers[3].Reset();*/
            }
            catch
            {
                //this.timers[3].Reset();
                this.GetToken();
                response = this.DocExists(locID, out docID, name);
            }

            if (docs != null && response && docs.Any(d => d.Key.Name.ToUpper().Contains("TEMPNAME")))
                throw new EmptyRecordException(locID);
            return response;
        }

        public void UploadNote(string locID, string note, string prefix = "VTNotes : ", NoteCode code = NoteCode.GEN)
        {
            this.UploadNote(
                new NoteModel(
                   locID, prefix + note, code.ToString()
                )
            );
        }

        public void UploadNote(string locID, string note, string prefix = "VTNotes : ", string code = "GEN")
        {
            this.UploadNote(
                new NoteModel(
                   locID, prefix + note, code
                )
            );
        }

        public void UploadNote(NoteModel note)
        {
            TryExecute(new RestRequest("locations/" + note.Associations.LocationID + "/notes", Method.POST).AddJsonBody(note), out _);
        }

        public void PatchNumber(string locID, string number)
        {
            RestRequest request = new RestRequest("Locations/" + locID, Method.PATCH);
            request.AddHeader("Accept", "text/plain, application/json, text/json");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(
                new List<OperationLocationInputModel>() {
                    new OperationLocationInputModel(number, "/MobilePhone", "replace")
                }
            ), ParameterType.RequestBody);
            
            IRestResponse response = this.client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                this.GetToken();
                this.PatchNumber(locID, number);
            }
        }

        public string GetTechID(string user)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>[]>(
                client.Execute(
                    new RestRequest(
                        "/lookups/Employees?username=" + user,
                        Method.GET
                    )
                ).Content
            )[0]["TechID"];
        }

        public string GetBranch(string zip)
        {
            return JSON.Deserialize<Dictionary<string, string>>(
                client.Execute(
                    new RestRequest(
                        "/lookups/Branches/zip/" + zip,
                        Method.GET
                    )
                ).Content
            )["Name"];
        }

        public static bool CheckInternet()
        {
            try
            {
                return new Ping().Send(
                    "8.8.8.8", 250
                ).Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
    public struct PatchEntry
    {
        public string UserFieldName { get; set; }
        public string DictionaryKey { get; set; }
        public bool ConvertNumeric { get; set; }

        public PatchEntry(string name, string key, bool num = false)
        {
            UserFieldName = name;
            DictionaryKey = key;
            ConvertNumeric = num;
        }
    }

    public class ExecutionEventArgs : EventArgs
    {
        public string ResponseCode { get; set; }
        public string URL { get; set; }
        public string Message { get; set; }
    }
}

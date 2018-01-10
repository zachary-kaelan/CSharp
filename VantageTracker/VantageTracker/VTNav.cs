using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Web;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using RestSharp;
using CsvHelper;
using CsvHelper.Configuration;
using Zillow = PPAPIOptimized.ZillowClient;
using PPLib;
using RGX.VT;
using RGX.Utils;
using Match = System.Text.RegularExpressions.Match;

namespace VantageTracker
{

    public class VTNav
    {
        public const string LOG_FORMAT = "[{0}] [{1}]:\t{2}";
        public const string LOG_DATE_FORMAT = "HH:mm:ss";
        public const string FILENAME_DATE_FORMAT = "MM.dd.yyyy";
        public static StreamWriter log = new StreamWriter(@"C:\DocUploads\Logs\VantageTracker\" + DateTime.Now.ToString(FILENAME_DATE_FORMAT) + ".txt", true) { AutoFlush = true };

        #region Dictionaries
        private static readonly IVTUpdateModel[] VTUpdateModels = new IVTUpdateModel[]
        {
            new VTServiceInfoModel(),
            new VTServiceAddrModel()
        };

        public static readonly Dictionary<
            string, Dictionary<string, string>
        > VTDictionaries = Directory.GetFiles(VT_DICTS_PATH)
            .ToDictionary(
                f => new string(new FileInfo(f).Name.TakeWhile(c => c != '.').ToArray()),
                f => File.ReadAllLines(f).ToDictionary(
                    s => s.Split(new string[] { " :=: " }, StringSplitOptions.None)[0],
                    s => s.Split(new string[] { " :=: " }, StringSplitOptions.None)[1]
                )
            );

        public static int VT_REQUESTS = 0;

        public static readonly Dictionary<
            string, Dictionary<string, string[]>
        > VTRequests = Directory.GetFiles(VT_REQS_PATH)
            .ToDictionary(
                f => new string(new FileInfo(f).Name.TakeWhile(c => c != '.').ToArray()),
                f => File.ReadAllLines(f).GroupBy(
                    s => HttpUtility.UrlEncode(s.Split(new string[] { " :=: " }, StringSplitOptions.None)[0]),
                    s => HttpUtility.UrlEncode(s.Split(new string[] { " :=: " }, StringSplitOptions.None)[1])
                ).ToDictionary(
                    g => g.Key, 
                    g => g.ToArray()/*g.SelectMany(
                        v => v.Split(
                            new string[] { ",", ", "}, 
                            StringSplitOptions.None
                        ).Select(p => HtmlUti)
                    ).ToArray()*/
                )
            );

        public const string VT_DICTS_PATH = @"C:\DocUploads\Program Files\VTDictionaries\";
        public const string VT_REQS_PATH = @"C:\DocUploads\Program Files\VTRequests\";
        private static readonly string TEMP_PATH = System.IO.Path.GetTempPath() + @"\VantageTracker\";
        private object branchLock = new object();
        public static ManualResetEvent downed = new ManualResetEvent(false);
        public const int REGEX_TIMEOUT = 3000;
        public static TimeSpan rgxTimeout = TimeSpan.FromMilliseconds(REGEX_TIMEOUT);
        const int TIMEOUT = 1000 * 60;
        private string currentBranch { get; set; }
        private DateTime taskStartTime { get; set; }

        private static readonly Dictionary<string, string> urls = new Dictionary<string, string>
        {
            {"base", "https://myvantagetracker.com/"},
            {"lgnScreen", "https://myvantagetracker.com/login"},
            {"lgnCheck", "https://myvantagetracker.com/login_check"},
            {"msgBoard", "https://myvantagetracker.com/message-board"},
            {"branchSwitch", "https://myvantagetracker.com/branch-switch"},
            {"search", "https://myvantagetracker.com/customer/search/suggestions"},
            {"srvcInf", "https://myvantagetracker.com/customer/{0}/service-information"},
            {"srvcInfUpd", "https://myvantagetracker.com/customer/{0}/service-information/update"},
            {"srvcHist", "https://myvantagetracker.com/customer/{0}/service-history"},
            {"invcInf", "https://myvantagetracker.com/customer/{0}/services/{1}/show"},
            {"blngInf", "https://myvantagetracker.com/customer/{0}/billing-information"},
            {"acctInf", "https://myvantagetracker.com/customer/{0}/payment-account/{1}/edit"},
            {"INPC", "https://myvantagetracker.com/finance/unpaid/billing-summary"},
            {"advSearch", "https://myvantagetracker.com/customers/advanced-search"},
            {"advSearchData", "https://myvantagetracker.com/customer/advanced-search/data"},
            {"update", "https://myvantagetracker.com/customer/{0}/{1}/{2}"},
            {"redirect", "https://myvantagetracker.com/customer/{0}/redirect-to"},
            {"advExport", "https://myvantagetracker.com/customers/advanced-export"},
            {"advExportFull", "https://myvantagetracker.com/customers/advanced-export"},
            {"capSig", "https://myvantagetracker.com/customer/{0}/contract/edit-signature"}
        };

        private static readonly Dictionary<string, string> tokens = new Dictionary<string, string>
        {
            {"lgnCheck", "https://myvantagetracker.com/login"},
            {"branchSwitch", "https://myvantagetracker.com/message-board"},
            {"INPC", "https://myvantagetracker.com/finance/unpaid"},
            {"srvcInfUpd", "https://myvantagetracker.com/customer/{0}/service-information"},
            {"advSearch", "https://myvantagetracker.com/customers/advanced-search/show"},
            {"advExport", "https://myvantagetracker.com/customers/advanced-export"},
            {"advExportFull", "https://myvantagetracker.com/customers/advanced-export"}
        };

        private static readonly Dictionary<string, string> findToken = new Dictionary<string, string>
        {
            {"INPC", "unpaidSearchTermsType["},
            {"lgnCheck", "_csrf"},
            {"branchSwitch", "office_select"},
            {"advSearch", "searchtype["},
            {"advExport", "searchexport["},
            {"advExportFull", "searchexport["}
        };

        public static readonly Dictionary<string, string> referers = new Dictionary<string, string>
        {
            {"branchSwitch", urls["msgBoard"]},
            {"advSearchData", urls["advSearch"]},
            {"advExport", urls["advSearch"]},
            {"advExportFull", urls["advSearch"]}
        };

        public static readonly Dictionary<string, string> downloadFileExts = new Dictionary<string, string>
        {
            {"customer-export", ".csv"},
            {"billing-summary", ".pdf"}
        };
#endregion

        public CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public CompareOptions opts = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;

        private readonly CookieContainer Cookies = new CookieContainer();

        public RestClient client { get; set; }
        public string referer { get; set; }
        private MessageBoard msgBoard { get; set; }
        /*public const string tokenRGX = "name=\"(.{0,30}" + @"(?:\[|" + "\"|_).{0,20}token" + @"(?:\]" + "\"|\")) value=\"(.+?)\"";
        public const string VTIDRGX = "\"sorting_1\">(.+?)<" + @"\/td><td>(?<x>.*?)<\/td><td>(?<y>.*?)<\/td><td>(.*?)<.+?href=" + "\"" + @"\/customer\/(\d*?)\/service-information" + "\"";
        public const string InvoiceIDRGX = "data-jobid=\"(.*?)\">" + @"[\s\S]{50,75}<td>([\/\d]*?)\s*?<\/td>";
        public const string ImageRGX = "^<img src=\"data:image.png;base64,([^\"]+)";*/
        //public static readonly Regex PPRGX.FILE_NUM = new Regex(@"(\d+)\.[a-z]+$", RegexOptions.RightToLeft | RegexOptions.Compiled);
        //public static readonly Regex PPRGX.INVOICE_INFO = new Regex("^ +<tr class=\"clickable\" data-id=\"(?<AccountID>[^\"]*)\" data-jobid=\"(?<JobID>[^\"]*)\"" + @"(?:<\/td)?>[^>]+>(?<InvoiceID>[^<]*)(?:<\/td)?>[^>]+>(?<Date>[^ ]*)\n +(?:<\/td)?>[^>]+>(?<Status>[^<]*)(?:<\/td)?>[^>]+>(?<Type>[^<]*)(?:<\/td)?>[^>]+>(?<Total>[^<]*)(?:<\/td)?>[^>]+>(?<Balance>[^<]*)(?:<\/td)?>[^>]+>(?<Paid>[^<]*)(?:<\/td)?>[^>]+>(?<Technician>[^<]*)(?:<\/td)?>[^>]+>", RegexOptions.Multiline | RegexOptions.Compiled);
        //public static readonly Regex PPRGX.TECH_NOTE = new Regex("input-xxlarge limit.>(?<TechNote>[^<]+)", RegexOptions.Compiled);
        //public const string billSumTimesRGX = @"on (\d\d\/\d\d\/\d\d (?:.+?) (?:AM|PM))";
        
        public const string DOCS_PATH = @"C:\DocUploads\Docs\";
        private LogManager Logger { get; set; }

        public VTNav(string initBranch = null, LogManager logger = null)
        {
            if (logger == null)
            {
                Logger = new LogManager(
                    "[{0}] [{1}]:\t{2} ~ {3}",
                    LOG_DATE_FORMAT

                );
            }

            Zillow.TryWriteLineAsync("", log);
            client = new RestClient("https://myvantagetracker.com/");
            client.AddDefaultHeader("Accept", "text/javascript, */*; q=0.01");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            client.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            client.AddDefaultHeader("Origin", "https://myvantagetracker.com");
            client.CookieContainer = Cookies;

            string[] phpsess = Environment.GetEnvironmentVariable("VT_SESS_ID", EnvironmentVariableTarget.Machine).Split(';');
            DateTime expires = DateTime.Parse(phpsess[1]);
            if (expires.CompareTo(DateTime.Now) <= 0 || (expires - DateTime.Now).TotalHours < 4)
                this.Login();
            else
                Cookies.Add(new Cookie("PHPSESSID", phpsess[0], "/", "myvantagetracker.com") { Expires = expires });

            try
            {
                this.POST("branchSwitch", new Dictionary<string, string>()
                {
                    {"office_select[office]",
                        String.IsNullOrWhiteSpace(initBranch) ?
                            "" : (Char.IsDigit(initBranch[0]) ?
                                initBranch : VTDictionaries["BranchIDs"][initBranch])}
                });
            }
            catch
            {
                this.Login();
                this.POST("branchSwitch", new Dictionary<string, string>()
                {
                    {"office_select[office]",
                        String.IsNullOrWhiteSpace(initBranch) ?
                            "" : (Char.IsDigit(initBranch[0]) ?
                                initBranch : VTDictionaries["BranchIDs"][initBranch])}
                });
            }


            HttpWebRequest request = this.defaultReq(urls["msgBoard"]);
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = urls["msgBoard"];
            msgBoard = new MessageBoard(request);

            taskStartTime = new DateTime(3000, 1, 1);
        }

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

        public void Login()
        {
            this.POST("lgnCheck");
            Cookie cookie = Cookies.GetCookies(
                new Uri("https://myvantagetracker.com")
            ).Cast<Cookie>().Single(
                c => c.Name == "PHPSESSID"
            );
            Environment.SetEnvironmentVariable(
                "VT_SESS_ID",
                cookie.Value + ";" + cookie.Expires.ToString("g"),
                EnvironmentVariableTarget.Machine
            );
        }

        public string POST(string URL, Dictionary<string, string[]> form = null, string accept = "application/json, text/javascript, */*; q=0.01", params string[] urlformat)
        {
            Dictionary<string, string[]> parameters = new Dictionary<string, string[]>(VTRequests[URL]);
            if (tokens.TryGetValue(URL, out _))
            {
                string tokenKey = parameters.Keys.Single(k => k.Contains("token"));
                try
                {
                    if (String.IsNullOrWhiteSpace(parameters[tokenKey].First()))
                        parameters[tokenKey] = new string[] { HttpUtility.UrlEncode(this.GetToken(URL)) };
                }
                catch
                {
                    parameters[tokenKey] = new string[] { HttpUtility.UrlEncode(this.GetToken(URL)) };
                }
            }

            byte[] data = Encoding.ASCII.GetBytes(parameters.ToFormURLEncoded(form));
            form = null;
            parameters = null;

            HttpWebRequest request = this.defaultReq(String.Format(urls[URL], urlformat));
            request.Accept = accept;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = GetReferer(URL, urlformat);
            string response = null;
            
            //request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            data = null;

            do
            {
                response = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();
            } while (response.Contains("error"));
            return response;
        }

        public string POST(string URL, Dictionary<string, string> form, params string[] urlformat)
        {
            if (URL == "branchSwitch")
            {
                string branch = form["office_select[office]"];
                if (this.currentBranch == branch)
                    return "success";
                else if (!String.IsNullOrEmpty(currentBranch))
                    Zillow.TryWriteLineAsync(
                        String.Format(
                            LOG_FORMAT,
                            DateTime.Now.ToString(LOG_DATE_FORMAT),
                            "BRANCH",
                            "Switching branch from " + currentBranch + " to " + branch + "."
                        ), log
                    );

                this.currentBranch = branch;
                branch = null;
            }

            if (form.TryGetValue("accept", out string accept))
            {
                form.Remove("accept");
                return this.POST(
                    URL,
                    form.ToDictionary(
                        kv => kv.Key,
                        kv => new string[] { kv.Value }
                    ), accept, urlformat
                );
            }

            return this.POST(
                URL,
                form.ToDictionary(
                    kv => kv.Key,
                    kv => new string[] {kv.Value}
                ), urlformat:urlformat
            );
        }

        public string GetReferer(string URL, params string[] urlformat)
        {
            try
            {
                return String.Format(
                    referers.TryGetValue(URL, out string temp) ?
                        temp : (!String.IsNullOrWhiteSpace(this.referer) ?
                            this.referer : ""
                        ),
                    urlformat
                );
            }
            catch
            {
                return "";
            }
        }

        public string GetToken(string URL, params string[] urlformat)
        {
            HttpWebRequest request = this.defaultReq(String.Format(tokens[URL], urlformat));
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = GetReferer(URL, urlformat); //String.IsNullOrEmpty(this.referer) ? tokens[URL] : this.referer;
            this.referer = String.IsNullOrWhiteSpace(request.Referer) 
                ? String.Format(tokens[URL], urlformat) : request.Referer;

            string str = request.GetResponseString();
            IEnumerable<Match> matches = PPRGX.TOKEN.Matches(str).Cast<Match>();
            request = null;

            if (matches.Count() == 1)
                return matches.First().Groups[2].Value;
            else
                return matches.First(m => m.Groups[1].Value.Contains(findToken[URL])).Groups[2].Value;
        }

        public string FindCust(string query)
        {
            HttpWebRequest request = this.defaultReq(
                String.Format(
                    urls["search"], 
                    "&" + HttpUtility.UrlEncode(query)
                )
            );

            request.Accept = "application/json, text/javascript, */*; q=0.01";
            this.referer = urls["msgBoard"];
            request.Referer = this.referer;
            request.Method = "GET";

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(
                new StreamReader(
                    ((HttpWebResponse)request.GetResponse())
                    .GetResponseStream()
                ).ReadToEnd())
            .Single(
                c => compInf.Compare(
                    c.Value, query, opts
                ) == 0
            ).Key;
        }

        public Customer[] AdvancedSearch(Dictionary<string, string> query, string branch=null) /*bool allBranches = false*/
        {
            //Dictionary<string, string[]> dict = defaultQuery.ToDictionary(kv => kv.Key, kv => kv.Value);
            object temp = new object();

            lock (branch == null ? temp : branchLock)
            {
                if (branch == null && !query.TryGetValue("searchtype[allBranches]", out _))
                    query.Add("searchtype[allBranches]", "");
                else
                {
                    string branchID = VTDictionaries["BranchIDs"][branch];
                    if (this.currentBranch != branchID)
                        this.POST(
                            "branchSwitch", 
                            new Dictionary<string, string>()
                            {{"office_select[office]", branchID}}
                        );
                }

                /*foreach (KeyValuePair<string, string> entry in query)
                {
                    dict[dict.Keys.Single(k => compInf.IndexOf(k, entry.Key) != -1)] = new string[] { entry.Value };
                }*/

                query.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                this.POST("advSearch", query);
                
                string jsonResponse = this.POST(
                    "advSearchData",
                    accept: "application/json, text/javascript, */*; q=0.01"
                );

                VTSearchResults results = JsonConvert.DeserializeObject<VTSearchResults>(jsonResponse);
                jsonResponse = null;
                return results.aaData;
            }
        }

        public static readonly string HttpEncodedSearchExport = "&" + HttpUtility.HtmlEncode("searchexport[exportColumns][]") + "=";
        public static readonly string HttpEncodedTokenExport = HttpUtility.HtmlEncode("searchexport[_token]") + "=";
        public static readonly Configuration cfg = Utils.csvConfig;

        public T[] AdvancedExport<T>()
        {
            //HttpWebRequest request = this.defaultReq(urls["advExport"]);
            //request.Accept = "application/json, text/javascript, */*; q=0.01";
            /*request.Referer = "https://myvantagetracker.com/customers/advanced-search";
            this.referer = "https://myvantagetracker.com/customers/advanced-search";

            StringBuilder sb = new StringBuilder(HttpEncodedTokenExport);
            sb.Append("=");
            sb.Append(HttpUtility.HtmlEncode(this.GetToken("advExport")));
            foreach(string column in columns)
            {
                sb.Append(HttpEncodedSearchExport);
                sb.Append(column);
            }*/

            Dictionary<string, string[]> columns = new Dictionary<string, string[]>()
            {
                {
                    "searchexport[exportColumns][]",
                    typeof(T).GetProperties().Select(
                        p => VTDictionaries["ExportModelToColumns"][p.Name]
                    ).ToArray()
                }
            };

            this.POST("advExport", columns);
            Thread.Sleep(3000);

            HttpWebRequest request = this.defaultReq(urls["msgBoard"]);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            string response = new StreamReader(
                    new GZipStream(
                        ((HttpWebResponse)request.GetResponse())
                        .GetResponseStream(), CompressionMode.Decompress
                    )
                ).ReadToEnd();
            request = null;

            string headers = response.Substring(0, response.IndexOf('\n'));
            response = response.Replace(
                headers, 
                response.Replace('/', ' ')
                .Replace("(Y/N)", "")
            );
            headers = null;
            columns = null;
            
            StringReader sr = new StringReader(response);
            CsvReader reader = new CsvReader(sr, cfg);
            T[] records = reader.GetRecords<T>().ToArray();
            reader.Dispose();
            reader = null;
            sr.Dispose();
            sr.Close();
            sr = null;
            return records;
        }

        public void FullExport()
        {
            DateTime now = DateTime.Now;
            var ts = now - DateTime.Parse(
                Environment.GetEnvironmentVariable(
                    "LAST_FULL_EXPORT",
                    EnvironmentVariableTarget.Machine
                )
            );
            if (ts.TotalDays >= 14)
            {
                DateTime earliestDocs = DateTime.Now;
                foreach (string branch in VTDictionaries["VTBranchIDs"].Keys)
                {
                    this.POST("branchSwitch", new Dictionary<string, string>()
                    {
                        {"office_select[office]", VTDictionaries["BranchIDs"][branch]}
                    });
                    VTExportModel[] model = AdvancedExport<VTExportModel>();
                }

                
            }
        }

        public DefaultExportModel[] AdvancedExport()
        {
            return AdvancedExport<DefaultExportModel>();
        }

        public Invoice GetInvoice(string accountID, bool getTechNote = true)
        {
            string input = this.defaultReq(String.Format(urls["srvcHist"], accountID)).GetResponseString();
            var invoices = PPRGX.INVOICE_INFO.ToDictionaries(input)
                .Select(d => new Invoice()
                {
                    AccountID = d["AccountID"],
                    JobID = d["JobID"],
                    InvoiceID = d["InvoiceID"],
                    Date = d["Date"],
                    Status = d["Status"],
                    Type = d["Type"],
                    Total = d["Total"],
                    Balance = d["Balance"],
                    Paid = d["Paid"],
                    Technician = d["Technician"]
                });
            Invoice invoice = invoices.Single(i => i.Type == "Initial");
            invoices = null;
            if (getTechNote)
                invoice.TechNote = PPRGX.TECH_NOTE.Match(
                    this.defaultReq(
                        String.Format(
                            urls["invcInf"], 
                            accountID, 
                            invoice.JobID
                        )
                    ).GetResponseString()).Groups["TechNote"].Value;

            return invoice;
        }

        public Dictionary<string, string> InvoiceByAccountID(string accountID, string branch)
        {
            Customer[] results = this.AdvancedSearch(new Dictionary<string, string>() { { "accountid", accountID } }, branch);

            if (results.Length == 1)
            {
                Customer cust = results.Single();
                Dictionary<string, string> customer = new Dictionary<string, string>()
                {
                    {"Name", cust.name},
                    {"Address", cust.address},
                    {"Phone", cust.phone},
                    {"Email", cust.email}
                };

                string VTID = cust.id;
                HttpWebRequest request = this.defaultReq(String.Format("https://myvantagetracker.com/customer/{0}/service-history", VTID));
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Referer = String.Format("https://myvantagetracker.com/customer/{0}/service-information", VTID);
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Method = "GET";

                Match match = PPRGX.INVOICE_ID.Matches(
                    new StreamReader(
                        new GZipStream(
                            ((HttpWebResponse)request.GetResponse())
                            .GetResponseStream(),
                            CompressionMode.Decompress
                        )
                    ).ReadToEnd()
                ).Cast<Match>().ToList().Find(
                    m => DateTime.Parse(m.Groups[2].Value).CompareTo(
                        DateTime.Now.AddMonths(-3)
                    ) >= 0
                );
                string invoiceID = match.Groups[1].Value;

                request = this.defaultReq(String.Format("https://myvantagetracker.com/customer/{0}/invoice/{1}/download", VTID, invoiceID));
                request.Referer = String.Format("https://myvantagetracker.com/customer/{0}/service-history", VTID);
                request.Headers.Add("Upgrade-Insecure-Requests", "1");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                string path = @"C:\DocUploads\Docs\" +
                    Regex.Match(
                        response.Headers.Get("Content-Disposition"),
                        "filename=\"(.*?)\""
                    ).Groups[1].Value;

                FileStream file = new FileStream(
                            path,
                            FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.Asynchronous);
                responseStream.CopyTo(file);
                file.Flush();
                file.Dispose();
                file.Close();
                responseStream.Flush();
                responseStream.Dispose();
                responseStream.Close();

                customer.Add("FileName", path);
                return customer;
            }
            return null;
        }

        public void DownloadSignature(string accountID, string path)
        {
            if (File.Exists(path))
                return;
            //RedirectBranch(accountID);
            HttpWebRequest request = this.defaultReq(String.Format(urls["capSig"], accountID));
            request.Referer = String.Format(urls["srvcInf"], accountID);
            request.Accept = "*/*";

            Match match = request.GetMatch(
                    PPRGX.INVOICE_INFO
                );
            if (match.Success)
            {
                using (MemoryStream ms = new MemoryStream(
                    Convert.FromBase64String(
                        match.Groups[1].Value
                    ))
                )   {
                    
                    Image.FromStream(ms).Save(path, ImageFormat.Png);
                }
            }   
        }

        public (Dictionary<string, string> cust, List<NoteModel> notes) GetNotes (string id)
        {
            HttpWebRequest request = this.defaultReq(String.Format(urls["srvcInf"], id));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = urls["msgBoard"];
            this.referer = String.Format(urls["srvcInf"], id);
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            string response = 
                new StreamReader(
                    new GZipStream(((HttpWebResponse)request.GetResponse())
                        .GetResponseStream(),
                        CompressionMode.Decompress))
                    .ReadToEnd();

            MatchCollection matches = Regex.Matches(response,
                "\"word-wrap: break-word\" data-date=\"(.+?)\"><strong>(.+?)" + Regex.Escape("</strong>") + "(.+?)" + Regex.Escape("</td>"));
            Match info = Regex.Match(response,
                @"\s+?<span class=" + "\"bigger-110 bolder\">(.+?)<" + @"\/span><br>\s+?(.+?)<br>\s+?(.+?)<br>\s+?(.+?)<label class=");

            string[] names = info.Groups[1].Value.Split(' ');
            string[] loc = info.Groups[3].Value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> cust = new Dictionary<string, string>()
            {
                {"FirstName", names[0]},
                {"LastName", names[1]},
                {"Address", info.Groups[2].Value.Trim()},
                {"City", loc[0]},
                {"State", loc[1]},
                {"Zip", loc[2]},
                {"Phone", info.Groups[4].Value.Trim().Replace("(", "").Replace(")", "").Replace(' ', '-')},
                {"VTID", Regex.Match(response, @"dt>\s+?<dd>\s+?(.+?)\s+?<").Groups[1].Value.Trim()}
            };

            List<NoteModel> notes = new List<NoteModel>();
            foreach (Match match in matches)
            {
                NoteModel note = new NoteModel();
                note.NoteDate = match.Groups[1].Value.Replace(' ', 'T') + ".000Z";
                note.Note = match.Groups[3].Value;
                notes.Add(note);
            }

            return (cust, notes);
        }

        public Card ExtractCard(string id)
        {
            HttpWebRequest request = this.defaultReq(String.Format(urls["blngInf"], id));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Referer = String.Format(urls["srvcInf"], id);
            this.referer = String.Format(urls["blngInf"], id);
            request.Headers.Add("Upgrade-Insecure-Requests", "1");

            string account = Regex.Match(
                new StreamReader(
                    new GZipStream(
                        ((HttpWebResponse)request.GetResponse())
                        .GetResponseStream(), CompressionMode.Decompress
                    )).ReadToEnd(),
                "data-accountid=\"(.+?)\"").Groups[1]
            .Value;

            request = this.defaultReq(String.Format(urls["acctInf"], id, account));
            request.Accept = "*/*";
            request.Referer = this.referer;
            request.Method = "GET";

            return new Card(new StreamReader(new GZipStream(((HttpWebResponse)request.GetResponse()).GetResponseStream(), CompressionMode.Decompress)).ReadToEnd());
        }

        public void RedirectBranch(string id)
        {
            HttpWebRequest request = this.defaultReq(String.Format(urls["redirect"], id));
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.GetResponse();
        }

        public Dictionary<string, string[]> GetCustomer(string id, IVTUpdateModel model, out string custid)
        {
            //this.RedirectBranch(id);
            HttpWebRequest editReq = this.defaultReq(String.Format(urls["update"], id, model.urlName, "edit"));
            editReq.Method = "GET";
            editReq.Accept = "*/*";
            editReq.Referer = String.Format(urls["srvcInf"], id);
            return model.FilterMatches(
                new StreamReader(
                    new GZipStream(
                        ((HttpWebResponse)editReq.GetResponse())
                        .GetResponseStream(), CompressionMode.Decompress
                    )
                ).ReadToEnd(), out custid
            );
        }

        public string UpdateCustomer(string id, string type, Dictionary<string, string> info, bool replace = true)
        {
            lock(branchLock)
            {
                IVTUpdateModel model = VTUpdateModels.Single(
                    m => compInf.IndexOf(m.urlName, type, opts) != -1
                );
                Dictionary<string, string[]> dict = this.GetCustomer(id, model, out string custid);

                //string time = null;
                if ((info.All(kv => dict.TryGetValue(kv.Key, out string[] val) && val.Contains(kv.Value))) || dict["service_info[contract][salesStatus]"].Contains("524"))
                    return custid;
                if (dict.TryGetValue("service_info[rescheduleInitialJob][timeScheduled]", out string[] time) && !String.IsNullOrWhiteSpace(time[0]))
                    dict["service_info[rescheduleInitialJob][timeScheduled]"] = new string[] { "" };

                foreach (KeyValuePair<string, string> entry in info)
                {
                    string[] entryArray = new string[] { entry.Value };
                    if (replace)
                        dict[entry.Key] = entryArray;
                    else
                        dict[entry.Key] = dict[entry.Key].Concat(entryArray).ToArray();
                }

                this.POST(
                    "update",
                    dict, urlformat: new string[] { id, model.urlName, "update" }
                );

                return custid;
            }
        }

        public void StartGetInvoices(DateTime earDate)
        {
            DateTime now = DateTime.Now;
            string end = now.ToString("d");
            string start = earDate.ToString("d");
            VT_REQUESTS = Convert.ToInt32(Environment.GetEnvironmentVariable("VT_REQUESTS", EnvironmentVariableTarget.Machine));
            if (VT_REQUESTS == 0 || VT_REQUESTS >= 17)
                Environment.SetEnvironmentVariable("EARLIEST_INVOICES", now.AddHours(-1.0).ToString("G"), EnvironmentVariableTarget.Machine);

            int count = 0;

            Zillow.TryWriteLineAsync(
                String.Format(
                    LOG_FORMAT,
                    DateTime.Now.ToString(LOG_DATE_FORMAT),
                    "DEBUG",
                    "Getting all invoices starting from " + earDate.ToShortDateString() + "."
                ), log
            );

            foreach (string branch in VTDictionaries["VTBranchIDs"].Keys)
            {
                Console.Write("-");
                ++count;
                if (count <= VT_REQUESTS)
                    continue;

                this.POST("branchSwitch", new Dictionary<string, string>()
                {
                    {"office_select[office]", VTDictionaries["BranchIDs"][branch]}
                });

                string response = null;
                do
                {
                    response = this.POST("INPC", new Dictionary<string, string>()
                    {
                        {"unpaidSearchTermsType[reminderSearchTermsType][searchTermsType][dates][dateStart]", start},
                        {"unpaidSearchTermsType[reminderSearchTermsType][searchTermsType][dates][dateEnd]", end}
                    });
                    Thread.Sleep(250);
                } while (!Convert.ToBoolean(JsonConvert.DeserializeObject<dynamic>(response).successful));
                
                ++VT_REQUESTS;

                Environment.SetEnvironmentVariable("VT_REQUESTS", VT_REQUESTS.ToString(), EnvironmentVariableTarget.Machine);
            }

            Console.WriteLine("Completed.");
        }

        public void DownloadSummaries(string path, int totalWeeks = 1, bool skipWait = false)
        {
            //DateTime now = DateTime.Now;
            taskStartTime = DateTime.Parse(Environment.GetEnvironmentVariable("EARLIEST_INVOICES", EnvironmentVariableTarget.Machine));
            Console.WriteLine("   Waiting for requests to complete...");

            string[] files = Directory.GetFiles(path).ToList().FindAll(f => !f.Contains("SPLIT - ") && f.Contains("billing-summary")).ToArray();
            int fileName = 0;
            if (files.Length > 0)
                fileName = files.Select(
                    f => Convert.ToInt32(
                        Regex.Match(
                            f, @"\\billing-summary(\d{1,2})\.pdf"
                        ).Groups[1].Value
                    )
                ).OrderBy(d => d).Last();
            
            VT_REQUESTS = Convert.ToInt32(Environment.GetEnvironmentVariable("VT_REQUESTS", EnvironmentVariableTarget.Machine));

            Zillow.TryWriteLineAsync(
                String.Format(
                    LOG_FORMAT,
                    DateTime.Now.ToString(LOG_DATE_FORMAT),
                    "DEBUG",
                    (skipWait ? "Skipping wait... " : "Waiting a bit... ") + totalWeeks + " weeks of documents. " + VT_REQUESTS + " document requests have been made."
                ), log
            );

            msgBoard.NotificationReceived += BillSumHandler;
            msgBoard.StartPolling();

            Zillow.TryWriteLineAsync(
                String.Format(
                    LOG_FORMAT,
                    DateTime.Now.ToString(LOG_DATE_FORMAT),
                    "DEBUG",
                    "All documents are ready. Waiting for downloads to complete..."
                ), log
            );

            SpinWait.SpinUntil(() => VT_REQUESTS == 0);
            Thread.Sleep(15000);
        }

        private void BillSumHandler(object sender, NotificationEventArgs e)
        {
            if (taskStartTime.CompareTo(e.date) <= 0 && e.type == "billing-summary")
            {
                Download(e);
            }
        }

        public void Download(NotificationEventArgs args)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(
                    async obj =>
                    {
                        
                        NotificationEventArgs data = (NotificationEventArgs)obj;
                        string path = TEMP_PATH + data.type +
                            Directory.GetFiles(TEMP_PATH, data.type + "*").Max(f => Convert.ToInt32(PPRGX.FILE_NUM.Match(f).Groups[1].Value)).ToString() +
                            downloadFileExts[data.type];

                        HttpWebRequest reqDown = this.defaultReq(data.url);
                        reqDown.Method = "GET";
                        reqDown.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                        reqDown.Referer = urls["msgBoard"];
                        reqDown.Headers.Add("X-DevTools-Request-Id", "13984.1177");
                        reqDown.Headers.Add("X-DevTools-Emulate-Network-Conditions-Client-Id", "fd3838d7-bd3b-4e12-a4f0-98f517252a8b");
                        reqDown.Headers.Add("Upgrade-Insecure-Requests", "1");
                        //reqDown.Proxy = null;

                        var resDown = reqDown.GetResponse();
                        Stream responseStream = resDown.GetResponseStream();
                        string output = null;

                        FileStream fs = new FileStream(
                                path, FileMode.CreateNew,
                                FileAccess.ReadWrite, FileShare.ReadWrite,
                                4096, FileOptions.Asynchronous
                            );
                        output = path;
                        await responseStream.CopyToAsync(fs);
                        await fs.FlushAsync();
                        fs.Dispose();
                        fs.Close();

                        /*if (!String.IsNullOrWhiteSpace(fileName))
                        {
                            
                        }
                        else
                        {
                            StreamReader sr = new StreamReader(responseStream);
                            output = sr.ReadToEnd();
                            sr.Dispose();
                            sr.Close();
                        }*/

                        responseStream.Flush();
                        responseStream.Dispose();
                        responseStream.Close();

                        msgBoard.MarkCompleted(data.dataID);

                        Zillow.TryWriteLineAsync(
                            String.Format(
                                LOG_FORMAT,
                                DateTime.Now.ToString(LOG_DATE_FORMAT),
                                "DOWNLOAD",
                                "Download completed ~ " + output
                            ), log
                        );

                        if (VT_REQUESTS > 0)
                            --VT_REQUESTS;
                        Environment.SetEnvironmentVariable("VT_REQUESTS", VT_REQUESTS.ToString());
                    }
                ), args
            );
        }

        private static void PDFRespCallback(IAsyncResult result)
        {
            //Thread.Sleep(20000);
            var state = (PDFData)result.AsyncState;
            HttpWebRequest req = state.request;
            HttpWebResponse res = (HttpWebResponse)req.EndGetResponse(result);
            state.response = res;
            state.contentLength = res.ContentLength;
            Stream stream = res.GetResponseStream();
            state.responseStream = stream;
            BinaryReader br = new BinaryReader(stream);
            //StreamReader sr = new StreamReader(stream);

            stream.BeginRead(state.buffer, 0, PDFData.BUFFER_SIZE, new AsyncCallback(PDFReadCallback), state);
        }

        private static void PDFReadCallback(IAsyncResult result)
        {
            var state = (PDFData)result.AsyncState;
            Stream stream = state.responseStream;
            //FileStream file = state.file;

            int read = stream.EndRead(result);
            if (read > 0 && state.bytesWritten < state.contentLength)
            {
                /*
                file.Write(
                    Encoding.ASCII.GetBytes(
                    HttpUtility.HtmlDecode(
                    Encoding.ASCII.GetString(state.buffer, 0, PDFData.BUFFER_SIZE)
                    )), 0, PDFData.BUFFER_SIZE);*/
                
                lock(state.bw)
                {
                    BinaryWriter bw = state.bw;
                    //FileStream file = state.file;
                    //StreamReader sr = state.sr;
                    //StreamWriter sw = new StreamWriter(file, sr.CurrentEncoding);
                    bw.Write(state.buffer, 0, PDFData.BUFFER_SIZE);
                }

                state.bytesWritten += read;

                //Thread.Sleep(250);
                stream.BeginRead(state.buffer, 0, PDFData.BUFFER_SIZE, new AsyncCallback(PDFReadCallback), state);
            }
            else
            {
                Debug.Assert(
                    state.bytesWritten == state.contentLength,
                    "ERROR: Content Length Mismatch",
                    "Response Content-Length = {0};\r\nBytes Written = {1};",
                    state.contentLength,
                    state.bytesWritten
                );

                state.file.Flush();
                state.file.Dispose();
                state.file.Close();
                stream.Flush();
                stream.Dispose();
                stream.Close();
                state.response.Dispose();
                state.response.Close();

                downed.Set();
            }
        }
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                    request.Abort();
            }
        }

        public static void SplitInvoices(string sourcePath, string destPath)
        {
            Random gen = new Random(DateTime.Now.Millisecond);

            string[] files = Directory.GetFiles(destPath, "*INPC*");
            int fileName = 0;
            if (files.Length > 0)
                fileName = files.ToList().FindAll(
                    f => Regex.IsMatch(f, @"INPC - (\d{1,5})\.pdf")
                ).Select(
                    f => Convert.ToInt32(
                        Regex.Match(
                            f, @"INPC - (\d{1,5})\.pdf"
                        ).Groups[1].Value
                    )
                ).OrderBy(d => d).Last();

            string[] billSums = Directory.GetFiles(
                sourcePath
            ).GroupBy(b => new FileInfo(b).Length)
            .ToList().FindAll(g => !g.Any(f => f.Contains("SPLIT - ")))
            .Select(g => g.First())
            .ToList().FindAll(
                b => !b.Contains("SPLIT") &&
                new FileInfo(b).Length > 5
            ).ToArray();

            Directory.GetFiles(
                sourcePath
            ).Except(billSums).ToList().ForEach(
                b => File.Delete(b)
            );

            for (int i = 0; i < billSums.Length; ++i)
            {
                string path = billSums[i];
                PdfReader pr = null;
                while (true)
                {
                    try
                    {
                        pr = new PdfReader(path);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(gen.Next(50, 150));
                    }
                }
                
                int count = pr.NumberOfPages;

                for (int j = 1; j <= count; ++j)
                {
                    ++fileName;
                    FileStream file = new FileStream(
                        destPath + "INPC - " + fileName.ToString() + ".pdf",
                        FileMode.Create
                    );
                    var doc = new iTextSharp.text.Document(
                        pr.GetPageSizeWithRotation(j));
                    var copy = new PdfCopy(doc, file);
                    doc.Open();

                    copy.AddPage(
                        copy.GetImportedPage(pr, j));
                    if (j < count && pr.GetPageContent(j + 1).Length < 27000) //20000 worked before
                    {
                        ++j;
                        copy.AddPage(copy.GetImportedPage(pr, j));
                    }

                    doc.Close();
                    file.Close();
                }

                pr.Close();
                File.Move(path, sourcePath + "SPLIT - " + new FileInfo(path).Name);
            }
        }
    }

    public class Card
    {
        public string accountNumber { get; set; }
        public string expiryMonth { get; set; }
        public string expiryYear { get; set; }

        public Card(string info)
        {
            Match acctMatch = Regex.Match(info, Regex.Escape("account[accountNumber]") + "\" class=\"input-medium required\" value=\"(.+?)\"");
            info = info.Substring(acctMatch.Index);
            this.accountNumber = acctMatch.Groups[1].Value;
            Match date = Regex.Match(info, "selected=\"selected\">(.+?)<");
            this.expiryMonth = date.Groups[1].Value;
            this.expiryYear = date.NextMatch().Groups[1].Value;
        }
    }

    public struct PDFData
    {
        public const int BUFFER_SIZE = 8192;
        public byte[] buffer;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream responseStream;
        //public readonly string tempPath = System.IO.Path.GetTempPath() + @"\VantageTracker\";
        public FileStream file;
        public string fileName;
        public StringBuilder sb;
        //public StreamReader sr;
        public BinaryWriter bw;
        public long contentLength;
        public long bytesWritten;

        public PDFData(string billSumsPath)
        {
            buffer = new byte[8192];
            request = null;
            response = null;
            responseStream = null;
            sb = new StringBuilder("");
            contentLength = 0;
            bytesWritten = 0;

            int num = 0;
            fileName = null;
            do
            {
                fileName = billSumsPath + "billing-summary" + (num > 0 ? num.ToString() : "") + ".pdf";
                ++num;
            } while (File.Exists(fileName));
            file = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 1024, FileOptions.Asynchronous);
            bw = new BinaryWriter(file);
            //sr = new StreamReader(file);
            //sw = new StreamWriter(file);
        }
    }

    public static class RequestExt
    {
        public static HttpWebRequest Clone(this HttpWebRequest req)
        {
            return req;
        }
    }

    public static class MatchExt
    {
        public static bool Equals(this Match x, object obj)
        {
            Match y = (Match)obj;
            return x.Groups.Cast<Group>().Select((e, i) => new { Item = e, Index = i }).Any(e => y.Groups[e.Index].Value == e.Item.Value);
        }

        public static int GetHashCode(this Match x)
        {
            return x.GetHashCode();
        }
    }

    class MatchCompare : IEqualityComparer<Match>
    {
        public bool Equals(Match x, Match y)
        {
            return x.Groups.Cast<Group>().Select((e, i) => new {Item = e, Index = i}).Any(e => y.Groups[e.Index].Value == e.Item.Value);
        }

        public int GetHashCode(Match obj)
        {
            return obj.GetHashCode();
        }
    }

    struct FileCompare : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return Math.Abs(new FileInfo(x).Length - new FileInfo(y).Length) < 1024;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    public static class DictionaryExtension
    {
        public static string ToFormURLEncoded(this Dictionary<string, string[]> dict, Dictionary<string, string[]> intersect = null)
        {
            if (intersect != null)
            {
                List<string> keys = dict.Keys.ToList();
                int keysCount = intersect.Keys.Count;
                for(int i = 0; i < keysCount; ++i)
                {
                    string key = intersect.Keys.ElementAt(i);
                    string encoded = HttpUtility.UrlEncode(key);
                    string[] encodedValue = intersect[key].Select(v => HttpUtility.UrlEncode(v)).ToArray();
                    key = null;
                    int index = keys.FindIndex(k => k.Contains(encoded));
                    if (index != -1)
                        dict[keys[index]] = encodedValue;
                    else
                        dict.Add(encoded, encodedValue);
                    encoded = null;
                    encodedValue = null;
                }
                intersect = null;
                keys = null;
            }

            int count = dict.Keys.Count;
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < count; ++i)
            {
                string key = dict.Keys.ElementAt(i);
                foreach(string value in dict[key])
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(value);
                    sb.Append("&");
                }
                key = null;
            }

            return sb.ToString(0, sb.Length - 1);
        }
    }
}

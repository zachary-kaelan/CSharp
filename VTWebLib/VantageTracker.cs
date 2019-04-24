using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Jil;
using PPLib;
using RestSharp;
using VTWebLib.Models;
using VTWebLib.Properties;
using ZachLib;
using ZachLib.HTTP;
using ZachLib.Logging;

namespace VTWebLib
{
    public enum VTFunctionType
    {
        Login,
        SwitchBranch,
        UpdateUserDetails,
        AddServiceType,
        EditServiceType,
        GetPestpacExportList,
        UpdateServiceAddress
    }

    public enum VTExportColumn
    {
        ID = 0,
        Customer,
        Office,
        Contract,
        LocationID,
        BillToID,
        ServiceOrder,
        ContractFile,
        ServiceSetup,
        CardID,
        Status,
        Function
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public static class VantageTracker
    {
        static VantageTracker()
        {
            LogManager.AddLog("VantageTracker", LogType.FolderFiles);
        }
        public static readonly Lazy<Dictionary<string, string>> BRANCH_ID_DICTIONARY = new Lazy<Dictionary<string, string>>(() => Utils.LoadDictionary(PATH_DICTIONARIES + "Branch-ID.txt"));
        public static readonly Lazy<Dictionary<string, string>> ID_BRANCH_DICTIONARY = new Lazy<Dictionary<string, string>>(() => Utils.LoadDictionary(PATH_DICTIONARIES + "ID-Branch.txt"));
        public static readonly Lazy<Dictionary<string, string>> BRANCH_IDS = new Lazy<Dictionary<string, string>>(() => Utils.LoadDictionary(PATH_DICTIONARIES + "Customers Branch IDs.txt"));
        public static readonly Lazy<Dictionary<string, int>> BRANCH_EXPORT_COUNTS = new Lazy<Dictionary<string, int>>(() => File.Exists(PATH_BRANCH_EXPORT_COUNTS) ? Utils.LoadJSON<Dictionary<string, int>>(PATH_BRANCH_EXPORT_COUNTS) : new Dictionary<string, int>());

        public static string CURRENT_BRANCH { get; private set; }
        private static string CURRENT_BRANCH_ID = "599";

        private static object PARALLEL_LOCK = new object();

        public static readonly char[] TRIM_CHARS = new char[] { '"', '\t', '\r', '\n', ' ' };

        public const string PATH_MAIN = @"E:\Work Programming\Insight Program Files\VantageTracker\";
        public const string PATH_DICTIONARIES = PATH_MAIN + @"Dictionaries\";
        public const string PATH_ALL_EXPORTS = PATH_MAIN + @"All Exports\";
        public const string PATH_FAILED_EXPORTS = PATH_MAIN + @"Failed Exports\";
        public const string PATH_FAILED_ADD_EXPORTS = PATH_MAIN + @"Failed Add Exports\";
        public const string PATH_NO_SERVICES = PATH_MAIN + @"NoServices\";
        public const string PATH_NO_SALES = PATH_MAIN + @"NoSales\";
        public const string PATH_TECHS_MISSING_USERNAMES = PATH_MAIN + @"Techs Missing Usernames\";
        public const string PATH_SALES_MISSING_USERNAMES = PATH_MAIN + @"Sales Missing Usernames\";
        public const string PATH_EMPLOYEES = PATH_MAIN + @"Employees\";
        public const string PATH_EXPORT_ERROR_MESSAGES = PATH_MAIN + @"Export Error Messages\";
        public const string PATH_USERNAME_ADD_FAILED = PATH_MAIN + @"Failed Add Username\";
        public const string PATH_BRANCH_EXPORT_COUNTS = PATH_MAIN + @"BranchExportCounts.txt";

        private static readonly Regex RGX_NAMES = new Regex(@"^(?<FirstName>[a-z]+) (?:(?<MiddleInitial>[a-z]\.?) )?(?:(?:and|[&\\\/]) (?<SpouseName>[a-z]+) )?(?<LastName>[a-z]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(10));

        private static readonly SortedDictionary<VTFunctionType, VTFunction> FUNCTIONS = new SortedDictionary<VTFunctionType, VTFunction>()
        {
            {
                VTFunctionType.Login,
                new VTFunction()
                {
                    TokenResource = "login",
                    TokenXPath = "/html/body/div[2]/div/div/div/div/div/form/fieldset/input",
                    FuncResource = "login_check",
                    FuncOutput = (request) =>
                    {
                        var response = POSTer.Execute(request);
                        GETer.Execute(new RestRequest("#", Method.GET));
                        try
                        {
                            var cookie = response.Cookies.First(c => c.Name == "PHPSESSID");
                            Settings.Default.PHPSESSID = cookie.Value;
                            Settings.Default.PHPSESSEXPIRES = cookie.Expires.ToLocalTime();
                            Settings.Default.Save();
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Response = response,
                                Content = err,
                                IsError = true
                            };
                        }
                        return new VTResponse()
                        {
                            Response = response,
                            Content = null,
                            IsError = false
                        };
                    },
                    POSTParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "form[username]",
                            Value = "zac.johnso",
                            Type = ParameterType.GetOrPost
                        },
                        new Parameter()
                        {
                            Name = "form[password]",
                            Value = "I15Zac$0208",
                            Type = ParameterType.GetOrPost
                        }
                    }
                }
            },
            {
                VTFunctionType.SwitchBranch,
                new VTFunction()
                {
                    TokenResource = "message-board",
                    TokenXPath = "/html/body/div[2]/div/div/div/ul/li[3]/ul/li[2]/form/div/input",
                    FuncResource = "branch-switch",
                    FuncOutput = (request) =>
                    {
                        var branchID = request.Parameters.First(p => p.Name == "office_select[office]").Value.ToString();
                        if (branchID == CURRENT_BRANCH_ID)
                            return new VTResponse()
                            {
                                Response = null,
                                Content = false,
                                IsError = false
                            };
                        var response = POSTer.Execute(request);
                        try
                        {
                            CURRENT_BRANCH = ID_BRANCH_DICTIONARY.Value[branchID];
                            CURRENT_BRANCH_ID = branchID;
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Response = response,
                                Content = err,
                                IsError = true
                            };
                        }
                        return new VTResponse()
                        {
                            Response = response,
                            Content = true,
                            IsError = false
                        };
                    },
                    POSTParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "text/javascript, */*; q=0.01",
                            Type = ParameterType.HttpHeader
                        }
                    }
                }
            },
            {
                VTFunctionType.UpdateUserDetails,
                new VTFunction()
                {
                    TokenResource = "user/{data-id}/edit",
                    TokenXPath = "/div[2]/form/div[last()]/input",
                    FuncResource = "user/{data-id}/update",
                    URLParams = ResourceURLSegments.Func | ResourceURLSegments.Token,
                    FuncOutput = (request) =>
                    {
                        var response = POSTer.Execute(request);
                        try
                        {
                            var status = JSON.Deserialize<VTFunctionStatus>(response.Content);
                            return new VTResponse()
                            {
                                Response = response,
                                Content = status,
                                IsError = false
                            };
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Response = response,
                                Content = err,
                                IsError = true
                            };
                        }
                    },
                    TokenResponseExtractor = (doc, request) =>
                    {
                        var nodes = doc.DocumentNode.SelectNodes(
                            "/div[2]/form/div/div/div//*[@id and @name]"
                        );
                        var parameterNames = request.Parameters.Select(p => p.Name).ToArray();
                        Array.Sort(parameterNames);
                        foreach(var node in nodes)
                        {
                            if (Array.BinarySearch(parameterNames, node.Name) < 0)
                                request.AddParameter(node.ToParameter());
                        }
                    },
                    GETParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "Referer",
                            Value = "https://myvantagetracker.com/users/",
                            Type = ParameterType.HttpHeader
                        },
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "*/*",
                            Type = ParameterType.HttpHeader
                        }
                    }
                }
            },
            {
                VTFunctionType.AddServiceType,
                new VTFunction()
                {
                    TokenResource = "pestpac-service-type/new",
                    TokenXPath = "/div[2]/div/form/div[last()]/input",
                    FuncResource = "/pestpac-service-type/",
                    URLParams = ResourceURLSegments.None,
                    FuncOutput = (request) =>
                    {
                        var response = POSTer.Execute(request);
                        try
                        {
                            var status = JSON.Deserialize<VTFunctionStatus>(response.Content);
                            return new VTResponse()
                            {
                                Response = response,
                                Content = status,
                                IsError = false
                            };
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Response = response,
                                Content = err,
                                IsError = true
                            };
                        }
                    },
                    TokenResponseExtractor = (doc, request) =>
                    {
                        var setupTypeIndex = request.Parameters.FindIndex(
                            p => p.Name == "pestpac_service[serviceType]"
                        );
                        var setupType = request.Parameters[setupTypeIndex].Value;
                        var node = doc.DocumentNode.SelectSingleNode(
                            "/div[2]/div/form/div[1]/div/div/select/option[text()='" + setupType + "']"
                        );
                        var id = node.GetAttributeValue("value", null);
                        request.Parameters[setupTypeIndex].Value = id;
                    },
                    GETParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "Referer",
                            Value = "https://myvantagetracker.com/pestpac-export-customer/settings",
                            Type = ParameterType.HttpHeader
                        },
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "*/*",
                            Type = ParameterType.HttpHeader
                        }
                    }
                }
            },
            {
                VTFunctionType.EditServiceType,
                new VTFunction()
                {
                    TokenResource = "pestpac-service-type/{data-id}/edit",
                    TokenXPath = "/div[2]/div/form/div[last()]/input",
                    FuncResource = "/pestpac-service-type/{data-id}/update",
                    URLParams = ResourceURLSegments.Func | ResourceURLSegments.Token,
                    FuncOutput = (request) =>
                    {
                        var response = POSTer.Execute(request);
                        try
                        {
                            var status = JSON.Deserialize<VTFunctionStatus>(response.Content);
                            return new VTResponse()
                            {
                                Content = status,
                                Response = response,
                                IsError = false
                            };
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Content = err,
                                Response = response,
                                IsError = true
                            };
                        }
                    },
                    TokenResponseExtractor = (doc, request) =>
                    {
                        var nodes = doc.DocumentNode.SelectNodes(
                            "/div[2]/div/form/div[position()<9]"
                        ).Select(n => n.SelectSingleNode("./div/div/select | ./div/div/input | ./div/div/div/input"));
                        var paramNames = request.Parameters.Select(p => p.Name).ToArray();
                        Array.Sort(paramNames);
                        foreach(var node in nodes)
                        {
                            string name = node.GetAttributeValue("name", "");
                            if(Array.BinarySearch(paramNames, name) < 0)
                                request.AddParameter(name, (node.Name == "select" ? node.SelectSingleNode("./option[@selected]") : node).GetAttributeValue("value", ""), ParameterType.GetOrPost);
                        }
                    },
                    GETParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "Referer",
                            Value = "https://myvantagetracker.com/pestpac-export-customer/settings",
                            Type = ParameterType.HttpHeader
                        },
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "*/*",
                            Type = ParameterType.HttpHeader
                        }
                    }
                }
            },
            {
                VTFunctionType.GetPestpacExportList,
                new VTFunction()
                {
                    TokenResource = "pestpac-export-customer/list-pp-customers",
                    FuncResource = "pestpac-export-customer/list-pp-customers",
                    TokenXPath = "/html/body/div[2]/div[2]/div[2]/div[2]/div/div/div[2]/div[1]/div/div/div[2]/div/form/div[2]/input",
                    URLParams = ResourceURLSegments.None,
                    FuncOutput = (request) =>
                    {
                        //request.AddParameter("pestpac_export_customer_filter[offices][]", CURRENT_BRANCH_ID, ParameterType.GetOrPost);
                        var response = POSTer.Execute(request);
                        try
                        {
                            /*HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(response.Content);
                            var tableBody = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[2]/div[2]/div[2]/div/div/div[2]/div[2]/table/tbody");
                            var rows = tableBody.SelectNodes("tr").Select(
                                r => new VTExportCust(r)
                            );
                            */
                            var list = JSON.Deserialize<VTExportList>(response.Content);
                            return new VTResponse()
                            {
                                Content = list,
                                IsError = false,
                                Response = response
                            };
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Content = err,
                                IsError = true,
                                Response = response
                            };
                        }
                    },
                    POSTParameters = new Dictionary<string, string>()
                    {
                        { "iColumns", "12" },
                        { "sColumns", ",,,,,,,,,,," },
                        { "mDataProp_0", "id" },
                        { "sSearch_0", "" },
                        { "bRegex_0", "false" },
                        { "bSearchable_0", "true" },
                        { "bSortable_0", "true" },
                        { "mDataProp_1", "customer" },
                        { "sSearch_1", "" },
                        { "bRegex_1", "false" },
                        { "bSearchable_1", "true" },
                        { "bSortable_1", "true" },
                        { "mDataProp_2", "office" },
                        { "sSearch_2", "" },
                        { "bRegex_2", "false" },
                        { "bSearchable_2", "true" },
                        { "bSortable_2", "true" },
                        { "mDataProp_3", "contract" },
                        { "sSearch_3", "" },
                        { "bRegex_3", "false" },
                        { "bSearchable_3", "true" },
                        { "bSortable_3", "true" },
                        { "mDataProp_4", "location_id" },
                        { "sSearch_4", "" },
                        { "bRegex_4", "false" },
                        { "bSearchable_4", "true" },
                        { "bSortable_4", "true" },
                        { "mDataProp_5", "bill_to_id" },
                        { "sSearch_5", "" },
                        { "bRegex_5", "false" },
                        { "bSearchable_5", "true" },
                        { "bSortable_5", "true" },
                        { "mDataProp_6", "service_order" },
                        { "sSearch_6", "" },
                        { "bRegex_6", "false" },
                        { "bSearchable_6", "true" },
                        { "bSortable_6", "true" },
                        { "mDataProp_7", "contract_file" },
                        { "sSearch_7", "" },
                        { "bRegex_7", "false" },
                        { "bSearchable_7", "true" },
                        { "bSortable_7", "true" },
                        { "mDataProp_8", "service_setup" },
                        { "sSearch_8", "" },
                        { "bRegex_8", "false" },
                        { "bSearchable_8", "true" },
                        { "bSortable_8", "true" },
                        { "mDataProp_9", "card_id" },
                        { "sSearch_9", "" },
                        { "bRegex_9", "false" },
                        { "bSearchable_9", "true" },
                        { "bSortable_9", "true" },
                        { "mDataProp_10", "status" },
                        { "sSearch_10", "" },
                        { "bRegex_10", "false" },
                        { "bSearchable_10", "true" },
                        { "bSortable_10", "true" },
                        { "mDataProp_11", "function" },
                        { "sSearch_11", "" },
                        { "bRegex_11", "false" },
                        { "bSearchable_11", "true" },
                        { "bSortable_11", "false" },
                        { "sSearch", "" },
                        { "bRegex", "false" }
                    }.ToParameters().ToArray()
                }
            },
            {
                VTFunctionType.UpdateServiceAddress,
                new VTFunction()
                {
                    TokenResource = "customer/{data-id}/service-address/edit",
                    TokenXPath = "div[2]/div/form/div[last()]/input",
                    FuncResource = "customer/{data-id}/service-address/update",
                    URLParams = ResourceURLSegments.Func | ResourceURLSegments.Token,
                    FuncOutput = (request) =>
                    {
                        var response = POSTer.Execute(request);
                        try
                        {
                            var status = JSON.Deserialize<VTFunctionStatus>(response.Content);
                            return new VTResponse()
                            {
                                Response = response,
                                Content = status,
                                IsError = false
                            };
                        }
                        catch (Exception err)
                        {
                            return new VTResponse()
                            {
                                Response = response,
                                Content = err,
                                IsError = true
                            };
                        }
                    },
                    TokenResponseExtractor = (doc, request) =>
                    {
                        var nodes = doc.DocumentNode.SelectNodes(
                            "/div[2]/div/form/div[position()<15]//*[@id and @name]"
                        );
                        var parameterNames = request.Parameters.Select(p => p.Name).ToArray();
                        Array.Sort(parameterNames);
                        foreach(var node in nodes)
                        {
                            if (Array.BinarySearch(parameterNames, node.Name) < 0)
                                request.AddParameter(node.ToParameter());
                        }
                    },
                    GETParameters = new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "*/*",
                            Type = ParameterType.HttpHeader
                        }
                    }
                }
            }
        };

        private static readonly Lazy<RestClient> _poster = new Lazy<RestClient>(() => InitializePOSTer());
        private static readonly Lazy<RestClient> _getter = new Lazy<RestClient>(() => InitializeGETer());
        public static RestClient POSTer { get => _poster.Value; }
        public static RestClient GETer { get => _getter.Value; }
        private static readonly CookieContainer COOKIES = new CookieContainer();

        public static bool SuppressErrors { get; set; }

        private static RestClient InitializeGETer()
        {
            RestClient client = new RestClient("https://myvantagetracker.com/");
            client.FollowRedirects = true;
            client.RemoveDefaultParameter("Accept");
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            client.CookieContainer = COOKIES;
            return client;
        }

        private static RestClient InitializePOSTer()
        {
            RestClient client = new RestClient("https://myvantagetracker.com/");
            client.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36";
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("Origin", "https://myvantagetracker.com");
            client.CookieContainer = COOKIES;
            return client;
        }

        #region PublicFunctions
        public static void Login(bool forceLogin = false)
        {
            var sessid = Settings.Default.PHPSESSID;
            var expires = Settings.Default.PHPSESSEXPIRES;
            var now = DateTime.Now;
            if (forceLogin || String.IsNullOrWhiteSpace(sessid) || expires.CompareTo(now) <= 0 || (expires - now).TotalHours < 2)
                ExecuteFunc(VTFunctionType.Login);
            else
                COOKIES.Add(new Cookie("PHPSESSID", sessid, "/", "myvantagetracker.com") { Expires = expires });
        }

        public static bool Login(string initialBranch, bool forceLogin = false)
        {
            Login(forceLogin);
            return SwitchBranch(initialBranch);
        }

        public static bool SwitchBranch(string branch)
        {
            string branchID = Char.IsDigit(branch[0]) ?
                branch :
                BRANCH_ID_DICTIONARY.Value[branch];
            return (bool)ExecuteFunc(
                VTFunctionType.SwitchBranch,
                new Parameter()
                {
                    Name = "office_select[office]",
                    Value = branchID,
                    Type = ParameterType.GetOrPost
                }
            ).Content;
        }

        public static SortedDictionary<string, string> SearchCustomers(string query) =>
            JSON.Deserialize<SortedDictionary<string, string>>(
                GETer.Execute(
                    new RestRequest("customer/search/suggestions", Method.GET).AddOrUpdateParameters(
                        new Parameter()
                        {
                            Name = "query",
                            Value = query,
                            Type = ParameterType.GetOrPost
                        },
                        new Parameter()
                        {
                            Name = "Accept",
                            Value = "application/json, text/javascript, */*; q=0.01",
                            Type = ParameterType.HttpHeader
                        }
                    )
                ).Content
            );

        public static VTServiceHistoryListModel[] GetCustomerServiceHistory(string id)
        {
            RestRequest request = new RestRequest("customer/" + id + "/service-history", Method.GET);
            HtmlDocument doc = new HtmlDocument();
            var response = GETer.Execute(request);
            doc.LoadHtml(response.Content);
            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[2]/div[2]/div[2]/div/div/div[2]/div[2]/div[2]/div/div/div[2]/div/table/tbody");
            if (node.ChildNodes.Count <= 1)
                return null;
            else
            {
                var nodes = node.SelectNodes("./tr");
                return nodes.Select(n => new VTServiceHistoryListModel(n)).ToArray();
            }
        }


        public static VTEmployee[] GetEmployees()
        {
            RestRequest request = new RestRequest("users", Method.GET);
            HtmlDocument doc = new HtmlDocument();
            var response = GETer.Execute(request);
            doc.LoadHtml(response.Content);
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div[2]/div[2]/div[2]/div[2]/div/div/table/tbody/tr");
            return nodes.Select(n => new VTEmployee(n)).ToArray();
        }

        public static VTResponse UpdatePestPacUsername(string userid, string username) => ExecuteFunc(
            VTFunctionType.UpdateUserDetails,
            new Parameter()
            {
                Name = "pocomos_user_profile[ppUsername]",
                Value = username,
                Type = ParameterType.GetOrPost
            },
            new Parameter()
            {
                Name = "data-id",
                Value = userid,
                Type = ParameterType.UrlSegment
            }
        );

        public static VTService[] GetVTServices()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(GETer.Execute(new RestRequest("pestpac-export-customer/settings", Method.GET)).Content);
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div[2]/div[2]/div[2]/div[2]/div/div/div[3]/div/div/div[2]/div/table/tbody/tr");
            return nodes == null ?
                new VTService[] { } :
                nodes.Select(
                    n => new VTService()
                    {
                        serviceId = n.SelectSingleNode("./td[1]").InnerText,
                        serviceType = n.SelectSingleNode("./td[2]").InnerText,
                        serviceSetupType = n.SelectSingleNode("./td[3]").InnerText,
                        serviceSetupColor = n.SelectSingleNode("./td[4]").InnerText,
                        serviceOrderType = n.SelectSingleNode("./td[5]").InnerText,
                        serviceOrderColor = n.SelectSingleNode("./td[6]").InnerText,
                        serviceSchedule = n.SelectSingleNode("./td[7]").InnerText
                    }
                ).ToArray();
        }

        public static VTResponse EditVTService(string id, params KeyValuePair<string, string>[] parameters) =>
            ExecuteFunc(
                VTFunctionType.EditServiceType,
                parameters.Select(
                    p => new Parameter()
                    {
                        Name = p.Key,
                        Value = p.Value,
                        Type = ParameterType.GetOrPost
                    }
                ).Append(
                    new Parameter()
                    {
                        Name = "data-id",
                        Value = id,
                        Type = ParameterType.UrlSegment
                    }
                ).ToArray()
            );

        public static VTResponse UpdateCustomerServiceAddress(string urlID, params KeyValuePair<string, string>[] parameters) =>
            ExecuteFunc(
                VTFunctionType.UpdateServiceAddress,
                parameters.Select(
                    p => new Parameter()
                    {
                        Name = p.Key,
                        Value = p.Value,
                        Type = ParameterType.GetOrPost
                    }
                ).Append(
                    new Parameter()
                    {
                        Name = "data-id",
                        Value = urlID,
                        Type = ParameterType.UrlSegment
                    }
                ).ToArray()
            );


        public static (CustomerServiceAddress, CustomerServiceInfo) GetServiceInfo(string urlID)
        {
            var response = GETer.Execute(
                new RestRequest(
                    "customer/" + urlID + "/service-information",
                    Method.GET
                )
            );

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response.Content);
            var node = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[2]/div[2]/div[2]/div/div/div[2]/div[2]");
            var serviceInfo = new CustomerServiceInfo(node.SelectSingleNode("./div[2]/div[2]/div/div/div[2]/div/div"));
            var serviceAddress = new CustomerServiceAddress(node.SelectSingleNode("./div[1]/div/div/div[2]/div/div"));
            return (serviceAddress, serviceInfo);
        }

        public static VTFunctionStatus AddCustomerToPPExport(string urlID) =>
            JSON.Deserialize<VTFunctionStatus>(
                POSTer.Execute(
                    new RestRequest(
                        "pestpac-export-customer/" + urlID + "/add",
                        Method.POST
                    )
                ).Content
            );

        public static void ChangeCustomerPPExportStatus(string ID, string newStatus) => POSTer.Execute(
                new RestRequest(
                    "pestpac-export-customer/" + ID + "/change-status/" + newStatus,
                    Method.POST
                )
            );

        public static VTExportCust[] GetFailedPestpacExports(string branch, int maxErrors)
        {
            SwitchBranch(branch);
            return GetPestpacExportsList(
                0, maxErrors, null, 
                new KeyValuePair<VTExportColumn, bool>(
                    VTExportColumn.Status, false
                ),
                new KeyValuePair<VTExportColumn, bool>(
                    VTExportColumn.ID, false
                )
            ).GetCustomers().Where(
                e => !e.OrderSucceeded ||
                e.Status == "Pending"
            ).ToArray();
        }

        public static VTExportCust[] GetFailedPestpacExports()
        {
            var allBranches = BRANCH_IDS.Value.Keys.ToArray();
            int newCount = GetPestpacExportsCount(allBranches, out int oldCount);
            int length = (newCount - oldCount) + Settings.Default.TotalFailed;
            return GetPestpacExportsList(
                0, length, allBranches, new KeyValuePair<VTExportColumn, bool>(
                    VTExportColumn.Status, false
                ), new KeyValuePair<VTExportColumn, bool>(
                    VTExportColumn.ID, false
                )
            ).GetCustomers().Where(
                e => !e.OrderSucceeded || 
                e.Status == "Pending"
            ).ToArray();
        }

        public static VTExportCust[] GetPestpacExports() => GetPestpacExports(0, 100);

        public static int GetPestpacExportsBranchCount(out int oldCount) => GetPestpacExportsBranchCount(null, out oldCount);

        public static int GetPestpacExportsCount(string[] branches, out int oldCount)
        {
            int newCount = GetPestpacExportsList(0, 1, branches).iTotalRecords;
            oldCount = Settings.Default.Count;
            if (oldCount == 0 || oldCount != newCount)
            {
                Settings.Default.Count = newCount;
                Settings.Default.Save();
            }
            return newCount;
        }

        public static int GetPestpacExportsBranchCount(string branch, out int oldCount)
        {
            if (branch != null)
                SwitchBranch(branch);
            int newCount = GetPestpacExportsList(0, 1, null).iTotalRecords;
            if (!BRANCH_EXPORT_COUNTS.Value.TryGetValue(CURRENT_BRANCH_ID, out oldCount))
                BRANCH_EXPORT_COUNTS.Value.Add(CURRENT_BRANCH_ID, newCount);
            if (oldCount == 0 || oldCount != newCount)
                BRANCH_EXPORT_COUNTS.Value.SaveAs(PATH_BRANCH_EXPORT_COUNTS, Options.PrettyPrint);
            return newCount;
        }

        private static VTExportCust[] GetPestpacExports(int start, int length, params KeyValuePair<VTExportColumn, bool>[] sortParams) => 
            GetPestpacExportsList(start, length, null, sortParams).GetCustomers();

        private static VTExportList GetPestpacExportsList(int start, int length, string[] offices, params KeyValuePair<VTExportColumn, bool>[] sortParams)
        {
            var parameters = sortParams.SelectMany(
                (p, i) => new Parameter[] {
                    new Parameter()
                    {
                        Name = "iSortCol_" + i,
                        Value = (int)p.Key,
                        Type = ParameterType.GetOrPost
                    },
                    new Parameter()
                    {
                        Name = "sSortDir_" + i,
                        Value = p.Value ? "desc" : "asc",
                        Type = ParameterType.GetOrPost
                    }
                }
            ).Concat(
                new Parameter[]
                {
                    new Parameter()
                    {
                        Name = "iDisplayStart",
                        Value = start,
                        Type = ParameterType.GetOrPost
                    },
                    new Parameter()
                    {
                        Name = "iDisplayLength",
                        Value = length,
                        Type = ParameterType.GetOrPost
                    },
                    new Parameter()
                    {
                        Name = "iSortingCols",
                        Value = sortParams.Length,
                        Type = ParameterType.GetOrPost
                    }
                }
            ).Concat(
                offices == null ?
                    new Parameter[]
                    {
                        new Parameter()
                        {
                            Name = "pestpac_export_customer_filter[offices][]",
                            Value = CURRENT_BRANCH_ID,
                            Type = ParameterType.GetOrPost
                        }
                    } : offices.Select(
                        o => new Parameter()
                        {
                            Name = "pestpac_export_customer_filter[offices][]",
                            Value = o,
                            Type = ParameterType.GetOrPost
                        }
                    )

            ).ToArray();
            
            var result = ExecuteFunc(VTFunctionType.GetPestpacExportList, parameters);
            return result.IsError || result.Content == null ? null : (VTExportList)result.Content;
        }

        public static VTExportDetails GetPestpacExportDetails(string id) => 
            new VTExportDetails(
                id, GETer.Execute(
                    new RestRequest(
                        "pestpac-export-customer/" + id + "/show", 
                        Method.GET
                    )
                ).Content
            );

        public static void TryExportAll(bool forceClean = false)
        {
            DateTime now = DateTime.Now;
            bool setLastClean = (now - Settings.Default.LastClean).TotalHours >= 12 || forceClean;
            if (setLastClean)
                Console.WriteLine("Doing a full sweep.\r\n");

            VTExportCust[] allFailed = null;
            int totalSucceeded = 0;
            Dictionary<string, int> successMessages = new Dictionary<string, int>();
            int totalNoError = 0;
            int totalFailed = 0;

            Console.Write("Fetching customer list... ");
            var tableRows = GetFailedPestpacExports();
            Console.WriteLine("Done");

            if (tableRows == null)
                Console.WriteLine("0 accounts to export...");
            else
            {
                var initialJobUnassigned = tableRows.Where(r => !String.IsNullOrWhiteSpace(r.ErrorMessage) && r.ErrorMessage.StartsWith("Initial Job")).ToArray();
                if (initialJobUnassigned.Any())
                {
                    Console.Write("Pausing {0} customers with routeless initial jobs... ", initialJobUnassigned.Length);
                    var tempLoop = Parallel.ForEach(
                        initialJobUnassigned,
                        new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = Math.Min(16, initialJobUnassigned.Length)
                        },
                        row =>
                        {
                            ChangeCustomerPPExportStatus(row.ID, "Paused");
                        }
                    );
                    SpinWait.SpinUntil(() => tempLoop.IsCompleted);
                    tableRows = tableRows.Where(r => !initialJobUnassigned.Any(e => e.URLID == r.URLID)).ToArray();
                    Console.WriteLine("Done");
                }

                VTExportCust[] prevFailedExports = null;

                string path = PATH_FAILED_EXPORTS + "AllBranches.txt";
                if (File.Exists(path))
                {
                    try
                    {
                        prevFailedExports = Utils.LoadJSON<VTExportCust[]>(path);
                    }
                    catch (Exception err)
                    {
                        prevFailedExports = Array.Empty<VTExportCust>();
                    }
                }
                else
                    setLastClean = true;

                int failed = -1;
                if (!setLastClean)
                {
                    failed = prevFailedExports.Length;
                    prevFailedExports = prevFailedExports.Where(e => !tableRows.Any(r => r.URLID == e.URLID)).ToArray();
                    totalFailed += prevFailedExports.Length;
                    failed -= prevFailedExports.Length;

                    if (failed > 0)
                        Console.WriteLine("{0} previously logged failures are now invalid and have been removed.", failed);

                    int rows = tableRows.Length;
                    tableRows = tableRows.Where(r => !prevFailedExports.Any(e => e.URLID == r.URLID)).ToArray();
                    rows -= tableRows.Length;
                    if (rows > 0)
                        Console.WriteLine("{0} exports have already been processed and will be skipped.", rows);
                }
                
                int updatesIter = 0;
                int updatesCount = tableRows.Length;
                Console.WriteLine("{0} accounts to export...", updatesCount);

                if (updatesCount > 0)
                {
                    var rowsLoop = Parallel.ForEach(tableRows, new ParallelOptions() { MaxDegreeOfParallelism = Math.Min(32, updatesCount) }, row =>
                    {
                        POSTer.Execute(new RestRequest("pestpac/test/" + row.ID, Method.POST));
                        lock (PARALLEL_LOCK)
                        {
                            ++updatesIter;
                            if (updatesIter % 25 == 0)
                                Console.WriteLine("{0} accounts completed...", updatesIter);
                        }
                    });
                    SpinWait.SpinUntil(() => rowsLoop.IsCompleted);

                    Console.WriteLine("FINISHED EXPORTS");
                    Console.Write("Checking for successes... ");
                    var failedTemp = GetFailedPestpacExports();
                    Console.WriteLine("Done");

                    bool isFailedTemp = failedTemp != null;
                    VTExportCust[] noErrorMessage = null;
                    bool isNoErrorMsg = false;
                    List<VTExportCust> failuresToLog = new List<VTExportCust>();

                    if (isFailedTemp)
                    {
                        noErrorMessage = failedTemp.Where(e => String.IsNullOrWhiteSpace(e.ErrorMessage)).ToArray();
                        if (noErrorMessage != null && noErrorMessage.Length > 0)
                        {
                            isNoErrorMsg = true;
                            failedTemp = failedTemp.Where(e => !String.IsNullOrWhiteSpace(e.ErrorMessage)).ToArray();
                            Console.WriteLine("{0} failed exports with no error message", noErrorMessage.Length);
                            totalNoError += noErrorMessage.Length;
                            failuresToLog.AddRange(noErrorMessage);
                        }

                        if (failedTemp.Length > 0)
                        {
                            Console.WriteLine("{0} failed exports with error message", failedTemp.Length);
                            totalFailed += failedTemp.Length;
                            failuresToLog.AddRange(failedTemp);
                        }
                        else
                            isFailedTemp = false;

                        if (tableRows.Any())
                            tableRows = tableRows.Where(r => !failuresToLog.Any(e => e.URLID == r.URLID) && !String.IsNullOrWhiteSpace(r.ErrorMessage)).ToArray();
                    }

                    if (!setLastClean)
                    {
                        failuresToLog.AddRange(prevFailedExports);
                        if (isFailedTemp)
                            prevFailedExports = prevFailedExports.Concat(failedTemp).ToArray();
                    }
                    else if (isFailedTemp)
                        prevFailedExports = failedTemp;

                    if (setLastClean || isFailedTemp || isNoErrorMsg || failed > 0)
                    {
                        Console.Write("Logging failures... ");
                        failuresToLog.SaveAs(path, Options.PrettyPrint);
                        Settings.Default.TotalFailed = failuresToLog.Count;
                        Settings.Default.Save();
                        Console.WriteLine("Done");
                    }


                    if (tableRows.Any())
                    {
                        totalSucceeded += tableRows.Length;
                        var msgCounts = tableRows.Select(r => r.ErrorMessage).GroupBy(m => m, m => "");
                        foreach (var msgCount in msgCounts)
                        {
                            if (successMessages.Any() && successMessages.TryGetValue(msgCount.Key, out int count))
                                successMessages[msgCount.Key] = count + msgCount.Count();
                            else
                                successMessages.Add(msgCount.Key, msgCount.Count());
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No exports that haven't already been processed.");
                    if (failed > 0)
                        prevFailedExports.SaveAs(path, Options.PrettyPrint);
                }

                allFailed = setLastClean ? 
                    prevFailedExports : 
                    prevFailedExports.Where(
                        e => !String.IsNullOrWhiteSpace(e.ErrorMessage)
                    ).ToArray();
            }

            Console.WriteLine();

            if (allFailed != null)
                allFailed.SaveAs(PATH_FAILED_EXPORTS + "AllBranches.txt", Options.PrettyPrint);

            if (setLastClean)
            {
                Settings.Default.LastClean = now;
                Settings.Default.Save();
            }

            Console.WriteLine("FINISHED ALL BRANCHES");
            Console.WriteLine("Total Failed: " + totalFailed);
            Console.WriteLine("Total Succeeded: " + totalSucceeded.ToString());
            Console.WriteLine();
            Console.WriteLine();
            if (successMessages.Any())
            {
                Console.WriteLine("Success Messages: ");
                foreach (var message in successMessages)
                {
                    string msg = message.Key;
                    if (msg.Contains(';'))
                        msg = String.Join("\r\n\t", msg.Split(';'));
                    Console.WriteLine("\t" + message.Value.ToString() + " - " + msg);
                    Console.WriteLine();
                }
            }

            Regex ErrorMessageTechRegex = new Regex("'([^']*)' ", RegexOptions.Compiled);
            Regex MaxFieldLengthRegex = new Regex(@"^The field (?<Field>[^ ]*) must be a string or array type with a maximum length of '(?<MaxLength>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var errorMessages = allFailed.GroupBy(
                e => String.IsNullOrWhiteSpace(e.ErrorMessage) ?
                    "" : (
                        e.ErrorMessage.StartsWith("Tech '") ?
                            ErrorMessageTechRegex.Replace(e.ErrorMessage, "") :
                            e.ErrorMessage
                        )
            ).Where(g => g.Key != "").OrderByDescending(g => g.Count());

            foreach (var errorMessage in errorMessages)
            {
                Console.WriteLine(errorMessage.Count() + " - " + errorMessage.Key);
                if (errorMessage.Key.StartsWith("Tech does not"))
                {
                    string path = PATH_EXPORT_ERROR_MESSAGES + "Tech does not exist.csv";
                    var techs = errorMessage.GroupBy(
                        e => e.ErrorMessage
                    ).OrderByDescending(t => t.Count());
                    List<VTMissingTechLogModel> models = new List<VTMissingTechLogModel>();
                    foreach (var tech in techs)
                    {
                        string techName = ErrorMessageTechRegex.Match(tech.Key).Groups[1].Value;
                        Console.WriteLine("\t{0} - {1}", tech.Count(), techName);
                        models.AddRange(tech.Select(e => new VTMissingTechLogModel(e, techName)));
                    }
                    models.SaveCSV(path, true);
                }
                else if (errorMessage.Key.StartsWith("The field"))
                {
                    var match = MaxFieldLengthRegex.Match(errorMessage.Key);
                    errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + match.Groups["Field"].Value + " - Max Length " + match.Groups["MaxLength"].Value + ".csv", true);
                }
                else if (errorMessage.Key.StartsWith("//"))
                    errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + "Document Size Error.csv", true);
                else
                    errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + errorMessage.Key + ".csv", true);
            }

            Console.WriteLine();
            Console.WriteLine("\t~~~");
            Console.WriteLine();
        }

        public static void TryExportAllBranches(bool forceClean = false)
        {
            Settings.Default.TotalFailed = 0;
            Settings.Default.Save();
            Stopwatch timer = Stopwatch.StartNew();
            DateTime now = DateTime.Now;
            int minutes = Convert.ToInt32((Settings.Default.PHPSESSEXPIRES - now).TotalMinutes);

            bool setLastClean = (now - Settings.Default.LastClean).TotalHours >= 12 || forceClean;
            if (setLastClean)
                Console.WriteLine("Doing a full sweep.\r\n");

            Console.Write("Fetching customer list... ");
            var branches = GetFailedPestpacExports().GroupBy(e => e.Office);
            Console.WriteLine("Done");
            List<VTExportCust> allBranches = new List<VTExportCust>();
            int totalSucceeded = 0;
            Dictionary<string, int> successMessages = new Dictionary<string, int>();
            int totalNoError = 0;
            foreach (var branch in branches)
            {
                SwitchBranch(branch.Key);
                string branchName = BRANCH_IDS.Value[CURRENT_BRANCH_ID];
                Console.WriteLine("Switched to " + branchName);
                var tableRows = branch.ToArray();

                if (tableRows == null)
                    Console.WriteLine("0 accounts to export...");
                else
                {
                    var initialJobUnassigned = tableRows.Where(r => !String.IsNullOrWhiteSpace(r.ErrorMessage) && r.ErrorMessage.StartsWith("Initial Job")).ToArray();
                    if (initialJobUnassigned.Any())
                    {
                        Console.Write("Pausing {0} customers with routeless initial jobs... ", initialJobUnassigned.Length);
                        var tempLoop = Parallel.ForEach(
                            initialJobUnassigned,
                            new ParallelOptions()
                            {
                                MaxDegreeOfParallelism = Math.Min(16, initialJobUnassigned.Length)
                            },
                            row =>
                            {
                                ChangeCustomerPPExportStatus(row.ID, "Paused");
                            }
                        );
                        SpinWait.SpinUntil(() => tempLoop.IsCompleted);
                        tableRows = tableRows.Where(r => !initialJobUnassigned.Any(e => e.URLID == r.URLID)).ToArray();
                        Console.WriteLine("Done");
                    }

                    VTExportCust[] prevFailedExports = null;

                    string path = PATH_FAILED_EXPORTS + branchName + ".txt";
                    bool branchClean = setLastClean;
                    if (File.Exists(path))
                    {
                        try
                        {
                            prevFailedExports = Utils.LoadJSON<VTExportCust[]>(path);
                        }
                        catch (Exception err)
                        {
                            prevFailedExports = Array.Empty<VTExportCust>();
                        }
                    }
                    else
                        branchClean = true;

                    int failed = -1;
                    if (!branchClean)
                    {
                        failed = prevFailedExports.Length;
                        prevFailedExports = prevFailedExports.Where(e => tableRows.Any(r => r.URLID == e.URLID)).ToArray();
                        Settings.Default.TotalFailed += prevFailedExports.Length;
                        failed -= prevFailedExports.Length;
                        
                        if (failed > 0)
                            Console.WriteLine("{0} previously logged failures are now invalid and have been removed.", failed);

                        int rows = tableRows.Length;
                        tableRows = tableRows.Where(r => !prevFailedExports.Any(e => e.URLID == r.URLID)).ToArray();
                        rows -= tableRows.Length;
                        if (rows > 0)
                            Console.WriteLine("{0} exports have already been processed and will be skipped.", rows);
                    }

                    /*var failedIds = File.ReadAllLines(path);
                    foreach (var id in failedIds)
                    {
                        if (tableRows.TryGetValue(id, out string message))
                        {
                            failedExports.Add(new KeyValuePair<string, string>(id, message));
                            tableRows.Remove(id);
                        }
                    }*/
                    int updatesIter = 0;
                    int updatesCount = tableRows.Length;
                    Console.WriteLine("{0} accounts to export...", updatesCount);

                    if (updatesCount > 0)
                    {
                        var rowsLoop = Parallel.ForEach(tableRows, new ParallelOptions() { MaxDegreeOfParallelism = Math.Min(32, updatesCount) }, row =>
                        {
                            POSTer.Execute(new RestRequest("pestpac/test/" + row.ID, Method.POST));
                            lock (PARALLEL_LOCK)
                            {
                                ++updatesIter;
                                if (updatesIter % 25 == 0)
                                {
                                    Console.WriteLine("{0} accounts completed...", updatesIter);
                                    if ((minutes - timer.Elapsed.TotalMinutes) >= 30)
                                    {
                                        Login();
                                        now = DateTime.Now;
                                        minutes = Convert.ToInt32((Settings.Default.PHPSESSEXPIRES - now).TotalMinutes);
                                    }
                                }
                            }
                        });
                        SpinWait.SpinUntil(() => rowsLoop.IsCompleted);

                        var failedTemp = GetFailedPestpacExports(branch.Key, updatesCount + prevFailedExports.Length);
                        bool isFailedTemp = failedTemp != null;
                        VTExportCust[] noErrorMessage = null;
                        bool isNoErrorMsg = false;
                        List<VTExportCust> failuresToLog = new List<VTExportCust>();

                        Console.WriteLine("FINISHED BRANCH");

                        if (isFailedTemp)
                        {
                            noErrorMessage = failedTemp.Where(e => String.IsNullOrWhiteSpace(e.ErrorMessage)).ToArray();
                            if (noErrorMessage != null && noErrorMessage.Length > 0)
                            {
                                isNoErrorMsg = true;
                                failedTemp = failedTemp.Where(e => !String.IsNullOrWhiteSpace(e.ErrorMessage)).ToArray();
                                Console.WriteLine("{0} failed exports with no error message", noErrorMessage.Length);
                                totalNoError += noErrorMessage.Length;
                                failuresToLog.AddRange(noErrorMessage);
                            }

                            if (failedTemp.Length > 0)
                            {
                                Console.WriteLine("{0} failed exports with error message", failedTemp.Length);
                                Settings.Default.TotalFailed += failedTemp.Length;
                                failuresToLog.AddRange(failedTemp);
                            }
                            else
                                isFailedTemp = false;

                            if (tableRows.Any())
                                tableRows = tableRows.Where(r => !failuresToLog.Any(e => e.URLID == r.URLID) && !String.IsNullOrWhiteSpace(r.ErrorMessage)).ToArray();
                        }

                        if (!branchClean)
                        {
                            failuresToLog.AddRange(prevFailedExports);
                            if (isFailedTemp)
                                prevFailedExports = prevFailedExports.Concat(failedTemp).ToArray();
                        }
                        else if (isFailedTemp)
                            prevFailedExports = failedTemp;

                        if (branchClean || isFailedTemp || isNoErrorMsg || failed > 0)
                        {
                            Console.Write("Logging failures... ");
                            failuresToLog.SaveAs(path, Options.PrettyPrint);
                            Console.WriteLine("Done");
                        }


                        if (tableRows.Any())
                        {
                            totalSucceeded += tableRows.Length;
                            var msgCounts = tableRows.Select(r => r.ErrorMessage).GroupBy(m => m, m => "");
                            foreach(var msgCount in msgCounts)
                            {
                                if (successMessages.Any() && successMessages.TryGetValue(msgCount.Key, out int count))
                                    successMessages[msgCount.Key] = count + msgCount.Count();
                                else
                                    successMessages.Add(msgCount.Key, msgCount.Count());
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No exports that haven't already been processed.");
                        if (failed > 0)
                            prevFailedExports.SaveAs(path, Options.PrettyPrint);
                    }

                    allBranches.AddRange(branchClean ? prevFailedExports : prevFailedExports.Where(e => !String.IsNullOrWhiteSpace(e.ErrorMessage)));
                }
                
                Console.WriteLine();
            }

            if (setLastClean)
            {
                Settings.Default.LastClean = now;
                Settings.Default.Save();
            }
            allBranches.SaveAs(PATH_FAILED_EXPORTS + "AllBranches.txt", Options.PrettyPrint);
            Console.WriteLine("FINISHED ALL BRANCHES");
            Console.WriteLine("Total Failed: " + Settings.Default.TotalFailed.ToString());
            Settings.Default.Save();
            Console.WriteLine("Total Succeeded: " + totalSucceeded.ToString());
            Console.WriteLine();
            Console.WriteLine();
            if (successMessages.Any())
            {
                Console.WriteLine("Success Messages: ");
                foreach (var message in successMessages)
                {
                    string msg = message.Key;
                    if (msg.Contains(';'))
                        msg = String.Join("\r\n\t", msg.Split(';'));
                    Console.WriteLine("\t" + message.Value.ToString() + " - " + msg);
                    Console.WriteLine();
                }
            }

            Regex ErrorMessageTechRegex = new Regex("'([^']*)' ", RegexOptions.Compiled);
            Regex MaxFieldLengthRegex = new Regex(@"^The field (?<Field>[^ ]*) must be a string or array type with a maximum length of '(?<MaxLength>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var errorMessages = allBranches.GroupBy(
                e => String.IsNullOrWhiteSpace(e.ErrorMessage) ? 
                    "" : (
                        e.ErrorMessage.StartsWith("Tech '") ? 
                            ErrorMessageTechRegex.Replace(e.ErrorMessage, "") :
                            e.ErrorMessage
                        )
            ).Where(g => g.Key != "").OrderByDescending(g => g.Count());

            foreach(var errorMessage in errorMessages)
            {
                try
                {
                    Console.WriteLine(errorMessage.Count() + " - " + errorMessage.Key);
                    if (errorMessage.Key.StartsWith("Tech does not"))
                    {
                        string path = PATH_EXPORT_ERROR_MESSAGES + "Tech does not exist.csv";
                        var techs = errorMessage.GroupBy(
                            e => e.ErrorMessage
                        ).OrderByDescending(t => t.Count());
                        List<VTMissingTechLogModel> models = new List<VTMissingTechLogModel>();
                        foreach (var tech in techs)
                        {
                            string techName = ErrorMessageTechRegex.Match(tech.Key).Groups[1].Value;
                            Console.WriteLine("\t{0} - {1}", tech.Count(), techName);
                            models.AddRange(tech.Select(e => new VTMissingTechLogModel(e, techName)));
                        }
                        models.SaveCSV(path, true);
                    }
                    else if (errorMessage.Key.StartsWith("The field"))
                    {
                        var match = MaxFieldLengthRegex.Match(errorMessage.Key);
                        errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + match.Groups["Field"].Value + " - Max Length " + match.Groups["MaxLength"].Value + ".csv", true);
                    }
                    else if (errorMessage.Key.StartsWith("//"))
                        errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + "Document Size Error.csv", true);
                    else
                        errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + errorMessage.Key + ".csv", true);
                }
                catch
                {
                    errorMessage.SaveCSV(PATH_EXPORT_ERROR_MESSAGES + ZachRGX.FILENAME_DISALLOWED_CHARACTERS.Replace(errorMessage.Key, "") + ".csv", true);
                }
            }

            Console.WriteLine();
            Console.WriteLine("\t~~~");
            Console.WriteLine();
        }

        public static void FixVTTechPPUsernames()
        {
            var offices = Utils.LoadJSON<VTExportCust[]>(
                PATH_FAILED_EXPORTS + "AllBranches.txt"
            ).GroupBy(e => e.Office);
            
            foreach (var office in offices)
            {
                List<VTExportCust> failedAgain = new List<VTExportCust>();
                VTExportCust[] failedExports = office.Where(
                    e => !String.IsNullOrWhiteSpace(e.ErrorMessage)
                ).ToArray();

                SwitchBranch(office.Key);
                var branch = BRANCH_IDS.Value[CURRENT_BRANCH_ID];
                Console.WriteLine(branch + " - " + failedExports.Length + " total failed");

                var fixableExports = failedExports.Select(
                    e =>
                    {
                        if (e.ErrorMessage.EndsWith("Technician PestPac Username"))
                            e.ErrorData = "T";
                        else if (e.ErrorMessage.EndsWith("Salesperson PestPac Username"))
                            e.ErrorData = "fS";
                        else if (e.ErrorMessage.StartsWith("The field Address"))
                            e.ErrorData = "fA";
                        else if (e.ErrorMessage.StartsWith("The field FirstName"))
                            e.ErrorData = "fFN";
                        else
                            e.ErrorData = "0";
                        return e;
                    }
                ).Where(e => e.ErrorData != "0").ToArray();

                Console.Write("Loading previous logs... ");
                string branchExt = branch + ".txt";
                var branchNoServsPath = PATH_NO_SERVICES + branchExt;
                var branchNoSalesPath = PATH_NO_SALES + branchExt;
                var branchTechsPath = PATH_TECHS_MISSING_USERNAMES + branchExt;
                var branchSalesPath = PATH_SALES_MISSING_USERNAMES + branchExt;

                List<string> noServices = File.Exists(branchNoServsPath) ? 
                    File.ReadAllLines(branchNoServsPath).ToList() : 
                    new List<string>();
                noServices.Sort();
                List<string> noSales = File.Exists(branchNoSalesPath) ?
                    File.ReadAllLines(branchNoSalesPath).ToList() :
                    new List<string>();
                noSales.Sort();
                List<string> techNames = File.Exists(branchTechsPath) ? 
                    File.ReadAllLines(branchTechsPath).ToList() : 
                    new List<string>();
                List<string> salesNames = File.Exists(branchSalesPath) ? 
                    File.ReadAllLines(branchSalesPath).ToList() : 
                    new List<string>();
                Console.WriteLine("DONE");

                fixableExports = fixableExports.Where(
                    e => noServices.BinarySearch(e.URLID) < 0 &&
                         noSales.BinarySearch(e.URLID) < 0
                ).ToArray();
                Console.WriteLine("{0} fixable exports", fixableExports.Length);

                List<KeyValuePair<string, VTExportCust>> customerTechsUngrouped = new List<KeyValuePair<string, VTExportCust>>();
                List<KeyValuePair<string, VTExportCust>> customerSalesUngrouped = new List<KeyValuePair<string, VTExportCust>>();

                Console.Write("Fetching customers' information... ");
                foreach (var customer in fixableExports)
                {
                    if (customer.ErrorData == "T")
                    {
                        var history = GetCustomerServiceHistory(customer.URLID);
                        if (history == null || !history.Any())
                            noServices.Add(customer.URLID);
                        else
                        {
                            string technician = history.First().Technician;
                            if (history.Length > 1 && !history.Skip(1).All(s => s.Technician == technician))
                                technician = history.OrderByDescending(s => s.Date).First().Technician;
                            if (!techNames.Contains(technician))
                                techNames.Add(technician);
                            customerTechsUngrouped.Add(new KeyValuePair<string, VTExportCust>(technician, customer));
                        }
                    }
                    else if (customer.ErrorData.StartsWith("f"))
                    {
                        (var serviceAddress, var serviceInfo) = GetServiceInfo(customer.URLID);
                        if (customer.ErrorData.EndsWith("S"))
                        {
                            string salesPerson = serviceInfo.Salesperson.Trim(' ', '\r', '\n', '\t', '-');
                            if (salesPerson.Length <= 3)
                                noSales.Add(customer.URLID);
                            else
                            {
                                salesNames.Add(salesPerson);
                                customerSalesUngrouped.Add(new KeyValuePair<string, VTExportCust>(salesPerson, customer));
                            }
                        }
                        else if (customer.ErrorData.EndsWith("FN"))
                        {
                            string name = HttpUtility.HtmlDecode(serviceAddress.Name);
                            var nameComponents = RGX_NAMES.ToDictionary(name);
                            string newFirstName = nameComponents["FirstName"];
                            var result = UpdateCustomerServiceAddress(
                                customer.URLID,
                                new KeyValuePair<string, string>(
                                    "service_address[firstName]", 
                                    newFirstName
                                )
                            );
                            Console.WriteLine("\t{0} - \"{1}\" changed to \"{2}\"", customer.URLID, name, newFirstName + " " + nameComponents["LastName"]);
                            POSTer.Execute(new RestRequest("pestpac/test/" + customer.ID, Method.POST));
                        }
                        else if (customer.ErrorData.EndsWith("A"))
                        {
                            string address = serviceAddress.AddressLine1;
                            string[] addressComponents = address.Split(' ').Select(
                                c => MiscDictionaries.ADDRESS_ABBRVS.TryGetValue(
                                    c.ToUpper(), 
                                    out string abbrv
                                ) ? abbrv : c
                            ).ToArray();
                            string newAddress = String.Join(" ", addressComponents);

                            var result = UpdateCustomerServiceAddress(
                                customer.URLID,
                                new KeyValuePair<string, string>(
                                    "service_address[contactAddress][street]",
                                    newAddress
                                )
                            );
                            Console.WriteLine("\t{0} - \"{1}\" changed to \"{2}\"", customer.URLID, address, newAddress);
                            POSTer.Execute(new RestRequest("pestpac/test/" + customer.ID, Method.POST));
                        }
                    }
                }
                Console.WriteLine("DONE");

                int noServicesLength = noServices.Count;
                noServices = noServices.Distinct().ToList();
                noServicesLength -= noServices.Count;
                Console.WriteLine("Accounts with no service: " + noServicesLength.ToString());
                File.WriteAllLines(PATH_NO_SERVICES + branchExt, noServices);

                string employeesPath = PATH_EMPLOYEES + branch + ".txt";
                VTEmployee[] branchEmployees = null;
                Dictionary<string, VTEmployee[]> employeesByType = null;

                Console.Write("Getting employees list... ");
                if (!File.Exists(employeesPath))
                {
                    branchEmployees = GetEmployees();
                    //branchEmployees.SaveAs(employeesPath);
                    employeesByType = branchEmployees.GroupBy(e => e.UserType).ToDictionary(g => g.Key, g => g.ToArray());
                    employeesByType.SaveAs(employeesPath);
                    branchEmployees = null;
                }
                else
                    employeesByType = Utils.LoadJSON<Dictionary<string, VTEmployee[]>>(employeesPath);
                Console.WriteLine("DONE");

                var techs = customerTechsUngrouped.GroupBy(c => c.Key, c => c.Value);
                Console.WriteLine("Accounts missing a tech: " + customerTechsUngrouped.Count);
                Console.WriteLine("\tNumber of techs: " + techNames.Count.ToString());
                File.WriteAllLines(PATH_TECHS_MISSING_USERNAMES + branchExt, techNames);

                if (techNames.Any())
                {
                    var branchTechs = new SortedDictionary<string, VTEmployee>(
                        employeesByType["Technician"].GroupBy(t => t.Name).Where(g => g.Count() == 1).Select(g => g.First()).ToDictionary(
                            t => t.Name,
                            t => t
                        )
                    );

                    foreach (var techGroup in techs)
                    {
                        object result = null;
                        Console.Write(techGroup.Key + " ... ");
                        if (branchTechs.TryGetValue(techGroup.Key, out VTEmployee tech))
                        {
                            result = UpdatePestPacUsername(tech.ID, tech.Username);
                            Console.WriteLine("Succeeded");
                            foreach (var customer in techGroup)
                            {
                                POSTer.Execute(new RestRequest("pestpac/test/" + customer.ID, Method.POST));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed");
                            foreach(var cust in techGroup)
                            {
                                var custTemp = cust;
                                custTemp.ErrorData = techGroup.Key;
                                failedAgain.Add(custTemp);
                            }
                        }
                    }
                }

                var sales = customerSalesUngrouped.GroupBy(c => c.Key, c => c.Value);
                Console.WriteLine("Accounts missing a salesperson: " + customerSalesUngrouped.Count);
                Console.WriteLine("\tNumber of salespeople: " + salesNames.Count.ToString());
                File.WriteAllLines(PATH_SALES_MISSING_USERNAMES + branchExt, salesNames);

                if (salesNames.Any())
                {
                    var branchSales = new SortedDictionary<string, VTEmployee>(
                        employeesByType["Salesperson"].GroupBy(s => s.Name).Where(
                            g => g.Count() == 1
                        ).Select(g => g.First()).ToDictionary(s => s.Name, s => s)
                    );

                    foreach(var salesGroup in sales)
                    {
                        object result = null;
                        Console.Write(salesGroup.Key + " ... ");
                        if (branchSales.TryGetValue(salesGroup.Key, out VTEmployee salesperson))
                        {
                            result = UpdatePestPacUsername(salesperson.ID, salesperson.Username);
                            Console.WriteLine("Succeeded");
                            foreach (var customer in salesGroup)
                            {
                                POSTer.Execute(new RestRequest("pestpac/test/" + customer.ID, Method.POST));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed");
                            foreach (var cust in salesGroup)
                            {
                                var custTemp = cust;
                                custTemp.ErrorData = salesGroup.Key;
                                failedAgain.Add(custTemp);
                            }
                        }
                    }
                }

                failedAgain.SaveCSV(PATH_USERNAME_ADD_FAILED + branch + ".csv");

                Console.WriteLine();
                Console.WriteLine("\t~~~");
                Console.WriteLine();
            }
        }

        private static readonly Dictionary<string, PropertyInfo> VT_SERVICE_PROPS = typeof(VTService).GetProperties().ToDictionary(
            p => String.Format("pestpac_service[{0}]", p.Name),
            p => p
        );

        public static object AddVTServiceType(VTService service) => ExecuteFunc(
            VTFunctionType.AddServiceType,
            VT_SERVICE_PROPS.Select(
                p => new Parameter()
                {
                    Name = p.Key,
                    Value = p.Value.GetValue(service),
                    Type = ParameterType.GetOrPost
                }
            ).Append(
                new Parameter()
                {
                    Name = "pestpac_service[enabled]",
                    Value = "1",
                    Type = ParameterType.GetOrPost
                }
            ).ToArray()
        );
        #endregion

        #region VTFunctions
        private static (HtmlDocument, RestRequest) InitializeRequest(VTFunction func, Parameter[] parameters)
        {
            var tokenRequest = new RestRequest(
                func.TokenResource,
                Method.GET
            );
            var funcRequest = new RestRequest(func.FuncResource, Method.POST);
            if (parameters.Any())
            {
                var body = parameters.Where(p => p.Type == ParameterType.GetOrPost);
                var nonBody = parameters.Except(body);
                funcRequest.AddParameters(body);
                if (func.URLParams.HasFlag(ResourceURLSegments.Token) | func.URLParams.HasFlag(ResourceURLSegments.Func))
                {
                    var urlSegments = nonBody.Where(p => p.Type == ParameterType.UrlSegment);
                    if (func.URLParams.HasFlag(ResourceURLSegments.Func))
                        funcRequest.AddParameters(urlSegments);
                    if (func.URLParams.HasFlag(ResourceURLSegments.Token))
                        tokenRequest.AddParameters(urlSegments);
                }
                parameters = body.ToArray();
            }

            var doc = new HtmlDocument();
            if (func.GETParameters != null && func.GETParameters.Any())
                tokenRequest.AddParameters(func.GETParameters);

            var tokenResponse = GETer.Execute(tokenRequest);
            doc.LoadHtml(tokenResponse.Content);
            HtmlNode tokenNode = null;
            try
            {
                tokenNode = doc.DocumentNode.SelectSingleNode(func.TokenXPath);
                funcRequest.AddParameter(tokenNode.ToParameter());
            }
            catch
            {
                if (tokenResponse.ResponseUri.ToString().EndsWith("login"))
                    Login(true);
                else
                    Utils.DoNothing();
                tokenResponse = GETer.Execute(tokenRequest);
                doc.LoadHtml(tokenResponse.Content);
                tokenNode = doc.DocumentNode.SelectSingleNode(func.TokenXPath);
                funcRequest.AddParameter(tokenNode.ToParameter());
            }

            if (func.POSTParameters != null && func.POSTParameters.Any())
                funcRequest.AddParameters(func.POSTParameters);
            funcRequest.AddHeader("Referer", tokenResponse.ResponseUri.ToString());

            return (doc, funcRequest);
        }

        private static VTResponse ExecuteRequest(RestRequest funcRequest, VTFunctionType funcType, VTFunction func, bool recursive = false)
        {
            VTResponse response = func.FuncOutput(funcRequest);
            var loginError = response.Response == null || response.Response.ResponseUri.ToString().EndsWith("login");
            if (loginError)
                Login(true);

            if (response.IsError && response.Content != null)
            {
                var error = (Exception)response.Content;
                LogManager.Enqueue(
                    "VantageTracker",
                    funcType.ToString(),
                    true,
                    funcRequest,
                    error
                );
                if (!SuppressErrors)
                    throw error;
            }

            if (!recursive && (response.IsError || loginError))
                return ExecuteRequest(funcRequest, funcType, func, true);

            return response;
        }

        private static VTResponse ExecuteFunc(VTFunctionType funcType, params Parameter[] parameters)
        {
            var func = FUNCTIONS[funcType];
            (HtmlDocument doc, RestRequest funcRequest) = InitializeRequest(func, parameters);
            func.TokenResponseExtractor?.Invoke(doc, funcRequest);
            return ExecuteRequest(funcRequest, funcType, func);
        }
        #endregion

        #region SubObjects
        [Flags]
        private enum ResourceURLSegments
        {
            None = 0,
            Token = 1,
            Func = 2
        }

        private struct VTFunction
        {
            public string TokenResource { get; set; }
            public string FuncResource { get; set; }
            public string TokenXPath { get; set; }
            public Func<RestRequest, VTResponse> FuncOutput { get; set; }
            public Parameter[] POSTParameters { get; set; }
            public Parameter[] GETParameters { get; set; }
            public ResourceURLSegments URLParams { get; set; }
            public Action<HtmlDocument, RestRequest> TokenResponseExtractor { get; set; }
        }
        #endregion
    }
}

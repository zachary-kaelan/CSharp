using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Expressions;
using CsvHelper.Configuration;
//using PPLib;
using RestSharp;
using Jil;

namespace PPAPIOptimized
{
    public enum Period
    {
        None,
        Week,
        Last7Days,
        Month,
        Last30Days,
        Year,
        LastYear
    };

    public class PidjClient
    {
        private const string PIDJ_PATH = @"E:\Pidj\";
        private const string CONTACTS_PATH = PIDJ_PATH + "Contacts.txt";
        //private static readonly RGX.Pidj.Contacts PIDJ_CONTACTS = new RGX.Pidj.Contacts();

        private static CookieContainer cookies = new CookieContainer();
        //private LogManager Logger { get; set; }
        private RestClient client { get; set; }
        private Postman ppClient { get; set; }
        private static readonly string[] periods = new string[] { "", "week", "7", "month", "30", "year", "365" };
        public Dictionary<string, PidjContact> Contacts { get; set; }

        public PidjClient(Postman postman)
        {
            if (!Directory.Exists(PIDJ_PATH))
                Directory.CreateDirectory(PIDJ_PATH);
            ppClient = postman;

            client = new RestClient("https://www.gopidj.com/");
            client.CookieContainer = cookies;
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.FollowRedirects = true;

            RestRequest request = new RestRequest("login", Method.POST);
            string token = PPLib.PPRGX.TOKEN.Match(
                    client.Execute(
                        new RestRequest(
                            "login",
                            Method.GET
                        )
                    ).Content
                ).Groups["Token"].Value;
            request.AddParameter(
                "_token",
                token,
                ParameterType.GetOrPost
            );
            request.AddParameter("login_company", "insight", ParameterType.GetOrPost);
            request.AddParameter("login_username", "j.allen@insightpest.com", ParameterType.GetOrPost);
            request.AddParameter("login_password", "jeff123", ParameterType.GetOrPost);
            request.AddParameter("redirect", "", ParameterType.GetOrPost);
            client.Execute(request);
            request = null;
            client.AddDefaultHeader("X-CSRF-TOKEN", token);
            token = null;

            /*if (!File.Exists(CONTACTS_PATH))
                this.ReloadContacts();
            else
                this.Contacts = JSON.Deserialize<Dictionary<string, PidjContact>>(File.ReadAllText(CONTACTS_PATH));*/
        }

        public void Debug()
        {
            RestRequest request = new RestRequest("contacts/pagination", Method.POST);
            request.AddParameter("p", "1", ParameterType.GetOrPost);
            request.AddParameter("s", "1000", ParameterType.GetOrPost);
            request.AddParameter("f", "", ParameterType.GetOrPost);
            request.AddParameter("o", "name", ParameterType.GetOrPost);
            request.AddParameter("d", "asc", ParameterType.GetOrPost);

            List<PidjContact> temp = new List<PidjContact>();

            for (int i = 0; i < 16; ++i)
            {
                temp.AddRange(
                    PPLib.PPRGX.PIDJ_CONTACTS.ToObjects<PidjContact>(
                        JSON.Deserialize<PidjPagination>(
                            client.Execute(request).Content
                        ).html
                    )
                );
            }

            var contactCodes = temp.GroupBy(c => c.Name, c => c.Code);
            var codes = temp.Select(c => c.Code);
            string codeTemp = codes.First();
            codes = codes.Skip(1);
            temp = null;

            string all = String.Join(
                "",
                Enumerable.Range(0, 192).Select(
                    i => Convert.ToInt32(
                        codes.All(c => c[i] == codeTemp[i])
                    ).ToString()
                )
            );

            Form1.lblQueue.Enqueue(new Form1.LabelUpdate("Bits for all: " + all));

            string[] individual = contactCodes.Select(
                contact =>
                {
                    codeTemp = contact.First();
                    string[] codesTemp = contact.Skip(1).ToArray();

                    string indTemp = "";
                    string str = String.Join(
                        "",
                        Enumerable.Range(0, 192).Select(
                            i => {
                                if (codesTemp.All(c => c[i] == codeTemp[i]))
                                {
                                    indTemp += codeTemp[i];
                                    return "1";
                                }
                                else
                                {
                                    indTemp += "0";
                                    return "0";
                                }
                            }
                        )
                    );
                    Form1.lstQueue.Enqueue("Bytes for " + contact.Key + ": " + indTemp);

                    return str;
                }
            ).ToArray();

            string allIndividual = String.Join(
                "",
                Enumerable.Range(0, 192).Select(
                    i => Convert.ToInt32(
                        all[i] != 1 && individual.All(ind => ind[i] == 1)
                    )
                )
            );

            Form1.lstQueue.Enqueue("All Individual: " + allIndividual);
        }

        #region GetTokens
        public IEnumerable<PidjToken> GetTokens(RestRequest request)
        {
            return PPLib.PPRGX.PIDJ_TOKENS.ToObjects<PidjToken>(
                client.Execute(
                    request
                ).Content
            );
        }
        
        public IEnumerable<PidjToken> GetTokens(string url, Method method = Method.GET)
        {
            return this.GetTokens(new RestRequest(url, method));
        }

        public IEnumerable<PidjToken> GetTokens(RestRequest request, string name)
        {
            return this.GetTokens(request).Where(t => t.Name == name);
        }

        public IEnumerable<PidjToken> GetTokens(string url, string name, Method method = Method.GET)
        {
            return this.GetTokens(url, method).Where(t => t.Name == name);
        }
#endregion

        public List<PidjContact> GetContacts(string query = "")
        {
            RestRequest request = new RestRequest("contacts/pagination", Method.POST);
            request.AddParameter("p", "1", ParameterType.GetOrPost);
            request.AddParameter("s", "1000", ParameterType.GetOrPost);
            request.AddParameter("f", query, ParameterType.GetOrPost);
            request.AddParameter("o", "name", ParameterType.GetOrPost);
            request.AddParameter("d", "asc", ParameterType.GetOrPost);

            int page = 1;
            PidjPagination pagination = new PidjPagination();
            List<PidjContact> contacts = new List<PidjContact>();

            do
            {
                pagination = JSON.Deserialize<PidjPagination>(client.Execute(request).Content);
                request.AddParameter("p", page++.ToString(), ParameterType.GetOrPost);
                contacts.AddRange(
                    PPLib.PPRGX.PIDJ_CONTACTS.ToObjects<PidjContact>(
                        pagination.html
                    )
                );
            } while (pagination.details.end_record_number < pagination.details.total_count);
            pagination = new PidjPagination();
            request = null;

            //this.Contacts.SaveAs(CONTACTS_PATH);
            return contacts;
        }

        public string FormatConversation(IEnumerable<PidjReportModel> messages, string tech)
        {
            StringBuilder sb = new StringBuilder("Conversation with ");
            sb.AppendLine(tech);
            sb.Append("\r\n");

            messages = messages.OrderBy(m => m.Date);
            foreach(PidjReportModel message in messages)
            {
                sb.AppendFormat("[{0}] ", message.Date.ToShortTimeString());
                sb.Append(message.Direction == "in" ? "Customer:\t" : "Tech:\t\t");
                sb.AppendLine(message.Message);
            }
            messages = null;
            return sb.ToString();
        }

#region Export
        public PidjReportModel[] Export()
        {
            return this.Export("", "", "", "", "");
        }

        public PidjReportModel[] Export(Period period)
        {
            return this.Export(period, "", "");
        }

        public PidjReportModel[] Export(Period period, string to, string from)
        {
            return this.Export(periods[(int)period], "", "", to, from);
        }

        public PidjReportModel[] Export(DateTime start, DateTime end)
        {
            return this.Export(start, end, "", "");
        }

        public PidjReportModel[] Export(DateTime start, DateTime end, string to, string from)
        {
            return this.Export("", start.ToString("s"), end.ToString("e"), to, from);
        }

        public PidjReportModel[] Export(string period, string start, string end, string to, string from)
        {
            RestRequest request = new RestRequest("report/deliverability", Method.POST);
            request.AddParameter(
                "_token",
                PPLib.PPRGX.TOKEN.Match(
                    client.Execute(
                        new RestRequest(
                            "report/deliverability",
                            Method.GET
                        )
                    ).Content
                ).Groups["Token"],
                ParameterType.GetOrPost
            );
            request.AddParameter("period", period, ParameterType.GetOrPost);
            request.AddParameter("tz", "America/New_York", ParameterType.GetOrPost);
            request.AddParameter("s", start, ParameterType.GetOrPost);
            request.AddParameter("e", end, ParameterType.GetOrPost);
            request.AddParameter("t", to, ParameterType.GetOrPost);
            request.AddParameter("f", from, ParameterType.GetOrPost);
            string text = client.Execute(request).Content;
            request = null;

            RestRequest export = new RestRequest(Method.POST);
            export.AddParameter(
                "_token",
                PPLib.PPRGX.TOKEN.Match(text).Groups["Token"],
                ParameterType.GetOrPost
            );

            export.Resource = "report/export/" + PPLib.PPRGX.URL_CODE.Match(text).Groups["Code"];
            text = null;

            using (StringReader str = new StringReader(client.Execute(export).Content))
            {
                using (CsvReader cr = new CsvReader(str, PPLib.Utils.csvConfig))
                {
                    return cr.GetRecords<PidjReportModel>().ToArray();
                }
            }
        }
#endregion
    }

    public struct PidjReportModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public string CodeError { get; set; }
    }

    public struct SearchResults
    {
        public string status { get; set; }
        public string matches { get; set; }
        public string html { get; set; }
        public string id { get; set; }
        public string message { get; set; }
    }

    public interface IPidjResponse
    {
        string code { get; set; }
        string status { get; set; }
        string message { get; set; }
    }

    public interface IPidjHtmlResponse
    {
        string html { get; set; }
        string status { get; set; }
    }

    public struct PidjPagination : IPidjHtmlResponse
    {
        public string html { get; set; }
        public string status { get; set; }
        public string query { get; set; }
        public PidjPaginationDetails details { get; set; }
    }

    public struct PidjPaginationDetails
    {
        public bool show_previous { get; set; }
        public bool show_next { get; set; }

        public string per_page { get; set; }

        public int[] show_pages { get; set; }

        public int total_count { get; set; }
        public int total_pages { get; set; }
        public int current_page { get; set; }
        public int start_record_number { get; set; }
        public int end_record_number { get; set; }
    }

    public struct PidjContact
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Business { get; set; }
        public string Display { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public static class ExtensionsTemp
    {
        public static IEnumerable<T> ToObjects<T>(this Regex rgx, string input) where T : struct
        {
            var matches = rgx.Matches(input).Cast<Match>().Select(m => m.Groups);
            string[] names = rgx.GetGroupNames().Skip(1).ToArray();
            string grpValue = null;
            PropertyInfo prop = null;

            foreach (var match in matches)
            {
                T obj = new T();
                Type objType = obj.GetType();

                foreach (string name in names)
                {
                    if (match.TryGetGroup(name, out grpValue) && objType.TryGetProperty(name, out prop))
                        prop.SetValue(obj, grpValue);
                }

                yield return obj;
            }

            matches = null;
            names = null;

            yield break;
        }

        public static bool TryGetProperty(this Type type, string name, out PropertyInfo prop)
        {
            PropertyInfo temp = null;
            try
            {
                temp = type.GetProperty(name, BindingFlags.Instance);
                if (temp == null)
                {
                    prop = null;
                    return false;
                }

                prop = temp;
                return true;
            }
            catch
            {
                prop = null;
                return false;
            }
        }

        public static bool TryGetGroup(this GroupCollection grps, string key, out string grpValue)
        {
            Group output = grps[key];
            grpValue = output.Value;
            return output.Success && !String.IsNullOrWhiteSpace(grpValue);
        }
    }

    public struct PidjToken
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}

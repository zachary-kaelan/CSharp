using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Java.IO;
using Org.Apache.Http;
using Environment = System.Environment;

namespace TimeSheetApp2
{
    [Service]
    public class TimeSheetService : Service
    {
        private static readonly string Tag = "X:" + typeof(TimeSheetService).Name;

        public Binder Binder { get; set; }

        public static readonly List<KeyValuePair<string, string>> DefaultHeaders =
            new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Host", "vrm.pestpac.com"),
                new KeyValuePair<string, string>("Cache-Control", "max-age=0"),
                new KeyValuePair<string, string>("Origin", "http://vrm.pestpac.com"),
                new KeyValuePair<string, string>("Upgrade-Insecure-Requests", "1"),
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded"),
                new KeyValuePair<string, string>("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"),
                new KeyValuePair<string, string>("Accept-Encoding", "gzip, deflate"),
                new KeyValuePair<string, string>("Accept-Language", "en-US,en;q=0.8")
            };

        public static readonly string DataFile =
            Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\userdata.txt";

        public static Dictionary<string, string> EmployeeDictionary { get; set; }

        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public Dictionary<string, Day> History { get; set; }

        public TimeSheetService() : base()
        {

        }

        public override IBinder OnBind(Intent intent)
        {
            this.Username = intent.Action;
            DateTime now = DateTime.Now;
            this.EmployeeId = MainActivity.EmployeeDictionary.TryGetValue(Username, out string idValue)
                ? idValue
                : "not found";
            this.History = new Dictionary<string, Day>()
            {
                {now.ToShortDateString(), new Day(now)}
            };

            this.Binder = new Binder();
            return this.Binder;
        }

        
    }

    public class ServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private static readonly string TAG = typeof(TimeSheetService).FullName;
        private MainActivity mainActivity { get; set; }
        public bool IsConnected { get; set; }
        public Binder Binder { get; private set; }

        public ServiceConnection(MainActivity activity)
        {
            this.mainActivity = activity;
            Binder = null;
            IsConnected = false;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as Binder;
            IsConnected = this.Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
        }
    }
}


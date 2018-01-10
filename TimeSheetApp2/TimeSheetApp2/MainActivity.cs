using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;
using Android.Bluetooth.LE;
using Android.Widget;
using Android.OS;
using Android.OS.Storage;
using Android.Runtime;
using TimeSheetApp2;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Android.Content;
using Android.Graphics;
using Environment = System.Environment;
using Newtonsoft.Json;
using RestSharp;

namespace TimeSheetApp2
{
    [Activity(Label = "TimeSheetApp2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        //private TimeSheetService Service { get; set; }
        //private ServiceConnection Connection { get; set; }

        public static readonly List<KeyValuePair<string, string>> DefaultHeaders = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>( "Host", "vrm.pestpac.com" ),
            new KeyValuePair<string, string>( "Cache-Control", "max-age=0" ),
            new KeyValuePair<string, string>( "Origin", "http://vrm.pestpac.com" ),
            new KeyValuePair<string, string>( "Upgrade-Insecure-Requests", "1" ),
            new KeyValuePair<string, string>( "Content-Type", "application/x-www-form-urlencoded" ),
            new KeyValuePair<string, string>( "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8" ),
            new KeyValuePair<string, string>( "Accept-Encoding", "gzip, deflate" ),
            new KeyValuePair<string, string>( "Accept-Language", "en-US,en;q=0.8" )
        };
        public static readonly string DataFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\userdata.txt";
        public static Dictionary<string, string> EmployeeDictionary { get; set; }
        public static Dictionary<string, string> BreaksDictionary { get; set; }
        public static UserInfo UserData = new UserInfo();
        public static Postman Client = new Postman();

        private EditText txtUser { get; set; }
        private Button btnPunch { get; set; }
        private Switch btnBreak { get; set; }
        private KeyValuePair<string, DateTime>? BreakIn { get; set; }
        private RadioGroup grpBreakSelect { get; set; }
        private List<RadioButton> radButtons { get; set; }
        
        public static bool GotUserInfo = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            Stream stream = typeof(Resource).GetTypeInfo().Assembly.GetManifestResourceStream("TimeSheetApp2.Employees.txt");
            using (var reader = new StreamReader(stream))
            {
                EmployeeDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
            }
            stream.Dispose();
            stream.Close();
            stream = null;

            stream = typeof(Resource).GetTypeInfo().Assembly.GetManifestResourceStream("TimeSheetApp2.EventReasons.txt");
            using (var reader = new StreamReader(stream))
            {
                BreaksDictionary = reader.ReadToEnd().Split(new char[] { '\n', 'r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Split(new string[] { " :=: " }, StringSplitOptions.None))
                    .ToDictionary(s => s[0], s => s[1]);
            }

            this.txtUser = FindViewById<EditText>(Resource.Id.editText1);
            this.btnPunch = FindViewById<ToggleButton>(Resource.Id.toggleButton1);
            this.btnBreak = FindViewById<Switch>(Resource.Id.switch1);
            this.grpBreakSelect = FindViewById<RadioGroup>(Resource.Id.radioGroup1);
            this.radButtons = Enumerable.Range(1, grpBreakSelect.ChildCount)
                .Select(i => FindViewById<RadioButton>(grpBreakSelect.GetChildAt(i).Id)).ToList();
            btnPunch.Click += btnPunch_Click;
            btnBreak.Click += btnBreak_Click;
        }

        private void btnPunch_Click(object sender, EventArgs e)
        {
            

            DateTime now = DateTime.Now;
            if (!GotUserInfo)
            {
                /*this.Connection = new ServiceConnection(this);
                ServiceBound = true;
                this.Service = new TimeSheetService();
                this.Service.BindService(new Intent(this.txtUser.Text), this.Connection, Bind.AutoCreate);*/

                if (!File.Exists(DataFile))
                {
                    UserData = new UserInfo(this.txtUser.Text, now);
                    File.WriteAllText(DataFile, JsonConvert.SerializeObject(UserData));
                }
                else
                {
                    UserData = JsonConvert.DeserializeObject<UserInfo>(File.ReadAllText(DataFile));
                    UserData.History.Add(now.ToShortDateString(), new Day(now));
                }

                btnPunch.SetBackgroundColor(Color.Red);
                btnBreak.Enabled = true;
                radButtons.ForEach(r => r.Enabled = true);
            }
            else
            {
                UserData.History.TryGetValue(now.ToShortDateString(), out Day day);
                day.ClockOut = now;
                btnPunch.SetBackgroundColor(new Color(235, 235, 15));
                btnBreak.Enabled = false;
                radButtons.ForEach(r => r.Enabled = false);
                File.WriteAllText(DataFile, JsonConvert.SerializeObject(UserData));

                /*CookieContainer cookies = new CookieContainer();
                RestClient client = new RestClient("http://vrm.pestpac.com/")
                {
                    CookieContainer = cookies,
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36",
                    FollowRedirects = true
                };
                DefaultHeaders.ForEach(h => client.AddDefaultHeader(h.Key, h.Value));

                RestRequest request = new RestRequest("Timesheets/Summary/Login", Method.POST);
                request.AddParameter("CompanyKey", "323480", ParameterType.GetOrPost);
                request.AddParameter("UserName", "zac.test", ParameterType.GetOrPost);
                request.AddParameter("Password", "I15Zac$0208", ParameterType.GetOrPost);
                request.AddParameter("RememberCompanyKey", "true", ParameterType.GetOrPost);
                request.AddParameter("RememberCompanyKey", "true", ParameterType.GetOrPost);
                request.AddParameter("returnUrl", "", ParameterType.GetOrPost);
                client.Execute(request);*/


                Client.AddPunch(UserData.EmployeeId, day.ClockIn, day.ClockOut, PunchType.Regular);
                foreach(var workBreak in day.Breaks)
                {
                    Client.AddPunch(UserData.EmployeeId, workBreak.Value.Key, workBreak.Value.Value, PunchType.Break, workBreak.Key);
                }
            }
        }

        private void btnBreak_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (BreakIn == null)
            {
                BreakIn = new KeyValuePair<string, DateTime>("", now);
                btnBreak.SetBackgroundColor(Color.Red);
            }
            else
            {
                UserData.History.TryGetValue(now.ToShortDateString(), out Day day);
                
                day.Breaks.Add(new KeyValuePair<string, KeyValuePair<DateTime, DateTime>>(BreakIn.Value.Key, new KeyValuePair<DateTime, DateTime>(BreakIn.Value.Value, now)));
                BreakIn = null;
                btnBreak.SetBackgroundColor(new Color(235, 235, 15));
            }
        }

        public static string ExceptionToString(Exception e, int tabCount = 0)
        {
            StringBuilder sb = new StringBuilder("");

            sb.Append("MESSAGE:\t\t");
            if (!String.IsNullOrWhiteSpace(e.Message))
                sb.AppendLine(e.Message);
            else if (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                sb.AppendLine(e.InnerException.Message);
            else
                sb.AppendLine("None.");

            sb.Append(new string('\t', tabCount));
            sb.Append("SOURCE:\t\t");
            sb.AppendLine(e.Source);

            sb.Append(new string('\t', tabCount));
            sb.Append("STACKTRACE:\t");
            sb.AppendLine(e.StackTrace.Replace("\n", "\n" + new string('\t', tabCount + 3)));

            sb.Append(new string('\t', tabCount));
            sb.Append("TARGETSITE:\t");
            sb.AppendLine(e.TargetSite.Name);

            return sb.ToString();
        }
    }

    public struct UserInfo
    {
        public string Username { get; set; }
        public string EmployeeId { get; set; }
        public string RefreshToken { get; set; }
        public string TokenExpireTime { get; set; }
        public Dictionary<string, Day> History { get; set; }

        public UserInfo(string user, DateTime now)
        {
            Username = user;
            EmployeeId = MainActivity.EmployeeDictionary.TryGetValue(Username, out string idValue) ? idValue : "not found";
            History = new Dictionary<string, Day>()
            {
                {now.ToShortDateString(), new Day(now) }
            };

            RefreshToken = null;
            TokenExpireTime = null;
        }
    }

    public struct Day
    {
        public DateTime ClockIn { get; set; }
        public DateTime ClockOut { get; set; }
        public List<KeyValuePair<string, KeyValuePair<DateTime, DateTime>>> Breaks { get; set; }

        public Day(DateTime initialPunch)
        {
            ClockIn = initialPunch;
            ClockOut = initialPunch;
            Breaks = new List<KeyValuePair<string, KeyValuePair<DateTime, DateTime>>>();
        }
    }
}


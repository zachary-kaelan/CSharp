using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Jil;
using RestSharp;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Net;
//using System.Net.WebSockets;
using WebSocketSharp;

namespace Slack
{
    public class RTMClient
    {
        public const string eventsPath = @"G:\Slack\Events\";
        public const string usersPath = @"G:\Slack\Users\";
        public const string notesPath = @"G:\Slack\Notes.txt";

        /*const string OAuthToken = "xoxp-16172464452-184906848646-221143767396-b4ffa3d8deaa0d1386daa178932f1b62";
        const string ClientID = "16172464452.222180667527";
        const string ClientSecret = "9bdacab3a43293f41e6956728d8eb8fe";
        const string VerifToken = "uiyaTwqgWrutFZRJdHrprjoQ";*/

        const int BUFFER_SIZE = 1024;
        /*const string OAuthToken = "xoxp-16172464452-184906848646-223197167812-30a94f5a19859a0f878bfadfb8f3c0ad";
        const string botOAuthToken = "xoxb-222616323777-JnDu9W5JrboDNaL5MVFS1g5T";
        const string ClientID = "16172464452.224228723319";
        const string ClientSecret = "fc4e1e1e4b8323accc3d9a63b2a03708";
        const string VerifToken = "75FWQKAKncF5KJD9Sy0XkdeZ";*/
        public string OAuthToken { get; set; }
        public string botOAuthToken { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string VerifToken { get; set; }

        public readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public ConcurrentQueue<ArraySegment<byte>> _queue { get; set; }
        public ConcurrentQueue<Dictionary<string,object>> _events { get; set; }
        public ConcurrentQueue<string> _notes { get; set; }
        public Dictionary<string, string> users { get; set; }

        public RestClient client { get; set; }
        //public string accessToken { get; set; }
        //public ClientWebSocket sock { get; set; }
        //public HttpListener listener { get; set; }
        public WebSocket ws { get; set; }
        public SlackClient slack { get; set; }

        public RestClient websocket { get; set; }
        public static CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public static CompareOptions options = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;
        public static JsonSerializerSettings serializerSettings = new JsonSerializerSettings { ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor };

        private Thread ReceiveThread { get; set; }
        private Thread DecodeThread { get; set; }
        private object consoleLock = new object();
        //public Thread HttpReceiveThread { get; set; }

        public RTMClient(SlackClient slackClient, string oauth, string botOAuth, string clientid, string clientsecret, string veriftoken)
        {
            OAuthToken = oauth;
            botOAuthToken = botOAuth;
            ClientID = clientid;
            ClientSecret = clientsecret;
            VerifToken = veriftoken;

            slack = slackClient;

            this._notes = new ConcurrentQueue<string>();
            client = new RestClient("https://slack.com/");

            //client.AddDefaultParameter("token", OAuthToken, ParameterType.QueryString);
            //client.AddDefaultHeader("Authorization", "Bearer " + OAuthToken);

            users = slack.GetUsers();

            ServicePointManager.UseNagleAlgorithm = false;
        }

        public string GetToken()
        {
            RestRequest request = new RestRequest("oauth/authorize", Method.GET);
            request.Parameters.AddRange(new Parameter[]
            {
                new Parameter() {Name="client_id", Type=ParameterType.GetOrPost, Value=ClientID},
                new Parameter() {Name="scope", Type=ParameterType.GetOrPost, Value="rtm:read"}
            });
            var response = this.client.Execute(request);
            string code = JSON.DeserializeDynamic(response.Content).code;

            request.Resource = "api/oauth.access";
            request.Parameters.RemoveAll(p => p.Name == "scope");
            request.AddParameter("client_secret", ClientSecret, ParameterType.GetOrPost);
            request.AddParameter("code", code, ParameterType.GetOrPost);

            response = this.client.Execute(request);
            return JSON.DeserializeDynamic(response.Content).access_token;
        }

        public void Start()
        {
            RestRequest request = new RestRequest("api/rtm.start", Method.POST);
            request.AddParameter("token", botOAuthToken, ParameterType.GetOrPost);
            var response = client.Execute(request);
            string websocketURL = JSON.DeserializeDynamic(response.Content).url;

            ws = new WebSocket(websocketURL);
            ws.OnMessage += Ws_OnMessage;
            ws.Log.File = @"G:\Slack\WebSocketLog.txt";
            ws.OnError += (sender, e) =>
            {
                lock (consoleLock) { Console.WriteLine("ERROR : {0}", e.Message); }
            };
            ws.OnOpen += (sender, e) =>
            {
                lock(consoleLock) { Console.WriteLine("Socket opened."); }
            };
            ws.OnClose += (sender, e) =>
            {
                lock(consoleLock) { Console.WriteLine("Socket closed."); }
            };

            ws.Connect();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            string content = e.Data;
            Dictionary<string, object> data = JSON.Deserialize<Dictionary<string, object>>(content);
            string type = data["type"].ToString();
            if (type == "reaction_added" && data["reaction"].ToString() == ":mark_for_upload:")
                this._notes.Enqueue(
                    data["item_user"] + " - " +
                        JSON.Deserialize<Dictionary<string, string>>(
                            data["item"].ToString())["ts"]);

            lock (consoleLock)
            {
                if (data.TryGetValue("user", out object user))
                {
                    string name;
                    if (user.ToString() == "U6JJ49HNV")
                        name = "Events Tracker";
                    else if (!this.users.TryGetValue(user.ToString(), out name))
                    {
                        Console.WriteLine("User list outdated. Updating...");
                        Console.WriteLine("{0} users updated.", slack.UpdateUserList());
                        this.users = slack.GetUsers();
                    }
                    content = Regex.Replace(content, "\"user\":\"(.+?)\"", name);
                }

                Console.WriteLine("Bytes received: {0}", e.RawData.Length);
                Console.WriteLine("Event of type " + type.ToUpper() + " received.");
            }
            
            string path = eventsPath + DateTime.Now.ToString("d").Replace('/', '.') + @"\" + type + @"\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            try
            {
                path += JSON.DeserializeDynamic(content).subtype + @"\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch
            {

            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            DateTime ts = DateTime.Now.Subtract(new TimeSpan(0, 0, 2));

            string format = @"HH\hmm\mss.";
            string filePath = "";
            do
            {
                format += "F";
                filePath = path + ts
                    .ToString(format + @"\s") + ".txt";
            } while (File.Exists(filePath));

            File.WriteAllText(filePath, content);
            //this._events.Enqueue(JSON.Deserialize<Dictionary<string, object>>(content));
        }
        /*

this.listener = new HttpListener();
this.listener.Prefixes.Add(websocketURL);
this.listener.Start();

this.sock = new ClientWebSocket();
await this.sock.ConnectAsync(new Uri(websocketURL), CancellationToken.None);

this.ReceiveThread = new Thread(this.Receive);
this.DecodeThread = new Thread(this.Decode);
//this.HttpReceiveThread = new Thread(this.HttpReceive);
}

public async void Receive()
{
byte[] buffer = new byte[BUFFER_SIZE];
ArraySegment<byte> seg = new ArraySegment<byte>(buffer);
while (true)
{
   WebSocketReceiveResult result = await this.sock.ReceiveAsync(seg, CancellationToken.None);
   SpinWait.SpinUntil(() => result.EndOfMessage);
   Console.Write("Bytes received: " + result.Count);
   this._queue.Enqueue(seg);
}
}

public async void HttpReceive()
{
while (true)
{
   var hc = await this.listener.GetContextAsync();
   var sockc = await hc.AcceptWebSocketAsync("json");

}
}

public void Decode()
{

ArraySegment<byte> result = new ArraySegment<byte>();
while (true)
{
   SpinWait.SpinUntil(() => this._queue.TryDequeue(out result));
   string content = Encoding.UTF8.GetString(result.Array);
   string type = JSON.DeserializeDynamic(content).type;
   Console.WriteLine("Event of type " + type.ToUpper() + " received.");
   string path = eventsPath + type + @"\";
   if (!Directory.Exists(path))
       Directory.CreateDirectory(path);
   try
   {
       path += JSON.DeserializeDynamic(content).subtype + @"\";
       if (!Directory.Exists(path))
           Directory.CreateDirectory(path);
   }
   catch
   {

   }

   path += DateTime.Now.ToString("d").Replace('/', '.') + @"\";
   if (!Directory.Exists(path))
       Directory.CreateDirectory(path);

   string format = "HH:mm:ss.";
   string filePath = "";
   do
   {
       format += "F";
       filePath = path + UnixTime.AddMilliseconds(
           Convert.ToDouble(
               JSON.DeserializeDynamic(
                   content
               ).ts
           )
       ).ToString(format);
   } while (File.Exists(filePath));

   File.WriteAllText(path, content);

   this._events.Enqueue(JSON.Deserialize<Dictionary<string, object>>(content));
}
}

public void SaveEvents()
{
string json = "";
while (true)
{
   //SpinWait.SpinUntil(() => this._events.TryDequeue(out json));
}
}*/
    }
}

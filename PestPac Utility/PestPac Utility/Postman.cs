using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;

using PestPac.Model;
using Jil;
using PPLib;
using RestSharp;

namespace PestPac_Utility
{
    public enum NoteCode
    {
        GEN,
        FEEDBACK
    };

    public class Postman
    {
        public string refreshToken { get; set; }
        public const string compkey = "323480";

        public RestClient client { get; set; }
        public RestClient tokenClient { get; set; }

        public static string LOGS_PATH = null;

        private LogManager Logger { get; set; }

        public static SortedDictionary<string, string> locationCodes =
            new SortedDictionary<string, string>(
                    new RGX.Utils.FileDictionary().Matches(
                        PestPac_Utility.Properties.Resources.LocationCodes
                    ).Cast<System.Text.RegularExpressions.Match>().ToDictionary(
                        m => m.Groups[1].Value, m => m.Groups[2].Value)
                );

        #region Setup

        public Postman(string refreshToken) : this()
        {
            this.refreshToken = refreshToken;
        }

        public Postman()
        {
            /*string locationCodesPath = Form1.MAIN_PATH + "LocationCodes.txt";
            if (!File.Exists(locationCodesPath))
                new RestClient("https://www.dropbox.com/s/pztg1m3a9zg945f/").DownloadData(
                    new RestRequest("LocationCodes.txt?dl=1", Method.GET)
                ).SaveAs(locationCodesPath);
            
            if (locationCodes == null)
                locationCodes = new SortedDictionary<string, string>(
                    new RGX.Utils.FileDictionary().Matches(
                        File.ReadAllText(locationCodesPath)
                    ).Cast<System.Text.RegularExpressions.Match>().ToDictionary(
                        m => m.Groups[1].Value, m => m.Groups[2].Value)
                );*/

            this.tokenClient = new RestClient("https://is.workwave.com/oauth2/token?scope=openid");
            this.tokenClient.AddDefaultHeader("content-type", "application/x-www-form-urlencoded");
            this.tokenClient.AddDefaultHeader("authorization", "Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh");

            this.client = new RestClient("https://api.workwave.com/pestpac/v1/");
            this.client.CookieContainer = new CookieContainer();
            this.client.AddDefaultHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            this.client.AddDefaultHeader("tenant-id", compkey);
            this.client.AddDefaultHeader("authorization", "Bearer ");

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

        public Postman(string refreshToken, LogManager logger) : this(logger)
        {
            this.refreshToken = refreshToken;
        }

        public void GetToken()
        {
            SpinWait.SpinUntil(() => Postman.CheckInternet());
            TokenResponse tokens = new TokenResponse();

            if (!String.IsNullOrWhiteSpace(this.refreshToken))
            {
                RestRequest request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "refresh_token", ParameterType.GetOrPost);
                request.AddParameter("refresh_token", this.refreshToken, ParameterType.GetOrPost);

                tokens = JSON.Deserialize<TokenResponse>(tokenClient.Execute(request).Content);
            }
            else
            {
                RestRequest request = new RestRequest(Method.POST);

                request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
                request.AddParameter("username", "pestpacapi@insightpest.com", ParameterType.GetOrPost);
                request.AddParameter("password", "!Pest6547!", ParameterType.GetOrPost);

                //request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=pestpacapi%40insightpest.com&password=!Pest6547!", ParameterType.RequestBody);
                tokens = JSON.Deserialize<TokenResponse>(tokenClient.Execute(request).Content);
                this.refreshToken = tokens.refresh_token;
                Form1.config.Append("RefreshToken", this.refreshToken);
            }

            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + tokens.access_token);
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

#endregion

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
            return JSON.Deserialize<List<NoteListModel>>(content);
        }

        #endregion

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

        #region UploadNote

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

#endregion

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
}

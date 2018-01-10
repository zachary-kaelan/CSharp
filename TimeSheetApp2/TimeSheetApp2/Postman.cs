using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;
using RestSharp;

namespace TimeSheetApp2
{
    public enum PunchType { Regular, Break, Benefit }

    public class Postman
    {
        public delegate void ExecutionHandler(object sender, ExecutionEventArgs e);
        public event ExecutionHandler OnExecute;

        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public DateTime expireTime { get; set; }
        public const string compkey = "323480";

        public RestClient client { get; set; }
        public RestClient tokenClient { get; set; }

        public Postman()
        {
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

        public void GetToken()
        {
            SpinWait.SpinUntil(() => Postman.CheckInternet());
            TokenResponse tokens = new TokenResponse();

            try
            {
                if (DateTime.Compare(expireTime, DateTime.Now) <= 0)
                    throw new Exception("Refresh.");
                RestRequest request = new RestRequest(Method.POST);

                request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
                request.AddParameter("username", "pestpacapi@insightpest.com", ParameterType.GetOrPost);
                request.AddParameter("password", "!Pest6547!", ParameterType.GetOrPost);

                //request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=pestpacapi%40insightpest.com&password=!Pest6547!", ParameterType.RequestBody);
                tokens = JsonConvert.DeserializeObject<TokenResponse>(tokenClient.Execute(request).Content);
            }
            catch
            {
                RestRequest request = new RestRequest(Method.POST);
                request.AddParameter("grant_type", "refresh_token", ParameterType.GetOrPost);
                request.AddParameter("refresh_token", this.refreshToken, ParameterType.GetOrPost);

                tokens = JsonConvert.DeserializeObject<TokenResponse>(tokenClient.Execute(request).Content);
            }

            this.accessToken = tokens.access_token;
            this.refreshToken = tokens.refresh_token;
            this.expireTime = DateTime.Now.AddSeconds(tokens.expires_in - 5);

            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + this.accessToken);

            MainActivity.UserData.RefreshToken = this.refreshToken;
            MainActivity.UserData.TokenExpireTime = this.expireTime.ToString("G");
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

        public void AddPunch(string employeeId, DateTime startDate, DateTime endDate, PunchType punchType = PunchType.Regular, string reason = null)
        {
            TryExecute(
                new RestRequest(
                    "lookups/employees/" + 
                    employeeId + 
                    "/timesheets", 
                    Method.POST
                ).AddJsonBody(
                    new TimesheetModel(
                        startDate,
                        endDate,
                        punchType,
                        reason
                    )
                ), out _);
            
        }

        public bool TryExecute(IRestRequest request, out string content)
        {

            RestResponse response = null;
            try
            {
                if (!CheckInternet())
                {
                    OnExecute(this, new ExecutionEventArgs() { ResponseCode = "NTWRK", Message = "Internet down, waiting..." });
                    SpinWait.SpinUntil(() => CheckInternet());
                    OnExecute(this, new ExecutionEventArgs() { ResponseCode = "NTWRK", Message = "Internet back up!" });
                }

                response = (RestResponse)client.Execute(request);
                content = response.Content;

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = "Getting new token...", URL = request.Resource });
                    this.GetToken();
                    return this.TryExecute(request, out content);
                }
                else if ((int)response.StatusCode >= 400)
                {
                    OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = response.Content.ToString(), URL = request.Resource });
                    return false;
                }
            }
            catch (Exception e)
            {
                OnExecute(this, new ExecutionEventArgs() { ResponseCode = ((int)response.StatusCode).ToString(), Message = MainActivity.ExceptionToString(e, 5), URL = request.Resource });
                content = null;
            }

            return (int)response.StatusCode <= 400;
        }

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

    public class ExecutionEventArgs : EventArgs
    {
        public string ResponseCode { get; set; }
        public string URL { get; set; }
        public string Message { get; set; }
    }

    public struct TimesheetModel
    {
        public const string timesheetDateTimeFormat = "yyyy'-'MM'-'dd HH:mm";

        public string PunchType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string BenefitType { get; set; }
        public string BreakReason { get; set; }

        public TimesheetModel(DateTime startDate, DateTime endDate, PunchType punchType = TimeSheetApp2.PunchType.Regular, string reason = null)
        {
            PunchType = punchType.ToString();
            StartDate = startDate.ToString(timesheetDateTimeFormat);
            EndDate = endDate.ToString(timesheetDateTimeFormat);
            BenefitType = null;
            BreakReason = null;

            if (punchType == TimeSheetApp2.PunchType.Benefit)
                BenefitType = reason;
            else if (punchType == TimeSheetApp2.PunchType.Break)
                BreakReason = reason;

        }
    }
}
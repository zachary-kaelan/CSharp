using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Jil;
using PestPac.Model;
using PostmanLib.Properties;
using PPLib;
using RestSharp;
//using ZachLib;
using ZachLib.Logging;

namespace PostmanLib
{
    public enum NoteCode
    {
        GEN,
        FEEDBACK
    };

    public static class Postman
    {
        #region Setup
        public delegate void ExecutionHandler(object sender, ExecutionEventArgs e);
        public static event ExecutionHandler OnExecute;
        private const string REFRESH_TOKEN = "2b77ff1d-1da4-35a8-9338-538dbaef50ca";

        private static readonly RestClient client = new RestClient("https://api.workwave.com/pestpac/v1/");
        private static readonly RestClient tokenClient = new RestClient("https://is.workwave.com/oauth2/token?scope=openid");

        static Postman()
        {
            LogManager.AddLog(
                "Postman",
                LogType.FolderFilesByDate
            );

            OnExecute += Postman_OnExecute;

            tokenClient.AddDefaultHeader("authorization", "Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh");
            tokenClient.AddDefaultParameter("grant_type", "refresh_token");
            tokenClient.AddDefaultParameter("refresh_token", REFRESH_TOKEN);

            client.AddDefaultHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            client.AddDefaultHeader("tenant-id", "323480");
            client.AddDefaultHeader("Authorization", "");

            WaitForInternet();
            DateTime now = DateTime.Now;

            if (DateTime.Compare(Settings.Default.expires_in, now) <= 0 || String.IsNullOrWhiteSpace(Settings.Default.access_token))
                GetToken(now);
        }

        private static void Postman_OnExecute(object sender, ExecutionEventArgs e)
        {
            if (!e.ResponseCode.IsOK())
            {
                if (e.ResponseCode == HttpStatusCode.NotImplemented)
                {
                    RestRequest request = (RestRequest)sender;
                    LogManager.Enqueue(
                        "Postman",
                        

                    );
                }
                LogManager.Enqueue(
                    "Postman",
                    EntryType.HTTP
                );
            }
        }

        private static void GetToken(DateTime now)
        {
            TokenResponse tokens = JSON.Deserialize<TokenResponse>(
                tokenClient.Execute(
                    new RestRequest(Method.POST)
                ).Content
            );

            Settings.Default.access_token = tokens.access_token;
            Settings.Default.expires_in = now.AddSeconds(tokens.expires_in - 5);
            Settings.Default.Save();

            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + Settings.Default.access_token);
            
        }

        private static void GetToken()
        {
            GetToken(DateTime.Now);
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

        private static bool WaitForInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return false;
                    }
                }
            }
            catch {
                while (true)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            using (client.OpenRead("http://clients3.google.com/generate_204"))
                            {
                                return true;
                            }
                        }
                    }
                    catch {}
                }
            }
        }
        #endregion

        #region TryExecute
        private static HttpStatusCode TryExecute(IRestRequest request, out string content)
        {
            var eventargs = new ExecutionEventArgs();
            eventargs.URL = request.Resource;
            try
            {
                IRestResponse response = client.Execute(request);
                eventargs.ResponseCode = response.StatusCode;
                
                content = response.Content;
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    GetToken();
                    eventargs.Message = "Getting new token...";
                    OnExecute(request, eventargs);
                    return TryExecute(request, out content);
                }
                else if ((int)response.StatusCode >= 400)
                {
                    eventargs.Message = response.Content;
                    OnExecute(request, eventargs);
                    return response.StatusCode;
                }
                else
                {
                    eventargs.Message = "Request successful! " + response.ContentLength.ToString() + " bytes received.";
                    OnExecute(request, eventargs);
                    return HttpStatusCode.OK;
                }
            }
            catch (Exception e)
            {
                if (WaitForInternet())
                {
                    eventargs.ResponseCode = HttpStatusCode.ServiceUnavailable;
                    eventargs.Message = "Internet was down. Retrying...";
                    OnExecute(request, eventargs);
                    return TryExecute(request, out content);
                }

                content = null;
                while (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                    e = e.InnerException;
                eventargs.ResponseCode = HttpStatusCode.NotImplemented;
                eventargs.Data = e;
                eventargs.Message = e.Message;
                OnExecute(request, eventargs);

                return HttpStatusCode.NotImplemented;
            }
        }

        private static HttpStatusCode TryExecute<T>(IRestRequest request, out T content)
        {
            var code = TryExecute(request, out string str);
            if (code.IsOK() && !String.IsNullOrWhiteSpace(str))
            {
                content = JSON.Deserialize<T>(str);
                return code;
            }

            content = default(T);
            return code;
        }

        private static HttpStatusCode TryExecute(IRestRequest request)
        {
            return TryExecute(request, out _);
        }

        private static bool IsOK(this HttpStatusCode code)
        {
            return (int)code <= 400;
        }
        #endregion

        #region Search
        public static bool Search(string query, out IEnumerable<LocationListModel> results)
        {
            return TryExecute<IEnumerable<LocationListModel>>(
                new RestRequest(
                    "locations", 
                    Method.GET
                ).AddOrUpdateParameter(
                    "q", query, ParameterType.QueryString
                ), out results
            ).IsOK();
        }
        #endregion

        #region UploadNote
        public static bool UploadNote(NoteModel note)
        {
            return TryExecute(
                new RestRequest(
                    "locations/" + 
                    note.Associations.LocationID + 
                    "/notes", Method.POST
                ).AddJsonBody(note)
            ).IsOK();
        }

        public static bool UploadNote(string locID, string note, string prefix = "VTNotes : ", string code = "GEN")
        {
            return UploadNote(
                new NoteModel(
                    NoteCode:code,
                    Note:note,
                    CreatedByUser:"ADMN",
                    Associations:new NoteAssociationModel(
                        LocationID:Convert.ToInt32(locID)
                    )
                )
            );
        }

        public static bool UploadNote(string locID, string note, string prefix = "VTNotes : ", NoteCode code = NoteCode.GEN)
        {
            return UploadNote(locID, note, prefix, code.ToString());
        }
        #endregion

        #region GetDocuments
        public static bool GetDocuments(string locID, out IEnumerable<DocumentListModel> docs)
        {
            if (TryExecute<IEnumerable<DocumentListModel>>(
                new RestRequest(
                    "locations/" + locID + "/documents",
                    Method.GET
                ), out IEnumerable<DocumentListModel> docsTemp
            ).IsOK()) {
                docs = docsTemp;
                return docs.Any();
            }
            docs = Enumerable.Empty<DocumentListModel>();
            return false;
        }

        public static bool GetDocuments(string locID, string name, out IEnumerable<DocumentListModel> docs)
        {
            if (GetDocuments(locID, out docs))
            {
                docs = docs.Where(
                    d => d.DocumentType == DocumentListModel.DocumentTypeEnum.Locationdocument &&
                        compInf.IsPrefix(d.Name, name, options) ||
                        compInf.IsSuffix(d.Name, name, options) ||
                        d.Name.Contains(" " + name + " ")
                );
                return docs.Any();
            }
            
            return false;    
        }

        public static bool GetDocuments(string locID, DateTime startingDate, string name = null, out IEnumerable<DocumentListModel> docs)
        {
            if (String.IsNullOrWhiteSpace(name) ? GetDocuments(locID, out docs) : GetDocuments(locID, name, out docs))
            {
                docs = docs.Where(
                    d => d.Date.HasValue &&
                        d.Date.Value.CompareTo(startingDate) > 0
                );

                return docs.Any();
            }

            return false;
        }
        #endregion

        #region DocExists
        public static bool DocExists(string locID, DateTime comparisonDate, string name = null, out IEnumerable<DocumentListModel> duplicates)
        {
            if (GetDocuments(locID, comparisonDate, name, out duplicates))
            {
                duplicates = duplicates.Where(
                    d => !compInf.IsPrefix(d.FileName, "temp", options)
                );

                return duplicates.Any();
            }
            
            return false;
        }

        public static bool DocExists(string locID, string name = null, out IEnumerable<DocumentListModel> duplicates)
        {
            return DocExists(locID, DateTime.Now, name, out duplicates);
        }
        #endregion

        #region SetDocRecord
        public static bool SetDocRecord(string locID, string name, out IEnumerable<DocumentListModel> duplicates)
        {
            if (!DocExists(locID, name, out duplicates))
            {   
                if (
                    !TryExecute(
                        new RestRequest(
                            "documents", 
                            Method.POST
                        ).AddJsonBody(
                            new DocumentModel(
                                DocumentModel.DocumentTypeEnum.Locationdocument,
                                Name: name,
                                Date: DateTime.Now,
                                Tags: "Postman",
                                FileName: "tempName.pdf",
                                OrderID: Convert.ToInt32(locID)
                            )
                        ), out string content
                ).IsOK()) {
                    throw new Exception(content);
                }

                return true;
            }

            return false;
        }

        public static bool SetDocRecord(string locID, out IEnumerable<DocumentListModel> duplicates)
        {
            return SetDocRecord(locID, null, out duplicates);
        }

        public static bool SetDocRecord(Customer cust, out IEnumerable<DocumentListModel> duplicates)
        {
            return SetDocRecord(cust.LocationID, cust.Type.ToString(), out duplicates);
        }
        #endregion

        #region UploadDoc
        private const string CONTENT_TYPE_FORMAT = "multipart/form-data; boundary=----WebKitFormBoundary{0}";
        private const string FORM_DATA_FORMAT = 
            "------WebKitFormBoundary{0}\r\n" +
            "Content-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\n" +
            "Content-Type: application/pdf\r\n\r\n\r\n" +
            "------WebKitFormBoundary{0}--";
        public static bool UploadDoc(string docID, string path, out DocumentModel doc)
        {
            string boundaryCode = Utils.GetRandomString(15);
            string name = Path.GetFileName(path);

            var request = new RestRequest(
                "documents/" + docID + "/upload",
                Method.POST
            ){
                AlwaysMultipartFormData = true,
                Timeout = 15000
            }.AddOrUpdateParameter(
                String.Format(
                    CONTENT_TYPE_FORMAT,
                    boundaryCode
                ), String.Format(
                    FORM_DATA_FORMAT,
                    boundaryCode,
                    name
                ), ParameterType.RequestBody
            ).AddFile(
                "file", File.ReadAllBytes(path),
                name, "multipart/form-data"
            );
            boundaryCode = null;
            name = null;

            request.IncreaseNumAttempts();
            request.IncreaseNumAttempts();

            bool check = TryExecute<DocumentModel>(request, out doc).IsOK();
            request = null;
            return check;
        }
#endregion


    }

    public class ExecutionEventArgs : EventArgs
    {
        public HttpStatusCode ResponseCode { get; set; }
        public string URL { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}

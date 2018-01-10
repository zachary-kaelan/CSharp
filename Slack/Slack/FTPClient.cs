using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeTypes;

namespace Slack
{
    class DriveClient
    {
        const string SERVER_ADDR = "servicepropest.com";
        const int PORT = 21;
        const string USER = "sam@insightpest.com";
        const string PASS = "Sam@1218";
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "SlackSlackFiles";
        static DriveService service = new DriveService();
        static RestClient client = new RestClient("https://www.googleapis.com/");
        static string accessToken;

        public DriveClient()
        {
            
        }

        public static void SimpleUpload(string path)
        {
            var file = new System.IO.FileInfo(path);

            RestRequest request = new RestRequest("upload/drive/v3", Method.POST);
            request.AddQueryParameter("uploadType", "media");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            //request.AddHeader("Content-Type", MimeTypeMap.GetMimeType(file.Extension));
            request.AddFile("file", path, MimeTypeMap.GetMimeType(file.Extension));

            var response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void AuthorizeSerivceAcc()
        {
            ServiceAccountCredential credential;
            
            credential = ServiceAccountCredential
                .FromServiceAccountData(
                new IO.FileStream(
                    "service_account_key.json", 
                    IO.FileMode.Open, 
                    IO.FileAccess.Read
                ));

            accessToken = credential.GetAccessTokenForRequestAsync("https://www.googleapis.com/auth/drive").Result;
            
        }
        
        public static void Authorize()
        {
            UserCredential credential;
            using (var stream = new IO.FileStream("client_secret.json", IO.FileMode.Open, IO.FileAccess.Read))
            {
                string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                credPath = IO.Path.Combine(credPath, ".credentials/slackfiles.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<File> files = listRequest.Execute().Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
            }
            else
                Console.WriteLine("No files found.");
            Console.Read();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using System.Net.Http.Formatting;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using XML = System.Xml;

namespace PPInternal
{

    class JilJSONS : ISerializer
    {
        string ISerializer.RootElement { get; set; }
        string ISerializer.Namespace { get; set; }
        string ISerializer.DateFormat { get; set; }
        public string ContentType { get; set; }

        public JilJSONS()
        {
            ContentType = "application/json";
        }

        public string Serialize(object obj)
        {
            return JSON.SerializeDynamic(obj);
        }
    }

    class XMLSerialize : ISerializer
    {
        string ISerializer.RootElement { get; set; }
        string ISerializer.Namespace { get; set; }
        string ISerializer.DateFormat { get; set; }
        public string ContentType { get; set; }

        public XMLSerialize()
        {
            ContentType = "application/x-www-form-urlencoded";
        }

        string ISerializer.Serialize(object obj)
        {
            throw new NotImplementedException();
        }
    }
    class Client
    {
        public List<KeyValuePair<string, string>> cookies { get; set; }
        public string user { get; set; }
        public Dictionary<string, string> skillIDs = new Dictionary<string, string>() { { "MA", "86" }, { "MD", "85" }, { "NH", "81" }, { "NY", "84" }, { "RI", "87" } };
        public Client()
        {

        }
        public void Login(params string[] creds)
        {
            RestClient client = new RestClient("http://app.pestpac.com/default.asp?Mode=Login");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("accept-language", "en-US,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("referer", "http://app.pestpac.com/default.asp");
            request.AddHeader("dnt", "1");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("x-devtools-emulate-network-conditions-client-id", "df7cd306-a36f-4bed-b988-d37174767c4a");
            request.AddHeader("origin", "http://app.pestpac.com");
            request.AddHeader("x-devtools-request-id", "25464.962");
            request.AddParameter("application/x-www-form-urlencoded", "CompanyKey=323480&Password=I15Zac$0208&RememberMe=1&RememberedAuth=0&SavePassword=1&Username=zac.johnso", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            this.cookies = response.Cookies.Select(c => new KeyValuePair<string, string>(c.Name, c.Value)).ToList();

            client = null;
            client = new RestClient("http://app.pestpac.com/userAuthenticatedEmails/ajax/getUser.asp?UserID=");
            request = null;
            request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "en-US,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("referer", "http://app.pestpac.com/location/default.asp");
            request.AddHeader("dnt", "1");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            request.AddHeader("x-devtools-request-id", "25464.1067");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("x-devtools-emulate-network-conditions-client-id", "df7cd306-a36f-4bed-b988-d37174767c4a");
            request.AddHeader("accept", "*/*");

            this.cookies.ForEach(delegate (KeyValuePair<string, string> cookie) { request.AddCookie(cookie.Key, cookie.Value); });
            response = null;
            response = client.Execute(request);
            this.cookies = response.Cookies.Select(c => new KeyValuePair<string, string>(c.Name, c.Value)).ToList();
            this.user = response.Content;
        }

        public void UploadSkills(Dictionary<string,string> skills)
        {
            RestClient client = new RestClient("http://app.pestpac.com/xml/LocationSkills.asp");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("accept-language", "en-US,en;q=0.8");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("referer", "http://app.pestpac.com/dialog/LocationSkills.asp?LocationID=148418&Mode=Edit");
            request.AddHeader("dnt", "1");
            request.AddHeader("x-devtools-request-id", "25464.1426");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("x-devtools-emulate-network-conditions-client-id", "df7cd306-a36f-4bed-b988-d37174767c4a");
            request.AddHeader("accept", "application/xml, text/xml, */*; q=0.01");
            request.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            request.AddHeader("origin", "http://app.pestpac.com");

            int count = skills.Count;
            foreach(KeyValuePair<string, string> skill in skills)
            {
                request.AddParameter("application/x-www-form-urlencoded; charset=UTF-8", String.Format("\n<params>\n<database></database>\n<locationid>{0}</locationid>\n<checked></checked>\n<skillid>{1}</skillid>\n<mode>Insert</mode>\n</params>\n", skill.Key, skillIDs[skill.Value]));
                
            }
        }
    }
}

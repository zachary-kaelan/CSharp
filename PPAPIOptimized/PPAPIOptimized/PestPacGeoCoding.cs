using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Deserializers;

namespace PPAPIOptimized
{
    public static class PestPacGeoCoding
    {
        public static RestClient client = new RestClient("http://app.pestpac.com/xml/");
        private static DotNetXmlSerializer xmlTool = new DotNetXmlSerializer();
        private static DotNetXmlDeserializer xmlTool2 = new DotNetXmlDeserializer();

        public static string GetTaxCode(Params prms)
        {
            RestRequest request = new RestRequest("loctaxcode.asp", Method.POST);
            //request.AddHeader("Host", "app.pestpac.com");
            //request.AddHeader("Connection", "keep-alive");
            //request.AddHeader("Content-Length", "394");
            //request.AddHeader("Accept", "application/xml, text/xml, */*; q=0.01");
            /*request.AddHeader("Origin", "http://app.pestpac.com");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("DNT", "1");
            request.AddHeader("Referer", "http://app.pestpac.com/location/add.asp");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept-Language", "en-US,en;q=0.8");*/

            request = (RestRequest)request.AddXmlBody(prms);
            xmlTool2.RootElement = "response";
            var response = client.Execute(request);
            string str = xmlTool2.Deserialize<response>(response).taxcode;
            return str;
        }
    }
    
    public struct Params
    {
        public int companyid { get; set; }
        public string custom { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string county { get; set; }
        public string branchid { get; set; }
        public int typeid { get; set; }

        public Params(string addr, string cty, string st, string zip)
        {
            companyid = 1;
            custom = "SERVICEPRO123480";
            address = addr;
            city = cty;
            state = st;
            this.zip = zip;
            county = "";
            branchid = "";
            typeid = 2;
        }
    }

    public struct response
    {
        public string taxcode;
        public string taxcodeid;
    }
}

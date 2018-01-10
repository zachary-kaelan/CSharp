using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using PPLib;

namespace PPAPIOptimized
{
    class PhoneClient
    {
        private static readonly RGX.HTML.ListElements RGX_LIST = new RGX.HTML.ListElements();
        public RestClient client { get; set; }
        public List<string> requests { get; set; }
        private readonly CookieContainer Cookies = new CookieContainer();

        public PhoneClient()
        {
            client = new RestClient("http://www.phonevalidator.com/");
            client.FollowRedirects = true;
            client.CookieContainer = Cookies;
            client.AddDefaultParameter("__EVENTTARGET", "", ParameterType.GetOrPost);
            client.AddDefaultParameter("__EVENTARGUMENT", "", ParameterType.GetOrPost);
            client.AddDefaultParameter("__VIEWSTATE", "/wEPDwUKLTcxMDE4MzMyNg9kFgJmD2QWAgIDD2QWAgIBD2QWAgIHDxYCHgdWaXNpYmxlaBYCAgEPDxYEHgpfUHVibGljS2V5BSg2TGNQX3dzVEFBQUFBR3lTX3Vwa0dYX3A4TkItcFhCTFpvVnBScGZGHgtfUHJpdmF0ZUtleQUoNkxjUF93c1RBQUFBQUxFZUNncld4SmNqdmFNM3U3WmZRRzJwaTYyUmRkZFkWt1KzEj34vx4SHb8b3vDA5zp3wDdEldITrSiIsQlf", ParameterType.GetOrPost);
            client.AddDefaultParameter("__VIEWSTATEGENERATOR", "90059987", ParameterType.GetOrPost);
            client.AddDefaultParameter("__EVENTVALIDATION", "/wEdAAOQv2gY9rJbui5TbsdQjQAhlIa2FBcsjQf5x522fkgJUoOEZ6AatNd0x/zGdfVUJjE8Soim44Y7AgtFF+lj6J18a//KKSch53/asQxEvvTkwQ==", ParameterType.GetOrPost);
            client.AddDefaultParameter("ctl00$ContentPlaceHolder1$SearchButton", "Search", ParameterType.GetOrPost);
            requests = new List<string>();
        }

        public PhoneInfo GetInfo(string number)
        {
            RestRequest request = new RestRequest("index.aspx", Method.POST);
            request.AddParameter("ctl00$ContentPlaceHolder1$txtPhone", number, ParameterType.GetOrPost);
            return new PhoneInfo(
                RGX_LIST.Matches(
                    client.Execute(request).Content
                )
            );
        }

        /*public bool IsMobile(string number)
        {
            RestRequest request = new RestRequest(Method.POST);

            //string response = client.Execute(new RestRequest(Method.GET)).Content;
            //var matches = Regex.Matches(response, FormPat);

            request.Parameters.AddRange(Regex.Matches(
                client.Execute(new RestRequest(Method.GET)).Content,
                FormPat
            ).Cast<Match>().Select(
                m => new Parameter()
                {
                    Name = m.Groups[1].Value,
                    Value = m.Groups[2].Value,
                    Type = ParameterType.GetOrPost
                }
            ).Concat(new Parameter[] {
                new Parameter() {
                    Name = "ctl00$ContentPlaceHolder1$txtPhone",
                    Value = number, Type=ParameterType.GetOrPost },
                new Parameter(){
                    Name= "ctl00$ContentPlaceHolder1$SearchButton",
                    Value = "Search", Type=ParameterType.GetOrPost },
                new Parameter(){
                    Name="__EVENTTARGET",
                    Value=""},
                new Parameter(){
                    Name="__EVENTARGUMENT",
                    Value=""}
            }));

            return Regex.Match(
                client.Execute(request).Content,
                PhoneTypePat).Groups[1].Value == "CELL PHONE";
        }*/


    }

    public struct PhoneInfo
    {
        public string PhoneNumber { get; set; }
        public string ReportDate { get; set; }
        public string PhoneType { get; set; }
        public string PhoneCompany { get; set; }
        public string PhoneLocation { get; set; }

        public PhoneInfo(MatchCollection matches)
        {
            Dictionary<string, string> dict = matches.ToDictionary();
            this.PhoneNumber = dict["PhoneNumberLabel"];
            this.ReportDate = dict["ReportDateLabel"];
            this.PhoneType = dict["PhoneTypeLabel"];
            this.PhoneCompany = dict["PhoneCompanyLabel"];
            this.PhoneLocation = dict["PhoneLocationLabel"];
        }
    }

    
}

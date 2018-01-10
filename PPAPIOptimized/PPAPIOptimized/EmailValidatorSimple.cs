using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;

namespace PPAPIOptimized
{
    public static class EmailValidatorSimple
    {
        private static readonly System.Net.CookieContainer Cookies = new System.Net.CookieContainer();
        private static RestClient client = new RestClient("https://email-checker.net/") { CookieContainer=Cookies};
        const string csrfPat = "name=\"[^\"]+csrf\"" + @"\s" + "+value=\"([^\"]+)\"";
        const string resultPat = "span class=\"[^\"]+\">([^<]+)<" + @"\/span> <small>([^<]+)<\/small>";
        const string emailValidPat = @"(?<username>#?[_a-zA-Z0-9-+]+(\.[_a-zA-Z0-9-+]+)*)@(?<domain>[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|arpa|asia|coop|info|jobs|mobi|museum|name|travel)))";
        private static string csrf = null;

        public static string Validate(string email)
        {
            if (!Regex.IsMatch(email, emailValidPat))
                return "INVALID";

            if (csrf == null)
                csrf = Regex.Match(client.Execute(new RestRequest(Method.GET)).Content, csrfPat).Groups[1].Value;

            RestRequest request = new RestRequest("check", Method.POST);
            request.AddParameter("_csrf", csrf, ParameterType.GetOrPost);
            request.AddParameter("email", email, ParameterType.GetOrPost);

            string response = client.Execute(request).Content;
            csrf = Regex.Match(response, csrfPat).Groups[1].Value;

            Match result = Regex.Match(response, resultPat);
            return result.Groups[1].Value;
        }
    }

    public struct SimpleEmailModel
    {
        public string LocationID { get; set; }
        public string Email { get; set; }
    }
}

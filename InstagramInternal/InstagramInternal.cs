using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace InstagramLib
{
    public static class InstagramInternal
    {
        static InstagramInternal()
        {

        }

        private static CookieContainer COOKIES = new CookieContainer();
        private static RestClient CLIENT = new RestClient("https://www.instagram.com")
        {
            CookieContainer = COOKIES,
            FollowRedirects = true,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36"
        };

        private static void Login()
        {
            CLIENT.Execute(
                new RestRequest("/", Method.GET)
            );
            CLIENT.Execute(
                new RestRequest(
                    "/accounts/login/ajax/",
                    Method.POST
                ).AddOrUpdateParameter(
                    "username",
                    "wafflefaffy",
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "password",
                    "meeko011",
                    ParameterType.GetOrPost
                )
            );

        }
    }
}

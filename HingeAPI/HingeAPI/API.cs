using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using HingeAPI.Models;
using Jil;
using RestSharp;
using ZachLib;
using ZachLib.HTTP;
using ZachLib.Logging;

namespace HingeAPI
{
    public static class API
    {
        public const string LOG_NAME = "Hinge";

        private static readonly CookieContainer _COOKIES = new CookieContainer();
        private static RestClient _client = new RestClient("https://prod-api.hingeaws.net/")
        {
            CookieContainer = _COOKIES,
            UserAgent = "Hinge/11037 CFNetwork/976 Darwin/18.2.0"
        };

        static API()
        {
            _client.RemoveDefaultParameter("Accept");
            _client.AddDefaultHeader("X-Device-Platform", "iOS");
            _client.AddDefaultHeader("Accept", "*/*");
            _client.AddDefaultHeader("Authorization", "Bearer o2Xx5P4tJb7sRoabhKK0nJTlMpeSEORqWD_5W-Cu-sE=");
            _client.AddDefaultHeader("X-Install-Id", "63F86D3A-C7C0-49A4-BADC-12E515B6FA84");
            _client.AddDefaultHeader("Accept-Encoding", "br, gzip, deflate");
            _client.AddDefaultHeader("Accept-Language", "en-us");
            _client.AddDefaultHeader("X-Build-Number", "11037");
            _client.AddDefaultHeader("X-App-Version", "7.4.0");
            _client.AddDefaultHeader("X-OS-Version", "12.1.4");
            _client.AddDefaultHeader("X-Device-Model", "iPhone 7");
        }

        public static SortedDictionary<string, Profile> GetUsers(params string[] ids)
        {
            RestRequest request = new RestRequest("user/public");
            request.AddParameter("ids", String.Join(",", ids), ParameterType.QueryString);
            if (
                _client.TryExecute(
                    request,
                    LOG_NAME,
                    null,
                    out ProfileListModel[] profiles
                ).IsOK()
            )
                return new SortedDictionary<string, Profile>(profiles.ToDictionary(p => p.identityId, p => p.profile));
            return null;
        }

        public static int? RateProfile(string subjectId, ProfileRating rating) =>
            _client.TryExecute(
                new RestRequest("rate", Method.POST).AddParameter(
                    "application/json",
                    JSON.Serialize(new ProfileRate(subjectId, rating), Options.ISO8601Utc),
                    ParameterType.RequestBody
                ),
                LOG_NAME,
                null,
                out RateLimitResponse response
            ).IsOK() ? (response.limit == null ? null : new Nullable<int>(response.limit.likes)) : null;
    }
}

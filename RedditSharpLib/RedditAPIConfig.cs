using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using RateLimiter;
using ZachLib;
using ZachLib.HTTP;
using ZachLib.Logging;

namespace RedditSharpLib
{
    public static class RedditAPIConfig
    {
        public static int DefaultItemsLimit {
            get; set;
        }

        public static string UserAgent
        {
            get
            {
                return RedditClient.UserAgent;
            }
            set
            {
                RedditClient.UserAgent = value;
                OAuthClient.UserAgent = value;
                SSLClient.UserAgent = value;
            }
        }

        private static TimeLimiter RateLimiter = null;
        private static readonly RestClient RedditClient = new RestClient("https://www.reddit.com/");
        private static readonly RestClient OAuthClient = new RestClient("https://oauth.reddit.com/");
        private static readonly RestClient SSLClient = new RestClient("https://ssl.reddit.com/");
        private static bool IsOAuth = false;

        public static int MaxTries = 20;
        internal static async Task<Dictionary<string, object>> MakeAPICall(string resource, Method method = Method.GET, params Parameter[] parameters)
        {
            IRestResponse response = null;
            RestRequest request = new RestRequest(resource, method);
            foreach(var parameter in parameters)
            {
                request.AddParameter(parameter);
            }
            RestRequestAsyncHandle handle = null;
            int tries = 0;
            do
            {
                handle = await RateLimiter.Perform<RestRequestAsyncHandle>(
                    () => (
                        IsOAuth ?
                            OAuthClient :
                            RedditClient
                    ).ExecuteAsync(
                        request,
                        r => response = r
                    )
                );
            } while (
                (response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable) &&
                tries < MaxTries
            );

            if (!response.IsSuccessful)
            {
                handle.Abort();
                throw new RedditHttpException(response.StatusCode);
            }

            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                Dictionary<string, object> json = null;
                try
                {
                    json = JSON.Deserialize<Dictionary<string, object>>(response.Content);
                    response = null;
                    if (json.TryGetValue("json", out object newJson) && newJson != null)
                        json = JSON.Deserialize<Dictionary<string, object>>(newJson.ToString());
                    if (json.TryGetValue("errors", out object errors) && errors != null)
                    {
                        switch (errors.ToString())
                        {
                            case "404":
                                throw new RedditException("File Not Found");

                            case "403":
                                throw new RedditException("Restricted");

                            case "invalid_grant":
                                break;

                            default:
                                throw new RedditException($"Error returned by Reddit: {errors}");
                        }
                    }

                    return json;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
                return response.Simplify().ConvertToRecursiveJSON();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RateLimiter;
using Jil;
using Newtonsoft.Json;

namespace FourChanLib
{
    public static class FourChan
    {
        internal static readonly RestClient client = new RestClient("https://a.4cdn.org/");
        internal static readonly TimeLimiter rateLimit = TimeLimiter.Compose(
            new CountByIntervalAwaitableConstraint(1, TimeSpan.FromSeconds(1))
        );
        internal static Dictionary<string, string> troll_flags = null;
        internal static IEnumerable<Board> boards = null;
        
        internal static T MakeRequest<T>(string resource, Method method = Method.GET)
        {
            return rateLimit.Perform<T>(
                () =>
                {
                    return JsonConvert.DeserializeObject<T>(
                        client.Execute(
                            new RestRequest(
                                resource,
                                method
                            )
                        ).Content
                    );
                }
            ).Result;
        }

        internal static bool TryMakeRequest<T>(string resource, string ifModifiedSince, out T content, Method method = Method.GET)
        {
            IRestResponse response = rateLimit.Perform<IRestResponse>(
                () =>
                {
                    return client.Execute(
                        new RestRequest(
                            resource,
                            method
                        ).AddOrUpdateParameter(
                            "If-Modified-Since",
                            ifModifiedSince,
                            ParameterType.HttpHeader
                        )
                    );
                }
            ).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                content = default(T);
                return false;
            }
            else
            {
                content = JsonConvert.DeserializeObject<T>(
                    response.Content
                );
                return true;
            }
        }

        public static IEnumerable<Board> GetBoards()
        {
            if (boards != null)
                return boards;
            GetBoardsResponse boardsResponse = MakeRequest<GetBoardsResponse>("boards.json");
            troll_flags = boardsResponse.troll_flags;
            boards = boardsResponse.boards;
            return boards;
        }

        internal static Dictionary<string, string> GetTrollFlags()
        {
            if (troll_flags != null)
                return troll_flags;
            GetBoardsResponse boardsResponse = MakeRequest<GetBoardsResponse>("boards.json");
            troll_flags = boardsResponse.troll_flags;
            boards = boardsResponse.boards;
            return troll_flags;
        }

        internal struct GetBoardsResponse
        {
            public IEnumerable<Board> boards { get; set; }
            public Dictionary<string, string> troll_flags { get; set; }
        }
    }
}

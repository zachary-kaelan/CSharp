using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RateLimiter;
using Jil;

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
                () => JSON.Deserialize<T>(
                    client.Execute(
                        new RestRequest(
                            resource,
                            method
                        )
                    ).Content
                )
            ).Result;
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

        public static Dictionary<string, string> GetTrollFlags()
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

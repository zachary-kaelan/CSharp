using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using PPLib;

namespace MusicAnalysis
{
    public static class SpotifyClient
    {
        public const string MAIN_PATH = @"E:\Spotify\";
        public const string TRACKS_PATH = MAIN_PATH + @"Tracks\";

        private static readonly RestClient client = new RestClient("https://api.spotify.com/v1/");
        private static readonly RestClient tokenClient = new RestClient("https://accounts.spotify.com/api/token");
        private static readonly RestRequest tokenRequest = new RestRequest(Method.POST);
        private static string accessToken = null;
        static SpotifyClient()
        {
            client.AddDefaultHeader("Authorization", "");
            client.AddDefaultParameter("limit", "50", ParameterType.QueryString);
            tokenRequest.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            tokenRequest.AddHeader("Authorization", "Basic OGZkYWEzZThiYTA2NGIwNjgzNWE4NjZhNWU5ZjI4MmQ6NTgwYzZkOGM5N2NiNDIyMjk1NGY4YTk5ZDg0MTEzMTY=");
            GetToken();
        }

        private static void GetToken()
        {
            accessToken = JSON.DeserializeDynamic(tokenClient.Execute(tokenRequest).Content).access_token;
            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + accessToken);
        }

        public static void GetAllTracks()
        {
            string[] genres = File.ReadAllLines(@"E:\Spotify\Genres.txt");
            StreamWriter genresWriter = new StreamWriter(@"E:\Spotify\CompletedGenres.txt", true);
            foreach(string genre in genres)
            {
                string genrePath = TRACKS_PATH + genre + @"\";
                if (!Directory.Exists(genrePath))
                    Directory.CreateDirectory(genrePath);

                var request = new RestRequest("search", Method.GET);
                request.AddQueryParameter("q", "genre:" + genre);
                request.AddQueryParameter("type", "track");
                request.AddQueryParameter("market", "US");
                request.AddQueryParameter("offset", Directory.EnumerateFiles(genrePath).Count().ToString());
                var tracks = PageThroughAll<Track, TrackModel>(
                    request, t => new TrackModel()
                    {
                        Artists = t.artists.Select(a => a.id),
                        Album = t.album.id,
                        Duration = t.duration_ms,
                        Explicit = t.is_explicit,
                        ID = t.id,
                        Name = t.name,
                        Popularity = t.popularity,
                        Preview = t.preview_url
                    }
                );

                foreach (var track in tracks)
                {
                    track.SaveAs(genrePath + track.ID + ".txt");
                }
                tracks = null;
                genresWriter.WriteLine(genre);
            }
            genresWriter.Close();
            genresWriter = null;
            genres = null;
        }

        private static IEnumerable<TResult> PageThroughAll<TInput, TResult>(RestRequest requestTemp, Func<TInput, TResult> func)
        {
            var parameters = requestTemp.Parameters.ToArray();
            foreach(var param in parameters)
            {
                client.AddDefaultParameter(param);
            }
            string resource = requestTemp.Resource;
            var method = requestTemp.Method;
            requestTemp = null;

            Pager<TInput> pager = new Pager<TInput>();
            int offset = 0;
            do
            {
                RestRequest request = new RestRequest(resource, method);

                var response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    GetToken();
                    response = client.Execute(request);
                }

                if ((int)response.StatusCode == 429)
                {
                    Thread.Sleep(Convert.ToInt32(response.Headers.Single(h => h.Name == "Retry-After").Value) + 1);
                    response = client.Execute(request);
                }

                pager = JSON.Deserialize<Dictionary<string, Pager<TInput>>>(response.Content).Single().Value;
                var items = pager.items.Select(func);
                foreach(var item in items)
                {
                    yield return item;
                }
                items = null;
                offset += 50;
                client.RemoveDefaultParameter("offset");
                client.AddDefaultParameter("offset", offset, ParameterType.QueryString);
                response = null;
            } while (offset < pager.total);

            pager = new Pager<TInput>();
            yield break;
        }

        private static IEnumerable<T> PageThroughAll<T>(RestRequest request)
        {
            return PageThroughAll<T, T>(request, i => i);
        }
    }
}

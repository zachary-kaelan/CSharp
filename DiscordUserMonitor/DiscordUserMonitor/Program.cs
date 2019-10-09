using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using RestSharp;

namespace DiscordUserMonitor
{
    class Program
    {
        private const string USERID = "364910087757889537";

        private static RestClient _client = new RestClient("https://discordapp.com/api/v6/")
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36",
            CookieContainer = new System.Net.CookieContainer()
        };

        static void Main(string[] args)
        {
            StreamWriter writer = new StreamWriter("log.txt") { AutoFlush = true };

            _client.CookieContainer.Add(new System.Net.Cookie("__cfduid", "daf8a1c1a365b60067303605ec6344f951535382336", "/", "discordapp.com"));
            _client.CookieContainer.Add(new System.Net.Cookie("locale", "en-US", "/", "discordapp.com"));
            _client.AddDefaultHeader("Authorization", "NTg4ODIyNzM0NTc0ODQ1OTc0.XTo9cg.lszxPzISjywh2M3bZrBC2-bqpfA");
            _client.RemoveDefaultParameter("Accept");
            _client.AddDefaultHeader("Accept", "*/*");

            RestClient _imageClient = new RestClient("https://cdn.discordapp.com/avatars/" + USERID + "/");

            User lastUser = null;

            while (true)
            {
                var user = JSON.Deserialize<User>(_client.Execute(new RestRequest("users/" + USERID)).Content);

                if (lastUser == null || user.username != lastUser.username)
                {
                    SystemSounds.Exclamation.Play();

                    Console.WriteLine("Username changed: " + user.username);
                    writer.WriteLine("Username changed: " + user.username);
                }

                if (lastUser == null || user.avatar != lastUser.avatar)
                {
                    SystemSounds.Exclamation.Play();

                    Console.WriteLine("Avatar changed: " + user.avatar);
                    writer.WriteLine("Avatar changed: " + user.avatar);
                    File.WriteAllBytes(
                        user.avatar + ".png",
                        _imageClient.DownloadData(
                            new RestRequest(user.avatar + ".png").AddParameter("size", 256)
                        )
                    );
                }

                lastUser = user;

                Thread.Sleep(5000);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;
using RestSharp.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Jil;

using RiotAPI.UNOFFICIAL_SPEC_API;

namespace RiotAPITesting
{
    class Program
    {
        static void Main(string[] args)
        {
            BlowfishEngine engine = new BlowfishEngine();
            RestClient client = new RestClient("http://spectator.na.lol.riotgames.com:80/observer-mode/rest/");
            JSON.Deserialize<Game[]>(
                JSON.DeserializeDynamic(
                    client.Execute(new RestRequest("featured", Method.GET)).Content
                ).gameList
            );
        }
    }
}

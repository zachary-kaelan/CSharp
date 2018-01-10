using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using System.Net;
using System.Net.Http;
using CryptSharp;
using CryptSharp.Utility;

namespace LeagueOb
{
    class Program
    {
        static void Main(string[] args)
        {
            RestClient client = new RestClient("http://spectator.na.lol.riotgames.com:80/observer-mode/rest/");
            Game[] games = JSON.Deserialize<KeyValuePair<string, Game[]>>(
                client.Execute(
                    new RestRequest("featured", Method.GET)
                ).Content
            ).Value;

            string gameid = games[0].gameId.ToString();
            string key = games[0].observers[0].encryptionKey;
            ChunkInfo chunk = JSON.Deserialize<ChunkInfo>(
                client.Execute(
                    new RestRequest("getLastChunkInfo/NA1/" + gameid + "/1/token")
                ).Content
            );

            HttpWebRequest request = HttpWebRequest.CreateHttp(
                "http://spectator.na.lol.riotgames.com:80/observer-mode/rest/consumer/getGameDataChunk/NA1/" + gameid + "/" + chunk.chunkId.ToString() + "/token"
            );

            var response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            
        }
    }

    public struct Game
    {
        public int gameId;
        public int mapId;
        public string gameMode;
        public string gameType;
        public int gameQueueConfigId;
        public Participant[] participants;
        public Observer[] observers;
        public string platformId;
        public int gameTypeConfigId;
        public BannedChamp[] bannedChampions;
        public long gameStartTime;
        public int gameLength;
    }

    public struct Participant
    {
        int teamId;
        int spell1Id;
        int spell2Id;
        int championId;
        int skinIndex;
        int profileIconId;
        string summonerName;
        bool bot;
    }

    public struct Observer
    {
        public string encryptionKey;
    }

    public struct BannedChamp
    {
        int championId;
        int teamId;
        int pickTurn;
    }

    public struct ChunkInfo
    {
        public byte chunkId;
        public int availableSince;
        public int nextAvailableChunk;
        public byte keyFrameId;
        public byte nextChunkId;
        public byte endStartupChunkId;
        public byte startGameCunkId;
        public byte endGameChunkId;
        public int duration;
    }
}

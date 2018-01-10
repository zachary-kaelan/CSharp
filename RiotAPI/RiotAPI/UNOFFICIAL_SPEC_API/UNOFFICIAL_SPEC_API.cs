using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using System.Net;
using CryptSharp;
using CryptSharp.Utility;

namespace RiotAPI.UNOFFICIAL_SPEC_API
{
    public class GameList
    {
        public List<Game> gameList { get; set; }
        public RestClient client { get; set; }
        private readonly CookieContainer Cookies = new CookieContainer();

        public GameList()
        {
            client = new RestClient("http://spectator.na.lol.riotgames.com:80/observer-mode/rest/");
            client.CookieContainer = this.Cookies;
            this.Refresh();
        }

        public void Refresh()
        {
            string content = client.Execute(new RestRequest("featured", Method.GET)).Content;
            this.gameList = JSON.Deserialize<KeyValuePair<string, Game[]>>(content).Value.ToList();
        }

        public void GetLastChunkInfo(string gameid)
        {
            string content = client.Execute(
                new RestRequest(
                    "consumer/getLastChunkInfo/NA1/" + gameid + "/1/token",
                    Method.GET
                )
            ).Content;

            string chunkid = JSON.DeserializeDynamic(content).chunkid;
        }

        private string GetGameDataChunk(string gameid, string chunkid)
        {
            string content = client.Execute(
                new RestRequest(
                    "consumer/getGameDataChunk/NA1/" + gameid + "/" + chunkid + "/token",
                    Method.GET
                )
            ).Content;

            var cipher = BlowfishCipher.Create(
                Encoding.ASCII.GetBytes(gameid)
            );

            int offset = 0;
            throw new NotImplementedException();
            //while (offset < )
        }
    }

    public class ChunkParser
    {
        const string tb = "0123456789abcdef"; // 0-15 to hex
        
        public static string HexDump(string bin)
        {
            string s = "";
            for (int i = 1; i <= bin.Length; ++i)
            {
                byte b = (byte)bin[i - 1];
                s = s.PadRight(1, tb[b >> 4]);
                s = s.PadRight(1, tb[b & 0xf]);
                if (i % 2 == 0)
                    s = s.PadRight(1);

                if (1 == bin.Length && i % 16 > 0)
                {
                    for (int j = i; Convert.ToBoolean(j & 0xf); ++j)
                    {
                        s = s.PadRight(
                            j % 2 != 0 ? 3 : 2
                        );
                    }
                    i = (i + 16) & ~0xf;
                }

                if(i % 16 == 0)
                {
                    for (int j = i-15; j <= i && j <= bin.Length; ++j)
                    {
                        s = s.PadRight(
                            !Char.IsControl(bin[j-1]) ? bin[j-1] : '.'
                        );
                    }
                    s = s.PadRight(1, '\n');
                }
            }

            return s;
        }

        class Block
        {
            byte flags;
            byte channel;
            float timestamp;
            short type;
            string param;
            string content;

            Block()
            {
                Reset();
            }

            void Reset()
            {
                flags = 0;
                channel = 0;
                timestamp = 0.0f;
                type = 0;
                param = String.Empty;
                content = string.Empty;
            }

            enum FLAG {
                FLAG_ONE_BYTE_CONTENT_LENGTH = 1 << 0,
                FLAG_ONE_BYTE_PARAM = 1 << 1,
                FLAG_SAME_TYPE = 1 << 2,
                FLAG_RELATIVE_TIME = 1 << 3
            };

            bool HasFlag(int f)
            {
                return Convert.ToBoolean(flags & f);
            }

            void Dump(StreamWriter os)
            {
                os.Write("(Block) Type");
                os.Write((int)type);
                os.Write(" Flags ");
                os.Write((int)flags);
                os.Write(" Channel ");
                os.Write((int)channel);
                os.Write(" Timestamp ");
                os.Write(timestamp);
                os.Write(" ContentLength ");
                os.Write(content.Length);
                os.WriteLine();
                os.Write(ChunkParser.HexDump(content));
            }
        }

        class Parser
        {
            private StreamReader inputStream;
            private Action<Block> handler;
            private Block last;
            private Block curr;
            private bool is_first;

            public void Parse()
            {
                //while (ParseOne()) ;

            }

            public void SetHander(Action<Block> h)
            {
                handler = h;
            }

            private void ParseOne()
            {
                Block blk = curr;
                byte marker;
                //if (!inputStream.Read(new char[] { (char)marker }, marker, 1))
                    //return false;

            }
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
        string encryptionKey;
    }

    public struct BannedChamp
    {
        int championId;
        int teamId;
        int pickTurn;
    }
}

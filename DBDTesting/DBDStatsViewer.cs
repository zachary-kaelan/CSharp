using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Windows.Forms;
using SAM.API;
using SAM.API.Types;
using APITypes = SAM.API.Types;
using SAM.Game;
using Stats = SAM.Game.Stats;

namespace DBDTesting
{
    class DBDStatsViewer
    {
        private const uint GAME_ID = 381210;
        private Client _SteamClient;
        private GameInfo Game;

        private SAM.API.Callbacks.AppDataChanged _AppDataChangedCallback;
        private SAM.API.Callbacks.UserStatsReceived _UserStatsReceivedCallback;

        private readonly BackgroundWorker Worker = new BackgroundWorker();

        private readonly List<Stats.StatDefinition> _StatDefinitions = new List<Stats.StatDefinition>();

        private readonly List<Stats.AchievementDefinition> _AchievementDefinitions =
            new List<Stats.AchievementDefinition>();

        //private readonly List<Stats.StatInfo> _Statistics = new List<Stats.StatInfo>();
        private readonly Dictionary<string, float> _Statistics = new Dictionary<string, float>();
        private readonly Dictionary<string, float> _PrevStats = new Dictionary<string, float>();

        public DBDStatsViewer()
        {
            if (Steam.GetInstallPath() == Application.StartupPath)
            {
                Console.WriteLine("This tool declines to being run from the Steam directory.");
                return;
            }

            _SteamClient = new Client();
        }

        public void GetGameInfo()
        {
            try
            {
                if (_SteamClient.Initialize(0) == false)
                {
                    Console.WriteLine("Steam is not running. Please start Steam then run this tool again.");
                    return;
                }
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("You've caused an exceptional error!");
                return;
            }

            this._AppDataChangedCallback = _SteamClient.CreateAndRegisterCallback<SAM.API.Callbacks.AppDataChanged>();
            this._AppDataChangedCallback.OnRun += this.OnAppDataChanged;

            Worker.DoWork += new DoWorkEventHandler(GetGames);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetGamesCompleted);

            Worker.RunWorkerAsync();
            Thread.Sleep(500);
            _SteamClient.RunCallbacks(false);
        }

        public void RefreshGameStats()
        {
            if (this._SteamClient.SteamUserStats.RequestCurrentStats() == false)
                Console.WriteLine("Failed.");
        }

        public void GetGameStats()
        {
            if (!_SteamClient.Initialize(GAME_ID))
            {
                Console.WriteLine("Steam is not running.");
                return;
            }

            this._UserStatsReceivedCallback = _SteamClient.CreateAndRegisterCallback<SAM.API.Callbacks.UserStatsReceived>();
            this._UserStatsReceivedCallback.OnRun += this.OnUserStatsReceived;

            Thread.Sleep(2500);
            _SteamClient.RunCallbacks(false);
        }

        private void GetGamesCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _SteamClient.RunCallbacks(false);

            string idString = Game.Id.ToString(CultureInfo.InvariantCulture);
            _SteamClient = new Client();
            long.TryParse(idString, out long appId);

            if (!_SteamClient.Initialize(appId))
            {
                Console.WriteLine("Steam is not running.");
                return;
            }

            this._UserStatsReceivedCallback = _SteamClient.CreateAndRegisterCallback<SAM.API.Callbacks.UserStatsReceived>();
            this._UserStatsReceivedCallback.OnRun += this.OnUserStatsReceived;

            if (this._SteamClient.SteamUserStats.RequestCurrentStats() == false)
                Console.WriteLine("Failed.");

            Thread.Sleep(2500);
            _SteamClient.RunCallbacks(false);
        }

        private void OnAppDataChanged(APITypes.AppDataChanged param)
        {
            if (param.Result && param.Id == GAME_ID)
            {
                Game.Name = this._SteamClient.SteamApps001.GetAppData(Game.Id, "name");
            }
        }

        private void GetGames(object sender, DoWorkEventArgs e)
        {
            byte[] bytes;
            using (var downloader = new WebClient())
            {
                bytes = downloader.DownloadData(new Uri(string.Format("http://gib.me/sam/games.xml")));
            }

            using (var stream = new MemoryStream(bytes, false))
            {
                var document = new XPathDocument(stream);
                var navigator = document.CreateNavigator();
                var nodes = navigator.Select("/games/game");
                while (nodes.MoveNext())
                {
                    string type = nodes.Current.GetAttribute("type", "");
                    if (String.IsNullOrWhiteSpace(type))
                    {
                        type = "normal";
                    }

                    var id = (uint)nodes.Current.ValueAsLong;
                    if (id == GAME_ID && this._SteamClient.SteamApps003.IsSubscribedApp(id))
                    {
                        var info = new GameInfo(id, type);
                        info.Name = this._SteamClient.SteamApps001.GetAppData(info.Id, "name");
                        info.Logo = this._SteamClient.SteamApps001.GetAppData(info.Id, "logo");
                        Game = info;
                        break;
                    }
                }
            }
        }

        private void OnUserStatsReceived(APITypes.UserStatsReceived param)
        {
            if (param.Result != 1)
            {
                Console.WriteLine(
                    "Error while retrieving stats: {0}",
                    TranslateError(param.Result));
                return;
            }

            if (this.LoadUserGameStatsSchema() == false)
            {
                Console.WriteLine("Failed to load schema.");
                return;
            }

            try
            {
                //this.GetAchievements();
                this.GetStatistics();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when handling stats retrieval.");
                return;
            }

            /*Console.WriteLine(
                "Retrieved {0} achievements and {1} statistics.",
                this._AchievementListView.Items.Count,
                this._StatisticsDataGridView.Rows.Count);*/
        }

        private bool LoadUserGameStatsSchema()
        {
            string path;

            try
            {
                path = Steam.GetInstallPath();
                path = Path.Combine(path, "appcache");
                path = Path.Combine(path, "stats");
                path = Path.Combine(path, string.Format("UserGameStatsSchema_{0}.bin", GAME_ID));

                if (File.Exists(path) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            var kv = KeyValue.LoadAsBinary(path);

            if (kv == null)
            {
                return false;
            }

            var currentLanguage = this._SteamClient.SteamApps003.GetCurrentGameLanguage();
            //var currentLanguage = "german";

            //this._AchievementDefinitions.Clear();
            this._StatDefinitions.Clear();

            var stats = kv[GAME_ID.ToString(CultureInfo.InvariantCulture)]["stats"];
            if (stats.Valid == false ||
                stats.Children == null)
            {
                return false;
            }

            foreach (var stat in stats.Children)
            {
                if (stat.Valid == false)
                {
                    continue;
                }

                var rawType = stat["type_int"].Valid
                                  ? stat["type_int"].AsInteger(0)
                                  : stat["type"].AsInteger(0);
                var type = (APITypes.UserStatType)rawType;
                switch (type)
                {
                    case SAM.API.Types.UserStatType.Invalid:
                        {
                            break;
                        }

                    case APITypes.UserStatType.Integer:
                        {
                            var id = stat["name"].AsString("");
                            string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                            this._StatDefinitions.Add(new Stats.IntegerStatDefinition()
                            {
                                Id = stat["name"].AsString(""),
                                DisplayName = name,
                                MinValue = stat["min"].AsInteger(int.MinValue),
                                MaxValue = stat["max"].AsInteger(int.MaxValue),
                                MaxChange = stat["maxchange"].AsInteger(0),
                                IncrementOnly = stat["incrementonly"].AsBoolean(false),
                                DefaultValue = stat["default"].AsInteger(0),
                                Permission = stat["permission"].AsInteger(0),
                            });
                            break;
                        }

                    case APITypes.UserStatType.Float:
                    case APITypes.UserStatType.AverageRate:
                        {
                            var id = stat["name"].AsString("");
                            string name = GetLocalizedString(stat["display"]["name"], currentLanguage, id);

                            this._StatDefinitions.Add(new Stats.FloatStatDefinition()
                            {
                                Id = stat["name"].AsString(""),
                                DisplayName = name,
                                MinValue = stat["min"].AsFloat(float.MinValue),
                                MaxValue = stat["max"].AsFloat(float.MaxValue),
                                MaxChange = stat["maxchange"].AsFloat(0.0f),
                                IncrementOnly = stat["incrementonly"].AsBoolean(false),
                                DefaultValue = stat["default"].AsFloat(0.0f),
                                Permission = stat["permission"].AsInteger(0),
                            });
                            break;
                        }

                    case APITypes.UserStatType.Achievements:
                    case APITypes.UserStatType.GroupAchievements:
                        {
                            if (stat.Children != null)
                            {
                                foreach (var bits in stat.Children.Where(
                                    b => b.Name.ToLowerInvariant() == "bits"))
                                {
                                    if (bits.Valid == false ||
                                        bits.Children == null)
                                    {
                                        continue;
                                    }

                                    foreach (var bit in bits.Children)
                                    {
                                        string id = bit["name"].AsString("");
                                        string name = GetLocalizedString(bit["display"]["name"], currentLanguage, id);
                                        string desc = GetLocalizedString(bit["display"]["desc"], currentLanguage, "");

                                        this._AchievementDefinitions.Add(new Stats.AchievementDefinition()
                                        {
                                            Id = id,
                                            Name = name,
                                            Description = desc,
                                            IconNormal = bit["display"]["icon"].AsString(""),
                                            IconLocked = bit["display"]["icon_gray"].AsString(""),
                                            IsHidden = bit["display"]["hidden"].AsBoolean(false),
                                            Permission = bit["permission"].AsInteger(0),
                                        });
                                    }
                                }
                            }

                            break;
                        }

                    default:
                        {
                            throw new InvalidOperationException("invalid stat type");
                        }
                }
            }

            return true;
        }

        private void GetStatistics()
        {
            if (this._Statistics.Count > 0)
            {
                if (_PrevStats.Count > 0)
                    _PrevStats.Clear();
                foreach(var stat in _Statistics)
                {
                    _PrevStats.Add(stat.Key, stat.Value);
                }

                this._Statistics.Clear();
            }

            foreach (var rdef in this._StatDefinitions)
            {
                if (string.IsNullOrEmpty(rdef.Id) == true)
                {
                    continue;
                }

                if (rdef is Stats.IntegerStatDefinition)
                {
                    var def = (Stats.IntegerStatDefinition)rdef;

                    int value;
                    if (this._SteamClient.SteamUserStats.GetStatValue(def.Id, out value))
                    {
                        /*this._Statistics.Add(new Stats.IntStatInfo()
                        {
                            Id = def.Id,
                            DisplayName = def.DisplayName,
                            IntValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = def.IncrementOnly,
                            Permission = def.Permission,
                        });*/
                        _Statistics.Add(def.DisplayName, value);
                    }
                }
                else if (rdef is Stats.FloatStatDefinition)
                {
                    var def = (Stats.FloatStatDefinition)rdef;

                    float value;
                    if (this._SteamClient.SteamUserStats.GetStatValue(def.Id, out value))
                    {
                        /*this._Statistics.Add(new Stats.FloatStatInfo()
                        {
                            Id = def.Id,
                            DisplayName = def.DisplayName,
                            FloatValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = def.IncrementOnly,
                            Permission = def.Permission,
                        });*/
                        _Statistics.Add(def.DisplayName, value);
                    }
                }
            }

            CompareStats();
        }

        /*public void PrintStats()
        {
            foreach(var stat in _Statistics)
            {
                Console.WriteLine(stat.Id + " - " + stat.DisplayName);
                Console.WriteLine("\t" + stat.Extra);
                Console.WriteLine("\t" + stat.Value.ToString());
            }
        }*/
        
        public void CompareStats()
        {
            if (_PrevStats.Count == 0)
                return;

            SortedDictionary<string, float> changes = new SortedDictionary<string, float>();
            int maxLength = 0;
            foreach(var stat in _Statistics)
            {
                float other = changes[stat.Key];
                if (stat.Value != other)
                {
                    changes.Add(stat.Key, stat.Value - other);
                    if (stat.Key.Length > maxLength)
                        maxLength = stat.Key.Length;
                }
            }

            maxLength += (maxLength % 4) + 4;

            foreach(var stat in changes)
            {
                Console.WriteLine(stat.Key.PadRight(maxLength) + ": \t" + stat.Value.ToString());
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        private static string TranslateError(int id)
        {
            switch (id)
            {
                case 2:
                    {
                        return "generic error -- this usually means you don't own the game";
                    }
            }

            return id.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetLocalizedString(KeyValue kv, string language, string defaultValue)
        {
            var name = kv[language].AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            if (language != "english")
            {
                name = kv["english"].AsString("");
                if (string.IsNullOrEmpty(name) == false)
                {
                    return name;
                }
            }

            name = kv.AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }

            return defaultValue;
        }
    }
}

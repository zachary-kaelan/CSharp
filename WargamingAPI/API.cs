using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using RateLimiter;
using RestSharp;
using WargamingAPI.Models;
using ZachLib;
using ZachLib.HTTP;

namespace WargamingAPI
{
    public static class API
    {
        static API()
        {
            BLITZ_CLIENT.AddDefaultParameter("application_id", APPLICATION_ID, ParameterType.GetOrPost);
        }

        public static readonly Options OPTS = Options.ExcludeNullsIncludeInherited;
        public const string PATH_MAIN = @"E:\World of Tanks\Blitz API\";
        public const string PATH_MODULES = PATH_MAIN + @"Modules\";
        public const string PATH_CLANS = PATH_MAIN + @"Clans\";
        public const string PATH_VEHICLES = PATH_MAIN + @"Vehicles\";
        public const string PATH_POPULARITIES = PATH_MAIN + @"Popularities\";

        private const string ACCESS_TOKEN = "";
        private const string APPLICATION_ID = "2c6e5fd3d1c23c3595718f3502ff9975";
        private static readonly RestClient BLITZ_CLIENT = new RestClient("https://api.wotblitz.com/wotb/");
        private static readonly TimeLimiter RATE_LIMITER = TimeLimiter.GetFromMaxCountByInterval(10, TimeSpan.FromSeconds(1));

        private static Lazy<SortedDictionary<string, int>> _tankIdByName = new Lazy<SortedDictionary<string, int>>(
            () => new SortedDictionary<string, int>(
                Utils.LoadDictionary(
                    PATH_VEHICLES + "Name-ID Dictionary.txt", Encoding.UTF8
                ).ToDictionary(
                    t => t.Key,
                    t => Convert.ToInt32(t.Value)
                )
            )
        );

        private static Lazy<SortedDictionary<int, BaseVehicle>> _tanks = new Lazy<SortedDictionary<int, BaseVehicle>>(
            () => Utils.LoadJSON<SortedDictionary<int, BaseVehicle>>(PATH_VEHICLES + "Vehicles.txt", Encoding.UTF8)
        );
        private static Lazy<SortedDictionary<int, TankAverages>> _tankAverages = new Lazy<SortedDictionary<int, TankAverages>>(
            () => new SortedDictionary<int, TankAverages>(Utils.LoadCSV<TankAverages>(PATH_MAIN + "TankAverages2.csv", Encoding.UTF8).ToDictionary(t => t.TankID, t => t))
        );

        private static Lazy<SortedDictionary<int, Suspension>> _suspensions = new Lazy<SortedDictionary<int, Suspension>>(
            () => Utils.LoadJSON<SortedDictionary<int, Suspension>>(PATH_MODULES + "Suspensions.txt", OPTS)
        );
        private static Lazy<SortedDictionary<int, Engine>> _engines = new Lazy<SortedDictionary<int, Engine>>(
            () => Utils.LoadJSON<SortedDictionary<int, Engine>>(PATH_MODULES + "Engines.txt", OPTS)
        );
        private static Lazy<SortedDictionary<int, Turret>> _turrets = new Lazy<SortedDictionary<int, Turret>>(
            () => Utils.LoadJSON<SortedDictionary<int, Turret>>(PATH_MODULES + "Turrets.txt", OPTS)
        );
        private static Lazy<SortedDictionary<int, Gun>> _guns = new Lazy<SortedDictionary<int, Gun>>(
            () => Utils.LoadJSON<SortedDictionary<int, Gun>>(PATH_MODULES + "Guns.txt")
        );
        private static Lazy<SortedDictionary<int, PopularityProfile>> _profiles = new Lazy<SortedDictionary<int, PopularityProfile>>(
            () => Utils.LoadJSON<SortedDictionary<int, PopularityProfile>>(PATH_POPULARITIES + "Profiles.txt")
        );
        private static Lazy<SortedDictionary<int, double>> _camoValues = new Lazy<SortedDictionary<int, double>>(
             
        );

        public static SortedDictionary<string, int> TANK_ID_BY_NAME { get => _tankIdByName.Value; }
        public static SortedDictionary<int, BaseVehicle> TANKS { get => _tanks.Value;  }
        public static SortedDictionary<int, TankAverages> TANK_AVERAGES { get => _tankAverages.Value; }
        public static SortedDictionary<int, PopularityProfile> POPULARITY_PROFILES { get => _profiles.Value; }

        public static SortedDictionary<int, Suspension> SUSPENSIONS { get => _suspensions.Value; }
        public static SortedDictionary<int, Engine> ENGINES { get => _engines.Value; }
        public static SortedDictionary<int, Turret> TURRETS { get => _turrets.Value; }
        public static SortedDictionary<int, Gun> GUNS { get => _guns.Value; }

        public static Request<T> Get<T>(string resource, params Parameter[] parameters)
        {
            return JSON.Deserialize<Request<T>>(
                RATE_LIMITER.Perform<string>(
                    () => BLITZ_CLIENT.Execute(
                        new RestRequest(resource, Method.GET).AddOrUpdateParameters(parameters)
                    ).Content
                ).Result
            );
        }

        public static T GetSingleObject<T>(string resource, params Parameter[] parameters)
        {
            return JSON.Deserialize<T>(
                JSON.Deserialize<Request>(
                    RATE_LIMITER.Perform<string>(
                        () => BLITZ_CLIENT.Execute(
                            new RestRequest(resource, Method.GET).AddOrUpdateParameters(
                                parameters
                            )
                        ).Content
                    ).Result
                ).data.First().Value.ToString(),
                Options.ExcludeNulls
            );
        }

        public static T GetObject<T>(string resource, params Parameter[] parameters) => Get<T>(resource, parameters).data;

        public static Parameter GetAccountIDParameter(int account_id)
        {
            return new Parameter()
            {
                Name = "account_id",
                Value = account_id,
                Type = ParameterType.GetOrPost
            };
        }

        public static Parameter GetTankIDParameter(int tank_id)
        {
            return new Parameter()
            {
                Name = "tank_id",
                Value = tank_id,
                Type = ParameterType.GetOrPost
            };
        }

        #region GetPlayerVehicleStats
        public static PlayerVehicleStats[] GetPlayerVehicleStats(int account_id)
        {
            return GetSingleObject<PlayerVehicleStats[]>(
                "tanks/stats/",
                GetAccountIDParameter(account_id)
            );
        }

        public static PlayerVehicleStats GetPlayerVehicleStats(int account_id, int tank_id)
        {
            return GetSingleObject<PlayerVehicleStats>(
                "account/tankstats/",
                GetTankIDParameter(tank_id),
                GetAccountIDParameter(account_id)
            );
        }
        #endregion

        public static VehicleAwards[] GetPlayerVehicleAwards(int account_id)
        {
            return GetSingleObject<VehicleAwards[]>("tanks/achievements/", GetAccountIDParameter(account_id));
        }
        public static VehicleAwards GetPlayerAllAwards(int account_id)
        {
            return GetSingleObject<VehicleAwards>("account/achievements/", GetAccountIDParameter(account_id));
        }

        public static int[] GetClanMemberIDs(int clanID)
        {
            return GetSingleObject<Dictionary<string, int[]>>(
                "clans/info/",
                new Parameter()
                {
                    Name = "clan_id",
                    Value = clanID,
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "fields",
                    Value = "members_ids",
                    Type = ParameterType.GetOrPost
                }
            ).First().Value;
        }

        public static Dictionary<int, ClanMember> GetClanMembers(int clanID)
        {
            return GetSingleObject<Dictionary<string, Dictionary<int, ClanMember>>>(
                "clans/info/",
                new Parameter()
                {
                    Name = "clan_id",
                    Value = clanID,
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "extra",
                    Value = "members",
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "fields",
                    Value = "members",
                    Type = ParameterType.GetOrPost
                }
            ).First().Value;
        }

        public static FullPlayerInfo GetPlayerInfo(int account_id)
        {
            return GetSingleObject<FullPlayerInfo>(
                "account/info/",
                new Parameter()
                {
                    Name = "account_id",
                    Value = account_id,
                    Type = ParameterType.GetOrPost
                }
            );
        }

        public static VehicleProfile GetVehicleProfile(int tank_id, string profile_id)
        {
            return GetSingleObject<VehicleProfile>(
                "encyclopedia/vehicleprofile/",
                new Parameter()
                {
                    Name = "tank_id",
                    Value = tank_id,
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "profile_id",
                    Value = profile_id,
                    Type = ParameterType.GetOrPost
                }
            );
        }

        public static Dictionary<int, ListVehicleProfile[]> GetVehicleProfiles(string tank_ids, string fields = null)
        {
            IEnumerable<Parameter> parameters = new Parameter[] {
                new Parameter()
                {
                    Name = "tank_id",
                    Value = tank_ids,
                    Type = ParameterType.GetOrPost
                }
            };

            if (!String.IsNullOrWhiteSpace(fields))
                parameters = parameters.Append(
                    new Parameter()
                    {
                        Name = "fields",
                        Value = fields,
                        Type = ParameterType.GetOrPost
                    }
                );

            return GetObject<Dictionary<int, ListVehicleProfile[]>>("encyclopedia/vehicleprofiles/", parameters.ToArray());
        }

        public static Dictionary<string, Player> SearchPlayers(string search, bool isExact = false, int limit = 100)
        {
            return GetObject<ListedPlayer[]>(
                "account/list/",
                new Parameter()
                {
                    Name = "search",
                    Value = search,
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "type",
                    Value = isExact ? "exact" : "startswith",
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "limit",
                    Value = Math.Min(100, limit),
                    Type = ParameterType.GetOrPost
                }
            ).ToDictionary(
                p => p.nickname,
                p => Player.Create(p.account_id)
            );
        }

        public static IEnumerable<BaseClan> SearchClans(string search, string fields)
        {
            List<Parameter> parametersList = new List<Parameter>(4)
            {
                new Parameter()
                {
                    Name = "limit",
                    Value = 100,
                    Type = ParameterType.GetOrPost
                },
                new Parameter()
                {
                    Name = "page_no",
                    Value = 1,
                    Type = ParameterType.GetOrPost
                }
            };
            if (!String.IsNullOrWhiteSpace(search) && search.Length >= 2)
                parametersList.Add(
                    new Parameter()
                    {
                        Name = "search",
                        Value = search,
                        Type = ParameterType.GetOrPost
                    }
                );
            if (!String.IsNullOrWhiteSpace(fields))
                parametersList.Add(
                    new Parameter()
                    {
                        Name = "fields",
                        Value = fields,
                        Type = ParameterType.GetOrPost
                    }
                );
            var parameters = parametersList.ToArray();

            int page_no = 1;
            var request = Get<BaseClan[]>("clans/list/", parameters);
            int count = request.meta["total"];
            var clans = new List<BaseClan>(count);

            while (request.data.Any())
            {
                foreach(var clan in request.data)
                {
                    yield return clan;
                }
                ++page_no;
                parameters[1].Value = page_no;
                request = Get<BaseClan[]>("clans/list/", parameters);
            }

            yield break;
        }

        public static Clan GetClanInfo(int clan_id, bool include_members)
        {
            var parameter = new Parameter()
            {
                Name = "clan_id",
                Value = clan_id,
                Type = ParameterType.GetOrPost
            };
            if (include_members)
                return GetSingleObject<Clan>(
                    "clans/info/",
                    parameter,
                    new Parameter()
                    {
                        Name = "extra",
                        Value = "members",
                        Type = ParameterType.GetOrPost
                    }
                );
            return GetSingleObject<Clan>("clans/info/", parameter);
        }

        public static void UpdateStoredData()
        {
            var modules = GetObject<ModulesRequest>("encyclopedia/modules/");
            new SortedDictionary<int, Gun>(modules.guns.ToDictionary(g => g.module_id)).SaveAs(PATH_MODULES + "Guns.txt", OPTS);
            new SortedDictionary<int, Suspension>(modules.suspensions.ToDictionary(g => g.module_id)).SaveAs(PATH_MODULES + "Suspensions.txt", OPTS);
            new SortedDictionary<int, Turret>(modules.turrets.ToDictionary(g => g.module_id)).SaveAs(PATH_MODULES + "Turrets.txt", OPTS);
            new SortedDictionary<int, Engine>(modules.engines.ToDictionary(g => g.module_id)).SaveAs(PATH_MODULES + "Engines.txt");
            modules.SaveAs(PATH_MODULES + "Modules.txt");

            SortedDictionary<int, BaseVehicle> newVehicles = GetObject<SortedDictionary<int, BaseVehicle>>(
                "encyclopedia/vehicles/",
                new Parameter()
                {
                    Name = "fields",
                    Value = "-default_profile.firepower,-default_profile.maneuverability,-default_profile.protection,-default_profile.shot_efficiency,-default_profile.shells",
                    Type = ParameterType.GetOrPost
                }
            );
            var missingNameIDs = newVehicles.Keys.Except(TANK_ID_BY_NAME.Values);
            foreach(var missingNameID in missingNameIDs)
            {
                TANK_ID_BY_NAME.Add(newVehicles[missingNameID].name, missingNameID);
            }
            TANK_ID_BY_NAME.SaveDictAs(PATH_VEHICLES + "Name-ID Dictionary.txt");

            var newKeys = newVehicles.Keys.Except(TANKS.Keys).ToArray();
            Console.WriteLine("{0} new tanks added.", newKeys.Length);
            foreach(var key in newKeys)
            {
                Console.WriteLine("\t{0} : {1}", key, newVehicles[key].name);
            }
            var oldKeys = API.TANKS.Keys.Except(newVehicles.Keys).ToArray();
            Console.WriteLine("{0} tanks removed.", oldKeys.Length);
            foreach (var key in oldKeys)
            {
                Console.WriteLine("\t{0} : {1}", key, TANKS[key].name);
            }

            newVehicles.SaveAs(PATH_VEHICLES + "Vehicles.txt", OPTS);
        }
    }
}

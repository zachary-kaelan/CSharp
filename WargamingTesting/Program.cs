using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using WargamingAPI;
using WargamingAPI.Models;
using WargamingAPI.XMLModels;
using ZachLib;
using ZachLib.Statistics;
using Jil;
using RestSharp;
using CsvHelper.Configuration;
using WargamingTesting.BlitzStarsXML;
using HtmlAgilityPack;

namespace WargamingTesting
{
    public enum Boosters
    {
        CrewXP,
        FreeXP,
        CombatXP,
        SpareParts,
        Credits
    }

    class Program
    {
        public static readonly Dictionary<int, int> CREW_SKILL_XP_COSTS = new Dictionary<int, int>()
        {
            {1, 25000 },
            {2, 50000 },
            {3, 100000 },
            {4, 200000 },
            {5, 624000 },
            {6, 1000000 },
            {7, 1100000 }
        };

        public static readonly Dictionary<string, Tuple<string, string, string, string>> AWARDS_NAMES = new Dictionary<string, Tuple<string, string, string, string>>()
        {
            {"mediumTank", new Tuple<string, string, string, string>("scout", "Scout", "Free XP", "Medium") },
            {"lightTank", new Tuple<string, string, string, string>("evileye", "Patrol Duty", "Combat XP", "Light") },
            {"heavyTank", new Tuple<string, string, string, string>("warrior", "Top Gun", "Crew XP", "Heavy") },
            {"AT-SPG", new Tuple<string, string, string, string>("supporter", "Confederate", "Spare Parts", "TD") }
        };


        public static readonly Dictionary<Boosters, double> BOOSTERS_WORTH = new Dictionary<Boosters, double>()
        {
            {Boosters.CombatXP, 1.0 / 7.5 },
            {Boosters.Credits, 1.0 / 14 },
            {Boosters.CrewXP, 1.0 / 15},
            {Boosters.SpareParts, 1.0 / 32.5 },
            {Boosters.FreeXP, 1.0 / 15 }
        };

        public struct AwardTankInfo
        {
            public string PlayerName { get; set; }
            public string Tank { get; set; }
            public string Image { get; set; }
            public double MinutesPerAward { get; set; }

            public AwardTankInfo(string player, string tank, string image, double minutesPerAward)
            {
                PlayerName = player;
                Tank = tank;
                Image = image;
                MinutesPerAward = minutesPerAward;
            }
        }

        public const int ACCOUNT_ID = 1000420176;
        public const int CLAN_ID = 38716;

        static void Main(string[] args)
        {
            var camoValues = MatchDefaultCamoValues();
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            List<double[]> xValues = new List<double[]>();
            List<double[]> yValues = new List<double[]>();
            var allStats = API.GetPlayerVehicleStats(ACCOUNT_ID);
            foreach(var tank in allStats)
            {
                
            }

            var namePrefixRegex = new Regex(@"^(?:F|Ch|G|Oth|GB|A|R)\d{2,3}_", RegexOptions.Compiled, TimeSpan.FromMilliseconds(15));
            var nationDirs = Directory.GetDirectories(@"E:\Gaming\Steam\steamapps\common\World of Tanks Blitz\Data\XML\item_defs\vehicles").Where(d => !d.EndsWith("common"));
            var nationTanks = new Dictionary<string, XMLBaseTechTree[]>();
            var tanksByNation = API.TANKS.Values.GroupBy(t => t.nation).ToDictionary(
                g => g.Key, 
                g => g.GroupBy(t => t.tier).ToDictionary(
                    g2 => g2.Key, 
                    g2 => g2.GroupBy(t => t.type).ToDictionary(
                        g3 => g3.Key,
                        g3 => g3.ToArray()
                    )
                )
            );
            var tankXMLInfos = new Dictionary<int, XMLTank>();
            Dictionary<string, XMLBaseTechTree[]> brokenTanks = new Dictionary<string, XMLBaseTechTree[]>();
            foreach(var nationDir in nationDirs)
            {
                string nation = Path.GetFileName(nationDir);
                if (nation == "other")
                    Console.Write(" ");
                Console.WriteLine(nation);
                var nationTanksInfo = tanksByNation[nation];
                var nationBrokenTanks = new List<XMLBaseTechTree>();

                List<XMLBaseTechTree> nationListTanks = new List<XMLBaseTechTree>();
                List<string> nodeNames = new List<string>();
                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(nationDir + @"\list.xml"));
                var xmlNodes = doc.SelectNodes("/root/*").Cast<XmlNode>();
                foreach (var node in xmlNodes)
                {
                    var name = node.Name;
                    if (nodeNames.Contains(name))
                        continue;
                    nodeNames.Add(name);
                    var serializer = new XmlSerializer(typeof(XMLBaseTechTree), new XmlRootAttribute(node.Name));
                    XMLBaseTechTree listedTank = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(ms);
                        writer.Write(node.OuterXml);
                        writer.Flush();
                        ms.Position = 0;
                        listedTank = (XMLBaseTechTree)serializer.Deserialize(ms);
                        writer.Close();
                        writer = null;
                    }
                    listedTank.name = namePrefixRegex.Replace(name, "").Replace("_", " ");
                    nationListTanks.Add(listedTank);
                    Console.Write("\t{0} : ", listedTank.name);

                    int tank_id = 0;
                    var price = Convert.ToInt32(listedTank.price.Value);
                    var tankType = listedTank.tags.Split(' ')[0];
                    BaseVehicle[] candidates = null;
                    Dictionary<string, BaseVehicle[]> levelTanks = null;

                    if (nationTanksInfo.TryGetValue(listedTank.level, out levelTanks) && levelTanks.TryGetValue(tankType, out candidates))
                    {
                        if (candidates.Length == 1)
                            tank_id = candidates.Single().tank_id;
                        else if (candidates.Length > 0)
                        {
                            bool isPremium = listedTank.price.gold != null;
                            var filteredByPremium = isPremium ?
                                candidates.Where(
                                    t => t.is_premium || (t.cost.HasValue && t.cost.Value.price_gold > 0)
                                ) :
                                candidates.Where(
                                    t => !t.is_premium || (t.cost.HasValue && t.cost.Value.price_credit > 0)
                                );

                            var filteredCandidates = (
                                filteredByPremium.Count() == 0 || price == 0 ?
                                    candidates.Where(
                                        t => !t.cost.HasValue || (t.cost.Value.price_credit + t.cost.Value.price_gold) == 0
                                    ) :
                                    filteredByPremium.Where(
                                        t => t.cost.HasValue && (t.cost.Value.price_credit + t.cost.Value.price_gold) == price
                                    )
                            ).ToArray();

                            if (filteredCandidates.Length > 0)
                            {
                                if (filteredCandidates.Length > 1)
                                {
                                    var candidatesDict = filteredCandidates.ToDictionary(t => ZachRGX.NON_ALPHA_NUMERIC.Replace(t.name, "").ToUpper(), t => t.tank_id);
                                    int searchLength = 1;
                                    string searchName = ZachRGX.NON_ALPHA_NUMERIC.Replace(listedTank.name, "").ToUpper();
                                    if (!candidatesDict.TryGetValue(searchName, out tank_id))
                                    {
                                        int nameLength = searchName.Length;
                                        while (candidatesDict.Count > 1)
                                        {
                                            string queryLeft = searchName.Substring(0, searchLength);
                                            string queryRight = searchName.Substring(nameLength - searchLength);
                                            var keys = candidatesDict.Keys.Where(k => !k.StartsWith(queryLeft) || !k.EndsWith(queryRight)).ToArray();
                                            if (keys.Length == candidatesDict.Count)
                                                keys = candidatesDict.Keys.Where(k => !k.StartsWith(queryLeft) && !k.EndsWith(queryRight)).ToArray();
                                            foreach (var key in keys)
                                                candidatesDict.Remove(key);
                                            ++searchLength;
                                        }
                                        if (candidatesDict.Count == 1)
                                            tank_id = candidatesDict.Single().Value;
                                    }
                                }
                                else
                                    tank_id = filteredCandidates.Single().tank_id;

                            }
                        }
                    }

                    if (tank_id == 0)
                    {
                        Console.WriteLine("BROKEN");
                        nationBrokenTanks.Add(listedTank);
                    }
                    else
                    {
                        Console.WriteLine(tank_id);
                        StreamReader reader = new StreamReader(nationDir + "\\" + name + ".xml");
                        serializer = new XmlSerializer(typeof(XMLTank), new XmlRootAttribute("root"));
                        tankXMLInfos.Add(tank_id, (XMLTank)serializer.Deserialize(reader));
                        reader.Close();
                        reader = null;

                        if (candidates.Length == 1)
                        {
                            levelTanks.Remove(tankType);
                            if (levelTanks.Count == 0)
                                nationTanksInfo.Remove(listedTank.level);
                        }
                        else
                            nationTanksInfo[listedTank.level][tankType] = candidates.Where(c => c.tank_id != tank_id).ToArray();
                    }
                }

                nationTanks.Add(nation, nationListTanks.ToArray());
                brokenTanks.Add(nation, nationBrokenTanks.ToArray());
                doc = null;
            }
            tankXMLInfos.SaveAs(API.PATH_VEHICLES + "XMLTanks.txt", API.OPTS);
            nationTanks.SaveAs(API.PATH_VEHICLES + "XMLListedTanksByNation.txt", API.OPTS);
            brokenTanks.SaveAs(API.PATH_VEHICLES + "XMLBrokenTanks.txt", API.OPTS);
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            Dictionary<string, KeyValuePair<int, int>> tierDifferences = new Dictionary<string, KeyValuePair<int, int>>();
            foreach(var tank in API.TANKS.Values)
            {
                tierDifferences.Add(tank.ToString(), new KeyValuePair<int, int>(tank.guns.Max(g => API.GUNS[g].tier), tank.tier));
            }
            foreach(var tierDif in tierDifferences.OrderByDescending(t => t.Value.Key - t.Value.Value).ThenBy(t => t.Value.Value).Take(10))
            {
                Console.WriteLine(tierDif.Key + ": \t" + tierDif.Value.Key.ToString());
            }
            Console.ReadLine();

            var clanMembers = API.GetClanMembers(CLAN_ID);
            Dictionary<int, double> totalHours = new Dictionary<int, double>();
            List<string> lines = new List<string>();
            foreach(var clanMember in clanMembers.Values.OrderBy(m => m.account_name))
            {
                var memberAwards = API.GetPlayerVehicleAwards(clanMember.account_id);
                var memberStats = API.GetPlayerVehicleStats(clanMember.account_id);
                Dictionary<int, Tuple<string, string, int, double>> memberTotals = new Dictionary<int, Tuple<string, string, int, double>>();
                foreach(var vehicle in memberAwards)
                {
                    var tank = API.TANKS[vehicle.tank_id];
                    if (tank.tier < 5)
                        continue;
                    var awards = vehicle.achievements;
                    double totalPremiumHours = 0;
                    int medals = 0;
                    if (awards.TryGetValue("heroesOfRassenay", out medals))
                        totalPremiumHours += medals * 24;
                    if (awards.TryGetValue("warrior", out medals))
                        totalPremiumHours += medals * 0.25;
                    if (awards.TryGetValue("medalLafayettePool", out medals))
                        totalPremiumHours += medals * 9;
                    if (awards.TryGetValue("medalRadleyWalters", out medals))
                        totalPremiumHours += medals;

                    if (totalPremiumHours > 0)
                    {
                        totalPremiumHours = memberStats[tank.tank_id].battle_life_time / totalPremiumHours;
                        if (totalHours.ContainsKey(vehicle.tank_id))
                            totalHours[vehicle.tank_id] += totalPremiumHours;
                        else
                            totalHours.Add(vehicle.tank_id, totalPremiumHours);
                        memberTotals.Add(tank.tank_id, new Tuple<string, string, int, double>(tank.name, tank.nation, tank.tier, totalPremiumHours));
                    }
                }

            }

            var vehicleStats = API.GetPlayerVehicleStats(ACCOUNT_ID);

            /*int count = API.TANKS.Count;
            var keys = API.TANKS.Keys.ToList();
            Dictionary<int, ListVehicleProfile[]> allProfiles = new Dictionary<int, ListVehicleProfile[]>(count);
            for (int i = 0; i < count; i += 25)
            {
                var vehicles = API.GetVehicleProfiles(String.Join(",", keys.GetRange(i, Math.Min(25, count - i))), "-price_xp,-tank_id,-battle_level_range_max,-battle_level_range_min,-protection,-shot_efficiency,-maneuverability,-firepower,-price_credit");
                foreach(var vehicle in vehicles)
                {
                    allProfiles.Add(vehicle.Key, vehicle.Value);
                }
            }
            allProfiles.SaveAs(API.PATH_MAIN + @"Vehicles\AllAPIProfiles.txt");
            Console.WriteLine("Profiles done...");*/

            

            /*var topConsistentDamage = new Dictionary<KeyValuePair<int, int>, double>();
            foreach(var vehicle in allProfiles)
            {
                var vehicleGuns = vehicle.Value.GroupBy(
                    p => p.shells.First().damage,
                    p => new KeyValuePair<int, GunDetailed>(p.gun_id, p.gun),
                    (k, g) => g.OrderBy(gun => gun.Value.aim_time).ThenBy(gun => gun.Value.reload_time).First()
                );

                foreach(var gun in vehicleGuns)
                {
                    double finalValue = gun.Value.reload_time;
                    var capacity = gun.Value.clip_capacity;
                    if (capacity > 1)
                    {
                        double meanShellTime = (((capacity - 1) * gun.Value.clip_reload_time) + gun.Value.reload_time) / capacity;
                        double stdDevShellTime = Math.Sqrt(((Math.Pow(gun.Value.clip_reload_time - meanShellTime, 2.0) * (capacity - 1)) + Math.Pow(gun.Value.reload_time - meanShellTime, 2.0)) / capacity);
                        finalValue = (meanShellTime + stdDevShellTime) / capacity;
                    }
                    topConsistentDamage.Add(new KeyValuePair<int, int>(gun.Key, vehicle.Key), finalValue);
                }
            }
            var top25ConsistentDamage = topConsistentDamage.OrderBy(kv => kv.Value).Take(25);
            foreach(var damage in top25ConsistentDamage)
            {
                var gun = API.GUNS[damage.Key.Key];
                Console.WriteLine(gun.name + " - " + gun.nation + " - " + gun.tier + " ; - ; " + API.TANKS[damage.Key.Value].ToString() + ": \t" + damage.Value.ToString("#.00"));
            }
            Console.ReadLine();*/

            var nationShells = new Dictionary<string, XMLShell[]>();
            foreach (var dir in nationDirs)
            {
                string nation = Path.GetFileName(dir);
                Console.WriteLine(nation);
                List<XMLShell> shells = new List<XMLShell>();
                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(dir + @"\components\shells.xml"));
                var xmlNodes = doc.SelectNodes("/root/*[position()>1]").Cast<XmlNode>();
                foreach(var node in xmlNodes)
                {
                    var serializer = new XmlSerializer(typeof(XMLShell), new XmlRootAttribute(node.Name));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        StreamWriter writer = new StreamWriter(ms);
                        writer.Write(node.OuterXml);
                        writer.Flush();
                        ms.Position = 0;
                        shells.Add((XMLShell)serializer.Deserialize(ms));
                        writer.Close();
                        writer = null;
                    }
                }
                nationShells.Add(nation, shells.ToArray());
                doc = null;
            }
            nationShells.SaveAs(API.PATH_MAIN + @"Modules\XMLShells.xml");
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var meAccountInfo = API.GetPlayerVehicleStats(1000420176).OrderByDescending(v => v.all.max_xp).Take(25);
            foreach(var tank in meAccountInfo)
            {
                Console.WriteLine(API.TANKS[tank.tank_id].name + " - " + tank.all.battles + " - " + tank.mark_of_mastery + ": \t" + tank.all.max_frags + " - " + tank.all.max_xp);
            }
            Console.ReadLine();

            File.WriteAllLines(
                @"E:\World of Tanks\Blitz API\Vehicles\VehiclesXML.xml",
                new string[] { "<root>\r\n" }.Concat(
                    Directory.GetFiles(@"E:\Gaming\Steam\steamapps\common\World of Tanks Blitz\Data\XML\item_defs\vehicles\usa").Where(
                        f => !f.EndsWith("customization.xml") &&
                                !f.EndsWith("list.xml") &&
                                !f.EndsWith("customization.xml")
                    ).SelectMany(
                        f => File.ReadLines(f).Select(l => "\t" + l)
                    )
                ).Append("</root>")
            );
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var tierViewRanges = API.TANKS.Values.GroupBy(
                t => t.tier,
                t => t.turrets.Any() ? t.turrets.Max(tr => API.TURRETS[tr].view_range) : t.default_profile.turret.view_range,
                (k, g) => new KeyValuePair<int, double>(k, g.Average())
            ).ToDictionary();
            //Dictionary<int, double> tierViewRangesAverages = new Dictionary<int, double>();
            foreach(var tier in tierViewRanges)
            {
                Console.WriteLine(tier.Key.ToString() + " - " + tier.Value.ToString("#.00"));
            }
            Console.ReadLine();

            var tierRangeRelativeMax = API.TANKS.Values.ToDictionary(
                t => t,
                t =>
                {
                    var viewRange = t.turrets.Any() ? t.turrets.Max(tr => API.TURRETS[tr].view_range) : t.default_profile.turret.view_range;
                    return (
                        tierViewRanges.TryGetValue(t.tier - 1, out double tierBelow) ?
                        (viewRange / tierBelow) - 1.0 : 0
                    ) + ((viewRange / tierViewRanges[t.tier]) - 1.0) + (
                        tierViewRanges.TryGetValue(t.tier + 1, out double tierAbove) ?
                        (viewRange / tierAbove) - 1.0 : 0
                    );
                        
                }
            ).OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key.tier).Take(25);
            foreach(var tank in tierRangeRelativeMax)
            {
                Console.WriteLine(tank.Key.ToString() + ": \t" + tank.Value.ToString("#.00"));
            }
            Console.ReadLine();

            var viewranges = API.TANKS.Values.Where(t => t.tier <= 5).ToDictionary(
                t => t,
                t => t.turrets.Any() ? t.turrets.Max(tr => API.TURRETS[tr].view_range) : t.default_profile.turret.view_range
            ).OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key.tier).Take(25);
            foreach(var tank in viewranges)
            {
                Console.WriteLine(tank.Key.name + " - " + tank.Key.nation + " - " + tank.Key.tier.ToString() + " - " + tank.Key.is_premium.ToString() + ": " + tank.Value.ToString());
            }
            Console.ReadLine();

            var meInfo = API.GetPlayerInfo(1000420176);
            var meStats = meInfo.GetVehicleStats();
            var meAwards = meInfo.GetVehicleAwards();

            

            /*API.TANK_ID_BY_NAME.OrderBy(kv => kv.Key).SaveDictAs(API.PATH_MAIN + "IDsByNamesSorted.txt");
            var avgs = Utils.LoadCSV<TankAveragesTemp>(
                API.PATH_MAIN + "TankAverages.csv", 
                Encoding.ASCII
            ).ToArray();
            List<TankAverages> avgs2 = new List<TankAverages>(avgs.Length);
            List<TankAveragesTemp> avgsFailed = new List<TankAveragesTemp>();
            foreach(var avg in avgs)
            {
                if (API.TANK_ID_BY_NAME.TryGetValue(avg.Tank, out int id) || API.ASCII_ID_BY_NAME.TryGetValue(avg.Tank, out id))
                {
                    var tank = API.TANKS[id];
                    avgs2.Add(new TankAverages(avg, id, tank.tier, tank.type, tank.nation));
                }
                else
                {
                    Console.WriteLine(avg.Tank + " -:- " + avg.Battles.ToString());
                    avgsFailed.Add(avg);
                }
            }
            var tempTemp = API.TANKS.Keys.Except(avgs2.Select(a => a.TankID)).Select(t => API.TANKS[t]);

            avgs2.SaveCSV(API.PATH_MAIN + "TankAverages2.csv", Encoding.ASCII);
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            var howitzers = API.GUNS.Values.Where(g => g.nation == "germany" && g.shells.Any(s => s.type == ShellType.ARMOR_PIERCING) && g.shells.Any(s => s.type == ShellType.HOLLOW_CHARGE)).OrderBy(g => g.tier).ThenBy(g => g.name);
            foreach(var howitzer in howitzers)
            {
                Console.WriteLine(howitzer.name + " - " + howitzer.module_id + " - " + howitzer.tier);
                Console.WriteLine("\tTanks: " + String.Join(", ", howitzer.tanks.Select(t => API.TANKS[t].name)));
                Console.WriteLine("\tAimTime: " + howitzer.aim_time.ToString());
                Console.WriteLine("\tDispersion: " + howitzer.dispersion.ToString());
                foreach(var shell in howitzer.shells)
                {
                    Console.WriteLine(String.Format("\t{0}: {1} damage, {2} penetration", shell.type, shell.damage, shell.penetration));
                }
            }
            Console.ReadLine();

            //var duplicateKeys = Utils.LoadDictionary(@"E:\World of Tanks\Blitz API\CreditCoefficients.txt").GroupBy(kv => ZachRGX.SYMBOLS.Replace(kv.Key, "").ToLower()).Where(g => g.Count() > 1);
            var creditCoeffs = Utils.LoadDictionary(@"E:\World of Tanks\Blitz API\CreditCoefficients.txt").ToDictionary(kv => ZachRGX.SYMBOLS.Replace(kv.Key, "").ToLower(), kv => Convert.ToDouble(kv.Value) / 100.0);
            var creditCoeffs2 = new Dictionary<int, double>();
            var missing = new List<Tuple<string, int, int>>();
            foreach(var tank in API.TANK_ID_BY_NAME)
            {
                if (creditCoeffs.TryGetValue(ZachRGX.SYMBOLS.Replace(tank.Key, "").ToLower(), out double coeff))
                    creditCoeffs2.Add(tank.Value, coeff);
                else
                    missing.Add(new Tuple<string, int, int>(tank.Key, tank.Value, API.TANKS[tank.Value].tier));
            }
            creditCoeffs2.SaveDictAs(API.PATH_MAIN + "CreditCoeffs.txt");
            Console.WriteLine(String.Join("\r\n", missing.OrderBy(m => m.Item3).Select(m => m.Item2.ToString() + " - " + m.Item1 + " - " + m.Item3)));
            Console.ReadLine();

            /*var averages = Utils.LoadJSON<TankAverages[]>(API.PATH_MAIN + "Averages.txt", API.OPTS);
            var totalBattles = averages.Sum(t => t.all.battles);
            var tankPopularity = averages.GroupBy(
                a => API.TIERS_BY_TANK_ID[a.tank_id],
                (tier, g) =>
                {
                    var tierSum = g.Sum(t => t.all.battles);
                    return new KeyValuePair<int, Dictionary<int, double>>(
                        tier, g.ToDictionary(
                            t => t.tank_id,
                            t => t.all.battles
                        )
                    );
                }
            ).ToDictionary();
            tankPopularity.SaveAs(API.PATH_MAIN + "TanksPopularity.txt", API.OPTS);
            
            foreach(var tier in tankPopularity)
            {
                foreach(var tank in tier.Value)
                {

                }
            }

            var battleHero = new Dictionary<string, string>()
            {
                { "mediumTank", "scout" },
                { "lightTank", "evileye" },
                { "heavyTank", "warrior" },
                { "AT-SPG", "supporter" }
            };*/
            

            /*var avg = tankBattles.Average();
            var stdDev = Math.Sqrt(tankBattles.Average(b => Math.Pow(b - avg, 2.0)));*/

            StreamWriter disbandedClans = new StreamWriter(API.PATH_MAIN + "DisbandedClans.txt") { AutoFlush = true };
            StreamWriter allPlayers = new StreamWriter(API.PATH_MAIN + "AllPlayers.txt");
            foreach(var clan in API.SearchClans("", ""))
            {
                var clanInfo = API.GetClanInfo(clan.clan_id, false);
                if (clanInfo.is_clan_disbanded)
                {
                    disbandedClans.WriteLine(clanInfo.clan_id);
                    continue;
                }
                clanInfo.SaveAs(API.PATH_CLANS + clanInfo.clan_id + ".txt", API.OPTS);
                Console.WriteLine(clanInfo.clan_id);

                foreach(var member in clanInfo.members_ids)
                {
                    allPlayers.WriteLine(member);
                }
                allPlayers.Flush();
            }
            disbandedClans.Close();
            disbandedClans = null;
            allPlayers.Close();
            allPlayers = null;
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var gunTiers = API.GUNS.Values.GroupBy(g => g.tier);
            var gunTiersNormalized = new Dictionary<int, Tuple<string, double, double, double>[]>();
            var gunTiersAll = new List<Tuple<string, double, double, double>>(API.GUNS.Count);
            foreach(var tier in gunTiers)
            {
                int gunsCount = tier.Count();

                double dispersionSum = 0;
                double aimTimeSum = 0;
                foreach (var gun in tier)
                {
                    dispersionSum += gun.dispersion;
                    aimTimeSum += gun.aim_time;
                }
                double dispersionMean = dispersionSum / gunsCount;
                double aimTimeMean = aimTimeSum / gunsCount;

                dispersionSum = 0;
                aimTimeSum = 0;
                foreach (var gun in API.GUNS.Values)
                {
                    dispersionSum += Math.Pow(gun.dispersion - dispersionMean, 2.0);
                    aimTimeSum += Math.Pow(gun.aim_time - aimTimeMean, 2.0);
                }
                double dispersionStdDev = Math.Sqrt(dispersionSum / gunsCount);
                double aimTimeStdDev = Math.Sqrt(aimTimeSum / gunsCount);

                var tierGuns = tier.Select(
                    g => new Tuple<string, double, double, double>(
                        g.name + " - " + g.nation + " - " + g.module_id.ToString(),
                        g.dispersion,
                        g.aim_time,
                        ((dispersionMean - g.dispersion) / dispersionStdDev) + ((aimTimeMean - g.aim_time) / aimTimeStdDev)
                    )
                ).ToArray();
                gunTiersNormalized.Add(tier.Key, tierGuns);
                gunTiersAll.AddRange(tierGuns);
            }

            var top10All = gunTiersAll.OrderByDescending(g => g.Item4).Take(10);
            foreach(var gun in top10All)
            {
                Console.WriteLine(gun);
            }
            top10All = null;
            Console.WriteLine();
            foreach(var tier in gunTiersNormalized.OrderBy(grp => grp.Key))
            {
                Console.WriteLine("Tier " + tier.Key.ToString());
                var top3Tier = tier.Value.OrderByDescending(g => g.Item4).Take(3);
                foreach(var gun in top3Tier)
                {
                    Console.WriteLine("\t{0} - {1} - {2} - {3}", gun.Item1, gun.Item2.ToString("#.000"), gun.Item3.ToString("#.0"), gun.Item4.ToString("#.00"));
                }
            }
            Console.ReadLine();

            

            /*var playersInfo = member_ids.Select(m => API.GetPlayerInfo(m)).ToArray();
            playersInfo.SaveAs(API.PATH_MAIN + "MembersInfo.txt", Options.ExcludeNullsIncludeInherited);
            var vehicles = new Dictionary<int, PlayerVehicleStats[]>();
            foreach(var player in playersInfo)
            {
                var playerVehicles = player.GetVehicleStats();
                vehicles.Add(player.account_id, playerVehicles);
            }*/

            /*var tank_ids = API.TANK_ID_BY_NAME.Values.AsEnumerable();
            int tanksCount = tank_ids.Count();
            var profiles = new Dictionary<int, VehicleProfile[]>().AsEnumerable();
            for (int i = 0; i < tanksCount; i += 25)
            {
                profiles = profiles.Concat(
                    API.GetObject<Dictionary<int, VehicleProfile[]>>(
                        "encyclopedia/vehicleprofiles/",
                        new Parameter()
                        {
                            Name = "tank_id",
                            Value = String.Join(",", tank_ids.Take(25)),
                            Type = ParameterType.GetOrPost
                        }
                    )
                );
                tank_ids = tank_ids.Skip(25);
            }
            profiles.ToDictionary().SaveAs(API.PATH_MAIN + "AllProfiles.txt", Options.ExcludeNullsIncludeInherited);
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/



            /*var profilesTemp = Utils.LoadJSON<Dictionary<int, VehicleProfile[]>>(API.PATH_MAIN + "AllProfiles.txt");
            Dictionary<int, VehicleProfile[]> profiles = profilesTemp.ToDictionary(
                t => t.Key,
                t => t.Value.Select(p => API.GetVehicleProfile(t.Key, p.profile_id)).ToArray()
            );
            profiles.SaveAs(API.PATH_MAIN + "AllTankConfigs.txt", Options.ExcludeNullsIncludeInherited);
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            profiles = Utils.LoadJSON<Dictionary<int, VehicleProfile[]>>(API.PATH_MAIN + "AllTankConfigs.txt").ToDictionary(
                t => t.Key,
                t =>
                {
                    var tankSuspensions = t.Value.Select(p => p.suspension).Distinct();
                    var tankEngines = t.Value.Select(p => p.engine).Distinct();
                    if (t.Key == 5393)
                        Console.WriteLine();
                    var topSuspension = tankSuspensions.OrderByDescending(s => s.tier).ThenByDescending(s => s.traverse_speed).First();
                    var topEngine = tankEngines.OrderByDescending(e => e.tier).ThenByDescending(e => e.power).First();
                    return t.Value.Where(p => p.suspension.name == topSuspension.name && p.engine.name == topEngine.name).OrderBy(p => p.weight).ToArray();
                }
            );
            profiles.SaveAs(API.PATH_MAIN + "AllTankConfigsTopManuv.txt", Options.ExcludeNullsIncludeInherited);
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            
                       

            Dictionary<string, KeyValuePair<int, double>> ramDamages = new Dictionary<string, KeyValuePair<int, double>>();
            //Console.ReadLine();

            var tiers = API.TANKS.GroupBy(t => t.Value.tier).ToDictionary(g => g.Key, g => new Tuple<double, double, double>(g.Average(t => t.Value.default_profile.armor.hull.sides) * 1.1, g.Average(t => t.Value.default_profile.weight), g.Average(t => t.Value.default_profile.hp)));

            /*Dictionary<string, int> creditsByTank = new Dictionary<string, int>();
            TankAverages[] averages = Utils.LoadJSON<TankAverages[]>(@"E:\World of Tanks\Blitz API\Averages.txt");
            foreach(var average in averages)
            {
                var special = average.special;
                var tank = API.TANKS[average.tank_id];
                creditsByTank.Add(
                    tank.name + " - " + tank.nation + " - " + tank.tier.ToString(),
                    Convert.ToInt32((tank.tier * 700) + (6 * tank.tier * special.winrate) + (100 * special.spotsPerBattle))
                );
            }*/

            foreach(var tank in API.TANKS.Values)
            {
                var tier = tiers[tank.tier];
                double combinedWeight = tank.default_profile.weight + tier.Item2;
                double damage = 0.5 * combinedWeight * (tank.default_profile.speed_forward * tank.default_profile.speed_forward);

                double otherDamageRatio = (tank.default_profile.weight / combinedWeight);
                double damageRatio = (1.0 - otherDamageRatio);
                double armorProtection = 1.1 * tank.default_profile.armor.hull.front;

                double damageTaken = (damageRatio * damage * 0.5) - armorProtection;
                double damageGiven = (otherDamageRatio * damage * 0.5) - tier.Item1;

                damage *= (damageTaken / tank.default_profile.hp > damageGiven / tier.Item3) ? ((tier.Item1 + tier.Item3) / damageGiven) : ((tank.default_profile.hp + armorProtection) / damageTaken);
                damageTaken = (damageRatio * damage * 0.5) - armorProtection;
                damageGiven = (otherDamageRatio * damage * 0.5) - tier.Item1;

                damage = (damageGiven - damageTaken) * (damageTaken / tank.default_profile.hp);
                ramDamages.Add(tank.name + " - " + tank.nation + " - " + (tank.tier % 10), new KeyValuePair<int, double>(Convert.ToInt32(damage), damage / tier.Item3));
            }

            var top5ByRatio = ramDamages.OrderByDescending(d => d.Value.Value).Take(5);
            var top3ByTier = ramDamages.GroupBy(
                t => t.Key.Last()
            ).ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(t => t.Value.Key).Take(3)
            ).OrderBy(g => g.Key);

            foreach(var tank in top5ByRatio)
            {
                Console.WriteLine(tank.Key + "\t : \t" + tank.Value.Key.ToString() + " - " + tank.Value.Value.ToString("#.00"));
            }

            Console.WriteLine("\t\t ~~~");

            foreach(var tier in top3ByTier)
            {
                Console.WriteLine(tier.Key);
                foreach(var tank in tier.Value)
                {
                    Console.WriteLine("\t" + tank.Key.Substring(0, tank.Key.Length - 4) + "\t : \t" + tank.Value.Key.ToString() + " - " + tank.Value.Value.ToString("#.00"));
                }
            }
            Console.ReadLine();
        }

        public void GetMemberAwards()
        {
            var members = API.GetClanMembers(38716).Values;
            List<AwardTankInfo> awardInfo = new List<AwardTankInfo>();
            Dictionary<int, int> numAwards = new Dictionary<int, int>();
            Dictionary<int, int> battleLifeTime = new Dictionary<int, int>();
            foreach (var member in members.OrderBy(m => m.account_name))
            {
                var stats = member.GetVehicleStats();
                var awards = member.GetVehicleAwards().ToDictionary(v => v.tank_id, v => v);
                var vehicles = new List<AwardTankInfo>();
                foreach (var vehicle in stats)
                {
                    int huntsman = 0;
                    int orlik = 0;
                    if (vehicle.all.battles > 8 && API.TANKS.TryGetValue(vehicle.tank_id, out BaseVehicle profile))
                    {
                        if (battleLifeTime.ContainsKey(vehicle.tank_id))
                            battleLifeTime[vehicle.tank_id] += vehicle.battle_life_time;
                        else
                            battleLifeTime.Add(vehicle.tank_id, vehicle.battle_life_time);
                        var award = awards[vehicle.tank_id].achievements;
                        if (award.TryGetValue("huntsman", out huntsman) || (profile.type == "lightTank" && award.TryGetValue("medalOrlik", out orlik)))
                        {
                            if (numAwards.ContainsKey(vehicle.tank_id))
                                numAwards[vehicle.tank_id] += huntsman + orlik;
                            else
                                numAwards.Add(vehicle.tank_id, huntsman + orlik);
                            vehicles.Add(
                                new AwardTankInfo(
                                    member.account_name,
                                    profile.name + " - " + profile.nation + " - " + profile.tier.ToString(),
                                    "=IMAGE(\"" + profile.images.preview + "\")",
                                    (double)vehicle.battle_life_time / (huntsman + orlik)
                                )
                            );
                        }
                    }

                }

                awardInfo.AddRange(vehicles.OrderBy(v => v.MinutesPerAward).Take(5));
            }
            List<AwardTankInfo> overall = new List<AwardTankInfo>();
            foreach (var numAward in numAwards)
            {
                var tank = API.TANKS[numAward.Key];
                overall.Add(
                    new AwardTankInfo(
                        "OVERALL",
                        tank.name + " - " + tank.nation + " - " + tank.tier.ToString(),
                        "=IMAGE(\"" + tank.images.preview + "\")",
                        battleLifeTime[numAward.Key] / (double)numAward.Value
                    )
                );
            }
            awardInfo.InsertRange(0, overall.OrderBy(t => t.MinutesPerAward).Take(7));
            awardInfo.SaveCSV(API.PATH_MAIN + "ClanAwards2.csv");
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public static SortedDictionary<int, CamoInfo> MatchDefaultCamoValues()
        {
            Regex RGXGunCamoValue = new Regex(@"^(?<Value>[\d.]*) - \((?<Key>.*)\)$", RegexOptions.Compiled);
            Regex RGXTankInfo = new Regex(@"^\s*(?<Key>[^\/\r\n\t]*)[^0-9]*(?<Value>\d*)", RegexOptions.Compiled);

            //API.UpdateStoredData();
            List<KeyValuePair<int, CamoInfo>> noGunMatches = new List<KeyValuePair<int, CamoInfo>>();
            var camoValues = new SortedDictionary<int, CamoInfo>();
            var camoDoc = new HtmlDocument();
            var gunsByName = new SortedDictionary<string, IEnumerable<KeyValuePair<int, int[]>>>(
                API.GUNS.GroupBy(g => g.Value.name).ToDictionary(
                    grp => grp.Key,
                    grp => grp.Select(g => new KeyValuePair<int, int[]>(g.Key, g.Value.tanks))
                )
            );
            camoDoc.Load(@"E:\World of Tanks\Blitz API\CamoValues.txt");
            var nodes = camoDoc.DocumentNode.SelectNodes("./div/div/div");
            foreach (var node in nodes)
            {
                string name = node.GetAttributeValue("name", null);
                string oldName = name;
                var tankInfo = RGXTankInfo.FromKeyValues(node.SelectSingleNode("./div[2]").InnerText).First();
                string tankType = null;
                if (tankInfo.Key == "SPGs")
                {
                    Console.WriteLine("TANK NOT FOUND: " + name + ", SPG");
                    continue;
                }
                else if (tankInfo.Key.StartsWith("tank"))
                    tankType = "AT-SPG";
                else
                    tankType = tankInfo.Key.Substring(0, tankInfo.Key.IndexOf(' ')) + "Tank";

                var otherNode = node.SelectSingleNode("./div[2]/div[last()]");
                var idAttr = otherNode.GetAttributeValue("id", null);
                int id = Convert.ToInt32(idAttr.Substring(0, idAttr.IndexOf('e')));
                /*string name = node.GetAttributeValue("name", null);
                if (!API.TANK_ID_BY_NAME.TryGetValue(name, out int id))
                {
                    Console.WriteLine(name);
                    continue;
                }*/
                BaseVehicle tank = null;
                if (!API.TANKS.TryGetValue(id, out tank))
                {
                    if (!API.TANK_ID_BY_NAME.TryGetValue(name, out id) || camoValues.ContainsKey(id))
                    {
                        Console.WriteLine("TANK NOT FOUND: " + name + ", " + id.ToString());
                        continue;
                    }
                    else
                        tank = API.TANKS[id];
                }
                else
                {
                    if (name != tank.name)
                        Console.WriteLine("WARNING: Using " + id.ToString() + "'s name \"" + tank.name + "\" instead of the original \"" + name + "\"");
                    name = tank.name;
                }
                if (tank.type != tankType || tank.tier != Convert.ToInt32(tankInfo.Value))
                {
                    Console.WriteLine(
                        String.Format(
                            "Tank {0}, {1}, {2} doesn't match {3}, {4}, {5}",
                            oldName, tankType, tankInfo.Value,
                            tank.name, tank.type, tank.tier
                        )
                    );
                    continue;
                }

                var guns = otherNode.SelectNodes("./div/span").Select(n => n.InnerText);
                bool success = false;
                foreach (var gun in guns)
                {
                    var kv = RGXGunCamoValue.FromKeyValues(gun).First();
                    if (gunsByName.TryGetValue(kv.Key, out IEnumerable<KeyValuePair<int, int[]>> potentialGuns))
                    {
                        try
                        {
                            var gunTemp = potentialGuns.First(g => g.Value.Contains(id));
                            success = true;
                            break;
                        }
                        catch
                        {
                            Console.WriteLine("GUN NOT A MATCH: " + id.ToString() + ", " + name + " - " + kv.Key + " - " + String.Join(", ", potentialGuns.Select(g => g.Key)));
                        }
                    }

                }

                var camo = new CamoInfo(
                    Convert.ToDouble(node.GetAttributeValue("camostationary", "0.0")),
                    Convert.ToDouble(node.GetAttributeValue("camomoving", "0.0")),
                    Convert.ToDouble(node.GetAttributeValue("camoshooting", "0.0"))
                );

                if (success)
                    camoValues.Add(id, camo);
                else
                {
                    Console.WriteLine("NO GUN MATCHES: " + id.ToString() + ", " + name + " - " + String.Join(", ", tank.guns));
                    noGunMatches.Add(new KeyValuePair<int, CamoInfo>(id, camo));
                }
            }

            camoValues.TryAddAll(noGunMatches);
            camoValues.SaveAs(API.PATH_VEHICLES + "CamoValues.txt");
            var missingTanks = API.TANKS.Keys.Except(camoValues.Keys);
            Console.WriteLine("\r\n\r\n\t~~~\r\n\r\n");
            foreach (var tankID in missingTanks)
            {
                var tank = API.TANKS[tankID];
                Console.WriteLine(tank.name + " - " + tank.tank_id.ToString());
            }
            return camoValues;
        }

        public static SortedDictionary<int, KeyValuePair<CamoInfo, Dictionary<int, double>>> MatchCamoValues()
        {
            Regex RGXGunCamoValue = new Regex(@"^(?<Value>[\d.]*) - \((?<Key>.*)\)$", RegexOptions.Compiled);
            Regex RGXTankInfo = new Regex(@"^\s*(?<Key>[^\/\r\n\t]*)[^0-9]*(?<Value>\d*)", RegexOptions.Compiled);
            Regex RGXNoPeriods = new Regex("[.]", RegexOptions.Compiled);

            //API.UpdateStoredData();
            var camoValues = new SortedDictionary<int, KeyValuePair<CamoInfo, Dictionary<int, double>>>();
            var camoDoc = new HtmlDocument();
            camoDoc.Load(@"E:\World of Tanks\Blitz API\CamoValues.txt");
            var nodes = camoDoc.DocumentNode.SelectNodes("./div/div/div");
            //var duplicateGuns = API.GUNS.GroupBy(g => g.Value.name).Where(g => g.Count() > 1);
            var gunsByName = new SortedDictionary<string, IEnumerable<KeyValuePair<int, int[]>>>(
                API.GUNS.GroupBy(g => g.Value.name).ToDictionary(
                    grp => RGXNoPeriods.Replace(grp.Key, ""),
                    grp => grp.Select(g => new KeyValuePair<int, int[]>(g.Key, g.Value.tanks))
                )
            );
            foreach (var node in nodes)
            {
                string name = node.GetAttributeValue("name", null);
                string oldName = name;
                var tankInfo = RGXTankInfo.FromKeyValues(node.SelectSingleNode("./div[2]").InnerText).First();
                string tankType = null;
                if (tankInfo.Key == "SPGs")
                {
                    Console.WriteLine("TANK NOT FOUND: " + name + ", SPG");
                    continue;
                }
                else if (tankInfo.Key.StartsWith("tank"))
                    tankType = "AT-SPG";
                else
                    tankType = tankInfo.Key.Substring(0, tankInfo.Key.IndexOf(' ')) + "Tank";

                var otherNode = node.SelectSingleNode("./div[2]/div[last()]");
                var idAttr = otherNode.GetAttributeValue("id", null);
                int id = Convert.ToInt32(idAttr.Substring(0, idAttr.IndexOf('e')));
                /*string name = node.GetAttributeValue("name", null);
                if (!API.TANK_ID_BY_NAME.TryGetValue(name, out int id))
                {
                    Console.WriteLine(name);
                    continue;
                }*/
                BaseVehicle tank = null;
                if (!API.TANKS.TryGetValue(id, out tank))
                {
                    if (!API.TANK_ID_BY_NAME.TryGetValue(name, out id) || camoValues.ContainsKey(id))
                    {
                        Console.WriteLine("TANK NOT FOUND: " + name + ", " + id.ToString());
                        continue;
                    }
                    else
                        tank = API.TANKS[id];
                }
                else
                {
                    if (name != tank.name)
                        Console.WriteLine("WARNING: Using " + id.ToString() + "'s name \"" + tank.name + "\" instead of the original \"" + name + "\"");
                    name = tank.name;
                }
                if (tank.type != tankType || tank.tier != Convert.ToInt32(tankInfo.Value))
                {
                    Console.WriteLine(
                        String.Format(
                            "Tank {0}, {1}, {2} doesn't match {3}, {4}, {5}",
                            oldName, tankType, tankInfo.Value,
                            tank.name, tank.type, tank.tier
                        )
                    );
                    continue;
                }
                if (id == 62977)
                    Utils.DoNothing();

                double stationary = Convert.ToDouble(node.GetAttributeValue("camostationary", "0.0"));
                double moving = Convert.ToDouble(node.GetAttributeValue("camomoving", "0.0"));
                var guns = otherNode.SelectNodes("./div/span").Select(n => n.InnerText);
                Dictionary<int, double> shootValues = new Dictionary<int, double>();
                foreach (var gun in guns)
                {
                    var kv = RGXGunCamoValue.FromKeyValues(gun).First();
                    if (!gunsByName.TryGetValue(kv.Key, out IEnumerable<KeyValuePair<int, int[]>> potentialGuns))
                        Console.WriteLine("MISSING GUN: " + id.ToString() + ", " + name + " - " + kv.Key + " - " + String.Join(", ", tank.guns));
                    else
                    {
                        try
                        {
                            shootValues.Add(potentialGuns.First(g => g.Value.Contains(id)).Key, Convert.ToDouble(kv.Value));
                        }
                        catch
                        {
                            Console.WriteLine("GUN NOT A MATCH: " + id.ToString() + ", " + name + " - " + kv.Key + " - " + String.Join(", ", potentialGuns.Select(g => g.Key)));
                        }
                    }

                }

                if (!shootValues.Any())
                {
                    Console.WriteLine("NO GUNS FOUND: " + id.ToString() + " - " + name + " - " + idAttr);
                    continue;
                }

                camoValues.Add(
                    id, new KeyValuePair<CamoInfo, Dictionary<int, double>>(
                        new CamoInfo(
                            stationary,
                            moving,
                            Convert.ToDouble(node.GetAttributeValue("camoshooting", "0.0"))
                        ),
                        shootValues
                    )
                );
            }
            camoValues.SaveAs(API.PATH_VEHICLES + "CamoValues.txt");
            var missingTanks = API.TANKS.Keys.Except(camoValues.Keys);
            Console.WriteLine("\r\n~~~\r\n");
            foreach (var tankID in missingTanks)
            {
                var tank = API.TANKS[tankID];
                Console.WriteLine(tank.name + " - " + tank.tank_id.ToString());
                foreach (var gunID in tank.guns)
                {
                    Console.WriteLine("\t" + API.GUNS[gunID].name + " - " + gunID.ToString());
                }
            }
            return camoValues;
        }

        public void GetCreditEarnings()
        {
            Dictionary<string, double> creditEarnings = new Dictionary<string, double>();

            foreach (var tank in API.TANK_AVERAGES.Values)
            {
                creditEarnings.Add(
                    API.TANKS[tank.TankID].name + " - " + tank.TankNation + " - " + tank.TankTier.ToString(),
                    (tank.BattlesPerMinute * (tank.TankTier * 700)) +
                    (tank.WinsPerMinute * (tank.TankTier * 600)) +
                    (tank.SpotsPerBattle * tank.BattlesPerMinute * 100)

                );
            }
        }

        public static void GetPopularities()
        {
            var byTier = API.TANK_AVERAGES.ToLookup(
                v => v.Value.TankTier,
                v => new KeyValuePair<int, TankAverages>(v.Key, v.Value)
            );
            Dictionary<int, double> popularityByTier = byTier.ToDictionary(t => t.Key, t => (double)t.Sum(v => v.Value.Battles - 40));
            var total = popularityByTier.Values.Sum();
            popularityByTier = popularityByTier.ToDictionary(t => t.Key, t => t.Value / total);
            Dictionary<int, double> popularityByTank = new Dictionary<int, double>();
            Dictionary<int, Dictionary<int, double>> popularityByTankAndTier = new Dictionary<int, Dictionary<int, double>>();

            var popularityWithinTier = byTier.ToDictionary(
                t => t.Key,
                t =>
                {
                    double tierTotal = t.Sum(v => v.Value.Battles - 40);
                    double tierFactor = popularityByTier[t.Key];
                    Dictionary<int, double> tierVehicles = new Dictionary<int, double>();
                    Dictionary<int, double> tierVehicles2 = new Dictionary<int, double>();
                    foreach(var vehicle in t)
                    {
                        var pop = (vehicle.Value.Battles - 40) / tierTotal;
                        tierVehicles.Add(vehicle.Key, pop);
                        popularityByTank.Add(vehicle.Key, (vehicle.Value.Battles - 40) / total);
                        tierVehicles2.Add(vehicle.Key, pop * tierFactor);
                    }
                    popularityByTankAndTier.Add(t.Key, tierVehicles2.OrderByDescending(v => v.Value).ToDictionary());
                    return tierVehicles.OrderByDescending(v => v.Value).ToDictionary();
                }
            );
            byTier = null;
            popularityByTank = popularityByTank.OrderByDescending(v => v.Value).ToDictionary();
            popularityByTier = popularityByTier.OrderByDescending(t => t.Value).ToDictionary();

            popularityByTier.SaveDictAs(@"E:\World of Tanks\Blitz API\Popularities\ByTier.txt");
            popularityByTank.SaveDictAs(@"E:\World of Tanks\Blitz API\Popularities\ByTank.txt");
            popularityWithinTier.SaveDictAs(@"E:\World of Tanks\Blitz API\Popularities\WithinTier.txt");
            popularityByTankAndTier.SaveAs(@"E:\World of Tanks\Blitz API\Popularities\ByTankAndTier.txt");

            var popularityProfiles = new Dictionary<int, PopularityProfile>();
            var battlesTimes = new Dictionary<int, KeyValuePair<double, double>>();
            for(int tier = 1; tier <= 10; ++tier)
            {
                int tierBelow = tier - 1;
                int tierAbove = tier + 1;

                var temp = new Dictionary<int, Dictionary<int, double>>();
                if (tier != 1)
                    temp.Add(tierBelow, popularityByTankAndTier[tierBelow]);
                if (tier != 10)
                    temp.Add(tierAbove, popularityByTankAndTier[tierAbove]);
                temp.Add(tier, popularityByTankAndTier[tier]);
                var profileTanks = temp.SelectMany(t => t.Value).ToDictionary();
                temp = null;
                var profileTotal = profileTanks.Values.Sum();

                var profile = new Dictionary<int, PopularityAverages>();
                PopularityAverages totals = new PopularityAverages()
                {
                    TankID = 0,
                    TankTier = 0,
                    TankType = "AVERAGE",
                    TankNation = null
                };
                List<KeyValuePair<double, double>> survivalTimes = new List<KeyValuePair<double, double>>();
                foreach(var tank in profileTanks)
                {
                    var avgs = API.TANK_AVERAGES[tank.Key] * (tank.Value / profileTotal);
                    avgs.TankName = API.TANKS[tank.Key].name;
                    profile.Add(tank.Key, avgs);
                    totals += avgs;
                    if (avgs.TankTier == tier)
                        survivalTimes.Add(new KeyValuePair<double, double>(avgs.SurvivalRate, avgs.MinutesPerBattle));
                }

                var orderedPoints = survivalTimes.OrderBy(s => s.Value).ToArray();
                survivalTimes = null;
                int count = orderedPoints.Length / 2;
                bool isEven = orderedPoints.Length % 2 == 0;
                double xTotal1 = 0;
                double yTotal1 = 0;
                for (int i = 0; i < count; ++i)
                {
                    var point = orderedPoints[i];
                    xTotal1 += point.Key;
                    yTotal1 += point.Value;
                }
                double xTotal2 = 0;
                double yTotal2 = 0;
                for(int i = count + (isEven ? 0 : 1); i < orderedPoints.Length; ++i)
                {
                    var point = orderedPoints[i];
                    xTotal2 += point.Key;
                    yTotal2 += point.Value;
                }
                double slope = (yTotal2 - yTotal1) / (xTotal2 - xTotal1);
                double intercept = 0;
                if (isEven)
                {
                    var point1 = orderedPoints[count - 1];
                    var point2 = orderedPoints[count];
                    intercept = ((point1.Value + point2.Value) / 2) - (((point1.Key + point2.Key) / 2) * slope);
                }
                else
                {
                    var point = orderedPoints[count];
                    intercept = point.Value - (point.Key * slope);
                }

                var battleTime = new KeyValuePair<double, double>(slope, intercept);
                battlesTimes.Add(tier, battleTime);
                // f(0) = intercept; the average battle time for death
                // f(1) = slope + intercept; the average battle time for survival (full match)
                // f(tank.SurvivalRate) / tank.Minutes Per Battle; a factor denoting how much longer a tank drags out matches

                var tierRange = new KeyValuePair<int, int>(
                    Math.Max(1, tierBelow),
                    Math.Min(10, tierAbove)
                );
                popularityProfiles.Add(tier, new PopularityProfile(profile, totals, battleTime, tierRange));
            }

            popularityProfiles.SaveAs(@"E:\World of Tanks\Blitz API\Popularities\Profiles.txt");
        }

        public void GetTopAlphas()
        {
            var allProfiles = Utils.LoadJSON<Dictionary<int, ListVehicleProfile[]>>(@"E:\World of Tanks\Blitz API\Vehicles\AllAPIProfiles.txt");
            var topAlphas = new Dictionary<KeyValuePair<string, int>, Tuple<int, int, double>>();
            var brokenTanks = new Dictionary<int, string>();
            foreach (var vehicle in allProfiles)
            {
                var tank = API.TANKS[vehicle.Key];
                var profiles = new Dictionary<string, Tuple<int, int, double>>();
                //var profiles = vehicle.Value.ToDictionary(p => p.profile_id, p => new Tuple<int, double>(p.gun.clip_capacity * p.shells.First().damage, p.gun.reload_time)).First();
                foreach (var profile in vehicle.Value)
                {
                    if (profile.gun.fire_rate >= 7.5)
                    {
                        var shell = profile.shells.Count() == 1 ? profile.shells.Single() : profile.shells.OrderByDescending(s => s.penetration).ElementAt(1);
                        profiles.Add(profile.profile_id, new Tuple<int, int, double>(profile.gun.clip_capacity * shell.damage, shell.penetration, profile.gun.reload_time));
                    }
                }
                var topGuns = profiles.GroupBy(
                    p => p.Value.Item1,
                    (k, g) => g.OrderByDescending(t => t.Value.Item2).ThenBy(t => t.Value.Item3).First()
                ).OrderByDescending(g => g.Value.Item2).ThenBy(g => g.Value.Item3).Take(3);
                foreach (var topGun in topGuns)
                {
                    try
                    {
                        topAlphas.Add(
                            new KeyValuePair<string, int>(
                                tank.name + " - " + API.GUNS[topGun.Key.Split('-').Select(i => Convert.ToInt32(i)).First(i => tank.guns.Contains(i))].name + " - " + tank.nation + " - " + tank.tier.ToString(),
                                tank.tier
                            ), topGun.Value
                        );
                    }
                    catch
                    {
                        Console.WriteLine(vehicle.Key + " - " + topGun.Key);
                        continue;
                    }
                }
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();
            var topAlphaTiers = topAlphas.GroupBy(
                t => t.Key.Value,
                t => new Tuple<string, int, int, double>(t.Key.Key, t.Value.Item1, t.Value.Item2, t.Value.Item3),
                (k, g) => new KeyValuePair<int, Tuple<string, int, int, double>[]>(k, g.OrderByDescending(t => t.Item2).ThenBy(t => t.Item3).Take(5).ToArray())
            ).OrderBy(kv => kv.Key);
            foreach (var tier in topAlphaTiers)
            {
                foreach (var topAlpha in tier.Value)
                {
                    Console.WriteLine(topAlpha.Item1 + ": \t" + topAlpha.Item2.ToString() + " - " + topAlpha.Item3.ToString() + " - " + topAlpha.Item4.ToString("#.00"));
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }

        public void GetModuleIncompatibilities()
        {
            Dictionary<int, ModulesCombination[]> moduleCombinations = Utils.LoadJSON<Dictionary<int, ModulesCombination[]>>(API.PATH_MAIN + @"Vehicles\ModuleCombos.txt");
            Dictionary<int, Dictionary<int, int[]>> incompatibilities = new Dictionary<int, Dictionary<int, int[]>>();
            ModuleType[] keys = new ModuleType[]
            {
                ModuleType.vehicleGun,
                ModuleType.vehicleEngine,
                ModuleType.vehicleTurret,
                ModuleType.vehicleChassis
            };
            foreach (var tank in API.TANKS.Values)
            {
                var combos = moduleCombinations[tank.tank_id];
                Dictionary<ModuleType, IEnumerable<IGrouping<int, int[]>>> moduleIDs = new Dictionary<ModuleType, IEnumerable<IGrouping<int, int[]>>>()
                {
                    {ModuleType.vehicleGun, combos.GroupBy(c => c.GunID, c => c.ToVector()) },
                    {ModuleType.vehicleEngine, combos.GroupBy(c => c.EngineID, c => c.ToVector()) },
                    {ModuleType.vehicleTurret, combos.GroupBy(c => c.TurretID, c => c.ToVector()) },
                    {ModuleType.vehicleChassis, combos.GroupBy(c => c.SuspensionID, c => c.ToVector()) }
                };

                Dictionary<int, int[]> tankIncompatibilities = new Dictionary<int, int[]>();

                for (int i = 0; i < 4; ++i)
                {
                    foreach (var group in moduleIDs[keys[i]])
                    {
                        List<int> incompatibleIDs = new List<int>();
                        int id = group.Key;
                        for (int j = 0; j < 4; ++j)
                        {
                            if (j != i)
                                incompatibleIDs.AddRange(moduleIDs[keys[j]].Where(g => !g.Any(c => c[i] == id)).Select(g => g.Key));
                        }

                        if (incompatibleIDs.Any())
                            tankIncompatibilities.Add(id, incompatibleIDs.ToArray());
                    }
                }

                if (tankIncompatibilities.Any())
                    incompatibilities.Add(tank.tank_id, tankIncompatibilities);
            }

            var moduleGroups = incompatibilities.SelectMany(kv => kv.Value.Select(v => new Tuple<int, int, int[]>(kv.Key, v.Key, v.Value))).GroupBy(c => new KeyValuePair<int, int[]>(c.Item2, c.Item3));
            var appliesAllTemp = moduleGroups.Where(g => g.Count() > 1).ToList();
            var appliesAll = appliesAllTemp.ToDictionary(g => g.Key.Key, g => g.Key.Value);
            moduleGroups = moduleGroups.Except(appliesAllTemp);
            var allModuleIDs = API.GUNS.Keys.Concat(API.SUSPENSIONS.Keys).Concat(API.TURRETS.Keys).Concat(API.ENGINES.Keys).Except(appliesAll.Keys);
            foreach (int moduleID in allModuleIDs)
            {
                var idIncompatibilities = moduleGroups.Where(g => g.Key.Key == moduleID);
                if (idIncompatibilities.Count() == 1)
                {
                    appliesAll.Add(moduleID, idIncompatibilities.Single().Key.Value);
                    appliesAllTemp.AddRange(idIncompatibilities);
                }
            }
            var remove = appliesAllTemp.SelectMany(g => g).Distinct().GroupBy(r => r.Item1);
            foreach (var removeID in remove)
            {
                var tankIncompatibilities = incompatibilities[removeID.Key];
                foreach (var moduleID in removeID)
                {
                    tankIncompatibilities.Remove(moduleID.Item2);
                }
                if (tankIncompatibilities.Count == 0)
                    incompatibilities.Remove(removeID.Key);
                else
                    incompatibilities[removeID.Key] = tankIncompatibilities;
            }
            incompatibilities.Add(0, appliesAll);
            incompatibilities.SaveAs(API.PATH_MAIN + @"Modules\Incompatibilities.txt");
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public void OrganizeProfiles()
        {
            int count = API.TANKS.Count;
            var keys = API.TANKS.Keys.ToList();
            Dictionary<int, VehicleProfile[]> vehicleProfiles = new Dictionary<int, VehicleProfile[]>(count);
            for (int i = 0; i < count; i += 25)
            {
                var vehicles = API.GetVehicleProfiles(String.Join(",", keys.GetRange(i, Math.Min(25, count - i))), "-price_xp,-tank_id,-battle_level_range_max,-battle_level_range_min,-protection,-shot_efficiency,-maneuverability,-firepower,-price_credit,-engine,-suspension,-shells");
                foreach(var vehicle in vehicles)
                {
                    int length = vehicle.Value.Length;
                    List<ListVehicleProfile> topManuvProfiles = new List<ListVehicleProfile>(length);
                    int maxHorsePower = 0;
                    int maxTraverse = 0;
                    foreach(var profile in vehicle.Value)
                    {
                        bool resetProfilesList = false;
                        if (profile.engine.power > maxHorsePower)
                        {
                            maxHorsePower = profile.engine.power;
                            topManuvProfiles = new List<ListVehicleProfile>();
                            resetProfilesList = true;
                        }

                        if (profile.suspension.traverse_speed > maxTraverse)
                        {
                            maxTraverse = profile.suspension.traverse_speed;
                            if (!resetProfilesList)
                            {
                                topManuvProfiles = new List<ListVehicleProfile>();
                                resetProfilesList = true;
                            }
                        }

                        if (profile.suspension.traverse_speed == maxTraverse && profile.engine.power == maxHorsePower)
                            topManuvProfiles.Add(profile);
                    }


                }
            }
        }

        public void GetBestDispersionFromBlitzStars()
        {
            var bsTanks = Utils.LoadJSON<BSTank[]>(@"E:\World of Tanks\Blitz API\Vehicles\XMLData.txt", Options.ExcludeNullsIncludeInherited);
            var bestDispersion = new Dictionary<string, Tuple<double, double, int>>();
            foreach (var bsTank in bsTanks)
            {
                var turret = bsTank.turretsList.OrderBy(t => t.guns.Min(g => g.shotDispersionRadius)).First();
                var gun = turret.guns.OrderBy(g => g.shotDispersionRadius).First();
                bestDispersion.Add(bsTank.names["en"] + " - " + bsTank.nation + " - " + bsTank.tier.ToString() + " - " + gun.name, new Tuple<double, double, int>(gun.shotDispersionRadius, gun.aimingTime, gun.shots.First().speed));
            }
            foreach (var gun in bestDispersion.OrderBy(g => g.Value.Item1).Take(25))
            {
                Console.WriteLine(gun.Key + ": \t" + gun.Value.Item1.ToString("#.000") + " - " + gun.Value.Item2.ToString("#.0") + " - " + gun.Value.Item3.ToString());
            }
            Console.ReadLine();

            var numbers = File.ReadAllLines(@"E:\World of Tanks\Blitz API\Probabilities\ShellDamageNumbers.txt").Select(n => Convert.ToDouble(n)).ToArray();
            double numMean = 60;
            double numStdDev = Math.Sqrt(numbers.Average(n => (n - numMean) * (n - numMean)));
            int[] stdDevs = new int[5];
            foreach (var number in numbers)
            {
                ++stdDevs[Convert.ToInt32(Math.Ceiling(Math.Abs((number - numMean) / numStdDev)))];
            }
            for (int i = 1; i < 5; ++i)
            {
                Console.WriteLine("{0} - {1} - {2}", i, stdDevs[i], ((double)stdDevs[i] / numbers.Length).ToString("#.000"));
            }
            Console.ReadLine();
        }
    }
}

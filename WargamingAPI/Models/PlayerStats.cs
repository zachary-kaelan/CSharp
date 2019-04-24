using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;
using Jil;

namespace WargamingAPI.Models
{
    public class Player : IEquatable<Player>
    {
        public int account_id { get; protected set; }
        
        public static Player Create(int account_id)
        {
            var player = new Player();
            player.account_id = account_id;
            return player;
        }

        public FullPlayerInfo GetAccountInfo()
        {
            return API.GetPlayerInfo(account_id);
        }

        public PlayerVehicleStats[] GetVehicleStats()
        {
            return API.GetPlayerVehicleStats(account_id);
        }

        public PlayerVehicleStats GetVehicleStats(int tank_id)
        {
            return API.GetPlayerVehicleStats(account_id, tank_id);
        }

        public VehicleAwards[] GetVehicleAwards()
        {
            return API.GetPlayerVehicleAwards(account_id);
        }

        public VehicleAwards GetAllAwards()
        {
            return API.GetPlayerAllAwards(account_id);
        }

        public bool Equals(Player other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return account_id == other.account_id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (Player)obj;
            return other.account_id == account_id;
        }

        public override int GetHashCode()
        {
            return account_id.GetHashCode();
        }
    }

    public sealed class ListedPlayer : Player
    {
        public string nickname { get; private set; }
    }

    public class PlayerData : Player
    {
        public long last_battle_time { get; protected set; }
        public long created_at { get; protected set; }
        public long updated_at { get; protected set; }
    }

    public sealed class PlayerVehicleStats
    {
        public AllBattles all { get; private set; }

        public long last_battle_time { get; private set; }
        public int mark_of_mastery { get; private set; }
        public int battle_life_time { get; private set; }
        public int tank_id { get; private set; }
    }

    public sealed class FullPlayerInfo : PlayerData
    {
        public string nickname { get; private set; }
        public Statistics statistics { get; private set; }
    }

    public class AllBattles
    {
        public double spotted { get; protected set; }
        public double hits { get; protected set; }
        public double frags { get; protected set; }
        public double max_xp { get; protected set; }
        public double wins { get; protected set; }
        public double losses { get; protected set; }
        public double capture_points { get; protected set; }
        public double battles { get; protected set; }
        public double damage_dealt { get; protected set; }
        public double damage_received { get; protected set; }
        public double max_frags { get; protected set; }
        public double shots { get; protected set; }
        public double frags8p { get; protected set; }
        public double xp { get; protected set; }
        public double win_and_survived { get; protected set; }
        public double survived_battles { get; protected set; }
        public double dropped_capture_points { get; protected set; }
        [Jil.JilDirective(true)]
        public double draws { get => battles - (wins + losses); }
        [Jil.JilDirective(true)]
        public double deaths { get => battles - survived_battles; }

        public int CreditsPerBattle(int tier)
        {
            return Convert.ToInt32(((600 * tier) * wins) + (
                (700 * tier) +
                (100 * spotted) +
                (7.5 * damage_dealt)
            ));
        }

        public StatsPerBattle GetStatsPerBattle()
        {
            return new StatsPerBattle(this);
        }

        public struct StatsPerBattle
        {
            [JilDirective]
            public double Spotted { get; private set; }
            public double Hits { get; private set; }
            public double Frags { get; private set; }
            public double CapturePoints { get; private set; }
            public double DamageDealt { get; private set; }
            public double DamageReceived { get; private set; }
            public double ShotsFired { get; private set; }
            public double FragsTier8Plus { get; private set; }
            public double Experience { get; private set; }
            public double DefensePoints { get; private set; }

            public double Accuracy { get; private set; }
            public double KillDeathRatio { get; private set; }
            public double SurvivalRate { get; private set; }
            public double Winrate { get; private set; }
            public double SurvivalWinrate { get; private set; }
            public double DrawRate { get; private set; }
            public double DamageRatio { get; private set; }
            public double DamagePerShot { get; private set; }
            public double DamagePerHit { get; private set; }

            public int NumBattles { get; private set; }

            public StatsPerBattle(AllBattles stats)
            {
                double dblBattles = stats.battles;

                Spotted = stats.spotted / dblBattles;
                Hits = stats.hits / dblBattles;
                Frags = stats.frags / dblBattles;
                CapturePoints = stats.capture_points / dblBattles;
                DamageDealt = stats.damage_dealt / dblBattles;
                DamageReceived = stats.damage_received / dblBattles;
                ShotsFired = stats.shots / dblBattles;
                FragsTier8Plus = stats.frags8p / dblBattles;
                Experience = stats.xp / dblBattles;
                DefensePoints = stats.dropped_capture_points / dblBattles;

                Accuracy = ((double)stats.hits) / stats.shots;
                KillDeathRatio = ((double)stats.frags) / stats.deaths;
                SurvivalRate = stats.survived_battles / dblBattles;
                Winrate = stats.wins / dblBattles;
                SurvivalWinrate = stats.win_and_survived / dblBattles;
                DrawRate = stats.draws / dblBattles;
                DamageRatio = ((double)stats.damage_dealt) / stats.damage_received;
                DamagePerShot = ((double)stats.damage_dealt) / stats.shots;
                DamagePerHit = ((double)stats.damage_dealt) / stats.hits;

                NumBattles = (int)stats.battles;
            }

        }
    }

    public class AllBattlesAllTanks : AllBattles
    {
        public int max_frags_tank_id { get; protected set; }
        public int max_xp_tank_id { get; protected set; }
    }


    public struct Statistics
    {
        public AllBattlesAllTanks clan { get; private set; }
        public AllBattlesAllTanks all { get; private set; }
        public Dictionary<int, int> frags { get; private set; }
    }

    public sealed class VehicleAwards : TankObject
    {
        public Dictionary<string, int> achievements { get; private set; }
        public Dictionary<string, int> max_series { get; private set; }
    }

    public struct Account
    {
        public int AccountID { get; private set; }
        public string Username { get; private set; }
        public DateTime LastBattle { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public ClanInfo Clan { get; private set; }
        public AllBattlesAllTanks Record { get; private set; }
        public Dictionary<int, PlayerVehicleStats> VehicleStats { get; private set; }
        public Dictionary<int, VehicleAwards> VehicleAwards { get; private set; }
        public VehicleAwards AllAwards { get; private set; }

        public Account(ClanMember member, int clan_id)
        {
            AccountID = member.account_id;
            Username = member.account_name;
            Clan = new ClanInfo(member.joined_at, member.role, clan_id);

            var info = member.GetAccountInfo();
            Created = Utils.ConvertUnixTimestamp(info.created_at);
            LastUpdated = Utils.ConvertUnixTimestamp(info.updated_at);
            LastBattle = Utils.ConvertUnixTimestamp(info.last_battle_time);
            Record = info.statistics.all;
            info = null;

            VehicleStats = member.GetVehicleStats().ToDictionary(t => t.tank_id, t => t);
            VehicleAwards = member.GetVehicleAwards().ToDictionary(t => t.tank_id, t => t);
            AllAwards = member.GetAllAwards();
        }

        public struct ClanInfo
        {
            public DateTime JoinedAt { get; private set; }
            public string Role { get; private set; }
            public int ClanID { get; private set; }

            public ClanInfo(long joined_at, string role, int clan_id)
            {
                JoinedAt = Utils.ConvertUnixTimestamp(joined_at);
                Role = role;
                ClanID = clan_id;
            }
        }
    }
}

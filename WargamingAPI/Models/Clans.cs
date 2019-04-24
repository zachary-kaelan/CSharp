using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.Models
{
    public sealed class ClanMember : Player
    {
        public string account_name { get; private set; }
        public long joined_at { get; private set; }
        public string role { get; private set; }
    }

    public class BaseClan
    {
        public int clan_id { get; protected set; }
        public long created_at { get; protected set; }
        public int members_count { get; protected set; }
        public string name { get; protected set; }
        public string tag { get; protected set; }
    }

    public sealed class Clan : BaseClan, IClan
    {
        public int creator_id { get; private set; }
        public string creator_name { get; private set; }
        public string description { get; private set; }
        public int? emblem_set_id { get; private set; }
        public bool is_clan_disbanded { get; private set; }
        public int leader_id { get; private set; }
        public string leader_name { get; private set; }
        public int[] members_ids { get; private set; }
        public string motto { get; private set; }
        public string old_name { get; private set; }
        public string old_tag { get; private set; }
        public string recruiting_policy { get; private set; }
        public long? renamed_at { get; private set; }
        public long? updated_at { get; private set; }
        public RecruitingOptions? recruiting_options { get; private set; }

        public struct RecruitingOptions
        {
            public int average_battles_per_day { get; private set; }
            public int average_damage { get; private set; }
            public int battles { get; private set; }
            public int vehicles_level { get; private set; }
            public int wins_ratio { get; private set; }
        }
    }

    public interface IClan
    {
        int clan_id { get; }
        bool is_clan_disbanded { get; }
        long? updated_at { get; }
    }
}

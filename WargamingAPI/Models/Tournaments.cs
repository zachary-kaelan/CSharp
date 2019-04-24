using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace WargamingAPI.Models
{
    public class TournamentObject
    {
        public int tournament_id { get; protected set; }
    }

    public class BaseTournament : TournamentObject
    {
        public string description { get; protected set; }
        public long end_at { get; protected set; }
        public long matches_start_at { get; protected set; }

        public long registration_end_at { get; protected set; }
        public long registration_start_at { get; protected set; }

        public long start_at { get; protected set; }

        public TournamentStatus status { get; protected set; }
        public string title { get; protected set; }

        public CurrencyAmount award { get; protected set; }
        public CurrencyAmount fee { get; protected set; }
        public Images logo { get; protected set; }
        public CurrencyAmount winner_award { get; protected set; }

        public enum TournamentStatus
        {
            UPCOMING,
            REGISTRATION_STARTED,
            REGISTRATION_FINISHED,
            RUNNING,
            FINISHED,
            COMPLETE
        }
    }

    public class CurrencyAmount
    {
        public int? amount { get; protected set; }
        public string currency { get; protected set; }
    }

    public class Tournament : BaseTournament
    {
        public int max_players_count { get; protected set; }
        public int min_players_count { get; protected set; }
        public string other_rules { get; protected set; }
        public string prize_description { get; protected set; }
        public string rules { get; protected set; }

        public MediaLinks media_links { get; protected set; }
        public Teams teams { get; protected set; }

        public struct MediaLinks
        {
            public string id { get; private set; }
            public string image { get; private set; }
            public string kind { get; private set; }
            public string url { get; private set; }
        }

        public struct Teams
        {
            public int confirmed { get; private set; }
            public int max { get; private set; }
            public int min { get; private set; }
            public int total { get; private set; }
        }
    }

    public class Team : TournamentObject
    {
        public int clan_id { get; protected set; }
        public TeamStatus status { get; protected set; }
        public int team_id { get; protected set; }
        public string title { get; protected set; }
        public TeamMember[] players { get; protected set; }

        public enum TeamStatus
        {
            STATIC,
            FORMING,
            CONFIRMED,
            DISQUALIFIED
        }
    }

    public sealed class TeamMember : Player
    {
        public string image { get; private set; }
        public string name { get; private set; }
        public string role { get; private set; }
    }

    public sealed class TournamentStage : TournamentObject
    {
        public int battle_limit { get; private set; }
        public string description { get; private set; }
        public long end_at { get; private set; }
        public int groups_count { get; private set; }
        public int max_tier { get; private set; }
        public int min_tier { get; private set; }
        public int[] rounds { get; private set; }
        public int rounds_count { get; private set; }
        public int stage_id { get; private set; }
        public long start_at { get; private set; }
        public StageState state { get; private set; }
        public string title { get; private set; }
        public BracketType type { get; private set; }
        public int victory_limit { get; private set; }
        public StageGroup[] groups { get; private set; }

        public enum StageState
        {
            DRAFT,
            GROUPS_READY,
            SCHEDULE_READY,
            COMPLETE
        }

        public enum BracketType
        {
            RR,
            SE,
            DE
        }

        public struct StageGroup
        {
            public int group_id { get; private set; }
            public int group_order { get; private set; }
        }
    }

    public sealed class TournamentMatch : TournamentObject
    {
        public int group_id { get; private set; }
        public string id { get; private set; }
        [JilDirective("next_match_for_looser")]
        public string next_match_for_loser { get; private set; }
        public string next_match_for_winner { get; private set; }
        public int round { get; private set; }
        public int stage_id { get; private set; }
        public long start_time { get; private set; }
        public MatchState state { get; private set; }
        public int team_1_id { get; private set; }
        public int team_1_score { get; private set; }
        public int team_2_id { get; private set; }
        public int team_2_score { get; private set; }
        public int winner_team_id { get; private set; }

        public enum MatchState
        {
            WAITING_RESULTS,
            GOT_RESULTS,
            CANCELED,
            UPCOMING
        }
    }

    public struct TournamentResults
    {
        public int battle_played { get; private set; }
        public int draws { get; private set; }
        public int group_id { get; private set; }
        public int losses { get; private set; }
        public int points { get; private set; }
        public int position { get; private set; }
        public int stage_id { get; private set; }
        public int team_id { get; private set; }
        public int wins { get; private set; }
    }

    public sealed class TournamentBrackets : TournamentObject
    {
        public int clan_emblem_preset_id { get; private set; }
        public int clan_id { get; private set; }
        public string clan_label { get; private set; }
        public int group_id { get; private set; }
        public int group_order { get; private set; }
        public int matches_played { get; private set; }
        public int position { get; private set; }
        public int round { get; private set; }
        public int stage_id { get; private set; }
        public int team_id { get; private set; }
        public int team_points { get; private set; }
        public string title { get; private set; }
    }
}

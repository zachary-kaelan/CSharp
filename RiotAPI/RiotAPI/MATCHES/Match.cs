using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.MATCHES
{
    public struct MatchDto
    {
        public int seasonId;
        public int queueId;
        public long gameId;
        public List<ParticipantIdentityDto> participantIdentities;
        public string gameVersion;
        public string platformId;
        public string gameMode;
        public int mapId;
        public string gameType;
        public List<TeamStatsDto> teams;
        public List<ParticipantDto> participants;
        public long gameDuration;
        public long gameCreation;
    }

    public struct TeamStatsDto
    {
        public bool firstDragon;
        public bool firstInhibitor;
        public List<Dictionary<string, int>> bans;
        public int baronKills;
        public bool firstRiftHerald;
        public bool firstBaron;
        public int riftHeraldKills;
        public bool firstBlood;
        public int teamId;
        public bool firstTower;
        public int vilemawKills;
        public int inhibitorKills;
        public int towerKills;
        public int dominionVictoryScore;
        public string win;
        public int dragonKills;
    }

    public struct ParticipantDto
    {
        public Dictionary<string, object> stats;
        public int participantId;
        public List<Dictionary<string, int>> runes;
        public ParticipantTimelineDto timeline;
        public int teamId;
        public int spell2Id;
        public List<Dictionary<string, int>> masteries;
        public string highestAchievedSeasonTIer;
        public int spell1Id;
        public int championId;
    }

    // RuneDto - Dictionary<string, int>

    public struct ParticipantTimelineDto
    {
        public string lane { get; set; }
        public int participantId { get; set; }
        public Dictionary<string, double> csDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> goldPerMinDeltas { get; set; }
        public Dictionary<string, double> xpDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> creepsPerMinDeltas { get; set; }
        public Dictionary<string, double> xpPerMinDeltas { get; set; }
        public string role { get; set; }
        public Dictionary<string, double> damageTakenDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> damageTakenPerMinDeltas { get; set; }
    }

    // MasteryDto - Dictionary<string, int>
}

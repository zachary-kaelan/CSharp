using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.MATCHES
{
    public struct MatchTimeLineDto
    {
        public List<MatchFrameDto> frames { get; set; }
        public long frameInterval { get; set; }
    }

    public struct MatchFrameDto
    {
        public long timestamp { get; set; }
        public Dictionary<int, MatchParticipantFrameDto> participantFrames { get; set; }
        public List<MatchEventDto> events { get; set; }
    }

    public struct MatchParticipantFrameDto
    {
        public int totalGold { get; set; }
        public int teamScore { get; set; }
        public int participantId { get; set; }
        public int level { get; set; }
        public int currentGold { get; set; }
        public int minionsKilled { get; set; }
        public int dominionScore { get; set; }
        public Dictionary<char, int> position { get; set; }
        public int xp { get; set; }
        public int jungleMinionsKilled { get; set; }
    }

    public struct MatchEventDto
    {
        public string eventType { get; set; }
        public string towerType { get; set; }
        public int teamId { get; set; }
        public string ascendedType { get; set; }
        public int killerId { get; set; }
        public string levelUpType { get; set; }
        public string pointCaptured { get; set; }
        public List<int> assistingParticipantIds { get; set; }
        public string wardType { get; set; }
        public string monsterType { get; set; }
        public string type { get; set; } // CHAMPION_KILL, WARD_PLACED, WARD_KILL, BUILDING_KILL, ELITE_MONSTER_KILL, ITEM_PURCHASED, ITEM_SOLD, ITEM_DESTROYED, ITEM_UNDO, SKILL_LEVEL_UP, ASCENDED_EVENT, CAPTURE_POINT, PORO_KING_SUMMON
        public int skillSlot { get; set; }
        public int victimId { get; set; }
        public long timestamp { get; set; }
        public int afterId { get; set; }
        public string monsterSubType { get; set; }
        public string laneType { get; set; }
        public int itemId { get; set; }
        public int participantId { get; set; }
        public string buildingType { get; set; }
        public int creatorId { get; set; }
        public Dictionary<char, int> position { get; set; }
        public int beforeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib
{
    public enum Die : byte
    {
        d4 = 4,
        d6 = 6,
        d8 = 8,
        d10 = 10,
        d12 = 12,
        d20 = 20
    }

    [Flags]
    public enum Abilities : byte
    {
        None,
        STR,
        DEX,
        CON = 4,
        INT = 8,
        WIS = 16,
        CHA = 32
    }

    [Flags]
    public enum Alignment : byte
    {
        TrueNeutral = 0,
        Chaotic = 1,
        Good = 2,
        Evil = 3,
        Lawful = 5,
        Neutral = 9,
        Unaligned = 16
    }

    public enum Skill : byte
    {
        None = 1,
        Acrobatics = 3,
        Animal_Handling = 11,
        Arcana = 6,
        Athletics = 2,
        Deception = 16,
        History = 7,
        Insight = 12,
        Intimidation = 17,
        Investigation = 8,
        Medicine = 13,
        Nature = 9,
        Perception = 14,
        Performance = 18,
        Persuasion = 19,
        Religion = 10,
        Sleight_of_Hand = 4,
        Stealth = 5,
        Survival = 15
    }

    public enum DamageAdjustment
    {
        None,
        Vulnerability,
        Immunity,
        Resistance
    }
    
    [Flags]
    public enum DamageType
    {
        None,
        Acid,
        Bludgeoning,
        Cold = 4,
        DamageByTraps = 8,
        Fire = 16,
        Force = 32,
        Lightning = 64,
        Necrotic = 128,
        Piercing = 256,
        Poison = 512,
        Psychic = 1024,
        Radiant = 2048,
        Ranged_Attacks = 4096,
        Slashing = 8192,
        Thunder = 16384,
        All = 32767
    }

    [Flags]
    public enum DamageModifier : byte
    {
        None,
        Magic = 1,
        Nonmetals = 2,
        Adamantine = 4,
        Silver = 8,
        InDimLightOrDarkness = 16,
        WieldedByGood = 32
    }

    [Flags]
    public enum Condition
    {
        None,
        Blinded,
        Charmed,
        Deafened = 4,
        Exhaustion = 8,
        Frightened = 16,
        Grappled = 32,
        Incapacitated = 64,
        Invisible = 128,
        Paralyzed = 256,
        Petrified = 512,
        Poisoned = 1024,
        Prone = 2048,
        Restrained = 4096,
        Stunned = 8192,
        Unconscious = 16384
    }

    [Flags]
    public enum Environment
    {
        None,
        Arctic,
        Coastal,
        Desert = 4,
        Forest = 8,
        Grassland = 16,
        Hill = 32,
        Mountain = 64,
        Swamp = 128,
        Underdark = 256,
        Underwater = 512,
        Urban = 1024
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using AlienRace;

namespace UndeadRaces
{
    public static class UndeadBaseTypes
    {
        static UndeadBaseTypes()
        {
            
        }
        
        public static readonly ThingDef_AlienRace AllUndead = new ThingDef_AlienRace()
        {
            alienRace = new ThingDef_AlienRace.AlienSettings()
            {
                generalSettings = new GeneralSettings()
                {
                    chemicalSettings = new List<ChemicalSettings>()
                    {
                        new ChemicalSettings()
                        {
                            chemical = "Beer",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Ambrosia",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "GoJuice",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Luciferum",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Penoxycyline",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Flake",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Pekoe",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Yayo",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "Smokeleaf",
                            ingestible = false
                        },
                        new ChemicalSettings()
                        {
                            chemical = "WakeUp",
                            ingestible = false
                        }
                    },
                    disallowedTraits = new List<string>()
                    {
                        "Ascetic",
                        "CreepyBreathing",
                        "Gay",
                        "Kind",
                        "Masochist",
                        "Nudist",
                        "Prosthophile",
                        "Prosthophobe",
                        "SuperImmune",
                        "Wimp",

                        "NaturalMood",
                        "Nerves",
                        "Neurotic"
                    },
                    factionRelations = new List<FactionRelationSettings>()
                    {
                        new FactionRelationSettings()
                        {
                            factions = new List<string>()
                            {
                                "Outlander",
                                "Pirate",
                                "Tribe"
                            },
                            goodwill = new Verse.FloatRange(-50f, -10f)
                        }
                    },
                    forcedRaceTraitEntries = new List<AlienTraitEntry>()
                    {
                        new AlienTraitEntry()
                        {
                            defName = "Psychopath"
                        },
                        new AlienTraitEntry()
                        {
                            defName = "Teetotaler"
                        },
                        new AlienTraitEntry()
                        {
                            defName = "Beauty",
                            degree = -2,
                            chance = 100,
                            commonalityFemale = 95f
                        },
                        new AlienTraitEntry()
                        {
                            defName = "NightOwl"
                        },
                        new AlienTraitEntry()
                        {
                            defName = "PsychicSensitivity",
                            degree = -2
                        }
                    },
                    immuneToAge = true,
                    maleGenderProbability = 65f,
                    pawnsSpecificBackstories = true
                },
                thoughtSettings = new AlienRace.ThoughtSettings()
                {
                    cannotReceiveThoughts = new List<string>()
                    {
                        // Wouldn't feel anyway, but...
                        "Sick",
                        "Pain",
                        "AteWithoutTable",
                        "SleepDisturbed",
                        "SleptOutside",
                        "SleptOnGround",
                        "SleptInCold",
                        "SleptInHeat",
                        "NeedFood",
                        "NeedRest",
                        "NeedComfort",
                        "NeedBeauty",
                        "NeedSpace",
                        "AteInImpressiveDiningRoom",
                        "JoyActivityInImpressiveRecRoom",
                        "SleptInBedroom",
                        "SleptInBarracks",

                        // Too human
                        "Joywire",
                        "Aurora",
                        "FeelingRandom",
                        "ArtificialMoodBoost",
                        "GreenThumbHappy",
                        "WitnessedDeathBloodlust",
                        "KilledHumanlikeBloodlust",
                        "KindWordsMood",
                        "RebuffedMyRomanceAttemptMood",
                        "FailedRomanceAttemptOnMeLowOpinionMood",
                        "BrokeUpWithMeMood",
                        "DivorcedMeMood",
                        "GotMarried",
                        "AttendedWedding",
                        "AttendedParty",
                        "PrisonCell",
                        "PrisonBarracks",
                        "HospitalPatientRoomStats",

                        // Quite the opposite
                        "HumanLeatherApparelSad",
                        "KnowBuriedInSarcophagus",
                        "ObservedLayingRottingCorpse",

                        // Undead specific
                        "EnvironmentDark",
                        "CabinFever",
                        "ApparelDamaged",
                        "DeadMansApparel",
                        "EnvironmentCold",
                        "MyOrganHarvested",

                        // Social
                        "RapportBuilt",
                        "HadCatharticFight",
                        "BotchedMySurgery",
                        "RescuedMe",
                        "RecruitedMe",
                        "CrashedTogether",
                        "KindWords",
                        "RebuffedMyRomanceAttempt",
                        "FailedRomanceAttemptMe",
                        "BrokeUpWithMe",
                        "DivorcedMe",
                        "IRejectedTheirProposal",
                        "HoneyMoonPhase",
                        "GotSomeLovin",
                        "Incestuous",
                        "Disfigured",
                        "Pretty",
                        "Ugly",
                        "Man",
                        "Woman",
                        "AteRawHumanlikeMeat",
                        "OpinionOfMyLover",
                        "WantToSleepWithSpouseOrLover",

                        // Death means nothing
                        "ExecutedPrisoner",
                        "KilledColonist",
                        "KilledColonyAnimal",
                        "KilledMajorColonyEnemy",
                    }
                },
                hairSettings = new AlienRace.HairSettings()
                {
                    hasHair = false
                }
            },
            fertility = 0f,
            apparel = new ApparelProperties() {
                careIfWornByCorpse = false,
            },
            filthLeaving = MiscDefs.CorpseFilth
        };

        public static readonly ThingDef_AlienRace Skeletal = new ThingDef_AlienRace()
        {
            alienRace = new ThingDef_AlienRace.AlienSettings()
            {
                generalSettings = new GeneralSettings()
                {
                    
                }
            }
        };

        public static readonly ThingDef_AlienRace Lich = new ThingDef_AlienRace()
        {
            alienRace = new ThingDef_AlienRace.AlienSettings()
            {
                relationSettings = new AlienRace.RelationSettings()
                {
                    relationChanceModifierChild = 0.05f,
                    relationChanceModifierFiance = 0.15f,
                    relationChanceModifierLover = 0.10f,
                    relationChanceModifierParent = 0.25f,
                    relationChanceModifierSibling = 0.35f,
                    relationChanceModifierSpouse = 0.05f,
                    relationChanceModifierExLover = 0.5f,
                    relationChanceModifierExSpouse = 0.5f,
                    renamer = new List<AlienRace.RelationRenamer>() {
                        new AlienRace.RelationRenamer()
                        {
                            relation = "Lover",
                            femaleLabel = "Lich Loved",
                            label = "Necrophiliac Obsession"
                        },
                        new AlienRace.RelationRenamer()
                        {
                            relation = "Spouse",
                            femaleLabel = "Necromaniac",
                            label = "Necromaniac Obsession"
                        },
                        new AlienRace.RelationRenamer()
                        {
                            relation = "Fiance",
                            femaleLabel = "Flesh Slave",
                            label = "Master"
                        }
                    }
                }
            }
        }
    }
}

using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using AlienRace;

namespace UndeadRaces
{
    public sealed class ThingDef_AlienRace : ThingDef
    {
        public AlienSettings alienRace;

        public override void ResolveReferences()
        {
            this.comps.Add(new CompProperties(typeof(AlienPartGenerator.AlienComp)));
            base.ResolveReferences();
            if (this.alienRace.graphicPaths.NullOrEmpty())
                this.alienRace.graphicPaths.Add(new GraphicPaths());
            this.alienRace.graphicPaths.ForEach(gp =>
            {
                if (gp.customDrawSize == Vector2.one)
                    gp.customDrawSize = this.alienRace.generalSettings.alienPartGenerator.customDrawSize;
                if (gp.customPortraitDrawSize == Vector2.one)
                    gp.customPortraitDrawSize = this.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize;
            });
            this.alienRace.generalSettings.alienPartGenerator.alienProps = this;
        }

        public sealed class AlienSettings
        {
            public GeneralSettings generalSettings = new GeneralSettings();
            public List<GraphicPaths> graphicPaths = new List<GraphicPaths>();
            public HairSettings hairSettings = new HairSettings();
            public PawnKindSettings pawnKindSettings = new PawnKindSettings();
            public ThoughtSettings thoughtSettings = new ThoughtSettings();
            public RelationSettings relationSettings = new RelationSettings();
            public RaceRestrictionSettings raceRestriction = new RaceRestrictionSettings();
        }
    }

    public sealed class GeneralSettings
    {
        /// <summary>
        /// Custom gender distribution in your race.
        /// <para>(Human Default: 0.5)</para>
        /// </summary>
        public float maleGenderProbability = 0.5f;

        /// <summary>
        /// Activate if you want to set custom Backstories for your Pawns.
        /// <para>(Default: false)</para>
        /// </summary>
        public bool pawnsSpecificBackstories = false;

        /// <summary>
        /// Immunity to age related problems like Carcinoma, Bad back, Frail, etc.
        /// <para>(Default: false)</para>
        /// </summary>
        public bool immuneToAge = false;

        /// <summary>
        /// In the rare case your race sleeps upright, you can set this to false.
        /// <para>(Default: true)</para>
        /// </summary>
        public bool canLayDown = true;

        /// <summary>
        /// If your race has their own beds, you can set them here.
        /// </summary>
        public List<string> validBeds;

        /// <summary>
        /// Set specific settings regarding the use of chemicals (drugs).
        /// <para>Used in cases where races have specifically different reactions to a chemical, or just straight up not making it ingestible.</para>
        /// </summary>
        public List<ChemicalSettings> chemicalSettings;

        /// <summary>
        /// Force traits onto the race, with an optionally specified chance and gender commonality.
        /// <para>Note: Be aware that the gender commonality fires as a second chance after the normal chance is set.</para>
        /// </summary>
        public List<AlienTraitEntry> forcedRaceTraitEntries;
        
        /// <summary>
        /// To prevent your race from having certain traits.
        /// </summary>
        public List<string> disallowedTraits;
        public AlienPartGenerator alienPartGenerator = new AlienPartGenerator();

        /// <summary>
        /// Starting with negative or positive goodwill with specific factions.
        /// </summary>
        public List<FactionRelationSettings> factionRelations;

        /// <summary>
        /// In case this race has special melee abilities they wouldn't use in a social fight.
        /// </summary>
        public int maxDamageForSocialfight = int.MaxValue;

        /// <summary>
        /// In the case that you want to enable human pawn bios for your race.
        /// <para>(Default: false)</para>
        /// </summary>
        public bool allowHumanBios = false;

        /// <summary>
        /// You can disable xenophobia (or xenophilia) if it doesn't make sense for this race to be a target.
        /// <para>(Default: false)</para>
        /// </summary>
        public bool immuneToXenophobia = false;

        /// <summary>
        /// Add races that are genetically close - or otherwise not considered "different" - here.
        /// </summary>
        public List<string> notXenophobistTowards = new List<string>();

        /// <summary>
        /// Adds human recipes to any body parts this race shares with them.
        /// <para>(Default: false)</para>
        /// </summary>
        public bool humanRecipeImport = false;
    }

    public sealed class FactionRelationSettings
    {
        public List<string> factions;
        public FloatRange goodwill;
    }

    public sealed class ChemicalSettings
    {
        public string chemical;
        public bool ingestible = true;
        public List<IngestionOutcomeDoer> reactions;
    }

    public sealed class AlienTraitEntry
    {
        public string defName;
        public int degree = 0;
        public float chance = 100;

        public float commonalityMale = -1f;
        public float commonalityFemale = -1f;
    }

    public sealed class GraphicPaths
    {
        public List<LifeStageDef> lifeStageDefs;
        public Vector2 customDrawSize = Vector2.one;
        public Vector2 customPortraitDrawSize = Vector2.one;

        public const string vanillaHeadPath = "Things/Pawn/Humanlike/Heads/";

        public string body = "Things/Pawn/Humanlike/Bodies/";
        public string head = "Things/Pawn/Humanlike/Heads/";
        public string skeleton = "Things/Pawn/Humanlike/HumanoidDessicated";
        public string skull = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
        public string stump = "Things/Pawn/Humanlike/Heads/None_Average_Stump";
    }

    public sealed class HairSettings
    {
        public bool hasHair = true;
        public List<string> hairTags;
        public int getsGreyAt = 40;
    }

    public sealed class PawnKindSettings
    {
        public List<PawnKindEntry> alienslavekinds;
        public List<PawnKindEntry> alienrefugeekinds;
        public List<FactionPawnKindEntry> startingColonists;
        public List<FactionPawnKindEntry> alienwandererkinds;
    }

    public sealed class PawnKindEntry
    {
        public List<string> kindDefs;
        public float chance;
    }

    public sealed class FactionPawnKindEntry
    {
        public List<PawnKindEntry> pawnKindEntries;
        public List<string> factionDefs;
    }

    public sealed class ThoughtSettings
    {
        public List<string> cannotReceiveThoughts;
        public bool cannotReceiveThoughtsAtAll = false;
        public List<string> canStillReceiveThoughts;

        public ButcherThought butcherThoughtGeneral = new ButcherThought();
        public List<ButcherThought> butcherThoughtSpecific = new List<ButcherThought>();

        public AteThought ateThoughtGeneral = new AteThought();
        public List<AteThought> ateThoughtSpecific = new List<AteThought>();

        public List<ThoughtReplacer> replacerList;
    }

    public sealed class ButcherThought
    {
        public List<string> raceList;
        public string thought = "ButcheredHumanlikeCorpse";
        public string knowThought = "KnowButcheredHumanlikeCorpse";
    }

    public sealed class AteThought
    {
        public List<string> raceList;
        public string thought = "AteHumanlikeMeatDirect";
        public string ingredientThought = "AteHumanlikeMeatAsIngredient";
    }

    public sealed class ThoughtReplacer
    {
        public string original;
        public string replacer;
    }

    public sealed class RelationSettings
    {
        public float relationChanceModifierChild = 1f;
        public float relationChanceModifierExLover = 1f;
        public float relationChanceModifierExSpouse = 1f;
        public float relationChanceModifierFiance = 1f;
        public float relationChanceModifierLover = 1f;
        public float relationChanceModifierParent = 1f;
        public float relationChanceModifierSibling = 1f;
        public float relationChanceModifierSpouse = 1f;

        public List<RelationRenamer> renamer;
    }

    public sealed class RelationRenamer
    {
        public string relation;
        public string label;
        public string femaleLabel;
    }

    public sealed class RaceRestrictionSettings
    {
        public bool onlyUseRaceRestrictedApparel = false;
        public List<string> apparelList;
        public List<string> whiteApparelList;

        public List<ResearchProjectRestrictions> researchList;

        public bool onlyUseRaceRestrictedWeapons = false;
        public List<string> weaponList;
        public List<string> whiteWeaponList;

        public bool onlyBuildRaceRestrictedBuildings = false;
        public List<string> buildingList;
        public List<string> whiteBuildingList;

        public bool onlyDoRaceRestrictedRecipes = false;
        public List<string> recipeList;
        public List<string> whiteRecipeList;

        public bool onlyDoRaceRastrictedPlants = false;
        public List<string> plantList;
        public List<string> whitePlantList;

        public bool onlyGetRaceRestrictedTraits = false;
        public List<string> traitList;
        public List<string> whiteTraitList;

        public bool onlyEatRaceRestrictedFood = false;
        public List<string> foodList;
        public List<string> whiteFoodList;

        public bool onlyTameRaceRestrictedPets = false;
        public List<string> petList;
        public List<string> whitePetList;

        public List<string> conceptList;

        public List<string> workGiverList;
    }

    public sealed class ResearchProjectRestrictions
    {
        public List<string> projects;
        public List<string> apparelList;
    }

    static class GraphicPathsExtension
    {
        public static GraphicPaths GetCurrentGraphicPath(this List<GraphicPaths> list, LifeStageDef lifeStageDef) => list.FirstOrDefault(gp => gp.lifeStageDefs?.Contains(lifeStageDef) ?? false) ?? list.First();
    }

    public class Info : DefModExtension
    {
        public bool usePawnKindBackstories = false;
    }

    /*public struct GeneralSettings
    {
        /// <summary>
        /// Custom gender distribution in your race
        /// <para>(human: 0.5)</para>
        /// </summary>
        public float MaleGenderProbability { get; set; }

        /// <summary>
        /// Activate if you want to set custom Backstories
        /// </summary>
        public bool PawnSpecificBackstories { get; set; }

        /// <summary>
        /// Immunity to age related problems
        /// <para>Carcinoma, Bad back, Frail, etc.</para>
        /// </summary>
        public bool ImmuneToAge { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanLayDown { get; set; }
    }*/

    public struct HairSettings
    {

    }

    public struct PawnKingSettings
    {

    }

    public struct ThoughtSettings
    {

    }

    public struct RelationSettings
    {

    }

    public struct RaceRestriction
    {

    }
}

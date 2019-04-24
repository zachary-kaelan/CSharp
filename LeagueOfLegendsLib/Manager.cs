using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using LeagueOfLegendsLib.RunesReforged;
using RestSharp;
using RiotSharp;
using RiotSharp.ChampionEndpoint;
using RiotSharp.ChampionMasteryEndpoint;
using RiotSharp.GameEndpoint;
using RiotSharp.Http;
using RiotSharp.Http.Interfaces;
using RiotSharp.Interfaces;
using RiotSharp.Misc;
using RiotSharp.RunesEndpoint;
using RiotSharp.StaticDataEndpoint;
using RiotSharp.StaticDataEndpoint.Champion;
using RiotSharp.StaticDataEndpoint.Champion.Enums;
using RiotSharp.StaticDataEndpoint.Item;
using RiotSharp.StaticDataEndpoint.Map;
using RiotSharp.StaticDataEndpoint.Rune;
using RiotSharp.StaticDataEndpoint.SummonerSpell;
using ZachLib;

namespace LeagueOfLegendsLib
{
    public static class Manager
    {
        private const string API_KEY = "RGAPI-20e16e75-c0bb-4e2b-9c8f-2798f59c1020";
        private static readonly RiotApi API = RiotApi.GetDevelopmentInstance(API_KEY);
        private static readonly StaticRiotApi STATIC_API = StaticRiotApi.GetInstance(API_KEY);
        public const string MAIN_PATH = @"E:\League of Legends\Riot API\";
        public const string CHAMPS_PATH = MAIN_PATH + @"Champions\";
        public const string ITEMS_PATH = MAIN_PATH + @"Items\";
        public const string IMAGES_PATH = MAIN_PATH + @"Images\";
        public const string RUNES_PATH = MAIN_PATH + @"Runes\";
        private const Region NA = Region.na;

        public static void UpdateAllData()
        {
            var champions = STATIC_API.GetChampions(NA);
            champions.Keys.SaveAs(CHAMPS_PATH + "ID-Name Dictionary.txt");
            foreach (var champion in champions.Champions.Values)
            {
                champion.Image.AddPath("Champions");
                champion.Passive.Image.AddPath(@"Champions\Abilities");
                for (int i = 0; i < 4; ++i)
                {
                    var spell = champion.Spells[i];
                    spell.Image.AddPath(@"Champions\Abilities");
                    var altCount = spell.Altimages.Count;
                    for(int j = 0; j < altCount; ++j)
                    {
                        spell.Altimages[j].AddPath(@"Champions\Abilities");
                    }
                }
                champion.SaveAs(CHAMPS_PATH + champion.Name + ".txt");
            }
            champions = null;

            UpdateItems();

            var maps = STATIC_API.GetMaps(NA);
            for (int i = 0; i < maps.Count; ++i)
            {
                maps[i].Image.AddPath("Maps");
            }
            maps.SaveAs(MAIN_PATH + "Maps.txt");

            List<ReforgedRunePathStatic> runePaths = null;
            for (int i = 0; i < runePaths.Count; ++i)
            {
                var runePath = runePaths[i];
                runePath.icon = @"Runes\" + Path.GetFileName(runePath.icon);
                string pathDir = RUNES_PATH + runePath.key;
                Directory.CreateDirectory(pathDir);
                pathDir += @"\";
                string iconPath = @"Runes\" + runePath.key + @"\";

                for (int j = 0; j < runePath.slots.Count; ++j)
                {
                    var slot = runePath.slots[j];
                    var slotDirName = "Slot " + (j + 1).ToString();
                    string slotDir = pathDir + slotDirName;
                    Directory.CreateDirectory(slotDir);
                    slotDir += @"\";
                    slotDirName += @"\";

                    foreach(var rune in slot.runes)
                    {
                        rune.icon = iconPath + slotDirName + Path.GetFileName(rune.icon);
                        rune.SaveAs(slotDir + rune.key + ".txt");
                    }
                }

                runePath.slots = null;
                runePath.SaveAs(pathDir + "PathInfo.txt");
            }
        }

        public static void UpdateItems()
        {
            var items = STATIC_API.GetItems(NA);
            var allTags = items.Items.Values.SelectMany(i => i.Tags).ToArray();
            Array.Sort(allTags);

            items.Trees.SaveAs(ITEMS_PATH + "Trees.txt");
            items.Groups.SaveAs(ITEMS_PATH + "Groups.txt");

            var groupedByRequiredChampion = new Dictionary<string, List<ItemStatic>>();
            List<ItemStatic> itemsNotSpecial = new List<ItemStatic>();
            foreach (var item in items.Items.Values)
            {
                bool notSpecial = true;
                item.Image.AddPath("Items");
                if (!String.IsNullOrWhiteSpace(item.RequiredChampion))
                {
                    if (groupedByRequiredChampion.ContainsKey(item.RequiredChampion))
                        groupedByRequiredChampion[item.RequiredChampion].Add(item);
                    else
                        groupedByRequiredChampion.Add(item.RequiredChampion, new List<ItemStatic>() { item });
                    notSpecial = false;
                }
                if (item.Consumed)
                {
                    notSpecial = false;
                    item.SaveAs(ITEMS_PATH + @"Consumables\" + item.Id + ".txt");
                }
                if (item.Metadata.IsRune)
                {
                    notSpecial = false;
                    item.SaveAs(ITEMS_PATH + @"Runes\" + item.Id + ".txt");
                }
                if (!item.Maps["11"])
                {
                    notSpecial = false;
                    item.SaveAs(ITEMS_PATH + @"OtherMaps\" + item.Id + ".txt");
                }
                if (notSpecial && (item.HideFromAll || !item.InStore || !item.Gold.Purchasable))
                {
                    notSpecial = false;
                    item.SaveAs(ITEMS_PATH + @"MiscSpecial\" + item.Id + ".txt");
                }

                if (notSpecial)
                {
                    itemsNotSpecial.Add(item);
                    item.SaveAs(ITEMS_PATH + item.Id + ".txt");
                }
            }

            /*foreach(var item in itemsNotSpecial)
            {
                if (!item.From.Any() && item)
            }*/

            items = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDLib
{
    public struct Loot : IDBDObject
    {
        public int Plunderers { get; set; }
        public int AceInTheHole { get; set; }
        public int Luck { get; set; }
        public Rarity ItemRarity { get; set; }
        public ItemType ItemType { get; set; }
        public Rarity AddonRarity { get; set; }
        public Rarity Addon2Rarity { get; set; }

        public string Condense()
        {
            return String.Join(
                "",
                new int[] {
                    Plunderers,
                    AceInTheHole,
                    (int)ItemRarity,
                    (int)ItemType,
                    (int)AddonRarity,
                    (int)Addon2Rarity,
                    Luck / 16
                }.Select(n => n.ToString())
            ) + Convert.ToString(Luck % 16, 16);
        }

        public void LoadCondensed(string condensed)
        {
            int[] numbers = condensed.Take(8).Select(
                c => Convert.ToInt32(
                    Char.GetNumericValue(c)
                )
            ).ToArray();

            Plunderers = numbers[0];
            AceInTheHole = numbers[1];
            Luck = (numbers[6] * 16) + Convert.ToInt32(condensed.Last().ToString(), 16);
            ItemRarity = (Rarity)numbers[2];
            ItemType = (ItemType)numbers[3];
            AddonRarity = (Rarity)numbers[4];
            Addon2Rarity = (Rarity)numbers[5];
        }

        private const string LONG_FORMAT = "Plunderer's Instinct: {0}\r\nAce In The Hole: {1}\r\nLuck: {2}\r\nItemType: {3}\r\nItemRarity: {4}\r\nAddonRarity: {5}\r\nAddon2Rarity: {6}";
        private const string SHORT_FORMAT = "[PI{0}, AitH{1}, L{2}, {4} {3} w/ {5} & {6}]";
        public override string ToString()
        {
            return String.Format(SHORT_FORMAT, Plunderers, AceInTheHole, Luck, ItemType, ItemRarity, AddonRarity);
        }

        public string ToString(bool shortened)
        {
            if (!shortened)
                return String.Format(LONG_FORMAT, Plunderers, AceInTheHole, Luck, ItemType, ItemRarity, AddonRarity, Addon2Rarity);
            else
                return ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftMathLib.Enchanting
{
    public enum ArmorMaterial
    {
        Leather = 2,
        Iron = 4,
        Chain = 5,
        Gold = 6,
        Diamond = 7
    }

    public enum WeaponToolMaterial
    {
        Bow = 0,
        Wood = 1,
        Stone = 3,
        Iron = 4,
        Gold = 6,
        Diamond = 7
    }

    public enum ItemType
    {
        Book = 0,
        Bow,
        Armor,
        Weapon
    }

    internal class Constants
    {
        private SortedDictionary<ArmorMaterial, int> ARMOR_ENCHANTABILITY = new SortedDictionary<ArmorMaterial, int>()
        {
            { ArmorMaterial.Leather, 15 },
            { ArmorMaterial.Iron, 9 },
            { ArmorMaterial.Chain, 12 },
            { ArmorMaterial.Diamond, 10 },
            { ArmorMaterial.Gold, 25 }
        };

        private SortedDictionary<WeaponToolMaterial, int> WEAPON_TOOL_ENCHANTABILITY = new SortedDictionary<WeaponToolMaterial, int>()
        {
            { WeaponToolMaterial.Wood, 15 },
            { WeaponToolMaterial.Stone, 5 },
            { WeaponToolMaterial.Iron, 14 },
            { WeaponToolMaterial.Diamond, 10 },
            { WeaponToolMaterial.Gold, 22 }
        };

        public (int, int) GetEnchantabilityRange(int ID, bool isArmor)
        {
            int enchantability = -1;
            if (
                isArmor ?
                    !ARMOR_ENCHANTABILITY.TryGetValue((ArmorMaterial)ID, out enchantability) :
                    !WEAPON_TOOL_ENCHANTABILITY.TryGetValue((WeaponToolMaterial)ID, out enchantability)
            )
                enchantability = 1;
            return (
                (int)Math.Round(enchantability * 0.85)
            );
        }
    }
}

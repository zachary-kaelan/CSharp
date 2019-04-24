using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public class Shell : IDictionary<ShellInstance, Block[]>
    {
        public float Velocity { get; set; }
        public float Volume { get; set; }
        public float ShellHealth { get; set; }
        private Dictionary<ShellInstance, Block[]> Shells { get; set; }

        public Block[] this[ShellInstance key] { get => Shells[key]; set => Shells[key] = value; }

        public Shell(float diameter, float velocity, float numFuses = 0)
        {
            Velocity = velocity;
            ShellHealth = 300f * Math.PI * Math.Pow(diameter, 2f);
            Volume = Math.Pow((diameter / 400f), 1.8) - (0.25 * numFuses);
            Shells = new Dictionary<ShellInstance, Block[]>();
        }

        public void Add(ShellInstance key, params Block[] blocks)
        {
            for (int i = 0; i < blocks.Length; ++i)
            {
                blocks[i].CombinedAC = DamageMath.CombinedLayersAC(blocks.Skip(i).ToArray());
            }
            if (!key.CheckRichocet(blocks.First()))
            {
                blocks[0].Destroyed = true;
            }

            Shells.Add(key, blocks);
        }

        #region IDictionary Implementation
        public ICollection<ShellInstance> Keys => Shells.Keys;

        public ICollection<Block[]> Values => Shells.Values;

        public int Count => Shells.Count;

        public bool IsReadOnly => ((IDictionary<ShellInstance, Block[]>)Shells).IsReadOnly;

        public bool ContainsKey(ShellInstance key)
        {
            return Shells.ContainsKey(key);
        }

        public bool Remove(ShellInstance key)
        {
            return Shells.Remove(key);
        }

        public bool TryGetValue(ShellInstance key, out Block[] value)
        {
            return Shells.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<ShellInstance, Block[]> item)
        {
            ((IDictionary<ShellInstance, Block[]>)Shells).Add(item);
        }

        public void Clear()
        {
            Shells.Clear();
        }

        public bool Contains(KeyValuePair<ShellInstance, Block[]> item)
        {
            return Shells.Contains(item);
        }

        public void CopyTo(KeyValuePair<ShellInstance, Block[]>[] array, int arrayIndex)
        {
            ((IDictionary<ShellInstance, Block[]>)Shells).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<ShellInstance, Block[]> item)
        {
            return ((IDictionary<ShellInstance, Block[]>)Shells).Remove(item);
        }

        public IEnumerator<KeyValuePair<ShellInstance, Block[]>> GetEnumerator()
        {
            return Shells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Shells.GetEnumerator();
        }
#endregion
    }

    public struct ShellDamage
    {
        public float Kinetic { get; set; }
        public float AP { get; set; }
        public float Explosive { get; set; }
        public float EMP { get; set; }
        public Nullable<Fragments> Fragmentation { get; set; }

        public ShellDamage(float kinetic, float armorPierce, float explosive, float emp, Fragments fragments)
        {
            this.Kinetic = kinetic;
            this.AP = armorPierce;
            this.Explosive = explosive;
            this.EMP = emp;
            this.Fragmentation = fragments;
        }

        public ShellDamage(float kin, float pierce, Fragments frag, float exp = 0, float elec = 0)
        {
            Kinetic = kin;
            AP = pierce;
            Explosive = exp;
            EMP = elec;
            Fragmentation = frag;
        }

        public ShellDamage(float vel, float kin, float pierce, float exp = 0, float elec = 0)
        {
            Kinetic = kin;
            AP = pierce;
            Explosive = exp;
            EMP = elec;
            Fragmentation = null;
        }

        public ShellDamage(float baseKinetic, float baseAP, Pellets effectivePellets)
        {
            Kinetic = baseKinetic + (100f * effectivePellets.Hardner);
            AP = baseAP + (1.5f * effectivePellets.Hardner);
            Explosive = 200f * effectivePellets.Explosive;
            EMP = effectivePellets.EMP * (10f * effectivePellets.Volume);
            Fragmentation = new Fragments(effectivePellets.Fragmentation);
        }
    }

    public struct KineticRichocet
    {
        public float RichocetChance { get; set; }
        private float CosTheta { get; set; }
        private float StandardMultiplier { get; set; }
        public float RichocetDamageMultiplier { get; }
        public float NonRichocetDamageMultiplier { get; }

        public KineticRichocet(float cosTheta, float standardMultiplier) : this()
        {
            CosTheta = cosTheta;
            StandardMultiplier = standardMultiplier;
        }

        public bool CanRichocet(float totalKineticDamage, float blockHealth, out KineticRemainingDamage remainingDamage)
        {
            remainingDamage = new KineticRemainingDamage();
            remainingDamage.DamageDealt = Math.Min(totalKineticDamage * StandardMultiplier * CosTheta, blockHealth);
            if (remainingDamage.DamageDealt >= blockHealth)
            {
                remainingDamage.NonRichocetDamage = totalKineticDamage - (blockHealth / StandardMultiplier);
                return false;
            }
            else
            {
                remainingDamage.NonRichocetDamage = totalKineticDamage - (remainingDamage.DamageDealt / (StandardMultiplier * CosTheta));
                if (RichocetChance == 0)
                    return false;
                else
                {
                    remainingDamage.RichocetDamage = totalKineticDamage - (remainingDamage.DamageDealt / StandardMultiplier);
                    return true;
                }
            }
        }
    }

    public struct KineticRemainingDamage
    {
        public float DamageDealt { get; set; }
        public float RichocetDamage { get; set; }
        public float NonRichocetDamage { get; set; }
    }

    public struct Fragments
    {
        public float count { get; set; }
        public float kinetic { get; set; }
        public const float AP = 6;

        public Fragments(float n)
        {
            count = Math.Min(n, 60f);
            kinetic = n > 60f ? ((100f * n) / 60f) : 100f;
        }
    }

    public class Block
    {
        public float Health { get; set; }
        public float Weight { get; set; }
        public float ArmourClass { get; set; }
        public float CombinedAC { get; set; }
        public Tuple<int, int, int> Size { get; set; }
        public int Cost { get; set; }
        public bool Destroyed { get; set;  }
    }

    public class Pellets
    {
        public float Hardner { get; set; }
        public float Explosive { get; set; }
        public float EMP { get; set; }
        public float Fragmentation { get; set; }

        private float Total { get; set; }
        internal float Volume { get; set; }

        public Pellets(float volume, float hardner = 0, float explosive = 0, float fragmentation = 0, float emp = 0)
        {
            Hardner = hardner;
            Explosive = explosive;
            Fragmentation = fragmentation;
            EMP = emp;
            Volume = volume;

            this.Total = this.Hardner + this.Explosive + this.EMP + this.Fragmentation;
        }

        public float GetTotal()
        {
            return this.Total;
        }

        public float GetDensity()
        {
            return this.Total / Volume;
        }

        public ShellDamage GetDamage(float velocity, bool isEffective = true)
        {
            Pellets temp = isEffective ? this : this.EffectivePellets();
            return new ShellDamage(
                (2f * velocity * Volume) + (100f * temp.Hardner),
                3f + (velocity / 150f) + (1.5f * temp.Hardner),
                200f * temp.Explosive,
                10f * Volume * temp.EMP,
                new Fragments(temp.Fragmentation)
            );
        }

        public float EffectiveTotal()
        {
            return EffectiveTotal(this.GetDensity());
        }

        public float EffectiveTotal(float density)
        {
            return 10f * (1f - (float)Math.Pow(0.9, density));
        }

        public float EffectiveToTotalRatio()
        {
            return this.EffectiveToTotalRatio(this.GetDensity());
        }

        public float EffectiveToTotalRatio(float density)
        {
            return this.EffectiveTotal(density) / this.Total;
        }

        public Pellets EffectivePellets()
        {
            return this.EffectivePellets(this.GetDensity());
        }

        public Pellets EffectivePellets(float density)
        {
            float effectiveToTotalRatio = this.EffectiveToTotalRatio(density);
            return new Pellets(
                Hardner * effectiveToTotalRatio,
                Explosive * effectiveToTotalRatio,
                EMP * effectiveToTotalRatio,
                Fragmentation * effectiveToTotalRatio
            );
        }
    }

    public class ShellInstance
    {
        public Pellets EffectivePellets { get; set; }
        public ShellDamage Damage { get; set; }
        public float AngleMultiplier { get; set; }
        public float StartingDamagePotential { get; set; }
        public float EndingDamagePotential { get; set; }
        public byte BlocksDestroyed { get; set; }

        public ShellInstance(Pellets pellets, float volume, float velocity, float angleOfPenetration = 0, bool radians = true)
        {
            BlocksDestroyed = 0;
            StartingDamagePotential = 0;
            EndingDamagePotential = 0;

            this.AngleMultiplier = (float)Math.Cos(radians ? angleOfPenetration : angleOfPenetration * (Math.PI / 180f));
            this.EffectivePellets = pellets.EffectivePellets();
            this.Damage = EffectivePellets.GetDamage(velocity);
        }

        private static readonly Random richocetChance = new Random();
        public bool CheckRichocet(Block outmostBlock)
        {
            float multiplier = DamageMath.DefaultDamageMultiplier(Damage.AP, outmostBlock.CombinedAC);
            StartingDamagePotential = Damage.Kinetic;
            EndingDamagePotential = Damage.Kinetic;

            float firstHit = Damage.Kinetic * multiplier * AngleMultiplier;
            // if the shell is unable to penetrate
            if (firstHit < outmostBlock.Health)
            {
                // if the shell richocets
                EndingDamagePotential = Damage.Kinetic - (firstHit / (
                    richocetChance.NextDouble() <= Math.Pow(
                        1f - AngleMultiplier,
                        (2f * Damage.AP) / outmostBlock.CombinedAC
                    ) ? multiplier : multiplier * AngleMultiplier
                ));
                return true;
            }

            EndingDamagePotential = Damage.Kinetic - (firstHit / multiplier);
            ++BlocksDestroyed;
            return false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public struct Shell : IDictionary<ShellInstance, Block[]>
    {
        public double Velocity { get; set; }
        public double Volume { get; set; }
        public double ShellHealth { get; set; }
        private Dictionary<ShellInstance, Block[]> Shells { get; set; }

        public Block[] this[ShellInstance key] { get => Shells[key]; set => Shells[key] = value; }

        public Shell(double diameter, double velocity, double numFuses = 0)
        {
            Velocity = velocity;
            ShellHealth = 300.0 * Math.PI * Math.Pow(diameter, 2.0);
            Volume = Math.Pow((diameter / 400.0), 1.8) - (0.25 * numFuses);
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
        public double Kinetic { get; set; }
        public double AP { get; set; }
        public double Explosive { get; set; }
        public double EMP { get; set; }
        public Nullable<Fragments> Fragmentation { get; set; }

        public ShellDamage(double kinetic, double armorPierce, double explosive, double emp, Fragments fragments)
        {
            this.Kinetic = kinetic;
            this.AP = armorPierce;
            this.Explosive = explosive;
            this.EMP = emp;
            this.Fragmentation = fragments;
        }

        public ShellDamage(double kin, double pierce, Fragments frag, double exp = 0, double elec = 0)
        {
            Kinetic = kin;
            AP = pierce;
            Explosive = exp;
            EMP = elec;
            Fragmentation = frag;
        }

        public ShellDamage(double vel, double kin, double pierce, double exp = 0, double elec = 0)
        {
            Kinetic = kin;
            AP = pierce;
            Explosive = exp;
            EMP = elec;
            Fragmentation = null;
        }

        public ShellDamage(double baseKinetic, double baseAP, Pellets effectivePellets)
        {
            Kinetic = baseKinetic + (100.0 * effectivePellets.Hardner);
            AP = baseAP + (1.5 * effectivePellets.Hardner);
            Explosive = 200.0 * effectivePellets.Explosive;
            EMP = effectivePellets.EMP * (10.0 * volume);
            Fragmentation = new Fragments(effectivePellets.Fragmentation);
        }
    }

    public struct Fragments
    {
        public double count { get; set; }
        public double kinetic { get; set; }
        public const double AP = 6;

        public Fragments(double n)
        {
            count = Math.Min(n, 60.0);
            kinetic = n > 60.0 ? ((100.0 * n) / 60.0) : 100.0;
        }
    }

    public struct Block
    {
        public double Health { get; set; }
        public double Weight { get; set; }
        public double ArmourClass { get; set; }
        public double CombinedAC { get; set; }
        public Tuple<int, int, int> Size { get; set; }
        public int Cost { get; set; }
        public bool Destroyed { get; set;  }
    }

    public struct Pellets
    {
        public double Hardner { get; set; }
        public double Explosive { get; set; }
        public double EMP { get; set; }
        public double Fragmentation { get; set; }

        private double Total { get; set; }
        private double Volume { get; set; }

        public Pellets(double volume, double hardner = 0, double explosive = 0, double fragmentation = 0, double emp = 0)
        {
            Hardner = hardner;
            Explosive = explosive;
            Fragmentation = fragmentation;
            EMP = emp;
            Volume = volume;

            this.Total = this.Hardner + this.Explosive + this.EMP + this.Fragmentation;
        }

        public double GetTotal()
        {
            return this.Total;
        }

        public double GetDensity()
        {
            return this.Total / Volume;
        }

        public ShellDamage GetDamage(double velocity, bool isEffective = true)
        {
            Pellets temp = isEffective ? this : this.EffectivePellets();
            return new ShellDamage(
                (2.0 * velocity * Volume) + (100.0 * temp.Hardner),
                3.0 + (velocity / 150.0) + (1.5 * temp.Hardner),
                200.0 * temp.Explosive,
                10.0 * Volume * temp.EMP,
                new Fragments(temp.Fragmentation)
            );
        }

        public double EffectiveTotal()
        {
            return EffectiveTotal(this.GetDensity());
        }

        public double EffectiveTotal(double density)
        {
            return 10.0 * (1.0 - Math.Pow(0.9, density));
        }

        public double EffectiveToTotalRatio()
        {
            return this.EffectiveToTotalRatio(this.GetDensity());
        }

        public double EffectiveToTotalRatio(double density)
        {
            return this.EffectiveTotal(density) / this.Total;
        }

        public Pellets EffectivePellets()
        {
            return this.EffectivePellets(this.GetDensity());
        }

        public Pellets EffectivePellets(double density)
        {
            double effectiveToTotalRatio = this.EffectiveToTotalRatio(density);
            return new Pellets(
                Hardner * effectiveToTotalRatio,
                Explosive * effectiveToTotalRatio,
                EMP * effectiveToTotalRatio,
                Fragmentation * effectiveToTotalRatio
            );
        }
    }

    public struct ShellInstance
    {
        public Pellets EffectivePellets { get; set; }
        public ShellDamage Damage { get; set; }
        public double AngleMultiplier { get; set; }
        public double StartingDamagePotential { get; set; }
        public double EndingDamagePotential { get; set; }
        public int BlocksDestroyed { get; set; }

        public ShellInstance(Pellets pellets, double volume, double velocity, double angleOfPenetration = 0, bool radians = true)
        {
            BlocksDestroyed = 0;
            StartingDamagePotential = 0;
            EndingDamagePotential = 0;

            this.AngleMultiplier = Math.Cos(radians ? angleOfPenetration : angleOfPenetration * (Math.PI / 180.0));
            this.EffectivePellets = pellets.EffectivePellets();
            this.Damage = EffectivePellets.GetDamage(velocity);
        }

        private static readonly Random richocetChance = new Random();
        public bool CheckRichocet(Block outmostBlock)
        {
            double multiplier = DamageMath.DefaultDamageMultiplier(Damage.AP, outmostBlock.CombinedAC);
            StartingDamagePotential = Damage.Kinetic;
            EndingDamagePotential = Damage.Kinetic;

            double firstHit = Damage.Kinetic * multiplier * AngleMultiplier;
            // if the shell is unable to penetrate
            if (firstHit < outmostBlock.Health)
            {
                // if the shell richocets
                EndingDamagePotential = Damage.Kinetic - (firstHit / (
                    richocetChance.NextDouble() <= Math.Pow(
                        1.0 - AngleMultiplier,
                        (2.0 * Damage.AP) / outmostBlock.CombinedAC
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

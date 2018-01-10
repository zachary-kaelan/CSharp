using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Economics_Utility
{
    enum AdvantageType { Comparative, Absolute }

    public class Country
    {
        public int laborHours { get; set; }
        public List<Good> goods { get; set; } // name of the good and number of labor hours to produce

        public Country(int hours, Dictionary<string, int> goods)
        {
            laborHours = hours;
            this.goods = goods.Select(g => new Good(g.Key, g.Value, this.laborHours)).ToList();
        }

        public bool CompareTo(Country value, string good, AdvantageType type = AdvantageType.Comparative)
        {
            if (type == AdvantageType.Comparative)
                return goods.Single(g => g.name == good).hoursToProduce / goods.Single(g => g.name != good).hoursToProduce;
        }
    }

    public struct Good
    {
        public string name { get; set; }
        public double hoursToProduce { get; set; }
        public int maxPossible { get; set; }
        public bool absoluteAdvantage { get; set; }
        public bool comparativeAdvantage { get; set; }

        public Good(string n, int h, int totalHours)
        {
            name = n;
            hoursToProduce = h;
            absoluteAdvantage = false;
            comparativeAdvantage = false;
            maxPossible = totalHours / hoursToProduce;
        }
    }

    
}

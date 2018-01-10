using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPSOptimization
{
    class Defender
    {
        public double AR { get; set; }
        public double HP { get; set; }
        public double HP5 { get; set; }

        public Defender(double[] p, double lvl = 18.0)
        {
            this.AR = p[0] + 24 + (3.5 * lvl);
            this.HP = p[1] + 580.0 + (80.0 * lvl);
            this.HP5 = p[2] + 8.0 + (0.7 * lvl);
        }
    }

    class Attacker
    {
        public double DMG { get; set; }
        public double aSPD { get; set; }
        public double cCNC { get; set; }
        public double cDMG { get; set; }
        public double flatAPEN { get; set; }
        public double percAPEN { get; set; }

        public Attacker(double[] p, double lvl = 18.0)
        {
            this.DMG = p[0] + 55.0 + (3.0 * lvl);
            this.aSPD = (p[1] + 1 + (3.0 * lvl)) * 0.625;
            this.cCNC = p[2];
            this.cDMG = p[3] + 2.0;
            this.flatAPEN = p[4];
            this.percAPEN = p[5];
        }
    }
    
    class Arena
    {
        public double Level { get; set; }
        public Arena(double lvl)
        {
            this.Level = lvl;
        }

        public Results Battle(double[] atkP, double[] defP)
        {
            Attacker atk = new Attacker(atkP, this.Level);
            Defender def = new Defender(defP, this.Level);
        }
    }

    class Results
    {

    }

    class Store
    {
        const double AD = 35.0;
        const double AP = 21.75;
        const double AR = 20.0;
        const double MR = 18.0;
        const double HP = (8.0 / 3.0);
        const double MP = 1.4;
        const double HP5 = 3.0;
        const double MP5 = 5.0;
        const double CritP = 40.0;
        const double AS = 25.0;
        const double MS = 12.0;


    }

    class Item
    {

    }
}

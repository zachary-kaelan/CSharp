using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalsLib
{
    class Constants
    {
    }

    public enum ElementCategory : byte
    {
        AlkaliMetals,
        AlkalineEarthMetals,
        Pnictogens,
        Chalcogens,
        Halogens,
        NobleGases,
        Lanthanoids,
        Actinoids,
        RareEarthMetals,
        TransitionElements
    }

    public enum MajorElementCategory : byte
    {
        Unknown = 0,
        Metals,
        Metalloids,
        Nonmetals
    }

    public enum ReactionType
    {
        Synthesis,
        Decomposition,
        SingleReplacement,
        DoubleReplacement
    }

    public enum ChemicalState
    {
        Irrelevant,
        Solid,
        Liquid,
        Gaseous,
        Aqueous // dissolved in water
    }
}

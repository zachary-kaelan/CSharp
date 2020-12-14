using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DNALib
{
    [Flags]
    public enum Nucleotide : byte
    {
        Zero = 0,                                           // 0000 |   0 
        Adenine = 1,                                        // 1000 |   1
        Cytosine = 2,                                       // 0100 |   2
        Guanine = 4,                                        // 0010 |   4
        Thymine = 8,                                        // 0001 |   8

        Purine = Guanine | Adenine,                         // 1010 |   5
        Pyrmidine = Thymine | Cytosine,                     // 0101 |   10
        Keto = Guanine | Thymine,                           // 0011 |   12
        Amino = Adenine | Cytosine,                         // 1100 |   3
        Strong_Bonds = Guanine | Cytosine,                  // 0110 |   6
        Weak_Bonds = Adenine | Thymine,                     // 1001 |   9

        Not_A = Cytosine | Guanine | Thymine,               // 0111 |   14
        Not_C = Adenine | Guanine | Thymine,                // 1011 |   13
        Not_G = Adenine | Cytosine | Thymine,               // 1101 |   11
        Not_T = Cytosine | Guanine | Adenine,               // 1110 |   7
        Any = Adenine | Cytosine | Guanine | Thymine        // 1111 |   15
    }

    public enum AminoProperties : byte
    {
        Nonpolar,
        Polar,
        Basic,
        Acidic,
        Start_Nonpolar = 4,
        Stop = 8
    }

    [Flags]
    public enum DNACodonFlags
    {
        NIL = 0,
        Phe = 1,        // F    Phenylalanine
        Leu = 2,        // L    Leucine
        Ile = 4,        // I    Isoleucine
        Met = 8,        // M    Methionine
        Val = 16,       // V    Valine

        Ser = 32,       // S    Serine
        Pro = 64,       // P    Propine
        Thr = 128,      // T    Threonine
        Ala = 256,      // A    Alanine

        Tyr = 512,      // Y    Tyrosine
        Och = 1024,     // Stop Ochre
        Pyl = 2048,     // O    Pyrrolysine/Amber
        His = 4096,     // H    Histidine
        Gln = 8192,     // Q    Glutamine
        Asn = 16384,    // N    Asparagine
        Lys = 32768,    // K    Lysine
        Asp = 65536,    // D    Aspartic acid
        Glu = 131072,   // E    Glutamic acid
        Cys = 262144,   // C    Cysteine
        Sec = 524288,   // U    Selenocysteine/Opal (or umber, in DNA)
        Trp = 1048576,  // W    Tryptophan
        Arg = 2097152,  // R    Arginine
        Gly = 4194304,  // G    Glycine

        B = Asp | Asn,
        J = Leu | Ile,
        Z = Glu | Gln,
        X = 8388607,

        STOP = Och | Pyl | Sec
    }

    internal static class Constants
    {
        static Constants()
        {
            ALL_POWERS_OF_2_REVERSE = new List<ulong>(64);
            ALL_POWERS_OF_2_REVERSE.Add(1);
            ulong prev = 1;
            for (int i = 1; i < 64; ++i)
            {
                prev *= 2;
                ALL_POWERS_OF_2_REVERSE.Add(prev);
            }
        }

        public static readonly List<byte> POWERS_OF_2 = new List<byte>() { 128, 64, 32, 16, 8, 4, 2, 1 };
        public static List<ulong> ALL_POWERS_OF_2_REVERSE { get; private set; }
        public static readonly List<byte> POWERS_OF_2_REVERSE = new List<byte>() { 1, 2, 4, 8, 16, 32, 64, 128 };

        public const string NUCLEOTIDE_LETTERS = "ZACMGRSVTWYHKDBN";

        // Amber is J for Jurassic World
        // Ochre is O
        // Opal is U for Umber
        public const string CODON_LETTERS =
        //         T        C        A        G
        /*T*/    "FFLL" + "LLLL" + "IIIM" + "VVVV" +
        /*C*/    "SSSS" + "PPPP" + "TTTT" + "AAAA" +
        /*A*/    "YYOJ" + "HHQQ" + "NNKK" + "DDEE" +
        /*G*/    "CCUW" + "RRRR" + "SSRR" + "GGGG";
        //        TCAG     TCAG     TCAG     TCAG

        public static readonly SortedDictionary<Nucleotide, Nucleotide> COMPLEMENTS = new SortedDictionary<Nucleotide, Nucleotide>()
        {
            { Nucleotide.Adenine, Nucleotide.Thymine },
            { Nucleotide.Cytosine, Nucleotide.Guanine},
            { Nucleotide.Guanine, Nucleotide.Cytosine },
            { Nucleotide.Thymine, Nucleotide.Adenine },
            { Nucleotide.Weak_Bonds, Nucleotide.Weak_Bonds },
            { Nucleotide.Strong_Bonds, Nucleotide.Strong_Bonds },
            { Nucleotide.Amino, Nucleotide.Keto },
            { Nucleotide.Keto, Nucleotide.Amino },
            { Nucleotide.Purine, Nucleotide.Pyrmidine },
            { Nucleotide.Pyrmidine, Nucleotide.Purine },
            { Nucleotide.Not_A, Nucleotide.Not_T },
            { Nucleotide.Not_C, Nucleotide.Not_G },
            { Nucleotide.Not_G, Nucleotide.Not_C },
            { Nucleotide.Not_T, Nucleotide.Not_A },
            { Nucleotide.Any, Nucleotide.Any },
            { Nucleotide.Zero, Nucleotide.Zero }
        };
    }
}

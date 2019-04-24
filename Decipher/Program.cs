using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RestSharp;
using ZachLib;

namespace Decipher
{
    class Program
    {
        static void Main(string[] args)
        {
            //RestClient client = new RestClient("wordfinder.yourdictionary.com/unscramble/");
            List<char> letters = "dekorvwz".ToList();
            string[] words = new string[]
            {
                "worked",
                "drove",
                "roved",
                "dower",
                "rowed",
                "dozer",
                "vowed",
                "vower",
                "drek",
                "doer",
                "dore",
                "redo",
                "rode",
                "dove",
                "devo",
                "owed",
                "doze",
                "drew",
                "dork",
                "word",
                "kore",
                "woke",
                "zerk",
                "over",
                "rove",
                "wore",
                "zero",
                "wove",
                "work",
                "vrow",
                "doe",
                "ode",
                "red",
                "dev",
                "dew",
                "wed",
                "zed",
                "dor",
                "rod",
                "dow",
                "oke",
                "zek",
                "ore",
                "roe",
                "voe",
                "owe",
                "woe",
                "rev",
                "kor",
                "wok",
                "row",
                "vow",
                "de",
                "do",
                "ed",
                "er",
                "od",
                "oe",
                "or",
                "ow",
                "re",
                "we",
                "wo"
            };

            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            string boloMachete = "anyok yeqkany'ye hvay'k l'gnoo eydokl'x : sekall'o ammk'ix";
            var dict = new SortedDictionary<char, int>(boloMachete.Where(c => Char.IsLetter(c)).GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count()));
            foreach(var alpha in alphabet.Except(dict.Keys))
            {
                dict.Add(alpha, 0);
            }
            int max = Convert.ToInt32(
                Math.Round(
                    StringAnalysis.LETTER_FREQUENCIES.Max(
                        l => dict.Max(
                            c => c.Value + l.Value
                        )
                    )
                )
            ) + 4;

            foreach(var kv in dict)
            {
                int letterFrequency = Convert.ToInt32(Math.Round(StringAnalysis.LETTER_FREQUENCIES[kv.Key]));
                Console.WriteLine(kv.Key + " - " + kv.Value.ToString() + "; \t" + new string('-', kv.Value) + "; " + new string(' ', max - (letterFrequency + kv.Value)) + new string('-', letterFrequency));
            }
            Console.ReadLine();

            List<string> combinations = new List<string>();
            foreach(string word in words)
            {

            }

        }
    }

}

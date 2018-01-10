using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;
using Jil;
using PPLib;
using RestSharp;

namespace Temp
{
    public static class Psychonauts
    {
        public static void Execute()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            RestClient client = new RestClient("https://psychonautwiki.org");
            timer.Stop();
            Console.WriteLine("Request \t\t: " + timer.ElapsedMilliseconds.ToString());
            timer.Restart();
            string effectIndex = client.Execute(new RestRequest("/wiki/Subjective_effect_index", Method.GET)).Content;
            int indexTemp = effectIndex.IndexOf(">contributors</a>.");
            effectIndex = effectIndex.Substring(indexTemp, effectIndex.IndexOf(">See also<") - indexTemp);
            timer.Stop();
            Console.WriteLine("Setup \t\t: " + timer.ElapsedMilliseconds.ToString());
            timer.Restart();

            RecursiveDictionary<RecursiveDictionaryValue> headers = new RecursiveDictionary<RecursiveDictionaryValue>(effectIndex, HTMLContent.ListElements);
            timer.Stop();
            Console.WriteLine("Initialization\t: " + timer.ElapsedMilliseconds.ToString());
            timer.Restart();
            File.WriteAllText(
                @"E:\Nootropics\Psychonauts\EffectsCategories.txt",
                "{" + (headers.Count == 1 ? headers.Single().Value.Serialize() : headers.Serialize()) + "}"
            );
            Console.WriteLine("Serialization\t: " + timer.ElapsedMilliseconds.ToString());
        }
    }
}

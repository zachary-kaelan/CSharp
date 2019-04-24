using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatamuseLib;
using ZachLib;

namespace DatamuseTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            SortedDictionary<string, long> verbs = new SortedDictionary<string, long>(
                Utils.LoadCSVDictionary(
                    @"E:\Programming\Chris Crawford\Full Verb List.csv"
                ).ToDictionary(kv => kv.Key, kv => Convert.ToInt64(kv.Value))
            );

            foreach(var verb in verbs)
            {
                IEnumerable<DatamuseLib.Models.FullLexicalModel> results = API.Words(
                    APIConstraints.MeansLike,
                    ExternalConstraints.None,
                    ExtraLexicalKnowledge.PartsOfSpeech | ExtraLexicalKnowledge.WordFrequency,
                    verb.Key
                );

                if (results.Any())
                {
                    int numResults = results.Count();
                    results = results.Where(w => w.PartsOfSpeech.HasFlag(PartOfSpeech.Verb));
                    if (results.Any())
                    {
                        int numVerbs = results.Count();
                        results = results.Where(v => verbs.ContainsKey(v.Word));
                        if (results.Any())
                        {
                            Console.WriteLine("{0}: {1} freq, {2} syn, {3} verbs, {4} redundancies", verb.Key, verb.Value, numResults, numVerbs, results.Count());
                            foreach(var result in results)
                            {
                                Console.WriteLine("\t{0}: {1}", result.Word, result.WordFrequency);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }
    }
}

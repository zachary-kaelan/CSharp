using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PPLib;

namespace TextComparisons
{
    class Program
    {
        static void Main(string[] args)
        {
            var vids = Directory.GetDirectories(@"E:\YouTube Downloading").Select(
                v => Directory.GetDirectories(v).Select(
                    q => new KeyValuePair<string, IEnumerable<IEnumerable<KeyValuePair<string, string>>>>(
                        Path.GetDirectoryName(q),
                        Directory.GetFiles(q).Select(
                            s => File.ReadAllLines(s).Select(
                                l => l.Split('\t')
                            ).Select(
                                l => new KeyValuePair<string, string>(
                                    l[0], l[1]
                                )
                            )
                        )
                    )
                )
            );

            var allParams = vids.SelectMany(
                v => v.SelectMany(
                    q => q.Value.SelectMany(
                        s => s
                    )
                )
            ).Distinct(new TextComparisons.KeyValuePairComparer<string, string>());

            //var allParams = paramsTemp.Distinct(new KeyValuePairComparer<string, string>());
            var paramsCounts = allParams.DistinctCounts();

            var paramsByVid = vids.Select(
                v => v.SelectMany(
                    q => q.Value.SelectMany(
                        s => s
                    )
                ).Distinct(new TextComparisons.KeyValuePairComparer<string, string>()).DistinctCounts()
            );

            var paramsByQuality = vids.SelectMany(
                v => v
            ).GroupBy(
                q => q.Key,
                q => q.Value,
                (k, g) => g.SelectMany(
                    q => q.SelectMany(
                        s => s
                    )
                ).Distinct(new TextComparisons.KeyValuePairComparer<string, string>()).DistinctCounts()
            );

            var paramsBySection = vids.SelectMany(
                v => v.SelectMany(
                    q => q.Value.Select(
                        (s, i) => new KeyValuePair<int, IEnumerable<KeyValuePair<string, string>>>(i, s)
                    )
                )
            ).GroupBy(
                s => s.Key,
                s => s.Value,
                (k, g) => g.SelectMany(
                    s => s
                ).Distinct(new TextComparisons.KeyValuePairComparer<string, string>()).DistinctCounts()
            );

            var paramNames = allParams.Select(s => s.Key).Distinct().ToArray();


            Dictionary<string, string> paramNeeds = paramNames.ToDictionary(
                p => p,
                paramName => {
                    if (paramsCounts[paramName] == 1)
                        return "Constant";
                    else if (paramsByVid.All(d => d[paramName] == 1))
                        return "ByVid";
                    else if (paramsByQuality.All(d => d[paramName] == 1))
                        return "ByQuality";
                    else if (paramsBySection.All(d => d[paramName] == 1))
                        return "BySection";
                    else
                        return "Unknown";
                }
            );

            paramNeeds.OrderBy(p => p.Value).SaveDictAs(@"E:\Temp\YouTubeDownloadParams.txt");

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }

    public class KeyValuePairComparer<K, V> : EqualityComparer<KeyValuePair<K, V>>
    {
        public override bool Equals(KeyValuePair<K, V> x, KeyValuePair<K, V> y)
        {
            return x.Key.Equals(y.Key) && x.Value.Equals(y.Value);
        }

        public override int GetHashCode(KeyValuePair<K, V> obj)
        {
            return obj.GetHashCode();
        }
    }
}

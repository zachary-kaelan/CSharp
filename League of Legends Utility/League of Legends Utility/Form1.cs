using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jil;
using RiotAPI;
using RiotAPI.STATIC;
using RiotAPI.MATCHES;

namespace League_of_Legends_Utility
{
    public partial class Form1 : Form
    {
        private Client client = null;
        private ItemListDto items;
        private Dictionary<string, double> goldValues = new Dictionary<string, double>();

        public const string sanDescStatsPat = @"\+(\d+?)(%)? ([a-zA-Z ]+?)(?<!per|level|target|of|champion|while|vs\.|and|in jungle)(?<!per |level |target |of |champion |while |vs\. |and | in jungle )(?=\+|<|UNIQUE)";
        public const RegexOptions rgxOpts = RegexOptions.IgnoreCase;
        public static TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromMilliseconds(1000);

        public Form1()
        {
            InitializeComponent();
            //client = new Client(true);


        }

        private void statGoldValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void statGoldValues()
        {
            var itemList = items.data.ToList();
            itemList.RemoveAll(
                i => i.Value.consumed ||
                !i.Value.gold.purchaseable ||
                !String.IsNullOrEmpty(i.Value.requiredChampion)
            );

            var singleStat = itemList.FindAll(
                i => i.Value.stats.Count(s => s.Value != 0) == 1 &&
                    i.Value.from.Count == 0
            ).Select(
                i => new KeyValuePair<double, KeyValuePair<string, double>>(
                    Convert.ToDouble(i.Value.gold.total),
                    i.Value.stats.Single(s => s.Value != 0)
                )
            ).GroupBy(i => i.Value.Key).Select(
                g => g.OrderBy(i => i.Key).First()
            ).ToList();

            foreach(var stat in singleStat)
            {
                goldValues.Add(
                    stat.Value.Key,
                    stat.Key / stat.Value.Value
                );
            }

            itemList.RemoveAll(
                i => i.Value.stats.Count(s => s.Value != 0) == 1);

            while (true)
            {
                var multiStats = itemList.FindAll(
                    i => i.Value.stats.Count(s => s.Value != 0 && !goldValues.TryGetValue(s.Key, out _)) == 1
                ).Select(
                    i => new KeyValuePair<double, KeyValuePair<KeyValuePair<string, double>, List<KeyValuePair<string, double>>>>(
                        Convert.ToDouble(i.Value.gold.total),
                        new KeyValuePair<KeyValuePair<string, double>, List<KeyValuePair<string, double>>>(
                            i.Value.stats.Single(s => s.Value != 0 && !goldValues.TryGetValue(s.Key, out _)),
                            i.Value.stats.ToList().FindAll(s => s.Value != 0 && goldValues.TryGetValue(s.Key, out _))
                        )
                    )
                ).GroupBy(i => i.Value.Key).Select(
                    g => g.OrderBy(i => i.Key).First()
                ).ToList();

                if (multiStats.Count == 0)
                    break;

                foreach (var stat in multiStats)
                {
                    goldValues.Add(
                        stat.Value.Key.Key,
                        (stat.Key - stat.Value.Value.Sum(s => goldValues[s.Key] * s.Value)) / stat.Value.Key.Value
                    );
                }
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            items = JSON.Deserialize<ItemListDto>(
                File.ReadAllText(Client.STATIC_PATH + "items.txt")
            );

            var descriptions = items.data.Select(
                i => i.Value.description + i.Value.sanitizedDescription
            ).ToList();

            var descStats = descriptions.FindAll(d => Regex.IsMatch(d, sanDescStatsPat, rgxOpts, DEFAULT_TIMEOUT))
            .SelectMany(
                d => Regex.Matches(d, sanDescStatsPat, rgxOpts, DEFAULT_TIMEOUT).Cast<Match>()
            ).GroupBy(
                s => s.Groups[3].Value.Trim() + s.Groups[2].Value
            ).Select(g => g.First()).OrderBy(m => m.Groups[3].Value)
            .ToDictionary(m => m.Groups[3].Value + m.Groups[2].Value, m => m.Groups[1].Value);

            /*var descStatGroups = descStats.GroupBy(
                s => s.Value
            );

            descStats = descStatGroups.Select(g => g.First()).ToDictionary(
                m => m.Key, m => m.Value
            );*/
        }
    }
}

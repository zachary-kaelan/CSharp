using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WargamingAPI;
using WargamingAPI.Models;

namespace BestAwardsFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static readonly Dictionary<string, Tuple<string, string, string, string>> AWARDS_NAMES = new Dictionary<string, Tuple<string, string, string, string>>()
        {
            {"mediumTank", new Tuple<string, string, string, string>("scout", "Scout", "Free XP", "Medium") },
            {"lightTank", new Tuple<string, string, string, string>("evileye", "Patrol Duty", "Combat XP", "Light") },
            {"heavyTank", new Tuple<string, string, string, string>("warrior", "Top Gun", "Crew XP", "Heavy") },
            {"AT-SPG", new Tuple<string, string, string, string>("supporter", "Confederate", "Spare Parts", "TD") }
        };

        private Dictionary<string, Player> CurrentSearchResults = null;
        private string nickname = null;
        private Player player = null;
        private string lastSuccessfulSearch = null;
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            string text = txtUsername.Text;
            txtUsername.AutoCompleteCustomSource.Clear();
            if (text.Length >= 3 && text.Length <= 24)
            {
                if (!CurrentSearchResults.TryGetValue(text, out player))
                {
                    CurrentSearchResults = API.SearchPlayers(text);
                    txtUsername.AutoCompleteCustomSource.AddRange(CurrentSearchResults.Keys.Take(15).ToArray());
                }
                else
                    nickname = text.ToUpper();

                if (CurrentSearchResults.Count != 0)
                    lastSuccessfulSearch = text;
                else
                    txtUsername.Text = lastSuccessfulSearch;
            }
                
        }

        private void btnGetResults_Click(object sender, EventArgs e)
        {
            string text = txtUsername.Text.ToUpper();
            if (nickname != text)
            {
                nickname = text;
                CurrentSearchResults = API.SearchPlayers(text, true);
                if (CurrentSearchResults.Count != 1)
                {

                    return;
                }
                var single = CurrentSearchResults.Single();
                nickname = single.Key;
                player = single.Value;
            }

            var allAwards = player.GetAllAwards();
            var stats = player.GetVehicleStats();
            var awards = player.GetVehicleAwards().ToDictionary(a => a.tank_id, a => a.achievements);
            var mean = stats.Average(s => s.all.battles);
            var stdDev = stats.Average(s => Math.Pow(s.all.battles - mean, 2.0));
            var cutoff = Convert.ToInt32(Math.Min(Math.Max(Math.Floor(mean - stdDev), 8.0), 24.0));
            stats = stats.Where(s => s.all.battles >= cutoff).ToArray();

            var minutesTotal = stats.Sum(v => v.battle_life_time) / 60.0;
            foreach(var award in AWARDS_NAMES.Values)
            {
                grdOverall.Rows.Add(award.Item4, String.Format("1 Standard Cooldown Booster & 1 Standard {0} Booster", award.Item3), award.Item2, minutesTotal / allAwards.achievements[award.Item1]);
            }

            Dictionary<string, List<Tuple<string, string, string, double>>> awardStats = AWARDS_NAMES.Values.ToDictionary(v => v.Item1, v => new List<Tuple<string, string, string, double>>());
            foreach (var tank in stats)
            {
                var tankInfo = API.TANKS[tank.tank_id];
                var stuff = AWARDS_NAMES[tankInfo.type];
                awardStats[stuff.Item1].Add(
                    new Tuple<string, string, string, double>(
                        tankInfo.name + " - " + tankInfo.nation + " - " + tankInfo.tier.ToString(),
                        tankInfo.images.preview,

                    )
                );
            }
            var lifeTimes = stats.ToDictionary(s => s.tank_id, s => s.battle_life_time / 60.0);

        }
    }
}

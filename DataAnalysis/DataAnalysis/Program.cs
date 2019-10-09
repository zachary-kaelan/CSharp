using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Jil;
using ZachLib;
using ZachLib.Statistics;
using TinderAPI.Models;
using InstagramLib;

namespace DataAnalysis
{
    class Program
    {
        private static string[] _headers = new string[]
        {
            "Number of Photos",
            "Distance",
            "Age",
            "Job Listed",
            "School Listed",
            "City Listed",
            "Number of Instagram Photos",
            "Bio Length",
            "Spotify",
            "Social Media Count"
        };

        static void Main(string[] args)
        {
            Internal.Initialize();
            DateTime now = DateTime.Now;
            Regex rgxActualAge = new Regex(@"I'?m (?:actually)? (\d\d)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            SortedSet<string> types = new SortedSet<string>();
            int numGroupMatched = 0;

            var files = Directory.GetFiles(@"E:\Programming\Tinder", "*json");

            Matrix inputs = new Matrix(files.Length, 10);
            double[] outputs = new double[files.Length];

            int index = 0;
            foreach(var file in files)
            {
                var info = Utils.LoadJSON<TinderData>(file);
                outputs[index] = info.score;

                bool hasBio = !String.IsNullOrWhiteSpace(info.bio);

                inputs[index, 0] = info.num_photos; ;
                inputs[index, 1] = info.distance_mi.HasValue ? info.distance_mi.Value : 25;
                inputs[index, 3] = info.jobs == null ? 0 : 1;
                inputs[index, 4] = info.schools == null ? 0 : 1;
                inputs[index, 5] = info.city == null ? 0 : 1;

                if (hasBio)
                {
                    inputs[index, 7] = info.bio.Length;
                    var actualAge = rgxActualAge.Match(info.bio);
                    if (actualAge.Success)
                        inputs[index, 2] = Convert.ToInt32(actualAge.Groups[1].Value);
                    else
                    {
                        DateTime birth_date = info.birth_date;
                        inputs[index, 2] = now.Year - birth_date.Year;
                    }
                }

                int socialMediaCount = 0;

                if (info.instagram_info != null)
                    inputs[index, 6] = info.instagram_info.media_count;
                else if (info.insta_media_count.HasValue)
                    inputs[index, 6] = info.insta_media_count.Value;
                else if (!String.IsNullOrWhiteSpace(info.instagram_user) && info.instagram_user.Length >= 6)
                {
                    try
                    {
                        var user = Internal.GetUser(info.instagram_user);
                        inputs[index, 6] = user.edge_owner_to_timeline_media.count;
                        ++socialMediaCount;
                    }
                    catch
                    {
                    }
                }

                if (!String.IsNullOrWhiteSpace(info.snapchat))
                    ++socialMediaCount;
                if (!String.IsNullOrWhiteSpace(info.venmo))
                    ++socialMediaCount;
                inputs[index, 9] = socialMediaCount;

                if (info.spotify != null)
                {
                    inputs[index, 8] = info.spotify.num_artists + (!String.IsNullOrWhiteSpace(info.spotify.theme_track) ? 3 : 0);
                }

                if (info.group_matched.HasValue && info.group_matched.Value)
                    ++numGroupMatched;

                ++index;
            }

            for (int c = 0; c <= 9; ++c)
            {
                Console.WriteLine(_headers[c] + ": " + inputs.GetColumn(c).GetPearsonCorrelation(outputs).ToString("#.0000"));
            }

            inputs.Serialize("", "Tinder_Data", 0);
            Console.ReadLine();
        }
    }
}

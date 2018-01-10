using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProtoBuf;
using ProtoBuf.Meta;
using ProtoBuf.ServiceModel;

namespace Slack
{
    static class Program
    {
        static RegexOptions opts = RegexOptions.IgnoreCase | RegexOptions.Multiline;
        public static Dictionary<KeyValuePair<string, string>, List<string>> sortedSlackFiles = new Dictionary<KeyValuePair<string, string>, List<string>>()
        {
            {
                new KeyValuePair<string, string>(
                    "Agreements",
                    @"^\s{4}(?:(?:.*?[_ -](?:SA|agreement)[_ -])|(?:\d{3,7})).*?\.pdf\s{4}$"
                ), new List<string>()
            },
            {
                new KeyValuePair<string, string>(
                    "Routes",
                    @"(?:\sr\.?s\s)|(?:route.sheet)|(?:^\s{4}[a-z.\s]{3,15}.\d\d?[.-]\d\d?)"
                ), new List<string>()
            },
            {
                new KeyValuePair<string, string>(
                    "Snips_Screens",
                    @"^.*\s(?:snip)|(?:screen\s?shot)|(?:pasted)|(?:capture).*$"
                ), new List<string>()
            }
        };
        /*
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        */

        /*const string OAuthToken = "xoxp-16172464452-184906848646-221143767396-b4ffa3d8deaa0d1386daa178932f1b62";
        const string ClientID = "16172464452.222180667527";
        const string ClientSecret = "9bdacab3a43293f41e6956728d8eb8fe";
        const string VerifToken = "uiyaTwqgWrutFZRJdHrprjoQ";*/

        static void Main()
        {
            SlackClient slack = new SlackClient("16172464452.222180667527", "9bdacab3a43293f41e6956728d8eb8fe", "xoxp-16172464452-184906848646-221143767396-b4ffa3d8deaa0d1386daa178932f1b62", "uiyaTwqgWrutFZRJdHrprjoQ");

            /*var files = client.GetAllSlackFiles(
                (DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0)) - client.UnixTime)
                .TotalSeconds.ToString()
            ).OrderBy(f => f.created).ToList();

            Console.WriteLine("{0} total files.", files.Count);
            Console.WriteLine("{0} files do not qualify.", files.RemoveAll(f => f.channels.Contains("C0N6F9YVB") || f.channels.Contains("C0P2X8C2K")));
            */

            /*
            System.IO.SlackFile.WriteAllLines(SlackClient.filesPath + "SlackFileNames0.txt", files.OrderBy(f => f.name).Select(f => "    " + f.name + "    "));
            SortSlackFileNames(
                files.Distinct(
                    new SlackFileComparer()
                ).Select(f => "    " + f.name + "    ").ToList()
            );
            */

            /*
            System.IO.SlackFile.AppendAllLines(
                SlackClient.filesPath, client.DownloadSlackFiles(files)
                    .Select(f => f.id + "\t~ " + f.name + "\t~ " + f.size.ToString())
            );
            */

            /*
            List<string> filesList = Directory.GetFiles(SlackClient.filesPath).Select(f => new FileInfo(f).Name).ToList();
            foreach (var key in sortedSlackFiles.Keys)
            {
                foreach (var file in filesList)
                {
                    if (file.StartsWith(key.Key))
                        File.Move(SlackClient.filesPath + file, SlackClient.filesPath + file.Insert(key.Key.Length, @"\"));
                }
            }

            var groups = filesList.GroupBy(file =>
                file.LastIndexOf('.') == file.Length - 4 ?
                    file.Substring(file.Length - 4).ToLower() :
                    "noExt"
            ).OrderBy(g => g.Key);

            foreach(var grp in groups)
            {
                foreach(var file in grp)
                {
                    if (!System.IO.Directory.Exists(SlackClient.filesPath + grp.Key))
                        System.IO.Directory.CreateDirectory(SlackClient.filesPath + grp.Key);
                    if (file.StartsWith(grp.Key))
                        File.Move(SlackClient.filesPath + file, SlackClient.filesPath + file.Insert(grp.Key.Length, @"\"));
                    else if (file.EndsWith(grp.Key))
                        File.Move(SlackClient.filesPath + file, SlackClient.filesPath + grp.Key + @"\" + file);
                }
            }
            */

            //SortSlackFileNames(System.IO.Directory.GetSlackFiles(SlackClient.filesPath).Select(f => new System.IO.SlackFileInfo(f).Name).ToList());

            //client.DeleteSlackFiles(files);

            //long totalSize = files.Sum(f => f.size);
            //Console.WriteLine("Total Size: {0}", totalSize);

            /*
            foreach (var file in files)
            {
                Console.WriteLine(file.name);
                Console.WriteLine("   Created: \t" + client.UnixTime.AddSeconds(Convert.ToDouble(file.created)).ToString("d"));
                Console.WriteLine("   SlackFileType: \t" + file.filetype);
                Console.WriteLine("   Size: \t{0}", file.size);
                Console.WriteLine("   URL: \t{0}", file.url_private);
                Console.WriteLine();
            }
            */

            //DriveClient.AuthorizeSerivceAcc();
            //DriveClient.SimpleUpload("client_secret.json");

            RTMClient client = new RTMClient(slack, "xoxp-16172464452-184906848646-223197167812-30a94f5a19859a0f878bfadfb8f3c0ad", "xoxb-222616323777-JnDu9W5JrboDNaL5MVFS1g5T", "16172464452.224228723319", "fc4e1e1e4b8323accc3d9a63b2a03708", "75FWQKAKncF5KJD9Sy0XkdeZ");
            client.Start();

           Console.ReadLine();
        }

        /*const string OAuthToken = "xoxp-16172464452-184906848646-223197167812-30a94f5a19859a0f878bfadfb8f3c0ad";
        const string botOAuthToken = "xoxb-222616323777-JnDu9W5JrboDNaL5MVFS1g5T";
        const string ClientID = "16172464452.224228723319";
        const string ClientSecret = "fc4e1e1e4b8323accc3d9a63b2a03708";
        const string VerifToken = "75FWQKAKncF5KJD9Sy0XkdeZ";*/

        public static void SortSlackFileNames(List<string> files)
        {
            //List<string> lines = new List<string>();
            files.Sort();
            foreach(var key in sortedSlackFiles.Keys)
            {
                sortedSlackFiles[key].AddRange(
                    files.FindAll(file =>
                        Regex.IsMatch("    " + file + "    ", key.Value, opts)
                    )
                );

                /*
                System.IO.SlackFile.WriteAllLines(
                    SlackClient.filesPath + key.Key + ".txt",
                    sortedSlackFiles[key]
                );
                */
                string path = SlackClient.filesPath + key.Key + @"\";
                System.IO.Directory.CreateDirectory(path);
                foreach(string file in sortedSlackFiles[key])
                    File.Move(SlackClient.filesPath + file, path + file);

                files = files.Except(sortedSlackFiles[key]).ToList();
            }

            var groups = files.GroupBy(file =>
                file.LastIndexOf('.') == file.Length - 4 ?
                    file.Substring(file.Length - 4).ToLower() :
                    "noExt"
            ).OrderBy(g => g.Key);

            foreach(var grp in groups)
            {
                if (grp.Count() < 3)
                    continue;
                var group = grp.ToList();
                group.Sort();
                string path = SlackClient.filesPath + grp.Key;
                System.IO.Directory.CreateDirectory(path);
                group.ForEach(file => File.Move(SlackClient.filesPath + file, path + file.Substring(4)));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using FChan;
using FChan.Library;
using RestSharp;
using RestSharp.Extensions;
using ZachLib;

namespace FourChanConsole
{
    class Program
    {
        public const string MAIN_PATH = @"C:\Program Files\Microsoft Games\Mahjong\en-US\resources\assets\Stalking\Random People\4chan\Programming\";
        public const string FILE_PATH = MAIN_PATH + "Queen Bee.txt";

        static void Main(string[] args)
        {
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            var board = Chan.GetBoard().Boards.Single(b => b.BoardName == "b");


            Dictionary<int, int> watchedPostNumbers = new Dictionary<int, int>();
            List<int> ignoredPostNumbers = File.Exists(
                FILE_PATH
            ) ? File.ReadAllLines(
                FILE_PATH
            ).Select(
                l => Convert.ToInt32(l)
            ).ToList() : new List<int>();
            Dictionary<int, int> loggedPostNumbers = new Dictionary<int, int>();

            Regex speechPatternsRGX = new Regex(
                @"(?=.*<3{2,})|(?:(?=.*(?:\.{2,}|friends|thx|luv|y{3,}|o{3,}))(?=.*(?:[:;]-[\)\(]|<3)))",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            int counter = 0;
            while (true)
            {
                if (counter == 3)
                {
                    counter = 0;
                    var loggedThreads = loggedPostNumbers.ToDictionary(
                        n => n.Key,
                        n =>
                        {
                            try {
                                return Chan.GetThread("b", n.Key).Posts.Skip(n.Value);
                            } catch {
                                return null;
                            }
                        } 
                    );

                    Console.WriteLine();
                    foreach(var loggedThread in loggedThreads)
                    {
                        if (loggedThread.Value == null)
                            loggedThreads.Remove(loggedThread.Key);
                        else
                        {
                            var posts = loggedThread.Value.Where(
                                p => !String.IsNullOrWhiteSpace(p.Comment)
                            ).Select(
                                p =>
                                {
                                    p.Comment = HttpUtility.HtmlDecode(
                                        ZachRGX.HTML_TAGS.Replace(
                                            p.Comment, ""
                                        )
                                    );
                                    return p;
                                }
                            ).Where(
                                p => speechPatternsRGX.IsMatch(p.Comment)
                            );
                            posts.SaveThreadTo(MAIN_PATH + loggedThread.Key.ToString());
                            loggedPostNumbers[loggedThread.Key] += loggedThread.Value.Count();
                            Console.WriteLine(loggedThread.Key.ToString() + " - " + posts.Count().ToString());
                            if (loggedThread.Value.Last().IsClosed || loggedThread.Value.Last().IsArchived)
                                loggedThreads.Remove(loggedThread.Key);
                        }
                    }
                    Console.WriteLine();
                }

                var threadsTEMP = Enumerable.Range(1, 10).SelectMany(
                    i => Chan.GetThreadPage("b", i).Threads
                ).GroupBy(t => t.Posts.First().PostNumber).Select(t => t.Last()).Where(
                    t =>
                    {
                        var post = t.Posts.First();
                        var comment = HttpUtility.HtmlDecode(ZachRGX.HTML_TAGS.Replace(post.Comment, ""));
                        if (ignoredPostNumbers.Contains(post.PostNumber))
                            return false;
                        else if ((
                            watchedPostNumbers.ContainsKey(post.PostNumber) ||
                            Utils.COMPARE_INFO.IndexOf(
                                comment,
                                "share",
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0 || Utils.COMPARE_INFO.IndexOf(
                                comment,
                                "shouldnt",
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0 || Utils.COMPARE_INFO.IndexOf(
                                comment,
                                "queen",
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0 || Utils.COMPARE_INFO.IndexOf(
                                comment,
                                "save",
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0 || Utils.COMPARE_INFO.IndexOf(
                                comment,
                                "femanon",
                                Utils.IGNORE_CASE_AND_SYMBOLS
                            ) >= 0
                        ))
                            return true;
                        else
                        {
                            ignoredPostNumbers.Add(post.PostNumber);
                            return false;
                        }
                    }
                ).Select(
                    t =>
                    {
                        var postnumber = t.Posts.First().PostNumber;
                        t = Chan.GetThread("b", postnumber);
                        int count = t.Posts.Count();
                        IEnumerable<Post> posts = null;
                        if (
                            watchedPostNumbers.TryGetValue(
                                t.Posts.First().PostNumber,
                                out int num
                        ))
                        {
                            posts = t.Posts.Skip(num);
                            count -= num;
                        }
                        else
                            posts = t.Posts.Skip(1);

                        return new KeyValuePair<int, KeyValuePair<int, Post[]>>(
                            postnumber,
                            new KeyValuePair<int, Post[]>(
                                count,
                                posts.Where(
                                    p => !String.IsNullOrWhiteSpace(p.Comment)
                                ).Select(
                                    p =>
                                    {
                                        p.Comment = HttpUtility.HtmlDecode(
                                            ZachRGX.HTML_TAGS.Replace(
                                                p.Comment, ""
                                            )
                                        );
                                        return p;
                                    }
                                ).Where(
                                    p => speechPatternsRGX.IsMatch(p.Comment)
                                ).ToArray()
                            )
                        );
                    }
                );

                var threads = threadsTEMP.ToDictionary();

                var threadids = threads.Where(
                    t => !t.Value.Value.Any()
                ).Select(t => t.Key);

                foreach (var thread in threadids)
                {
                    int count = threads[thread].Key;
                    if (watchedPostNumbers.TryGetValue(thread, out int numPosts))
                        watchedPostNumbers[thread] = numPosts + count;
                    else
                        watchedPostNumbers.Add(thread, count);
                }

                threadids = threads.Keys.Except(threadids);
                File.AppendAllLines(
                    FILE_PATH,
                    threadids.Select(t => t.ToString())
                );

                foreach(var id in threadids)
                {
                    var thread = threads[id];
                    Console.WriteLine(id.ToString() + " - " + thread.Value.Length.ToString());
                    if (watchedPostNumbers.TryGetValue(id, out int count))
                    {
                        watchedPostNumbers.Remove(id);
                        loggedPostNumbers.Add(id, thread.Key + count);
                    }

                    ignoredPostNumbers.Add(id);
                    string path = MAIN_PATH + id.ToString();
                    Directory.CreateDirectory(path);

                    thread.Value.SaveThreadTo(path);
                }

                System.Threading.Thread.Sleep(15000);
                ++counter;
            }
            
        }
    }

    public static class FChanExtensions
    {
        private static RestClient client = new RestClient("https://i.4cdn.org/b/");
        public static void DownloadFileTo(this Post post, string location)
        {
            if (!String.IsNullOrWhiteSpace(post.OriginalFileName))
            {
                string name = post.OriginalFileName + post.FileExtension;
                if (!File.Exists(location + @"\" + name))
                    client.DownloadData(new RestRequest(post.FileName.ToString() + post.FileExtension, Method.GET)).SaveAs(location + @"\" + name);
            }
        }

        public static void SaveThreadTo(this IEnumerable<Post> thread, string location)
        {
            foreach (var post in thread)
            {
                post.DownloadFileTo(location);
            }

            File.AppendAllText(
                location + @"\Messages.txt",
                String.Join(
                    "\r\n\r\n~~~\r\n\r\n",
                    thread.Select(
                        p => p.PostNumber.ToString() + " -:- " + p.Date + "\r\n" +
                            (p.FileDeleted.HasValue && p.FileDeleted == 1 ?
                                "[FILE DELETED]\r\n" :
                                (
                                    p.HasImage ?
                                        "[" + p.OriginalFileName + p.FileExtension + ", " +
                                            (p.FileSize.HasValue ? p.FileSize.Value : 0).ToString() + ", " +
                                            p.ImageWidth + "x" + p.ImageHeight + "]\r\n" :
                                        ""
                                )
                            ) + p.Comment
                    )
                ) + "\r\n\r\n~~~\r\n\r\n"
            );
        }
    }
}

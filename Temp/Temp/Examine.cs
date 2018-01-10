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
    public static class Examine
    {
        public static Dictionary<string, ConcurrentBag<long>> times = new Dictionary<string, ConcurrentBag<long>>()
        {
            { "Request", new ConcurrentBag<long>() },
            { "Text Manipulation", new ConcurrentBag<long>() },
            { "Supplement Initialization", new ConcurrentBag<long>() },
            { "Citations", new ConcurrentBag<long>() },
            { "Headers Initialization", new ConcurrentBag<long>() },
            { "Effects", new ConcurrentBag<long>() },
            { "Links & Tooltips", new ConcurrentBag<long>() },
            { "Things to Know", new ConcurrentBag<long>() },
            { "Thoughts & Usage", new ConcurrentBag<long>() },
            { "Disposal", new ConcurrentBag<long>() }
        };

        public static ConcurrentDictionary<string, string> Tooltips = new ConcurrentDictionary<string, string>();
        public static ConcurrentDictionary<string, string> Effects = new ConcurrentDictionary<string, string>();

        public static void Execute()
        {
            CompareOptions opts = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;
            Stopwatch setupTimer = new Stopwatch();
            setupTimer.Start();

            var existingFiles = Directory.GetFiles(@"E:\Nootropics\Examine\Supplements");
            byte charByteModifier = ((byte)'a') - 1;
            string[] supplements = new CsvReader(
                new StreamReader(
                    @"E:\Nootropics\Examine Supplements.txt"
                )
            ).GetRecords<Regex101Match>().Select(s => s.content)/*.Where(
                s => !existingFiles.Any(f => String.Compare(s, 13, f, 0, 25, CultureInfo.CurrentCulture, opts) == 0)
            )*/.ToArray();

            RestClient client = new RestClient("https://examine.com");
            CookieContainer cookies = new CookieContainer();
            client.CookieContainer = cookies;
            client.Execute(new RestRequest("/supplements", Method.GET));

            setupTimer.Stop();
            Console.WriteLine("Setup: {0}ms", setupTimer.ElapsedMilliseconds);

            AppDomain.CurrentDomain.ProcessExit += SaveTimes;
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                while (true)
                {
                    SaveTimes();
                    Thread.Sleep(5000);
                }
            }));

            var result = Parallel.ForEach(supplements, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, supplementUrl => {
                //foreach(string supplementUrl in supplements) {

                Stopwatch timer = new Stopwatch();
                timer.Start();
                string text = client.Execute(
                    new RestRequest(
                        supplementUrl,
                        Method.GET
                    )
                ).Content;
                timer.Stop();
                times["Request"].Add(timer.ElapsedMilliseconds);

                /*int index = text.IndexOf("<article id=\"summary\">");
                ExamineSupplement supplement = new ExamineSupplement() { Name = PPRGX.EXAMINE_TITLE.Match(text).Value };
                text = text.Substring(index, text.IndexOf("<article id=\"scientific-research\">") - index);*/

                timer.Restart();
                text = text.Substring(text.IndexOf("<h1>"));
                int index = text.IndexOf("</span>History</a>");
                text = text.Remove(index, text.IndexOf("<!-- END: Supplements Sticky Menu -->", index) - index);
                index = text.IndexOf("myModalLabel2");
                text = text.Remove(index, text.IndexOf("replaceContent(data.action)", index) - index);
                index = text.IndexOf("<article id=\"scientific-research\">");
                if (index != -1)
                    text = text.Remove(index, text.IndexOf("<article id=\"citations\">", index) - index);
                timer.Stop();
                times["Text Manipulation"].Add(timer.ElapsedMilliseconds);

                timer.Restart();
                ExamineSupplement supplement = new ExamineSupplement();
                supplement.AKA = Enumerable.Empty<string>();
                supplement.BadWith = Enumerable.Empty<string>();
                supplement.GoodWith = Enumerable.Empty<string>();
                supplement.IncludedIn = Enumerable.Empty<string>();
                supplement.NegativeInteractions = Enumerable.Empty<string>();
                supplement.PositiveInteractions = Enumerable.Empty<string>();
                supplement.EditorThoughts = Enumerable.Empty<string>();
                supplement.Notes = Enumerable.Empty<string>();
                supplement.NotToBeConfusedWith = Enumerable.Empty<string>();
                supplement.Summary = Enumerable.Empty<string>();
                supplement.Tags = Enumerable.Empty<string>();
                supplement.Usage = Enumerable.Empty<string>();
                supplement.Warnings = Enumerable.Empty<string>();

                supplement.Tooltip = "";
                supplement.Name = "";
                supplement.Editor = "";
                supplement.Links = new Dictionary<string, string>();
                supplement.FAQ = new Dictionary<string, string>();
                supplement.Effects = new Dictionary<string, Effect>();
                supplement.Citations = Enumerable.Empty<Citation>();
                timer.Stop();
                times["Supplement Initialization"].Add(timer.ElapsedMilliseconds);

                timer.Restart();
                index = text.IndexOf("<h3>References</h3>");
                if (index >= 0)
                {
                    supplement.Citations = PPRGX.EXAMINE_CITATIONS.ToObjects<Citation>(text.Substring(index));
                    text = text.Substring(0, index);
                    timer.Stop();
                    times["Citations"].Add(timer.ElapsedMilliseconds);
                }

                timer.Restart();
                //Console.WriteLine("{0} citations found.", supplement.Citations.Count());
                supplement.Editor = PPRGX.EXAMINE_EDITOR.Match(text).Value;
                //Console.WriteLine("Editor: " + supplement.Editor);
                var headers = (MultiRGX.Headers)MultiRGX.SplitByHeader(text);
                supplement.Name = headers.Single().Key;
                //Console.WriteLine("Name: " + supplement.Name);
                headers = (MultiRGX.Headers)headers[supplement.Name, false];
                timer.Stop();
                times["Headers Initialization"].Add(timer.ElapsedMilliseconds);

                timer.Restart();
                index = text.IndexOf("<th>Notes</th>");
                if (index >= 0)
                {
                    var effects = PPRGX.EXAMINE_EFFECTS.ToDictionaries(text.Substring(index));
                    supplement.Effects = effects.ToDictionary(
                        e => e["Effect"],
                        e =>
                        {
                            Effects.TryAdd(e["Effect"], e["URLName"]);

                            Effect effect = new Effect()
                            {
                                Comments = e.TryGetValue("Comments", out string comments) ? comments : "",
                                Name = e["Effect"]
                            };

                            if (e["Magnitude"] != "-")
                            {
                                var magnitude = e["Magnitude"].Split('-');
                                effect.Magnitude = (sbyte)(Convert.ToInt32(magnitude[1]) * (magnitude[0] == "up" ? 1 : -1));
                            }
                            else
                                effect.Magnitude = 0;
                            effect.LevelOfEvidence = (byte)((byte)e["LevelOfEvidence"][0] - charByteModifier);

                            return effect;
                        }
                    );

                    effects.ToList().ForEach(e => Effects.TryAdd(e["Effect"], e["URLName"]));
                    effects = null;

                    timer.Stop();
                    times["Effects"].Add(timer.ElapsedMilliseconds);
                }
                text = null;
                //Console.WriteLine("{0} effects found.", supplement.Effects.Count);

                timer.Restart();
                supplement.Tooltip = headers[supplement.Name].Paragraphs.First();
                Tooltips.TryAdd(supplement.Name, supplement.Tooltip);
                headers.Tooltips.ToList().ForEach(
                    t => Tooltips.TryAdd(t.Key, t.Value)
                );
                supplement.Links = headers.Links.Where(l => l.Key != "").Distinct(
                    new KeyValuePairComparer<string, string>()
                ).ToDictionary(
                    kv => kv.Key.Trim(),
                    kv => kv.Value.Trim()
                );
                //Console.WriteLine("{0} links found.", supplement.Links.Count);
                timer.Stop();
                times["Links & Tooltips"].Add(timer.ElapsedMilliseconds);

                timer.Restart();
                IHeader temp = null;
                IHeader temp2 = null;
                if (((
                        headers.TryGetValue("Summary", out temp) &&
                        temp != null &&
                        ((MultiRGX.Headers)temp).TryGetValue("All Essential Benefits/Effects/Facts & Information", out temp)
                    ) || (
                        headers.TryGetValue("Goals", out temp) &&
                        temp != null &&
                        ((MultiRGX.Headers)temp).TryGetValue("", out temp)
                    )) && temp != null
                )
                    supplement.Summary = temp.HasSubheaders && ((MultiRGX.Headers)temp).TryGetValue("In Progress", out temp2) ?
                        (temp2 == null ?
                            Enumerable.Empty<string>() :
                            temp2.Paragraphs.Skip(1)
                        ) : temp.Paragraphs;


                if (headers.TryGetValue("Things to Know", out temp))
                {
                    var thingsToKnow = (MultiRGX.Headers)temp;

                    if (thingsToKnow.TryGetValue("Also Known As", out temp) && temp != null)
                        supplement.AKA = new string(
                            temp.Paragraphs.Single().TakeWhile(
                                c => c != ';'
                            ).ToArray()
                        ).Split(
                            new string[] { ", ", "," },
                            StringSplitOptions.RemoveEmptyEntries
                        );
                    //Console.WriteLine("AKA: " + String.Join(", ", supplement.AKA));

                    if (thingsToKnow.TryGetValue("Do Not Confuse With", out temp) && temp != null)
                        supplement.NotToBeConfusedWith = new string(
                            temp.Paragraphs.Single().TakeWhile(
                                c => c != ';'
                            ).ToArray()
                        ).Split(
                            new string[] { ", ", "," },
                            StringSplitOptions.RemoveEmptyEntries
                        );
                    //Console.WriteLine("NTBCW: " + String.Join(", ", supplement.NotToBeConfusedWith));

                    if (headers.TryGetValue("Things to Note", out temp) && temp != null)
                        supplement.Notes = temp.Paragraphs;
                    if (headers.TryGetValue("Is a Form Of", out temp) && temp != null)
                        supplement.Tags = temp.Tooltips.Keys;
                    //Console.WriteLine("Tags: " + String.Join(", ", supplement.Tags));

                    if (thingsToKnow.TryGetValue("Goes Well With", out temp) && temp != null)
                    {
                        supplement.GoodWith = temp.Tooltips.Keys;
                        supplement.PositiveInteractions = temp.Paragraphs;
                    }
                    //Console.WriteLine("Good With: " + String.Join(", ", supplement.GoodWith));
                    if (thingsToKnow.TryGetValue("Does Not Go Well With", out temp) && temp != null)
                    {
                        supplement.BadWith = temp.Tooltips.Keys;
                        supplement.NegativeInteractions = temp.Paragraphs;
                    }
                    //Console.WriteLine("Bad With: " + String.Join(", ", supplement.BadWith));

                    if (thingsToKnow.TryGetValue("Caution Notice", out temp) && temp != null)
                        supplement.Warnings = temp.Paragraphs;
                    //Console.WriteLine("{0} warnings found.", supplement.Warnings.Count());
                    thingsToKnow.Dispose();

                    timer.Stop();
                    times["Things to Know"].Add(timer.ElapsedMilliseconds);
                }

                timer.Restart();
                if (headers.TryGetValue("How to Take", out temp) && temp != null && ((MultiRGX.Headers)temp).TryGetValue("Recommended dosage, active amounts, other details", out temp) && temp != null)
                    supplement.Usage = temp.Paragraphs;

                if (headers.Any(h => h.Key.Contains("Thoughts") || h.Key.Contains("Editor")))
                    supplement.EditorThoughts = headers.First(
                        h => h.Key.Contains("Thoughts") ||
                        h.Key.Contains("Editor")
                    ).Value.Paragraphs;
                //Console.WriteLine("{0} thoughts found.", supplement.EditorThoughts.Count());

                if (headers.TryGetValue("Frequently Asked Questions", out temp) && temp != null)
                    supplement.FAQ = ((MultiRGX.Headers)temp).Paragraphs.Select(
                        p => PPRGX.EXAMINE_QnA.Match(p).ToKeyValue()
                    ).Distinct(
                        new KeyValuePairComparer<string, string>()
                    ).ToDictionary(
                        kv => kv.Key.Trim(),
                        kv => kv.Value.Trim()
                    );
                //Console.WriteLine("{0} FAQ answers found.", supplement.FAQ.Count);
                timer.Stop();
                times["Thoughts & Usage"].Add(timer.ElapsedMilliseconds);

                timer.Restart();
                supplement.SaveAs(@"E:\Nootropics\Examine\Supplements\" + supplement.Name + ".txt");
                headers.Dispose();
                if (temp != null)
                    temp.Dispose();
                temp = null;
                timer.Stop();
                times["Disposal"].Add(timer.ElapsedMilliseconds);
            });

            SpinWait.SpinUntil(() => result.IsCompleted);
            Effects.SaveDictAs(@"E:\Nootropics\Examine\Effects.txt");
            Tooltips.SaveDictAs(@"E:\Nootropics\Examine\Tooltips.txt");

            supplements = null;
            client = null;
            cookies = null;
            Effects.Clear();
            Effects = null;
            Tooltips.Clear();
            Tooltips = null;
        }

        public static void SaveTimes(object sender = null, object other = null)
        {
            string str = String.Join(
                "\r\n",
                times.Where(t => t.Value.Count > 0).Select(
                    t => t.Key + ": \t" + t.Value.Average().ToString("#")
                )
            );
            Console.WriteLine(str + "\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~");
            File.WriteAllText(@"E:\Nootropics\Examine\Times.txt", str);
            Effects.SaveDictAs(@"E:\Nootropics\Examine\Effects.txt");
            Tooltips.SaveDictAs(@"E:\Nootropics\Examine\Tooltips.txt");
        }

        public struct Regex101Match
        {
            public int match { get; set; }
            public int group { get; set; }
            public string is_participating { get; set; }
            public int start { get; set; }
            public int end { get; set; }
            public string content { get; set; }
        }

        public struct ExamineSupplement
        {
            public string Name { get; set; }
            public string Tooltip { get; set; }
            public string Editor { get; set; }

            // Tooltips
            public IEnumerable<string> Tags { get; set; }
            public IEnumerable<string> GoodWith { get; set; }
            public IEnumerable<string> BadWith { get; set; }
            public IEnumerable<string> IncludedIn { get; set; }

            // Paragraphs
            public IEnumerable<string> Summary { get; set; }
            public IEnumerable<string> Notes { get; set; }
            public IEnumerable<string> PositiveInteractions { get; set; }
            public IEnumerable<string> NegativeInteractions { get; set; }
            public IEnumerable<string> Warnings { get; set; }
            public IEnumerable<string> Usage { get; set; }
            public IEnumerable<string> EditorThoughts { get; set; }

            // Formatted
            public IEnumerable<string> AKA { get; set; }
            public IEnumerable<string> NotToBeConfusedWith { get; set; }
            public Dictionary<string, string> FAQ { get; set; }
            public Dictionary<string, Effect> Effects { get; set; }
            public IEnumerable<Citation> Citations { get; set; }

            public Dictionary<string, string> Links { get; set; }
        }
    }

    public struct Effect
    {
        public byte LevelOfEvidence { get; set; }
        public string Name { get; set; }
        public sbyte Magnitude { get; set; }
        public string Comments { get; set; }
    }

    public struct Citation
    {
        public string Author { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
    }
}

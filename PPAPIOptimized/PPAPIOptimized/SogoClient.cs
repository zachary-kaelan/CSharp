using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using PPLib;
using ZachLib.Logging;

namespace PPAPIOptimized
{
    public class SogoClient : IDisposable
    {
        public const string SURVEYS_PATH = @"C:\DocUploads\Program Files\Surveys\";

        public static readonly Dictionary<string, Tuple<string[], string, string[]>> Surveys = new Dictionary<string, Tuple<string[], string, string[]>>()
        {
            {
                "QTPC",
                new Tuple<string[], string, string[]>(
                    new string[]
                    {
                        "SurveyNumber",
                        "ResponseNumber",
                        "LocationCode",
                        "LastName",
                        "FirstName",
                        "Phone",
                        "TechUsername",
                        "TechFirstName",
                        "TechFullName",
                        "ServiceCode",
                        "Branch",
                        "Date"
                    }, "{0}{1}\r\nDo you agree with the following statements about [[TechFullName]]?:\r\n{2}{3}{4}{5}{6}{7}{8}{9}{10}",
                    new string[]
                    {
                        "On a scale from 0-10, how likely are you to recommend Insight Pest Solutions to a friend or colleague?\r\n",
                        "\r\nWere you home for our visit on [[WorkDate]]?\r\n",
                        " - I believe the technician spent adequate time treating my home.\t\t\t\t\t\t\t",
                        " - I believe the technician's appearance was professional.\t\t\t\t\t\t\t\t",
                        " - I feel that the technician was friendly, knowledgeable, and took the time to explain the service.\t",
                        " - Overall I felt my technician effectively treated my home and answered my questions.\t\t\t",
                        " - The technician's notes in the emailed service report were informative and helpful.\t\t\t",
                        " - The technician left a 'door-hanger' at my home.\t\t\t\t\t\t\t\t\t",
                        "\r\nDo you have any comments, questions, or suggestions?\r\n",
                        "\r\nBefore you go, we noticed one or more of your answers indicate that we didn't provide you with the customer satisfaction we aim for. Would you like to be contacted by a customer service representative?\r\n",
                        "\r\nPlease provide your preferred phone or email for follow up:\r\n"
                    }
                )
            },
            {
                "Retention",
                new Tuple<string[], string, string[]>(
                    new string[]
                    {
                        "SurveyNumber",
                        "ResponseNumber",
                        "Email",
                        "LocationCode",
                        "FirstName",
                        "LastName",
                        "NoteCode",
                        "Date",
                        "RetentionRep",
                        "RepEmail",
                        "Branch"
                    }, "{0}\r\nDo you agree with the following statements about the last person you spoke with?:\r\n{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}",
                    new string[]
                    {
                        "What was the reason for your call today?\r\n",
                        " - My concerns were adequately listened to.\t\t\t",
                        " - The representative empathized with my situation.\t\t",
                        " - This representative helped me find a solution.\t\t\t",
                        " - Overall, this person provided excellent customer service.\t",
                        "\r\nDo you have any comments about the last person you spoke with?\r\n",
                        "\r\nWhat, if anything, could we have done to keep you as a customer?\r\n",
                        "\r\nHow long do you expect to stay with Insight?\r\n",
                        "\r\nWhy did you decide to keep your service with Insight?\r\n",
                        "\r\nWhy did you decide to start a new one or two year commitment with Insight?\r\n",
                        "\r\nWould you like a call from a manager?\r\n",
                        "\r\nIs there anything else you would like to tell us?\r\n"
                    }
                )
            },
            {
                "INPC",
                new Tuple<string[], string, string[]>(
                    new string[]
                    {
                        "SurveyNumber",
                        "ResponseNumber",
                        "Email",
                        "LocationCode",
                        "LastName",
                        "FirstName",
                        "Phone",
                        "TechName",
                        "Sales",
                        "Branch",
                        "Service",
                        "Source",
                        "Date",
                        "SubTotal"
                    }, "{0}{1}\r\nDo you agree with the following statements about the technician who treated your home?:\r\n{2}{3}{4}{5}{6}{7}\r\nDo you agree or disagree with the following statements?:\r\n{8}{9}{10}{11}{12}",
                    new string[]
                    {
                        "If you were to write a review about your overall experience with Insight Pest Solutions so far, what would it say?\r\n",
                        "\r\nBased on your experience so far, how likely is it that you would recommend Insight Pest Solutions to a friend or colleague?\r\n",
                        " - My technician was respectful and professional; I was greeted with a smile.",
                        " - I feel that my technician spent adequate time treating my home.",
                        " - Overall, I feel that my technician effectively treated my home and answered my questions.",
                        "\r\nDid your technician request that you give a high score on this survey?\r\n",
                        "\r\nYou are almost done. Would you like to answer 5 bonus questions? We will double your chance of winning the raffle!\r\n",
                        "\r\nHow did you hear about us?\r\n",
                        " - The one-year service agreement was clearly explained.",
                        " - The salesperson said we will never see a bug.",
                        " - I was told that certain bugs/pests were not included in a quarterly service plan.",
                        "\r\nWhat did you think of the video the technician showed you at the beginning of the service? What can we do to improve the video?\r\n",
                        "\r\nWhat was your biggest fear or concern about using Insight Pest Solutions?\r\n",
                        "\r\nIn your opinion, what could we do to improve your experience with Insight?\r\n",
                        "\r\nBefore you go, we noticed that one or more of your answers indicate that we didn't provide you with the customer satisfaction we aim for. Would you like to be contacted by a customer service representative?\r\n",
                        "\r\nPlease provide your preferred phone or email for follow up:\r\n"
                    }
                )
            }
        };

        private static readonly RGX.Utils.SecondaryFormatting RGX_FORMAT = new RGX.Utils.SecondaryFormatting();
        private static readonly CookieContainer cookies = new CookieContainer();
        private RestClient client = new RestClient("https://www.sogosurvey.com/") { CookieContainer = cookies, UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36" };
        private ConcurrentDictionary<string, KeyValuePair<string[], string[]>> responses { get; set; }
        private string dateString { get; set; }
        private static ConcurrentQueue<string> uploadedQueue = new ConcurrentQueue<string>();
        //private static StreamWriter uploaded = new StreamWriter(SURVEYS_PATH + "Uploaded.txt") { AutoFlush = true };
        //private static StreamWriter missingLocationCodes = new StreamWriter(SURVEYS_PATH + "MissingLocationCodes.txt") { AutoFlush = true };
        private LogManager Logger { get; set; }

        private string type { get; set; }
        private string[] fields { get; set; }
        private string format { get; set; }
        private string[] questions { get; set; }

        public SogoClient(LogManager logger)
        {
            this.Logger = logger;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            cookies.Add(new Cookie("sogoUName", "lIQ9P2/FWP7XQcm30dEaMQ==", "/", "www.sogosurvey.com"));
            cookies.Add(new Cookie("sogoPwd", "UToqUWZmAH+Tv7E5vqlB7A==", "/", "www.sogosurvey.com"));

            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            this.Dispose(false);
        }

        public void LoadSurvey(string path, string type = "QTPC")
        {
            this.type = type;
            string uploadedPath = SURVEYS_PATH + type + " - Uploaded.txt";
            string missingLocationsPath = SURVEYS_PATH + "MissingLocationCodes.txt";
            
            dateString = "Date of " + type + ":{0}\r\n";

            if (path.EndsWith("csv"))
            {
                var temp = Surveys[type];
                this.fields = temp.Item1;
                this.format = temp.Item2;
                this.questions = temp.Item3;
                temp = null;

                string text = File.ReadAllText(path);
                text = text.Substring(text.IndexOf("\"1\""));

                /*var matches = Regex.Matches(
                        text,
                        "^\"(?<" + String.Join(">[^\"]*)\",\"(?<", fields) + ">[^\"]*)\"," +
                            String.Join("", Enumerable.Range(0, questions.Length).Select(n => "\"([^\"]*)\",")),
                        RegexOptions.Multiline, TimeSpan.FromMilliseconds(10000)
                    ).Cast<Match>();
                var duplicates = matches.Select(m => m.Groups["LocationCode"].Value).ToArray();
                duplicates = duplicates.GroupBy(d => d).ToList().FindAll(g => g.Count() > 1).Select(g => g.Key).ToArray();*/
                var matches = Regex.Matches(
                        text,
                        "^\"(?<" + String.Join(">[^\"]*)\",\"(?<", fields) + ">[^\"]*)\"," +
                            String.Join("", Enumerable.Range(0, questions.Length).Select(n => "\"([^\"]*)\",")),
                        RegexOptions.Multiline, TimeSpan.FromMilliseconds(10000)
                    ).Cast<Match>();
                text = null;
                string[] missing = File.ReadAllLines(missingLocationsPath);
                string[] completed = File.ReadAllLines(uploadedPath);

                this.responses = new ConcurrentDictionary<string, KeyValuePair<string[], string[]>>(
                    matches.ToList().FindAll(
                        m => m.Groups.TryGetGroup("LocationCode", out string value) && 
                        !missing.Contains(value) && 
                        !completed.Contains(value + " -:- " + m.Groups["Date"].Value)
                    ).ToDictionary(
                        m => m.Groups["ResponseNumber"].Value,
                        m => new KeyValuePair<string[], string[]>(
                            m.Groups.Cast<Group>().ToList().FindAll(
                                g => !Char.IsDigit(g.Name[0])
                            ).Select(g => g.Value).ToArray(),
                            m.Groups.Cast<Group>().Skip(1).Take(questions.Length).Select(
                                g => g.Value
                            ).ToArray()
                        )
                    )
                );
                missing = null;
                completed = null;
                Form1.prgThread.RunWorkerAsync(this.responses.Count);
                Form1.prgCounter += (matches.Count() - this.responses.Count);
                matches = null;
                GC.Collect();
            }

            Logger.AddLogs(
                uploadedPath,
                missingLocationsPath
            );
        }

        public void UploadNotes()
        {
            string logName = type + " - Uploaded";
            Postman postman = new Postman(Logger);
            postman.OnExecute += Form1.Client_OnExecute;
            /*Thread logThread = new Thread(() => ManageLogQueue());
            logThread.Priority = ThreadPriority.BelowNormal;
            logThread.IsBackground = true;
            logThread.Start();*/
            string[] keys = responses.Keys.ToArray();
            string noteCode = type == "Retention" ? null : "FEEDBACK";
            bool nullCode = String.IsNullOrWhiteSpace(noteCode);

            var result = Parallel.ForEach(keys, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, key =>
            //foreach(string key in keys)
            {
                var temp = responses[key];
                Dictionary<string, string> info = fields.Zip(temp.Key, (k, v) => new { k, v }).ToDictionary(kv => kv.k, kv => kv.v);
                if (!Postman.locationCodes.TryGetValue(info["LocationCode"], out string id))
                {
                    //missingLocationCodes.TryWriteLineAsync(info["LocationCode"]);
                    Logger.Enqueue(
                        new LogUpdate(
                            "MissingLocationCodes",
                            info["LocationCode"]
                        )
                    );
                    this.responses.TryRemove(key, out _);
                }
                string[] answers = temp.Value;
                temp = new KeyValuePair<string[], string[]>();

                string note = String.Format(format, questions.Zip(answers, (q, r) => String.IsNullOrWhiteSpace(r) ? "" : q + r + "\r\n").ToArray());
                RGX_FORMAT.Matches(note).Cast<Match>().ToList().ForEach(
                    m => note = note.Replace(
                        m.Value, info[m.Groups[1].Value]
                    )
                );

                string tempCode = nullCode ? info["NoteCode"] : noteCode;
                if (
                    !postman.GetNotes(
                        id, tempCode, 
                        DateTime.Parse(info["Date"])
                    ).Any(/*n => n.CreatedByUser == "ADMN" || !n.Note.StartsWith("Work Date:")*/)
                )
                    postman.UploadNote(
                        id, note, 
                        String.Format(
                            dateString, 
                            info["Date"]) + 
                                (info.TryGetValue("Source", out string source) ? 
                                    "Source: " + source : "") + 
                                "\r\n", 
                        tempCode
                    );
                this.responses.TryRemove(key, out _);
                note = null;
                id = null;

                Logger.Enqueue(
                    new LogUpdate(
                        logName,
                        info["LocationCode"] + " -:- " + info["Date"]
                    )
                );
                //uploadedQueue.Enqueue(info["LocationCode"] + " -:- " + info["WorkDate"]);
                
                ++Form1.prgCounter;
            });

            SpinWait.SpinUntil(() => result.IsCompleted && uploadedQueue.IsEmpty);
            /*uploadedQueue = null;
            logThread.Join(500);
            logThread.Abort();
            Thread.Sleep(500);
            logThread = null;*/
            postman = null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                this.responses.SaveAs(SURVEYS_PATH + type + ".txt");
                this.responses = null;

                if (disposing)
                {
                    this.fields = null;
                    this.format = null;
                    this.questions = null;
                    this.type = null;
                    this.client = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                /*uploaded.Flush();
                uploaded.Dispose();
                uploaded.Close();
                uploaded = null;*/

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SogoClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

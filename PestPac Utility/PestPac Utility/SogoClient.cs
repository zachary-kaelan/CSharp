using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Jil;
using PPLib;
using PestPac_Utility.Properties;

namespace PestPac_Utility
{
    public class SogoClient : IDisposable
    {
        private static string SURVEYS_PATH = null;
        private static string MISSING_PATH = null;
        private static string UPLOADED_PATH = null;

        private static string[] Missing = null;
        private static string[] Uploaded = null;

        private static Dictionary<string, SurveyType> Types = null;
        private static List<SurveyTypeHelper> TypeHelpers = null;
        private static readonly RGX.Utils.SecondaryFormatting RGX_FORMAT = new RGX.Utils.SecondaryFormatting();
        private ConcurrentDictionary<string, KeyValuePair<string[], string[]>> responses { get; set; }
        private string dateString { get; set; }
        private static LogManager Logger { get; set; }

        private SurveyType Type { get; set; }

        public SogoClient()
        {
            if (SurveyRegexes == null)
            {
                Types = JSON.Deserialize<SurveyTypeHelper[]>(
                    File.ReadAllText(SURVEYS_PATH + "SurveyRegexes.txt")
                ).Select(h => new SurveyType(h)).ToList();
            }
            
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        public static void SetPath(string path, LogManager logger = null)
        {
            SURVEYS_PATH = path;
            MISSING_PATH = SURVEYS_PATH + "MissingLocationCodes.txt";
            UPLOADED_PATH = SURVEYS_PATH + "{0} - Uploaded.txt";

            Missing = File.Exists(MISSING_PATH) ? File.ReadAllLines(MISSING_PATH) : Array.Empty<string>();
            Uploaded = File.Exists(UPLOADED_PATH) ? File.ReadAllLines(UPLOADED_PATH) : Array.Empty<string>();

            if (logger != null)
            {
                logger.AddLogs(MISSING_PATH, UPLOADED_PATH);
                Logger = logger;
            }
                
        }

        public static string CreateFormat(string path, string type = null)
        {
            string text = File.ReadAllText(path);
            text = text.Substring(0, text.IndexOf(",,\" \""));
            var tempQuestions = PPRGX.SOGO_QUESTIONS.ToObjects<SurveyQuestion>(text);
            text = null;
            int firstQuestion = Convert.ToInt32(tempQuestions.First(q => q.Question.Contains("?")).QuestionNumber);
            string[] infoFields = new string[] { "Sr.No.", "Response No" }.Concat(tempQuestions.Take(firstQuestion - 1).Select(q => q.Question)).ToArray();
            tempQuestions = tempQuestions.Skip(firstQuestion - 1);
            int totalQuestions = tempQuestions.Count();

            var variableQuestions = tempQuestions.ToList().FindAll(
                q => !String.IsNullOrWhiteSpace(q.Variable)
            ).Select(
                q => new KeyValuePair<string, string>(
                    PPRGX.FORMATTING.Replace(q.Variable, "$1"),
                    (q.Question + q.Subquestion).ToLower()
                )
            ).ToList();
            Dictionary<string, string> variables = variableQuestions.Select(
                q => q.Key
            ).Distinct().ToDictionary(
                v => "[[" + v + "]]",
                v => "[[" + (variableQuestions.FindAll(
                    q => q.Key == v
                ).Any(q => q.Value.Contains("visit")) ?
                    infoFields.Single(f => f.Contains("Date")) :
                    infoFields.Single(f => f.Contains("Tech") && f.Contains("Full"))) + "]]"
            );
            variableQuestions = null;

            var questions = tempQuestions.GroupBy(
                q => new Tuple<string, string, string>(
                    (Convert.ToInt32(q.QuestionNumber) - firstQuestion).ToString(),
                    PPRGX.SOGO_VARIABLE_SPACES.Replace(q.Question, " $1").Trim(),
                    q.Variable
                ), q => new KeyValuePair<char, string>(
                    String.IsNullOrEmpty(q.SubquestionLetter) ? ' ' : q.SubquestionLetter[0],
                    q.Subquestion
                ), (q, g) => {
                    if (!String.IsNullOrWhiteSpace(q.Item3))
                    {
                        string variable = variables[q.Item3];
                        string question = q.Item2.Replace(q.Item3, variable);

                        return g.Count() == 1 ?
                            new SurveyQuestion(
                                q.Item1,
                                question,
                                variable
                            ) : new SurveyQuestion(
                                q.Item1,
                                question,
                                variable,
                                g.OrderBy(sq => sq.Key).Select(
                                    sq => " - " + sq.Value.Replace(q.Item3, variable)
                                ).ToArray()
                            );
                    }

                    return g.Count() == 1 ?
                        new SurveyQuestion(
                            q.Item1,
                            q.Item2,
                            ""
                        ) : new SurveyQuestion(
                            q.Item1,
                            q.Item2,
                            "",
                            g.OrderBy(sq => sq.Key).Select(
                                sq => " - " + sq.Value
                            ).ToArray()
                        );
                }
            ).OrderBy(q => Convert.ToInt32(q.QuestionNumber)).ToArray();
            tempQuestions = null;

            StringBuilder sb = new StringBuilder("");
            List<string> formattedQuestions = new List<string>();

            int questionNumber = 0;
            string previousHeader = "";
            foreach (SurveyQuestion question in questions)
            {
                int count = question.Count;
                string line = "\r\n" + question.Question + "\r\n";

                if (count == 0)
                {
                    previousHeader = "";
                    sb.Append("{" + questionNumber++.ToString() + "}");
                    formattedQuestions.Add(line);
                }
                else
                {
                    sb.Append(previousHeader == question.Question ? "\r\n" : line);
                    sb.Append("{" + String.Join("}{", Enumerable.Range(questionNumber, count)) + "}");
                    formattedQuestions.AddRange(question);
                    questionNumber += count;
                    previousHeader = question.Question;
                }
            }
            questions = null;
            type = String.IsNullOrWhiteSpace(type) ? Path.GetFileNameWithoutExtension(path) : type;
            
            sb.Clear();
            sb = null;
            infoFields = null;
            formattedQuestions = null;

            Types.Add(
                type,
                new SurveyType(
                    new SurveyTypeHelper(
                        type,
                        infoFields.ToArray(),
                        sb.ToString(),
                        formattedQuestions.ToArray()
                    ), true
                )
            );
            return type;
        }

        #region LoadSurvey
        public void LoadSurvey(string path, string type)
        {
            dateString = "Date of " + type + ":{0}\r\n";
            if (path.EndsWith("csv"))
            {
                if (!Types.TryGetValue(type, out SurveyType Type))
                {
                    CreateFormat(path, type);
                    Type = Types[type];
                }

                string text = File.ReadAllText(path);
                var matches = Type.Regex.ToDictionaries(text.Substring(text.IndexOf("\"1\"")));
                text = null;

                this.responses = new ConcurrentDictionary<string, KeyValuePair<string[], string[]>>(
                    matches.Where(
                        m => m.TryGetValue("LocationCode", out string grpValue) &&
                        !Missing.Contains(grpValue) &&
                        !Uploaded.Contains(grpValue + " -:- " + m["Date"])
                    ).ToDictionary(
                        m => m["ResponseNumber"],
                        m => new KeyValuePair<string[], string[]>(
                            m.Where(
                                g => !Char.IsDigit(g.Key[0])
                            ).Select(g => g.Value).ToArray(),
                            m.Skip(1).Take(Type.Questions.Length).Select(
                                g => g.Value
                            ).ToArray()
                        )
                    )
                );

                Form1.prgThread.RunWorkerAsync(this.responses.Count);
                Form1.prgCounter += (matches.Count() - this.responses.Count);
                matches = null;
                GC.Collect();
            }
        }

        public void LoadSurvey(string path)
        {
            LoadSurvey(path, CreateFormat(path));
        }
        #endregion

        public void UploadNotes()
        {
            string logName = type + " - Uploaded";
            Postman postman = new Postman(Logger);
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

            SpinWait.SpinUntil(() => result.IsCompleted);
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
                this.responses.SaveAs(SURVEYS_PATH + Type.Name + ".txt");
                this.responses = null;

                if (disposing)
                {
                    Type = new SurveyType();
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

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Types.Clear();
            Types = null;
            TypeHelpers.Clear();
            TypeHelpers = null;

            this.Dispose(false);
        }
        #endregion

        public struct SurveyType
        {
            private static Dictionary<string, Type> regexes = Assembly.LoadFrom("SurveyRegexes").GetExportedTypes()
                .ToDictionary(
                    t => t.Name,
                    t => t
                );

            public string Name { get; set; }
            public string[] InfoFields { get; set; }
            public string Format { get; set; }
            public string[] Questions { get; set; }
            public Regex Regex { get; set; }

            public SurveyType(SurveyTypeHelper surveyType, bool makeRegex = false)
            {
                Name = surveyType.Name;
                InfoFields = surveyType.InfoFields;
                Format = surveyType.Format;
                Questions = surveyType.Questions;

                if (makeRegex)
                {
                    int existingTypeIndex = TypeHelpers.FindIndex(r => r.Name == surveyType.Name);
                    if (existingTypeIndex != -1)
                        TypeHelpers.RemoveAt(existingTypeIndex);
                    TypeHelpers.Add(surveyType);
                    TypeHelpers.SaveAs(Form1.MAIN_PATH + "SurveyTypes.txt");

                    System.Text.RegularExpressions.Regex.CompileToAssembly(
                        TypeHelpers.Select(h => h.CompileInfo).ToArray(),
                        new AssemblyName(Settings.Default.MainPath + "SurveyRegexes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
                    );

                    regexes = Assembly.LoadFrom("SurveyRegexes").GetExportedTypes()
                    .ToDictionary(
                        t => t.Name,
                        t => t
                    );
                }

                Regex = (Regex)regexes[Name].GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
        }
    }

    public struct SurveyQuestion : IList<string>
    {
        public string this[int index] { get => Subquestions[index]; set => Subquestions[index] = value; }

        public string QuestionNumber { get; set; }
        public string SubquestionLetter { get; set; }
        public string Question { get; set; }
        public string Variable { get; set; }
        public string Subquestion { get; set; }

        public int Count => Subquestions.Count;

        public bool IsReadOnly => ((IList<string>)Subquestions).IsReadOnly;

        private List<string> Subquestions { get; set; }

        public SurveyQuestion(string number, string question, string variable = null)
        {
            QuestionNumber = number;
            Question = question;
            Subquestion = null;
            Variable = variable;
            SubquestionLetter = null;
            Subquestions = new List<string>();
        }

        public SurveyQuestion(string number, string header, string variable, params string[] subquestions)
        {
            QuestionNumber = number;
            Question = header;
            Variable = variable;
            Subquestion = null;
            SubquestionLetter = null;
            Subquestions = subquestions.ToList();
        }

        public void Add(string item)
        {
            Subquestions.Add(item);
        }

        public void Clear()
        {
            Subquestions.Clear();
        }

        public bool Contains(string item)
        {
            return Subquestions.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Subquestions.CopyTo(array, arrayIndex);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Subquestions.GetEnumerator();
        }

        public int IndexOf(string item)
        {
            return Subquestions.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            Subquestions.Insert(index, item);
        }

        public bool Remove(string item)
        {
            return Subquestions.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Subquestions.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Subquestions.GetEnumerator();
        }
    }

    public struct SurveyTypeHelper
    {
        public string Name { get; set; }
        public string[] InfoFields { get; set; }
        public string Format { get; set; }
        public string[] Questions { get; set; }
        public RegexCompilationInfo CompileInfo { get; set; }

        public SurveyTypeHelper(string name, string[] fields, string format, string[] questions)
        {
            Name = name;
            InfoFields = fields;
            Format = format;
            Questions = questions;

            CompileInfo = new RegexCompilationInfo(
                "^\"(?<" + String.Join(">[^\"]*)\",\"(?<", InfoFields) + ">[^\"]*)\"," +
                    String.Join("", Enumerable.Range(0, Questions.Length).Select(n => "\"([^\"]*)\",")),
                RegexOptions.Multiline,
                Name, "SurveyRGX", true,
                TimeSpan.FromSeconds(10)
            );
        }

        public RegexCompilationInfo CreateRegex()
        {
            return new RegexCompilationInfo(
                "^\"(?<" + String.Join(">[^\"]*)\",\"(?<", InfoFields) + ">[^\"]*)\"," +
                    String.Join("", Enumerable.Range(0, Questions.Length).Select(n => "\"([^\"]*)\",")),
                RegexOptions.Multiline,
                Name, "SurveyRGX", true, 
                TimeSpan.FromSeconds(10)
            );
        }
    }
}

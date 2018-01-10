using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

namespace PPAPIStreamLined
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public OpenFileDialog filog { get; set; }

        public const string DocPath = @"C:\DocUploads\";
        public const string ErrorPath = DocPath + @"Erroneous Docs\";
        public const string CodeErrorPath = DocPath + @"Code Errors\";
        public const string DupesPath = DocPath + @"Duplicates\";
        public const string LogsPath = DocPath + @"logs\";
        public const string UnsignedPath = DocPath + @"Unsigned\";
        public const string SourcePath = DocPath + @"Docs\";
        public const string GhostsPath = DocPath + @"Ghosts\";
        public const string FailedPath = DocPath + @"Failed\";
        public const string NotesPath = DocPath + @"Notes\";
        public const string FailedNotesPath = NotesPath + @"Failed\";
        public const string SuccessNotesPath = NotesPath + @"Success\";
        public const string SourceNotesPath = NotesPath + @"Docs\";
        public const string DupeNotesPath = NotesPath + @"Dupes\";

        public const string AbbrvsPath = LogsPath + "Abbreviations.txt";
        public const string DupeLog = LogsPath + "Duplicates.txt";
        public const string ErrorLog = LogsPath + "UploadErrors.txt";
        public const string CodeLog = LogsPath + "CodeErrors.txt";
        public const string GhostLog = LogsPath + "Ghosts.txt";
        public const string NotesLog = LogsPath + "Notes.txt";

        Dictionary<string, string> abbrvs = new Dictionary<string, string>();
        Dictionary<string, string> abbrvs2 = new Dictionary<string, string>();

        List<Dictionary<string, string>> emptyNotes = new List<Dictionary<string, string>>();

        //JsonSerializer _jsonWriter = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
        Dictionary<Dictionary<string, string>, List<Dictionary<string,string>>> errors = new Dictionary<Dictionary<string, string>, List<Dictionary<string,string>>>();
        Dictionary<string, Dictionary<string, string>> codeErrors = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<Dictionary<string, string>, List<Dictionary<string, string>>> ghosts = new Dictionary<Dictionary<string, string>, List<Dictionary<string, string>>>();
        List<Dictionary<string, string>> failed = new List<Dictionary<string, string>>();
        public JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore, Formatting = Formatting.Indented };

        private void UpdateStatus(string update, bool clear = false)
        {
            if (clear)
                lblProcess.Text = "";
            lblProcess.Text += update;
            lblProcess.Invalidate();
            lblProcess.Update();
            lblProcess.Refresh();
            Application.DoEvents();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string path in new string[]{ DocPath, ErrorPath, DupesPath, LogsPath, UnsignedPath, GhostsPath, FailedPath, SourcePath, NotesPath, SourceNotesPath, FailedNotesPath, SuccessNotesPath, DupeNotesPath})
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            List<string> abbrvsTemp = new List<string>(File.ReadAllLines(AbbrvsPath));
            foreach (string abbrv in abbrvsTemp)
            {
                try
                {
                    abbrvs.Add(abbrv.Split(',')[0], abbrv.Split(',')[1]);
                    abbrvs2.Add(abbrv.Split(',')[1], abbrv.Split(',')[0]);
                }
                catch
                {
                    if (abbrv.Split(',')[1].Length < abbrvs2[abbrv.Split(',')[1]].Length)
                        abbrvs2[abbrv.Split(',')[1]] = abbrv.Split(',')[0];
                    continue;
                }
            }
        }
        
        private void cboErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name = cboErrors.Text.Split(' ');
            KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>> q = errors.ToList().Find(entry => entry.Key["FirstName"] == name[0] && entry.Key["LastName"] == name[1]);
            txtCurLog.Text = "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tSearch Query\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += JsonConvert.SerializeObject(q.Key, settings) + "\r\n";

            txtCurLog.Text += "\r\n~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tSearch Results\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            foreach (Dictionary<string, string> res in q.Value)
            {
                txtCurLog.Text += "\r\n" + JsonConvert.SerializeObject(res, settings) + "\r\n";
            }
        }

        private void cboNameErrors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            List<string> pdfs = Directory.GetFiles(SourcePath, "*.pdf").Select(p => new FileInfo(p).Name).ToList();
            List<string> unsigned = pdfs.FindAll(p => !p.ToLower().Contains("signed") && !p.ToLower().Contains("service") && !p.ToLower().Contains("inpc") && !p.ToLower().Contains("sa-"));
            pdfs.RemoveAll(p => !p.ToLower().Contains("signed") && !p.ToLower().Contains("service") && !p.ToLower().Contains("inpc") && !p.ToLower().Contains("sa-"));
            foreach (string pdf in unsigned)
            {
                File.Move(SourcePath + pdf, UnsignedPath + pdf);
            }

            prgBar.Maximum = pdfs.Count;

            Postman client = new Postman();
            client.abbrvs = abbrvs;
            client.abbrvs2 = abbrvs2;
            foreach (string pdf in pdfs)
            {
                try
                {
                    lblProcess.Text = "Processing File: " + pdf;
                    lblProcess.Text += "\nExtracting PDF Information... ";
                    Dictionary<string, string> cust = await Task.Run<Dictionary<string,string>>(() => client.ExtractPDF(SourcePath + pdf));
                    lblProcess.Text += "Completed.\nGetting Location ID... ";
                    List<Dictionary<string, string>> results = await Task.Run<List<Dictionary<string,string>>>(() => client.GetLocID(cust));
                    if (results == null)
                    {
                        lblProcess.Text += "Failed.\n";
                        failed.Add(cust);
                        cboFailed.Items.Add(cust["FirstName"] + " " + cust["LastName"]);
                        File.Move(SourcePath + pdf, FailedPath + pdf);
                        continue;
                    }
                    
                    if (results.Count == 1)
                    {
                        lblProcess.Text += "Completed.\nUploading Document... ";
                        cust = results[0];
                        string docID = await Task.Run<string>(() => client.SetDocRecord(cust, pdf.Contains("INPC") ? "INPC" : "SA"));
                        if (docID != "Doc Exists")
                            await Task.Run(() => client.UploadDoc(docID, SourcePath + pdf));
                        lblProcess.Text += "Completed.";
                        File.Move(SourcePath + pdf, DocPath + pdf);
                    }
                    else if (results[0].TryGetValue("Missing", out _))
                    {
                        lblProcess.Text += "Failed.\n";
                        results[0].Remove("Missing");
                        ghosts.Add(cust, results);
                        File.Move(SourcePath + pdf, GhostsPath + pdf);
                        cboGhosts.Items.Add(cust["FirstName"] + " " + cust["LastName"]);
                    }
                    else
                    {
                        lblProcess.Text += "Failed.\n";
                        cust["SearchString"] = results[0]["SearchString"];
                        cust["File"] = ErrorPath + pdf;
                        results.RemoveAt(0);
                        errors.Add(cust, results);
                        cboErrors.Items.Add(cust["FirstName"] + " " + cust["LastName"]);
                        File.Move(SourcePath + pdf, ErrorPath + pdf);
                    }
                }
                catch (DuplicateWaitObjectException err2)
                {
                    File.Move(SourcePath + pdf, DupesPath + pdf);
                }
                catch (Exception err)
                {
                    lblProcess.Text += "Failed\n";
                    Dictionary<string, string> newErr = new Dictionary<string, string>();
                    foreach (var prop in err.GetType().GetProperties())
                    {
                        if (prop.GetValue(err, null) != null)
                            newErr.Add(prop.Name, prop.GetValue(err, null).ToString());
                    }
                    codeErrors.Add(pdf, newErr);
                    cboCodeErrors.Items.Add(pdf.Substring(0, pdf.Length - 4));
                }
                finally
                {
                    ++prgBar.Value;
                }
            }
        }

        private void cboCodeErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            var err = codeErrors.ToList().Find(p => p.Key == cboCodeErrors.Text);
            txtCurLog.Text = "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tProblem PDF\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~- \t" + err.Key + " \t-~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n\r\n";

            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tError Details\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n\r\n";

            txtCurLog.Text += JsonConvert.SerializeObject(err.Value);
        }

        private void cboGhosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] name = cboGhosts.Text.Split(' ');
            KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>> q = ghosts.ToList().Find(entry => entry.Key["FirstName"] == name[0] && entry.Key["LastName"] == name[1]);
            txtCurLog.Text = "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tSearch Query\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += JsonConvert.SerializeObject(q.Key, settings) + "\r\n";

            txtCurLog.Text += "\r\n~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\tSearch Results\t~~~---~~~\r\n";
            txtCurLog.Text += "~~~---~~~\t~~~---~~~\t~~~---~~~\r\n";
            foreach (Dictionary<string, string> res in q.Value)
            {
                txtCurLog.Text += "\r\n" + JsonConvert.SerializeObject(res, settings) + "\r\n";
            }
        }

        private void cboFailed_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurLog.Text = JsonConvert.SerializeObject(failed.Find(c => c["FirstName"] == cboFailed.Text.Split(' ')[0] && c["LastName"] == cboFailed.Text.Split(' ')[1]));
        }

        private void lblProcess_Click(object sender, EventArgs e)
        {

        }

        private async void btnNotes_Click(object sender, EventArgs e)
        {
            //Directory.Delete(SourceNotesPath, true);
            //Directory.CreateDirectory(SourceNotesPath);

            List<string> noteFiles = Directory.GetFiles(NotesPath, "*.csv").ToList().FindAll(f => !f.Contains("UPLOADED"));
            List<string> lines = new List<string>();
            int sbCap = 0;
            foreach (string noteFile in noteFiles)
            {
                sbCap += Convert.ToInt32(new FileInfo(@noteFile).Length);
                List<string> linesTemp = File.ReadAllLines(@noteFile).ToList();
                if (linesTemp[0].ToLower().Contains("note"))
                    linesTemp.RemoveAt(0);
                lines.AddRange(linesTemp);
            }

            prgBar.Value = 0;
            prgBar.Maximum = lines.Count;
            UpdateStatus("Processing Notes Files...", true);

            string nullCheck(List<string> list, int index)
            {
                try
                {
                    if (list[index] == null || list[index] == " ")
                        return "";
                    else
                        return list[index].Trim();
                }
                catch
                {
                    return "";
                }
            }

            List<Dictionary<string, string>> notes = new List<Dictionary<string, string>>();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                Dictionary<string, string> dict = new Dictionary<string, string>();
                List<string> info = line.Split(',').ToList();
                if (info[0] == "11422284")
                {
                    int n = Convert.ToInt32(info[0]);
                }
                if (line == "")
                    continue;
                info.RemoveAt(0);

                if (info[0].Trim()[0] == '"')
                {
                    info[0] += " &" + info[1];
                    info.RemoveAt(1);
                    info[0] = info[0].Trim().Substring(1, info[0].Length - 2);
                }

                if (info[1].Trim()[0] == '"')
                {
                    info[1] += "." + info[2];
                    info.RemoveAt(2);
                }

                List<string> locInfo = info[1].Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                dict.Add("Address", locInfo[0]);
                locInfo.AddRange(nullCheck(locInfo, 1).Split(' '));
                if (locInfo.Count > 4)
                {
                    dict.Add("City", String.Join(" ", locInfo.GetRange(2, locInfo.Count - 3)));
                    dict.Add("Zip", locInfo[locInfo.Count-1]);
                }
                else
                {
                    dict.Add("City", nullCheck(locInfo, 2));
                    dict.Add("Zip", nullCheck(locInfo, 3));
                }

                dict.Add("State", "");
                try
                {
                    dict.Add("Phone", info[2].Insert(3, "-").Insert(7, "-"));
                }
                catch
                {
                    dict.Add("Phone", "");
                }

                if (info.Count > 4)
                {
                    info[3] = String.Join(",", info.GetRange(3, info.Count - 3));
                    info.RemoveRange(4, info.Count - 4);
                }
                
                List<string> temp = new List<string>(info[0].Replace("/", " & ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                if (temp.Count == 2)
                {
                    dict.Add("FirstName", temp[0]);
                    dict.Add("LastName", temp[1]);
                }
                else if (temp.Count == 1)
                {
                    dict.Add("LastName", temp[0]);
                    dict.Add("FirstName", "");
                }
                else
                {
                    string[] ands = new string[] { "AND", "&" };
                    int and = temp.FindIndex(n => ands.Contains(n));
                    if (and != -1)
                    {
                        dict.Add("FirstName", nullCheck(temp, and - 1));
                        dict.Add("SpouseName", nullCheck(temp, and + 1));
                        dict.Add("LastName", nullCheck(temp, temp.Count - 1));
                    }
                    else
                    {
                        dict.Add("FirstName", nullCheck(temp, 0));
                        dict.Add("LastName", nullCheck(temp, temp.Count - 1));
                    }
                }
                if (!dict.TryGetValue("SpouseName", out _))
                    dict.Add("SpouseName", "");

                if (info[3].Trim() != "")
                {
                    if (info[3].Trim()[0] == '"')
                    {
                        if (info[3].Substring(0, Math.Min(5, info[3].Length / 2)).Count(c => c == '"') == info[3].Substring(info[3].Length - Math.Min(5, info[3].Length / 2)).Count(c => c == '"'))
                            dict.Add("Note", info[3].Substring(1, info[3].Length - 2));
                        else
                        {
                            dict.Add("Note", info[3]);
                            do
                            {
                                ++i;
                                dict["Note"] += "\r\n" + lines[i];
                                lines[i] = lines[i].Trim();
                            }
                            while (!(lines[i+1].Split(',').Length >= 5 && Int32.TryParse(lines[i+1].Split(',')[0], out _))/*lines[i] == "" || ((lines[i].Substring(lines[i].Length-Math.Min(5, lines[i].Length / 2)).Count(c => c == '"') - lines[i].Substring(0, Math.Min(5, lines[i].Length / 2)).Count(c => c == '"')) != 1 && info[3].Substring(0, Math.Min(5, info[3].Length / 2)).Count(c => c == '"') != lines[i].Substring(lines[i].Length-Math.Min(5, lines[i].Length / 2)).Count(c => c == '"'))*/);
                            //!(lines[i][lines[i].Length - 1] == '"' && lines[i][lines[i].Length - 2] != '"'));
                            //dict["Note"] += "\r\n" + lines[i];
                        }
                    }
                    else
                        dict.Add("Note", info[3]);

                    notes.Add(dict);
                }
                else
                    emptyNotes.Add(dict);

                ++prgBar.Value;
            }
            
            lines = null;

            //var newNotes = notes.GroupBy(note => new Dictionary<string,string>() { { "Phone", note["Phone"] }, { "FirstName", note["FirstName"] }, { "LastName", note["LastName"] }, { "SpouseName", note["SpouseName"] }, { "Address", note["Address"] }, { "Zip", note["Zip"] }, { "City", note["City"] } }, note => note["Note"]).OrderBy(note => note.Key["LastName"]).ThenBy(note => note.Key["FirstName"]);
            
            Dispose();

            Regex rgx = new Regex("[^a-zA-Z0-9 ,-]");

            prgBar.Value = 0;
            prgBar.Maximum = notes.Count;
            UpdateStatus(" \tCompleted\nExtracting Notes...");
            for (int i = 0; i < notes.Count; ++i)
            {
                ExtractNote(notes[i]);
                ++prgBar.Value;
            }

            foreach (string noteFile in noteFiles)
            {
                File.Move(noteFile, noteFile.Insert(noteFile.Length - 4, " - UPLOADED"));
            }

            prgBar.Value = 0;
            UpdateStatus(" \tCompleted\nUploading Notes...");

            Postman client = new Postman();
            client.abbrvs = abbrvs;
            client.abbrvs2 = abbrvs2;
            
            string dictCheck(Dictionary<string, string> list, string index)
            {
                try
                {
                    if (list[index] == null || list[index] == " ")
                        return "";
                    else
                        return list[index].Trim();
                }
                catch
                {
                    return "";
                }
            }

            void NoteToFile(string notePath, string status = "failed")
            {
                string text = new FileInfo(notePath).Name;
                text = text.Substring(0, text.Length - 4);
                int lastSpace = text.LastIndexOf(' ');
                if (lastSpace < text.Length - 1 && text[lastSpace + 1] == '-')
                    text = text.Remove(lastSpace);
                text = text.Trim() + ".txt";
                string fileName = null;
                if (status == "failed")
                    fileName = FailedNotesPath;
                else if (status == "success")
                    fileName = SuccessNotesPath;
                else if (status == "dupe")
                    fileName = DupeNotesPath;
                else
                    throw new System.Exception("Invalid argment.");
                
                fileName += text;
                if (File.Exists(fileName))
                {
                    File.AppendAllText(fileName, File.ReadAllText(notePath));
                    File.Delete(notePath);
                }
                else
                    File.Move(notePath, fileName);

                /*
                string fileName = String.Format("Note - {0}{1}{2} {3} - {4}, {5}, {6}",
                    failed ? FailedNotesPath : SuccessNotesPath,
                    note["FirstName"],
                    !String.IsNullOrEmpty(note["SpouseName"]) ? " and " : "",
                    note["SpouseName"],
                    note["LastName"],
                    note["Address"],
                    note["City"],
                    note["State"] + " " + note["Zip"]).ToUpper() + ".txt";

                string src = SourceNotesPath + new FileInfo(fileName).Name;
                if (File.Exists(fileName))
                {
                    if (!File.ReadAllText(fileName).Contains(note["Note"]))
                        File.AppendAllText(fileName, note["Note"] + "\n\n~~~~~~~~~~~~~~~~~~~~~~~~\n\n");
                }
                else
                    File.Move(src, fileName);
                    */
            }

            void ExtractNote(Dictionary<string, string> note)
            {
                string fileName = String.Format("Note - {0}{1}{2} {3} - {4}, {5}, {6} ",
                    note["FirstName"],
                    !String.IsNullOrEmpty(note["SpouseName"]) ? " and " : "",
                    note["SpouseName"],
                    note["LastName"],
                    note["Address"],
                    note["City"],
                    note["State"] + " " + note["Zip"]).ToUpper();

                fileName = rgx.Replace(fileName, String.Empty);
                fileName = SourceNotesPath + fileName;
                while (File.Exists(fileName + ".txt"))
                    fileName += "-";
                fileName += ".txt";
                File.WriteAllText(fileName, note["Note"] + "\r\n\r\n~~~~~~~~~~~~~~~~~~~~~~~~\r\n\r\n");
            }

            notes = null;
            Dispose();

            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

            string[] notesText = Directory.GetFiles(SourceNotesPath, "*.txt");
            prgBar.Maximum = notesText.Length;

            for (int i = 0; i < notesText.Length; ++i)
            {
                Dictionary<string, string> note = null;
                try
                {
                    note = client.ExtractPDF(notesText[i]);
                    List<Dictionary<string, string>> results = client.GetLocID(note);

                    if (results == null)
                    {
                        failed.Add(note);
                        cboFailed.Items.Add(note["FirstName"] + " " + note["LastName"]);
                        NoteToFile(notesText[i]);
                        continue;
                    }

                    if (results.Count == 1)
                    {
                        note["State"] = results[0]["State"];
                        string response = client.UploadNote(results[0]["LocationID"], "VTNotes : " + File.ReadAllText(notesText[i]).Replace("~~~~~~~~~~~~~~~~~~~~~~~~", String.Empty).Trim());
                        lblProcess.Text += "Completed.";
                        NoteToFile(notesText[i], "success");
                    }
                    else if (results[0].TryGetValue("Missing", out _))
                    {
                        results[0].Remove("Missing");
                        ghosts.Add(note, results);
                        NoteToFile(notesText[i]);
                        cboGhosts.Items.Add(note["FirstName"] + " " + note["LastName"]);
                    }
                    else
                    {
                        note["SearchString"] = results[0]["SearchString"];
                        results.RemoveAt(0);
                        errors.Add(note, results);
                        cboErrors.Items.Add(note["FirstName"] + " " + note["LastName"]);
                        NoteToFile(notesText[i]);
                    }
                }
                catch (DuplicateWaitObjectException err2)
                {
                    File.Move(notesText[i], DupeNotesPath + new FileInfo(notesText[i]).Name);
                }
                catch (Exception err)
                {
                    if (err.InnerException != null && err.InnerException.Message == "Duplicate objects in argument.")
                    {
                        NoteToFile(notesText[i], "dupe");
                    }
                    else
                    {
                        Dictionary<string, string> newErr = new Dictionary<string, string>();
                        foreach (var prop in err.GetType().GetProperties())
                        {
                            if (prop.GetValue(err, null) != null)
                                newErr.Add(prop.Name, prop.GetValue(err, null).ToString());
                        }
                        if (note != null)
                        {
                            codeErrors.Add(note["FirstName"] + " " + note["LastName"], newErr);
                            cboCodeErrors.Items.Add(note["FirstName"] + " " + note["LastName"]);
                        }
                    }
                }
                finally
                {
                    ++prgBar.Value;
                }
            }

            UpdateStatus("Completed.");

            /*
            int notesCount = newNotes.Count();
            for (int i = 0; i < notesCount; ++i)
            {
                var tempGroup = newNotes.ElementAt(i);
                notesDisplay.AppendFormat("{0} {1}\n", tempGroup.Key["FirstName"], tempGroup.Key["LastName"]);
                for (int j = 0; j < tempGroup.Count(); ++j)
                {
                    notesDisplay.Append("  ");
                    notesDisplay.AppendLine(tempGroup.ElementAt(j));
                }
            }
            */



            /*
            foreach (var group in newNotes)
            {
                notesDisplay += group.Key["FirstName"] + " " + group.Key["LastName"] + "\n";
                if (group.Count() <= 1)
                    notesDisplay += "  " + group.ElementAt(0) + "\n";
                foreach(string note in group)
                    notesDisplay += "  " + note + "\n";
            }
            */

            //File.WriteAllText(NotesLog, notesDisplay.ToString());

            /*
            this.filog = new System.Windows.Forms.OpenFileDialog();
            Postman client = new Postman();
            client.abbrvs = abbrvs;
            client.abbrvs2 = abbrvs2;

            //OpenFileDialog filog = new OpenFileDialog();
            this.filog.InitialDirectory = @"C:\DocUploads";
            this.filog.Multiselect = true;
            this.filog.Title = "Select a notes file.";
            this.filog.ShowDialog(this);
            */
        }

        private void filog_fileOk(object sender, CancelEventArgs e)
        {
            this.Activate();
            //string[] files = 
        }
    }
}

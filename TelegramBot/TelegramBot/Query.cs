using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputMessageContents;
using System.IO;
using System.Threading;
using TelegramBot.Properties;

namespace TelegramBot
{
    enum QueryType { Doc, Initialize}
    class Query
    {
        public string authToken { get; set; }
        public TelegramBotClient Bot { get; set; }
        public string botPath { get; set; }
        public QueryType type { get; set; }
        public bool privacy { get; set; }
        public string docType { get; set; }
        public Update update { get; set; }
        public List<Query> subQueries { get; set; }
        public List<Query> msgs { get; set; }
        public string branch { get; set; }
        public string fileName { get; set; }
        public string name { get; set; }

        public Query(Update update)
        {
            this.update = update;
            this.Bot = new TelegramBotClient("330375662:AAHlMQIhlz7tIkwy_3s5jPBz8OlPMy1ca-0");
            this.botPath = @"C:\IPSRouteBot\";
            List<string> p;
            if (update.Message.Text.Contains("upload"))
            {
                p = new List<string>(update.Message.Text.Split(' '));
                this.type = QueryType.Initialize;
                try
                {
                    this.privacy = Convert.ToBoolean(p.Find(priv => priv.ToLower() == "true" || priv.ToLower() == "false"));
                }
                catch
                {
                    this.privacy = false;
                }
                finally
                {
                    if (this.privacy)
                    {
                        int temp = p.IndexOf("true");
                        this.name = p[temp + 1] + " " + p[temp + 2];
                        System.IO.File.AppendAllText(botPath + "UpdateLog.txt", String.Format("{0} - {1} {2} ({3}) - Content:{4} {5}", update.Message.Date.ToString("g3"), update.Message.From.FirstName, update.Message.From.LastName));
                    }
                    else
                    {
                        System.IO.File.AppendAllText(botPath + "UpdateLog.txt", String.Format("{0} - {1} {2} ({3}) - Content:{4} {5}", update.Message.Date.ToString("g3"), update.Message.From.FirstName, update.Message.From.LastName, update.Message.From.Username, update.Message.Text, update.Message.Document.FileName));
                        this.name = this.update.Message.From.FirstName + " " + this.update.Message.From.LastName;
                    }
                }
                this.docType = p.Find(typ => typ.ToLower() == "route" || typ.ToLower() == "ncc");
                Task<bool> x;
                do
                {
                    x = this.GetBranch();
                    x.Wait();
                } while (!x.Result);

                do
                {
                    x = this.GetUpdates();
                    x.Wait();
                } while (!x.Result);
            }
            else if (update.Message.Document != null)
            {
                this.type = QueryType.Doc;
                Task<bool> x;
                do
                {
                    x = this.GetFile();
                    x.Wait();
                } while (!x.Result);
            }
            else
            {

            }
        }

        public Query()
        {
            Task<bool> x;
            do
            {
                this.GetUpdates(true).Wait();
            } while (!x.Result);
        }

        public void AddQuery (Query q)
        {

        }

        public async Task<bool> GetBranch()
        {
            Dictionary<string, string> b = new Dictionary<string, string>()
            {
                {"BOS", "Boston"},
                {"RHI", "Rhode Island"},
                {"NJR", "New Jersey"},
                {"CON", "Connecticut"},
                {"PHI", "Philadelphia"},
                {"CHI", "Chicago"},
                {"CLE", "Cleveland"},
                {"COL", "Columbus"},
                {"VIR", "Virginia"},
                {"VAB", "Virginia Beach"},
                {"FTL", "Fort Lauderdale"},
                {"KAN", "Kansas City"},
                {"IND", "Indianapolis"},
                {"MIN", "Minneapolis"},
                {"WIL", "Wilmington"}
            };

            KeyboardButton[][] buttons = new KeyboardButton[4][];
            var keys = b.Keys.ToArray();
            int count = 15;
            for (int i = 0; i < 4; i++)
            {
                buttons[i] = new KeyboardButton[Math.Min(4, count)];
                for (int j = 0; j < buttons[i].Length; j++)
                {
                    buttons[i][j] = keys[15 - count];
                    --count;
                }
            }
            var send = await this.Bot.SendTextMessageAsync(this.update.Message.Chat.Id, "What is your branch?", replyMarkup: new ReplyKeyboardMarkup(buttons, false, false));
            if (send != null)
            {
                Task<bool> x;
                do
                {
                    x = this.GetUpdates();
                    x.Wait();
                } while (!x.Result);
                send = await this.Bot.SendTextMessageAsync(this.update.Message.Chat.Id, "Upload your doc!", replyMarkup: new ReplyKeyboardHide());
                if (b.TryGetValue(this.msgs[0].update.Message.Text, out string branch))
                {
                    this.branch = branch;
                    return true;
                }
                else
                {
                    var err = this.msgs[0].update.Message;
                    string log = "\r\nGETBRANCH EXCEPTION - INVALID INPUT\r\n";
                    foreach (var prop in err.GetType().GetProperties())
                    {
                        log += String.Format("{0} = {1}\r\n", prop.Name, prop.GetValue(err, null));
                    }
                    System.IO.File.AppendAllText(this.botPath + "ErrorLog.txt", log);
                    return false;
                }
            }
            else
            {
                var err = this.update.Message.From;
                string log = "\r\nGETBRANCH EXCEPTION - MESSAGE FAILED TO SEND\r\n";
                foreach (var prop in err.GetType().GetProperties())
                {
                    log += String.Format("{0} = {1}\r\n", prop.Name, prop.GetValue(err, null));
                }
                System.IO.File.AppendAllText(this.botPath + "ErrorLog.txt", log);
                return false;
            }
        }

        public async void testApiAsync()
        {
            var me = await Bot.GetMeAsync();
            Console.WriteLine("Hello my name is " + me.FirstName);
        }

        public async Task<bool> GetFile()
        {
            try
            {
                var doc = this.update.Message.Document;
                string docName = this.botPath + String.Format("{0}, {1} - {2} Route Sheet.pdf", this.branch, this.name, this.update.Message.Date.ToString("MMM d, yyyy"));
                var file = await Bot.GetFileAsync(doc.FileId, System.IO.File.Create(docName));
                string log = "\r\nNEW DOCUMENT FROM " + this.name + " AT " + docName + "\r\n";
                foreach (var prop in doc.GetType().GetProperties())
                {
                    log += String.Format("{0} = {1}\r\n", prop.Name, prop.GetValue(doc, null));
                }
                System.IO.File.AppendAllText(botPath + "DocLog.txt", log);
                return true;
            }
            catch (Exception err)
            {
                if (err.GetType() != new NullReferenceException().GetType())
                {
                    string log = "\r\nNEW EXCEPTION OF TYPE " + err.ToString() + "\r\n";
                    Console.WriteLine(log + "Check error log.");
                    foreach (var prop in err.GetType().GetProperties())
                    {
                        log += String.Format("{0} = {1}\r\n", prop.Name, prop.GetValue(err, null));
                    }
                    System.IO.File.AppendAllText(botPath + "ErrorLog.txt", log);
                }
            }
            return false;
        }

        public async Task<bool> GetUpdates(bool main = false)
        {
            var temp = await Bot.GetUpdatesAsync();
            List<Update> updates = new List<Update>(temp);
            updates.RemoveRange(0, Settings.Default.UpdateCount);
            if (updates.Count == 0)
                return false;
            Settings.Default.UpdateCount += updates.Count;
            Settings.Default.Save();

            foreach (Update update in updates)
            {
                this.subQueries.Add(new Query(update));
            }

            if (main)
                return true;
            this.msgs = this.subQueries.FindAll(q => q.update.Message.Chat.Id == this.update.Message.Chat.Id);
            this.subQueries.RemoveAll(q => q.update.Message.Chat.Id == this.update.Message.Chat.Id);
            return true;
        }

    }
}

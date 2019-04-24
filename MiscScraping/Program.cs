using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RestSharp;
using RestSharp.Extensions;
using ZachLib;

namespace MiscScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            var linesPath = @"E:\Text To Speech\Voice Lines - Portal 2\";
            StringBuilder transcript = new StringBuilder();
            StringBuilder allLines = new StringBuilder();
            StringBuilder linesDict = new StringBuilder();
            string format = "  ( {0} \"{1}\" )\r\n";
            var tempClient = new RestClient("http://overwikifiles.com");
            RestClient gladosClient = new RestClient("http://combineoverwiki.net/wiki/");

            var text = gladosClient.Execute(new RestRequest("GLaDOS/Quotes/Portal_2_single-player", Method.GET)).Content;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var rows = doc.DocumentNode.SelectNodes("/html/body/div[3]/div[2]/div[4]/table[3]/tr");
            Dictionary<string, string> linesDict2 = new Dictionary<string, string>();
            foreach(var row in rows.Skip(1))
            {
                var children = row.SelectNodes("./td");
                string fileName = children[0].SelectSingleNode("./code").InnerText;
                Console.Write("Downloading {0}... ", fileName);
                string quote = children[1].InnerText.Trim();
                transcript.AppendFormat(format, fileName, quote);
                linesDict2.Add(fileName, quote);
                allLines.AppendLine(quote);

                string path = linesPath + fileName + ".ogg";
                tempClient.DownloadData(new RestRequest(children[2].SelectSingleNode("./div/img").GetAttributeValue("class", null), Method.GET)).SaveAs(path);
                Console.Write("Processing {0}... ", fileName);
                var process = Process.Start(
                    new ProcessStartInfo(@"ffmpeg", "-i \"" + path + "\" \"" + linesPath + fileName + ".wav" + "\"")
                    {
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                );
                process.PriorityClass = ProcessPriorityClass.AboveNormal;
                process.WaitForExit();
                process.Close();
                process = null;
                Console.WriteLine("Done");
                File.Delete(path);
            }

            File.WriteAllText(@"E:\Text To Speech\Transcript.txt", transcript.ToString());
            File.WriteAllText(@"E:\Text To Speech\All Quotes.txt", allLines.ToString());
            linesDict2.SaveDictAs(@"E:\Text To Speech\Lines Dictionary.txt");
            Console.WriteLine();
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            
        }
    }
}

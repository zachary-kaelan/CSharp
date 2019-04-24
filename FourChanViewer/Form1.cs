using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using FourChanLib;
using ZachLib;

namespace FourChanViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Board random = new Board("b");
            Regex speechPatternsRGX = new Regex(
                @"(?=.*<3{2,})|(?:(?=.*(?:\.{3,}|thx|luv|[a-z]{3,}))(?=.*(?:;-\)|<3)))",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
            );
            List<Thread> threads = new List<Thread>();
            List<int> loggedThreads = new List<int>();

            while(true)
            {
                threads.AddRange(
                    random.GetCatalog().Where(
                        t => !loggedThreads.Contains(t.PostID) && 
                        !threads.Any(t2 => t2.PostID == t.PostID) && (
                        Utils.COMPARE_INFO.IndexOf(
                            t.Comment,
                            "share",
                            Utils.IGNORE_CASE_AND_SYMBOLS
                        ) >= 0 || Utils.COMPARE_INFO.IndexOf(
                            t.Comment,
                            "shouldnt",
                            Utils.IGNORE_CASE_AND_SYMBOLS
                        ) >= 0)
                    )
                );

                var threadidstemp = threads.Where(
                    t => t.GetReplies().Any(
                        p => speechPatternsRGX.IsMatch(p.Comment)
                    )
                ).Select(t => t.PostID);
                loggedThreads.AddRange(threadidstemp);
                File.AppendAllLines(
                    @"C:\Program Files\Microsoft Games\Mahjong\en-US\resources\assets\Stalking\Random People\4chan\Programming\Queen Bee.txt",
                    threadidstemp.Select(i => i.ToString())
                );

                System.Threading.Thread.Sleep(15000);
            }
        }
    }
}

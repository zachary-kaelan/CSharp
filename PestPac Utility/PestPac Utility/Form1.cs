using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

using Jil;
using PPLib;

namespace PestPac_Utility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public const string LOG_FORMAT = "[{0}] [{1}]:\t{2}";
        public const string LOG_DATE_FORMAT = "HH:mm:ss";

        public static string MAIN_PATH = PestPac_Utility.Properties.Settings.Default.MainPath;
        private static string LOGS_PATH = null;
        private static string CONFIG_PATH = null;
        public static Dictionary<string, string> config = null;

        public static int prgCounter = 0;
        public static BackgroundWorker prgThread = new BackgroundWorker();

        public static Dispatcher UIThread = null;

        public static LogManager Logger = new LogManager(
            "[{0}] [{1}]:\t{2} ~ {3}",
            LOG_DATE_FORMAT
        );

        private void Form1_Load(object sender, EventArgs e)
        {
            SogoClient sogo = new SogoClient(Logger);
            SogoClient.Surveys["QTPC"].SaveAs(@"E:\Temp\QTPCFormat.txt");
            sogo.CreateFormat(@"C:\DocUploads\Program Files\Surveys\QTPC.csv").Value.SaveAs(@"E:\Temp\CreateFormatTest1.txt");
            sogo.CreateFormat(@"C:\DocUploads\Program Files\Surveys\SoGoSurvey_QTPC Survey 2.2_14.csv").Value.SaveAs(@"E:\Temp\CreateFormatTest2.txt");

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            UIThread = Dispatcher.CurrentDispatcher;

            prgThread.DoWork += prgThread_DoWork;
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater(prgThread, ref prgCounter, 250)));
            Logger.OnEntry += Logger_OnEntry;
        }

        private void UIUpdater(BackgroundWorker worker, ref int counter, int sleepTime)
        {
            while (true)
            {
                if (counter > 0 && !worker.IsBusy)
                {
                    lock (worker)
                    {
                        int temp = counter;
                        worker.RunWorkerAsync(-1 * temp);
                        counter -= temp;
                    }
                }

                Thread.Sleep(sleepTime);
            }
        }


        private bool Setup()
        {
            if (String.IsNullOrWhiteSpace(MAIN_PATH))
            {
                DialogResult result = MessageBox.Show(@"This is your first time using this program on this computer. Would you like to choose a location to store miscellaneous program files? (Default is C:\PestPac\))", "PestPac Utility", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                switch(result)
                {
                    case DialogResult.Yes:
                        FolderBrowserDialog dialog = new FolderBrowserDialog() { ShowNewFolderButton=true, RootFolder=Environment.SpecialFolder.DesktopDirectory };
                        result = dialog.ShowDialog();
                        switch(result)
                        {
                            case DialogResult.OK:
                                MAIN_PATH = dialog.SelectedPath + @"\";
                                dialog.Dispose();
                                break;

                            default:
                                dialog.Dispose();
                                return false;
                        }
                        break;

                    case DialogResult.No:
                        MAIN_PATH = @"C:\PestPac\";
                        break;

                    default:
                        return false;
                }

                PestPac_Utility.Properties.Settings.Default.MainPath = MAIN_PATH;
                PestPac_Utility.Properties.Settings.Default.Save();
            }
            
            CONFIG_PATH = MAIN_PATH + "Config.txt";
            LOGS_PATH = MAIN_PATH + @"Logs\";
            Postman.LOGS_PATH = LOGS_PATH + @"Postman\";
            SogoClient.SURVEYS_PATH = LOGS_PATH + @"Surveys\";
            LogManager.LOGGER_PATH = LOGS_PATH + @"Logger\";

            foreach (string str in new string[] {
                    MAIN_PATH,
                    LOGS_PATH,
                    Postman.LOGS_PATH,
                    SogoClient.SURVEYS_PATH,
                    LogManager.LOGGER_PATH
                })
            {
                if (!Directory.Exists(str))
                    Directory.CreateDirectory(str);
            }

            config = File.Exists(CONFIG_PATH) ?
                JSON.Deserialize<Dictionary<string, string>>(File.ReadAllText(MAIN_PATH + "Config.txt")) :
                new Dictionary<string, string>();

            return true;
        }

        private void btnSurveys_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "csv",
                AddExtension = true,
                InitialDirectory = Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile
                    ) + @"\Downloads",
                Title = "Choose a Source File - PestPac Utility",
                RestoreDirectory = true,
                Multiselect = false
            };

            string lastPath = null;
            string temp = null;
            if (config.TryGetValue("LastPath", out temp))
            {
                if (!(MessageBox.Show(
                        "The current target is set to \"" +
                            lastPath + "\", would you like to change it?",
                        "PestPac Utility",
                        MessageBoxButtons.YesNo
                    ) == DialogResult.Yes &&
                    dialog.TryGetPath(out lastPath))
                )
                    lastPath = temp;
            }
            else if (!dialog.TryGetPath(out lastPath))
                return;

            dialog.Dispose();
            dialog = null;

            string type = grpType.Controls.Cast<RadioButton>().Single(r => r.Checked).Text;
            grpType.Enabled = false;
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(
                    (o) => {
                        SogoClient sogo = new SogoClient(Logger);
                        sogo.LoadSurvey(lastPath, o.ToString());
                        sogo.UploadNotes();
                        UIThread.Invoke(() => grpType.Enabled = true);
                    }
                ), type
            );
        }

        private void prgThread_DoWork(object sender, DoWorkEventArgs e)
        {
            int arg = (int)e.Argument;

            if (arg == 0)
                UIThread.Invoke(() => prgBar.Increment(1));
            else if (arg < 0)
                UIThread.Invoke(() => prgBar.Increment(Math.Abs(arg)));
            else
            {
                UIThread.Invoke(() => prgBar.Value = 0);
                UIThread.Invoke(() => prgBar.Maximum = arg);
            }
        }

        private void Logger_OnEntry(object sender, string logName, string entry)
        {
            UIThread.Invoke(() => lstConsole.Items.Add(entry));
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            config.SaveAs(CONFIG_PATH);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!Setup())
                Application.Exit();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MatrixViewer
{
    static class Program
    {

        public static Stopwatch Timer = new Stopwatch();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Timer.Start();
            args = new string[] { @"E:\Insight Program Files\Neural Network\AreaStats2.mtx" };
            if (args != null && args.Length > 0)
            {
                File.WriteAllLines(@"E:\Insight Program Files\Neural Network\Test.txt", args);
                string path = args[0];
                if (File.Exists(path))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    Form1 mainForm = new Form1();
                    mainForm.OpenFile(path);
                    Application.Run(mainForm);
                }
                else
                {
                    MessageBox.Show("The file does not exist!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}

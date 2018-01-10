namespace TaskBarApp
{
    using MutexManager;
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            if (!SingleInstance.Start())
            {
                MessageBox.Show("Please check your tray icons near the clock in the menu bar - the text app is already running...", "Text App", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new TaskBarApplication());
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message, "Program Terminated Unexpectedly", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                SingleInstance.Stop();
            }
        }
    }
}


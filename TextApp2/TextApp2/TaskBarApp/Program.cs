using MutexManager;
using System;
using System.Windows.Forms;

namespace TaskBarApp
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			if (!SingleInstance.Start())
			{
				MessageBox.Show("Please check your tray icons near the clock in the menu bar - the text app is already running...", "Text App", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new TaskBarApplication());
			}
			catch (Exception arg_32_0)
			{
				MessageBox.Show(arg_32_0.Message, "Program Terminated Unexpectedly", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			SingleInstance.Stop();
		}
	}
}

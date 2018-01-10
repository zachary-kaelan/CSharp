using AutoUpdaterDotNET;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TaskBarApp.Properties;

namespace TaskBarApp
{
	public class TaskBarApplication : ApplicationContext
	{
		private IContainer components;

		private NotifyIcon notifyIcon;

		private readonly ApplicationManager appManager;

		public TaskBarApplication()
		{
			this.Initialize();
			this.appManager = new ApplicationManager(this.notifyIcon);
			this.appManager.InitializeApplicationVariables();
			this.appManager.InitializeAccountRegistryConfiguration(true);
			if (this.appManager.m_bIsBranded)
			{
				AutoUpdater.LetUserSelectSkip = false;
				AutoUpdater.Start(this.appManager.m_strAutoUpdateFileURL);
			}
			this.appManager.UpdateIcon();
			this.appManager.StartMonitorNotifyFormTimer();
			this.appManager.StartMonitorServerSyncTimer();
			this.appManager.StartMonitorTextMessageTimer();
			this.appManager.GroupSchedulFileLoad();
			if (this.appManager.m_bDashboardMode)
			{
				this.appManager.StartMonitorAccountDashboardTimer();
				this.appManager.InitializeAccountList();
				this.appManager.ShowAccountDashboard();
				this.appManager.ShowBalloon("Please click refresh to load the dasboard or double click on an account to access your text messages", 5);
				return;
			}
			this.appManager.LogIn(true, null, null, null, null);
		}

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			if (this.appManager.m_bConnected)
			{
				this.appManager.ShowMessages();
			}
		}

		private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.notifyIcon, null);
			}
		}

		private void notifyIcon_BalloonTipShown(object sender, EventArgs e)
		{
			this.appManager.BalloonShown();
		}

		private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
		{
			this.appManager.BalloonClick();
		}

		private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			e.Cancel = false;
			this.notifyIcon.ContextMenuStrip.Items.Clear();
			if (!this.appManager.m_bConnected)
			{
				this.appManager.UpdateIcon();
			}
			if (this.appManager.m_bConnected)
			{
				if (this.appManager.m_bEnableKeywordProcessing)
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
					toolStripMenuItem.Text = "Keyword Processing Enabled";
					toolStripMenuItem.BackColor = ColorTranslator.FromHtml("#93FF14");
					this.notifyIcon.ContextMenuStrip.Items.Add(toolStripMenuItem);
				}
				if (this.appManager.m_bEnableGroupScheduleProcessing)
				{
					ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
					toolStripMenuItem2.Text = "Group Schedule Processing Enabled";
					toolStripMenuItem2.BackColor = ColorTranslator.FromHtml("#93FF14");
					this.notifyIcon.ContextMenuStrip.Items.Add(toolStripMenuItem2);
					this.notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
				}
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Messages", new EventHandler(this.showMessages_Click)));
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&New Message", new EventHandler(this.showNewMessage_Click)));
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("Edit &Contacts", new EventHandler(this.showNewContact_Click)));
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("Edit &Groups", new EventHandler(this.showNewGroup_Click)));
				if (this.appManager.m_bKeywordFeature)
				{
					this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("Edit &Keyword Auto Response", new EventHandler(this.showKeywordAutoResponse_Click)));
				}
				if (this.appManager.m_bGroupScheduleFeature)
				{
					this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("Edit Group S&chedule", new EventHandler(this.showGroupSchedule_Click)));
				}
				this.notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			}
			if (this.appManager.m_bDashboardMode)
			{
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Dashboard", new EventHandler(this.showDashboard_Click)));
				if (this.appManager.m_bConnected)
				{
					this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Log Out " + this.appManager.FormatPhone(this.appManager.m_strUserName), new EventHandler(this.logOut_Click)));
				}
			}
			else if (this.appManager.m_bConnected)
			{
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Log Out " + this.appManager.FormatPhone(this.appManager.m_strUserName), new EventHandler(this.logOut_Click)));
			}
			else
			{
				this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Log In", new EventHandler(this.showUserLogIn_Click)));
			}
			this.notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Settings", new EventHandler(this.showSettings_Click)));
			this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&About/Help " + this.appManager.m_AssemblyVersion, new EventHandler(this.showAbout_Click)));
			this.notifyIcon.ContextMenuStrip.Items.Add(this.appManager.ToolStripMenuItemWithHandler("&Exit", new EventHandler(this.exitItem_Click)));
		}

		private void logOut_Click(object sender, EventArgs e)
		{
			this.appManager.LogOut(true);
		}

		private void showUserLogIn_Click(object sender, EventArgs e)
		{
			this.appManager.LogIn(true, null, null, null, null);
		}

		public void showMessages_Click(object sender, EventArgs e)
		{
			this.appManager.ShowMessages();
		}

		private void showNewMessage_Click(object sender, EventArgs e)
		{
			this.appManager.ShowNewMessage();
		}

		private void showSettings_Click(object sender, EventArgs e)
		{
			this.appManager.ShowSettings();
		}

		private void showMessageTemplates_Click(object sender, EventArgs e)
		{
			this.appManager.ShowMessageTemplate();
		}

		private void showNewContact_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditContact(true);
		}

		private void showNewGroup_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditGroups();
		}

		private void showKeywordAutoResponse_Click(object sender, EventArgs e)
		{
			this.appManager.ShowKeywordAutoResponse();
		}

		private void showGroupSchedule_Click(object sender, EventArgs e)
		{
			this.appManager.ShowGroupSchedule();
		}

		private void showDashboard_Click(object sender, EventArgs e)
		{
			this.appManager.ShowAccountDashboard();
		}

		private void showAbout_Click(object sender, EventArgs e)
		{
			this.appManager.LaunchWebsite(this.appManager.m_strHelpURL);
		}

		private void Initialize()
		{
			this.components = new Container();
			this.notifyIcon = new NotifyIcon(this.components)
			{
				ContextMenuStrip = new ContextMenuStrip(),
				Icon = Resources.blank,
				Text = "",
				Visible = true
			};
			this.notifyIcon.ContextMenuStrip.Opening += new CancelEventHandler(this.ContextMenuStrip_Opening);
			this.notifyIcon.MouseUp += new MouseEventHandler(this.notifyIcon_MouseUp);
			this.notifyIcon.BalloonTipClicked += new EventHandler(this.notifyIcon_BalloonTipClicked);
			this.notifyIcon.DoubleClick += new EventHandler(this.notifyIcon_DoubleClick);
			this.notifyIcon.BalloonTipShown += new EventHandler(this.notifyIcon_BalloonTipShown);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
		}

		private void exitItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to exit " + this.appManager.m_strApplicationName + "? Incoming messages will not be displayed.", this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				base.ExitThread();
			}
		}

		protected override void ExitThreadCore()
		{
			if (this.appManager.formUserLogin != null)
			{
				this.appManager.formUserLogin.Close();
			}
			if (this.appManager.formMessages != null)
			{
				this.appManager.formMessages.Close();
			}
			if (this.appManager.formNewMessage != null)
			{
				this.appManager.formNewMessage.Close();
			}
			if (this.appManager.formSettings != null)
			{
				this.appManager.formSettings.Close();
			}
			if (this.appManager.formEditContact != null)
			{
				this.appManager.formEditContact.Close();
			}
			if (this.appManager.formEditGroups != null)
			{
				this.appManager.formEditGroups.Close();
			}
			if (this.appManager.formMessageTemplate != null)
			{
				this.appManager.formMessageTemplate.Close();
			}
			if (this.appManager.formAccountDashboard != null)
			{
				this.appManager.formAccountDashboard.Close();
			}
			if (this.appManager.formPrintConversation != null)
			{
				this.appManager.formPrintConversation.Close();
			}
			if (this.appManager.formEditAccounts != null)
			{
				this.appManager.formEditAccounts.Close();
			}
			if (this.appManager.formKeywordAutoResponse != null)
			{
				this.appManager.formKeywordAutoResponse.Close();
			}
			if (this.appManager.formGroupSchedule != null)
			{
				this.appManager.formGroupSchedule.Close();
			}
			if (this.appManager.formEditGroupSchedule != null)
			{
				this.appManager.formEditGroupSchedule.Close();
			}
			this.appManager.LogOut(false);
			this.notifyIcon.Visible = false;
			base.ExitThreadCore();
		}
	}
}

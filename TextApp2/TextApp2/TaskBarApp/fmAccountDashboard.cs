using AutoUpdaterDotNET;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaskBarApp.Objects;

namespace TaskBarApp
{
	public class fmAccountDashboard : Form
	{
		private class CustomPopupControl : ToolStripControlHost
		{
			public CustomPopupControl(string title, string message) : base(new Panel())
			{
				if (string.IsNullOrEmpty(message))
				{
					message = "No messages found...";
				}
				Label label = new Label();
				label.BackColor = Color.LightGray;
				label.Text = title;
				label.Dock = DockStyle.Top;
				Label label2 = new Label();
				label2.BackColor = ColorTranslator.FromHtml("#E8FFCC");
				label2.Text = message;
				label2.Dock = DockStyle.Fill;
				base.Control.MinimumSize = new Size(300, 350);
				base.Control.Controls.Add(label2);
				base.Control.Controls.Add(label);
			}
		}

		private string strError = string.Empty;

		private DataGridViewColumn AccountDashboardSortedColumn;

		private ListSortDirection AccountDashboardSortedDirection;

		private IContainer components;

		private MenuStrip menuStripNewMessage;

		private ToolStripMenuItem optionsToolStripMenuItem;

		private ToolStripMenuItem settingsToolStripMenuItem;

		private Button buttonRefresh;

		private Label labelTotalAccounts;

		private Label labelTotalUnread;

		private DataGridView dataGridViewAccounts;

		private DataGridViewTextBoxColumn colPhoneNumber;

		private DataGridViewTextBoxColumn colTitle;

		private DataGridViewTextBoxColumn colUnReadCount;

		private DataGridViewTextBoxColumn colOldestUnRead;

		private DataGridViewTextBoxColumn colLastSyncDate;

		private DataGridViewTextBoxColumn colConnectionStatus;

		private ToolStripMenuItem editToolStripMenuItem;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem generalHelpToolStripMenuItem;

		private ToolStripMenuItem settingsHelpToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem exitToolStripMenuItem;

		private ToolStripMenuItem editAccountsToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem openDashboardWithUnreadReminderToolStripMenuItem;

		private CheckBox checkBoxFullRefresh;

		private ToolStripMenuItem syncFeaturesToolStripMenuItem;

		private ToolStripMenuItem versionToolStripMenuItem;

		private ToolStripMenuItem tryBETAToolStripMenuItem;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmAccountDashboard()
		{
			this.InitializeComponent();
		}

		private void fmAccountDashboard_Load(object sender, EventArgs e)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				RegistryKey expr_0F = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.GetValue(expr_0F, "local_FormAccountDashboardWidth", ref num, ref this.strError);
				AppRegistry.GetValue(expr_0F, "local_FormAccountDashboardHeight", ref num2, ref this.strError);
				if (num2 != 0)
				{
					base.Height = num2;
				}
				if (num != 0)
				{
					base.Width = num;
				}
				this.versionToolStripMenuItem.Text = this.appManager.m_AssemblyVersion + " (Update)";
				if (this.appManager.m_bIsBranded)
				{
					this.versionToolStripMenuItem.Text = this.appManager.m_AssemblyVersion;
					this.versionToolStripMenuItem.Enabled = false;
					this.tryBETAToolStripMenuItem.Visible = false;
				}
				this.Text = this.appManager.m_strApplicationName + " Account Dashboard";
				base.Icon = this.appManager.iTextApp;
				this.LoadGridViewAccounts();
			}
			catch (Exception ex)
			{
				this.strError = "Unexpected application error while loading Account Dashboard window: " + ex.Message;
			}
			if (this.strError.Length > 0)
			{
				this.appManager.ShowBalloon(this.strError, 5);
			}
		}

		private void fmAccountDashboard_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_0B, "local_FormAccountDashboardWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_FormAccountDashboardHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
			}
			catch
			{
			}
		}

		private void fmAccountDashboard_Activated(object sender, EventArgs e)
		{
			this.LoadGridViewAccounts();
		}

		public void LoadGridViewAccounts()
		{
			if (this.appManager.m_lsAccountItems != null)
			{
				this.dataGridViewAccounts.Rows.Clear();
				foreach (AccountItem current in this.appManager.m_lsAccountItems)
				{
					string text = this.appManager.GetAccountItemOldestUnreadDate(current);
					if (current.connectionStatus == "Logged In")
					{
						text = "See Message Window";
					}
					this.dataGridViewAccounts.Rows.Add(new object[]
					{
						this.appManager.FormatPhone(current.number),
						current.title,
						current.unReadMessageList.Count<TextMessage>().ToString(),
						text,
						current.lastSyncDate,
						current.connectionStatus
					});
				}
				if (this.AccountDashboardSortedColumn == null)
				{
					this.dataGridViewAccounts.Sort(this.dataGridViewAccounts.Columns[1], ListSortDirection.Ascending);
				}
				else
				{
					DataGridViewColumn dataGridViewColumn = this.dataGridViewAccounts.Columns[this.AccountDashboardSortedColumn.Name];
					this.dataGridViewAccounts.Sort(dataGridViewColumn, this.AccountDashboardSortedDirection);
				}
				foreach (DataGridViewRow dataGridViewRow in ((IEnumerable)this.dataGridViewAccounts.Rows))
				{
					if (this.dataGridViewAccounts["colConnectionStatus", dataGridViewRow.Index].Value.ToString() == "Logged In")
					{
						this.dataGridViewAccounts.Rows[dataGridViewRow.Index].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8FFCC");
					}
				}
				this.labelTotalAccounts.Text = "Total Accounts: " + this.appManager.m_lsAccountItems.Count.ToString();
				this.labelTotalUnread.Text = "Total Unread: " + this.appManager.GetTotalDashboardUnread(false).ToString();
				this.DisplayRefreshCount();
			}
		}

		public void DisplayRefreshCount()
		{
			if (this.appManager.m_bAccountDashboardLoading)
			{
				this.labelTotalAccounts.BackColor = ColorTranslator.FromHtml("#93FF14");
				this.labelTotalAccounts.Text = "Refreshing: " + this.appManager.m_nAccountNDX.ToString() + " of " + this.appManager.m_lsAccountItems.Count.ToString();
				return;
			}
			this.labelTotalAccounts.BackColor = Control.DefaultBackColor;
			this.labelTotalAccounts.Text = "Total Accounts: " + this.appManager.m_lsAccountItems.Count.ToString();
		}

		private void dataGridViewAccounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			try
			{
				string phone = this.appManager.FormatContactAddress(this.dataGridViewAccounts.Rows[e.RowIndex].Cells["colPhoneNumber"].Value.ToString(), true, false);
				this.labelTotalAccounts.BackColor = ColorTranslator.FromHtml("#93FF14");
				this.labelTotalAccounts.Text = "Loading account " + this.appManager.FormatPhone(phone);
				Application.DoEvents();
				AccountItem accountItem = this.appManager.GetAccountItem(phone);
				if (accountItem.connectionStatus == "Logged In")
				{
					this.appManager.ShowMessages();
				}
				else if (accountItem.connectionStatus == "Failed Authentication")
				{
					this.appManager.ShowBalloon("Account " + this.appManager.FormatPhone(phone) + " cannot connect.  Please review the Connection Status and verify account information accordingly.", 5);
				}
				else
				{
					this.appManager.ShowBalloon("Logging into account " + this.appManager.FormatPhone(phone) + "...", 5);
					if (this.appManager.m_bConnected)
					{
						this.appManager.LogOut(false);
					}
					this.appManager.LogIn(false, accountItem.number.ToString(), accountItem.password, accountItem.countryCode, accountItem.title);
				}
				this.DisplayRefreshCount();
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("There was an exception selecting account, please click Refresh to relaod account list - Exception: " + ex.Message, 5);
			}
		}

		private void dataGridViewAccounts_Sorted(object sender, EventArgs e)
		{
			this.AccountDashboardSortedColumn = this.dataGridViewAccounts.SortedColumn;
			if (this.dataGridViewAccounts.SortOrder == SortOrder.Descending)
			{
				this.AccountDashboardSortedDirection = ListSortDirection.Descending;
				return;
			}
			this.AccountDashboardSortedDirection = ListSortDirection.Ascending;
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			this.labelTotalAccounts.BackColor = ColorTranslator.FromHtml("#93FF14");
			this.labelTotalAccounts.Text = "Refreshing...";
			Application.DoEvents();
			this.LoadGridViewAccounts();
			Application.DoEvents();
			this.buttonRefresh.Enabled = false;
			this.labelTotalAccounts.Focus();
			if (this.checkBoxFullRefresh.Checked)
			{
				this.appManager.ShowBalloon("Getting full refresh of text message counts for " + this.appManager.m_lsAccountItems.Count.ToString() + " accounts...", 5);
				this.appManager.m_bAccountDashboardRefreshMessages = true;
				this.checkBoxFullRefresh.Checked = false;
			}
			this.appManager.LoadAccountDashboard();
			this.buttonRefresh.Enabled = true;
		}

		private void optionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			this.openDashboardWithUnreadReminderToolStripMenuItem.Checked = this.appManager.m_bPopDashboardWindow;
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowSettings();
		}

		private void editAccountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditAccounts(true);
		}

		private void openDashboardWithUnreadReminderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "PopDashboardWindow", this.openDashboardWithUnreadReminderToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Open dashboard with unread reminder save error: " + text;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bPopDashboardWindow = this.openDashboardWithUnreadReminderToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Open dashboard with unread reminder save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 10);
			}
		}

		private void generalHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.LaunchWebsite(this.appManager.m_strHelpURL);
		}

		private void settingsHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.LaunchWebsite(this.appManager.m_strSettingsURL);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to exit " + this.appManager.m_strApplicationName + "? Incoming messages will not be displayed.", this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Application.Exit();
			}
		}

		private void syncFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.m_bNotifyServerSync = true;
			this.appManager.GetServerSettings(true);
		}

		private void versionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AutoUpdater.ShowUserNoUpdateAvailable = true;
			AutoUpdater.LetUserSelectRemindLater = false;
			AutoUpdater.LetUserSelectSkip = false;
			AutoUpdater.Start(this.appManager.m_strUpdateFileURL);
		}

		private void tryBETAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AutoUpdater.ShowUserNoUpdateAvailable = true;
			AutoUpdater.LetUserSelectRemindLater = false;
			AutoUpdater.LetUserSelectSkip = false;
			AutoUpdater.Start(this.appManager.m_strBETAUpdateFileURL);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmAccountDashboard));
			this.menuStripNewMessage = new MenuStrip();
			this.editToolStripMenuItem = new ToolStripMenuItem();
			this.editAccountsToolStripMenuItem = new ToolStripMenuItem();
			this.optionsToolStripMenuItem = new ToolStripMenuItem();
			this.settingsToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.openDashboardWithUnreadReminderToolStripMenuItem = new ToolStripMenuItem();
			this.helpToolStripMenuItem = new ToolStripMenuItem();
			this.generalHelpToolStripMenuItem = new ToolStripMenuItem();
			this.settingsHelpToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.exitToolStripMenuItem = new ToolStripMenuItem();
			this.syncFeaturesToolStripMenuItem = new ToolStripMenuItem();
			this.versionToolStripMenuItem = new ToolStripMenuItem();
			this.dataGridViewAccounts = new DataGridView();
			this.colPhoneNumber = new DataGridViewTextBoxColumn();
			this.colTitle = new DataGridViewTextBoxColumn();
			this.colUnReadCount = new DataGridViewTextBoxColumn();
			this.colOldestUnRead = new DataGridViewTextBoxColumn();
			this.colLastSyncDate = new DataGridViewTextBoxColumn();
			this.colConnectionStatus = new DataGridViewTextBoxColumn();
			this.buttonRefresh = new Button();
			this.labelTotalAccounts = new Label();
			this.labelTotalUnread = new Label();
			this.checkBoxFullRefresh = new CheckBox();
			this.tryBETAToolStripMenuItem = new ToolStripMenuItem();
			this.menuStripNewMessage.SuspendLayout();
			((ISupportInitialize)this.dataGridViewAccounts).BeginInit();
			base.SuspendLayout();
			this.menuStripNewMessage.Items.AddRange(new ToolStripItem[]
			{
				this.editToolStripMenuItem,
				this.optionsToolStripMenuItem,
				this.helpToolStripMenuItem
			});
			this.menuStripNewMessage.Location = new Point(0, 0);
			this.menuStripNewMessage.Name = "menuStripNewMessage";
			this.menuStripNewMessage.Size = new Size(640, 24);
			this.menuStripNewMessage.TabIndex = 8;
			this.menuStripNewMessage.Text = "menuStrip1";
			this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.editAccountsToolStripMenuItem
			});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			this.editAccountsToolStripMenuItem.Name = "editAccountsToolStripMenuItem";
			this.editAccountsToolStripMenuItem.Size = new Size(147, 22);
			this.editAccountsToolStripMenuItem.Text = "Edit &Accounts";
			this.editAccountsToolStripMenuItem.Click += new EventHandler(this.editAccountsToolStripMenuItem_Click);
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.settingsToolStripMenuItem,
				this.toolStripSeparator2,
				this.openDashboardWithUnreadReminderToolStripMenuItem
			});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new Size(286, 22);
			this.settingsToolStripMenuItem.Text = "&Settings";
			this.settingsToolStripMenuItem.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(283, 6);
			this.openDashboardWithUnreadReminderToolStripMenuItem.CheckOnClick = true;
			this.openDashboardWithUnreadReminderToolStripMenuItem.Name = "openDashboardWithUnreadReminderToolStripMenuItem";
			this.openDashboardWithUnreadReminderToolStripMenuItem.Size = new Size(286, 22);
			this.openDashboardWithUnreadReminderToolStripMenuItem.Text = "Open Dashboard With Unread Reminder";
			this.openDashboardWithUnreadReminderToolStripMenuItem.Click += new EventHandler(this.openDashboardWithUnreadReminderToolStripMenuItem_Click);
			this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.generalHelpToolStripMenuItem,
				this.settingsHelpToolStripMenuItem,
				this.toolStripSeparator1,
				this.exitToolStripMenuItem,
				this.syncFeaturesToolStripMenuItem,
				this.versionToolStripMenuItem,
				this.tryBETAToolStripMenuItem
			});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
			this.generalHelpToolStripMenuItem.Size = new Size(152, 22);
			this.generalHelpToolStripMenuItem.Text = "General Help";
			this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
			this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
			this.settingsHelpToolStripMenuItem.Size = new Size(152, 22);
			this.settingsHelpToolStripMenuItem.Text = "Settings Help";
			this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(149, 6);
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new Size(152, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
			this.syncFeaturesToolStripMenuItem.Name = "syncFeaturesToolStripMenuItem";
			this.syncFeaturesToolStripMenuItem.Size = new Size(152, 22);
			this.syncFeaturesToolStripMenuItem.Text = "Sync Features";
			this.syncFeaturesToolStripMenuItem.Click += new EventHandler(this.syncFeaturesToolStripMenuItem_Click);
			this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
			this.versionToolStripMenuItem.Size = new Size(152, 22);
			this.versionToolStripMenuItem.Text = "Version";
			this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
			this.dataGridViewAccounts.AllowUserToAddRows = false;
			this.dataGridViewAccounts.AllowUserToDeleteRows = false;
			this.dataGridViewAccounts.AllowUserToResizeRows = false;
			this.dataGridViewAccounts.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.dataGridViewAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridViewAccounts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
			this.dataGridViewAccounts.BackgroundColor = SystemColors.Control;
			this.dataGridViewAccounts.BorderStyle = BorderStyle.None;
			this.dataGridViewAccounts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Control;
			dataGridViewCellStyle.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.True;
			this.dataGridViewAccounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle;
			this.dataGridViewAccounts.Columns.AddRange(new DataGridViewColumn[]
			{
				this.colPhoneNumber,
				this.colTitle,
				this.colUnReadCount,
				this.colOldestUnRead,
				this.colLastSyncDate,
				this.colConnectionStatus
			});
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.Window;
			dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.Padding = new Padding(1);
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
			this.dataGridViewAccounts.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridViewAccounts.EditMode = DataGridViewEditMode.EditProgrammatically;
			this.dataGridViewAccounts.EnableHeadersVisualStyles = false;
			this.dataGridViewAccounts.GridColor = SystemColors.Control;
			this.dataGridViewAccounts.Location = new Point(0, 24);
			this.dataGridViewAccounts.MultiSelect = false;
			this.dataGridViewAccounts.Name = "dataGridViewAccounts";
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = SystemColors.Control;
			dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(232, 255, 204);
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.Desktop;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
			this.dataGridViewAccounts.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridViewAccounts.RowHeadersVisible = false;
			this.dataGridViewAccounts.RowHeadersWidth = 30;
			dataGridViewCellStyle4.BackColor = Color.White;
			dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle4.ForeColor = Color.Black;
			dataGridViewCellStyle4.SelectionBackColor = Color.White;
			dataGridViewCellStyle4.SelectionForeColor = Color.Black;
			this.dataGridViewAccounts.RowsDefaultCellStyle = dataGridViewCellStyle4;
			this.dataGridViewAccounts.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			this.dataGridViewAccounts.RowTemplate.DefaultCellStyle.Padding = new Padding(1);
			this.dataGridViewAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewAccounts.Size = new Size(640, 263);
			this.dataGridViewAccounts.TabIndex = 9;
			this.dataGridViewAccounts.TabStop = false;
			this.dataGridViewAccounts.CellDoubleClick += new DataGridViewCellEventHandler(this.dataGridViewAccounts_CellDoubleClick);
			this.dataGridViewAccounts.Sorted += new EventHandler(this.dataGridViewAccounts_Sorted);
			this.colPhoneNumber.FillWeight = 50f;
			this.colPhoneNumber.HeaderText = "Number";
			this.colPhoneNumber.MinimumWidth = 100;
			this.colPhoneNumber.Name = "colPhoneNumber";
			this.colTitle.FillWeight = 90f;
			this.colTitle.HeaderText = "Title";
			this.colTitle.MinimumWidth = 70;
			this.colTitle.Name = "colTitle";
			this.colUnReadCount.FillWeight = 40f;
			this.colUnReadCount.HeaderText = "Unread";
			this.colUnReadCount.MinimumWidth = 20;
			this.colUnReadCount.Name = "colUnReadCount";
			this.colOldestUnRead.HeaderText = "Oldest Unread";
			this.colOldestUnRead.MinimumWidth = 100;
			this.colOldestUnRead.Name = "colOldestUnRead";
			this.colLastSyncDate.HeaderText = "Last Refresh";
			this.colLastSyncDate.MinimumWidth = 100;
			this.colLastSyncDate.Name = "colLastSyncDate";
			this.colConnectionStatus.FillWeight = 60f;
			this.colConnectionStatus.HeaderText = "Connection Status";
			this.colConnectionStatus.MinimumWidth = 50;
			this.colConnectionStatus.Name = "colConnectionStatus";
			this.buttonRefresh.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonRefresh.Location = new Point(543, 298);
			this.buttonRefresh.Margin = new Padding(4);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new Size(84, 27);
			this.buttonRefresh.TabIndex = 11;
			this.buttonRefresh.Text = "Refresh";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
			this.labelTotalAccounts.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.labelTotalAccounts.AutoSize = true;
			this.labelTotalAccounts.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelTotalAccounts.Location = new Point(167, 302);
			this.labelTotalAccounts.Name = "labelTotalAccounts";
			this.labelTotalAccounts.Size = new Size(111, 18);
			this.labelTotalAccounts.TabIndex = 12;
			this.labelTotalAccounts.Text = "Total Accounts:";
			this.labelTotalUnread.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.labelTotalUnread.AutoSize = true;
			this.labelTotalUnread.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelTotalUnread.Location = new Point(12, 302);
			this.labelTotalUnread.Name = "labelTotalUnread";
			this.labelTotalUnread.Size = new Size(97, 18);
			this.labelTotalUnread.TabIndex = 14;
			this.labelTotalUnread.Text = "Total Unread:";
			this.checkBoxFullRefresh.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.checkBoxFullRefresh.AutoSize = true;
			this.checkBoxFullRefresh.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.checkBoxFullRefresh.ForeColor = Color.FromArgb(64, 64, 64);
			this.checkBoxFullRefresh.Location = new Point(449, 304);
			this.checkBoxFullRefresh.Name = "checkBoxFullRefresh";
			this.checkBoxFullRefresh.Size = new Size(87, 18);
			this.checkBoxFullRefresh.TabIndex = 15;
			this.checkBoxFullRefresh.Text = "Full Refresh";
			this.checkBoxFullRefresh.UseVisualStyleBackColor = true;
			this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
			this.tryBETAToolStripMenuItem.Size = new Size(152, 22);
			this.tryBETAToolStripMenuItem.Text = "Try BETA!";
			this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(640, 337);
			base.Controls.Add(this.checkBoxFullRefresh);
			base.Controls.Add(this.labelTotalUnread);
			base.Controls.Add(this.labelTotalAccounts);
			base.Controls.Add(this.buttonRefresh);
			base.Controls.Add(this.dataGridViewAccounts);
			base.Controls.Add(this.menuStripNewMessage);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MainMenuStrip = this.menuStripNewMessage;
			this.MinimumSize = new Size(600, 375);
			base.Name = "fmAccountDashboard";
			this.Text = "Account Dashboard";
			base.Activated += new EventHandler(this.fmAccountDashboard_Activated);
			base.FormClosing += new FormClosingEventHandler(this.fmAccountDashboard_FormClosing);
			base.Load += new EventHandler(this.fmAccountDashboard_Load);
			this.menuStripNewMessage.ResumeLayout(false);
			this.menuStripNewMessage.PerformLayout();
			((ISupportInitialize)this.dataGridViewAccounts).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

namespace TaskBarApp
{
    using AutoUpdaterDotNET;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;

    public class fmAccountDashboard : Form
    {
        private DataGridViewColumn AccountDashboardSortedColumn = null;
        private ListSortDirection AccountDashboardSortedDirection;
        private Button buttonRefresh = null;
        private CheckBox checkBoxFullRefresh = null;
        private DataGridViewTextBoxColumn colConnectionStatus = null;
        private DataGridViewTextBoxColumn colLastSyncDate = null;
        private DataGridViewTextBoxColumn colOldestUnRead = null;
        private DataGridViewTextBoxColumn colPhoneNumber = null;
        private DataGridViewTextBoxColumn colTitle = null;
        private DataGridViewTextBoxColumn colUnReadCount = null;
        private IContainer components = null;
        private DataGridView dataGridViewAccounts = null;
        private ToolStripMenuItem editAccountsToolStripMenuItem = null;
        private ToolStripMenuItem editToolStripMenuItem = null;
        private ToolStripMenuItem exitToolStripMenuItem = null;
        private ToolStripMenuItem generalHelpToolStripMenuItem = null;
        private ToolStripMenuItem helpToolStripMenuItem = null;
        private Label labelTotalAccounts = null;
        private Label labelTotalUnread = null;
        private MenuStrip menuStripNewMessage = null;
        private ToolStripMenuItem openDashboardWithUnreadReminderToolStripMenuItem = null;
        private ToolStripMenuItem optionsToolStripMenuItem = null;
        private ToolStripMenuItem settingsHelpToolStripMenuItem = null;
        private ToolStripMenuItem settingsToolStripMenuItem = null;
        private string strError = string.Empty;
        private ToolStripMenuItem syncFeaturesToolStripMenuItem = null;
        private ToolStripSeparator toolStripSeparator1 = null;
        private ToolStripSeparator toolStripSeparator2 = null;
        private ToolStripMenuItem tryBETAToolStripMenuItem = null;
        private ToolStripMenuItem versionToolStripMenuItem = null;

        public fmAccountDashboard()
        {
            this.InitializeComponent();
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

        private void dataGridViewAccounts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
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
                catch (Exception exception)
                {
                    this.appManager.ShowBalloon("There was an exception selecting account, please click Refresh to relaod account list - Exception: " + exception.Message, 5);
                }
            }
        }

        private void dataGridViewAccounts_Sorted(object sender, EventArgs e)
        {
            this.AccountDashboardSortedColumn = this.dataGridViewAccounts.SortedColumn;
            if (this.dataGridViewAccounts.SortOrder == SortOrder.Descending)
            {
                this.AccountDashboardSortedDirection = ListSortDirection.Descending;
            }
            else
            {
                this.AccountDashboardSortedDirection = ListSortDirection.Ascending;
            }
        }

        public void DisplayRefreshCount()
        {
            if (this.appManager.m_bAccountDashboardLoading)
            {
                this.labelTotalAccounts.BackColor = ColorTranslator.FromHtml("#93FF14");
                this.labelTotalAccounts.Text = "Refreshing: " + this.appManager.m_nAccountNDX.ToString() + " of " + this.appManager.m_lsAccountItems.Count.ToString();
            }
            else
            {
                this.labelTotalAccounts.BackColor = Control.DefaultBackColor;
                this.labelTotalAccounts.Text = "Total Accounts: " + this.appManager.m_lsAccountItems.Count.ToString();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void editAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowEditAccounts(true);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit " + this.appManager.m_strApplicationName + "? Incoming messages will not be displayed.", this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void fmAccountDashboard_Activated(object sender, EventArgs e)
        {
            this.LoadGridViewAccounts();
        }

        private void fmAccountDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormAccountDashboardWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormAccountDashboardHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmAccountDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormAccountDashboardWidth", ref num, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormAccountDashboardHeight", ref num2, ref this.strError);
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
            catch (Exception exception)
            {
                this.strError = "Unexpected application error while loading Account Dashboard window: " + exception.Message;
            }
            if (this.strError.Length > 0)
            {
                this.appManager.ShowBalloon(this.strError, 5);
            }
        }

        private void generalHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.LaunchWebsite(this.appManager.m_strHelpURL);
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            DataGridViewCellStyle style4 = new DataGridViewCellStyle();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmAccountDashboard));
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
            ((ISupportInitialize) this.dataGridViewAccounts).BeginInit();
            base.SuspendLayout();
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.editToolStripMenuItem, this.optionsToolStripMenuItem, this.helpToolStripMenuItem };
            this.menuStripNewMessage.Items.AddRange(toolStripItems);
            this.menuStripNewMessage.Location = new Point(0, 0);
            this.menuStripNewMessage.Name = "menuStripNewMessage";
            this.menuStripNewMessage.Size = new Size(640, 0x18);
            this.menuStripNewMessage.TabIndex = 8;
            this.menuStripNewMessage.Text = "menuStrip1";
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.editAccountsToolStripMenuItem };
            this.editToolStripMenuItem.DropDownItems.AddRange(itemArray2);
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new Size(0x27, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.editAccountsToolStripMenuItem.Name = "editAccountsToolStripMenuItem";
            this.editAccountsToolStripMenuItem.Size = new Size(0x93, 0x16);
            this.editAccountsToolStripMenuItem.Text = "Edit &Accounts";
            this.editAccountsToolStripMenuItem.Click += new EventHandler(this.editAccountsToolStripMenuItem_Click);
            ToolStripItem[] itemArray3 = new ToolStripItem[] { this.settingsToolStripMenuItem, this.toolStripSeparator2, this.openDashboardWithUnreadReminderToolStripMenuItem };
            this.optionsToolStripMenuItem.DropDownItems.AddRange(itemArray3);
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new Size(0x3d, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new Size(0x11e, 0x16);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(0x11b, 6);
            this.openDashboardWithUnreadReminderToolStripMenuItem.CheckOnClick = true;
            this.openDashboardWithUnreadReminderToolStripMenuItem.Name = "openDashboardWithUnreadReminderToolStripMenuItem";
            this.openDashboardWithUnreadReminderToolStripMenuItem.Size = new Size(0x11e, 0x16);
            this.openDashboardWithUnreadReminderToolStripMenuItem.Text = "Open Dashboard With Unread Reminder";
            this.openDashboardWithUnreadReminderToolStripMenuItem.Click += new EventHandler(this.openDashboardWithUnreadReminderToolStripMenuItem_Click);
            ToolStripItem[] itemArray4 = new ToolStripItem[] { this.generalHelpToolStripMenuItem, this.settingsHelpToolStripMenuItem, this.toolStripSeparator1, this.exitToolStripMenuItem, this.syncFeaturesToolStripMenuItem, this.versionToolStripMenuItem, this.tryBETAToolStripMenuItem };
            this.helpToolStripMenuItem.DropDownItems.AddRange(itemArray4);
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(0x2c, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
            this.generalHelpToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.generalHelpToolStripMenuItem.Text = "General Help";
            this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
            this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
            this.settingsHelpToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.settingsHelpToolStripMenuItem.Text = "Settings Help";
            this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(0x95, 6);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            this.syncFeaturesToolStripMenuItem.Name = "syncFeaturesToolStripMenuItem";
            this.syncFeaturesToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.syncFeaturesToolStripMenuItem.Text = "Sync Features";
            this.syncFeaturesToolStripMenuItem.Click += new EventHandler(this.syncFeaturesToolStripMenuItem_Click);
            this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            this.versionToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.versionToolStripMenuItem.Text = "Version";
            this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
            this.dataGridViewAccounts.AllowUserToAddRows = false;
            this.dataGridViewAccounts.AllowUserToDeleteRows = false;
            this.dataGridViewAccounts.AllowUserToResizeRows = false;
            this.dataGridViewAccounts.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.dataGridViewAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewAccounts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dataGridViewAccounts.BackgroundColor = SystemColors.Control;
            this.dataGridViewAccounts.BorderStyle = BorderStyle.None;
            this.dataGridViewAccounts.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Control;
            style.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.WindowText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this.dataGridViewAccounts.ColumnHeadersDefaultCellStyle = style;
            DataGridViewColumn[] dataGridViewColumns = new DataGridViewColumn[] { this.colPhoneNumber, this.colTitle, this.colUnReadCount, this.colOldestUnRead, this.colLastSyncDate, this.colConnectionStatus };
            this.dataGridViewAccounts.Columns.AddRange(dataGridViewColumns);
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = SystemColors.Window;
            style2.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style2.ForeColor = SystemColors.ControlText;
            style2.Padding = new Padding(1);
            style2.SelectionBackColor = SystemColors.Highlight;
            style2.SelectionForeColor = SystemColors.HighlightText;
            style2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewAccounts.DefaultCellStyle = style2;
            this.dataGridViewAccounts.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewAccounts.EnableHeadersVisualStyles = false;
            this.dataGridViewAccounts.GridColor = SystemColors.Control;
            this.dataGridViewAccounts.Location = new Point(0, 0x18);
            this.dataGridViewAccounts.MultiSelect = false;
            this.dataGridViewAccounts.Name = "dataGridViewAccounts";
            style3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style3.BackColor = SystemColors.Control;
            style3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style3.ForeColor = SystemColors.WindowText;
            style3.SelectionBackColor = System.Drawing.Color.FromArgb(0xe8, 0xff, 0xcc);
            style3.SelectionForeColor = SystemColors.Desktop;
            style3.WrapMode = DataGridViewTriState.True;
            this.dataGridViewAccounts.RowHeadersDefaultCellStyle = style3;
            this.dataGridViewAccounts.RowHeadersVisible = false;
            this.dataGridViewAccounts.RowHeadersWidth = 30;
            style4.BackColor = System.Drawing.Color.White;
            style4.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style4.ForeColor = System.Drawing.Color.Black;
            style4.SelectionBackColor = System.Drawing.Color.White;
            style4.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridViewAccounts.RowsDefaultCellStyle = style4;
            this.dataGridViewAccounts.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewAccounts.RowTemplate.DefaultCellStyle.Padding = new Padding(1);
            this.dataGridViewAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewAccounts.Size = new Size(640, 0x107);
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
            this.buttonRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonRefresh.Location = new Point(0x21f, 0x12a);
            this.buttonRefresh.Margin = new Padding(4);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new Size(0x54, 0x1b);
            this.buttonRefresh.TabIndex = 11;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
            this.labelTotalAccounts.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelTotalAccounts.AutoSize = true;
            this.labelTotalAccounts.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelTotalAccounts.Location = new Point(0xa7, 0x12e);
            this.labelTotalAccounts.Name = "labelTotalAccounts";
            this.labelTotalAccounts.Size = new Size(0x6f, 0x12);
            this.labelTotalAccounts.TabIndex = 12;
            this.labelTotalAccounts.Text = "Total Accounts:";
            this.labelTotalUnread.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelTotalUnread.AutoSize = true;
            this.labelTotalUnread.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelTotalUnread.Location = new Point(12, 0x12e);
            this.labelTotalUnread.Name = "labelTotalUnread";
            this.labelTotalUnread.Size = new Size(0x61, 0x12);
            this.labelTotalUnread.TabIndex = 14;
            this.labelTotalUnread.Text = "Total Unread:";
            this.checkBoxFullRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.checkBoxFullRefresh.AutoSize = true;
            this.checkBoxFullRefresh.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.checkBoxFullRefresh.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.checkBoxFullRefresh.Location = new Point(0x1c1, 0x130);
            this.checkBoxFullRefresh.Name = "checkBoxFullRefresh";
            this.checkBoxFullRefresh.Size = new Size(0x57, 0x12);
            this.checkBoxFullRefresh.TabIndex = 15;
            this.checkBoxFullRefresh.Text = "Full Refresh";
            this.checkBoxFullRefresh.UseVisualStyleBackColor = true;
            this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
            this.tryBETAToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.tryBETAToolStripMenuItem.Text = "Try BETA!";
            this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(640, 0x151);
            base.Controls.Add(this.checkBoxFullRefresh);
            base.Controls.Add(this.labelTotalUnread);
            base.Controls.Add(this.labelTotalAccounts);
            base.Controls.Add(this.buttonRefresh);
            base.Controls.Add(this.dataGridViewAccounts);
            base.Controls.Add(this.menuStripNewMessage);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MainMenuStrip = this.menuStripNewMessage;
            this.MinimumSize = new Size(600, 0x177);
            base.Name = "fmAccountDashboard";
            this.Text = "Account Dashboard";
            base.Activated += new EventHandler(this.fmAccountDashboard_Activated);
            base.FormClosing += new FormClosingEventHandler(this.fmAccountDashboard_FormClosing);
            base.Load += new EventHandler(this.fmAccountDashboard_Load);
            this.menuStripNewMessage.ResumeLayout(false);
            this.menuStripNewMessage.PerformLayout();
            ((ISupportInitialize) this.dataGridViewAccounts).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void LoadGridViewAccounts()
        {
            if (this.appManager.m_lsAccountItems != null)
            {
                this.dataGridViewAccounts.Rows.Clear();
                foreach (AccountItem item in this.appManager.m_lsAccountItems)
                {
                    string accountItemOldestUnreadDate = this.appManager.GetAccountItemOldestUnreadDate(item);
                    if (item.connectionStatus == "Logged In")
                    {
                        accountItemOldestUnreadDate = "See Message Window";
                    }
                    object[] values = new object[] { this.appManager.FormatPhone(item.number), item.title, item.unReadMessageList.Count<TextMessage>().ToString(), accountItemOldestUnreadDate, item.lastSyncDate, item.connectionStatus };
                    this.dataGridViewAccounts.Rows.Add(values);
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
                foreach (DataGridViewRow row in (IEnumerable) this.dataGridViewAccounts.Rows)
                {
                    if (this.dataGridViewAccounts["colConnectionStatus", row.Index].Value.ToString() == "Logged In")
                    {
                        this.dataGridViewAccounts.Rows[row.Index].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#E8FFCC");
                    }
                }
                this.labelTotalAccounts.Text = "Total Accounts: " + this.appManager.m_lsAccountItems.Count.ToString();
                this.labelTotalUnread.Text = "Total Unread: " + this.appManager.GetTotalDashboardUnread(false).ToString();
                this.DisplayRefreshCount();
            }
        }

        private void openDashboardWithUnreadReminderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "PopDashboardWindow", this.openDashboardWithUnreadReminderToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Open dashboard with unread reminder save error: " + errorMessage;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bPopDashboardWindow = this.openDashboardWithUnreadReminderToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Open dashboard with unread reminder save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 10);
            }
        }

        private void optionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.openDashboardWithUnreadReminderToolStripMenuItem.Checked = this.appManager.m_bPopDashboardWindow;
        }

        private void settingsHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.LaunchWebsite(this.appManager.m_strSettingsURL);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowSettings();
        }

        private void syncFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.m_bNotifyServerSync = true;
            this.appManager.GetServerSettings(true);
        }

        private void tryBETAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdater.ShowUserNoUpdateAvailable = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.LetUserSelectSkip = false;
            AutoUpdater.Start(this.appManager.m_strBETAUpdateFileURL);
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdater.ShowUserNoUpdateAvailable = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.LetUserSelectSkip = false;
            AutoUpdater.Start(this.appManager.m_strUpdateFileURL);
        }

        public ApplicationManager appManager { get; set; }

        private class CustomPopupControl : ToolStripControlHost
        {
            public CustomPopupControl(string title, string message) : base(new Panel())
            {
                if (string.IsNullOrEmpty(message))
                {
                    message = "No messages found...";
                }
                Label label = new Label {
                    BackColor = System.Drawing.Color.LightGray,
                    Text = title,
                    Dock = DockStyle.Top
                };
                Label label2 = new Label {
                    BackColor = ColorTranslator.FromHtml("#E8FFCC"),
                    Text = message,
                    Dock = DockStyle.Fill
                };
                base.Control.MinimumSize = new Size(300, 350);
                base.Control.Controls.Add(label2);
                base.Control.Controls.Add(label);
            }
        }
    }
}


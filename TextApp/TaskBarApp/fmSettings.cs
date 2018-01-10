namespace TaskBarApp
{
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Media;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class fmSettings : Form
    {
        private bool bLoading;
        private bool bRefreshMessageForm;
        private Button buttonHelp;
        private Button buttonSave;
        private DialogResult changeEnableDashbaordMode;
        private CheckBox checkBoxDisableDashboardSettingChangeNotifications;
        private CheckBox checkBoxEnableDashboard;
        private CheckBox checkBoxEnableSignature;
        private ComboBox comboBoxDashboardNotificationSound;
        private ComboBox comboBoxDashboardRefreshInterval;
        private ComboBox comboBoxFontSize;
        private ComboBox comboBoxFontSizeDT;
        private ComboBox comboBoxLastMessageStatus;
        private ComboBox comboBoxNotificationReminder;
        private ComboBox comboBoxNotificationSound;
        private ComboBox comboBoxUnreadMessageLimit;
        private IContainer components;
        private Label labelDashboardNotificationSound;
        private Label labelDashboardRefreshInterval;
        private Label labelFontSize;
        private Label labelFontSizeDT;
        private Label labelLastMessage;
        private Label labelMachineID;
        private Label labelNotificationSound;
        private Label labelReminderInterval;
        private Label labelUnreadMessageLimit;
        private string strError = string.Empty;
        private TextBox textBoxSignature;

        public fmSettings()
        {
            this.InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool flag = true;
            string errorMessage = string.Empty;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref errorMessage);
            try
            {
                AppRegistry.SaveValue(rootKey, "NotificationSound", this.comboBoxNotificationSound.SelectedValue.ToString(), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Notification sound save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strNotificationSound = this.comboBoxNotificationSound.SelectedValue.ToString();
                    if (this.appManager.m_strNotificationSound == "None")
                    {
                        this.appManager.m_bPlaySound = false;
                    }
                    else
                    {
                        this.appManager.m_bPlaySound = true;
                    }
                }
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Notification sound save error: " + exception.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "NotificationInterval", Convert.ToInt32(this.comboBoxNotificationReminder.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Notificaiton reminder save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_nNotificationInterval = Convert.ToInt32(this.comboBoxNotificationReminder.SelectedValue);
                    this.appManager.m_dtNextNotification = DateTime.Now.AddMinutes((double) this.appManager.m_nNotificationInterval);
                }
            }
            catch (Exception exception2)
            {
                this.strError = this.strError + "Notificaiton reminder save error: " + exception2.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "FontSize", Convert.ToInt32(this.comboBoxFontSize.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Font size save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_nFontSize = Convert.ToInt32(this.comboBoxFontSize.SelectedValue);
                    this.appManager.m_fontSize = new Font("Arial", (float) this.appManager.m_nFontSize);
                    this.bRefreshMessageForm = true;
                }
            }
            catch (Exception exception3)
            {
                this.strError = this.strError + "Font size save error: " + exception3.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "FontSizeDT", Convert.ToInt32(this.comboBoxFontSizeDT.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Font size Date/Time save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_nFontSizeDT = Convert.ToInt32(this.comboBoxFontSizeDT.SelectedValue);
                    this.appManager.m_fontSizeDT = new Font("Arial", (float) this.appManager.m_nFontSizeDT);
                    this.bRefreshMessageForm = true;
                }
            }
            catch (Exception exception4)
            {
                this.strError = this.strError + "Font size Date/Time save error: " + exception4.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "LastMessageStatusLimit", Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Last message status save error: " + errorMessage;
                    flag = false;
                }
                else if (this.appManager.m_nLastMessageStatusLimit != Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue))
                {
                    this.appManager.m_nLastMessageStatusLimit = Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue);
                    this.bRefreshMessageForm = true;
                    if (this.appManager.m_nLastMessageStatusLimit == 0)
                    {
                        this.appManager.m_lsConversationMetaData.Clear();
                    }
                    else
                    {
                        this.appManager.LoadConversations(true);
                    }
                }
            }
            catch (Exception exception5)
            {
                this.strError = this.strError + "Last message status save error: " + exception5.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "UnreadMessageLimit", Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "UnreadyMessage Count limit save error: " + errorMessage;
                    flag = false;
                }
                else if (this.appManager.m_nUnreadMessageLimit != Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue))
                {
                    this.appManager.m_nUnreadMessageLimit = Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue);
                }
            }
            catch (Exception exception6)
            {
                this.strError = this.strError + "Unread Message Count limit save error: " + exception6.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "EnableSignature", this.checkBoxEnableSignature.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "Signature", this.textBoxSignature.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Enable Signature save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_bEnableSignature = this.checkBoxEnableSignature.Checked;
                    this.appManager.m_strSignature = this.textBoxSignature.Text;
                    this.bRefreshMessageForm = true;
                }
            }
            catch (Exception exception7)
            {
                this.strError = this.strError + "Enable Signature save error: " + exception7.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "local_AccountDashboardRefreshInterval", Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Dashboard refresh interval save error: " + errorMessage;
                    flag = false;
                }
                else if (this.appManager.m_nAccountDashboardRefreshInterval != Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue))
                {
                    this.appManager.m_nAccountDashboardRefreshInterval = Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue);
                    this.appManager.StartMonitorAccountDashboardTimer();
                }
            }
            catch (Exception exception8)
            {
                this.strError = this.strError + "Dashboard refresh interval save error: " + exception8.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "local_DashboardNotificationSound", this.comboBoxDashboardNotificationSound.SelectedValue.ToString(), ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Dashboard notification sound save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strDashboardNotificationSound = this.comboBoxDashboardNotificationSound.SelectedValue.ToString();
                    if (this.appManager.m_strDashboardNotificationSound == "None")
                    {
                        this.appManager.m_bPlayDashboardSound = false;
                    }
                    else
                    {
                        this.appManager.m_bPlayDashboardSound = true;
                    }
                }
            }
            catch (Exception exception9)
            {
                this.strError = this.strError + "Dashboard notification sound save error: " + exception9.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "local_DisableDashboardSettingChangeNotifications", this.checkBoxDisableDashboardSettingChangeNotifications.Checked, ref this.strError, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Dashboard Settign Change Notification save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_bDisableDashboardSettingChangeNotifications = this.checkBoxDisableDashboardSettingChangeNotifications.Checked;
                }
            }
            catch (Exception exception10)
            {
                this.strError = this.strError + "Dashboard Settign Change Notification save error: " + exception10.Message;
                flag = false;
            }
            if (this.appManager.m_bDashboardMode != this.checkBoxEnableDashboard.Checked)
            {
                this.changeEnableDashbaordMode = MessageBox.Show("You must exit and re-open the " + this.appManager.m_strApplicationName + " application for the change to Dashboard mode to take effect.  Would you like to exit " + this.appManager.m_strApplicationName + " now?", "Multiple Account Mode Change", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (this.changeEnableDashbaordMode == DialogResult.Yes)
                {
                    try
                    {
                        AppRegistry.SaveValue(rootKey, "local_DashboardMode", this.checkBoxEnableDashboard.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                        if (errorMessage != string.Empty)
                        {
                            this.strError = this.strError + "Enable Dashboard save error: " + errorMessage;
                            flag = false;
                        }
                        else
                        {
                            AppRegistry.SaveUserName(rootKey, "", ref errorMessage);
                            if (errorMessage != string.Empty)
                            {
                                this.strError = this.strError + "\nUserName Save Error: " + errorMessage;
                            }
                            AppRegistry.SavePassword(rootKey, "", ref errorMessage);
                            if (errorMessage != string.Empty)
                            {
                                this.strError = this.strError + "\nPassword Save Error: " + errorMessage;
                            }
                        }
                    }
                    catch (Exception exception11)
                    {
                        this.strError = this.strError + "Enable Dashboard save error: " + exception11.Message;
                        flag = false;
                    }
                }
            }
            if (flag)
            {
                if (this.changeEnableDashbaordMode == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else if (this.changeEnableDashbaordMode == DialogResult.No)
                {
                    this.checkBoxEnableDashboard.Checked = this.appManager.m_bDashboardMode;
                }
                else
                {
                    base.Close();
                    if ((this.appManager.formMessages != null) && this.bRefreshMessageForm)
                    {
                        this.appManager.formMessages.Close();
                        this.appManager.ShowMessages();
                    }
                    this.appManager.ShowBalloon("Your settings have been saved.", 5);
                }
            }
            else
            {
                this.appManager.ShowBalloon("Exception saving settings: " + this.strError, 5);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            this.appManager.LaunchWebsite(this.appManager.m_strSettingsURL);
        }

        private void checkBoxSignature_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxEnableSignature.Checked)
            {
                this.textBoxSignature.Enabled = true;
            }
            else
            {
                this.textBoxSignature.Enabled = false;
            }
        }

        private void comboBoxDashboardNotificationSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.bLoading && (this.comboBoxDashboardNotificationSound.SelectedValue.ToString() != "None"))
            {
                new SoundPlayer(this.appManager.m_strSoundPath + this.comboBoxDashboardNotificationSound.SelectedValue.ToString()).Play();
            }
        }

        private void comboBoxNotificationSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.bLoading && (this.comboBoxNotificationSound.SelectedValue.ToString() != "None"))
            {
                new SoundPlayer(this.appManager.m_strSoundPath + this.comboBoxNotificationSound.SelectedValue.ToString()).Play();
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

        private void fmSettings_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = this.appManager.m_strApplicationName + " Settings";
                base.Icon = this.appManager.iTextApp;
                this.checkBoxEnableSignature.Checked = this.appManager.m_bEnableSignature;
                this.textBoxSignature.Enabled = this.appManager.m_bEnableSignature;
                this.textBoxSignature.Text = this.appManager.m_strSignature;
                this.checkBoxEnableDashboard.Checked = this.appManager.m_bDashboardMode;
                this.checkBoxDisableDashboardSettingChangeNotifications.Checked = this.appManager.m_bDisableDashboardSettingChangeNotifications;
                this.labelMachineID.Text = "Machine ID: " + this.appManager.m_strMachineID;
                if (this.appManager.m_bDashboardFeature)
                {
                    this.checkBoxEnableDashboard.Enabled = true;
                }
                else
                {
                    this.checkBoxEnableDashboard.Enabled = false;
                }
                if (this.appManager.m_bDashboardMode)
                {
                    base.Height = this.MaximumSize.Height;
                    this.labelDashboardNotificationSound.Visible = true;
                    this.labelDashboardRefreshInterval.Visible = true;
                    this.comboBoxDashboardRefreshInterval.Visible = true;
                    this.comboBoxDashboardNotificationSound.Visible = true;
                    this.checkBoxDisableDashboardSettingChangeNotifications.Visible = true;
                }
                else
                {
                    base.Height = this.MinimumSize.Height;
                    this.labelDashboardNotificationSound.Visible = false;
                    this.labelDashboardRefreshInterval.Visible = false;
                    this.comboBoxDashboardRefreshInterval.Visible = false;
                    this.comboBoxDashboardNotificationSound.Visible = false;
                    this.checkBoxDisableDashboardSettingChangeNotifications.Visible = false;
                }
                this.LoadNotificationReminderIntervalDDL();
                this.LoadDashbaordNotificationSoundlDDL();
                this.LoadDashboardRefreshIntervalDDL();
                this.LoadLastMessageStatusLimitDDL();
                this.LoadNotificationSoundlDDL();
                this.LoadUnreadMessageLimitDDL();
                this.LoadFontSizeDDL();
                this.LoadFontSizeDTDDL();
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception creating settings form: " + exception.Message, 5);
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmSettings));
            this.buttonSave = new Button();
            this.comboBoxNotificationReminder = new ComboBox();
            this.labelReminderInterval = new Label();
            this.textBoxSignature = new TextBox();
            this.checkBoxEnableSignature = new CheckBox();
            this.labelDashboardRefreshInterval = new Label();
            this.comboBoxDashboardRefreshInterval = new ComboBox();
            this.checkBoxEnableDashboard = new CheckBox();
            this.labelLastMessage = new Label();
            this.comboBoxLastMessageStatus = new ComboBox();
            this.buttonHelp = new Button();
            this.labelNotificationSound = new Label();
            this.comboBoxNotificationSound = new ComboBox();
            this.labelUnreadMessageLimit = new Label();
            this.comboBoxUnreadMessageLimit = new ComboBox();
            this.comboBoxDashboardNotificationSound = new ComboBox();
            this.labelDashboardNotificationSound = new Label();
            this.labelMachineID = new Label();
            this.checkBoxDisableDashboardSettingChangeNotifications = new CheckBox();
            this.labelFontSize = new Label();
            this.comboBoxFontSize = new ComboBox();
            this.labelFontSizeDT = new Label();
            this.comboBoxFontSizeDT = new ComboBox();
            base.SuspendLayout();
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x175, 0x207);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x45, 0x1b);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.btnSave_Click);
            this.comboBoxNotificationReminder.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxNotificationReminder.FlatStyle = FlatStyle.Flat;
            this.comboBoxNotificationReminder.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxNotificationReminder.FormattingEnabled = true;
            this.comboBoxNotificationReminder.Location = new Point(0x160, 0x2c);
            this.comboBoxNotificationReminder.Name = "comboBoxNotificationReminder";
            this.comboBoxNotificationReminder.Size = new Size(90, 0x19);
            this.comboBoxNotificationReminder.TabIndex = 7;
            this.labelReminderInterval.AutoSize = true;
            this.labelReminderInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelReminderInterval.Location = new Point(0x16, 0x2f);
            this.labelReminderInterval.Name = "labelReminderInterval";
            this.labelReminderInterval.Size = new Size(0x13b, 0x11);
            this.labelReminderInterval.TabIndex = 8;
            this.labelReminderInterval.Text = "Unread Message Notification Reminder Interval:";
            this.textBoxSignature.Location = new Point(0x19, 0x123);
            this.textBoxSignature.Multiline = true;
            this.textBoxSignature.Name = "textBoxSignature";
            this.textBoxSignature.Size = new Size(0x1a1, 0x4c);
            this.textBoxSignature.TabIndex = 11;
            this.checkBoxEnableSignature.AutoSize = true;
            this.checkBoxEnableSignature.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxEnableSignature.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxEnableSignature.Location = new Point(0x1a, 0x10d);
            this.checkBoxEnableSignature.Name = "checkBoxEnableSignature";
            this.checkBoxEnableSignature.Size = new Size(0xc9, 0x15);
            this.checkBoxEnableSignature.TabIndex = 13;
            this.checkBoxEnableSignature.Text = "Enable Message Signature";
            this.checkBoxEnableSignature.UseVisualStyleBackColor = false;
            this.checkBoxEnableSignature.CheckedChanged += new EventHandler(this.checkBoxSignature_CheckedChanged);
            this.labelDashboardRefreshInterval.AutoSize = true;
            this.labelDashboardRefreshInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelDashboardRefreshInterval.Location = new Point(0x15, 0x19c);
            this.labelDashboardRefreshInterval.Name = "labelDashboardRefreshInterval";
            this.labelDashboardRefreshInterval.Size = new Size(0xe2, 0x11);
            this.labelDashboardRefreshInterval.TabIndex = 0x12;
            this.labelDashboardRefreshInterval.Text = "Dashboard Refresh Time Interval:";
            this.comboBoxDashboardRefreshInterval.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxDashboardRefreshInterval.FlatStyle = FlatStyle.Flat;
            this.comboBoxDashboardRefreshInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxDashboardRefreshInterval.FormattingEnabled = true;
            this.comboBoxDashboardRefreshInterval.Location = new Point(0x113, 0x198);
            this.comboBoxDashboardRefreshInterval.Name = "comboBoxDashboardRefreshInterval";
            this.comboBoxDashboardRefreshInterval.Size = new Size(0xa7, 0x19);
            this.comboBoxDashboardRefreshInterval.TabIndex = 0x11;
            this.checkBoxEnableDashboard.AutoSize = true;
            this.checkBoxEnableDashboard.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxEnableDashboard.Checked = true;
            this.checkBoxEnableDashboard.CheckState = CheckState.Checked;
            this.checkBoxEnableDashboard.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxEnableDashboard.Location = new Point(0x1a, 0x178);
            this.checkBoxEnableDashboard.Name = "checkBoxEnableDashboard";
            this.checkBoxEnableDashboard.Size = new Size(0xbb, 0x15);
            this.checkBoxEnableDashboard.TabIndex = 0x15;
            this.checkBoxEnableDashboard.Text = "Enable Dashboard Mode";
            this.checkBoxEnableDashboard.UseVisualStyleBackColor = false;
            this.labelLastMessage.AutoSize = true;
            this.labelLastMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelLastMessage.Location = new Point(0x16, 0x79);
            this.labelLastMessage.Name = "labelLastMessage";
            this.labelLastMessage.Size = new Size(0xb8, 0x11);
            this.labelLastMessage.TabIndex = 0x1a;
            this.labelLastMessage.Text = "Last Message Status Limit:";
            this.comboBoxLastMessageStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxLastMessageStatus.FlatStyle = FlatStyle.Flat;
            this.comboBoxLastMessageStatus.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxLastMessageStatus.FormattingEnabled = true;
            this.comboBoxLastMessageStatus.Location = new Point(0x116, 0x76);
            this.comboBoxLastMessageStatus.Name = "comboBoxLastMessageStatus";
            this.comboBoxLastMessageStatus.Size = new Size(0xa4, 0x19);
            this.comboBoxLastMessageStatus.TabIndex = 0x19;
            this.buttonHelp.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonHelp.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonHelp.Location = new Point(0x116, 0x207);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new Size(0x4a, 0x1b);
            this.buttonHelp.TabIndex = 0x1d;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new EventHandler(this.buttonHelp_Click);
            this.labelNotificationSound.AutoSize = true;
            this.labelNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelNotificationSound.Location = new Point(0x17, 0x54);
            this.labelNotificationSound.Name = "labelNotificationSound";
            this.labelNotificationSound.Size = new Size(0xc0, 0x11);
            this.labelNotificationSound.TabIndex = 30;
            this.labelNotificationSound.Text = "Message Notification Sound:";
            this.comboBoxNotificationSound.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxNotificationSound.FlatStyle = FlatStyle.Flat;
            this.comboBoxNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxNotificationSound.FormattingEnabled = true;
            this.comboBoxNotificationSound.Location = new Point(0x116, 0x51);
            this.comboBoxNotificationSound.Name = "comboBoxNotificationSound";
            this.comboBoxNotificationSound.Size = new Size(0xa4, 0x19);
            this.comboBoxNotificationSound.TabIndex = 0x1f;
            this.comboBoxNotificationSound.SelectedIndexChanged += new EventHandler(this.comboBoxNotificationSound_SelectedIndexChanged);
            this.labelUnreadMessageLimit.AutoSize = true;
            this.labelUnreadMessageLimit.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelUnreadMessageLimit.Location = new Point(0x16, 160);
            this.labelUnreadMessageLimit.Name = "labelUnreadMessageLimit";
            this.labelUnreadMessageLimit.Size = new Size(0xcb, 0x11);
            this.labelUnreadMessageLimit.TabIndex = 0x21;
            this.labelUnreadMessageLimit.Text = "Unread Message Check Limit:";
            this.comboBoxUnreadMessageLimit.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxUnreadMessageLimit.FlatStyle = FlatStyle.Flat;
            this.comboBoxUnreadMessageLimit.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxUnreadMessageLimit.FormattingEnabled = true;
            this.comboBoxUnreadMessageLimit.Location = new Point(0x116, 0x9d);
            this.comboBoxUnreadMessageLimit.Name = "comboBoxUnreadMessageLimit";
            this.comboBoxUnreadMessageLimit.Size = new Size(0xa4, 0x19);
            this.comboBoxUnreadMessageLimit.TabIndex = 0x20;
            this.comboBoxDashboardNotificationSound.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxDashboardNotificationSound.FlatStyle = FlatStyle.Flat;
            this.comboBoxDashboardNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxDashboardNotificationSound.FormattingEnabled = true;
            this.comboBoxDashboardNotificationSound.Location = new Point(0x113, 0x1bd);
            this.comboBoxDashboardNotificationSound.Name = "comboBoxDashboardNotificationSound";
            this.comboBoxDashboardNotificationSound.Size = new Size(0xa7, 0x19);
            this.comboBoxDashboardNotificationSound.TabIndex = 0x23;
            this.comboBoxDashboardNotificationSound.SelectedIndexChanged += new EventHandler(this.comboBoxDashboardNotificationSound_SelectedIndexChanged);
            this.labelDashboardNotificationSound.AutoSize = true;
            this.labelDashboardNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelDashboardNotificationSound.Location = new Point(0x16, 0x1c0);
            this.labelDashboardNotificationSound.Name = "labelDashboardNotificationSound";
            this.labelDashboardNotificationSound.Size = new Size(0xcd, 0x11);
            this.labelDashboardNotificationSound.TabIndex = 0x22;
            this.labelDashboardNotificationSound.Text = "Dashboard Notification Sound:";
            this.labelMachineID.AutoSize = true;
            this.labelMachineID.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMachineID.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelMachineID.Location = new Point(0x17, 12);
            this.labelMachineID.Name = "labelMachineID";
            this.labelMachineID.Size = new Size(80, 0x11);
            this.labelMachineID.TabIndex = 0x24;
            this.labelMachineID.Text = "MachineID:";
            this.checkBoxDisableDashboardSettingChangeNotifications.AutoSize = true;
            this.checkBoxDisableDashboardSettingChangeNotifications.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxDisableDashboardSettingChangeNotifications.Checked = true;
            this.checkBoxDisableDashboardSettingChangeNotifications.CheckState = CheckState.Checked;
            this.checkBoxDisableDashboardSettingChangeNotifications.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxDisableDashboardSettingChangeNotifications.Location = new Point(0x19, 0x1e7);
            this.checkBoxDisableDashboardSettingChangeNotifications.Name = "checkBoxDisableDashboardSettingChangeNotifications";
            this.checkBoxDisableDashboardSettingChangeNotifications.Size = new Size(0x184, 0x15);
            this.checkBoxDisableDashboardSettingChangeNotifications.TabIndex = 0x25;
            this.checkBoxDisableDashboardSettingChangeNotifications.Text = "Disable Setting Change Notifications Between Accounts";
            this.checkBoxDisableDashboardSettingChangeNotifications.UseVisualStyleBackColor = false;
            this.labelFontSize.AutoSize = true;
            this.labelFontSize.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelFontSize.Location = new Point(0x15, 0xc7);
            this.labelFontSize.Name = "labelFontSize";
            this.labelFontSize.Size = new Size(0xa3, 0x11);
            this.labelFontSize.TabIndex = 0x27;
            this.labelFontSize.Text = "Text Message Font Size";
            this.comboBoxFontSize.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxFontSize.FlatStyle = FlatStyle.Flat;
            this.comboBoxFontSize.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxFontSize.FormattingEnabled = true;
            this.comboBoxFontSize.Location = new Point(0x115, 0xc4);
            this.comboBoxFontSize.Name = "comboBoxFontSize";
            this.comboBoxFontSize.Size = new Size(0xa4, 0x19);
            this.comboBoxFontSize.TabIndex = 0x26;
            this.labelFontSizeDT.AutoSize = true;
            this.labelFontSizeDT.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelFontSizeDT.Location = new Point(0x15, 0xee);
            this.labelFontSizeDT.Name = "labelFontSizeDT";
            this.labelFontSizeDT.Size = new Size(0xab, 0x11);
            this.labelFontSizeDT.TabIndex = 0x29;
            this.labelFontSizeDT.Text = "Text Date/Time Font Size";
            this.comboBoxFontSizeDT.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxFontSizeDT.FlatStyle = FlatStyle.Flat;
            this.comboBoxFontSizeDT.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxFontSizeDT.FormattingEnabled = true;
            this.comboBoxFontSizeDT.Location = new Point(0x115, 0xeb);
            this.comboBoxFontSizeDT.Name = "comboBoxFontSizeDT";
            this.comboBoxFontSizeDT.Size = new Size(0xa4, 0x19);
            this.comboBoxFontSizeDT.TabIndex = 40;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1cb, 0x231);
            base.Controls.Add(this.labelFontSizeDT);
            base.Controls.Add(this.comboBoxFontSizeDT);
            base.Controls.Add(this.labelFontSize);
            base.Controls.Add(this.comboBoxFontSize);
            base.Controls.Add(this.checkBoxDisableDashboardSettingChangeNotifications);
            base.Controls.Add(this.labelMachineID);
            base.Controls.Add(this.comboBoxDashboardNotificationSound);
            base.Controls.Add(this.labelDashboardNotificationSound);
            base.Controls.Add(this.labelUnreadMessageLimit);
            base.Controls.Add(this.comboBoxUnreadMessageLimit);
            base.Controls.Add(this.comboBoxNotificationSound);
            base.Controls.Add(this.labelNotificationSound);
            base.Controls.Add(this.buttonHelp);
            base.Controls.Add(this.labelLastMessage);
            base.Controls.Add(this.comboBoxLastMessageStatus);
            base.Controls.Add(this.checkBoxEnableDashboard);
            base.Controls.Add(this.labelDashboardRefreshInterval);
            base.Controls.Add(this.comboBoxDashboardRefreshInterval);
            base.Controls.Add(this.checkBoxEnableSignature);
            base.Controls.Add(this.textBoxSignature);
            base.Controls.Add(this.labelReminderInterval);
            base.Controls.Add(this.comboBoxNotificationReminder);
            base.Controls.Add(this.buttonSave);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            this.MaximumSize = new Size(0x1db, 600);
            base.MinimizeBox = false;
            this.MinimumSize = new Size(0x1c7, 500);
            base.Name = "fmSettings";
            this.Text = "Settings";
            base.Load += new EventHandler(this.fmSettings_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadDashbaordNotificationSoundlDDL()
        {
            this.bLoading = true;
            this.comboBoxDashboardNotificationSound.DisplayMember = "Text";
            this.comboBoxDashboardNotificationSound.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "None",
                Value = "None"
            }, new { 
                Text = "Bubble",
                Value = "Bubble.wav"
            }, new { 
                Text = "Doorbell",
                Value = "DoorBell.wav"
            }, new { 
                Text = "Electric Dog",
                Value = "ElectricDog.wav"
            }, new { 
                Text = "String",
                Value = "String.wav"
            }, new { 
                Text = "Water Drop",
                Value = "WaterDrop.wav"
            } };
            this.comboBoxDashboardNotificationSound.DataSource = typeArray;
            this.comboBoxDashboardNotificationSound.SelectedValue = this.appManager.m_strDashboardNotificationSound;
            this.bLoading = false;
        }

        private void LoadDashboardRefreshIntervalDDL()
        {
            this.comboBoxDashboardRefreshInterval.DisplayMember = "Text";
            this.comboBoxDashboardRefreshInterval.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "1",
                Value = "1"
            }, new { 
                Text = "5",
                Value = "5"
            }, new { 
                Text = "10",
                Value = "10"
            }, new { 
                Text = "30",
                Value = "30"
            }, new { 
                Text = "60",
                Value = "60"
            } };
            this.comboBoxDashboardRefreshInterval.DataSource = typeArray;
            this.comboBoxDashboardRefreshInterval.SelectedIndex = this.comboBoxDashboardRefreshInterval.FindString(this.appManager.m_nAccountDashboardRefreshInterval.ToString());
        }

        private void LoadFontSizeDDL()
        {
            this.comboBoxFontSize.DisplayMember = "Text";
            this.comboBoxFontSize.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "6",
                Value = "6"
            }, new { 
                Text = "8",
                Value = "8"
            }, new { 
                Text = "10",
                Value = "10"
            }, new { 
                Text = "12",
                Value = "12"
            }, new { 
                Text = "14",
                Value = "14"
            }, new { 
                Text = "16",
                Value = "16"
            }, new { 
                Text = "18",
                Value = "18"
            } };
            this.comboBoxFontSize.DataSource = typeArray;
            this.comboBoxFontSize.SelectedIndex = this.comboBoxFontSize.FindString(this.appManager.m_nFontSize.ToString());
        }

        private void LoadFontSizeDTDDL()
        {
            this.comboBoxFontSizeDT.DisplayMember = "Text";
            this.comboBoxFontSizeDT.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "6",
                Value = "6"
            }, new { 
                Text = "8",
                Value = "8"
            }, new { 
                Text = "10",
                Value = "10"
            }, new { 
                Text = "12",
                Value = "12"
            }, new { 
                Text = "14",
                Value = "14"
            }, new { 
                Text = "16",
                Value = "16"
            }, new { 
                Text = "18",
                Value = "18"
            } };
            this.comboBoxFontSizeDT.DataSource = typeArray;
            this.comboBoxFontSizeDT.SelectedIndex = this.comboBoxFontSizeDT.FindString(this.appManager.m_nFontSizeDT.ToString());
        }

        private void LoadLastMessageStatusLimitDDL()
        {
            this.comboBoxLastMessageStatus.DisplayMember = "Text";
            this.comboBoxLastMessageStatus.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "None",
                Value = "0"
            }, new { 
                Text = "250",
                Value = "250"
            }, new { 
                Text = "500",
                Value = "500"
            }, new { 
                Text = "999",
                Value = "999"
            } };
            this.comboBoxLastMessageStatus.DataSource = typeArray;
            if (this.appManager.m_nLastMessageStatusLimit == 0)
            {
                this.comboBoxLastMessageStatus.SelectedIndex = this.comboBoxLastMessageStatus.FindString("None");
            }
            else
            {
                this.comboBoxLastMessageStatus.SelectedIndex = this.comboBoxLastMessageStatus.FindString(this.appManager.m_nLastMessageStatusLimit.ToString());
            }
        }

        private void LoadNotificationReminderIntervalDDL()
        {
            this.comboBoxNotificationReminder.DisplayMember = "Text";
            this.comboBoxNotificationReminder.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "5 min",
                Value = "5"
            }, new { 
                Text = "15 min",
                Value = "15"
            }, new { 
                Text = "30 min",
                Value = "30"
            }, new { 
                Text = "60 min",
                Value = "60"
            } };
            this.comboBoxNotificationReminder.DataSource = typeArray;
            this.comboBoxNotificationReminder.SelectedIndex = this.comboBoxNotificationReminder.FindString(this.appManager.m_nNotificationInterval.ToString());
        }

        private void LoadNotificationSoundlDDL()
        {
            this.bLoading = true;
            this.comboBoxNotificationSound.DisplayMember = "Text";
            this.comboBoxNotificationSound.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "None",
                Value = "None"
            }, new { 
                Text = "Bubble",
                Value = "Bubble.wav"
            }, new { 
                Text = "Doorbell",
                Value = "DoorBell.wav"
            }, new { 
                Text = "Electric Dog",
                Value = "ElectricDog.wav"
            }, new { 
                Text = "String",
                Value = "String.wav"
            }, new { 
                Text = "Water Drop",
                Value = "WaterDrop.wav"
            } };
            this.comboBoxNotificationSound.DataSource = typeArray;
            this.comboBoxNotificationSound.SelectedValue = this.appManager.m_strNotificationSound;
            this.bLoading = false;
        }

        private void LoadUnreadMessageLimitDDL()
        {
            this.comboBoxUnreadMessageLimit.DisplayMember = "Text";
            this.comboBoxUnreadMessageLimit.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "250",
                Value = "250"
            }, new { 
                Text = "500",
                Value = "500"
            }, new { 
                Text = "999",
                Value = "999"
            } };
            this.comboBoxUnreadMessageLimit.DataSource = typeArray;
            this.comboBoxUnreadMessageLimit.SelectedIndex = this.comboBoxUnreadMessageLimit.FindString(this.appManager.m_nUnreadMessageLimit.ToString());
        }

        public ApplicationManager appManager { get; set; }
    }
}


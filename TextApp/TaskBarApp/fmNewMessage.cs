namespace TaskBarApp
{
    using AutoUpdaterDotNET;
    using Keyoti.RapidSpell;
    using Keyoti.RapidSpell.Options;
    using Microsoft.Win32;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;
    using TaskBarApp.Properties;

    public class fmNewMessage : Form
    {
        private bool bAddComboBoxContactList;
        private bool bAddContactOnLeave;
        private Button buttonSend;
        private ComboBox comboBoxContactList;
        private IContainer components;
        private ToolStripMenuItem controlEnterToolStripMenuItem;
        private DateTimePicker dateTimePickerScheduleDate;
        private ToolStripMenuItem editGroupScheduleToolStripMenuItem;
        private ToolStripMenuItem editKeywordAutoResponseToolStripMenuItem;
        private ToolStripMenuItem editMessageTemplatesToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem generalHelpToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private Label label1;
        private Label labelAttRemove;
        private Label labelClickRemove;
        private Label labelCount;
        private Label lblCharCount;
        private LinkLabel linkLabelRemoveAll;
        private ListBox listBoxSelectedContacts;
        private ToolStripMenuItem logOutToolStripMenuItem;
        private ToolStripMenuItem manageContactsToolStripMenuItem;
        private ToolStripMenuItem manageGroupsToolStripMenuItem;
        private MenuStrip menuStripNewMessage;
        private ToolStripMenuItem messageTemplatesToolStripMenuItem;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private PictureBox pictureBoxAttachment;
        private PictureBox pictureBoxLink;
        private RapidSpellAsYouType rapidSpellAsYouTypeNewMessage;
        private ToolStripMenuItem settingsHelpToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem1;
        private string strError = string.Empty;
        private ToolStripMenuItem syncFeaturesToolStripMenuItem;
        private RichTextBox textBoxNewMessage;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem tryBETAToolStripMenuItem;
        private ToolStripMenuItem validatePhoneNumbersToolStripMenuItem;
        private ToolStripMenuItem versionToolStripMenuItem;

        public fmNewMessage()
        {
            this.InitializeComponent();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            Predicate<Conversation> <>9__0;
            Predicate<ConversationMetaData> <>9__1;
            this.strError = null;
            bool flag = false;
            int count = this.listBoxSelectedContacts.Items.Count;
            IRestResponse<TextMessageSendResponse> response = null;
            IRestResponse<MMSSendResponse> response2 = null;
            string updateConversationFingerprint = string.Empty;
            if (this.listBoxSelectedContacts.Items.Count == 0)
            {
                this.buttonSend.Enabled = true;
            }
            else if ((this.textBoxNewMessage.Text.Length == 0) || (this.textBoxNewMessage.Text.Trim() == this.appManager.m_strSignature.Trim()))
            {
                MessageBox.Show("Please enter a message.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    this.buttonSend.Enabled = false;
                    string address = string.Empty;
                    string filePath = string.Empty;
                    DateTime scheduleDate = this.dateTimePickerScheduleDate.Value;
                    if (count > 1)
                    {
                        if (this.appManager.m_bGroupSend)
                        {
                            this.appManager.ShowBalloon("Please wait until current group send is complete...", 5);
                        }
                        else
                        {
                            List<string> groupSend = new List<string>();
                            this.appManager.ShowBalloon("Your group message to " + count.ToString() + " recipients will be sent in the background...", 5);
                            for (int j = 0; j < count; j++)
                            {
                                MessageRecipient recipient = (MessageRecipient) this.listBoxSelectedContacts.Items[j];
                                groupSend.Add(recipient.address);
                            }
                            if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
                            {
                                filePath = this.pictureBoxAttachment.ImageLocation;
                            }
                            this.appManager.SendGroupMessage(groupSend, this.textBoxNewMessage.Text, scheduleDate, filePath);
                            this.buttonSend.Enabled = true;
                            base.Close();
                            if (this.appManager.formMessages == null)
                            {
                                this.appManager.ShowMessages();
                            }
                        }
                        return;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        MessageRecipient recipient2 = (MessageRecipient) this.listBoxSelectedContacts.Items[i];
                        address = recipient2.address;
                        if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
                        {
                            response2 = this.appManager.m_textService.SendMessageMMS(this.textBoxNewMessage.Text, this.appManager.FormatContactAddress(address, true, true), this.appManager.m_strSession, this.pictureBoxAttachment.ImageLocation);
                            if (!string.IsNullOrEmpty(response2.ErrorMessage))
                            {
                                this.strError = "Error calling MMS messaging/send: " + response2.ErrorMessage;
                            }
                            else if (!response2.Data.success)
                            {
                                this.strError = "Error from MMS messaging/send...";
                            }
                        }
                        else
                        {
                            response = this.appManager.m_textService.SendMessage(this.textBoxNewMessage.Text, address, this.appManager.m_strSession, scheduleDate);
                            if (!string.IsNullOrEmpty(response.ErrorMessage))
                            {
                                this.strError = "Error calling message/send for " + address + ": " + response.ErrorMessage;
                            }
                            else if (response.Data.success)
                            {
                                updateConversationFingerprint = response.Data.response.fingerprint;
                            }
                            else
                            {
                                this.strError = "Error from message/send for: " + address;
                            }
                        }
                        if (string.IsNullOrEmpty(this.strError))
                        {
                            Conversation conversation = this.appManager.m_lsConversation.Find(<>9__0 ?? (<>9__0 = p => p.fingerprint == updateConversationFingerprint));
                            if (conversation != null)
                            {
                                DateTime now = DateTime.Now;
                                conversation.lastMessageDate = now.ToString("s");
                                ConversationMetaData updateConversationMetaData = this.appManager.m_lsConversationMetaData.Find(<>9__1 ?? (<>9__1 = var => var.fingerprint == updateConversationFingerprint));
                                if (updateConversationMetaData != null)
                                {
                                    updateConversationMetaData.lastMessageDirection = "Out";
                                    updateConversationMetaData.lastMessageDate = new DateTime?(now);
                                    updateConversationMetaData.lastMessageIsError = false;
                                    this.appManager.m_lsConversationMetaData = (from val in this.appManager.m_lsConversationMetaData
                                        where val.fingerprint != updateConversationMetaData.fingerprint
                                        select val).ToList<ConversationMetaData>();
                                    this.appManager.m_lsConversationMetaData.Add(updateConversationMetaData);
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Error sending text: " + exception.Message;
                }
                if (!string.IsNullOrEmpty(this.strError))
                {
                    this.appManager.ShowBalloon(this.strError, 5);
                    this.strError = string.Empty;
                }
                else
                {
                    base.Close();
                    if (flag)
                    {
                        this.appManager.LoadContacts(false);
                        if (!this.appManager.m_bConversationCountLocked)
                        {
                            this.appManager.m_nConversationLimit = this.appManager.m_nConversationLimitDefault;
                        }
                        this.appManager.LoadConversations(false);
                    }
                    if (this.appManager.formMessages == null)
                    {
                        this.appManager.ShowMessages();
                    }
                    this.appManager.formMessages.DisplayConversatoinList();
                    if (!string.IsNullOrEmpty(updateConversationFingerprint))
                    {
                        this.appManager.formMessages.DisplayConversation(updateConversationFingerprint, false, true);
                    }
                }
                this.buttonSend.Enabled = true;
            }
        }

        private void comboBoxContactList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Contains(e.KeyChar.ToString()))
            {
                this.comboBoxContactList_Load(this.comboBoxContactList.Text + e.KeyChar.ToString());
                this.comboBoxContactList.Select(this.comboBoxContactList.Text.Length + 1, 0);
                e.Handled = true;
            }
            else if (e.KeyChar == '\b')
            {
                if (this.comboBoxContactList.Text.Length > 0)
                {
                    this.comboBoxContactList_Load(this.comboBoxContactList.Text.Substring(0, this.comboBoxContactList.Text.Length - 1));
                    this.comboBoxContactList.Select(this.comboBoxContactList.Text.Length + 1, 0);
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                this.bAddComboBoxContactList = true;
            }
            this.comboBoxContactList.DroppedDown = true;
        }

        private void comboBoxContactList_Leave(object sender, EventArgs e)
        {
            this.bAddContactOnLeave = true;
            if (this.comboBoxContactList.Text.Length > 0)
            {
                this.listBoxSelectedContacts_Add();
            }
            this.bAddContactOnLeave = false;
        }

        public void comboBoxContactList_Load(string match)
        {
            List<MessageRecipient> list = new List<MessageRecipient>();
            List<MessageRecipient> collection = new List<MessageRecipient>();
            MessageRecipient item = new MessageRecipient();
            MessageRecipient recipient2 = new MessageRecipient();
            if (match.Contains("#"))
            {
                item.address = match;
            }
            else
            {
                item.address = this.appManager.FormatContactAddress(match, false, false);
            }
            item.contact = match;
            list.Add(item);
            if (match.Contains("#"))
            {
                item.address = match;
                foreach (string str in this.appManager.m_lsGroupTags)
                {
                    recipient2.contact = str;
                    recipient2.address = str;
                    if (string.IsNullOrEmpty(match))
                    {
                        collection.Add(recipient2);
                    }
                    else if (recipient2.contact.ToLower().Contains(match.ToLower()))
                    {
                        collection.Add(recipient2);
                    }
                }
            }
            foreach (Contact contact in this.appManager.m_lsContact)
            {
                item = new MessageRecipient();
                string str2 = contact.firstName + " " + contact.lastName;
                if (str2.Trim().Length == 0)
                {
                    str2 = "Unnamed";
                }
                else
                {
                    str2 = str2.Trim();
                }
                item.contact = str2 + " " + this.appManager.FormatPhone(contact.mobileNumber);
                item.address = contact.address;
                if (!this.listBoxSelectedContacts.Items.Contains(item))
                {
                    if (string.IsNullOrEmpty(match))
                    {
                        collection.Add(item);
                    }
                    else
                    {
                        string str3 = this.appManager.FormatAlphaNumeric(match);
                        if (item.contact.ToLower().Contains(match.ToLower()))
                        {
                            collection.Add(item);
                        }
                        else if ((str3 != "") && item.address.Contains(str3))
                        {
                            collection.Add(item);
                        }
                    }
                }
            }
            collection = (from c in collection
                orderby c.contact
                select c).ToList<MessageRecipient>();
            list.AddRange(collection);
            this.comboBoxContactList.DataSource = list;
        }

        private void comboBoxContactList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxContactList.Text.Length > 0)
            {
                this.comboBoxContactList.DroppedDown = true;
            }
        }

        private void comboBoxContactList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.comboBoxContactList.DroppedDown && !this.bAddContactOnLeave)
            {
                this.listBoxSelectedContacts_Add();
            }
        }

        private void comboBoxContactList_TextChanged(object sender, EventArgs e)
        {
            if (this.bAddComboBoxContactList)
            {
                this.comboBoxContactList_Load(this.comboBoxContactList.Text);
                this.comboBoxContactList.Select(this.comboBoxContactList.Text.Length + 1, 0);
                this.bAddComboBoxContactList = false;
            }
        }

        private bool comboBoxContactList_Validate(string Text) => 
            this.appManager.IsValidMobileNumber(Text);

        private void controlEnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref this.strError), "ControlEnter", this.controlEnterToolStripMenuItem.Checked, ref this.strError, false, RegistryValueKind.Unknown);
                if (this.strError != string.Empty)
                {
                    this.strError = this.strError + "Control Enter save error: " + this.strError;
                    this.appManager.ShowBalloon(this.strError, 5);
                }
                else
                {
                    this.appManager.m_bControlEnter = this.controlEnterToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Control Enter save error: " + exception.Message;
                this.appManager.ShowBalloon(this.strError, 5);
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

        private void editGroupScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowGroupSchedule();
        }

        private void editMessageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowMessageTemplate();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit " + this.appManager.m_strApplicationName + "? Incoming messages will not be displayed.", this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void fmNewMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.appManager.m_strForwardMessage = string.Empty;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormNewMessageWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormNewMessageHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmNewMessage_Load(object sender, EventArgs e)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormNewMessageWidth", ref num, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormNewMessageHeight", ref num2, ref this.strError);
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
                this.textBoxNewMessage.Font = this.appManager.m_fontSize;
                if (this.appManager.m_bKeywordFeature)
                {
                    this.editKeywordAutoResponseToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.editKeywordAutoResponseToolStripMenuItem.Enabled = false;
                }
                if (this.appManager.m_bGroupScheduleFeature)
                {
                    this.editGroupScheduleToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.editGroupScheduleToolStripMenuItem.Enabled = false;
                }
                if (this.appManager.m_bMessageTemplateFeature)
                {
                    this.messageTemplatesToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.messageTemplatesToolStripMenuItem.Enabled = false;
                }
                try
                {
                    this.rapidSpellAsYouTypeNewMessage.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                }
                catch (Exception)
                {
                }
                this.listBoxSelectedContacts.DrawMode = DrawMode.OwnerDrawVariable;
                this.listBoxSelectedContacts.MeasureItem += new MeasureItemEventHandler(this.listBoxSelectedContacts_MeasureItem);
                this.listBoxSelectedContacts.DrawItem += new DrawItemEventHandler(this.listBoxSelectedContacts_DrawItem);
                this.listBoxSelectedContacts.ValueMember = "address";
                this.listBoxSelectedContacts.DisplayMember = "contact";
                this.comboBoxContactList.ValueMember = "address";
                this.comboBoxContactList.DisplayMember = "contact";
                this.comboBoxContactList_Load(string.Empty);
                this.dateTimePickerScheduleDate.CustomFormat = "MM/dd/yyyy hh:mm tt";
                this.LoadMessageTemplateMenu();
                this.Text = this.appManager.m_strApplicationName + " New Message " + this.appManager.FormatPhone(this.appManager.m_strUserName);
                base.Icon = this.appManager.iTextApp;
                if (this.appManager.m_strForwardMessage.Length > 0)
                {
                    if (this.appManager.m_strForwardMessage.Length > 250)
                    {
                        this.appManager.m_strForwardMessage = this.appManager.m_strForwardMessage.Substring(0, 250);
                    }
                    this.textBoxNewMessage.Text = this.appManager.m_strForwardMessage;
                    this.appManager.m_strForwardMessage = string.Empty;
                }
                if (this.appManager.m_bEnableSignature && (this.textBoxNewMessage.Text.Length == 0))
                {
                    this.textBoxNewMessage.Text = "\r\n" + this.appManager.m_strSignature;
                    this.textBoxNewMessage.Select(0, 0);
                }
            }
            catch (Exception exception)
            {
                this.strError = "Unexpected application error while loading New Message window: " + exception.Message;
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
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmNewMessage));
            this.buttonSend = new Button();
            this.textBoxNewMessage = new RichTextBox();
            this.lblCharCount = new Label();
            this.comboBoxContactList = new ComboBox();
            this.listBoxSelectedContacts = new ListBox();
            this.labelClickRemove = new Label();
            this.label1 = new Label();
            this.menuStripNewMessage = new MenuStrip();
            this.messageTemplatesToolStripMenuItem = new ToolStripMenuItem();
            this.editMessageTemplatesToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.editToolStripMenuItem = new ToolStripMenuItem();
            this.manageGroupsToolStripMenuItem = new ToolStripMenuItem();
            this.manageContactsToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator4 = new ToolStripSeparator();
            this.editKeywordAutoResponseToolStripMenuItem = new ToolStripMenuItem();
            this.editGroupScheduleToolStripMenuItem = new ToolStripMenuItem();
            this.optionsToolStripMenuItem = new ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.controlEnterToolStripMenuItem = new ToolStripMenuItem();
            this.validatePhoneNumbersToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.generalHelpToolStripMenuItem = new ToolStripMenuItem();
            this.settingsHelpToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator3 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.syncFeaturesToolStripMenuItem = new ToolStripMenuItem();
            this.versionToolStripMenuItem = new ToolStripMenuItem();
            this.logOutToolStripMenuItem = new ToolStripMenuItem();
            this.linkLabelRemoveAll = new LinkLabel();
            this.dateTimePickerScheduleDate = new DateTimePicker();
            this.labelCount = new Label();
            this.labelAttRemove = new Label();
            this.pictureBoxLink = new PictureBox();
            this.pictureBoxAttachment = new PictureBox();
            this.openFileDialog = new OpenFileDialog();
            this.rapidSpellAsYouTypeNewMessage = new RapidSpellAsYouType(this.components);
            this.tryBETAToolStripMenuItem = new ToolStripMenuItem();
            this.menuStripNewMessage.SuspendLayout();
            ((ISupportInitialize) this.pictureBoxLink).BeginInit();
            ((ISupportInitialize) this.pictureBoxAttachment).BeginInit();
            base.SuspendLayout();
            this.buttonSend.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSend.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSend.Location = new Point(340, 0x174);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new Size(0x38, 0x1b);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new EventHandler(this.buttonSend_Click);
            this.textBoxNewMessage.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.textBoxNewMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxNewMessage.Location = new Point(12, 0xc9);
            this.textBoxNewMessage.MaxLength = 250;
            this.textBoxNewMessage.Multiline = true;
            this.textBoxNewMessage.Name = "textBoxNewMessage";
            this.textBoxNewMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
            this.textBoxNewMessage.Size = new Size(0x181, 0x8d);
            this.textBoxNewMessage.TabIndex = 1;
            this.textBoxNewMessage.TextChanged += new EventHandler(this.textBoxNewMessage_TextChanged);
            this.textBoxNewMessage.KeyPress += new KeyPressEventHandler(this.textBoxNewMessage_KeyPress);
            this.lblCharCount.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.lblCharCount.AutoSize = true;
            this.lblCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lblCharCount.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.lblCharCount.Location = new Point(0x117, 0x17b);
            this.lblCharCount.MinimumSize = new Size(0x37, 0);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new Size(0x37, 15);
            this.lblCharCount.TabIndex = 3;
            this.lblCharCount.Text = "0/250";
            this.lblCharCount.TextAlign = ContentAlignment.MiddleRight;
            this.comboBoxContactList.AllowDrop = true;
            this.comboBoxContactList.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxContactList.FormattingEnabled = true;
            this.comboBoxContactList.Location = new Point(12, 0x36);
            this.comboBoxContactList.Name = "comboBoxContactList";
            this.comboBoxContactList.Size = new Size(0x181, 0x19);
            this.comboBoxContactList.TabIndex = 0;
            this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
            this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
            this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
            this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
            this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
            this.listBoxSelectedContacts.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listBoxSelectedContacts.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.listBoxSelectedContacts.FormattingEnabled = true;
            this.listBoxSelectedContacts.ItemHeight = 0x11;
            this.listBoxSelectedContacts.Location = new Point(12, 0x66);
            this.listBoxSelectedContacts.Name = "listBoxSelectedContacts";
            this.listBoxSelectedContacts.ScrollAlwaysVisible = true;
            this.listBoxSelectedContacts.Size = new Size(0x181, 0x59);
            this.listBoxSelectedContacts.Sorted = true;
            this.listBoxSelectedContacts.TabIndex = 5;
            this.listBoxSelectedContacts.TabStop = false;
            this.listBoxSelectedContacts.DoubleClick += new EventHandler(this.listBoxSelectedContacts_DoubleClick);
            this.labelClickRemove.AutoSize = true;
            this.labelClickRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelClickRemove.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelClickRemove.Location = new Point(12, 0x55);
            this.labelClickRemove.Name = "labelClickRemove";
            this.labelClickRemove.Size = new Size(180, 14);
            this.labelClickRemove.TabIndex = 6;
            this.labelClickRemove.Text = "Double-click to remove a recipient.";
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.label1.Location = new Point(12, 0x22);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x131, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Enter phone number, existing contact name, or group tag (#).";
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.messageTemplatesToolStripMenuItem, this.editToolStripMenuItem, this.optionsToolStripMenuItem, this.helpToolStripMenuItem };
            this.menuStripNewMessage.Items.AddRange(toolStripItems);
            this.menuStripNewMessage.Location = new Point(0, 0);
            this.menuStripNewMessage.Name = "menuStripNewMessage";
            this.menuStripNewMessage.Size = new Size(0x199, 0x18);
            this.menuStripNewMessage.TabIndex = 8;
            this.menuStripNewMessage.Text = "menuStrip1";
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.editMessageTemplatesToolStripMenuItem, this.toolStripSeparator1 };
            this.messageTemplatesToolStripMenuItem.DropDownItems.AddRange(itemArray2);
            this.messageTemplatesToolStripMenuItem.Name = "messageTemplatesToolStripMenuItem";
            this.messageTemplatesToolStripMenuItem.Size = new Size(0x7a, 20);
            this.messageTemplatesToolStripMenuItem.Text = "Message &Templates";
            this.editMessageTemplatesToolStripMenuItem.Name = "editMessageTemplatesToolStripMenuItem";
            this.editMessageTemplatesToolStripMenuItem.Size = new Size(0xd1, 0x16);
            this.editMessageTemplatesToolStripMenuItem.Text = "&Edit Message Templates...";
            this.editMessageTemplatesToolStripMenuItem.Click += new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(0xce, 6);
            ToolStripItem[] itemArray3 = new ToolStripItem[] { this.manageGroupsToolStripMenuItem, this.manageContactsToolStripMenuItem, this.toolStripSeparator4, this.editKeywordAutoResponseToolStripMenuItem, this.editGroupScheduleToolStripMenuItem };
            this.editToolStripMenuItem.DropDownItems.AddRange(itemArray3);
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new Size(0x27, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            this.manageGroupsToolStripMenuItem.Name = "manageGroupsToolStripMenuItem";
            this.manageGroupsToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.manageGroupsToolStripMenuItem.Text = "Edit &Groups";
            this.manageGroupsToolStripMenuItem.Click += new EventHandler(this.manageGroupsToolStripMenuItem_Click);
            this.manageContactsToolStripMenuItem.Name = "manageContactsToolStripMenuItem";
            this.manageContactsToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.manageContactsToolStripMenuItem.Text = "Edit &Contacts";
            this.manageContactsToolStripMenuItem.Click += new EventHandler(this.manageContactToolStripMenuItem_Click);
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new Size(0xde, 6);
            this.editKeywordAutoResponseToolStripMenuItem.Name = "editKeywordAutoResponseToolStripMenuItem";
            this.editKeywordAutoResponseToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.editKeywordAutoResponseToolStripMenuItem.Text = "Edit &Keyword Auto Response";
            this.editKeywordAutoResponseToolStripMenuItem.Click += new EventHandler(this.manageKeywordAutoResponseToolStripMenuItem_Click);
            this.editGroupScheduleToolStripMenuItem.Name = "editGroupScheduleToolStripMenuItem";
            this.editGroupScheduleToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.editGroupScheduleToolStripMenuItem.Text = "Edit Group &Schedule";
            this.editGroupScheduleToolStripMenuItem.Click += new EventHandler(this.editGroupScheduleToolStripMenuItem_Click);
            ToolStripItem[] itemArray4 = new ToolStripItem[] { this.settingsToolStripMenuItem1, this.toolStripSeparator2, this.controlEnterToolStripMenuItem, this.validatePhoneNumbersToolStripMenuItem };
            this.optionsToolStripMenuItem.DropDownItems.AddRange(itemArray4);
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new Size(0x3d, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new Size(0x14c, 0x16);
            this.settingsToolStripMenuItem1.Text = "Settings";
            this.settingsToolStripMenuItem1.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(0x149, 6);
            this.controlEnterToolStripMenuItem.CheckOnClick = true;
            this.controlEnterToolStripMenuItem.Name = "controlEnterToolStripMenuItem";
            this.controlEnterToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.controlEnterToolStripMenuItem.Text = "Use Control + Enter To Send Instead Of Just Enter";
            this.controlEnterToolStripMenuItem.Click += new EventHandler(this.controlEnterToolStripMenuItem_Click);
            this.validatePhoneNumbersToolStripMenuItem.CheckOnClick = true;
            this.validatePhoneNumbersToolStripMenuItem.Name = "validatePhoneNumbersToolStripMenuItem";
            this.validatePhoneNumbersToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.validatePhoneNumbersToolStripMenuItem.Text = "Require 10 Digit Phone Numbers";
            this.validatePhoneNumbersToolStripMenuItem.Click += new EventHandler(this.validatePhoneNumbersToolStripMenuItem_Click);
            ToolStripItem[] itemArray5 = new ToolStripItem[] { this.generalHelpToolStripMenuItem, this.settingsHelpToolStripMenuItem, this.toolStripSeparator3, this.exitToolStripMenuItem, this.syncFeaturesToolStripMenuItem, this.versionToolStripMenuItem, this.tryBETAToolStripMenuItem, this.logOutToolStripMenuItem };
            this.helpToolStripMenuItem.DropDownItems.AddRange(itemArray5);
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(0x2c, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
            this.generalHelpToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.generalHelpToolStripMenuItem.Text = "General Help";
            this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
            this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
            this.settingsHelpToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.settingsHelpToolStripMenuItem.Text = "Settings Help";
            this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new Size(0x95, 6);
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
            this.versionToolStripMenuItem.Text = "version";
            this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.logOutToolStripMenuItem.Text = "Log Out";
            this.logOutToolStripMenuItem.Click += new EventHandler(this.logOutToolStripMenuItem_Click);
            this.linkLabelRemoveAll.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.linkLabelRemoveAll.AutoSize = true;
            this.linkLabelRemoveAll.LinkColor = System.Drawing.Color.Olive;
            this.linkLabelRemoveAll.Location = new Point(0x14f, 0x56);
            this.linkLabelRemoveAll.Name = "linkLabelRemoveAll";
            this.linkLabelRemoveAll.Size = new Size(0x3d, 13);
            this.linkLabelRemoveAll.TabIndex = 9;
            this.linkLabelRemoveAll.TabStop = true;
            this.linkLabelRemoveAll.Text = "Remove All";
            this.linkLabelRemoveAll.VisitedLinkColor = System.Drawing.Color.Olive;
            this.linkLabelRemoveAll.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelRemoveAll_LinkClicked);
            this.dateTimePickerScheduleDate.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.dateTimePickerScheduleDate.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.dateTimePickerScheduleDate.Format = DateTimePickerFormat.Custom;
            this.dateTimePickerScheduleDate.Location = new Point(0x76, 0x177);
            this.dateTimePickerScheduleDate.MinDate = new DateTime(0x76c, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerScheduleDate.Name = "dateTimePickerScheduleDate";
            this.dateTimePickerScheduleDate.Size = new Size(0xa7, 0x16);
            this.dateTimePickerScheduleDate.TabIndex = 10;
            this.labelCount.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.labelCount.AutoSize = true;
            this.labelCount.Font = new Font("Arial", 8.25f, FontStyle.Italic | FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelCount.Location = new Point(0x13b, 0x55);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new Size(13, 13);
            this.labelCount.TabIndex = 11;
            this.labelCount.Text = "0";
            this.labelCount.TextAlign = ContentAlignment.MiddleRight;
            this.labelAttRemove.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelAttRemove.AutoSize = true;
            this.labelAttRemove.BackColor = System.Drawing.Color.Transparent;
            this.labelAttRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.labelAttRemove.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelAttRemove.Location = new Point(9, 0x17a);
            this.labelAttRemove.Name = "labelAttRemove";
            this.labelAttRemove.Size = new Size(0x54, 14);
            this.labelAttRemove.TabIndex = 14;
            this.labelAttRemove.Text = "Click to remove";
            this.labelAttRemove.Visible = false;
            this.labelAttRemove.Click += new EventHandler(this.labelAttRemove_Click);
            this.pictureBoxLink.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.pictureBoxLink.Image = Resources.Paperclip;
            this.pictureBoxLink.Location = new Point(0x5c, 0x178);
            this.pictureBoxLink.Name = "pictureBoxLink";
            this.pictureBoxLink.Size = new Size(20, 20);
            this.pictureBoxLink.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxLink.TabIndex = 13;
            this.pictureBoxLink.TabStop = false;
            this.pictureBoxLink.Click += new EventHandler(this.pictureBoxLink_Click);
            this.pictureBoxAttachment.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.pictureBoxAttachment.Location = new Point(12, 350);
            this.pictureBoxAttachment.Name = "pictureBoxAttachment";
            this.pictureBoxAttachment.Size = new Size(0x49, 0x2e);
            this.pictureBoxAttachment.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxAttachment.TabIndex = 12;
            this.pictureBoxAttachment.TabStop = false;
            this.pictureBoxAttachment.Click += new EventHandler(this.labelAttRemove_Click);
            this.rapidSpellAsYouTypeNewMessage.AddMenuText = "Add";
            this.rapidSpellAsYouTypeNewMessage.AllowAnyCase = false;
            this.rapidSpellAsYouTypeNewMessage.AllowMixedCase = false;
            this.rapidSpellAsYouTypeNewMessage.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeNewMessage.CheckAsYouType = true;
            this.rapidSpellAsYouTypeNewMessage.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeNewMessage.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeNewMessage.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeNewMessage.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeNewMessage.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeNewMessage.DictFilePath = null;
            this.rapidSpellAsYouTypeNewMessage.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeNewMessage.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeNewMessage.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeNewMessage.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeNewMessage.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeNewMessage.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeNewMessage.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeNewMessage.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeNewMessage.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeNewMessage.IgnoreXML = false;
            this.rapidSpellAsYouTypeNewMessage.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeNewMessage.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeNewMessage.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeNewMessage.OptionsEnabled = true;
            this.rapidSpellAsYouTypeNewMessage.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeNewMessage.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeNewMessage.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeNewMessage.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeNewMessage.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeNewMessage.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeNewMessage.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeNewMessage.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeNewMessage.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeNewMessage.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeNewMessage.TextBoxBase = this.textBoxNewMessage;
            this.rapidSpellAsYouTypeNewMessage.TextComponent = null;
            this.rapidSpellAsYouTypeNewMessage.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeNewMessage.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeNewMessage.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeNewMessage.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeNewMessage.V2Parser = true;
            this.rapidSpellAsYouTypeNewMessage.WarnDuplicates = true;
            this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
            this.tryBETAToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.tryBETAToolStripMenuItem.Text = "Try BETA!";
            this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x199, 0x19b);
            base.Controls.Add(this.labelAttRemove);
            base.Controls.Add(this.pictureBoxLink);
            base.Controls.Add(this.pictureBoxAttachment);
            base.Controls.Add(this.labelCount);
            base.Controls.Add(this.dateTimePickerScheduleDate);
            base.Controls.Add(this.linkLabelRemoveAll);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.labelClickRemove);
            base.Controls.Add(this.listBoxSelectedContacts);
            base.Controls.Add(this.comboBoxContactList);
            base.Controls.Add(this.lblCharCount);
            base.Controls.Add(this.textBoxNewMessage);
            base.Controls.Add(this.buttonSend);
            base.Controls.Add(this.menuStripNewMessage);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MainMenuStrip = this.menuStripNewMessage;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new Size(0x1a9, 450);
            base.Name = "fmNewMessage";
            this.Text = "New Message";
            base.FormClosing += new FormClosingEventHandler(this.fmNewMessage_FormClosing);
            base.Load += new EventHandler(this.fmNewMessage_Load);
            this.menuStripNewMessage.ResumeLayout(false);
            this.menuStripNewMessage.PerformLayout();
            ((ISupportInitialize) this.pictureBoxLink).EndInit();
            ((ISupportInitialize) this.pictureBoxAttachment).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void labelAttRemove_Click(object sender, EventArgs e)
        {
            this.pictureBoxAttachment.ImageLocation = null;
            this.labelAttRemove.Visible = false;
            this.dateTimePickerScheduleDate.Enabled = true;
        }

        private void linkLabelRemoveAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.listBoxSelectedContacts.Items.Clear();
            this.labelCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
            this.comboBoxContactList_Load(string.Empty);
            this.comboBoxContactList.Focus();
        }

        private void listBoxSelectedContacts_Add()
        {
            MessageRecipient selectedItem;
            if (this.comboBoxContactList.SelectedIndex >= 0)
            {
                selectedItem = (MessageRecipient) this.comboBoxContactList.SelectedItem;
                if (selectedItem.address.Contains("#"))
                {
                    foreach (GroupTagContact contact in (from val in this.appManager.m_lsGroupTagContacts
                        where val.groupTag == selectedItem.address
                        select val).ToList<GroupTagContact>())
                    {
                        MessageRecipient recipient = new MessageRecipient {
                            contact = contact.contact,
                            address = contact.address
                        };
                        if (!this.listBoxSelectedContacts.Items.Contains(recipient))
                        {
                            this.listBoxSelectedContacts.Items.Add(recipient);
                        }
                    }
                    this.comboBoxContactList_Load(string.Empty);
                }
                else if (this.comboBoxContactList_Validate(selectedItem.address))
                {
                    MessageRecipient item = new MessageRecipient {
                        address = this.appManager.FormatContactAddress(selectedItem.address, false, false),
                        contact = selectedItem.contact
                    };
                    this.listBoxSelectedContacts.Items.Add(item);
                    this.comboBoxContactList_Load(string.Empty);
                }
                else
                {
                    MessageBox.Show("Please enter a 10 digit phone number or select a valid recipient from your existing contact list.  You may turn off Text Number validation in your Settings...", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                this.labelCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
            }
        }

        private void listBoxSelectedContacts_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.listBoxSelectedContacts.Items.Remove(this.listBoxSelectedContacts.SelectedItem);
                this.labelCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
                this.comboBoxContactList_Load(string.Empty);
            }
            catch
            {
            }
        }

        private void listBoxSelectedContacts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ListBox box = (ListBox) sender;
                MessageRecipient recipient = (MessageRecipient) box.Items[e.Index];
                float width = 2f;
                e.DrawBackground();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.WhiteSmoke, e.Bounds);
                }
                e.Graphics.DrawString(recipient.contact, box.Font, Brushes.Black, (float) (e.Bounds.Left + 5), (float) (e.Bounds.Top + 4));
                e.Graphics.DrawRectangle(new Pen(System.Drawing.Color.DimGray, width), e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void listBoxSelectedContacts_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 0x18;
        }

        public void LoadMessageTemplateMenu()
        {
            try
            {
                string str = string.Empty;
                ToolStripItem item = null;
                this.messageTemplatesToolStripMenuItem.DropDownItems.Clear();
                item = new ToolStripMenuItem("&Edit Message Templates...", null, new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click));
                this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                this.messageTemplatesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate1))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate1);
                    item = new ToolStripMenuItem("&1. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "1");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate2))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate2);
                    item = new ToolStripMenuItem("&2. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "2");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate3))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate3);
                    item = new ToolStripMenuItem("&3. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "3");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate4))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate4);
                    item = new ToolStripMenuItem("&4. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "4");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate5))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate5);
                    item = new ToolStripMenuItem("&5. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "5");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate6))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate6);
                    item = new ToolStripMenuItem("&6. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "6");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate7))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate7);
                    item = new ToolStripMenuItem("&7. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "7");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate8))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate8);
                    item = new ToolStripMenuItem("&8. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "8");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate9))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate9);
                    item = new ToolStripMenuItem("&9. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "9");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
                if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate10))
                {
                    str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate10);
                    item = new ToolStripMenuItem("&10. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "10");
                    this.messageTemplatesToolStripMenuItem.DropDownItems.Add(item);
                }
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception loading message template menue item list: " + exception.Message, 5);
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.LogOut(true);
        }

        private void manageContactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowEditContact(true);
        }

        private void manageGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowEditGroups();
        }

        private void manageKeywordAutoResponseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowKeywordAutoResponse();
        }

        private void messageTemplateMenuItem_Click(object sender, EventArgs e)
        {
            string str = string.Empty;
            string name = ((ToolStripDropDownItem) sender).Name;
            switch (<PrivateImplementationDetails>.ComputeStringHash(name))
            {
                case 0x310ca263:
                    if (name == "4")
                    {
                        str = this.appManager.m_strMessageTemplate4;
                        goto Label_021F;
                    }
                    break;

                case 0x320ca3f6:
                    if (name == "7")
                    {
                        str = this.appManager.m_strMessageTemplate7;
                        goto Label_021F;
                    }
                    break;

                case 0x330ca589:
                    if (name == "6")
                    {
                        str = this.appManager.m_strMessageTemplate6;
                        goto Label_021F;
                    }
                    break;

                case 0x1beb2a44:
                    if (name == "10")
                    {
                        str = this.appManager.m_strMessageTemplate10;
                        goto Label_021F;
                    }
                    break;

                case 0x300ca0d0:
                    if (name == "5")
                    {
                        str = this.appManager.m_strMessageTemplate5;
                        goto Label_021F;
                    }
                    break;

                case 0x340ca71c:
                    if (name == "1")
                    {
                        str = this.appManager.m_strMessageTemplate1;
                        goto Label_021F;
                    }
                    break;

                case 0x360caa42:
                    if (name == "3")
                    {
                        str = this.appManager.m_strMessageTemplate3;
                        goto Label_021F;
                    }
                    break;

                case 0x370cabd5:
                    if (name == "2")
                    {
                        str = this.appManager.m_strMessageTemplate2;
                        goto Label_021F;
                    }
                    break;

                case 0x3c0cb3b4:
                    if (name == "9")
                    {
                        str = this.appManager.m_strMessageTemplate9;
                        goto Label_021F;
                    }
                    break;

                case 0x3d0cb547:
                    if (name == "8")
                    {
                        str = this.appManager.m_strMessageTemplate8;
                        goto Label_021F;
                    }
                    break;
            }
            this.appManager.ShowBalloon("Invalid message template selection", 5);
        Label_021F:
            this.textBoxNewMessage.Text = str;
            this.textBoxNewMessage.Select(this.textBoxNewMessage.Text.Length, 0);
        }

        private void optionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.validatePhoneNumbersToolStripMenuItem.Checked = this.appManager.m_bValidateMobileNumbers;
            this.controlEnterToolStripMenuItem.Checked = this.appManager.m_bControlEnter;
        }

        private void pictureBoxLink_Click(object sender, EventArgs e)
        {
            if (!this.appManager.m_bMMSSendFeature)
            {
                MessageBox.Show("Sending MMS texts (messages with attachements) is not enabled for this account - please contact support to turn on...", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                this.openFileDialog.Title = "Select a file to be sent";
                this.openFileDialog.CheckFileExists = true;
                this.openFileDialog.Multiselect = false;
                this.openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";
                if (this.openFileDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    string fileName = this.openFileDialog.FileName;
                    if ((new FileInfo(fileName).Length / 0x400L) < 0x400L)
                    {
                        this.pictureBoxAttachment.ImageLocation = fileName;
                        this.labelAttRemove.Visible = true;
                        this.dateTimePickerScheduleDate.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("File must be less than one MB", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
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

        private void textBoxNewMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.appManager.m_bControlEnter && (e.KeyChar == '\n'))
            {
                this.buttonSend_Click(sender, new EventArgs());
                e.Handled = true;
            }
            if (!this.appManager.m_bControlEnter && (e.KeyChar == '\r'))
            {
                this.buttonSend_Click(sender, new EventArgs());
                e.Handled = true;
            }
        }

        private void textBoxNewMessage_TextChanged(object sender, EventArgs e)
        {
            this.lblCharCount.ForeColor = new System.Drawing.Color();
            int length = this.textBoxNewMessage.Text.Length;
            if (length == 250)
            {
                this.lblCharCount.ForeColor = System.Drawing.Color.Red;
            }
            else if (length > 240)
            {
                this.lblCharCount.ForeColor = System.Drawing.Color.Orange;
            }
            this.lblCharCount.Text = length.ToString() + "/250";
        }

        private void tryBETAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdater.ShowUserNoUpdateAvailable = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.LetUserSelectSkip = false;
            AutoUpdater.Start(this.appManager.m_strBETAUpdateFileURL);
        }

        private void validatePhoneNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref this.strError), "ValidateMobileNumbers", this.validatePhoneNumbersToolStripMenuItem.Checked, ref this.strError, false, RegistryValueKind.Unknown);
                if (this.strError != string.Empty)
                {
                    this.strError = this.strError + "Require 10 digit numbers save error: " + this.strError;
                    this.appManager.ShowBalloon(this.strError, 5);
                }
                else
                {
                    this.appManager.m_bValidateMobileNumbers = this.validatePhoneNumbersToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Require 10 digit numbers save error: " + exception.Message;
                this.appManager.ShowBalloon(this.strError, 5);
            }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoUpdater.ShowUserNoUpdateAvailable = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.LetUserSelectSkip = false;
            AutoUpdater.Start(this.appManager.m_strUpdateFileURL);
        }

        public ApplicationManager appManager { get; set; }

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly fmNewMessage.<>c <>9 = new fmNewMessage.<>c();
            public static Func<fmNewMessage.MessageRecipient, string> <>9__15_0;

            internal string <comboBoxContactList_Load>b__15_0(fmNewMessage.MessageRecipient c) => 
                c.contact;
        }*/

        [StructLayout(LayoutKind.Sequential)]
        private struct MessageRecipient
        {
            public string address { get; set; }
            public string contact { get; set; }
        }
    }
}


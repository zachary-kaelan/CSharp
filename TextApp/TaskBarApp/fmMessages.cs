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
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;
    using TaskBarApp.Properties;

    public class fmMessages : Form
    {
        private bool bConversationActionLoad;
        private bool bConversationDelete;
        private bool bConversationExport;
        private bool bConversationListLoading;
        private bool bConversationLoading;
        private bool bStopConversationListSelectIndexChange;
        private Button buttonClear;
        private Button buttonMarkAllRead;
        private Button buttonRefresh;
        private Button buttonSend;
        private CheckBox checkBoxFullRefresh;
        private ComboBox comboBoxConversationAction;
        private ComboBox comboBoxFilter;
        private IContainer components;
        private ContextMenuStrip contextMenuStripMessageText;
        private ContextMenuStrip contextMenuStripPictureBox;
        private ToolStripMenuItem controlEnterToolStripMenuItem;
        private ToolStripMenuItem copySelectedTextToolStripMenuItem;
        private ToolStripMenuItem copyTextToolStripMenuItem;
        private DateTimePicker dateTimePickerScheduleDate;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem displayMMSAttachmentsToolStripMenuItem;
        private ToolStripMenuItem downloadToolStripMenuItem;
        private ToolStripMenuItem editGroupScheduleToolStripMenuItem;
        private ToolStripMenuItem editKeywordAutoResponseToolStripMenuItem;
        private ToolStripMenuItem editMessageTemplatesToolStripMenuItem;
        private ToolStripMenuItem editsToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem forwardTextMessage;
        private FormWindowState fwsLastWindowState = FormWindowState.Minimized;
        private ToolStripMenuItem generalHelpToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem keepSelectedConversationInFocusToolStripMenuItem;
        private Label labelAttRemove;
        private Label labelCharCount;
        private Label labelConversationsCount;
        private Label labelLoadingConversation;
        private Label labelMesssageCount;
        private Label labelProcessing;
        private Label labelTextBoxSearchHint;
        private Label labelUnread;
        private Label labelUnreadCount;
        private LinkLabel linkLabelEditContact;
        private LinkLabel linkLabelMoreConversations;
        private LinkLabel linkLabelMoreMessages;
        private ListBox listBoxConversationList;
        private ToolStripMenuItem logOutToolStripMenuItem;
        private List<TextMessage> lsMessagesWorking = new List<TextMessage>();
        private ToolStripMenuItem manageContactsToolStripMenuItem;
        private ToolStripMenuItem manageGroupsToolStripMenuItem;
        private ToolStripMenuItem markAsReadToolStripMenuItem;
        private MenuStrip menuStripMessage;
        private ToolStripMenuItem messageTemplatesToolStripMenuItem;
        private ToolStripMenuItem newMessageToolStripMenuItem;
        private int nMessageLimit = 50;
        private int nPadding = 6;
        private long nSelectedMessageID;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem openMessagesWindowWithUnreadReminderToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private PictureBox pictureBoxAttachment;
        private PictureBox pictureBoxConversationCountLock;
        private PictureBox pictureBoxLink;
        private PictureBox pictureBoxSelectedMessage;
        public Panel pnMessages;
        private Panel pnMoreMessages;
        private Panel pnSend;
        private RapidSpellAsYouType rapidSpellAsYouTypeText;
        private ToolStripMenuItem requireClickToMarkMessageReadToolStripMenuItem;
        private SaveFileDialog saveFileDialogPrintConversation;
        private ToolStripMenuItem settingsHelpToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private Size sFormSize;
        private SplitContainer splitContainerMessages;
        private string strError = string.Empty;
        private string strImageStorageKey = string.Empty;
        private ToolStripMenuItem syncFeaturesToolStripMenuItem;
        private TableLayoutPanel tableLayoutPanelContactName;
        private TableLayoutPanel tableLayoutPanelContactPhone;
        private TableLayoutPanel tableLayoutPanelMessages;
        private TextBox textBoxContactName;
        private TextBox textBoxContactPhone;
        private TextBox textBoxMessage;
        private TextBox textBoxSearch;
        private RichTextBox textBoxSelectedMessage;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolTip toolTipMarkAllRead;
        private ToolStripMenuItem tryBETAToolStripMenuItem;
        private ToolStripMenuItem versionToolStripMenuItem;

        public fmMessages()
        {
            this.InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.textBoxSearch.Clear();
            this.textBoxSearch.Focus();
        }

        private void buttonMarkAllRead_Click(object sender, EventArgs e)
        {
            this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, true, true);
            this.listBoxConversationList.Invalidate();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.buttonRefresh.Enabled = false;
            if (this.appManager.m_strCurentConversationFingerprint.Length > 0)
            {
                this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
            }
            if (this.checkBoxFullRefresh.Checked)
            {
                this.checkBoxFullRefresh.Checked = false;
                if (!this.appManager.m_bConversationCountLocked)
                {
                    this.appManager.m_nConversationLimit = this.appManager.m_nConversationLimitDefault;
                }
                if (this.listBoxConversationList.Items.Count > 0)
                {
                    this.listBoxConversationList.Items.Clear();
                }
                this.labelLoadingConversation.Visible = true;
                this.appManager.LoadContacts(true);
                this.appManager.LoadConversations(true);
            }
            this.buttonRefresh.Enabled = true;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            this.strError = null;
            IRestResponse<TextMessageSendResponse> response = null;
            IRestResponse<MMSSendResponse> response2 = null;
            if ((this.textBoxMessage.Text.Length == 0) || (this.textBoxMessage.Text.Trim() == this.appManager.m_strSignature.Trim()))
            {
                this.textBoxMessage.Focus();
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
                    {
                        response2 = this.appManager.m_textService.SendMessageMMS(this.textBoxMessage.Text, this.appManager.FormatContactAddress(this.appManager.m_strCurrentContactAddress, true, true), this.appManager.m_strSession, this.pictureBoxAttachment.ImageLocation);
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
                        DateTime scheduledDate = this.dateTimePickerScheduleDate.Value;
                        response = this.appManager.m_textService.SendMessage(this.textBoxMessage.Text, this.appManager.m_strCurrentContactAddress, this.appManager.m_strSession, scheduledDate);
                        if (!string.IsNullOrEmpty(response.ErrorMessage))
                        {
                            this.strError = "Error calling message/send: " + response.ErrorMessage;
                        }
                        else if (!response.Data.success)
                        {
                            this.strError = "Error from message/send...";
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
                    this.appManager.LoadUpdates(false);
                    if (this.appManager.m_bEnableSignature)
                    {
                        this.textBoxMessage.Text = "\r\n" + this.appManager.m_strSignature;
                        this.textBoxMessage.Select(0, 0);
                    }
                    else
                    {
                        this.textBoxMessage.Text = "";
                    }
                    this.pictureBoxAttachment.ImageLocation = null;
                    this.labelAttRemove.Visible = false;
                    this.dateTimePickerScheduleDate.Value = DateTime.Now;
                    this.dateTimePickerScheduleDate.Enabled = true;
                }
            }
        }

        private void comboBoxConversationAction_Leave(object sender, EventArgs e)
        {
            this.comboBoxConversationAction_Load();
        }

        private void comboBoxConversationAction_Load()
        {
            this.bConversationActionLoad = true;
            this.comboBoxConversationAction.Items.Clear();
            this.comboBoxConversationAction.Items.Add("- Select Action -");
            this.comboBoxConversationAction.Items.Add("Print Conversation");
            this.comboBoxConversationAction.Items.Add("Export Conversation");
            if (this.appManager.m_bAllowDelete)
            {
                this.comboBoxConversationAction.Items.Add("Delete Conversation");
            }
            this.comboBoxConversationAction.SelectedIndex = 0;
        }

        private void comboBoxConversationAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.bConversationActionLoad)
            {
                this.bConversationActionLoad = false;
            }
            else if (this.bConversationDelete)
            {
                this.bConversationDelete = false;
            }
            else if (this.bConversationExport)
            {
                this.bConversationExport = false;
            }
            else
            {
                this.comboBoxConversationAction.DroppedDown = true;
            }
        }

        private void comboBoxConversationAction_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.comboBoxConversationAction.DroppedDown)
            {
                bool flag = false;
                if ((this.comboBoxConversationAction.SelectedItem == "Print Conversation") && (this.appManager.m_strCurentConversationFingerprint != "0"))
                {
                    this.bConversationExport = true;
                    this.appManager.ShowPrintConversation();
                }
                else if ((this.comboBoxConversationAction.SelectedItem == "Export Conversation") && (this.appManager.m_strCurentConversationFingerprint != "0"))
                {
                    this.bConversationExport = true;
                    this.ExportConversation();
                }
                else if ((this.comboBoxConversationAction.SelectedItem == "Delete Conversation") && (this.appManager.m_strCurentConversationFingerprint != "0"))
                {
                    try
                    {
                        this.bConversationDelete = true;
                        if (MessageBox.Show("Delete the entire conversation with " + this.textBoxContactPhone.Text + "?\n\n Please Note this cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            if (this.appManager.m_nCurrentContactID > 0L)
                            {
                                string[] textArray1 = new string[] { "Would you also like to delete contact ", this.textBoxContactName.Text, " ", this.textBoxContactPhone.Text, "?" };
                                if (MessageBox.Show(string.Concat(textArray1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    flag = true;
                                }
                            }
                            if (this.appManager.m_textService.ConversationDelete(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession).Data.success)
                            {
                                Conversation item = this.appManager.m_lsConversation.Find(var => var.fingerprint == this.appManager.m_strCurentConversationFingerprint);
                                if (item != null)
                                {
                                    this.appManager.m_lsConversation.Remove(item);
                                }
                                ConversationMetaData data = this.appManager.m_lsConversationMetaData.Find(var => var.fingerprint == this.appManager.m_strCurentConversationFingerprint);
                                if (data != null)
                                {
                                    this.appManager.m_lsConversationMetaData.Remove(data);
                                }
                                if (flag)
                                {
                                    this.appManager.RemoveGroupTag(this.appManager.m_nCurrentContactID, "all");
                                    if (!this.appManager.m_textService.DeleteContact(this.appManager.m_nCurrentContactID, this.appManager.m_strSession).Data.success)
                                    {
                                        this.strError = "Error calling contact/delete...";
                                    }
                                    else
                                    {
                                        Contact contact = this.appManager.m_lsContact.Find(var => var.id == this.appManager.m_nCurrentContactID);
                                        if (contact != null)
                                        {
                                            this.appManager.m_lsContact.Remove(contact);
                                        }
                                    }
                                }
                                this.DisplayConversatoinList();
                            }
                            else
                            {
                                this.strError = "Error calling conversation/delete...";
                            }
                        }
                        else
                        {
                            this.comboBoxConversationAction_Load();
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        this.strError = "Error deleting records: " + exception.Message;
                    }
                    if (!string.IsNullOrEmpty(this.strError))
                    {
                        this.appManager.ShowBalloon(this.strError, 5);
                        this.strError = string.Empty;
                    }
                    else
                    {
                        string text = string.Empty;
                        this.appManager.m_strCurentConversationFingerprint = string.Empty;
                        this.appManager.m_strCurrentContactAddress = string.Empty;
                        this.appManager.m_nCurrentContactID = 0L;
                        if (flag)
                        {
                            text = "Contact and Conversation deleted";
                        }
                        else
                        {
                            text = "Conversation delete.";
                        }
                        this.appManager.ShowBalloon(text, 5);
                        this.DisplayConversatoinList();
                        this.ResetMessageForm(null);
                    }
                }
            }
        }

        private void comboBoxFilter_Leave(object sender, EventArgs e)
        {
            this.DisplayConversatoinList();
        }

        private void comboBoxFilter_Load()
        {
            this.comboBoxFilter.Items.Clear();
            this.comboBoxFilter.Items.Add("- No Filter Selected -");
            this.comboBoxFilter.Items.Add("Only Unread");
            if (this.appManager.m_nLastMessageStatusLimit > 0)
            {
                this.comboBoxFilter.Items.Add("Only In");
                this.comboBoxFilter.Items.Add("Only Out");
                this.comboBoxFilter.Items.Add("Only Error");
            }
            this.comboBoxFilter.SelectedIndex = 0;
        }

        private void comboBoxFilter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.comboBoxFilter.DroppedDown)
            {
                this.DisplayConversatoinList();
            }
        }

        private void contextMenuStripMessageText_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip strip = (ContextMenuStrip) sender;
            this.textBoxSelectedMessage = (RichTextBox) strip.SourceControl;
            this.nSelectedMessageID = Convert.ToInt64(this.textBoxSelectedMessage.Name);
            bool isRead = false;
            bool flag2 = false;
            bool flag3 = false;
            TextMessage message = this.appManager.m_lsMessages.Find(m => m.id == this.nSelectedMessageID);
            if (message != null)
            {
                isRead = message.isRead;
                if (message.destAddress != this.appManager.m_strUserName)
                {
                    flag2 = true;
                }
                if (!string.IsNullOrEmpty(message.scheduledDate))
                {
                    flag3 = true;
                }
            }
            if ((this.appManager.m_bRequreClickToMarkMessageRead && !flag2) && !isRead)
            {
                this.markAsReadToolStripMenuItem.Visible = true;
            }
            else
            {
                this.markAsReadToolStripMenuItem.Visible = false;
            }
            if (this.appManager.m_bAllowDelete | flag3)
            {
                this.deleteToolStripMenuItem.Visible = true;
            }
            else
            {
                this.deleteToolStripMenuItem.Visible = false;
            }
        }

        private void contextMenuStripPictureBox_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                ContextMenuStrip strip = (ContextMenuStrip) sender;
                this.pictureBoxSelectedMessage = (PictureBox) strip.SourceControl;
                int index = this.pictureBoxSelectedMessage.Name.IndexOf("~");
                string str = this.pictureBoxSelectedMessage.Name.Substring(0, index);
                this.nSelectedMessageID = Convert.ToInt64(str);
                this.strImageStorageKey = this.pictureBoxSelectedMessage.Name.Substring(index + 1, (this.pictureBoxSelectedMessage.Name.Length - index) - 1);
            }
            catch (Exception)
            {
                this.strImageStorageKey = string.Empty;
            }
        }

        private void controlEnterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "ControlEnter", this.controlEnterToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Control Enter save error: " + this.strError;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bControlEnter = this.controlEnterToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Control Enter save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 5);
            }
        }

        private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.textBoxSelectedMessage.SelectedText);
        }

        private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.textBoxSelectedMessage.Text);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this message? Please Note this cannot be undone!\n\n" + this.textBoxSelectedMessage.Text, this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.appManager.m_textService.MarkMessageRead(this.nSelectedMessageID, this.appManager.m_strSession);
                if (!this.appManager.m_textService.DeleteMessage(this.nSelectedMessageID, this.appManager.m_strSession).Data.success)
                {
                    this.appManager.ShowBalloon("Error calling message/delete...", 5);
                }
                else
                {
                    this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
                    this.DisplayConversatoinList();
                }
            }
        }

        public void DisplayConversation(string fingerprint, bool bForceMarkAsRead = false, bool bRefreshMessages = true)
        {
            if (!this.bConversationLoading)
            {
                this.bConversationLoading = true;
                this.pnMessages.Controls.Clear();
                bool flag = false;
                RichTextBox activeControl = null;
                PictureBox box2 = null;
                IRestResponse<ConversationGet> response = null;
                this.buttonMarkAllRead.BackColor = ColorTranslator.FromHtml("#93FF14");
                this.buttonMarkAllRead.Visible = false;
                this.labelMesssageCount.Text = "0 Messages displayed";
                try
                {
                    Label label = new Label {
                        ForeColor = System.Drawing.Color.Black,
                        Font = this.appManager.m_fontSize,
                        Text = "Working...",
                        AutoSize = false,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    this.pnMessages.Controls.Add(label);
                    Application.DoEvents();
                    if (fingerprint == null)
                    {
                        if (!string.IsNullOrEmpty(this.appManager.m_strCurentConversationFingerprint))
                        {
                            fingerprint = this.appManager.m_strCurentConversationFingerprint;
                        }
                        else
                        {
                            this.bConversationLoading = false;
                            return;
                        }
                    }
                    if (fingerprint == "0")
                    {
                        label.Text = "There are no current messages to display but there may be older messages. To display them, send a new message to the contact or click the 'more' link in the messages list to get additional records...";
                        this.DisplayConversationHeader();
                    }
                    else
                    {
                        if (bRefreshMessages)
                        {
                            response = this.appManager.m_textService.GetConversation(fingerprint, this.appManager.m_strSession, 0, this.nMessageLimit);
                            if (response != null)
                            {
                                ConversationResponse response2 = response.Data.response;
                                if (response2.messages != null)
                                {
                                    this.appManager.m_strCurentConversationFingerprint = response2.conversation.fingerprint;
                                    this.appManager.m_strCurrentContactAddress = response2.conversation.address;
                                    this.appManager.m_nCurrentContactID = response2.conversation.lastContactId;
                                    this.appManager.m_lsMessages = response2.messages;
                                }
                                else
                                {
                                    this.strError = "Error calling conversation/get...";
                                }
                            }
                        }
                        this.DisplayConversationHeader();
                        if (this.appManager.m_lsMessages.Count > 0)
                        {
                            this.appManager.m_lsMessages = (from c in this.appManager.m_lsMessages
                                orderby c.dateCreated, c.id
                                select c).ToList<TextMessage>();
                            int count = this.appManager.m_lsMessages.Count;
                            this.labelMesssageCount.Text = count.ToString() + " Messages displayed";
                            if (count >= this.nMessageLimit)
                            {
                                this.linkLabelMoreMessages.Visible = true;
                            }
                            else
                            {
                                this.linkLabelMoreMessages.Visible = false;
                            }
                            int y = 10;
                            using (List<TextMessage>.Enumerator enumerator = this.appManager.m_lsMessages.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    DateTime time;
                                    DateTime time2;
                                    TextMessage message = enumerator.Current;
                                    activeControl = null;
                                    box2 = null;
                                    DateTime.TryParse(message.dateCreated, out time);
                                    DateTime.TryParse(message.scheduledDate, out time2);
                                    if (!message.isRead && (this.appManager.FormatContactAddress(message.destAddress, true, true) == this.appManager.FormatContactAddress(this.appManager.m_strUserName, true, true)))
                                    {
                                        if (this.appManager.m_bRequreClickToMarkMessageRead && !bForceMarkAsRead)
                                        {
                                            this.buttonMarkAllRead.Visible = true;
                                        }
                                        else if (!this.appManager.m_textService.MarkMessageRead(message.id, this.appManager.m_strSession).Data.success)
                                        {
                                            this.appManager.ShowBalloon("Error calling message/read...", 5);
                                        }
                                        else
                                        {
                                            flag = true;
                                            TextMessage item = this.appManager.m_lsUnReadMessages.Find(m => m.id == message.id);
                                            this.appManager.m_lsUnReadMessages.Remove(item);
                                        }
                                    }
                                    Label label2 = new Label {
                                        Font = new Font(this.appManager.m_fontSizeDT, FontStyle.Italic),
                                        ForeColor = System.Drawing.Color.DarkGray,
                                        Height = this.appManager.m_fontSizeDT.Height,
                                        Width = ((int) this.appManager.m_fontSizeDT.Size) * 8,
                                        Text = time.ToShortTimeString()
                                    };
                                    Label label3 = new Label {
                                        Font = new Font(this.appManager.m_fontSizeDT, FontStyle.Italic),
                                        ForeColor = System.Drawing.Color.DarkGray,
                                        Height = this.appManager.m_fontSizeDT.Height,
                                        Width = ((int) this.appManager.m_fontSizeDT.Size) * 8,
                                        Text = time.ToShortDateString()
                                    };
                                    string str = message.body.Replace("\n", "\r\n");
                                    RichTextBox box3 = new RichTextBox {
                                        DetectUrls = true
                                    };
                                    box3.LinkClicked += new LinkClickedEventHandler(this.richTextBox_LinkClick);
                                    box3.ReadOnly = true;
                                    box3.ContextMenuStrip = this.contextMenuStripMessageText;
                                    box3.Name = message.id.ToString();
                                    box3.ForeColor = System.Drawing.Color.Black;
                                    box3.BorderStyle = BorderStyle.None;
                                    box3.Font = this.appManager.m_fontSize;
                                    box3.Text = str.Trim();
                                    if (message.scheduledDate != null)
                                    {
                                        RichTextBox box4;
                                        if (time2 > DateTime.Now)
                                        {
                                            box4 = box3;
                                            string[] textArray1 = new string[] { box4.Text, "\r\n\r\n-----------------------------------------\r\nScheduled: ", time2.ToShortDateString(), " ", time2.ToShortTimeString() };
                                            box4.Text = string.Concat(textArray1);
                                        }
                                        else
                                        {
                                            box4 = box3;
                                            string[] textArray2 = new string[] { box4.Text, "\r\n\r\n-----------------------------------------\r\nSent: ", time2.ToShortDateString(), " ", time2.ToShortTimeString() };
                                            box4.Text = string.Concat(textArray2);
                                        }
                                    }
                                    if (message.transmissionState.name == "ERROR")
                                    {
                                        box3.Text = box3.Text + "\r\n\r\n-----------------------------------------\r\nDelivery Failure.";
                                    }
                                    int num3 = 0;
                                    int height = this.appManager.m_fontSize.Height;
                                    if (height < ((this.appManager.m_fontSizeDT.Height * 2) + this.nPadding))
                                    {
                                        height = (this.appManager.m_fontSizeDT.Height * 2) + this.nPadding;
                                    }
                                    if (this.pnMessages.VerticalScroll.Enabled)
                                    {
                                        num3 = SystemInformation.VerticalScrollBarWidth + 40;
                                    }
                                    box3.Width = ((this.pnMessages.ClientSize.Width - label2.Size.Width) - num3) + 30;
                                    TextFormatFlags flags = TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
                                    Size size = TextRenderer.MeasureText(box3.Text, box3.Font, new Size(box3.Width, height), flags);
                                    box3.Height = size.Height;
                                    if (box3.Height < height)
                                    {
                                        box3.Height = height;
                                    }
                                    if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
                                    {
                                        label2.TextAlign = ContentAlignment.TopRight;
                                        label3.TextAlign = ContentAlignment.TopRight;
                                        box3.BackColor = System.Drawing.Color.GhostWhite;
                                        if ((message.scheduledDate != null) && (time2 > DateTime.Now))
                                        {
                                            box3.BackColor = System.Drawing.Color.LightYellow;
                                        }
                                        if (message.transmissionState.name == "ERROR")
                                        {
                                            box3.BackColor = ColorTranslator.FromHtml("#fce3c2");
                                        }
                                        label2.Location = new Point(5, y + 1);
                                        label3.Location = new Point(5, ((y + this.appManager.m_fontSizeDT.Height) + this.nPadding) - 1);
                                        box3.Location = new Point(label3.Size.Width + 10, y);
                                    }
                                    else
                                    {
                                        if (!message.isRead && !bForceMarkAsRead)
                                        {
                                            box3.BackColor = ColorTranslator.FromHtml("#93FF14");
                                        }
                                        else
                                        {
                                            box3.BackColor = ColorTranslator.FromHtml("#E8FFCC");
                                        }
                                        label2.Location = new Point(box3.Width + 10, y + 1);
                                        label3.Location = new Point(box3.Width + 10, ((y + this.appManager.m_fontSizeDT.Height) + this.nPadding) - 1);
                                        box3.Location = new Point(10, y);
                                    }
                                    this.pnMessages.Controls.Add(box3);
                                    this.pnMessages.Controls.Add(label2);
                                    if (Convert.ToDateTime(DateTime.Now.ToShortDateString()) != Convert.ToDateTime(time.ToShortDateString()))
                                    {
                                        this.pnMessages.Controls.Add(label3);
                                    }
                                    y += box3.Size.Height;
                                    activeControl = box3;
                                    if (message.hasAttachment && this.appManager.m_bMMSFeature)
                                    {
                                        try
                                        {
                                            IRestResponse<TextMessageAttachmentList> attachmentList = this.appManager.m_textService.GetAttachmentList(this.appManager.m_strSession, message.id);
                                            new List<TextMessageAttachment>();
                                            if ((attachmentList != null) && attachmentList.Data.success)
                                            {
                                                foreach (TextMessageAttachment attachment in attachmentList.Data.response)
                                                {
                                                    if (attachment.mimeType.Contains("image/"))
                                                    {
                                                        PictureBox box5 = new PictureBox {
                                                            ContextMenuStrip = this.contextMenuStripPictureBox,
                                                            BackColor = System.Drawing.Color.Black,
                                                            Name = message.id + "~" + attachment.storageKey,
                                                            SizeMode = PictureBoxSizeMode.Zoom
                                                        };
                                                        if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
                                                        {
                                                            box5.Location = new Point(label3.Size.Width + 10, y);
                                                        }
                                                        else
                                                        {
                                                            box5.Location = new Point(5, y);
                                                        }
                                                        if (this.appManager.m_bDisplayMMSAttachments)
                                                        {
                                                            Image image = this.appManager.m_textService.GetAttachment(this.appManager.m_strSession, attachment.storageKey);
                                                            if (image != null)
                                                            {
                                                                box5.Image = image;
                                                                box5.Height = 300;
                                                                box5.Width = 300;
                                                                y += 300;
                                                            }
                                                            else
                                                            {
                                                                box5.Image = Resources.LoadFail;
                                                                box5.BackColor = System.Drawing.Color.Transparent;
                                                                box5.Height = 50;
                                                                box5.Width = 50;
                                                                y += 50;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            box5.Image = Resources.Download;
                                                            box5.BackColor = System.Drawing.Color.Transparent;
                                                            box5.Height = 50;
                                                            box5.Width = 50;
                                                            y += 50;
                                                        }
                                                        this.pnMessages.Controls.Add(box5);
                                                        box2 = box5;
                                                        y += 5;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            this.strError = "Error displaying MMS message attachment: " + exception.Message;
                                        }
                                    }
                                    y += 10;
                                }
                            }
                            if (box2 != null)
                            {
                                this.pnMessages.ScrollControlIntoView(box2);
                            }
                            else
                            {
                                this.pnMessages.ScrollControlIntoView(activeControl);
                            }
                            label.Visible = false;
                            if (flag)
                            {
                                this.appManager.LoadUpdates(true);
                            }
                        }
                        else
                        {
                            this.ResetMessageForm("No Messages found...");
                        }
                    }
                }
                catch (Exception exception2)
                {
                    this.strError = "Error getting text messages for conversation: " + exception2.Message;
                }
                this.comboBoxConversationAction.Visible = true;
                this.linkLabelEditContact.Visible = true;
                this.buttonSend.Visible = true;
                this.pictureBoxLink.Visible = true;
                this.textBoxMessage.Visible = true;
                this.dateTimePickerScheduleDate.Visible = true;
                this.labelCharCount.Visible = true;
                if (this.appManager.m_bEnableSignature && (this.textBoxMessage.Text.Length == 0))
                {
                    this.textBoxMessage.Text = "\r\n" + this.appManager.m_strSignature;
                    this.textBoxMessage.Select(0, 0);
                }
                if (!string.IsNullOrEmpty(this.strError))
                {
                    this.appManager.ShowBalloon(this.strError, 5);
                    this.appManager.m_strCurentConversationFingerprint = string.Empty;
                    this.appManager.m_strCurrentContactAddress = string.Empty;
                    this.appManager.m_nCurrentContactID = 0L;
                    this.ResetMessageForm("There was an issue getting this item, please make sure 'Include Contacts and Conversations in Refresh' is set in the Options Menu and hit the Refesh button to rest and select a different item...");
                    this.strError = string.Empty;
                }
                this.bConversationLoading = false;
            }
        }

        public void DisplayConversationHeader()
        {
            Contact contactByID = null;
            this.textBoxContactPhone.Visible = true;
            this.textBoxContactName.Visible = true;
            this.textBoxContactName.BackColor = ColorTranslator.FromHtml("#E8FFCC");
            this.textBoxContactPhone.BackColor = ColorTranslator.FromHtml("#E8FFCC");
            this.tableLayoutPanelContactName.BackColor = ColorTranslator.FromHtml("#E8FFCC");
            this.tableLayoutPanelContactPhone.BackColor = ColorTranslator.FromHtml("#E8FFCC");
            if (this.appManager.m_nCurrentContactID != 0)
            {
                contactByID = this.appManager.GetContactByID(this.appManager.m_nCurrentContactID);
                if (contactByID == null)
                {
                    this.textBoxContactPhone.Text = this.appManager.FormatPhone(this.appManager.m_strCurrentContactAddress);
                    this.textBoxContactName.Text = "Unknown";
                }
                else
                {
                    this.textBoxContactPhone.Text = this.appManager.FormatPhone(contactByID.mobileNumber);
                    this.textBoxContactName.Text = contactByID.firstName + " " + contactByID.lastName;
                    if (this.textBoxContactName.Text == " ")
                    {
                        this.textBoxContactName.Text = "Unnamed";
                    }
                }
            }
            else
            {
                this.ResetMessageForm(null);
            }
        }

        public void DisplayConversatoinList()
        {
            if (!this.bConversationListLoading)
            {
                this.bConversationListLoading = true;
                try
                {
                    ConversationItem item;
                    this.bStopConversationListSelectIndexChange = true;
                    if (this.appManager.m_lsConversation == null)
                    {
                        goto Label_092B;
                    }
                    this.appManager.m_lsConversation = (from c in this.appManager.m_lsConversation
                        orderby c.lastMessageDate descending
                        select c).ToList<Conversation>();
                    if (this.listBoxConversationList.Items.Count > 0)
                    {
                        this.listBoxConversationList.Items.Clear();
                    }
                    int num = 0;
                    int num2 = 0;
                    int num3 = 0;
                    bool flag = false;
                    string str = this.textBoxSearch.Text.Trim();
                    foreach (Conversation conversation in this.appManager.m_lsConversation)
                    {
                        DateTime time;
                        string str2 = string.Empty;
                        string str3 = string.Empty;
                        int unreadCount = 0;
                        Contact contactByID = null;
                        if (conversation.lastContactId == 0)
                        {
                            str3 = this.appManager.FormatPhone(conversation.address);
                            if (str3.Length > 14)
                            {
                                str2 = "Multiple Recipients";
                            }
                            else
                            {
                                str2 = "Unknown";
                            }
                        }
                        else
                        {
                            contactByID = this.appManager.GetContactByID(conversation.lastContactId);
                            if (contactByID != null)
                            {
                                str3 = this.appManager.FormatPhone(contactByID.mobileNumber);
                                str2 = contactByID.firstName + " " + contactByID.lastName;
                                if (str2 == " ")
                                {
                                    str2 = "Unnamed";
                                }
                            }
                            else
                            {
                                str2 = "Unknown";
                                str3 = this.appManager.FormatPhone(conversation.address);
                            }
                        }
                        unreadCount = conversation.unreadCount;
                        DateTime.TryParse(conversation.lastMessageDate, out time);
                        item = new ConversationItem {
                            fingerprint = conversation.fingerprint,
                            contactID = conversation.lastContactId,
                            contactAddress = conversation.address,
                            contactName = str2,
                            contactPhone = str3,
                            dtLastDate = new DateTime?(time),
                            unreadCount = unreadCount
                        };
                        ConversationItem conversationItem = item;
                        if (string.IsNullOrEmpty(str))
                        {
                            this.listBoxConversationList.Items.Add(conversationItem);
                            num++;
                        }
                        else
                        {
                            string str4 = this.appManager.FormatContactAddress(conversationItem.contactPhone, true, false);
                            string str5 = this.appManager.FormatAlphaNumeric(str);
                            if (conversationItem.contactName.ToLower().Contains(str.ToLower()))
                            {
                                this.listBoxConversationList.Items.Add(conversationItem);
                                num++;
                            }
                            else if (conversationItem.contactPhone.ToLower().Contains(str.ToLower()))
                            {
                                this.listBoxConversationList.Items.Add(conversationItem);
                                num++;
                            }
                            else if ((str5 != "") && str4.Contains(str5))
                            {
                                this.listBoxConversationList.Items.Add(conversationItem);
                                num++;
                            }
                        }
                        if (this.comboBoxFilter.SelectedItem == "Only In")
                        {
                            flag = true;
                            this.comboBoxFilter.BackColor = System.Drawing.Color.Yellow;
                            ConversationMetaData data = this.appManager.m_lsConversationMetaData.Find(var => var.fingerprint == conversationItem.fingerprint);
                            if (data != null)
                            {
                                if (data.lastMessageDirection != "In")
                                {
                                    this.listBoxConversationList.Items.Remove(conversationItem);
                                }
                            }
                            else
                            {
                                this.listBoxConversationList.Items.Remove(conversationItem);
                            }
                        }
                        else if (this.comboBoxFilter.SelectedItem == "Only Out")
                        {
                            flag = true;
                            this.comboBoxFilter.BackColor = System.Drawing.Color.Yellow;
                            ConversationMetaData data2 = this.appManager.m_lsConversationMetaData.Find(var => var.fingerprint == conversationItem.fingerprint);
                            if (data2 != null)
                            {
                                if (data2.lastMessageDirection == "In")
                                {
                                    this.listBoxConversationList.Items.Remove(conversationItem);
                                }
                            }
                            else
                            {
                                this.listBoxConversationList.Items.Remove(conversationItem);
                            }
                        }
                        else if (this.comboBoxFilter.SelectedItem == "Only Error")
                        {
                            flag = true;
                            this.comboBoxFilter.BackColor = System.Drawing.Color.Yellow;
                            ConversationMetaData data3 = this.appManager.m_lsConversationMetaData.Find(var => var.fingerprint == conversationItem.fingerprint);
                            if (data3 != null)
                            {
                                if (!data3.lastMessageIsError)
                                {
                                    this.listBoxConversationList.Items.Remove(conversationItem);
                                }
                            }
                            else
                            {
                                this.listBoxConversationList.Items.Remove(conversationItem);
                            }
                        }
                        else if (this.comboBoxFilter.SelectedItem == "Only Unread")
                        {
                            flag = true;
                            this.comboBoxFilter.BackColor = System.Drawing.Color.Yellow;
                            if (conversationItem.unreadCount < 1)
                            {
                                this.listBoxConversationList.Items.Remove(conversationItem);
                            }
                        }
                        else
                        {
                            System.Drawing.Color color = new System.Drawing.Color();
                            this.comboBoxFilter.BackColor = color;
                        }
                        if (conversationItem.fingerprint == this.appManager.m_strCurentConversationFingerprint)
                        {
                            this.listBoxConversationList.SelectedItem = conversationItem;
                        }
                        num3 += conversationItem.unreadCount;
                    }
                    if (((this.appManager.m_lsContact != null) && !string.IsNullOrEmpty(str)) && !flag)
                    {
                        using (List<Contact>.Enumerator enumerator2 = this.appManager.m_lsContact.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                Contact contact = enumerator2.Current;
                                if (this.appManager.m_lsConversation.Find(var => var.lastContactId == contact.id) == null)
                                {
                                    string str6 = string.Empty;
                                    string str7 = string.Empty;
                                    str7 = this.appManager.FormatPhone(contact.mobileNumber);
                                    str6 = contact.firstName + " " + contact.lastName;
                                    if (str6 == " ")
                                    {
                                        str6 = "Unnamed";
                                    }
                                    item = new ConversationItem {
                                        fingerprint = "0",
                                        contactID = contact.id,
                                        contactAddress = contact.address,
                                        contactName = str6,
                                        contactPhone = str7
                                    };
                                    item.dtLastDate = null;
                                    item.unreadCount = 0;
                                    ConversationItem item2 = item;
                                    string str8 = this.appManager.FormatContactAddress(item2.contactPhone, false, false);
                                    string str9 = this.appManager.FormatAlphaNumeric(str);
                                    if (item2.contactName.ToLower().Contains(str.ToLower()))
                                    {
                                        this.listBoxConversationList.Items.Add(item2);
                                        num2++;
                                    }
                                    else if (item2.contactPhone.ToLower().Contains(str.ToLower()))
                                    {
                                        this.listBoxConversationList.Items.Add(item2);
                                        num2++;
                                    }
                                    else if ((str9 != "ptn:/") && str8.Contains(str9))
                                    {
                                        this.listBoxConversationList.Items.Add(item2);
                                        num2++;
                                    }
                                    if (num2 == this.appManager.m_nContactLimit)
                                    {
                                        goto Label_0818;
                                    }
                                    if (item2.contactID == this.appManager.m_nCurrentContactID)
                                    {
                                        this.listBoxConversationList.SelectedItem = item2;
                                    }
                                }
                            }
                        }
                    }
                Label_0818:
                    if (this.appManager.m_bKeepConversationFocus && (this.listBoxConversationList.SelectedIndices.Count != 0))
                    {
                        this.listBoxConversationList.TopIndex = this.listBoxConversationList.SelectedIndex;
                    }
                    else
                    {
                        this.listBoxConversationList.TopIndex = 0;
                    }
                    if (num3 > this.appManager.m_lsUnReadMessages.Count<TextMessage>())
                    {
                        this.labelUnreadCount.Text = num3.ToString();
                    }
                    else
                    {
                        this.labelUnreadCount.Text = this.appManager.m_lsUnReadMessages.Count<TextMessage>().ToString();
                    }
                    this.labelConversationsCount.Text = this.listBoxConversationList.Items.Count.ToString() + " Items displayed";
                    if (this.appManager.m_lsConversation.Count >= this.appManager.m_nConversationLimit)
                    {
                        this.linkLabelMoreConversations.Visible = this.pictureBoxConversationCountLock.Visible = true;
                    }
                    else
                    {
                        this.linkLabelMoreConversations.Visible = this.pictureBoxConversationCountLock.Visible = false;
                    }
                Label_092B:
                    if (this.appManager.m_lsConversation.Count == 0)
                    {
                        this.labelLoadingConversation.Visible = true;
                    }
                    else
                    {
                        this.labelLoadingConversation.Visible = false;
                    }
                    this.bStopConversationListSelectIndexChange = false;
                }
                catch (Exception exception)
                {
                    this.appManager.ShowBalloon("Exception displaying text message conversation list: " + exception.Message, 5);
                }
                this.bConversationListLoading = false;
            }
        }

        private void displayMMSAttachmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "DisplayMMSAttachments", this.displayMMSAttachmentsToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Display MMS Attachments save error: " + errorMessage;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bDisplayMMSAttachments = this.displayMMSAttachmentsToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Display MMS Attachments save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 5);
            }
        }

        public void DisplayProcessingMessage(string strProcessingMessage = null)
        {
            this.labelProcessing.BackColor = ColorTranslator.FromHtml("#93FF14");
            this.labelProcessing.Text = "Processing Enabled";
            this.labelProcessing.Visible = false;
            if (!string.IsNullOrEmpty(strProcessingMessage))
            {
                this.labelProcessing.Text = strProcessingMessage;
                this.labelProcessing.Visible = true;
            }
            else
            {
                if (this.appManager.m_bEnableKeywordProcessing)
                {
                    this.labelProcessing.Visible = true;
                }
                if (this.appManager.m_bEnableGroupScheduleProcessing)
                {
                    this.labelProcessing.Visible = true;
                }
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

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MMSImage image = this.appManager.m_textService.GetAttachment(this.appManager.m_strSession, this.strImageStorageKey, this.nSelectedMessageID);
                if (image != null)
                {
                    if (!string.IsNullOrEmpty(image.ext))
                    {
                        string filename = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\" + this.nSelectedMessageID.ToString() + image.ext;
                        image.image.Save(filename);
                        Process.Start(filename);
                    }
                    else
                    {
                        this.strError = "File is not a supported format: .bmp, .gif, .png, .jpg";
                    }
                }
            }
            catch (Exception exception)
            {
                this.strError = "Error downloading message attachment: " + exception.Message;
            }
            if (!string.IsNullOrEmpty(this.strError))
            {
                this.appManager.ShowBalloon(this.strError, 5);
                this.strError = string.Empty;
            }
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

        public void ExportConversation()
        {
            this.strError = string.Empty;
            string path = string.Empty;
            ConversationResponse response = this.appManager.m_textService.GetConversation(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession, 0, 0x3e7).Data.response;
            this.lsMessagesWorking = response.messages;
            this.lsMessagesWorking = (from c in this.lsMessagesWorking
                orderby c.dateCreated, c.id
                select c).ToList<TextMessage>();
            string str2 = "Entire Conversation of " + this.lsMessagesWorking.Count.ToString() + " Messages";
            this.saveFileDialogPrintConversation.Title = "Export " + this.appManager.m_strApplicationName + " Conversation";
            this.saveFileDialogPrintConversation.FileName = this.appManager.FormatFileName(this.textBoxContactName.Text);
            if (this.saveFileDialogPrintConversation.ShowDialog(this) != DialogResult.Cancel)
            {
                try
                {
                    path = this.saveFileDialogPrintConversation.FileName;
                    StreamWriter writer = new StreamWriter(path);
                    writer.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                    writer.WriteLine("<html><head><title>" + this.appManager.m_strApplicationName + " Conversation Export</title></head><body>");
                    writer.WriteLine("<H1>" + this.textBoxContactName.Text + " " + this.textBoxContactPhone.Text + "</H1>");
                    writer.WriteLine("<H3>" + str2 + " &nbsp; &nbsp; &nbsp; &nbsp; Date Created: " + DateTime.Now.ToString() + "</H3>");
                    writer.WriteLine("<table cellpadding=\"10\">");
                    writer.WriteLine("<tr><th>Date</th><th>Phone Number</th><th>Contact</th><th>Message</th></tr>");
                    foreach (TextMessage message in this.lsMessagesWorking)
                    {
                        DateTime time2;
                        DateTime.TryParse(message.dateCreated, out time2);
                        if (1 != 0)
                        {
                            writer.WriteLine("<tr>");
                            if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
                            {
                                writer.WriteLine("<td nowrap=\"nowrap\">" + time2 + "</td>");
                                writer.WriteLine("<td nowrap=\"nowrap\">" + this.appManager.FormatPhone(this.appManager.m_strUserName) + "</td>");
                                writer.WriteLine("<td></td>");
                                writer.WriteLine("<td>" + message.body + "</td>");
                            }
                            else
                            {
                                writer.WriteLine("<td nowrap=\"nowrap\">" + time2 + "</td>");
                                writer.WriteLine("<td nowrap=\"nowrap\">" + this.textBoxContactPhone.Text + "</td>");
                                writer.WriteLine("<td>" + this.textBoxContactName.Text + "</td>");
                                writer.WriteLine("<td bgcolor=\"#E8FFCC\">" + message.body + "</td>");
                            }
                            writer.WriteLine("</tr>");
                        }
                    }
                    writer.WriteLine("</table>");
                    writer.WriteLine("</body></html>");
                    writer.Close();
                }
                catch (Exception exception)
                {
                    this.strError = "Error exporting conversation: " + exception.Message;
                }
                if (!string.IsNullOrEmpty(this.strError))
                {
                    this.appManager.ShowBalloon(this.strError, 5);
                    this.strError = string.Empty;
                }
                else
                {
                    this.comboBoxConversationAction_Load();
                    Process.Start(path);
                    string text = "Conversation exported to file: " + path;
                    this.appManager.ShowBalloon(text, 5);
                }
            }
        }

        private void fmMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormMessageWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormMessageHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_splitContainerMessagesDistance", this.splitContainerMessages.SplitterDistance, ref this.strError, false, RegistryValueKind.Unknown);
                this.appManager.m_strCurentConversationFingerprint = string.Empty;
                this.appManager.m_strCurrentContactAddress = string.Empty;
                this.appManager.m_nCurrentContactID = 0L;
            }
            catch
            {
            }
        }

        private void fmMessage_Load(object sender, EventArgs e)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormMessageWidth", ref num, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormMessageHeight", ref num2, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_splitContainerMessagesDistance", ref num3, ref this.strError);
                if (num2 != 0)
                {
                    base.Height = num2;
                }
                if (num != 0)
                {
                    base.Width = num;
                }
                if (num3 != 0)
                {
                    this.splitContainerMessages.SplitterDistance = num3;
                }
                this.versionToolStripMenuItem.Text = this.appManager.m_AssemblyVersion + " (Update)";
                if (this.appManager.m_bIsBranded)
                {
                    this.versionToolStripMenuItem.Text = this.appManager.m_AssemblyVersion;
                    this.versionToolStripMenuItem.Enabled = false;
                    this.tryBETAToolStripMenuItem.Visible = false;
                }
                this.labelLoadingConversation.Dock = DockStyle.Fill;
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
                if (this.appManager.m_bMMSFeature)
                {
                    this.displayMMSAttachmentsToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.displayMMSAttachmentsToolStripMenuItem.Enabled = false;
                }
                if (this.appManager.m_bConversationCountLocked)
                {
                    this.pictureBoxConversationCountLock.Image = Resources.locked;
                }
                else
                {
                    this.pictureBoxConversationCountLock.Image = Resources.openlock;
                }
                try
                {
                    this.rapidSpellAsYouTypeText.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                }
                catch (Exception)
                {
                }
                this.appManager.m_strCurentConversationFingerprint = string.Empty;
                this.appManager.m_strCurrentContactAddress = string.Empty;
                this.appManager.m_nCurrentContactID = 0L;
                this.textBoxContactPhone.Text = "";
                this.textBoxContactName.Text = "";
                this.labelProcessing.BackColor = ColorTranslator.FromHtml("#93FF14");
                this.labelProcessing.Visible = false;
                this.buttonMarkAllRead.BackColor = ColorTranslator.FromHtml("#93FF14");
                this.dateTimePickerScheduleDate.CustomFormat = "MM/dd/yyyy hh:mm tt";
                this.comboBoxConversationAction_Load();
                this.comboBoxFilter_Load();
                this.DisplayConversatoinList();
                this.DisplayProcessingMessage(this.appManager.m_strCurentProcessingMessage);
                this.listBoxConversationList.DrawMode = DrawMode.OwnerDrawVariable;
                this.listBoxConversationList.MeasureItem += new MeasureItemEventHandler(this.listBoxConversationList_MeasureItem);
                this.listBoxConversationList.DrawItem += new DrawItemEventHandler(this.listBoxConversationList_DrawItem);
                this.listBoxConversationList.ValueMember = "fingerprint";
                this.listBoxConversationList.DisplayMember = "item";
                this.LoadMessageTemplateMenu();
                string[] textArray1 = new string[] { this.appManager.m_strApplicationName, " Messages ", this.appManager.FormatPhone(this.appManager.m_strUserName), " ", this.appManager.m_strAccounTitle };
                this.Text = string.Concat(textArray1);
                base.Icon = this.appManager.iTextApp;
                this.ResetMessageForm(null);
                this.sFormSize = base.Size;
            }
            catch (Exception exception)
            {
                this.strError = "Unexpected application error while loading message window: " + exception.Message;
            }
            if (this.strError.Length > 0)
            {
                this.appManager.ShowBalloon(this.strError, 5);
            }
        }

        private void fmMessages_Activated(object sender, EventArgs e)
        {
            if (this.appManager.m_bRefreshMessageFormConversationList)
            {
                this.DisplayConversatoinList();
                if (!string.IsNullOrEmpty(this.appManager.m_strCurentConversationFingerprint))
                {
                    this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, false);
                }
                this.appManager.m_bRefreshMessageFormConversationList = false;
            }
        }

        private void fmMessages_Resize(object sender, EventArgs e)
        {
            try
            {
                if (base.WindowState != this.fwsLastWindowState)
                {
                    this.fwsLastWindowState = base.WindowState;
                    if (base.WindowState == FormWindowState.Maximized)
                    {
                        this.DisplayConversation(null, false, false);
                    }
                    if (base.WindowState == FormWindowState.Normal)
                    {
                        this.DisplayConversation(null, false, false);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void fmMessages_ResizeEnd(object sender, EventArgs e)
        {
            try
            {
                if (this.sFormSize != base.Size)
                {
                    this.sFormSize = base.Size;
                    this.DisplayConversation(null, false, false);
                }
            }
            catch (Exception)
            {
            }
        }

        private void forwardTextMessage_Click(object sender, EventArgs e)
        {
            this.appManager.m_strForwardMessage = this.textBoxSelectedMessage.Text;
            this.appManager.ShowNewMessage();
        }

        private void generalHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.LaunchWebsite(this.appManager.m_strHelpURL);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmMessages));
            this.splitContainerMessages = new SplitContainer();
            this.labelTextBoxSearchHint = new Label();
            this.pictureBoxConversationCountLock = new PictureBox();
            this.labelUnread = new Label();
            this.linkLabelMoreConversations = new LinkLabel();
            this.comboBoxFilter = new ComboBox();
            this.buttonClear = new Button();
            this.textBoxSearch = new TextBox();
            this.labelConversationsCount = new Label();
            this.labelUnreadCount = new Label();
            this.labelLoadingConversation = new Label();
            this.listBoxConversationList = new ListBox();
            this.tableLayoutPanelMessages = new TableLayoutPanel();
            this.pnSend = new Panel();
            this.checkBoxFullRefresh = new CheckBox();
            this.labelAttRemove = new Label();
            this.pictureBoxLink = new PictureBox();
            this.pictureBoxAttachment = new PictureBox();
            this.buttonMarkAllRead = new Button();
            this.dateTimePickerScheduleDate = new DateTimePicker();
            this.buttonRefresh = new Button();
            this.buttonSend = new Button();
            this.labelCharCount = new Label();
            this.textBoxMessage = new TextBox();
            this.pnMessages = new Panel();
            this.pnMoreMessages = new Panel();
            this.labelMesssageCount = new Label();
            this.linkLabelMoreMessages = new LinkLabel();
            this.tableLayoutPanelContactName = new TableLayoutPanel();
            this.textBoxContactName = new TextBox();
            this.linkLabelEditContact = new LinkLabel();
            this.tableLayoutPanelContactPhone = new TableLayoutPanel();
            this.textBoxContactPhone = new TextBox();
            this.comboBoxConversationAction = new ComboBox();
            this.menuStripMessage = new MenuStrip();
            this.newMessageToolStripMenuItem = new ToolStripMenuItem();
            this.messageTemplatesToolStripMenuItem = new ToolStripMenuItem();
            this.editMessageTemplatesToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.editsToolStripMenuItem = new ToolStripMenuItem();
            this.manageGroupsToolStripMenuItem = new ToolStripMenuItem();
            this.manageContactsToolStripMenuItem = new ToolStripMenuItem();
            this.editKeywordAutoResponseToolStripMenuItem = new ToolStripMenuItem();
            this.editGroupScheduleToolStripMenuItem = new ToolStripMenuItem();
            this.optionsToolStripMenuItem = new ToolStripMenuItem();
            this.settingsToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.controlEnterToolStripMenuItem = new ToolStripMenuItem();
            this.requireClickToMarkMessageReadToolStripMenuItem = new ToolStripMenuItem();
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem = new ToolStripMenuItem();
            this.keepSelectedConversationInFocusToolStripMenuItem = new ToolStripMenuItem();
            this.displayMMSAttachmentsToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.generalHelpToolStripMenuItem = new ToolStripMenuItem();
            this.settingsHelpToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator3 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.syncFeaturesToolStripMenuItem = new ToolStripMenuItem();
            this.versionToolStripMenuItem = new ToolStripMenuItem();
            this.tryBETAToolStripMenuItem = new ToolStripMenuItem();
            this.logOutToolStripMenuItem = new ToolStripMenuItem();
            this.saveFileDialogPrintConversation = new SaveFileDialog();
            this.contextMenuStripMessageText = new ContextMenuStrip(this.components);
            this.forwardTextMessage = new ToolStripMenuItem();
            this.copyTextToolStripMenuItem = new ToolStripMenuItem();
            this.copySelectedTextToolStripMenuItem = new ToolStripMenuItem();
            this.deleteToolStripMenuItem = new ToolStripMenuItem();
            this.markAsReadToolStripMenuItem = new ToolStripMenuItem();
            this.toolTipMarkAllRead = new ToolTip(this.components);
            this.labelProcessing = new Label();
            this.openFileDialog = new OpenFileDialog();
            this.contextMenuStripPictureBox = new ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem = new ToolStripMenuItem();
            this.rapidSpellAsYouTypeText = new RapidSpellAsYouType(this.components);
            this.splitContainerMessages.Panel1.SuspendLayout();
            this.splitContainerMessages.Panel2.SuspendLayout();
            this.splitContainerMessages.SuspendLayout();
            ((ISupportInitialize) this.pictureBoxConversationCountLock).BeginInit();
            this.tableLayoutPanelMessages.SuspendLayout();
            this.pnSend.SuspendLayout();
            ((ISupportInitialize) this.pictureBoxLink).BeginInit();
            ((ISupportInitialize) this.pictureBoxAttachment).BeginInit();
            this.pnMoreMessages.SuspendLayout();
            this.tableLayoutPanelContactName.SuspendLayout();
            this.tableLayoutPanelContactPhone.SuspendLayout();
            this.menuStripMessage.SuspendLayout();
            this.contextMenuStripMessageText.SuspendLayout();
            this.contextMenuStripPictureBox.SuspendLayout();
            base.SuspendLayout();
            this.splitContainerMessages.Dock = DockStyle.Fill;
            this.splitContainerMessages.FixedPanel = FixedPanel.Panel1;
            this.splitContainerMessages.Location = new Point(0, 0x18);
            this.splitContainerMessages.MinimumSize = new Size(0x20c, 0x1b5);
            this.splitContainerMessages.Name = "splitContainerMessages";
            this.splitContainerMessages.Panel1.Controls.Add(this.labelTextBoxSearchHint);
            this.splitContainerMessages.Panel1.Controls.Add(this.pictureBoxConversationCountLock);
            this.splitContainerMessages.Panel1.Controls.Add(this.labelUnread);
            this.splitContainerMessages.Panel1.Controls.Add(this.linkLabelMoreConversations);
            this.splitContainerMessages.Panel1.Controls.Add(this.comboBoxFilter);
            this.splitContainerMessages.Panel1.Controls.Add(this.buttonClear);
            this.splitContainerMessages.Panel1.Controls.Add(this.textBoxSearch);
            this.splitContainerMessages.Panel1.Controls.Add(this.labelConversationsCount);
            this.splitContainerMessages.Panel1.Controls.Add(this.labelUnreadCount);
            this.splitContainerMessages.Panel1.Controls.Add(this.labelLoadingConversation);
            this.splitContainerMessages.Panel1.Controls.Add(this.listBoxConversationList);
            this.splitContainerMessages.Panel1MinSize = 170;
            this.splitContainerMessages.Panel2.Controls.Add(this.tableLayoutPanelMessages);
            this.splitContainerMessages.Panel2MinSize = 0x159;
            this.splitContainerMessages.Size = new Size(0x248, 0x219);
            this.splitContainerMessages.SplitterDistance = 0xe1;
            this.splitContainerMessages.SplitterWidth = 5;
            this.splitContainerMessages.TabIndex = 0;
            this.splitContainerMessages.TabStop = false;
            this.labelTextBoxSearchHint.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelTextBoxSearchHint.AutoSize = true;
            this.labelTextBoxSearchHint.BackColor = SystemColors.Window;
            this.labelTextBoxSearchHint.Font = new Font("Arial", 9f, FontStyle.Italic);
            this.labelTextBoxSearchHint.ForeColor = SystemColors.ControlDark;
            this.labelTextBoxSearchHint.Location = new Point(11, 0x1fa);
            this.labelTextBoxSearchHint.Margin = new Padding(0);
            this.labelTextBoxSearchHint.Name = "labelTextBoxSearchHint";
            this.labelTextBoxSearchHint.Size = new Size(0x58, 15);
            this.labelTextBoxSearchHint.TabIndex = 0x12;
            this.labelTextBoxSearchHint.Text = "Type to Search";
            this.labelTextBoxSearchHint.TextAlign = ContentAlignment.MiddleLeft;
            this.labelTextBoxSearchHint.Click += new EventHandler(this.labelTextBoxSearchHint_Click);
            this.pictureBoxConversationCountLock.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.pictureBoxConversationCountLock.Image = Resources.openlock;
            this.pictureBoxConversationCountLock.Location = new Point(0xa2, 4);
            this.pictureBoxConversationCountLock.Name = "pictureBoxConversationCountLock";
            this.pictureBoxConversationCountLock.Size = new Size(0x10, 0x10);
            this.pictureBoxConversationCountLock.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxConversationCountLock.TabIndex = 0x11;
            this.pictureBoxConversationCountLock.TabStop = false;
            this.pictureBoxConversationCountLock.Click += new EventHandler(this.pictureBoxConversationCountLock_Click);
            this.labelUnread.AutoSize = true;
            this.labelUnread.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelUnread.Location = new Point(8, 4);
            this.labelUnread.Name = "labelUnread";
            this.labelUnread.Size = new Size(0x35, 0x10);
            this.labelUnread.TabIndex = 0x10;
            this.labelUnread.Text = "Unread:";
            this.linkLabelMoreConversations.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.linkLabelMoreConversations.AutoSize = true;
            this.linkLabelMoreConversations.LinkColor = System.Drawing.Color.Olive;
            this.linkLabelMoreConversations.Location = new Point(0xb2, 7);
            this.linkLabelMoreConversations.Name = "linkLabelMoreConversations";
            this.linkLabelMoreConversations.Size = new Size(40, 13);
            this.linkLabelMoreConversations.TabIndex = 11;
            this.linkLabelMoreConversations.TabStop = true;
            this.linkLabelMoreConversations.Text = "More...";
            this.linkLabelMoreConversations.Visible = false;
            this.linkLabelMoreConversations.VisitedLinkColor = System.Drawing.Color.Olive;
            this.linkLabelMoreConversations.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelMoreConversations_LinkClicked);
            this.comboBoxFilter.AllowDrop = true;
            this.comboBoxFilter.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxFilter.FlatStyle = FlatStyle.Flat;
            this.comboBoxFilter.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxFilter.FormattingEnabled = true;
            this.comboBoxFilter.ItemHeight = 15;
            this.comboBoxFilter.Location = new Point(8, 0x1d7);
            this.comboBoxFilter.Name = "comboBoxFilter";
            this.comboBoxFilter.Size = new Size(210, 0x17);
            this.comboBoxFilter.TabIndex = 14;
            this.comboBoxFilter.SelectionChangeCommitted += new EventHandler(this.comboBoxFilter_SelectionChangeCommitted);
            this.comboBoxFilter.Leave += new EventHandler(this.comboBoxFilter_Leave);
            this.buttonClear.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonClear.Enabled = false;
            this.buttonClear.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonClear.Location = new Point(0xa2, 0x1f3);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new Size(0x38, 0x1b);
            this.buttonClear.TabIndex = 5;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new EventHandler(this.buttonClear_Click);
            this.textBoxSearch.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.textBoxSearch.BorderStyle = BorderStyle.None;
            this.textBoxSearch.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxSearch.Location = new Point(8, 0x1f8);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new Size(0x94, 0x12);
            this.textBoxSearch.TabIndex = 4;
            this.textBoxSearch.TextChanged += new EventHandler(this.textBoxSearch_TextChanged);
            this.textBoxSearch.KeyPress += new KeyPressEventHandler(this.textBoxSearch_KeyPress);
            this.labelConversationsCount.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.labelConversationsCount.AutoSize = true;
            this.labelConversationsCount.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.labelConversationsCount.Location = new Point(8, 0x15);
            this.labelConversationsCount.Name = "labelConversationsCount";
            this.labelConversationsCount.Size = new Size(0x98, 14);
            this.labelConversationsCount.TabIndex = 7;
            this.labelConversationsCount.Text = "50 Displayed conversations...";
            this.labelUnreadCount.AutoSize = true;
            this.labelUnreadCount.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelUnreadCount.Location = new Point(0x43, 4);
            this.labelUnreadCount.Name = "labelUnreadCount";
            this.labelUnreadCount.Size = new Size(15, 0x10);
            this.labelUnreadCount.TabIndex = 5;
            this.labelUnreadCount.Text = "0";
            this.labelUnreadCount.TextChanged += new EventHandler(this.labelUnreadCount_TextChanged);
            this.labelLoadingConversation.BackColor = System.Drawing.Color.Transparent;
            this.labelLoadingConversation.Font = new Font("Arial", 12f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelLoadingConversation.Location = new Point(0x21, 0x87);
            this.labelLoadingConversation.Name = "labelLoadingConversation";
            this.labelLoadingConversation.Size = new Size(90, 0xc3);
            this.labelLoadingConversation.TabIndex = 15;
            this.labelLoadingConversation.Text = "Loading Conversations...";
            this.labelLoadingConversation.TextAlign = ContentAlignment.MiddleCenter;
            this.listBoxConversationList.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listBoxConversationList.BackColor = SystemColors.ControlLight;
            this.listBoxConversationList.BorderStyle = BorderStyle.None;
            this.listBoxConversationList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.listBoxConversationList.FormattingEnabled = true;
            this.listBoxConversationList.ItemHeight = 0x11;
            this.listBoxConversationList.Location = new Point(11, 40);
            this.listBoxConversationList.Name = "listBoxConversationList";
            this.listBoxConversationList.ScrollAlwaysVisible = true;
            this.listBoxConversationList.Size = new Size(0xcf, 0x198);
            this.listBoxConversationList.TabIndex = 8;
            this.listBoxConversationList.TabStop = false;
            this.listBoxConversationList.SelectedIndexChanged += new EventHandler(this.listBoxConversationList_SelectedIndexChanged);
            this.tableLayoutPanelMessages.ColumnCount = 1;
            this.tableLayoutPanelMessages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelMessages.Controls.Add(this.pnSend, 0, 4);
            this.tableLayoutPanelMessages.Controls.Add(this.pnMessages, 0, 3);
            this.tableLayoutPanelMessages.Controls.Add(this.pnMoreMessages, 0, 2);
            this.tableLayoutPanelMessages.Controls.Add(this.tableLayoutPanelContactName, 0, 0);
            this.tableLayoutPanelMessages.Controls.Add(this.tableLayoutPanelContactPhone, 0, 1);
            this.tableLayoutPanelMessages.Dock = DockStyle.Fill;
            this.tableLayoutPanelMessages.Location = new Point(0, 0);
            this.tableLayoutPanelMessages.Margin = new Padding(0);
            this.tableLayoutPanelMessages.Name = "tableLayoutPanelMessages";
            this.tableLayoutPanelMessages.RowCount = 5;
            this.tableLayoutPanelMessages.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
            this.tableLayoutPanelMessages.RowStyles.Add(new RowStyle(SizeType.Absolute, 27f));
            this.tableLayoutPanelMessages.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            this.tableLayoutPanelMessages.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelMessages.RowStyles.Add(new RowStyle(SizeType.Absolute, 172f));
            this.tableLayoutPanelMessages.Size = new Size(0x162, 0x219);
            this.tableLayoutPanelMessages.TabIndex = 0x11;
            this.pnSend.Controls.Add(this.checkBoxFullRefresh);
            this.pnSend.Controls.Add(this.labelAttRemove);
            this.pnSend.Controls.Add(this.pictureBoxLink);
            this.pnSend.Controls.Add(this.pictureBoxAttachment);
            this.pnSend.Controls.Add(this.buttonMarkAllRead);
            this.pnSend.Controls.Add(this.dateTimePickerScheduleDate);
            this.pnSend.Controls.Add(this.buttonRefresh);
            this.pnSend.Controls.Add(this.buttonSend);
            this.pnSend.Controls.Add(this.labelCharCount);
            this.pnSend.Controls.Add(this.textBoxMessage);
            this.pnSend.Dock = DockStyle.Fill;
            this.pnSend.Location = new Point(3, 0x170);
            this.pnSend.Name = "pnSend";
            this.pnSend.Size = new Size(0x15c, 0xa6);
            this.pnSend.TabIndex = 3;
            this.checkBoxFullRefresh.AutoSize = true;
            this.checkBoxFullRefresh.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.checkBoxFullRefresh.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.checkBoxFullRefresh.Location = new Point(8, 0x20);
            this.checkBoxFullRefresh.Name = "checkBoxFullRefresh";
            this.checkBoxFullRefresh.Size = new Size(0x57, 0x12);
            this.checkBoxFullRefresh.TabIndex = 8;
            this.checkBoxFullRefresh.Text = "Full Refresh";
            this.checkBoxFullRefresh.UseVisualStyleBackColor = true;
            this.labelAttRemove.AutoSize = true;
            this.labelAttRemove.BackColor = System.Drawing.Color.Transparent;
            this.labelAttRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.labelAttRemove.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelAttRemove.Location = new Point(5, 0x76);
            this.labelAttRemove.Name = "labelAttRemove";
            this.labelAttRemove.Size = new Size(0x54, 14);
            this.labelAttRemove.TabIndex = 7;
            this.labelAttRemove.Text = "Click to remove";
            this.labelAttRemove.Visible = false;
            this.labelAttRemove.Click += new EventHandler(this.labelAttRemove_Click);
            this.pictureBoxLink.Image = Resources.Paperclip;
            this.pictureBoxLink.Location = new Point(0x48, 0x85);
            this.pictureBoxLink.Name = "pictureBoxLink";
            this.pictureBoxLink.Size = new Size(20, 20);
            this.pictureBoxLink.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxLink.TabIndex = 6;
            this.pictureBoxLink.TabStop = false;
            this.pictureBoxLink.Click += new EventHandler(this.pictureBoxLink_Click);
            this.pictureBoxAttachment.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.pictureBoxAttachment.Location = new Point(8, 0x6c);
            this.pictureBoxAttachment.Name = "pictureBoxAttachment";
            this.pictureBoxAttachment.Size = new Size(0x51, 0x2e);
            this.pictureBoxAttachment.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxAttachment.TabIndex = 5;
            this.pictureBoxAttachment.TabStop = false;
            this.pictureBoxAttachment.Click += new EventHandler(this.labelAttRemove_Click);
            this.buttonMarkAllRead.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonMarkAllRead.BackColor = System.Drawing.Color.LimeGreen;
            this.buttonMarkAllRead.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonMarkAllRead.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonMarkAllRead.Location = new Point(8, 0x34);
            this.buttonMarkAllRead.Name = "buttonMarkAllRead";
            this.buttonMarkAllRead.Size = new Size(0x51, 50);
            this.buttonMarkAllRead.TabIndex = 4;
            this.buttonMarkAllRead.Text = "Clear Unread";
            this.toolTipMarkAllRead.SetToolTip(this.buttonMarkAllRead, "Click to mark all messages in this conversation as read.");
            this.buttonMarkAllRead.UseVisualStyleBackColor = false;
            this.buttonMarkAllRead.Visible = false;
            this.buttonMarkAllRead.Click += new EventHandler(this.buttonMarkAllRead_Click);
            this.dateTimePickerScheduleDate.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            this.dateTimePickerScheduleDate.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.dateTimePickerScheduleDate.Format = DateTimePickerFormat.Custom;
            this.dateTimePickerScheduleDate.Location = new Point(0x5e, 0x84);
            this.dateTimePickerScheduleDate.MinDate = new DateTime(0x76c, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerScheduleDate.Name = "dateTimePickerScheduleDate";
            this.dateTimePickerScheduleDate.Size = new Size(0x7f, 0x16);
            this.dateTimePickerScheduleDate.TabIndex = 1;
            this.dateTimePickerScheduleDate.Visible = false;
            this.buttonRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonRefresh.Location = new Point(8, 5);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new Size(0x51, 0x1b);
            this.buttonRefresh.TabIndex = 3;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
            this.buttonSend.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSend.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSend.Location = new Point(0x114, 0x80);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new Size(0x38, 0x1b);
            this.buttonSend.TabIndex = 2;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Visible = false;
            this.buttonSend.Click += new EventHandler(this.buttonSend_Click);
            this.labelCharCount.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.labelCharCount.AutoSize = true;
            this.labelCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelCharCount.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelCharCount.Location = new Point(0xde, 0x87);
            this.labelCharCount.MinimumSize = new Size(0x37, 0);
            this.labelCharCount.Name = "labelCharCount";
            this.labelCharCount.Size = new Size(0x37, 15);
            this.labelCharCount.TabIndex = 1;
            this.labelCharCount.Text = "250/250";
            this.labelCharCount.TextAlign = ContentAlignment.MiddleRight;
            this.labelCharCount.Visible = false;
            this.textBoxMessage.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.textBoxMessage.BorderStyle = BorderStyle.None;
            this.textBoxMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxMessage.Location = new Point(0x5f, 3);
            this.textBoxMessage.MaxLength = 250;
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessage.Size = new Size(0xf4, 0x76);
            this.textBoxMessage.TabIndex = 0;
            this.textBoxMessage.Visible = false;
            this.textBoxMessage.TextChanged += new EventHandler(this.textBoxMessage_TextChanged);
            this.textBoxMessage.KeyPress += new KeyPressEventHandler(this.textBoxMessage_KeyPress);
            this.pnMessages.AutoScroll = true;
            this.pnMessages.BackColor = SystemColors.ControlLight;
            this.pnMessages.Cursor = Cursors.Default;
            this.pnMessages.Dock = DockStyle.Fill;
            this.pnMessages.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.pnMessages.Location = new Point(3, 0x4e);
            this.pnMessages.MinimumSize = new Size(0x157, 0);
            this.pnMessages.Name = "pnMessages";
            this.pnMessages.Size = new Size(0x15c, 0x11c);
            this.pnMessages.TabIndex = 5;
            this.pnMoreMessages.Controls.Add(this.labelMesssageCount);
            this.pnMoreMessages.Controls.Add(this.linkLabelMoreMessages);
            this.pnMoreMessages.Dock = DockStyle.Fill;
            this.pnMoreMessages.Location = new Point(3, 0x3a);
            this.pnMoreMessages.Name = "pnMoreMessages";
            this.pnMoreMessages.Size = new Size(0x15c, 14);
            this.pnMoreMessages.TabIndex = 5;
            this.labelMesssageCount.AutoSize = true;
            this.labelMesssageCount.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.labelMesssageCount.Location = new Point(0, 1);
            this.labelMesssageCount.Name = "labelMesssageCount";
            this.labelMesssageCount.Size = new Size(0x75, 14);
            this.labelMesssageCount.TabIndex = 11;
            this.labelMesssageCount.Text = "0 Messages displayed";
            this.linkLabelMoreMessages.AutoSize = true;
            this.linkLabelMoreMessages.Dock = DockStyle.Right;
            this.linkLabelMoreMessages.LinkColor = System.Drawing.Color.Olive;
            this.linkLabelMoreMessages.Location = new Point(0x134, 0);
            this.linkLabelMoreMessages.Name = "linkLabelMoreMessages";
            this.linkLabelMoreMessages.Size = new Size(40, 13);
            this.linkLabelMoreMessages.TabIndex = 10;
            this.linkLabelMoreMessages.TabStop = true;
            this.linkLabelMoreMessages.Text = "More...";
            this.linkLabelMoreMessages.Visible = false;
            this.linkLabelMoreMessages.VisitedLinkColor = System.Drawing.Color.Olive;
            this.linkLabelMoreMessages.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelMoreMessages_LinkClicked);
            this.tableLayoutPanelContactName.ColumnCount = 2;
            this.tableLayoutPanelContactName.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelContactName.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48f));
            this.tableLayoutPanelContactName.Controls.Add(this.textBoxContactName, 0, 0);
            this.tableLayoutPanelContactName.Controls.Add(this.linkLabelEditContact, 1, 0);
            this.tableLayoutPanelContactName.Dock = DockStyle.Fill;
            this.tableLayoutPanelContactName.Location = new Point(0, 0);
            this.tableLayoutPanelContactName.Margin = new Padding(0);
            this.tableLayoutPanelContactName.Name = "tableLayoutPanelContactName";
            this.tableLayoutPanelContactName.RowCount = 1;
            this.tableLayoutPanelContactName.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelContactName.Size = new Size(0x162, 0x1c);
            this.tableLayoutPanelContactName.TabIndex = 6;
            this.textBoxContactName.BackColor = SystemColors.Control;
            this.textBoxContactName.BorderStyle = BorderStyle.None;
            this.textBoxContactName.Dock = DockStyle.Fill;
            this.textBoxContactName.Font = new Font("Arial", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.textBoxContactName.Location = new Point(3, 3);
            this.textBoxContactName.Name = "textBoxContactName";
            this.textBoxContactName.ReadOnly = true;
            this.textBoxContactName.Size = new Size(300, 0x12);
            this.textBoxContactName.TabIndex = 14;
            this.textBoxContactName.TabStop = false;
            this.textBoxContactName.Text = "Name";
            this.linkLabelEditContact.AutoSize = true;
            this.linkLabelEditContact.Dock = DockStyle.Right;
            this.linkLabelEditContact.LinkColor = System.Drawing.Color.Olive;
            this.linkLabelEditContact.Location = new Point(0x13d, 0);
            this.linkLabelEditContact.Name = "linkLabelEditContact";
            this.linkLabelEditContact.Size = new Size(0x22, 0x1c);
            this.linkLabelEditContact.TabIndex = 0x10;
            this.linkLabelEditContact.TabStop = true;
            this.linkLabelEditContact.Text = "Edit...";
            this.linkLabelEditContact.VisitedLinkColor = System.Drawing.Color.Olive;
            this.linkLabelEditContact.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelEditContact_LinkClicked);
            this.tableLayoutPanelContactPhone.ColumnCount = 2;
            this.tableLayoutPanelContactPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelContactPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 176f));
            this.tableLayoutPanelContactPhone.Controls.Add(this.textBoxContactPhone, 0, 0);
            this.tableLayoutPanelContactPhone.Controls.Add(this.comboBoxConversationAction, 1, 0);
            this.tableLayoutPanelContactPhone.Dock = DockStyle.Fill;
            this.tableLayoutPanelContactPhone.Location = new Point(0, 0x1c);
            this.tableLayoutPanelContactPhone.Margin = new Padding(0);
            this.tableLayoutPanelContactPhone.Name = "tableLayoutPanelContactPhone";
            this.tableLayoutPanelContactPhone.RowCount = 1;
            this.tableLayoutPanelContactPhone.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tableLayoutPanelContactPhone.Size = new Size(0x162, 0x1b);
            this.tableLayoutPanelContactPhone.TabIndex = 7;
            this.textBoxContactPhone.BackColor = SystemColors.Control;
            this.textBoxContactPhone.BorderStyle = BorderStyle.None;
            this.textBoxContactPhone.Dock = DockStyle.Fill;
            this.textBoxContactPhone.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxContactPhone.Location = new Point(3, 3);
            this.textBoxContactPhone.Name = "textBoxContactPhone";
            this.textBoxContactPhone.ReadOnly = true;
            this.textBoxContactPhone.Size = new Size(0xac, 0x12);
            this.textBoxContactPhone.TabIndex = 15;
            this.textBoxContactPhone.TabStop = false;
            this.textBoxContactPhone.Text = "(555) 555-5555";
            this.comboBoxConversationAction.AllowDrop = true;
            this.comboBoxConversationAction.Dock = DockStyle.Fill;
            this.comboBoxConversationAction.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxConversationAction.FlatStyle = FlatStyle.Flat;
            this.comboBoxConversationAction.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxConversationAction.FormattingEnabled = true;
            this.comboBoxConversationAction.ItemHeight = 15;
            this.comboBoxConversationAction.Location = new Point(0xb5, 3);
            this.comboBoxConversationAction.Name = "comboBoxConversationAction";
            this.comboBoxConversationAction.Size = new Size(170, 0x17);
            this.comboBoxConversationAction.TabIndex = 13;
            this.comboBoxConversationAction.Visible = false;
            this.comboBoxConversationAction.SelectedIndexChanged += new EventHandler(this.comboBoxConversationAction_SelectedIndexChanged);
            this.comboBoxConversationAction.SelectionChangeCommitted += new EventHandler(this.comboBoxConversationAction_SelectionChangeCommitted);
            this.comboBoxConversationAction.Leave += new EventHandler(this.comboBoxConversationAction_Leave);
            this.menuStripMessage.ImageScalingSize = new Size(0x18, 0x18);
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.newMessageToolStripMenuItem, this.messageTemplatesToolStripMenuItem, this.editsToolStripMenuItem, this.optionsToolStripMenuItem, this.helpToolStripMenuItem };
            this.menuStripMessage.Items.AddRange(toolStripItems);
            this.menuStripMessage.Location = new Point(0, 0);
            this.menuStripMessage.Name = "menuStripMessage";
            this.menuStripMessage.Size = new Size(0x248, 0x18);
            this.menuStripMessage.TabIndex = 2;
            this.menuStripMessage.Text = "menuStrip1";
            this.newMessageToolStripMenuItem.Name = "newMessageToolStripMenuItem";
            this.newMessageToolStripMenuItem.Size = new Size(0x5c, 20);
            this.newMessageToolStripMenuItem.Text = "&New Message";
            this.newMessageToolStripMenuItem.Click += new EventHandler(this.newMessageToolStripMenuItem_Click);
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.editMessageTemplatesToolStripMenuItem, this.toolStripSeparator1 };
            this.messageTemplatesToolStripMenuItem.DropDownItems.AddRange(itemArray2);
            this.messageTemplatesToolStripMenuItem.Name = "messageTemplatesToolStripMenuItem";
            this.messageTemplatesToolStripMenuItem.Size = new Size(0x7a, 20);
            this.messageTemplatesToolStripMenuItem.Text = "Message &Templates";
            this.editMessageTemplatesToolStripMenuItem.Name = "editMessageTemplatesToolStripMenuItem";
            this.editMessageTemplatesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
            this.editMessageTemplatesToolStripMenuItem.ShowShortcutKeys = false;
            this.editMessageTemplatesToolStripMenuItem.Size = new Size(0xca, 0x16);
            this.editMessageTemplatesToolStripMenuItem.Text = "&Edit Message Templates...";
            this.editMessageTemplatesToolStripMenuItem.Click += new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(0xc7, 6);
            ToolStripItem[] itemArray3 = new ToolStripItem[] { this.manageGroupsToolStripMenuItem, this.manageContactsToolStripMenuItem, this.editKeywordAutoResponseToolStripMenuItem, this.editGroupScheduleToolStripMenuItem };
            this.editsToolStripMenuItem.DropDownItems.AddRange(itemArray3);
            this.editsToolStripMenuItem.Name = "editsToolStripMenuItem";
            this.editsToolStripMenuItem.Size = new Size(0x27, 20);
            this.editsToolStripMenuItem.Text = "&Edit";
            this.manageGroupsToolStripMenuItem.Name = "manageGroupsToolStripMenuItem";
            this.manageGroupsToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.manageGroupsToolStripMenuItem.Text = "Edit &Groups";
            this.manageGroupsToolStripMenuItem.Click += new EventHandler(this.manageGroupsToolStripMenuItem_Click);
            this.manageContactsToolStripMenuItem.Name = "manageContactsToolStripMenuItem";
            this.manageContactsToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.manageContactsToolStripMenuItem.Text = "Edit &Contacts";
            this.manageContactsToolStripMenuItem.Click += new EventHandler(this.manageContactsToolStripMenuItem_Click);
            this.editKeywordAutoResponseToolStripMenuItem.Name = "editKeywordAutoResponseToolStripMenuItem";
            this.editKeywordAutoResponseToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.editKeywordAutoResponseToolStripMenuItem.Text = "Edit &Keyword Auto Response";
            this.editKeywordAutoResponseToolStripMenuItem.Click += new EventHandler(this.manageKeywordAutoResponseToolStripMenuItem_Click);
            this.editGroupScheduleToolStripMenuItem.Name = "editGroupScheduleToolStripMenuItem";
            this.editGroupScheduleToolStripMenuItem.Size = new Size(0xe1, 0x16);
            this.editGroupScheduleToolStripMenuItem.Text = "Edit Group &Schedule";
            this.editGroupScheduleToolStripMenuItem.Click += new EventHandler(this.editGroupScheduleToolStripMenuItem_Click);
            ToolStripItem[] itemArray4 = new ToolStripItem[] { this.settingsToolStripMenuItem, this.toolStripSeparator2, this.controlEnterToolStripMenuItem, this.requireClickToMarkMessageReadToolStripMenuItem, this.openMessagesWindowWithUnreadReminderToolStripMenuItem, this.keepSelectedConversationInFocusToolStripMenuItem, this.displayMMSAttachmentsToolStripMenuItem };
            this.optionsToolStripMenuItem.DropDownItems.AddRange(itemArray4);
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new Size(0x3d, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(0x149, 6);
            this.controlEnterToolStripMenuItem.CheckOnClick = true;
            this.controlEnterToolStripMenuItem.Name = "controlEnterToolStripMenuItem";
            this.controlEnterToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.controlEnterToolStripMenuItem.Text = "Use Control + Enter To Send Instead Of Just Enter";
            this.controlEnterToolStripMenuItem.Click += new EventHandler(this.controlEnterToolStripMenuItem_Click);
            this.requireClickToMarkMessageReadToolStripMenuItem.CheckOnClick = true;
            this.requireClickToMarkMessageReadToolStripMenuItem.Name = "requireClickToMarkMessageReadToolStripMenuItem";
            this.requireClickToMarkMessageReadToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.requireClickToMarkMessageReadToolStripMenuItem.Text = "Require Click to Mark Message Read";
            this.requireClickToMarkMessageReadToolStripMenuItem.Click += new EventHandler(this.requireClickToMarkMessageReadToolStripMenuItem_Click);
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.CheckOnClick = true;
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Name = "openMessagesWindowWithUnreadReminderToolStripMenuItem";
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Text = "Open Messages Window With Unread Reminder";
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Click += new EventHandler(this.openMessagesWindowWithUnreadReminderToolStripMenuItem_Click);
            this.keepSelectedConversationInFocusToolStripMenuItem.CheckOnClick = true;
            this.keepSelectedConversationInFocusToolStripMenuItem.Name = "keepSelectedConversationInFocusToolStripMenuItem";
            this.keepSelectedConversationInFocusToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.keepSelectedConversationInFocusToolStripMenuItem.Text = "Keep Selected Conversation in Focus";
            this.keepSelectedConversationInFocusToolStripMenuItem.Click += new EventHandler(this.keepSelectedConversationInFocusToolStripMenuItem_Click);
            this.displayMMSAttachmentsToolStripMenuItem.CheckOnClick = true;
            this.displayMMSAttachmentsToolStripMenuItem.Name = "displayMMSAttachmentsToolStripMenuItem";
            this.displayMMSAttachmentsToolStripMenuItem.Size = new Size(0x14c, 0x16);
            this.displayMMSAttachmentsToolStripMenuItem.Text = "Display MMS Attachments";
            this.displayMMSAttachmentsToolStripMenuItem.Click += new EventHandler(this.displayMMSAttachmentsToolStripMenuItem_Click);
            ToolStripItem[] itemArray5 = new ToolStripItem[] { this.generalHelpToolStripMenuItem, this.settingsHelpToolStripMenuItem, this.toolStripSeparator3, this.exitToolStripMenuItem, this.syncFeaturesToolStripMenuItem, this.versionToolStripMenuItem, this.tryBETAToolStripMenuItem, this.logOutToolStripMenuItem };
            this.helpToolStripMenuItem.DropDownItems.AddRange(itemArray5);
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(0x2c, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
            this.generalHelpToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.generalHelpToolStripMenuItem.Text = "General Help";
            this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
            this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
            this.settingsHelpToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.settingsHelpToolStripMenuItem.Text = "Settings Help";
            this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new Size(0x8f, 6);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            this.syncFeaturesToolStripMenuItem.Name = "syncFeaturesToolStripMenuItem";
            this.syncFeaturesToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.syncFeaturesToolStripMenuItem.Text = "Sync Features";
            this.syncFeaturesToolStripMenuItem.Click += new EventHandler(this.syncFeaturesToolStripMenuItem_Click);
            this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
            this.versionToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.versionToolStripMenuItem.Text = "version";
            this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
            this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
            this.tryBETAToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.tryBETAToolStripMenuItem.Text = "Try BETA!";
            this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new Size(0x92, 0x16);
            this.logOutToolStripMenuItem.Text = "Log Out";
            this.logOutToolStripMenuItem.Click += new EventHandler(this.logOutToolStripMenuItem_Click);
            this.saveFileDialogPrintConversation.DefaultExt = "htm";
            this.saveFileDialogPrintConversation.Filter = "*.htm|*.html";
            this.contextMenuStripMessageText.ImageScalingSize = new Size(0x18, 0x18);
            ToolStripItem[] itemArray6 = new ToolStripItem[] { this.forwardTextMessage, this.copyTextToolStripMenuItem, this.copySelectedTextToolStripMenuItem, this.deleteToolStripMenuItem, this.markAsReadToolStripMenuItem };
            this.contextMenuStripMessageText.Items.AddRange(itemArray6);
            this.contextMenuStripMessageText.Name = "contextMenuStripMessageText";
            this.contextMenuStripMessageText.Size = new Size(0xae, 0x72);
            this.contextMenuStripMessageText.Opening += new CancelEventHandler(this.contextMenuStripMessageText_Opening);
            this.forwardTextMessage.Name = "forwardTextMessage";
            this.forwardTextMessage.Size = new Size(0xad, 0x16);
            this.forwardTextMessage.Text = "Forward";
            this.forwardTextMessage.Click += new EventHandler(this.forwardTextMessage_Click);
            this.copyTextToolStripMenuItem.Name = "copyTextToolStripMenuItem";
            this.copyTextToolStripMenuItem.Size = new Size(0xad, 0x16);
            this.copyTextToolStripMenuItem.Text = "Copy All Text";
            this.copyTextToolStripMenuItem.Click += new EventHandler(this.copyTextToolStripMenuItem_Click);
            this.copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
            this.copySelectedTextToolStripMenuItem.Size = new Size(0xad, 0x16);
            this.copySelectedTextToolStripMenuItem.Text = "Copy Selected Text";
            this.copySelectedTextToolStripMenuItem.Click += new EventHandler(this.copySelectedTextToolStripMenuItem_Click);
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new Size(0xad, 0x16);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
            this.markAsReadToolStripMenuItem.Name = "markAsReadToolStripMenuItem";
            this.markAsReadToolStripMenuItem.Size = new Size(0xad, 0x16);
            this.markAsReadToolStripMenuItem.Text = "Mark as Read";
            this.markAsReadToolStripMenuItem.Visible = false;
            this.markAsReadToolStripMenuItem.Click += new EventHandler(this.markAsReadToolStripMenuItem_Click);
            this.labelProcessing.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.labelProcessing.AutoSize = true;
            this.labelProcessing.BackColor = System.Drawing.Color.LimeGreen;
            this.labelProcessing.ForeColor = System.Drawing.Color.Black;
            this.labelProcessing.Location = new Point(0x1d8, 5);
            this.labelProcessing.Name = "labelProcessing";
            this.labelProcessing.Size = new Size(0x65, 13);
            this.labelProcessing.TabIndex = 3;
            this.labelProcessing.Text = "Processing Enabled";
            this.labelProcessing.TextAlign = ContentAlignment.MiddleRight;
            this.contextMenuStripPictureBox.ImageScalingSize = new Size(0x18, 0x18);
            ToolStripItem[] itemArray7 = new ToolStripItem[] { this.downloadToolStripMenuItem };
            this.contextMenuStripPictureBox.Items.AddRange(itemArray7);
            this.contextMenuStripPictureBox.Name = "contextMenuStripPictureBox";
            this.contextMenuStripPictureBox.Size = new Size(0x81, 0x1a);
            this.contextMenuStripPictureBox.Opening += new CancelEventHandler(this.contextMenuStripPictureBox_Opening);
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new Size(0x80, 0x16);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new EventHandler(this.downloadToolStripMenuItem_Click);
            this.rapidSpellAsYouTypeText.AddMenuText = "Add";
            this.rapidSpellAsYouTypeText.AllowAnyCase = false;
            this.rapidSpellAsYouTypeText.AllowMixedCase = false;
            this.rapidSpellAsYouTypeText.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeText.CheckAsYouType = true;
            this.rapidSpellAsYouTypeText.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeText.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeText.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeText.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeText.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeText.DictFilePath = null;
            this.rapidSpellAsYouTypeText.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeText.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeText.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeText.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeText.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeText.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeText.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeText.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeText.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeText.IgnoreXML = false;
            this.rapidSpellAsYouTypeText.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeText.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeText.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeText.OptionsEnabled = true;
            this.rapidSpellAsYouTypeText.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeText.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeText.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeText.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeText.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeText.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeText.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeText.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeText.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeText.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeText.TextBoxBase = this.textBoxMessage;
            this.rapidSpellAsYouTypeText.TextComponent = null;
            this.rapidSpellAsYouTypeText.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeText.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeText.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeText.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeText.V2Parser = true;
            this.rapidSpellAsYouTypeText.WarnDuplicates = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x248, 0x231);
            base.Controls.Add(this.splitContainerMessages);
            base.Controls.Add(this.labelProcessing);
            base.Controls.Add(this.menuStripMessage);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MainMenuStrip = this.menuStripMessage;
            this.MinimumSize = new Size(540, 500);
            base.Name = "fmMessages";
            this.Text = "Messages";
            base.Activated += new EventHandler(this.fmMessages_Activated);
            base.FormClosing += new FormClosingEventHandler(this.fmMessage_FormClosing);
            base.Load += new EventHandler(this.fmMessage_Load);
            base.ResizeEnd += new EventHandler(this.fmMessages_ResizeEnd);
            base.Resize += new EventHandler(this.fmMessages_Resize);
            this.splitContainerMessages.Panel1.ResumeLayout(false);
            this.splitContainerMessages.Panel1.PerformLayout();
            this.splitContainerMessages.Panel2.ResumeLayout(false);
            this.splitContainerMessages.ResumeLayout(false);
            ((ISupportInitialize) this.pictureBoxConversationCountLock).EndInit();
            this.tableLayoutPanelMessages.ResumeLayout(false);
            this.pnSend.ResumeLayout(false);
            this.pnSend.PerformLayout();
            ((ISupportInitialize) this.pictureBoxLink).EndInit();
            ((ISupportInitialize) this.pictureBoxAttachment).EndInit();
            this.pnMoreMessages.ResumeLayout(false);
            this.pnMoreMessages.PerformLayout();
            this.tableLayoutPanelContactName.ResumeLayout(false);
            this.tableLayoutPanelContactName.PerformLayout();
            this.tableLayoutPanelContactPhone.ResumeLayout(false);
            this.tableLayoutPanelContactPhone.PerformLayout();
            this.menuStripMessage.ResumeLayout(false);
            this.menuStripMessage.PerformLayout();
            this.contextMenuStripMessageText.ResumeLayout(false);
            this.contextMenuStripPictureBox.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void keepSelectedConversationInFocusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "KeepConversationFocus", this.keepSelectedConversationInFocusToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Keep conversation in focus save error: " + errorMessage;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bKeepConversationFocus = this.keepSelectedConversationInFocusToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Keep conversation in focus save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 5);
            }
        }

        private void labelAttRemove_Click(object sender, EventArgs e)
        {
            this.pictureBoxAttachment.ImageLocation = null;
            this.labelAttRemove.Visible = false;
            this.dateTimePickerScheduleDate.Enabled = true;
        }

        private void labelTextBoxSearchHint_Click(object sender, EventArgs e)
        {
            this.textBoxSearch.Focus();
        }

        private void labelUnreadCount_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(this.labelUnreadCount.Text) > 0)
                {
                    this.labelUnreadCount.BackColor = ColorTranslator.FromHtml("#93FF14");
                    this.labelUnread.BackColor = ColorTranslator.FromHtml("#93FF14");
                }
                else
                {
                    System.Drawing.Color color = new System.Drawing.Color();
                    this.labelUnreadCount.BackColor = color;
                    this.labelUnread.BackColor = new System.Drawing.Color();
                }
            }
            catch
            {
            }
        }

        private void linkLabelEditContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.appManager.ShowEditContact(false);
        }

        private void linkLabelMoreConversations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationManager appManager = this.appManager;
            appManager.m_nConversationLimit += 50;
            this.appManager.LoadConversations(true);
        }

        private void linkLabelMoreMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.nMessageLimit += 50;
            this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
        }

        private void listBoxConversationList_DrawItem(object sender, DrawItemEventArgs e)
        {
            ConversationItem currentItem;
            if (e.Index >= 0)
            {
                int height = this.appManager.m_fontSize.Height;
                ListBox box = (ListBox) sender;
                currentItem = (ConversationItem) box.Items[e.Index];
                ConversationMetaData data = this.appManager.m_lsConversationMetaData.Find(var => var.fingerprint == currentItem.fingerprint);
                if (data == null)
                {
                    data = new ConversationMetaData();
                }
                Conversation conversation = this.appManager.m_lsConversation.Find(p => p.fingerprint == currentItem.fingerprint);
                string s = string.Empty;
                string contactPhone = string.Empty;
                string str3 = string.Empty;
                int unreadCount = 0;
                if ((conversation != null) && (conversation.unreadCount > 0))
                {
                    unreadCount = conversation.unreadCount;
                    str3 = conversation.unreadCount.ToString();
                }
                s = currentItem.contactName;
                contactPhone = currentItem.contactPhone;
                e.DrawBackground();
                if (currentItem.fingerprint == "0")
                {
                    if (currentItem.contactID == this.appManager.m_nCurrentContactID)
                    {
                        e.Graphics.FillRectangle(this.appManager.m_brushSelected, e.Bounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(this.appManager.m_brushLightGray, e.Bounds);
                    }
                }
                else if (currentItem.fingerprint == this.appManager.m_strCurentConversationFingerprint)
                {
                    e.Graphics.FillRectangle(this.appManager.m_brushSelected, e.Bounds);
                }
                else if (unreadCount > 0)
                {
                    e.Graphics.FillRectangle(this.appManager.m_brushHighlight, e.Bounds);
                }
                else if (data.lastMessageIsError)
                {
                    e.Graphics.FillRectangle(this.appManager.m_brushError, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(this.appManager.m_brushLightGray, e.Bounds);
                }
                Size size = TextRenderer.MeasureText("XX/XX/XXXX XX:XX:XX XX", new Font(this.appManager.m_fontSizeDT, FontStyle.Italic));
                Size size2 = TextRenderer.MeasureText("(XXX)XXXX - XXXX", this.appManager.m_fontSize);
                int width = size.Width;
                if (width < size2.Width)
                {
                    width = size2.Width;
                }
                e.Graphics.DrawString(s, this.appManager.m_fontSize, this.appManager.m_brushBlack, (float) (e.Bounds.Left + this.nPadding), (float) (e.Bounds.Top + this.nPadding));
                e.Graphics.DrawString(contactPhone, this.appManager.m_fontSize, this.appManager.m_brushBlack, (float) (e.Bounds.Left + this.nPadding), (float) ((e.Bounds.Top + height) + this.nPadding));
                e.Graphics.DrawString(currentItem.dtLastDate.ToString(), new Font(this.appManager.m_fontSizeDT, FontStyle.Italic), this.appManager.m_brushDimGray, (float) (e.Bounds.Left + this.nPadding), (float) ((e.Bounds.Top + (height * 2)) + this.nPadding));
                if (data.lastMessageIsError)
                {
                    e.Graphics.DrawString("Failure", new Font(this.appManager.m_fontSize, FontStyle.Bold), this.appManager.m_brushDimGray, (float) (e.Bounds.Left + width), (float) ((e.Bounds.Top + height) + this.nPadding));
                }
                else
                {
                    e.Graphics.DrawString(str3, new Font(this.appManager.m_fontSize, FontStyle.Bold), this.appManager.m_brushBlack, (float) (e.Bounds.Left + width), (float) ((e.Bounds.Top + height) + this.nPadding));
                }
                e.Graphics.DrawString(data.lastMessageDirection, new Font(this.appManager.m_fontSizeDT, FontStyle.Bold), this.appManager.m_brushDimGray, (float) (e.Bounds.Left + width), (float) ((e.Bounds.Top + (height * 2)) + this.nPadding));
                e.Graphics.DrawRectangle(this.appManager.m_penGray, e.Bounds);
            }
        }

        private void listBoxConversationList_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = ((this.appManager.m_fontSize.Height * 2) + (this.nPadding * 2)) + this.appManager.m_fontSizeDT.Height;
        }

        private void listBoxConversationList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.bStopConversationListSelectIndexChange)
            {
                this.bStopConversationListSelectIndexChange = false;
            }
            else
            {
                ListBox box = (ListBox) sender;
                if (box.SelectedItem != null)
                {
                    ConversationItem selectedItem = (ConversationItem) box.SelectedItem;
                    this.appManager.m_strCurentConversationFingerprint = selectedItem.fingerprint;
                    this.appManager.m_strCurrentContactAddress = selectedItem.contactAddress;
                    this.appManager.m_nCurrentContactID = selectedItem.contactID;
                    this.nMessageLimit = 50;
                    this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
                }
                this.listBoxConversationList.Invalidate();
            }
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

        private void manageContactsToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void markAsReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.appManager.m_textService.MarkMessageRead(this.nSelectedMessageID, this.appManager.m_strSession).Data.success)
            {
                this.appManager.ShowBalloon("Error calling message/read...", 5);
            }
            else
            {
                this.appManager.LoadUpdates(false);
            }
        }

        private void messageTemplateMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.textBoxMessage.Enabled)
            {
                this.appManager.ShowBalloon("You must first select an item before adding a message template response...", 5);
                return;
            }
            string str = string.Empty;
            string name = ((ToolStripDropDownItem) sender).Name;
            switch (<PrivateImplementationDetails>.ComputeStringHash(name))
            {
                case 0x310ca263:
                    if (name == "4")
                    {
                        str = this.appManager.m_strMessageTemplate4;
                        goto Label_022F;
                    }
                    break;

                case 0x320ca3f6:
                    if (name == "7")
                    {
                        str = this.appManager.m_strMessageTemplate7;
                        goto Label_022F;
                    }
                    break;

                case 0x330ca589:
                    if (name == "6")
                    {
                        str = this.appManager.m_strMessageTemplate6;
                        goto Label_022F;
                    }
                    break;

                case 0x1beb2a44:
                    if (name == "10")
                    {
                        str = this.appManager.m_strMessageTemplate10;
                        goto Label_022F;
                    }
                    break;

                case 0x300ca0d0:
                    if (name == "5")
                    {
                        str = this.appManager.m_strMessageTemplate5;
                        goto Label_022F;
                    }
                    break;

                case 0x340ca71c:
                    if (name == "1")
                    {
                        str = this.appManager.m_strMessageTemplate1;
                        goto Label_022F;
                    }
                    break;

                case 0x360caa42:
                    if (name == "3")
                    {
                        str = this.appManager.m_strMessageTemplate3;
                        goto Label_022F;
                    }
                    break;

                case 0x370cabd5:
                    if (name == "2")
                    {
                        str = this.appManager.m_strMessageTemplate2;
                        goto Label_022F;
                    }
                    break;

                case 0x3c0cb3b4:
                    if (name == "9")
                    {
                        str = this.appManager.m_strMessageTemplate9;
                        goto Label_022F;
                    }
                    break;

                case 0x3d0cb547:
                    if (name == "8")
                    {
                        str = this.appManager.m_strMessageTemplate8;
                        goto Label_022F;
                    }
                    break;
            }
            this.appManager.ShowBalloon("Invalid message template selection", 5);
        Label_022F:
            this.textBoxMessage.Text = str;
            this.textBoxMessage.Select(this.textBoxMessage.Text.Length, 0);
        }

        private void newMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.appManager.ShowNewMessage();
        }

        private void openMessagesWindowWithUnreadReminderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "PopMessageWindow", this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Open message window with unread reminder save error: " + errorMessage;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bPopMessageWindow = this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Open message window with unread reminder save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 5);
            }
        }

        private void optionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.controlEnterToolStripMenuItem.Checked = this.appManager.m_bControlEnter;
            this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Checked = this.appManager.m_bPopMessageWindow;
            this.requireClickToMarkMessageReadToolStripMenuItem.Checked = this.appManager.m_bRequreClickToMarkMessageRead;
            this.keepSelectedConversationInFocusToolStripMenuItem.Checked = this.appManager.m_bKeepConversationFocus;
            if (this.displayMMSAttachmentsToolStripMenuItem.Enabled)
            {
                this.displayMMSAttachmentsToolStripMenuItem.Checked = this.appManager.m_bDisplayMMSAttachments;
            }
        }

        private void pictureBoxConversationCountLock_Click(object sender, EventArgs e)
        {
            this.appManager.m_bConversationCountLocked = !this.appManager.m_bConversationCountLocked;
            if (this.appManager.m_bConversationCountLocked)
            {
                this.pictureBoxConversationCountLock.Image = Resources.locked;
            }
            else
            {
                this.pictureBoxConversationCountLock.Image = Resources.openlock;
            }
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

        public void PrintConversation(bool EntireConversation, DateTime? StartDate, DateTime? EndDate)
        {
            this.strError = string.Empty;
            string tempPath = Path.GetTempPath();
            object[] objArray1 = new object[] { tempPath, this.appManager.FormatFileName(this.textBoxContactName.Text), DateTime.Now.ToFileTimeUtc(), ".htm" };
            tempPath = string.Concat(objArray1);
            string str2 = "Most Recent " + this.appManager.m_lsMessages.Count.ToString() + " Messages";
            this.lsMessagesWorking = this.appManager.m_lsMessages;
            if (EntireConversation)
            {
                ConversationResponse response = this.appManager.m_textService.GetConversation(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession, 0, 0x3e7).Data.response;
                this.lsMessagesWorking = response.messages;
                this.lsMessagesWorking = (from c in this.lsMessagesWorking
                    orderby c.dateCreated, c.id
                    select c).ToList<TextMessage>();
                str2 = "Entire Conversation of " + this.lsMessagesWorking.Count.ToString() + " Messages";
            }
            if (StartDate.HasValue)
            {
                StartDate = new DateTime?(StartDate.Value.Date);
                EndDate = new DateTime?(EndDate.Value.Date);
                str2 = "Messages From: " + StartDate.Value.ToShortDateString() + " To: " + EndDate.Value.ToShortDateString();
            }
            try
            {
                StreamWriter writer = new StreamWriter(tempPath);
                writer.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
                writer.WriteLine("<html><head><title>" + this.appManager.m_strApplicationName + " Conversation Export</title></head><body>");
                writer.WriteLine("<script>");
                writer.WriteLine("function printFunction() {");
                writer.WriteLine("window.print();");
                writer.WriteLine("}");
                writer.WriteLine("</script>");
                writer.WriteLine("<H1>" + this.textBoxContactName.Text + " " + this.textBoxContactPhone.Text + "</H1>");
                writer.WriteLine("<H3>" + str2 + " &nbsp; &nbsp; &nbsp; Date Created: " + DateTime.Now.ToString() + " &nbsp; &nbsp; &nbsp; <button onclick=\"printFunction()\">Print</button></H3>");
                writer.WriteLine("<table cellpadding=\"10\">");
                writer.WriteLine("<tr><th>Date</th><th>Phone Number</th><th>Contact</th><th>Message</th></tr>");
                foreach (TextMessage message in this.lsMessagesWorking)
                {
                    DateTime time;
                    DateTime time2;
                    DateTime? nullable;
                    bool flag = true;
                    DateTime.TryParse(message.dateCreated, out time2);
                    if (StartDate.HasValue)
                    {
                        time = time2;
                        nullable = StartDate;
                        if (nullable.HasValue ? (time < nullable.GetValueOrDefault()) : false)
                        {
                            flag = false;
                        }
                    }
                    if (EndDate.HasValue)
                    {
                        time = time2;
                        nullable = EndDate;
                        if (nullable.HasValue ? (time > nullable.GetValueOrDefault()) : false)
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        writer.WriteLine("<tr>");
                        if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
                        {
                            writer.WriteLine("<td nowrap=\"nowrap\">" + time2 + "</td>");
                            writer.WriteLine("<td nowrap=\"nowrap\">" + this.appManager.FormatPhone(this.appManager.m_strUserName) + "</td>");
                            writer.WriteLine("<td></td>");
                            writer.WriteLine("<td>" + message.body + "</td>");
                        }
                        else
                        {
                            writer.WriteLine("<td nowrap=\"nowrap\">" + time2 + "</td>");
                            writer.WriteLine("<td nowrap=\"nowrap\">" + this.textBoxContactPhone.Text + "</td>");
                            writer.WriteLine("<td>" + this.textBoxContactName.Text + "</td>");
                            writer.WriteLine("<td bgcolor=\"#E8FFCC\">" + message.body + "</td>");
                        }
                        writer.WriteLine("</tr>");
                    }
                }
                writer.WriteLine("</table>");
                writer.WriteLine("</body></html>");
                writer.Close();
            }
            catch (Exception exception)
            {
                this.strError = "Error exporting conversation: " + exception.Message;
            }
            if (!string.IsNullOrEmpty(this.strError))
            {
                this.appManager.ShowBalloon(this.strError, 5);
                this.strError = string.Empty;
            }
            else
            {
                this.comboBoxConversationAction_Load();
                string text = "Use the Print function from your browser to print the conversation...";
                this.appManager.ShowBalloon(text, 5);
                Process.Start(tempPath);
            }
        }

        private void requireClickToMarkMessageReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string errorMessage = string.Empty;
            try
            {
                AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "ClickMarkMessageRead", this.requireClickToMarkMessageReadToolStripMenuItem.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    errorMessage = "Click Mark Message Read save error: " + errorMessage;
                    this.appManager.ShowBalloon(errorMessage, 5);
                }
                else
                {
                    this.appManager.m_bRequreClickToMarkMessageRead = this.requireClickToMarkMessageReadToolStripMenuItem.Checked;
                }
            }
            catch (Exception exception)
            {
                errorMessage = "Click Mark Message Read save error: " + exception.Message;
                this.appManager.ShowBalloon(errorMessage, 5);
            }
        }

        public void ResetMessageForm(string strDisplayText = null)
        {
            if (strDisplayText == null)
            {
                strDisplayText = "Select item...";
            }
            this.pnMessages.Controls.Clear();
            Label label = new Label {
                Font = this.appManager.m_fontSize,
                ForeColor = System.Drawing.Color.Black,
                Text = strDisplayText,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.pnMessages.Controls.Add(label);
            this.labelMesssageCount.Text = "0 Messages displayed";
            this.comboBoxConversationAction.Visible = false;
            this.textBoxContactPhone.Visible = false;
            this.linkLabelEditContact.Visible = false;
            this.textBoxContactName.Visible = false;
            this.textBoxMessage.Visible = false;
            this.dateTimePickerScheduleDate.Visible = false;
            this.buttonSend.Visible = false;
            this.pictureBoxLink.Visible = false;
            this.labelCharCount.Visible = false;
            this.tableLayoutPanelContactName.BackColor = Control.DefaultBackColor;
            this.tableLayoutPanelContactPhone.BackColor = Control.DefaultBackColor;
            this.textBoxContactName.BackColor = Control.DefaultBackColor;
            this.textBoxContactPhone.BackColor = Control.DefaultBackColor;
            this.pictureBoxAttachment.ImageLocation = null;
            this.textBoxMessage.Font = this.appManager.m_fontSize;
            this.textBoxContactName.Font = this.appManager.m_fontSize;
            this.textBoxContactPhone.Font = this.appManager.m_fontSize;
            this.tableLayoutPanelMessages.RowStyles[0].Height = this.nPadding + this.appManager.m_fontSize.Height;
            this.tableLayoutPanelMessages.RowStyles[1].Height = this.nPadding + this.appManager.m_fontSize.Height;
        }

        private void richTextBox_LinkClick(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
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

        private void textBoxMessage_KeyPress(object sender, KeyPressEventArgs e)
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
            if (e.KeyChar == '\x0012')
            {
                this.buttonRefresh_Click(sender, new EventArgs());
                e.Handled = true;
            }
        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {
            this.labelCharCount.ForeColor = new System.Drawing.Color();
            int length = this.textBoxMessage.Text.Length;
            if (length == 250)
            {
                this.labelCharCount.ForeColor = System.Drawing.Color.Red;
            }
            else if (length > 240)
            {
                this.labelCharCount.ForeColor = System.Drawing.Color.Orange;
            }
            this.labelCharCount.Text = length.ToString() + "/250";
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\x0012')
            {
                this.buttonRefresh_Click(sender, new EventArgs());
                e.Handled = true;
            }
            if (e.KeyChar == '\x0003')
            {
                this.buttonClear_Click(sender, new EventArgs());
                e.Handled = true;
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (this.textBoxSearch.Text.Trim().Length > 0)
            {
                this.textBoxSearch.BackColor = System.Drawing.Color.Yellow;
                this.labelTextBoxSearchHint.Visible = false;
                this.buttonClear.Enabled = true;
            }
            else
            {
                this.textBoxSearch.BackColor = System.Drawing.Color.White;
                this.labelTextBoxSearchHint.Visible = true;
                this.buttonClear.Enabled = false;
            }
            this.DisplayConversatoinList();
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

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly fmMessages.<>c <>9 = new fmMessages.<>c();
            public static Func<Conversation, string> <>9__28_0;
            public static Func<TextMessage, string> <>9__29_0;
            public static Func<TextMessage, long> <>9__29_1;
            public static Func<TextMessage, string> <>9__32_0;
            public static Func<TextMessage, long> <>9__32_1;
            public static Func<TextMessage, string> <>9__33_0;
            public static Func<TextMessage, long> <>9__33_1;

            internal string <DisplayConversation>b__29_0(TextMessage c) => 
                c.dateCreated;

            internal long <DisplayConversation>b__29_1(TextMessage n) => 
                n.id;

            internal string <DisplayConversatoinList>b__28_0(Conversation c) => 
                c.lastMessageDate;

            internal string <ExportConversation>b__33_0(TextMessage c) => 
                c.dateCreated;

            internal long <ExportConversation>b__33_1(TextMessage n) => 
                n.id;

            internal string <PrintConversation>b__32_0(TextMessage c) => 
                c.dateCreated;

            internal long <PrintConversation>b__32_1(TextMessage n) => 
                n.id;
        }*/

        [StructLayout(LayoutKind.Sequential)]
        private struct ConversationItem
        {
            public string fingerprint { get; set; }
            public long contactID { get; set; }
            public string contactAddress { get; set; }
            public string contactName { get; set; }
            public string contactPhone { get; set; }
            public DateTime? dtLastDate { get; set; }
            public int unreadCount { get; set; }
        }
    }
}


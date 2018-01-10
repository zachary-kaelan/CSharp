namespace TaskBarApp
{
    using CsvHelper;
    using Microsoft.Win32;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using TaskBarApp.Objects;
    using Zipwhip;

    public class ApplicationManager
    {
        public fmAccountDashboard formAccountDashboard;
        public fmEditAccounts formEditAccounts;
        public fmEditContacts formEditContact;
        public fmEditGroups formEditGroups;
        public fmEditGroupSchedule formEditGroupSchedule;
        public fmGroupSchedule formGroupSchedule;
        public fmKeywordAutoResponse formKeywordAutoResponse;
        public fmMessages formMessages;
        public fmMessageTemplate formMessageTemplate;
        public fmNewMessage formNewMessage;
        public fmPrintConversation formPrintConversation;
        public fmSettings formSettings;
        public fmUserLogin formUserLogin;
        private string gs_FilePath;
        private List<string> gs_GroupSend;
        private DateTime gs_ScheduleDate;
        private string gs_TextMessage;
        public Icon iTextApp;
        public string m_AssemblyVersion = string.Empty;
        public bool m_bAccountDashboardLoading;
        public bool m_bAccountDashboardRefreshMessages = true;
        public bool m_bAccountDashboardRefreshMessagesManualClick;
        public bool m_bAllowBlock = true;
        public bool m_bAllowDelete;
        public bool m_bConnected;
        public bool m_bContactsInitiated;
        public bool m_bControlEnter = true;
        public bool m_bConversationCountLocked;
        public bool m_bDashboardFeature;
        public bool m_bDashboardMode;
        public bool m_bDisableDashboardSettingChangeNotifications;
        public bool m_bDisableRemoteFeatureSync;
        public bool m_bDisableRemoteFeatureUpdates;
        public bool m_bDisplayMMSAttachments;
        public bool m_bEnableGroupScheduleProcessing;
        public bool m_bEnableKeywordProcessing;
        public bool m_bEnableSignature = true;
        public bool m_bGroupScheduleFeature;
        public bool m_bGroupScheduleProcessing;
        public bool m_bGroupSend;
        public bool m_bIsBranded;
        public bool m_bIsLoggedOut;
        public bool m_bKeepConversationFocus;
        public bool m_bKeywordFeature;
        public bool m_bMessageChanged;
        public bool m_bMessageTemplateFeature = true;
        public bool m_bMMSFeature = true;
        public bool m_bMMSSendFeature;
        public bool m_bNotifyKeywordProcessing;
        public bool m_bNotifyServerSync;
        public bool m_bOpenDashboardForm;
        public bool m_bOpenMessageForm;
        public bool m_bPlayDashboardSound = true;
        public bool m_bPlaySound = true;
        public bool m_bPopDashboardWindow = true;
        public bool m_bPopMessageWindow = true;
        public bool m_bRefreshDashboardCounts;
        public bool m_bRefreshDashboardForm;
        public bool m_bRefreshMessageFormConversationList;
        public bool m_bRefreshMessageFormProcessingLabel;
        public bool m_bRequreClickToMarkMessageRead = true;
        public SolidBrush m_brushBlack = new SolidBrush(System.Drawing.Color.Black);
        public SolidBrush m_brushDimGray = new SolidBrush(System.Drawing.Color.DimGray);
        public SolidBrush m_brushError = new SolidBrush(ColorTranslator.FromHtml("#fce3c2"));
        public SolidBrush m_brushHighlight = new SolidBrush(ColorTranslator.FromHtml("#93FF14"));
        public SolidBrush m_brushLightGray = new SolidBrush(System.Drawing.Color.LightGray);
        public SolidBrush m_brushSelected = new SolidBrush(ColorTranslator.FromHtml("#E8FFCC"));
        public bool m_bSaveLogIn = true;
        public bool m_bSyncAllContacts;
        public bool m_bTextMessagesLoading;
        public bool m_bValidateMobileNumbers = true;
        public DateTime m_dtNextNotification = DateTime.Now;
        public Font m_fontSize = new Font("Arial", 12f);
        public Font m_fontSizeDT = new Font("Arial", 8f);
        public List<AccountItem> m_lsAccountItems;
        public List<Contact> m_lsContact = new List<Contact>();
        public List<Conversation> m_lsConversation;
        public List<ConversationMetaData> m_lsConversationMetaData = new List<ConversationMetaData>();
        public List<ScheduleFileItem> m_lsGroupSchedule = new List<ScheduleFileItem>();
        public List<GroupTagContact> m_lsGroupTagContacts = new List<GroupTagContact>();
        public List<string> m_lsGroupTags = new List<string>();
        public List<TextMessage> m_lsMessages = new List<TextMessage>();
        public List<TextMessage> m_lsUnReadMessages = new List<TextMessage>();
        public int m_nAccountDashboardRefreshInterval = 10;
        public int m_nAccountNDX;
        public int m_nContactLimit = 50;
        public int m_nConversationLimit = 50;
        public int m_nConversationLimitDefault = 50;
        public long m_nCurrentContactID;
        public int m_nFontSize = 12;
        public int m_nFontSizeDT = 8;
        public int m_nGroupScheduleBackHourLimit = -24;
        public int m_nGroupSendPauseIntervalSeconds = 1;
        public int m_nLastMessageStatusLimit = 500;
        public int m_nMonitorTextMessageInterval = 10;
        public long m_nMostRecentMetaDataMessageID;
        public int m_nNotificationInterval = 15;
        public int m_nServerSyncRefreshInterval = 60;
        public int m_nUnreadMessageLimit = 500;
        public Pen m_penGray = new Pen(System.Drawing.Color.DimGray, 2f);
        public string m_strAccounTitle = string.Empty;
        public string m_strApplicationName = string.Empty;
        public string m_strAutoUpdateFileURL = string.Empty;
        public string m_strBETAUpdateFileURL = string.Empty;
        public string m_strConnectedIconPath = string.Empty;
        public string m_strCountryCode = string.Empty;
        public string m_strCSVScheduleFile = string.Empty;
        public string m_strCSVScheduleFilePath = string.Empty;
        public string m_strCurentConversationFingerprint = string.Empty;
        public string m_strCurentProcessingMessage = string.Empty;
        public string m_strCurrentContactAddress = string.Empty;
        public string m_strDashboardNotificationSound = string.Empty;
        public string m_strForwardMessage = string.Empty;
        public string m_strHelpURL = string.Empty;
        public string m_strLoginSplashPath = string.Empty;
        public string m_strMachineID = string.Empty;
        public string m_strMessageTemplate1 = string.Empty;
        public string m_strMessageTemplate10 = string.Empty;
        public string m_strMessageTemplate2 = string.Empty;
        public string m_strMessageTemplate3 = string.Empty;
        public string m_strMessageTemplate4 = string.Empty;
        public string m_strMessageTemplate5 = string.Empty;
        public string m_strMessageTemplate6 = string.Empty;
        public string m_strMessageTemplate7 = string.Empty;
        public string m_strMessageTemplate8 = string.Empty;
        public string m_strMessageTemplate9 = string.Empty;
        public string m_strNotConnectedIconPath = string.Empty;
        public string m_strNotificationSound = string.Empty;
        public string m_strPassword = string.Empty;
        public string m_strRemoveGroupKeyword = "remove";
        public string m_strSession = string.Empty;
        public string m_strSettingsURL = string.Empty;
        public string m_strSignature = "-Signature";
        public string m_strSoundPath = string.Empty;
        public string m_strUpdateFileURL = string.Empty;
        public string m_strUserDictionaryFile = string.Empty;
        public string m_strUserDictionaryFilePath = string.Empty;
        public string m_strUserName = string.Empty;
        public TextService m_textService;
        private System.Windows.Forms.Timer monitorAccountDashboard;
        private System.Windows.Forms.Timer monitorRefreshForm;
        private System.Windows.Forms.Timer monitorServerSync;
        private System.Windows.Forms.Timer monitorTextMessages;
        private readonly NotifyIcon notifyIcon;
        private string strError = string.Empty;

        public ApplicationManager(NotifyIcon notifyIcon)
        {
            this.notifyIcon = notifyIcon;
        }

        public string AddGroupTag(long ContactID, string GroupTag)
        {
            string str = string.Empty;
            Contact contact = new Contact();
            try
            {
                contact = this.GetContactByID(ContactID);
                List<string> list = this.GetContactGroupTags(contact.notes);
                if (list.Contains(GroupTag))
                    return str;

                list.Add(GroupTag);
                string notes = string.Join("", list.ToArray());
                if (!this.m_textService.SaveContact(contact.address, contact.firstName, contact.lastName, notes).Data.success)
                    return "Error calling contact/save...";

                string str3 = (contact.firstName + " " + contact.lastName).Trim();
                if (str3.Length == 0)
                    str3 = "Unnamed";

                GroupTagContact contactGroupTags = new GroupTagContact {
                    groupTag = GroupTag,
                    contactId = contact.id,
                    contact = str3 + " " + this.FormatPhone(contact.mobileNumber),
                    address = contact.address
                };
                GroupTagContact item = this.m_lsGroupTagContacts.Find(var => (var.contactId == contact.id) && (var.groupTag == contactGroupTags.groupTag));
                if (item != null)
                    this.m_lsGroupTagContacts.Remove(item);

                this.m_lsGroupTagContacts.Add(contactGroupTags);
                this.m_lsContact.Find(c => c.id == contact.id).notes = notes;
                if (!this.m_lsGroupTags.Contains(GroupTag))
                    this.m_lsGroupTags.Add(GroupTag);

                return str;
            }
            catch (Exception exception)
            {
                return "Error adding group tag to contact " + contact.address + ": " + exception.Message;
            }
        }

        public void BalloonClick()
        {
            typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this.notifyIcon, null);
        }

        public void BalloonShown()
        {
            this.BalloonWaiting = false;
        }

        private void ClearGlobalAccountVariables()
        {
            this.m_bContactsInitiated = false;
            this.m_nCurrentContactID = 0L;
            this.m_nMostRecentMetaDataMessageID = 0L;
            this.m_nConversationLimit = this.m_nConversationLimitDefault;
            this.m_strCurrentContactAddress = string.Empty;
            this.m_strCurentConversationFingerprint = string.Empty;
            this.m_dtNextNotification = DateTime.Now;
            this.m_lsUnReadMessages = new List<TextMessage>();
            this.m_lsGroupTags = new List<string>();
            this.m_lsGroupTagContacts = new List<GroupTagContact>();
            this.m_lsConversation = new List<Conversation>();
            this.m_lsConversationMetaData = new List<ConversationMetaData>();
            this.m_lsContact = new List<Contact>();
            if (this.m_bDashboardMode)
            {
                this.m_bEnableKeywordProcessing = false;
                this.m_bEnableGroupScheduleProcessing = false;
            }
        }

        private void formAccountDashboard_Closed(object sender, EventArgs e)
        {
            this.formAccountDashboard = null;
        }

        public string FormatAlphaNumeric(string item)
        {
            string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            foreach (char ch in item)
            {
                if (!source.Contains<char>(ch))
                {
                    item = item.Replace(ch.ToString(), "");
                }
            }
            return item;
        }

        public string FormatContactAddress(string address, bool onlyDigits = false, bool removeLeadingOne = false)
        {
            if (address.Length > 0)
            {
                foreach (char ch in address)
                {
                    if ((ch < '0') || (ch > '9'))
                    {
                        address = address.Replace(ch.ToString(), "");
                    }
                }
                if (removeLeadingOne && (address.Substring(0, 1) == "1"))
                {
                    address = address.Substring(1, address.Length - 1);
                }
                if (!onlyDigits)
                {
                    address = "ptn:/" + address;
                }
            }
            return address;
        }

        public string FormatFileName(string fileName)
        {
            string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
            foreach (char ch in fileName)
            {
                if (!source.Contains<char>(ch))
                {
                    fileName = fileName.Replace(ch.ToString(), "");
                }
            }
            return fileName;
        }

        public string FormatGroupTag(string grouptag)
        {
            string source = "#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            foreach (char ch in grouptag)
            {
                if (!source.Contains<char>(ch))
                {
                    grouptag = grouptag.Replace(ch.ToString(), "");
                }
            }
            grouptag = grouptag.ToLower();
            return grouptag;
        }

        public string FormatMenuItem(string item)
        {
            item = item.Replace("\r\n", "");
            item = item.Replace("\n", "");
            item = item.Replace("\r", "");
            item = item.Replace("\t", "");
            int length = item.Length;
            if (length > 50)
            {
                length = 50;
            }
            item = item.Substring(0, length) + "...";
            return item;
        }

        public string FormatPhone(string phone)
        {
            phone = phone.Replace("+", "");
            if (this.IsDigitsOnly(phone))
            {
                if (phone.Length == 10)
                {
                    phone = $"{double.Parse(phone.Substring(0, 10)):(###) ###-####}";
                }
                else if ((phone.Length == 11) && (phone.Substring(0, 1) == "1"))
                {
                    phone = $"{double.Parse(phone.Substring(0, 11)):# (###) ###-####}";
                }
            }
            else if (phone.Contains("device:/"))
            {
                phone = "GROUP";
            }
            else
            {
                int num = (phone.Length - phone.Replace("ptn:/", "").Length) / "ptn:/".Length;
                string str = phone;
                phone = string.Empty;
                while (num > 0)
                {
                    int startIndex = str.IndexOf("ptn:/") + 5;
                    string address = string.Empty;
                    try
                    {
                        if (str.Substring(startIndex, 1) == "1")
                        {
                            address = $"{double.Parse(str.Substring(startIndex, 11)):# (###) ###-####}";
                        }
                        else
                        {
                            address = $"{double.Parse(str.Substring(startIndex, 10)):(###) ###-####}";
                        }
                    }
                    catch
                    {
                        address = str.Replace(",ptn:/", "").Replace("ptn:/", "");
                    }
                    if (string.IsNullOrEmpty(phone))
                    {
                        phone = address;
                    }
                    else
                    {
                        phone = phone + " " + address;
                    }
                    str = str.Replace(this.FormatContactAddress(address, false, false), "");
                    num--;
                }
            }
            return phone.Trim();
        }

        private void formEditAccounts_Closed(object sender, EventArgs e)
        {
            this.formEditAccounts = null;
        }

        private void formEditContact_Closed(object sender, EventArgs e)
        {
            this.formEditContact = null;
        }

        private void formEditGroups_Closed(object sender, EventArgs e)
        {
            this.formEditGroups = null;
        }

        private void formEditGroupSchedule_Closed(object sender, EventArgs e)
        {
            this.formEditGroupSchedule = null;
        }

        private void formGroupSchedule_Closed(object sender, EventArgs e)
        {
            this.formGroupSchedule = null;
        }

        private void formKeywordAutoResponse_Closed(object sender, EventArgs e)
        {
            this.formKeywordAutoResponse = null;
        }

        private void formMessages_Closed(object sender, EventArgs e)
        {
            this.formMessages = null;
        }

        private void formMessageTemplate_Closed(object sender, EventArgs e)
        {
            this.formMessageTemplate = null;
        }

        private void formNewMessage_Closed(object sender, EventArgs e)
        {
            this.formNewMessage = null;
        }

        private void formPrintConversation_Closed(object sender, EventArgs e)
        {
            this.formPrintConversation = null;
        }

        private void formSettings_Closed(object sender, EventArgs e)
        {
            this.formSettings = null;
        }

        private void formUserLogIn_Closed(object sender, EventArgs e)
        {
            this.formUserLogin = null;
        }

        private void GetAccountDashboard()
        {
            this.m_bAccountDashboardLoading = true;
            List<AccountItem> list = (from c in this.m_lsAccountItems
                orderby c.lastSyncDate
                select c).ToList<AccountItem>();
            this.m_nAccountNDX = 0;
            int count = list.Count;
            if (this.m_textService == null)
            {
                this.m_textService = new TextService();
            }
            while (this.m_nAccountNDX < count)
            {
                AccountItem item = list[this.m_nAccountNDX];
                bool flag = false;
                bool flag2 = false;
                string str = string.Empty;
                string connectionStatus = item.connectionStatus;
                try
                {
                    if (item.connectionStatus != "Failed Authentication")
                    {
                        if (string.IsNullOrEmpty(item.session) || (item.connectionStatus == "Unknown"))
                        {
                            try
                            {
                                if (AppRegistry.AuthorizeUser(item.number, ref this.strError))
                                {
                                    flag = this.m_textService.AccountLogIn(item.number, item.password, item.countryCode, ref str);
                                }
                            }
                            catch
                            {
                                flag2 = true;
                            }
                            if (!flag)
                            {
                                connectionStatus = "Failed Authentication";
                                str = string.Empty;
                            }
                            if (flag)
                            {
                                connectionStatus = "Connected";
                                RegistryKey regKey = AppRegistry.GetSubKey(AppRegistry.GetRootKey(ref this.strError), item.number, true, ref this.strError);
                                if (this.strError == string.Empty)
                                {
                                    AppRegistry.SaveValue(regKey, "local_Session", str, ref this.strError, false, RegistryValueKind.Unknown);
                                }
                            }
                        }
                        else
                        {
                            str = item.session;
                            if (!string.IsNullOrEmpty(item.session))
                            {
                                connectionStatus = "Connected";
                            }
                        }
                        if ((item.number == this.m_strUserName) && this.m_bConnected)
                        {
                            item.connectionStatus = "Logged In";
                        }
                        else
                        {
                            item.connectionStatus = connectionStatus;
                        }
                        item.session = str;
                        if (!string.IsNullOrEmpty(item.session))
                        {
                            try
                            {
                                if (this.m_bAccountDashboardRefreshMessages || !item.lastSyncDate.HasValue)
                                {
                                    IRestResponse<TextMessageList> response = this.m_textService.GetMessageList(item.session, 0, this.m_nUnreadMessageLimit);
                                    if (response.Data.success)
                                    {
                                        if (response.Data.total > 0)
                                        {
                                            item.lastSyncDate = new DateTime?(DateTime.Now);
                                            item.unReadMessageList = new List<TextMessage>();
                                            foreach (TextMessage message in response.Data.response)
                                            {
                                                if (!message.isRead && (this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(item.number, true, true)))
                                                {
                                                    item.unReadMessageList.Add(message);
                                                }
                                            }
                                        }
                                    }
                                    else if (response.ResponseStatus.ToString() != "Completed")
                                    {
                                        item.session = string.Empty;
                                        connectionStatus = "Unknown";
                                    }
                                }
                                else
                                {
                                    item.lastSyncDate = new DateTime?(DateTime.Now);
                                    int totalSeconds = (int) DateTime.UtcNow.Subtract(new DateTime(0x7b2, 1, 1)).TotalSeconds;
                                    IRestResponse<UpdateGet> updates = this.m_textService.GetUpdates(item.session, totalSeconds);
                                    if (updates.Data != null)
                                    {
                                        foreach (Session session in updates.Data.sessions)
                                        {
                                            if (session.message != null)
                                            {
                                                using (List<TextMessage>.Enumerator enumerator = session.message.GetEnumerator())
                                                {
                                                    while (enumerator.MoveNext())
                                                    {
                                                        TextMessage message = enumerator.Current;
                                                        if (this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(item.number, true, true))
                                                        {
                                                            TextMessage message2 = item.unReadMessageList.Find(m => m.id == message.id);
                                                            if ((message2 == null) && !message.isRead)
                                                            {
                                                                item.unReadMessageList.Add(message);
                                                            }
                                                            if ((message2 != null) && message.isRead)
                                                            {
                                                                item.unReadMessageList.Remove(message2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                flag2 = true;
                                item.session = string.Empty;
                                item.connectionStatus = "Unknown";
                                this.ShowBalloon("There was an error getting message counts for " + this.FormatPhone(item.number) + ": " + exception.Message, 5);
                            }
                        }
                        list[this.m_nAccountNDX] = item;
                    }
                }
                catch (Exception exception2)
                {
                    this.ShowBalloon("There was an error connecting to " + this.FormatPhone(item.number) + ": " + exception2.Message, 5);
                }
                if (flag2)
                {
                    this.ShowBalloon("Exception connecting to Text Service\n\nPlease check your internet connection...", 5);
                }
                this.m_nAccountNDX++;
                this.m_bRefreshDashboardCounts = true;
            }
            this.m_lsAccountItems = list;
            this.m_bAccountDashboardLoading = false;
            this.m_bAccountDashboardRefreshMessages = false;
            this.m_bRefreshDashboardForm = true;
            if (this.GetTotalDashboardUnread(true) > 0)
            {
                this.ShowBalloon("You have " + this.GetTotalDashboardUnread(false).ToString() + " unread text messages from " + this.m_lsAccountItems.Count.ToString() + " accounts on the Account Dashboard.", 5);
                if (this.m_bPopDashboardWindow)
                {
                    this.m_bOpenDashboardForm = true;
                }
                if (this.m_bPlayDashboardSound)
                {
                    new SoundPlayer(this.m_strSoundPath + this.m_strDashboardNotificationSound).Play();
                }
            }
        }

        public AccountItem GetAccountItem(string phone)
        {
            AccountItem item = new AccountItem();
            if ((this.m_lsAccountItems != null) || (phone.Length != 0))
            {
                foreach (AccountItem item2 in this.m_lsAccountItems)
                {
                    if (item2.number == phone)
                    {
                        return item2;
                    }
                }
            }
            return item;
        }

        public string GetAccountItemOldestUnreadDate(AccountItem accoutItem)
        {
            DateTime? nullable = null;
            using (List<TextMessage>.Enumerator enumerator = accoutItem.unReadMessageList.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    DateTime time;
                    DateTime.TryParse(enumerator.Current.dateCreated, out time);
                    if (!nullable.HasValue)
                    {
                        nullable = new DateTime?(time);
                    }
                    else
                    {
                        DateTime time2 = time;
                        DateTime? nullable2 = nullable;
                        if (nullable2.HasValue ? (time2 < nullable2.GetValueOrDefault()) : false)
                        {
                            nullable = new DateTime?(time);
                        }
                    }
                }
            }
            if (nullable.HasValue)
            {
                return nullable.ToString();
            }
            return "";
        }

        public Contact GetContactByID(long ContactID)
        {
            Contact contact = null;
            if ((this.m_lsContact == null) && (ContactID == 0))
            {
                return contact;
            }
            return this.m_lsContact.Find(c => c.id == ContactID);
        }

        public List<string> GetContactGroupTags(string contactNote)
        {
            List<string> list = new List<string>();
            contactNote = this.FormatGroupTag(contactNote);
            while (contactNote.Contains("#"))
            {
                int index = contactNote.IndexOf("#");
                int num2 = contactNote.IndexOf("#", 1);
                int length = 0;
                string item = string.Empty;
                if (num2 > 0)
                {
                    length = num2;
                }
                else
                {
                    length = contactNote.Length;
                }
                if ((index >= 0) && (num2 != index))
                {
                    item = this.FormatGroupTag(contactNote.Substring(index, length));
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
                contactNote = contactNote.Substring(length);
            }
            return list;
        }

        private void GetContacts()
        {
            if (this.m_bConnected)
            {
                try
                {
                    List<Contact> lsContact = this.m_lsContact;
                    List<string> lsGroupTags = this.m_lsGroupTags;
                    List<GroupTagContact> lsGroupTagContacts = this.m_lsGroupTagContacts;
                    IRestResponse<ContactListResponse> response = this.m_textService.GetContactList(this.m_strUserName, 1, 0x3e7);
                    int pages = response.Data.pages;
                    int num2 = 200;
                    if ((lsContact.Count == 0) || this.m_bSyncAllContacts)
                    {
                        this.m_bSyncAllContacts = false;
                        num2 = pages * response.Data.total;
                        lsContact = new List<Contact>();
                        lsGroupTags = new List<string>();
                        lsGroupTagContacts = new List<GroupTagContact>();
                    }
                    while (pages > 0)
                    {
                        Application.DoEvents();
                        using (List<Contact>.Enumerator enumerator = response.Data.response.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Contact contact = enumerator.Current;
                                Application.DoEvents();
                                string address = contact.address;
                                if (this.FormatPhone(address) != "GROUP")
                                {
                                    Contact item = lsContact.Find(var => var.id == contact.id);
                                    if (item != null)
                                    {
                                        lsContact.Remove(item);
                                    }
                                    lsContact.Add(contact);
                                    lsGroupTagContacts = (from var in lsGroupTagContacts
                                        where var.contactId != contact.id
                                        select var).ToList<GroupTagContact>();
                                    foreach (string str2 in this.GetContactGroupTags(contact.notes))
                                    {
                                        string str3 = contact.firstName + " " + contact.lastName;
                                        if (str3.Trim().Length == 0)
                                        {
                                            str3 = "Unnamed";
                                        }
                                        else
                                        {
                                            str3 = str3.Trim();
                                        }
                                        GroupTagContact contact2 = new GroupTagContact {
                                            groupTag = str2,
                                            contactId = contact.id,
                                            contact = str3 + " " + this.FormatPhone(contact.mobileNumber),
                                            address = contact.address
                                        };
                                        lsGroupTagContacts.Add(contact2);
                                        if (!lsGroupTags.Contains(str2))
                                        {
                                            lsGroupTags.Add(str2);
                                        }
                                    }
                                }
                            }
                        }
                        if ((lsContact.Count >= num2) || (pages == 1))
                        {
                            break;
                        }
                        response = this.m_textService.GetContactList(this.m_strUserName, pages, 0x3e7);
                        pages--;
                    }
                    this.m_lsContact = lsContact;
                    this.m_lsGroupTagContacts = lsGroupTagContacts;
                    this.m_lsGroupTags = lsGroupTags;
                    this.m_bContactsInitiated = true;
                }
                catch (Exception exception)
                {
                    this.ShowBalloon("Exception getting contact list: " + exception.Message, 5);
                }
            }
        }

        private void GetConversations()
        {
            if (this.m_bConnected)
            {
                try
                {
                    List<Conversation> lsConversation = this.m_lsConversation;
                    List<ConversationMetaData> list2 = new List<ConversationMetaData>();
                    lsConversation = this.m_textService.GetConversationList(0, this.m_nConversationLimit).Data.response;
                    if (this.m_nLastMessageStatusLimit > 0)
                    {
                        IRestResponse<TextMessageList> response = this.m_textService.GetMessageList(0, 1);
                        if (response.Data.total > 0)
                        {
                            TextMessage message = response.Data.response[0];
                            if (this.m_nMostRecentMetaDataMessageID != message.id)
                            {
                                this.m_nMostRecentMetaDataMessageID = message.id;
                                response = this.m_textService.GetMessageList(0, this.m_nLastMessageStatusLimit);
                                if (response.Data.total > 0)
                                {
                                    using (List<TextMessage>.Enumerator enumerator = response.Data.response.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            DateTime time;
                                            TextMessage message = enumerator.Current;
                                            DateTime.TryParse(message.dateCreated, out time);
                                            ConversationMetaData item = new ConversationMetaData();
                                            Application.DoEvents();
                                            bool flag = true;
                                            bool flag2 = false;
                                            foreach (ConversationMetaData data2 in list2)
                                            {
                                                Application.DoEvents();
                                                if (data2.fingerprint == message.fingerprint)
                                                {
                                                    flag = false;
                                                    DateTime? lastMessageDate = data2.lastMessageDate;
                                                    DateTime time2 = time;
                                                    if (lastMessageDate.HasValue ? (lastMessageDate.GetValueOrDefault() < time2) : false)
                                                    {
                                                        flag2 = true;
                                                        flag = true;
                                                    }
                                                }
                                            }
                                            if (flag2)
                                            {
                                                list2 = (from val in list2
                                                    where val.fingerprint != message.fingerprint
                                                    select val).ToList<ConversationMetaData>();
                                            }
                                            if (flag)
                                            {
                                                item.fingerprint = message.fingerprint;
                                                item.lastContactId = message.contactId;
                                                item.lastMessageDate = new DateTime?(time);
                                                if (message.destAddress == this.m_strUserName)
                                                {
                                                    item.lastMessageDirection = "In";
                                                }
                                                else
                                                {
                                                    item.lastMessageDirection = "Out";
                                                }
                                                if (message.transmissionState.name == "ERROR")
                                                {
                                                    item.lastMessageIsError = true;
                                                }
                                                else
                                                {
                                                    item.lastMessageIsError = false;
                                                }
                                                list2.Add(item);
                                            }
                                        }
                                    }
                                }
                                this.m_lsConversationMetaData = list2;
                            }
                        }
                    }
                    this.m_lsConversation = lsConversation;
                }
                catch (Exception exception)
                {
                    this.ShowBalloon("Exception getting conversation list: " + exception.Message, 5);
                }
                this.m_bRefreshMessageFormConversationList = true;
            }
        }

        public void GetServerSettings(bool bPushToServer = false)
        {
            try
            {
                bool flag = true;
                if (this.m_bNotifyServerSync)
                {
                    this.ShowBalloon("Connecting to " + this.m_strApplicationName + " server...", 5);
                }
                try
                {
                    flag = this.m_textService.AccountValidate(this.m_strUserName, this.m_strPassword, this.m_strCountryCode);
                    if (flag)
                    {
                        flag = AppRegistry.AuthorizeUser(this.m_strUserName, ref this.strError);
                    }
                }
                catch (Exception)
                {
                    flag = true;
                }
                if (!flag)
                {
                    MessageBox.Show("You are not authorized for this account: " + this.FormatPhone(this.m_strUserName), this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.m_bConnected = false;
                    if (this.formMessages != null)
                    {
                        this.formMessages.Close();
                    }
                    if (this.m_bDashboardMode)
                    {
                        AccountItem item = this.m_lsAccountItems.Find(var => var.number == this.m_strUserName);
                        if (item != null)
                        {
                            this.m_lsAccountItems.Remove(item);
                            item.connectionStatus = "Failed Authentication";
                            this.m_lsAccountItems.Add(item);
                        }
                        this.m_bRefreshDashboardForm = true;
                    }
                }
                else
                {
                    string strItemsUpdated = string.Empty;
                    AppRegistry.SyncServer(ref strItemsUpdated, this.m_strUserName);
                    this.InitializeAccountRegistryConfiguration(bPushToServer);
                    if (!string.IsNullOrEmpty(strItemsUpdated))
                    {
                        if (this.m_bDashboardMode)
                        {
                            if (!this.m_bDisableDashboardSettingChangeNotifications)
                            {
                                MessageBox.Show("The following Features and Settings have been updated from the server:\r\n" + strItemsUpdated, this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The following Features and Settings have been updated from the server:\r\n" + strItemsUpdated, this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        if (this.formMessages != null)
                        {
                            this.m_bRefreshMessageFormConversationList = true;
                        }
                    }
                    else if (this.m_bNotifyServerSync)
                    {
                        this.m_bNotifyServerSync = false;
                        this.ShowBalloon("Feature sync complete...", 5);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public int GetTotalDashboardUnread(bool excludeLoggedIn = false)
        {
            int num = 0;
            if (this.m_lsAccountItems != null)
            {
                foreach (AccountItem item in this.m_lsAccountItems)
                {
                    if (!excludeLoggedIn || (item.connectionStatus != "Logged In"))
                    {
                        int? nullable = new int?(item.unReadMessageList.Count<TextMessage>());
                        int? nullable2 = nullable;
                        int num2 = 0;
                        if ((nullable2.GetValueOrDefault() > num2) ? nullable2.HasValue : false)
                        {
                            num += Convert.ToInt32(nullable);
                        }
                    }
                }
            }
            return num;
        }

        private void GetUpdates()
        {
            this.m_bTextMessagesLoading = true;
            if (this.m_bConnected)
            {
                try
                {
                    bool flag = false;
                    int totalSeconds = (int) DateTime.UtcNow.Subtract(new DateTime(0x7b2, 1, 1)).TotalSeconds;
                    IRestResponse<UpdateGet> updates = this.m_textService.GetUpdates(totalSeconds);
                    if (updates.Data != null)
                    {
                        foreach (Session session in updates.Data.sessions)
                        {
                            if (session.message != null)
                            {
                                this.m_bRefreshMessageFormConversationList = true;
                                using (List<TextMessage>.Enumerator enumerator2 = session.message.GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        DateTime time2;
                                        TextMessage message = enumerator2.Current;
                                        if ((this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(this.m_strUserName, true, true)) && !this.ProcessMessageForKeyword(message))
                                        {
                                            TextMessage message = this.m_lsUnReadMessages.Find(m => m.id == message.id);
                                            if ((message == null) && !message.isRead)
                                            {
                                                this.m_lsUnReadMessages.Add(message);
                                                flag = true;
                                            }
                                            if ((message != null) && message.isRead)
                                            {
                                                this.m_lsUnReadMessages.Remove(message);
                                            }
                                        }
                                        if (message.fingerprint == this.m_strCurentConversationFingerprint)
                                        {
                                            TextMessage message2 = this.m_lsMessages.Find(m => m.id == message.id);
                                            if (message2 == null)
                                            {
                                                this.m_lsMessages.Add(message);
                                                this.m_bMessageChanged = true;
                                            }
                                            else
                                            {
                                                this.m_lsMessages.Remove(message2);
                                                this.m_lsMessages.Add(message);
                                            }
                                        }
                                        DateTime.TryParse(message.dateCreated, out time2);
                                        ConversationMetaData item = new ConversationMetaData();
                                        ConversationMetaData data2 = this.m_lsConversationMetaData.Find(var => var.fingerprint == message.fingerprint);
                                        if (data2 != null)
                                        {
                                            this.m_lsConversationMetaData.Remove(data2);
                                        }
                                        item.fingerprint = message.fingerprint;
                                        item.lastContactId = message.contactId;
                                        item.lastMessageDate = new DateTime?(time2);
                                        if (message.destAddress == this.m_strUserName)
                                        {
                                            item.lastMessageDirection = "In";
                                        }
                                        else
                                        {
                                            item.lastMessageDirection = "Out";
                                        }
                                        if (message.transmissionState.name == "ERROR")
                                        {
                                            item.lastMessageIsError = true;
                                        }
                                        else
                                        {
                                            item.lastMessageIsError = false;
                                        }
                                        this.m_lsConversationMetaData.Add(item);
                                    }
                                }
                            }
                            if (session.conversation != null)
                            {
                                this.m_bRefreshMessageFormConversationList = true;
                                using (List<Conversation>.Enumerator enumerator3 = session.conversation.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        Conversation conversation = enumerator3.Current;
                                        Conversation conversation = this.m_lsConversation.Find(var => var.id == conversation.id);
                                        if (conversation != null)
                                        {
                                            this.m_lsConversation.Remove(conversation);
                                        }
                                        if (!conversation.deleted)
                                        {
                                            this.m_lsConversation.Add(conversation);
                                        }
                                    }
                                }
                            }
                            if (session.contact != null)
                            {
                                this.m_bRefreshMessageFormConversationList = true;
                                using (List<Contact>.Enumerator enumerator4 = session.contact.GetEnumerator())
                                {
                                    while (enumerator4.MoveNext())
                                    {
                                        Contact contact = enumerator4.Current;
                                        string address = contact.address;
                                        if (this.FormatPhone(address) != "GROUP")
                                        {
                                            Contact contact = this.m_lsContact.Find(var => var.id == contact.id);
                                            if (contact != null)
                                            {
                                                this.m_lsContact.Remove(contact);
                                            }
                                            if (!contact.deleted)
                                            {
                                                this.m_lsContact.Add(contact);
                                                this.m_lsGroupTagContacts = (from var in this.m_lsGroupTagContacts
                                                    where var.contactId != contact.id
                                                    select var).ToList<GroupTagContact>();
                                                foreach (string str2 in this.GetContactGroupTags(contact.notes))
                                                {
                                                    string str3 = contact.firstName + " " + contact.lastName;
                                                    if (str3.Trim().Length == 0)
                                                    {
                                                        str3 = "Unnamed";
                                                    }
                                                    else
                                                    {
                                                        str3 = str3.Trim();
                                                    }
                                                    GroupTagContact contact2 = new GroupTagContact {
                                                        groupTag = str2,
                                                        contactId = contact.id,
                                                        contact = str3 + " " + this.FormatPhone(contact.mobileNumber),
                                                        address = contact.address
                                                    };
                                                    this.m_lsGroupTagContacts.Add(contact2);
                                                    if (!this.m_lsGroupTags.Contains(str2))
                                                    {
                                                        this.m_lsGroupTags.Add(str2);
                                                    }
                                                }
                                                if (contact.id == this.m_nCurrentContactID)
                                                {
                                                    this.m_bMessageChanged = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag || ((this.m_dtNextNotification <= DateTime.Now) && (this.m_lsUnReadMessages.Count<TextMessage>() > 0)))
                    {
                        this.m_dtNextNotification = DateTime.Now.AddMinutes((double) this.m_nNotificationInterval);
                        string text = "You have " + this.m_lsUnReadMessages.Count<TextMessage>().ToString() + " unread text messages for account " + this.FormatPhone(this.m_strUserName);
                        this.ShowBalloon(text, 5);
                        if (this.m_bPopMessageWindow)
                        {
                            this.m_bOpenMessageForm = true;
                        }
                        if (this.m_bPlaySound)
                        {
                            new SoundPlayer(this.m_strSoundPath + this.m_strNotificationSound).Play();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            this.m_bTextMessagesLoading = false;
        }

        public ScheduleFileItem GroupScheduleFileValidateItem(ScheduleFileItem validateItem)
        {
            string sendStatus = validateItem.SendStatus;
            if (sendStatus.Contains("Error:"))
            {
                sendStatus = "";
            }
            if (!sendStatus.Contains("Sent:"))
            {
                DateTime time;
                if (validateItem.TextMessage.Length > 250)
                {
                    sendStatus = sendStatus + "Error: Text message over 250 characters";
                }
                if (validateItem.TextMessage.Length == 0)
                {
                    sendStatus = sendStatus + "Error: Must have a text message.";
                }
                if (!DateTime.TryParse(validateItem.SendDate, out time))
                {
                    sendStatus = sendStatus + "Error: Send Date not valid";
                }
                if (validateItem.Address.Substring(0, 1) == "#")
                {
                    if (!this.m_lsGroupTags.Contains(validateItem.Address))
                    {
                        sendStatus = sendStatus + "Error: Group tag (" + validateItem.Address + ") does not exist";
                    }
                }
                else
                {
                    string str = this.FormatContactAddress(validateItem.Address, true, false);
                    if (!this.IsDigitsOnly(str) || !this.IsValidMobileNumber(str))
                    {
                        sendStatus = sendStatus + "Error: Phone number is not correctly formated";
                    }
                }
                validateItem.SendStatus = sendStatus;
            }
            return validateItem;
        }

        public string GroupSchedulFileAddItem(ScheduleFileItem csvRow)
        {
            this.strError = string.Empty;
            try
            {
                csvRow = this.GroupScheduleFileValidateItem(csvRow);
                this.m_lsGroupSchedule.Add(csvRow);
                this.GroupSchedulFileSave(this.m_lsGroupSchedule);
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Group Schedule file Add error: " + exception.Message;
            }
            return this.strError;
        }

        public void GroupSchedulFileInitialize(bool bClear = false)
        {
            this.m_strCSVScheduleFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CSV\";
            if (this.m_strCSVScheduleFilePath != string.Empty)
            {
                try
                {
                    if (!Directory.Exists(this.m_strCSVScheduleFilePath))
                    {
                        Directory.CreateDirectory(this.m_strCSVScheduleFilePath);
                    }
                    this.m_strCSVScheduleFile = this.m_strCSVScheduleFilePath + "ScheduleFile.csv";
                    if (!File.Exists(this.m_strCSVScheduleFile) | bClear)
                    {
                        IEnumerable<ScheduleFileItem> records = new List<ScheduleFileItem>();
                        StreamWriter writer = new StreamWriter(this.m_strCSVScheduleFile);
                        new CsvWriter(writer).WriteRecords(records);
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Failed to initialize the Group Schedule File '" + this.m_strCSVScheduleFile + "'. Error: " + exception.Message;
                }
                if (this.strError.Length > 0)
                {
                    this.ShowBalloon(this.strError, 5);
                    this.strError = string.Empty;
                }
            }
        }

        public void GroupSchedulFileLoad()
        {
            this.strError = string.Empty;
            List<ScheduleFileItem> list = new List<ScheduleFileItem>();
            try
            {
                if (this.m_strCSVScheduleFile.Length == 0)
                {
                    this.GroupSchedulFileInitialize(false);
                }
                StreamReader reader = new StreamReader(this.m_strCSVScheduleFile);
                list = new CsvReader(reader).GetRecords<ScheduleFileItem>().ToList<ScheduleFileItem>();
                reader.Close();
            }
            catch (Exception exception)
            {
                this.strError = exception.Message;
            }
            if (this.strError.Length > 0)
            {
                this.ShowBalloon("There was an error loading the Group Schedule File" + this.strError, 5);
                this.strError = string.Empty;
            }
            this.m_lsGroupSchedule = list;
        }

        public void GroupSchedulFileSave(List<ScheduleFileItem> csvRecords)
        {
            this.strError = string.Empty;
            try
            {
                StreamWriter writer = new StreamWriter(this.m_strCSVScheduleFile);
                new CsvWriter(writer).WriteRecords(csvRecords);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception exception)
            {
                this.strError = "Group Schedule file Save error: " + exception.Message;
            }
            if (this.strError.Length == 0)
            {
                this.GroupSchedulFileLoad();
            }
            else
            {
                this.ShowBalloon("There was an error saving the Group Schedule File" + this.strError, 5);
                this.strError = string.Empty;
            }
        }

        public void InitializeAccountList()
        {
            try
            {
                this.m_lsAccountItems = new List<AccountItem>();
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                foreach (string str in AppRegistry.GetSubKeyNames(rootKey, null, ref this.strError))
                {
                    if (this.IsDigitsOnly(str))
                    {
                        RegistryKey regKey = AppRegistry.GetSubKey(rootKey, str, false, ref this.strError);
                        AccountItem item = new AccountItem();
                        string str2 = string.Empty;
                        AppRegistry.GetValue(regKey, "Title", ref str2, ref this.strError);
                        item.title = str2;
                        AppRegistry.GetValue(regKey, "CountryCode", ref str2, ref this.strError);
                        item.countryCode = str2;
                        item.session = string.Empty;
                        item.number = AppRegistry.GetUserName(regKey, ref this.strError);
                        item.password = AppRegistry.GetPassword(regKey, ref this.strError);
                        item.connectionStatus = "Unknown";
                        item.unReadMessageList = new List<TextMessage>();
                        this.m_lsAccountItems.Add(item);
                    }
                }
            }
            catch (Exception exception)
            {
                this.ShowBalloon("There was an error getting multiple account information: " + exception.Message, 5);
            }
        }

        public void InitializeAccountRegistryConfiguration(bool bSync)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_DashboardNotificationSound", ref this.m_strDashboardNotificationSound, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strDashboardNotificationSound))
                {
                    this.m_strDashboardNotificationSound = "DoorBell.wav";
                }
                else if (this.m_strDashboardNotificationSound == "None")
                {
                    this.m_bPlayDashboardSound = false;
                }
                AppRegistry.GetValue(rootKey, "local_AccountDashboardRefreshInterval", ref this.m_nAccountDashboardRefreshInterval, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_DisableDashboardSettingChangeNotifications", ref this.m_bDisableDashboardSettingChangeNotifications, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_DashboardMode", ref this.m_bDashboardMode, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_SaveLogIn", ref this.m_bSaveLogIn, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_DisableRemoteFeatureUpdates", ref this.m_bDisableRemoteFeatureUpdates, ref this.strError);
                AppRegistry.GetValue(rootKey, "FontSize", ref this.m_nFontSize, ref this.strError);
                AppRegistry.GetValue(rootKey, "FontSizeDT", ref this.m_nFontSizeDT, ref this.strError);
                this.m_fontSize = new Font("Arial", (float) this.m_nFontSize);
                this.m_fontSizeDT = new Font("Arial", (float) this.m_nFontSizeDT);
                AppRegistry.GetValue(rootKey, "AutoUpdateFile", ref this.m_strAutoUpdateFileURL, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strAutoUpdateFileURL))
                {
                    this.m_strAutoUpdateFileURL = "http://www.isready.us/updates/TextApp.xml";
                }
                AppRegistry.GetValue(rootKey, "CurrentUpdateFile", ref this.m_strUpdateFileURL, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strUpdateFileURL))
                {
                    this.m_strUpdateFileURL = "http://www.isready.us/updates/TextAppCurrentVersion.xml";
                }
                AppRegistry.GetValue(rootKey, "BETAUpdateFile", ref this.m_strBETAUpdateFileURL, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strBETAUpdateFileURL))
                {
                    this.m_strBETAUpdateFileURL = "http://www.isready.us/updates/TextAppBETAVersion.xml";
                }
                AppRegistry.GetValue(rootKey, "NotificationSound", ref this.m_strNotificationSound, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strNotificationSound))
                {
                    this.m_strNotificationSound = "DoorBell.wav";
                }
                else if (this.m_strNotificationSound == "None")
                {
                    this.m_bPlaySound = false;
                }
                string str = string.Empty;
                AppRegistry.GetValue(rootKey, "Signature", ref str, ref this.strError);
                if (!string.IsNullOrEmpty(str))
                {
                    this.m_strSignature = str;
                }
                AppRegistry.GetValue(rootKey, "RemoveGroupKeyword", ref this.m_strRemoveGroupKeyword, ref this.strError);
                if (string.IsNullOrEmpty(this.m_strRemoveGroupKeyword))
                {
                    this.m_strRemoveGroupKeyword = "remove";
                }
                AppRegistry.GetValue(rootKey, "ConversationLimitDefault", ref this.m_nConversationLimitDefault, ref this.strError);
                this.m_nConversationLimit = this.m_nConversationLimitDefault;
                AppRegistry.GetValue(rootKey, "UnreadMessageLimit", ref this.m_nUnreadMessageLimit, ref this.strError);
                AppRegistry.GetValue(rootKey, "PopMessageWindow", ref this.m_bPopMessageWindow, ref this.strError);
                AppRegistry.GetValue(rootKey, "NotificationInterval", ref this.m_nNotificationInterval, ref this.strError);
                AppRegistry.GetValue(rootKey, "TextMessageIntervalSeconds", ref this.m_nMonitorTextMessageInterval, ref this.strError);
                AppRegistry.GetValue(rootKey, "GroupSendPauseIntervalSeconds", ref this.m_nGroupSendPauseIntervalSeconds, ref this.strError);
                AppRegistry.GetValue(rootKey, "EnableSignature", ref this.m_bEnableSignature, ref this.strError);
                AppRegistry.GetValue(rootKey, "EnableKeywordProcessing", ref this.m_bEnableKeywordProcessing, ref this.strError);
                AppRegistry.GetValue(rootKey, "NotifyKeywordProcessing", ref this.m_bNotifyKeywordProcessing, ref this.strError);
                AppRegistry.GetValue(rootKey, "EnableGroupScheduleProcessing", ref this.m_bEnableGroupScheduleProcessing, ref this.strError);
                AppRegistry.GetValue(rootKey, "ValidateMobileNumbers", ref this.m_bValidateMobileNumbers, ref this.strError);
                AppRegistry.GetValue(rootKey, "ClickMarkMessageRead", ref this.m_bRequreClickToMarkMessageRead, ref this.strError);
                AppRegistry.GetValue(rootKey, "KeepConversationFocus", ref this.m_bKeepConversationFocus, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate1", ref this.m_strMessageTemplate1, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate2", ref this.m_strMessageTemplate2, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate3", ref this.m_strMessageTemplate3, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate4", ref this.m_strMessageTemplate4, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate5", ref this.m_strMessageTemplate5, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate6", ref this.m_strMessageTemplate6, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate7", ref this.m_strMessageTemplate7, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate8", ref this.m_strMessageTemplate8, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate9", ref this.m_strMessageTemplate9, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplate10", ref this.m_strMessageTemplate10, ref this.strError);
                AppRegistry.GetValue(rootKey, "PopDashboardWindow", ref this.m_bPopDashboardWindow, ref this.strError);
                AppRegistry.GetValue(rootKey, "ControlEnter", ref this.m_bControlEnter, ref this.strError);
                AppRegistry.GetValue(rootKey, "DisplayMMSAttachments", ref this.m_bDisplayMMSAttachments, ref this.strError);
                AppRegistry.GetValue(rootKey, "AllowDelete", ref this.m_bAllowDelete, ref this.strError);
                AppRegistry.GetValue(rootKey, "AllowBlock", ref this.m_bAllowBlock, ref this.strError);
                AppRegistry.GetValue(rootKey, "LastMessageStatusLimit", ref this.m_nLastMessageStatusLimit, ref this.strError);
                AppRegistry.GetValue(rootKey, "KeywordFeature", ref this.m_bKeywordFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "GroupScheduleFeature", ref this.m_bGroupScheduleFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "MessageTemplateFeature", ref this.m_bMessageTemplateFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "DashboardFeature", ref this.m_bDashboardFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "MMSFeature", ref this.m_bMMSFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "MMSSendFeature", ref this.m_bMMSSendFeature, ref this.strError);
                AppRegistry.GetValue(rootKey, "DisableRemoteFeatureSync", ref this.m_bDisableRemoteFeatureSync, ref this.strError);
                AppRegistry.GetValue(rootKey, "FeatureSyncInterval", ref this.m_nServerSyncRefreshInterval, ref this.strError);
                if (!this.m_bKeywordFeature)
                {
                    this.m_bEnableKeywordProcessing = false;
                }
                if (!this.m_bGroupScheduleFeature)
                {
                    this.m_bEnableGroupScheduleProcessing = false;
                }
                if (!this.m_bMMSFeature)
                {
                    this.m_bDisplayMMSAttachments = false;
                }
                if (!this.m_bDashboardFeature)
                {
                    this.m_bDashboardMode = false;
                }
                if (this.m_bDashboardMode)
                {
                    this.m_bEnableKeywordProcessing = false;
                }
                AppRegistry.SaveValue(rootKey, "local_DisableRemoteFeatureUpdates", this.m_bDisableRemoteFeatureUpdates, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_ApplicationVersion", this.m_AssemblyVersion, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_DashboardMode", this.m_bDashboardMode, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_SaveLogIn", this.m_bSaveLogIn, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_DashboardNotificationSound", this.m_strDashboardNotificationSound, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_AccountDashboardRefreshInterval", this.m_nAccountDashboardRefreshInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_DisableDashboardSettingChangeNotifications", this.m_bDisableDashboardSettingChangeNotifications, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "FontSize", this.m_nFontSize, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "FontSizeDT", this.m_nFontSizeDT, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "ConversationLimitDefault", this.m_nConversationLimitDefault, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "NotificationSound", this.m_strNotificationSound, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "GroupScheduleBackHourLimit", this.m_nGroupScheduleBackHourLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "TextMessageIntervalSeconds", this.m_nMonitorTextMessageInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "GroupSendPauseIntervalSeconds", this.m_nGroupSendPauseIntervalSeconds, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MMSSendFeature", this.m_bMMSSendFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MMSFeature", this.m_bMMSFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "DashboardFeature", this.m_bDashboardFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplateFeature", this.m_bMessageTemplateFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "GroupScheduleFeature", this.m_bGroupScheduleFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "KeywordFeature", this.m_bKeywordFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "DisableRemoteFeatureSync", this.m_bDisableRemoteFeatureSync, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "FeatureSyncInterval", this.m_nServerSyncRefreshInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "AllowDelete", this.m_bAllowDelete, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "AllowBlock", this.m_bAllowBlock, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "LastMessageStatusLimit", this.m_nLastMessageStatusLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "DisplayMMSAttachments", this.m_bDisplayMMSAttachments, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "ControlEnter", this.m_bControlEnter, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "PopDashboardWindow", this.m_bPopDashboardWindow, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate10", this.m_strMessageTemplate10, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate9", this.m_strMessageTemplate9, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate8", this.m_strMessageTemplate8, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate7", this.m_strMessageTemplate7, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate6", this.m_strMessageTemplate6, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate5", this.m_strMessageTemplate5, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate4", this.m_strMessageTemplate4, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate3", this.m_strMessageTemplate3, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate2", this.m_strMessageTemplate2, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "MessageTemplate1", this.m_strMessageTemplate1, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "KeepConversationFocus", this.m_bKeepConversationFocus, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "ClickMarkMessageRead", this.m_bRequreClickToMarkMessageRead, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "ValidateMobileNumbers", this.m_bValidateMobileNumbers, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "EnableGroupScheduleProcessing", this.m_bEnableGroupScheduleProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "NotifyKeywordProcessing", this.m_bNotifyKeywordProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "EnableKeywordProcessing", this.m_bEnableKeywordProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "EnableSignature", this.m_bEnableSignature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "NotificationInterval", this.m_nNotificationInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "PopMessageWindow", this.m_bPopMessageWindow, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "UnreadMessageLimit", this.m_nUnreadMessageLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "Signature", this.m_strSignature, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "RemoveGroupKeyword", this.m_strRemoveGroupKeyword, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "AutoUpdateFile", this.m_strAutoUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "UpdateFile", this.m_strUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "BETAUpdateFile", this.m_strBETAUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "There was an error getting account registry configuration: " + exception.Message;
            }
            if (this.strError.Length > 0)
            {
                this.ShowBalloon(this.strError, 5);
            }
        }

        public void InitializeApplicationVariables()
        {
            try
            {
                this.m_strMachineID = AppRegistry.GetMachineID();
                this.m_AssemblyVersion = "v" + Assembly.GetCallingAssembly().GetName().Version.ToString();
                string localPath = string.Empty;
                try
                {
                    localPath = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
                }
                catch
                {
                    if (Directory.Exists(@"C:\Program Files\Line1\TextService"))
                    {
                        localPath = @"C:\Program Files\Line1\TextService";
                    }
                    else if (Directory.Exists(@"C:\Program Files (x86)\Line1\TextService"))
                    {
                        localPath = @"C:\Program Files (x86)\Line1\TextService";
                    }
                }
                this.iTextApp = new Icon(localPath + @"\TextApp.ico");
                try
                {
                    this.m_strApplicationName = File.ReadAllText(localPath + @"\Resources\ApplicationName.ini");
                }
                catch
                {
                    this.m_strApplicationName = "TextBox";
                }
                try
                {
                    this.m_strHelpURL = File.ReadAllText(localPath + @"\Resources\HelpURL.ini");
                }
                catch
                {
                    this.m_strHelpURL = "http://www.textbox.cc/help";
                }
                try
                {
                    this.m_strSettingsURL = File.ReadAllText(localPath + @"\Resources\SettingsURL.ini");
                }
                catch
                {
                    this.m_strSettingsURL = "http://www.textbox.cc/settings";
                }
                if (this.m_strApplicationName != "TextBox")
                {
                    this.m_bIsBranded = true;
                }
                this.m_strSoundPath = localPath + @"\Resources\";
                this.m_strConnectedIconPath = localPath + @"\Resources\Connected.ico";
                this.m_strNotConnectedIconPath = localPath + @"\Resources\NotConnected.ico";
                this.m_strLoginSplashPath = localPath + @"\Resources\BackgroundSplash.png";
                this.UserDictionaryFileInitialize(false);
            }
            catch (Exception exception)
            {
                this.ShowBalloon("There was an error getting application variables: " + exception.Message, 5);
            }
        }

        public bool IsDigitsOnly(string str)
        {
            foreach (char ch in str)
            {
                if ((ch < '0') || (ch > '9'))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidMobileNumber(string text)
        {
            bool flag = true;
            if (this.m_bValidateMobileNumbers)
            {
                text = text.Replace("ptn:/", "");
                text = text.Replace("+", "");
                if (text.Length != 10)
                {
                    flag = false;
                }
                if ((text.Length == 11) && (text.Substring(0, 1) == "1"))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public void LaunchWebsite(string strURL)
        {
            Process.Start(strURL);
        }

        public void LoadAccountDashboard()
        {
            if (!this.m_bAccountDashboardLoading)
            {
                new Thread(new ThreadStart(this.GetAccountDashboard)).Start();
            }
        }

        public void LoadContacts(bool bUseNewThread = false)
        {
            if (bUseNewThread)
            {
                new Thread(new ThreadStart(this.GetContacts)).Start();
            }
            else
            {
                this.GetContacts();
            }
        }

        public void LoadConversations(bool bUseNewThread = false)
        {
            if (bUseNewThread)
            {
                new Thread(new ThreadStart(this.GetConversations)).Start();
            }
            else
            {
                this.GetConversations();
            }
        }

        private void LoadServerSettings()
        {
            this.GetServerSettings(false);
        }

        public void LoadUpdates(bool bUseNewThread = false)
        {
            if (!this.m_bTextMessagesLoading)
            {
                if (bUseNewThread)
                {
                    new Thread(new ThreadStart(this.GetUpdates)).Start();
                }
                else
                {
                    this.GetUpdates();
                }
            }
        }

        public void LogIn(bool bNotify = true, string strUserName = null, string strPassword = null, string strCountryCode = null, string strAccountTitle = null)
        {
            bool flag = false;
            bool flag2 = false;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
            if (((strUserName == null) && (strPassword == null)) && (strCountryCode == null))
            {
                this.m_strUserName = AppRegistry.GetUserName(rootKey, ref this.strError);
                this.m_strPassword = AppRegistry.GetPassword(rootKey, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_CountryCode", ref this.m_strCountryCode, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_IsLoggedOut", ref this.m_bIsLoggedOut, ref this.strError);
            }
            else
            {
                this.m_strUserName = strUserName;
                this.m_strPassword = strPassword;
                this.m_strCountryCode = strCountryCode;
                this.m_strAccounTitle = strAccountTitle;
                this.m_bIsLoggedOut = false;
                this.m_bConnected = false;
                AppRegistry.SaveUserName(rootKey, strUserName, ref this.strError);
                AppRegistry.SavePassword(rootKey, strPassword, ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_CountryCode", strCountryCode, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_IsLoggedOut", false, ref this.strError, false, RegistryValueKind.Unknown);
            }
            if ((this.m_strUserName != string.Empty) && (this.m_strPassword != string.Empty))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            if ((flag && !this.m_bConnected) && !this.m_bIsLoggedOut)
            {
                if (bNotify)
                {
                    this.ShowBalloon("Logging into account " + this.FormatPhone(this.m_strUserName) + "...", 20);
                }
                if (!AppRegistry.AuthorizeUser(this.m_strUserName, ref this.strError))
                {
                    MessageBox.Show("You are not authorized for this account from this computer: " + this.FormatPhone(this.m_strUserName), this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (this.m_bDashboardMode)
                    {
                        AccountItem item = this.m_lsAccountItems.Find(var => var.number == this.m_strUserName);
                        if (item != null)
                        {
                            this.m_lsAccountItems.Remove(item);
                            item.connectionStatus = "Failed Authentication";
                            this.m_lsAccountItems.Add(item);
                        }
                        this.m_bRefreshDashboardForm = true;
                    }
                    return;
                }
                if (this.m_textService == null)
                {
                    this.m_textService = new TextService();
                }
                try
                {
                    this.m_bConnected = this.m_textService.AccountLogIn(this.m_strUserName, this.m_strPassword, this.m_strCountryCode, ref this.m_strSession);
                }
                catch (Exception exception)
                {
                    this.m_bConnected = false;
                    flag2 = true;
                    MessageBox.Show("Exception connecting to Text Service\n\nPlease check your internet connection...\n\nError details: " + exception.Message, "Log In Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            this.UpdateIcon();
            if (this.m_bConnected)
            {
                this.GetServerSettings(false);
                this.ClearGlobalAccountVariables();
                this.ShowBalloon("You have successfully logged into  account " + this.FormatPhone(this.m_strUserName) + "! Just a moment while we load your account information...", 5);
                Application.DoEvents();
                this.SetUnreadMessageCount();
                this.LoadContacts(false);
                this.LoadConversations(false);
                this.ShowMessages();
                Application.DoEvents();
                if (this.m_bDashboardMode)
                {
                    AccountItem item2 = this.m_lsAccountItems.Find(var => var.number == this.m_strUserName);
                    if (item2 != null)
                    {
                        this.m_lsAccountItems.Remove(item2);
                        item2.connectionStatus = "Logged In";
                        this.m_lsAccountItems.Add(item2);
                    }
                    this.m_bRefreshDashboardForm = true;
                }
            }
            else if (((!this.m_bConnected & flag) && !this.m_bIsLoggedOut) && !flag2)
            {
                MessageBox.Show("Log in failure.\n\nPlease check your log in information and selected country...", "Log In Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (this.m_bDashboardMode)
                {
                    this.m_lsAccountItems = (from val in this.m_lsAccountItems
                        where val.number != this.m_strUserName
                        select val).ToList<AccountItem>();
                    AccountItem item3 = new AccountItem {
                        countryCode = this.m_strCountryCode,
                        number = this.m_strUserName,
                        password = this.m_strPassword,
                        title = strAccountTitle,
                        session = string.Empty,
                        connectionStatus = "Failed Authentication"
                    };
                    this.m_lsAccountItems.Add(item3);
                }
                else
                {
                    this.ShowUserLogIn();
                }
            }
            else if (!flag2 && !this.m_bDashboardMode)
            {
                this.ShowUserLogIn();
            }
            else if (this.m_bDashboardMode)
            {
                this.ShowAccountDashboard();
            }
            else
            {
                this.ShowUserLogIn();
            }
        }

        public void LogOut(bool bNotify = true)
        {
            string errorMessage = string.Empty;
            this.strError = string.Empty;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref errorMessage);
            string strUserName = this.m_strUserName;
            bool bIsLoggedOut = this.m_bIsLoggedOut;
            try
            {
                this.ClearGlobalAccountVariables();
                if (this.formEditContact != null)
                {
                    this.formEditContact.Close();
                }
                if (this.formEditGroups != null)
                {
                    this.formEditGroups.Close();
                }
                if (this.formEditGroupSchedule != null)
                {
                    this.formEditGroupSchedule.Close();
                }
                if (this.formGroupSchedule != null)
                {
                    this.formGroupSchedule.Close();
                }
                if (this.formKeywordAutoResponse != null)
                {
                    this.formKeywordAutoResponse.Close();
                }
                if (this.formMessages != null)
                {
                    this.formMessages.Close();
                }
                if (this.formMessageTemplate != null)
                {
                    this.formMessageTemplate.Close();
                }
                if (this.formNewMessage != null)
                {
                    this.formNewMessage.Close();
                }
                if (this.formPrintConversation != null)
                {
                    this.formPrintConversation.Close();
                }
                if (this.formUserLogin != null)
                {
                    this.formUserLogin.Close();
                }
                if (this.m_bDashboardMode)
                {
                    AccountItem item = this.m_lsAccountItems.Find(var => var.connectionStatus == "Logged In");
                    if (item != null)
                    {
                        this.m_lsAccountItems.Remove(item);
                        item.connectionStatus = "Connected";
                        item.lastSyncDate = null;
                        item.unReadMessageList = new List<TextMessage>();
                        this.m_lsAccountItems.Add(item);
                    }
                    this.m_bRefreshDashboardForm = true;
                    this.m_strAccounTitle = string.Empty;
                }
                AppRegistry.SaveValue(rootKey, "local_IsLoggedOut", true, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "\nIsLoggedOut Error: " + errorMessage;
                }
                if (!this.m_bSaveLogIn)
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
                this.m_strSession = string.Empty;
                this.m_strUserName = string.Empty;
                this.m_strPassword = string.Empty;
                this.m_bIsLoggedOut = true;
            }
            catch (Exception exception)
            {
                this.ShowBalloon("Exception during logout: " + exception.Message + "\nError details: " + this.strError, 5);
            }
            if ((this.strError == string.Empty) && !bIsLoggedOut)
            {
                this.m_bConnected = false;
                if (bNotify)
                {
                    this.ShowBalloon("You have successfully logged out of account " + this.FormatPhone(strUserName), 5);
                }
            }
            this.UpdateIcon();
        }

        private void ProcessGroupMessage()
        {
            if (this.m_bConnected && !this.m_bGroupSend)
            {
                this.strError = string.Empty;
                int count = this.gs_GroupSend.Count;
                int num2 = 1;
                this.m_bGroupSend = true;
                try
                {
                    IRestResponse<TextMessageSendResponse> response = null;
                    IRestResponse<MMSSendResponse> response2 = null;
                    foreach (string str in this.gs_GroupSend)
                    {
                        this.m_strCurentProcessingMessage = "Sending " + num2.ToString() + " of " + count.ToString();
                        this.m_bRefreshMessageFormProcessingLabel = true;
                        if (!string.IsNullOrEmpty(this.gs_FilePath))
                        {
                            response2 = this.m_textService.SendMessageMMS(this.gs_TextMessage, this.FormatContactAddress(str, true, true), this.gs_FilePath);
                            if (!string.IsNullOrEmpty(response2.ErrorMessage))
                            {
                                this.strError = "Error calling MMS messaging/send for: " + str + ": " + response2.ErrorMessage;
                            }
                            else if (!response2.Data.success)
                            {
                                this.strError = this.strError + "Error from MMS message/send for: " + str;
                            }
                        }
                        else
                        {
                            response = this.m_textService.SendMessage(this.gs_TextMessage, str, this.gs_ScheduleDate);
                            if (!string.IsNullOrEmpty(response.ErrorMessage))
                            {
                                string[] textArray1 = new string[] { this.strError, "Error calling message/send for: ", str, ": ", response.ErrorMessage };
                                this.strError = string.Concat(textArray1);
                            }
                            else if (!response.Data.success)
                            {
                                this.strError = this.strError + "Error from message/send for: " + str;
                            }
                        }
                        num2++;
                        Thread.Sleep((int) (0x3e8 * this.m_nGroupSendPauseIntervalSeconds));
                    }
                }
                catch (Exception exception)
                {
                    this.strError = exception.Message;
                }
                if (this.strError.Length > 0)
                {
                    this.ShowBalloon("Group Send Error: " + this.strError, 5);
                    this.strError = string.Empty;
                }
                this.gs_ScheduleDate = DateTime.Now;
                this.gs_GroupSend = null;
                this.gs_TextMessage = null;
                this.gs_FilePath = null;
                this.m_strCurentProcessingMessage = string.Empty;
                this.m_bRefreshMessageFormProcessingLabel = true;
                this.m_bGroupSend = false;
            }
        }

        private void ProcessGroupSchedule()
        {
            if (this.m_bEnableGroupScheduleProcessing && this.m_bContactsInitiated)
            {
                this.m_bGroupScheduleProcessing = true;
                bool flag = false;
                if (this.m_bConnected)
                {
                    try
                    {
                        using (List<ScheduleFileItem>.Enumerator enumerator = this.m_lsGroupSchedule.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                ScheduleFileItem sendItem = enumerator.Current;
                                ScheduleFileItem item = this.GroupScheduleFileValidateItem(sendItem);
                                sendItem.SendStatus = item.SendStatus;
                                if (sendItem.SendStatus.Length == 0)
                                {
                                    DateTime time;
                                    string str = string.Empty;
                                    DateTime.TryParse(sendItem.SendDate, out time);
                                    if (time < DateTime.Now)
                                    {
                                        if (time < DateTime.Now.AddHours((double) this.m_nGroupScheduleBackHourLimit))
                                        {
                                            object[] objArray1 = new object[] { "Not Sent: Send date is over ", this.m_nGroupScheduleBackHourLimit, " hours in the past from the time it was processed ", DateTime.Now.ToString(), "." };
                                            str = string.Concat(objArray1);
                                        }
                                        else
                                        {
                                            this.LoadContacts(false);
                                            IRestResponse<TextMessageSendResponse> response = null;
                                            if (sendItem.Address.Substring(0, 1) == "#")
                                            {
                                                foreach (GroupTagContact contact in (from val in this.m_lsGroupTagContacts
                                                    where val.groupTag == sendItem.Address
                                                    select val).ToList<GroupTagContact>())
                                                {
                                                    response = this.m_textService.SendMessage(sendItem.TextMessage, contact.address, DateTime.Now);
                                                    if (!response.Data.success)
                                                    {
                                                        string[] textArray1 = new string[] { str, "Error: ", DateTime.Now.ToString(), " - ", response.ErrorMessage };
                                                        this.strError = str = string.Concat(textArray1);
                                                    }
                                                    else
                                                    {
                                                        if (str.Length > 0)
                                                        {
                                                            str = str + "; ";
                                                        }
                                                        string[] textArray2 = new string[] { str, "Sent: ", contact.contact, " at ", DateTime.Now.ToString() };
                                                        str = string.Concat(textArray2);
                                                        flag = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                string formatedContacts = this.FormatContactAddress(sendItem.Address, false, false);
                                                response = this.m_textService.SendMessage(sendItem.TextMessage, formatedContacts, DateTime.Now);
                                                if (!response.Data.success)
                                                {
                                                    this.strError = str = "Error: " + DateTime.Now.ToString() + " - " + response.ErrorMessage;
                                                }
                                                else
                                                {
                                                    string[] textArray3 = new string[] { str, "Sent: ", this.FormatPhone(formatedContacts), " at ", DateTime.Now.ToString() };
                                                    str = string.Concat(textArray3);
                                                    flag = true;
                                                }
                                            }
                                            if (str.Length == 0)
                                            {
                                                str = "Sent: " + DateTime.Now.ToString();
                                            }
                                        }
                                    }
                                    sendItem.SendStatus = str;
                                }
                            }
                        }
                        this.GroupSchedulFileSave(this.m_lsGroupSchedule);
                        if (flag)
                        {
                            this.LoadConversations(false);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.strError = exception.Message;
                    }
                    if (this.strError.Length > 0)
                    {
                        this.ShowBalloon("Process Schedule File Error: " + this.strError, 5);
                        this.strError = string.Empty;
                    }
                }
                this.m_bGroupScheduleProcessing = false;
            }
        }

        public bool ProcessMessageForKeyword(TextMessage message)
        {
            bool flag = false;
            if (!this.m_bEnableKeywordProcessing || !this.m_bContactsInitiated)
            {
                return flag;
            }
            if (message.isRead)
            {
                return flag;
            }
            string body = string.Empty;
            string str2 = message.body.ToLower().Trim();
            string item = string.Empty;
            Contact contact = new Contact();
            List<string> contactGroupTags = new List<string>();
            try
            {
                item = "#" + str2;
                contact = this.GetContactByID(message.contactId);
                if (contact == null)
                {
                    this.LoadContacts(false);
                    contact = this.GetContactByID(message.contactId);
                }
                if (contact != null)
                {
                    contactGroupTags = this.GetContactGroupTags(contact.notes);
                }
            }
            catch (Exception exception)
            {
                this.strError = exception.Message;
            }
            if (this.m_lsGroupTags.Contains(item) && (contact != null))
            {
                if (contactGroupTags.Contains(item))
                {
                    string[] textArray1 = new string[] { "Auto response:\r\n\r\nYou are already a member of the ", str2.ToUpper(), " group\r\n\r\nYou may leave the group at any time by replying ", this.m_strRemoveGroupKeyword.ToUpper(), " ", str2.ToUpper() };
                    body = string.Concat(textArray1);
                }
                else if (!contactGroupTags.Contains(item))
                {
                    this.strError = this.AddGroupTag(contact.id, item);
                    if (this.strError.Length == 0)
                    {
                        string[] textArray2 = new string[] { "Auto response:\r\n\r\nYou have been added to the ", str2.ToUpper(), " group\r\n\r\nYou may leave the group at any time by replying ", this.m_strRemoveGroupKeyword.ToUpper(), " ", str2.ToUpper() };
                        body = string.Concat(textArray2);
                    }
                }
            }
            if (str2.Contains(this.m_strRemoveGroupKeyword) && (contact != null))
            {
                str2 = str2.Replace(this.m_strRemoveGroupKeyword, "").Trim();
                if (str2.Length == 0)
                {
                    if (contactGroupTags.Count > 0)
                    {
                        try
                        {
                            contact.notes = "";
                            if (!this.m_textService.SaveContact(contact.address, contact.firstName, contact.lastName, "").Data.success)
                            {
                                this.strError = "Error calling contact/save...";
                            }
                            else
                            {
                                this.m_lsGroupTagContacts = (from val in this.m_lsGroupTagContacts
                                    where val.contactId != contact.id
                                    select val).ToList<GroupTagContact>();
                                body = "Auto response:\r\n\r\nYou have been removed from ALL groups associated to this number";
                            }
                            goto Label_02EC;
                        }
                        catch (Exception exception2)
                        {
                            this.strError = "Error removing all gorup tag: " + exception2.Message;
                            goto Label_02EC;
                        }
                    }
                    body = "Auto response:\r\n\r\nYou are not currently a member of any groups associated to this number";
                }
                else
                {
                    item = "#" + str2;
                    if (contactGroupTags.Contains(item))
                    {
                        this.strError = this.RemoveGroupTag(contact.id, item);
                        if (this.strError.Length == 0)
                        {
                            body = "Auto response:\r\n\r\nYou have been removed from group " + str2.ToUpper();
                        }
                    }
                }
            }
        Label_02EC:
            if (this.strError.Length > 0)
            {
                this.ShowBalloon("Process Message Keyword: " + this.strError, 5);
                this.strError = string.Empty;
                return flag;
            }
            if (body.Length > 0)
            {
                flag = true;
                if (this.m_bNotifyKeywordProcessing)
                {
                    this.ShowBalloon("Processing auto response for " + this.FormatPhone(contact.mobileNumber) + "\r\n\r\n" + body, 5);
                }
                string formatedContacts = this.FormatContactAddress(contact.mobileNumber, false, false);
                if (!this.m_textService.SendMessage(body, formatedContacts, DateTime.Now).Data.success)
                {
                    this.strError = "Error calling sendMessage for auto response on: " + contact.mobileNumber + " - " + body;
                    return flag;
                }
                this.m_textService.MarkMessageRead(message.id);
            }
            return flag;
        }

        public string RemoveGroupTag(long ContactID, string GroupTag)
        {
            string str = string.Empty;
            Contact contact = new Contact();
            try
            {
                contact = this.GetContactByID(ContactID);
                string notes = string.Empty;
                if (GroupTag.ToLower() != "all")
                {
                    List<string> contactGroupTags = this.GetContactGroupTags(contact.notes);
                    contactGroupTags.Remove(GroupTag);
                    notes = string.Join("", contactGroupTags.ToArray());
                }
                if (!this.m_textService.SaveContact(contact.address, contact.firstName, contact.lastName, notes).Data.success)
                {
                    return "Error calling contact/save...";
                }
                if (GroupTag.ToLower() == "all")
                {
                    this.m_lsGroupTagContacts.RemoveAll(var => var.contactId == contact.id);
                }
                else
                {
                    GroupTagContact item = this.m_lsGroupTagContacts.Find(var => (var.contactId == contact.id) && (var.groupTag == GroupTag));
                    if (item != null)
                    {
                        this.m_lsGroupTagContacts.Remove(item);
                    }
                }
                this.m_lsContact.Find(var => var.id == contact.id).notes = notes;
            }
            catch (Exception exception)
            {
                str = "Error removing group tag from contact " + contact.address + ": " + exception.Message;
            }
            return str;
        }

        public void RunMonitorAcountDashboardTimer(object sender, EventArgs e)
        {
            this.LoadAccountDashboard();
        }

        public void RunMonitorNotifyFormTimer(object sender, EventArgs e)
        {
            try
            {
                if ((this.m_bRefreshMessageFormProcessingLabel && (this.formMessages != null)) && this.formMessages.ContainsFocus)
                {
                    this.formMessages.DisplayProcessingMessage(this.m_strCurentProcessingMessage);
                    this.m_bRefreshMessageFormProcessingLabel = false;
                }
                if (this.m_bRefreshMessageFormConversationList && (this.formMessages != null))
                {
                    if (this.formMessages.ContainsFocus)
                    {
                        this.formMessages.DisplayProcessingMessage(this.m_strCurentProcessingMessage);
                        this.formMessages.DisplayConversatoinList();
                        if (this.m_bMessageChanged)
                        {
                            if (!string.IsNullOrEmpty(this.m_strCurentConversationFingerprint))
                            {
                                this.formMessages.DisplayConversation(this.m_strCurentConversationFingerprint, false, false);
                            }
                            this.m_bMessageChanged = false;
                        }
                        this.m_bRefreshMessageFormConversationList = false;
                    }
                    this.formMessages.LoadMessageTemplateMenu();
                }
                if (this.m_bOpenMessageForm)
                {
                    this.ShowMessages();
                    this.m_bOpenMessageForm = false;
                }
                if (this.m_bRefreshDashboardForm)
                {
                    if ((this.formAccountDashboard != null) && this.formAccountDashboard.ContainsFocus)
                    {
                        this.formAccountDashboard.LoadGridViewAccounts();
                    }
                    this.m_bRefreshDashboardForm = false;
                }
                if (this.m_bRefreshDashboardCounts)
                {
                    if ((this.formAccountDashboard != null) && this.formAccountDashboard.ContainsFocus)
                    {
                        this.formAccountDashboard.DisplayRefreshCount();
                    }
                    this.m_bRefreshDashboardCounts = false;
                }
                if (this.m_bOpenDashboardForm)
                {
                    this.ShowAccountDashboard();
                    this.m_bOpenDashboardForm = false;
                }
            }
            catch (Exception)
            {
            }
        }

        public void RunMonitorServerSyncTimer(object sender, EventArgs e)
        {
            if (!this.m_bDisableRemoteFeatureSync)
            {
                new Thread(new ThreadStart(this.LoadServerSettings)).Start();
            }
        }

        public void RunMonitorTextMessageTimer(object sender, EventArgs e)
        {
            this.LoadUpdates(true);
            this.SendGroupSchedule();
        }

        public void SendGroupMessage(List<string> groupSend, string textMessage, DateTime scheduleDate, string filePath)
        {
            this.gs_GroupSend = groupSend;
            this.gs_TextMessage = textMessage;
            this.gs_ScheduleDate = scheduleDate;
            this.gs_FilePath = filePath;
            new Thread(new ThreadStart(this.ProcessGroupMessage)).Start();
        }

        public void SendGroupSchedule()
        {
            if (!this.m_bGroupScheduleProcessing)
            {
                new Thread(new ThreadStart(this.ProcessGroupSchedule)).Start();
            }
        }

        public void SetAccountFeatures()
        {
        }

        private void SetUnreadMessageCount()
        {
            if (this.m_bConnected)
            {
                try
                {
                    List<TextMessage> list = new List<TextMessage>();
                    IRestResponse<TextMessageList> response = this.m_textService.GetMessageList(0, this.m_nUnreadMessageLimit);
                    if (response.Data.total > 0)
                    {
                        foreach (TextMessage message in response.Data.response)
                        {
                            if (!message.isRead && (this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(this.m_strUserName, true, true)))
                            {
                                list.Add(message);
                            }
                        }
                    }
                    this.m_lsUnReadMessages = list;
                }
                catch (Exception exception)
                {
                    this.ShowBalloon("Exception setting unread message list: " + exception.Message, 5);
                }
            }
        }

        public void ShowAccountDashboard()
        {
            if (this.formAccountDashboard == null)
            {
                fmAccountDashboard dashboard1 = new fmAccountDashboard {
                    appManager = this
                };
                this.formAccountDashboard = dashboard1;
                this.formAccountDashboard.Closed += new EventHandler(this.formAccountDashboard_Closed);
                this.formAccountDashboard.Show();
            }
            else
            {
                this.formAccountDashboard.Activate();
                this.formAccountDashboard.WindowState = FormWindowState.Normal;
                this.formAccountDashboard.BringToFront();
            }
        }

        public void ShowBalloon(string text, int timeout)
        {
            timeout *= 0x3e8;
            this.notifyIcon.BalloonTipTitle = this.m_strApplicationName;
            this.notifyIcon.BalloonTipText = text;
            if (!this.BalloonWaiting)
            {
                this.BalloonWaiting = true;
                this.notifyIcon.ShowBalloonTip(timeout);
            }
            else
            {
                this.notifyIcon.Visible = false;
                this.notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(100);
            }
        }

        public void ShowEditAccounts(bool newAccount)
        {
            if (this.formEditAccounts == null)
            {
                fmEditAccounts accounts1 = new fmEditAccounts {
                    appManager = this,
                    bNewAccount = newAccount
                };
                this.formEditAccounts = accounts1;
                this.formEditAccounts.Closed += new EventHandler(this.formEditAccounts_Closed);
                this.formEditAccounts.Show();
            }
            else
            {
                this.formEditAccounts.Activate();
                this.formEditAccounts.WindowState = FormWindowState.Normal;
                this.formEditAccounts.BringToFront();
                this.formEditAccounts.comboBoxAccountList_Load(string.Empty);
            }
        }

        public void ShowEditContact(bool newContact)
        {
            if (this.formEditContact == null)
            {
                fmEditContacts contacts1 = new fmEditContacts {
                    appManager = this,
                    bNewContact = newContact
                };
                this.formEditContact = contacts1;
                this.formEditContact.Closed += new EventHandler(this.formEditContact_Closed);
                this.formEditContact.Show();
            }
            else
            {
                this.formEditContact.Activate();
                this.formEditContact.WindowState = FormWindowState.Normal;
                this.formEditContact.BringToFront();
                this.formEditContact.DisplayContact();
            }
        }

        public void ShowEditGroups()
        {
            if (this.formEditGroups == null)
            {
                fmEditGroups groups1 = new fmEditGroups {
                    appManager = this
                };
                this.formEditGroups = groups1;
                this.formEditGroups.Closed += new EventHandler(this.formEditGroups_Closed);
                this.formEditGroups.Show();
            }
            else
            {
                this.formEditGroups.Activate();
                this.formEditGroups.WindowState = FormWindowState.Normal;
                this.formEditGroups.BringToFront();
                this.formEditGroups.comboBoxGroups_Load(string.Empty);
                this.formEditGroups.comboBoxContactList_Load(string.Empty);
            }
        }

        public void ShowEditGroupSchedule()
        {
            if (this.formEditGroupSchedule == null)
            {
                fmEditGroupSchedule schedule1 = new fmEditGroupSchedule {
                    appManager = this
                };
                this.formEditGroupSchedule = schedule1;
                this.formEditGroupSchedule.Closed += new EventHandler(this.formEditGroupSchedule_Closed);
                this.formEditGroupSchedule.Show();
            }
            else
            {
                this.formEditGroupSchedule.Activate();
                this.formEditGroupSchedule.WindowState = FormWindowState.Normal;
                this.formEditGroupSchedule.BringToFront();
            }
        }

        public void ShowGroupSchedule()
        {
            if (this.formGroupSchedule == null)
            {
                fmGroupSchedule schedule1 = new fmGroupSchedule {
                    appManager = this
                };
                this.formGroupSchedule = schedule1;
                this.formGroupSchedule.Closed += new EventHandler(this.formGroupSchedule_Closed);
                this.formGroupSchedule.Show();
            }
            else
            {
                this.formGroupSchedule.Activate();
                this.formGroupSchedule.WindowState = FormWindowState.Normal;
                this.formGroupSchedule.BringToFront();
                this.formGroupSchedule.dataGridViewGroupScheduleFile_Load();
            }
        }

        public void ShowKeywordAutoResponse()
        {
            if (this.formKeywordAutoResponse == null)
            {
                fmKeywordAutoResponse response1 = new fmKeywordAutoResponse {
                    appManager = this
                };
                this.formKeywordAutoResponse = response1;
                this.formKeywordAutoResponse.Closed += new EventHandler(this.formKeywordAutoResponse_Closed);
                this.formKeywordAutoResponse.Show();
            }
            else
            {
                this.formKeywordAutoResponse.Activate();
                this.formKeywordAutoResponse.WindowState = FormWindowState.Normal;
                this.formKeywordAutoResponse.BringToFront();
            }
        }

        public void ShowMessages()
        {
            if (this.formMessages == null)
            {
                fmMessages messages1 = new fmMessages {
                    appManager = this
                };
                this.formMessages = messages1;
                this.formMessages.Closed += new EventHandler(this.formMessages_Closed);
                this.formMessages.Show();
            }
            else
            {
                this.formMessages.Activate();
                this.formMessages.WindowState = FormWindowState.Normal;
                this.formMessages.BringToFront();
                this.formMessages.DisplayConversatoinList();
                this.formMessages.LoadMessageTemplateMenu();
            }
        }

        public void ShowMessageTemplate()
        {
            if (this.formMessageTemplate == null)
            {
                fmMessageTemplate template1 = new fmMessageTemplate {
                    appManager = this
                };
                this.formMessageTemplate = template1;
                this.formMessageTemplate.Closed += new EventHandler(this.formMessageTemplate_Closed);
                this.formMessageTemplate.Show();
            }
            else
            {
                this.formMessageTemplate.Activate();
                this.formMessageTemplate.WindowState = FormWindowState.Normal;
                this.formMessageTemplate.BringToFront();
            }
        }

        public void ShowNewMessage()
        {
            if (this.formNewMessage == null)
            {
                fmNewMessage message1 = new fmNewMessage {
                    appManager = this
                };
                this.formNewMessage = message1;
                this.formNewMessage.Closed += new EventHandler(this.formNewMessage_Closed);
                this.formNewMessage.Show();
            }
            else
            {
                this.formNewMessage.Activate();
                this.formNewMessage.WindowState = FormWindowState.Normal;
                this.formNewMessage.BringToFront();
                this.formNewMessage.LoadMessageTemplateMenu();
            }
        }

        public void ShowPrintConversation()
        {
            if (this.formPrintConversation == null)
            {
                fmPrintConversation conversation1 = new fmPrintConversation {
                    appManager = this
                };
                this.formPrintConversation = conversation1;
                this.formPrintConversation.Closed += new EventHandler(this.formPrintConversation_Closed);
                this.formPrintConversation.Show();
            }
            else
            {
                this.formPrintConversation.Activate();
                this.formPrintConversation.WindowState = FormWindowState.Normal;
                this.formPrintConversation.BringToFront();
            }
        }

        public void ShowSettings()
        {
            if (this.formSettings == null)
            {
                fmSettings settings1 = new fmSettings {
                    appManager = this
                };
                this.formSettings = settings1;
                this.formSettings.Closed += new EventHandler(this.formSettings_Closed);
                this.formSettings.Show();
            }
            else
            {
                this.formSettings.Activate();
                this.formSettings.WindowState = FormWindowState.Normal;
                this.formSettings.BringToFront();
            }
        }

        public void ShowUserLogIn()
        {
            if (this.formUserLogin == null)
            {
                fmUserLogin login1 = new fmUserLogin {
                    appManager = this
                };
                this.formUserLogin = login1;
                this.formUserLogin.Closed += new EventHandler(this.formUserLogIn_Closed);
                this.formUserLogin.Show();
            }
            else
            {
                this.formUserLogin.Activate();
                this.formUserLogin.WindowState = FormWindowState.Normal;
                this.formUserLogin.BringToFront();
            }
        }

        public void StartMonitorAccountDashboardTimer()
        {
            if (this.monitorAccountDashboard == null)
            {
                this.monitorAccountDashboard = new System.Windows.Forms.Timer();
                this.monitorAccountDashboard.Tick += new EventHandler(this.RunMonitorAcountDashboardTimer);
            }
            else
            {
                this.monitorAccountDashboard.Stop();
            }
            int num = 0xea60 * this.m_nAccountDashboardRefreshInterval;
            this.monitorAccountDashboard.Interval = num;
            this.monitorAccountDashboard.Enabled = true;
            this.monitorAccountDashboard.Start();
        }

        public void StartMonitorNotifyFormTimer()
        {
            if (this.monitorRefreshForm == null)
            {
                this.monitorRefreshForm = new System.Windows.Forms.Timer();
                this.monitorRefreshForm.Tick += new EventHandler(this.RunMonitorNotifyFormTimer);
            }
            else
            {
                this.monitorRefreshForm.Stop();
            }
            this.monitorRefreshForm.Interval = 0x3e8;
            this.monitorRefreshForm.Enabled = true;
            this.monitorRefreshForm.Start();
        }

        public void StartMonitorServerSyncTimer()
        {
            if (this.monitorServerSync == null)
            {
                this.monitorServerSync = new System.Windows.Forms.Timer();
                this.monitorServerSync.Tick += new EventHandler(this.RunMonitorServerSyncTimer);
            }
            else
            {
                this.monitorServerSync.Stop();
            }
            int num = 0xea60 * this.m_nServerSyncRefreshInterval;
            this.monitorServerSync.Interval = num;
            this.monitorServerSync.Enabled = true;
            this.monitorServerSync.Start();
        }

        public void StartMonitorTextMessageTimer()
        {
            if (this.monitorTextMessages == null)
            {
                this.monitorTextMessages = new System.Windows.Forms.Timer();
                this.monitorTextMessages.Tick += new EventHandler(this.RunMonitorTextMessageTimer);
            }
            else
            {
                this.monitorTextMessages.Stop();
            }
            int num = 0x3e8 * this.m_nMonitorTextMessageInterval;
            this.monitorTextMessages.Interval = num;
            this.monitorTextMessages.Enabled = true;
            this.monitorTextMessages.Start();
        }

        public ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }
            return item;
        }

        public void UpdateIcon()
        {
            if (this.m_bConnected)
            {
                this.notifyIcon.Text = this.m_strApplicationName + " - Connected to " + this.FormatPhone(this.m_strUserName);
                this.notifyIcon.Icon = new Icon(this.m_strConnectedIconPath);
            }
            else
            {
                this.notifyIcon.Text = this.m_strApplicationName + " - Not Connected";
                this.notifyIcon.Icon = new Icon(this.m_strNotConnectedIconPath);
            }
        }

        public void UserDictionaryFileInitialize(bool bClear = false)
        {
            this.m_strUserDictionaryFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Dictionary\";
            if (this.m_strUserDictionaryFilePath != string.Empty)
            {
                try
                {
                    if (!Directory.Exists(this.m_strUserDictionaryFilePath))
                    {
                        Directory.CreateDirectory(this.m_strUserDictionaryFilePath);
                    }
                    this.m_strUserDictionaryFile = this.m_strUserDictionaryFilePath + "UserDictionary.txt";
                    if (!File.Exists(this.m_strUserDictionaryFile) | bClear)
                    {
                        File.Create(this.m_strUserDictionaryFile).Dispose();
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Failed to initialize the User Dictionary File '" + this.m_strUserDictionaryFile + "'. Error: " + exception.Message;
                }
                if (this.strError.Length > 0)
                {
                    this.ShowBalloon(this.strError, 5);
                    this.strError = string.Empty;
                }
            }
        }

        public bool BalloonWaiting { get; set; }
    }
}


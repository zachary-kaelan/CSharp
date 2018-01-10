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
using System.Threading;
using System.Windows.Forms;
using TaskBarApp.Objects;
using Zipwhip;

namespace TaskBarApp
{
	public class ApplicationManager
	{
		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly ApplicationManager.<>c <>9 = new ApplicationManager.<>c();

			public static Func<AccountItem, DateTime?> <>9__158_0;

			public static Predicate<AccountItem> <>9__176_0;

			internal DateTime? <GetAccountDashboard>b__158_0(AccountItem c)
			{
				return c.lastSyncDate;
			}

			internal bool <LogOut>b__176_0(AccountItem var)
			{
				return var.connectionStatus == "Logged In";
			}
		}

		public TextService m_textService;

		private readonly NotifyIcon notifyIcon;

		public Icon iTextApp;

		private string strError = string.Empty;

		private System.Windows.Forms.Timer monitorAccountDashboard;

		private System.Windows.Forms.Timer monitorTextMessages;

		private System.Windows.Forms.Timer monitorRefreshForm;

		private System.Windows.Forms.Timer monitorServerSync;

		public bool m_bConnected;

		public bool m_bRefreshDashboardForm;

		public bool m_bRefreshDashboardCounts;

		public bool m_bOpenDashboardForm;

		public bool m_bRefreshMessageFormConversationList;

		public bool m_bRefreshMessageFormProcessingLabel;

		public bool m_bOpenMessageForm;

		public bool m_bAccountDashboardLoading;

		public bool m_bTextMessagesLoading;

		public bool m_bGroupScheduleProcessing;

		public bool m_bGroupSend;

		public bool m_bSyncAllContacts;

		public int m_nAccountNDX;

		public string m_strMachineID = string.Empty;

		public string m_AssemblyVersion = string.Empty;

		public string m_strConnectedIconPath = string.Empty;

		public string m_strNotConnectedIconPath = string.Empty;

		public string m_strLoginSplashPath = string.Empty;

		public string m_strAutoUpdateFileURL = string.Empty;

		public string m_strUpdateFileURL = string.Empty;

		public string m_strBETAUpdateFileURL = string.Empty;

		public string m_strSession = string.Empty;

		public bool m_bMessageChanged;

		public bool m_bDashboardMode;

		public bool m_bDisableDashboardSettingChangeNotifications;

		public bool m_bIsLoggedOut;

		public bool m_bSaveLogIn = true;

		public bool m_bPlaySound = true;

		public bool m_bControlEnter = true;

		public bool m_bPopMessageWindow = true;

		public bool m_bPopDashboardWindow = true;

		public bool m_bPlayDashboardSound = true;

		public bool m_bEnableSignature = true;

		public bool m_bValidateMobileNumbers = true;

		public bool m_bRequreClickToMarkMessageRead = true;

		public bool m_bKeepConversationFocus;

		public bool m_bDisplayMMSAttachments;

		public bool m_bConversationCountLocked;

		public bool m_bNotifyServerSync;

		public string m_strRemoveGroupKeyword = "remove";

		public string m_strSignature = "-Signature";

		public string m_strSoundPath = string.Empty;

		public string m_strMessageTemplate1 = string.Empty;

		public string m_strMessageTemplate2 = string.Empty;

		public string m_strMessageTemplate3 = string.Empty;

		public string m_strMessageTemplate4 = string.Empty;

		public string m_strMessageTemplate5 = string.Empty;

		public string m_strMessageTemplate6 = string.Empty;

		public string m_strMessageTemplate7 = string.Empty;

		public string m_strMessageTemplate8 = string.Empty;

		public string m_strMessageTemplate9 = string.Empty;

		public string m_strMessageTemplate10 = string.Empty;

		public string m_strAccounTitle = string.Empty;

		public string m_strNotificationSound = string.Empty;

		public string m_strDashboardNotificationSound = string.Empty;

		public int m_nServerSyncRefreshInterval = 60;

		public int m_nAccountDashboardRefreshInterval = 10;

		public bool m_bAccountDashboardRefreshMessages = true;

		public bool m_bAccountDashboardRefreshMessagesManualClick;

		public int m_nConversationLimit = 50;

		public int m_nConversationLimitDefault = 50;

		public int m_nUnreadMessageLimit = 500;

		public int m_nLastMessageStatusLimit = 500;

		public int m_nContactLimit = 50;

		public int m_nGroupScheduleBackHourLimit = -24;

		public int m_nMonitorTextMessageInterval = 10;

		public int m_nNotificationInterval = 15;

		public int m_nGroupSendPauseIntervalSeconds = 1;

		public string m_strCSVScheduleFilePath = string.Empty;

		public string m_strCSVScheduleFile = string.Empty;

		public string m_strUserDictionaryFilePath = string.Empty;

		public string m_strUserDictionaryFile = string.Empty;

		public string m_strForwardMessage = string.Empty;

		public bool m_bKeywordFeature;

		public bool m_bGroupScheduleFeature;

		public bool m_bMessageTemplateFeature = true;

		public bool m_bDashboardFeature;

		public bool m_bMMSFeature = true;

		public bool m_bMMSSendFeature;

		public bool m_bDisableRemoteFeatureUpdates;

		public bool m_bDisableRemoteFeatureSync;

		public string m_strApplicationName = string.Empty;

		public string m_strSettingsURL = string.Empty;

		public string m_strHelpURL = string.Empty;

		public bool m_bIsBranded;

		public bool m_bAllowDelete;

		public bool m_bAllowBlock = true;

		public bool m_bEnableKeywordProcessing;

		public bool m_bNotifyKeywordProcessing;

		public bool m_bEnableGroupScheduleProcessing;

		public string m_strUserName = string.Empty;

		public string m_strPassword = string.Empty;

		public string m_strCountryCode = string.Empty;

		public bool m_bContactsInitiated;

		public long m_nMostRecentMetaDataMessageID;

		public long m_nCurrentContactID;

		public string m_strCurrentContactAddress = string.Empty;

		public string m_strCurentConversationFingerprint = string.Empty;

		public string m_strCurentProcessingMessage = string.Empty;

		public DateTime m_dtNextNotification = DateTime.Now;

		public List<TextMessage> m_lsUnReadMessages = new List<TextMessage>();

		public List<string> m_lsGroupTags = new List<string>();

		public List<GroupTagContact> m_lsGroupTagContacts = new List<GroupTagContact>();

		public List<AccountItem> m_lsAccountItems;

		public List<ConversationMetaData> m_lsConversationMetaData = new List<ConversationMetaData>();

		public List<Conversation> m_lsConversation;

		public List<Contact> m_lsContact = new List<Contact>();

		public List<TextMessage> m_lsMessages = new List<TextMessage>();

		public List<ScheduleFileItem> m_lsGroupSchedule = new List<ScheduleFileItem>();

		private List<string> gs_GroupSend;

		private string gs_TextMessage;

		private DateTime gs_ScheduleDate;

		private string gs_FilePath;

		public int m_nFontSize = 12;

		public int m_nFontSizeDT = 8;

		public SolidBrush m_brushHighlight = new SolidBrush(ColorTranslator.FromHtml("#93FF14"));

		public SolidBrush m_brushSelected = new SolidBrush(ColorTranslator.FromHtml("#E8FFCC"));

		public SolidBrush m_brushError = new SolidBrush(ColorTranslator.FromHtml("#fce3c2"));

		public SolidBrush m_brushBlack = new SolidBrush(Color.Black);

		public SolidBrush m_brushDimGray = new SolidBrush(Color.DimGray);

		public SolidBrush m_brushLightGray = new SolidBrush(Color.LightGray);

		public Pen m_penGray = new Pen(Color.DimGray, 2f);

		public Font m_fontSize = new Font("Arial", 12f);

		public Font m_fontSizeDT = new Font("Arial", 8f);

		public fmUserLogin formUserLogin;

		public fmMessages formMessages;

		public fmNewMessage formNewMessage;

		public fmEditContacts formEditContact;

		public fmSettings formSettings;

		public fmMessageTemplate formMessageTemplate;

		public fmEditGroups formEditGroups;

		public fmPrintConversation formPrintConversation;

		public fmAccountDashboard formAccountDashboard;

		public fmEditAccounts formEditAccounts;

		public fmKeywordAutoResponse formKeywordAutoResponse;

		public fmGroupSchedule formGroupSchedule;

		public fmEditGroupSchedule formEditGroupSchedule;

		public bool BalloonWaiting
		{
			get;
			set;
		}

		public ApplicationManager(NotifyIcon notifyIcon)
		{
			this.notifyIcon = notifyIcon;
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
			int interval = 1000 * this.m_nMonitorTextMessageInterval;
			this.monitorTextMessages.Interval = interval;
			this.monitorTextMessages.Enabled = true;
			this.monitorTextMessages.Start();
		}

		public void RunMonitorTextMessageTimer(object sender, EventArgs e)
		{
			this.LoadUpdates(true);
			this.SendGroupSchedule();
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
			int interval = 60000 * this.m_nAccountDashboardRefreshInterval;
			this.monitorAccountDashboard.Interval = interval;
			this.monitorAccountDashboard.Enabled = true;
			this.monitorAccountDashboard.Start();
		}

		public void RunMonitorAcountDashboardTimer(object sender, EventArgs e)
		{
			this.LoadAccountDashboard();
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
			int interval = 1000;
			this.monitorRefreshForm.Interval = interval;
			this.monitorRefreshForm.Enabled = true;
			this.monitorRefreshForm.Start();
		}

		public void RunMonitorNotifyFormTimer(object sender, EventArgs e)
		{
			try
			{
				if (this.m_bRefreshMessageFormProcessingLabel && this.formMessages != null && this.formMessages.ContainsFocus)
				{
					this.formMessages.DisplayProcessingMessage(this.m_strCurentProcessingMessage);
					this.m_bRefreshMessageFormProcessingLabel = false;
				}
				if (this.m_bRefreshMessageFormConversationList && this.formMessages != null)
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
					if (this.formAccountDashboard != null && this.formAccountDashboard.ContainsFocus)
					{
						this.formAccountDashboard.LoadGridViewAccounts();
					}
					this.m_bRefreshDashboardForm = false;
				}
				if (this.m_bRefreshDashboardCounts)
				{
					if (this.formAccountDashboard != null && this.formAccountDashboard.ContainsFocus)
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
			int interval = 60000 * this.m_nServerSyncRefreshInterval;
			this.monitorServerSync.Interval = interval;
			this.monitorServerSync.Enabled = true;
			this.monitorServerSync.Start();
		}

		public void RunMonitorServerSyncTimer(object sender, EventArgs e)
		{
			if (this.m_bDisableRemoteFeatureSync)
			{
				return;
			}
			new Thread(new ThreadStart(this.LoadServerSettings)).Start();
		}

		public void InitializeApplicationVariables()
		{
			try
			{
				this.m_strMachineID = AppRegistry.GetMachineID();
				this.m_AssemblyVersion = "v" + Assembly.GetCallingAssembly().GetName().Version.ToString();
				string str = string.Empty;
				try
				{
					str = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath;
				}
				catch
				{
					if (Directory.Exists("C:\\Program Files\\Line1\\TextService"))
					{
						str = "C:\\Program Files\\Line1\\TextService";
					}
					else if (Directory.Exists("C:\\Program Files (x86)\\Line1\\TextService"))
					{
						str = "C:\\Program Files (x86)\\Line1\\TextService";
					}
				}
				this.iTextApp = new Icon(str + "\\TextApp.ico");
				try
				{
					this.m_strApplicationName = File.ReadAllText(str + "\\Resources\\ApplicationName.ini");
				}
				catch
				{
					this.m_strApplicationName = "TextBox";
				}
				try
				{
					this.m_strHelpURL = File.ReadAllText(str + "\\Resources\\HelpURL.ini");
				}
				catch
				{
					this.m_strHelpURL = "http://www.textbox.cc/help";
				}
				try
				{
					this.m_strSettingsURL = File.ReadAllText(str + "\\Resources\\SettingsURL.ini");
				}
				catch
				{
					this.m_strSettingsURL = "http://www.textbox.cc/settings";
				}
				if (this.m_strApplicationName != "TextBox")
				{
					this.m_bIsBranded = true;
				}
				this.m_strSoundPath = str + "\\Resources\\";
				this.m_strConnectedIconPath = str + "\\Resources\\Connected.ico";
				this.m_strNotConnectedIconPath = str + "\\Resources\\NotConnected.ico";
				this.m_strLoginSplashPath = str + "\\Resources\\BackgroundSplash.png";
				this.UserDictionaryFileInitialize(false);
			}
			catch (Exception ex)
			{
				this.ShowBalloon("There was an error getting application variables: " + ex.Message, 5);
			}
		}

		public void InitializeAccountList()
		{
			try
			{
				this.m_lsAccountItems = new List<AccountItem>();
				RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
				string[] subKeyNames = AppRegistry.GetSubKeyNames(rootKey, null, ref this.strError);
				for (int i = 0; i < subKeyNames.Length; i++)
				{
					string text = subKeyNames[i];
					if (this.IsDigitsOnly(text))
					{
						RegistryKey subKey = AppRegistry.GetSubKey(rootKey, text, false, ref this.strError);
						AccountItem accountItem = new AccountItem();
						string empty = string.Empty;
						AppRegistry.GetValue(subKey, "Title", ref empty, ref this.strError);
						accountItem.title = empty;
						AppRegistry.GetValue(subKey, "CountryCode", ref empty, ref this.strError);
						accountItem.countryCode = empty;
						accountItem.session = string.Empty;
						accountItem.number = AppRegistry.GetUserName(subKey, ref this.strError);
						accountItem.password = AppRegistry.GetPassword(subKey, ref this.strError);
						accountItem.connectionStatus = "Unknown";
						accountItem.unReadMessageList = new List<TextMessage>();
						this.m_lsAccountItems.Add(accountItem);
					}
				}
			}
			catch (Exception ex)
			{
				this.ShowBalloon("There was an error getting multiple account information: " + ex.Message, 5);
			}
		}

		public void InitializeAccountRegistryConfiguration(bool bSync)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.GetValue(expr_0B, "local_DashboardNotificationSound", ref this.m_strDashboardNotificationSound, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strDashboardNotificationSound))
				{
					this.m_strDashboardNotificationSound = "DoorBell.wav";
				}
				else if (this.m_strDashboardNotificationSound == "None")
				{
					this.m_bPlayDashboardSound = false;
				}
				AppRegistry.GetValue(expr_0B, "local_AccountDashboardRefreshInterval", ref this.m_nAccountDashboardRefreshInterval, ref this.strError);
				AppRegistry.GetValue(expr_0B, "local_DisableDashboardSettingChangeNotifications", ref this.m_bDisableDashboardSettingChangeNotifications, ref this.strError);
				AppRegistry.GetValue(expr_0B, "local_DashboardMode", ref this.m_bDashboardMode, ref this.strError);
				AppRegistry.GetValue(expr_0B, "local_SaveLogIn", ref this.m_bSaveLogIn, ref this.strError);
				AppRegistry.GetValue(expr_0B, "local_DisableRemoteFeatureUpdates", ref this.m_bDisableRemoteFeatureUpdates, ref this.strError);
				AppRegistry.GetValue(expr_0B, "FontSize", ref this.m_nFontSize, ref this.strError);
				AppRegistry.GetValue(expr_0B, "FontSizeDT", ref this.m_nFontSizeDT, ref this.strError);
				this.m_fontSize = new Font("Arial", (float)this.m_nFontSize);
				this.m_fontSizeDT = new Font("Arial", (float)this.m_nFontSizeDT);
				AppRegistry.GetValue(expr_0B, "AutoUpdateFile", ref this.m_strAutoUpdateFileURL, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strAutoUpdateFileURL))
				{
					this.m_strAutoUpdateFileURL = "http://www.isready.us/updates/TextApp.xml";
				}
				AppRegistry.GetValue(expr_0B, "CurrentUpdateFile", ref this.m_strUpdateFileURL, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strUpdateFileURL))
				{
					this.m_strUpdateFileURL = "http://www.isready.us/updates/TextAppCurrentVersion.xml";
				}
				AppRegistry.GetValue(expr_0B, "BETAUpdateFile", ref this.m_strBETAUpdateFileURL, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strBETAUpdateFileURL))
				{
					this.m_strBETAUpdateFileURL = "http://www.isready.us/updates/TextAppBETAVersion.xml";
				}
				AppRegistry.GetValue(expr_0B, "NotificationSound", ref this.m_strNotificationSound, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strNotificationSound))
				{
					this.m_strNotificationSound = "DoorBell.wav";
				}
				else if (this.m_strNotificationSound == "None")
				{
					this.m_bPlaySound = false;
				}
				string empty = string.Empty;
				AppRegistry.GetValue(expr_0B, "Signature", ref empty, ref this.strError);
				if (!string.IsNullOrEmpty(empty))
				{
					this.m_strSignature = empty;
				}
				AppRegistry.GetValue(expr_0B, "RemoveGroupKeyword", ref this.m_strRemoveGroupKeyword, ref this.strError);
				if (string.IsNullOrEmpty(this.m_strRemoveGroupKeyword))
				{
					this.m_strRemoveGroupKeyword = "remove";
				}
				AppRegistry.GetValue(expr_0B, "ConversationLimitDefault", ref this.m_nConversationLimitDefault, ref this.strError);
				this.m_nConversationLimit = this.m_nConversationLimitDefault;
				AppRegistry.GetValue(expr_0B, "UnreadMessageLimit", ref this.m_nUnreadMessageLimit, ref this.strError);
				AppRegistry.GetValue(expr_0B, "PopMessageWindow", ref this.m_bPopMessageWindow, ref this.strError);
				AppRegistry.GetValue(expr_0B, "NotificationInterval", ref this.m_nNotificationInterval, ref this.strError);
				AppRegistry.GetValue(expr_0B, "TextMessageIntervalSeconds", ref this.m_nMonitorTextMessageInterval, ref this.strError);
				AppRegistry.GetValue(expr_0B, "GroupSendPauseIntervalSeconds", ref this.m_nGroupSendPauseIntervalSeconds, ref this.strError);
				AppRegistry.GetValue(expr_0B, "EnableSignature", ref this.m_bEnableSignature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "EnableKeywordProcessing", ref this.m_bEnableKeywordProcessing, ref this.strError);
				AppRegistry.GetValue(expr_0B, "NotifyKeywordProcessing", ref this.m_bNotifyKeywordProcessing, ref this.strError);
				AppRegistry.GetValue(expr_0B, "EnableGroupScheduleProcessing", ref this.m_bEnableGroupScheduleProcessing, ref this.strError);
				AppRegistry.GetValue(expr_0B, "ValidateMobileNumbers", ref this.m_bValidateMobileNumbers, ref this.strError);
				AppRegistry.GetValue(expr_0B, "ClickMarkMessageRead", ref this.m_bRequreClickToMarkMessageRead, ref this.strError);
				AppRegistry.GetValue(expr_0B, "KeepConversationFocus", ref this.m_bKeepConversationFocus, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate1", ref this.m_strMessageTemplate1, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate2", ref this.m_strMessageTemplate2, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate3", ref this.m_strMessageTemplate3, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate4", ref this.m_strMessageTemplate4, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate5", ref this.m_strMessageTemplate5, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate6", ref this.m_strMessageTemplate6, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate7", ref this.m_strMessageTemplate7, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate8", ref this.m_strMessageTemplate8, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate9", ref this.m_strMessageTemplate9, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplate10", ref this.m_strMessageTemplate10, ref this.strError);
				AppRegistry.GetValue(expr_0B, "PopDashboardWindow", ref this.m_bPopDashboardWindow, ref this.strError);
				AppRegistry.GetValue(expr_0B, "ControlEnter", ref this.m_bControlEnter, ref this.strError);
				AppRegistry.GetValue(expr_0B, "DisplayMMSAttachments", ref this.m_bDisplayMMSAttachments, ref this.strError);
				AppRegistry.GetValue(expr_0B, "AllowDelete", ref this.m_bAllowDelete, ref this.strError);
				AppRegistry.GetValue(expr_0B, "AllowBlock", ref this.m_bAllowBlock, ref this.strError);
				AppRegistry.GetValue(expr_0B, "LastMessageStatusLimit", ref this.m_nLastMessageStatusLimit, ref this.strError);
				AppRegistry.GetValue(expr_0B, "KeywordFeature", ref this.m_bKeywordFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "GroupScheduleFeature", ref this.m_bGroupScheduleFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MessageTemplateFeature", ref this.m_bMessageTemplateFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "DashboardFeature", ref this.m_bDashboardFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MMSFeature", ref this.m_bMMSFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "MMSSendFeature", ref this.m_bMMSSendFeature, ref this.strError);
				AppRegistry.GetValue(expr_0B, "DisableRemoteFeatureSync", ref this.m_bDisableRemoteFeatureSync, ref this.strError);
				AppRegistry.GetValue(expr_0B, "FeatureSyncInterval", ref this.m_nServerSyncRefreshInterval, ref this.strError);
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
				AppRegistry.SaveValue(expr_0B, "local_DisableRemoteFeatureUpdates", this.m_bDisableRemoteFeatureUpdates, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_ApplicationVersion", this.m_AssemblyVersion, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_DashboardMode", this.m_bDashboardMode, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_SaveLogIn", this.m_bSaveLogIn, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_DashboardNotificationSound", this.m_strDashboardNotificationSound, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_AccountDashboardRefreshInterval", this.m_nAccountDashboardRefreshInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_DisableDashboardSettingChangeNotifications", this.m_bDisableDashboardSettingChangeNotifications, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "FontSize", this.m_nFontSize, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "FontSizeDT", this.m_nFontSizeDT, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "ConversationLimitDefault", this.m_nConversationLimitDefault, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "NotificationSound", this.m_strNotificationSound, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "GroupScheduleBackHourLimit", this.m_nGroupScheduleBackHourLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "TextMessageIntervalSeconds", this.m_nMonitorTextMessageInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "GroupSendPauseIntervalSeconds", this.m_nGroupSendPauseIntervalSeconds, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MMSSendFeature", this.m_bMMSSendFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MMSFeature", this.m_bMMSFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "DashboardFeature", this.m_bDashboardFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplateFeature", this.m_bMessageTemplateFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "GroupScheduleFeature", this.m_bGroupScheduleFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "KeywordFeature", this.m_bKeywordFeature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "DisableRemoteFeatureSync", this.m_bDisableRemoteFeatureSync, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "FeatureSyncInterval", this.m_nServerSyncRefreshInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "AllowDelete", this.m_bAllowDelete, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "AllowBlock", this.m_bAllowBlock, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "LastMessageStatusLimit", this.m_nLastMessageStatusLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "DisplayMMSAttachments", this.m_bDisplayMMSAttachments, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "ControlEnter", this.m_bControlEnter, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "PopDashboardWindow", this.m_bPopDashboardWindow, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate10", this.m_strMessageTemplate10, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate9", this.m_strMessageTemplate9, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate8", this.m_strMessageTemplate8, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate7", this.m_strMessageTemplate7, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate6", this.m_strMessageTemplate6, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate5", this.m_strMessageTemplate5, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate4", this.m_strMessageTemplate4, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate3", this.m_strMessageTemplate3, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate2", this.m_strMessageTemplate2, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "MessageTemplate1", this.m_strMessageTemplate1, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "KeepConversationFocus", this.m_bKeepConversationFocus, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "ClickMarkMessageRead", this.m_bRequreClickToMarkMessageRead, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "ValidateMobileNumbers", this.m_bValidateMobileNumbers, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "EnableGroupScheduleProcessing", this.m_bEnableGroupScheduleProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "NotifyKeywordProcessing", this.m_bNotifyKeywordProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "EnableKeywordProcessing", this.m_bEnableKeywordProcessing, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "EnableSignature", this.m_bEnableSignature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "NotificationInterval", this.m_nNotificationInterval, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "PopMessageWindow", this.m_bPopMessageWindow, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "UnreadMessageLimit", this.m_nUnreadMessageLimit, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "Signature", this.m_strSignature, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "RemoveGroupKeyword", this.m_strRemoveGroupKeyword, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "AutoUpdateFile", this.m_strAutoUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "UpdateFile", this.m_strUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "BETAUpdateFile", this.m_strBETAUpdateFileURL, ref this.strError, bSync, RegistryValueKind.Unknown);
			}
			catch (Exception ex)
			{
				this.strError = this.strError + "There was an error getting account registry configuration: " + ex.Message;
			}
			if (this.strError.Length > 0)
			{
				this.ShowBalloon(this.strError, 5);
			}
		}

		public void UserDictionaryFileInitialize(bool bClear = false)
		{
			this.m_strUserDictionaryFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Dictionary\\";
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
				catch (Exception ex)
				{
					this.strError = "Failed to initialize the User Dictionary File '" + this.m_strUserDictionaryFile + "'. Error: " + ex.Message;
				}
				if (this.strError.Length > 0)
				{
					this.ShowBalloon(this.strError, 5);
					this.strError = string.Empty;
				}
			}
		}

		public void GroupSchedulFileInitialize(bool bClear = false)
		{
			this.m_strCSVScheduleFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CSV\\";
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
						StreamWriter expr_7E = new StreamWriter(this.m_strCSVScheduleFile);
						new CsvWriter(expr_7E).WriteRecords(records);
						expr_7E.Close();
						expr_7E.Dispose();
					}
				}
				catch (Exception ex)
				{
					this.strError = "Failed to initialize the Group Schedule File '" + this.m_strCSVScheduleFile + "'. Error: " + ex.Message;
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
			List<ScheduleFileItem> lsGroupSchedule = new List<ScheduleFileItem>();
			try
			{
				if (this.m_strCSVScheduleFile.Length == 0)
				{
					this.GroupSchedulFileInitialize(false);
				}
				StreamReader expr_30 = new StreamReader(this.m_strCSVScheduleFile);
				lsGroupSchedule = new CsvReader(expr_30).GetRecords<ScheduleFileItem>().ToList<ScheduleFileItem>();
				expr_30.Close();
			}
			catch (Exception ex)
			{
				this.strError = ex.Message;
			}
			if (this.strError.Length > 0)
			{
				this.ShowBalloon("There was an error loading the Group Schedule File" + this.strError, 5);
				this.strError = string.Empty;
			}
			this.m_lsGroupSchedule = lsGroupSchedule;
		}

		public void GroupSchedulFileSave(List<ScheduleFileItem> csvRecords)
		{
			this.strError = string.Empty;
			try
			{
				StreamWriter expr_16 = new StreamWriter(this.m_strCSVScheduleFile);
				new CsvWriter(expr_16).WriteRecords(csvRecords);
				expr_16.Close();
				expr_16.Dispose();
			}
			catch (Exception ex)
			{
				this.strError = "Group Schedule file Save error: " + ex.Message;
			}
			if (this.strError.Length == 0)
			{
				this.GroupSchedulFileLoad();
				return;
			}
			this.ShowBalloon("There was an error saving the Group Schedule File" + this.strError, 5);
			this.strError = string.Empty;
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
			catch (Exception ex)
			{
				this.strError = this.strError + "Group Schedule file Add error: " + ex.Message;
			}
			return this.strError;
		}

		public ScheduleFileItem GroupScheduleFileValidateItem(ScheduleFileItem validateItem)
		{
			string text = validateItem.SendStatus;
			if (text.Contains("Error:"))
			{
				text = "";
			}
			if (!text.Contains("Sent:"))
			{
				if (validateItem.TextMessage.Length > 250)
				{
					text += "Error: Text message over 250 characters";
				}
				if (validateItem.TextMessage.Length == 0)
				{
					text += "Error: Must have a text message.";
				}
				DateTime dateTime;
				if (!DateTime.TryParse(validateItem.SendDate, out dateTime))
				{
					text += "Error: Send Date not valid";
				}
				if (validateItem.Address.Substring(0, 1) == "#")
				{
					if (!this.m_lsGroupTags.Contains(validateItem.Address))
					{
						text = text + "Error: Group tag (" + validateItem.Address + ") does not exist";
					}
				}
				else
				{
					string text2 = this.FormatContactAddress(validateItem.Address, true, false);
					if (!this.IsDigitsOnly(text2) || !this.IsValidMobileNumber(text2))
					{
						text += "Error: Phone number is not correctly formated";
					}
				}
				validateItem.SendStatus = text;
			}
			return validateItem;
		}

		public void SendGroupSchedule()
		{
			if (this.m_bGroupScheduleProcessing)
			{
				return;
			}
			new Thread(new ThreadStart(this.ProcessGroupSchedule)).Start();
		}

		public void SendGroupMessage(List<string> groupSend, string textMessage, DateTime scheduleDate, string filePath)
		{
			this.gs_GroupSend = groupSend;
			this.gs_TextMessage = textMessage;
			this.gs_ScheduleDate = scheduleDate;
			this.gs_FilePath = filePath;
			new Thread(new ThreadStart(this.ProcessGroupMessage)).Start();
		}

		private void SetUnreadMessageCount()
		{
			if (this.m_bConnected)
			{
				try
				{
					List<TextMessage> list = new List<TextMessage>();
					IRestResponse<TextMessageList> messageList = this.m_textService.GetMessageList(this.m_strSession, 0, this.m_nUnreadMessageLimit);
					if (messageList.Data.total > 0)
					{
						foreach (TextMessage current in messageList.Data.response)
						{
							if (!current.isRead && this.FormatContactAddress(current.destAddress, true, true) == this.FormatContactAddress(this.m_strUserName, true, true))
							{
								list.Add(current);
							}
						}
					}
					this.m_lsUnReadMessages = list;
				}
				catch (Exception ex)
				{
					this.ShowBalloon("Exception setting unread message list: " + ex.Message, 5);
				}
			}
		}

		public void LoadAccountDashboard()
		{
			if (this.m_bAccountDashboardLoading)
			{
				return;
			}
			new Thread(new ThreadStart(this.GetAccountDashboard)).Start();
		}

		private void GetAccountDashboard()
		{
			this.m_bAccountDashboardLoading = true;
			List<AccountItem> list = this.m_lsAccountItems;
			IEnumerable<AccountItem> arg_2E_0 = list;
			Func<AccountItem, DateTime?> arg_2E_1;
			if ((arg_2E_1 = ApplicationManager.<>c.<>9__158_0) == null)
			{
				arg_2E_1 = (ApplicationManager.<>c.<>9__158_0 = new Func<AccountItem, DateTime?>(ApplicationManager.<>c.<>9.<GetAccountDashboard>b__158_0));
			}
			list = arg_2E_0.OrderBy(arg_2E_1).ToList<AccountItem>();
			this.m_nAccountNDX = 0;
			int count = list.Count;
			if (this.m_textService == null)
			{
				this.m_textService = new TextService();
			}
			while (this.m_nAccountNDX < count)
			{
				AccountItem accountItem = list[this.m_nAccountNDX];
				bool flag = false;
				bool flag2 = false;
				string text = string.Empty;
				string connectionStatus = accountItem.connectionStatus;
				try
				{
					if (accountItem.connectionStatus != "Failed Authentication")
					{
						if (string.IsNullOrEmpty(accountItem.session) || accountItem.connectionStatus == "Unknown")
						{
							try
							{
								if (AppRegistry.AuthorizeUser(accountItem.number, ref this.strError))
								{
									flag = this.m_textService.AccountLogIn(accountItem.number, accountItem.password, accountItem.countryCode, ref text);
								}
							}
							catch
							{
								flag2 = true;
							}
							if (!flag)
							{
								connectionStatus = "Failed Authentication";
								text = string.Empty;
							}
							if (flag)
							{
								connectionStatus = "Connected";
								RegistryKey subKey = AppRegistry.GetSubKey(AppRegistry.GetRootKey(ref this.strError), accountItem.number, true, ref this.strError);
								if (this.strError == string.Empty)
								{
									AppRegistry.SaveValue(subKey, "local_Session", text, ref this.strError, false, RegistryValueKind.Unknown);
								}
							}
						}
						else
						{
							text = accountItem.session;
							if (!string.IsNullOrEmpty(accountItem.session))
							{
								connectionStatus = "Connected";
							}
						}
						if (accountItem.number == this.m_strUserName && this.m_bConnected)
						{
							accountItem.connectionStatus = "Logged In";
						}
						else
						{
							accountItem.connectionStatus = connectionStatus;
						}
						accountItem.session = text;
						if (!string.IsNullOrEmpty(accountItem.session))
						{
							try
							{
								if (this.m_bAccountDashboardRefreshMessages || !accountItem.lastSyncDate.HasValue)
								{
									IRestResponse<TextMessageList> messageList = this.m_textService.GetMessageList(accountItem.session, 0, this.m_nUnreadMessageLimit);
									if (messageList.Data.success)
									{
										if (messageList.Data.total <= 0)
										{
											goto IL_44E;
										}
										accountItem.lastSyncDate = new DateTime?(DateTime.Now);
										accountItem.unReadMessageList = new List<TextMessage>();
										using (List<TextMessage>.Enumerator enumerator = messageList.Data.response.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												TextMessage current = enumerator.Current;
												if (!current.isRead && this.FormatContactAddress(current.destAddress, true, true) == this.FormatContactAddress(accountItem.number, true, true))
												{
													accountItem.unReadMessageList.Add(current);
												}
											}
											goto IL_44E;
										}
									}
									if (messageList.ResponseStatus.ToString() != "Completed")
									{
										accountItem.session = string.Empty;
										connectionStatus = "Unknown";
									}
								}
								else
								{
									accountItem.lastSyncDate = new DateTime?(DateTime.Now);
									int lastCheck = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
									IRestResponse<UpdateGet> updates = this.m_textService.GetUpdates(accountItem.session, lastCheck);
									if (updates.Data != null)
									{
										foreach (Session current2 in updates.Data.sessions)
										{
											if (current2.message != null)
											{
												using (List<TextMessage>.Enumerator enumerator = current2.message.GetEnumerator())
												{
													while (enumerator.MoveNext())
													{
														TextMessage message = enumerator.Current;
														if (this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(accountItem.number, true, true))
														{
															TextMessage textMessage = accountItem.unReadMessageList.Find((TextMessage m) => m.id == message.id);
															if (textMessage == null && !message.isRead)
															{
																accountItem.unReadMessageList.Add(message);
															}
															if (textMessage != null && message.isRead)
															{
																accountItem.unReadMessageList.Remove(textMessage);
															}
														}
													}
												}
											}
										}
									}
								}
								IL_44E:;
							}
							catch (Exception ex)
							{
								flag2 = true;
								accountItem.session = string.Empty;
								accountItem.connectionStatus = "Unknown";
								this.ShowBalloon("There was an error getting message counts for " + this.FormatPhone(accountItem.number) + ": " + ex.Message, 5);
							}
						}
						list[this.m_nAccountNDX] = accountItem;
					}
				}
				catch (Exception ex2)
				{
					this.ShowBalloon("There was an error connecting to " + this.FormatPhone(accountItem.number) + ": " + ex2.Message, 5);
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
				this.ShowBalloon(string.Concat(new string[]
				{
					"You have ",
					this.GetTotalDashboardUnread(false).ToString(),
					" unread text messages from ",
					this.m_lsAccountItems.Count.ToString(),
					" accounts on the Account Dashboard."
				}), 5);
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

		public void LoadUpdates(bool bUseNewThread = false)
		{
			if (this.m_bTextMessagesLoading)
			{
				return;
			}
			if (bUseNewThread)
			{
				new Thread(new ThreadStart(this.GetUpdates)).Start();
				return;
			}
			this.GetUpdates();
		}

		private void GetUpdates()
		{
			this.m_bTextMessagesLoading = true;
			if (this.m_bConnected)
			{
				try
				{
					bool flag = false;
					int lastCheck = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
					IRestResponse<UpdateGet> updates = this.m_textService.GetUpdates(this.m_strSession, lastCheck);
					if (updates.Data != null)
					{
						foreach (Session current in updates.Data.sessions)
						{
							if (current.message != null)
							{
								this.m_bRefreshMessageFormConversationList = true;
								using (List<TextMessage>.Enumerator enumerator2 = current.message.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										TextMessage message = enumerator2.Current;
										if (this.FormatContactAddress(message.destAddress, true, true) == this.FormatContactAddress(this.m_strUserName, true, true) && !this.ProcessMessageForKeyword(message))
										{
											TextMessage textMessage = this.m_lsUnReadMessages.Find((TextMessage m) => m.id == message.id);
											if (textMessage == null && !message.isRead)
											{
												this.m_lsUnReadMessages.Add(message);
												flag = true;
											}
											if (textMessage != null && message.isRead)
											{
												this.m_lsUnReadMessages.Remove(textMessage);
											}
										}
										if (message.fingerprint == this.m_strCurentConversationFingerprint)
										{
											TextMessage textMessage2 = this.m_lsMessages.Find((TextMessage m) => m.id == message.id);
											if (textMessage2 == null)
											{
												this.m_lsMessages.Add(message);
												this.m_bMessageChanged = true;
											}
											else
											{
												this.m_lsMessages.Remove(textMessage2);
												this.m_lsMessages.Add(message);
											}
										}
										DateTime value;
										DateTime.TryParse(message.dateCreated, out value);
										ConversationMetaData conversationMetaData = new ConversationMetaData();
										ConversationMetaData conversationMetaData2 = this.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == message.fingerprint);
										if (conversationMetaData2 != null)
										{
											this.m_lsConversationMetaData.Remove(conversationMetaData2);
										}
										conversationMetaData.fingerprint = message.fingerprint;
										conversationMetaData.lastContactId = message.contactId;
										conversationMetaData.lastMessageDate = new DateTime?(value);
										if (message.destAddress == this.m_strUserName)
										{
											conversationMetaData.lastMessageDirection = "In";
										}
										else
										{
											conversationMetaData.lastMessageDirection = "Out";
										}
										if (message.transmissionState.name == "ERROR")
										{
											conversationMetaData.lastMessageIsError = true;
										}
										else
										{
											conversationMetaData.lastMessageIsError = false;
										}
										this.m_lsConversationMetaData.Add(conversationMetaData);
									}
								}
							}
							if (current.conversation != null)
							{
								this.m_bRefreshMessageFormConversationList = true;
								using (List<Conversation>.Enumerator enumerator3 = current.conversation.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										Conversation conversation = enumerator3.Current;
										Conversation conversation2 = this.m_lsConversation.Find((Conversation var) => var.id == conversation.id);
										if (conversation2 != null)
										{
											this.m_lsConversation.Remove(conversation2);
										}
										if (!conversation.deleted)
										{
											this.m_lsConversation.Add(conversation);
										}
									}
								}
							}
							if (current.contact != null)
							{
								this.m_bRefreshMessageFormConversationList = true;
								using (List<Contact>.Enumerator enumerator4 = current.contact.GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										Contact contact = enumerator4.Current;
										string address = contact.address;
										if (this.FormatPhone(address) != "GROUP")
										{
											Contact contact2 = this.m_lsContact.Find((Contact var) => var.id == contact.id);
											if (contact2 != null)
											{
												this.m_lsContact.Remove(contact2);
											}
											if (!contact.deleted)
											{
												this.m_lsContact.Add(contact);
												List<string> arg_442_0 = this.GetContactGroupTags(contact.notes);
												this.m_lsGroupTagContacts = (from var in this.m_lsGroupTagContacts
												where var.contactId != contact.id
												select var).ToList<GroupTagContact>();
												foreach (string current2 in arg_442_0)
												{
													string text = contact.firstName + " " + contact.lastName;
													if (text.Trim().Length == 0)
													{
														text = "Unnamed";
													}
													else
													{
														text = text.Trim();
													}
													GroupTagContact groupTagContact = new GroupTagContact();
													groupTagContact.groupTag = current2;
													groupTagContact.contactId = contact.id;
													groupTagContact.contact = text + " " + this.FormatPhone(contact.mobileNumber);
													groupTagContact.address = contact.address;
													this.m_lsGroupTagContacts.Add(groupTagContact);
													if (!this.m_lsGroupTags.Contains(current2))
													{
														this.m_lsGroupTags.Add(current2);
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
					if (flag || (this.m_dtNextNotification <= DateTime.Now && this.m_lsUnReadMessages.Count<TextMessage>() > 0))
					{
						this.m_dtNextNotification = DateTime.Now.AddMinutes((double)this.m_nNotificationInterval);
						string text2 = "You have " + this.m_lsUnReadMessages.Count<TextMessage>().ToString() + " unread text messages for account " + this.FormatPhone(this.m_strUserName);
						this.ShowBalloon(text2, 5);
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

		public void LoadContacts(bool bUseNewThread = false)
		{
			if (bUseNewThread)
			{
				new Thread(new ThreadStart(this.GetContacts)).Start();
				return;
			}
			this.GetContacts();
		}

		private void GetContacts()
		{
			if (this.m_bConnected)
			{
				try
				{
					List<Contact> list = this.m_lsContact;
					List<string> list2 = this.m_lsGroupTags;
					List<GroupTagContact> list3 = this.m_lsGroupTagContacts;
					IRestResponse<ContactListResponse> contactList = this.m_textService.GetContactList(this.m_strUserName, this.m_strSession, 1, 999);
					int i = contactList.Data.pages;
					int num = 200;
					if (list.Count == 0 || this.m_bSyncAllContacts)
					{
						this.m_bSyncAllContacts = false;
						num = i * contactList.Data.total;
						list = new List<Contact>();
						list2 = new List<string>();
						list3 = new List<GroupTagContact>();
					}
					while (i > 0)
					{
						Application.DoEvents();
						using (List<Contact>.Enumerator enumerator = contactList.Data.response.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Contact contact = enumerator.Current;
								Application.DoEvents();
								string address = contact.address;
								if (this.FormatPhone(address) != "GROUP")
								{
									Contact contact2 = list.Find((Contact var) => var.id == contact.id);
									if (contact2 != null)
									{
										list.Remove(contact2);
									}
									list.Add(contact);
									List<string> arg_148_0 = this.GetContactGroupTags(contact.notes);
									list3 = (from var in list3
									where var.contactId != contact.id
									select var).ToList<GroupTagContact>();
									foreach (string current in arg_148_0)
									{
										string text = contact.firstName + " " + contact.lastName;
										if (text.Trim().Length == 0)
										{
											text = "Unnamed";
										}
										else
										{
											text = text.Trim();
										}
										list3.Add(new GroupTagContact
										{
											groupTag = current,
											contactId = contact.id,
											contact = text + " " + this.FormatPhone(contact.mobileNumber),
											address = contact.address
										});
										if (!list2.Contains(current))
										{
											list2.Add(current);
										}
									}
								}
							}
						}
						if (list.Count >= num || i == 1)
						{
							break;
						}
						contactList = this.m_textService.GetContactList(this.m_strUserName, this.m_strSession, i, 999);
						i--;
					}
					this.m_lsContact = list;
					this.m_lsGroupTagContacts = list3;
					this.m_lsGroupTags = list2;
					this.m_bContactsInitiated = true;
				}
				catch (Exception ex)
				{
					this.ShowBalloon("Exception getting contact list: " + ex.Message, 5);
				}
			}
		}

		public void LoadConversations(bool bUseNewThread = false)
		{
			if (bUseNewThread)
			{
				new Thread(new ThreadStart(this.GetConversations)).Start();
				return;
			}
			this.GetConversations();
		}

		private void GetConversations()
		{
			if (this.m_bConnected)
			{
				try
				{
					List<Conversation> lsConversation = this.m_lsConversation;
					List<ConversationMetaData> list = new List<ConversationMetaData>();
					lsConversation = this.m_textService.GetConversationList(this.m_strSession, 0, this.m_nConversationLimit).Data.response;
					if (this.m_nLastMessageStatusLimit > 0)
					{
						IRestResponse<TextMessageList> messageList = this.m_textService.GetMessageList(this.m_strSession, 0, 1);
						if (messageList.Data.total > 0)
						{
							TextMessage textMessage = messageList.Data.response[0];
							if (this.m_nMostRecentMetaDataMessageID != textMessage.id)
							{
								this.m_nMostRecentMetaDataMessageID = textMessage.id;
								messageList = this.m_textService.GetMessageList(this.m_strSession, 0, this.m_nLastMessageStatusLimit);
								if (messageList.Data.total > 0)
								{
									using (List<TextMessage>.Enumerator enumerator = messageList.Data.response.GetEnumerator())
									{
										while (enumerator.MoveNext())
										{
											TextMessage message = enumerator.Current;
											DateTime dateTime;
											DateTime.TryParse(message.dateCreated, out dateTime);
											ConversationMetaData conversationMetaData = new ConversationMetaData();
											Application.DoEvents();
											bool flag = true;
											bool flag2 = false;
											foreach (ConversationMetaData current in list)
											{
												Application.DoEvents();
												if (current.fingerprint == message.fingerprint)
												{
													flag = false;
													if (current.lastMessageDate < dateTime)
													{
														flag2 = true;
														flag = true;
													}
												}
											}
											if (flag2)
											{
												list = (from val in list
												where val.fingerprint != message.fingerprint
												select val).ToList<ConversationMetaData>();
											}
											if (flag)
											{
												conversationMetaData.fingerprint = message.fingerprint;
												conversationMetaData.lastContactId = message.contactId;
												conversationMetaData.lastMessageDate = new DateTime?(dateTime);
												if (message.destAddress == this.m_strUserName)
												{
													conversationMetaData.lastMessageDirection = "In";
												}
												else
												{
													conversationMetaData.lastMessageDirection = "Out";
												}
												if (message.transmissionState.name == "ERROR")
												{
													conversationMetaData.lastMessageIsError = true;
												}
												else
												{
													conversationMetaData.lastMessageIsError = false;
												}
												list.Add(conversationMetaData);
											}
										}
									}
								}
								this.m_lsConversationMetaData = list;
							}
						}
					}
					this.m_lsConversation = lsConversation;
				}
				catch (Exception ex)
				{
					this.ShowBalloon("Exception getting conversation list: " + ex.Message, 5);
				}
				this.m_bRefreshMessageFormConversationList = true;
			}
		}

		private void LoadServerSettings()
		{
			this.GetServerSettings(false);
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
						AccountItem accountItem = this.m_lsAccountItems.Find((AccountItem var) => var.number == this.m_strUserName);
						if (accountItem != null)
						{
							this.m_lsAccountItems.Remove(accountItem);
							accountItem.connectionStatus = "Failed Authentication";
							this.m_lsAccountItems.Add(accountItem);
						}
						this.m_bRefreshDashboardForm = true;
					}
				}
				else
				{
					string empty = string.Empty;
					AppRegistry.SyncServer(ref empty, this.m_strUserName);
					this.InitializeAccountRegistryConfiguration(bPushToServer);
					if (!string.IsNullOrEmpty(empty))
					{
						if (this.m_bDashboardMode)
						{
							if (!this.m_bDisableDashboardSettingChangeNotifications)
							{
								MessageBox.Show("The following Features and Settings have been updated from the server:\r\n" + empty, this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							}
						}
						else
						{
							MessageBox.Show("The following Features and Settings have been updated from the server:\r\n" + empty, this.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

		public bool ProcessMessageForKeyword(TextMessage message)
		{
			bool result = false;
			if (!this.m_bEnableKeywordProcessing || !this.m_bContactsInitiated)
			{
				return result;
			}
			if (message.isRead)
			{
				return result;
			}
			string text = string.Empty;
			string text2 = message.body.ToLower().Trim();
			string text3 = string.Empty;
			Contact contact = new Contact();
			List<string> list = new List<string>();
			try
			{
				text3 = "#" + text2;
				contact = this.GetContactByID(message.contactId);
				if (contact == null)
				{
					this.LoadContacts(false);
					contact = this.GetContactByID(message.contactId);
				}
				if (contact != null)
				{
					list = this.GetContactGroupTags(contact.notes);
				}
			}
			catch (Exception ex)
			{
				this.strError = ex.Message;
			}
			if (this.m_lsGroupTags.Contains(text3) && contact != null)
			{
				if (list.Contains(text3))
				{
					text = string.Concat(new string[]
					{
						"Auto response:\r\n\r\nYou are already a member of the ",
						text2.ToUpper(),
						" group\r\n\r\nYou may leave the group at any time by replying ",
						this.m_strRemoveGroupKeyword.ToUpper(),
						" ",
						text2.ToUpper()
					});
				}
				else if (!list.Contains(text3))
				{
					this.strError = this.AddGroupTag(contact.id, text3);
					if (this.strError.Length == 0)
					{
						text = string.Concat(new string[]
						{
							"Auto response:\r\n\r\nYou have been added to the ",
							text2.ToUpper(),
							" group\r\n\r\nYou may leave the group at any time by replying ",
							this.m_strRemoveGroupKeyword.ToUpper(),
							" ",
							text2.ToUpper()
						});
					}
				}
			}
			if (text2.Contains(this.m_strRemoveGroupKeyword) && contact != null)
			{
				text2 = text2.Replace(this.m_strRemoveGroupKeyword, "").Trim();
				if (text2.Length == 0)
				{
					if (list.Count > 0)
					{
						try
						{
							contact.notes = "";
							if (!this.m_textService.SaveContact(contact.address, this.m_strSession, contact.firstName, contact.lastName, "").Data.success)
							{
								this.strError = "Error calling contact/save...";
							}
							else
							{
								this.m_lsGroupTagContacts = (from val in this.m_lsGroupTagContacts
								where val.contactId != contact.id
								select val).ToList<GroupTagContact>();
								text = "Auto response:\r\n\r\nYou have been removed from ALL groups associated to this number";
							}
							goto IL_2EC;
						}
						catch (Exception ex2)
						{
							this.strError = "Error removing all gorup tag: " + ex2.Message;
							goto IL_2EC;
						}
					}
					text = "Auto response:\r\n\r\nYou are not currently a member of any groups associated to this number";
				}
				else
				{
					text3 = "#" + text2;
					if (list.Contains(text3))
					{
						this.strError = this.RemoveGroupTag(contact.id, text3);
						if (this.strError.Length == 0)
						{
							text = "Auto response:\r\n\r\nYou have been removed from group " + text2.ToUpper();
						}
					}
				}
			}
			IL_2EC:
			if (this.strError.Length > 0)
			{
				this.ShowBalloon("Process Message Keyword: " + this.strError, 5);
				this.strError = string.Empty;
			}
			else if (text.Length > 0)
			{
				result = true;
				if (this.m_bNotifyKeywordProcessing)
				{
					this.ShowBalloon("Processing auto response for " + this.FormatPhone(contact.mobileNumber) + "\r\n\r\n" + text, 5);
				}
				string formatedContacts = this.FormatContactAddress(contact.mobileNumber, false, false);
				if (!this.m_textService.SendMessage(text, formatedContacts, this.m_strSession, DateTime.Now).Data.success)
				{
					this.strError = "Error calling sendMessage for auto response on: " + contact.mobileNumber + " - " + text;
				}
				else
				{
					this.m_textService.MarkMessageRead(message.id, this.m_strSession);
				}
			}
			return result;
		}

		private void ProcessGroupSchedule()
		{
			if (!this.m_bEnableGroupScheduleProcessing || !this.m_bContactsInitiated)
			{
				return;
			}
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
							ScheduleFileItem scheduleFileItem = this.GroupScheduleFileValidateItem(sendItem);
							sendItem.SendStatus = scheduleFileItem.SendStatus;
							if (sendItem.SendStatus.Length == 0)
							{
								string text = string.Empty;
								DateTime t;
								DateTime.TryParse(sendItem.SendDate, out t);
								if (t < DateTime.Now)
								{
									if (t < DateTime.Now.AddHours((double)this.m_nGroupScheduleBackHourLimit))
									{
										text = string.Concat(new object[]
										{
											"Not Sent: Send date is over ",
											this.m_nGroupScheduleBackHourLimit,
											" hours in the past from the time it was processed ",
											DateTime.Now.ToString(),
											"."
										});
									}
									else
									{
										this.LoadContacts(false);
										IRestResponse<TextMessageSendResponse> restResponse;
										if (sendItem.Address.Substring(0, 1) == "#")
										{
											using (List<GroupTagContact>.Enumerator enumerator2 = (from val in this.m_lsGroupTagContacts
											where val.groupTag == sendItem.Address
											select val).ToList<GroupTagContact>().GetEnumerator())
											{
												while (enumerator2.MoveNext())
												{
													GroupTagContact current = enumerator2.Current;
													restResponse = this.m_textService.SendMessage(sendItem.TextMessage, current.address, this.m_strSession, DateTime.Now);
													if (!restResponse.Data.success)
													{
														text = (this.strError = string.Concat(new string[]
														{
															text,
															"Error: ",
															DateTime.Now.ToString(),
															" - ",
															restResponse.ErrorMessage
														}));
													}
													else
													{
														if (text.Length > 0)
														{
															text += "; ";
														}
														text = string.Concat(new string[]
														{
															text,
															"Sent: ",
															current.contact,
															" at ",
															DateTime.Now.ToString()
														});
														flag = true;
													}
												}
												goto IL_315;
											}
											goto IL_25E;
										}
										goto IL_25E;
										IL_315:
										if (text.Length == 0)
										{
											text = "Sent: " + DateTime.Now.ToString();
											goto IL_338;
										}
										goto IL_338;
										IL_25E:
										string text2 = this.FormatContactAddress(sendItem.Address, false, false);
										restResponse = this.m_textService.SendMessage(sendItem.TextMessage, text2, this.m_strSession, DateTime.Now);
										if (!restResponse.Data.success)
										{
											text = (this.strError = "Error: " + DateTime.Now.ToString() + " - " + restResponse.ErrorMessage);
											goto IL_315;
										}
										text = string.Concat(new string[]
										{
											text,
											"Sent: ",
											this.FormatPhone(text2),
											" at ",
											DateTime.Now.ToString()
										});
										flag = true;
										goto IL_315;
									}
								}
								IL_338:
								sendItem.SendStatus = text;
							}
						}
					}
					this.GroupSchedulFileSave(this.m_lsGroupSchedule);
					if (flag)
					{
						this.LoadConversations(false);
					}
				}
				catch (Exception ex)
				{
					this.strError = ex.Message;
				}
				if (this.strError.Length > 0)
				{
					this.ShowBalloon("Process Schedule File Error: " + this.strError, 5);
					this.strError = string.Empty;
				}
			}
			this.m_bGroupScheduleProcessing = false;
		}

		private void ProcessGroupMessage()
		{
			if (this.m_bConnected && !this.m_bGroupSend)
			{
				this.strError = string.Empty;
				int count = this.gs_GroupSend.Count;
				int num = 1;
				this.m_bGroupSend = true;
				try
				{
					foreach (string current in this.gs_GroupSend)
					{
						this.m_strCurentProcessingMessage = "Sending " + num.ToString() + " of " + count.ToString();
						this.m_bRefreshMessageFormProcessingLabel = true;
						if (!string.IsNullOrEmpty(this.gs_FilePath))
						{
							IRestResponse<MMSSendResponse> restResponse = this.m_textService.SendMessageMMS(this.gs_TextMessage, this.FormatContactAddress(current, true, true), this.m_strSession, this.gs_FilePath);
							if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
							{
								this.strError = "Error calling MMS messaging/send for: " + current + ": " + restResponse.ErrorMessage;
							}
							else if (!restResponse.Data.success)
							{
								this.strError = this.strError + "Error from MMS message/send for: " + current;
							}
						}
						else
						{
							IRestResponse<TextMessageSendResponse> restResponse2 = this.m_textService.SendMessage(this.gs_TextMessage, current, this.m_strSession, this.gs_ScheduleDate);
							if (!string.IsNullOrEmpty(restResponse2.ErrorMessage))
							{
								this.strError = string.Concat(new string[]
								{
									this.strError,
									"Error calling message/send for: ",
									current,
									": ",
									restResponse2.ErrorMessage
								});
							}
							else if (!restResponse2.Data.success)
							{
								this.strError = this.strError + "Error from message/send for: " + current;
							}
						}
						num++;
						Thread.Sleep(1000 * this.m_nGroupSendPauseIntervalSeconds);
					}
				}
				catch (Exception ex)
				{
					this.strError = ex.Message;
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

		public ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(displayText);
			if (eventHandler != null)
			{
				toolStripMenuItem.Click += eventHandler;
			}
			return toolStripMenuItem;
		}

		public void UpdateIcon()
		{
			if (this.m_bConnected)
			{
				this.notifyIcon.Text = this.m_strApplicationName + " - Connected to " + this.FormatPhone(this.m_strUserName);
				this.notifyIcon.Icon = new Icon(this.m_strConnectedIconPath);
				return;
			}
			this.notifyIcon.Text = this.m_strApplicationName + " - Not Connected";
			this.notifyIcon.Icon = new Icon(this.m_strNotConnectedIconPath);
		}

		public void LaunchWebsite(string strURL)
		{
			Process.Start(strURL);
		}

		public void BalloonClick()
		{
			typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.notifyIcon, null);
		}

		public void ShowBalloon(string text, int timeout)
		{
			timeout *= 1000;
			this.notifyIcon.BalloonTipTitle = this.m_strApplicationName;
			this.notifyIcon.BalloonTipText = text;
			if (!this.BalloonWaiting)
			{
				this.BalloonWaiting = true;
				this.notifyIcon.ShowBalloonTip(timeout);
				return;
			}
			this.notifyIcon.Visible = false;
			this.notifyIcon.Visible = true;
			this.notifyIcon.ShowBalloonTip(100);
		}

		public void BalloonShown()
		{
			this.BalloonWaiting = false;
		}

		public void LogOut(bool bNotify = true)
		{
			string empty = string.Empty;
			this.strError = string.Empty;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref empty);
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
					List<AccountItem> arg_11B_0 = this.m_lsAccountItems;
					Predicate<AccountItem> arg_11B_1;
					if ((arg_11B_1 = ApplicationManager.<>c.<>9__176_0) == null)
					{
						arg_11B_1 = (ApplicationManager.<>c.<>9__176_0 = new Predicate<AccountItem>(ApplicationManager.<>c.<>9.<LogOut>b__176_0));
					}
					AccountItem accountItem = arg_11B_0.Find(arg_11B_1);
					if (accountItem != null)
					{
						this.m_lsAccountItems.Remove(accountItem);
						accountItem.connectionStatus = "Connected";
						accountItem.lastSyncDate = null;
						accountItem.unReadMessageList = new List<TextMessage>();
						this.m_lsAccountItems.Add(accountItem);
					}
					this.m_bRefreshDashboardForm = true;
					this.m_strAccounTitle = string.Empty;
				}
				AppRegistry.SaveValue(rootKey, "local_IsLoggedOut", true, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nIsLoggedOut Error: " + empty;
				}
				if (!this.m_bSaveLogIn)
				{
					AppRegistry.SaveUserName(rootKey, "", ref empty);
					if (empty != string.Empty)
					{
						this.strError = this.strError + "\nUserName Save Error: " + empty;
					}
					AppRegistry.SavePassword(rootKey, "", ref empty);
					if (empty != string.Empty)
					{
						this.strError = this.strError + "\nPassword Save Error: " + empty;
					}
				}
				this.m_strSession = string.Empty;
				this.m_strUserName = string.Empty;
				this.m_strPassword = string.Empty;
				this.m_bIsLoggedOut = true;
			}
			catch (Exception ex)
			{
				this.ShowBalloon("Exception during logout: " + ex.Message + "\nError details: " + this.strError, 5);
			}
			if (this.strError == string.Empty && !bIsLoggedOut)
			{
				this.m_bConnected = false;
				if (bNotify)
				{
					this.ShowBalloon("You have successfully logged out of account " + this.FormatPhone(strUserName), 5);
				}
			}
			this.UpdateIcon();
		}

		public void LogIn(bool bNotify = true, string strUserName = null, string strPassword = null, string strCountryCode = null, string strAccountTitle = null)
		{
			bool flag = false;
			bool flag2 = false;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
			if (strUserName == null && strPassword == null && strCountryCode == null)
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
			flag = (this.m_strUserName != string.Empty && this.m_strPassword != string.Empty);
			if (flag && !this.m_bConnected && !this.m_bIsLoggedOut)
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
						AccountItem accountItem = this.m_lsAccountItems.Find((AccountItem var) => var.number == this.m_strUserName);
						if (accountItem != null)
						{
							this.m_lsAccountItems.Remove(accountItem);
							accountItem.connectionStatus = "Failed Authentication";
							this.m_lsAccountItems.Add(accountItem);
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
				catch (Exception ex)
				{
					this.m_bConnected = false;
					flag2 = true;
					MessageBox.Show("Exception connecting to Text Service\n\nPlease check your internet connection...\n\nError details: " + ex.Message, "Log In Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
					AccountItem accountItem2 = this.m_lsAccountItems.Find((AccountItem var) => var.number == this.m_strUserName);
					if (accountItem2 != null)
					{
						this.m_lsAccountItems.Remove(accountItem2);
						accountItem2.connectionStatus = "Logged In";
						this.m_lsAccountItems.Add(accountItem2);
					}
					this.m_bRefreshDashboardForm = true;
					return;
				}
			}
			else if ((!this.m_bConnected & flag) && !this.m_bIsLoggedOut && !flag2)
			{
				MessageBox.Show("Log in failure.\n\nPlease check your log in information and selected country...", "Log In Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				if (this.m_bDashboardMode)
				{
					this.m_lsAccountItems = (from val in this.m_lsAccountItems
					where val.number != this.m_strUserName
					select val).ToList<AccountItem>();
					AccountItem accountItem3 = new AccountItem();
					accountItem3.countryCode = this.m_strCountryCode;
					accountItem3.number = this.m_strUserName;
					accountItem3.password = this.m_strPassword;
					accountItem3.title = strAccountTitle;
					accountItem3.session = string.Empty;
					accountItem3.connectionStatus = "Failed Authentication";
					this.m_lsAccountItems.Add(accountItem3);
					return;
				}
				this.ShowUserLogIn();
				return;
			}
			else
			{
				if (!flag2 && !this.m_bDashboardMode)
				{
					this.ShowUserLogIn();
					return;
				}
				if (this.m_bDashboardMode)
				{
					this.ShowAccountDashboard();
					return;
				}
				this.ShowUserLogIn();
			}
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

		public void SetAccountFeatures()
		{
		}

		public string FormatPhone(string phone)
		{
			phone = phone.Replace("+", "");
			if (this.IsDigitsOnly(phone))
			{
				if (phone.Length == 10)
				{
					phone = string.Format("{0:(###) ###-####}", double.Parse(phone.Substring(0, 10)));
				}
				else if (phone.Length == 11 && phone.Substring(0, 1) == "1")
				{
					phone = string.Format("{0:# (###) ###-####}", double.Parse(phone.Substring(0, 11)));
				}
			}
			else if (phone.Contains("device:/"))
			{
				phone = "GROUP";
			}
			else
			{
				int i = (phone.Length - phone.Replace("ptn:/", "").Length) / "ptn:/".Length;
				string text = phone;
				phone = string.Empty;
				while (i > 0)
				{
					int startIndex = text.IndexOf("ptn:/") + 5;
					string text2 = string.Empty;
					try
					{
						if (text.Substring(startIndex, 1) == "1")
						{
							text2 = string.Format("{0:# (###) ###-####}", double.Parse(text.Substring(startIndex, 11)));
						}
						else
						{
							text2 = string.Format("{0:(###) ###-####}", double.Parse(text.Substring(startIndex, 10)));
						}
					}
					catch
					{
						text2 = text.Replace(",ptn:/", "").Replace("ptn:/", "");
					}
					if (string.IsNullOrEmpty(phone))
					{
						phone = text2;
					}
					else
					{
						phone = phone + " " + text2;
					}
					text = text.Replace(this.FormatContactAddress(text2, false, false), "");
					i--;
				}
			}
			return phone.Trim();
		}

		public string FormatGroupTag(string grouptag)
		{
			string source = "#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			string arg_0B_0 = string.Empty;
			string text = grouptag;
			for (int i = 0; i < text.Length; i++)
			{
				char value = text[i];
				if (!source.Contains(value))
				{
					grouptag = grouptag.Replace(value.ToString(), "");
				}
			}
			grouptag = grouptag.ToLower();
			return grouptag;
		}

		public bool IsDigitsOnly(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c < '0' || c > '9')
				{
					return false;
				}
			}
			return true;
		}

		public bool IsValidMobileNumber(string text)
		{
			bool result = true;
			if (this.m_bValidateMobileNumbers)
			{
				text = text.Replace("ptn:/", "");
				text = text.Replace("+", "");
				if (text.Length != 10)
				{
					result = false;
				}
				if (text.Length == 11 && text.Substring(0, 1) == "1")
				{
					result = true;
				}
			}
			return result;
		}

		public string FormatContactAddress(string address, bool onlyDigits = false, bool removeLeadingOne = false)
		{
			if (address.Length > 0)
			{
				string text = address;
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (c < '0' || c > '9')
					{
						address = address.Replace(c.ToString(), "");
					}
				}
				if (removeLeadingOne && address.Substring(0, 1) == "1")
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

		public string FormatMenuItem(string item)
		{
			item = item.Replace("\r\n", "");
			item = item.Replace("\n", "");
			item = item.Replace("\r", "");
			item = item.Replace("\t", "");
			int num = item.Length;
			if (num > 50)
			{
				num = 50;
			}
			item = item.Substring(0, num) + "...";
			return item;
		}

		public string FormatFileName(string fileName)
		{
			string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
			string text = fileName;
			for (int i = 0; i < text.Length; i++)
			{
				char value = text[i];
				if (!source.Contains(value))
				{
					fileName = fileName.Replace(value.ToString(), "");
				}
			}
			return fileName;
		}

		public string FormatAlphaNumeric(string item)
		{
			string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string text = item;
			for (int i = 0; i < text.Length; i++)
			{
				char value = text[i];
				if (!source.Contains(value))
				{
					item = item.Replace(value.ToString(), "");
				}
			}
			return item;
		}

		public string RemoveGroupTag(long ContactID, string GroupTag)
		{
			string result = string.Empty;
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
				if (!this.m_textService.SaveContact(contact.address, this.m_strSession, contact.firstName, contact.lastName, notes).Data.success)
				{
					result = "Error calling contact/save...";
				}
				else
				{
					if (GroupTag.ToLower() == "all")
					{
						this.m_lsGroupTagContacts.RemoveAll((GroupTagContact var) => var.contactId == contact.id);
					}
					else
					{
						GroupTagContact groupTagContact = this.m_lsGroupTagContacts.Find((GroupTagContact var) => var.contactId == contact.id && var.groupTag == GroupTag);
						if (groupTagContact != null)
						{
							this.m_lsGroupTagContacts.Remove(groupTagContact);
						}
					}
					this.m_lsContact.Find((Contact var) => var.id == contact.id).notes = notes;
				}
			}
			catch (Exception ex)
			{
				result = "Error removing group tag from contact " + contact.address + ": " + ex.Message;
			}
			return result;
		}

		public string AddGroupTag(long ContactID, string GroupTag)
		{
			ApplicationManager.<>c__DisplayClass189_0 <>c__DisplayClass189_ = new ApplicationManager.<>c__DisplayClass189_0();
			string result = string.Empty;
			<>c__DisplayClass189_.contact = new Contact();
			try
			{
				<>c__DisplayClass189_.contact = this.GetContactByID(ContactID);
				List<string> contactGroupTags2 = this.GetContactGroupTags(<>c__DisplayClass189_.contact.notes);
				if (!contactGroupTags2.Contains(GroupTag))
				{
					contactGroupTags2.Add(GroupTag);
					string notes = string.Join("", contactGroupTags2.ToArray());
					if (!this.m_textService.SaveContact(<>c__DisplayClass189_.contact.address, this.m_strSession, <>c__DisplayClass189_.contact.firstName, <>c__DisplayClass189_.contact.lastName, notes).Data.success)
					{
						result = "Error calling contact/save...";
					}
					else
					{
						string text = <>c__DisplayClass189_.contact.firstName + " " + <>c__DisplayClass189_.contact.lastName;
						if (text.Trim().Length == 0)
						{
							text = "Unnamed";
						}
						else
						{
							text = text.Trim();
						}
						GroupTagContact contactGroupTags = new GroupTagContact();
						contactGroupTags.groupTag = GroupTag;
						contactGroupTags.contactId = <>c__DisplayClass189_.contact.id;
						contactGroupTags.contact = text + " " + this.FormatPhone(<>c__DisplayClass189_.contact.mobileNumber);
						contactGroupTags.address = <>c__DisplayClass189_.contact.address;
						GroupTagContact groupTagContact = this.m_lsGroupTagContacts.Find((GroupTagContact var) => var.contactId == <>c__DisplayClass189_.contact.id && var.groupTag == contactGroupTags.groupTag);
						if (groupTagContact != null)
						{
							this.m_lsGroupTagContacts.Remove(groupTagContact);
						}
						this.m_lsGroupTagContacts.Add(contactGroupTags);
						this.m_lsContact.Find(new Predicate<Contact>(<>c__DisplayClass189_.<AddGroupTag>b__1)).notes = notes;
						if (!this.m_lsGroupTags.Contains(GroupTag))
						{
							this.m_lsGroupTags.Add(GroupTag);
						}
					}
				}
			}
			catch (Exception ex)
			{
				result = "Error adding group tag to contact " + <>c__DisplayClass189_.contact.address + ": " + ex.Message;
			}
			return result;
		}

		public List<string> GetContactGroupTags(string contactNote)
		{
			List<string> list = new List<string>();
			contactNote = this.FormatGroupTag(contactNote);
			while (contactNote.Contains("#"))
			{
				int num = contactNote.IndexOf("#");
				int num2 = contactNote.IndexOf("#", 1);
				string item = string.Empty;
				int num3;
				if (num2 > 0)
				{
					num3 = num2;
				}
				else
				{
					num3 = contactNote.Length;
				}
				if (num >= 0 && num2 != num)
				{
					item = this.FormatGroupTag(contactNote.Substring(num, num3));
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
				contactNote = contactNote.Substring(num3);
			}
			return list;
		}

		public Contact GetContactByID(long ContactID)
		{
			Contact result = null;
			if (this.m_lsContact != null || ContactID != 0L)
			{
				result = this.m_lsContact.Find((Contact c) => c.id == ContactID);
			}
			return result;
		}

		public AccountItem GetAccountItem(string phone)
		{
			AccountItem result = new AccountItem();
			if (this.m_lsAccountItems != null || phone.Length != 0)
			{
				foreach (AccountItem current in this.m_lsAccountItems)
				{
					if (current.number == phone)
					{
						result = current;
						break;
					}
				}
			}
			return result;
		}

		public string GetAccountItemOldestUnreadDate(AccountItem accoutItem)
		{
			DateTime? dateTime = null;
			using (List<TextMessage>.Enumerator enumerator = accoutItem.unReadMessageList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DateTime dateTime2;
					DateTime.TryParse(enumerator.Current.dateCreated, out dateTime2);
					if (!dateTime.HasValue)
					{
						dateTime = new DateTime?(dateTime2);
					}
					else if (dateTime2 < dateTime)
					{
						dateTime = new DateTime?(dateTime2);
					}
				}
			}
			if (dateTime.HasValue)
			{
				return dateTime.ToString();
			}
			return "";
		}

		public int GetTotalDashboardUnread(bool excludeLoggedIn = false)
		{
			int num = 0;
			if (this.m_lsAccountItems != null)
			{
				foreach (AccountItem current in this.m_lsAccountItems)
				{
					if (!excludeLoggedIn || !(current.connectionStatus == "Logged In"))
					{
						int? num2 = new int?(current.unReadMessageList.Count<TextMessage>());
						if (num2 > 0)
						{
							num += Convert.ToInt32(num2);
						}
					}
				}
			}
			return num;
		}

		public void ShowUserLogIn()
		{
			if (this.formUserLogin == null)
			{
				this.formUserLogin = new fmUserLogin
				{
					appManager = this
				};
				this.formUserLogin.Closed += new EventHandler(this.formUserLogIn_Closed);
				this.formUserLogin.Show();
				return;
			}
			this.formUserLogin.Activate();
			this.formUserLogin.WindowState = FormWindowState.Normal;
			this.formUserLogin.BringToFront();
		}

		private void formUserLogIn_Closed(object sender, EventArgs e)
		{
			this.formUserLogin = null;
		}

		public void ShowMessages()
		{
			if (this.formMessages == null)
			{
				this.formMessages = new fmMessages
				{
					appManager = this
				};
				this.formMessages.Closed += new EventHandler(this.formMessages_Closed);
				this.formMessages.Show();
				return;
			}
			this.formMessages.Activate();
			this.formMessages.WindowState = FormWindowState.Normal;
			this.formMessages.BringToFront();
			this.formMessages.DisplayConversatoinList();
			this.formMessages.LoadMessageTemplateMenu();
		}

		private void formMessages_Closed(object sender, EventArgs e)
		{
			this.formMessages = null;
		}

		public void ShowNewMessage()
		{
			if (this.formNewMessage == null)
			{
				this.formNewMessage = new fmNewMessage
				{
					appManager = this
				};
				this.formNewMessage.Closed += new EventHandler(this.formNewMessage_Closed);
				this.formNewMessage.Show();
				return;
			}
			this.formNewMessage.Activate();
			this.formNewMessage.WindowState = FormWindowState.Normal;
			this.formNewMessage.BringToFront();
			this.formNewMessage.LoadMessageTemplateMenu();
		}

		private void formNewMessage_Closed(object sender, EventArgs e)
		{
			this.formNewMessage = null;
		}

		public void ShowEditContact(bool newContact)
		{
			if (this.formEditContact == null)
			{
				this.formEditContact = new fmEditContacts
				{
					appManager = this,
					bNewContact = newContact
				};
				this.formEditContact.Closed += new EventHandler(this.formEditContact_Closed);
				this.formEditContact.Show();
				return;
			}
			this.formEditContact.Activate();
			this.formEditContact.WindowState = FormWindowState.Normal;
			this.formEditContact.BringToFront();
			this.formEditContact.DisplayContact();
		}

		private void formEditContact_Closed(object sender, EventArgs e)
		{
			this.formEditContact = null;
		}

		public void ShowSettings()
		{
			if (this.formSettings == null)
			{
				this.formSettings = new fmSettings
				{
					appManager = this
				};
				this.formSettings.Closed += new EventHandler(this.formSettings_Closed);
				this.formSettings.Show();
				return;
			}
			this.formSettings.Activate();
			this.formSettings.WindowState = FormWindowState.Normal;
			this.formSettings.BringToFront();
		}

		private void formSettings_Closed(object sender, EventArgs e)
		{
			this.formSettings = null;
		}

		public void ShowMessageTemplate()
		{
			if (this.formMessageTemplate == null)
			{
				this.formMessageTemplate = new fmMessageTemplate
				{
					appManager = this
				};
				this.formMessageTemplate.Closed += new EventHandler(this.formMessageTemplate_Closed);
				this.formMessageTemplate.Show();
				return;
			}
			this.formMessageTemplate.Activate();
			this.formMessageTemplate.WindowState = FormWindowState.Normal;
			this.formMessageTemplate.BringToFront();
		}

		private void formMessageTemplate_Closed(object sender, EventArgs e)
		{
			this.formMessageTemplate = null;
		}

		public void ShowEditGroups()
		{
			if (this.formEditGroups == null)
			{
				this.formEditGroups = new fmEditGroups
				{
					appManager = this
				};
				this.formEditGroups.Closed += new EventHandler(this.formEditGroups_Closed);
				this.formEditGroups.Show();
				return;
			}
			this.formEditGroups.Activate();
			this.formEditGroups.WindowState = FormWindowState.Normal;
			this.formEditGroups.BringToFront();
			this.formEditGroups.comboBoxGroups_Load(string.Empty);
			this.formEditGroups.comboBoxContactList_Load(string.Empty);
		}

		private void formEditGroups_Closed(object sender, EventArgs e)
		{
			this.formEditGroups = null;
		}

		public void ShowPrintConversation()
		{
			if (this.formPrintConversation == null)
			{
				this.formPrintConversation = new fmPrintConversation
				{
					appManager = this
				};
				this.formPrintConversation.Closed += new EventHandler(this.formPrintConversation_Closed);
				this.formPrintConversation.Show();
				return;
			}
			this.formPrintConversation.Activate();
			this.formPrintConversation.WindowState = FormWindowState.Normal;
			this.formPrintConversation.BringToFront();
		}

		private void formPrintConversation_Closed(object sender, EventArgs e)
		{
			this.formPrintConversation = null;
		}

		public void ShowAccountDashboard()
		{
			if (this.formAccountDashboard == null)
			{
				this.formAccountDashboard = new fmAccountDashboard
				{
					appManager = this
				};
				this.formAccountDashboard.Closed += new EventHandler(this.formAccountDashboard_Closed);
				this.formAccountDashboard.Show();
				return;
			}
			this.formAccountDashboard.Activate();
			this.formAccountDashboard.WindowState = FormWindowState.Normal;
			this.formAccountDashboard.BringToFront();
		}

		private void formAccountDashboard_Closed(object sender, EventArgs e)
		{
			this.formAccountDashboard = null;
		}

		public void ShowEditAccounts(bool newAccount)
		{
			if (this.formEditAccounts == null)
			{
				this.formEditAccounts = new fmEditAccounts
				{
					appManager = this,
					bNewAccount = newAccount
				};
				this.formEditAccounts.Closed += new EventHandler(this.formEditAccounts_Closed);
				this.formEditAccounts.Show();
				return;
			}
			this.formEditAccounts.Activate();
			this.formEditAccounts.WindowState = FormWindowState.Normal;
			this.formEditAccounts.BringToFront();
			this.formEditAccounts.comboBoxAccountList_Load(string.Empty);
		}

		private void formEditAccounts_Closed(object sender, EventArgs e)
		{
			this.formEditAccounts = null;
		}

		public void ShowKeywordAutoResponse()
		{
			if (this.formKeywordAutoResponse == null)
			{
				this.formKeywordAutoResponse = new fmKeywordAutoResponse
				{
					appManager = this
				};
				this.formKeywordAutoResponse.Closed += new EventHandler(this.formKeywordAutoResponse_Closed);
				this.formKeywordAutoResponse.Show();
				return;
			}
			this.formKeywordAutoResponse.Activate();
			this.formKeywordAutoResponse.WindowState = FormWindowState.Normal;
			this.formKeywordAutoResponse.BringToFront();
		}

		private void formKeywordAutoResponse_Closed(object sender, EventArgs e)
		{
			this.formKeywordAutoResponse = null;
		}

		public void ShowGroupSchedule()
		{
			if (this.formGroupSchedule == null)
			{
				this.formGroupSchedule = new fmGroupSchedule
				{
					appManager = this
				};
				this.formGroupSchedule.Closed += new EventHandler(this.formGroupSchedule_Closed);
				this.formGroupSchedule.Show();
				return;
			}
			this.formGroupSchedule.Activate();
			this.formGroupSchedule.WindowState = FormWindowState.Normal;
			this.formGroupSchedule.BringToFront();
			this.formGroupSchedule.dataGridViewGroupScheduleFile_Load();
		}

		private void formGroupSchedule_Closed(object sender, EventArgs e)
		{
			this.formGroupSchedule = null;
		}

		public void ShowEditGroupSchedule()
		{
			if (this.formEditGroupSchedule == null)
			{
				this.formEditGroupSchedule = new fmEditGroupSchedule
				{
					appManager = this
				};
				this.formEditGroupSchedule.Closed += new EventHandler(this.formEditGroupSchedule_Closed);
				this.formEditGroupSchedule.Show();
				return;
			}
			this.formEditGroupSchedule.Activate();
			this.formEditGroupSchedule.WindowState = FormWindowState.Normal;
			this.formEditGroupSchedule.BringToFront();
		}

		private void formEditGroupSchedule_Closed(object sender, EventArgs e)
		{
			this.formEditGroupSchedule = null;
		}
	}
}

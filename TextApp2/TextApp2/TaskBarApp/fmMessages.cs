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
using System.Windows.Forms;
using TaskBarApp.Objects;
using TaskBarApp.Properties;

namespace TaskBarApp
{
	public class fmMessages : Form
	{
		private struct ConversationItem
		{
			public string fingerprint
			{
				get;
				set;
			}

			public long contactID
			{
				get;
				set;
			}

			public string contactAddress
			{
				get;
				set;
			}

			public string contactName
			{
				get;
				set;
			}

			public string contactPhone
			{
				get;
				set;
			}

			public DateTime? dtLastDate
			{
				get;
				set;
			}

			public int unreadCount
			{
				get;
				set;
			}
		}

		[CompilerGenerated]
		[Serializable]
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

			internal string <DisplayConversatoinList>b__28_0(Conversation c)
			{
				return c.lastMessageDate;
			}

			internal string <DisplayConversation>b__29_0(TextMessage c)
			{
				return c.dateCreated;
			}

			internal long <DisplayConversation>b__29_1(TextMessage n)
			{
				return n.id;
			}

			internal string <PrintConversation>b__32_0(TextMessage c)
			{
				return c.dateCreated;
			}

			internal long <PrintConversation>b__32_1(TextMessage n)
			{
				return n.id;
			}

			internal string <ExportConversation>b__33_0(TextMessage c)
			{
				return c.dateCreated;
			}

			internal long <ExportConversation>b__33_1(TextMessage n)
			{
				return n.id;
			}
		}

		private string strError = string.Empty;

		private long nSelectedMessageID;

		private string strImageStorageKey = string.Empty;

		private RichTextBox textBoxSelectedMessage;

		private PictureBox pictureBoxSelectedMessage;

		private bool bConversationActionLoad;

		private bool bConversationDelete;

		private bool bConversationExport;

		private bool bStopConversationListSelectIndexChange;

		private bool bConversationLoading;

		private bool bConversationListLoading;

		private int nMessageLimit = 50;

		private List<TextMessage> lsMessagesWorking = new List<TextMessage>();

		private FormWindowState fwsLastWindowState = FormWindowState.Minimized;

		private Size sFormSize;

		private int nPadding = 6;

		private IContainer components;

		private SplitContainer splitContainerMessages;

		private Label labelUnreadCount;

		private Label labelConversationsCount;

		private ListBox listBoxConversationList;

		public Panel pnMessages;

		private TextBox textBoxSearch;

		private Button buttonClear;

		private MenuStrip menuStripMessage;

		private ToolStripMenuItem newMessageToolStripMenuItem;

		private ToolStripMenuItem editsToolStripMenuItem;

		private ToolStripMenuItem messageTemplatesToolStripMenuItem;

		private ToolStripMenuItem editMessageTemplatesToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private ComboBox comboBoxConversationAction;

		private ToolStripMenuItem manageGroupsToolStripMenuItem;

		private ToolStripMenuItem manageContactsToolStripMenuItem;

		private SaveFileDialog saveFileDialogPrintConversation;

		private ContextMenuStrip contextMenuStripMessageText;

		private ToolStripMenuItem copyTextToolStripMenuItem;

		private ToolStripMenuItem deleteToolStripMenuItem;

		private ToolStripMenuItem markAsReadToolStripMenuItem;

		private ToolStripMenuItem optionsToolStripMenuItem;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem generalHelpToolStripMenuItem;

		private ToolStripMenuItem settingsHelpToolStripMenuItem;

		private ToolStripMenuItem exitToolStripMenuItem;

		private ToolTip toolTipMarkAllRead;

		private ToolStripMenuItem settingsToolStripMenuItem;

		private ToolStripMenuItem controlEnterToolStripMenuItem;

		private ToolStripMenuItem requireClickToMarkMessageReadToolStripMenuItem;

		private ToolStripMenuItem openMessagesWindowWithUnreadReminderToolStripMenuItem;

		private ComboBox comboBoxFilter;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripSeparator toolStripSeparator3;

		private Label labelLoadingConversation;

		private TextBox textBoxContactPhone;

		private TextBox textBoxContactName;

		private Panel pnSend;

		private Button buttonMarkAllRead;

		private DateTimePicker dateTimePickerScheduleDate;

		private Button buttonRefresh;

		private Button buttonSend;

		private Label labelCharCount;

		private TextBox textBoxMessage;

		private LinkLabel linkLabelMoreMessages;

		private LinkLabel linkLabelMoreConversations;

		private Label labelUnread;

		private ToolStripMenuItem keepSelectedConversationInFocusToolStripMenuItem;

		private ToolStripMenuItem versionToolStripMenuItem;

		private Label labelProcessing;

		private ToolStripMenuItem editKeywordAutoResponseToolStripMenuItem;

		private ToolStripMenuItem editGroupScheduleToolStripMenuItem;

		private PictureBox pictureBoxAttachment;

		private PictureBox pictureBoxLink;

		private OpenFileDialog openFileDialog;

		private Label labelAttRemove;

		private ToolStripMenuItem displayMMSAttachmentsToolStripMenuItem;

		private ContextMenuStrip contextMenuStripPictureBox;

		private ToolStripMenuItem downloadToolStripMenuItem;

		private ToolStripMenuItem syncFeaturesToolStripMenuItem;

		private PictureBox pictureBoxConversationCountLock;

		private Label labelTextBoxSearchHint;

		private CheckBox checkBoxFullRefresh;

		private RapidSpellAsYouType rapidSpellAsYouTypeText;

		private LinkLabel linkLabelEditContact;

		private ToolStripMenuItem logOutToolStripMenuItem;

		private ToolStripMenuItem forwardTextMessage;

		private ToolStripMenuItem tryBETAToolStripMenuItem;

		private TableLayoutPanel tableLayoutPanelMessages;

		private Panel pnMoreMessages;

		private Label labelMesssageCount;

		private TableLayoutPanel tableLayoutPanelContactName;

		private TableLayoutPanel tableLayoutPanelContactPhone;

		private ToolStripMenuItem copySelectedTextToolStripMenuItem;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmMessages()
		{
			this.InitializeComponent();
		}

		private void fmMessage_Load(object sender, EventArgs e)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				RegistryKey expr_11 = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.GetValue(expr_11, "local_FormMessageWidth", ref num, ref this.strError);
				AppRegistry.GetValue(expr_11, "local_FormMessageHeight", ref num2, ref this.strError);
				AppRegistry.GetValue(expr_11, "local_splitContainerMessagesDistance", ref num3, ref this.strError);
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
				this.Text = string.Concat(new string[]
				{
					this.appManager.m_strApplicationName,
					" Messages ",
					this.appManager.FormatPhone(this.appManager.m_strUserName),
					" ",
					this.appManager.m_strAccounTitle
				});
				base.Icon = this.appManager.iTextApp;
				this.ResetMessageForm(null);
				this.sFormSize = base.Size;
			}
			catch (Exception ex)
			{
				this.strError = "Unexpected application error while loading message window: " + ex.Message;
			}
			if (this.strError.Length > 0)
			{
				this.appManager.ShowBalloon(this.strError, 5);
			}
		}

		private void fmMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_0B, "local_FormMessageWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_FormMessageHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_splitContainerMessagesDistance", this.splitContainerMessages.SplitterDistance, ref this.strError, false, RegistryValueKind.Unknown);
				this.appManager.m_strCurentConversationFingerprint = string.Empty;
				this.appManager.m_strCurrentContactAddress = string.Empty;
				this.appManager.m_nCurrentContactID = 0L;
			}
			catch
			{
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

		public void LoadMessageTemplateMenu()
		{
			try
			{
				string str = string.Empty;
				this.messageTemplatesToolStripMenuItem.DropDownItems.Clear();
				ToolStripItem value = new ToolStripMenuItem("&Edit Message Templates...", null, new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click));
				this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				this.messageTemplatesToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate1))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate1);
					value = new ToolStripMenuItem("&1. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "1");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate2))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate2);
					value = new ToolStripMenuItem("&2. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "2");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate3))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate3);
					value = new ToolStripMenuItem("&3. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "3");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate4))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate4);
					value = new ToolStripMenuItem("&4. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "4");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate5))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate5);
					value = new ToolStripMenuItem("&5. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "5");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate6))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate6);
					value = new ToolStripMenuItem("&6. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "6");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate7))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate7);
					value = new ToolStripMenuItem("&7. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "7");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate8))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate8);
					value = new ToolStripMenuItem("&8. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "8");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate9))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate9);
					value = new ToolStripMenuItem("&9. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "9");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
				if (!string.IsNullOrEmpty(this.appManager.m_strMessageTemplate10))
				{
					str = this.appManager.FormatMenuItem(this.appManager.m_strMessageTemplate10);
					value = new ToolStripMenuItem("&10. " + str, null, new EventHandler(this.messageTemplateMenuItem_Click), "10");
					this.messageTemplatesToolStripMenuItem.DropDownItems.Add(value);
				}
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception loading message template menue item list: " + ex.Message, 5);
			}
		}

		public void DisplayConversatoinList()
		{
			if (this.bConversationListLoading)
			{
				return;
			}
			this.bConversationListLoading = true;
			try
			{
				this.bStopConversationListSelectIndexChange = true;
				if (this.appManager.m_lsConversation != null)
				{
					ApplicationManager arg_61_0 = this.appManager;
					IEnumerable<Conversation> arg_57_0 = this.appManager.m_lsConversation;
					Func<Conversation, string> arg_57_1;
					if ((arg_57_1 = fmMessages.<>c.<>9__28_0) == null)
					{
						arg_57_1 = (fmMessages.<>c.<>9__28_0 = new Func<Conversation, string>(fmMessages.<>c.<>9.<DisplayConversatoinList>b__28_0));
					}
					arg_61_0.m_lsConversation = arg_57_0.OrderByDescending(arg_57_1).ToList<Conversation>();
					if (this.listBoxConversationList.Items.Count > 0)
					{
						this.listBoxConversationList.Items.Clear();
					}
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					bool flag = false;
					string text = this.textBoxSearch.Text.Trim();
					foreach (Conversation current in this.appManager.m_lsConversation)
					{
						string text2 = string.Empty;
						string text3 = string.Empty;
						if (current.lastContactId == 0L)
						{
							text3 = this.appManager.FormatPhone(current.address);
							if (text3.Length > 14)
							{
								text2 = "Multiple Recipients";
							}
							else
							{
								text2 = "Unknown";
							}
						}
						else
						{
							Contact contactByID = this.appManager.GetContactByID(current.lastContactId);
							if (contactByID != null)
							{
								text3 = this.appManager.FormatPhone(contactByID.mobileNumber);
								text2 = contactByID.firstName + " " + contactByID.lastName;
								if (text2 == " ")
								{
									text2 = "Unnamed";
								}
							}
							else
							{
								text2 = "Unknown";
								text3 = this.appManager.FormatPhone(current.address);
							}
						}
						int unreadCount = current.unreadCount;
						DateTime value;
						DateTime.TryParse(current.lastMessageDate, out value);
						fmMessages.ConversationItem conversationItem = new fmMessages.ConversationItem
						{
							fingerprint = current.fingerprint,
							contactID = current.lastContactId,
							contactAddress = current.address,
							contactName = text2,
							contactPhone = text3,
							dtLastDate = new DateTime?(value),
							unreadCount = unreadCount
						};
						if (string.IsNullOrEmpty(text))
						{
							this.listBoxConversationList.Items.Add(conversationItem);
							num++;
						}
						else
						{
							string text4 = this.appManager.FormatContactAddress(conversationItem.contactPhone, true, false);
							string text5 = this.appManager.FormatAlphaNumeric(text);
							if (conversationItem.contactName.ToLower().Contains(text.ToLower()))
							{
								this.listBoxConversationList.Items.Add(conversationItem);
								num++;
							}
							else if (conversationItem.contactPhone.ToLower().Contains(text.ToLower()))
							{
								this.listBoxConversationList.Items.Add(conversationItem);
								num++;
							}
							else if (text5 != "" && text4.Contains(text5))
							{
								this.listBoxConversationList.Items.Add(conversationItem);
								num++;
							}
						}
						if (this.comboBoxFilter.SelectedItem == "Only In")
						{
							flag = true;
							this.comboBoxFilter.BackColor = Color.Yellow;
							ConversationMetaData conversationMetaData = this.appManager.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == conversationItem.fingerprint);
							if (conversationMetaData != null)
							{
								if (conversationMetaData.lastMessageDirection != "In")
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
							this.comboBoxFilter.BackColor = Color.Yellow;
							ConversationMetaData conversationMetaData2 = this.appManager.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == conversationItem.fingerprint);
							if (conversationMetaData2 != null)
							{
								if (conversationMetaData2.lastMessageDirection == "In")
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
							this.comboBoxFilter.BackColor = Color.Yellow;
							ConversationMetaData conversationMetaData3 = this.appManager.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == conversationItem.fingerprint);
							if (conversationMetaData3 != null)
							{
								if (!conversationMetaData3.lastMessageIsError)
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
							this.comboBoxFilter.BackColor = Color.Yellow;
							if (conversationItem.unreadCount < 1)
							{
								this.listBoxConversationList.Items.Remove(conversationItem);
							}
						}
						else
						{
							this.comboBoxFilter.BackColor = default(Color);
						}
						if (conversationItem.fingerprint == this.appManager.m_strCurentConversationFingerprint)
						{
							this.listBoxConversationList.SelectedItem = conversationItem;
						}
						num3 += conversationItem.unreadCount;
					}
					if (this.appManager.m_lsContact != null && !string.IsNullOrEmpty(text) && !flag)
					{
						using (List<Contact>.Enumerator enumerator2 = this.appManager.m_lsContact.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Contact contact = enumerator2.Current;
								if (this.appManager.m_lsConversation.Find((Conversation var) => var.lastContactId == contact.id) == null)
								{
									string text6 = string.Empty;
									string contactPhone = string.Empty;
									contactPhone = this.appManager.FormatPhone(contact.mobileNumber);
									text6 = contact.firstName + " " + contact.lastName;
									if (text6 == " ")
									{
										text6 = "Unnamed";
									}
									fmMessages.ConversationItem conversationItem2 = new fmMessages.ConversationItem
									{
										fingerprint = "0",
										contactID = contact.id,
										contactAddress = contact.address,
										contactName = text6,
										contactPhone = contactPhone,
										dtLastDate = null,
										unreadCount = 0
									};
									string text7 = this.appManager.FormatContactAddress(conversationItem2.contactPhone, false, false);
									string text8 = this.appManager.FormatAlphaNumeric(text);
									if (conversationItem2.contactName.ToLower().Contains(text.ToLower()))
									{
										this.listBoxConversationList.Items.Add(conversationItem2);
										num2++;
									}
									else if (conversationItem2.contactPhone.ToLower().Contains(text.ToLower()))
									{
										this.listBoxConversationList.Items.Add(conversationItem2);
										num2++;
									}
									else if (text8 != "ptn:/" && text7.Contains(text8))
									{
										this.listBoxConversationList.Items.Add(conversationItem2);
										num2++;
									}
									if (num2 == this.appManager.m_nContactLimit)
									{
										break;
									}
									if (conversationItem2.contactID == this.appManager.m_nCurrentContactID)
									{
										this.listBoxConversationList.SelectedItem = conversationItem2;
									}
								}
							}
						}
					}
					if (this.appManager.m_bKeepConversationFocus && this.listBoxConversationList.SelectedIndices.Count != 0)
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
						this.linkLabelMoreConversations.Visible = (this.pictureBoxConversationCountLock.Visible = true);
					}
					else
					{
						this.linkLabelMoreConversations.Visible = (this.pictureBoxConversationCountLock.Visible = false);
					}
				}
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
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception displaying text message conversation list: " + ex.Message, 5);
			}
			this.bConversationListLoading = false;
		}

		public void DisplayConversation(string fingerprint, bool bForceMarkAsRead = false, bool bRefreshMessages = true)
		{
			if (this.bConversationLoading)
			{
				return;
			}
			this.bConversationLoading = true;
			this.pnMessages.Controls.Clear();
			bool flag = false;
			RichTextBox activeControl = null;
			PictureBox pictureBox = null;
			this.buttonMarkAllRead.BackColor = ColorTranslator.FromHtml("#93FF14");
			this.buttonMarkAllRead.Visible = false;
			this.labelMesssageCount.Text = "0 Messages displayed";
			try
			{
				Label label = new Label();
				label.ForeColor = Color.Black;
				label.Font = this.appManager.m_fontSize;
				label.Text = "Working...";
				label.AutoSize = false;
				label.Dock = DockStyle.Fill;
				label.TextAlign = ContentAlignment.MiddleCenter;
				this.pnMessages.Controls.Add(label);
				Application.DoEvents();
				if (fingerprint == null)
				{
					if (string.IsNullOrEmpty(this.appManager.m_strCurentConversationFingerprint))
					{
						this.bConversationLoading = false;
						return;
					}
					fingerprint = this.appManager.m_strCurentConversationFingerprint;
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
						IRestResponse<ConversationGet> conversation = this.appManager.m_textService.GetConversation(fingerprint, this.appManager.m_strSession, 0, this.nMessageLimit);
						if (conversation != null)
						{
							ConversationResponse response = conversation.Data.response;
							if (response.messages != null)
							{
								this.appManager.m_strCurentConversationFingerprint = response.conversation.fingerprint;
								this.appManager.m_strCurrentContactAddress = response.conversation.address;
								this.appManager.m_nCurrentContactID = response.conversation.lastContactId;
								this.appManager.m_lsMessages = response.messages;
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
						ApplicationManager arg_22F_0 = this.appManager;
						IEnumerable<TextMessage> arg_201_0 = this.appManager.m_lsMessages;
						Func<TextMessage, string> arg_201_1;
						if ((arg_201_1 = fmMessages.<>c.<>9__29_0) == null)
						{
							arg_201_1 = (fmMessages.<>c.<>9__29_0 = new Func<TextMessage, string>(fmMessages.<>c.<>9.<DisplayConversation>b__29_0));
						}
						IOrderedEnumerable<TextMessage> arg_225_0 = arg_201_0.OrderBy(arg_201_1);
						Func<TextMessage, long> arg_225_1;
						if ((arg_225_1 = fmMessages.<>c.<>9__29_1) == null)
						{
							arg_225_1 = (fmMessages.<>c.<>9__29_1 = new Func<TextMessage, long>(fmMessages.<>c.<>9.<DisplayConversation>b__29_1));
						}
						arg_22F_0.m_lsMessages = arg_225_0.ThenBy(arg_225_1).ToList<TextMessage>();
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
						int num = 10;
						using (List<TextMessage>.Enumerator enumerator = this.appManager.m_lsMessages.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TextMessage message = enumerator.Current;
								activeControl = null;
								pictureBox = null;
								string arg_2BF_0 = string.Empty;
								DateTime dateTime;
								DateTime.TryParse(message.dateCreated, out dateTime);
								DateTime t;
								DateTime.TryParse(message.scheduledDate, out t);
								if (!message.isRead && this.appManager.FormatContactAddress(message.destAddress, true, true) == this.appManager.FormatContactAddress(this.appManager.m_strUserName, true, true))
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
										TextMessage item = this.appManager.m_lsUnReadMessages.Find((TextMessage m) => m.id == message.id);
										this.appManager.m_lsUnReadMessages.Remove(item);
									}
								}
								Label label2 = new Label();
								label2.Font = new Font(this.appManager.m_fontSizeDT, FontStyle.Italic);
								label2.ForeColor = Color.DarkGray;
								label2.Height = this.appManager.m_fontSizeDT.Height;
								label2.Width = (int)this.appManager.m_fontSizeDT.Size * 8;
								label2.Text = dateTime.ToShortTimeString();
								Label label3 = new Label();
								label3.Font = new Font(this.appManager.m_fontSizeDT, FontStyle.Italic);
								label3.ForeColor = Color.DarkGray;
								label3.Height = this.appManager.m_fontSizeDT.Height;
								label3.Width = (int)this.appManager.m_fontSizeDT.Size * 8;
								label3.Text = dateTime.ToShortDateString();
								string text = message.body.Replace("\n", "\r\n");
								RichTextBox richTextBox = new RichTextBox();
								richTextBox.DetectUrls = true;
								richTextBox.LinkClicked += new LinkClickedEventHandler(this.richTextBox_LinkClick);
								richTextBox.ReadOnly = true;
								richTextBox.ContextMenuStrip = this.contextMenuStripMessageText;
								richTextBox.Name = message.id.ToString();
								richTextBox.ForeColor = Color.Black;
								richTextBox.BorderStyle = BorderStyle.None;
								richTextBox.Font = this.appManager.m_fontSize;
								richTextBox.Text = text.Trim();
								if (message.scheduledDate != null)
								{
									if (t > DateTime.Now)
									{
										RichTextBox richTextBox2 = richTextBox;
										richTextBox2.Text = string.Concat(new string[]
										{
											richTextBox2.Text,
											"\r\n\r\n-----------------------------------------\r\nScheduled: ",
											t.ToShortDateString(),
											" ",
											t.ToShortTimeString()
										});
									}
									else
									{
										RichTextBox richTextBox2 = richTextBox;
										richTextBox2.Text = string.Concat(new string[]
										{
											richTextBox2.Text,
											"\r\n\r\n-----------------------------------------\r\nSent: ",
											t.ToShortDateString(),
											" ",
											t.ToShortTimeString()
										});
									}
								}
								if (message.transmissionState.name == "ERROR")
								{
									RichTextBox expr_60C = richTextBox;
									expr_60C.Text += "\r\n\r\n-----------------------------------------\r\nDelivery Failure.";
								}
								int num2 = 0;
								int num3 = this.appManager.m_fontSize.Height;
								if (num3 < this.appManager.m_fontSizeDT.Height * 2 + this.nPadding)
								{
									num3 = this.appManager.m_fontSizeDT.Height * 2 + this.nPadding;
								}
								if (this.pnMessages.VerticalScroll.Enabled)
								{
									num2 = SystemInformation.VerticalScrollBarWidth + 40;
								}
								richTextBox.Width = this.pnMessages.ClientSize.Width - label2.Size.Width - num2 + 30;
								TextFormatFlags flags = TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak;
								richTextBox.Height = TextRenderer.MeasureText(richTextBox.Text, richTextBox.Font, new Size(richTextBox.Width, num3), flags).Height;
								if (richTextBox.Height < num3)
								{
									richTextBox.Height = num3;
								}
								if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
								{
									label2.TextAlign = ContentAlignment.TopRight;
									label3.TextAlign = ContentAlignment.TopRight;
									richTextBox.BackColor = Color.GhostWhite;
									if (message.scheduledDate != null && t > DateTime.Now)
									{
										richTextBox.BackColor = Color.LightYellow;
									}
									if (message.transmissionState.name == "ERROR")
									{
										richTextBox.BackColor = ColorTranslator.FromHtml("#fce3c2");
									}
									label2.Location = new Point(5, num + 1);
									label3.Location = new Point(5, num + this.appManager.m_fontSizeDT.Height + this.nPadding - 1);
									richTextBox.Location = new Point(label3.Size.Width + 10, num);
								}
								else
								{
									if (!message.isRead && !bForceMarkAsRead)
									{
										richTextBox.BackColor = ColorTranslator.FromHtml("#93FF14");
									}
									else
									{
										richTextBox.BackColor = ColorTranslator.FromHtml("#E8FFCC");
									}
									label2.Location = new Point(richTextBox.Width + 10, num + 1);
									label3.Location = new Point(richTextBox.Width + 10, num + this.appManager.m_fontSizeDT.Height + this.nPadding - 1);
									richTextBox.Location = new Point(10, num);
								}
								this.pnMessages.Controls.Add(richTextBox);
								this.pnMessages.Controls.Add(label2);
								if (Convert.ToDateTime(DateTime.Now.ToShortDateString()) != Convert.ToDateTime(dateTime.ToShortDateString()))
								{
									this.pnMessages.Controls.Add(label3);
								}
								num += richTextBox.Size.Height;
								activeControl = richTextBox;
								if (message.hasAttachment && this.appManager.m_bMMSFeature)
								{
									try
									{
										IRestResponse<TextMessageAttachmentList> attachmentList = this.appManager.m_textService.GetAttachmentList(this.appManager.m_strSession, message.id);
										new List<TextMessageAttachment>();
										if (attachmentList != null && attachmentList.Data.success)
										{
											foreach (TextMessageAttachment current in attachmentList.Data.response)
											{
												if (current.mimeType.Contains("image/"))
												{
													PictureBox pictureBox2 = new PictureBox();
													pictureBox2.ContextMenuStrip = this.contextMenuStripPictureBox;
													pictureBox2.BackColor = Color.Black;
													pictureBox2.Name = message.id + "~" + current.storageKey;
													pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
													if (this.appManager.FormatContactAddress(message.destAddress, true, true) != this.appManager.m_strUserName)
													{
														pictureBox2.Location = new Point(label3.Size.Width + 10, num);
													}
													else
													{
														pictureBox2.Location = new Point(5, num);
													}
													if (this.appManager.m_bDisplayMMSAttachments)
													{
														Image attachment = this.appManager.m_textService.GetAttachment(this.appManager.m_strSession, current.storageKey);
														if (attachment != null)
														{
															pictureBox2.Image = attachment;
															pictureBox2.Height = 300;
															pictureBox2.Width = 300;
															num += 300;
														}
														else
														{
															pictureBox2.Image = Resources.LoadFail;
															pictureBox2.BackColor = Color.Transparent;
															pictureBox2.Height = 50;
															pictureBox2.Width = 50;
															num += 50;
														}
													}
													else
													{
														pictureBox2.Image = Resources.Download;
														pictureBox2.BackColor = Color.Transparent;
														pictureBox2.Height = 50;
														pictureBox2.Width = 50;
														num += 50;
													}
													this.pnMessages.Controls.Add(pictureBox2);
													pictureBox = pictureBox2;
													num += 5;
												}
											}
										}
									}
									catch (Exception ex)
									{
										this.strError = "Error displaying MMS message attachment: " + ex.Message;
									}
								}
								num += 10;
							}
						}
						if (pictureBox != null)
						{
							this.pnMessages.ScrollControlIntoView(pictureBox);
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
			catch (Exception ex2)
			{
				this.strError = "Error getting text messages for conversation: " + ex2.Message;
			}
			this.comboBoxConversationAction.Visible = true;
			this.linkLabelEditContact.Visible = true;
			this.buttonSend.Visible = true;
			this.pictureBoxLink.Visible = true;
			this.textBoxMessage.Visible = true;
			this.dateTimePickerScheduleDate.Visible = true;
			this.labelCharCount.Visible = true;
			if (this.appManager.m_bEnableSignature && this.textBoxMessage.Text.Length == 0)
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

		public void DisplayProcessingMessage(string strProcessingMessage = null)
		{
			this.labelProcessing.BackColor = ColorTranslator.FromHtml("#93FF14");
			this.labelProcessing.Text = "Processing Enabled";
			this.labelProcessing.Visible = false;
			if (!string.IsNullOrEmpty(strProcessingMessage))
			{
				this.labelProcessing.Text = strProcessingMessage;
				this.labelProcessing.Visible = true;
				return;
			}
			if (this.appManager.m_bEnableKeywordProcessing)
			{
				this.labelProcessing.Visible = true;
			}
			if (this.appManager.m_bEnableGroupScheduleProcessing)
			{
				this.labelProcessing.Visible = true;
			}
		}

		public void DisplayConversationHeader()
		{
			this.textBoxContactPhone.Visible = true;
			this.textBoxContactName.Visible = true;
			this.textBoxContactName.BackColor = ColorTranslator.FromHtml("#E8FFCC");
			this.textBoxContactPhone.BackColor = ColorTranslator.FromHtml("#E8FFCC");
			this.tableLayoutPanelContactName.BackColor = ColorTranslator.FromHtml("#E8FFCC");
			this.tableLayoutPanelContactPhone.BackColor = ColorTranslator.FromHtml("#E8FFCC");
			if (this.appManager.m_nCurrentContactID != 0L)
			{
				Contact contactByID = this.appManager.GetContactByID(this.appManager.m_nCurrentContactID);
				if (contactByID == null)
				{
					this.textBoxContactPhone.Text = this.appManager.FormatPhone(this.appManager.m_strCurrentContactAddress);
					this.textBoxContactName.Text = "Unknown";
					return;
				}
				this.textBoxContactPhone.Text = this.appManager.FormatPhone(contactByID.mobileNumber);
				this.textBoxContactName.Text = contactByID.firstName + " " + contactByID.lastName;
				if (this.textBoxContactName.Text == " ")
				{
					this.textBoxContactName.Text = "Unnamed";
					return;
				}
			}
			else
			{
				this.ResetMessageForm(null);
			}
		}

		public void PrintConversation(bool EntireConversation, DateTime? StartDate, DateTime? EndDate)
		{
			this.strError = string.Empty;
			string text = Path.GetTempPath();
			text = string.Concat(new object[]
			{
				text,
				this.appManager.FormatFileName(this.textBoxContactName.Text),
				DateTime.Now.ToFileTimeUtc(),
				".htm"
			});
			string text2 = "Most Recent " + this.appManager.m_lsMessages.Count.ToString() + " Messages";
			this.lsMessagesWorking = this.appManager.m_lsMessages;
			if (EntireConversation)
			{
				ConversationResponse response = this.appManager.m_textService.GetConversation(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession, 0, 999).Data.response;
				this.lsMessagesWorking = response.messages;
				IEnumerable<TextMessage> arg_101_0 = this.lsMessagesWorking;
				Func<TextMessage, string> arg_101_1;
				if ((arg_101_1 = fmMessages.<>c.<>9__32_0) == null)
				{
					arg_101_1 = (fmMessages.<>c.<>9__32_0 = new Func<TextMessage, string>(fmMessages.<>c.<>9.<PrintConversation>b__32_0));
				}
				IOrderedEnumerable<TextMessage> arg_125_0 = arg_101_0.OrderBy(arg_101_1);
				Func<TextMessage, long> arg_125_1;
				if ((arg_125_1 = fmMessages.<>c.<>9__32_1) == null)
				{
					arg_125_1 = (fmMessages.<>c.<>9__32_1 = new Func<TextMessage, long>(fmMessages.<>c.<>9.<PrintConversation>b__32_1));
				}
				this.lsMessagesWorking = arg_125_0.ThenBy(arg_125_1).ToList<TextMessage>();
				text2 = "Entire Conversation of " + this.lsMessagesWorking.Count.ToString() + " Messages";
			}
			if (StartDate.HasValue)
			{
				StartDate = new DateTime?(StartDate.Value.Date);
				EndDate = new DateTime?(EndDate.Value.Date);
				text2 = "Messages From: " + StartDate.Value.ToShortDateString() + " To: " + EndDate.Value.ToShortDateString();
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(text);
				streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
				streamWriter.WriteLine("<html><head><title>" + this.appManager.m_strApplicationName + " Conversation Export</title></head><body>");
				streamWriter.WriteLine("<script>");
				streamWriter.WriteLine("function printFunction() {");
				streamWriter.WriteLine("window.print();");
				streamWriter.WriteLine("}");
				streamWriter.WriteLine("</script>");
				streamWriter.WriteLine(string.Concat(new string[]
				{
					"<H1>",
					this.textBoxContactName.Text,
					" ",
					this.textBoxContactPhone.Text,
					"</H1>"
				}));
				streamWriter.WriteLine(string.Concat(new string[]
				{
					"<H3>",
					text2,
					" &nbsp; &nbsp; &nbsp; Date Created: ",
					DateTime.Now.ToString(),
					" &nbsp; &nbsp; &nbsp; <button onclick=\"printFunction()\">Print</button></H3>"
				}));
				streamWriter.WriteLine("<table cellpadding=\"10\">");
				streamWriter.WriteLine("<tr><th>Date</th><th>Phone Number</th><th>Contact</th><th>Message</th></tr>");
				foreach (TextMessage current in this.lsMessagesWorking)
				{
					string arg_2E8_0 = string.Empty;
					bool flag = true;
					DateTime dateTime;
					DateTime.TryParse(current.dateCreated, out dateTime);
					if (StartDate.HasValue && dateTime < StartDate)
					{
						flag = false;
					}
					if (EndDate.HasValue && dateTime > EndDate)
					{
						flag = false;
					}
					if (flag)
					{
						streamWriter.WriteLine("<tr>");
						if (this.appManager.FormatContactAddress(current.destAddress, true, true) != this.appManager.m_strUserName)
						{
							streamWriter.WriteLine("<td nowrap=\"nowrap\">" + dateTime + "</td>");
							streamWriter.WriteLine("<td nowrap=\"nowrap\">" + this.appManager.FormatPhone(this.appManager.m_strUserName) + "</td>");
							streamWriter.WriteLine("<td></td>");
							streamWriter.WriteLine("<td>" + current.body + "</td>");
						}
						else
						{
							streamWriter.WriteLine("<td nowrap=\"nowrap\">" + dateTime + "</td>");
							streamWriter.WriteLine("<td nowrap=\"nowrap\">" + this.textBoxContactPhone.Text + "</td>");
							streamWriter.WriteLine("<td>" + this.textBoxContactName.Text + "</td>");
							streamWriter.WriteLine("<td bgcolor=\"#E8FFCC\">" + current.body + "</td>");
						}
						streamWriter.WriteLine("</tr>");
					}
				}
				streamWriter.WriteLine("</table>");
				streamWriter.WriteLine("</body></html>");
				streamWriter.Close();
			}
			catch (Exception ex)
			{
				this.strError = "Error exporting conversation: " + ex.Message;
			}
			if (!string.IsNullOrEmpty(this.strError))
			{
				this.appManager.ShowBalloon(this.strError, 5);
				this.strError = string.Empty;
				return;
			}
			this.comboBoxConversationAction_Load();
			string text3 = "Use the Print function from your browser to print the conversation...";
			this.appManager.ShowBalloon(text3, 5);
			Process.Start(text);
		}

		public void ExportConversation()
		{
			this.strError = string.Empty;
			string text = string.Empty;
			ConversationResponse response = this.appManager.m_textService.GetConversation(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession, 0, 999).Data.response;
			this.lsMessagesWorking = response.messages;
			IEnumerable<TextMessage> arg_7A_0 = this.lsMessagesWorking;
			Func<TextMessage, string> arg_7A_1;
			if ((arg_7A_1 = fmMessages.<>c.<>9__33_0) == null)
			{
				arg_7A_1 = (fmMessages.<>c.<>9__33_0 = new Func<TextMessage, string>(fmMessages.<>c.<>9.<ExportConversation>b__33_0));
			}
			IOrderedEnumerable<TextMessage> arg_9E_0 = arg_7A_0.OrderBy(arg_7A_1);
			Func<TextMessage, long> arg_9E_1;
			if ((arg_9E_1 = fmMessages.<>c.<>9__33_1) == null)
			{
				arg_9E_1 = (fmMessages.<>c.<>9__33_1 = new Func<TextMessage, long>(fmMessages.<>c.<>9.<ExportConversation>b__33_1));
			}
			this.lsMessagesWorking = arg_9E_0.ThenBy(arg_9E_1).ToList<TextMessage>();
			string text2 = "Entire Conversation of " + this.lsMessagesWorking.Count.ToString() + " Messages";
			this.saveFileDialogPrintConversation.Title = "Export " + this.appManager.m_strApplicationName + " Conversation";
			this.saveFileDialogPrintConversation.FileName = this.appManager.FormatFileName(this.textBoxContactName.Text);
			if (this.saveFileDialogPrintConversation.ShowDialog(this) != DialogResult.Cancel)
			{
				try
				{
					text = this.saveFileDialogPrintConversation.FileName;
					StreamWriter streamWriter = new StreamWriter(text);
					streamWriter.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
					streamWriter.WriteLine("<html><head><title>" + this.appManager.m_strApplicationName + " Conversation Export</title></head><body>");
					streamWriter.WriteLine(string.Concat(new string[]
					{
						"<H1>",
						this.textBoxContactName.Text,
						" ",
						this.textBoxContactPhone.Text,
						"</H1>"
					}));
					streamWriter.WriteLine(string.Concat(new string[]
					{
						"<H3>",
						text2,
						" &nbsp; &nbsp; &nbsp; &nbsp; Date Created: ",
						DateTime.Now.ToString(),
						"</H3>"
					}));
					streamWriter.WriteLine("<table cellpadding=\"10\">");
					streamWriter.WriteLine("<tr><th>Date</th><th>Phone Number</th><th>Contact</th><th>Message</th></tr>");
					foreach (TextMessage current in this.lsMessagesWorking)
					{
						string arg_226_0 = string.Empty;
						bool arg_237_0 = true;
						DateTime dateTime;
						DateTime.TryParse(current.dateCreated, out dateTime);
						if (arg_237_0)
						{
							streamWriter.WriteLine("<tr>");
							if (this.appManager.FormatContactAddress(current.destAddress, true, true) != this.appManager.m_strUserName)
							{
								streamWriter.WriteLine("<td nowrap=\"nowrap\">" + dateTime + "</td>");
								streamWriter.WriteLine("<td nowrap=\"nowrap\">" + this.appManager.FormatPhone(this.appManager.m_strUserName) + "</td>");
								streamWriter.WriteLine("<td></td>");
								streamWriter.WriteLine("<td>" + current.body + "</td>");
							}
							else
							{
								streamWriter.WriteLine("<td nowrap=\"nowrap\">" + dateTime + "</td>");
								streamWriter.WriteLine("<td nowrap=\"nowrap\">" + this.textBoxContactPhone.Text + "</td>");
								streamWriter.WriteLine("<td>" + this.textBoxContactName.Text + "</td>");
								streamWriter.WriteLine("<td bgcolor=\"#E8FFCC\">" + current.body + "</td>");
							}
							streamWriter.WriteLine("</tr>");
						}
					}
					streamWriter.WriteLine("</table>");
					streamWriter.WriteLine("</body></html>");
					streamWriter.Close();
				}
				catch (Exception ex)
				{
					this.strError = "Error exporting conversation: " + ex.Message;
				}
				if (!string.IsNullOrEmpty(this.strError))
				{
					this.appManager.ShowBalloon(this.strError, 5);
					this.strError = string.Empty;
					return;
				}
				this.comboBoxConversationAction_Load();
				Process.Start(text);
				string text3 = "Conversation exported to file: " + text;
				this.appManager.ShowBalloon(text3, 5);
			}
		}

		public void ResetMessageForm(string strDisplayText = null)
		{
			if (strDisplayText == null)
			{
				strDisplayText = "Select item...";
			}
			this.pnMessages.Controls.Clear();
			Label label = new Label();
			label.Font = this.appManager.m_fontSize;
			label.ForeColor = Color.Black;
			label.Text = strDisplayText;
			label.AutoSize = false;
			label.Dock = DockStyle.Fill;
			label.TextAlign = ContentAlignment.MiddleCenter;
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
			this.tableLayoutPanelMessages.RowStyles[0].Height = (float)(this.nPadding + this.appManager.m_fontSize.Height);
			this.tableLayoutPanelMessages.RowStyles[1].Height = (float)(this.nPadding + this.appManager.m_fontSize.Height);
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			this.strError = null;
			if (this.textBoxMessage.Text.Length == 0 || this.textBoxMessage.Text.Trim() == this.appManager.m_strSignature.Trim())
			{
				this.textBoxMessage.Focus();
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
				{
					IRestResponse<MMSSendResponse> restResponse = this.appManager.m_textService.SendMessageMMS(this.textBoxMessage.Text, this.appManager.FormatContactAddress(this.appManager.m_strCurrentContactAddress, true, true), this.appManager.m_strSession, this.pictureBoxAttachment.ImageLocation);
					if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
					{
						this.strError = "Error calling MMS messaging/send: " + restResponse.ErrorMessage;
					}
					else if (!restResponse.Data.success)
					{
						this.strError = "Error from MMS messaging/send...";
					}
				}
				else
				{
					DateTime value = this.dateTimePickerScheduleDate.Value;
					IRestResponse<TextMessageSendResponse> restResponse2 = this.appManager.m_textService.SendMessage(this.textBoxMessage.Text, this.appManager.m_strCurrentContactAddress, this.appManager.m_strSession, value);
					if (!string.IsNullOrEmpty(restResponse2.ErrorMessage))
					{
						this.strError = "Error calling message/send: " + restResponse2.ErrorMessage;
					}
					else if (!restResponse2.Data.success)
					{
						this.strError = "Error from message/send...";
					}
				}
			}
			catch (Exception ex)
			{
				this.strError = "Error sending text: " + ex.Message;
			}
			if (!string.IsNullOrEmpty(this.strError))
			{
				this.appManager.ShowBalloon(this.strError, 5);
				this.strError = string.Empty;
				return;
			}
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

		private void buttonClear_Click(object sender, EventArgs e)
		{
			this.textBoxSearch.Clear();
			this.textBoxSearch.Focus();
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

		private void buttonMarkAllRead_Click(object sender, EventArgs e)
		{
			this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, true, true);
			this.listBoxConversationList.Invalidate();
		}

		private void linkLabelMoreMessages_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.nMessageLimit += 50;
			this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
		}

		private void linkLabelMoreConversations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.appManager.m_nConversationLimit += 50;
			this.appManager.LoadConversations(true);
		}

		private void pictureBoxConversationCountLock_Click(object sender, EventArgs e)
		{
			this.appManager.m_bConversationCountLocked = !this.appManager.m_bConversationCountLocked;
			if (this.appManager.m_bConversationCountLocked)
			{
				this.pictureBoxConversationCountLock.Image = Resources.locked;
				return;
			}
			this.pictureBoxConversationCountLock.Image = Resources.openlock;
		}

		private void linkLabelEditContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.appManager.ShowEditContact(false);
		}

		private void listBoxConversationList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.bStopConversationListSelectIndexChange)
			{
				this.bStopConversationListSelectIndexChange = false;
				return;
			}
			ListBox listBox = (ListBox)sender;
			if (listBox.SelectedItem != null)
			{
				fmMessages.ConversationItem conversationItem = (fmMessages.ConversationItem)listBox.SelectedItem;
				this.appManager.m_strCurentConversationFingerprint = conversationItem.fingerprint;
				this.appManager.m_strCurrentContactAddress = conversationItem.contactAddress;
				this.appManager.m_nCurrentContactID = conversationItem.contactID;
				this.nMessageLimit = 50;
				this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
			}
			this.listBoxConversationList.Invalidate();
		}

		private void listBoxConversationList_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = this.appManager.m_fontSize.Height * 2 + this.nPadding * 2 + this.appManager.m_fontSizeDT.Height;
		}

		private void listBoxConversationList_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
			{
				return;
			}
			int height = this.appManager.m_fontSize.Height;
			ListBox listBox = (ListBox)sender;
			fmMessages.ConversationItem currentItem = (fmMessages.ConversationItem)listBox.Items[e.Index];
			ConversationMetaData conversationMetaData = this.appManager.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == currentItem.fingerprint);
			if (conversationMetaData == null)
			{
				conversationMetaData = new ConversationMetaData();
			}
			Conversation conversation = this.appManager.m_lsConversation.Find((Conversation p) => p.fingerprint == currentItem.fingerprint);
			string s = string.Empty;
			string s2 = string.Empty;
			string s3 = string.Empty;
			int num = 0;
			if (conversation != null && conversation.unreadCount > 0)
			{
				num = conversation.unreadCount;
				s3 = conversation.unreadCount.ToString();
			}
			s = currentItem.contactName;
			s2 = currentItem.contactPhone;
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
			else if (num > 0)
			{
				e.Graphics.FillRectangle(this.appManager.m_brushHighlight, e.Bounds);
			}
			else if (conversationMetaData.lastMessageIsError)
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
			e.Graphics.DrawString(s, this.appManager.m_fontSize, this.appManager.m_brushBlack, (float)(e.Bounds.Left + this.nPadding), (float)(e.Bounds.Top + this.nPadding));
			e.Graphics.DrawString(s2, this.appManager.m_fontSize, this.appManager.m_brushBlack, (float)(e.Bounds.Left + this.nPadding), (float)(e.Bounds.Top + height + this.nPadding));
			e.Graphics.DrawString(currentItem.dtLastDate.ToString(), new Font(this.appManager.m_fontSizeDT, FontStyle.Italic), this.appManager.m_brushDimGray, (float)(e.Bounds.Left + this.nPadding), (float)(e.Bounds.Top + height * 2 + this.nPadding));
			if (conversationMetaData.lastMessageIsError)
			{
				e.Graphics.DrawString("Failure", new Font(this.appManager.m_fontSize, FontStyle.Bold), this.appManager.m_brushDimGray, (float)(e.Bounds.Left + width), (float)(e.Bounds.Top + height + this.nPadding));
			}
			else
			{
				e.Graphics.DrawString(s3, new Font(this.appManager.m_fontSize, FontStyle.Bold), this.appManager.m_brushBlack, (float)(e.Bounds.Left + width), (float)(e.Bounds.Top + height + this.nPadding));
			}
			e.Graphics.DrawString(conversationMetaData.lastMessageDirection, new Font(this.appManager.m_fontSizeDT, FontStyle.Bold), this.appManager.m_brushDimGray, (float)(e.Bounds.Left + width), (float)(e.Bounds.Top + height * 2 + this.nPadding));
			e.Graphics.DrawRectangle(this.appManager.m_penGray, e.Bounds);
		}

		private void textBoxSearch_TextChanged(object sender, EventArgs e)
		{
			if (this.textBoxSearch.Text.Trim().Length > 0)
			{
				this.textBoxSearch.BackColor = Color.Yellow;
				this.labelTextBoxSearchHint.Visible = false;
				this.buttonClear.Enabled = true;
			}
			else
			{
				this.textBoxSearch.BackColor = Color.White;
				this.labelTextBoxSearchHint.Visible = true;
				this.buttonClear.Enabled = false;
			}
			this.DisplayConversatoinList();
		}

		private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\u0012')
			{
				this.buttonRefresh_Click(sender, new EventArgs());
				e.Handled = true;
			}
			if (e.KeyChar == '\u0003')
			{
				this.buttonClear_Click(sender, new EventArgs());
				e.Handled = true;
			}
		}

		private void textBoxMessage_TextChanged(object sender, EventArgs e)
		{
			this.labelCharCount.ForeColor = default(Color);
			int length = this.textBoxMessage.Text.Length;
			if (length == 250)
			{
				this.labelCharCount.ForeColor = Color.Red;
			}
			else if (length > 240)
			{
				this.labelCharCount.ForeColor = Color.Orange;
			}
			this.labelCharCount.Text = length.ToString() + "/250";
		}

		private void textBoxMessage_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.appManager.m_bControlEnter && e.KeyChar == '\n')
			{
				this.buttonSend_Click(sender, new EventArgs());
				e.Handled = true;
			}
			if (!this.appManager.m_bControlEnter && e.KeyChar == '\r')
			{
				this.buttonSend_Click(sender, new EventArgs());
				e.Handled = true;
			}
			if (e.KeyChar == '\u0012')
			{
				this.buttonRefresh_Click(sender, new EventArgs());
				e.Handled = true;
			}
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
					this.labelUnreadCount.BackColor = default(Color);
					this.labelUnread.BackColor = default(Color);
				}
			}
			catch
			{
			}
		}

		private void newMessageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowNewMessage();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowSettings();
		}

		private void manageGroupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditGroups();
		}

		private void manageContactsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditContact(true);
		}

		private void manageKeywordAutoResponseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowKeywordAutoResponse();
		}

		private void editGroupScheduleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowGroupSchedule();
		}

		private void editMessageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowMessageTemplate();
		}

		private void messageTemplateMenuItem_Click(object sender, EventArgs e)
		{
			if (this.textBoxMessage.Enabled)
			{
				ToolStripItem arg_1C_0 = (ToolStripDropDownItem)sender;
				string text = string.Empty;
				string name = arg_1C_0.Name;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
				if (num <= 856466825u)
				{
					if (num <= 806133968u)
					{
						if (num != 468396612u)
						{
							if (num == 806133968u)
							{
								if (name == "5")
								{
									text = this.appManager.m_strMessageTemplate5;
									goto IL_22F;
								}
							}
						}
						else if (name == "10")
						{
							text = this.appManager.m_strMessageTemplate10;
							goto IL_22F;
						}
					}
					else if (num != 822911587u)
					{
						if (num != 839689206u)
						{
							if (num == 856466825u)
							{
								if (name == "6")
								{
									text = this.appManager.m_strMessageTemplate6;
									goto IL_22F;
								}
							}
						}
						else if (name == "7")
						{
							text = this.appManager.m_strMessageTemplate7;
							goto IL_22F;
						}
					}
					else if (name == "4")
					{
						text = this.appManager.m_strMessageTemplate4;
						goto IL_22F;
					}
				}
				else if (num <= 906799682u)
				{
					if (num != 873244444u)
					{
						if (num == 906799682u)
						{
							if (name == "3")
							{
								text = this.appManager.m_strMessageTemplate3;
								goto IL_22F;
							}
						}
					}
					else if (name == "1")
					{
						text = this.appManager.m_strMessageTemplate1;
						goto IL_22F;
					}
				}
				else if (num != 923577301u)
				{
					if (num != 1007465396u)
					{
						if (num == 1024243015u)
						{
							if (name == "8")
							{
								text = this.appManager.m_strMessageTemplate8;
								goto IL_22F;
							}
						}
					}
					else if (name == "9")
					{
						text = this.appManager.m_strMessageTemplate9;
						goto IL_22F;
					}
				}
				else if (name == "2")
				{
					text = this.appManager.m_strMessageTemplate2;
					goto IL_22F;
				}
				this.appManager.ShowBalloon("Invalid message template selection", 5);
				IL_22F:
				this.textBoxMessage.Text = text;
				this.textBoxMessage.Select(this.textBoxMessage.Text.Length, 0);
				return;
			}
			this.appManager.ShowBalloon("You must first select an item before adding a message template response...", 5);
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

		private void controlEnterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "ControlEnter", this.controlEnterToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Control Enter save error: " + this.strError;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bControlEnter = this.controlEnterToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Control Enter save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 5);
			}
		}

		private void requireClickToMarkMessageReadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "ClickMarkMessageRead", this.requireClickToMarkMessageReadToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Click Mark Message Read save error: " + text;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bRequreClickToMarkMessageRead = this.requireClickToMarkMessageReadToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Click Mark Message Read save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 5);
			}
		}

		private void openMessagesWindowWithUnreadReminderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "PopMessageWindow", this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Open message window with unread reminder save error: " + text;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bPopMessageWindow = this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Open message window with unread reminder save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 5);
			}
		}

		private void keepSelectedConversationInFocusToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "KeepConversationFocus", this.keepSelectedConversationInFocusToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Keep conversation in focus save error: " + text;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bKeepConversationFocus = this.keepSelectedConversationInFocusToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Keep conversation in focus save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 5);
			}
		}

		private void displayMMSAttachmentsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = string.Empty;
			try
			{
				AppRegistry.SaveValue(AppRegistry.GetRootKey(ref text), "DisplayMMSAttachments", this.displayMMSAttachmentsToolStripMenuItem.Checked, ref text, false, RegistryValueKind.Unknown);
				if (text != string.Empty)
				{
					text = "Display MMS Attachments save error: " + text;
					this.appManager.ShowBalloon(text, 5);
				}
				else
				{
					this.appManager.m_bDisplayMMSAttachments = this.displayMMSAttachmentsToolStripMenuItem.Checked;
				}
			}
			catch (Exception ex)
			{
				text = "Display MMS Attachments save error: " + ex.Message;
				this.appManager.ShowBalloon(text, 5);
			}
		}

		private void syncFeaturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.m_bNotifyServerSync = true;
			this.appManager.GetServerSettings(true);
		}

		private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.LogOut(true);
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

		private void comboBoxConversationAction_Leave(object sender, EventArgs e)
		{
			this.comboBoxConversationAction_Load();
		}

		private void comboBoxConversationAction_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.bConversationActionLoad)
			{
				this.bConversationActionLoad = false;
				return;
			}
			if (this.bConversationDelete)
			{
				this.bConversationDelete = false;
				return;
			}
			if (this.bConversationExport)
			{
				this.bConversationExport = false;
				return;
			}
			this.comboBoxConversationAction.DroppedDown = true;
		}

		private void comboBoxConversationAction_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (!this.comboBoxConversationAction.DroppedDown)
			{
				return;
			}
			bool flag = false;
			if (this.comboBoxConversationAction.SelectedItem == "Print Conversation" && this.appManager.m_strCurentConversationFingerprint != "0")
			{
				this.bConversationExport = true;
				this.appManager.ShowPrintConversation();
				return;
			}
			if (this.comboBoxConversationAction.SelectedItem == "Export Conversation" && this.appManager.m_strCurentConversationFingerprint != "0")
			{
				this.bConversationExport = true;
				this.ExportConversation();
				return;
			}
			if (this.comboBoxConversationAction.SelectedItem == "Delete Conversation" && this.appManager.m_strCurentConversationFingerprint != "0")
			{
				try
				{
					this.bConversationDelete = true;
					if (MessageBox.Show("Delete the entire conversation with " + this.textBoxContactPhone.Text + "?\n\n Please Note this cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					{
						this.comboBoxConversationAction_Load();
						return;
					}
					if (this.appManager.m_nCurrentContactID > 0L && MessageBox.Show(string.Concat(new string[]
					{
						"Would you also like to delete contact ",
						this.textBoxContactName.Text,
						" ",
						this.textBoxContactPhone.Text,
						"?"
					}), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						flag = true;
					}
					if (this.appManager.m_textService.ConversationDelete(this.appManager.m_strCurentConversationFingerprint, this.appManager.m_strSession).Data.success)
					{
						Conversation conversation = this.appManager.m_lsConversation.Find((Conversation var) => var.fingerprint == this.appManager.m_strCurentConversationFingerprint);
						if (conversation != null)
						{
							this.appManager.m_lsConversation.Remove(conversation);
						}
						ConversationMetaData conversationMetaData = this.appManager.m_lsConversationMetaData.Find((ConversationMetaData var) => var.fingerprint == this.appManager.m_strCurentConversationFingerprint);
						if (conversationMetaData != null)
						{
							this.appManager.m_lsConversationMetaData.Remove(conversationMetaData);
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
								Contact contact = this.appManager.m_lsContact.Find((Contact var) => var.id == this.appManager.m_nCurrentContactID);
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
				catch (Exception ex)
				{
					this.strError = "Error deleting records: " + ex.Message;
				}
				if (!string.IsNullOrEmpty(this.strError))
				{
					this.appManager.ShowBalloon(this.strError, 5);
					this.strError = string.Empty;
					return;
				}
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

		private void comboBoxFilter_Leave(object sender, EventArgs e)
		{
			this.DisplayConversatoinList();
		}

		private void comboBoxFilter_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (!this.comboBoxFilter.DroppedDown)
			{
				return;
			}
			this.DisplayConversatoinList();
		}

		private void richTextBox_LinkClick(object sender, LinkClickedEventArgs e)
		{
			Process.Start(e.LinkText);
		}

		private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(this.textBoxSelectedMessage.Text);
		}

		private void copySelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(this.textBoxSelectedMessage.SelectedText);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this message? Please Note this cannot be undone!\n\n" + this.textBoxSelectedMessage.Text, this.appManager.m_strApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.appManager.m_textService.MarkMessageRead(this.nSelectedMessageID, this.appManager.m_strSession);
				if (!this.appManager.m_textService.DeleteMessage(this.nSelectedMessageID, this.appManager.m_strSession).Data.success)
				{
					this.appManager.ShowBalloon("Error calling message/delete...", 5);
					return;
				}
				this.DisplayConversation(this.appManager.m_strCurentConversationFingerprint, false, true);
				this.DisplayConversatoinList();
			}
		}

		private void markAsReadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!this.appManager.m_textService.MarkMessageRead(this.nSelectedMessageID, this.appManager.m_strSession).Data.success)
			{
				this.appManager.ShowBalloon("Error calling message/read...", 5);
				return;
			}
			this.appManager.LoadUpdates(false);
		}

		private void contextMenuStripMessageText_Opening(object sender, CancelEventArgs e)
		{
			ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
			this.textBoxSelectedMessage = (RichTextBox)contextMenuStrip.SourceControl;
			this.nSelectedMessageID = Convert.ToInt64(this.textBoxSelectedMessage.Name);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			TextMessage textMessage = this.appManager.m_lsMessages.Find((TextMessage m) => m.id == this.nSelectedMessageID);
			if (textMessage != null)
			{
				flag = textMessage.isRead;
				if (textMessage.destAddress != this.appManager.m_strUserName)
				{
					flag2 = true;
				}
				if (!string.IsNullOrEmpty(textMessage.scheduledDate))
				{
					flag3 = true;
				}
			}
			if (this.appManager.m_bRequreClickToMarkMessageRead && !flag2 && !flag)
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
				return;
			}
			this.deleteToolStripMenuItem.Visible = false;
		}

		private void pictureBoxLink_Click(object sender, EventArgs e)
		{
			if (!this.appManager.m_bMMSSendFeature)
			{
				MessageBox.Show("Sending MMS texts (messages with attachements) is not enabled for this account - please contact support to turn on...", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			this.openFileDialog.Title = "Select a file to be sent";
			this.openFileDialog.CheckFileExists = true;
			this.openFileDialog.Multiselect = false;
			this.openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;*.bmp";
			if (this.openFileDialog.ShowDialog(this) != DialogResult.Cancel)
			{
				string fileName = this.openFileDialog.FileName;
				if (new FileInfo(fileName).Length / 1024L < 1024L)
				{
					this.pictureBoxAttachment.ImageLocation = fileName;
					this.labelAttRemove.Visible = true;
					this.dateTimePickerScheduleDate.Enabled = false;
					return;
				}
				MessageBox.Show("File must be less than one MB", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

		private void contextMenuStripPictureBox_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
				this.pictureBoxSelectedMessage = (PictureBox)contextMenuStrip.SourceControl;
				int num = this.pictureBoxSelectedMessage.Name.IndexOf("~");
				string value = this.pictureBoxSelectedMessage.Name.Substring(0, num);
				this.nSelectedMessageID = Convert.ToInt64(value);
				this.strImageStorageKey = this.pictureBoxSelectedMessage.Name.Substring(num + 1, this.pictureBoxSelectedMessage.Name.Length - num - 1);
			}
			catch (Exception)
			{
				this.strImageStorageKey = string.Empty;
			}
		}

		private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				MMSImage attachment = this.appManager.m_textService.GetAttachment(this.appManager.m_strSession, this.strImageStorageKey, this.nSelectedMessageID);
				if (attachment != null)
				{
					if (!string.IsNullOrEmpty(attachment.ext))
					{
						string text = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + this.nSelectedMessageID.ToString() + attachment.ext;
						attachment.image.Save(text);
						Process.Start(text);
					}
					else
					{
						this.strError = "File is not a supported format: .bmp, .gif, .png, .jpg";
					}
				}
			}
			catch (Exception ex)
			{
				this.strError = "Error downloading message attachment: " + ex.Message;
			}
			if (!string.IsNullOrEmpty(this.strError))
			{
				this.appManager.ShowBalloon(this.strError, 5);
				this.strError = string.Empty;
			}
		}

		private void forwardTextMessage_Click(object sender, EventArgs e)
		{
			this.appManager.m_strForwardMessage = this.textBoxSelectedMessage.Text;
			this.appManager.ShowNewMessage();
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmMessages));
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
			((ISupportInitialize)this.pictureBoxConversationCountLock).BeginInit();
			this.tableLayoutPanelMessages.SuspendLayout();
			this.pnSend.SuspendLayout();
			((ISupportInitialize)this.pictureBoxLink).BeginInit();
			((ISupportInitialize)this.pictureBoxAttachment).BeginInit();
			this.pnMoreMessages.SuspendLayout();
			this.tableLayoutPanelContactName.SuspendLayout();
			this.tableLayoutPanelContactPhone.SuspendLayout();
			this.menuStripMessage.SuspendLayout();
			this.contextMenuStripMessageText.SuspendLayout();
			this.contextMenuStripPictureBox.SuspendLayout();
			base.SuspendLayout();
			this.splitContainerMessages.Dock = DockStyle.Fill;
			this.splitContainerMessages.FixedPanel = FixedPanel.Panel1;
			this.splitContainerMessages.Location = new Point(0, 24);
			this.splitContainerMessages.MinimumSize = new Size(524, 437);
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
			this.splitContainerMessages.Panel2MinSize = 345;
			this.splitContainerMessages.Size = new Size(584, 537);
			this.splitContainerMessages.SplitterDistance = 225;
			this.splitContainerMessages.SplitterWidth = 5;
			this.splitContainerMessages.TabIndex = 0;
			this.splitContainerMessages.TabStop = false;
			this.labelTextBoxSearchHint.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.labelTextBoxSearchHint.AutoSize = true;
			this.labelTextBoxSearchHint.BackColor = SystemColors.Window;
			this.labelTextBoxSearchHint.Font = new Font("Arial", 9f, FontStyle.Italic);
			this.labelTextBoxSearchHint.ForeColor = SystemColors.ControlDark;
			this.labelTextBoxSearchHint.Location = new Point(11, 506);
			this.labelTextBoxSearchHint.Margin = new Padding(0);
			this.labelTextBoxSearchHint.Name = "labelTextBoxSearchHint";
			this.labelTextBoxSearchHint.Size = new Size(88, 15);
			this.labelTextBoxSearchHint.TabIndex = 18;
			this.labelTextBoxSearchHint.Text = "Type to Search";
			this.labelTextBoxSearchHint.TextAlign = ContentAlignment.MiddleLeft;
			this.labelTextBoxSearchHint.Click += new EventHandler(this.labelTextBoxSearchHint_Click);
			this.pictureBoxConversationCountLock.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.pictureBoxConversationCountLock.Image = Resources.openlock;
			this.pictureBoxConversationCountLock.Location = new Point(162, 4);
			this.pictureBoxConversationCountLock.Name = "pictureBoxConversationCountLock";
			this.pictureBoxConversationCountLock.Size = new Size(16, 16);
			this.pictureBoxConversationCountLock.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBoxConversationCountLock.TabIndex = 17;
			this.pictureBoxConversationCountLock.TabStop = false;
			this.pictureBoxConversationCountLock.Click += new EventHandler(this.pictureBoxConversationCountLock_Click);
			this.labelUnread.AutoSize = true;
			this.labelUnread.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelUnread.Location = new Point(8, 4);
			this.labelUnread.Name = "labelUnread";
			this.labelUnread.Size = new Size(53, 16);
			this.labelUnread.TabIndex = 16;
			this.labelUnread.Text = "Unread:";
			this.linkLabelMoreConversations.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.linkLabelMoreConversations.AutoSize = true;
			this.linkLabelMoreConversations.LinkColor = Color.Olive;
			this.linkLabelMoreConversations.Location = new Point(178, 7);
			this.linkLabelMoreConversations.Name = "linkLabelMoreConversations";
			this.linkLabelMoreConversations.Size = new Size(40, 13);
			this.linkLabelMoreConversations.TabIndex = 11;
			this.linkLabelMoreConversations.TabStop = true;
			this.linkLabelMoreConversations.Text = "More...";
			this.linkLabelMoreConversations.Visible = false;
			this.linkLabelMoreConversations.VisitedLinkColor = Color.Olive;
			this.linkLabelMoreConversations.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelMoreConversations_LinkClicked);
			this.comboBoxFilter.AllowDrop = true;
			this.comboBoxFilter.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxFilter.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFilter.FlatStyle = FlatStyle.Flat;
			this.comboBoxFilter.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxFilter.FormattingEnabled = true;
			this.comboBoxFilter.ItemHeight = 15;
			this.comboBoxFilter.Location = new Point(8, 471);
			this.comboBoxFilter.Name = "comboBoxFilter";
			this.comboBoxFilter.Size = new Size(210, 23);
			this.comboBoxFilter.TabIndex = 14;
			this.comboBoxFilter.SelectionChangeCommitted += new EventHandler(this.comboBoxFilter_SelectionChangeCommitted);
			this.comboBoxFilter.Leave += new EventHandler(this.comboBoxFilter_Leave);
			this.buttonClear.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonClear.Enabled = false;
			this.buttonClear.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonClear.Location = new Point(162, 499);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new Size(56, 27);
			this.buttonClear.TabIndex = 5;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new EventHandler(this.buttonClear_Click);
			this.textBoxSearch.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxSearch.BorderStyle = BorderStyle.None;
			this.textBoxSearch.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxSearch.Location = new Point(8, 504);
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new Size(148, 18);
			this.textBoxSearch.TabIndex = 4;
			this.textBoxSearch.TextChanged += new EventHandler(this.textBoxSearch_TextChanged);
			this.textBoxSearch.KeyPress += new KeyPressEventHandler(this.textBoxSearch_KeyPress);
			this.labelConversationsCount.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.labelConversationsCount.AutoSize = true;
			this.labelConversationsCount.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.labelConversationsCount.Location = new Point(8, 21);
			this.labelConversationsCount.Name = "labelConversationsCount";
			this.labelConversationsCount.Size = new Size(152, 14);
			this.labelConversationsCount.TabIndex = 7;
			this.labelConversationsCount.Text = "50 Displayed conversations...";
			this.labelUnreadCount.AutoSize = true;
			this.labelUnreadCount.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelUnreadCount.Location = new Point(67, 4);
			this.labelUnreadCount.Name = "labelUnreadCount";
			this.labelUnreadCount.Size = new Size(15, 16);
			this.labelUnreadCount.TabIndex = 5;
			this.labelUnreadCount.Text = "0";
			this.labelUnreadCount.TextChanged += new EventHandler(this.labelUnreadCount_TextChanged);
			this.labelLoadingConversation.BackColor = Color.Transparent;
			this.labelLoadingConversation.Font = new Font("Arial", 12f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelLoadingConversation.Location = new Point(33, 135);
			this.labelLoadingConversation.Name = "labelLoadingConversation";
			this.labelLoadingConversation.Size = new Size(90, 195);
			this.labelLoadingConversation.TabIndex = 15;
			this.labelLoadingConversation.Text = "Loading Conversations...";
			this.labelLoadingConversation.TextAlign = ContentAlignment.MiddleCenter;
			this.listBoxConversationList.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.listBoxConversationList.BackColor = SystemColors.ControlLight;
			this.listBoxConversationList.BorderStyle = BorderStyle.None;
			this.listBoxConversationList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.listBoxConversationList.FormattingEnabled = true;
			this.listBoxConversationList.ItemHeight = 17;
			this.listBoxConversationList.Location = new Point(11, 40);
			this.listBoxConversationList.Name = "listBoxConversationList";
			this.listBoxConversationList.ScrollAlwaysVisible = true;
			this.listBoxConversationList.Size = new Size(207, 408);
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
			this.tableLayoutPanelMessages.Size = new Size(354, 537);
			this.tableLayoutPanelMessages.TabIndex = 17;
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
			this.pnSend.Location = new Point(3, 368);
			this.pnSend.Name = "pnSend";
			this.pnSend.Size = new Size(348, 166);
			this.pnSend.TabIndex = 3;
			this.checkBoxFullRefresh.AutoSize = true;
			this.checkBoxFullRefresh.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.checkBoxFullRefresh.ForeColor = Color.FromArgb(64, 64, 64);
			this.checkBoxFullRefresh.Location = new Point(8, 32);
			this.checkBoxFullRefresh.Name = "checkBoxFullRefresh";
			this.checkBoxFullRefresh.Size = new Size(87, 18);
			this.checkBoxFullRefresh.TabIndex = 8;
			this.checkBoxFullRefresh.Text = "Full Refresh";
			this.checkBoxFullRefresh.UseVisualStyleBackColor = true;
			this.labelAttRemove.AutoSize = true;
			this.labelAttRemove.BackColor = Color.Transparent;
			this.labelAttRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.labelAttRemove.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelAttRemove.Location = new Point(5, 118);
			this.labelAttRemove.Name = "labelAttRemove";
			this.labelAttRemove.Size = new Size(84, 14);
			this.labelAttRemove.TabIndex = 7;
			this.labelAttRemove.Text = "Click to remove";
			this.labelAttRemove.Visible = false;
			this.labelAttRemove.Click += new EventHandler(this.labelAttRemove_Click);
			this.pictureBoxLink.Image = Resources.Paperclip;
			this.pictureBoxLink.Location = new Point(72, 133);
			this.pictureBoxLink.Name = "pictureBoxLink";
			this.pictureBoxLink.Size = new Size(20, 20);
			this.pictureBoxLink.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBoxLink.TabIndex = 6;
			this.pictureBoxLink.TabStop = false;
			this.pictureBoxLink.Click += new EventHandler(this.pictureBoxLink_Click);
			this.pictureBoxAttachment.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.pictureBoxAttachment.Location = new Point(8, 108);
			this.pictureBoxAttachment.Name = "pictureBoxAttachment";
			this.pictureBoxAttachment.Size = new Size(81, 46);
			this.pictureBoxAttachment.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBoxAttachment.TabIndex = 5;
			this.pictureBoxAttachment.TabStop = false;
			this.pictureBoxAttachment.Click += new EventHandler(this.labelAttRemove_Click);
			this.buttonMarkAllRead.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonMarkAllRead.BackColor = Color.LimeGreen;
			this.buttonMarkAllRead.FlatAppearance.BorderColor = Color.Black;
			this.buttonMarkAllRead.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonMarkAllRead.Location = new Point(8, 52);
			this.buttonMarkAllRead.Name = "buttonMarkAllRead";
			this.buttonMarkAllRead.Size = new Size(81, 50);
			this.buttonMarkAllRead.TabIndex = 4;
			this.buttonMarkAllRead.Text = "Clear Unread";
			this.toolTipMarkAllRead.SetToolTip(this.buttonMarkAllRead, "Click to mark all messages in this conversation as read.");
			this.buttonMarkAllRead.UseVisualStyleBackColor = false;
			this.buttonMarkAllRead.Visible = false;
			this.buttonMarkAllRead.Click += new EventHandler(this.buttonMarkAllRead_Click);
			this.dateTimePickerScheduleDate.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.dateTimePickerScheduleDate.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.dateTimePickerScheduleDate.Format = DateTimePickerFormat.Custom;
			this.dateTimePickerScheduleDate.Location = new Point(94, 132);
			this.dateTimePickerScheduleDate.MinDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
			this.dateTimePickerScheduleDate.Name = "dateTimePickerScheduleDate";
			this.dateTimePickerScheduleDate.Size = new Size(127, 22);
			this.dateTimePickerScheduleDate.TabIndex = 1;
			this.dateTimePickerScheduleDate.Visible = false;
			this.buttonRefresh.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonRefresh.Location = new Point(8, 5);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new Size(81, 27);
			this.buttonRefresh.TabIndex = 3;
			this.buttonRefresh.Text = "Refresh";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
			this.buttonSend.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSend.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSend.Location = new Point(276, 128);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new Size(56, 27);
			this.buttonSend.TabIndex = 2;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Visible = false;
			this.buttonSend.Click += new EventHandler(this.buttonSend_Click);
			this.labelCharCount.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.labelCharCount.AutoSize = true;
			this.labelCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelCharCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelCharCount.Location = new Point(222, 135);
			this.labelCharCount.MinimumSize = new Size(55, 0);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new Size(55, 15);
			this.labelCharCount.TabIndex = 1;
			this.labelCharCount.Text = "250/250";
			this.labelCharCount.TextAlign = ContentAlignment.MiddleRight;
			this.labelCharCount.Visible = false;
			this.textBoxMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxMessage.BorderStyle = BorderStyle.None;
			this.textBoxMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxMessage.Location = new Point(95, 3);
			this.textBoxMessage.MaxLength = 250;
			this.textBoxMessage.Multiline = true;
			this.textBoxMessage.Name = "textBoxMessage";
			this.textBoxMessage.ScrollBars = ScrollBars.Vertical;
			this.textBoxMessage.Size = new Size(244, 118);
			this.textBoxMessage.TabIndex = 0;
			this.textBoxMessage.Visible = false;
			this.textBoxMessage.TextChanged += new EventHandler(this.textBoxMessage_TextChanged);
			this.textBoxMessage.KeyPress += new KeyPressEventHandler(this.textBoxMessage_KeyPress);
			this.pnMessages.AutoScroll = true;
			this.pnMessages.BackColor = SystemColors.ControlLight;
			this.pnMessages.Cursor = Cursors.Default;
			this.pnMessages.Dock = DockStyle.Fill;
			this.pnMessages.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.pnMessages.Location = new Point(3, 78);
			this.pnMessages.MinimumSize = new Size(343, 0);
			this.pnMessages.Name = "pnMessages";
			this.pnMessages.Size = new Size(348, 284);
			this.pnMessages.TabIndex = 5;
			this.pnMoreMessages.Controls.Add(this.labelMesssageCount);
			this.pnMoreMessages.Controls.Add(this.linkLabelMoreMessages);
			this.pnMoreMessages.Dock = DockStyle.Fill;
			this.pnMoreMessages.Location = new Point(3, 58);
			this.pnMoreMessages.Name = "pnMoreMessages";
			this.pnMoreMessages.Size = new Size(348, 14);
			this.pnMoreMessages.TabIndex = 5;
			this.labelMesssageCount.AutoSize = true;
			this.labelMesssageCount.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.labelMesssageCount.Location = new Point(0, 1);
			this.labelMesssageCount.Name = "labelMesssageCount";
			this.labelMesssageCount.Size = new Size(117, 14);
			this.labelMesssageCount.TabIndex = 11;
			this.labelMesssageCount.Text = "0 Messages displayed";
			this.linkLabelMoreMessages.AutoSize = true;
			this.linkLabelMoreMessages.Dock = DockStyle.Right;
			this.linkLabelMoreMessages.LinkColor = Color.Olive;
			this.linkLabelMoreMessages.Location = new Point(308, 0);
			this.linkLabelMoreMessages.Name = "linkLabelMoreMessages";
			this.linkLabelMoreMessages.Size = new Size(40, 13);
			this.linkLabelMoreMessages.TabIndex = 10;
			this.linkLabelMoreMessages.TabStop = true;
			this.linkLabelMoreMessages.Text = "More...";
			this.linkLabelMoreMessages.Visible = false;
			this.linkLabelMoreMessages.VisitedLinkColor = Color.Olive;
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
			this.tableLayoutPanelContactName.Size = new Size(354, 28);
			this.tableLayoutPanelContactName.TabIndex = 6;
			this.textBoxContactName.BackColor = SystemColors.Control;
			this.textBoxContactName.BorderStyle = BorderStyle.None;
			this.textBoxContactName.Dock = DockStyle.Fill;
			this.textBoxContactName.Font = new Font("Arial", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.textBoxContactName.Location = new Point(3, 3);
			this.textBoxContactName.Name = "textBoxContactName";
			this.textBoxContactName.ReadOnly = true;
			this.textBoxContactName.Size = new Size(300, 18);
			this.textBoxContactName.TabIndex = 14;
			this.textBoxContactName.TabStop = false;
			this.textBoxContactName.Text = "Name";
			this.linkLabelEditContact.AutoSize = true;
			this.linkLabelEditContact.Dock = DockStyle.Right;
			this.linkLabelEditContact.LinkColor = Color.Olive;
			this.linkLabelEditContact.Location = new Point(317, 0);
			this.linkLabelEditContact.Name = "linkLabelEditContact";
			this.linkLabelEditContact.Size = new Size(34, 28);
			this.linkLabelEditContact.TabIndex = 16;
			this.linkLabelEditContact.TabStop = true;
			this.linkLabelEditContact.Text = "Edit...";
			this.linkLabelEditContact.VisitedLinkColor = Color.Olive;
			this.linkLabelEditContact.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelEditContact_LinkClicked);
			this.tableLayoutPanelContactPhone.ColumnCount = 2;
			this.tableLayoutPanelContactPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanelContactPhone.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 176f));
			this.tableLayoutPanelContactPhone.Controls.Add(this.textBoxContactPhone, 0, 0);
			this.tableLayoutPanelContactPhone.Controls.Add(this.comboBoxConversationAction, 1, 0);
			this.tableLayoutPanelContactPhone.Dock = DockStyle.Fill;
			this.tableLayoutPanelContactPhone.Location = new Point(0, 28);
			this.tableLayoutPanelContactPhone.Margin = new Padding(0);
			this.tableLayoutPanelContactPhone.Name = "tableLayoutPanelContactPhone";
			this.tableLayoutPanelContactPhone.RowCount = 1;
			this.tableLayoutPanelContactPhone.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
			this.tableLayoutPanelContactPhone.Size = new Size(354, 27);
			this.tableLayoutPanelContactPhone.TabIndex = 7;
			this.textBoxContactPhone.BackColor = SystemColors.Control;
			this.textBoxContactPhone.BorderStyle = BorderStyle.None;
			this.textBoxContactPhone.Dock = DockStyle.Fill;
			this.textBoxContactPhone.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxContactPhone.Location = new Point(3, 3);
			this.textBoxContactPhone.Name = "textBoxContactPhone";
			this.textBoxContactPhone.ReadOnly = true;
			this.textBoxContactPhone.Size = new Size(172, 18);
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
			this.comboBoxConversationAction.Location = new Point(181, 3);
			this.comboBoxConversationAction.Name = "comboBoxConversationAction";
			this.comboBoxConversationAction.Size = new Size(170, 23);
			this.comboBoxConversationAction.TabIndex = 13;
			this.comboBoxConversationAction.Visible = false;
			this.comboBoxConversationAction.SelectedIndexChanged += new EventHandler(this.comboBoxConversationAction_SelectedIndexChanged);
			this.comboBoxConversationAction.SelectionChangeCommitted += new EventHandler(this.comboBoxConversationAction_SelectionChangeCommitted);
			this.comboBoxConversationAction.Leave += new EventHandler(this.comboBoxConversationAction_Leave);
			this.menuStripMessage.ImageScalingSize = new Size(24, 24);
			this.menuStripMessage.Items.AddRange(new ToolStripItem[]
			{
				this.newMessageToolStripMenuItem,
				this.messageTemplatesToolStripMenuItem,
				this.editsToolStripMenuItem,
				this.optionsToolStripMenuItem,
				this.helpToolStripMenuItem
			});
			this.menuStripMessage.Location = new Point(0, 0);
			this.menuStripMessage.Name = "menuStripMessage";
			this.menuStripMessage.Size = new Size(584, 24);
			this.menuStripMessage.TabIndex = 2;
			this.menuStripMessage.Text = "menuStrip1";
			this.newMessageToolStripMenuItem.Name = "newMessageToolStripMenuItem";
			this.newMessageToolStripMenuItem.Size = new Size(92, 20);
			this.newMessageToolStripMenuItem.Text = "&New Message";
			this.newMessageToolStripMenuItem.Click += new EventHandler(this.newMessageToolStripMenuItem_Click);
			this.messageTemplatesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.editMessageTemplatesToolStripMenuItem,
				this.toolStripSeparator1
			});
			this.messageTemplatesToolStripMenuItem.Name = "messageTemplatesToolStripMenuItem";
			this.messageTemplatesToolStripMenuItem.Size = new Size(122, 20);
			this.messageTemplatesToolStripMenuItem.Text = "Message &Templates";
			this.editMessageTemplatesToolStripMenuItem.Name = "editMessageTemplatesToolStripMenuItem";
			this.editMessageTemplatesToolStripMenuItem.ShortcutKeys = (Keys)131141;
			this.editMessageTemplatesToolStripMenuItem.ShowShortcutKeys = false;
			this.editMessageTemplatesToolStripMenuItem.Size = new Size(202, 22);
			this.editMessageTemplatesToolStripMenuItem.Text = "&Edit Message Templates...";
			this.editMessageTemplatesToolStripMenuItem.Click += new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(199, 6);
			this.editsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.manageGroupsToolStripMenuItem,
				this.manageContactsToolStripMenuItem,
				this.editKeywordAutoResponseToolStripMenuItem,
				this.editGroupScheduleToolStripMenuItem
			});
			this.editsToolStripMenuItem.Name = "editsToolStripMenuItem";
			this.editsToolStripMenuItem.Size = new Size(39, 20);
			this.editsToolStripMenuItem.Text = "&Edit";
			this.manageGroupsToolStripMenuItem.Name = "manageGroupsToolStripMenuItem";
			this.manageGroupsToolStripMenuItem.Size = new Size(225, 22);
			this.manageGroupsToolStripMenuItem.Text = "Edit &Groups";
			this.manageGroupsToolStripMenuItem.Click += new EventHandler(this.manageGroupsToolStripMenuItem_Click);
			this.manageContactsToolStripMenuItem.Name = "manageContactsToolStripMenuItem";
			this.manageContactsToolStripMenuItem.Size = new Size(225, 22);
			this.manageContactsToolStripMenuItem.Text = "Edit &Contacts";
			this.manageContactsToolStripMenuItem.Click += new EventHandler(this.manageContactsToolStripMenuItem_Click);
			this.editKeywordAutoResponseToolStripMenuItem.Name = "editKeywordAutoResponseToolStripMenuItem";
			this.editKeywordAutoResponseToolStripMenuItem.Size = new Size(225, 22);
			this.editKeywordAutoResponseToolStripMenuItem.Text = "Edit &Keyword Auto Response";
			this.editKeywordAutoResponseToolStripMenuItem.Click += new EventHandler(this.manageKeywordAutoResponseToolStripMenuItem_Click);
			this.editGroupScheduleToolStripMenuItem.Name = "editGroupScheduleToolStripMenuItem";
			this.editGroupScheduleToolStripMenuItem.Size = new Size(225, 22);
			this.editGroupScheduleToolStripMenuItem.Text = "Edit Group &Schedule";
			this.editGroupScheduleToolStripMenuItem.Click += new EventHandler(this.editGroupScheduleToolStripMenuItem_Click);
			this.optionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.settingsToolStripMenuItem,
				this.toolStripSeparator2,
				this.controlEnterToolStripMenuItem,
				this.requireClickToMarkMessageReadToolStripMenuItem,
				this.openMessagesWindowWithUnreadReminderToolStripMenuItem,
				this.keepSelectedConversationInFocusToolStripMenuItem,
				this.displayMMSAttachmentsToolStripMenuItem
			});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new Size(332, 22);
			this.settingsToolStripMenuItem.Text = "Settings";
			this.settingsToolStripMenuItem.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(329, 6);
			this.controlEnterToolStripMenuItem.CheckOnClick = true;
			this.controlEnterToolStripMenuItem.Name = "controlEnterToolStripMenuItem";
			this.controlEnterToolStripMenuItem.Size = new Size(332, 22);
			this.controlEnterToolStripMenuItem.Text = "Use Control + Enter To Send Instead Of Just Enter";
			this.controlEnterToolStripMenuItem.Click += new EventHandler(this.controlEnterToolStripMenuItem_Click);
			this.requireClickToMarkMessageReadToolStripMenuItem.CheckOnClick = true;
			this.requireClickToMarkMessageReadToolStripMenuItem.Name = "requireClickToMarkMessageReadToolStripMenuItem";
			this.requireClickToMarkMessageReadToolStripMenuItem.Size = new Size(332, 22);
			this.requireClickToMarkMessageReadToolStripMenuItem.Text = "Require Click to Mark Message Read";
			this.requireClickToMarkMessageReadToolStripMenuItem.Click += new EventHandler(this.requireClickToMarkMessageReadToolStripMenuItem_Click);
			this.openMessagesWindowWithUnreadReminderToolStripMenuItem.CheckOnClick = true;
			this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Name = "openMessagesWindowWithUnreadReminderToolStripMenuItem";
			this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Size = new Size(332, 22);
			this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Text = "Open Messages Window With Unread Reminder";
			this.openMessagesWindowWithUnreadReminderToolStripMenuItem.Click += new EventHandler(this.openMessagesWindowWithUnreadReminderToolStripMenuItem_Click);
			this.keepSelectedConversationInFocusToolStripMenuItem.CheckOnClick = true;
			this.keepSelectedConversationInFocusToolStripMenuItem.Name = "keepSelectedConversationInFocusToolStripMenuItem";
			this.keepSelectedConversationInFocusToolStripMenuItem.Size = new Size(332, 22);
			this.keepSelectedConversationInFocusToolStripMenuItem.Text = "Keep Selected Conversation in Focus";
			this.keepSelectedConversationInFocusToolStripMenuItem.Click += new EventHandler(this.keepSelectedConversationInFocusToolStripMenuItem_Click);
			this.displayMMSAttachmentsToolStripMenuItem.CheckOnClick = true;
			this.displayMMSAttachmentsToolStripMenuItem.Name = "displayMMSAttachmentsToolStripMenuItem";
			this.displayMMSAttachmentsToolStripMenuItem.Size = new Size(332, 22);
			this.displayMMSAttachmentsToolStripMenuItem.Text = "Display MMS Attachments";
			this.displayMMSAttachmentsToolStripMenuItem.Click += new EventHandler(this.displayMMSAttachmentsToolStripMenuItem_Click);
			this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.generalHelpToolStripMenuItem,
				this.settingsHelpToolStripMenuItem,
				this.toolStripSeparator3,
				this.exitToolStripMenuItem,
				this.syncFeaturesToolStripMenuItem,
				this.versionToolStripMenuItem,
				this.tryBETAToolStripMenuItem,
				this.logOutToolStripMenuItem
			});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
			this.generalHelpToolStripMenuItem.Size = new Size(146, 22);
			this.generalHelpToolStripMenuItem.Text = "General Help";
			this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
			this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
			this.settingsHelpToolStripMenuItem.Size = new Size(146, 22);
			this.settingsHelpToolStripMenuItem.Text = "Settings Help";
			this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(143, 6);
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new Size(146, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
			this.syncFeaturesToolStripMenuItem.Name = "syncFeaturesToolStripMenuItem";
			this.syncFeaturesToolStripMenuItem.Size = new Size(146, 22);
			this.syncFeaturesToolStripMenuItem.Text = "Sync Features";
			this.syncFeaturesToolStripMenuItem.Click += new EventHandler(this.syncFeaturesToolStripMenuItem_Click);
			this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
			this.versionToolStripMenuItem.Size = new Size(146, 22);
			this.versionToolStripMenuItem.Text = "version";
			this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
			this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
			this.tryBETAToolStripMenuItem.Size = new Size(146, 22);
			this.tryBETAToolStripMenuItem.Text = "Try BETA!";
			this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
			this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
			this.logOutToolStripMenuItem.Size = new Size(146, 22);
			this.logOutToolStripMenuItem.Text = "Log Out";
			this.logOutToolStripMenuItem.Click += new EventHandler(this.logOutToolStripMenuItem_Click);
			this.saveFileDialogPrintConversation.DefaultExt = "htm";
			this.saveFileDialogPrintConversation.Filter = "*.htm|*.html";
			this.contextMenuStripMessageText.ImageScalingSize = new Size(24, 24);
			this.contextMenuStripMessageText.Items.AddRange(new ToolStripItem[]
			{
				this.forwardTextMessage,
				this.copyTextToolStripMenuItem,
				this.copySelectedTextToolStripMenuItem,
				this.deleteToolStripMenuItem,
				this.markAsReadToolStripMenuItem
			});
			this.contextMenuStripMessageText.Name = "contextMenuStripMessageText";
			this.contextMenuStripMessageText.Size = new Size(174, 114);
			this.contextMenuStripMessageText.Opening += new CancelEventHandler(this.contextMenuStripMessageText_Opening);
			this.forwardTextMessage.Name = "forwardTextMessage";
			this.forwardTextMessage.Size = new Size(173, 22);
			this.forwardTextMessage.Text = "Forward";
			this.forwardTextMessage.Click += new EventHandler(this.forwardTextMessage_Click);
			this.copyTextToolStripMenuItem.Name = "copyTextToolStripMenuItem";
			this.copyTextToolStripMenuItem.Size = new Size(173, 22);
			this.copyTextToolStripMenuItem.Text = "Copy All Text";
			this.copyTextToolStripMenuItem.Click += new EventHandler(this.copyTextToolStripMenuItem_Click);
			this.copySelectedTextToolStripMenuItem.Name = "copySelectedTextToolStripMenuItem";
			this.copySelectedTextToolStripMenuItem.Size = new Size(173, 22);
			this.copySelectedTextToolStripMenuItem.Text = "Copy Selected Text";
			this.copySelectedTextToolStripMenuItem.Click += new EventHandler(this.copySelectedTextToolStripMenuItem_Click);
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new Size(173, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
			this.markAsReadToolStripMenuItem.Name = "markAsReadToolStripMenuItem";
			this.markAsReadToolStripMenuItem.Size = new Size(173, 22);
			this.markAsReadToolStripMenuItem.Text = "Mark as Read";
			this.markAsReadToolStripMenuItem.Visible = false;
			this.markAsReadToolStripMenuItem.Click += new EventHandler(this.markAsReadToolStripMenuItem_Click);
			this.labelProcessing.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.labelProcessing.AutoSize = true;
			this.labelProcessing.BackColor = Color.LimeGreen;
			this.labelProcessing.ForeColor = Color.Black;
			this.labelProcessing.Location = new Point(472, 5);
			this.labelProcessing.Name = "labelProcessing";
			this.labelProcessing.Size = new Size(101, 13);
			this.labelProcessing.TabIndex = 3;
			this.labelProcessing.Text = "Processing Enabled";
			this.labelProcessing.TextAlign = ContentAlignment.MiddleRight;
			this.contextMenuStripPictureBox.ImageScalingSize = new Size(24, 24);
			this.contextMenuStripPictureBox.Items.AddRange(new ToolStripItem[]
			{
				this.downloadToolStripMenuItem
			});
			this.contextMenuStripPictureBox.Name = "contextMenuStripPictureBox";
			this.contextMenuStripPictureBox.Size = new Size(129, 26);
			this.contextMenuStripPictureBox.Opening += new CancelEventHandler(this.contextMenuStripPictureBox_Opening);
			this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
			this.downloadToolStripMenuItem.Size = new Size(128, 22);
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
			this.rapidSpellAsYouTypeText.UnderlineColor = Color.Red;
			this.rapidSpellAsYouTypeText.UnderlineStyle = UnderlineStyle.Wavy;
			this.rapidSpellAsYouTypeText.UpdateAllTextBoxes = true;
			this.rapidSpellAsYouTypeText.UserDictionaryFile = null;
			this.rapidSpellAsYouTypeText.V2Parser = true;
			this.rapidSpellAsYouTypeText.WarnDuplicates = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(584, 561);
			base.Controls.Add(this.splitContainerMessages);
			base.Controls.Add(this.labelProcessing);
			base.Controls.Add(this.menuStripMessage);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
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
			((ISupportInitialize)this.pictureBoxConversationCountLock).EndInit();
			this.tableLayoutPanelMessages.ResumeLayout(false);
			this.pnSend.ResumeLayout(false);
			this.pnSend.PerformLayout();
			((ISupportInitialize)this.pictureBoxLink).EndInit();
			((ISupportInitialize)this.pictureBoxAttachment).EndInit();
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
	}
}

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
using System.Windows.Forms;
using TaskBarApp.Objects;
using TaskBarApp.Properties;

namespace TaskBarApp
{
	public class fmNewMessage : Form
	{
		private struct MessageRecipient
		{
			public string address
			{
				get;
				set;
			}

			public string contact
			{
				get;
				set;
			}
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			public static readonly fmNewMessage.<>c <>9 = new fmNewMessage.<>c();

			public static Func<fmNewMessage.MessageRecipient, string> <>9__15_0;

			internal string <comboBoxContactList_Load>b__15_0(fmNewMessage.MessageRecipient c)
			{
				return c.contact;
			}
		}

		private string strError = string.Empty;

		private bool bAddContactOnLeave;

		private bool bAddComboBoxContactList;

		private IContainer components;

		private Button buttonSend;

		private RichTextBox textBoxNewMessage;

		private Label lblCharCount;

		private ComboBox comboBoxContactList;

		private ListBox listBoxSelectedContacts;

		private Label labelClickRemove;

		private Label label1;

		private MenuStrip menuStripNewMessage;

		private ToolStripMenuItem editToolStripMenuItem;

		private ToolStripMenuItem messageTemplatesToolStripMenuItem;

		private ToolStripMenuItem editMessageTemplatesToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator1;

		private LinkLabel linkLabelRemoveAll;

		private ToolStripMenuItem manageGroupsToolStripMenuItem;

		private ToolStripMenuItem manageContactsToolStripMenuItem;

		private DateTimePicker dateTimePickerScheduleDate;

		private ToolStripMenuItem optionsToolStripMenuItem;

		private ToolStripMenuItem helpToolStripMenuItem;

		private ToolStripMenuItem generalHelpToolStripMenuItem;

		private ToolStripMenuItem settingsHelpToolStripMenuItem;

		private ToolStripMenuItem exitToolStripMenuItem;

		private ToolStripMenuItem settingsToolStripMenuItem1;

		private ToolStripSeparator toolStripSeparator2;

		private ToolStripMenuItem validatePhoneNumbersToolStripMenuItem;

		private ToolStripMenuItem controlEnterToolStripMenuItem;

		private Label labelCount;

		private ToolStripSeparator toolStripSeparator3;

		private ToolStripMenuItem versionToolStripMenuItem;

		private ToolStripMenuItem editKeywordAutoResponseToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator4;

		private ToolStripMenuItem editGroupScheduleToolStripMenuItem;

		private ToolStripMenuItem syncFeaturesToolStripMenuItem;

		private Label labelAttRemove;

		private PictureBox pictureBoxLink;

		private PictureBox pictureBoxAttachment;

		private OpenFileDialog openFileDialog;

		private RapidSpellAsYouType rapidSpellAsYouTypeNewMessage;

		private ToolStripMenuItem logOutToolStripMenuItem;

		private ToolStripMenuItem tryBETAToolStripMenuItem;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmNewMessage()
		{
			this.InitializeComponent();
		}

		private void fmNewMessage_Load(object sender, EventArgs e)
		{
			try
			{
				int num = 0;
				int num2 = 0;
				RegistryKey expr_0F = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.GetValue(expr_0F, "local_FormNewMessageWidth", ref num, ref this.strError);
				AppRegistry.GetValue(expr_0F, "local_FormNewMessageHeight", ref num2, ref this.strError);
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
				if (this.appManager.m_bEnableSignature && this.textBoxNewMessage.Text.Length == 0)
				{
					this.textBoxNewMessage.Text = "\r\n" + this.appManager.m_strSignature;
					this.textBoxNewMessage.Select(0, 0);
				}
			}
			catch (Exception ex)
			{
				this.strError = "Unexpected application error while loading New Message window: " + ex.Message;
			}
			if (this.strError.Length > 0)
			{
				this.appManager.ShowBalloon(this.strError, 5);
			}
		}

		private void fmNewMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				this.appManager.m_strForwardMessage = string.Empty;
				RegistryKey expr_1B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_1B, "local_FormNewMessageWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_1B, "local_FormNewMessageHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
			}
			catch
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

		private void buttonSend_Click(object sender, EventArgs e)
		{
			this.strError = null;
			bool flag = false;
			int count = this.listBoxSelectedContacts.Items.Count;
			string updateConversationFingerprint = string.Empty;
			if (this.listBoxSelectedContacts.Items.Count == 0)
			{
				this.buttonSend.Enabled = true;
				return;
			}
			if (this.textBoxNewMessage.Text.Length == 0 || this.textBoxNewMessage.Text.Trim() == this.appManager.m_strSignature.Trim())
			{
				MessageBox.Show("Please enter a message.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			try
			{
				this.buttonSend.Enabled = false;
				string text = string.Empty;
				string filePath = string.Empty;
				DateTime value = this.dateTimePickerScheduleDate.Value;
				if (count > 1)
				{
					if (this.appManager.m_bGroupSend)
					{
						this.appManager.ShowBalloon("Please wait until current group send is complete...", 5);
						return;
					}
					List<string> list = new List<string>();
					this.appManager.ShowBalloon("Your group message to " + count.ToString() + " recipients will be sent in the background...", 5);
					for (int i = 0; i < count; i++)
					{
						list.Add(((fmNewMessage.MessageRecipient)this.listBoxSelectedContacts.Items[i]).address);
					}
					if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
					{
						filePath = this.pictureBoxAttachment.ImageLocation;
					}
					this.appManager.SendGroupMessage(list, this.textBoxNewMessage.Text, value, filePath);
					this.buttonSend.Enabled = true;
					base.Close();
					if (this.appManager.formMessages == null)
					{
						this.appManager.ShowMessages();
					}
					return;
				}
				else
				{
					Predicate<Conversation> <>9__0;
					Predicate<ConversationMetaData> <>9__1;
					for (int j = 0; j < count; j++)
					{
						text = ((fmNewMessage.MessageRecipient)this.listBoxSelectedContacts.Items[j]).address;
						if (!string.IsNullOrEmpty(this.pictureBoxAttachment.ImageLocation))
						{
							IRestResponse<MMSSendResponse> restResponse = this.appManager.m_textService.SendMessageMMS(this.textBoxNewMessage.Text, this.appManager.FormatContactAddress(text, true, true), this.appManager.m_strSession, this.pictureBoxAttachment.ImageLocation);
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
							IRestResponse<TextMessageSendResponse> restResponse2 = this.appManager.m_textService.SendMessage(this.textBoxNewMessage.Text, text, this.appManager.m_strSession, value);
							if (!string.IsNullOrEmpty(restResponse2.ErrorMessage))
							{
								this.strError = "Error calling message/send for " + text + ": " + restResponse2.ErrorMessage;
							}
							else if (restResponse2.Data.success)
							{
								updateConversationFingerprint = restResponse2.Data.response.fingerprint;
							}
							else
							{
								this.strError = "Error from message/send for: " + text;
							}
						}
						if (string.IsNullOrEmpty(this.strError))
						{
							List<Conversation> arg_34E_0 = this.appManager.m_lsConversation;
							Predicate<Conversation> arg_34E_1;
							if ((arg_34E_1 = <>9__0) == null)
							{
								arg_34E_1 = (<>9__0 = ((Conversation p) => p.fingerprint == updateConversationFingerprint));
							}
							Conversation conversation = arg_34E_0.Find(arg_34E_1);
							if (conversation != null)
							{
								fmNewMessage.<>c__DisplayClass12_1 <>c__DisplayClass12_2 = new fmNewMessage.<>c__DisplayClass12_1();
								DateTime now = DateTime.Now;
								conversation.lastMessageDate = now.ToString("s");
								fmNewMessage.<>c__DisplayClass12_1 arg_3B0_0 = <>c__DisplayClass12_2;
								List<ConversationMetaData> arg_3AB_0 = this.appManager.m_lsConversationMetaData;
								Predicate<ConversationMetaData> arg_3AB_1;
								if ((arg_3AB_1 = <>9__1) == null)
								{
									arg_3AB_1 = (<>9__1 = ((ConversationMetaData var) => var.fingerprint == updateConversationFingerprint));
								}
								arg_3B0_0.updateConversationMetaData = arg_3AB_0.Find(arg_3AB_1);
								if (<>c__DisplayClass12_2.updateConversationMetaData != null)
								{
									<>c__DisplayClass12_2.updateConversationMetaData.lastMessageDirection = "Out";
									<>c__DisplayClass12_2.updateConversationMetaData.lastMessageDate = new DateTime?(now);
									<>c__DisplayClass12_2.updateConversationMetaData.lastMessageIsError = false;
									this.appManager.m_lsConversationMetaData = (from val in this.appManager.m_lsConversationMetaData
									where val.fingerprint != <>c__DisplayClass12_2.updateConversationMetaData.fingerprint
									select val).ToList<ConversationMetaData>();
									this.appManager.m_lsConversationMetaData.Add(<>c__DisplayClass12_2.updateConversationMetaData);
								}
							}
							else
							{
								flag = true;
							}
						}
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

		private void textBoxNewMessage_KeyPress(object sender, KeyPressEventArgs e)
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
		}

		private void textBoxNewMessage_TextChanged(object sender, EventArgs e)
		{
			this.lblCharCount.ForeColor = default(Color);
			int length = this.textBoxNewMessage.Text.Length;
			if (length == 250)
			{
				this.lblCharCount.ForeColor = Color.Red;
			}
			else if (length > 240)
			{
				this.lblCharCount.ForeColor = Color.Orange;
			}
			this.lblCharCount.Text = length.ToString() + "/250";
		}

		public void comboBoxContactList_Load(string match)
		{
			List<fmNewMessage.MessageRecipient> list = new List<fmNewMessage.MessageRecipient>();
			List<fmNewMessage.MessageRecipient> list2 = new List<fmNewMessage.MessageRecipient>();
			fmNewMessage.MessageRecipient messageRecipient = default(fmNewMessage.MessageRecipient);
			fmNewMessage.MessageRecipient item = default(fmNewMessage.MessageRecipient);
			if (match.Contains("#"))
			{
				messageRecipient.address = match;
			}
			else
			{
				messageRecipient.address = this.appManager.FormatContactAddress(match, false, false);
			}
			messageRecipient.contact = match;
			list.Add(messageRecipient);
			if (match.Contains("#"))
			{
				messageRecipient.address = match;
				foreach (string current in this.appManager.m_lsGroupTags)
				{
					item.contact = current;
					item.address = current;
					if (string.IsNullOrEmpty(match))
					{
						list2.Add(item);
					}
					else if (item.contact.ToLower().Contains(match.ToLower()))
					{
						list2.Add(item);
					}
				}
			}
			foreach (Contact current2 in this.appManager.m_lsContact)
			{
				messageRecipient = default(fmNewMessage.MessageRecipient);
				string text = current2.firstName + " " + current2.lastName;
				if (text.Trim().Length == 0)
				{
					text = "Unnamed";
				}
				else
				{
					text = text.Trim();
				}
				messageRecipient.contact = text + " " + this.appManager.FormatPhone(current2.mobileNumber);
				messageRecipient.address = current2.address;
				if (!this.listBoxSelectedContacts.Items.Contains(messageRecipient))
				{
					if (string.IsNullOrEmpty(match))
					{
						list2.Add(messageRecipient);
					}
					else
					{
						string text2 = this.appManager.FormatAlphaNumeric(match);
						if (messageRecipient.contact.ToLower().Contains(match.ToLower()))
						{
							list2.Add(messageRecipient);
						}
						else if (text2 != "" && messageRecipient.address.Contains(text2))
						{
							list2.Add(messageRecipient);
						}
					}
				}
			}
			IEnumerable<fmNewMessage.MessageRecipient> arg_237_0 = list2;
			Func<fmNewMessage.MessageRecipient, string> arg_237_1;
			if ((arg_237_1 = fmNewMessage.<>c.<>9__15_0) == null)
			{
				arg_237_1 = (fmNewMessage.<>c.<>9__15_0 = new Func<fmNewMessage.MessageRecipient, string>(fmNewMessage.<>c.<>9.<comboBoxContactList_Load>b__15_0));
			}
			list2 = arg_237_0.OrderBy(arg_237_1).ToList<fmNewMessage.MessageRecipient>();
			list.AddRange(list2);
			this.comboBoxContactList.DataSource = list;
		}

		private void comboBoxContactList_KeyPress(object sender, KeyPressEventArgs e)
		{
			string arg_19_0 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			string arg_0A_0 = string.Empty;
			if (arg_19_0.Contains(e.KeyChar.ToString()))
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

		private void comboBoxContactList_TextChanged(object sender, EventArgs e)
		{
			if (this.bAddComboBoxContactList)
			{
				this.comboBoxContactList_Load(this.comboBoxContactList.Text);
				this.comboBoxContactList.Select(this.comboBoxContactList.Text.Length + 1, 0);
				this.bAddComboBoxContactList = false;
			}
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
			if (!this.comboBoxContactList.DroppedDown || this.bAddContactOnLeave)
			{
				return;
			}
			this.listBoxSelectedContacts_Add();
		}

		private bool comboBoxContactList_Validate(string Text)
		{
			return this.appManager.IsValidMobileNumber(Text);
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

		private void listBoxSelectedContacts_Add()
		{
			if (this.comboBoxContactList.SelectedIndex < 0)
			{
				return;
			}
			fmNewMessage.MessageRecipient selectedItem = (fmNewMessage.MessageRecipient)this.comboBoxContactList.SelectedItem;
			if (selectedItem.address.Contains("#"))
			{
				foreach (GroupTagContact current in (from val in this.appManager.m_lsGroupTagContacts
				where val.groupTag == selectedItem.address
				select val).ToList<GroupTagContact>())
				{
					fmNewMessage.MessageRecipient messageRecipient = default(fmNewMessage.MessageRecipient);
					messageRecipient.contact = current.contact;
					messageRecipient.address = current.address;
					if (!this.listBoxSelectedContacts.Items.Contains(messageRecipient))
					{
						this.listBoxSelectedContacts.Items.Add(messageRecipient);
					}
				}
				this.comboBoxContactList_Load(string.Empty);
			}
			else
			{
				if (!this.comboBoxContactList_Validate(selectedItem.address))
				{
					MessageBox.Show("Please enter a 10 digit phone number or select a valid recipient from your existing contact list.  You may turn off Text Number validation in your Settings...", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}
				this.listBoxSelectedContacts.Items.Add(new fmNewMessage.MessageRecipient
				{
					address = this.appManager.FormatContactAddress(selectedItem.address, false, false),
					contact = selectedItem.contact
				});
				this.comboBoxContactList_Load(string.Empty);
			}
			this.labelCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
		}

		private void listBoxSelectedContacts_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 24;
		}

		private void listBoxSelectedContacts_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
			{
				return;
			}
			ListBox listBox = (ListBox)sender;
			fmNewMessage.MessageRecipient messageRecipient = (fmNewMessage.MessageRecipient)listBox.Items[e.Index];
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
			e.Graphics.DrawString(messageRecipient.contact, listBox.Font, Brushes.Black, (float)(e.Bounds.Left + 5), (float)(e.Bounds.Top + 4));
			e.Graphics.DrawRectangle(new Pen(Color.DimGray, width), e.Bounds);
			e.DrawFocusRectangle();
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

		private void linkLabelRemoveAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.listBoxSelectedContacts.Items.Clear();
			this.labelCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
			this.comboBoxContactList_Load(string.Empty);
			this.comboBoxContactList.Focus();
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowSettings();
		}

		private void manageGroupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditGroups();
		}

		private void manageContactToolStripMenuItem_Click(object sender, EventArgs e)
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

		private void messageTemplateMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripItem arg_0C_0 = (ToolStripDropDownItem)sender;
			string text = string.Empty;
			string name = arg_0C_0.Name;
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
								goto IL_21F;
							}
						}
					}
					else if (name == "10")
					{
						text = this.appManager.m_strMessageTemplate10;
						goto IL_21F;
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
								goto IL_21F;
							}
						}
					}
					else if (name == "7")
					{
						text = this.appManager.m_strMessageTemplate7;
						goto IL_21F;
					}
				}
				else if (name == "4")
				{
					text = this.appManager.m_strMessageTemplate4;
					goto IL_21F;
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
							goto IL_21F;
						}
					}
				}
				else if (name == "1")
				{
					text = this.appManager.m_strMessageTemplate1;
					goto IL_21F;
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
							goto IL_21F;
						}
					}
				}
				else if (name == "9")
				{
					text = this.appManager.m_strMessageTemplate9;
					goto IL_21F;
				}
			}
			else if (name == "2")
			{
				text = this.appManager.m_strMessageTemplate2;
				goto IL_21F;
			}
			this.appManager.ShowBalloon("Invalid message template selection", 5);
			IL_21F:
			this.textBoxNewMessage.Text = text;
			this.textBoxNewMessage.Select(this.textBoxNewMessage.Text.Length, 0);
		}

		private void editMessageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.appManager.ShowMessageTemplate();
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
			catch (Exception ex)
			{
				this.strError = this.strError + "Require 10 digit numbers save error: " + ex.Message;
				this.appManager.ShowBalloon(this.strError, 5);
			}
		}

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
			catch (Exception ex)
			{
				this.strError = this.strError + "Control Enter save error: " + ex.Message;
				this.appManager.ShowBalloon(this.strError, 5);
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
			this.validatePhoneNumbersToolStripMenuItem.Checked = this.appManager.m_bValidateMobileNumbers;
			this.controlEnterToolStripMenuItem.Checked = this.appManager.m_bControlEnter;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmNewMessage));
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
			((ISupportInitialize)this.pictureBoxLink).BeginInit();
			((ISupportInitialize)this.pictureBoxAttachment).BeginInit();
			base.SuspendLayout();
			this.buttonSend.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSend.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSend.Location = new Point(340, 372);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new Size(56, 27);
			this.buttonSend.TabIndex = 2;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			this.buttonSend.Click += new EventHandler(this.buttonSend_Click);
			this.textBoxNewMessage.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxNewMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxNewMessage.Location = new Point(12, 201);
			this.textBoxNewMessage.MaxLength = 250;
			this.textBoxNewMessage.Multiline = true;
			this.textBoxNewMessage.Name = "textBoxNewMessage";
			this.textBoxNewMessage.ScrollBars = RichTextBoxScrollBars.Vertical;
			this.textBoxNewMessage.Size = new Size(385, 141);
			this.textBoxNewMessage.TabIndex = 1;
			this.textBoxNewMessage.TextChanged += new EventHandler(this.textBoxNewMessage_TextChanged);
			this.textBoxNewMessage.KeyPress += new KeyPressEventHandler(this.textBoxNewMessage_KeyPress);
			this.lblCharCount.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.lblCharCount.AutoSize = true;
			this.lblCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblCharCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.lblCharCount.Location = new Point(279, 379);
			this.lblCharCount.MinimumSize = new Size(55, 0);
			this.lblCharCount.Name = "lblCharCount";
			this.lblCharCount.Size = new Size(55, 15);
			this.lblCharCount.TabIndex = 3;
			this.lblCharCount.Text = "0/250";
			this.lblCharCount.TextAlign = ContentAlignment.MiddleRight;
			this.comboBoxContactList.AllowDrop = true;
			this.comboBoxContactList.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxContactList.FormattingEnabled = true;
			this.comboBoxContactList.Location = new Point(12, 54);
			this.comboBoxContactList.Name = "comboBoxContactList";
			this.comboBoxContactList.Size = new Size(385, 25);
			this.comboBoxContactList.TabIndex = 0;
			this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
			this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
			this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
			this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
			this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
			this.listBoxSelectedContacts.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.listBoxSelectedContacts.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.listBoxSelectedContacts.FormattingEnabled = true;
			this.listBoxSelectedContacts.ItemHeight = 17;
			this.listBoxSelectedContacts.Location = new Point(12, 102);
			this.listBoxSelectedContacts.Name = "listBoxSelectedContacts";
			this.listBoxSelectedContacts.ScrollAlwaysVisible = true;
			this.listBoxSelectedContacts.Size = new Size(385, 89);
			this.listBoxSelectedContacts.Sorted = true;
			this.listBoxSelectedContacts.TabIndex = 5;
			this.listBoxSelectedContacts.TabStop = false;
			this.listBoxSelectedContacts.DoubleClick += new EventHandler(this.listBoxSelectedContacts_DoubleClick);
			this.labelClickRemove.AutoSize = true;
			this.labelClickRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelClickRemove.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelClickRemove.Location = new Point(12, 85);
			this.labelClickRemove.Name = "labelClickRemove";
			this.labelClickRemove.Size = new Size(180, 14);
			this.labelClickRemove.TabIndex = 6;
			this.labelClickRemove.Text = "Double-click to remove a recipient.";
			this.label1.AutoSize = true;
			this.label1.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.label1.ForeColor = Color.FromArgb(64, 64, 64);
			this.label1.Location = new Point(12, 34);
			this.label1.Name = "label1";
			this.label1.Size = new Size(305, 14);
			this.label1.TabIndex = 7;
			this.label1.Text = "Enter phone number, existing contact name, or group tag (#).";
			this.menuStripNewMessage.Items.AddRange(new ToolStripItem[]
			{
				this.messageTemplatesToolStripMenuItem,
				this.editToolStripMenuItem,
				this.optionsToolStripMenuItem,
				this.helpToolStripMenuItem
			});
			this.menuStripNewMessage.Location = new Point(0, 0);
			this.menuStripNewMessage.Name = "menuStripNewMessage";
			this.menuStripNewMessage.Size = new Size(409, 24);
			this.menuStripNewMessage.TabIndex = 8;
			this.menuStripNewMessage.Text = "menuStrip1";
			this.messageTemplatesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.editMessageTemplatesToolStripMenuItem,
				this.toolStripSeparator1
			});
			this.messageTemplatesToolStripMenuItem.Name = "messageTemplatesToolStripMenuItem";
			this.messageTemplatesToolStripMenuItem.Size = new Size(122, 20);
			this.messageTemplatesToolStripMenuItem.Text = "Message &Templates";
			this.editMessageTemplatesToolStripMenuItem.Name = "editMessageTemplatesToolStripMenuItem";
			this.editMessageTemplatesToolStripMenuItem.Size = new Size(209, 22);
			this.editMessageTemplatesToolStripMenuItem.Text = "&Edit Message Templates...";
			this.editMessageTemplatesToolStripMenuItem.Click += new EventHandler(this.editMessageTemplatesToolStripMenuItem_Click);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(206, 6);
			this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.manageGroupsToolStripMenuItem,
				this.manageContactsToolStripMenuItem,
				this.toolStripSeparator4,
				this.editKeywordAutoResponseToolStripMenuItem,
				this.editGroupScheduleToolStripMenuItem
			});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			this.manageGroupsToolStripMenuItem.Name = "manageGroupsToolStripMenuItem";
			this.manageGroupsToolStripMenuItem.Size = new Size(225, 22);
			this.manageGroupsToolStripMenuItem.Text = "Edit &Groups";
			this.manageGroupsToolStripMenuItem.Click += new EventHandler(this.manageGroupsToolStripMenuItem_Click);
			this.manageContactsToolStripMenuItem.Name = "manageContactsToolStripMenuItem";
			this.manageContactsToolStripMenuItem.Size = new Size(225, 22);
			this.manageContactsToolStripMenuItem.Text = "Edit &Contacts";
			this.manageContactsToolStripMenuItem.Click += new EventHandler(this.manageContactToolStripMenuItem_Click);
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new Size(222, 6);
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
				this.settingsToolStripMenuItem1,
				this.toolStripSeparator2,
				this.controlEnterToolStripMenuItem,
				this.validatePhoneNumbersToolStripMenuItem
			});
			this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
			this.optionsToolStripMenuItem.Size = new Size(61, 20);
			this.optionsToolStripMenuItem.Text = "&Options";
			this.optionsToolStripMenuItem.DropDownOpening += new EventHandler(this.optionsToolStripMenuItem_DropDownOpening);
			this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
			this.settingsToolStripMenuItem1.Size = new Size(332, 22);
			this.settingsToolStripMenuItem1.Text = "Settings";
			this.settingsToolStripMenuItem1.Click += new EventHandler(this.settingsToolStripMenuItem_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(329, 6);
			this.controlEnterToolStripMenuItem.CheckOnClick = true;
			this.controlEnterToolStripMenuItem.Name = "controlEnterToolStripMenuItem";
			this.controlEnterToolStripMenuItem.Size = new Size(332, 22);
			this.controlEnterToolStripMenuItem.Text = "Use Control + Enter To Send Instead Of Just Enter";
			this.controlEnterToolStripMenuItem.Click += new EventHandler(this.controlEnterToolStripMenuItem_Click);
			this.validatePhoneNumbersToolStripMenuItem.CheckOnClick = true;
			this.validatePhoneNumbersToolStripMenuItem.Name = "validatePhoneNumbersToolStripMenuItem";
			this.validatePhoneNumbersToolStripMenuItem.Size = new Size(332, 22);
			this.validatePhoneNumbersToolStripMenuItem.Text = "Require 10 Digit Phone Numbers";
			this.validatePhoneNumbersToolStripMenuItem.Click += new EventHandler(this.validatePhoneNumbersToolStripMenuItem_Click);
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
			this.helpToolStripMenuItem.Text = "Help";
			this.generalHelpToolStripMenuItem.Name = "generalHelpToolStripMenuItem";
			this.generalHelpToolStripMenuItem.Size = new Size(152, 22);
			this.generalHelpToolStripMenuItem.Text = "General Help";
			this.generalHelpToolStripMenuItem.Click += new EventHandler(this.generalHelpToolStripMenuItem_Click);
			this.settingsHelpToolStripMenuItem.Name = "settingsHelpToolStripMenuItem";
			this.settingsHelpToolStripMenuItem.Size = new Size(152, 22);
			this.settingsHelpToolStripMenuItem.Text = "Settings Help";
			this.settingsHelpToolStripMenuItem.Click += new EventHandler(this.settingsHelpToolStripMenuItem_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(149, 6);
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
			this.versionToolStripMenuItem.Text = "version";
			this.versionToolStripMenuItem.Click += new EventHandler(this.versionToolStripMenuItem_Click);
			this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
			this.logOutToolStripMenuItem.Size = new Size(152, 22);
			this.logOutToolStripMenuItem.Text = "Log Out";
			this.logOutToolStripMenuItem.Click += new EventHandler(this.logOutToolStripMenuItem_Click);
			this.linkLabelRemoveAll.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.linkLabelRemoveAll.AutoSize = true;
			this.linkLabelRemoveAll.LinkColor = Color.Olive;
			this.linkLabelRemoveAll.Location = new Point(335, 86);
			this.linkLabelRemoveAll.Name = "linkLabelRemoveAll";
			this.linkLabelRemoveAll.Size = new Size(61, 13);
			this.linkLabelRemoveAll.TabIndex = 9;
			this.linkLabelRemoveAll.TabStop = true;
			this.linkLabelRemoveAll.Text = "Remove All";
			this.linkLabelRemoveAll.VisitedLinkColor = Color.Olive;
			this.linkLabelRemoveAll.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabelRemoveAll_LinkClicked);
			this.dateTimePickerScheduleDate.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.dateTimePickerScheduleDate.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.dateTimePickerScheduleDate.Format = DateTimePickerFormat.Custom;
			this.dateTimePickerScheduleDate.Location = new Point(118, 375);
			this.dateTimePickerScheduleDate.MinDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
			this.dateTimePickerScheduleDate.Name = "dateTimePickerScheduleDate";
			this.dateTimePickerScheduleDate.Size = new Size(167, 22);
			this.dateTimePickerScheduleDate.TabIndex = 10;
			this.labelCount.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.labelCount.AutoSize = true;
			this.labelCount.Font = new Font("Arial", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelCount.Location = new Point(315, 85);
			this.labelCount.Name = "labelCount";
			this.labelCount.Size = new Size(13, 13);
			this.labelCount.TabIndex = 11;
			this.labelCount.Text = "0";
			this.labelCount.TextAlign = ContentAlignment.MiddleRight;
			this.labelAttRemove.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.labelAttRemove.AutoSize = true;
			this.labelAttRemove.BackColor = Color.Transparent;
			this.labelAttRemove.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.labelAttRemove.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelAttRemove.Location = new Point(9, 378);
			this.labelAttRemove.Name = "labelAttRemove";
			this.labelAttRemove.Size = new Size(84, 14);
			this.labelAttRemove.TabIndex = 14;
			this.labelAttRemove.Text = "Click to remove";
			this.labelAttRemove.Visible = false;
			this.labelAttRemove.Click += new EventHandler(this.labelAttRemove_Click);
			this.pictureBoxLink.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.pictureBoxLink.Image = Resources.Paperclip;
			this.pictureBoxLink.Location = new Point(92, 376);
			this.pictureBoxLink.Name = "pictureBoxLink";
			this.pictureBoxLink.Size = new Size(20, 20);
			this.pictureBoxLink.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBoxLink.TabIndex = 13;
			this.pictureBoxLink.TabStop = false;
			this.pictureBoxLink.Click += new EventHandler(this.pictureBoxLink_Click);
			this.pictureBoxAttachment.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.pictureBoxAttachment.Location = new Point(12, 350);
			this.pictureBoxAttachment.Name = "pictureBoxAttachment";
			this.pictureBoxAttachment.Size = new Size(73, 46);
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
			this.rapidSpellAsYouTypeNewMessage.UnderlineColor = Color.Red;
			this.rapidSpellAsYouTypeNewMessage.UnderlineStyle = UnderlineStyle.Wavy;
			this.rapidSpellAsYouTypeNewMessage.UpdateAllTextBoxes = true;
			this.rapidSpellAsYouTypeNewMessage.UserDictionaryFile = null;
			this.rapidSpellAsYouTypeNewMessage.V2Parser = true;
			this.rapidSpellAsYouTypeNewMessage.WarnDuplicates = true;
			this.tryBETAToolStripMenuItem.Name = "tryBETAToolStripMenuItem";
			this.tryBETAToolStripMenuItem.Size = new Size(152, 22);
			this.tryBETAToolStripMenuItem.Text = "Try BETA!";
			this.tryBETAToolStripMenuItem.Click += new EventHandler(this.tryBETAToolStripMenuItem_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(409, 411);
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
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MainMenuStrip = this.menuStripNewMessage;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new Size(425, 450);
			base.Name = "fmNewMessage";
			this.Text = "New Message";
			base.FormClosing += new FormClosingEventHandler(this.fmNewMessage_FormClosing);
			base.Load += new EventHandler(this.fmNewMessage_Load);
			this.menuStripNewMessage.ResumeLayout(false);
			this.menuStripNewMessage.PerformLayout();
			((ISupportInitialize)this.pictureBoxLink).EndInit();
			((ISupportInitialize)this.pictureBoxAttachment).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

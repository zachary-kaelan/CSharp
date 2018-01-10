using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TaskBarApp.Objects;

namespace TaskBarApp
{
	public class fmEditContacts : Form
	{
		private struct ContactItem
		{
			public long Id
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
			public static readonly fmEditContacts.<>c <>9 = new fmEditContacts.<>c();

			public static Func<fmEditContacts.ContactItem, string> <>9__17_0;

			public static Func<string, string> <>9__23_0;

			internal string <comboBoxContactList_Load>b__17_0(fmEditContacts.ContactItem c)
			{
				return c.contact;
			}

			internal string <comboBoxGroups_Load>b__23_0(string g)
			{
				return g;
			}
		}

		private string strError = string.Empty;

		private bool bNotAddedcomboBox;

		private bool bAutoAddRecipient;

		private bool bAddComboBoxGroups;

		private bool bAddGroupOnLeave;

		private IContainer components;

		private TextBox textBoxFirstName;

		private TextBox textBoxLastName;

		private Label labelFirstName;

		private Label labelLastName;

		private Label labelPhoneNumber;

		private Button buttonSave;

		private Label labelGroupTags;

		private Label labelContactAddress;

		private MaskedTextBox textBoxPhoneNumber;

		private ComboBox comboBoxContactList;

		private Button buttonClear;

		private Button buttonDelete;

		private Label labelContactID;

		private ComboBox comboBoxGroups;

		private Label labelGroupTagInstructions;

		private ListBox listBoxSelectedGroups;

		private Label labelContactSelect;

		private Label labelContactCount;

		private Button buttonBlock;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public bool bNewContact
		{
			get;
			set;
		}

		public fmEditContacts()
		{
			this.InitializeComponent();
		}

		private void fmEditContact_Load(object sender, EventArgs e)
		{
			this.Text = this.appManager.m_strApplicationName + " Edit Contacts " + this.appManager.FormatPhone(this.appManager.m_strUserName);
			base.Icon = this.appManager.iTextApp;
			int num = 0;
			int num2 = 0;
			RegistryKey expr_51 = AppRegistry.GetRootKey(ref this.strError);
			AppRegistry.GetValue(expr_51, "local_FormEditContactWidth", ref num, ref this.strError);
			AppRegistry.GetValue(expr_51, "local_FormEditContactHeight", ref num2, ref this.strError);
			if (num2 != 0)
			{
				base.Height = num2;
			}
			if (num != 0)
			{
				base.Width = num;
			}
			if (!this.appManager.m_bAllowDelete)
			{
				this.buttonDelete.Visible = false;
			}
			if (!this.appManager.m_bAllowBlock)
			{
				this.buttonBlock.Visible = false;
			}
			this.comboBoxContactList.ValueMember = "id";
			this.comboBoxContactList.DisplayMember = "contact";
			this.comboBoxContactList_Load(string.Empty);
			this.comboBoxGroups_Load(string.Empty);
			this.listBoxSelectedGroups.DrawMode = DrawMode.OwnerDrawVariable;
			this.listBoxSelectedGroups.MeasureItem += new MeasureItemEventHandler(this.listBoxSelectedGroups_MeasureItem);
			this.listBoxSelectedGroups.DrawItem += new DrawItemEventHandler(this.listBoxSelectedGroups_DrawItem);
			if (this.bNewContact)
			{
				this.textBoxPhoneNumber.Enabled = true;
				this.textBoxPhoneNumber.Select();
			}
			else
			{
				try
				{
					this.DisplayContact();
					this.textBoxFirstName.Select();
				}
				catch (Exception ex)
				{
					this.strError = "Error getting current contact information: " + ex.Message;
				}
			}
			if (!string.IsNullOrEmpty(this.strError))
			{
				this.appManager.ShowBalloon(this.strError, 5);
				this.strError = string.Empty;
			}
			if (this.appManager.m_bValidateMobileNumbers && this.bNewContact)
			{
				this.textBoxPhoneNumber.Mask = "(000) 000-0000";
				return;
			}
			this.textBoxPhoneNumber.Mask = null;
		}

		private void fmEditContact_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_0B, "local_FormEditContactWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_FormEditContactHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
			}
			catch
			{
			}
		}

		private void comboBoxContactList_Load(string match)
		{
			List<fmEditContacts.ContactItem> list = new List<fmEditContacts.ContactItem>();
			List<fmEditContacts.ContactItem> list2 = new List<fmEditContacts.ContactItem>();
			list2.Add(new fmEditContacts.ContactItem
			{
				Id = 0L,
				contact = match
			});
			foreach (Contact current in this.appManager.m_lsContact)
			{
				fmEditContacts.ContactItem item = default(fmEditContacts.ContactItem);
				string text = current.firstName + " " + current.lastName;
				if (text.Trim().Length == 0)
				{
					text = "Unnamed";
				}
				else
				{
					text = text.Trim();
				}
				item.Id = current.id;
				item.contact = text + " " + this.appManager.FormatPhone(current.mobileNumber);
				if (string.IsNullOrEmpty(match))
				{
					list.Add(item);
				}
				else
				{
					string text2 = this.appManager.FormatAlphaNumeric(match);
					if (item.contact.ToLower().Contains(match.ToLower()))
					{
						list.Add(item);
					}
					else if (text2 != "" && current.address.Contains(text2))
					{
						list.Add(item);
					}
				}
			}
			IEnumerable<fmEditContacts.ContactItem> arg_162_0 = list;
			Func<fmEditContacts.ContactItem, string> arg_162_1;
			if ((arg_162_1 = fmEditContacts.<>c.<>9__17_0) == null)
			{
				arg_162_1 = (fmEditContacts.<>c.<>9__17_0 = new Func<fmEditContacts.ContactItem, string>(fmEditContacts.<>c.<>9.<comboBoxContactList_Load>b__17_0));
			}
			list = arg_162_0.OrderBy(arg_162_1).ToList<fmEditContacts.ContactItem>();
			list2.AddRange(list);
			this.comboBoxContactList.DataSource = list2;
			this.labelContactCount.Text = list.Count.ToString() + " Contacts";
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
				this.bNotAddedcomboBox = true;
			}
			this.comboBoxContactList.DroppedDown = true;
		}

		private void comboBoxContactList_TextChanged(object sender, EventArgs e)
		{
			if (this.bNotAddedcomboBox)
			{
				this.comboBoxContactList_Load(this.comboBoxContactList.Text);
				this.comboBoxContactList.Select(this.comboBoxContactList.Text.Length + 1, 0);
				this.bNotAddedcomboBox = false;
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
			if (!this.comboBoxContactList.DroppedDown || this.bAutoAddRecipient)
			{
				return;
			}
			if (this.comboBoxContactList.SelectedIndex <= 0)
			{
				return;
			}
			this.DisplayContact();
		}

		private void comboBoxContactList_Leave(object sender, EventArgs e)
		{
			this.bAutoAddRecipient = true;
			if (this.comboBoxContactList.SelectedIndex > 0)
			{
				this.DisplayContact();
			}
			this.bAutoAddRecipient = false;
		}

		public void comboBoxGroups_Load(string match)
		{
			List<string> list = new List<string>();
			list.Add(match);
			foreach (string current in this.appManager.m_lsGroupTags)
			{
				if (!this.listBoxSelectedGroups.Items.Contains(current))
				{
					if (string.IsNullOrEmpty(match))
					{
						list.Add(current);
					}
					else if (current.ToLower().Contains(match.ToLower()))
					{
						list.Add(current);
					}
				}
			}
			IEnumerable<string> arg_9F_0 = list;
			Func<string, string> arg_9F_1;
			if ((arg_9F_1 = fmEditContacts.<>c.<>9__23_0) == null)
			{
				arg_9F_1 = (fmEditContacts.<>c.<>9__23_0 = new Func<string, string>(fmEditContacts.<>c.<>9.<comboBoxGroups_Load>b__23_0));
			}
			list = arg_9F_0.OrderBy(arg_9F_1).ToList<string>();
			this.comboBoxGroups.DataSource = list;
		}

		private void comboBoxGroups_KeyPress(object sender, KeyPressEventArgs e)
		{
			string arg_19_0 = "#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			string arg_0A_0 = string.Empty;
			if (arg_19_0.Contains(e.KeyChar.ToString()))
			{
				this.comboBoxGroups_Load(this.comboBoxGroups.Text + e.KeyChar.ToString());
				this.comboBoxGroups.Select(this.comboBoxGroups.Text.Length + 1, 0);
				e.Handled = true;
			}
			else if (e.KeyChar == '\b')
			{
				if (this.comboBoxGroups.Text.Length > 0)
				{
					this.comboBoxGroups_Load(this.comboBoxGroups.Text.Substring(0, this.comboBoxGroups.Text.Length - 1));
					this.comboBoxGroups.Select(this.comboBoxGroups.Text.Length + 1, 0);
				}
				e.Handled = true;
			}
			else
			{
				e.Handled = false;
				this.bAddComboBoxGroups = true;
			}
			this.comboBoxGroups.DroppedDown = true;
		}

		private void comboBoxGroups_TextChanged(object sender, EventArgs e)
		{
			if (this.bAddComboBoxGroups)
			{
				this.comboBoxGroups_Load(this.comboBoxGroups.Text);
				this.comboBoxGroups.Select(this.comboBoxGroups.Text.Length + 1, 0);
				this.bAddComboBoxGroups = false;
			}
		}

		private void comboBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.comboBoxGroups.Text.Length > 0)
			{
				this.comboBoxGroups.DroppedDown = true;
			}
		}

		private void comboBoxGroups_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (!this.comboBoxGroups.DroppedDown || this.bAddGroupOnLeave)
			{
				return;
			}
			this.listBoxSelectedGroups_Add();
		}

		private void comboBoxGroups_Leave(object sender, EventArgs e)
		{
			this.bAddGroupOnLeave = true;
			this.listBoxSelectedGroups_Add();
			this.bAddGroupOnLeave = false;
		}

		private void listBoxSelectedGroups_Add()
		{
			if (this.comboBoxGroups.SelectedIndex < 0)
			{
				return;
			}
			if (this.comboBoxGroups.SelectedItem.ToString() == "")
			{
				return;
			}
			string item = this.comboBoxGroups.SelectedItem.ToString();
			if (this.appManager.m_lsGroupTags.Contains(item))
			{
				this.listBoxSelectedGroups.Items.Add(item);
			}
			this.comboBoxGroups_Load(string.Empty);
		}

		private void listBoxSelectedGroups_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight = 24;
		}

		private void listBoxSelectedGroups_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
			{
				return;
			}
			ListBox listBox = (ListBox)sender;
			string s = (string)listBox.Items[e.Index];
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
			e.Graphics.DrawString(s, listBox.Font, Brushes.Black, (float)(e.Bounds.Left + 5), (float)(e.Bounds.Top + 4));
			e.Graphics.DrawRectangle(new Pen(Color.DimGray, width), e.Bounds);
			e.DrawFocusRectangle();
		}

		private void listBoxSelectedGroups_DoubleClick(object sender, EventArgs e)
		{
			if (this.listBoxSelectedGroups.Items.Count == 0)
			{
				return;
			}
			string value = this.listBoxSelectedGroups.SelectedItem.ToString();
			this.listBoxSelectedGroups.Items.Remove(value);
			this.comboBoxGroups_Load(string.Empty);
		}

		public void DisplayContact()
		{
			long num = 0L;
			this.textBoxPhoneNumber.Mask = null;
			if (this.comboBoxContactList.SelectedIndex > 0)
			{
				num = ((fmEditContacts.ContactItem)this.comboBoxContactList.SelectedItem).Id;
			}
			Contact contactByID;
			if (num != 0L)
			{
				contactByID = this.appManager.GetContactByID(num);
			}
			else
			{
				contactByID = this.appManager.GetContactByID(this.appManager.m_nCurrentContactID);
			}
			if (contactByID != null)
			{
				this.textBoxPhoneNumber.Text = this.appManager.FormatPhone(contactByID.mobileNumber);
				this.textBoxFirstName.Text = contactByID.firstName;
				this.textBoxLastName.Text = contactByID.lastName;
				this.labelContactAddress.Text = contactByID.address;
				this.labelContactID.Text = contactByID.id.ToString();
				this.listBoxSelectedGroups.Items.Clear();
				using (List<string>.Enumerator enumerator = this.appManager.GetContactGroupTags(contactByID.notes).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						this.listBoxSelectedGroups.Items.Add(current);
					}
					goto IL_1A8;
				}
			}
			this.textBoxFirstName.Text = string.Empty;
			this.textBoxLastName.Text = string.Empty;
			this.labelContactID.Text = string.Empty;
			this.textBoxPhoneNumber.Text = this.appManager.FormatPhone(this.appManager.m_strCurrentContactAddress);
			this.labelContactAddress.Text = this.appManager.m_strCurrentContactAddress;
			this.listBoxSelectedGroups.Items.Clear();
			IL_1A8:
			this.comboBoxContactList_Load(string.Empty);
			this.comboBoxGroups_Load(string.Empty);
			this.textBoxPhoneNumber.Enabled = false;
			this.bNewContact = false;
			if (this.labelContactID.Text.Length > 0)
			{
				this.buttonDelete.Enabled = true;
			}
		}

		private void ResetDisplay()
		{
			this.textBoxPhoneNumber.Text = string.Empty;
			if (this.appManager.m_bValidateMobileNumbers)
			{
				this.textBoxPhoneNumber.Mask = "(000) 000-0000";
			}
			else
			{
				this.textBoxPhoneNumber.Mask = null;
			}
			this.textBoxFirstName.Text = string.Empty;
			this.textBoxLastName.Text = string.Empty;
			this.listBoxSelectedGroups.Items.Clear();
			this.labelContactAddress.Text = string.Empty;
			this.labelContactID.Text = string.Empty;
			this.comboBoxContactList_Load(string.Empty);
			this.comboBoxGroups_Load(string.Empty);
			this.buttonDelete.Enabled = false;
			this.textBoxPhoneNumber.Enabled = true;
			this.comboBoxContactList.Focus();
			this.bNewContact = true;
		}

		private void SaveContact(bool bShowSaveBaloon = true)
		{
			this.textBoxPhoneNumber.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
			string text = string.Empty;
			long nSavedContactID = 0L;
			if (this.textBoxPhoneNumber.Text.Length == 0)
			{
				return;
			}
			try
			{
				if (this.bNewContact)
				{
					if (!this.appManager.IsDigitsOnly(this.textBoxPhoneNumber.Text))
					{
						this.strError = "Phone number must be numeric digits...";
						this.textBoxPhoneNumber.Select();
					}
					if (!this.appManager.IsValidMobileNumber(this.appManager.FormatContactAddress(this.textBoxPhoneNumber.Text, true, false)))
					{
						this.strError = "Phone number is not a valid number of digits...";
						this.textBoxPhoneNumber.Select();
					}
					this.labelContactAddress.Text = this.appManager.FormatContactAddress(this.textBoxPhoneNumber.Text, false, false);
				}
				if (string.IsNullOrEmpty(this.strError))
				{
					foreach (string str in this.listBoxSelectedGroups.Items)
					{
						text += str;
					}
					IRestResponse<ContactSaveResponse> restResponse = this.appManager.m_textService.SaveContact(this.labelContactAddress.Text, this.appManager.m_strSession, this.textBoxFirstName.Text, this.textBoxLastName.Text, text);
					if (!restResponse.Data.success)
					{
						this.strError = "Error calling contact/save...";
					}
					else
					{
						nSavedContactID = restResponse.Data.response.id;
					}
				}
			}
			catch (Exception ex)
			{
				this.strError = "Error saving contact: " + ex.Message;
			}
			if (!string.IsNullOrEmpty(this.strError))
			{
				MessageBox.Show(this.strError, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.strError = string.Empty;
				this.buttonSave.Enabled = true;
				return;
			}
			Contact contact = this.appManager.m_lsContact.Find((Contact var) => var.id == nSavedContactID);
			if (contact != null)
			{
				contact.lastName = this.textBoxLastName.Text;
				contact.firstName = this.textBoxFirstName.Text;
				contact.notes = text;
				using (IEnumerator enumerator = this.listBoxSelectedGroups.Items.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string groupTag = (string)enumerator.Current;
						this.appManager.AddGroupTag(nSavedContactID, groupTag);
					}
					goto IL_292;
				}
			}
			this.appManager.LoadUpdates(false);
			IL_292:
			if (this.appManager.formMessages != null)
			{
				this.appManager.formMessages.DisplayConversationHeader();
				this.appManager.formMessages.DisplayConversatoinList();
				this.appManager.ShowEditContact(true);
			}
			if (bShowSaveBaloon)
			{
				this.appManager.ShowBalloon("Contact saved", 5);
			}
			this.ResetDisplay();
		}

		private void linkLabelEditGroups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.appManager.ShowEditGroups();
		}

		private void textBoxPhoneNumbere_Click(object sender, EventArgs e)
		{
			this.PositionCursorInMaskedTextBox(this.textBoxPhoneNumber);
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			this.buttonSave.Enabled = false;
			this.SaveContact(true);
			this.buttonSave.Enabled = true;
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			this.ResetDisplay();
			this.textBoxPhoneNumber.Focus();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			bool flag = false;
			if (this.labelContactID.Text.Length > 0)
			{
				this.buttonDelete.Enabled = false;
				string text = string.Empty;
				long contactID = Convert.ToInt64(this.labelContactID.Text);
				try
				{
					if (MessageBox.Show(string.Concat(new string[]
					{
						"Delete contact ",
						this.textBoxFirstName.Text,
						" ",
						this.textBoxLastName.Text,
						"?"
					}), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					{
						this.buttonDelete.Enabled = true;
						return;
					}
					if (MessageBox.Show(string.Concat(new string[]
					{
						"Delete any conversations associated with ",
						this.textBoxFirstName.Text,
						" ",
						this.textBoxLastName.Text,
						"?\n\n Please Note this cannot be undone!"
					}), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						flag = true;
					}
					this.appManager.RemoveGroupTag(contactID, "all");
					if (this.appManager.m_textService.DeleteContact(contactID, this.appManager.m_strSession).Data.success)
					{
						if (flag && this.appManager.m_lsConversation != null)
						{
							Conversation conversation = this.appManager.m_lsConversation.Find((Conversation var) => var.lastContactId == contactID);
							if (conversation != null)
							{
								text = conversation.fingerprint;
								if (!this.appManager.m_textService.ConversationDelete(text, this.appManager.m_strSession).Data.success)
								{
									this.strError = "Error calling conversation/delete...";
								}
								else
								{
									this.appManager.m_lsConversation.Remove(conversation);
								}
							}
						}
					}
					else
					{
						this.strError = "Error calling contact/delete...";
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
					this.buttonSave.Enabled = true;
				}
				else
				{
					string text2 = string.Empty;
					this.appManager.m_lsContact = (from val in this.appManager.m_lsContact
					where val.id != contactID
					select val).ToList<Contact>();
					this.appManager.m_strCurrentContactAddress = string.Empty;
					this.appManager.m_nCurrentContactID = 0L;
					this.appManager.m_strCurentConversationFingerprint = string.Empty;
					if (flag)
					{
						text2 = "Conversation and Contact deleted";
					}
					else
					{
						text2 = "Contact deleted";
					}
					this.appManager.ShowBalloon(text2, 5);
					if (this.appManager.formMessages != null)
					{
						this.appManager.formMessages.DisplayConversatoinList();
						this.appManager.formMessages.DisplayConversationHeader();
						if (this.appManager.m_strCurentConversationFingerprint == text)
						{
							this.appManager.formMessages.ResetMessageForm(null);
						}
						this.appManager.ShowEditContact(true);
					}
				}
				this.buttonDelete.Enabled = true;
				this.ResetDisplay();
			}
		}

		private void buttonBlock_Click(object sender, EventArgs e)
		{
			if (this.labelContactID.Text.Length > 0)
			{
				long contactID = Convert.ToInt64(this.labelContactID.Text);
				try
				{
					if (MessageBox.Show(string.Concat(new string[]
					{
						"Are you sure you want to block ",
						this.textBoxFirstName.Text,
						" ",
						this.textBoxLastName.Text,
						" ",
						this.textBoxPhoneNumber.Text,
						" from ever sending texts to your number in the future?\n\n Please Note this cannot be undone!"
					}), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
					{
						return;
					}
					if (!this.appManager.m_textService.BlockContact(this.appManager.FormatAlphaNumeric(this.textBoxPhoneNumber.Text), this.appManager.m_strSession).Data.success)
					{
						this.strError = "Error calling contact/block...";
					}
				}
				catch (Exception ex)
				{
					this.strError = "Error blocking number: " + ex.Message;
				}
				if (!string.IsNullOrEmpty(this.strError))
				{
					this.appManager.ShowBalloon(this.strError, 5);
					this.strError = string.Empty;
					return;
				}
				this.appManager.RemoveGroupTag(contactID, "all");
				this.textBoxFirstName.Text = "BLOCKED" + this.textBoxFirstName.Text;
				this.SaveContact(false);
				this.appManager.ShowBalloon("The number has been permanently blocked.", 5);
				this.ResetDisplay();
			}
		}

		private void PositionCursorInMaskedTextBox(MaskedTextBox mtb)
		{
			if (mtb == null)
			{
				return;
			}
			mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
			if (mtb.Text.Length <= 3)
			{
				mtb.Select(mtb.Text.Length + 1, 0);
			}
			if (mtb.Text.Length > 3 && mtb.Text.Length <= 6)
			{
				mtb.Select(mtb.Text.Length + 3, 0);
			}
			if (mtb.Text.Length > 6)
			{
				mtb.Select(mtb.Text.Length + 4, 0);
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmEditContacts));
			this.textBoxFirstName = new TextBox();
			this.textBoxLastName = new TextBox();
			this.labelFirstName = new Label();
			this.labelLastName = new Label();
			this.labelPhoneNumber = new Label();
			this.buttonSave = new Button();
			this.labelGroupTags = new Label();
			this.labelContactAddress = new Label();
			this.textBoxPhoneNumber = new MaskedTextBox();
			this.comboBoxContactList = new ComboBox();
			this.buttonClear = new Button();
			this.buttonDelete = new Button();
			this.labelContactID = new Label();
			this.comboBoxGroups = new ComboBox();
			this.labelGroupTagInstructions = new Label();
			this.listBoxSelectedGroups = new ListBox();
			this.labelContactSelect = new Label();
			this.labelContactCount = new Label();
			this.buttonBlock = new Button();
			base.SuspendLayout();
			this.textBoxFirstName.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxFirstName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxFirstName.Location = new Point(131, 131);
			this.textBoxFirstName.Margin = new Padding(4);
			this.textBoxFirstName.Name = "textBoxFirstName";
			this.textBoxFirstName.Size = new Size(188, 25);
			this.textBoxFirstName.TabIndex = 3;
			this.textBoxLastName.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxLastName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxLastName.Location = new Point(131, 179);
			this.textBoxLastName.Margin = new Padding(4);
			this.textBoxLastName.Name = "textBoxLastName";
			this.textBoxLastName.Size = new Size(188, 25);
			this.textBoxLastName.TabIndex = 4;
			this.labelFirstName.AutoSize = true;
			this.labelFirstName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelFirstName.Location = new Point(14, 134);
			this.labelFirstName.Margin = new Padding(4, 0, 4, 0);
			this.labelFirstName.Name = "labelFirstName";
			this.labelFirstName.Size = new Size(110, 17);
			this.labelFirstName.TabIndex = 2;
			this.labelFirstName.Text = "Contact Field 1:";
			this.labelLastName.AutoSize = true;
			this.labelLastName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelLastName.Location = new Point(16, 183);
			this.labelLastName.Margin = new Padding(4, 0, 4, 0);
			this.labelLastName.Name = "labelLastName";
			this.labelLastName.Size = new Size(110, 17);
			this.labelLastName.TabIndex = 3;
			this.labelLastName.Text = "Contact Field 2:";
			this.labelPhoneNumber.AutoSize = true;
			this.labelPhoneNumber.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelPhoneNumber.Location = new Point(16, 87);
			this.labelPhoneNumber.Margin = new Padding(4, 0, 4, 0);
			this.labelPhoneNumber.Name = "labelPhoneNumber";
			this.labelPhoneNumber.Size = new Size(94, 17);
			this.labelPhoneNumber.TabIndex = 4;
			this.labelPhoneNumber.Text = "Text Number:";
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(262, 429);
			this.buttonSave.Margin = new Padding(4);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(56, 27);
			this.buttonSave.TabIndex = 10;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.labelGroupTags.AutoSize = true;
			this.labelGroupTags.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelGroupTags.Location = new Point(16, 220);
			this.labelGroupTags.Margin = new Padding(4, 0, 4, 0);
			this.labelGroupTags.Name = "labelGroupTags";
			this.labelGroupTags.Size = new Size(87, 17);
			this.labelGroupTags.TabIndex = 7;
			this.labelGroupTags.Text = "Group Tags:";
			this.labelContactAddress.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.labelContactAddress.AutoSize = true;
			this.labelContactAddress.Location = new Point(160, 434);
			this.labelContactAddress.Margin = new Padding(4, 0, 4, 0);
			this.labelContactAddress.Name = "labelContactAddress";
			this.labelContactAddress.Size = new Size(156, 17);
			this.labelContactAddress.TabIndex = 8;
			this.labelContactAddress.Text = "hiddenContactAddress";
			this.labelContactAddress.Visible = false;
			this.textBoxPhoneNumber.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxPhoneNumber.Location = new Point(131, 84);
			this.textBoxPhoneNumber.Mask = "(000) 000-0000";
			this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
			this.textBoxPhoneNumber.Size = new Size(187, 25);
			this.textBoxPhoneNumber.TabIndex = 2;
			this.textBoxPhoneNumber.Click += new EventHandler(this.textBoxPhoneNumbere_Click);
			this.comboBoxContactList.AllowDrop = true;
			this.comboBoxContactList.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxContactList.FormattingEnabled = true;
			this.comboBoxContactList.Location = new Point(16, 16);
			this.comboBoxContactList.Name = "comboBoxContactList";
			this.comboBoxContactList.Size = new Size(302, 25);
			this.comboBoxContactList.TabIndex = 1;
			this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
			this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
			this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
			this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
			this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
			this.buttonClear.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonClear.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonClear.Location = new Point(19, 429);
			this.buttonClear.Margin = new Padding(4);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new Size(60, 27);
			this.buttonClear.TabIndex = 13;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new EventHandler(this.buttonClear_Click);
			this.buttonDelete.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonDelete.Location = new Point(112, 429);
			this.buttonDelete.Margin = new Padding(4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new Size(67, 27);
			this.buttonDelete.TabIndex = 11;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
			this.labelContactID.AutoSize = true;
			this.labelContactID.Location = new Point(16, 92);
			this.labelContactID.Margin = new Padding(4, 0, 4, 0);
			this.labelContactID.Name = "labelContactID";
			this.labelContactID.Size = new Size(0, 17);
			this.labelContactID.TabIndex = 14;
			this.labelContactID.Visible = false;
			this.comboBoxGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxGroups.FormattingEnabled = true;
			this.comboBoxGroups.Location = new Point(17, 244);
			this.comboBoxGroups.Name = "comboBoxGroups";
			this.comboBoxGroups.Size = new Size(299, 25);
			this.comboBoxGroups.TabIndex = 5;
			this.comboBoxGroups.SelectedIndexChanged += new EventHandler(this.comboBoxGroups_SelectedIndexChanged);
			this.comboBoxGroups.SelectionChangeCommitted += new EventHandler(this.comboBoxGroups_SelectionChangeCommitted);
			this.comboBoxGroups.TextChanged += new EventHandler(this.comboBoxGroups_TextChanged);
			this.comboBoxGroups.KeyPress += new KeyPressEventHandler(this.comboBoxGroups_KeyPress);
			this.comboBoxGroups.Leave += new EventHandler(this.comboBoxGroups_Leave);
			this.labelGroupTagInstructions.AutoSize = true;
			this.labelGroupTagInstructions.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelGroupTagInstructions.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelGroupTagInstructions.Location = new Point(15, 272);
			this.labelGroupTagInstructions.MaximumSize = new Size(310, 0);
			this.labelGroupTagInstructions.Name = "labelGroupTagInstructions";
			this.labelGroupTagInstructions.Size = new Size(305, 28);
			this.labelGroupTagInstructions.TabIndex = 19;
			this.labelGroupTagInstructions.Text = "Select a group to add the contact to the group.  Double-click a group to remove the contact from the group.";
			this.listBoxSelectedGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.listBoxSelectedGroups.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.listBoxSelectedGroups.FormattingEnabled = true;
			this.listBoxSelectedGroups.ItemHeight = 17;
			this.listBoxSelectedGroups.Location = new Point(17, 312);
			this.listBoxSelectedGroups.Name = "listBoxSelectedGroups";
			this.listBoxSelectedGroups.ScrollAlwaysVisible = true;
			this.listBoxSelectedGroups.Size = new Size(299, 106);
			this.listBoxSelectedGroups.Sorted = true;
			this.listBoxSelectedGroups.TabIndex = 20;
			this.listBoxSelectedGroups.TabStop = false;
			this.listBoxSelectedGroups.DoubleClick += new EventHandler(this.listBoxSelectedGroups_DoubleClick);
			this.labelContactSelect.AutoSize = true;
			this.labelContactSelect.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelContactSelect.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelContactSelect.Location = new Point(21, 44);
			this.labelContactSelect.MaximumSize = new Size(310, 0);
			this.labelContactSelect.Name = "labelContactSelect";
			this.labelContactSelect.Size = new Size(297, 28);
			this.labelContactSelect.TabIndex = 21;
			this.labelContactSelect.Text = "Select a contact to edit.  Click the Clear button to reset the form and add a new contact.";
			this.labelContactCount.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.labelContactCount.AutoSize = true;
			this.labelContactCount.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelContactCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelContactCount.Location = new Point(256, 58);
			this.labelContactCount.MaximumSize = new Size(310, 0);
			this.labelContactCount.Name = "labelContactCount";
			this.labelContactCount.Size = new Size(60, 14);
			this.labelContactCount.TabIndex = 22;
			this.labelContactCount.Text = "0 Contacts";
			this.labelContactCount.TextAlign = ContentAlignment.TopRight;
			this.buttonBlock.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonBlock.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonBlock.Location = new Point(187, 429);
			this.buttonBlock.Margin = new Padding(4);
			this.buttonBlock.Name = "buttonBlock";
			this.buttonBlock.Size = new Size(67, 27);
			this.buttonBlock.TabIndex = 23;
			this.buttonBlock.Text = "Block";
			this.buttonBlock.UseVisualStyleBackColor = true;
			this.buttonBlock.Click += new EventHandler(this.buttonBlock_Click);
			base.AutoScaleDimensions = new SizeF(8f, 17f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(334, 469);
			base.Controls.Add(this.buttonBlock);
			base.Controls.Add(this.labelContactCount);
			base.Controls.Add(this.labelContactSelect);
			base.Controls.Add(this.listBoxSelectedGroups);
			base.Controls.Add(this.labelGroupTagInstructions);
			base.Controls.Add(this.comboBoxGroups);
			base.Controls.Add(this.labelContactID);
			base.Controls.Add(this.buttonDelete);
			base.Controls.Add(this.buttonClear);
			base.Controls.Add(this.comboBoxContactList);
			base.Controls.Add(this.textBoxPhoneNumber);
			base.Controls.Add(this.labelGroupTags);
			base.Controls.Add(this.buttonSave);
			base.Controls.Add(this.labelPhoneNumber);
			base.Controls.Add(this.labelLastName);
			base.Controls.Add(this.labelFirstName);
			base.Controls.Add(this.textBoxLastName);
			base.Controls.Add(this.textBoxFirstName);
			base.Controls.Add(this.labelContactAddress);
			this.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new Padding(4);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new Size(350, 420);
			base.Name = "fmEditContacts";
			this.Text = "Edit Contacts";
			base.FormClosing += new FormClosingEventHandler(this.fmEditContact_FormClosing);
			base.Load += new EventHandler(this.fmEditContact_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

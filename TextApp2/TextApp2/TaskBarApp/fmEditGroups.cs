using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using TaskBarApp.Objects;

namespace TaskBarApp
{
	public class fmEditGroups : Form
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
			public static readonly fmEditGroups.<>c <>9 = new fmEditGroups.<>c();

			public static Func<string, string> <>9__13_0;

			public static Func<fmEditGroups.ContactItem, string> <>9__22_0;

			internal string <comboBoxGroups_Load>b__13_0(string g)
			{
				return g;
			}

			internal string <comboBoxContactList_Load>b__22_0(fmEditGroups.ContactItem c)
			{
				return c.contact;
			}
		}

		private string strError = string.Empty;

		private bool bAddComboBoxContactList;

		private bool bAddContactOnLeave;

		private bool bAddComboBoxGroups;

		private bool bAddGroupOnLeave;

		private IContainer components;

		private ComboBox comboBoxContactList;

		private Button buttonNew;

		private Button buttonDelete;

		private ComboBox comboBoxGroups;

		private Label labelGroupTag;

		private Label labelGroupTagInstructions;

		private ListBox listBoxSelectedContacts;

		private Label labelGroupTagInstruction;

		private Button buttonSave;

		private Label textBoxGroupTag;

		private Label labelContactCount;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmEditGroups()
		{
			this.InitializeComponent();
		}

		private void fmEditGroups_Load(object sender, EventArgs e)
		{
			this.Text = this.appManager.m_strApplicationName + " Edit Groups " + this.appManager.FormatPhone(this.appManager.m_strUserName);
			base.Icon = this.appManager.iTextApp;
			int num = 0;
			int num2 = 0;
			RegistryKey expr_51 = AppRegistry.GetRootKey(ref this.strError);
			AppRegistry.GetValue(expr_51, "local_FormEditGroupsWidth", ref num, ref this.strError);
			AppRegistry.GetValue(expr_51, "local_FormEditGroupsHeight", ref num2, ref this.strError);
			if (num2 != 0)
			{
				base.Height = num2;
			}
			if (num != 0)
			{
				base.Width = num;
			}
			this.textBoxGroupTag.Text = "";
			this.comboBoxContactList.ValueMember = "id";
			this.comboBoxContactList.DisplayMember = "contact";
			this.comboBoxContactList_Load(string.Empty);
			this.comboBoxGroups_Load(string.Empty);
			this.listBoxSelectedContacts.DrawMode = DrawMode.OwnerDrawVariable;
			this.listBoxSelectedContacts.MeasureItem += new MeasureItemEventHandler(this.listBoxSelectedContacts_MeasureItem);
			this.listBoxSelectedContacts.DrawItem += new DrawItemEventHandler(this.listBoxSelectedContacts_DrawItem);
			this.listBoxSelectedContacts.ValueMember = "id";
			this.listBoxSelectedContacts.DisplayMember = "contact";
			if (!this.appManager.m_bAllowDelete)
			{
				this.buttonDelete.Visible = false;
			}
		}

		private void fmEditGroups_FormClosed(object sender, FormClosedEventArgs e)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_0B, "local_FormEditGroupsWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_FormEditGroupsHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
			}
			catch
			{
			}
		}

		public void comboBoxGroups_Load(string match)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			list2.Add(match);
			foreach (string current in this.appManager.m_lsGroupTags)
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
			IEnumerable<string> arg_92_0 = list;
			Func<string, string> arg_92_1;
			if ((arg_92_1 = fmEditGroups.<>c.<>9__13_0) == null)
			{
				arg_92_1 = (fmEditGroups.<>c.<>9__13_0 = new Func<string, string>(fmEditGroups.<>c.<>9.<comboBoxGroups_Load>b__13_0));
			}
			list = arg_92_0.OrderBy(arg_92_1).ToList<string>();
			list2.AddRange(list);
			this.comboBoxGroups.DataSource = list2;
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
			this.DisplayGroupTag();
		}

		private void comboBoxGroups_Leave(object sender, EventArgs e)
		{
			this.bAddGroupOnLeave = true;
			this.DisplayGroupTag();
			this.bAddGroupOnLeave = false;
		}

		private void textBoxGroupTag_TextChanged(object sender, EventArgs e)
		{
			if (this.IsGroupTagValid(this.textBoxGroupTag.Text))
			{
				this.comboBoxContactList.Enabled = true;
				return;
			}
			this.comboBoxContactList.Enabled = false;
			this.listBoxSelectedContacts.Items.Clear();
		}

		private bool IsGroupTagValid(string Text)
		{
			bool result = false;
			if (Text.Length > 1 && Text.Substring(0, 1) == "#")
			{
				result = true;
			}
			return result;
		}

		private void DisplayGroupTag()
		{
			string text = string.Empty;
			if (this.comboBoxGroups.SelectedIndex < 0)
			{
				return;
			}
			if (this.comboBoxGroups.SelectedIndex == 0 && this.comboBoxGroups.SelectedItem.ToString().Length < 2)
			{
				return;
			}
			text = this.appManager.FormatGroupTag(this.comboBoxGroups.SelectedItem.ToString());
			this.textBoxGroupTag.Text = text;
			try
			{
				if (this.IsGroupTagValid(text))
				{
					this.listBoxSelectedContacts.Items.Clear();
					foreach (GroupTagContact current in this.appManager.m_lsGroupTagContacts)
					{
						if (text == current.groupTag)
						{
							this.listBoxSelectedContacts.Items.Add(new fmEditGroups.ContactItem
							{
								Id = current.contactId,
								contact = current.contact
							});
						}
					}
					this.comboBoxGroups_Load(string.Empty);
					this.comboBoxContactList_Load(string.Empty);
					this.comboBoxContactList.Focus();
					this.buttonDelete.Enabled = true;
					this.labelContactCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
				}
				else
				{
					MessageBox.Show("Group tags must start with a # and be at least one character with no spaces or special charaters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					this.textBoxGroupTag.Text = "";
					this.comboBoxGroups.Focus();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error displaying group: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		public void comboBoxContactList_Load(string match)
		{
			List<fmEditGroups.ContactItem> list = new List<fmEditGroups.ContactItem>();
			List<fmEditGroups.ContactItem> list2 = new List<fmEditGroups.ContactItem>();
			list2.Add(new fmEditGroups.ContactItem
			{
				Id = 0L,
				contact = match
			});
			foreach (Contact current in this.appManager.m_lsContact)
			{
				fmEditGroups.ContactItem contactItem = default(fmEditGroups.ContactItem);
				string text = current.firstName + " " + current.lastName;
				if (text.Trim().Length == 0)
				{
					text = "Unnamed";
				}
				else
				{
					text = text.Trim();
				}
				contactItem.Id = current.id;
				contactItem.contact = text + " " + this.appManager.FormatPhone(current.mobileNumber);
				if (!this.listBoxSelectedContacts.Items.Contains(contactItem))
				{
					if (string.IsNullOrEmpty(match))
					{
						list.Add(contactItem);
					}
					else
					{
						string text2 = this.appManager.FormatAlphaNumeric(match);
						if (contactItem.contact.ToLower().Contains(match.ToLower()))
						{
							list.Add(contactItem);
						}
						else if (text2 != "" && current.address.Contains(text2))
						{
							list.Add(contactItem);
						}
					}
				}
			}
			IEnumerable<fmEditGroups.ContactItem> arg_17A_0 = list;
			Func<fmEditGroups.ContactItem, string> arg_17A_1;
			if ((arg_17A_1 = fmEditGroups.<>c.<>9__22_0) == null)
			{
				arg_17A_1 = (fmEditGroups.<>c.<>9__22_0 = new Func<fmEditGroups.ContactItem, string>(fmEditGroups.<>c.<>9.<comboBoxContactList_Load>b__22_0));
			}
			list = arg_17A_0.OrderBy(arg_17A_1).ToList<fmEditGroups.ContactItem>();
			list2.AddRange(list);
			this.comboBoxContactList.DataSource = list2;
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

		private void comboBoxContactList_Leave(object sender, EventArgs e)
		{
			this.bAddContactOnLeave = true;
			if (this.comboBoxContactList.SelectedIndex > 0)
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
			fmEditGroups.ContactItem contactItem = (fmEditGroups.ContactItem)this.comboBoxContactList.SelectedItem;
			if (contactItem.Id == 0L)
			{
				return;
			}
			this.strError = this.appManager.AddGroupTag(contactItem.Id, this.textBoxGroupTag.Text);
			if (!string.IsNullOrEmpty(this.strError))
			{
				MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.strError = string.Empty;
				return;
			}
			this.listBoxSelectedContacts.Items.Add(new fmEditGroups.ContactItem
			{
				Id = contactItem.Id,
				contact = contactItem.contact
			});
			this.appManager.ShowBalloon(contactItem.contact + " has been added to group tag " + this.textBoxGroupTag.Text, 5);
			this.labelContactCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
			this.comboBoxContactList_Load(string.Empty);
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
			fmEditGroups.ContactItem contactItem = (fmEditGroups.ContactItem)listBox.Items[e.Index];
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
			e.Graphics.DrawString(contactItem.contact, listBox.Font, Brushes.Black, (float)(e.Bounds.Left + 5), (float)(e.Bounds.Top + 4));
			e.Graphics.DrawRectangle(new Pen(Color.DimGray, width), e.Bounds);
			e.DrawFocusRectangle();
		}

		private void listBoxSelectedContacts_DoubleClick(object sender, EventArgs e)
		{
			if (this.listBoxSelectedContacts.Items.Count == 0)
			{
				return;
			}
			fmEditGroups.ContactItem contactItem = (fmEditGroups.ContactItem)this.listBoxSelectedContacts.SelectedItem;
			this.strError = this.appManager.RemoveGroupTag(contactItem.Id, this.textBoxGroupTag.Text);
			if (!string.IsNullOrEmpty(this.strError))
			{
				MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.strError = string.Empty;
				return;
			}
			this.listBoxSelectedContacts.Items.Remove(contactItem);
			this.labelContactCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
			this.appManager.ShowBalloon(contactItem.contact + " has been removed from group tag " + this.textBoxGroupTag.Text, 5);
			if (this.listBoxSelectedContacts.Items.Count == 0)
			{
				this.appManager.m_lsGroupTags.Remove(this.textBoxGroupTag.Text);
				this.ResetDisplay();
				return;
			}
			this.comboBoxContactList_Load(string.Empty);
		}

		private void ResetDisplay()
		{
			this.comboBoxGroups_Load(string.Empty);
			this.comboBoxContactList_Load(string.Empty);
			this.textBoxGroupTag.Text = "";
			this.listBoxSelectedContacts.Items.Clear();
			this.buttonDelete.Enabled = false;
			this.comboBoxGroups.Focus();
		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			this.ResetDisplay();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (this.textBoxGroupTag.Text.Length < 1)
			{
				return;
			}
			if (this.listBoxSelectedContacts.Items.Count == 0)
			{
				return;
			}
			try
			{
				this.buttonDelete.Enabled = false;
				if (MessageBox.Show("Delete group tag " + this.textBoxGroupTag.Text + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
				{
					this.buttonDelete.Enabled = true;
					this.buttonDelete.Focus();
					return;
				}
				for (int i = 0; i < this.listBoxSelectedContacts.Items.Count; i++)
				{
					fmEditGroups.ContactItem contactItem = (fmEditGroups.ContactItem)this.listBoxSelectedContacts.Items[i];
					this.strError = this.appManager.RemoveGroupTag(contactItem.Id, this.textBoxGroupTag.Text);
				}
				this.appManager.m_lsGroupTags.Remove(this.textBoxGroupTag.Text);
			}
			catch (Exception ex)
			{
				this.strError = "Error deleting gorup tag: " + ex.Message;
			}
			this.buttonDelete.Enabled = true;
			if (!string.IsNullOrEmpty(this.strError))
			{
				MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				this.strError = string.Empty;
				return;
			}
			this.appManager.ShowBalloon("Group tag deleted", 5);
			this.appManager.LoadContacts(true);
			this.ResetDisplay();
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			if (this.textBoxGroupTag.Text.Length < 1)
			{
				return;
			}
			if (this.listBoxSelectedContacts.Items.Count == 0)
			{
				return;
			}
			this.appManager.ShowBalloon("Group tag saved", 5);
			this.appManager.LoadContacts(true);
			this.ResetDisplay();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmEditGroups));
			this.comboBoxContactList = new ComboBox();
			this.buttonNew = new Button();
			this.buttonDelete = new Button();
			this.comboBoxGroups = new ComboBox();
			this.labelGroupTag = new Label();
			this.labelGroupTagInstructions = new Label();
			this.listBoxSelectedContacts = new ListBox();
			this.labelGroupTagInstruction = new Label();
			this.buttonSave = new Button();
			this.textBoxGroupTag = new Label();
			this.labelContactCount = new Label();
			base.SuspendLayout();
			this.comboBoxContactList.AllowDrop = true;
			this.comboBoxContactList.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxContactList.Enabled = false;
			this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxContactList.FormattingEnabled = true;
			this.comboBoxContactList.Location = new Point(19, 157);
			this.comboBoxContactList.Name = "comboBoxContactList";
			this.comboBoxContactList.Size = new Size(299, 25);
			this.comboBoxContactList.TabIndex = 2;
			this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
			this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
			this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
			this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
			this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
			this.buttonNew.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonNew.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonNew.Location = new Point(19, 332);
			this.buttonNew.Margin = new Padding(4);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new Size(61, 27);
			this.buttonNew.TabIndex = 12;
			this.buttonNew.Text = "Clear";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new EventHandler(this.buttonClear_Click);
			this.buttonDelete.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonDelete.Location = new Point(189, 332);
			this.buttonDelete.Margin = new Padding(4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new Size(67, 27);
			this.buttonDelete.TabIndex = 11;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
			this.comboBoxGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxGroups.FormattingEnabled = true;
			this.comboBoxGroups.Location = new Point(19, 13);
			this.comboBoxGroups.Name = "comboBoxGroups";
			this.comboBoxGroups.Size = new Size(299, 25);
			this.comboBoxGroups.TabIndex = 1;
			this.comboBoxGroups.SelectedIndexChanged += new EventHandler(this.comboBoxGroups_SelectedIndexChanged);
			this.comboBoxGroups.SelectionChangeCommitted += new EventHandler(this.comboBoxGroups_SelectionChangeCommitted);
			this.comboBoxGroups.TextChanged += new EventHandler(this.comboBoxGroups_TextChanged);
			this.comboBoxGroups.KeyPress += new KeyPressEventHandler(this.comboBoxGroups_KeyPress);
			this.comboBoxGroups.Leave += new EventHandler(this.comboBoxGroups_Leave);
			this.labelGroupTag.AutoSize = true;
			this.labelGroupTag.Location = new Point(17, 94);
			this.labelGroupTag.Name = "labelGroupTag";
			this.labelGroupTag.Size = new Size(79, 17);
			this.labelGroupTag.TabIndex = 15;
			this.labelGroupTag.Text = "Group Tag:";
			this.labelGroupTagInstructions.AutoSize = true;
			this.labelGroupTagInstructions.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelGroupTagInstructions.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelGroupTagInstructions.Location = new Point(16, 120);
			this.labelGroupTagInstructions.MaximumSize = new Size(320, 0);
			this.labelGroupTagInstructions.Name = "labelGroupTagInstructions";
			this.labelGroupTagInstructions.Size = new Size(292, 28);
			this.labelGroupTagInstructions.TabIndex = 18;
			this.labelGroupTagInstructions.Text = "Select a contact to add them to the group.  Double-click a contact to remove them from a group.";
			this.listBoxSelectedContacts.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.listBoxSelectedContacts.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.listBoxSelectedContacts.FormattingEnabled = true;
			this.listBoxSelectedContacts.ItemHeight = 17;
			this.listBoxSelectedContacts.Location = new Point(19, 190);
			this.listBoxSelectedContacts.Name = "listBoxSelectedContacts";
			this.listBoxSelectedContacts.ScrollAlwaysVisible = true;
			this.listBoxSelectedContacts.Size = new Size(299, 123);
			this.listBoxSelectedContacts.Sorted = true;
			this.listBoxSelectedContacts.TabIndex = 3;
			this.listBoxSelectedContacts.TabStop = false;
			this.listBoxSelectedContacts.DoubleClick += new EventHandler(this.listBoxSelectedContacts_DoubleClick);
			this.labelGroupTagInstruction.AutoSize = true;
			this.labelGroupTagInstruction.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelGroupTagInstruction.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelGroupTagInstruction.Location = new Point(12, 42);
			this.labelGroupTagInstruction.MaximumSize = new Size(320, 0);
			this.labelGroupTagInstruction.Name = "labelGroupTagInstruction";
			this.labelGroupTagInstruction.Size = new Size(318, 42);
			this.labelGroupTagInstruction.TabIndex = 20;
			this.labelGroupTagInstruction.Text = "Group tags must start with the # sign and have no spaces or special characters. Group tags are not case sensitive. Click on the Clear button to reset the form and add a new group.";
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(264, 332);
			this.buttonSave.Margin = new Padding(4);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(54, 27);
			this.buttonSave.TabIndex = 10;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.textBoxGroupTag.AutoSize = true;
			this.textBoxGroupTag.Font = new Font("Arial", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.textBoxGroupTag.Location = new Point(102, 94);
			this.textBoxGroupTag.Name = "textBoxGroupTag";
			this.textBoxGroupTag.Size = new Size(138, 18);
			this.textBoxGroupTag.TabIndex = 21;
			this.textBoxGroupTag.Text = "Group Tag Display";
			this.textBoxGroupTag.TextChanged += new EventHandler(this.textBoxGroupTag_TextChanged);
			this.labelContactCount.AutoSize = true;
			this.labelContactCount.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelContactCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelContactCount.Location = new Point(261, 134);
			this.labelContactCount.MaximumSize = new Size(320, 0);
			this.labelContactCount.Name = "labelContactCount";
			this.labelContactCount.Size = new Size(13, 14);
			this.labelContactCount.TabIndex = 22;
			this.labelContactCount.Text = "0";
			base.AutoScaleDimensions = new SizeF(8f, 17f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(334, 375);
			base.Controls.Add(this.labelContactCount);
			base.Controls.Add(this.textBoxGroupTag);
			base.Controls.Add(this.buttonSave);
			base.Controls.Add(this.labelGroupTagInstruction);
			base.Controls.Add(this.listBoxSelectedContacts);
			base.Controls.Add(this.labelGroupTagInstructions);
			base.Controls.Add(this.labelGroupTag);
			base.Controls.Add(this.comboBoxGroups);
			base.Controls.Add(this.buttonDelete);
			base.Controls.Add(this.buttonNew);
			base.Controls.Add(this.comboBoxContactList);
			this.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new Padding(4);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new Size(350, 375);
			base.Name = "fmEditGroups";
			this.Text = "Edit Group Tags";
			base.FormClosed += new FormClosedEventHandler(this.fmEditGroups_FormClosed);
			base.Load += new EventHandler(this.fmEditGroups_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

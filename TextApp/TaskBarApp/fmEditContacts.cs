namespace TaskBarApp
{
    using Microsoft.Win32;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;

    public class fmEditContacts : Form
    {
        private bool bAddComboBoxGroups;
        private bool bAddGroupOnLeave;
        private bool bAutoAddRecipient;
        private bool bNotAddedcomboBox;
        private Button buttonBlock;
        private Button buttonClear;
        private Button buttonDelete;
        private Button buttonSave;
        private ComboBox comboBoxContactList;
        private ComboBox comboBoxGroups;
        private IContainer components;
        private Label labelContactAddress;
        private Label labelContactCount;
        private Label labelContactID;
        private Label labelContactSelect;
        private Label labelFirstName;
        private Label labelGroupTagInstructions;
        private Label labelGroupTags;
        private Label labelLastName;
        private Label labelPhoneNumber;
        private ListBox listBoxSelectedGroups;
        private string strError = string.Empty;
        private TextBox textBoxFirstName;
        private TextBox textBoxLastName;
        private MaskedTextBox textBoxPhoneNumber;

        public fmEditContacts()
        {
            this.InitializeComponent();
        }

        private void buttonBlock_Click(object sender, EventArgs e)
        {
            if (this.labelContactID.Text.Length > 0)
            {
                long contactID = Convert.ToInt64(this.labelContactID.Text);
                try
                {
                    string[] textArray1 = new string[] { "Are you sure you want to block ", this.textBoxFirstName.Text, " ", this.textBoxLastName.Text, " ", this.textBoxPhoneNumber.Text, " from ever sending texts to your number in the future?\n\n Please Note this cannot be undone!" };
                    if (MessageBox.Show(string.Concat(textArray1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        if (!this.appManager.m_textService.BlockContact(this.appManager.FormatAlphaNumeric(this.textBoxPhoneNumber.Text), this.appManager.m_strSession).Data.success)
                        {
                            this.strError = "Error calling contact/block...";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Error blocking number: " + exception.Message;
                }
                if (!string.IsNullOrEmpty(this.strError))
                {
                    this.appManager.ShowBalloon(this.strError, 5);
                    this.strError = string.Empty;
                }
                else
                {
                    this.appManager.RemoveGroupTag(contactID, "all");
                    this.textBoxFirstName.Text = "BLOCKED" + this.textBoxFirstName.Text;
                    this.SaveContact(false);
                    this.appManager.ShowBalloon("The number has been permanently blocked.", 5);
                    this.ResetDisplay();
                }
            }
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
                string fingerprint = string.Empty;
                long contactID = Convert.ToInt64(this.labelContactID.Text);
                try
                {
                    string[] textArray1 = new string[] { "Delete contact ", this.textBoxFirstName.Text, " ", this.textBoxLastName.Text, "?" };
                    if (MessageBox.Show(string.Concat(textArray1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        string[] textArray2 = new string[] { "Delete any conversations associated with ", this.textBoxFirstName.Text, " ", this.textBoxLastName.Text, "?\n\n Please Note this cannot be undone!" };
                        if (MessageBox.Show(string.Concat(textArray2), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            flag = true;
                        }
                        this.appManager.RemoveGroupTag(contactID, "all");
                        if (this.appManager.m_textService.DeleteContact(contactID, this.appManager.m_strSession).Data.success)
                        {
                            if (flag && (this.appManager.m_lsConversation != null))
                            {
                                Conversation item = this.appManager.m_lsConversation.Find(var => var.lastContactId == contactID);
                                if (item != null)
                                {
                                    fingerprint = item.fingerprint;
                                    if (!this.appManager.m_textService.ConversationDelete(fingerprint, this.appManager.m_strSession).Data.success)
                                    {
                                        this.strError = "Error calling conversation/delete...";
                                    }
                                    else
                                    {
                                        this.appManager.m_lsConversation.Remove(item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.strError = "Error calling contact/delete...";
                        }
                    }
                    else
                    {
                        this.buttonDelete.Enabled = true;
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
                    this.buttonSave.Enabled = true;
                }
                else
                {
                    string text = string.Empty;
                    this.appManager.m_lsContact = (from val in this.appManager.m_lsContact
                        where val.id != contactID
                        select val).ToList<Contact>();
                    this.appManager.m_strCurrentContactAddress = string.Empty;
                    this.appManager.m_nCurrentContactID = 0L;
                    this.appManager.m_strCurentConversationFingerprint = string.Empty;
                    if (flag)
                    {
                        text = "Conversation and Contact deleted";
                    }
                    else
                    {
                        text = "Contact deleted";
                    }
                    this.appManager.ShowBalloon(text, 5);
                    if (this.appManager.formMessages != null)
                    {
                        this.appManager.formMessages.DisplayConversatoinList();
                        this.appManager.formMessages.DisplayConversationHeader();
                        if (this.appManager.m_strCurentConversationFingerprint == fingerprint)
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.buttonSave.Enabled = false;
            this.SaveContact(true);
            this.buttonSave.Enabled = true;
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
                this.bNotAddedcomboBox = true;
            }
            this.comboBoxContactList.DroppedDown = true;
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

        private void comboBoxContactList_Load(string match)
        {
            List<ContactItem> collection = new List<ContactItem>();
            List<ContactItem> list2 = new List<ContactItem>();
            ContactItem item = new ContactItem {
                Id = 0L,
                contact = match
            };
            list2.Add(item);
            foreach (Contact contact in this.appManager.m_lsContact)
            {
                item = new ContactItem();
                string str = contact.firstName + " " + contact.lastName;
                if (str.Trim().Length == 0)
                {
                    str = "Unnamed";
                }
                else
                {
                    str = str.Trim();
                }
                item.Id = contact.id;
                item.contact = str + " " + this.appManager.FormatPhone(contact.mobileNumber);
                if (string.IsNullOrEmpty(match))
                {
                    collection.Add(item);
                }
                else
                {
                    string str2 = this.appManager.FormatAlphaNumeric(match);
                    if (item.contact.ToLower().Contains(match.ToLower()))
                    {
                        collection.Add(item);
                    }
                    else if ((str2 != "") && contact.address.Contains(str2))
                    {
                        collection.Add(item);
                    }
                }
            }
            collection = (from c in collection
                orderby c.contact
                select c).ToList<ContactItem>();
            list2.AddRange(collection);
            this.comboBoxContactList.DataSource = list2;
            this.labelContactCount.Text = collection.Count.ToString() + " Contacts";
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
            if ((this.comboBoxContactList.DroppedDown && !this.bAutoAddRecipient) && (this.comboBoxContactList.SelectedIndex > 0))
            {
                this.DisplayContact();
            }
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

        private void comboBoxGroups_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Contains(e.KeyChar.ToString()))
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

        private void comboBoxGroups_Leave(object sender, EventArgs e)
        {
            this.bAddGroupOnLeave = true;
            this.listBoxSelectedGroups_Add();
            this.bAddGroupOnLeave = false;
        }

        public void comboBoxGroups_Load(string match)
        {
            List<string> list = new List<string> {
                match
            };
            foreach (string str in this.appManager.m_lsGroupTags)
            {
                if (!this.listBoxSelectedGroups.Items.Contains(str))
                {
                    if (string.IsNullOrEmpty(match))
                    {
                        list.Add(str);
                    }
                    else if (str.ToLower().Contains(match.ToLower()))
                    {
                        list.Add(str);
                    }
                }
            }
            list = (from g in list
                orderby g
                select g).ToList<string>();
            this.comboBoxGroups.DataSource = list;
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
            if (this.comboBoxGroups.DroppedDown && !this.bAddGroupOnLeave)
            {
                this.listBoxSelectedGroups_Add();
            }
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

        public void DisplayContact()
        {
            Contact contactByID = null;
            long contactID = 0L;
            this.textBoxPhoneNumber.Mask = null;
            if (this.comboBoxContactList.SelectedIndex > 0)
            {
                ContactItem selectedItem = (ContactItem) this.comboBoxContactList.SelectedItem;
                contactID = selectedItem.Id;
            }
            if (contactID != 0)
            {
                contactByID = this.appManager.GetContactByID(contactID);
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
                foreach (string str in this.appManager.GetContactGroupTags(contactByID.notes))
                {
                    this.listBoxSelectedGroups.Items.Add(str);
                }
            }
            else
            {
                this.textBoxFirstName.Text = string.Empty;
                this.textBoxLastName.Text = string.Empty;
                this.labelContactID.Text = string.Empty;
                this.textBoxPhoneNumber.Text = this.appManager.FormatPhone(this.appManager.m_strCurrentContactAddress);
                this.labelContactAddress.Text = this.appManager.m_strCurrentContactAddress;
                this.listBoxSelectedGroups.Items.Clear();
            }
            this.comboBoxContactList_Load(string.Empty);
            this.comboBoxGroups_Load(string.Empty);
            this.textBoxPhoneNumber.Enabled = false;
            this.bNewContact = false;
            if (this.labelContactID.Text.Length > 0)
            {
                this.buttonDelete.Enabled = true;
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

        private void fmEditContact_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormEditContactWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormEditContactHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmEditContact_Load(object sender, EventArgs e)
        {
            this.Text = this.appManager.m_strApplicationName + " Edit Contacts " + this.appManager.FormatPhone(this.appManager.m_strUserName);
            base.Icon = this.appManager.iTextApp;
            int num = 0;
            int num2 = 0;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditContactWidth", ref num, ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditContactHeight", ref num2, ref this.strError);
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
                catch (Exception exception)
                {
                    this.strError = "Error getting current contact information: " + exception.Message;
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
            }
            else
            {
                this.textBoxPhoneNumber.Mask = null;
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmEditContacts));
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
            this.textBoxFirstName.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxFirstName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxFirstName.Location = new Point(0x83, 0x83);
            this.textBoxFirstName.Margin = new Padding(4);
            this.textBoxFirstName.Name = "textBoxFirstName";
            this.textBoxFirstName.Size = new Size(0xbc, 0x19);
            this.textBoxFirstName.TabIndex = 3;
            this.textBoxLastName.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxLastName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxLastName.Location = new Point(0x83, 0xb3);
            this.textBoxLastName.Margin = new Padding(4);
            this.textBoxLastName.Name = "textBoxLastName";
            this.textBoxLastName.Size = new Size(0xbc, 0x19);
            this.textBoxLastName.TabIndex = 4;
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelFirstName.Location = new Point(14, 0x86);
            this.labelFirstName.Margin = new Padding(4, 0, 4, 0);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new Size(110, 0x11);
            this.labelFirstName.TabIndex = 2;
            this.labelFirstName.Text = "Contact Field 1:";
            this.labelLastName.AutoSize = true;
            this.labelLastName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelLastName.Location = new Point(0x10, 0xb7);
            this.labelLastName.Margin = new Padding(4, 0, 4, 0);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new Size(110, 0x11);
            this.labelLastName.TabIndex = 3;
            this.labelLastName.Text = "Contact Field 2:";
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelPhoneNumber.Location = new Point(0x10, 0x57);
            this.labelPhoneNumber.Margin = new Padding(4, 0, 4, 0);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new Size(0x5e, 0x11);
            this.labelPhoneNumber.TabIndex = 4;
            this.labelPhoneNumber.Text = "Text Number:";
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x106, 0x1ad);
            this.buttonSave.Margin = new Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x38, 0x1b);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.labelGroupTags.AutoSize = true;
            this.labelGroupTags.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelGroupTags.Location = new Point(0x10, 220);
            this.labelGroupTags.Margin = new Padding(4, 0, 4, 0);
            this.labelGroupTags.Name = "labelGroupTags";
            this.labelGroupTags.Size = new Size(0x57, 0x11);
            this.labelGroupTags.TabIndex = 7;
            this.labelGroupTags.Text = "Group Tags:";
            this.labelContactAddress.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.labelContactAddress.AutoSize = true;
            this.labelContactAddress.Location = new Point(160, 0x1b2);
            this.labelContactAddress.Margin = new Padding(4, 0, 4, 0);
            this.labelContactAddress.Name = "labelContactAddress";
            this.labelContactAddress.Size = new Size(0x9c, 0x11);
            this.labelContactAddress.TabIndex = 8;
            this.labelContactAddress.Text = "hiddenContactAddress";
            this.labelContactAddress.Visible = false;
            this.textBoxPhoneNumber.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxPhoneNumber.Location = new Point(0x83, 0x54);
            this.textBoxPhoneNumber.Mask = "(000) 000-0000";
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new Size(0xbb, 0x19);
            this.textBoxPhoneNumber.TabIndex = 2;
            this.textBoxPhoneNumber.Click += new EventHandler(this.textBoxPhoneNumbere_Click);
            this.comboBoxContactList.AllowDrop = true;
            this.comboBoxContactList.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxContactList.FormattingEnabled = true;
            this.comboBoxContactList.Location = new Point(0x10, 0x10);
            this.comboBoxContactList.Name = "comboBoxContactList";
            this.comboBoxContactList.Size = new Size(0x12e, 0x19);
            this.comboBoxContactList.TabIndex = 1;
            this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
            this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
            this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
            this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
            this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
            this.buttonClear.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonClear.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonClear.Location = new Point(0x13, 0x1ad);
            this.buttonClear.Margin = new Padding(4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new Size(60, 0x1b);
            this.buttonClear.TabIndex = 13;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new EventHandler(this.buttonClear_Click);
            this.buttonDelete.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonDelete.Location = new Point(0x70, 0x1ad);
            this.buttonDelete.Margin = new Padding(4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x43, 0x1b);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.labelContactID.AutoSize = true;
            this.labelContactID.Location = new Point(0x10, 0x5c);
            this.labelContactID.Margin = new Padding(4, 0, 4, 0);
            this.labelContactID.Name = "labelContactID";
            this.labelContactID.Size = new Size(0, 0x11);
            this.labelContactID.TabIndex = 14;
            this.labelContactID.Visible = false;
            this.comboBoxGroups.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new Point(0x11, 0xf4);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new Size(0x12b, 0x19);
            this.comboBoxGroups.TabIndex = 5;
            this.comboBoxGroups.SelectedIndexChanged += new EventHandler(this.comboBoxGroups_SelectedIndexChanged);
            this.comboBoxGroups.SelectionChangeCommitted += new EventHandler(this.comboBoxGroups_SelectionChangeCommitted);
            this.comboBoxGroups.TextChanged += new EventHandler(this.comboBoxGroups_TextChanged);
            this.comboBoxGroups.KeyPress += new KeyPressEventHandler(this.comboBoxGroups_KeyPress);
            this.comboBoxGroups.Leave += new EventHandler(this.comboBoxGroups_Leave);
            this.labelGroupTagInstructions.AutoSize = true;
            this.labelGroupTagInstructions.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelGroupTagInstructions.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelGroupTagInstructions.Location = new Point(15, 0x110);
            this.labelGroupTagInstructions.MaximumSize = new Size(310, 0);
            this.labelGroupTagInstructions.Name = "labelGroupTagInstructions";
            this.labelGroupTagInstructions.Size = new Size(0x131, 0x1c);
            this.labelGroupTagInstructions.TabIndex = 0x13;
            this.labelGroupTagInstructions.Text = "Select a group to add the contact to the group.  Double-click a group to remove the contact from the group.";
            this.listBoxSelectedGroups.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listBoxSelectedGroups.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.listBoxSelectedGroups.FormattingEnabled = true;
            this.listBoxSelectedGroups.ItemHeight = 0x11;
            this.listBoxSelectedGroups.Location = new Point(0x11, 0x138);
            this.listBoxSelectedGroups.Name = "listBoxSelectedGroups";
            this.listBoxSelectedGroups.ScrollAlwaysVisible = true;
            this.listBoxSelectedGroups.Size = new Size(0x12b, 0x6a);
            this.listBoxSelectedGroups.Sorted = true;
            this.listBoxSelectedGroups.TabIndex = 20;
            this.listBoxSelectedGroups.TabStop = false;
            this.listBoxSelectedGroups.DoubleClick += new EventHandler(this.listBoxSelectedGroups_DoubleClick);
            this.labelContactSelect.AutoSize = true;
            this.labelContactSelect.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelContactSelect.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelContactSelect.Location = new Point(0x15, 0x2c);
            this.labelContactSelect.MaximumSize = new Size(310, 0);
            this.labelContactSelect.Name = "labelContactSelect";
            this.labelContactSelect.Size = new Size(0x129, 0x1c);
            this.labelContactSelect.TabIndex = 0x15;
            this.labelContactSelect.Text = "Select a contact to edit.  Click the Clear button to reset the form and add a new contact.";
            this.labelContactCount.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.labelContactCount.AutoSize = true;
            this.labelContactCount.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelContactCount.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelContactCount.Location = new Point(0x100, 0x3a);
            this.labelContactCount.MaximumSize = new Size(310, 0);
            this.labelContactCount.Name = "labelContactCount";
            this.labelContactCount.Size = new Size(60, 14);
            this.labelContactCount.TabIndex = 0x16;
            this.labelContactCount.Text = "0 Contacts";
            this.labelContactCount.TextAlign = ContentAlignment.TopRight;
            this.buttonBlock.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonBlock.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonBlock.Location = new Point(0xbb, 0x1ad);
            this.buttonBlock.Margin = new Padding(4);
            this.buttonBlock.Name = "buttonBlock";
            this.buttonBlock.Size = new Size(0x43, 0x1b);
            this.buttonBlock.TabIndex = 0x17;
            this.buttonBlock.Text = "Block";
            this.buttonBlock.UseVisualStyleBackColor = true;
            this.buttonBlock.Click += new EventHandler(this.buttonBlock_Click);
            base.AutoScaleDimensions = new SizeF(8f, 17f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x14e, 0x1d5);
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
            base.Icon = (Icon) manager.GetObject("$this.Icon");
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

        private void linkLabelEditGroups_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.appManager.ShowEditGroups();
        }

        private void listBoxSelectedGroups_Add()
        {
            if ((this.comboBoxGroups.SelectedIndex >= 0) && (this.comboBoxGroups.SelectedItem.ToString() != ""))
            {
                string item = this.comboBoxGroups.SelectedItem.ToString();
                if (this.appManager.m_lsGroupTags.Contains(item))
                {
                    this.listBoxSelectedGroups.Items.Add(item);
                }
                this.comboBoxGroups_Load(string.Empty);
            }
        }

        private void listBoxSelectedGroups_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBoxSelectedGroups.Items.Count != 0)
            {
                string str = this.listBoxSelectedGroups.SelectedItem.ToString();
                this.listBoxSelectedGroups.Items.Remove(str);
                this.comboBoxGroups_Load(string.Empty);
            }
        }

        private void listBoxSelectedGroups_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ListBox box = (ListBox) sender;
                string s = (string) box.Items[e.Index];
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
                e.Graphics.DrawString(s, box.Font, Brushes.Black, (float) (e.Bounds.Left + 5), (float) (e.Bounds.Top + 4));
                e.Graphics.DrawRectangle(new Pen(System.Drawing.Color.DimGray, width), e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void listBoxSelectedGroups_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 0x18;
        }

        private void PositionCursorInMaskedTextBox(MaskedTextBox mtb)
        {
            if (mtb != null)
            {
                mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (mtb.Text.Length <= 3)
                {
                    mtb.Select(mtb.Text.Length + 1, 0);
                }
                if ((mtb.Text.Length > 3) && (mtb.Text.Length <= 6))
                {
                    mtb.Select(mtb.Text.Length + 3, 0);
                }
                if (mtb.Text.Length > 6)
                {
                    mtb.Select(mtb.Text.Length + 4, 0);
                }
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
            string notes = string.Empty;
            long nSavedContactID = 0L;
            if (this.textBoxPhoneNumber.Text.Length != 0)
            {
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
                        foreach (string str2 in this.listBoxSelectedGroups.Items)
                        {
                            notes = notes + str2;
                        }
                        IRestResponse<ContactSaveResponse> response = this.appManager.m_textService.SaveContact(this.labelContactAddress.Text, this.appManager.m_strSession, this.textBoxFirstName.Text, this.textBoxLastName.Text, notes);
                        if (!response.Data.success)
                        {
                            this.strError = "Error calling contact/save...";
                        }
                        else
                        {
                            nSavedContactID = response.Data.response.id;
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Error saving contact: " + exception.Message;
                }
                if (!string.IsNullOrEmpty(this.strError))
                {
                    MessageBox.Show(this.strError, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.strError = string.Empty;
                    this.buttonSave.Enabled = true;
                }
                else
                {
                    Contact contact = this.appManager.m_lsContact.Find(var => var.id == nSavedContactID);
                    if (contact != null)
                    {
                        contact.lastName = this.textBoxLastName.Text;
                        contact.firstName = this.textBoxFirstName.Text;
                        contact.notes = notes;
                        foreach (string str3 in this.listBoxSelectedGroups.Items)
                        {
                            this.appManager.AddGroupTag(nSavedContactID, str3);
                        }
                    }
                    else
                    {
                        this.appManager.LoadUpdates(false);
                    }
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
            }
        }

        private void textBoxPhoneNumbere_Click(object sender, EventArgs e)
        {
            this.PositionCursorInMaskedTextBox(this.textBoxPhoneNumber);
        }

        public ApplicationManager appManager { get; set; }

        public bool bNewContact { get; set; }

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly fmEditContacts.<>c <>9 = new fmEditContacts.<>c();
            public static Func<fmEditContacts.ContactItem, string> <>9__17_0;
            public static Func<string, string> <>9__23_0;

            internal string <comboBoxContactList_Load>b__17_0(fmEditContacts.ContactItem c) => 
                c.contact;

            internal string <comboBoxGroups_Load>b__23_0(string g) => 
                g;
        }*/

        [StructLayout(LayoutKind.Sequential)]
        private struct ContactItem
        {
            public long Id { get; set; }
            public string contact { get; set; }
        }
    }
}


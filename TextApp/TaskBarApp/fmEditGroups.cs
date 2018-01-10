namespace TaskBarApp
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;

    public class fmEditGroups : Form
    {
        private bool bAddComboBoxContactList;
        private bool bAddComboBoxGroups;
        private bool bAddContactOnLeave;
        private bool bAddGroupOnLeave;
        private Button buttonDelete;
        private Button buttonNew;
        private Button buttonSave;
        private ComboBox comboBoxContactList;
        private ComboBox comboBoxGroups;
        private IContainer components;
        private Label labelContactCount;
        private Label labelGroupTag;
        private Label labelGroupTagInstruction;
        private Label labelGroupTagInstructions;
        private ListBox listBoxSelectedContacts;
        private string strError = string.Empty;
        private Label textBoxGroupTag;

        public fmEditGroups()
        {
            this.InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.ResetDisplay();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if ((this.textBoxGroupTag.Text.Length >= 1) && (this.listBoxSelectedContacts.Items.Count != 0))
            {
                try
                {
                    this.buttonDelete.Enabled = false;
                    if (MessageBox.Show("Delete group tag " + this.textBoxGroupTag.Text + "?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        for (int i = 0; i < this.listBoxSelectedContacts.Items.Count; i++)
                        {
                            ContactItem item = (ContactItem) this.listBoxSelectedContacts.Items[i];
                            this.strError = this.appManager.RemoveGroupTag(item.Id, this.textBoxGroupTag.Text);
                        }
                        this.appManager.m_lsGroupTags.Remove(this.textBoxGroupTag.Text);
                    }
                    else
                    {
                        this.buttonDelete.Enabled = true;
                        this.buttonDelete.Focus();
                        return;
                    }
                }
                catch (Exception exception)
                {
                    this.strError = "Error deleting gorup tag: " + exception.Message;
                }
                this.buttonDelete.Enabled = true;
                if (!string.IsNullOrEmpty(this.strError))
                {
                    MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.strError = string.Empty;
                }
                else
                {
                    this.appManager.ShowBalloon("Group tag deleted", 5);
                    this.appManager.LoadContacts(true);
                    this.ResetDisplay();
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if ((this.textBoxGroupTag.Text.Length >= 1) && (this.listBoxSelectedContacts.Items.Count != 0))
            {
                this.appManager.ShowBalloon("Group tag saved", 5);
                this.appManager.LoadContacts(true);
                this.ResetDisplay();
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
            if (this.comboBoxContactList.SelectedIndex > 0)
            {
                this.listBoxSelectedContacts_Add();
            }
            this.bAddContactOnLeave = false;
        }

        public void comboBoxContactList_Load(string match)
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
                if (!this.listBoxSelectedContacts.Items.Contains(item))
                {
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
            }
            collection = (from c in collection
                orderby c.contact
                select c).ToList<ContactItem>();
            list2.AddRange(collection);
            this.comboBoxContactList.DataSource = list2;
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
            this.DisplayGroupTag();
            this.bAddGroupOnLeave = false;
        }

        public void comboBoxGroups_Load(string match)
        {
            List<string> collection = new List<string>();
            List<string> list2 = new List<string> {
                match
            };
            foreach (string str in this.appManager.m_lsGroupTags)
            {
                if (string.IsNullOrEmpty(match))
                {
                    collection.Add(str);
                }
                else if (str.ToLower().Contains(match.ToLower()))
                {
                    collection.Add(str);
                }
            }
            collection = (from g in collection
                orderby g
                select g).ToList<string>();
            list2.AddRange(collection);
            this.comboBoxGroups.DataSource = list2;
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
                this.DisplayGroupTag();
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

        private void DisplayGroupTag()
        {
            string text = string.Empty;
            if ((this.comboBoxGroups.SelectedIndex >= 0) && ((this.comboBoxGroups.SelectedIndex != 0) || (this.comboBoxGroups.SelectedItem.ToString().Length >= 2)))
            {
                text = this.appManager.FormatGroupTag(this.comboBoxGroups.SelectedItem.ToString());
                this.textBoxGroupTag.Text = text;
                try
                {
                    if (this.IsGroupTagValid(text))
                    {
                        this.listBoxSelectedContacts.Items.Clear();
                        foreach (GroupTagContact contact in this.appManager.m_lsGroupTagContacts)
                        {
                            if (text == contact.groupTag)
                            {
                                ContactItem item = new ContactItem {
                                    Id = contact.contactId,
                                    contact = contact.contact
                                };
                                this.listBoxSelectedContacts.Items.Add(item);
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
                catch (Exception exception)
                {
                    MessageBox.Show("Error displaying group: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
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

        private void fmEditGroups_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormEditGroupsWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormEditGroupsHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmEditGroups_Load(object sender, EventArgs e)
        {
            this.Text = this.appManager.m_strApplicationName + " Edit Groups " + this.appManager.FormatPhone(this.appManager.m_strUserName);
            base.Icon = this.appManager.iTextApp;
            int num = 0;
            int num2 = 0;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditGroupsWidth", ref num, ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditGroupsHeight", ref num2, ref this.strError);
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

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmEditGroups));
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
            this.comboBoxContactList.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxContactList.Enabled = false;
            this.comboBoxContactList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxContactList.FormattingEnabled = true;
            this.comboBoxContactList.Location = new Point(0x13, 0x9d);
            this.comboBoxContactList.Name = "comboBoxContactList";
            this.comboBoxContactList.Size = new Size(0x12b, 0x19);
            this.comboBoxContactList.TabIndex = 2;
            this.comboBoxContactList.SelectedIndexChanged += new EventHandler(this.comboBoxContactList_SelectedIndexChanged);
            this.comboBoxContactList.SelectionChangeCommitted += new EventHandler(this.comboBoxContactList_SelectionChangeCommitted);
            this.comboBoxContactList.TextChanged += new EventHandler(this.comboBoxContactList_TextChanged);
            this.comboBoxContactList.KeyPress += new KeyPressEventHandler(this.comboBoxContactList_KeyPress);
            this.comboBoxContactList.Leave += new EventHandler(this.comboBoxContactList_Leave);
            this.buttonNew.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonNew.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonNew.Location = new Point(0x13, 0x14c);
            this.buttonNew.Margin = new Padding(4);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new Size(0x3d, 0x1b);
            this.buttonNew.TabIndex = 12;
            this.buttonNew.Text = "Clear";
            this.buttonNew.UseVisualStyleBackColor = true;
            this.buttonNew.Click += new EventHandler(this.buttonClear_Click);
            this.buttonDelete.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonDelete.Enabled = false;
            this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonDelete.Location = new Point(0xbd, 0x14c);
            this.buttonDelete.Margin = new Padding(4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x43, 0x1b);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.comboBoxGroups.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new Point(0x13, 13);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new Size(0x12b, 0x19);
            this.comboBoxGroups.TabIndex = 1;
            this.comboBoxGroups.SelectedIndexChanged += new EventHandler(this.comboBoxGroups_SelectedIndexChanged);
            this.comboBoxGroups.SelectionChangeCommitted += new EventHandler(this.comboBoxGroups_SelectionChangeCommitted);
            this.comboBoxGroups.TextChanged += new EventHandler(this.comboBoxGroups_TextChanged);
            this.comboBoxGroups.KeyPress += new KeyPressEventHandler(this.comboBoxGroups_KeyPress);
            this.comboBoxGroups.Leave += new EventHandler(this.comboBoxGroups_Leave);
            this.labelGroupTag.AutoSize = true;
            this.labelGroupTag.Location = new Point(0x11, 0x5e);
            this.labelGroupTag.Name = "labelGroupTag";
            this.labelGroupTag.Size = new Size(0x4f, 0x11);
            this.labelGroupTag.TabIndex = 15;
            this.labelGroupTag.Text = "Group Tag:";
            this.labelGroupTagInstructions.AutoSize = true;
            this.labelGroupTagInstructions.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelGroupTagInstructions.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelGroupTagInstructions.Location = new Point(0x10, 120);
            this.labelGroupTagInstructions.MaximumSize = new Size(320, 0);
            this.labelGroupTagInstructions.Name = "labelGroupTagInstructions";
            this.labelGroupTagInstructions.Size = new Size(0x124, 0x1c);
            this.labelGroupTagInstructions.TabIndex = 0x12;
            this.labelGroupTagInstructions.Text = "Select a contact to add them to the group.  Double-click a contact to remove them from a group.";
            this.listBoxSelectedContacts.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.listBoxSelectedContacts.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.listBoxSelectedContacts.FormattingEnabled = true;
            this.listBoxSelectedContacts.ItemHeight = 0x11;
            this.listBoxSelectedContacts.Location = new Point(0x13, 190);
            this.listBoxSelectedContacts.Name = "listBoxSelectedContacts";
            this.listBoxSelectedContacts.ScrollAlwaysVisible = true;
            this.listBoxSelectedContacts.Size = new Size(0x12b, 0x7b);
            this.listBoxSelectedContacts.Sorted = true;
            this.listBoxSelectedContacts.TabIndex = 3;
            this.listBoxSelectedContacts.TabStop = false;
            this.listBoxSelectedContacts.DoubleClick += new EventHandler(this.listBoxSelectedContacts_DoubleClick);
            this.labelGroupTagInstruction.AutoSize = true;
            this.labelGroupTagInstruction.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelGroupTagInstruction.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelGroupTagInstruction.Location = new Point(12, 0x2a);
            this.labelGroupTagInstruction.MaximumSize = new Size(320, 0);
            this.labelGroupTagInstruction.Name = "labelGroupTagInstruction";
            this.labelGroupTagInstruction.Size = new Size(0x13e, 0x2a);
            this.labelGroupTagInstruction.TabIndex = 20;
            this.labelGroupTagInstruction.Text = "Group tags must start with the # sign and have no spaces or special characters. Group tags are not case sensitive. Click on the Clear button to reset the form and add a new group.";
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x108, 0x14c);
            this.buttonSave.Margin = new Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x36, 0x1b);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.textBoxGroupTag.AutoSize = true;
            this.textBoxGroupTag.Font = new Font("Arial", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.textBoxGroupTag.Location = new Point(0x66, 0x5e);
            this.textBoxGroupTag.Name = "textBoxGroupTag";
            this.textBoxGroupTag.Size = new Size(0x8a, 0x12);
            this.textBoxGroupTag.TabIndex = 0x15;
            this.textBoxGroupTag.Text = "Group Tag Display";
            this.textBoxGroupTag.TextChanged += new EventHandler(this.textBoxGroupTag_TextChanged);
            this.labelContactCount.AutoSize = true;
            this.labelContactCount.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelContactCount.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelContactCount.Location = new Point(0x105, 0x86);
            this.labelContactCount.MaximumSize = new Size(320, 0);
            this.labelContactCount.Name = "labelContactCount";
            this.labelContactCount.Size = new Size(13, 14);
            this.labelContactCount.TabIndex = 0x16;
            this.labelContactCount.Text = "0";
            base.AutoScaleDimensions = new SizeF(8f, 17f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x14e, 0x177);
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
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Margin = new Padding(4);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new Size(350, 0x177);
            base.Name = "fmEditGroups";
            this.Text = "Edit Group Tags";
            base.FormClosed += new FormClosedEventHandler(this.fmEditGroups_FormClosed);
            base.Load += new EventHandler(this.fmEditGroups_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsGroupTagValid(string Text)
        {
            bool flag = false;
            if ((Text.Length > 1) && (Text.Substring(0, 1) == "#"))
            {
                flag = true;
            }
            return flag;
        }

        private void listBoxSelectedContacts_Add()
        {
            if (this.comboBoxContactList.SelectedIndex >= 0)
            {
                ContactItem selectedItem = (ContactItem) this.comboBoxContactList.SelectedItem;
                if (selectedItem.Id != 0)
                {
                    this.strError = this.appManager.AddGroupTag(selectedItem.Id, this.textBoxGroupTag.Text);
                    if (!string.IsNullOrEmpty(this.strError))
                    {
                        MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.strError = string.Empty;
                    }
                    else
                    {
                        ContactItem item = new ContactItem {
                            Id = selectedItem.Id,
                            contact = selectedItem.contact
                        };
                        this.listBoxSelectedContacts.Items.Add(item);
                        this.appManager.ShowBalloon(selectedItem.contact + " has been added to group tag " + this.textBoxGroupTag.Text, 5);
                        this.labelContactCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
                        this.comboBoxContactList_Load(string.Empty);
                    }
                }
            }
        }

        private void listBoxSelectedContacts_DoubleClick(object sender, EventArgs e)
        {
            if (this.listBoxSelectedContacts.Items.Count != 0)
            {
                ContactItem selectedItem = (ContactItem) this.listBoxSelectedContacts.SelectedItem;
                this.strError = this.appManager.RemoveGroupTag(selectedItem.Id, this.textBoxGroupTag.Text);
                if (!string.IsNullOrEmpty(this.strError))
                {
                    MessageBox.Show(this.strError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.strError = string.Empty;
                }
                else
                {
                    this.listBoxSelectedContacts.Items.Remove(selectedItem);
                    this.labelContactCount.Text = this.listBoxSelectedContacts.Items.Count.ToString();
                    this.appManager.ShowBalloon(selectedItem.contact + " has been removed from group tag " + this.textBoxGroupTag.Text, 5);
                    if (this.listBoxSelectedContacts.Items.Count == 0)
                    {
                        this.appManager.m_lsGroupTags.Remove(this.textBoxGroupTag.Text);
                        this.ResetDisplay();
                    }
                    else
                    {
                        this.comboBoxContactList_Load(string.Empty);
                    }
                }
            }
        }

        private void listBoxSelectedContacts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ListBox box = (ListBox) sender;
                ContactItem item = (ContactItem) box.Items[e.Index];
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
                e.Graphics.DrawString(item.contact, box.Font, Brushes.Black, (float) (e.Bounds.Left + 5), (float) (e.Bounds.Top + 4));
                e.Graphics.DrawRectangle(new Pen(System.Drawing.Color.DimGray, width), e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        private void listBoxSelectedContacts_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 0x18;
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

        private void textBoxGroupTag_TextChanged(object sender, EventArgs e)
        {
            if (this.IsGroupTagValid(this.textBoxGroupTag.Text))
            {
                this.comboBoxContactList.Enabled = true;
            }
            else
            {
                this.comboBoxContactList.Enabled = false;
                this.listBoxSelectedContacts.Items.Clear();
            }
        }

        public ApplicationManager appManager { get; set; }

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly fmEditGroups.<>c <>9 = new fmEditGroups.<>c();
            public static Func<string, string> <>9__13_0;
            public static Func<fmEditGroups.ContactItem, string> <>9__22_0;

            internal string <comboBoxContactList_Load>b__22_0(fmEditGroups.ContactItem c) => 
                c.contact;

            internal string <comboBoxGroups_Load>b__13_0(string g) => 
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


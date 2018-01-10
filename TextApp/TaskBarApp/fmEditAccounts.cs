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
    using Zipwhip;

    public class fmEditAccounts : Form
    {
        private bool bAutoAddRecipient;
        private bool bNotAddedcomboBox;
        private Button buttonClear;
        private Button buttonDelete;
        private Button buttonSave;
        private ComboBox comboBoxAccountList;
        private ComboBox comboBoxCountry;
        private IContainer components;
        private Label labelAccountTitle;
        private Label labelContactSelect;
        private Label labelCountry;
        private Label labelPassword;
        private Label labelPhoneNumber;
        private string strError = string.Empty;
        private TextBox textBoxAccountTitle;
        private TextBox textBoxPassword;
        private MaskedTextBox textBoxPhoneNumber;

        public fmEditAccounts()
        {
            this.InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.ResetDisplay();
            this.textBoxPhoneNumber.Focus();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            this.textBoxPhoneNumber.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            string strPhoneDigits = this.appManager.FormatContactAddress(this.textBoxPhoneNumber.Text, true, false);
            this.buttonDelete.Enabled = false;
            try
            {
                string[] textArray1 = new string[] { "Delete account ", this.textBoxAccountTitle.Text, " ", this.textBoxPhoneNumber.Text, "?" };
                if (MessageBox.Show(string.Concat(textArray1), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    if (this.appManager.m_bAccountDashboardLoading)
                    {
                        MessageBox.Show("The Dashboard is currently refreshing accounts, please wait until complete to delete account.", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        this.buttonDelete.Enabled = true;
                        return;
                    }
                    AppRegistry.GetRootKey(ref this.strError).DeleteSubKeyTree(strPhoneDigits);
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
                text = "Account deleted";
                this.appManager.ShowBalloon(text, 5);
                this.appManager.m_lsAccountItems = (from val in this.appManager.m_lsAccountItems
                    where val.number != strPhoneDigits
                    select val).ToList<AccountItem>();
            }
            this.buttonDelete.Enabled = true;
            this.ResetDisplay();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.appManager.m_bAccountDashboardLoading)
            {
                MessageBox.Show("The Dashboard is currently refreshing accounts, please wait until complete to save account.", this.appManager.m_strApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                this.textBoxPhoneNumber.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                if (this.textBoxPhoneNumber.Text.Length != 0)
                {
                    try
                    {
                        this.buttonSave.Enabled = false;
                        if (this.bNewAccount)
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
                            if (this.appManager.GetAccountItem(this.textBoxPhoneNumber.Text).number != null)
                            {
                                this.strError = "Phone number is already saved, please select from the dropdown and update...";
                                this.comboBoxAccountList.Focus();
                            }
                            if (this.textBoxPassword.Text.Length == 0)
                            {
                                this.strError = "Please enter a password...";
                                this.textBoxPassword.Select();
                            }
                            if (this.comboBoxCountry.SelectedValue.ToString() == "")
                            {
                                this.strError = "Please select a country code...";
                                this.comboBoxCountry.Focus();
                            }
                        }
                        if (string.IsNullOrEmpty(this.strError))
                        {
                            bool flag = false;
                            bool flag2 = false;
                            string session = string.Empty;
                            this.appManager.ShowBalloon("Please wait, account information is being verified...", 5);
                            string strPhoneDigits = this.appManager.FormatContactAddress(this.textBoxPhoneNumber.Text, true, false);
                            if (this.appManager.m_textService == null)
                            {
                                this.appManager.m_textService = new TextService();
                            }
                            try
                            {
                                if (AppRegistry.AuthorizeUser(strPhoneDigits, ref this.strError))
                                {
                                    flag = this.appManager.m_textService.AccountLogIn(strPhoneDigits, this.textBoxPassword.Text, this.comboBoxCountry.SelectedValue.ToString(), ref session);
                                }
                            }
                            catch
                            {
                                flag2 = true;
                            }
                            if (flag2)
                            {
                                this.strError = "Exception connecting to Text Service\n\nPlease check your internet connection...";
                            }
                            else if (!flag)
                            {
                                this.strError = "Account Failed Authentication.\n\nPlease check your log in information and selected country...";
                            }
                            else
                            {
                                RegistryKey key = AppRegistry.GetSubKey(AppRegistry.GetRootKey(ref this.strError), strPhoneDigits, true, ref this.strError);
                                if (this.strError == string.Empty)
                                {
                                    AppRegistry.SaveUserName(key, strPhoneDigits, ref this.strError);
                                    AppRegistry.SavePassword(key, this.textBoxPassword.Text, ref this.strError);
                                    AppRegistry.SaveValue(key, "CountryCode", this.comboBoxCountry.SelectedValue.ToString(), ref this.strError, false, RegistryValueKind.Unknown);
                                    AppRegistry.SaveValue(key, "Title", this.textBoxAccountTitle.Text, ref this.strError, false, RegistryValueKind.Unknown);
                                    AppRegistry.SaveValue(key, "Session", session, ref this.strError, false, RegistryValueKind.Unknown);
                                }
                                if (this.strError == string.Empty)
                                {
                                    this.appManager.m_lsAccountItems = (from val in this.appManager.m_lsAccountItems
                                        where val.number != strPhoneDigits
                                        select val).ToList<AccountItem>();
                                    AccountItem item = new AccountItem {
                                        countryCode = this.comboBoxCountry.SelectedValue.ToString(),
                                        number = strPhoneDigits,
                                        password = this.textBoxPassword.Text,
                                        title = this.textBoxAccountTitle.Text,
                                        session = session,
                                        unReadMessageList = new List<TextMessage>(),
                                        connectionStatus = "Connected"
                                    };
                                    this.appManager.m_lsAccountItems.Add(item);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        this.strError = "Error saving account: " + exception.Message;
                    }
                    if (!string.IsNullOrEmpty(this.strError))
                    {
                        MessageBox.Show(this.strError, this.appManager.m_strApplicationName + " Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.strError = string.Empty;
                        this.buttonSave.Enabled = true;
                    }
                    else
                    {
                        this.appManager.ShowBalloon("Account has been verified and saved.", 5);
                        this.ResetDisplay();
                    }
                    this.buttonSave.Enabled = true;
                }
            }
        }

        private void comboBoxAccountList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Contains(e.KeyChar.ToString()))
            {
                this.comboBoxAccountList_Load(this.comboBoxAccountList.Text + e.KeyChar.ToString());
                this.comboBoxAccountList.Select(this.comboBoxAccountList.Text.Length + 1, 0);
                e.Handled = true;
            }
            else if (e.KeyChar == '\b')
            {
                if (this.comboBoxAccountList.Text.Length > 0)
                {
                    this.comboBoxAccountList_Load(this.comboBoxAccountList.Text.Substring(0, this.comboBoxAccountList.Text.Length - 1));
                    this.comboBoxAccountList.Select(this.comboBoxAccountList.Text.Length + 1, 0);
                }
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                this.bNotAddedcomboBox = true;
            }
            this.comboBoxAccountList.DroppedDown = true;
        }

        private void comboBoxAccountList_Leave(object sender, EventArgs e)
        {
            this.bAutoAddRecipient = true;
            if (this.comboBoxAccountList.SelectedIndex > 0)
            {
                this.DisplayAccount();
            }
            this.bAutoAddRecipient = false;
        }

        public void comboBoxAccountList_Load(string match)
        {
            List<AccountListItem> collection = new List<AccountListItem>();
            List<AccountListItem> list2 = new List<AccountListItem>();
            AccountListItem item = new AccountListItem {
                phoneNumber = "",
                display = match
            };
            list2.Add(item);
            foreach (AccountItem item2 in this.appManager.m_lsAccountItems)
            {
                item = new AccountListItem();
                string str = (this.appManager.FormatPhone(item2.number.ToString()) + " " + item2.title).Trim();
                item.phoneNumber = item2.number.ToString();
                item.display = str;
                if (string.IsNullOrEmpty(match))
                {
                    collection.Add(item);
                }
                else
                {
                    string str2 = this.appManager.FormatAlphaNumeric(match);
                    if (item.display.ToLower().Contains(match.ToLower()))
                    {
                        collection.Add(item);
                    }
                    else if ((str2 != "") && item.phoneNumber.Contains(str2))
                    {
                        collection.Add(item);
                    }
                }
            }
            collection = (from a in collection
                orderby a.display
                select a).ToList<AccountListItem>();
            list2.AddRange(collection);
            this.comboBoxAccountList.DataSource = list2;
        }

        private void comboBoxAccountList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxAccountList.Text.Length > 0)
            {
                this.comboBoxAccountList.DroppedDown = true;
            }
        }

        private void comboBoxAccountList_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if ((this.comboBoxAccountList.DroppedDown && !this.bAutoAddRecipient) && (this.comboBoxAccountList.SelectedIndex > 0))
            {
                this.DisplayAccount();
            }
        }

        private void comboBoxAccountList_TextChanged(object sender, EventArgs e)
        {
            if (this.bNotAddedcomboBox)
            {
                this.comboBoxAccountList_Load(this.comboBoxAccountList.Text);
                this.comboBoxAccountList.Select(this.comboBoxAccountList.Text.Length + 1, 0);
                this.bNotAddedcomboBox = false;
            }
        }

        public void DisplayAccount()
        {
            this.textBoxPhoneNumber.Mask = null;
            AccountItem accountItem = new AccountItem();
            if (this.comboBoxAccountList.SelectedIndex > 0)
            {
                AccountListItem selectedItem = (AccountListItem) this.comboBoxAccountList.SelectedItem;
                accountItem = this.appManager.GetAccountItem(selectedItem.phoneNumber);
            }
            if (accountItem.number.Length > 0)
            {
                this.textBoxPhoneNumber.Text = this.appManager.FormatPhone(accountItem.number);
                this.textBoxAccountTitle.Text = accountItem.title;
                this.textBoxPassword.Text = accountItem.password;
                this.comboBoxCountry.SelectedValue = accountItem.countryCode;
            }
            this.comboBoxAccountList_Load(string.Empty);
            this.textBoxPhoneNumber.Enabled = false;
            this.bNewAccount = false;
            this.buttonDelete.Enabled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fmEditAccounts_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormEditAccountsWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormEditAccountsHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmEditAccounts_Load(object sender, EventArgs e)
        {
            this.Text = this.appManager.m_strApplicationName + " Edit Accounts";
            base.Icon = this.appManager.iTextApp;
            int num = 0;
            int num2 = 0;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditAccountsWidth", ref num, ref this.strError);
            AppRegistry.GetValue(rootKey, "local_FormEditAccountsHeight", ref num2, ref this.strError);
            if (num2 != 0)
            {
                base.Height = num2;
            }
            if (num != 0)
            {
                base.Width = num;
            }
            this.comboBoxAccountList.ValueMember = "phoneNumber";
            this.comboBoxAccountList.DisplayMember = "display";
            this.comboBoxAccountList_Load(string.Empty);
            this.LoadCountryDDL();
            this.comboBoxCountry.SelectedValue = "USA";
            if (this.bNewAccount)
            {
                this.textBoxPhoneNumber.Enabled = true;
                this.textBoxPhoneNumber.Focus();
            }
            else
            {
                try
                {
                    this.DisplayAccount();
                    this.textBoxPassword.Focus();
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
            if (this.appManager.m_bValidateMobileNumbers && this.bNewAccount)
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
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmEditAccounts));
            this.textBoxAccountTitle = new TextBox();
            this.textBoxPassword = new TextBox();
            this.labelAccountTitle = new Label();
            this.labelPassword = new Label();
            this.labelPhoneNumber = new Label();
            this.buttonSave = new Button();
            this.textBoxPhoneNumber = new MaskedTextBox();
            this.comboBoxAccountList = new ComboBox();
            this.buttonClear = new Button();
            this.buttonDelete = new Button();
            this.comboBoxCountry = new ComboBox();
            this.labelCountry = new Label();
            this.labelContactSelect = new Label();
            base.SuspendLayout();
            this.textBoxAccountTitle.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxAccountTitle.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxAccountTitle.Location = new Point(130, 220);
            this.textBoxAccountTitle.Margin = new Padding(4);
            this.textBoxAccountTitle.MaxLength = 50;
            this.textBoxAccountTitle.Name = "textBoxAccountTitle";
            this.textBoxAccountTitle.Size = new Size(0xbc, 0x19);
            this.textBoxAccountTitle.TabIndex = 5;
            this.textBoxPassword.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxPassword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxPassword.Location = new Point(130, 0x81);
            this.textBoxPassword.Margin = new Padding(4);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new Size(0xbd, 0x19);
            this.textBoxPassword.TabIndex = 3;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.labelAccountTitle.AutoSize = true;
            this.labelAccountTitle.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelAccountTitle.Location = new Point(13, 0xdf);
            this.labelAccountTitle.Margin = new Padding(4, 0, 4, 0);
            this.labelAccountTitle.Name = "labelAccountTitle";
            this.labelAccountTitle.Size = new Size(0x5f, 0x11);
            this.labelAccountTitle.TabIndex = 2;
            this.labelAccountTitle.Text = "Account Title:";
            this.labelPassword.AutoSize = true;
            this.labelPassword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelPassword.Location = new Point(13, 0x84);
            this.labelPassword.Margin = new Padding(4, 0, 4, 0);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new Size(0x4e, 0x11);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password:";
            this.labelPhoneNumber.AutoSize = true;
            this.labelPhoneNumber.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelPhoneNumber.Location = new Point(13, 0x55);
            this.labelPhoneNumber.Margin = new Padding(4, 0, 4, 0);
            this.labelPhoneNumber.Name = "labelPhoneNumber";
            this.labelPhoneNumber.Size = new Size(110, 0x11);
            this.labelPhoneNumber.TabIndex = 4;
            this.labelPhoneNumber.Text = "Phone Number:";
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x106, 0x103);
            this.buttonSave.Margin = new Padding(4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x38, 0x1b);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.textBoxPhoneNumber.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxPhoneNumber.Location = new Point(130, 0x52);
            this.textBoxPhoneNumber.Mask = "(000) 000-0000";
            this.textBoxPhoneNumber.Name = "textBoxPhoneNumber";
            this.textBoxPhoneNumber.Size = new Size(0xbc, 0x19);
            this.textBoxPhoneNumber.TabIndex = 2;
            this.textBoxPhoneNumber.Click += new EventHandler(this.textBoxPhoneNumbere_Click);
            this.comboBoxAccountList.AllowDrop = true;
            this.comboBoxAccountList.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxAccountList.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxAccountList.FormattingEnabled = true;
            this.comboBoxAccountList.Location = new Point(0x10, 0x10);
            this.comboBoxAccountList.Name = "comboBoxAccountList";
            this.comboBoxAccountList.Size = new Size(0x12e, 0x19);
            this.comboBoxAccountList.TabIndex = 1;
            this.comboBoxAccountList.SelectedIndexChanged += new EventHandler(this.comboBoxAccountList_SelectedIndexChanged);
            this.comboBoxAccountList.SelectionChangeCommitted += new EventHandler(this.comboBoxAccountList_SelectionChangeCommitted);
            this.comboBoxAccountList.TextChanged += new EventHandler(this.comboBoxAccountList_TextChanged);
            this.comboBoxAccountList.KeyPress += new KeyPressEventHandler(this.comboBoxAccountList_KeyPress);
            this.comboBoxAccountList.Leave += new EventHandler(this.comboBoxAccountList_Leave);
            this.buttonClear.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonClear.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonClear.Location = new Point(0x10, 0x103);
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
            this.buttonDelete.Location = new Point(0xbb, 0x103);
            this.buttonDelete.Margin = new Padding(4);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x43, 0x1b);
            this.buttonDelete.TabIndex = 11;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.comboBoxCountry.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCountry.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxCountry.FormattingEnabled = true;
            this.comboBoxCountry.Location = new Point(130, 0xaf);
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.Size = new Size(0xbc, 0x19);
            this.comboBoxCountry.TabIndex = 4;
            this.labelCountry.AutoSize = true;
            this.labelCountry.BackColor = System.Drawing.Color.Transparent;
            this.labelCountry.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelCountry.ForeColor = System.Drawing.Color.Black;
            this.labelCountry.Location = new Point(13, 0xb2);
            this.labelCountry.Name = "labelCountry";
            this.labelCountry.Size = new Size(0x3f, 0x11);
            this.labelCountry.TabIndex = 15;
            this.labelCountry.Text = "Country:";
            this.labelContactSelect.AutoSize = true;
            this.labelContactSelect.Font = new Font("Arial", 8.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelContactSelect.ForeColor = System.Drawing.Color.FromArgb(0x40, 0x40, 0x40);
            this.labelContactSelect.Location = new Point(14, 0x2c);
            this.labelContactSelect.MaximumSize = new Size(310, 0);
            this.labelContactSelect.Name = "labelContactSelect";
            this.labelContactSelect.Size = new Size(0x132, 0x1c);
            this.labelContactSelect.TabIndex = 0x16;
            this.labelContactSelect.Text = "Select an account to edit.  Click the Clear button to reset the form and add a new account.";
            base.AutoScaleDimensions = new SizeF(8f, 17f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x14e, 0x12b);
            base.Controls.Add(this.labelContactSelect);
            base.Controls.Add(this.comboBoxCountry);
            base.Controls.Add(this.labelCountry);
            base.Controls.Add(this.buttonDelete);
            base.Controls.Add(this.buttonClear);
            base.Controls.Add(this.comboBoxAccountList);
            base.Controls.Add(this.textBoxPhoneNumber);
            base.Controls.Add(this.buttonSave);
            base.Controls.Add(this.labelPhoneNumber);
            base.Controls.Add(this.labelPassword);
            base.Controls.Add(this.labelAccountTitle);
            base.Controls.Add(this.textBoxPassword);
            base.Controls.Add(this.textBoxAccountTitle);
            this.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Margin = new Padding(4);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            this.MinimumSize = new Size(350, 0x151);
            base.Name = "fmEditAccounts";
            this.Text = "Edit Accounts";
            base.FormClosing += new FormClosingEventHandler(this.fmEditAccounts_FormClosing);
            base.Load += new EventHandler(this.fmEditAccounts_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void LoadCountryDDL()
        {
            this.comboBoxCountry.DisplayMember = "Text";
            this.comboBoxCountry.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "United States",
                Value = "USA"
            }, new { 
                Text = "Canada",
                Value = "CAN"
            } };
            this.comboBoxCountry.DataSource = typeArray;
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
            this.textBoxAccountTitle.Text = string.Empty;
            this.textBoxPassword.Text = string.Empty;
            this.comboBoxAccountList_Load(string.Empty);
            this.LoadCountryDDL();
            this.buttonDelete.Enabled = false;
            this.textBoxPhoneNumber.Enabled = true;
            this.comboBoxAccountList.Focus();
            this.bNewAccount = true;
        }

        private void textBoxPhoneNumbere_Click(object sender, EventArgs e)
        {
            this.PositionCursorInMaskedTextBox(this.textBoxPhoneNumber);
        }

        public ApplicationManager appManager { get; set; }

        public bool bNewAccount { get; set; }

        /*[Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly fmEditAccounts.<>c <>9 = new fmEditAccounts.<>c();
            public static Func<fmEditAccounts.AccountListItem, string> <>9__16_0;

            internal string <comboBoxAccountList_Load>b__16_0(fmEditAccounts.AccountListItem a) => 
                a.display;
        }*/

        [StructLayout(LayoutKind.Sequential)]
        private struct AccountListItem
        {
            public string display { get; set; }
            public string phoneNumber { get; set; }
        }
    }
}


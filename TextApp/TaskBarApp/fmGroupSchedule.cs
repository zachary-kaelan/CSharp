namespace TaskBarApp
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;

    public class fmGroupSchedule : Form
    {
        private bool bRefreshMessageForm;
        private Button buttonAddNew;
        private Button buttonClearFile;
        private Button buttonDelete;
        private Button buttonOpenFile;
        private Button buttonRefresh;
        private Button buttonSave;
        private CheckBox checkBoxEnableGroupSchedule;
        private DataGridViewTextBoxColumn ColumnAddress;
        private DataGridViewTextBoxColumn ColumnSendDate;
        private DataGridViewTextBoxColumn ColumnSendStatus;
        private DataGridViewTextBoxColumn ColumnTextMessage;
        private IContainer components;
        private DataGridView dataGridViewGroupScheduleFile;
        private Label labelProcessing;
        private string strError = string.Empty;
        private TabControl tabControlSchedule;
        private TabPage tabPage1;

        public fmGroupSchedule()
        {
            this.InitializeComponent();
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            this.appManager.ShowEditGroupSchedule();
        }

        private void buttonClearFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all records and reset Group Schedule File?\n\n Please Note this cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.appManager.GroupSchedulFileInitialize(true);
                this.appManager.GroupSchedulFileLoad();
                this.dataGridViewGroupScheduleFile_Load();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if ((this.dataGridViewGroupScheduleFile.SelectedRows.Count > 0) && (MessageBox.Show("Delete the selected row?\n\n Please Note this cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
            {
                this.dataGridViewGroupScheduleFile.Rows.RemoveAt(this.dataGridViewGroupScheduleFile.SelectedRows[0].Index);
                this.dataGridViewGroupScheduleFile_Save();
                this.appManager.GroupSchedulFileLoad();
                this.dataGridViewGroupScheduleFile_Load();
            }
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            this.strError = string.Empty;
            string errorMessage = string.Empty;
            if (MessageBox.Show("Group Schedule Processing must be disabled to open the raw schedule file to avoid file lock conflicts.\r\n\r\nWhile working with the file you MUST keep the header row unchanged or the file will be rejected.  You MUST not populate the SendStatus column or the record will not be processed.\r\n\r\nOnce additions to the file are complete, open the Edit Group Schedule screen re-enable the process and click on Validate & Save button.\r\n\r\nDo you want to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "EnableGroupScheduleProcessing", false, ref errorMessage, false, RegistryValueKind.Unknown);
                    if (errorMessage != string.Empty)
                    {
                        this.strError = this.strError + "Disable Group Schedule Processing save error: " + errorMessage;
                    }
                    else
                    {
                        this.appManager.m_bEnableGroupScheduleProcessing = false;
                    }
                }
                catch (Exception exception)
                {
                    this.strError = this.strError + "Disable Group Schedule Processing save error: " + exception.Message;
                }
                if (this.strError.Length == 0)
                {
                    base.Close();
                    Process.Start(this.appManager.m_strCSVScheduleFilePath);
                }
                else
                {
                    this.appManager.ShowBalloon("Exception opening raw file: " + this.strError, 5);
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            this.appManager.GroupSchedulFileLoad();
            this.dataGridViewGroupScheduleFile_Load();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.dataGridViewGroupScheduleFile_Save())
            {
                this.appManager.ShowBalloon("The file has been saved, please check the status column for details on validation.", 5);
                if ((this.appManager.formMessages != null) && this.bRefreshMessageForm)
                {
                    this.appManager.formMessages.Close();
                    this.appManager.ShowMessages();
                }
            }
        }

        private void checkBoxEnableGroupSchedule_CheckedChanged(object sender, EventArgs e)
        {
            this.strError = string.Empty;
            string errorMessage = string.Empty;
            if (this.appManager.m_bEnableGroupScheduleProcessing != this.checkBoxEnableGroupSchedule.Checked)
            {
                try
                {
                    AppRegistry.SaveValue(AppRegistry.GetRootKey(ref errorMessage), "EnableGroupScheduleProcessing", this.checkBoxEnableGroupSchedule.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                    if (errorMessage != string.Empty)
                    {
                        this.strError = this.strError + errorMessage;
                    }
                    else
                    {
                        this.appManager.m_bEnableGroupScheduleProcessing = this.checkBoxEnableGroupSchedule.Checked;
                        this.bRefreshMessageForm = true;
                    }
                }
                catch (Exception exception)
                {
                    this.strError = this.strError + exception.Message;
                }
                if (this.strError.Length == 0)
                {
                    this.appManager.ShowBalloon("Setting saved.", 5);
                }
                else
                {
                    this.appManager.ShowBalloon("checkBoxEnableGroupSchedule_CheckedChanged error: " + this.strError, 5);
                }
            }
        }

        public bool dataGridViewGroupScheduleFile_Load()
        {
            bool flag = true;
            try
            {
                this.dataGridViewGroupScheduleFile.Rows.Clear();
                foreach (ScheduleFileItem item in this.appManager.m_lsGroupSchedule)
                {
                    DateTime time;
                    string sendDate = item.SendDate;
                    DateTime.TryParse(item.SendDate, out time);
                    string address = item.Address;
                    if (!address.Contains("#"))
                    {
                        address = this.appManager.FormatPhone(item.Address);
                    }
                    object[] values = new object[] { address, time, item.TextMessage, item.SendStatus };
                    this.dataGridViewGroupScheduleFile.Rows.Add(values);
                }
                this.dataGridViewGroupScheduleFile.Sort(this.dataGridViewGroupScheduleFile.Columns[1], ListSortDirection.Ascending);
                if (this.dataGridViewGroupScheduleFile.RowCount > 0)
                {
                    this.dataGridViewGroupScheduleFile.CurrentCell = this.dataGridViewGroupScheduleFile.Rows[0].Cells[0];
                }
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("dataGridViewGroupScheduleFile_Load error: " + exception.Message, 5);
                flag = false;
            }
            return flag;
        }

        private bool dataGridViewGroupScheduleFile_Save()
        {
            bool flag = true;
            try
            {
                List<ScheduleFileItem> csvRecords = new List<ScheduleFileItem>();
                foreach (DataGridViewRow row in (IEnumerable) this.dataGridViewGroupScheduleFile.Rows)
                {
                    if (row.Cells[0].Value == null)
                    {
                        break;
                    }
                    string str = row.Cells[0].Value.ToString();
                    string str2 = row.Cells[1].Value.ToString();
                    string str3 = row.Cells[2].Value.ToString();
                    string str4 = "";
                    if (row.Cells[3].Value != null)
                    {
                        str4 = row.Cells[3].Value.ToString();
                    }
                    ScheduleFileItem validateItem = new ScheduleFileItem {
                        Address = str,
                        SendDate = str2,
                        TextMessage = str3,
                        SendStatus = str4
                    };
                    validateItem = this.appManager.GroupScheduleFileValidateItem(validateItem);
                    csvRecords.Add(validateItem);
                }
                this.appManager.GroupSchedulFileSave(csvRecords);
                this.dataGridViewGroupScheduleFile_Load();
            }
            catch (Exception exception)
            {
                this.strError = "dataGridViewGroupScheduleFile_Save error: " + exception.Message;
                flag = false;
            }
            return flag;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fmGroupSchedule_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormGroupScheduleWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormGroupScheduleHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void GroupSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = this.appManager.m_strApplicationName + " Group Schedule";
                base.Icon = this.appManager.iTextApp;
                int num = 0;
                int num2 = 0;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormGroupScheduleWidth", ref num, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormGroupScheduleHeight", ref num2, ref this.strError);
                if (num2 != 0)
                {
                    base.Height = num2;
                }
                if (num != 0)
                {
                    base.Width = num;
                }
                this.labelProcessing.BackColor = ColorTranslator.FromHtml("#93FF14");
                this.labelProcessing.Visible = false;
                this.dataGridViewGroupScheduleFile.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                this.dataGridViewGroupScheduleFile.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E8FFCC");
                this.dataGridViewGroupScheduleFile.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
                if (this.appManager.m_bGroupScheduleProcessing)
                {
                    this.labelProcessing.Visible = true;
                    this.buttonOpenFile.Enabled = false;
                    this.buttonSave.Enabled = false;
                }
                else
                {
                    this.labelProcessing.Visible = false;
                    this.buttonOpenFile.Enabled = true;
                    this.buttonSave.Enabled = true;
                }
                if (this.appManager.m_bEnableGroupScheduleProcessing)
                {
                    this.checkBoxEnableGroupSchedule.Checked = true;
                }
                this.dataGridViewGroupScheduleFile_Load();
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception creating Group Schedule form: " + exception.Message, 5);
            }
        }

        private void InitializeComponent()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmGroupSchedule));
            this.checkBoxEnableGroupSchedule = new CheckBox();
            this.buttonSave = new Button();
            this.buttonRefresh = new Button();
            this.buttonOpenFile = new Button();
            this.labelProcessing = new Label();
            this.tabPage1 = new TabPage();
            this.dataGridViewGroupScheduleFile = new DataGridView();
            this.ColumnAddress = new DataGridViewTextBoxColumn();
            this.ColumnSendDate = new DataGridViewTextBoxColumn();
            this.ColumnTextMessage = new DataGridViewTextBoxColumn();
            this.ColumnSendStatus = new DataGridViewTextBoxColumn();
            this.tabControlSchedule = new TabControl();
            this.buttonAddNew = new Button();
            this.buttonDelete = new Button();
            this.buttonClearFile = new Button();
            this.tabPage1.SuspendLayout();
            ((ISupportInitialize) this.dataGridViewGroupScheduleFile).BeginInit();
            this.tabControlSchedule.SuspendLayout();
            base.SuspendLayout();
            this.checkBoxEnableGroupSchedule.AutoSize = true;
            this.checkBoxEnableGroupSchedule.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxEnableGroupSchedule.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxEnableGroupSchedule.Location = new Point(12, 12);
            this.checkBoxEnableGroupSchedule.Name = "checkBoxEnableGroupSchedule";
            this.checkBoxEnableGroupSchedule.Size = new Size(0x103, 0x15);
            this.checkBoxEnableGroupSchedule.TabIndex = 0x26;
            this.checkBoxEnableGroupSchedule.Text = "Enable Group Schedule Processing";
            this.checkBoxEnableGroupSchedule.UseVisualStyleBackColor = false;
            this.checkBoxEnableGroupSchedule.CheckedChanged += new EventHandler(this.checkBoxEnableGroupSchedule_CheckedChanged);
            this.buttonSave.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(360, 0x16b);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0xa2, 0x1b);
            this.buttonSave.TabIndex = 0x27;
            this.buttonSave.Text = "Validate && Save File";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.buttonRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonRefresh.Location = new Point(0x98, 0x16b);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new Size(0x66, 0x1b);
            this.buttonRefresh.TabIndex = 40;
            this.buttonRefresh.Text = "Reload File";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
            this.buttonOpenFile.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonOpenFile.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonOpenFile.Location = new Point(12, 0x16b);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new Size(0x86, 0x1b);
            this.buttonOpenFile.TabIndex = 0x29;
            this.buttonOpenFile.Text = "Access Raw File";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new EventHandler(this.buttonOpenFile_Click);
            this.labelProcessing.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.labelProcessing.AutoSize = true;
            this.labelProcessing.Location = new Point(550, 20);
            this.labelProcessing.Name = "labelProcessing";
            this.labelProcessing.Size = new Size(0xca, 13);
            this.labelProcessing.TabIndex = 0x2a;
            this.labelProcessing.Text = "Group Schedule is Currently Processing...";
            this.labelProcessing.TextAlign = ContentAlignment.MiddleRight;
            this.tabPage1.Controls.Add(this.dataGridViewGroupScheduleFile);
            this.tabPage1.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tabPage1.Location = new Point(4, 0x1a);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(0x2dc, 0x117);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Schedule List";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.dataGridViewGroupScheduleFile.AllowUserToAddRows = false;
            this.dataGridViewGroupScheduleFile.AllowUserToOrderColumns = true;
            this.dataGridViewGroupScheduleFile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewGroupScheduleFile.BackgroundColor = SystemColors.Control;
            this.dataGridViewGroupScheduleFile.BorderStyle = BorderStyle.None;
            this.dataGridViewGroupScheduleFile.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewGroupScheduleFile.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewColumn[] dataGridViewColumns = new DataGridViewColumn[] { this.ColumnAddress, this.ColumnSendDate, this.ColumnTextMessage, this.ColumnSendStatus };
            this.dataGridViewGroupScheduleFile.Columns.AddRange(dataGridViewColumns);
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Window;
            style.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.ControlText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this.dataGridViewGroupScheduleFile.DefaultCellStyle = style;
            this.dataGridViewGroupScheduleFile.Dock = DockStyle.Fill;
            this.dataGridViewGroupScheduleFile.Location = new Point(3, 3);
            this.dataGridViewGroupScheduleFile.MultiSelect = false;
            this.dataGridViewGroupScheduleFile.Name = "dataGridViewGroupScheduleFile";
            this.dataGridViewGroupScheduleFile.ReadOnly = true;
            this.dataGridViewGroupScheduleFile.RowTemplate.Height = 40;
            this.dataGridViewGroupScheduleFile.ScrollBars = ScrollBars.Vertical;
            this.dataGridViewGroupScheduleFile.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewGroupScheduleFile.Size = new Size(0x2d6, 0x111);
            this.dataGridViewGroupScheduleFile.TabIndex = 0;
            this.ColumnAddress.FillWeight = 50f;
            this.ColumnAddress.HeaderText = "Address";
            this.ColumnAddress.Name = "ColumnAddress";
            this.ColumnAddress.ReadOnly = true;
            this.ColumnSendDate.FillWeight = 50f;
            this.ColumnSendDate.HeaderText = "Send Date";
            this.ColumnSendDate.Name = "ColumnSendDate";
            this.ColumnSendDate.ReadOnly = true;
            this.ColumnTextMessage.HeaderText = "Text Message";
            this.ColumnTextMessage.Name = "ColumnTextMessage";
            this.ColumnTextMessage.ReadOnly = true;
            this.ColumnSendStatus.FillWeight = 50f;
            this.ColumnSendStatus.HeaderText = "Status";
            this.ColumnSendStatus.Name = "ColumnSendStatus";
            this.ColumnSendStatus.ReadOnly = true;
            this.tabControlSchedule.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.tabControlSchedule.Controls.Add(this.tabPage1);
            this.tabControlSchedule.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tabControlSchedule.Location = new Point(12, 0x30);
            this.tabControlSchedule.Name = "tabControlSchedule";
            this.tabControlSchedule.SelectedIndex = 0;
            this.tabControlSchedule.Size = new Size(740, 0x135);
            this.tabControlSchedule.TabIndex = 0x2b;
            this.buttonAddNew.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonAddNew.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonAddNew.Location = new Point(0x299, 0x16b);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new Size(0x53, 0x1b);
            this.buttonAddNew.TabIndex = 0x2c;
            this.buttonAddNew.Text = "Add New";
            this.buttonAddNew.UseVisualStyleBackColor = true;
            this.buttonAddNew.Click += new EventHandler(this.buttonAddNew_Click);
            this.buttonDelete.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonDelete.Location = new Point(530, 0x16b);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new Size(0x81, 0x1b);
            this.buttonDelete.TabIndex = 0x2d;
            this.buttonDelete.Text = "Delete Selected";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
            this.buttonClearFile.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            this.buttonClearFile.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonClearFile.Location = new Point(260, 0x16b);
            this.buttonClearFile.Name = "buttonClearFile";
            this.buttonClearFile.Size = new Size(0x5e, 0x1b);
            this.buttonClearFile.TabIndex = 0x2e;
            this.buttonClearFile.Text = "Clear File";
            this.buttonClearFile.UseVisualStyleBackColor = true;
            this.buttonClearFile.Click += new EventHandler(this.buttonClearFile_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x2fc, 0x192);
            base.Controls.Add(this.buttonClearFile);
            base.Controls.Add(this.buttonDelete);
            base.Controls.Add(this.buttonAddNew);
            base.Controls.Add(this.tabControlSchedule);
            base.Controls.Add(this.labelProcessing);
            base.Controls.Add(this.buttonOpenFile);
            base.Controls.Add(this.buttonRefresh);
            base.Controls.Add(this.buttonSave);
            base.Controls.Add(this.checkBoxEnableGroupSchedule);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            this.MinimumSize = new Size(780, 440);
            base.Name = "fmGroupSchedule";
            this.Text = "Group Schedule";
            base.FormClosing += new FormClosingEventHandler(this.fmGroupSchedule_FormClosing);
            base.Load += new EventHandler(this.GroupSchedule_Load);
            this.tabPage1.ResumeLayout(false);
            ((ISupportInitialize) this.dataGridViewGroupScheduleFile).EndInit();
            this.tabControlSchedule.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public ApplicationManager appManager { get; set; }
    }
}


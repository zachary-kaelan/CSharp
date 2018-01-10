namespace TaskBarApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;
    using TaskBarApp.Objects;

    public class fmEditGroupSchedule : Form
    {
        private Button buttonSave;
        private CheckBox checkBoxRepeat;
        private ComboBox comboBoxGroups;
        private ComboBox comboBoxRepeat;
        private IContainer components;
        private DateTimePicker dateTimePickerStart;
        private DateTimePicker dateTimePickerStop;
        private GroupBox groupBoxRepeat;
        private Label labelGroupTag;
        private Label labelMessage;
        private Label labelRepeatCount;
        private Label labelStartDate;
        private Label labelUntil;
        private Label lblCharCount;
        private List<ScheduleFileItem> lsSaveItems = new List<ScheduleFileItem>();
        private string strError = string.Empty;
        private TextBox textBoxNewMessage;

        public fmEditGroupSchedule()
        {
            this.InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.appManager.m_bGroupScheduleProcessing)
            {
                MessageBox.Show("Group Schedule Send is currently processing, please wait a moment and try again...", this.appManager.m_strApplicationName, MessageBoxButtons.OK);
            }
            else if (this.ValidateForm())
            {
                if (this.checkBoxRepeat.Checked)
                {
                    this.SetSaveItemsList();
                }
                else
                {
                    this.lsSaveItems.Clear();
                    ScheduleFileItem item = new ScheduleFileItem {
                        Address = this.comboBoxGroups.Text,
                        SendDate = this.dateTimePickerStart.Value.ToString(),
                        TextMessage = this.textBoxNewMessage.Text,
                        SendStatus = string.Empty
                    };
                    this.lsSaveItems.Add(item);
                }
                foreach (ScheduleFileItem item2 in this.lsSaveItems)
                {
                    string str = string.Empty;
                    str = this.appManager.GroupSchedulFileAddItem(item2);
                    if (str.Length > 0)
                    {
                        this.strError = this.strError + str;
                    }
                }
                if (this.strError.Length > 0)
                {
                    this.appManager.ShowBalloon("There was error saving the group schedule: " + this.strError, 5);
                }
                else
                {
                    this.appManager.ShowBalloon("Your group schedule has been saved.", 5);
                    base.Close();
                    this.appManager.ShowGroupSchedule();
                }
            }
        }

        private void checkBoxRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxRepeat.Checked)
            {
                this.comboBoxRepeat.Enabled = true;
                this.dateTimePickerStop.Enabled = true;
                this.labelRepeatCount.Enabled = true;
                this.labelRepeatCount.Visible = true;
                this.labelUntil.Enabled = true;
                this.SetSaveItemsList();
            }
            else
            {
                this.comboBoxRepeat.Enabled = false;
                this.dateTimePickerStop.Enabled = false;
                this.labelRepeatCount.Visible = false;
                this.labelUntil.Enabled = false;
            }
        }

        public void comboBoxGroups_Load(string strGroup)
        {
            List<string> list = new List<string> { "[Select Group]" };
            foreach (string str in this.appManager.m_lsGroupTags)
            {
                if (string.IsNullOrEmpty(strGroup))
                {
                    list.Add(str);
                }
                else if (str.ToLower().Contains(strGroup.ToLower()))
                {
                    list.Add(str);
                }
            }
            this.comboBoxGroups.DataSource = list;
        }

        private void comboBoxRepeat_Load()
        {
            this.comboBoxRepeat.DisplayMember = "Text";
            this.comboBoxRepeat.ValueMember = "Value";
            var typeArray = new [] { new { 
                Text = "Daily",
                Value = "1"
            }, new { 
                Text = "Weekly",
                Value = "2"
            }, new { 
                Text = "Monthly",
                Value = "3"
            } };
            this.comboBoxRepeat.DataSource = typeArray;
        }

        private void comboBoxRepeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetSaveItemsList();
        }

        private void dateTimePickerStop_ValueChanged(object sender, EventArgs e)
        {
            this.SetSaveItemsList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void fmEditGroupSchedule_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = this.appManager.m_strApplicationName + " Edit Group Schedule";
                base.Icon = this.appManager.iTextApp;
                this.ResetFrom();
                this.dateTimePickerStart.Format = DateTimePickerFormat.Custom;
                this.dateTimePickerStart.CustomFormat = "ddd dd MMM yyyy h:mm tt";
                this.dateTimePickerStop.Format = DateTimePickerFormat.Custom;
                this.dateTimePickerStop.CustomFormat = "ddd dd MMM yyyy";
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception creating Group Schedule form: " + exception.Message, 5);
            }
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmEditGroupSchedule));
            this.buttonSave = new Button();
            this.textBoxNewMessage = new TextBox();
            this.lblCharCount = new Label();
            this.comboBoxGroups = new ComboBox();
            this.labelGroupTag = new Label();
            this.dateTimePickerStart = new DateTimePicker();
            this.labelStartDate = new Label();
            this.comboBoxRepeat = new ComboBox();
            this.dateTimePickerStop = new DateTimePicker();
            this.labelMessage = new Label();
            this.groupBoxRepeat = new GroupBox();
            this.labelRepeatCount = new Label();
            this.labelUntil = new Label();
            this.checkBoxRepeat = new CheckBox();
            this.groupBoxRepeat.SuspendLayout();
            base.SuspendLayout();
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x137, 0x189);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x3d, 0x1b);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.textBoxNewMessage.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.textBoxNewMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxNewMessage.Location = new Point(12, 0xe2);
            this.textBoxNewMessage.MaxLength = 250;
            this.textBoxNewMessage.Multiline = true;
            this.textBoxNewMessage.Name = "textBoxNewMessage";
            this.textBoxNewMessage.ScrollBars = ScrollBars.Vertical;
            this.textBoxNewMessage.Size = new Size(360, 0x91);
            this.textBoxNewMessage.TabIndex = 6;
            this.textBoxNewMessage.TextChanged += new EventHandler(this.textBoxNewMessage_TextChanged);
            this.lblCharCount.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.lblCharCount.AutoSize = true;
            this.lblCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lblCharCount.ForeColor = Color.FromArgb(0x40, 0x40, 0x40);
            this.lblCharCount.Location = new Point(0x13d, 0x176);
            this.lblCharCount.MinimumSize = new Size(0x37, 0);
            this.lblCharCount.Name = "lblCharCount";
            this.lblCharCount.Size = new Size(0x37, 15);
            this.lblCharCount.TabIndex = 0x2a;
            this.lblCharCount.Text = "0/250";
            this.lblCharCount.TextAlign = ContentAlignment.MiddleRight;
            this.comboBoxGroups.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxGroups.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new Point(0x87, 12);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new Size(0xed, 0x19);
            this.comboBoxGroups.TabIndex = 0;
            this.labelGroupTag.AutoSize = true;
            this.labelGroupTag.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelGroupTag.Location = new Point(12, 15);
            this.labelGroupTag.Name = "labelGroupTag";
            this.labelGroupTag.Size = new Size(0x61, 0x11);
            this.labelGroupTag.TabIndex = 0x2c;
            this.labelGroupTag.Text = "Select Group:";
            this.dateTimePickerStart.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.dateTimePickerStart.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.dateTimePickerStart.Location = new Point(0x87, 0x39);
            this.dateTimePickerStart.MinDate = new DateTime(0x7df, 7, 14, 12, 0x33, 0x17, 0);
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.Size = new Size(0xed, 0x19);
            this.dateTimePickerStart.TabIndex = 1;
            this.dateTimePickerStart.Value = new DateTime(0x7df, 7, 14, 12, 0x33, 0x17, 0);
            this.labelStartDate.AutoSize = true;
            this.labelStartDate.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelStartDate.Location = new Point(12, 0x3f);
            this.labelStartDate.Name = "labelStartDate";
            this.labelStartDate.Size = new Size(0x75, 0x11);
            this.labelStartDate.TabIndex = 0x2e;
            this.labelStartDate.Text = "Send Date/Time:";
            this.comboBoxRepeat.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.comboBoxRepeat.Enabled = false;
            this.comboBoxRepeat.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.comboBoxRepeat.FormattingEnabled = true;
            this.comboBoxRepeat.Location = new Point(6, 0x1d);
            this.comboBoxRepeat.Name = "comboBoxRepeat";
            this.comboBoxRepeat.Size = new Size(0x6c, 0x19);
            this.comboBoxRepeat.TabIndex = 4;
            this.comboBoxRepeat.SelectedIndexChanged += new EventHandler(this.comboBoxRepeat_SelectedIndexChanged);
            this.dateTimePickerStop.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.dateTimePickerStop.Enabled = false;
            this.dateTimePickerStop.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.dateTimePickerStop.Location = new Point(0xa2, 0x1d);
            this.dateTimePickerStop.MinDate = new DateTime(0x7df, 7, 14, 12, 0x33, 0x17, 0);
            this.dateTimePickerStop.Name = "dateTimePickerStop";
            this.dateTimePickerStop.Size = new Size(0xbd, 0x19);
            this.dateTimePickerStop.TabIndex = 5;
            this.dateTimePickerStop.Value = new DateTime(0x7df, 7, 14, 12, 0x33, 0x17, 0);
            this.dateTimePickerStop.ValueChanged += new EventHandler(this.dateTimePickerStop_ValueChanged);
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessage.Location = new Point(12, 0xce);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new Size(0x47, 0x11);
            this.labelMessage.TabIndex = 0x33;
            this.labelMessage.Text = "Message:";
            this.groupBoxRepeat.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBoxRepeat.Controls.Add(this.labelRepeatCount);
            this.groupBoxRepeat.Controls.Add(this.labelUntil);
            this.groupBoxRepeat.Controls.Add(this.checkBoxRepeat);
            this.groupBoxRepeat.Controls.Add(this.comboBoxRepeat);
            this.groupBoxRepeat.Controls.Add(this.dateTimePickerStop);
            this.groupBoxRepeat.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.groupBoxRepeat.Location = new Point(15, 0x67);
            this.groupBoxRepeat.Name = "groupBoxRepeat";
            this.groupBoxRepeat.Size = new Size(0x165, 0x5d);
            this.groupBoxRepeat.TabIndex = 2;
            this.groupBoxRepeat.TabStop = false;
            this.groupBoxRepeat.Text = "Repeat";
            this.labelRepeatCount.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.labelRepeatCount.AutoSize = true;
            this.labelRepeatCount.Enabled = false;
            this.labelRepeatCount.Font = new Font("Arial", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labelRepeatCount.Location = new Point(6, 0x43);
            this.labelRepeatCount.Name = "labelRepeatCount";
            this.labelRepeatCount.Size = new Size(0x7c, 0x10);
            this.labelRepeatCount.TabIndex = 0x36;
            this.labelRepeatCount.Text = "(Will Create 0 Jobs)";
            this.labelRepeatCount.TextAlign = ContentAlignment.MiddleRight;
            this.labelUntil.AutoSize = true;
            this.labelUntil.Enabled = false;
            this.labelUntil.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelUntil.Location = new Point(120, 0x21);
            this.labelUntil.Name = "labelUntil";
            this.labelUntil.Size = new Size(40, 0x11);
            this.labelUntil.TabIndex = 0x35;
            this.labelUntil.Text = "Until:";
            this.checkBoxRepeat.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.checkBoxRepeat.AutoSize = true;
            this.checkBoxRepeat.Font = new Font("Arial", 9f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.checkBoxRepeat.Location = new Point(190, 0);
            this.checkBoxRepeat.Name = "checkBoxRepeat";
            this.checkBoxRepeat.Size = new Size(0xa1, 0x13);
            this.checkBoxRepeat.TabIndex = 3;
            this.checkBoxRepeat.Text = "Click to Edit This Section";
            this.checkBoxRepeat.UseVisualStyleBackColor = true;
            this.checkBoxRepeat.CheckedChanged += new EventHandler(this.checkBoxRepeat_CheckedChanged);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x180, 0x1b0);
            base.Controls.Add(this.groupBoxRepeat);
            base.Controls.Add(this.labelMessage);
            base.Controls.Add(this.labelStartDate);
            base.Controls.Add(this.dateTimePickerStart);
            base.Controls.Add(this.labelGroupTag);
            base.Controls.Add(this.comboBoxGroups);
            base.Controls.Add(this.lblCharCount);
            base.Controls.Add(this.textBoxNewMessage);
            base.Controls.Add(this.buttonSave);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            this.MinimumSize = new Size(400, 470);
            base.Name = "fmEditGroupSchedule";
            this.Text = "EditGroupSchedule";
            base.Load += new EventHandler(this.fmEditGroupSchedule_Load);
            this.groupBoxRepeat.ResumeLayout(false);
            this.groupBoxRepeat.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void ResetFrom()
        {
            DateTime time = DateTime.Now.AddDays(1.0);
            DateTime time2 = new DateTime(time.Year, time.Month, time.Day, 10, 0, 0);
            this.dateTimePickerStart.Value = time2;
            time = time.AddDays(1.0);
            time2 = new DateTime(time.Year, time.Month, time.Day, 0x17, 0x3b, 0x3b);
            this.dateTimePickerStop.Value = time2;
            this.comboBoxGroups_Load(string.Empty);
            this.comboBoxRepeat_Load();
            this.checkBoxRepeat.Checked = false;
            this.labelRepeatCount.Visible = false;
            this.textBoxNewMessage.Text = string.Empty;
        }

        private void SetSaveItemsList()
        {
            string text = this.comboBoxRepeat.Text;
            DateTime time = this.dateTimePickerStop.Value;
            DateTime time2 = this.dateTimePickerStart.Value;
            this.lsSaveItems.Clear();
            while (time2 <= time)
            {
                ScheduleFileItem item = new ScheduleFileItem {
                    SendStatus = string.Empty,
                    Address = this.comboBoxGroups.Text,
                    TextMessage = this.textBoxNewMessage.Text,
                    SendDate = time2.ToString()
                };
                this.lsSaveItems.Add(item);
                if (text == "Monthly")
                {
                    time2 = time2.AddMonths(1);
                }
                else
                {
                    if (text == "Weekly")
                    {
                        time2 = time2.AddDays(7.0);
                        continue;
                    }
                    time2 = time2.AddDays(1.0);
                }
            }
            this.labelRepeatCount.Text = "Repeat Settings Will Create " + this.lsSaveItems.Count.ToString() + " Jobs.";
        }

        private void textBoxNewMessage_TextChanged(object sender, EventArgs e)
        {
            this.lblCharCount.ForeColor = new Color();
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

        private bool ValidateForm()
        {
            DateTime time;
            string text = string.Empty;
            bool flag = true;
            if (this.comboBoxGroups.SelectedIndex == 0)
            {
                text = text + "Please select a group.\n";
            }
            if (this.textBoxNewMessage.Text.Length == 0)
            {
                text = text + "Please enter a text message.\n";
            }
            if (this.checkBoxRepeat.Checked && (this.dateTimePickerStart.Value > this.dateTimePickerStop.Value))
            {
                text = text + "Until date must be greater than the initial Send date.\n";
            }
            if (this.dateTimePickerStart.Value < DateTime.Now)
            {
                text = text + "Send date must be in the future.\n";
            }
            DateTime.TryParse(this.dateTimePickerStart.Value.ToString("dd-MMM-yyyy HH:mm:ss"), out time);
            if (((time.Hour < 9) || (time.Hour > 0x15)) && (MessageBox.Show("Send date is outside of normal business hours...\n\nDo you want to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No))
            {
                flag = false;
            }
            if (text.Length > 0)
            {
                flag = false;
                MessageBox.Show(text, this.appManager.m_strApplicationName, MessageBoxButtons.OK);
            }
            return flag;
        }

        public ApplicationManager appManager { get; set; }
    }
}


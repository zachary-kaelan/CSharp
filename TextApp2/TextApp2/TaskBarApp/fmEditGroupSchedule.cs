using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TaskBarApp.Objects;

namespace TaskBarApp
{
	public class fmEditGroupSchedule : Form
	{
		private string strError = string.Empty;

		private List<ScheduleFileItem> lsSaveItems = new List<ScheduleFileItem>();

		private IContainer components;

		private Button buttonSave;

		private TextBox textBoxNewMessage;

		private Label lblCharCount;

		private ComboBox comboBoxGroups;

		private Label labelGroupTag;

		private DateTimePicker dateTimePickerStart;

		private Label labelStartDate;

		private ComboBox comboBoxRepeat;

		private DateTimePicker dateTimePickerStop;

		private Label labelMessage;

		private GroupBox groupBoxRepeat;

		private CheckBox checkBoxRepeat;

		private Label labelUntil;

		private Label labelRepeatCount;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmEditGroupSchedule()
		{
			this.InitializeComponent();
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
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception creating Group Schedule form: " + ex.Message, 5);
			}
		}

		public void comboBoxGroups_Load(string strGroup)
		{
			List<string> list = new List<string>();
			list.Add("[Select Group]");
			foreach (string current in this.appManager.m_lsGroupTags)
			{
				if (string.IsNullOrEmpty(strGroup))
				{
					list.Add(current);
				}
				else if (current.ToLower().Contains(strGroup.ToLower()))
				{
					list.Add(current);
				}
			}
			this.comboBoxGroups.DataSource = list;
		}

		private void comboBoxRepeat_Load()
		{
			this.comboBoxRepeat.DisplayMember = "Text";
			this.comboBoxRepeat.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "Daily",
					Value = "1"
				},
				new
				{
					Text = "Weekly",
					Value = "2"
				},
				new
				{
					Text = "Monthly",
					Value = "3"
				}
			};
			this.comboBoxRepeat.DataSource = dataSource;
		}

		private void ResetFrom()
		{
			DateTime dateTime = DateTime.Now.AddDays(1.0);
			DateTime value = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 10, 0, 0);
			this.dateTimePickerStart.Value = value;
			dateTime = dateTime.AddDays(1.0);
			value = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
			this.dateTimePickerStop.Value = value;
			this.comboBoxGroups_Load(string.Empty);
			this.comboBoxRepeat_Load();
			this.checkBoxRepeat.Checked = false;
			this.labelRepeatCount.Visible = false;
			this.textBoxNewMessage.Text = string.Empty;
		}

		private bool ValidateForm()
		{
			string text = string.Empty;
			bool result = true;
			if (this.comboBoxGroups.SelectedIndex == 0)
			{
				text += "Please select a group.\n";
			}
			if (this.textBoxNewMessage.Text.Length == 0)
			{
				text += "Please enter a text message.\n";
			}
			if (this.checkBoxRepeat.Checked && this.dateTimePickerStart.Value > this.dateTimePickerStop.Value)
			{
				text += "Until date must be greater than the initial Send date.\n";
			}
			if (this.dateTimePickerStart.Value < DateTime.Now)
			{
				text += "Send date must be in the future.\n";
			}
			DateTime dateTime;
			DateTime.TryParse(this.dateTimePickerStart.Value.ToString("dd-MMM-yyyy HH:mm:ss"), out dateTime);
			if ((dateTime.Hour < 9 || dateTime.Hour > 21) && MessageBox.Show("Send date is outside of normal business hours...\n\nDo you want to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
			{
				result = false;
			}
			if (text.Length > 0)
			{
				result = false;
				MessageBox.Show(text, this.appManager.m_strApplicationName, MessageBoxButtons.OK);
			}
			return result;
		}

		private void SetSaveItemsList()
		{
			string text = this.comboBoxRepeat.Text;
			DateTime value = this.dateTimePickerStop.Value;
			DateTime t = this.dateTimePickerStart.Value;
			this.lsSaveItems.Clear();
			while (t <= value)
			{
				ScheduleFileItem scheduleFileItem = new ScheduleFileItem();
				scheduleFileItem.SendStatus = string.Empty;
				scheduleFileItem.Address = this.comboBoxGroups.Text;
				scheduleFileItem.TextMessage = this.textBoxNewMessage.Text;
				scheduleFileItem.SendDate = t.ToString();
				this.lsSaveItems.Add(scheduleFileItem);
				if (text == "Monthly")
				{
					t = t.AddMonths(1);
				}
				else if (text == "Weekly")
				{
					t = t.AddDays(7.0);
				}
				else
				{
					t = t.AddDays(1.0);
				}
			}
			this.labelRepeatCount.Text = "Repeat Settings Will Create " + this.lsSaveItems.Count.ToString() + " Jobs.";
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			if (this.appManager.m_bGroupScheduleProcessing)
			{
				MessageBox.Show("Group Schedule Send is currently processing, please wait a moment and try again...", this.appManager.m_strApplicationName, MessageBoxButtons.OK);
				return;
			}
			if (this.ValidateForm())
			{
				if (this.checkBoxRepeat.Checked)
				{
					this.SetSaveItemsList();
				}
				else
				{
					this.lsSaveItems.Clear();
					ScheduleFileItem scheduleFileItem = new ScheduleFileItem();
					scheduleFileItem.Address = this.comboBoxGroups.Text;
					scheduleFileItem.SendDate = this.dateTimePickerStart.Value.ToString();
					scheduleFileItem.TextMessage = this.textBoxNewMessage.Text;
					scheduleFileItem.SendStatus = string.Empty;
					this.lsSaveItems.Add(scheduleFileItem);
				}
				foreach (ScheduleFileItem current in this.lsSaveItems)
				{
					string text = string.Empty;
					text = this.appManager.GroupSchedulFileAddItem(current);
					if (text.Length > 0)
					{
						this.strError += text;
					}
				}
				if (this.strError.Length > 0)
				{
					this.appManager.ShowBalloon("There was error saving the group schedule: " + this.strError, 5);
					return;
				}
				this.appManager.ShowBalloon("Your group schedule has been saved.", 5);
				base.Close();
				this.appManager.ShowGroupSchedule();
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
				return;
			}
			this.comboBoxRepeat.Enabled = false;
			this.dateTimePickerStop.Enabled = false;
			this.labelRepeatCount.Visible = false;
			this.labelUntil.Enabled = false;
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
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmEditGroupSchedule));
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
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(311, 393);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(61, 27);
			this.buttonSave.TabIndex = 10;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.textBoxNewMessage.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxNewMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxNewMessage.Location = new Point(12, 226);
			this.textBoxNewMessage.MaxLength = 250;
			this.textBoxNewMessage.Multiline = true;
			this.textBoxNewMessage.Name = "textBoxNewMessage";
			this.textBoxNewMessage.ScrollBars = ScrollBars.Vertical;
			this.textBoxNewMessage.Size = new Size(360, 145);
			this.textBoxNewMessage.TabIndex = 6;
			this.textBoxNewMessage.TextChanged += new EventHandler(this.textBoxNewMessage_TextChanged);
			this.lblCharCount.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.lblCharCount.AutoSize = true;
			this.lblCharCount.Font = new Font("Arial", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblCharCount.ForeColor = Color.FromArgb(64, 64, 64);
			this.lblCharCount.Location = new Point(317, 374);
			this.lblCharCount.MinimumSize = new Size(55, 0);
			this.lblCharCount.Name = "lblCharCount";
			this.lblCharCount.Size = new Size(55, 15);
			this.lblCharCount.TabIndex = 42;
			this.lblCharCount.Text = "0/250";
			this.lblCharCount.TextAlign = ContentAlignment.MiddleRight;
			this.comboBoxGroups.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxGroups.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxGroups.FormattingEnabled = true;
			this.comboBoxGroups.Location = new Point(135, 12);
			this.comboBoxGroups.Name = "comboBoxGroups";
			this.comboBoxGroups.Size = new Size(237, 25);
			this.comboBoxGroups.TabIndex = 0;
			this.labelGroupTag.AutoSize = true;
			this.labelGroupTag.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelGroupTag.Location = new Point(12, 15);
			this.labelGroupTag.Name = "labelGroupTag";
			this.labelGroupTag.Size = new Size(97, 17);
			this.labelGroupTag.TabIndex = 44;
			this.labelGroupTag.Text = "Select Group:";
			this.dateTimePickerStart.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.dateTimePickerStart.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.dateTimePickerStart.Location = new Point(135, 57);
			this.dateTimePickerStart.MinDate = new DateTime(2015, 7, 14, 12, 51, 23, 0);
			this.dateTimePickerStart.Name = "dateTimePickerStart";
			this.dateTimePickerStart.Size = new Size(237, 25);
			this.dateTimePickerStart.TabIndex = 1;
			this.dateTimePickerStart.Value = new DateTime(2015, 7, 14, 12, 51, 23, 0);
			this.labelStartDate.AutoSize = true;
			this.labelStartDate.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelStartDate.Location = new Point(12, 63);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new Size(117, 17);
			this.labelStartDate.TabIndex = 46;
			this.labelStartDate.Text = "Send Date/Time:";
			this.comboBoxRepeat.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.comboBoxRepeat.Enabled = false;
			this.comboBoxRepeat.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxRepeat.FormattingEnabled = true;
			this.comboBoxRepeat.Location = new Point(6, 29);
			this.comboBoxRepeat.Name = "comboBoxRepeat";
			this.comboBoxRepeat.Size = new Size(108, 25);
			this.comboBoxRepeat.TabIndex = 4;
			this.comboBoxRepeat.SelectedIndexChanged += new EventHandler(this.comboBoxRepeat_SelectedIndexChanged);
			this.dateTimePickerStop.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.dateTimePickerStop.Enabled = false;
			this.dateTimePickerStop.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.dateTimePickerStop.Location = new Point(162, 29);
			this.dateTimePickerStop.MinDate = new DateTime(2015, 7, 14, 12, 51, 23, 0);
			this.dateTimePickerStop.Name = "dateTimePickerStop";
			this.dateTimePickerStop.Size = new Size(189, 25);
			this.dateTimePickerStop.TabIndex = 5;
			this.dateTimePickerStop.Value = new DateTime(2015, 7, 14, 12, 51, 23, 0);
			this.dateTimePickerStop.ValueChanged += new EventHandler(this.dateTimePickerStop_ValueChanged);
			this.labelMessage.AutoSize = true;
			this.labelMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelMessage.Location = new Point(12, 206);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new Size(71, 17);
			this.labelMessage.TabIndex = 51;
			this.labelMessage.Text = "Message:";
			this.groupBoxRepeat.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.groupBoxRepeat.Controls.Add(this.labelRepeatCount);
			this.groupBoxRepeat.Controls.Add(this.labelUntil);
			this.groupBoxRepeat.Controls.Add(this.checkBoxRepeat);
			this.groupBoxRepeat.Controls.Add(this.comboBoxRepeat);
			this.groupBoxRepeat.Controls.Add(this.dateTimePickerStop);
			this.groupBoxRepeat.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.groupBoxRepeat.Location = new Point(15, 103);
			this.groupBoxRepeat.Name = "groupBoxRepeat";
			this.groupBoxRepeat.Size = new Size(357, 93);
			this.groupBoxRepeat.TabIndex = 2;
			this.groupBoxRepeat.TabStop = false;
			this.groupBoxRepeat.Text = "Repeat";
			this.labelRepeatCount.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.labelRepeatCount.AutoSize = true;
			this.labelRepeatCount.Enabled = false;
			this.labelRepeatCount.Font = new Font("Arial", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.labelRepeatCount.Location = new Point(6, 67);
			this.labelRepeatCount.Name = "labelRepeatCount";
			this.labelRepeatCount.Size = new Size(124, 16);
			this.labelRepeatCount.TabIndex = 54;
			this.labelRepeatCount.Text = "(Will Create 0 Jobs)";
			this.labelRepeatCount.TextAlign = ContentAlignment.MiddleRight;
			this.labelUntil.AutoSize = true;
			this.labelUntil.Enabled = false;
			this.labelUntil.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelUntil.Location = new Point(120, 33);
			this.labelUntil.Name = "labelUntil";
			this.labelUntil.Size = new Size(40, 17);
			this.labelUntil.TabIndex = 53;
			this.labelUntil.Text = "Until:";
			this.checkBoxRepeat.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.checkBoxRepeat.AutoSize = true;
			this.checkBoxRepeat.Font = new Font("Arial", 9f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.checkBoxRepeat.Location = new Point(190, 0);
			this.checkBoxRepeat.Name = "checkBoxRepeat";
			this.checkBoxRepeat.Size = new Size(161, 19);
			this.checkBoxRepeat.TabIndex = 3;
			this.checkBoxRepeat.Text = "Click to Edit This Section";
			this.checkBoxRepeat.UseVisualStyleBackColor = true;
			this.checkBoxRepeat.CheckedChanged += new EventHandler(this.checkBoxRepeat_CheckedChanged);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(384, 432);
			base.Controls.Add(this.groupBoxRepeat);
			base.Controls.Add(this.labelMessage);
			base.Controls.Add(this.labelStartDate);
			base.Controls.Add(this.dateTimePickerStart);
			base.Controls.Add(this.labelGroupTag);
			base.Controls.Add(this.comboBoxGroups);
			base.Controls.Add(this.lblCharCount);
			base.Controls.Add(this.textBoxNewMessage);
			base.Controls.Add(this.buttonSave);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new Size(400, 470);
			base.Name = "fmEditGroupSchedule";
			this.Text = "EditGroupSchedule";
			base.Load += new EventHandler(this.fmEditGroupSchedule_Load);
			this.groupBoxRepeat.ResumeLayout(false);
			this.groupBoxRepeat.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

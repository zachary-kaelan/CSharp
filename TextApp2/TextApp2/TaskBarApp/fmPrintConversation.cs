using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TaskBarApp
{
	public class fmPrintConversation : Form
	{
		private string strError = string.Empty;

		private IContainer components;

		private RadioButton radioButtonDisplayed;

		private RadioButton radioButtonDate;

		private Button buttonPrint;

		private Button buttonCancel;

		private DateTimePicker dateTimePickerFrom;

		private DateTimePicker dateTimePickerTo;

		private Label labelFrom;

		private Label labelTo;

		private RadioButton radioButtonEntireConversation;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmPrintConversation()
		{
			this.InitializeComponent();
		}

		private void fmPrintConversation_Load(object sender, EventArgs e)
		{
			this.Text = this.appManager.m_strApplicationName + " Print Conversation";
			base.Icon = this.appManager.iTextApp;
			this.radioButtonDisplayed.Text = "Print Displayed Messages";
			this.dateTimePickerTo.Value = DateTime.Now;
			this.dateTimePickerFrom.Value = DateTime.Now.AddMonths(-1);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void buttonPrint_Click(object sender, EventArgs e)
		{
			DateTime? startDate = null;
			DateTime? endDate = null;
			bool entireConversation = false;
			if (this.radioButtonDate.Checked)
			{
				startDate = new DateTime?(this.dateTimePickerFrom.Value);
				endDate = new DateTime?(this.dateTimePickerTo.Value.AddDays(1.0));
				entireConversation = true;
			}
			if (this.radioButtonEntireConversation.Checked)
			{
				entireConversation = true;
			}
			this.appManager.formMessages.PrintConversation(entireConversation, startDate, endDate);
			base.Close();
		}

		private void radioButtonDate_CheckedChanged(object sender, EventArgs e)
		{
			if (this.radioButtonDate.Checked)
			{
				this.dateTimePickerFrom.Enabled = true;
				this.dateTimePickerTo.Enabled = true;
				return;
			}
			this.dateTimePickerFrom.Enabled = false;
			this.dateTimePickerTo.Enabled = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmPrintConversation));
			this.radioButtonDisplayed = new RadioButton();
			this.radioButtonDate = new RadioButton();
			this.buttonPrint = new Button();
			this.buttonCancel = new Button();
			this.dateTimePickerFrom = new DateTimePicker();
			this.dateTimePickerTo = new DateTimePicker();
			this.labelFrom = new Label();
			this.labelTo = new Label();
			this.radioButtonEntireConversation = new RadioButton();
			base.SuspendLayout();
			this.radioButtonDisplayed.AutoSize = true;
			this.radioButtonDisplayed.Checked = true;
			this.radioButtonDisplayed.Location = new Point(18, 12);
			this.radioButtonDisplayed.Name = "radioButtonDisplayed";
			this.radioButtonDisplayed.Size = new Size(95, 17);
			this.radioButtonDisplayed.TabIndex = 0;
			this.radioButtonDisplayed.TabStop = true;
			this.radioButtonDisplayed.Text = "Print Displayed";
			this.radioButtonDisplayed.UseVisualStyleBackColor = true;
			this.radioButtonDate.AutoSize = true;
			this.radioButtonDate.Location = new Point(18, 73);
			this.radioButtonDate.Name = "radioButtonDate";
			this.radioButtonDate.Size = new Size(143, 17);
			this.radioButtonDate.TabIndex = 1;
			this.radioButtonDate.Text = "Print Select Date Range:";
			this.radioButtonDate.UseVisualStyleBackColor = true;
			this.radioButtonDate.CheckedChanged += new EventHandler(this.radioButtonDate_CheckedChanged);
			this.buttonPrint.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonPrint.Location = new Point(189, 171);
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.Size = new Size(75, 27);
			this.buttonPrint.TabIndex = 2;
			this.buttonPrint.Text = "Print";
			this.buttonPrint.UseVisualStyleBackColor = true;
			this.buttonPrint.Click += new EventHandler(this.buttonPrint_Click);
			this.buttonCancel.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonCancel.Location = new Point(18, 171);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new Size(75, 27);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
			this.dateTimePickerFrom.Enabled = false;
			this.dateTimePickerFrom.Location = new Point(64, 99);
			this.dateTimePickerFrom.Name = "dateTimePickerFrom";
			this.dateTimePickerFrom.Size = new Size(200, 20);
			this.dateTimePickerFrom.TabIndex = 4;
			this.dateTimePickerTo.Enabled = false;
			this.dateTimePickerTo.Location = new Point(64, 134);
			this.dateTimePickerTo.Name = "dateTimePickerTo";
			this.dateTimePickerTo.Size = new Size(200, 20);
			this.dateTimePickerTo.TabIndex = 5;
			this.labelFrom.AutoSize = true;
			this.labelFrom.Location = new Point(25, 102);
			this.labelFrom.Name = "labelFrom";
			this.labelFrom.Size = new Size(33, 13);
			this.labelFrom.TabIndex = 6;
			this.labelFrom.Text = "From:";
			this.labelTo.AutoSize = true;
			this.labelTo.Location = new Point(35, 137);
			this.labelTo.Name = "labelTo";
			this.labelTo.Size = new Size(23, 13);
			this.labelTo.TabIndex = 7;
			this.labelTo.Text = "To:";
			this.radioButtonEntireConversation.AutoSize = true;
			this.radioButtonEntireConversation.Location = new Point(18, 41);
			this.radioButtonEntireConversation.Name = "radioButtonEntireConversation";
			this.radioButtonEntireConversation.Size = new Size(141, 17);
			this.radioButtonEntireConversation.TabIndex = 8;
			this.radioButtonEntireConversation.Text = "Print Entire Conversation";
			this.radioButtonEntireConversation.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(284, 212);
			base.Controls.Add(this.radioButtonEntireConversation);
			base.Controls.Add(this.labelTo);
			base.Controls.Add(this.labelFrom);
			base.Controls.Add(this.dateTimePickerTo);
			base.Controls.Add(this.dateTimePickerFrom);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonPrint);
			base.Controls.Add(this.radioButtonDate);
			base.Controls.Add(this.radioButtonDisplayed);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new Size(300, 250);
			base.MinimizeBox = false;
			this.MinimumSize = new Size(300, 250);
			base.Name = "fmPrintConversation";
			this.Text = "Print Conversation";
			base.Load += new EventHandler(this.fmPrintConversation_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

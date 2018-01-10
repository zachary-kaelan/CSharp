using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TaskBarApp
{
	public class fmKeywordAutoResponse : Form
	{
		private string strError = string.Empty;

		private bool bRefreshMessageForm;

		private IContainer components;

		private Label labelTakenKeywords;

		private Label labelGroupRemoveKeyword;

		private TextBox textBoxRemoveFromGroupKeyword;

		private CheckBox checkBoxEnableKeywordProcessing;

		private Button buttonSave;

		private CheckBox checkBoxNotifyKeywordProcessing;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmKeywordAutoResponse()
		{
			this.InitializeComponent();
		}

		private void KeywordAutoResponse_Load(object sender, EventArgs e)
		{
			try
			{
				this.checkBoxEnableKeywordProcessing.Checked = this.appManager.m_bEnableKeywordProcessing;
				this.checkBoxNotifyKeywordProcessing.Checked = this.appManager.m_bNotifyKeywordProcessing;
				this.textBoxRemoveFromGroupKeyword.Text = this.appManager.m_strRemoveGroupKeyword;
				this.textBoxRemoveFromGroupKeyword.Enabled = false;
				if (!this.appManager.m_bEnableKeywordProcessing)
				{
					this.checkBoxNotifyKeywordProcessing.Enabled = false;
				}
				this.Text = this.appManager.m_strApplicationName + " Keyword Auto Response";
				base.Icon = this.appManager.iTextApp;
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception creating Keyword Auto Response form: " + ex.Message, 5);
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			bool flag = true;
			string empty = string.Empty;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref empty);
			try
			{
				AppRegistry.SaveValue(rootKey, "EnableKeywordProcessing", this.checkBoxEnableKeywordProcessing.Checked, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Enable Keyword Processing save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_bEnableKeywordProcessing = this.checkBoxEnableKeywordProcessing.Checked;
					this.bRefreshMessageForm = true;
				}
			}
			catch (Exception ex)
			{
				this.strError = this.strError + "Enable Keyword Processing save error: " + ex.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "NotifyKeywordProcessing", this.checkBoxNotifyKeywordProcessing.Checked, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Notify Keyword Processing save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_bNotifyKeywordProcessing = this.checkBoxNotifyKeywordProcessing.Checked;
				}
			}
			catch (Exception ex2)
			{
				this.strError = this.strError + "Notify Keyword Processing save error: " + ex2.Message;
				flag = false;
			}
			if (flag)
			{
				base.Close();
				if (this.appManager.formMessages != null && this.bRefreshMessageForm)
				{
					this.appManager.formMessages.Close();
					this.appManager.ShowMessages();
				}
				this.appManager.ShowBalloon("Your settings have been saved.", 5);
				return;
			}
			this.appManager.ShowBalloon("Exception saving keyword auto response settings: " + this.strError, 5);
		}

		private void checkBoxEnableKeywordProcessing_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBoxEnableKeywordProcessing.Checked)
			{
				this.checkBoxNotifyKeywordProcessing.Enabled = true;
				return;
			}
			this.checkBoxNotifyKeywordProcessing.Enabled = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmKeywordAutoResponse));
			this.labelTakenKeywords = new Label();
			this.labelGroupRemoveKeyword = new Label();
			this.textBoxRemoveFromGroupKeyword = new TextBox();
			this.checkBoxEnableKeywordProcessing = new CheckBox();
			this.buttonSave = new Button();
			this.checkBoxNotifyKeywordProcessing = new CheckBox();
			base.SuspendLayout();
			this.labelTakenKeywords.AutoSize = true;
			this.labelTakenKeywords.Font = new Font("Arial", 8.25f, FontStyle.Italic);
			this.labelTakenKeywords.Location = new Point(13, 121);
			this.labelTakenKeywords.Name = "labelTakenKeywords";
			this.labelTakenKeywords.Size = new Size(231, 14);
			this.labelTakenKeywords.TabIndex = 0;
			this.labelTakenKeywords.Text = "Please Note: Cannot be \"Quit\"   \"Stop\"   \"End\"";
			this.labelGroupRemoveKeyword.AutoSize = true;
			this.labelGroupRemoveKeyword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelGroupRemoveKeyword.Location = new Point(13, 94);
			this.labelGroupRemoveKeyword.Name = "labelGroupRemoveKeyword";
			this.labelGroupRemoveKeyword.Size = new Size(202, 17);
			this.labelGroupRemoveKeyword.TabIndex = 1;
			this.labelGroupRemoveKeyword.Text = "Remove from Group Keyword";
			this.textBoxRemoveFromGroupKeyword.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.textBoxRemoveFromGroupKeyword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.textBoxRemoveFromGroupKeyword.Location = new Point(234, 91);
			this.textBoxRemoveFromGroupKeyword.Name = "textBoxRemoveFromGroupKeyword";
			this.textBoxRemoveFromGroupKeyword.Size = new Size(163, 25);
			this.textBoxRemoveFromGroupKeyword.TabIndex = 2;
			this.checkBoxEnableKeywordProcessing.AutoSize = true;
			this.checkBoxEnableKeywordProcessing.BackColor = Color.Transparent;
			this.checkBoxEnableKeywordProcessing.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxEnableKeywordProcessing.Location = new Point(12, 13);
			this.checkBoxEnableKeywordProcessing.Name = "checkBoxEnableKeywordProcessing";
			this.checkBoxEnableKeywordProcessing.Size = new Size(328, 21);
			this.checkBoxEnableKeywordProcessing.TabIndex = 37;
			this.checkBoxEnableKeywordProcessing.Text = "Enable Keyword Processing && Auto Response";
			this.checkBoxEnableKeywordProcessing.UseVisualStyleBackColor = false;
			this.checkBoxEnableKeywordProcessing.CheckedChanged += new EventHandler(this.checkBoxEnableKeywordProcessing_CheckedChanged);
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(328, 144);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(69, 27);
			this.buttonSave.TabIndex = 38;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.checkBoxNotifyKeywordProcessing.AutoSize = true;
			this.checkBoxNotifyKeywordProcessing.BackColor = Color.Transparent;
			this.checkBoxNotifyKeywordProcessing.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxNotifyKeywordProcessing.Location = new Point(12, 49);
			this.checkBoxNotifyKeywordProcessing.Name = "checkBoxNotifyKeywordProcessing";
			this.checkBoxNotifyKeywordProcessing.Size = new Size(319, 21);
			this.checkBoxNotifyKeywordProcessing.TabIndex = 39;
			this.checkBoxNotifyKeywordProcessing.Text = "Display Notification For Keyword Processing ";
			this.checkBoxNotifyKeywordProcessing.UseVisualStyleBackColor = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(409, 183);
			base.Controls.Add(this.checkBoxNotifyKeywordProcessing);
			base.Controls.Add(this.buttonSave);
			base.Controls.Add(this.checkBoxEnableKeywordProcessing);
			base.Controls.Add(this.textBoxRemoveFromGroupKeyword);
			base.Controls.Add(this.labelGroupRemoveKeyword);
			base.Controls.Add(this.labelTakenKeywords);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "fmKeywordAutoResponse";
			this.Text = "Keyword Auto Response";
			base.Load += new EventHandler(this.KeywordAutoResponse_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

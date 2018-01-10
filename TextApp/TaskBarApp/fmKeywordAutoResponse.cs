namespace TaskBarApp
{
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class fmKeywordAutoResponse : Form
    {
        private bool bRefreshMessageForm;
        private Button buttonSave;
        private CheckBox checkBoxEnableKeywordProcessing;
        private CheckBox checkBoxNotifyKeywordProcessing;
        private IContainer components;
        private Label labelGroupRemoveKeyword;
        private Label labelTakenKeywords;
        private string strError = string.Empty;
        private TextBox textBoxRemoveFromGroupKeyword;

        public fmKeywordAutoResponse()
        {
            this.InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            bool flag = true;
            string errorMessage = string.Empty;
            RegistryKey rootKey = AppRegistry.GetRootKey(ref errorMessage);
            try
            {
                AppRegistry.SaveValue(rootKey, "EnableKeywordProcessing", this.checkBoxEnableKeywordProcessing.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Enable Keyword Processing save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_bEnableKeywordProcessing = this.checkBoxEnableKeywordProcessing.Checked;
                    this.bRefreshMessageForm = true;
                }
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Enable Keyword Processing save error: " + exception.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "NotifyKeywordProcessing", this.checkBoxNotifyKeywordProcessing.Checked, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Notify Keyword Processing save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_bNotifyKeywordProcessing = this.checkBoxNotifyKeywordProcessing.Checked;
                }
            }
            catch (Exception exception2)
            {
                this.strError = this.strError + "Notify Keyword Processing save error: " + exception2.Message;
                flag = false;
            }
            if (flag)
            {
                base.Close();
                if ((this.appManager.formMessages != null) && this.bRefreshMessageForm)
                {
                    this.appManager.formMessages.Close();
                    this.appManager.ShowMessages();
                }
                this.appManager.ShowBalloon("Your settings have been saved.", 5);
            }
            else
            {
                this.appManager.ShowBalloon("Exception saving keyword auto response settings: " + this.strError, 5);
            }
        }

        private void checkBoxEnableKeywordProcessing_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxEnableKeywordProcessing.Checked)
            {
                this.checkBoxNotifyKeywordProcessing.Enabled = true;
            }
            else
            {
                this.checkBoxNotifyKeywordProcessing.Enabled = false;
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

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmKeywordAutoResponse));
            this.labelTakenKeywords = new Label();
            this.labelGroupRemoveKeyword = new Label();
            this.textBoxRemoveFromGroupKeyword = new TextBox();
            this.checkBoxEnableKeywordProcessing = new CheckBox();
            this.buttonSave = new Button();
            this.checkBoxNotifyKeywordProcessing = new CheckBox();
            base.SuspendLayout();
            this.labelTakenKeywords.AutoSize = true;
            this.labelTakenKeywords.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            this.labelTakenKeywords.Location = new Point(13, 0x79);
            this.labelTakenKeywords.Name = "labelTakenKeywords";
            this.labelTakenKeywords.Size = new Size(0xe7, 14);
            this.labelTakenKeywords.TabIndex = 0;
            this.labelTakenKeywords.Text = "Please Note: Cannot be \"Quit\"   \"Stop\"   \"End\"";
            this.labelGroupRemoveKeyword.AutoSize = true;
            this.labelGroupRemoveKeyword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelGroupRemoveKeyword.Location = new Point(13, 0x5e);
            this.labelGroupRemoveKeyword.Name = "labelGroupRemoveKeyword";
            this.labelGroupRemoveKeyword.Size = new Size(0xca, 0x11);
            this.labelGroupRemoveKeyword.TabIndex = 1;
            this.labelGroupRemoveKeyword.Text = "Remove from Group Keyword";
            this.textBoxRemoveFromGroupKeyword.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxRemoveFromGroupKeyword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.textBoxRemoveFromGroupKeyword.Location = new Point(0xea, 0x5b);
            this.textBoxRemoveFromGroupKeyword.Name = "textBoxRemoveFromGroupKeyword";
            this.textBoxRemoveFromGroupKeyword.Size = new Size(0xa3, 0x19);
            this.textBoxRemoveFromGroupKeyword.TabIndex = 2;
            this.checkBoxEnableKeywordProcessing.AutoSize = true;
            this.checkBoxEnableKeywordProcessing.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxEnableKeywordProcessing.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxEnableKeywordProcessing.Location = new Point(12, 13);
            this.checkBoxEnableKeywordProcessing.Name = "checkBoxEnableKeywordProcessing";
            this.checkBoxEnableKeywordProcessing.Size = new Size(0x148, 0x15);
            this.checkBoxEnableKeywordProcessing.TabIndex = 0x25;
            this.checkBoxEnableKeywordProcessing.Text = "Enable Keyword Processing && Auto Response";
            this.checkBoxEnableKeywordProcessing.UseVisualStyleBackColor = false;
            this.checkBoxEnableKeywordProcessing.CheckedChanged += new EventHandler(this.checkBoxEnableKeywordProcessing_CheckedChanged);
            this.buttonSave.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0x148, 0x90);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(0x45, 0x1b);
            this.buttonSave.TabIndex = 0x26;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.checkBoxNotifyKeywordProcessing.AutoSize = true;
            this.checkBoxNotifyKeywordProcessing.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxNotifyKeywordProcessing.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBoxNotifyKeywordProcessing.Location = new Point(12, 0x31);
            this.checkBoxNotifyKeywordProcessing.Name = "checkBoxNotifyKeywordProcessing";
            this.checkBoxNotifyKeywordProcessing.Size = new Size(0x13f, 0x15);
            this.checkBoxNotifyKeywordProcessing.TabIndex = 0x27;
            this.checkBoxNotifyKeywordProcessing.Text = "Display Notification For Keyword Processing ";
            this.checkBoxNotifyKeywordProcessing.UseVisualStyleBackColor = false;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x199, 0xb7);
            base.Controls.Add(this.checkBoxNotifyKeywordProcessing);
            base.Controls.Add(this.buttonSave);
            base.Controls.Add(this.checkBoxEnableKeywordProcessing);
            base.Controls.Add(this.textBoxRemoveFromGroupKeyword);
            base.Controls.Add(this.labelGroupRemoveKeyword);
            base.Controls.Add(this.labelTakenKeywords);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "fmKeywordAutoResponse";
            this.Text = "Keyword Auto Response";
            base.Load += new EventHandler(this.KeywordAutoResponse_Load);
            base.ResumeLayout(false);
            base.PerformLayout();
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
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception creating Keyword Auto Response form: " + exception.Message, 5);
            }
        }

        public ApplicationManager appManager { get; set; }
    }
}


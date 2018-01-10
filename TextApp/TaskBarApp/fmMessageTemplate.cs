namespace TaskBarApp
{
    using Keyoti.RapidSpell;
    using Keyoti.RapidSpell.Options;
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class fmMessageTemplate : Form
    {
        private Button buttonSave;
        private Button buttonSave2;
        private IContainer components;
        private Label labelMessageTemplate1;
        private Label labelMessageTemplate10;
        private Label labelMessageTemplate2;
        private Label labelMessageTemplate3;
        private Label labelMessageTemplate4;
        private Label labelMessageTemplate5;
        private Label labelMessageTemplate6;
        private Label labelMessageTemplate7;
        private Label labelMessageTemplate8;
        private Label labelMessageTemplate9;
        private Panel panelMessageTemplate;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate1;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate10;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate2;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate3;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate4;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate5;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate6;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate7;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate8;
        private RapidSpellAsYouType rapidSpellAsYouTypeTemplate9;
        private string strError = string.Empty;
        private TextBox textBoxMessageTemplate1;
        private TextBox textBoxMessageTemplate10;
        private TextBox textBoxMessageTemplate2;
        private TextBox textBoxMessageTemplate3;
        private TextBox textBoxMessageTemplate4;
        private TextBox textBoxMessageTemplate5;
        private TextBox textBoxMessageTemplate6;
        private TextBox textBoxMessageTemplate7;
        private TextBox textBoxMessageTemplate8;
        private TextBox textBoxMessageTemplate9;

        public fmMessageTemplate()
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
                AppRegistry.SaveValue(rootKey, "MessageTemplate1", this.textBoxMessageTemplate1.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 1 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate1 = this.textBoxMessageTemplate1.Text;
                }
            }
            catch (Exception exception)
            {
                this.strError = this.strError + "Message template 1 save error: " + exception.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate2", this.textBoxMessageTemplate2.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 2 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate2 = this.textBoxMessageTemplate2.Text;
                }
            }
            catch (Exception exception2)
            {
                this.strError = this.strError + "Message template 2 save error: " + exception2.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate3", this.textBoxMessageTemplate3.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 3 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate3 = this.textBoxMessageTemplate3.Text;
                }
            }
            catch (Exception exception3)
            {
                this.strError = this.strError + "Message template 3 save error: " + exception3.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate4", this.textBoxMessageTemplate4.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 4 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate4 = this.textBoxMessageTemplate4.Text;
                }
            }
            catch (Exception exception4)
            {
                this.strError = this.strError + "Message template 4 save error: " + exception4.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate5", this.textBoxMessageTemplate5.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 5 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate5 = this.textBoxMessageTemplate5.Text;
                }
            }
            catch (Exception exception5)
            {
                this.strError = this.strError + "Message template 5 save error: " + exception5.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate6", this.textBoxMessageTemplate6.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 6 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate6 = this.textBoxMessageTemplate6.Text;
                }
            }
            catch (Exception exception6)
            {
                this.strError = this.strError + "Message template 6 save error: " + exception6.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate7", this.textBoxMessageTemplate7.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 7 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate7 = this.textBoxMessageTemplate7.Text;
                }
            }
            catch (Exception exception7)
            {
                this.strError = this.strError + "Message template 7 save error: " + exception7.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate8", this.textBoxMessageTemplate8.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 8 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate8 = this.textBoxMessageTemplate8.Text;
                }
            }
            catch (Exception exception8)
            {
                this.strError = this.strError + "Message template 8 save error: " + exception8.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate9", this.textBoxMessageTemplate9.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 9 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate9 = this.textBoxMessageTemplate9.Text;
                }
            }
            catch (Exception exception9)
            {
                this.strError = this.strError + "Message template 9 save error: " + exception9.Message;
                flag = false;
            }
            try
            {
                AppRegistry.SaveValue(rootKey, "MessageTemplate10", this.textBoxMessageTemplate10.Text, ref errorMessage, false, RegistryValueKind.Unknown);
                if (errorMessage != string.Empty)
                {
                    this.strError = this.strError + "Message template 10 save error: " + errorMessage;
                    flag = false;
                }
                else
                {
                    this.appManager.m_strMessageTemplate10 = this.textBoxMessageTemplate10.Text;
                }
            }
            catch (Exception exception10)
            {
                this.strError = this.strError + "Message template 10 save error: " + exception10.Message;
                flag = false;
            }
            if (flag)
            {
                if (this.appManager.formMessages != null)
                {
                    this.appManager.ShowMessages();
                }
                if (this.appManager.formNewMessage != null)
                {
                    this.appManager.ShowNewMessage();
                }
                base.Close();
                this.appManager.ShowBalloon("Your message templates have been saved.", 5);
            }
            else
            {
                this.appManager.ShowBalloon("Exception saving message templates: " + this.strError, 5);
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

        private void fmMessageTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.SaveValue(rootKey, "local_FormMessageTemplateWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
                AppRegistry.SaveValue(rootKey, "local_FormMessageTemplateHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
            }
            catch
            {
            }
        }

        private void fmMessageTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                int num = 0;
                int num2 = 0;
                RegistryKey rootKey = AppRegistry.GetRootKey(ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormMessageTemplateWidth", ref num, ref this.strError);
                AppRegistry.GetValue(rootKey, "local_FormMessageTemplateHeight", ref num2, ref this.strError);
                if (num2 != 0)
                {
                    base.Height = num2;
                }
                if (num != 0)
                {
                    base.Width = num;
                }
                this.Text = this.appManager.m_strApplicationName + " Edit Message Templates";
                base.Icon = this.appManager.iTextApp;
                try
                {
                    this.rapidSpellAsYouTypeTemplate1.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate2.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate3.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate4.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate5.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate6.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate7.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate8.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate9.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                    this.rapidSpellAsYouTypeTemplate10.UserDictionaryFile = this.appManager.m_strUserDictionaryFile;
                }
                catch (Exception)
                {
                }
                this.textBoxMessageTemplate1.Text = this.appManager.m_strMessageTemplate1;
                this.textBoxMessageTemplate2.Text = this.appManager.m_strMessageTemplate2;
                this.textBoxMessageTemplate3.Text = this.appManager.m_strMessageTemplate3;
                this.textBoxMessageTemplate4.Text = this.appManager.m_strMessageTemplate4;
                this.textBoxMessageTemplate5.Text = this.appManager.m_strMessageTemplate5;
                this.textBoxMessageTemplate6.Text = this.appManager.m_strMessageTemplate6;
                this.textBoxMessageTemplate7.Text = this.appManager.m_strMessageTemplate7;
                this.textBoxMessageTemplate8.Text = this.appManager.m_strMessageTemplate8;
                this.textBoxMessageTemplate9.Text = this.appManager.m_strMessageTemplate9;
                this.textBoxMessageTemplate10.Text = this.appManager.m_strMessageTemplate10;
                this.buttonSave2.Select();
            }
            catch (Exception exception)
            {
                this.appManager.ShowBalloon("Exception creating Message Templates form: " + exception.Message, 5);
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(fmMessageTemplate));
            this.buttonSave = new Button();
            this.textBoxMessageTemplate1 = new TextBox();
            this.labelMessageTemplate1 = new Label();
            this.labelMessageTemplate2 = new Label();
            this.textBoxMessageTemplate2 = new TextBox();
            this.labelMessageTemplate4 = new Label();
            this.textBoxMessageTemplate4 = new TextBox();
            this.labelMessageTemplate3 = new Label();
            this.textBoxMessageTemplate3 = new TextBox();
            this.labelMessageTemplate6 = new Label();
            this.textBoxMessageTemplate6 = new TextBox();
            this.labelMessageTemplate5 = new Label();
            this.textBoxMessageTemplate5 = new TextBox();
            this.labelMessageTemplate8 = new Label();
            this.textBoxMessageTemplate8 = new TextBox();
            this.labelMessageTemplate7 = new Label();
            this.textBoxMessageTemplate7 = new TextBox();
            this.labelMessageTemplate10 = new Label();
            this.textBoxMessageTemplate10 = new TextBox();
            this.labelMessageTemplate9 = new Label();
            this.textBoxMessageTemplate9 = new TextBox();
            this.buttonSave2 = new Button();
            this.panelMessageTemplate = new Panel();
            this.rapidSpellAsYouTypeTemplate1 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate2 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate3 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate4 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate5 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate6 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate7 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate8 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate9 = new RapidSpellAsYouType(this.components);
            this.rapidSpellAsYouTypeTemplate10 = new RapidSpellAsYouType(this.components);
            this.panelMessageTemplate.SuspendLayout();
            base.SuspendLayout();
            this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave.Location = new Point(0xf7, 0x349);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(80, 0x19);
            this.buttonSave.TabIndex = 0;
            this.buttonSave.Text = "Save All";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            this.textBoxMessageTemplate1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate1.Location = new Point(12, 0x30);
            this.textBoxMessageTemplate1.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate1.MaxLength = 250;
            this.textBoxMessageTemplate1.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate1.Multiline = true;
            this.textBoxMessageTemplate1.Name = "textBoxMessageTemplate1";
            this.textBoxMessageTemplate1.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate1.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate1.TabIndex = 11;
            this.labelMessageTemplate1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate1.AutoSize = true;
            this.labelMessageTemplate1.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate1.Location = new Point(12, 0x1d);
            this.labelMessageTemplate1.Name = "labelMessageTemplate1";
            this.labelMessageTemplate1.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate1.TabIndex = 0x11;
            this.labelMessageTemplate1.Text = "Message Template 1";
            this.labelMessageTemplate2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate2.AutoSize = true;
            this.labelMessageTemplate2.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate2.Location = new Point(12, 110);
            this.labelMessageTemplate2.Name = "labelMessageTemplate2";
            this.labelMessageTemplate2.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate2.TabIndex = 20;
            this.labelMessageTemplate2.Text = "Message Template 2";
            this.textBoxMessageTemplate2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate2.Location = new Point(12, 0x81);
            this.textBoxMessageTemplate2.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate2.MaxLength = 250;
            this.textBoxMessageTemplate2.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate2.Multiline = true;
            this.textBoxMessageTemplate2.Name = "textBoxMessageTemplate2";
            this.textBoxMessageTemplate2.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate2.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate2.TabIndex = 0x12;
            this.labelMessageTemplate4.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate4.AutoSize = true;
            this.labelMessageTemplate4.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate4.Location = new Point(12, 0x10f);
            this.labelMessageTemplate4.Name = "labelMessageTemplate4";
            this.labelMessageTemplate4.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate4.TabIndex = 0x1a;
            this.labelMessageTemplate4.Text = "Message Template 4";
            this.textBoxMessageTemplate4.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate4.Location = new Point(12, 290);
            this.textBoxMessageTemplate4.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate4.MaxLength = 250;
            this.textBoxMessageTemplate4.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate4.Multiline = true;
            this.textBoxMessageTemplate4.Name = "textBoxMessageTemplate4";
            this.textBoxMessageTemplate4.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate4.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate4.TabIndex = 0x18;
            this.labelMessageTemplate3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate3.AutoSize = true;
            this.labelMessageTemplate3.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate3.Location = new Point(12, 0xbf);
            this.labelMessageTemplate3.Name = "labelMessageTemplate3";
            this.labelMessageTemplate3.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate3.TabIndex = 0x17;
            this.labelMessageTemplate3.Text = "Message Template 3";
            this.textBoxMessageTemplate3.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate3.Location = new Point(12, 210);
            this.textBoxMessageTemplate3.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate3.MaxLength = 250;
            this.textBoxMessageTemplate3.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate3.Multiline = true;
            this.textBoxMessageTemplate3.Name = "textBoxMessageTemplate3";
            this.textBoxMessageTemplate3.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate3.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate3.TabIndex = 0x15;
            this.labelMessageTemplate6.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate6.AutoSize = true;
            this.labelMessageTemplate6.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate6.Location = new Point(13, 0x1b1);
            this.labelMessageTemplate6.Name = "labelMessageTemplate6";
            this.labelMessageTemplate6.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate6.TabIndex = 0x20;
            this.labelMessageTemplate6.Text = "Message Template 6";
            this.textBoxMessageTemplate6.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate6.Location = new Point(13, 0x1c4);
            this.textBoxMessageTemplate6.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate6.MaxLength = 250;
            this.textBoxMessageTemplate6.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate6.Multiline = true;
            this.textBoxMessageTemplate6.Name = "textBoxMessageTemplate6";
            this.textBoxMessageTemplate6.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate6.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate6.TabIndex = 30;
            this.labelMessageTemplate5.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate5.AutoSize = true;
            this.labelMessageTemplate5.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate5.Location = new Point(12, 0x160);
            this.labelMessageTemplate5.Name = "labelMessageTemplate5";
            this.labelMessageTemplate5.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate5.TabIndex = 0x1d;
            this.labelMessageTemplate5.Text = "Message Template 5";
            this.textBoxMessageTemplate5.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate5.Location = new Point(12, 0x173);
            this.textBoxMessageTemplate5.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate5.MaxLength = 250;
            this.textBoxMessageTemplate5.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate5.Multiline = true;
            this.textBoxMessageTemplate5.Name = "textBoxMessageTemplate5";
            this.textBoxMessageTemplate5.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate5.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate5.TabIndex = 0x1b;
            this.labelMessageTemplate8.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate8.AutoSize = true;
            this.labelMessageTemplate8.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate8.Location = new Point(13, 0x252);
            this.labelMessageTemplate8.Name = "labelMessageTemplate8";
            this.labelMessageTemplate8.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate8.TabIndex = 0x26;
            this.labelMessageTemplate8.Text = "Message Template 8";
            this.textBoxMessageTemplate8.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate8.Location = new Point(13, 0x265);
            this.textBoxMessageTemplate8.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate8.MaxLength = 250;
            this.textBoxMessageTemplate8.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate8.Multiline = true;
            this.textBoxMessageTemplate8.Name = "textBoxMessageTemplate8";
            this.textBoxMessageTemplate8.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate8.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate8.TabIndex = 0x24;
            this.labelMessageTemplate7.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate7.AutoSize = true;
            this.labelMessageTemplate7.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate7.Location = new Point(12, 0x201);
            this.labelMessageTemplate7.Name = "labelMessageTemplate7";
            this.labelMessageTemplate7.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate7.TabIndex = 0x23;
            this.labelMessageTemplate7.Text = "Message Template 7";
            this.textBoxMessageTemplate7.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate7.Location = new Point(12, 0x214);
            this.textBoxMessageTemplate7.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate7.MaxLength = 250;
            this.textBoxMessageTemplate7.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate7.Multiline = true;
            this.textBoxMessageTemplate7.Name = "textBoxMessageTemplate7";
            this.textBoxMessageTemplate7.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate7.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate7.TabIndex = 0x21;
            this.labelMessageTemplate10.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate10.AutoSize = true;
            this.labelMessageTemplate10.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate10.Location = new Point(12, 0x2f4);
            this.labelMessageTemplate10.Name = "labelMessageTemplate10";
            this.labelMessageTemplate10.Size = new Size(0x87, 0x10);
            this.labelMessageTemplate10.TabIndex = 0x2c;
            this.labelMessageTemplate10.Text = "Message Template 10";
            this.textBoxMessageTemplate10.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate10.Location = new Point(12, 0x307);
            this.textBoxMessageTemplate10.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate10.MaxLength = 250;
            this.textBoxMessageTemplate10.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate10.Multiline = true;
            this.textBoxMessageTemplate10.Name = "textBoxMessageTemplate10";
            this.textBoxMessageTemplate10.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate10.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate10.TabIndex = 0x2a;
            this.labelMessageTemplate9.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelMessageTemplate9.AutoSize = true;
            this.labelMessageTemplate9.Font = new Font("Arial", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.labelMessageTemplate9.Location = new Point(13, 0x2a3);
            this.labelMessageTemplate9.Name = "labelMessageTemplate9";
            this.labelMessageTemplate9.Size = new Size(0x80, 0x10);
            this.labelMessageTemplate9.TabIndex = 0x29;
            this.labelMessageTemplate9.Text = "Message Template 9";
            this.textBoxMessageTemplate9.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxMessageTemplate9.Location = new Point(13, 0x2b6);
            this.textBoxMessageTemplate9.MaximumSize = new Size(800, 60);
            this.textBoxMessageTemplate9.MaxLength = 250;
            this.textBoxMessageTemplate9.MinimumSize = new Size(0x13b, 60);
            this.textBoxMessageTemplate9.Multiline = true;
            this.textBoxMessageTemplate9.Name = "textBoxMessageTemplate9";
            this.textBoxMessageTemplate9.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessageTemplate9.Size = new Size(0x13b, 60);
            this.textBoxMessageTemplate9.TabIndex = 0x27;
            this.buttonSave2.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.buttonSave2.Location = new Point(0xf7, 8);
            this.buttonSave2.Name = "buttonSave2";
            this.buttonSave2.Size = new Size(80, 0x19);
            this.buttonSave2.TabIndex = 0x2d;
            this.buttonSave2.Text = "Save All";
            this.buttonSave2.UseVisualStyleBackColor = true;
            this.buttonSave2.Click += new EventHandler(this.buttonSave_Click);
            this.panelMessageTemplate.AutoScroll = true;
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate9);
            this.panelMessageTemplate.Controls.Add(this.buttonSave2);
            this.panelMessageTemplate.Controls.Add(this.buttonSave);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate10);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate1);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate10);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate1);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate9);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate2);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate2);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate8);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate3);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate8);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate3);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate7);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate4);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate7);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate4);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate6);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate5);
            this.panelMessageTemplate.Controls.Add(this.textBoxMessageTemplate6);
            this.panelMessageTemplate.Controls.Add(this.labelMessageTemplate5);
            this.panelMessageTemplate.Dock = DockStyle.Fill;
            this.panelMessageTemplate.Location = new Point(0, 0);
            this.panelMessageTemplate.Name = "panelMessageTemplate";
            this.panelMessageTemplate.Size = new Size(0x162, 0x1a7);
            this.panelMessageTemplate.TabIndex = 0x2e;
            this.rapidSpellAsYouTypeTemplate1.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate1.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate1.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate1.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate1.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate1.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate1.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate1.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate1.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate1.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate1.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate1.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate1.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate1.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate1.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate1.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate1.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate1.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate1.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate1.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate1.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate1.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate1.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate1.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate1.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate1.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate1.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate1.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate1.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate1.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate1.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate1.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate1.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate1.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate1.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate1.TextBoxBase = this.textBoxMessageTemplate1;
            this.rapidSpellAsYouTypeTemplate1.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate1.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate1.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate1.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate1.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate1.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate1.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate2.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate2.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate2.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate2.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate2.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate2.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate2.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate2.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate2.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate2.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate2.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate2.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate2.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate2.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate2.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate2.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate2.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate2.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate2.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate2.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate2.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate2.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate2.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate2.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate2.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate2.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate2.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate2.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate2.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate2.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate2.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate2.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate2.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate2.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate2.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate2.TextBoxBase = this.textBoxMessageTemplate2;
            this.rapidSpellAsYouTypeTemplate2.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate2.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate2.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate2.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate2.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate2.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate2.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate3.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate3.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate3.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate3.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate3.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate3.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate3.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate3.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate3.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate3.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate3.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate3.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate3.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate3.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate3.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate3.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate3.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate3.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate3.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate3.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate3.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate3.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate3.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate3.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate3.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate3.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate3.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate3.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate3.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate3.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate3.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate3.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate3.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate3.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate3.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate3.TextBoxBase = this.textBoxMessageTemplate3;
            this.rapidSpellAsYouTypeTemplate3.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate3.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate3.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate3.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate3.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate3.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate3.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate4.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate4.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate4.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate4.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate4.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate4.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate4.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate4.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate4.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate4.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate4.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate4.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate4.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate4.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate4.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate4.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate4.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate4.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate4.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate4.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate4.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate4.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate4.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate4.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate4.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate4.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate4.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate4.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate4.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate4.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate4.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate4.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate4.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate4.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate4.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate4.TextBoxBase = this.textBoxMessageTemplate4;
            this.rapidSpellAsYouTypeTemplate4.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate4.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate4.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate4.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate4.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate4.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate4.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate5.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate5.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate5.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate5.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate5.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate5.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate5.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate5.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate5.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate5.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate5.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate5.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate5.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate5.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate5.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate5.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate5.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate5.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate5.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate5.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate5.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate5.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate5.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate5.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate5.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate5.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate5.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate5.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate5.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate5.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate5.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate5.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate5.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate5.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate5.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate5.TextBoxBase = this.textBoxMessageTemplate5;
            this.rapidSpellAsYouTypeTemplate5.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate5.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate5.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate5.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate5.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate5.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate5.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate6.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate6.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate6.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate6.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate6.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate6.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate6.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate6.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate6.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate6.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate6.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate6.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate6.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate6.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate6.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate6.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate6.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate6.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate6.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate6.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate6.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate6.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate6.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate6.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate6.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate6.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate6.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate6.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate6.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate6.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate6.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate6.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate6.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate6.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate6.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate6.TextBoxBase = this.textBoxMessageTemplate6;
            this.rapidSpellAsYouTypeTemplate6.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate6.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate6.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate6.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate6.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate6.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate6.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate7.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate7.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate7.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate7.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate7.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate7.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate7.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate7.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate7.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate7.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate7.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate7.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate7.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate7.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate7.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate7.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate7.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate7.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate7.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate7.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate7.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate7.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate7.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate7.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate7.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate7.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate7.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate7.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate7.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate7.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate7.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate7.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate7.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate7.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate7.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate7.TextBoxBase = this.textBoxMessageTemplate7;
            this.rapidSpellAsYouTypeTemplate7.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate7.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate7.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate7.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate7.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate7.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate7.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate8.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate8.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate8.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate8.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate8.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate8.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate8.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate8.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate8.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate8.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate8.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate8.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate8.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate8.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate8.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate8.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate8.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate8.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate8.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate8.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate8.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate8.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate8.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate8.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate8.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate8.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate8.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate8.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate8.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate8.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate8.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate8.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate8.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate8.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate8.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate8.TextBoxBase = this.textBoxMessageTemplate8;
            this.rapidSpellAsYouTypeTemplate8.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate8.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate8.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate8.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate8.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate8.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate8.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate9.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate9.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate9.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate9.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate9.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate9.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate9.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate9.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate9.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate9.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate9.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate9.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate9.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate9.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate9.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate9.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate9.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate9.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate9.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate9.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate9.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate9.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate9.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate9.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate9.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate9.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate9.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate9.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate9.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate9.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate9.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate9.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate9.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate9.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate9.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate9.TextBoxBase = this.textBoxMessageTemplate9;
            this.rapidSpellAsYouTypeTemplate9.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate9.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate9.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate9.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate9.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate9.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate9.WarnDuplicates = true;
            this.rapidSpellAsYouTypeTemplate10.AddMenuText = "Add";
            this.rapidSpellAsYouTypeTemplate10.AllowAnyCase = false;
            this.rapidSpellAsYouTypeTemplate10.AllowMixedCase = false;
            this.rapidSpellAsYouTypeTemplate10.AutoCorrectEnabled = true;
            this.rapidSpellAsYouTypeTemplate10.CheckAsYouType = true;
            this.rapidSpellAsYouTypeTemplate10.CheckCompoundWords = false;
            this.rapidSpellAsYouTypeTemplate10.CheckDisabledTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate10.CheckReadOnlyTextBoxes = false;
            this.rapidSpellAsYouTypeTemplate10.ConsiderationRange = 500;
            this.rapidSpellAsYouTypeTemplate10.ContextMenuStripEnabled = true;
            this.rapidSpellAsYouTypeTemplate10.DictFilePath = null;
            this.rapidSpellAsYouTypeTemplate10.FindCapitalizedSuggestions = true;
            this.rapidSpellAsYouTypeTemplate10.GUILanguage = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate10.IgnoreAllMenuText = "Ignore All";
            this.rapidSpellAsYouTypeTemplate10.IgnoreCapitalizedWords = false;
            this.rapidSpellAsYouTypeTemplate10.IgnoreIncorrectSentenceCapitalization = false;
            this.rapidSpellAsYouTypeTemplate10.IgnoreInEnglishLowerCaseI = false;
            this.rapidSpellAsYouTypeTemplate10.IgnoreMenuText = "Ignore";
            this.rapidSpellAsYouTypeTemplate10.IgnoreURLsAndEmailAddresses = true;
            this.rapidSpellAsYouTypeTemplate10.IgnoreWordsWithDigits = true;
            this.rapidSpellAsYouTypeTemplate10.IgnoreXML = false;
            this.rapidSpellAsYouTypeTemplate10.IncludeUserDictionaryInSuggestions = false;
            this.rapidSpellAsYouTypeTemplate10.LanguageParser = LanguageType.ENGLISH;
            this.rapidSpellAsYouTypeTemplate10.LookIntoHyphenatedText = true;
            this.rapidSpellAsYouTypeTemplate10.OptionsEnabled = true;
            this.rapidSpellAsYouTypeTemplate10.OptionsFileName = "RapidSpell_UserSettings.xml";
            this.rapidSpellAsYouTypeTemplate10.OptionsStorageLocation = UserOptions.StorageType.IsolatedStorage;
            this.rapidSpellAsYouTypeTemplate10.RemoveDuplicateWordText = "Remove duplicate word";
            this.rapidSpellAsYouTypeTemplate10.SeparateHyphenWords = false;
            this.rapidSpellAsYouTypeTemplate10.ShowAddMenuOption = true;
            this.rapidSpellAsYouTypeTemplate10.ShowCutCopyPasteMenuOnTextBoxBase = true;
            this.rapidSpellAsYouTypeTemplate10.ShowSuggestionsContextMenu = true;
            this.rapidSpellAsYouTypeTemplate10.ShowSuggestionsWhenTextIsSelected = false;
            this.rapidSpellAsYouTypeTemplate10.SuggestionsMethod = SuggestionsMethodType.HashingSuggestions;
            this.rapidSpellAsYouTypeTemplate10.SuggestSplitWords = true;
            this.rapidSpellAsYouTypeTemplate10.TextBoxBase = this.textBoxMessageTemplate10;
            this.rapidSpellAsYouTypeTemplate10.TextComponent = null;
            this.rapidSpellAsYouTypeTemplate10.UnderlineColor = System.Drawing.Color.Red;
            this.rapidSpellAsYouTypeTemplate10.UnderlineStyle = UnderlineStyle.Wavy;
            this.rapidSpellAsYouTypeTemplate10.UpdateAllTextBoxes = true;
            this.rapidSpellAsYouTypeTemplate10.UserDictionaryFile = null;
            this.rapidSpellAsYouTypeTemplate10.V2Parser = true;
            this.rapidSpellAsYouTypeTemplate10.WarnDuplicates = true;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x162, 0x1a7);
            base.Controls.Add(this.panelMessageTemplate);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            this.MaximumSize = new Size(370, 0x390);
            base.MinimizeBox = false;
            this.MinimumSize = new Size(370, 300);
            base.Name = "fmMessageTemplate";
            this.Text = "Edit Message Template";
            base.FormClosing += new FormClosingEventHandler(this.fmMessageTemplate_FormClosing);
            base.Load += new EventHandler(this.fmMessageTemplate_Load);
            this.panelMessageTemplate.ResumeLayout(false);
            this.panelMessageTemplate.PerformLayout();
            base.ResumeLayout(false);
        }

        public ApplicationManager appManager { get; set; }
    }
}


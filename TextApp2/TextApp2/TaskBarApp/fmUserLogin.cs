using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TaskBarApp
{
	public class fmUserLogin : Form
	{
		private string strError = string.Empty;

		private IContainer components;

		private Label lblUserName;

		private Label lblPassword;

		private MaskedTextBox txtUserName;

		private TextBox txtPassword;

		private Button btnUserLogin;

		private CheckBox ckbSaveLogIn;

		private Label labelCountry;

		private ComboBox comboBoxCountry;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmUserLogin()
		{
			this.InitializeComponent();
			base.KeyPress += new KeyPressEventHandler(this.keyDown_Enter);
		}

		private void fmUserLogin_Load(object sender, EventArgs e)
		{
			try
			{
				this.Text = this.appManager.m_strApplicationName + " Log In";
				base.Icon = this.appManager.iTextApp;
				if (this.appManager.m_strLoginSplashPath != string.Empty)
				{
					Bitmap backgroundImage = (Bitmap)Image.FromFile(this.appManager.m_strLoginSplashPath);
					this.BackgroundImage = backgroundImage;
					this.BackgroundImageLayout = ImageLayout.Stretch;
				}
				if (this.appManager.m_strUserName != string.Empty && this.appManager.m_strPassword != string.Empty)
				{
					this.txtUserName.Text = this.appManager.m_strUserName;
					this.txtPassword.Text = this.appManager.m_strPassword;
				}
				this.LoadCountryDDL();
				this.comboBoxCountry.SelectedValue = "USA";
				this.ckbSaveLogIn.Checked = this.appManager.m_bSaveLogIn;
				this.txtUserName.Focus();
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception creating user log in form: " + ex.Message, 5);
			}
		}

		private void LoadCountryDDL()
		{
			this.comboBoxCountry.DisplayMember = "Text";
			this.comboBoxCountry.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "United States",
					Value = "USA"
				},
				new
				{
					Text = "Canada",
					Value = "CAN"
				}
			};
			this.comboBoxCountry.DataSource = dataSource;
			this.comboBoxCountry.SelectedValue = this.appManager.m_strCountryCode;
		}

		private void keyDown_Enter(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btnUserLogin.PerformClick();
			}
		}

		private void btnUserLogin_Click(object sender, EventArgs e)
		{
			string empty = string.Empty;
			this.btnUserLogin.Enabled = false;
			if (this.txtUserName.Text.Length == 0 || this.txtPassword.Text.Length == 0)
			{
				this.btnUserLogin.Enabled = true;
				return;
			}
			this.txtUserName.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
			string text = this.txtUserName.Text;
			string text2 = this.txtPassword.Text;
			string arg_6C_0 = string.Empty;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref empty);
			try
			{
				AppRegistry.SaveUserName(rootKey, text, ref empty);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nUserName save error: " + empty;
				}
				AppRegistry.SavePassword(rootKey, text2, ref empty);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nPassword save error: " + empty;
				}
				AppRegistry.SaveValue(rootKey, "local_IsLoggedOut", false, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nIsLoggedOut save error: " + empty;
				}
				AppRegistry.SaveValue(rootKey, "local_SaveLogIn", this.ckbSaveLogIn.Checked, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nSaveLogIn save error: " + empty;
				}
				AppRegistry.SaveValue(rootKey, "local_CountryCode", this.comboBoxCountry.SelectedValue, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "\nSaveCountryCode save error: " + empty;
				}
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception with the login information: " + ex.Message + "\nError details: " + this.strError, 5);
			}
			base.Close();
			this.appManager.LogIn(true, null, null, null, null);
		}

		private void txtUserName_Click(object sender, EventArgs e)
		{
			this.PositionCursorInMaskedTextBox(this.txtUserName);
		}

		private void PositionCursorInMaskedTextBox(MaskedTextBox mtb)
		{
			if (mtb == null)
			{
				return;
			}
			mtb.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
			if (mtb.Text.Length <= 3)
			{
				mtb.Select(mtb.Text.Length + 1, 0);
			}
			if (mtb.Text.Length > 3 && mtb.Text.Length <= 6)
			{
				mtb.Select(mtb.Text.Length + 3, 0);
			}
			if (mtb.Text.Length > 6)
			{
				mtb.Select(mtb.Text.Length + 4, 0);
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmUserLogin));
			this.lblUserName = new Label();
			this.lblPassword = new Label();
			this.txtUserName = new MaskedTextBox();
			this.txtPassword = new TextBox();
			this.btnUserLogin = new Button();
			this.ckbSaveLogIn = new CheckBox();
			this.labelCountry = new Label();
			this.comboBoxCountry = new ComboBox();
			base.SuspendLayout();
			this.lblUserName.AutoSize = true;
			this.lblUserName.BackColor = Color.Transparent;
			this.lblUserName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblUserName.ForeColor = Color.Black;
			this.lblUserName.Location = new Point(242, 27);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new Size(110, 17);
			this.lblUserName.TabIndex = 0;
			this.lblUserName.Text = "Phone Number:";
			this.lblPassword.AutoSize = true;
			this.lblPassword.BackColor = Color.Transparent;
			this.lblPassword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblPassword.ForeColor = Color.Black;
			this.lblPassword.Location = new Point(242, 73);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new Size(78, 17);
			this.lblPassword.TabIndex = 1;
			this.lblPassword.Text = "Password:";
			this.txtUserName.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.txtUserName.Location = new Point(358, 24);
			this.txtUserName.Mask = "(000) 000-0000";
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.ResetOnSpace = false;
			this.txtUserName.Size = new Size(124, 25);
			this.txtUserName.TabIndex = 1;
			this.txtUserName.Click += new EventHandler(this.txtUserName_Click);
			this.txtPassword.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.txtPassword.Location = new Point(326, 70);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Size = new Size(156, 25);
			this.txtPassword.TabIndex = 2;
			this.txtPassword.UseSystemPasswordChar = true;
			this.btnUserLogin.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.btnUserLogin.Location = new Point(413, 161);
			this.btnUserLogin.Name = "btnUserLogin";
			this.btnUserLogin.Size = new Size(69, 27);
			this.btnUserLogin.TabIndex = 10;
			this.btnUserLogin.Text = "Log In";
			this.btnUserLogin.UseVisualStyleBackColor = true;
			this.btnUserLogin.Click += new EventHandler(this.btnUserLogin_Click);
			this.ckbSaveLogIn.AutoSize = true;
			this.ckbSaveLogIn.BackColor = Color.Transparent;
			this.ckbSaveLogIn.Checked = true;
			this.ckbSaveLogIn.CheckState = CheckState.Checked;
			this.ckbSaveLogIn.Font = new Font("Arial", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.ckbSaveLogIn.Location = new Point(246, 166);
			this.ckbSaveLogIn.Name = "ckbSaveLogIn";
			this.ckbSaveLogIn.Size = new Size(165, 20);
			this.ckbSaveLogIn.TabIndex = 20;
			this.ckbSaveLogIn.TabStop = false;
			this.ckbSaveLogIn.Text = "Save Log In Information";
			this.ckbSaveLogIn.UseVisualStyleBackColor = false;
			this.labelCountry.AutoSize = true;
			this.labelCountry.BackColor = Color.Transparent;
			this.labelCountry.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelCountry.ForeColor = Color.Black;
			this.labelCountry.Location = new Point(243, 119);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new Size(63, 17);
			this.labelCountry.TabIndex = 11;
			this.labelCountry.Text = "Country:";
			this.comboBoxCountry.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxCountry.FlatStyle = FlatStyle.Flat;
			this.comboBoxCountry.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxCountry.FormattingEnabled = true;
			this.comboBoxCountry.Location = new Point(326, 116);
			this.comboBoxCountry.Name = "comboBoxCountry";
			this.comboBoxCountry.Size = new Size(156, 25);
			this.comboBoxCountry.TabIndex = 3;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			base.ClientSize = new Size(498, 200);
			base.Controls.Add(this.comboBoxCountry);
			base.Controls.Add(this.labelCountry);
			base.Controls.Add(this.ckbSaveLogIn);
			base.Controls.Add(this.btnUserLogin);
			base.Controls.Add(this.txtPassword);
			base.Controls.Add(this.txtUserName);
			base.Controls.Add(this.lblPassword);
			base.Controls.Add(this.lblUserName);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "fmUserLogin";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Log In";
			base.Load += new EventHandler(this.fmUserLogin_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

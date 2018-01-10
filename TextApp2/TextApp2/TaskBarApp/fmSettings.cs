using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace TaskBarApp
{
	public class fmSettings : Form
	{
		private string strError = string.Empty;

		private DialogResult changeEnableDashbaordMode;

		private bool bRefreshMessageForm;

		private bool bLoading;

		private IContainer components;

		private Button buttonSave;

		private ComboBox comboBoxNotificationReminder;

		private Label labelReminderInterval;

		private TextBox textBoxSignature;

		private CheckBox checkBoxEnableSignature;

		private Label labelDashboardRefreshInterval;

		private ComboBox comboBoxDashboardRefreshInterval;

		private CheckBox checkBoxEnableDashboard;

		private Label labelLastMessage;

		private ComboBox comboBoxLastMessageStatus;

		private Button buttonHelp;

		private Label labelNotificationSound;

		private ComboBox comboBoxNotificationSound;

		private Label labelUnreadMessageLimit;

		private ComboBox comboBoxUnreadMessageLimit;

		private ComboBox comboBoxDashboardNotificationSound;

		private Label labelDashboardNotificationSound;

		private Label labelMachineID;

		private CheckBox checkBoxDisableDashboardSettingChangeNotifications;

		private Label labelFontSize;

		private ComboBox comboBoxFontSize;

		private Label labelFontSizeDT;

		private ComboBox comboBoxFontSizeDT;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmSettings()
		{
			this.InitializeComponent();
		}

		private void fmSettings_Load(object sender, EventArgs e)
		{
			try
			{
				this.Text = this.appManager.m_strApplicationName + " Settings";
				base.Icon = this.appManager.iTextApp;
				string arg_31_0 = string.Empty;
				this.checkBoxEnableSignature.Checked = this.appManager.m_bEnableSignature;
				this.textBoxSignature.Enabled = this.appManager.m_bEnableSignature;
				this.textBoxSignature.Text = this.appManager.m_strSignature;
				this.checkBoxEnableDashboard.Checked = this.appManager.m_bDashboardMode;
				this.checkBoxDisableDashboardSettingChangeNotifications.Checked = this.appManager.m_bDisableDashboardSettingChangeNotifications;
				this.labelMachineID.Text = "Machine ID: " + this.appManager.m_strMachineID;
				if (this.appManager.m_bDashboardFeature)
				{
					this.checkBoxEnableDashboard.Enabled = true;
				}
				else
				{
					this.checkBoxEnableDashboard.Enabled = false;
				}
				if (this.appManager.m_bDashboardMode)
				{
					base.Height = this.MaximumSize.Height;
					this.labelDashboardNotificationSound.Visible = true;
					this.labelDashboardRefreshInterval.Visible = true;
					this.comboBoxDashboardRefreshInterval.Visible = true;
					this.comboBoxDashboardNotificationSound.Visible = true;
					this.checkBoxDisableDashboardSettingChangeNotifications.Visible = true;
				}
				else
				{
					base.Height = this.MinimumSize.Height;
					this.labelDashboardNotificationSound.Visible = false;
					this.labelDashboardRefreshInterval.Visible = false;
					this.comboBoxDashboardRefreshInterval.Visible = false;
					this.comboBoxDashboardNotificationSound.Visible = false;
					this.checkBoxDisableDashboardSettingChangeNotifications.Visible = false;
				}
				this.LoadNotificationReminderIntervalDDL();
				this.LoadDashbaordNotificationSoundlDDL();
				this.LoadDashboardRefreshIntervalDDL();
				this.LoadLastMessageStatusLimitDDL();
				this.LoadNotificationSoundlDDL();
				this.LoadUnreadMessageLimitDDL();
				this.LoadFontSizeDDL();
				this.LoadFontSizeDTDDL();
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception creating settings form: " + ex.Message, 5);
			}
		}

		private void LoadNotificationSoundlDDL()
		{
			this.bLoading = true;
			this.comboBoxNotificationSound.DisplayMember = "Text";
			this.comboBoxNotificationSound.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "None",
					Value = "None"
				},
				new
				{
					Text = "Bubble",
					Value = "Bubble.wav"
				},
				new
				{
					Text = "Doorbell",
					Value = "DoorBell.wav"
				},
				new
				{
					Text = "Electric Dog",
					Value = "ElectricDog.wav"
				},
				new
				{
					Text = "String",
					Value = "String.wav"
				},
				new
				{
					Text = "Water Drop",
					Value = "WaterDrop.wav"
				}
			};
			this.comboBoxNotificationSound.DataSource = dataSource;
			this.comboBoxNotificationSound.SelectedValue = this.appManager.m_strNotificationSound;
			this.bLoading = false;
		}

		private void LoadNotificationReminderIntervalDDL()
		{
			this.comboBoxNotificationReminder.DisplayMember = "Text";
			this.comboBoxNotificationReminder.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "5 min",
					Value = "5"
				},
				new
				{
					Text = "15 min",
					Value = "15"
				},
				new
				{
					Text = "30 min",
					Value = "30"
				},
				new
				{
					Text = "60 min",
					Value = "60"
				}
			};
			this.comboBoxNotificationReminder.DataSource = dataSource;
			this.comboBoxNotificationReminder.SelectedIndex = this.comboBoxNotificationReminder.FindString(this.appManager.m_nNotificationInterval.ToString());
		}

		private void LoadLastMessageStatusLimitDDL()
		{
			this.comboBoxLastMessageStatus.DisplayMember = "Text";
			this.comboBoxLastMessageStatus.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "None",
					Value = "0"
				},
				new
				{
					Text = "250",
					Value = "250"
				},
				new
				{
					Text = "500",
					Value = "500"
				},
				new
				{
					Text = "999",
					Value = "999"
				}
			};
			this.comboBoxLastMessageStatus.DataSource = dataSource;
			if (this.appManager.m_nLastMessageStatusLimit == 0)
			{
				this.comboBoxLastMessageStatus.SelectedIndex = this.comboBoxLastMessageStatus.FindString("None");
				return;
			}
			this.comboBoxLastMessageStatus.SelectedIndex = this.comboBoxLastMessageStatus.FindString(this.appManager.m_nLastMessageStatusLimit.ToString());
		}

		private void LoadDashboardRefreshIntervalDDL()
		{
			this.comboBoxDashboardRefreshInterval.DisplayMember = "Text";
			this.comboBoxDashboardRefreshInterval.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "1",
					Value = "1"
				},
				new
				{
					Text = "5",
					Value = "5"
				},
				new
				{
					Text = "10",
					Value = "10"
				},
				new
				{
					Text = "30",
					Value = "30"
				},
				new
				{
					Text = "60",
					Value = "60"
				}
			};
			this.comboBoxDashboardRefreshInterval.DataSource = dataSource;
			this.comboBoxDashboardRefreshInterval.SelectedIndex = this.comboBoxDashboardRefreshInterval.FindString(this.appManager.m_nAccountDashboardRefreshInterval.ToString());
		}

		private void LoadDashbaordNotificationSoundlDDL()
		{
			this.bLoading = true;
			this.comboBoxDashboardNotificationSound.DisplayMember = "Text";
			this.comboBoxDashboardNotificationSound.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "None",
					Value = "None"
				},
				new
				{
					Text = "Bubble",
					Value = "Bubble.wav"
				},
				new
				{
					Text = "Doorbell",
					Value = "DoorBell.wav"
				},
				new
				{
					Text = "Electric Dog",
					Value = "ElectricDog.wav"
				},
				new
				{
					Text = "String",
					Value = "String.wav"
				},
				new
				{
					Text = "Water Drop",
					Value = "WaterDrop.wav"
				}
			};
			this.comboBoxDashboardNotificationSound.DataSource = dataSource;
			this.comboBoxDashboardNotificationSound.SelectedValue = this.appManager.m_strDashboardNotificationSound;
			this.bLoading = false;
		}

		private void LoadUnreadMessageLimitDDL()
		{
			this.comboBoxUnreadMessageLimit.DisplayMember = "Text";
			this.comboBoxUnreadMessageLimit.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "250",
					Value = "250"
				},
				new
				{
					Text = "500",
					Value = "500"
				},
				new
				{
					Text = "999",
					Value = "999"
				}
			};
			this.comboBoxUnreadMessageLimit.DataSource = dataSource;
			this.comboBoxUnreadMessageLimit.SelectedIndex = this.comboBoxUnreadMessageLimit.FindString(this.appManager.m_nUnreadMessageLimit.ToString());
		}

		private void LoadFontSizeDDL()
		{
			this.comboBoxFontSize.DisplayMember = "Text";
			this.comboBoxFontSize.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "6",
					Value = "6"
				},
				new
				{
					Text = "8",
					Value = "8"
				},
				new
				{
					Text = "10",
					Value = "10"
				},
				new
				{
					Text = "12",
					Value = "12"
				},
				new
				{
					Text = "14",
					Value = "14"
				},
				new
				{
					Text = "16",
					Value = "16"
				},
				new
				{
					Text = "18",
					Value = "18"
				}
			};
			this.comboBoxFontSize.DataSource = dataSource;
			this.comboBoxFontSize.SelectedIndex = this.comboBoxFontSize.FindString(this.appManager.m_nFontSize.ToString());
		}

		private void LoadFontSizeDTDDL()
		{
			this.comboBoxFontSizeDT.DisplayMember = "Text";
			this.comboBoxFontSizeDT.ValueMember = "Value";
			var dataSource = new <>f__AnonymousType0<string, string>[]
			{
				new
				{
					Text = "6",
					Value = "6"
				},
				new
				{
					Text = "8",
					Value = "8"
				},
				new
				{
					Text = "10",
					Value = "10"
				},
				new
				{
					Text = "12",
					Value = "12"
				},
				new
				{
					Text = "14",
					Value = "14"
				},
				new
				{
					Text = "16",
					Value = "16"
				},
				new
				{
					Text = "18",
					Value = "18"
				}
			};
			this.comboBoxFontSizeDT.DataSource = dataSource;
			this.comboBoxFontSizeDT.SelectedIndex = this.comboBoxFontSizeDT.FindString(this.appManager.m_nFontSizeDT.ToString());
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			this.appManager.LaunchWebsite(this.appManager.m_strSettingsURL);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			bool flag = true;
			string empty = string.Empty;
			RegistryKey rootKey = AppRegistry.GetRootKey(ref empty);
			try
			{
				AppRegistry.SaveValue(rootKey, "NotificationSound", this.comboBoxNotificationSound.SelectedValue.ToString(), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Notification sound save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_strNotificationSound = this.comboBoxNotificationSound.SelectedValue.ToString();
					if (this.appManager.m_strNotificationSound == "None")
					{
						this.appManager.m_bPlaySound = false;
					}
					else
					{
						this.appManager.m_bPlaySound = true;
					}
				}
			}
			catch (Exception ex)
			{
				this.strError = this.strError + "Notification sound save error: " + ex.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "NotificationInterval", Convert.ToInt32(this.comboBoxNotificationReminder.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Notificaiton reminder save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_nNotificationInterval = Convert.ToInt32(this.comboBoxNotificationReminder.SelectedValue);
					this.appManager.m_dtNextNotification = DateTime.Now.AddMinutes((double)this.appManager.m_nNotificationInterval);
				}
			}
			catch (Exception ex2)
			{
				this.strError = this.strError + "Notificaiton reminder save error: " + ex2.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "FontSize", Convert.ToInt32(this.comboBoxFontSize.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Font size save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_nFontSize = Convert.ToInt32(this.comboBoxFontSize.SelectedValue);
					this.appManager.m_fontSize = new Font("Arial", (float)this.appManager.m_nFontSize);
					this.bRefreshMessageForm = true;
				}
			}
			catch (Exception ex3)
			{
				this.strError = this.strError + "Font size save error: " + ex3.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "FontSizeDT", Convert.ToInt32(this.comboBoxFontSizeDT.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Font size Date/Time save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_nFontSizeDT = Convert.ToInt32(this.comboBoxFontSizeDT.SelectedValue);
					this.appManager.m_fontSizeDT = new Font("Arial", (float)this.appManager.m_nFontSizeDT);
					this.bRefreshMessageForm = true;
				}
			}
			catch (Exception ex4)
			{
				this.strError = this.strError + "Font size Date/Time save error: " + ex4.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "LastMessageStatusLimit", Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Last message status save error: " + empty;
					flag = false;
				}
				else if (this.appManager.m_nLastMessageStatusLimit != Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue))
				{
					this.appManager.m_nLastMessageStatusLimit = Convert.ToInt32(this.comboBoxLastMessageStatus.SelectedValue);
					this.bRefreshMessageForm = true;
					if (this.appManager.m_nLastMessageStatusLimit == 0)
					{
						this.appManager.m_lsConversationMetaData.Clear();
					}
					else
					{
						this.appManager.LoadConversations(true);
					}
				}
			}
			catch (Exception ex5)
			{
				this.strError = this.strError + "Last message status save error: " + ex5.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "UnreadMessageLimit", Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "UnreadyMessage Count limit save error: " + empty;
					flag = false;
				}
				else if (this.appManager.m_nUnreadMessageLimit != Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue))
				{
					this.appManager.m_nUnreadMessageLimit = Convert.ToInt32(this.comboBoxUnreadMessageLimit.SelectedValue);
				}
			}
			catch (Exception ex6)
			{
				this.strError = this.strError + "Unread Message Count limit save error: " + ex6.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "EnableSignature", this.checkBoxEnableSignature.Checked, ref empty, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(rootKey, "Signature", this.textBoxSignature.Text, ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Enable Signature save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_bEnableSignature = this.checkBoxEnableSignature.Checked;
					this.appManager.m_strSignature = this.textBoxSignature.Text;
					this.bRefreshMessageForm = true;
				}
			}
			catch (Exception ex7)
			{
				this.strError = this.strError + "Enable Signature save error: " + ex7.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "local_AccountDashboardRefreshInterval", Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Dashboard refresh interval save error: " + empty;
					flag = false;
				}
				else if (this.appManager.m_nAccountDashboardRefreshInterval != Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue))
				{
					this.appManager.m_nAccountDashboardRefreshInterval = Convert.ToInt32(this.comboBoxDashboardRefreshInterval.SelectedValue);
					this.appManager.StartMonitorAccountDashboardTimer();
				}
			}
			catch (Exception ex8)
			{
				this.strError = this.strError + "Dashboard refresh interval save error: " + ex8.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "local_DashboardNotificationSound", this.comboBoxDashboardNotificationSound.SelectedValue.ToString(), ref empty, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Dashboard notification sound save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_strDashboardNotificationSound = this.comboBoxDashboardNotificationSound.SelectedValue.ToString();
					if (this.appManager.m_strDashboardNotificationSound == "None")
					{
						this.appManager.m_bPlayDashboardSound = false;
					}
					else
					{
						this.appManager.m_bPlayDashboardSound = true;
					}
				}
			}
			catch (Exception ex9)
			{
				this.strError = this.strError + "Dashboard notification sound save error: " + ex9.Message;
				flag = false;
			}
			try
			{
				AppRegistry.SaveValue(rootKey, "local_DisableDashboardSettingChangeNotifications", this.checkBoxDisableDashboardSettingChangeNotifications.Checked, ref this.strError, false, RegistryValueKind.Unknown);
				if (empty != string.Empty)
				{
					this.strError = this.strError + "Dashboard Settign Change Notification save error: " + empty;
					flag = false;
				}
				else
				{
					this.appManager.m_bDisableDashboardSettingChangeNotifications = this.checkBoxDisableDashboardSettingChangeNotifications.Checked;
				}
			}
			catch (Exception ex10)
			{
				this.strError = this.strError + "Dashboard Settign Change Notification save error: " + ex10.Message;
				flag = false;
			}
			if (this.appManager.m_bDashboardMode != this.checkBoxEnableDashboard.Checked)
			{
				this.changeEnableDashbaordMode = MessageBox.Show(string.Concat(new string[]
				{
					"You must exit and re-open the ",
					this.appManager.m_strApplicationName,
					" application for the change to Dashboard mode to take effect.  Would you like to exit ",
					this.appManager.m_strApplicationName,
					" now?"
				}), "Multiple Account Mode Change", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (this.changeEnableDashbaordMode == DialogResult.Yes)
				{
					try
					{
						AppRegistry.SaveValue(rootKey, "local_DashboardMode", this.checkBoxEnableDashboard.Checked, ref empty, false, RegistryValueKind.Unknown);
						if (empty != string.Empty)
						{
							this.strError = this.strError + "Enable Dashboard save error: " + empty;
							flag = false;
						}
						else
						{
							AppRegistry.SaveUserName(rootKey, "", ref empty);
							if (empty != string.Empty)
							{
								this.strError = this.strError + "\nUserName Save Error: " + empty;
							}
							AppRegistry.SavePassword(rootKey, "", ref empty);
							if (empty != string.Empty)
							{
								this.strError = this.strError + "\nPassword Save Error: " + empty;
							}
						}
					}
					catch (Exception ex11)
					{
						this.strError = this.strError + "Enable Dashboard save error: " + ex11.Message;
						flag = false;
					}
				}
			}
			if (!flag)
			{
				this.appManager.ShowBalloon("Exception saving settings: " + this.strError, 5);
				return;
			}
			if (this.changeEnableDashbaordMode == DialogResult.Yes)
			{
				Application.Exit();
				return;
			}
			if (this.changeEnableDashbaordMode == DialogResult.No)
			{
				this.checkBoxEnableDashboard.Checked = this.appManager.m_bDashboardMode;
				return;
			}
			base.Close();
			if (this.appManager.formMessages != null && this.bRefreshMessageForm)
			{
				this.appManager.formMessages.Close();
				this.appManager.ShowMessages();
			}
			this.appManager.ShowBalloon("Your settings have been saved.", 5);
		}

		private void checkBoxSignature_CheckedChanged(object sender, EventArgs e)
		{
			if (this.checkBoxEnableSignature.Checked)
			{
				this.textBoxSignature.Enabled = true;
				return;
			}
			this.textBoxSignature.Enabled = false;
		}

		private void comboBoxNotificationSound_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.bLoading && this.comboBoxNotificationSound.SelectedValue.ToString() != "None")
			{
				new SoundPlayer(this.appManager.m_strSoundPath + this.comboBoxNotificationSound.SelectedValue.ToString()).Play();
			}
		}

		private void comboBoxDashboardNotificationSound_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.bLoading && this.comboBoxDashboardNotificationSound.SelectedValue.ToString() != "None")
			{
				new SoundPlayer(this.appManager.m_strSoundPath + this.comboBoxDashboardNotificationSound.SelectedValue.ToString()).Play();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmSettings));
			this.buttonSave = new Button();
			this.comboBoxNotificationReminder = new ComboBox();
			this.labelReminderInterval = new Label();
			this.textBoxSignature = new TextBox();
			this.checkBoxEnableSignature = new CheckBox();
			this.labelDashboardRefreshInterval = new Label();
			this.comboBoxDashboardRefreshInterval = new ComboBox();
			this.checkBoxEnableDashboard = new CheckBox();
			this.labelLastMessage = new Label();
			this.comboBoxLastMessageStatus = new ComboBox();
			this.buttonHelp = new Button();
			this.labelNotificationSound = new Label();
			this.comboBoxNotificationSound = new ComboBox();
			this.labelUnreadMessageLimit = new Label();
			this.comboBoxUnreadMessageLimit = new ComboBox();
			this.comboBoxDashboardNotificationSound = new ComboBox();
			this.labelDashboardNotificationSound = new Label();
			this.labelMachineID = new Label();
			this.checkBoxDisableDashboardSettingChangeNotifications = new CheckBox();
			this.labelFontSize = new Label();
			this.comboBoxFontSize = new ComboBox();
			this.labelFontSizeDT = new Label();
			this.comboBoxFontSizeDT = new ComboBox();
			base.SuspendLayout();
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(373, 519);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(69, 27);
			this.buttonSave.TabIndex = 0;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.btnSave_Click);
			this.comboBoxNotificationReminder.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxNotificationReminder.FlatStyle = FlatStyle.Flat;
			this.comboBoxNotificationReminder.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxNotificationReminder.FormattingEnabled = true;
			this.comboBoxNotificationReminder.Location = new Point(352, 44);
			this.comboBoxNotificationReminder.Name = "comboBoxNotificationReminder";
			this.comboBoxNotificationReminder.Size = new Size(90, 25);
			this.comboBoxNotificationReminder.TabIndex = 7;
			this.labelReminderInterval.AutoSize = true;
			this.labelReminderInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelReminderInterval.Location = new Point(22, 47);
			this.labelReminderInterval.Name = "labelReminderInterval";
			this.labelReminderInterval.Size = new Size(315, 17);
			this.labelReminderInterval.TabIndex = 8;
			this.labelReminderInterval.Text = "Unread Message Notification Reminder Interval:";
			this.textBoxSignature.Location = new Point(25, 291);
			this.textBoxSignature.Multiline = true;
			this.textBoxSignature.Name = "textBoxSignature";
			this.textBoxSignature.Size = new Size(417, 76);
			this.textBoxSignature.TabIndex = 11;
			this.checkBoxEnableSignature.AutoSize = true;
			this.checkBoxEnableSignature.BackColor = Color.Transparent;
			this.checkBoxEnableSignature.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxEnableSignature.Location = new Point(26, 269);
			this.checkBoxEnableSignature.Name = "checkBoxEnableSignature";
			this.checkBoxEnableSignature.Size = new Size(201, 21);
			this.checkBoxEnableSignature.TabIndex = 13;
			this.checkBoxEnableSignature.Text = "Enable Message Signature";
			this.checkBoxEnableSignature.UseVisualStyleBackColor = false;
			this.checkBoxEnableSignature.CheckedChanged += new EventHandler(this.checkBoxSignature_CheckedChanged);
			this.labelDashboardRefreshInterval.AutoSize = true;
			this.labelDashboardRefreshInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelDashboardRefreshInterval.Location = new Point(21, 412);
			this.labelDashboardRefreshInterval.Name = "labelDashboardRefreshInterval";
			this.labelDashboardRefreshInterval.Size = new Size(226, 17);
			this.labelDashboardRefreshInterval.TabIndex = 18;
			this.labelDashboardRefreshInterval.Text = "Dashboard Refresh Time Interval:";
			this.comboBoxDashboardRefreshInterval.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxDashboardRefreshInterval.FlatStyle = FlatStyle.Flat;
			this.comboBoxDashboardRefreshInterval.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxDashboardRefreshInterval.FormattingEnabled = true;
			this.comboBoxDashboardRefreshInterval.Location = new Point(275, 408);
			this.comboBoxDashboardRefreshInterval.Name = "comboBoxDashboardRefreshInterval";
			this.comboBoxDashboardRefreshInterval.Size = new Size(167, 25);
			this.comboBoxDashboardRefreshInterval.TabIndex = 17;
			this.checkBoxEnableDashboard.AutoSize = true;
			this.checkBoxEnableDashboard.BackColor = Color.Transparent;
			this.checkBoxEnableDashboard.Checked = true;
			this.checkBoxEnableDashboard.CheckState = CheckState.Checked;
			this.checkBoxEnableDashboard.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxEnableDashboard.Location = new Point(26, 376);
			this.checkBoxEnableDashboard.Name = "checkBoxEnableDashboard";
			this.checkBoxEnableDashboard.Size = new Size(187, 21);
			this.checkBoxEnableDashboard.TabIndex = 21;
			this.checkBoxEnableDashboard.Text = "Enable Dashboard Mode";
			this.checkBoxEnableDashboard.UseVisualStyleBackColor = false;
			this.labelLastMessage.AutoSize = true;
			this.labelLastMessage.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelLastMessage.Location = new Point(22, 121);
			this.labelLastMessage.Name = "labelLastMessage";
			this.labelLastMessage.Size = new Size(184, 17);
			this.labelLastMessage.TabIndex = 26;
			this.labelLastMessage.Text = "Last Message Status Limit:";
			this.comboBoxLastMessageStatus.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxLastMessageStatus.FlatStyle = FlatStyle.Flat;
			this.comboBoxLastMessageStatus.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxLastMessageStatus.FormattingEnabled = true;
			this.comboBoxLastMessageStatus.Location = new Point(278, 118);
			this.comboBoxLastMessageStatus.Name = "comboBoxLastMessageStatus";
			this.comboBoxLastMessageStatus.Size = new Size(164, 25);
			this.comboBoxLastMessageStatus.TabIndex = 25;
			this.buttonHelp.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonHelp.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonHelp.Location = new Point(278, 519);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new Size(74, 27);
			this.buttonHelp.TabIndex = 29;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.UseVisualStyleBackColor = true;
			this.buttonHelp.Click += new EventHandler(this.buttonHelp_Click);
			this.labelNotificationSound.AutoSize = true;
			this.labelNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelNotificationSound.Location = new Point(23, 84);
			this.labelNotificationSound.Name = "labelNotificationSound";
			this.labelNotificationSound.Size = new Size(192, 17);
			this.labelNotificationSound.TabIndex = 30;
			this.labelNotificationSound.Text = "Message Notification Sound:";
			this.comboBoxNotificationSound.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxNotificationSound.FlatStyle = FlatStyle.Flat;
			this.comboBoxNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxNotificationSound.FormattingEnabled = true;
			this.comboBoxNotificationSound.Location = new Point(278, 81);
			this.comboBoxNotificationSound.Name = "comboBoxNotificationSound";
			this.comboBoxNotificationSound.Size = new Size(164, 25);
			this.comboBoxNotificationSound.TabIndex = 31;
			this.comboBoxNotificationSound.SelectedIndexChanged += new EventHandler(this.comboBoxNotificationSound_SelectedIndexChanged);
			this.labelUnreadMessageLimit.AutoSize = true;
			this.labelUnreadMessageLimit.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelUnreadMessageLimit.Location = new Point(22, 160);
			this.labelUnreadMessageLimit.Name = "labelUnreadMessageLimit";
			this.labelUnreadMessageLimit.Size = new Size(203, 17);
			this.labelUnreadMessageLimit.TabIndex = 33;
			this.labelUnreadMessageLimit.Text = "Unread Message Check Limit:";
			this.comboBoxUnreadMessageLimit.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxUnreadMessageLimit.FlatStyle = FlatStyle.Flat;
			this.comboBoxUnreadMessageLimit.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxUnreadMessageLimit.FormattingEnabled = true;
			this.comboBoxUnreadMessageLimit.Location = new Point(278, 157);
			this.comboBoxUnreadMessageLimit.Name = "comboBoxUnreadMessageLimit";
			this.comboBoxUnreadMessageLimit.Size = new Size(164, 25);
			this.comboBoxUnreadMessageLimit.TabIndex = 32;
			this.comboBoxDashboardNotificationSound.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxDashboardNotificationSound.FlatStyle = FlatStyle.Flat;
			this.comboBoxDashboardNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxDashboardNotificationSound.FormattingEnabled = true;
			this.comboBoxDashboardNotificationSound.Location = new Point(275, 445);
			this.comboBoxDashboardNotificationSound.Name = "comboBoxDashboardNotificationSound";
			this.comboBoxDashboardNotificationSound.Size = new Size(167, 25);
			this.comboBoxDashboardNotificationSound.TabIndex = 35;
			this.comboBoxDashboardNotificationSound.SelectedIndexChanged += new EventHandler(this.comboBoxDashboardNotificationSound_SelectedIndexChanged);
			this.labelDashboardNotificationSound.AutoSize = true;
			this.labelDashboardNotificationSound.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelDashboardNotificationSound.Location = new Point(22, 448);
			this.labelDashboardNotificationSound.Name = "labelDashboardNotificationSound";
			this.labelDashboardNotificationSound.Size = new Size(205, 17);
			this.labelDashboardNotificationSound.TabIndex = 34;
			this.labelDashboardNotificationSound.Text = "Dashboard Notification Sound:";
			this.labelMachineID.AutoSize = true;
			this.labelMachineID.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelMachineID.ForeColor = Color.FromArgb(64, 64, 64);
			this.labelMachineID.Location = new Point(23, 12);
			this.labelMachineID.Name = "labelMachineID";
			this.labelMachineID.Size = new Size(80, 17);
			this.labelMachineID.TabIndex = 36;
			this.labelMachineID.Text = "MachineID:";
			this.checkBoxDisableDashboardSettingChangeNotifications.AutoSize = true;
			this.checkBoxDisableDashboardSettingChangeNotifications.BackColor = Color.Transparent;
			this.checkBoxDisableDashboardSettingChangeNotifications.Checked = true;
			this.checkBoxDisableDashboardSettingChangeNotifications.CheckState = CheckState.Checked;
			this.checkBoxDisableDashboardSettingChangeNotifications.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxDisableDashboardSettingChangeNotifications.Location = new Point(25, 487);
			this.checkBoxDisableDashboardSettingChangeNotifications.Name = "checkBoxDisableDashboardSettingChangeNotifications";
			this.checkBoxDisableDashboardSettingChangeNotifications.Size = new Size(388, 21);
			this.checkBoxDisableDashboardSettingChangeNotifications.TabIndex = 37;
			this.checkBoxDisableDashboardSettingChangeNotifications.Text = "Disable Setting Change Notifications Between Accounts";
			this.checkBoxDisableDashboardSettingChangeNotifications.UseVisualStyleBackColor = false;
			this.labelFontSize.AutoSize = true;
			this.labelFontSize.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelFontSize.Location = new Point(21, 199);
			this.labelFontSize.Name = "labelFontSize";
			this.labelFontSize.Size = new Size(163, 17);
			this.labelFontSize.TabIndex = 39;
			this.labelFontSize.Text = "Text Message Font Size";
			this.comboBoxFontSize.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFontSize.FlatStyle = FlatStyle.Flat;
			this.comboBoxFontSize.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxFontSize.FormattingEnabled = true;
			this.comboBoxFontSize.Location = new Point(277, 196);
			this.comboBoxFontSize.Name = "comboBoxFontSize";
			this.comboBoxFontSize.Size = new Size(164, 25);
			this.comboBoxFontSize.TabIndex = 38;
			this.labelFontSizeDT.AutoSize = true;
			this.labelFontSizeDT.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.labelFontSizeDT.Location = new Point(21, 238);
			this.labelFontSizeDT.Name = "labelFontSizeDT";
			this.labelFontSizeDT.Size = new Size(171, 17);
			this.labelFontSizeDT.TabIndex = 41;
			this.labelFontSizeDT.Text = "Text Date/Time Font Size";
			this.comboBoxFontSizeDT.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFontSizeDT.FlatStyle = FlatStyle.Flat;
			this.comboBoxFontSizeDT.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.comboBoxFontSizeDT.FormattingEnabled = true;
			this.comboBoxFontSizeDT.Location = new Point(277, 235);
			this.comboBoxFontSizeDT.Name = "comboBoxFontSizeDT";
			this.comboBoxFontSizeDT.Size = new Size(164, 25);
			this.comboBoxFontSizeDT.TabIndex = 40;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(459, 561);
			base.Controls.Add(this.labelFontSizeDT);
			base.Controls.Add(this.comboBoxFontSizeDT);
			base.Controls.Add(this.labelFontSize);
			base.Controls.Add(this.comboBoxFontSize);
			base.Controls.Add(this.checkBoxDisableDashboardSettingChangeNotifications);
			base.Controls.Add(this.labelMachineID);
			base.Controls.Add(this.comboBoxDashboardNotificationSound);
			base.Controls.Add(this.labelDashboardNotificationSound);
			base.Controls.Add(this.labelUnreadMessageLimit);
			base.Controls.Add(this.comboBoxUnreadMessageLimit);
			base.Controls.Add(this.comboBoxNotificationSound);
			base.Controls.Add(this.labelNotificationSound);
			base.Controls.Add(this.buttonHelp);
			base.Controls.Add(this.labelLastMessage);
			base.Controls.Add(this.comboBoxLastMessageStatus);
			base.Controls.Add(this.checkBoxEnableDashboard);
			base.Controls.Add(this.labelDashboardRefreshInterval);
			base.Controls.Add(this.comboBoxDashboardRefreshInterval);
			base.Controls.Add(this.checkBoxEnableSignature);
			base.Controls.Add(this.textBoxSignature);
			base.Controls.Add(this.labelReminderInterval);
			base.Controls.Add(this.comboBoxNotificationReminder);
			base.Controls.Add(this.buttonSave);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			this.MaximumSize = new Size(475, 600);
			base.MinimizeBox = false;
			this.MinimumSize = new Size(455, 500);
			base.Name = "fmSettings";
			this.Text = "Settings";
			base.Load += new EventHandler(this.fmSettings_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

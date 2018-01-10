using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TaskBarApp.Objects;

namespace TaskBarApp
{
	public class fmGroupSchedule : Form
	{
		private string strError = string.Empty;

		private bool bRefreshMessageForm;

		private IContainer components;

		private CheckBox checkBoxEnableGroupSchedule;

		private Button buttonSave;

		private Button buttonRefresh;

		private Button buttonOpenFile;

		private Label labelProcessing;

		private TabPage tabPage1;

		private DataGridView dataGridViewGroupScheduleFile;

		private DataGridViewTextBoxColumn ColumnAddress;

		private DataGridViewTextBoxColumn ColumnSendDate;

		private DataGridViewTextBoxColumn ColumnTextMessage;

		private DataGridViewTextBoxColumn ColumnSendStatus;

		private TabControl tabControlSchedule;

		private Button buttonAddNew;

		private Button buttonDelete;

		private Button buttonClearFile;

		public ApplicationManager appManager
		{
			get;
			set;
		}

		public fmGroupSchedule()
		{
			this.InitializeComponent();
		}

		private void GroupSchedule_Load(object sender, EventArgs e)
		{
			try
			{
				this.Text = this.appManager.m_strApplicationName + " Group Schedule";
				base.Icon = this.appManager.iTextApp;
				int num = 0;
				int num2 = 0;
				RegistryKey expr_3B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.GetValue(expr_3B, "local_FormGroupScheduleWidth", ref num, ref this.strError);
				AppRegistry.GetValue(expr_3B, "local_FormGroupScheduleHeight", ref num2, ref this.strError);
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
				this.dataGridViewGroupScheduleFile.DefaultCellStyle.SelectionForeColor = Color.Black;
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
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("Exception creating Group Schedule form: " + ex.Message, 5);
			}
		}

		private void fmGroupSchedule_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				RegistryKey expr_0B = AppRegistry.GetRootKey(ref this.strError);
				AppRegistry.SaveValue(expr_0B, "local_FormGroupScheduleWidth", base.Width, ref this.strError, false, RegistryValueKind.Unknown);
				AppRegistry.SaveValue(expr_0B, "local_FormGroupScheduleHeight", base.Height, ref this.strError, false, RegistryValueKind.Unknown);
			}
			catch
			{
			}
		}

		public bool dataGridViewGroupScheduleFile_Load()
		{
			bool result = true;
			try
			{
				this.dataGridViewGroupScheduleFile.Rows.Clear();
				foreach (ScheduleFileItem current in this.appManager.m_lsGroupSchedule)
				{
					string arg_33_0 = current.SendDate;
					DateTime dateTime;
					DateTime.TryParse(current.SendDate, out dateTime);
					string text = current.Address;
					if (!text.Contains("#"))
					{
						text = this.appManager.FormatPhone(current.Address);
					}
					this.dataGridViewGroupScheduleFile.Rows.Add(new object[]
					{
						text,
						dateTime,
						current.TextMessage,
						current.SendStatus
					});
				}
				this.dataGridViewGroupScheduleFile.Sort(this.dataGridViewGroupScheduleFile.Columns[1], ListSortDirection.Ascending);
				if (this.dataGridViewGroupScheduleFile.RowCount > 0)
				{
					this.dataGridViewGroupScheduleFile.CurrentCell = this.dataGridViewGroupScheduleFile.Rows[0].Cells[0];
				}
			}
			catch (Exception ex)
			{
				this.appManager.ShowBalloon("dataGridViewGroupScheduleFile_Load error: " + ex.Message, 5);
				result = false;
			}
			return result;
		}

		private bool dataGridViewGroupScheduleFile_Save()
		{
			bool result = true;
			try
			{
				List<ScheduleFileItem> list = new List<ScheduleFileItem>();
				foreach (DataGridViewRow dataGridViewRow in ((IEnumerable)this.dataGridViewGroupScheduleFile.Rows))
				{
					if (dataGridViewRow.Cells[0].Value == null)
					{
						break;
					}
					string address = dataGridViewRow.Cells[0].Value.ToString();
					string sendDate = dataGridViewRow.Cells[1].Value.ToString();
					string textMessage = dataGridViewRow.Cells[2].Value.ToString();
					string sendStatus = "";
					if (dataGridViewRow.Cells[3].Value != null)
					{
						sendStatus = dataGridViewRow.Cells[3].Value.ToString();
					}
					ScheduleFileItem scheduleFileItem = new ScheduleFileItem();
					scheduleFileItem.Address = address;
					scheduleFileItem.SendDate = sendDate;
					scheduleFileItem.TextMessage = textMessage;
					scheduleFileItem.SendStatus = sendStatus;
					scheduleFileItem = this.appManager.GroupScheduleFileValidateItem(scheduleFileItem);
					list.Add(scheduleFileItem);
				}
				this.appManager.GroupSchedulFileSave(list);
				this.dataGridViewGroupScheduleFile_Load();
			}
			catch (Exception ex)
			{
				this.strError = "dataGridViewGroupScheduleFile_Save error: " + ex.Message;
				result = false;
			}
			return result;
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			if (this.dataGridViewGroupScheduleFile_Save())
			{
				this.appManager.ShowBalloon("The file has been saved, please check the status column for details on validation.", 5);
				if (this.appManager.formMessages != null && this.bRefreshMessageForm)
				{
					this.appManager.formMessages.Close();
					this.appManager.ShowMessages();
				}
			}
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			this.appManager.GroupSchedulFileLoad();
			this.dataGridViewGroupScheduleFile_Load();
		}

		private void buttonOpenFile_Click(object sender, EventArgs e)
		{
			this.strError = string.Empty;
			string empty = string.Empty;
			if (MessageBox.Show("Group Schedule Processing must be disabled to open the raw schedule file to avoid file lock conflicts.\r\n\r\nWhile working with the file you MUST keep the header row unchanged or the file will be rejected.  You MUST not populate the SendStatus column or the record will not be processed.\r\n\r\nOnce additions to the file are complete, open the Edit Group Schedule screen re-enable the process and click on Validate & Save button.\r\n\r\nDo you want to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				try
				{
					AppRegistry.SaveValue(AppRegistry.GetRootKey(ref empty), "EnableGroupScheduleProcessing", false, ref empty, false, RegistryValueKind.Unknown);
					if (empty != string.Empty)
					{
						this.strError = this.strError + "Disable Group Schedule Processing save error: " + empty;
					}
					else
					{
						this.appManager.m_bEnableGroupScheduleProcessing = false;
					}
				}
				catch (Exception ex)
				{
					this.strError = this.strError + "Disable Group Schedule Processing save error: " + ex.Message;
				}
				if (this.strError.Length == 0)
				{
					base.Close();
					Process.Start(this.appManager.m_strCSVScheduleFilePath);
					return;
				}
				this.appManager.ShowBalloon("Exception opening raw file: " + this.strError, 5);
			}
		}

		private void buttonAddNew_Click(object sender, EventArgs e)
		{
			this.appManager.ShowEditGroupSchedule();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (this.dataGridViewGroupScheduleFile.SelectedRows.Count > 0 && MessageBox.Show("Delete the selected row?\n\n Please Note this cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				this.dataGridViewGroupScheduleFile.Rows.RemoveAt(this.dataGridViewGroupScheduleFile.SelectedRows[0].Index);
				this.dataGridViewGroupScheduleFile_Save();
				this.appManager.GroupSchedulFileLoad();
				this.dataGridViewGroupScheduleFile_Load();
			}
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

		private void checkBoxEnableGroupSchedule_CheckedChanged(object sender, EventArgs e)
		{
			this.strError = string.Empty;
			string empty = string.Empty;
			if (this.appManager.m_bEnableGroupScheduleProcessing != this.checkBoxEnableGroupSchedule.Checked)
			{
				try
				{
					AppRegistry.SaveValue(AppRegistry.GetRootKey(ref empty), "EnableGroupScheduleProcessing", this.checkBoxEnableGroupSchedule.Checked, ref empty, false, RegistryValueKind.Unknown);
					if (empty != string.Empty)
					{
						this.strError += empty;
					}
					else
					{
						this.appManager.m_bEnableGroupScheduleProcessing = this.checkBoxEnableGroupSchedule.Checked;
						this.bRefreshMessageForm = true;
					}
				}
				catch (Exception ex)
				{
					this.strError += ex.Message;
				}
				if (this.strError.Length == 0)
				{
					this.appManager.ShowBalloon("Setting saved.", 5);
					return;
				}
				this.appManager.ShowBalloon("checkBoxEnableGroupSchedule_CheckedChanged error: " + this.strError, 5);
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
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(fmGroupSchedule));
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
			((ISupportInitialize)this.dataGridViewGroupScheduleFile).BeginInit();
			this.tabControlSchedule.SuspendLayout();
			base.SuspendLayout();
			this.checkBoxEnableGroupSchedule.AutoSize = true;
			this.checkBoxEnableGroupSchedule.BackColor = Color.Transparent;
			this.checkBoxEnableGroupSchedule.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.checkBoxEnableGroupSchedule.Location = new Point(12, 12);
			this.checkBoxEnableGroupSchedule.Name = "checkBoxEnableGroupSchedule";
			this.checkBoxEnableGroupSchedule.Size = new Size(259, 21);
			this.checkBoxEnableGroupSchedule.TabIndex = 38;
			this.checkBoxEnableGroupSchedule.Text = "Enable Group Schedule Processing";
			this.checkBoxEnableGroupSchedule.UseVisualStyleBackColor = false;
			this.checkBoxEnableGroupSchedule.CheckedChanged += new EventHandler(this.checkBoxEnableGroupSchedule_CheckedChanged);
			this.buttonSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonSave.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonSave.Location = new Point(360, 363);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(162, 27);
			this.buttonSave.TabIndex = 39;
			this.buttonSave.Text = "Validate && Save File";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.buttonRefresh.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonRefresh.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonRefresh.Location = new Point(152, 363);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new Size(102, 27);
			this.buttonRefresh.TabIndex = 40;
			this.buttonRefresh.Text = "Reload File";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
			this.buttonOpenFile.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonOpenFile.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonOpenFile.Location = new Point(12, 363);
			this.buttonOpenFile.Name = "buttonOpenFile";
			this.buttonOpenFile.Size = new Size(134, 27);
			this.buttonOpenFile.TabIndex = 41;
			this.buttonOpenFile.Text = "Access Raw File";
			this.buttonOpenFile.UseVisualStyleBackColor = true;
			this.buttonOpenFile.Click += new EventHandler(this.buttonOpenFile_Click);
			this.labelProcessing.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.labelProcessing.AutoSize = true;
			this.labelProcessing.Location = new Point(550, 20);
			this.labelProcessing.Name = "labelProcessing";
			this.labelProcessing.Size = new Size(202, 13);
			this.labelProcessing.TabIndex = 42;
			this.labelProcessing.Text = "Group Schedule is Currently Processing...";
			this.labelProcessing.TextAlign = ContentAlignment.MiddleRight;
			this.tabPage1.Controls.Add(this.dataGridViewGroupScheduleFile);
			this.tabPage1.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.tabPage1.Location = new Point(4, 26);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new Padding(3);
			this.tabPage1.Size = new Size(732, 279);
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
			this.dataGridViewGroupScheduleFile.Columns.AddRange(new DataGridViewColumn[]
			{
				this.ColumnAddress,
				this.ColumnSendDate,
				this.ColumnTextMessage,
				this.ColumnSendStatus
			});
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.True;
			this.dataGridViewGroupScheduleFile.DefaultCellStyle = dataGridViewCellStyle;
			this.dataGridViewGroupScheduleFile.Dock = DockStyle.Fill;
			this.dataGridViewGroupScheduleFile.Location = new Point(3, 3);
			this.dataGridViewGroupScheduleFile.MultiSelect = false;
			this.dataGridViewGroupScheduleFile.Name = "dataGridViewGroupScheduleFile";
			this.dataGridViewGroupScheduleFile.ReadOnly = true;
			this.dataGridViewGroupScheduleFile.RowTemplate.Height = 40;
			this.dataGridViewGroupScheduleFile.ScrollBars = ScrollBars.Vertical;
			this.dataGridViewGroupScheduleFile.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dataGridViewGroupScheduleFile.Size = new Size(726, 273);
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
			this.tabControlSchedule.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tabControlSchedule.Controls.Add(this.tabPage1);
			this.tabControlSchedule.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.tabControlSchedule.Location = new Point(12, 48);
			this.tabControlSchedule.Name = "tabControlSchedule";
			this.tabControlSchedule.SelectedIndex = 0;
			this.tabControlSchedule.Size = new Size(740, 309);
			this.tabControlSchedule.TabIndex = 43;
			this.buttonAddNew.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonAddNew.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonAddNew.Location = new Point(665, 363);
			this.buttonAddNew.Name = "buttonAddNew";
			this.buttonAddNew.Size = new Size(83, 27);
			this.buttonAddNew.TabIndex = 44;
			this.buttonAddNew.Text = "Add New";
			this.buttonAddNew.UseVisualStyleBackColor = true;
			this.buttonAddNew.Click += new EventHandler(this.buttonAddNew_Click);
			this.buttonDelete.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonDelete.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonDelete.Location = new Point(530, 363);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new Size(129, 27);
			this.buttonDelete.TabIndex = 45;
			this.buttonDelete.Text = "Delete Selected";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);
			this.buttonClearFile.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.buttonClearFile.Font = new Font("Arial", 11.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.buttonClearFile.Location = new Point(260, 363);
			this.buttonClearFile.Name = "buttonClearFile";
			this.buttonClearFile.Size = new Size(94, 27);
			this.buttonClearFile.TabIndex = 46;
			this.buttonClearFile.Text = "Clear File";
			this.buttonClearFile.UseVisualStyleBackColor = true;
			this.buttonClearFile.Click += new EventHandler(this.buttonClearFile_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(764, 402);
			base.Controls.Add(this.buttonClearFile);
			base.Controls.Add(this.buttonDelete);
			base.Controls.Add(this.buttonAddNew);
			base.Controls.Add(this.tabControlSchedule);
			base.Controls.Add(this.labelProcessing);
			base.Controls.Add(this.buttonOpenFile);
			base.Controls.Add(this.buttonRefresh);
			base.Controls.Add(this.buttonSave);
			base.Controls.Add(this.checkBoxEnableGroupSchedule);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			this.MinimumSize = new Size(780, 440);
			base.Name = "fmGroupSchedule";
			this.Text = "Group Schedule";
			base.FormClosing += new FormClosingEventHandler(this.fmGroupSchedule_FormClosing);
			base.Load += new EventHandler(this.GroupSchedule_Load);
			this.tabPage1.ResumeLayout(false);
			((ISupportInitialize)this.dataGridViewGroupScheduleFile).EndInit();
			this.tabControlSchedule.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

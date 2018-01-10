namespace PPAPIStreamLined
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboErrors = new System.Windows.Forms.ComboBox();
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.txtCurLog = new System.Windows.Forms.TextBox();
            this.lblLocIDLog = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.cboCodeErrors = new System.Windows.Forms.ComboBox();
            this.lblErrors = new System.Windows.Forms.Label();
            this.lblGhosts = new System.Windows.Forms.Label();
            this.cboGhosts = new System.Windows.Forms.ComboBox();
            this.lblFailed = new System.Windows.Forms.Label();
            this.cboFailed = new System.Windows.Forms.ComboBox();
            this.lblProcess = new System.Windows.Forms.Label();
            this.btnNotes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboErrors
            // 
            this.cboErrors.FormattingEnabled = true;
            this.cboErrors.Location = new System.Drawing.Point(12, 26);
            this.cboErrors.Name = "cboErrors";
            this.cboErrors.Size = new System.Drawing.Size(121, 21);
            this.cboErrors.TabIndex = 0;
            this.cboErrors.SelectedIndexChanged += new System.EventHandler(this.cboErrors_SelectedIndexChanged);
            // 
            // prgBar
            // 
            this.prgBar.Location = new System.Drawing.Point(13, 385);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(432, 23);
            this.prgBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.prgBar.TabIndex = 1;
            // 
            // txtCurLog
            // 
            this.txtCurLog.AcceptsReturn = true;
            this.txtCurLog.AcceptsTab = true;
            this.txtCurLog.Location = new System.Drawing.Point(149, 12);
            this.txtCurLog.Multiline = true;
            this.txtCurLog.Name = "txtCurLog";
            this.txtCurLog.ReadOnly = true;
            this.txtCurLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCurLog.Size = new System.Drawing.Size(296, 367);
            this.txtCurLog.TabIndex = 3;
            // 
            // lblLocIDLog
            // 
            this.lblLocIDLog.Location = new System.Drawing.Point(12, 9);
            this.lblLocIDLog.Name = "lblLocIDLog";
            this.lblLocIDLog.Size = new System.Drawing.Size(121, 14);
            this.lblLocIDLog.TabIndex = 4;
            this.lblLocIDLog.Text = "Customers Not Found";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(12, 356);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(131, 23);
            this.btnUpload.TabIndex = 7;
            this.btnUpload.Text = "Upload Docs";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // cboCodeErrors
            // 
            this.cboCodeErrors.FormattingEnabled = true;
            this.cboCodeErrors.Location = new System.Drawing.Point(12, 113);
            this.cboCodeErrors.Name = "cboCodeErrors";
            this.cboCodeErrors.Size = new System.Drawing.Size(121, 21);
            this.cboCodeErrors.TabIndex = 8;
            this.cboCodeErrors.SelectedIndexChanged += new System.EventHandler(this.cboCodeErrors_SelectedIndexChanged);
            // 
            // lblErrors
            // 
            this.lblErrors.AutoSize = true;
            this.lblErrors.Location = new System.Drawing.Point(12, 97);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(62, 13);
            this.lblErrors.TabIndex = 9;
            this.lblErrors.Text = "Code Errors";
            // 
            // lblGhosts
            // 
            this.lblGhosts.AutoSize = true;
            this.lblGhosts.Location = new System.Drawing.Point(12, 194);
            this.lblGhosts.Name = "lblGhosts";
            this.lblGhosts.Size = new System.Drawing.Size(97, 13);
            this.lblGhosts.TabIndex = 10;
            this.lblGhosts.Text = "Missing Customers:";
            // 
            // cboGhosts
            // 
            this.cboGhosts.FormattingEnabled = true;
            this.cboGhosts.Location = new System.Drawing.Point(12, 210);
            this.cboGhosts.Name = "cboGhosts";
            this.cboGhosts.Size = new System.Drawing.Size(121, 21);
            this.cboGhosts.TabIndex = 11;
            this.cboGhosts.SelectedIndexChanged += new System.EventHandler(this.cboGhosts_SelectedIndexChanged);
            // 
            // lblFailed
            // 
            this.lblFailed.AutoSize = true;
            this.lblFailed.Location = new System.Drawing.Point(12, 281);
            this.lblFailed.Name = "lblFailed";
            this.lblFailed.Size = new System.Drawing.Size(83, 13);
            this.lblFailed.TabIndex = 12;
            this.lblFailed.Text = "Failed Searches";
            // 
            // cboFailed
            // 
            this.cboFailed.FormattingEnabled = true;
            this.cboFailed.Location = new System.Drawing.Point(12, 297);
            this.cboFailed.Name = "cboFailed";
            this.cboFailed.Size = new System.Drawing.Size(121, 21);
            this.cboFailed.TabIndex = 13;
            this.cboFailed.SelectedIndexChanged += new System.EventHandler(this.cboFailed_SelectedIndexChanged);
            // 
            // lblProcess
            // 
            this.lblProcess.Location = new System.Drawing.Point(12, 421);
            this.lblProcess.Name = "lblProcess";
            this.lblProcess.Size = new System.Drawing.Size(433, 119);
            this.lblProcess.TabIndex = 14;
            this.lblProcess.Click += new System.EventHandler(this.lblProcess_Click);
            // 
            // btnNotes
            // 
            this.btnNotes.Location = new System.Drawing.Point(12, 327);
            this.btnNotes.Name = "btnNotes";
            this.btnNotes.Size = new System.Drawing.Size(131, 23);
            this.btnNotes.TabIndex = 15;
            this.btnNotes.Text = "Upload Notes";
            this.btnNotes.UseVisualStyleBackColor = true;
            this.btnNotes.Click += new System.EventHandler(this.btnNotes_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 549);
            this.Controls.Add(this.btnNotes);
            this.Controls.Add(this.lblProcess);
            this.Controls.Add(this.cboFailed);
            this.Controls.Add(this.lblFailed);
            this.Controls.Add(this.cboGhosts);
            this.Controls.Add(this.lblGhosts);
            this.Controls.Add(this.lblErrors);
            this.Controls.Add(this.cboCodeErrors);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.lblLocIDLog);
            this.Controls.Add(this.txtCurLog);
            this.Controls.Add(this.prgBar);
            this.Controls.Add(this.cboErrors);
            this.Name = "Form1";
            this.Text = "Document Uploads";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboErrors;
        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.TextBox txtCurLog;
        private System.Windows.Forms.Label lblLocIDLog;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.ComboBox cboCodeErrors;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.Label lblGhosts;
        private System.Windows.Forms.ComboBox cboGhosts;
        private System.Windows.Forms.Label lblFailed;
        private System.Windows.Forms.ComboBox cboFailed;
        private System.Windows.Forms.Label lblProcess;
        private System.Windows.Forms.Button btnNotes;
    }
}


namespace PestPac_Utility
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
            this.prgBar = new System.Windows.Forms.ProgressBar();
            this.lstConsole = new System.Windows.Forms.ListBox();
            this.btnSurveys = new System.Windows.Forms.Button();
            this.grpType = new System.Windows.Forms.GroupBox();
            this.radINPC = new System.Windows.Forms.RadioButton();
            this.radQTPC = new System.Windows.Forms.RadioButton();
            this.radRetention = new System.Windows.Forms.RadioButton();
            this.grpType.SuspendLayout();
            this.SuspendLayout();
            // 
            // prgBar
            // 
            this.prgBar.Location = new System.Drawing.Point(200, 408);
            this.prgBar.Name = "prgBar";
            this.prgBar.Size = new System.Drawing.Size(354, 23);
            this.prgBar.TabIndex = 0;
            // 
            // lstConsole
            // 
            this.lstConsole.FormattingEnabled = true;
            this.lstConsole.HorizontalScrollbar = true;
            this.lstConsole.Location = new System.Drawing.Point(200, 6);
            this.lstConsole.Name = "lstConsole";
            this.lstConsole.Size = new System.Drawing.Size(354, 394);
            this.lstConsole.TabIndex = 1;
            // 
            // btnSurveys
            // 
            this.btnSurveys.Location = new System.Drawing.Point(12, 12);
            this.btnSurveys.Name = "btnSurveys";
            this.btnSurveys.Size = new System.Drawing.Size(182, 23);
            this.btnSurveys.TabIndex = 2;
            this.btnSurveys.Text = "Upload Surveys";
            this.btnSurveys.UseVisualStyleBackColor = true;
            this.btnSurveys.Click += new System.EventHandler(this.btnSurveys_Click);
            // 
            // grpType
            // 
            this.grpType.Controls.Add(this.radRetention);
            this.grpType.Controls.Add(this.radQTPC);
            this.grpType.Controls.Add(this.radINPC);
            this.grpType.Location = new System.Drawing.Point(12, 55);
            this.grpType.Name = "grpType";
            this.grpType.Size = new System.Drawing.Size(124, 90);
            this.grpType.TabIndex = 3;
            this.grpType.TabStop = false;
            this.grpType.Text = "Survey Type";
            // 
            // radINPC
            // 
            this.radINPC.AutoSize = true;
            this.radINPC.Location = new System.Drawing.Point(6, 19);
            this.radINPC.Name = "radINPC";
            this.radINPC.Size = new System.Drawing.Size(50, 17);
            this.radINPC.TabIndex = 0;
            this.radINPC.TabStop = true;
            this.radINPC.Text = "INPC";
            this.radINPC.UseVisualStyleBackColor = true;
            // 
            // radQTPC
            // 
            this.radQTPC.AutoSize = true;
            this.radQTPC.Checked = true;
            this.radQTPC.Location = new System.Drawing.Point(6, 43);
            this.radQTPC.Name = "radQTPC";
            this.radQTPC.Size = new System.Drawing.Size(54, 17);
            this.radQTPC.TabIndex = 1;
            this.radQTPC.TabStop = true;
            this.radQTPC.Text = "QTPC";
            this.radQTPC.UseVisualStyleBackColor = true;
            // 
            // radRetention
            // 
            this.radRetention.AutoSize = true;
            this.radRetention.Location = new System.Drawing.Point(7, 67);
            this.radRetention.Name = "radRetention";
            this.radRetention.Size = new System.Drawing.Size(71, 17);
            this.radRetention.TabIndex = 2;
            this.radRetention.TabStop = true;
            this.radRetention.Text = "Retention";
            this.radRetention.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 443);
            this.Controls.Add(this.grpType);
            this.Controls.Add(this.btnSurveys);
            this.Controls.Add(this.lstConsole);
            this.Controls.Add(this.prgBar);
            this.Name = "Form1";
            this.Text = "PestPac Utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.grpType.ResumeLayout(false);
            this.grpType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgBar;
        private System.Windows.Forms.ListBox lstConsole;
        private System.Windows.Forms.Button btnSurveys;
        private System.Windows.Forms.GroupBox grpType;
        private System.Windows.Forms.RadioButton radRetention;
        private System.Windows.Forms.RadioButton radQTPC;
        private System.Windows.Forms.RadioButton radINPC;
    }
}


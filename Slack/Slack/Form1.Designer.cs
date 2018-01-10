namespace Slack
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
            this.lstHist = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstHist
            // 
            this.lstHist.FormattingEnabled = true;
            this.lstHist.Location = new System.Drawing.Point(12, 12);
            this.lstHist.Name = "lstHist";
            this.lstHist.Size = new System.Drawing.Size(234, 316);
            this.lstHist.TabIndex = 0;
            this.lstHist.SelectedIndexChanged += new System.EventHandler(this.lstHist_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 335);
            this.Controls.Add(this.lstHist);
            this.Name = "Form1";
            this.Text = "Slack API";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstHist;
    }
}


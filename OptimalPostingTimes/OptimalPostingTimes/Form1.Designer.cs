﻿namespace OptimalPostingTimes
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
            this.txtBody = new System.Windows.Forms.TextBox();
            this.webPreview = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // txtBody
            // 
            this.txtBody.Location = new System.Drawing.Point(12, 8);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(420, 469);
            this.txtBody.TabIndex = 0;
            // 
            // webPreview
            // 
            this.webPreview.Location = new System.Drawing.Point(440, 8);
            this.webPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPreview.Name = "webPreview";
            this.webPreview.Size = new System.Drawing.Size(420, 469);
            this.webPreview.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 489);
            this.Controls.Add(this.webPreview);
            this.Controls.Add(this.txtBody);
            this.Name = "Form1";
            this.Text = "Optimal Posting Times";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.WebBrowser webPreview;
    }
}


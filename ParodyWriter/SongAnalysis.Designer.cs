namespace ParodyWriter
{
    partial class SongAnalysis
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
            this.lstPercentages = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSongName = new System.Windows.Forms.TextBox();
            this.btnAnalysis = new System.Windows.Forms.Button();
            this.lstAnalysis = new System.Windows.Forms.ListBox();
            this.txtSong = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lstPercentages
            // 
            this.lstPercentages.FormattingEnabled = true;
            this.lstPercentages.Location = new System.Drawing.Point(611, 13);
            this.lstPercentages.Name = "lstPercentages";
            this.lstPercentages.Size = new System.Drawing.Size(120, 589);
            this.lstPercentages.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Song Name:";
            // 
            // txtSongName
            // 
            this.txtSongName.Location = new System.Drawing.Point(93, 9);
            this.txtSongName.Name = "txtSongName";
            this.txtSongName.Size = new System.Drawing.Size(203, 20);
            this.txtSongName.TabIndex = 9;
            // 
            // btnAnalysis
            // 
            this.btnAnalysis.Location = new System.Drawing.Point(302, 7);
            this.btnAnalysis.Name = "btnAnalysis";
            this.btnAnalysis.Size = new System.Drawing.Size(177, 23);
            this.btnAnalysis.TabIndex = 8;
            this.btnAnalysis.Text = "Run Analysis";
            this.btnAnalysis.UseVisualStyleBackColor = true;
            // 
            // lstAnalysis
            // 
            this.lstAnalysis.FormattingEnabled = true;
            this.lstAnalysis.Location = new System.Drawing.Point(485, 39);
            this.lstAnalysis.Name = "lstAnalysis";
            this.lstAnalysis.Size = new System.Drawing.Size(120, 563);
            this.lstAnalysis.TabIndex = 7;
            // 
            // txtSong
            // 
            this.txtSong.Location = new System.Drawing.Point(12, 39);
            this.txtSong.Multiline = true;
            this.txtSong.Name = "txtSong";
            this.txtSong.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSong.Size = new System.Drawing.Size(467, 570);
            this.txtSong.TabIndex = 6;
            // 
            // SongAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 621);
            this.Controls.Add(this.lstPercentages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSongName);
            this.Controls.Add(this.btnAnalysis);
            this.Controls.Add(this.lstAnalysis);
            this.Controls.Add(this.txtSong);
            this.Name = "SongAnalysis";
            this.Text = "SongAnalysis";
            this.Load += new System.EventHandler(this.SongAnalysis_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstPercentages;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSongName;
        private System.Windows.Forms.Button btnAnalysis;
        private System.Windows.Forms.ListBox lstAnalysis;
        private System.Windows.Forms.TextBox txtSong;
    }
}
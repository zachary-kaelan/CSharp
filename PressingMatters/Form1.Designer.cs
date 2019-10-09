namespace PressingMatters
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pbxPicture = new System.Windows.Forms.PictureBox();
            this.lblPictureInfo = new System.Windows.Forms.Label();
            this.lstTags = new System.Windows.Forms.CheckedListBox();
            this.lblPicInfoPrompt = new System.Windows.Forms.Label();
            this.lblTagsPrompt = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitMetadata = new System.Windows.Forms.SplitContainer();
            this.lstFolders = new System.Windows.Forms.CheckedListBox();
            this.lblFoldersPrompt = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMetadata)).BeginInit();
            this.splitMetadata.Panel1.SuspendLayout();
            this.splitMetadata.Panel2.SuspendLayout();
            this.splitMetadata.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbxPicture
            // 
            this.pbxPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxPicture.Image = ((System.Drawing.Image)(resources.GetObject("pbxPicture.Image")));
            this.pbxPicture.InitialImage = ((System.Drawing.Image)(resources.GetObject("pbxPicture.InitialImage")));
            this.pbxPicture.Location = new System.Drawing.Point(3, 3);
            this.pbxPicture.Name = "pbxPicture";
            this.pbxPicture.Size = new System.Drawing.Size(509, 577);
            this.pbxPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxPicture.TabIndex = 0;
            this.pbxPicture.TabStop = false;
            this.pbxPicture.Click += new System.EventHandler(this.pbxPicture_Click);
            // 
            // lblPictureInfo
            // 
            this.lblPictureInfo.Location = new System.Drawing.Point(17, 55);
            this.lblPictureInfo.Name = "lblPictureInfo";
            this.lblPictureInfo.Size = new System.Drawing.Size(211, 217);
            this.lblPictureInfo.TabIndex = 1;
            // 
            // lstTags
            // 
            this.lstTags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lstTags.FormattingEnabled = true;
            this.lstTags.Location = new System.Drawing.Point(234, 58);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new System.Drawing.Size(218, 214);
            this.lstTags.TabIndex = 2;
            // 
            // lblPicInfoPrompt
            // 
            this.lblPicInfoPrompt.Font = new System.Drawing.Font("Modern No. 20", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPicInfoPrompt.Location = new System.Drawing.Point(13, 9);
            this.lblPicInfoPrompt.Name = "lblPicInfoPrompt";
            this.lblPicInfoPrompt.Size = new System.Drawing.Size(209, 46);
            this.lblPicInfoPrompt.TabIndex = 4;
            this.lblPicInfoPrompt.Text = "Picture Info";
            this.lblPicInfoPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTagsPrompt
            // 
            this.lblTagsPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTagsPrompt.Font = new System.Drawing.Font("Modern No. 20", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTagsPrompt.Location = new System.Drawing.Point(231, 9);
            this.lblTagsPrompt.Name = "lblTagsPrompt";
            this.lblTagsPrompt.Size = new System.Drawing.Size(218, 46);
            this.lblTagsPrompt.TabIndex = 5;
            this.lblTagsPrompt.Text = "Tags";
            this.lblTagsPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitMain
            // 
            this.splitMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitMain.Location = new System.Drawing.Point(9, 24);
            this.splitMain.Margin = new System.Windows.Forms.Padding(0);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.pbxPicture);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.splitMetadata);
            this.splitMain.Size = new System.Drawing.Size(990, 568);
            this.splitMain.SplitterDistance = 515;
            this.splitMain.TabIndex = 6;
            // 
            // splitMetadata
            // 
            this.splitMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitMetadata.Location = new System.Drawing.Point(7, 3);
            this.splitMetadata.Name = "splitMetadata";
            this.splitMetadata.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMetadata.Panel1
            // 
            this.splitMetadata.Panel1.Controls.Add(this.lblPicInfoPrompt);
            this.splitMetadata.Panel1.Controls.Add(this.lstTags);
            this.splitMetadata.Panel1.Controls.Add(this.lblTagsPrompt);
            this.splitMetadata.Panel1.Controls.Add(this.lblPictureInfo);
            // 
            // splitMetadata.Panel2
            // 
            this.splitMetadata.Panel2.Controls.Add(this.lstFolders);
            this.splitMetadata.Panel2.Controls.Add(this.lblFoldersPrompt);
            this.splitMetadata.Size = new System.Drawing.Size(461, 576);
            this.splitMetadata.SplitterDistance = 288;
            this.splitMetadata.TabIndex = 6;
            // 
            // lstFolders
            // 
            this.lstFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lstFolders.FormattingEnabled = true;
            this.lstFolders.Location = new System.Drawing.Point(238, 59);
            this.lstFolders.Name = "lstFolders";
            this.lstFolders.Size = new System.Drawing.Size(214, 214);
            this.lstFolders.TabIndex = 7;
            // 
            // lblFoldersPrompt
            // 
            this.lblFoldersPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFoldersPrompt.Font = new System.Drawing.Font("Modern No. 20", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFoldersPrompt.Location = new System.Drawing.Point(234, 9);
            this.lblFoldersPrompt.Name = "lblFoldersPrompt";
            this.lblFoldersPrompt.Size = new System.Drawing.Size(218, 46);
            this.lblFoldersPrompt.TabIndex = 6;
            this.lblFoldersPrompt.Text = "Folders";
            this.lblFoldersPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 601);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1024, 640);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.pbxPicture)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitMetadata.Panel1.ResumeLayout(false);
            this.splitMetadata.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMetadata)).EndInit();
            this.splitMetadata.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxPicture;
        private System.Windows.Forms.Label lblPictureInfo;
        private System.Windows.Forms.CheckedListBox lstTags;
        private System.Windows.Forms.Label lblPicInfoPrompt;
        private System.Windows.Forms.Label lblTagsPrompt;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.SplitContainer splitMetadata;
        private System.Windows.Forms.CheckedListBox lstFolders;
        private System.Windows.Forms.Label lblFoldersPrompt;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}


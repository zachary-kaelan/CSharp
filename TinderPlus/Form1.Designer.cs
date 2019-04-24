namespace TinderPlus
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
            this.picPhotos = new System.Windows.Forms.PictureBox();
            this.picInsta1 = new System.Windows.Forms.PictureBox();
            this.picInsta2 = new System.Windows.Forms.PictureBox();
            this.picInsta3 = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblBio = new System.Windows.Forms.Label();
            this.lblInstaInfo = new System.Windows.Forms.Label();
            this.numCurPhoto = new System.Windows.Forms.NumericUpDown();
            this.numPhotoCount = new System.Windows.Forms.NumericUpDown();
            this.lblOutOf = new System.Windows.Forms.Label();
            this.btnSaveCurrent = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnSaveInsta = new System.Windows.Forms.Button();
            this.lblControls1 = new System.Windows.Forms.Label();
            this.lblControls2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblControls3 = new System.Windows.Forms.Label();
            this.lblControls4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPhotos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPhotoCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picPhotos
            // 
            this.picPhotos.Location = new System.Drawing.Point(12, 12);
            this.picPhotos.Name = "picPhotos";
            this.picPhotos.Size = new System.Drawing.Size(640, 640);
            this.picPhotos.TabIndex = 0;
            this.picPhotos.TabStop = false;
            // 
            // picInsta1
            // 
            this.picInsta1.Location = new System.Drawing.Point(659, 418);
            this.picInsta1.Name = "picInsta1";
            this.picInsta1.Size = new System.Drawing.Size(150, 150);
            this.picInsta1.TabIndex = 3;
            this.picInsta1.TabStop = false;
            // 
            // picInsta2
            // 
            this.picInsta2.Location = new System.Drawing.Point(815, 417);
            this.picInsta2.Name = "picInsta2";
            this.picInsta2.Size = new System.Drawing.Size(150, 150);
            this.picInsta2.TabIndex = 4;
            this.picInsta2.TabStop = false;
            // 
            // picInsta3
            // 
            this.picInsta3.Location = new System.Drawing.Point(971, 417);
            this.picInsta3.Name = "picInsta3";
            this.picInsta3.Size = new System.Drawing.Size(150, 150);
            this.picInsta3.TabIndex = 5;
            this.picInsta3.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(659, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(461, 29);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "label1";
            // 
            // lblBio
            // 
            this.lblBio.Location = new System.Drawing.Point(663, 46);
            this.lblBio.Name = "lblBio";
            this.lblBio.Size = new System.Drawing.Size(458, 368);
            this.lblBio.TabIndex = 7;
            this.lblBio.Text = "label1";
            this.lblBio.Click += new System.EventHandler(this.lblBio_Click);
            // 
            // lblInstaInfo
            // 
            this.lblInstaInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstaInfo.Location = new System.Drawing.Point(659, 575);
            this.lblInstaInfo.Name = "lblInstaInfo";
            this.lblInstaInfo.Size = new System.Drawing.Size(461, 77);
            this.lblInstaInfo.TabIndex = 8;
            this.lblInstaInfo.Text = "label1";
            // 
            // numCurPhoto
            // 
            this.numCurPhoto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCurPhoto.Location = new System.Drawing.Point(253, 658);
            this.numCurPhoto.Name = "numCurPhoto";
            this.numCurPhoto.Size = new System.Drawing.Size(32, 26);
            this.numCurPhoto.TabIndex = 9;
            // 
            // numPhotoCount
            // 
            this.numPhotoCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPhotoCount.Location = new System.Drawing.Point(316, 658);
            this.numPhotoCount.Name = "numPhotoCount";
            this.numPhotoCount.ReadOnly = true;
            this.numPhotoCount.Size = new System.Drawing.Size(32, 26);
            this.numPhotoCount.TabIndex = 10;
            // 
            // lblOutOf
            // 
            this.lblOutOf.AutoSize = true;
            this.lblOutOf.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutOf.Location = new System.Drawing.Point(291, 658);
            this.lblOutOf.Name = "lblOutOf";
            this.lblOutOf.Size = new System.Drawing.Size(19, 26);
            this.lblOutOf.TabIndex = 11;
            this.lblOutOf.Text = "/";
            // 
            // btnSaveCurrent
            // 
            this.btnSaveCurrent.Location = new System.Drawing.Point(12, 658);
            this.btnSaveCurrent.Name = "btnSaveCurrent";
            this.btnSaveCurrent.Size = new System.Drawing.Size(234, 23);
            this.btnSaveCurrent.TabIndex = 12;
            this.btnSaveCurrent.Text = "Save Current";
            this.btnSaveCurrent.UseVisualStyleBackColor = true;
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(354, 658);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(455, 23);
            this.btnSaveAll.TabIndex = 13;
            this.btnSaveAll.Text = "Save All";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            // 
            // btnSaveInsta
            // 
            this.btnSaveInsta.Location = new System.Drawing.Point(815, 658);
            this.btnSaveInsta.Name = "btnSaveInsta";
            this.btnSaveInsta.Size = new System.Drawing.Size(306, 23);
            this.btnSaveInsta.TabIndex = 14;
            this.btnSaveInsta.Text = "Save Instagram Info";
            this.btnSaveInsta.UseVisualStyleBackColor = true;
            // 
            // lblControls1
            // 
            this.lblControls1.AutoSize = true;
            this.lblControls1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls1.Location = new System.Drawing.Point(8, 9);
            this.lblControls1.Name = "lblControls1";
            this.lblControls1.Size = new System.Drawing.Size(30, 100);
            this.lblControls1.TabIndex = 15;
            this.lblControls1.Text = "W:\r\nD:\r\nA:\r\nE:\r\nQ:";
            this.lblControls1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblControls2
            // 
            this.lblControls2.AutoSize = true;
            this.lblControls2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls2.Location = new System.Drawing.Point(44, 9);
            this.lblControls2.Name = "lblControls2";
            this.lblControls2.Size = new System.Drawing.Size(129, 100);
            this.lblControls2.TabIndex = 16;
            this.lblControls2.Text = "Super Like\r\nLike\r\nNope\r\nNext Photo\r\nPrevious Photo";
            this.lblControls2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 708);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblControls1);
            this.splitContainer1.Panel1.Controls.Add(this.lblControls2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblControls4);
            this.splitContainer1.Panel2.Controls.Add(this.lblControls3);
            this.splitContainer1.Size = new System.Drawing.Size(537, 118);
            this.splitContainer1.SplitterDistance = 187;
            this.splitContainer1.TabIndex = 17;
            // 
            // lblControls3
            // 
            this.lblControls3.AutoSize = true;
            this.lblControls3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls3.Location = new System.Drawing.Point(8, 9);
            this.lblControls3.Name = "lblControls3";
            this.lblControls3.Size = new System.Drawing.Size(132, 60);
            this.lblControls3.TabIndex = 0;
            this.lblControls3.Text = "Ctrl + S:\r\nCtrl + Shift + S:\r\nCtrl + G:";
            // 
            // lblControls4
            // 
            this.lblControls4.AutoSize = true;
            this.lblControls4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls4.Location = new System.Drawing.Point(158, 9);
            this.lblControls4.Name = "lblControls4";
            this.lblControls4.Size = new System.Drawing.Size(172, 60);
            this.lblControls4.TabIndex = 1;
            this.lblControls4.Text = "Save Current\r\nSave All\r\nSave Instagram Info";
            this.lblControls4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 838);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnSaveInsta);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.btnSaveCurrent);
            this.Controls.Add(this.lblOutOf);
            this.Controls.Add(this.numPhotoCount);
            this.Controls.Add(this.numCurPhoto);
            this.Controls.Add(this.lblInstaInfo);
            this.Controls.Add(this.lblBio);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.picInsta3);
            this.Controls.Add(this.picInsta2);
            this.Controls.Add(this.picInsta1);
            this.Controls.Add(this.picPhotos);
            this.Name = "Form1";
            this.Text = "Tinder Plus";
            ((System.ComponentModel.ISupportInitialize)(this.picPhotos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInsta3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPhotoCount)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picPhotos;
        private System.Windows.Forms.PictureBox picInsta1;
        private System.Windows.Forms.PictureBox picInsta2;
        private System.Windows.Forms.PictureBox picInsta3;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblBio;
        private System.Windows.Forms.Label lblInstaInfo;
        private System.Windows.Forms.NumericUpDown numCurPhoto;
        private System.Windows.Forms.NumericUpDown numPhotoCount;
        private System.Windows.Forms.Label lblOutOf;
        private System.Windows.Forms.Button btnSaveCurrent;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnSaveInsta;
        private System.Windows.Forms.Label lblControls1;
        private System.Windows.Forms.Label lblControls2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblControls4;
        private System.Windows.Forms.Label lblControls3;
    }
}


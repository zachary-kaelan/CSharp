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
            this.pbxPhotos = new System.Windows.Forms.PictureBox();
            this.pbxInsta1 = new System.Windows.Forms.PictureBox();
            this.pbxInsta2 = new System.Windows.Forms.PictureBox();
            this.pbxInsta3 = new System.Windows.Forms.PictureBox();
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
            this.lblControls4 = new System.Windows.Forms.Label();
            this.lblControls3 = new System.Windows.Forms.Label();
            this.btnSaveAllInfo = new System.Windows.Forms.Button();
            this.grpCategory = new System.Windows.Forms.GroupBox();
            this.radFun = new System.Windows.Forms.RadioButton();
            this.radUgly = new System.Windows.Forms.RadioButton();
            this.radBigNo = new System.Windows.Forms.RadioButton();
            this.radMate = new System.Windows.Forms.RadioButton();
            this.radHot = new System.Windows.Forms.RadioButton();
            this.radDull = new System.Windows.Forms.RadioButton();
            this.pbxInsta6 = new System.Windows.Forms.PictureBox();
            this.pbxInsta5 = new System.Windows.Forms.PictureBox();
            this.pbxInsta4 = new System.Windows.Forms.PictureBox();
            this.prgLoading = new System.Windows.Forms.ProgressBar();
            this.lblLoading1 = new System.Windows.Forms.Label();
            this.lblLoading2 = new System.Windows.Forms.Label();
            this.lblLoading3 = new System.Windows.Forms.Label();
            this.lblNumAutoSwiped = new System.Windows.Forms.Label();
            this.numAutoSwiped = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPhotos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurPhoto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPhotoCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpCategory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoSwiped)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxPhotos
            // 
            this.pbxPhotos.Location = new System.Drawing.Point(12, 12);
            this.pbxPhotos.Name = "pbxPhotos";
            this.pbxPhotos.Size = new System.Drawing.Size(640, 800);
            this.pbxPhotos.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxPhotos.TabIndex = 0;
            this.pbxPhotos.TabStop = false;
            // 
            // pbxInsta1
            // 
            this.pbxInsta1.Location = new System.Drawing.Point(659, 418);
            this.pbxInsta1.Name = "pbxInsta1";
            this.pbxInsta1.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta1.TabIndex = 3;
            this.pbxInsta1.TabStop = false;
            // 
            // pbxInsta2
            // 
            this.pbxInsta2.Location = new System.Drawing.Point(815, 418);
            this.pbxInsta2.Name = "pbxInsta2";
            this.pbxInsta2.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta2.TabIndex = 4;
            this.pbxInsta2.TabStop = false;
            // 
            // pbxInsta3
            // 
            this.pbxInsta3.Location = new System.Drawing.Point(971, 418);
            this.pbxInsta3.Name = "pbxInsta3";
            this.pbxInsta3.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta3.TabIndex = 5;
            this.pbxInsta3.TabStop = false;
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
            this.lblBio.Location = new System.Drawing.Point(663, 47);
            this.lblBio.Name = "lblBio";
            this.lblBio.Size = new System.Drawing.Size(458, 368);
            this.lblBio.TabIndex = 7;
            this.lblBio.Text = "label1";
            this.lblBio.Click += new System.EventHandler(this.lblBio_Click);
            // 
            // lblInstaInfo
            // 
            this.lblInstaInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstaInfo.Location = new System.Drawing.Point(662, 735);
            this.lblInstaInfo.Name = "lblInstaInfo";
            this.lblInstaInfo.Size = new System.Drawing.Size(461, 77);
            this.lblInstaInfo.TabIndex = 8;
            this.lblInstaInfo.Text = "label1";
            // 
            // numCurPhoto
            // 
            this.numCurPhoto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numCurPhoto.Location = new System.Drawing.Point(254, 818);
            this.numCurPhoto.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numCurPhoto.Name = "numCurPhoto";
            this.numCurPhoto.Size = new System.Drawing.Size(32, 26);
            this.numCurPhoto.TabIndex = 9;
            this.numCurPhoto.ValueChanged += new System.EventHandler(this.numCurPhoto_ValueChanged);
            // 
            // numPhotoCount
            // 
            this.numPhotoCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPhotoCount.Increment = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numPhotoCount.InterceptArrowKeys = false;
            this.numPhotoCount.Location = new System.Drawing.Point(317, 818);
            this.numPhotoCount.Name = "numPhotoCount";
            this.numPhotoCount.ReadOnly = true;
            this.numPhotoCount.Size = new System.Drawing.Size(32, 26);
            this.numPhotoCount.TabIndex = 10;
            // 
            // lblOutOf
            // 
            this.lblOutOf.AutoSize = true;
            this.lblOutOf.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutOf.Location = new System.Drawing.Point(292, 818);
            this.lblOutOf.Name = "lblOutOf";
            this.lblOutOf.Size = new System.Drawing.Size(19, 26);
            this.lblOutOf.TabIndex = 11;
            this.lblOutOf.Text = "/";
            // 
            // btnSaveCurrent
            // 
            this.btnSaveCurrent.Location = new System.Drawing.Point(13, 818);
            this.btnSaveCurrent.Name = "btnSaveCurrent";
            this.btnSaveCurrent.Size = new System.Drawing.Size(234, 23);
            this.btnSaveCurrent.TabIndex = 12;
            this.btnSaveCurrent.Text = "Save Current";
            this.btnSaveCurrent.UseVisualStyleBackColor = true;
            this.btnSaveCurrent.Click += new System.EventHandler(this.btnSaveCurrent_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(355, 818);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(223, 23);
            this.btnSaveAll.TabIndex = 13;
            this.btnSaveAll.Text = "Save All";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnSaveInsta
            // 
            this.btnSaveInsta.Location = new System.Drawing.Point(584, 818);
            this.btnSaveInsta.Name = "btnSaveInsta";
            this.btnSaveInsta.Size = new System.Drawing.Size(306, 23);
            this.btnSaveInsta.TabIndex = 14;
            this.btnSaveInsta.Text = "Save Instagram Info";
            this.btnSaveInsta.UseVisualStyleBackColor = true;
            this.btnSaveInsta.Click += new System.EventHandler(this.btnSaveInsta_Click);
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
            this.splitContainer1.Location = new System.Drawing.Point(13, 868);
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
            // lblControls4
            // 
            this.lblControls4.AutoSize = true;
            this.lblControls4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls4.Location = new System.Drawing.Point(158, 9);
            this.lblControls4.Name = "lblControls4";
            this.lblControls4.Size = new System.Drawing.Size(172, 80);
            this.lblControls4.TabIndex = 1;
            this.lblControls4.Text = "Save Current Photo\r\nSave All Photos\r\nSave Instagram Info\r\nSave All Info";
            this.lblControls4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblControls3
            // 
            this.lblControls3.AutoSize = true;
            this.lblControls3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblControls3.Location = new System.Drawing.Point(8, 9);
            this.lblControls3.Name = "lblControls3";
            this.lblControls3.Size = new System.Drawing.Size(134, 80);
            this.lblControls3.TabIndex = 0;
            this.lblControls3.Text = "Ctrl + S:\r\nCtrl + Shift + S:\r\nCtrl + G:\r\nCtrl + Shift + G:";
            // 
            // btnSaveAllInfo
            // 
            this.btnSaveAllInfo.Location = new System.Drawing.Point(896, 818);
            this.btnSaveAllInfo.Name = "btnSaveAllInfo";
            this.btnSaveAllInfo.Size = new System.Drawing.Size(226, 23);
            this.btnSaveAllInfo.TabIndex = 18;
            this.btnSaveAllInfo.Text = "Save All Info";
            this.btnSaveAllInfo.UseVisualStyleBackColor = true;
            this.btnSaveAllInfo.Click += new System.EventHandler(this.btnSaveAllInfo_Click);
            // 
            // grpCategory
            // 
            this.grpCategory.Controls.Add(this.radFun);
            this.grpCategory.Controls.Add(this.radUgly);
            this.grpCategory.Controls.Add(this.radBigNo);
            this.grpCategory.Controls.Add(this.radMate);
            this.grpCategory.Controls.Add(this.radHot);
            this.grpCategory.Controls.Add(this.radDull);
            this.grpCategory.Location = new System.Drawing.Point(922, 847);
            this.grpCategory.Name = "grpCategory";
            this.grpCategory.Size = new System.Drawing.Size(200, 93);
            this.grpCategory.TabIndex = 19;
            this.grpCategory.TabStop = false;
            this.grpCategory.Text = "Category";
            // 
            // radFun
            // 
            this.radFun.AutoSize = true;
            this.radFun.Location = new System.Drawing.Point(109, 65);
            this.radFun.Name = "radFun";
            this.radFun.Size = new System.Drawing.Size(80, 17);
            this.radFun.TabIndex = 5;
            this.radFun.TabStop = true;
            this.radFun.Text = "(I)nteresting";
            this.radFun.UseVisualStyleBackColor = true;
            this.radFun.CheckedChanged += new System.EventHandler(this.radFun_CheckedChanged);
            // 
            // radUgly
            // 
            this.radUgly.AutoSize = true;
            this.radUgly.Location = new System.Drawing.Point(6, 42);
            this.radUgly.Name = "radUgly";
            this.radUgly.Size = new System.Drawing.Size(52, 17);
            this.radUgly.TabIndex = 4;
            this.radUgly.TabStop = true;
            this.radUgly.Text = "(U)gly";
            this.radUgly.UseVisualStyleBackColor = true;
            this.radUgly.CheckedChanged += new System.EventHandler(this.radUgly_CheckedChanged);
            // 
            // radBigNo
            // 
            this.radBigNo.AutoSize = true;
            this.radBigNo.Location = new System.Drawing.Point(6, 19);
            this.radBigNo.Name = "radBigNo";
            this.radBigNo.Size = new System.Drawing.Size(66, 17);
            this.radBigNo.TabIndex = 0;
            this.radBigNo.TabStop = true;
            this.radBigNo.Text = "Hell (N)o";
            this.radBigNo.UseVisualStyleBackColor = true;
            this.radBigNo.CheckedChanged += new System.EventHandler(this.radBigNo_CheckedChanged);
            // 
            // radMate
            // 
            this.radMate.AutoSize = true;
            this.radMate.Location = new System.Drawing.Point(109, 19);
            this.radMate.Name = "radMate";
            this.radMate.Size = new System.Drawing.Size(85, 17);
            this.radMate.TabIndex = 3;
            this.radMate.Text = "(B) Soulmate";
            this.radMate.UseVisualStyleBackColor = true;
            this.radMate.CheckedChanged += new System.EventHandler(this.radMate_CheckedChanged);
            // 
            // radHot
            // 
            this.radHot.AutoSize = true;
            this.radHot.Location = new System.Drawing.Point(135, 42);
            this.radHot.Name = "radHot";
            this.radHot.Size = new System.Drawing.Size(59, 17);
            this.radHot.TabIndex = 2;
            this.radHot.Text = "(H)ottie";
            this.radHot.UseVisualStyleBackColor = true;
            this.radHot.CheckedChanged += new System.EventHandler(this.radHot_CheckedChanged);
            // 
            // radDull
            // 
            this.radDull.AutoSize = true;
            this.radDull.Checked = true;
            this.radDull.Location = new System.Drawing.Point(6, 65);
            this.radDull.Name = "radDull";
            this.radDull.Size = new System.Drawing.Size(87, 17);
            this.radDull.TabIndex = 1;
            this.radDull.TabStop = true;
            this.radDull.Text = "Uninteresting";
            this.radDull.UseVisualStyleBackColor = true;
            this.radDull.CheckedChanged += new System.EventHandler(this.radDull_CheckedChanged);
            // 
            // pbxInsta6
            // 
            this.pbxInsta6.Location = new System.Drawing.Point(971, 574);
            this.pbxInsta6.Name = "pbxInsta6";
            this.pbxInsta6.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta6.TabIndex = 22;
            this.pbxInsta6.TabStop = false;
            // 
            // pbxInsta5
            // 
            this.pbxInsta5.Location = new System.Drawing.Point(815, 574);
            this.pbxInsta5.Name = "pbxInsta5";
            this.pbxInsta5.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta5.TabIndex = 21;
            this.pbxInsta5.TabStop = false;
            // 
            // pbxInsta4
            // 
            this.pbxInsta4.Location = new System.Drawing.Point(659, 574);
            this.pbxInsta4.Name = "pbxInsta4";
            this.pbxInsta4.Size = new System.Drawing.Size(150, 150);
            this.pbxInsta4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxInsta4.TabIndex = 20;
            this.pbxInsta4.TabStop = false;
            // 
            // prgLoading
            // 
            this.prgLoading.Location = new System.Drawing.Point(556, 868);
            this.prgLoading.Name = "prgLoading";
            this.prgLoading.Size = new System.Drawing.Size(360, 23);
            this.prgLoading.TabIndex = 23;
            // 
            // lblLoading1
            // 
            this.lblLoading1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoading1.Location = new System.Drawing.Point(556, 894);
            this.lblLoading1.Name = "lblLoading1";
            this.lblLoading1.Size = new System.Drawing.Size(360, 23);
            this.lblLoading1.TabIndex = 24;
            this.lblLoading1.Text = "lblLoading1";
            // 
            // lblLoading2
            // 
            this.lblLoading2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoading2.Location = new System.Drawing.Point(556, 917);
            this.lblLoading2.Name = "lblLoading2";
            this.lblLoading2.Size = new System.Drawing.Size(360, 23);
            this.lblLoading2.TabIndex = 25;
            this.lblLoading2.Text = "lblLoading2";
            // 
            // lblLoading3
            // 
            this.lblLoading3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoading3.Location = new System.Drawing.Point(556, 940);
            this.lblLoading3.Name = "lblLoading3";
            this.lblLoading3.Size = new System.Drawing.Size(360, 23);
            this.lblLoading3.TabIndex = 26;
            this.lblLoading3.Text = "lblLoading3";
            // 
            // lblNumAutoSwiped
            // 
            this.lblNumAutoSwiped.Location = new System.Drawing.Point(982, 968);
            this.lblNumAutoSwiped.Name = "lblNumAutoSwiped";
            this.lblNumAutoSwiped.Size = new System.Drawing.Size(100, 23);
            this.lblNumAutoSwiped.TabIndex = 27;
            this.lblNumAutoSwiped.Text = "Auto-swipe Count:";
            // 
            // numAutoSwiped
            // 
            this.numAutoSwiped.InterceptArrowKeys = false;
            this.numAutoSwiped.Location = new System.Drawing.Point(1088, 966);
            this.numAutoSwiped.Name = "numAutoSwiped";
            this.numAutoSwiped.ReadOnly = true;
            this.numAutoSwiped.Size = new System.Drawing.Size(35, 20);
            this.numAutoSwiped.TabIndex = 28;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 998);
            this.Controls.Add(this.numAutoSwiped);
            this.Controls.Add(this.lblNumAutoSwiped);
            this.Controls.Add(this.lblLoading3);
            this.Controls.Add(this.lblLoading2);
            this.Controls.Add(this.lblLoading1);
            this.Controls.Add(this.prgLoading);
            this.Controls.Add(this.pbxInsta6);
            this.Controls.Add(this.pbxInsta5);
            this.Controls.Add(this.pbxInsta4);
            this.Controls.Add(this.grpCategory);
            this.Controls.Add(this.btnSaveAllInfo);
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
            this.Controls.Add(this.pbxInsta3);
            this.Controls.Add(this.pbxInsta2);
            this.Controls.Add(this.pbxInsta1);
            this.Controls.Add(this.pbxPhotos);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Tinder Plus";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbxPhotos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurPhoto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPhotoCount)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.grpCategory.ResumeLayout(false);
            this.grpCategory.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxInsta4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoSwiped)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxPhotos;
        private System.Windows.Forms.PictureBox pbxInsta1;
        private System.Windows.Forms.PictureBox pbxInsta2;
        private System.Windows.Forms.PictureBox pbxInsta3;
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
        private System.Windows.Forms.Button btnSaveAllInfo;
        private System.Windows.Forms.GroupBox grpCategory;
        private System.Windows.Forms.RadioButton radMate;
        private System.Windows.Forms.RadioButton radHot;
        private System.Windows.Forms.RadioButton radDull;
        private System.Windows.Forms.RadioButton radBigNo;
        private System.Windows.Forms.PictureBox pbxInsta6;
        private System.Windows.Forms.PictureBox pbxInsta5;
        private System.Windows.Forms.PictureBox pbxInsta4;
        private System.Windows.Forms.ProgressBar prgLoading;
        private System.Windows.Forms.Label lblLoading1;
        private System.Windows.Forms.Label lblLoading2;
        private System.Windows.Forms.Label lblLoading3;
        private System.Windows.Forms.RadioButton radUgly;
        private System.Windows.Forms.RadioButton radFun;
        private System.Windows.Forms.Label lblNumAutoSwiped;
        private System.Windows.Forms.NumericUpDown numAutoSwiped;
    }
}


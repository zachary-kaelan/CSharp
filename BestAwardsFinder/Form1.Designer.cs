namespace BestAwardsFinder
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
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblNickname = new System.Windows.Forms.Label();
            this.btnGetResults = new System.Windows.Forms.Button();
            this.grdByTank = new System.Windows.Forms.DataGridView();
            this.grdOverall = new System.Windows.Forms.DataGridView();
            this.colTank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colAward = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPerAward = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTankType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRewards = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAwardName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotalBattlesPer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdByTank)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOverall)).BeginInit();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtUsername.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtUsername.Location = new System.Drawing.Point(12, 29);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(13, 13);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(55, 13);
            this.lblNickname.TabIndex = 1;
            this.lblNickname.Text = "Username";
            // 
            // btnGetResults
            // 
            this.btnGetResults.Location = new System.Drawing.Point(12, 55);
            this.btnGetResults.Name = "btnGetResults";
            this.btnGetResults.Size = new System.Drawing.Size(100, 23);
            this.btnGetResults.TabIndex = 2;
            this.btnGetResults.Text = "Get Results";
            this.btnGetResults.UseVisualStyleBackColor = true;
            this.btnGetResults.Click += new System.EventHandler(this.btnGetResults_Click);
            // 
            // grdByTank
            // 
            this.grdByTank.AllowUserToAddRows = false;
            this.grdByTank.AllowUserToDeleteRows = false;
            this.grdByTank.AllowUserToOrderColumns = true;
            this.grdByTank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdByTank.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTank,
            this.colImage,
            this.colAward,
            this.colPerAward});
            this.grdByTank.Location = new System.Drawing.Point(118, 168);
            this.grdByTank.Name = "grdByTank";
            this.grdByTank.ReadOnly = true;
            this.grdByTank.Size = new System.Drawing.Size(606, 150);
            this.grdByTank.TabIndex = 3;
            // 
            // grdOverall
            // 
            this.grdOverall.AllowUserToAddRows = false;
            this.grdOverall.AllowUserToDeleteRows = false;
            this.grdOverall.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdOverall.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTankType,
            this.colRewards,
            this.colAwardName,
            this.colTotalBattlesPer});
            this.grdOverall.Location = new System.Drawing.Point(118, 12);
            this.grdOverall.Name = "grdOverall";
            this.grdOverall.ReadOnly = true;
            this.grdOverall.Size = new System.Drawing.Size(606, 150);
            this.grdOverall.TabIndex = 4;
            // 
            // colTank
            // 
            this.colTank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTank.HeaderText = "Tank";
            this.colTank.Name = "colTank";
            this.colTank.ReadOnly = true;
            this.colTank.Width = 57;
            // 
            // colImage
            // 
            this.colImage.HeaderText = "Image";
            this.colImage.Name = "colImage";
            this.colImage.ReadOnly = true;
            // 
            // colAward
            // 
            this.colAward.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAward.HeaderText = "Award";
            this.colAward.Name = "colAward";
            this.colAward.ReadOnly = true;
            this.colAward.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colAward.Width = 62;
            // 
            // colPerAward
            // 
            this.colPerAward.HeaderText = "Battles per Award";
            this.colPerAward.Name = "colPerAward";
            this.colPerAward.ReadOnly = true;
            this.colPerAward.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colPerAward.Width = 115;
            // 
            // colTankType
            // 
            this.colTankType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTankType.HeaderText = "Tank Type";
            this.colTankType.Name = "colTankType";
            this.colTankType.ReadOnly = true;
            this.colTankType.Width = 84;
            // 
            // colRewards
            // 
            this.colRewards.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colRewards.HeaderText = "Mission Rewards";
            this.colRewards.MinimumWidth = 110;
            this.colRewards.Name = "colRewards";
            this.colRewards.ReadOnly = true;
            this.colRewards.Width = 110;
            // 
            // colAwardName
            // 
            this.colAwardName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAwardName.HeaderText = "Award";
            this.colAwardName.Name = "colAwardName";
            this.colAwardName.ReadOnly = true;
            this.colAwardName.Width = 62;
            // 
            // colTotalBattlesPer
            // 
            this.colTotalBattlesPer.HeaderText = "Battles per Award";
            this.colTotalBattlesPer.Name = "colTotalBattlesPer";
            this.colTotalBattlesPer.ReadOnly = true;
            this.colTotalBattlesPer.Width = 115;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 450);
            this.Controls.Add(this.grdOverall);
            this.Controls.Add(this.grdByTank);
            this.Controls.Add(this.btnGetResults);
            this.Controls.Add(this.lblNickname);
            this.Controls.Add(this.txtUsername);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.grdByTank)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdOverall)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.Button btnGetResults;
        private System.Windows.Forms.DataGridView grdByTank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTank;
        private System.Windows.Forms.DataGridViewImageColumn colImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAward;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPerAward;
        private System.Windows.Forms.DataGridView grdOverall;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTankType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRewards;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAwardName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalBattlesPer;
    }
}


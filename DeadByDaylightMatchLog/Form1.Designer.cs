﻿namespace DeadByDaylightMatchLog
{
    partial class frmLogMatch
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
            this.btnLogMatch = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.mnuLogMatch = new System.Windows.Forms.MenuStrip();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblKillerOrSurvivorPrompt = new System.Windows.Forms.Label();
            this.lblKillerPrompt = new System.Windows.Forms.Label();
            this.lblOR = new System.Windows.Forms.Label();
            this.lblSurvivorPrompt = new System.Windows.Forms.Label();
            this.cboKiller = new System.Windows.Forms.ComboBox();
            this.cboSurvivor = new System.Windows.Forms.ComboBox();
            this.picPortrait = new System.Windows.Forms.PictureBox();
            this.txtBloodpoints = new System.Windows.Forms.TextBox();
            this.lblBloodpointsPrompt = new System.Windows.Forms.Label();
            this.lblPipsPrompt = new System.Windows.Forms.Label();
            this.numPips = new System.Windows.Forms.NumericUpDown();
            this.cboOfferings = new System.Windows.Forms.ComboBox();
            this.lblOfferingsPrompt = new System.Windows.Forms.Label();
            this.lstOfferings = new System.Windows.Forms.ListBox();
            this.lblPerksPrompt = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cboPerks = new System.Windows.Forms.ComboBox();
            this.lstPerks = new System.Windows.Forms.ListBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.mnuLogMatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPortrait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPips)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLogMatch
            // 
            this.btnLogMatch.Enabled = false;
            this.btnLogMatch.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.btnLogMatch.Location = new System.Drawing.Point(12, 27);
            this.btnLogMatch.Name = "btnLogMatch";
            this.btnLogMatch.Size = new System.Drawing.Size(243, 23);
            this.btnLogMatch.TabIndex = 0;
            this.btnLogMatch.Text = "LOG A MATCH";
            this.btnLogMatch.UseVisualStyleBackColor = true;
            // 
            // btnViewHistory
            // 
            this.btnViewHistory.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.btnViewHistory.Location = new System.Drawing.Point(261, 27);
            this.btnViewHistory.Name = "btnViewHistory";
            this.btnViewHistory.Size = new System.Drawing.Size(243, 23);
            this.btnViewHistory.TabIndex = 1;
            this.btnViewHistory.Text = "VIEW HISTORY";
            this.btnViewHistory.UseVisualStyleBackColor = true;
            // 
            // mnuLogMatch
            // 
            this.mnuLogMatch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.mnuLogMatch.Location = new System.Drawing.Point(0, 0);
            this.mnuLogMatch.Name = "mnuLogMatch";
            this.mnuLogMatch.Size = new System.Drawing.Size(516, 24);
            this.mnuLogMatch.TabIndex = 2;
            this.mnuLogMatch.Text = "menuStrip1";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // lblKillerOrSurvivorPrompt
            // 
            this.lblKillerOrSurvivorPrompt.AutoSize = true;
            this.lblKillerOrSurvivorPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKillerOrSurvivorPrompt.Location = new System.Drawing.Point(21, 63);
            this.lblKillerOrSurvivorPrompt.Name = "lblKillerOrSurvivorPrompt";
            this.lblKillerOrSurvivorPrompt.Size = new System.Drawing.Size(92, 13);
            this.lblKillerOrSurvivorPrompt.TabIndex = 3;
            this.lblKillerOrSurvivorPrompt.Text = "Were you playing:";
            // 
            // lblKillerPrompt
            // 
            this.lblKillerPrompt.AutoSize = true;
            this.lblKillerPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblKillerPrompt.Location = new System.Drawing.Point(185, 63);
            this.lblKillerPrompt.Name = "lblKillerPrompt";
            this.lblKillerPrompt.Size = new System.Drawing.Size(60, 15);
            this.lblKillerPrompt.TabIndex = 4;
            this.lblKillerPrompt.Text = "Killer...";
            // 
            // lblOR
            // 
            this.lblOR.AutoSize = true;
            this.lblOR.Location = new System.Drawing.Point(328, 63);
            this.lblOR.Name = "lblOR";
            this.lblOR.Size = new System.Drawing.Size(32, 13);
            this.lblOR.TabIndex = 5;
            this.lblOR.Text = "OR...";
            // 
            // lblSurvivorPrompt
            // 
            this.lblSurvivorPrompt.AutoSize = true;
            this.lblSurvivorPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblSurvivorPrompt.Location = new System.Drawing.Point(430, 61);
            this.lblSurvivorPrompt.Name = "lblSurvivorPrompt";
            this.lblSurvivorPrompt.Size = new System.Drawing.Size(75, 15);
            this.lblSurvivorPrompt.TabIndex = 6;
            this.lblSurvivorPrompt.Text = "Survivor?";
            // 
            // cboKiller
            // 
            this.cboKiller.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboKiller.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboKiller.FormattingEnabled = true;
            this.cboKiller.Items.AddRange(new object[] {
            "",
            "Cannibal",
            "Doctor",
            "Hag",
            "Hillbilly",
            "Huntress",
            "Nightmare",
            "Nurse",
            "Shape",
            "Trapper",
            "Wraith"});
            this.cboKiller.Location = new System.Drawing.Point(185, 79);
            this.cboKiller.Name = "cboKiller";
            this.cboKiller.Size = new System.Drawing.Size(117, 21);
            this.cboKiller.Sorted = true;
            this.cboKiller.TabIndex = 7;
            this.cboKiller.SelectedIndexChanged += new System.EventHandler(this.cboKiller_SelectedIndexChanged);
            // 
            // cboSurvivor
            // 
            this.cboSurvivor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboSurvivor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboSurvivor.FormattingEnabled = true;
            this.cboSurvivor.Items.AddRange(new object[] {
            "",
            "Ace",
            "Bill",
            "Claudette",
            "David",
            "Dwight",
            "Feng",
            "Jake",
            "Laurie",
            "Meg",
            "Nea",
            "Quentin"});
            this.cboSurvivor.Location = new System.Drawing.Point(390, 79);
            this.cboSurvivor.Name = "cboSurvivor";
            this.cboSurvivor.Size = new System.Drawing.Size(117, 21);
            this.cboSurvivor.Sorted = true;
            this.cboSurvivor.TabIndex = 8;
            this.cboSurvivor.SelectedIndexChanged += new System.EventHandler(this.cboSurvivor_SelectedIndexChanged);
            // 
            // picPortrait
            // 
            this.picPortrait.InitialImage = null;
            this.picPortrait.Location = new System.Drawing.Point(15, 79);
            this.picPortrait.Name = "picPortrait";
            this.picPortrait.Size = new System.Drawing.Size(164, 243);
            this.picPortrait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picPortrait.TabIndex = 9;
            this.picPortrait.TabStop = false;
            // 
            // txtBloodpoints
            // 
            this.txtBloodpoints.Location = new System.Drawing.Point(303, 114);
            this.txtBloodpoints.Name = "txtBloodpoints";
            this.txtBloodpoints.Size = new System.Drawing.Size(78, 20);
            this.txtBloodpoints.TabIndex = 10;
            // 
            // lblBloodpointsPrompt
            // 
            this.lblBloodpointsPrompt.AutoSize = true;
            this.lblBloodpointsPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblBloodpointsPrompt.Location = new System.Drawing.Point(203, 117);
            this.lblBloodpointsPrompt.Name = "lblBloodpointsPrompt";
            this.lblBloodpointsPrompt.Size = new System.Drawing.Size(94, 15);
            this.lblBloodpointsPrompt.TabIndex = 11;
            this.lblBloodpointsPrompt.Text = "Bloodpoints";
            // 
            // lblPipsPrompt
            // 
            this.lblPipsPrompt.AutoSize = true;
            this.lblPipsPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblPipsPrompt.Location = new System.Drawing.Point(408, 117);
            this.lblPipsPrompt.Name = "lblPipsPrompt";
            this.lblPipsPrompt.Size = new System.Drawing.Size(35, 15);
            this.lblPipsPrompt.TabIndex = 12;
            this.lblPipsPrompt.Text = "Pips";
            // 
            // numPips
            // 
            this.numPips.Location = new System.Drawing.Point(449, 114);
            this.numPips.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numPips.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            this.numPips.Name = "numPips";
            this.numPips.Size = new System.Drawing.Size(31, 20);
            this.numPips.TabIndex = 13;
            this.numPips.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cboOfferings
            // 
            this.cboOfferings.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboOfferings.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOfferings.FormattingEnabled = true;
            this.cboOfferings.Items.AddRange(new object[] {
            "Slightly Increased Luck",
            "Moderately Increased Luck",
            "Considerably Increased Luck",
            "Slightly Increased Luck for all Survivors",
            "Moderately Increased Luck for all Survivors",
            "Considerably Increased Luck for all Survivors",
            "Slightly Lesser Mist",
            "Slightly Thicker Mist",
            "Moderately Thicker Mist",
            "Considerably Thicker Mist",
            "Darkest Moonlight",
            "Dimmed Moonlight",
            "Brighter Moonlight",
            "Brightest Moonlight",
            "2 Less Chests",
            "1 Less Chest",
            "1 More Chest",
            "2 More Chests",
            "1 Less Hook",
            "1 More Hook",
            "2 More Hooks",
            "3 More Hooks",
            "Cypress Memento Mori",
            "Ivory Memento Mori",
            "Ebony Memento Mori",
            "Start with another Survivor",
            "Start furthest from the Killer",
            "All Survivors Together",
            "All Survivors Separated "});
            this.cboOfferings.Location = new System.Drawing.Point(83, 3);
            this.cboOfferings.Name = "cboOfferings";
            this.cboOfferings.Size = new System.Drawing.Size(176, 21);
            this.cboOfferings.TabIndex = 14;
            this.cboOfferings.SelectedIndexChanged += new System.EventHandler(this.cboOfferings_SelectedIndexChanged);
            // 
            // lblOfferingsPrompt
            // 
            this.lblOfferingsPrompt.AutoSize = true;
            this.lblOfferingsPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblOfferingsPrompt.Location = new System.Drawing.Point(3, 3);
            this.lblOfferingsPrompt.Name = "lblOfferingsPrompt";
            this.lblOfferingsPrompt.Size = new System.Drawing.Size(74, 15);
            this.lblOfferingsPrompt.TabIndex = 15;
            this.lblOfferingsPrompt.Text = "Offerings";
            // 
            // lstOfferings
            // 
            this.lstOfferings.FormattingEnabled = true;
            this.lstOfferings.Location = new System.Drawing.Point(82, 30);
            this.lstOfferings.Name = "lstOfferings";
            this.lstOfferings.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstOfferings.Size = new System.Drawing.Size(176, 69);
            this.lstOfferings.TabIndex = 16;
            this.lstOfferings.SelectedIndexChanged += new System.EventHandler(this.lstOfferings_SelectedIndexChanged);
            this.lstOfferings.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstOfferings_MouseDoubleClick);
            // 
            // lblPerksPrompt
            // 
            this.lblPerksPrompt.AutoSize = true;
            this.lblPerksPrompt.Font = new System.Drawing.Font("Showcard Gothic", 9F);
            this.lblPerksPrompt.Location = new System.Drawing.Point(3, 4);
            this.lblPerksPrompt.Name = "lblPerksPrompt";
            this.lblPerksPrompt.Size = new System.Drawing.Size(46, 15);
            this.lblPerksPrompt.TabIndex = 17;
            this.lblPerksPrompt.Text = "Perks";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblOfferingsPrompt);
            this.panel1.Controls.Add(this.cboOfferings);
            this.panel1.Controls.Add(this.lstOfferings);
            this.panel1.Location = new System.Drawing.Point(246, 150);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 112);
            this.panel1.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cboPerks);
            this.panel2.Controls.Add(this.lstPerks);
            this.panel2.Controls.Add(this.lblPerksPrompt);
            this.panel2.Location = new System.Drawing.Point(246, 268);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(261, 99);
            this.panel2.TabIndex = 20;
            // 
            // cboPerks
            // 
            this.cboPerks.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboPerks.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboPerks.FormattingEnabled = true;
            this.cboPerks.Items.AddRange(new object[] {
            "Slightly Increased Luck",
            "Moderately Increased Luck",
            "Considerably Increased Luck",
            "Slightly Increased Luck for all Survivors",
            "Moderately Increased Luck for all Survivors",
            "Considerably Increased Luck for all Survivors",
            "Slightly Lesser Mist",
            "Slightly Thicker Mist",
            "Moderately Thicker Mist",
            "Considerably Thicker Mist",
            "Darkest Moonlight",
            "Dimmed Moonlight",
            "Brighter Moonlight",
            "Brightest Moonlight",
            "2 Less Chests",
            "1 Less Chest",
            "1 More Chest",
            "2 More Chests",
            "1 Less Hook",
            "1 More Hook",
            "2 More Hooks",
            "3 More Hooks",
            "Cypress Memento Mori",
            "Ivory Memento Mori",
            "Ebony Memento Mori",
            "Start with another Survivor",
            "Start furthest from the Killer",
            "All Survivors Together",
            "All Survivors Separated "});
            this.cboPerks.Location = new System.Drawing.Point(83, 3);
            this.cboPerks.Name = "cboPerks";
            this.cboPerks.Size = new System.Drawing.Size(176, 21);
            this.cboPerks.TabIndex = 14;
            // 
            // lstPerks
            // 
            this.lstPerks.FormattingEnabled = true;
            this.lstPerks.Location = new System.Drawing.Point(82, 30);
            this.lstPerks.Name = "lstPerks";
            this.lstPerks.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lstPerks.Size = new System.Drawing.Size(176, 56);
            this.lstPerks.TabIndex = 16;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(73, 321);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 45);
            this.trackBar1.TabIndex = 21;
            // 
            // frmLogMatch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 414);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.numPips);
            this.Controls.Add(this.lblPipsPrompt);
            this.Controls.Add(this.lblBloodpointsPrompt);
            this.Controls.Add(this.txtBloodpoints);
            this.Controls.Add(this.picPortrait);
            this.Controls.Add(this.cboSurvivor);
            this.Controls.Add(this.cboKiller);
            this.Controls.Add(this.lblSurvivorPrompt);
            this.Controls.Add(this.lblOR);
            this.Controls.Add(this.lblKillerPrompt);
            this.Controls.Add(this.lblKillerOrSurvivorPrompt);
            this.Controls.Add(this.btnViewHistory);
            this.Controls.Add(this.btnLogMatch);
            this.Controls.Add(this.mnuLogMatch);
            this.MainMenuStrip = this.mnuLogMatch;
            this.Name = "frmLogMatch";
            this.Text = "Dead By Daylight Match Logger";
            this.mnuLogMatch.ResumeLayout(false);
            this.mnuLogMatch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPortrait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPips)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLogMatch;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.MenuStrip mnuLogMatch;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Label lblKillerOrSurvivorPrompt;
        private System.Windows.Forms.Label lblKillerPrompt;
        private System.Windows.Forms.Label lblOR;
        private System.Windows.Forms.Label lblSurvivorPrompt;
        private System.Windows.Forms.ComboBox cboKiller;
        private System.Windows.Forms.ComboBox cboSurvivor;
        private System.Windows.Forms.PictureBox picPortrait;
        private System.Windows.Forms.TextBox txtBloodpoints;
        private System.Windows.Forms.Label lblBloodpointsPrompt;
        private System.Windows.Forms.Label lblPipsPrompt;
        private System.Windows.Forms.NumericUpDown numPips;
        private System.Windows.Forms.ComboBox cboOfferings;
        private System.Windows.Forms.Label lblOfferingsPrompt;
        private System.Windows.Forms.ListBox lstOfferings;
        private System.Windows.Forms.Label lblPerksPrompt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cboPerks;
        private System.Windows.Forms.ListBox lstPerks;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}


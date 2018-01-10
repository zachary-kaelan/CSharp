namespace Spaced_Repetition
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Arithmetic");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Scientific", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Categories", new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cxtCategories = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddCategory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStart = new System.Windows.Forms.Button();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.chrtTimes = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.treCategories = new System.Windows.Forms.TreeView();
            this.mnuCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMain.SuspendLayout();
            this.cxtCategories.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chrtTimes)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(814, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // cxtCategories
            // 
            this.cxtCategories.AutoSize = false;
            this.cxtCategories.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCollapseAll,
            this.mnuExpandAll,
            this.toolStripSeparator1,
            this.mnuAddCategory,
            this.mnuAddGroup});
            this.cxtCategories.Name = "cxtCategories";
            this.cxtCategories.Size = new System.Drawing.Size(157, 120);
            this.cxtCategories.Opening += new System.ComponentModel.CancelEventHandler(this.cxtCategories_Opening);
            // 
            // mnuAddCategory
            // 
            this.mnuAddCategory.Name = "mnuAddCategory";
            this.mnuAddCategory.Size = new System.Drawing.Size(156, 22);
            this.mnuAddCategory.Text = "Add Category...";
            this.mnuAddCategory.Click += new System.EventHandler(this.mnuAddCategory_Click);
            // 
            // mnuAddGroup
            // 
            this.mnuAddGroup.Name = "mnuAddGroup";
            this.mnuAddGroup.Size = new System.Drawing.Size(156, 22);
            this.mnuAddGroup.Text = "Add Group...";
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.LimeGreen;
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnStart.Location = new System.Drawing.Point(12, 482);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(265, 31);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start Session";
            this.btnStart.UseVisualStyleBackColor = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // mnuExpandAll
            // 
            this.mnuExpandAll.Name = "mnuExpandAll";
            this.mnuExpandAll.Size = new System.Drawing.Size(156, 22);
            this.mnuExpandAll.Text = "Expand All";
            this.mnuExpandAll.Click += new System.EventHandler(this.mnuExpandAll_Click);
            // 
            // chrtTimes
            // 
            chartArea1.Name = "ChartArea1";
            this.chrtTimes.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chrtTimes.Legends.Add(legend1);
            this.chrtTimes.Location = new System.Drawing.Point(284, 27);
            this.chrtTimes.Name = "chrtTimes";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chrtTimes.Series.Add(series1);
            this.chrtTimes.Size = new System.Drawing.Size(518, 486);
            this.chrtTimes.TabIndex = 1;
            this.chrtTimes.Text = "Scores";
            // 
            // treCategories
            // 
            this.treCategories.CheckBoxes = true;
            this.treCategories.ContextMenuStrip = this.cxtCategories;
            this.treCategories.Location = new System.Drawing.Point(9, 27);
            this.treCategories.Name = "treCategories";
            treeNode1.Name = "ndeMath";
            treeNode1.Text = "Arithmetic";
            treeNode2.Name = "ndeSci";
            treeNode2.Text = "Scientific";
            treeNode3.Name = "ndeRoot";
            treeNode3.Text = "Categories";
            this.treCategories.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.treCategories.Size = new System.Drawing.Size(268, 449);
            this.treCategories.TabIndex = 2;
            this.treCategories.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treCategories_AfterCheck);
            // 
            // mnuCollapseAll
            // 
            this.mnuCollapseAll.Name = "mnuCollapseAll";
            this.mnuCollapseAll.Size = new System.Drawing.Size(156, 22);
            this.mnuCollapseAll.Text = "Collapse All";
            this.mnuCollapseAll.Click += new System.EventHandler(this.mnuCollapseAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 525);
            this.Controls.Add(this.treCategories);
            this.Controls.Add(this.chrtTimes);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.mnuMain);
            this.MainMenuStrip = this.mnuMain;
            this.Name = "Form1";
            this.Text = "Spaced Repetition";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.cxtCategories.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chrtTimes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cxtCategories;
        private System.Windows.Forms.ToolStripMenuItem mnuAddCategory;
        private System.Windows.Forms.ToolStripMenuItem mnuAddGroup;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ToolStripMenuItem mnuExpandAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chrtTimes;
        private System.Windows.Forms.TreeView treCategories;
        private System.Windows.Forms.ToolStripMenuItem mnuCollapseAll;
    }
}


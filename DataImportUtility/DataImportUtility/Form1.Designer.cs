namespace DataImportUtility
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
            this.trvLookup = new System.Windows.Forms.TreeView();
            this.ofdInput = new System.Windows.Forms.OpenFileDialog();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lstMapping = new System.Windows.Forms.ListBox();
            this.lstLinked = new DataImportUtility.NavListBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trvLookup
            // 
            this.trvLookup.CheckBoxes = true;
            this.trvLookup.Location = new System.Drawing.Point(12, 12);
            this.trvLookup.Name = "trvLookup";
            this.trvLookup.ShowNodeToolTips = true;
            this.trvLookup.Size = new System.Drawing.Size(321, 439);
            this.trvLookup.TabIndex = 0;
            this.trvLookup.Tag = "";
            this.trvLookup.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvLookup_AfterSelect);
            // 
            // ofdInput
            // 
            this.ofdInput.Title = "Data Import Utility";
            this.ofdInput.FileOk += new System.ComponentModel.CancelEventHandler(this.ofdInput_FileOk);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(339, 428);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(145, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load File";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lstMapping
            // 
            this.lstMapping.FormattingEnabled = true;
            this.lstMapping.Location = new System.Drawing.Point(339, 12);
            this.lstMapping.Name = "lstMapping";
            this.lstMapping.Size = new System.Drawing.Size(145, 407);
            this.lstMapping.TabIndex = 2;
            this.lstMapping.SelectedIndexChanged += new System.EventHandler(this.lstMapping_SelectedIndexChanged);
            // 
            // lstLinked
            // 
            this.lstLinked.FormattingEnabled = true;
            this.lstLinked.Location = new System.Drawing.Point(490, 12);
            this.lstLinked.Name = "lstLinked";
            this.lstLinked.Size = new System.Drawing.Size(145, 407);
            this.lstLinked.TabIndex = 3;
            this.lstLinked.DragOver += new System.Windows.Forms.DragEventHandler(this.lstLinked_DragOver);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(490, 428);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(145, 23);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 463);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lstLinked);
            this.Controls.Add(this.lstMapping);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.trvLookup);
            this.Name = "Form1";
            this.Text = "Data Import Utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trvLookup;
        private System.Windows.Forms.OpenFileDialog ofdInput;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ListBox lstMapping;
        private NavListBox lstLinked;
        private System.Windows.Forms.Button btnExport;
    }
}


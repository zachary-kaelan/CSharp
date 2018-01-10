namespace DnD
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
            this.btnTables = new System.Windows.Forms.Button();
            this.lstTest = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnTables
            // 
            this.btnTables.Location = new System.Drawing.Point(12, 12);
            this.btnTables.Name = "btnTables";
            this.btnTables.Size = new System.Drawing.Size(107, 23);
            this.btnTables.TabIndex = 0;
            this.btnTables.Text = "Extract Tables";
            this.btnTables.UseVisualStyleBackColor = true;
            this.btnTables.Click += new System.EventHandler(this.btnTables_Click);
            // 
            // lstTest
            // 
            this.lstTest.FormattingEnabled = true;
            this.lstTest.HorizontalScrollbar = true;
            this.lstTest.Location = new System.Drawing.Point(125, 12);
            this.lstTest.Name = "lstTest";
            this.lstTest.ScrollAlwaysVisible = true;
            this.lstTest.Size = new System.Drawing.Size(433, 433);
            this.lstTest.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 463);
            this.Controls.Add(this.lstTest);
            this.Controls.Add(this.btnTables);
            this.Name = "Form1";
            this.Text = "DnD";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTables;
        private System.Windows.Forms.ListBox lstTest;
    }
}


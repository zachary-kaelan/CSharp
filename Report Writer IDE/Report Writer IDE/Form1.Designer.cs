namespace Report_Writer_IDE
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
            this.txtAuto = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtAuto
            // 
            this.txtAuto.Location = new System.Drawing.Point(12, 12);
            this.txtAuto.Multiline = true;
            this.txtAuto.Name = "txtAuto";
            this.txtAuto.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAuto.Size = new System.Drawing.Size(260, 237);
            this.txtAuto.TabIndex = 0;
            this.txtAuto.WordWrap = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.txtAuto);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAuto;
    }
}


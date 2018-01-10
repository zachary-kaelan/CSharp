namespace MusicPlayer
{
    partial class Equalizer
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
            this.sldPreamp = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.sldPreamp)).BeginInit();
            this.SuspendLayout();
            // 
            // sldPreamp
            // 
            this.sldPreamp.LargeChange = 4;
            this.sldPreamp.Location = new System.Drawing.Point(28, 59);
            this.sldPreamp.Maximum = 256;
            this.sldPreamp.Name = "sldPreamp";
            this.sldPreamp.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.sldPreamp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sldPreamp.Size = new System.Drawing.Size(45, 190);
            this.sldPreamp.SmallChange = 2;
            this.sldPreamp.TabIndex = 0;
            this.sldPreamp.TickFrequency = 16;
            this.sldPreamp.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // Equalizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 297);
            this.Controls.Add(this.sldPreamp);
            this.Name = "Equalizer";
            this.Text = "Equalizer";
            this.Load += new System.EventHandler(this.Equalizer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sldPreamp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar sldPreamp;
    }
}
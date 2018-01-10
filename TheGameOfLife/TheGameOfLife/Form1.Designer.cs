namespace TheGameOfLife
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
            this.lblGame = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.grpPlayer = new System.Windows.Forms.GroupBox();
            this.radBlack = new System.Windows.Forms.RadioButton();
            this.radWhite = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.sldSpeed = new System.Windows.Forms.TrackBar();
            this.grpPlayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // lblGame
            // 
            this.lblGame.Font = new System.Drawing.Font("OCR A Extended", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGame.Location = new System.Drawing.Point(12, 73);
            this.lblGame.Name = "lblGame";
            this.lblGame.Size = new System.Drawing.Size(260, 243);
            this.lblGame.TabIndex = 0;
            this.lblGame.Click += new System.EventHandler(this.lblGame_Click);
            // 
            // txtInput
            // 
            this.txtInput.Font = new System.Drawing.Font("OCR A Extended", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(278, 76);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(270, 240);
            this.txtInput.TabIndex = 1;
            // 
            // grpPlayer
            // 
            this.grpPlayer.Controls.Add(this.radBlack);
            this.grpPlayer.Controls.Add(this.radWhite);
            this.grpPlayer.Location = new System.Drawing.Point(278, 22);
            this.grpPlayer.Name = "grpPlayer";
            this.grpPlayer.Size = new System.Drawing.Size(270, 48);
            this.grpPlayer.TabIndex = 2;
            this.grpPlayer.TabStop = false;
            this.grpPlayer.Text = "Player";
            // 
            // radBlack
            // 
            this.radBlack.AutoSize = true;
            this.radBlack.Location = new System.Drawing.Point(158, 19);
            this.radBlack.Name = "radBlack";
            this.radBlack.Size = new System.Drawing.Size(59, 17);
            this.radBlack.TabIndex = 1;
            this.radBlack.Text = "BLACK";
            this.radBlack.UseVisualStyleBackColor = true;
            // 
            // radWhite
            // 
            this.radWhite.AutoSize = true;
            this.radWhite.Checked = true;
            this.radWhite.Location = new System.Drawing.Point(48, 19);
            this.radWhite.Name = "radWhite";
            this.radWhite.Size = new System.Drawing.Size(61, 17);
            this.radWhite.TabIndex = 0;
            this.radWhite.TabStop = true;
            this.radWhite.Text = "WHITE";
            this.radWhite.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(14, 11);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(175, 11);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(75, 23);
            this.btnStep.TabIndex = 4;
            this.btnStep.Text = "Step";
            this.btnStep.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(96, 11);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // sldSpeed
            // 
            this.sldSpeed.Location = new System.Drawing.Point(14, 41);
            this.sldSpeed.Name = "sldSpeed";
            this.sldSpeed.Size = new System.Drawing.Size(258, 45);
            this.sldSpeed.TabIndex = 6;
            this.sldSpeed.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.sldSpeed.Value = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 325);
            this.Controls.Add(this.sldSpeed);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.grpPlayer);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblGame);
            this.Name = "Form1";
            this.Text = "The Game of Life";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grpPlayer.ResumeLayout(false);
            this.grpPlayer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblGame;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.GroupBox grpPlayer;
        private System.Windows.Forms.RadioButton radBlack;
        private System.Windows.Forms.RadioButton radWhite;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TrackBar sldSpeed;
    }
}


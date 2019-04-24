namespace NeuralNet
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnTrain = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblInputsPrompt = new System.Windows.Forms.Label();
            this.lblPromptOutputs = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.chkLstInputs = new System.Windows.Forms.CheckedListBox();
            this.lblPromptBinary = new System.Windows.Forms.Label();
            this.prgEpochs = new System.Windows.Forms.ProgressBar();
            this.ttpEpochs = new System.Windows.Forms.ToolTip(this.components);
            this.chtError = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chkOutput = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chtError)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(12, 396);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(594, 111);
            this.treeView1.TabIndex = 0;
            this.treeView1.Enter += new System.EventHandler(this.treeView1_Enter);
            this.treeView1.Leave += new System.EventHandler(this.treeView1_Leave);
            // 
            // btnTrain
            // 
            this.btnTrain.Location = new System.Drawing.Point(612, 396);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(135, 40);
            this.btnTrain.TabIndex = 1;
            this.btnTrain.Text = "Train";
            this.btnTrain.UseVisualStyleBackColor = true;
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(753, 396);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(135, 40);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Test";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(735, 378);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblInputsPrompt
            // 
            this.lblInputsPrompt.AutoSize = true;
            this.lblInputsPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInputsPrompt.Location = new System.Drawing.Point(754, 13);
            this.lblInputsPrompt.Name = "lblInputsPrompt";
            this.lblInputsPrompt.Size = new System.Drawing.Size(50, 15);
            this.lblInputsPrompt.TabIndex = 4;
            this.lblInputsPrompt.Text = "Inputs:";
            // 
            // lblPromptOutputs
            // 
            this.lblPromptOutputs.AutoSize = true;
            this.lblPromptOutputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPromptOutputs.Location = new System.Drawing.Point(754, 43);
            this.lblPromptOutputs.Name = "lblPromptOutputs";
            this.lblPromptOutputs.Size = new System.Drawing.Size(60, 15);
            this.lblPromptOutputs.TabIndex = 5;
            this.lblPromptOutputs.Text = "Outputs:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(850, 13);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown1.TabIndex = 6;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(850, 43);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(38, 20);
            this.numericUpDown2.TabIndex = 7;
            // 
            // chkLstInputs
            // 
            this.chkLstInputs.FormattingEnabled = true;
            this.chkLstInputs.Items.AddRange(new object[] {
            "Input 1",
            "Input 2",
            "Input 3"});
            this.chkLstInputs.Location = new System.Drawing.Point(753, 341);
            this.chkLstInputs.Name = "chkLstInputs";
            this.chkLstInputs.Size = new System.Drawing.Size(135, 49);
            this.chkLstInputs.TabIndex = 8;
            // 
            // lblPromptBinary
            // 
            this.lblPromptBinary.AutoSize = true;
            this.lblPromptBinary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPromptBinary.Location = new System.Drawing.Point(754, 322);
            this.lblPromptBinary.Name = "lblPromptBinary";
            this.lblPromptBinary.Size = new System.Drawing.Size(104, 15);
            this.lblPromptBinary.TabIndex = 9;
            this.lblPromptBinary.Text = "Inputs (Binary):";
            // 
            // prgEpochs
            // 
            this.prgEpochs.Location = new System.Drawing.Point(13, 514);
            this.prgEpochs.Maximum = 60000;
            this.prgEpochs.Name = "prgEpochs";
            this.prgEpochs.Size = new System.Drawing.Size(874, 23);
            this.prgEpochs.TabIndex = 10;
            // 
            // ttpEpochs
            // 
            this.ttpEpochs.Active = false;
            // 
            // chtError
            // 
            chartArea4.Name = "ChartArea1";
            this.chtError.ChartAreas.Add(chartArea4);
            this.chtError.Location = new System.Drawing.Point(894, 12);
            this.chtError.Name = "chtError";
            this.chtError.Size = new System.Drawing.Size(332, 525);
            this.chtError.TabIndex = 11;
            this.chtError.Text = "chart1";
            // 
            // chkOutput
            // 
            this.chkOutput.AutoCheck = false;
            this.chkOutput.AutoSize = true;
            this.chkOutput.Enabled = false;
            this.chkOutput.Location = new System.Drawing.Point(753, 442);
            this.chkOutput.Name = "chkOutput";
            this.chkOutput.Size = new System.Drawing.Size(58, 17);
            this.chkOutput.TabIndex = 13;
            this.chkOutput.Text = "Output";
            this.chkOutput.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1238, 549);
            this.Controls.Add(this.chkOutput);
            this.Controls.Add(this.chtError);
            this.Controls.Add(this.prgEpochs);
            this.Controls.Add(this.lblPromptBinary);
            this.Controls.Add(this.chkLstInputs);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.lblPromptOutputs);
            this.Controls.Add(this.lblInputsPrompt);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnTrain);
            this.Controls.Add(this.treeView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chtError)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnTrain;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblInputsPrompt;
        private System.Windows.Forms.Label lblPromptOutputs;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.CheckedListBox chkLstInputs;
        private System.Windows.Forms.Label lblPromptBinary;
        private System.Windows.Forms.ProgressBar prgEpochs;
        private System.Windows.Forms.ToolTip ttpEpochs;
        private System.Windows.Forms.DataVisualization.Charting.Chart chtError;
        private System.Windows.Forms.CheckBox chkOutput;
    }
}


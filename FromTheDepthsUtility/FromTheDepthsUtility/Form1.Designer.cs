namespace FromTheDepthsUtility
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
            this.cboEquations = new System.Windows.Forms.ComboBox();
            this.lblDropdown = new System.Windows.Forms.Label();
            this.btnSimulation = new System.Windows.Forms.Button();
            this.lstSimulation = new System.Windows.Forms.ListView();
            this.clmCycle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmFuel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmGas = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // cboEquations
            // 
            this.cboEquations.FormattingEnabled = true;
            this.cboEquations.Location = new System.Drawing.Point(257, 12);
            this.cboEquations.Name = "cboEquations";
            this.cboEquations.Size = new System.Drawing.Size(121, 21);
            this.cboEquations.TabIndex = 0;
            // 
            // lblDropdown
            // 
            this.lblDropdown.AutoSize = true;
            this.lblDropdown.Location = new System.Drawing.Point(13, 13);
            this.lblDropdown.Name = "lblDropdown";
            this.lblDropdown.Size = new System.Drawing.Size(140, 13);
            this.lblDropdown.TabIndex = 1;
            this.lblDropdown.Text = "Pick something to calculate.";
            // 
            // btnSimulation
            // 
            this.btnSimulation.Location = new System.Drawing.Point(16, 43);
            this.btnSimulation.Name = "btnSimulation";
            this.btnSimulation.Size = new System.Drawing.Size(117, 23);
            this.btnSimulation.TabIndex = 3;
            this.btnSimulation.Text = "Start Simulation";
            this.btnSimulation.UseVisualStyleBackColor = true;
            this.btnSimulation.Click += new System.EventHandler(this.btnSimulation_Click);
            // 
            // lstSimulation
            // 
            this.lstSimulation.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmCycle,
            this.clmFuel,
            this.clmTime,
            this.clmGas});
            this.lstSimulation.GridLines = true;
            this.lstSimulation.LabelWrap = false;
            this.lstSimulation.Location = new System.Drawing.Point(16, 72);
            this.lstSimulation.Name = "lstSimulation";
            this.lstSimulation.Size = new System.Drawing.Size(362, 342);
            this.lstSimulation.TabIndex = 4;
            this.lstSimulation.UseCompatibleStateImageBehavior = false;
            this.lstSimulation.View = System.Windows.Forms.View.Details;
            // 
            // clmCycle
            // 
            this.clmCycle.Text = "Cycle";
            this.clmCycle.Width = 86;
            // 
            // clmFuel
            // 
            this.clmFuel.Text = "Fuel";
            this.clmFuel.Width = 88;
            // 
            // clmTime
            // 
            this.clmTime.Text = "Time";
            this.clmTime.Width = 98;
            // 
            // clmGas
            // 
            this.clmGas.Text = "Gas";
            this.clmGas.Width = 180;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 427);
            this.Controls.Add(this.lstSimulation);
            this.Controls.Add(this.btnSimulation);
            this.Controls.Add(this.lblDropdown);
            this.Controls.Add(this.cboEquations);
            this.Name = "Form1";
            this.Text = "From The Depths Utility";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboEquations;
        private System.Windows.Forms.Label lblDropdown;
        private System.Windows.Forms.Button btnSimulation;
        private System.Windows.Forms.ListView lstSimulation;
        private System.Windows.Forms.ColumnHeader clmCycle;
        private System.Windows.Forms.ColumnHeader clmFuel;
        private System.Windows.Forms.ColumnHeader clmTime;
        private System.Windows.Forms.ColumnHeader clmGas;
    }
}


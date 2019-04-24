namespace RoutineEnforcer
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
            this.txtNewTask = new System.Windows.Forms.TextBox();
            this.lstTasks = new System.Windows.Forms.ListBox();
            this.btnAddTask = new System.Windows.Forms.Button();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.cboDurationUnit = new System.Windows.Forms.ComboBox();
            this.chkRepeat = new System.Windows.Forms.CheckBox();
            this.lblRepeatEvery = new System.Windows.Forms.Label();
            this.cboRepeatUnit = new System.Windows.Forms.ComboBox();
            this.numRepeat = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepeat)).BeginInit();
            this.SuspendLayout();
            // 
            // txtNewTask
            // 
            this.txtNewTask.Location = new System.Drawing.Point(13, 13);
            this.txtNewTask.Name = "txtNewTask";
            this.txtNewTask.Size = new System.Drawing.Size(181, 20);
            this.txtNewTask.TabIndex = 0;
            // 
            // lstTasks
            // 
            this.lstTasks.FormattingEnabled = true;
            this.lstTasks.Location = new System.Drawing.Point(12, 77);
            this.lstTasks.Name = "lstTasks";
            this.lstTasks.Size = new System.Drawing.Size(181, 303);
            this.lstTasks.TabIndex = 1;
            // 
            // btnAddTask
            // 
            this.btnAddTask.Location = new System.Drawing.Point(13, 39);
            this.btnAddTask.Name = "btnAddTask";
            this.btnAddTask.Size = new System.Drawing.Size(180, 32);
            this.btnAddTask.TabIndex = 2;
            this.btnAddTask.Text = "ADD TASK";
            this.btnAddTask.UseVisualStyleBackColor = true;
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "dddd, MMMM dd, yyyy h:mm tt";
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(260, 17);
            this.dtpStartTime.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.dtpStartTime.MinDate = new System.DateTime(2018, 3, 20, 0, 0, 0, 0);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.Size = new System.Drawing.Size(253, 20);
            this.dtpStartTime.TabIndex = 3;
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(200, 20);
            this.lblStartTime.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(55, 13);
            this.lblStartTime.TabIndex = 4;
            this.lblStartTime.Text = "Start Time";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(200, 55);
            this.lblDuration.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(47, 13);
            this.lblDuration.TabIndex = 5;
            this.lblDuration.Text = "Duration";
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(260, 53);
            this.numDuration.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.numDuration.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(57, 20);
            this.numDuration.TabIndex = 6;
            this.numDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cboDurationUnit
            // 
            this.cboDurationUnit.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboDurationUnit.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDurationUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDurationUnit.FormattingEnabled = true;
            this.cboDurationUnit.Items.AddRange(new object[] {
            "Minute(s)",
            "Hour(s)",
            "Day(s)",
            "Week(s)",
            "Month(s)"});
            this.cboDurationUnit.Location = new System.Drawing.Point(323, 52);
            this.cboDurationUnit.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.cboDurationUnit.Name = "cboDurationUnit";
            this.cboDurationUnit.Size = new System.Drawing.Size(121, 21);
            this.cboDurationUnit.TabIndex = 7;
            // 
            // chkRepeat
            // 
            this.chkRepeat.AutoSize = true;
            this.chkRepeat.Location = new System.Drawing.Point(201, 90);
            this.chkRepeat.Margin = new System.Windows.Forms.Padding(5);
            this.chkRepeat.Name = "chkRepeat";
            this.chkRepeat.Size = new System.Drawing.Size(61, 17);
            this.chkRepeat.TabIndex = 8;
            this.chkRepeat.Text = "Repeat";
            this.chkRepeat.UseVisualStyleBackColor = true;
            // 
            // lblRepeatEvery
            // 
            this.lblRepeatEvery.AutoSize = true;
            this.lblRepeatEvery.Location = new System.Drawing.Point(257, 91);
            this.lblRepeatEvery.Name = "lblRepeatEvery";
            this.lblRepeatEvery.Size = new System.Drawing.Size(33, 13);
            this.lblRepeatEvery.TabIndex = 9;
            this.lblRepeatEvery.Text = "every";
            // 
            // cboRepeatUnit
            // 
            this.cboRepeatUnit.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboRepeatUnit.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboRepeatUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRepeatUnit.FormattingEnabled = true;
            this.cboRepeatUnit.Items.AddRange(new object[] {
            "Minute(s)",
            "Hour(s)",
            "Day(s)",
            "Week(s)",
            "Month(s)"});
            this.cboRepeatUnit.Location = new System.Drawing.Point(358, 88);
            this.cboRepeatUnit.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.cboRepeatUnit.Name = "cboRepeatUnit";
            this.cboRepeatUnit.Size = new System.Drawing.Size(121, 21);
            this.cboRepeatUnit.TabIndex = 11;
            // 
            // numRepeat
            // 
            this.numRepeat.Location = new System.Drawing.Point(295, 89);
            this.numRepeat.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.numRepeat.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.numRepeat.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numRepeat.Name = "numRepeat";
            this.numRepeat.Size = new System.Drawing.Size(57, 20);
            this.numRepeat.TabIndex = 10;
            this.numRepeat.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 392);
            this.Controls.Add(this.cboRepeatUnit);
            this.Controls.Add(this.numRepeat);
            this.Controls.Add(this.lblRepeatEvery);
            this.Controls.Add(this.chkRepeat);
            this.Controls.Add(this.cboDurationUnit);
            this.Controls.Add(this.numDuration);
            this.Controls.Add(this.lblDuration);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.dtpStartTime);
            this.Controls.Add(this.btnAddTask);
            this.Controls.Add(this.lstTasks);
            this.Controls.Add(this.txtNewTask);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRepeat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNewTask;
        private System.Windows.Forms.ListBox lstTasks;
        private System.Windows.Forms.Button btnAddTask;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.ComboBox cboDurationUnit;
        private System.Windows.Forms.CheckBox chkRepeat;
        private System.Windows.Forms.Label lblRepeatEvery;
        private System.Windows.Forms.ComboBox cboRepeatUnit;
        private System.Windows.Forms.NumericUpDown numRepeat;
    }
}


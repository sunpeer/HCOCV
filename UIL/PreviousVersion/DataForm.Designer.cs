namespace OCV
{
    partial class DataForm
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
            this.batteryDataGridView = new System.Windows.Forms.DataGridView();
            this.logSearchBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.logTypeLabel = new System.Windows.Forms.Label();
            this.localData = new System.Windows.Forms.TabControl();
            this.log = new System.Windows.Forms.TabPage();
            this.battery = new System.Windows.Forms.TabPage();
            this.logDataGridView = new System.Windows.Forms.DataGridView();
            this.logTypeComboBox = new System.Windows.Forms.ComboBox();
            this.batterySearchBtn = new System.Windows.Forms.Button();
            this.startDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.endDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.logRefresh = new System.Windows.Forms.Button();
            this.batteryRefresh = new System.Windows.Forms.Button();
            this.snLabel = new System.Windows.Forms.Label();
            this.snTextBox = new System.Windows.Forms.TextBox();
            this.okRradioButton = new System.Windows.Forms.RadioButton();
            this.resultGroupBox = new System.Windows.Forms.GroupBox();
            this.ngRadionButton = new System.Windows.Forms.RadioButton();
            this.errtypeComboBox = new System.Windows.Forms.ComboBox();
            this.errtypeLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.sendResultGroupBox = new System.Windows.Forms.GroupBox();
            this.sendFailRadioButton = new System.Windows.Forms.RadioButton();
            this.sendSuccessRadioButton = new System.Windows.Forms.RadioButton();
            this.verifiedGroupBox = new System.Windows.Forms.GroupBox();
            this.varifiedNGRadioButton = new System.Windows.Forms.RadioButton();
            this.verifiedOKradioButton = new System.Windows.Forms.RadioButton();
            this.verifiedComboBox = new System.Windows.Forms.ComboBox();
            this.sendResultComboBox = new System.Windows.Forms.ComboBox();
            this.verifiedLabel = new System.Windows.Forms.Label();
            this.sendResultLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.batteryDataGridView)).BeginInit();
            this.localData.SuspendLayout();
            this.log.SuspendLayout();
            this.battery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).BeginInit();
            this.resultGroupBox.SuspendLayout();
            this.sendResultGroupBox.SuspendLayout();
            this.verifiedGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // batteryDataGridView
            // 
            this.batteryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.batteryDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.batteryDataGridView.Location = new System.Drawing.Point(3, 3);
            this.batteryDataGridView.Name = "batteryDataGridView";
            this.batteryDataGridView.RowTemplate.Height = 23;
            this.batteryDataGridView.Size = new System.Drawing.Size(703, 211);
            this.batteryDataGridView.TabIndex = 0;
            // 
            // logSearchBtn
            // 
            this.logSearchBtn.Location = new System.Drawing.Point(87, 511);
            this.logSearchBtn.Name = "logSearchBtn";
            this.logSearchBtn.Size = new System.Drawing.Size(75, 23);
            this.logSearchBtn.TabIndex = 1;
            this.logSearchBtn.Text = "查询";
            this.logSearchBtn.UseVisualStyleBackColor = true;
            this.logSearchBtn.Click += new System.EventHandler(this.logSearchBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 469);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "员工：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(217, 469);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "日期：";
            // 
            // userTextBox
            // 
            this.userTextBox.Location = new System.Drawing.Point(77, 466);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(100, 21);
            this.userTextBox.TabIndex = 9;
            // 
            // logTypeLabel
            // 
            this.logTypeLabel.AutoSize = true;
            this.logTypeLabel.Location = new System.Drawing.Point(523, 475);
            this.logTypeLabel.Name = "logTypeLabel";
            this.logTypeLabel.Size = new System.Drawing.Size(65, 12);
            this.logTypeLabel.TabIndex = 13;
            this.logTypeLabel.Text = "日志分类：";
            // 
            // localData
            // 
            this.localData.Controls.Add(this.log);
            this.localData.Controls.Add(this.battery);
            this.localData.Location = new System.Drawing.Point(32, 3);
            this.localData.Name = "localData";
            this.localData.SelectedIndex = 0;
            this.localData.Size = new System.Drawing.Size(717, 243);
            this.localData.TabIndex = 15;
            // 
            // log
            // 
            this.log.Controls.Add(this.batteryDataGridView);
            this.log.Location = new System.Drawing.Point(4, 22);
            this.log.Name = "log";
            this.log.Padding = new System.Windows.Forms.Padding(3);
            this.log.Size = new System.Drawing.Size(709, 217);
            this.log.TabIndex = 0;
            this.log.Text = "日志";
            this.log.UseVisualStyleBackColor = true;
            this.log.Enter += new System.EventHandler(this.log_Enter);
            // 
            // battery
            // 
            this.battery.Controls.Add(this.logDataGridView);
            this.battery.Location = new System.Drawing.Point(4, 22);
            this.battery.Name = "battery";
            this.battery.Padding = new System.Windows.Forms.Padding(3);
            this.battery.Size = new System.Drawing.Size(709, 217);
            this.battery.TabIndex = 1;
            this.battery.Text = "电池信息";
            this.battery.UseVisualStyleBackColor = true;
            this.battery.Enter += new System.EventHandler(this.battery_Enter);
            // 
            // logDataGridView
            // 
            this.logDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logDataGridView.Location = new System.Drawing.Point(3, 3);
            this.logDataGridView.Name = "logDataGridView";
            this.logDataGridView.RowTemplate.Height = 23;
            this.logDataGridView.Size = new System.Drawing.Size(703, 211);
            this.logDataGridView.TabIndex = 0;
            // 
            // logTypeComboBox
            // 
            this.logTypeComboBox.FormattingEnabled = true;
            this.logTypeComboBox.Items.AddRange(new object[] {
            "启动",
            "停机",
            "调零",
            "获取首件标准",
            "来件检验",
            "员工登录",
            "不限"});
            this.logTypeComboBox.Location = new System.Drawing.Point(593, 474);
            this.logTypeComboBox.Name = "logTypeComboBox";
            this.logTypeComboBox.Size = new System.Drawing.Size(121, 20);
            this.logTypeComboBox.TabIndex = 16;
            // 
            // batterySearchBtn
            // 
            this.batterySearchBtn.Location = new System.Drawing.Point(87, 511);
            this.batterySearchBtn.Name = "batterySearchBtn";
            this.batterySearchBtn.Size = new System.Drawing.Size(75, 23);
            this.batterySearchBtn.TabIndex = 17;
            this.batterySearchBtn.Text = "查询";
            this.batterySearchBtn.UseVisualStyleBackColor = true;
            this.batterySearchBtn.Click += new System.EventHandler(this.batterySearchBtn_Click);
            // 
            // startDateTimePicker
            // 
            this.startDateTimePicker.Location = new System.Drawing.Point(264, 463);
            this.startDateTimePicker.Name = "startDateTimePicker";
            this.startDateTimePicker.Size = new System.Drawing.Size(200, 21);
            this.startDateTimePicker.TabIndex = 18;
            // 
            // endDateTimePicker
            // 
            this.endDateTimePicker.Location = new System.Drawing.Point(264, 500);
            this.endDateTimePicker.Name = "endDateTimePicker";
            this.endDateTimePicker.Size = new System.Drawing.Size(200, 21);
            this.endDateTimePicker.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(262, 487);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "——";
            // 
            // logRefresh
            // 
            this.logRefresh.Location = new System.Drawing.Point(183, 342);
            this.logRefresh.Name = "logRefresh";
            this.logRefresh.Size = new System.Drawing.Size(75, 23);
            this.logRefresh.TabIndex = 22;
            this.logRefresh.Text = "刷新";
            this.logRefresh.UseVisualStyleBackColor = true;
            this.logRefresh.Click += new System.EventHandler(this.logRefresh_Click);
            // 
            // batteryRefresh
            // 
            this.batteryRefresh.Location = new System.Drawing.Point(183, 342);
            this.batteryRefresh.Name = "batteryRefresh";
            this.batteryRefresh.Size = new System.Drawing.Size(75, 23);
            this.batteryRefresh.TabIndex = 23;
            this.batteryRefresh.Text = "刷新";
            this.batteryRefresh.UseVisualStyleBackColor = true;
            this.batteryRefresh.Click += new System.EventHandler(this.batteryRefresh_Click);
            // 
            // snLabel
            // 
            this.snLabel.AutoSize = true;
            this.snLabel.Location = new System.Drawing.Point(104, 416);
            this.snLabel.Name = "snLabel";
            this.snLabel.Size = new System.Drawing.Size(53, 12);
            this.snLabel.TabIndex = 24;
            this.snLabel.Text = "序列号：";
            // 
            // snTextBox
            // 
            this.snTextBox.Location = new System.Drawing.Point(158, 413);
            this.snTextBox.Name = "snTextBox";
            this.snTextBox.Size = new System.Drawing.Size(100, 21);
            this.snTextBox.TabIndex = 25;
            // 
            // okRradioButton
            // 
            this.okRradioButton.AutoSize = true;
            this.okRradioButton.Location = new System.Drawing.Point(15, 20);
            this.okRradioButton.Name = "okRradioButton";
            this.okRradioButton.Size = new System.Drawing.Size(47, 16);
            this.okRradioButton.TabIndex = 26;
            this.okRradioButton.TabStop = true;
            this.okRradioButton.Text = "合格";
            this.okRradioButton.UseVisualStyleBackColor = true;
            // 
            // resultGroupBox
            // 
            this.resultGroupBox.Controls.Add(this.ngRadionButton);
            this.resultGroupBox.Controls.Add(this.okRradioButton);
            this.resultGroupBox.Location = new System.Drawing.Point(549, 252);
            this.resultGroupBox.Name = "resultGroupBox";
            this.resultGroupBox.Size = new System.Drawing.Size(200, 100);
            this.resultGroupBox.TabIndex = 27;
            this.resultGroupBox.TabStop = false;
            this.resultGroupBox.Text = "结果";
            // 
            // ngRadionButton
            // 
            this.ngRadionButton.AutoSize = true;
            this.ngRadionButton.Location = new System.Drawing.Point(68, 20);
            this.ngRadionButton.Name = "ngRadionButton";
            this.ngRadionButton.Size = new System.Drawing.Size(59, 16);
            this.ngRadionButton.TabIndex = 27;
            this.ngRadionButton.TabStop = true;
            this.ngRadionButton.Text = "不合格";
            this.ngRadionButton.UseVisualStyleBackColor = true;
            // 
            // errtypeComboBox
            // 
            this.errtypeComboBox.FormattingEnabled = true;
            this.errtypeComboBox.Items.AddRange(new object[] {
            "不限",
            "正常",
            "低于正常电阻值",
            "大于正常电阻值",
            "低于正常电压值",
            "大于正常电压值",
            "低于正常电压值",
            "低于正常电阻值",
            "低于正常电压值",
            "大于正常正常电阻值",
            "大于正常电压值",
            "低于正常电阻值",
            "大于正常电压值",
            "大于正常电阻值"});
            this.errtypeComboBox.Location = new System.Drawing.Point(183, 287);
            this.errtypeComboBox.Name = "errtypeComboBox";
            this.errtypeComboBox.Size = new System.Drawing.Size(121, 20);
            this.errtypeComboBox.TabIndex = 28;
            // 
            // errtypeLabel
            // 
            this.errtypeLabel.AutoSize = true;
            this.errtypeLabel.Location = new System.Drawing.Point(112, 287);
            this.errtypeLabel.Name = "errtypeLabel";
            this.errtypeLabel.Size = new System.Drawing.Size(65, 12);
            this.errtypeLabel.TabIndex = 29;
            this.errtypeLabel.Text = "错误类型：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(565, 336);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 12);
            this.label6.TabIndex = 28;
            // 
            // sendResultGroupBox
            // 
            this.sendResultGroupBox.Controls.Add(this.sendFailRadioButton);
            this.sendResultGroupBox.Controls.Add(this.sendSuccessRadioButton);
            this.sendResultGroupBox.Location = new System.Drawing.Point(564, 372);
            this.sendResultGroupBox.Name = "sendResultGroupBox";
            this.sendResultGroupBox.Size = new System.Drawing.Size(200, 56);
            this.sendResultGroupBox.TabIndex = 30;
            this.sendResultGroupBox.TabStop = false;
            this.sendResultGroupBox.Text = "上传结果";
            // 
            // sendFailRadioButton
            // 
            this.sendFailRadioButton.AutoSize = true;
            this.sendFailRadioButton.Location = new System.Drawing.Point(109, 22);
            this.sendFailRadioButton.Name = "sendFailRadioButton";
            this.sendFailRadioButton.Size = new System.Drawing.Size(71, 16);
            this.sendFailRadioButton.TabIndex = 1;
            this.sendFailRadioButton.TabStop = true;
            this.sendFailRadioButton.Text = "上传失败";
            this.sendFailRadioButton.UseVisualStyleBackColor = true;
            // 
            // sendSuccessRadioButton
            // 
            this.sendSuccessRadioButton.AutoSize = true;
            this.sendSuccessRadioButton.Location = new System.Drawing.Point(7, 23);
            this.sendSuccessRadioButton.Name = "sendSuccessRadioButton";
            this.sendSuccessRadioButton.Size = new System.Drawing.Size(71, 16);
            this.sendSuccessRadioButton.TabIndex = 0;
            this.sendSuccessRadioButton.TabStop = true;
            this.sendSuccessRadioButton.Text = "上传成功";
            this.sendSuccessRadioButton.UseVisualStyleBackColor = true;
            // 
            // verifiedGroupBox
            // 
            this.verifiedGroupBox.Controls.Add(this.varifiedNGRadioButton);
            this.verifiedGroupBox.Controls.Add(this.verifiedOKradioButton);
            this.verifiedGroupBox.Location = new System.Drawing.Point(344, 262);
            this.verifiedGroupBox.Name = "verifiedGroupBox";
            this.verifiedGroupBox.Size = new System.Drawing.Size(199, 52);
            this.verifiedGroupBox.TabIndex = 31;
            this.verifiedGroupBox.TabStop = false;
            this.verifiedGroupBox.Text = "来件校验";
            // 
            // varifiedNGRadioButton
            // 
            this.varifiedNGRadioButton.AutoSize = true;
            this.varifiedNGRadioButton.Location = new System.Drawing.Point(126, 21);
            this.varifiedNGRadioButton.Name = "varifiedNGRadioButton";
            this.varifiedNGRadioButton.Size = new System.Drawing.Size(47, 16);
            this.varifiedNGRadioButton.TabIndex = 1;
            this.varifiedNGRadioButton.TabStop = true;
            this.varifiedNGRadioButton.Text = "错误";
            this.varifiedNGRadioButton.UseVisualStyleBackColor = true;
            // 
            // verifiedOKradioButton
            // 
            this.verifiedOKradioButton.AutoSize = true;
            this.verifiedOKradioButton.Location = new System.Drawing.Point(16, 21);
            this.verifiedOKradioButton.Name = "verifiedOKradioButton";
            this.verifiedOKradioButton.Size = new System.Drawing.Size(47, 16);
            this.verifiedOKradioButton.TabIndex = 0;
            this.verifiedOKradioButton.TabStop = true;
            this.verifiedOKradioButton.Text = "正确";
            this.verifiedOKradioButton.UseVisualStyleBackColor = true;
            // 
            // verifiedComboBox
            // 
            this.verifiedComboBox.FormattingEnabled = true;
            this.verifiedComboBox.Items.AddRange(new object[] {
            "不限",
            "校验成功",
            "校验失败"});
            this.verifiedComboBox.Location = new System.Drawing.Point(391, 387);
            this.verifiedComboBox.Name = "verifiedComboBox";
            this.verifiedComboBox.Size = new System.Drawing.Size(121, 20);
            this.verifiedComboBox.TabIndex = 32;
            // 
            // sendResultComboBox
            // 
            this.sendResultComboBox.FormattingEnabled = true;
            this.sendResultComboBox.Items.AddRange(new object[] {
            "不限",
            "上传失败",
            "上传成功"});
            this.sendResultComboBox.Location = new System.Drawing.Point(391, 428);
            this.sendResultComboBox.Name = "sendResultComboBox";
            this.sendResultComboBox.Size = new System.Drawing.Size(121, 20);
            this.sendResultComboBox.TabIndex = 33;
            // 
            // verifiedLabel
            // 
            this.verifiedLabel.AutoSize = true;
            this.verifiedLabel.Location = new System.Drawing.Point(300, 390);
            this.verifiedLabel.Name = "verifiedLabel";
            this.verifiedLabel.Size = new System.Drawing.Size(89, 12);
            this.verifiedLabel.TabIndex = 34;
            this.verifiedLabel.Text = "来件校验结果：";
            // 
            // sendResultLabel
            // 
            this.sendResultLabel.AutoSize = true;
            this.sendResultLabel.Location = new System.Drawing.Point(317, 432);
            this.sendResultLabel.Name = "sendResultLabel";
            this.sendResultLabel.Size = new System.Drawing.Size(65, 12);
            this.sendResultLabel.TabIndex = 35;
            this.sendResultLabel.Text = "上传结果：";
            // 
            // DataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.sendResultLabel);
            this.Controls.Add(this.verifiedLabel);
            this.Controls.Add(this.sendResultComboBox);
            this.Controls.Add(this.verifiedComboBox);
            this.Controls.Add(this.errtypeLabel);
            this.Controls.Add(this.verifiedGroupBox);
            this.Controls.Add(this.errtypeComboBox);
            this.Controls.Add(this.sendResultGroupBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.resultGroupBox);
            this.Controls.Add(this.snTextBox);
            this.Controls.Add(this.snLabel);
            this.Controls.Add(this.batteryRefresh);
            this.Controls.Add(this.logRefresh);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.endDateTimePicker);
            this.Controls.Add(this.startDateTimePicker);
            this.Controls.Add(this.batterySearchBtn);
            this.Controls.Add(this.logTypeComboBox);
            this.Controls.Add(this.localData);
            this.Controls.Add(this.logTypeLabel);
            this.Controls.Add(this.userTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logSearchBtn);
            this.Name = "DataForm";
            this.Load += new System.EventHandler(this.DataForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.batteryDataGridView)).EndInit();
            this.localData.ResumeLayout(false);
            this.log.ResumeLayout(false);
            this.battery.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).EndInit();
            this.resultGroupBox.ResumeLayout(false);
            this.resultGroupBox.PerformLayout();
            this.sendResultGroupBox.ResumeLayout(false);
            this.sendResultGroupBox.PerformLayout();
            this.verifiedGroupBox.ResumeLayout(false);
            this.verifiedGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView batteryDataGridView;
        private System.Windows.Forms.Button logSearchBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.Label logTypeLabel;
        private System.Windows.Forms.TabControl localData;
        private System.Windows.Forms.TabPage log;
        private System.Windows.Forms.TabPage battery;
        private System.Windows.Forms.ComboBox logTypeComboBox;
        private System.Windows.Forms.DataGridView logDataGridView;
        private System.Windows.Forms.Button batterySearchBtn;
        private System.Windows.Forms.DateTimePicker startDateTimePicker;
        private System.Windows.Forms.DateTimePicker endDateTimePicker;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button logRefresh;
        private System.Windows.Forms.Button batteryRefresh;
        private System.Windows.Forms.Label snLabel;
        private System.Windows.Forms.TextBox snTextBox;
        private System.Windows.Forms.RadioButton okRradioButton;
        private System.Windows.Forms.GroupBox resultGroupBox;
        private System.Windows.Forms.RadioButton ngRadionButton;
        private System.Windows.Forms.ComboBox errtypeComboBox;
        private System.Windows.Forms.Label errtypeLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox sendResultGroupBox;
        private System.Windows.Forms.RadioButton sendFailRadioButton;
        private System.Windows.Forms.RadioButton sendSuccessRadioButton;
        private System.Windows.Forms.GroupBox verifiedGroupBox;
        private System.Windows.Forms.RadioButton varifiedNGRadioButton;
        private System.Windows.Forms.RadioButton verifiedOKradioButton;
        private System.Windows.Forms.ComboBox verifiedComboBox;
        private System.Windows.Forms.ComboBox sendResultComboBox;
        private System.Windows.Forms.Label verifiedLabel;
        private System.Windows.Forms.Label sendResultLabel;
    }
}
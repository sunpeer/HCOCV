namespace OCV
{
    partial class SystemForm
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
            this.batteryNum2Rbtn = new System.Windows.Forms.RadioButton();
            this.batteryNum4Rbtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.adjButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label63 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.getTechStandardBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // batteryNum2Rbtn
            // 
            this.batteryNum2Rbtn.AutoSize = true;
            this.batteryNum2Rbtn.Location = new System.Drawing.Point(24, 29);
            this.batteryNum2Rbtn.Name = "batteryNum2Rbtn";
            this.batteryNum2Rbtn.Size = new System.Drawing.Size(65, 16);
            this.batteryNum2Rbtn.TabIndex = 1;
            this.batteryNum2Rbtn.TabStop = true;
            this.batteryNum2Rbtn.Text = "2块电池";
            this.batteryNum2Rbtn.UseVisualStyleBackColor = true;
            // 
            // batteryNum4Rbtn
            // 
            this.batteryNum4Rbtn.AutoSize = true;
            this.batteryNum4Rbtn.Location = new System.Drawing.Point(118, 29);
            this.batteryNum4Rbtn.Name = "batteryNum4Rbtn";
            this.batteryNum4Rbtn.Size = new System.Drawing.Size(65, 16);
            this.batteryNum4Rbtn.TabIndex = 2;
            this.batteryNum4Rbtn.TabStop = true;
            this.batteryNum4Rbtn.Text = "4块电池";
            this.batteryNum4Rbtn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.batteryNum4Rbtn);
            this.groupBox1.Controls.Add(this.saveBtn);
            this.groupBox1.Controls.Add(this.batteryNum2Rbtn);
            this.groupBox1.Location = new System.Drawing.Point(80, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 86);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一次检测电池数量";
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(64, 57);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 3;
            this.saveBtn.Text = "保存";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.adjButton);
            this.groupBox3.Location = new System.Drawing.Point(12, 283);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 88);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "电池检测仪调零";
            // 
            // adjButton
            // 
            this.adjButton.Location = new System.Drawing.Point(43, 33);
            this.adjButton.Name = "adjButton";
            this.adjButton.Size = new System.Drawing.Size(75, 23);
            this.adjButton.TabIndex = 0;
            this.adjButton.Text = "确定";
            this.adjButton.UseVisualStyleBackColor = true;
            this.adjButton.Click += new System.EventHandler(this.adjButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.getTechStandardBtn);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(64, 23);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 140);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "获取工艺标准";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(89, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label63
            // 
            this.label63.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label63.Location = new System.Drawing.Point(0, 80);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(250, 57);
            this.label63.TabIndex = 0;
            this.label63.Text = "执行获取首件工艺标准操作时，请放置一块电池到上层电池盒内，放置成功后，点确定按钮，系统自动运行获取工艺标准。";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "执行获取首件工艺标准操作时，请放置一块电池到上层电池盒内，放置成功后，点确定按钮，系统自动运行获取工艺标准。";
            // 
            // getTechStandardBtn
            // 
            this.getTechStandardBtn.Location = new System.Drawing.Point(89, 37);
            this.getTechStandardBtn.Name = "getTechStandardBtn";
            this.getTechStandardBtn.Size = new System.Drawing.Size(75, 23);
            this.getTechStandardBtn.TabIndex = 1;
            this.getTechStandardBtn.Text = "确定";
            this.getTechStandardBtn.UseVisualStyleBackColor = true;
            this.getTechStandardBtn.Click += new System.EventHandler(this.getTechStandardBtn_Click);
            // 
            // SystemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(379, 383);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SystemForm";
            this.Text = "系统设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SystemForm_FormClosing);
            this.Load += new System.EventHandler(this.SystemForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton batteryNum2Rbtn;
        private System.Windows.Forms.RadioButton batteryNum4Rbtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button adjButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button getTechStandardBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label63;
    }
}
namespace OCV
{
    partial class DeviceForm
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.comcombobox = new System.Windows.Forms.ComboBox();
            this.hokidisconbutton = new System.Windows.Forms.Button();
            this.hokiconbutton = new System.Windows.Forms.Button();
            this.baudratecombobox = new System.Windows.Forms.ComboBox();
            this.baudratelabel = new System.Windows.Forms.Label();
            this.comlabel = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ip_textbox4 = new System.Windows.Forms.TextBox();
            this.ip_textbox3 = new System.Windows.Forms.TextBox();
            this.ip_textbox2 = new System.Windows.Forms.TextBox();
            this.ip_textbox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.keyencedisconbutton = new System.Windows.Forms.Button();
            this.keyenceconbutton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.plcdisconbutton = new System.Windows.Forms.Button();
            this.plcconbutton = new System.Windows.Forms.Button();
            this.plcIDtextbox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.hokiStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.keyenceStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.statusStrip4 = new System.Windows.Forms.StatusStrip();
            this.PLCStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.statusStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Enabled = false;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.comcombobox);
            this.splitContainer2.Panel1.Controls.Add(this.hokidisconbutton);
            this.splitContainer2.Panel1.Controls.Add(this.hokiconbutton);
            this.splitContainer2.Panel1.Controls.Add(this.baudratecombobox);
            this.splitContainer2.Panel1.Controls.Add(this.baudratelabel);
            this.splitContainer2.Panel1.Controls.Add(this.comlabel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer2.Size = new System.Drawing.Size(266, 372);
            this.splitContainer2.SplitterDistance = 177;
            this.splitContainer2.TabIndex = 1;
            // 
            // comcombobox
            // 
            this.comcombobox.FormattingEnabled = true;
            this.comcombobox.Location = new System.Drawing.Point(94, 3);
            this.comcombobox.Name = "comcombobox";
            this.comcombobox.Size = new System.Drawing.Size(121, 20);
            this.comcombobox.TabIndex = 0;
            // 
            // hokidisconbutton
            // 
            this.hokidisconbutton.Location = new System.Drawing.Point(129, 77);
            this.hokidisconbutton.Name = "hokidisconbutton";
            this.hokidisconbutton.Size = new System.Drawing.Size(75, 23);
            this.hokidisconbutton.TabIndex = 5;
            this.hokidisconbutton.Text = "断开";
            this.hokidisconbutton.UseVisualStyleBackColor = true;
            this.hokidisconbutton.Click += new System.EventHandler(this.hokidisconbutton_Click);
            // 
            // hokiconbutton
            // 
            this.hokiconbutton.Location = new System.Drawing.Point(27, 77);
            this.hokiconbutton.Name = "hokiconbutton";
            this.hokiconbutton.Size = new System.Drawing.Size(75, 23);
            this.hokiconbutton.TabIndex = 4;
            this.hokiconbutton.Text = "连接";
            this.hokiconbutton.UseVisualStyleBackColor = true;
            this.hokiconbutton.Click += new System.EventHandler(this.hokiconbutton_Click);
            // 
            // baudratecombobox
            // 
            this.baudratecombobox.FormattingEnabled = true;
            this.baudratecombobox.Location = new System.Drawing.Point(94, 38);
            this.baudratecombobox.Name = "baudratecombobox";
            this.baudratecombobox.Size = new System.Drawing.Size(121, 20);
            this.baudratecombobox.TabIndex = 3;
            // 
            // baudratelabel
            // 
            this.baudratelabel.AutoSize = true;
            this.baudratelabel.Location = new System.Drawing.Point(27, 38);
            this.baudratelabel.Name = "baudratelabel";
            this.baudratelabel.Size = new System.Drawing.Size(53, 12);
            this.baudratelabel.TabIndex = 2;
            this.baudratelabel.Text = "波特率：";
            // 
            // comlabel
            // 
            this.comlabel.AutoSize = true;
            this.comlabel.Location = new System.Drawing.Point(25, 6);
            this.comlabel.Name = "comlabel";
            this.comlabel.Size = new System.Drawing.Size(47, 12);
            this.comlabel.TabIndex = 1;
            this.comlabel.Text = "COM口：";
            // 
            // groupBox3
            // 
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(266, 191);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "bt3562参数设置";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Enabled = false;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.label4);
            this.splitContainer4.Panel1.Controls.Add(this.label3);
            this.splitContainer4.Panel1.Controls.Add(this.label2);
            this.splitContainer4.Panel1.Controls.Add(this.ip_textbox4);
            this.splitContainer4.Panel1.Controls.Add(this.ip_textbox3);
            this.splitContainer4.Panel1.Controls.Add(this.ip_textbox2);
            this.splitContainer4.Panel1.Controls.Add(this.ip_textbox1);
            this.splitContainer4.Panel1.Controls.Add(this.label1);
            this.splitContainer4.Panel1.Controls.Add(this.keyencedisconbutton);
            this.splitContainer4.Panel1.Controls.Add(this.keyenceconbutton);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer4.Size = new System.Drawing.Size(266, 372);
            this.splitContainer4.SplitterDistance = 175;
            this.splitContainer4.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(220, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = ".";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(178, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = ".";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(137, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = ".";
            // 
            // ip_textbox4
            // 
            this.ip_textbox4.Location = new System.Drawing.Point(237, 12);
            this.ip_textbox4.MaxLength = 3;
            this.ip_textbox4.Name = "ip_textbox4";
            this.ip_textbox4.Size = new System.Drawing.Size(29, 21);
            this.ip_textbox4.TabIndex = 8;
            // 
            // ip_textbox3
            // 
            this.ip_textbox3.Location = new System.Drawing.Point(193, 12);
            this.ip_textbox3.MaxLength = 3;
            this.ip_textbox3.Name = "ip_textbox3";
            this.ip_textbox3.Size = new System.Drawing.Size(25, 21);
            this.ip_textbox3.TabIndex = 7;
            // 
            // ip_textbox2
            // 
            this.ip_textbox2.Location = new System.Drawing.Point(154, 13);
            this.ip_textbox2.MaxLength = 3;
            this.ip_textbox2.Name = "ip_textbox2";
            this.ip_textbox2.Size = new System.Drawing.Size(22, 21);
            this.ip_textbox2.TabIndex = 6;
            // 
            // ip_textbox1
            // 
            this.ip_textbox1.Location = new System.Drawing.Point(109, 14);
            this.ip_textbox1.MaxLength = 3;
            this.ip_textbox1.Name = "ip_textbox1";
            this.ip_textbox1.Size = new System.Drawing.Size(27, 21);
            this.ip_textbox1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "扫码枪IP地址：";
            // 
            // keyencedisconbutton
            // 
            this.keyencedisconbutton.Location = new System.Drawing.Point(128, 74);
            this.keyencedisconbutton.Name = "keyencedisconbutton";
            this.keyencedisconbutton.Size = new System.Drawing.Size(75, 23);
            this.keyencedisconbutton.TabIndex = 1;
            this.keyencedisconbutton.Text = "断开";
            this.keyencedisconbutton.UseVisualStyleBackColor = true;
            this.keyencedisconbutton.Click += new System.EventHandler(this.keyencedisconbutton_Click);
            // 
            // keyenceconbutton
            // 
            this.keyenceconbutton.Location = new System.Drawing.Point(23, 74);
            this.keyenceconbutton.Name = "keyenceconbutton";
            this.keyenceconbutton.Size = new System.Drawing.Size(75, 23);
            this.keyenceconbutton.TabIndex = 0;
            this.keyenceconbutton.Text = "连接";
            this.keyenceconbutton.UseVisualStyleBackColor = true;
            this.keyenceconbutton.Click += new System.EventHandler(this.keyenceconbutton_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(266, 193);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "SR-2000W参数设置";
            // 
            // plcdisconbutton
            // 
            this.plcdisconbutton.Location = new System.Drawing.Point(143, 62);
            this.plcdisconbutton.Name = "plcdisconbutton";
            this.plcdisconbutton.Size = new System.Drawing.Size(75, 23);
            this.plcdisconbutton.TabIndex = 5;
            this.plcdisconbutton.Text = "断开";
            this.plcdisconbutton.UseVisualStyleBackColor = true;
            this.plcdisconbutton.Click += new System.EventHandler(this.plcdisconbutton_Click);
            // 
            // plcconbutton
            // 
            this.plcconbutton.Location = new System.Drawing.Point(21, 62);
            this.plcconbutton.Name = "plcconbutton";
            this.plcconbutton.Size = new System.Drawing.Size(75, 23);
            this.plcconbutton.TabIndex = 4;
            this.plcconbutton.Text = "连接";
            this.plcconbutton.UseVisualStyleBackColor = true;
            this.plcconbutton.Click += new System.EventHandler(this.plcconbutton_Click);
            // 
            // plcIDtextbox
            // 
            this.plcIDtextbox.Location = new System.Drawing.Point(128, 14);
            this.plcIDtextbox.Name = "plcIDtextbox";
            this.plcIDtextbox.Size = new System.Drawing.Size(100, 21);
            this.plcIDtextbox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "PLC编号";
            // 
            // groupBox8
            // 
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(0, 0);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(266, 251);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "fx5参数设置";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Enabled = false;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.plcdisconbutton);
            this.splitContainer1.Panel1.Controls.Add(this.plcIDtextbox);
            this.splitContainer1.Panel1.Controls.Add(this.plcconbutton);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox8);
            this.splitContainer1.Size = new System.Drawing.Size(266, 372);
            this.splitContainer1.SplitterDistance = 117;
            this.splitContainer1.TabIndex = 1;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(280, 426);
            this.tabControl2.TabIndex = 4;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.Controls.Add(this.statusStrip1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(272, 400);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "电池检测仪";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hokiStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(3, 375);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(266, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // hokiStatusLabel
            // 
            this.hokiStatusLabel.Name = "hokiStatusLabel";
            this.hokiStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer4);
            this.tabPage5.Controls.Add(this.statusStrip2);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(272, 400);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "扫码枪";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyenceStatusLabel});
            this.statusStrip2.Location = new System.Drawing.Point(3, 375);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(266, 22);
            this.statusStrip2.TabIndex = 0;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // keyenceStatusLabel
            // 
            this.keyenceStatusLabel.Name = "keyenceStatusLabel";
            this.keyenceStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.splitContainer1);
            this.tabPage6.Controls.Add(this.statusStrip4);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(272, 400);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "PLC";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // statusStrip4
            // 
            this.statusStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PLCStatusLabel});
            this.statusStrip4.Location = new System.Drawing.Point(3, 375);
            this.statusStrip4.Name = "statusStrip4";
            this.statusStrip4.Size = new System.Drawing.Size(266, 22);
            this.statusStrip4.TabIndex = 0;
            this.statusStrip4.Text = "statusStrip4";
            // 
            // PLCStatusLabel
            // 
            this.PLCStatusLabel.Name = "PLCStatusLabel";
            this.PLCStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // DeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(280, 426);
            this.Controls.Add(this.tabControl2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceForm";
            this.Text = "设备管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeviceForm_FormClosing);
            this.Load += new System.EventHandler(this.deviceform_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.statusStrip4.ResumeLayout(false);
            this.statusStrip4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button hokidisconbutton;
        private System.Windows.Forms.Button hokiconbutton;
        private System.Windows.Forms.ComboBox baudratecombobox;
        private System.Windows.Forms.Label baudratelabel;
        private System.Windows.Forms.Label comlabel;
        private System.Windows.Forms.ComboBox comcombobox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button keyencedisconbutton;
        private System.Windows.Forms.Button keyenceconbutton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ip_textbox4;
        private System.Windows.Forms.TextBox ip_textbox3;
        private System.Windows.Forms.TextBox ip_textbox2;
        private System.Windows.Forms.TextBox ip_textbox1;
        private System.Windows.Forms.TextBox plcIDtextbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button plcdisconbutton;
        private System.Windows.Forms.Button plcconbutton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.StatusStrip statusStrip4;
        private System.Windows.Forms.ToolStripStatusLabel keyenceStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel PLCStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel hokiStatusLabel;
    }
}
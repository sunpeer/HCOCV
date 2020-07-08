namespace OCV
{
    partial class UserForm
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.loginstatelabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.logingroupbox = new System.Windows.Forms.GroupBox();
            this.loginbutton = new System.Windows.Forms.Button();
            this.pwdtextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numtextbox = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.logingroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginstatelabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 141);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(322, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "登录状态栏";
            // 
            // loginstatelabel
            // 
            this.loginstatelabel.Name = "loginstatelabel";
            this.loginstatelabel.Size = new System.Drawing.Size(0, 17);
            // 
            // logingroupbox
            // 
            this.logingroupbox.Controls.Add(this.loginbutton);
            this.logingroupbox.Controls.Add(this.pwdtextbox);
            this.logingroupbox.Controls.Add(this.label2);
            this.logingroupbox.Controls.Add(this.label1);
            this.logingroupbox.Controls.Add(this.numtextbox);
            this.logingroupbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logingroupbox.Location = new System.Drawing.Point(0, 0);
            this.logingroupbox.Name = "logingroupbox";
            this.logingroupbox.Size = new System.Drawing.Size(322, 141);
            this.logingroupbox.TabIndex = 0;
            this.logingroupbox.TabStop = false;
            this.logingroupbox.Text = "登录";
            // 
            // loginbutton
            // 
            this.loginbutton.Location = new System.Drawing.Point(114, 101);
            this.loginbutton.Name = "loginbutton";
            this.loginbutton.Size = new System.Drawing.Size(75, 23);
            this.loginbutton.TabIndex = 4;
            this.loginbutton.Text = "登录";
            this.loginbutton.UseVisualStyleBackColor = true;
            this.loginbutton.Click += new System.EventHandler(this.loginbutton_Click);
            // 
            // pwdtextbox
            // 
            this.pwdtextbox.Location = new System.Drawing.Point(136, 63);
            this.pwdtextbox.Name = "pwdtextbox";
            this.pwdtextbox.Size = new System.Drawing.Size(100, 21);
            this.pwdtextbox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "工号：";
            // 
            // numtextbox
            // 
            this.numtextbox.Location = new System.Drawing.Point(136, 30);
            this.numtextbox.Name = "numtextbox";
            this.numtextbox.Size = new System.Drawing.Size(100, 21);
            this.numtextbox.TabIndex = 0;
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(322, 163);
            this.Controls.Add(this.logingroupbox);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserForm";
            this.Text = "用户管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserForm_FormClosing);
            this.Load += new System.EventHandler(this.userform_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.logingroupbox.ResumeLayout(false);
            this.logingroupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel loginstatelabel;
        private System.Windows.Forms.GroupBox logingroupbox;
        private System.Windows.Forms.Button loginbutton;
        private System.Windows.Forms.TextBox pwdtextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox numtextbox;
    }
}
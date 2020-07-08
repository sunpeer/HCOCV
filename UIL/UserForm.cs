using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using BLL;

namespace UIL
{
    public partial class UserForm : Form
    {
        public UserForm(App softState)
        {
            InitializeComponent();
            this.softState = softState;
        }
        App softState = null;
        public Action<bool> SetCurUserEvent;
        private void userform_Load(object sender, EventArgs e)
        {
            loginstatelabel.Text = softState.curUser.IsLoggined?(softState.curUser.Id+"已登录"):"未登录";
        }

        private void LoginDown(bool flag,string re)
        {
            //登录成功
            if(flag)
            {
                loginstatelabel.Text = "员工：" + numtextbox.Text + "已登录";
                SetCurUserEvent?.Invoke(true);
            }
            else//失败
            {
                MessageBox.Show(re, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Invoke(new Action(() =>
            {
                pwdtextbox.Clear();
                numtextbox.Clear();
            }));

        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            string UserId = numtextbox.Text;
            //string UserPwd = pwdtextbox.Text;
            softState.Login(UserId, new Action<bool, string>(LoginDown));
        }

        private void UserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void UserForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}

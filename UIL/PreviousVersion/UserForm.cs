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
using MySql.Data.MySqlClient;

namespace OCV
{
    public partial class UserForm : Form
    {
        public UserForm(SoftState softState)
        {
            InitializeComponent();
            this.softState = softState;
        }
        SoftState softState = null;
        public Action<User> SetCurUserEvent;
        private void userform_Load(object sender, EventArgs e)
        {
            loginstatelabel.Text = "未登录";
        }

        private void loginbutton_Click(object sender, EventArgs e)
        {
            User user = new User(numtextbox.Text, pwdtextbox.Text);
            if (Mes.Login(numtextbox.Text, pwdtextbox.Text))
            {
                loginstatelabel.Text = "员工：" + numtextbox.Text + "已登录";
                pwdtextbox.Clear();
                numtextbox.Clear();
                SetCurUserEvent.Invoke(user);
                softState.CreateUserLoginLog();
            }
            else MessageBox.Show("登录失败，账号或密码输入错误","登录错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        private void UserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

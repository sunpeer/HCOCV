using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Configuration;
using System.IO;
using BLL;

namespace UIL
{
    public partial class DeviceForm : Form
    {
        public  DeviceForm(App app)
        {
            InitializeComponent();
            this.app = app;
        }
        App app;
        #region 窗口加载函数
        private void deviceform_Load(object sender, EventArgs e)
        {

            string[] coms = SerialPort.GetPortNames();
            if (coms.Length > 0)
            {
                foreach (string com in coms)
                {
                    comcombobox.Items.Add(com);
                    temPeratureCombox.Items.Add(com);
                }
            }
            if (app.bt3562.IsConnected)
            {
                //baudratecombobox.Text = app.curConfig.Bt3562Baudrate.ToString();
                //comcombobox.SelectedItem = app.curConfig.Bt3562Com;
                //comcombobox.Enabled = false;
                //baudratecombobox.Enabled = false;
                hokiconbutton.Enabled = false;
                hokiStatusLabel.Text="电池检测仪已连接";
            }
            else
            {
                //baudratecombobox.Text = app.curConfig.Bt3562Baudrate.ToString();
                //if (comcombobox.Items.Count > 0)
                //    comcombobox.SelectedIndex = 0;
                //hokidisconbutton.Enabled = false;
                hokiStatusLabel.Text = "电池检测仪未连接";
            }
            /*
             * 
             * 
             * 
             * 
             * 电池检测仪参数查询函数
             * 
             * */

            //ipstrdisplay(app.curConfig.Sr2000wIp);
            if (app.sr2000w.IsConnected)
            {
                //ip_textbox1.Enabled = false;
                //ip_textbox2.Enabled = false;
                //ip_textbox3.Enabled = false;
                //ip_textbox4.Enabled = false;
                keyenceconbutton.Enabled = false;
                keyenceStatusLabel.Text = "扫码枪已连接";
            }
            else
            {
                //keyencedisconbutton.Enabled = false;
                keyenceStatusLabel.Text = "扫码枪未连接";
            }


            //plcIDtextbox.Text = app.curConfig.Fx5uId.ToString();
            if (app.fx5u.IsConnected)
            {
                //plcIDtextbox.Enabled = false;
                plcconbutton.Enabled = false;
                PLCStatusLabel.Text = "PLC已连接";
            }
            else
            {
                //plcdisconbutton.Enabled = false;
                PLCStatusLabel.Text = "PLC未连接";
            }
            if (app.isTemperatureConnected)
                temperaturerConBtn.Enabled = false;
        }
        #endregion
        string Com;
        public Action<bool> HokiConEvent;
        #region 电池检测仪连接按钮动作函数
        private void hokiconbutton_Click(object sender, EventArgs e)
        {
            Com= comcombobox.Text;
            //string errMsg = "";
            //bool result=app.Connect(app.bt3562,Com,ref errMsg);
            app.ConnectBt3562(Com,new Action<string>(bt3562Test));
        }
        #endregion
        private void bt3562Test(string str)
        {
            if (str.StartsWith("HIOKI"))
            {
                app.bt3562.IsConnected = true;
                app.curConfig.Bt3562Com = Com;
                //连接正确
                Bt3562ConDownCallBack(true);
            }
        }


        private void Bt3562ConDownCallBack(bool flag)
        {
            if (flag)
            {
                HokiConEvent?.Invoke(true);
                this.Invoke(new Action(() =>
                {
                    hokiStatusLabel.Text = "电池检测仪已连接";
                }));
                
            }
        }

        public Action<bool> KeyenceConEvent;
        #region 扫码枪连接按钮动作函数
        private void keyenceconbutton_Click(object sender, EventArgs e)
        {
            string ip;
            try
            {
                ip = displaytoip();
            }
            catch
            {
                MessageBox.Show("输入不正确");
                return;
            }
            string errMsg = "";
            bool result = app.Connect(app.sr2000w,ip,ref errMsg);
            if (result)
            {
                Sr2000wCallBack(true);
                MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接失败，" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Sr2000wCallBack(bool obj)
        {
            if (obj)
            {
                this.Invoke(new Action(() =>
                {
                    keyenceconbutton.Enabled = false;
                    keyenceStatusLabel.Text = "扫码枪已连接";
                }));
                
                KeyenceConEvent?.Invoke(true);

            }
        }
        #endregion

        public Action<bool> PlcConEvent;
        #region PLC连接按钮动作函数
        private void plcconbutton_Click(object sender, EventArgs e)
        {
            string id= plcIDtextbox.Text;
            string errMsg = "";
            bool result = app.Connect(app.fx5u, id,ref errMsg);
            //连接成功
            if(result)
            {
                Fx5uConDownCallBack(true);
                MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接失败，" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Fx5uConDownCallBack(bool obj)
        {
            if(obj)
            {
                PlcConEvent?.Invoke(true);
                this.Invoke(new Action(() =>
                {
                    PLCStatusLabel.Text = "PLC已连接";
                    //plcIDtextbox.Enabled = false;
                    plcconbutton.Enabled = false;
                    //plcdisconbutton.Enabled = true;
                }));
            }
        }
        #endregion

        #region ip显示相关函数
        private void ipstrdisplay(string ipstr)
        {
            string[] ipparts=ipstr.Split('.');
            ip_textbox1.Text = ipparts[0];
            ip_textbox2.Text = ipparts[1];
            ip_textbox3.Text = ipparts[2];
            ip_textbox4.Text = ipparts[3];
        }
        private string displaytoip()
        {
            return ip_textbox1.Text + '.'+ip_textbox2.Text +'.'+ ip_textbox3.Text +'.'+ ip_textbox4.Text;
        }
        #endregion

        private void temperaturerConBtn_Click(object sender, EventArgs e)
        {
            string com = temPeratureCombox.Text;
            if (app.ConnectTemperature(com))
                MessageBox.Show("连接成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("连接失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DeviceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}

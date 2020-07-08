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

namespace OCV
{
    public partial class DeviceForm : Form
    {
        public  DeviceForm(HokiCon hokiCon,KeyenceCon keyenceCon,MitsubishiCon mitsubishiCon)
        {
            InitializeComponent();
            this.hokiCon = hokiCon;
            this.keyenceCon = keyenceCon;
            this.mitsubishiCon = mitsubishiCon;
        }
        HokiCon hokiCon = null;
        KeyenceCon keyenceCon = null;
        MitsubishiCon mitsubishiCon = null;
        #region 窗口加载函数
        private void deviceform_Load(object sender, EventArgs e)
        {
            foreach (int i in HokiCon.baudrates)
            {
                baudratecombobox.Items.Add(i);
            }
            string[] coms = SerialPort.GetPortNames();
            if (coms.Length > 0)
            {
                foreach (string com in coms)
                {
                    comcombobox.Items.Add(com);
                }
            }
            if (hokiCon.connected==true)
            {
                baudratecombobox.SelectedItem = int.Parse(ConfigurationManager.AppSettings["bt3562Baudrate"]);
                comcombobox.SelectedItem = ConfigurationManager.AppSettings["bt3562Com"];
                comcombobox.Enabled = false;
                baudratecombobox.Enabled = false;
                hokiconbutton.Enabled = false;
                hokiStatusLabel.Text="电池检测仪已连接";
            }
            else
            {
                baudratecombobox.SelectedItem = int.Parse(ConfigurationManager.AppSettings["bt3562Baudrate"]);
                if (comcombobox.Items.Count > 0)
                    comcombobox.SelectedIndex = 0;
                hokidisconbutton.Enabled = false;
                hokiStatusLabel.Text = "电池检测已未连接";
            }
            /*
             * 
             * 
             * 
             * 
             * 电池检测仪参数查询函数
             * 
             * */

            ipstrdisplay(ConfigurationManager.AppSettings["IP"]);
            if (keyenceCon.connected)
            {
                ip_textbox1.Enabled = false;
                ip_textbox2.Enabled = false;
                ip_textbox3.Enabled = false;
                ip_textbox4.Enabled = false;
                keyenceconbutton.Enabled = false;
                keyenceStatusLabel.Text = "扫码枪已连接";
            }
            else
            {
                keyencedisconbutton.Enabled = false;
                keyenceStatusLabel.Text = "扫码枪未连接";
            }


            plcIDtextbox.Text = ConfigurationManager.AppSettings["PLCID"];
            if (mitsubishiCon.connected & mitsubishiCon.entried)
            {
                plcIDtextbox.Enabled = false;
                plcconbutton.Enabled = false;
                PLCStatusLabel.Text = "PLC已连接";
            }
            else
            {
                plcdisconbutton.Enabled = false;
                PLCStatusLabel.Text = "PLC未连接";
            }
            
        }
        #endregion

        public Action<bool> HokiConEvent;
        #region 电池检测仪连接按钮动作函数
        private void hokiconbutton_Click(object sender, EventArgs e)
        {
            if (hokiCon.Open(comcombobox.Text, (int)(baudratecombobox.SelectedItem)))
            {
                HokiConEvent.Invoke(true);
                hokiStatusLabel.Text = "电池检测仪已连接";
                ConfigOper.SetAppSettingsValue("bt3562Baudrate", baudratecombobox.Text, false);
                ConfigOper.SetAppSettingsValue("bt3562Com", comcombobox.Text, false);
                hokiconbutton.Enabled = false;
                hokidisconbutton.Enabled = true;
                comcombobox.Enabled = false;
                baudratecombobox.Enabled = false;
            }
            else MessageBox.Show("连接失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        public Action<bool> KeyenceConEvent;
        #region 扫码枪连接按钮动作函数
        private void keyenceconbutton_Click(object sender, EventArgs e)
        {
            string ip = displaytoip();
            if (keyenceCon.Connect(ip))
            {
                KeyenceConEvent.Invoke(true);
                keyenceStatusLabel.Text = "扫码枪已连接";
                ConfigOper.SetAppSettingsValue("keyenceIp", ip, false);
                ip_textbox1.Enabled = false;
                ip_textbox2.Enabled = false;
                ip_textbox3.Enabled = false;
                ip_textbox4.Enabled = false;
                keyenceconbutton.Enabled = false;
                keyencedisconbutton.Enabled = true;
            }
            else MessageBox.Show("连接失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        public Action<bool> PlcConEvent;
        #region PLC连接按钮动作函数
        private void plcconbutton_Click(object sender, EventArgs e)
        {
            int iReturnCode = mitsubishiCon.Open(int.Parse(plcIDtextbox.Text));
            if(iReturnCode==0)
            {
                PlcConEvent.Invoke(true);
                PLCStatusLabel.Text = "PLC已连接";
                ConfigOper.SetAppSettingsValue("PLCID", plcIDtextbox.Text, false);
                plcIDtextbox.Enabled = false;
                plcconbutton.Enabled = false;
                plcdisconbutton.Enabled = true;
            }
            else
            {
                if (!mitsubishiCon.connected) MessageBox.Show("连接失败,错误代码：" + iReturnCode, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else MessageBox.Show("PLC软元件状态监视注册失败，错误代码：" + iReturnCode, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 电池检测仪断开按钮函数
        private void hokidisconbutton_Click(object sender, EventArgs e)
        {
            hokiCon.SerialDiscon();
            hokiStatusLabel.Text = "电池检测仪未连接";
            HokiConEvent.Invoke(false);
            baudratecombobox.Enabled = true;
            comcombobox.Enabled = true;
            hokidisconbutton.Enabled = false;
            hokiconbutton.Enabled = true;
        }
        #endregion

        #region 扫码枪断开按钮动作函数
        private void keyencedisconbutton_Click(object sender, EventArgs e)
        {
            keyenceCon.Disconnect();
            KeyenceConEvent.Invoke(false);
            keyenceStatusLabel.Text = "扫码枪未连接";
            ip_textbox1.Enabled = true;
            ip_textbox2.Enabled = true;
            ip_textbox3.Enabled = true;
            ip_textbox4.Enabled = true;
            hokiconbutton.Enabled = true;
            hokidisconbutton.Enabled = false;
        }
        #endregion

        #region PLC断开按钮点击事件函数
        private void plcdisconbutton_Click(object sender, EventArgs e)
        {
            mitsubishiCon.FreeDeviceStatus();
            mitsubishiCon.Close();
            PlcConEvent.Invoke(false);
            PLCStatusLabel.Text = "扫码枪未连接";
            plcIDtextbox.Enabled = true;
            plcdisconbutton.Enabled = false;
            plcconbutton.Enabled = true;
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

        private void DeviceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

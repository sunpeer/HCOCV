using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace OCV
{
    public partial class SystemForm : Form
    {
        public SystemForm(HokiCon hokiCon,MitsubishiCon mitsubishiCon,KeyenceCon keyenceCon)
        {
            InitializeComponent();
            this.mitsubishiCon = mitsubishiCon;
            this.keyenceCon = keyenceCon;
            this.hokiCon = hokiCon;
        }

        HokiCon hokiCon = null;
        MitsubishiCon mitsubishiCon = null;
        KeyenceCon keyenceCon = null;
        bool onPositionFalg=false;

        private void SystemForm_Load(object sender, EventArgs e)
        {
            mitsubishiCon.workFlag = false;
            mitsubishiCon.OnPositionEvent += new Action(OnPositionEventCallBack);
            string batteryNum = ConfigurationManager.AppSettings["batteryNum"];
            batteryNum4Rbtn.Checked = (batteryNum == "4") ? true : false;
        }

        void OnPositionEventCallBack()
        {
            onPositionFalg = true;
        }

        private void getTechStandardBtn_Click(object sender, EventArgs e)
        {
            //该按钮按下后就告诉BBL层获取工艺标准，而后等待直到被返回结果
            getTechStandardBtn.Enabled = false;
            if (keyenceCon.SetScanNum(1)) 
            {
                mitsubishiCon.SendPlcCmd(PlcLabel.UpBatteryToPosition);
                while (onPositionFalg) ;
                onPositionFalg = false;
                string sn = keyenceCon.Triger();
                RequestAJson requestA = new RequestAJson();
                requestA.sfc = sn;
                RequestAResponseRootJson responseB=Mes.SendRequest(requestA);
                if (responseB.status)
                {
                    SetTechStandardEvent.Invoke(responseB.result.dy_lowerLimit, responseB.result.dy_upperLimit, responseB.result.dz_lowerLimit, responseB.result.dz_upperLimit);//各项工艺标准主界面显示
                    ConfigOper.SetAppSettingsValue("minResistance",responseB.result.dz_lowerLimit,false);//各项工艺标准全部在appconfig文件内设置
                    ConfigOper.SetAppSettingsValue("maxResistance", responseB.result.dz_upperLimit, false);//各项工艺标准全部在appconfig文件内设置
                    ConfigOper.SetAppSettingsValue("minVoltage", responseB.result.dy_lowerLimit, false);//各项工艺标准全部在appconfig文件内设置
                    ConfigOper.SetAppSettingsValue("maxVoltage", responseB.result.dy_upperLimit, false);//各项工艺标准全部在appconfig文件内设置

                    MessageBox.Show("工艺标准已设置成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("获取工艺标准失败，失败信息：\n" + string.Join("\n", responseB.errMessage.Select(temp => temp.message)), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mitsubishiCon.SendPlcCmd(PlcLabel.UpBatteryResetStart);
            }
            else MessageBox.Show("获取工艺标准失败,请联系维修人员", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            getTechStandardBtn.Enabled = true;
        }

        private void SystemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mitsubishiCon.workFlag = true;
            mitsubishiCon.OnPositionEvent -= new Action(OnPositionEventCallBack);
        }
        public Action<string,string,string,string> SetTechStandardEvent;
        public Action<int> SetScanNumEvent;
        private void saveBtn_Click(object sender, EventArgs e)
        {
            if(batteryNum2Rbtn.Checked)
            {
                if (keyenceCon.SetScanNum(2))
                {
                    ConfigOper.SetAppSettingsValue("batteryNum", "2", false);
                    SetScanNumEvent.Invoke(2);
                    MessageBox.Show("设置成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("设置失败，请联系维修人员", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (keyenceCon.SetScanNum(4))
                {
                    ConfigOper.SetAppSettingsValue("batteryNum", "4", false);
                    SetScanNumEvent.Invoke(4);
                    MessageBox.Show("设置成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else MessageBox.Show("设置失败,请联系维修人员", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void adjButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("调零最长可能会持续10s，请耐心等待", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            adjButton.Enabled = false;
            if (hokiCon.Adj())
            {
                MessageBox.Show("调零成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("调零失败，请检查正负极是否短接", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            adjButton.Enabled = true;
        }
    }
}

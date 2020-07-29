using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using BLL;

namespace UIL
{
    public partial class SystemForm : Form
    {
        public SystemForm(App app)
        {
            InitializeComponent();
            this.app = app;
        }
        App app;
        double ProbeSupportOnPositionTime;
        long startTime;
        long stopTime;
        private void SystemForm_Load(object sender, EventArgs e)
        {
            batteryNum4Rbtn.Checked = (app.curConfig.BatteryNum == 4) ? true : false;
            batteryNum2Rbtn.Checked = (app.curConfig.BatteryNum == 4) ? false : true;
            scaner_move_noRbtn.Checked = (app.curConfig.scaner_move_enable == "0") ? true : false;
            scanner_move_yesRbtn.Checked = (app.curConfig.scaner_move_enable == "1") ? true : false;
            switch(app.curConfig.scaner_position)
            {
                case "0":
                    cur_sacnner_position_combox.SelectedItem = cur_sacnner_position_combox.Items[0];
                    break;
                case "1":
                    cur_sacnner_position_combox.SelectedItem = cur_sacnner_position_combox.Items[1];
                    break;
                case "2":
                    cur_sacnner_position_combox.SelectedItem = cur_sacnner_position_combox.Items[2];
                    break;
            }
            batterDisplayNumTxtBox.Text = app.curConfig.BatteryDisplayNum.ToString();
            logDisplayNumTxtBox.Text = app.curConfig.LogDisplayNum.ToString();
            errorRateTxtBox.Text = app.curConfig.ErrorRate.ToString();
            statisticTxtBox.Text = app.curConfig.StatisticNum.ToString();
            srLiveForm.EndReceive();
            srLiveForm.IpAddress = app.curConfig.Sr2000wIp;
            tech_StdCombox.Text = app.curConfig.tech_Std;
            if (!srLiveForm.BeginReceive())
            {
                onprocessStripStatusLabel.Text = "实时显示窗口不能正常显示";
            }
        }
        private void getTechStandardBtn_Click(object sender, EventArgs e)
        {
            //该按钮按下后就告诉BBL层获取工艺标准，在返回结果之前该按钮不生效
            getTechStandardBtn.Enabled = false;
            //注册获取工艺标准结束调用的界面函数
            app.StandardGetDown = new Action<bool, string>(SetDownEventShow);
            string opId = operationIdCombox.Text;
            string errMsg = "";
            bool result=app.GetTechStdInit(opId,ref errMsg);
            //如果不满足获取工艺标准的条件，返回一个结果
            if (!result)
            {
                getTechStandardBtn.Enabled = true;
                MessageBox.Show(errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                onprocessStripStatusLabel.Text = "正在获取工艺标准，请等待！！";
        }
        private void SetDownEventShow(bool flag, string errMsg)
        {
            this.Invoke(new Action(() =>
            {
                getTechStandardBtn.Enabled = true;
                if (flag)
                {
                    //onprocessStripStatusLabel.Text = "工艺标准获取成功";
                    //这里的工艺标准全部在这里显示
                    techIdTxtBox.Text = app.curConfig.tech_no;
                    onprocessStripStatusLabel.Text = "工艺标准获取成功";
                    maxResistance.Text = app.curConfig.MaxResistance.ToString();
                    maxVoltage.Text = app.curConfig.MaxVoltage.ToString();
                    minResistance.Text = app.curConfig.MinResistance.ToString();
                    minVoltage.Text = app.curConfig.MinVoltage.ToString();
                    maxTemLabel.Text = app.curConfig.maxTem.ToString();
                    minTemLabel.Text = app.curConfig.minTem.ToString();
                    temCoeffLabel.Text = app.curConfig.temCoeff.ToString();
                    if (app.curConfig.operation_id == "O2T")
                    {
                        minKLabel.Text = app.curConfig.minK.ToString();
                        maxKLabel.Text = app.curConfig.maxK.ToString();
                    }
                    else if(app.curConfig.operation_id=="O1T")
                    {
                        minKLabel.Text = "";
                        maxKLabel.Text = "";
                    }
                    SetTechStandardEvent(true);
                }
            }));
            if(!flag) MessageBox.Show("工艺标准获取失败：" + errMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public Action<bool> SetTechStandardEvent;
        public Action<int> SetScanNumEvent;
        public Action<string> SetScannerPositionEvent;
        private void saveBtn_Click(object sender, EventArgs e)
        {
            switch(tech_StdCombox.Text)
            {
                case "VK2698B5":
                    app.fx5u.ResetRegister("M500", 0);
                    Thread.Sleep(100);
                    app.fx5u.ResetRegister("M500", 1);
                    app.fx5u.ResetRegister("M500", 0);
                    app.curConfig.tech_Std = "VK2698B5";
                    break;
                case "VK405974":
                    app.fx5u.ResetRegister("M501", 0);
                    Thread.Sleep(100);
                    app.fx5u.ResetRegister("M501", 1);
                    app.fx5u.ResetRegister("M501", 0);
                    app.curConfig.tech_Std = "VK405974";
                    break;
                case "VK466574":
                    app.fx5u.ResetRegister("M502", 0);
                    Thread.Sleep(100);
                    app.fx5u.ResetRegister("M502", 1);
                    app.fx5u.ResetRegister("M502", 0);
                    app.curConfig.tech_Std = "VK466574";
                    break;
                case "VK386786":
                    app.fx5u.ResetRegister("M503", 0);
                    Thread.Sleep(100);
                    app.fx5u.ResetRegister("M503", 1);
                    app.fx5u.ResetRegister("M503", 0);
                    app.curConfig.tech_Std = "VK386786";
                    break;
            }
            if(batteryNum2Rbtn.Checked)
            {
                app.SetScanNum(2,true);
                SetScanNumEvent?.Invoke(2);
                MessageBox.Show("设置成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                app.SetScanNum(4, true);
                SetScanNumEvent?.Invoke(4);
                MessageBox.Show("设置成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if(scaner_move_noRbtn.Checked)
            {
                app.curConfig.scaner_move_enable = "0"; //不横移
                if(app.curConfig.scaner_position=="0")
                {
                    app.fx5u.SendCmd("D175");
                    app.curConfig.scaner_position = "1";
                    SetScannerPositionEvent("中间");
                }
                else if(app.curConfig.scaner_position=="2")
                {
                    app.fx5u.SendCmd("D174");
                    app.curConfig.scaner_position = "1";
                    SetScannerPositionEvent("中间");
                }
            }
            else if(scanner_move_yesRbtn.Checked)
            {
                app.curConfig.scaner_move_enable = "1";
                if(app.curConfig.scaner_position=="1")
                {
                    app.fx5u.SendCmd("D174");
                    app.curConfig.scaner_position = "0";
                    SetScannerPositionEvent("最左边");
                }
            }

        }
        private void adjButton_Click(object sender, EventArgs e)
        {
        //    MessageBox.Show("调零最长可能会持续10s，请耐心等待", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    adjButton.Enabled = false;
        //    if (hokiCon.Adj())
        //    {
        //        MessageBox.Show("调零成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //    else
        //        MessageBox.Show("调零失败，请检查正负极是否短接", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    adjButton.Enabled = true;
        }

        private void techSaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                app.curConfig.MaxResistance = double.Parse(maxResistance.Text);
                app.curConfig.MaxVoltage = double.Parse(maxVoltage.Text);
                app.curConfig.MinResistance = double.Parse(minResistance.Text);
                app.curConfig.MinVoltage = double.Parse(minVoltage.Text);
            }
            catch
            {
                MessageBox.Show("工艺标准设置出错，请检查输入是否正确");
            }
            SetTechStandardEvent.Invoke(true);
        }
        public Action SetSrTime;
        private bool UpOnPosition=false;
        private bool DownOnPosition=false;

        private void errorAlwaysSaveBtn_Click(object sender, EventArgs e)
        {
            double errorRate;
            int statisticCounter;
            if(int.TryParse(statisticTxtBox.Text,out statisticCounter)&&double.TryParse(errorRateTxtBox.Text,out errorRate))
            {
                if(statisticCounter>0&&0<errorRate&&errorRate<1)
                {
                    app.curConfig.StatisticNum = statisticCounter;
                    app.curConfig.ErrorRate = errorRate;
                    return;
                }
            }
            MessageBox.Show("输入数值不正确，统计数量应为大于0的整数，错误率：0~1", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void srTimeOutSaveBtn_Click(object sender, EventArgs e)
        {
            int timeOutSetting;
            if(int.TryParse(sr2000wTimetxtBox.Text, out timeOutSetting))
            {
                if(timeOutSetting>0)
                {
                    app.curConfig.Sr2000wTimeOut = timeOutSetting;
                    SetSrTime?.Invoke();
                    onprocessStripStatusLabel.Text = "扫码枪超时时间保存成功";
                }
            }
        }

        private void SystemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            srLiveForm.Dispose();

        }

        private void inBtn_Click(object sender, EventArgs e)
        {
            string errMsg="";
            UpOnPosition = false;
            DownOnPosition = false;
            inBtn.Enabled = false;
            if(!app.TestInit(ref errMsg))
            {
                inBtn.Enabled = true;
                onprocessStripStatusLabel.Text = errMsg;
                return;
            }
            app.TestUpBatteryOnPositionEvent = TestUpBatteryOnPositionCallBack;
            app.TestDownBatteryOnPositionEvent = TestDownBatteryOnPositionCallBack;
            app.TestResetEvent = TesetResetCallBack;
        }
        private void TestUpBatteryOnPositionCallBack()
        {
            this.Invoke(new Action(() =>
            {
                ProbeSupportOnPositionTime = app.timeInterval.TotalMilliseconds;
                UpOnPosition = true;
                DownOnPosition = false;
                onprocessStripStatusLabel.Text = "上层电池已到检测位";
                changeLevelBtn.Enabled = true;
            }));
        }
        private void TestDownBatteryOnPositionCallBack()
        {
            this.Invoke(new Action(() =>
            {
                ProbeSupportOnPositionTime = app.timeInterval.TotalMilliseconds;
                DownOnPosition = true;
                UpOnPosition = false;
                onprocessStripStatusLabel.Text = "下层电池已到检测位";
                changeLevelBtn.Enabled = true;
            }));
        }
        private void scanStartBtn_Click(object sender, EventArgs e)
        {
            if (inBtn.Enabled)
            {
                onprocessStripStatusLabel.Text = "未放电池进入，无法检测";
                return;
            }
            scanStartBtn.Enabled = false;
            app.SetScanNum(app.curConfig.BatteryNum, false);
            if (app.curConfig.scaner_move_enable=="0")
            {
                string sns;
                app.sr2000w.DataReceivedEventHandler = Sr2000wGet;
                startTime = DateTime.Now.Ticks;
                sns = app.sr2000w.SendCmd("LON");
                if (app.CheckIsSns(sns))
                {
                    app.sr2000w.DataReceivedEventHandler = null;
                    Sr2000wGet(sns);
                }
            }
            else if(app.curConfig.scaner_move_enable=="1")
            {
                app.sr2000w.DataReceivedEventHandler = Sr2000wGet;
                app.sr2000w.SendCmd("LON");
                if(app.curConfig.scaner_position=="0")
                {
                    app.fx5u.SendCmd("D177");
                    app.curConfig.scaner_position = "2";
                    SetScannerPositionEvent("最右边");
                }
                else if(app.curConfig.scaner_position=="2")
                {
                    app.fx5u.SendCmd("D176");
                    app.curConfig.scaner_position = "0";
                    SetScannerPositionEvent("最左边");
                }
            }
        }
        private void Sr2000wGet(string sns)
        {
            this.Invoke(new Action(() =>
            {
                scanStartBtn.Enabled = true;
                if(app.curConfig.scaner_move_enable=="0")
                {
                    onprocessStripStatusLabel.Text = "已经扫到码了,建议的扫码超时时间已生成";
                    stopTime = DateTime.Now.Ticks;
                    TimeSpan timeInterval = new TimeSpan(stopTime - startTime);
                    double millSr2000wTimeout = timeInterval.TotalMilliseconds;
                    double dallTime = millSr2000wTimeout + ProbeSupportOnPositionTime;
                    int iallTime = Convert.ToInt32(dallTime) + 1000;
                    sr2000wTimetxtBox.Text = iallTime.ToString();
                }
                //把码显示出来
                sns = sns.Substring(0, sns.Length - 1);
                string[] snArray = sns.Split(',');
                if (app.curConfig.BatteryNum == 4)
                {
                    channel1sns.Text = snArray[3];
                    channel2sns.Text = snArray[2];
                    channel3sns.Text = snArray[1];
                    channel4sns.Text = snArray[0];
                }
                else
                {
                    channel2sns.Text = snArray[1];
                    channel3sns.Text = snArray[0];
                }
            }));
        }
        private void TesetResetCallBack()
        {
            this.Invoke(new Action(() =>
            {
                UpOnPosition = false;
                DownOnPosition = false;
                inBtn.Enabled = true;
            }));
            app.TestDownBatteryOnPositionEvent = null;
            app.TestUpBatteryOnPositionEvent = null;
        }

        private void scanStopBtn_Click(object sender, EventArgs e)
        {
            scanStartBtn.Enabled = true;
            app.sr2000w.DataReceivedEventHandler = null;
            app.sr2000w.SendCmd("LOFF");
        }

        private void testCh1Btn_Click(object sender, EventArgs e)
        {
            if (!UpOnPosition && !DownOnPosition)//如果没有任意一层电池到了检测位
            {
                onprocessStripStatusLabel.Text = "没有电池进入检测位,无法测试";
                return;
            }
            app.fx5u.SendCmd("D142");
            Thread.Sleep(100);
            if (UpOnPosition)
            {
                app.fx5u.ResetRegister("D6", 0);
                app.fx5u.ResetRegister("D7", 0);
                app.fx5u.ResetRegister("D8", 0);
                app.fx5u.SendCmd("D5");
            }
            else
            {
                app.fx5u.ResetRegister("D17", 0);
                app.fx5u.ResetRegister("D18", 0);
                app.fx5u.ResetRegister("D19", 0);
                app.fx5u.SendCmd("D16");
            }
            
            Thread.Sleep(1000);
            app.bt3562.DataReceivedEventHandler = Channel1Callback;
            app.bt3562.SendCmd(":FETCH?");
        }

        private void Channel1Callback(string RandV)
        {
            this.Invoke(new Action(() =>
            {
                double v;
                double r;
                RandV = RandV.Replace(" ", "");
                //区分电压和电阻
                string[] values = RandV.Split(',');
                r = Math.Abs(double.Parse(values[0]));
                v = Math.Abs(double.Parse(values[1]));
                channel1r.Text = r.ToString();
                channel1v.Text = v.ToString();
                if (!app.CheckRVNormal(r, v))
                    Ch1Led.ForeColor = Color.Red;
                else
                    Ch1Led.ForeColor = Color.Lime;
            }));
        }
        private void Channel2Callback(string RandV)
        {
            this.Invoke(new Action(() =>
            {
                double v;
                double r;
                RandV = RandV.Replace(" ", "");
                //区分电压和电阻
                string[] values = RandV.Split(',');
                r = Math.Abs(double.Parse(values[0]));
                v = Math.Abs(double.Parse(values[1]));
                channel2r.Text = r.ToString();
                channel2v.Text = v.ToString();
                if (!app.CheckRVNormal(r, v))
                    Ch2Led.ForeColor = Color.Red;
                else
                    Ch2Led.ForeColor = Color.Lime;
                app.fx5u.SendCmd("D143");
            }));
        }
        private void Channel3Callback(string RandV)
        {
            this.Invoke(new Action(() =>
            {
                double v;
                double r;
                RandV = RandV.Replace(" ", "");
                //区分电压和电阻
                string[] values = RandV.Split(',');
                r = Math.Abs(double.Parse(values[0]));
                v = Math.Abs(double.Parse(values[1]));
                channel3r.Text = r.ToString();
                channel3v.Text = v.ToString();
                if (!app.CheckRVNormal(r, v))
                    Ch3Led.ForeColor = Color.Red;
                else
                    Ch3Led.ForeColor = Color.Lime;
                app.fx5u.SendCmd("D143");
            }));
            
        }
        private void Channel4Callback(string RandV)
        {
            this.Invoke(new Action(() =>
            {
                double v;
                double r;
                RandV = RandV.Replace(" ", "");
                //区分电压和电阻
                string[] values = RandV.Split(',');
                r = Math.Abs(double.Parse(values[0]));
                v = Math.Abs(double.Parse(values[1]));
                channel4r.Text = r.ToString();
                channel4v.Text = v.ToString();
                if (!app.CheckRVNormal(r, v))
                    Ch4Led.ForeColor = Color.Red;
                else
                    Ch4Led.ForeColor = Color.Lime;
                app.fx5u.SendCmd("D143");
            }));
            
        }

        private void testCh2Btn_Click(object sender, EventArgs e)
        {
            if (!UpOnPosition && !DownOnPosition)//如果没有任意一层电池到了检测位
            {
                onprocessStripStatusLabel.Text = "没有电池进入检测位,无法测试";
                return;
            }
            app.fx5u.SendCmd("D142");
            Thread.Sleep(100);
            if (UpOnPosition)
            {
                app.fx5u.ResetRegister("D5", 0);
                app.fx5u.ResetRegister("D7", 0);
                app.fx5u.ResetRegister("D8", 0);
                app.fx5u.SendCmd("D6");
            }
            else
            {
                app.fx5u.ResetRegister("D16", 0);
                app.fx5u.ResetRegister("D18", 0);
                app.fx5u.ResetRegister("D19", 0);
                app.fx5u.SendCmd("D17");
            }
            Thread.Sleep(1000);
            app.bt3562.DataReceivedEventHandler = Channel2Callback;
            app.bt3562.SendCmd(":FETCH?");
        }
                
        private void testCh3Btn_Click(object sender, EventArgs e)
        {
            if (!UpOnPosition && !DownOnPosition)//如果没有任意一层电池到了检测位
            {
                onprocessStripStatusLabel.Text = "没有电池进入检测位,无法测试";
                return;
            }
            app.fx5u.SendCmd("D142");
            Thread.Sleep(100);

            if (UpOnPosition)
            {
                app.fx5u.ResetRegister("D5", 0);
                app.fx5u.ResetRegister("D6", 0);
                app.fx5u.ResetRegister("D8", 0);
                app.fx5u.SendCmd("D7");
            }
            else
            {
                app.fx5u.ResetRegister("D16", 0);
                app.fx5u.ResetRegister("D17", 0);
                app.fx5u.ResetRegister("D19", 0);
                app.fx5u.SendCmd("D18");
            }
            
            Thread.Sleep(1000);
            app.bt3562.DataReceivedEventHandler = Channel3Callback;
            app.bt3562.SendCmd(":FETCH?");
        }

        private void testCh4Btn_Click(object sender, EventArgs e)
        {
            if (!UpOnPosition && !DownOnPosition)//如果没有任意一层电池到了检测位
            {
                onprocessStripStatusLabel.Text = "没有电池进入检测位,无法测试";
                return;
            }
            app.fx5u.SendCmd("D142");
            Thread.Sleep(100);

            if (UpOnPosition)
            {
                app.fx5u.ResetRegister("D5", 0);
                app.fx5u.ResetRegister("D7", 0);
                app.fx5u.ResetRegister("D6", 0);
                app.fx5u.SendCmd("D8");
            }
            else
            {
                app.fx5u.ResetRegister("D16", 0);
                app.fx5u.ResetRegister("D18", 0);
                app.fx5u.ResetRegister("D17", 0);
                app.fx5u.SendCmd("D19");
            }
            
            Thread.Sleep(1000);
            app.bt3562.DataReceivedEventHandler = Channel4Callback;
            app.bt3562.SendCmd(":FETCH?");
        }

        private void displayNumTxtBtn_Click(object sender, EventArgs e)
        {
            int batteryDisplayNum;
            int logDisplayNum;
            if(int.TryParse(batterDisplayNumTxtBox.Text,out batteryDisplayNum)&&int.TryParse(logDisplayNumTxtBox.Text,out logDisplayNum))
            {
                if(batteryDisplayNum>0&&logDisplayNum>0)
                {
                    app.curConfig.BatteryDisplayNum = int.Parse(batterDisplayNumTxtBox.Text);
                    app.curConfig.LogDisplayNum = int.Parse(logDisplayNumTxtBox.Text);
                    //改变datatable的数量，多的删掉
                    while (app.batteryDataTable.Rows.Count > app.curConfig.BatteryDisplayNum)
                        //for (int i = app.curConfig.BatteryDisplayNum; i < app.batteryDataTable.Rows.Count; i++)
                        app.batteryDataTable.Rows.RemoveAt(app.batteryDataTable.Rows.Count - 1);
                    while (app.logDataTable.Rows.Count > app.curConfig.LogDisplayNum)
                        //for (int i = app.curConfig.LogDisplayNum; i < app.logDataTable.Rows.Count; i++)
                        app.logDataTable.Rows.RemoveAt(app.logDataTable.Rows.Count - 1);
                    return;
                }
            }
            MessageBox.Show("输入值不正确，请输入大于0的整数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void changeLevelBtn_Click(object sender, EventArgs e)
        {
            if (inBtn.Enabled)
                return;
            if (UpOnPosition)//如果上层进去了
            {
                app.fx5u.SendCmd("D142");
                Thread.Sleep(50);
                app.fx5u.SendCmd("D143");
                Thread.Sleep(50);
                app.fx5u.SendCmd("D108");
            }
            if(DownOnPosition)
            {
                app.fx5u.SendCmd("D142");
                Thread.Sleep(50);
                app.fx5u.SendCmd("D143");
                Thread.Sleep(50);
                app.fx5u.SendCmd("D119");//如果下层进去了
            }
            changeLevelBtn.Enabled = false;
            UpOnPosition = false;
            DownOnPosition = false;
        }

        private void temperature_testBtn_Click(object sender, EventArgs e)
        {
            double tem=app.GetTemperature();
            temLabel.Text = tem.ToString();
        }

        private void SystemForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void batteryNum2Rbtn_Click(object sender, EventArgs e)
        {
            batteryNum2Rbtn.Checked = true;
            batteryNum4Rbtn.Checked = false;
        }

        private void batteryNum4Rbtn_Click(object sender, EventArgs e)
        {
            batteryNum2Rbtn.Checked = false;
            batteryNum4Rbtn.Checked = true;
        }

        private void scanner_move_yesRbtn_Click(object sender, EventArgs e)
        {
            scanner_move_yesRbtn.Checked = true;
            scaner_move_noRbtn.Checked = false;
        }

        private void scaner_move_noRbtn_Click(object sender, EventArgs e)
        {
            scanner_move_yesRbtn.Checked = false;
            scaner_move_noRbtn.Checked = true;
        }

        private void set_Scanner_position_Click(object sender, EventArgs e)
        {
            app.curConfig.scaner_position=cur_sacnner_position_combox.SelectedIndex.ToString();
            SetScannerPositionEvent(cur_sacnner_position_combox.Text);
        }
    }
}

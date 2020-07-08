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
using System.Diagnostics;

namespace OCV
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        Stopwatch softRunWatch = new Stopwatch();
        Stopwatch workRunWatch = new Stopwatch();

        SoftState softState = null;

        DeviceForm deviceForm = null;
        UserForm userForm = null;
        SystemForm systemForm = null;


        DataTable batterydatatable = null;

        void MyMonitorCallBack(Object sender,PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "in":
                    break;
                case "ok":
                    break;
                case "test":
                    break;
                case "saved":
                    break;

            }
        }


        #region 子窗口相关

        private void user_MenuItem_Click(object sender, EventArgs e)
        {
            userForm.Show();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void device_MenuItem_Click(object sender, EventArgs e)
        {
            if (softState.mitsubishiCon.plcState == PlcState.STOP)
            {
                deviceForm = new DeviceForm(softState.hokiCon, softState.keyenceCon, softState.mitsubishiCon);
                deviceForm.HokiConEvent += new Action<bool>(HokiConShowCallBack);
                deviceForm.KeyenceConEvent += new Action<bool>(KeyenceConShowCallBack);
                deviceForm.PlcConEvent += new Action<bool>(MitSubishiConShowCallBack);
            }
            else MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void systemMenuItem_Click(object sender, EventArgs e)
        {
            if (softState.mitsubishiCon.plcState == PlcState.STOP)
            {
                systemForm = new SystemForm(softState.hokiCon,softState.mitsubishiCon,softState.keyenceCon);
                systemForm.SetTechStandardEvent += new Action<string, string, string, string>(SetTechStandardCallBack);
                systemForm.SetScanNumEvent += new Action<int>(SetScanNumCallBack);
            }
            else MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        List<Label> snLabelList = null;
        List<Label> resistanceLabelList = null;
        List<Label> voltageLabelList = null;
        List<Label> verifyLedList = null;
        List<List<Label>> batteryLedList = null;
        List<Label> battery1LedList = null;
        List<Label> battery2LedList = null;
        List<Label> battery3LedList = null;
        List<Label> battery4LedList = null;
        #region 主窗口加载函数
        private void mainform_Load(object sender, EventArgs e)
        {

            snLabelList = new List<Label>();
            snLabelList.Add(sn41);
            snLabelList.Add(sn42);
            snLabelList.Add(sn43);
            snLabelList.Add(sn44);

            resistanceLabelList = new List<Label>();
            resistanceLabelList.Add(resistance41);
            resistanceLabelList.Add(resistance42);
            resistanceLabelList.Add(resistance43);
            resistanceLabelList.Add(resistance44);

            voltageLabelList = new List<Label>();
            voltageLabelList.Add(voltage41);
            voltageLabelList.Add(voltage42);
            voltageLabelList.Add(voltage43);
            voltageLabelList.Add(voltage44);

            verifyLedList = new List<Label>();
            verifyLedList.Add(verify41Led);
            verifyLedList.Add(verify42Led);
            verifyLedList.Add(verify43Led);
            verifyLedList.Add(verify44Led);

            battery1LedList = new List<Label>();
            battery1LedList.Add(send41Led);
            battery1LedList.Add(result41Led);
            battery1LedList.Add(voltage41Led);
            battery1LedList.Add(resistance41Led);

            battery2LedList = new List<Label>();
            battery2LedList.Add(send42Led);
            battery2LedList.Add(result42Led);
            battery2LedList.Add(voltage42Led);
            battery2LedList.Add(resistance42Led);


            battery3LedList = new List<Label>();
            battery3LedList.Add(send43Led);
            battery3LedList.Add(result43Led);
            battery3LedList.Add(voltage43Led);
            battery3LedList.Add(resistance43Led);


            battery4LedList = new List<Label>();
            battery4LedList.Add(send44Led);
            battery4LedList.Add(result44Led);
            battery4LedList.Add(voltage44Led);
            battery4LedList.Add(resistance44Led);

            batteryLedList = new List<List<Label>>();
            batteryLedList.Add(battery1LedList);
            batteryLedList.Add(battery2LedList);
            batteryLedList.Add(battery3LedList);
            batteryLedList.Add(battery4LedList);

            softRunWatch.Start();
            sTimer.Start();

            userForm = new UserForm(softState);
            userForm.SetCurUserEvent += new Action<User>(SetCurUserCallBack);


            softState = new SoftState(
                            BatteryOnPositionShowMainFormCallBack,

                            KeyenceScanDownShowMainFormCallBack,

                            KeyenceReceivedDataSavedShowMainFormCallBack,

                            BatteryNormalResetDownShowMainFormCallBack,

                            BatteryAbnormalResetDownShowMainFormCallBack,

                            ChannelStartMeasureShowMainFormCallBack,

                            MeasureDownShowMainFormCallback,

                            SendBatteryDownShowMainFormCallBack,

                            BatteryStartResetShowMainFormCallBack,

                            StopCallBack,

                            new PropertyChangedEventHandler( MyMonitorCallBack)
                            );

            #region 电池检测仪相关
            if (softState.hokiCon.Open(ConfigurationManager.AppSettings["bt3562Com"], int.Parse(ConfigurationManager.AppSettings["bt3562Baudrate"])))
            {
                hokiconled.ForeColor = Color.Lime;
                onProcessNameToolStripStatusLabel.Text = "电池检测仪已连接";
            }
            #endregion

            #region 扫码枪相关
            if (softState.keyenceCon.Connect(ConfigurationManager.AppSettings["IP"]))
            {
                keyenceconled.ForeColor = Color.Lime;
                onProcessNameToolStripStatusLabel.Text = "扫码枪已连接";
            }
            #endregion

            #region PLC相关
            if(softState.mitsubishiCon.Open(int.Parse(ConfigurationManager.AppSettings["PLCID"]))==0)
            {
                fx5conled.ForeColor = Color.Lime;
                onProcessNameToolStripStatusLabel.Text = "PLC已连接";
            }
            #endregion

            batterydatagridview.DataSource = softState.batteryDataTable;
            logDataGridView.DataSource = softState.logDataTable;

            batterydatagridview.RowsAdded += new DataGridViewRowsAddedEventHandler(this.BatteryDGV_RowsAdded);

        }
        #endregion

        //电池数据表插入数据函数
        private void BatteryDGVadd(Battery battery)
        {
            //添加进datagridview
            DataRow dr = softState.batteryDataTable.NewRow(); 
            DataManager.GetBatteryLatest(ref dr);
            softState.batteryDataTable.Rows.InsertAt(dr, 0);
        }

        //add_rows调用方法
        private void BatteryDGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if ((string)(batterydatagridview.Rows[e.RowIndex].Cells["result"].Value) == "不合格")
            {
                batterydatagridview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            }
            //只显示20行
            batterydatagridview.Rows.RemoveAt(20);
        }

        void SetBatteryInfo(Battery battery, List<Label> batteryLedList)
        {
            batteryLedList[0].ForeColor = battery.MesSaved ? Color.Lime : Color.Red;
            batteryLedList[1].ForeColor = battery.result ? Color.Lime : Color.Red;
            int voltageJudge = (Convert.ToInt32(battery.errorType)) & 12;
            int resistanceJudge = (Convert.ToInt32(battery.errorType)) & 3;
            batteryLedList[2].ForeColor = (voltageJudge == 0) ? Color.Lime : ((voltageJudge == 8) ? Color.Red : Color.Blue);
            batteryLedList[3].ForeColor = (resistanceJudge == 0) ? Color.Lime : ((resistanceJudge == 4) ? Color.Red : Color.Blue);
        }
        void ShowBatteryInfo(Battery battery,short i)
        {
            SetBatteryInfo(battery, batteryLedList[i]);
        }
        private void PLCstartbtn_Click(object sender, EventArgs e)
        {
            if(softState.curUser==null||!softState.hokiCon.connected||
                !softState.keyenceCon.connected||!softState.mitsubishiCon.entried||!softState.mitsubishiCon.connected)
            {
                MessageBox.Show("系统未准备，请检查各设备的连接情况，以及是否登录等！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //会先判断一下光幕的状态再启动
            softState.mitsubishiCon.Run();
            onProcessNameToolStripStatusLabel.Text = "开始工作";
            softState.CreateSystemStartLog();
            workRunWatch.Start();
            PLCstartbtn.Enabled = false;
            PLCstopbtn.Enabled = true;
        }

        private void PLCstopbtn_Click(object sender, EventArgs e)
        {
            softState.mitsubishiCon.setStopFlag = true;
            MessageBox.Show("机器完成当前操作后会自动停机！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void StopCallBack()
        {
            MessageBox.Show("设备已停机", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            onProcessNameToolStripStatusLabel.Text = "停止工作";
            softState.CreateSystemStopLog();
            workRunWatch.Reset();
            PLCstartbtn.Enabled = true;
            PLCstartbtn.Enabled = false;
        }
       
        private void KeyenceScanDownShowMainFormCallBack()
        {
            scanLed.ForeColor = Color.Gray;
            verifyLed.ForeColor = Color.Lime;
            onProcessNameToolStripStatusLabel.Text = "正在校验";
        }
        private void KeyenceReceivedDataSavedShowMainFormCallBack()
        {
            bool verifyFlag=true;
            verifyLed.ForeColor = Color.Gray;
            for(int i=0;i<softState.curBattery.Length;i++)
            {
                snLabelList[i].Text = softState.curBattery[i].sn;
                verifyLedList[i].ForeColor = softState.curBattery[i].verifyFlag ? Color.Lime : Color.Red;
            }
            for (int i = 0; i < softState.curBattery.Length; i++)
            {
                if (!softState.curBattery[i].verifyFlag) verifyFlag=false;
            }
            onProcessNameToolStripStatusLabel.Text = verifyFlag ? "准备测量" : "正在退料";
            batteryOutLed.ForeColor = verifyFlag ? Color.Gray : Color.Lime;
            measureLed.ForeColor = verifyFlag ? Color.Lime : Color.Gray;
        }
        private void BatteryInShowMainFormCallBack(bool batteryLevel)
        {
            onProcessNameToolStripStatusLabel.Text = batteryLevel ? "上层电池正在进料" : "下层电池正在进料";
            batteryInLed.ForeColor = Color.Lime;
            layBatteryLed.ForeColor = Color.Gray;
        }
        private void BatteryOnPositionShowMainFormCallBack(bool batteryLevel)
        {
            //重置
            ResetLabelListText(snLabelList, "            ");//重置所有序列号
            ResetLabelListColor(verifyLedList, Color.Gray);//重置所有校验灯
            ResetLabelListText(voltageLabelList, "            ");//重置所有电压
            ResetLabelListText(resistanceLabelList, "            ");//重置所有电阻
            foreach(List<Label> ledList in batteryLedList)//重置所有灯
            {
                foreach(Label ledItem in ledList)
                {
                    ledItem.ForeColor = Color.Gray;
                }
            }
            batteryInLed.ForeColor = Color.Gray;//熄灭进料灯
            onProcessNameToolStripStatusLabel.Text = "正在扫码";
            scanLed.ForeColor = Color.Lime;//点亮扫码灯
        }
        private void ChannelStartMeasureShowMainFormCallBack(short i)
        {
            onProcessNameToolStripStatusLabel.Text = "第" + i.ToString() + "通道电池正在检测";
        }
        private void SendBatteryDownShowMainFormCallBack(Battery battery,short i)
        {
            ShowBatteryInfo(battery, i);
            onProcessNameToolStripStatusLabel.Text = battery.MesSaved ? "MES上传成功" : "MES上传失败，已保存至本地";
            saveLed.ForeColor = Color.Gray;
        }

        private void MeasureDownShowMainFormCallback(short i)
        {
            measureLed.ForeColor = Color.Gray;
            saveLed.ForeColor = Color.Lime;
            onProcessNameToolStripStatusLabel.Text = "第"+i.ToString()+"通道电池信息正在录入";
        }

        private void BatteryStartResetShowMainFormCallBack(bool batteryLevel)
        {
            resetLed.ForeColor = Color.Lime;
            onProcessNameToolStripStatusLabel.Text = batteryLevel?"上层电池盒正在复位":"下层电池盒正在复位";
        }
        private void BatteryNormalResetDownShowMainFormCallBack(bool batteryLevel)
        {
            resetLed.ForeColor = Color.Gray;
            layBatteryLed.ForeColor = Color.Lime;
            onProcessNameToolStripStatusLabel.Text = batteryLevel ? "等待下层电池放置" : "等待上层电池放置";
        }
        private void BatteryAbnormalResetDownShowMainFormCallBack(bool batteryLevel)
        {
            batteryOutLed.ForeColor = Color.Gray;
            layBatteryLed.ForeColor = Color.Lime;//通过光幕调用batteryInShowMainFormCallBack函数熄灭
            onProcessNameToolStripStatusLabel.Text = batteryLevel ? "等待上层电池重新放置" : "等待下层电池重新放置";
        }

        private void SetCurUserCallBack(User user)
        {
            softState.curUser = user;
            currentUserToolStripStatusLabel.Text = "当前员工：" + user.num;
            userlogled.ForeColor = Color.Lime;
        }

        private void ResetLabelListText(List<Label> labelList,string defaultText)
        {
            foreach(Label label in labelList)
            {
                label.Text = defaultText;
            }
        }
        private void ResetLabelListColor(List<Label> labelList,Color color)
        {
            foreach(Label label in labelList)
            {
                label.ForeColor = color;
            }
        }

        private void sTimer_Tick(object sender, EventArgs e)
        {
            softwareStampToolStripStatusLabel.Text = "软件运行时间："+softRunWatch.Elapsed.ToString();
            workRunTimeToolStripStatusLabel.Text = workRunWatch.IsRunning?"工作运行时间：" + workRunWatch.Elapsed.ToString():"工作运行时间：";
        }

        private void HokiConShowCallBack(bool connectedFlag)
        {
            hokiconled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
            onProcessNameToolStripStatusLabel.Text = connectedFlag?"电池检测仪已连接":"电池检测仪已断开连接";
        }
        private void KeyenceConShowCallBack(bool connectedFlag)
        {
            keyenceconled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
            onProcessNameToolStripStatusLabel.Text = connectedFlag ? "扫码枪已连接" : "扫码枪已断开连接";
        }
        private void MitSubishiConShowCallBack(bool connectedFlag)
        {
            fx5conled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
            onProcessNameToolStripStatusLabel.Text = connectedFlag ? "PLC已连接" : "PLC已断开连接";
        }

        private void SetTechStandardCallBack(string minVoltage, string maxVoltage, string minResistance, string maxResistance)
        {
            minVoltagelabel.Text = minVoltage;
            maxVoltagelabel.Text = maxVoltage;
            minResistancelabel.Text = minResistance;
            maxVoltagelabel.Text = maxResistance;
        }
        private void SetScanNumCallBack(int scanNum)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (softState.mitsubishiCon.plcState != PlcState.STOP)
            {
                e.Cancel = true;
                MessageBox.Show("请先停机再退出软件","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void Data_MenuItem_Click(object sender, EventArgs e)
        {
            DataForm dataForm = new DataForm();
            dataForm.Show();
        }
    }
}
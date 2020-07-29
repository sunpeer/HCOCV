#define NOSENSOR
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
using BLL;
using Model;
using DAL;
using Microsoft.VisualBasic.PowerPacks;


namespace UIL
{
    public partial class MainForm : Form
    {
        static int VOLTAGE_UNIT = 1000;
        static int RESISTANCE_UNIT = 1000;
        App app;
        public MainForm()
        {
            InitializeComponent();
            app = new App();
        }
        Stopwatch softRunWatch = new Stopwatch();
        Stopwatch workRunWatch = new Stopwatch();

        DeviceForm deviceForm;
        UserForm userForm;
        SystemForm systemForm;
        
        void MyMonitorCallBack(Object sender,PropertyChangedEventArgs e)
        {
            this.Invoke(new Action(() => {
                switch (e.PropertyName)
                {
                    case "InBatteryNum":
                        inBatteryNum.Text = app.myMonitor.InBatteryNum.ToString();
                        break;
                    case "OkBatteryNum":
                        OkBatteryNum.Text = app.myMonitor.OkBatteryNum.ToString();
                        break;
                    case "TestBatteryNum":
                        MeasurBatteryNum.Text = app.myMonitor.TestBatteryNum.ToString();
                        app.curUserProductionCounter++;
                        userProductionCounterLabel.Text = app.curUserProductionCounter.ToString();
                        todayProductionCounterLabel.Text = (int.Parse(todayProductionCounterLabel.Text) + 1).ToString();
                        break;
                    case "SavedBatteryNum":
                        SavedBatteryNum.Text = app.myMonitor.SavedBatteryNum.ToString();
                        break;
                }
            }));
        }

        #region 子窗口相关

        private void user_MenuItem_Click(object sender, EventArgs e)
        {
            if (userForm != null)
                if (!userForm.IsDisposed)
                    return;
            if (app.plcState == PlcState.STOP)
            {
                userForm = new UserForm(app);
                userForm.SetCurUserEvent = UserLoginCallBack;
                userForm.ShowDialog();
            }
            else
                MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void device_MenuItem_Click(object sender, EventArgs e)
        {
            if(deviceForm!=null)
                if (!deviceForm.IsDisposed)
                    return;
            if (app.plcState==PlcState.STOP)
            {
                deviceForm = new DeviceForm(app);
                deviceForm.HokiConEvent += new Action<bool>(HokiConShowCallBack);
                deviceForm.KeyenceConEvent += new Action<bool>(KeyenceConShowCallBack);
                deviceForm.PlcConEvent += new Action<bool>(MitSubishiConShowCallBack);
                deviceForm.ShowDialog();
            }
            else MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void systemMenuItem_Click(object sender, EventArgs e)
        {
            if(systemForm!=null)
                if (!systemForm.IsDisposed)
                    return;
            if (app.plcState == PlcState.STOP)
            {
                systemForm = new SystemForm(app);
                systemForm.SetTechStandardEvent = new Action<bool>(SetTechStandardCallBack);
                systemForm.SetScanNumEvent = SetScanNumCallBack;
                systemForm.SetScannerPositionEvent = SetScannerPositionCallBack;
                systemForm.SetSrTime = sr2000wTimOutSetEvent;
                systemForm.ShowDialog();
            }
            else MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void SetScannerPositionCallBack(string position)
        {
            this.Invoke(new Action(() =>
            {
                scanner_move_desc.Text = "扫码枪在" + position;
            }));
        }

        #endregion
        
        List<Label> snLabelList = null;
        List<Label> resistanceLabelList = null;
        List<Label> voltageLabelList = null;
        List<Label> KLabelList = null;
        List<Label> verifyLedList = null;
        List<List<Label>> batteryLedList = null;
        List<Label> descList = null;
        List<Label> battery1LedList = null;
        List<Label> battery2LedList = null;
        List<Label> battery3LedList = null;
        List<Label> battery4LedList = null;

        Dictionary<BatteryErrorType, string> myErrorDictionary;

        void Inint()
        {

            snLabelList = new List<Label>();
            snLabelList.Add(sn41);
            snLabelList.Add(sn42);
            snLabelList.Add(sn43);
            snLabelList.Add(sn44);

            descList = new List<Label>();
            descList.Add(desc41);
            descList.Add(desc42);
            descList.Add(desc43);
            descList.Add(desc44);

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

            KLabelList = new List<Label>();
            KLabelList.Add(K41);
            KLabelList.Add(K42);
            KLabelList.Add(K43);
            KLabelList.Add(K44);

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
            battery1LedList.Add(K41Led);
            

            battery2LedList = new List<Label>();
            battery2LedList.Add(send42Led);
            battery2LedList.Add(result42Led);
            battery2LedList.Add(voltage42Led);
            battery2LedList.Add(resistance42Led);
            battery2LedList.Add(K42Led);


            battery3LedList = new List<Label>();
            battery3LedList.Add(send43Led);
            battery3LedList.Add(result43Led);
            battery3LedList.Add(voltage43Led);
            battery3LedList.Add(resistance43Led);
            battery3LedList.Add(K43Led);

            battery4LedList = new List<Label>();
            battery4LedList.Add(send44Led);
            battery4LedList.Add(result44Led);
            battery4LedList.Add(voltage44Led);
            battery4LedList.Add(resistance44Led);
            battery4LedList.Add(K44Led);

            batteryLedList = new List<List<Label>>();
            batteryLedList.Add(battery1LedList);
            batteryLedList.Add(battery2LedList);
            batteryLedList.Add(battery3LedList);
            batteryLedList.Add(battery4LedList);

            myErrorDictionary = new Dictionary<BatteryErrorType, string>();
            myErrorDictionary.Add(BatteryErrorType.OK, "正常");
            myErrorDictionary.Add(BatteryErrorType.BelowMinResistance, "低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxResistance, "大于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltage, "低于正常电压值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltageBelowMinResistance, "低于正常电压值，低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltageOverMaxResistance, "低于正常电压值，大于正常正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltage, "大于正常电压值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltageBelowMinResistance, "大于正常电压值，低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltageOverMaxResistance, "大于正常电压值，大于正常电阻值");

        }

        #region 主窗口加载函数
        private void mainform_Load(object sender, EventArgs e)
        {
            //Action asyInit = new Action(Inint);
            //asyInit.BeginInvoke(null, null);   //不可以异步，会影响后面的程序
            Inint();
            sr2000wTimOutSetEvent();
            softRunWatch.Start();
            sTimer.Start();

            //userForm = new UserForm(app);
            //batterydatagridview.RowsAdded += new DataGridViewRowsAddedEventHandler(this.BatteryDGV_RowsAdded);

            //关注一些必要的委托
            app.myMonitor.PropertyChanged+= new PropertyChangedEventHandler(MyMonitorCallBack);

            //userForm.SetCurUserEvent += new Action<User>(SetCurUserCallBack);
            Action asyappInit = new Action(app.Init);
            asyappInit.Invoke();
            //数据源绑定
            batterydatagridview.DataSource = app.batteryDataTable;
            logDataGridView.DataSource = app.logDataTable;
            //DataGridView修改函数注册
            app.batteryInfoShowInDataGridView = BatteryDGVadd;
            app.LogShowInDataGridView = LogDGVadd;
            //一次检测电池数量界面设置
            SetScanNumCallBack(app.curConfig.BatteryNum);
            //一些加工参数加载
            SetTechStandardCallBack(true);
            //Action<Action<bool>> asyGetResource = new Action<Action<bool>>(app.GetResource);
            //asyGetResource.BeginInvoke(GetResourceCallback, null, null);          //异步执行会导致后面的数据库访问出错
            app.GetResource(GetResourceCallback);
            ////依次无参连接设备
            app.Bt3562ConDownEvent = Bt3562ConnectedCallBack;
            Action asyBt3562Con = new Action(app.ConnectBt3562);
            asyBt3562Con.BeginInvoke(null, null);
            //app.ConnectBt3562();
            //app.Connect(app.bt3562, new Action<bool>(Bt3562ConnectedCallBack));
            app.Connect(app.sr2000w, new Action<bool>(Sr2000wConnectedCallBack));
            //Action<Action<bool>> asyFx5uCon = new Action<Action<bool>>(app.Connect);
            app.Connect(app.fx5u, new Action<bool>(Fx5uConnectedCallBack));
            app.ConnectTemperature();
            app.setUpOperation();           //不可以异步，异步会影响后面的数据库访问程序
            //异步登录
            Action<Action<bool>> myAsy = new Action<Action<bool>>(app.Login);
            myAsy.BeginInvoke(UserLoginCallBack, null, null);
            todayProductionCounterLabel.Text = app.getProductionNum().ToString();
            string position = app.curConfig.scaner_position != "0" ? (app.curConfig.scaner_position == "1" ? "中间" : "最右边") : "最左边";
            SetScannerPositionCallBack(position);
        }
        void GetResourceCallback(bool result)
        {
            if (result)
            {
                this.Invoke(new Action(() => { deviceResourceIdLabel.Text = app.curConfig.resouce_id; }));
            }

        }
        void sr2000wTimOutSetEvent()
        {
            this.Invoke(new Action(() =>
            {
                sr2000wTimelabel.Text = "扫码枪超时时间设置：" + app.curConfig.Sr2000wTimeOut.ToString() + " ms";
            }));
            
        }
        private void UserLoginCallBack(bool obj)
        {
            int number;
            if(obj)
            {
                number=app.getDataByUser(app.curUser.Id);
                app.curUserProductionCounter = number;
                this.Invoke(new Action(() =>
                {
                    currentUserToolStripStatusLabel.Text = "当前员工：" + app.curUser.Id;
                    onProcessNameToolStripStatusLabel.Text = "员工" + app.curUser.Id + "已登录";
                    userlogled.ForeColor = Color.Lime;
                    userProductionCounterLabel.Text = number.ToString();
                }));
            }
        }

        private void Fx5uConnectedCallBack(bool obj)
        {
            
            if(obj)
            {
                this.Invoke(new Action(() =>
                {
                    fx5conled.ForeColor = Color.Lime;
                    onProcessNameToolStripStatusLabel.Text = "PLC已连接";
                }));
                
            }
        }

        private void Sr2000wConnectedCallBack(bool obj)
        {
            if(obj)
            {
                this.Invoke(new Action(() =>
                {
                    keyenceconled.ForeColor = Color.Lime;
                    onProcessNameToolStripStatusLabel.Text = "扫码枪已连接";
                }));
            }
        }

        private void Bt3562ConnectedCallBack(bool obj)
        {
            if (obj)
            {
                hokiconled.ForeColor = Color.Lime;
                onProcessNameToolStripStatusLabel.Text = "电池检测仪已连接";
            }

        }
        #endregion

        //电池数据表插入数据函数
        private void BatteryDGVadd(Battery battery)
        {
            DataTable batteryDataTable = batterydatagridview.DataSource as DataTable;
            DataRow row = batteryDataTable.NewRow();
            row["sn"] = battery.sn;
            row["voltage"] = Math.Round(battery.voltage*VOLTAGE_UNIT,4);
            row["resistance"] = Math.Round(battery.resistance*RESISTANCE_UNIT,4);
            row["result"] = battery.result ? "合格" : "不合格";
            row["opertime"] = battery.operTime.ToString("yyyy-MM-dd HH:mm:ss");
            row["errtype"] = myErrorDictionary[battery.errorType];
            if (battery.operation_id == "O2T")
            {
                if (battery.errorType == BatteryErrorType.OK && !battery.k_flag)  //如果电阻电压正常，而K值异常
                    row["errtype"] = "K值异常";
                else if (battery.errorType != BatteryErrorType.OK && !battery.k_flag)   //如果电阻电压不正常，而且K值异常
                    row["errtype"] += "，K值异常";
                row["K"] = battery.K;
                row["o1_voltage"] = Math.Round(battery.o1_voltage*VOLTAGE_UNIT,4);
                row["o1_date"] = battery.o1_date;
            }
            row["temperature"] = battery.temperature;
            row["origin_voltage"] = Math.Round(battery.origin_voltage*VOLTAGE_UNIT,4);
            row["operation_id"] = battery.operation_id;
            row["user"] = battery.user;
            row["savedflag"] = battery.MesSaved ? "上传成功" : "上传失败";
            row["verifyflag"] = battery.verifyFlag ? "校验成功" : "校验失败";
            row["techId"] = battery.techId;
            row["operation_id"] = battery.operation_id;
            //row["resourceId"] = battery.resourceId;
            this.Invoke(new Action(() =>
            {
                batteryDataTable.Rows.InsertAt(row, 0);
                if (batteryDataTable.Rows.Count > app.curConfig.BatteryDisplayNum)
                    batteryDataTable.Rows.RemoveAt(app.curConfig.BatteryDisplayNum);
                if(batterydatagridview.RowCount!=0)
                    batterydatagridview.FirstDisplayedScrollingRowIndex = 0;
            }));
        }

        private void LogDGVadd(Log myLog)
        {
            DataTable logDataTable = logDataGridView.DataSource as DataTable;
            DataRow row = logDataTable.NewRow();
            row["operTime"] = myLog.operTime;
            row["fName"] = myLog.fName;
            row["process"] = myLog.process;
            row["userId"] = myLog.userId;
            row["resourceId"] = myLog.resourceId;
            row["sn"] = myLog.sn;
            row["result"] = myLog.result;
            row["message"] = myLog.message;
            row["userName"] = myLog.userName;
            row["shop_order"] = myLog.shop_order;
            row["tech_no"] = myLog.tech_no;
            row["inspection_Item"] = myLog.inspection_Item;
            row["inspection_Desc"] = myLog.inspection_Desc;
            row["standard"] = myLog.standard;
            row["upper_limit"] = myLog.upper_limit;
            row["lower_limit"] = myLog.lower_limit;
            row["flag"] = myLog.flag;
            row["ng_code"] = myLog.ng_code;
            row["resistance"] = myLog.resistance;
            row["voltage"] = myLog.voltage;
            row["status"] = myLog.status;
            row["status_des"] = myLog.status_des;
            row["err_code"] = myLog.err_code;
            row["err_desc"] = myLog.err_desc;
            row["temperature"] = myLog.tempearture;
            row["origin_voltage"] = myLog.origin_voltage;
            row["o1_voltage"] = myLog.o1_voltage;
            row["o1_date"] = myLog.o1_date;
            row["K"] = myLog.K;
            this.Invoke(new Action(() =>
            {
                logDataTable.Rows.InsertAt(row, 0);
                if (logDataTable.Rows.Count > app.curConfig.LogDisplayNum)
                    logDataTable.Rows.RemoveAt(app.curConfig.LogDisplayNum);
                if(logDataGridView.RowCount!=0)
                    logDataGridView.FirstDisplayedScrollingRowIndex = 0;
            }));
        }
        private void PLCstartbtn_Click(object sender, EventArgs e)
        {
            //订阅错误抛出事件
            app.ErrorEventHandler = new Action<string>(ErrorCallBack);
            //一堆事件订阅
            app.workRunWatchEventHandler = ControlWorkRunWatch;
            app.BatteryOnPositionEvent = BatteryOnPositionShowMainFormCallBack;
            app.KeyenceReceivedDataVerifiedEvent = KeyenceReceivedDataVerifiedShowMainFormCallBack;
            app.SendBatteryDownShowEvent = SendBatteryDownShowMainFormCallBack;
            app.ScannerPositionEvent = SetScannerPositionCallBack;
            app.StopEvent = StopCallBack;
            app.TryScanEvent = TryScanEventShowMainFormCallback;
            bool isBatteryFull = isBatteryNoFullCheckBox.Checked;
            app.AutoRunInit(isBatteryFull ,new Action<bool>(AutoRunInitDownCallBack));
        }

        private void TryScanEventShowMainFormCallback(int obj)
        {
            this.Invoke(new Action(() =>
            {
                onProcessNameToolStripStatusLabel.Text = "扫码超时，正在尝试第" + obj.ToString() + "次扫码......";
            }));
        }

        //工作故障提示
        private void ErrorCallBack(string obj)
        {
            this.Invoke(new Action(() =>
            {
                errortextBox.AppendText(obj + "\r\n");
                error_image.Visible = true;
                ok_image.Visible = false;
            }));
        }
        private void AutoRunInitDownCallBack(bool obj)
        {
            if(obj)
            {
                this.Invoke(new Action(() =>
                {
                    batteryInLed.ForeColor = Color.Lime;
                    PLCstartbtn.Enabled = false;
                    ShowCurTem();
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                  {
                      onProcessNameToolStripStatusLabel.Text = "系统未准备，请检查设备连接情况和员工是否登录";
                  }));
            }
        }
        private void ShowCurTem(double curTem=0)
        {
            if(curTem==0)
                curTem = app.GetTemperature();
            
            curTemLabel.Text = curTem.ToString();
        }
        private void ControlWorkRunWatch(bool flag)
        {
            this.Invoke(new Action(() =>
            {
                if (flag)    //如果开始
                {
                    workRunWatch.Start();
                }
                else
                {
                    workRunWatch.Stop();
                }

            }));
            
        }

        private void StopCallBack()
        {
            this.Invoke(new Action(() =>
            {
                onProcessNameToolStripStatusLabel.Text = "设备停止工作......";
                app.ErrorEventHandler = null;
                //一堆事件订阅
                app.BatteryOnPositionEvent = null;
                app.KeyenceReceivedDataVerifiedEvent = null;
                app.SendBatteryDownShowEvent = null;
                app.StopEvent = null;
                app.ScannerPositionEvent = null;
                app.TryScanEvent = null;
                //制作停止log
                //app.CreateSystemStopLog();
                workRunWatch.Reset();
                PLCstartbtn.Enabled = true;
                //重置
                ResetLabelListText(snLabelList, "");//重置所有序列号
                ResetLabelListText(descList, "");
                ResetLabelListColor(verifyLedList, Color.Gray);//重置所有校验灯
                ResetLabelListText(voltageLabelList, "");//重置所有电压
                ResetLabelListText(KLabelList, ""); //重置所有K值
                ResetLabelListText(resistanceLabelList, "");//重置所有电阻
                foreach (List<Label> ledList in batteryLedList)//重置所有灯
                {
                    foreach (Label ledItem in ledList)
                    {
                        ledItem.ForeColor = Color.Gray;
                    }
                }
                //重置状态指示灯
                batteryInLed.ForeColor = Color.Gray;
                scanLed.ForeColor = Color.Gray;
                measureLed.ForeColor = Color.Gray;
                isBatteryNoFullCheckBox.Checked = false;
            }));
        }
        private void KeyenceReceivedDataVerifiedShowMainFormCallBack()
        {
            int iStart = app.curConfig.BatteryNum == 4 ? 0 : 1;
            this.Invoke(new Action(() =>
            {
                for (int i = 0; i < app.curBattery.Length; i++)
                {
                    //app.logStreamWriter.WriteLine(DateTime.Now.ToString()+"正在显示电池" + i.ToString() + "的校验结果："+app.curBattery[i].verifyFlag.ToString());
                    snLabelList[iStart + i].Text = app.curBattery[i].sn;
                    descList[iStart + i].Text = app.curBattery[i].desc;
                    verifyLedList[iStart + i].ForeColor = app.curBattery[i].verifyFlag ? Color.Lime : Color.Red;
                }
                //onProcessNameToolStripStatusLabel.Text = app.normalFlag ? "测量电池信息......" : "有电池非本机器生产，正在切换......";
                scanLed.ForeColor = Color.Gray;
                measureLed.ForeColor = Color.Lime;
                //batteryInLed.ForeColor = app.normalFlag ? Color.Gray : Color.Lime;
                onProcessNameToolStripStatusLabel.Text = "开始测量.....";
                ShowCurTem();
            }));
        }
        private void BatteryOnPositionShowMainFormCallBack()
        {
            this.Invoke(new Action(() =>
            {
                //重置
                ResetLabelListText(snLabelList, "");//重置所有序列号
                ResetLabelListColor(verifyLedList, Color.Gray);//重置所有校验灯
                ResetLabelListText(descList, "");   //重置所有电池描述
                ResetLabelListText(voltageLabelList, "");//重置所有电压
                ResetLabelListText(KLabelList, ""); //重置所有K值
                ResetLabelListText(resistanceLabelList, "");//重置所有电阻
                foreach (List<Label> ledList in batteryLedList)//重置所有灯
                {
                    foreach (Label ledItem in ledList)
                    {
                        ledItem.ForeColor = Color.Gray;
                    }
                }
                batteryInLed.ForeColor = Color.Gray;//熄灭进料灯
                measureLed.ForeColor = Color.Gray;//熄灭测量灯
                scanLed.ForeColor = Color.Lime;//点亮扫码灯 
                onProcessNameToolStripStatusLabel.Text = "开始扫码......";
            }));
        }
        private void SendBatteryDownShowMainFormCallBack()
        {
            int iStart = (app.curConfig.BatteryNum==4)?0:1;
            this.Invoke(new Action(() =>
            {
                if (app.curBattery[app.curBatteryi - 1].desc == "电压电阻测量异常")
                {
                    descList[iStart + app.curBatteryi - 1].Text = app.curBattery[app.curBatteryi - 1].desc+",请重新测量";
                    verifyLedList[iStart + app.curBatteryi - 1].ForeColor = Color.Red;
                }
                else
                {
                    resistanceLabelList[iStart + app.curBatteryi - 1].Text = Math.Round(app.curBattery[app.curBatteryi - 1].resistance * RESISTANCE_UNIT, 4).ToString();
                    voltageLabelList[iStart + app.curBatteryi - 1].Text = Math.Round(app.curBattery[app.curBatteryi - 1].voltage * VOLTAGE_UNIT, 4).ToString();
                    if (app.curConfig.operation_id == "O2T")
                        KLabelList[iStart + app.curBatteryi - 1].Text = (app.curBattery[app.curBatteryi - 1].K).ToString();
                    batteryLedList[iStart + app.curBatteryi - 1][1].ForeColor = app.curBattery[app.curBatteryi - 1].result ? Color.Lime : Color.Red;
                    int voltageJudge = (Convert.ToInt32(app.curBattery[app.curBatteryi - 1].errorType)) & 12;
                    int resistanceJudge = (Convert.ToInt32(app.curBattery[app.curBatteryi - 1].errorType)) & 3;

                    batteryLedList[iStart + app.curBatteryi - 1][2].ForeColor = (voltageJudge == 0) ? Color.Lime : ((voltageJudge == 8) ? Color.Red : Color.Blue); //电压灯
                    batteryLedList[iStart + app.curBatteryi - 1][3].ForeColor = (resistanceJudge == 0) ? Color.Lime : ((resistanceJudge == 2) ? Color.Red : Color.Blue); //电阻灯
                    batteryLedList[iStart + app.curBatteryi - 1][0].ForeColor = app.curBattery[app.curBatteryi - 1].MesSaved ? Color.Lime : Color.Red;
                    if (app.curConfig.operation_id == "O2T")
                        batteryLedList[iStart + app.curBatteryi - 1][4].ForeColor = app.curBattery[app.curBatteryi - 1].k_flag ? Color.Lime : Color.Red;
                }
                measureLed.ForeColor = Color.Gray;
                scanLed.ForeColor = Color.Gray;
                batteryInLed.ForeColor = Color.Lime;
            }));
        }
        private void ResetLabelListText(List<Label> labelList,string defaultText)
        {
            this.Invoke(new Action<List<Label>, string>((List<Label> l, string dt) =>
             {
                 foreach (Label label in l)
                 {
                     label.Text = dt;
                 }
             }), labelList, defaultText);
        }
        private void ResetLabelListColor(List<Label> labelList,Color color)
        {
            this.Invoke(new Action<List<Label>, Color>((List<Label> l, Color cr) =>
            {
                foreach (Label label in l)
                {
                    label.ForeColor = cr;
                }
            }), labelList, color);
        }
        private void sTimer_Tick(object sender, EventArgs e)
        {
            softRunTimeToolStripStatusLabel.Text = "软件运行时间："+softRunWatch.Elapsed.ToString(@"hh\:mm\:ss");
            if (app.plcState == PlcState.STOP)
                workRunTimeToolStripStatusLabel.Text = "生产时间：";
            else
                workRunTimeToolStripStatusLabel.Text = "生产时间：" + workRunWatch.Elapsed.ToString(@"hh\:mm\:ss");
        }
        private void HokiConShowCallBack(bool connectedFlag)
        {
            this.Invoke(new Action(() =>
            {
                hokiconled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
                onProcessNameToolStripStatusLabel.Text = connectedFlag ? "电池检测仪已连接" : "电池检测仪已断开连接";
            }));
        }
        private void KeyenceConShowCallBack(bool connectedFlag)
        {
            this.Invoke(new Action(() =>
            {
                keyenceconled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
                onProcessNameToolStripStatusLabel.Text = connectedFlag ? "扫码枪已连接" : "扫码枪已断开连接";
            }));
        }
        private void MitSubishiConShowCallBack(bool connectedFlag)
        {
            this.Invoke(new Action(() =>
            {
                fx5conled.ForeColor = connectedFlag ? Color.Lime : Color.Gray;
                onProcessNameToolStripStatusLabel.Text = connectedFlag ? "PLC已连接" : "PLC已断开连接";
            }));
            
        }
        private void SetTechStandardCallBack(bool flag)
        {
            this.Invoke(new Action(() =>
            {
                minVoltagelabel.Text = Math.Round(app.curConfig.MinVoltage*VOLTAGE_UNIT,4).ToString();
                maxVoltagelabel.Text = Math.Round(app.curConfig.MaxVoltage*VOLTAGE_UNIT,4).ToString();
                minResistancelabel.Text = Math.Round(app.curConfig.MinResistance*RESISTANCE_UNIT,4).ToString();
                maxResistancelabel.Text = Math.Round(app.curConfig.MaxResistance* RESISTANCE_UNIT,4).ToString();
                techIdLabel.Text = app.curConfig.tech_no;
                minTemLabel.Text = app.curConfig.minTem.ToString();
                maxTemLabel.Text = app.curConfig.maxTem.ToString();
                temCoefficientLabel.Text = app.curConfig.temCoeff.ToString();
                processIdLabel.Text = app.curConfig.operation_id;
                if(app.curConfig.operation_id=="O2T")
                {
                    minKLabel.Text = app.curConfig.minK.ToString();
                    maxKLabel.Text = app.curConfig.maxK.ToString();
                }
                else if (app.curConfig.operation_id=="O1T")
                {
                    minKLabel.Text = "";
                    maxKLabel.Text = "";
                }
            }));
        }
        private void SetScanNumCallBack(int scanNum)
        {
            if(scanNum==2)
            {
                for(int i=0;i<4;i+=3)
                {
                    snLabelList[i].Enabled = false;
                    resistanceLabelList[i].Enabled = false;
                    voltageLabelList[i].Enabled = false;
                    descList[i].Enabled = false;
                    KLabelList[i].Enabled = false;
                    verifyLedList[i].Enabled = false;
                    foreach(Label led in batteryLedList[i])
                    {
                        led.Enabled = false;
                    }
                }
                for (int i = 1; i < 3; i++)
                {
                    snLabelList[i].Enabled = true;
                    resistanceLabelList[i].Enabled = true;
                    voltageLabelList[i].Enabled = true;
                    KLabelList[i].Enabled = true;
                    verifyLedList[i].Enabled = true;
                    descList[i].Enabled = true;
                    foreach (Label led in batteryLedList[i])
                    {
                        led.Enabled = true;
                    }
                }
            }
            if(scanNum==4)
            {
                for (int i = 0; i < 4; i++)
                {
                    snLabelList[i].Enabled = true;
                    resistanceLabelList[i].Enabled = true;
                    voltageLabelList[i].Enabled = true;
                    descList[i].Enabled = true;
                    verifyLedList[i].Enabled = true;
                    foreach (Label led in batteryLedList[i])
                    {
                        led.Enabled = true;
                    }
                }
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (app.plcState == PlcState.RUN)
            {
                e.Cancel = true;
                MessageBox.Show("设备正在运行，请先停机再退出软件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SoftConfig.SaveAllConfig(app.curConfig);
                app.CloseMySqlConnection();
                app.logsave();
                app.productionLogSave();
            }
        }

        DataForm dataForm;
        private void Data_MenuItem_Click(object sender, EventArgs e)
        {
            if(dataForm!=null)
                if (!dataForm.IsDisposed)
                    return;
            if(app.plcState==PlcState.STOP)
            {
                dataForm = new DataForm(app);
                dataForm.ShowDialog();
            }
            else
                MessageBox.Show("设备未停机，请先停止运行再操作！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void batterydatagridview_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                if ((string)(batterydatagridview.Rows[e.RowIndex].Cells["result"].Value) == "不合格")
                {
                    batterydatagridview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                }
            }
            catch
            {
                return;
            }
        }

        private void error_image_Click(object sender, EventArgs e)
        {
            errortextBox.Clear();
            error_image.Visible = false;
            ok_image.Visible = true;
            if(app.plcState==PlcState.STOP)
            {
                app.statusUpload.UploadStatus(DeviceStatus.D);
            }
        }

        private void ledResetBtn_Click(object sender, EventArgs e)
        {
            app.ResetLeds();
        }

        private void batterydatagridview_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if ((string)(batterydatagridview.Rows[e.RowIndex].Cells["result"].Value) == "不合格")
            {
                batterydatagridview.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
            }
            //batterydatagridview.FirstDisplayedScrollingRowIndex = 0;
        }
    }
}
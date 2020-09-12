#define TEST
//#define SLEEP
#define RUNCHECKENABLE
//#define LOADWITHBATTERYANDLOGSHOWINMAINFORM
#define RESETPLC
#define BT3562ENABLE
#define REDINIT

//#define VERIFYALWAYSTRUE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Model;
using Services;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
using DAL;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Timers;

namespace BLL
{
    /// <summary>
    /// 该类是程序的主逻辑
    /// </summary>
    public class App
    {
        private Mutex mut;
        private List<string> MySns;
        private List<string> VandR;
        private bool verifyFalseFlag;
        static int VOLTAGE_UNIT=1000;
        static int tryScanNum = 4;
        private int scanNum = 0;
        static int RESISTANCE_UNIT=1000;
        public Bt3562 bt3562;
        public Sr2000w sr2000w;
        public String curIOAddrStr;
        public Fx5u fx5u;
        public User curUser;
        public SoftConfig curConfig;
        public Battery[] curBattery;
        public StatiscticsMonitor myMonitor;
        public DataTable batteryDataTable;
        public DataTable logDataTable;
        public bool curLevel;
        public Dictionary<BatteryErrorType, string> myErrorDictionary;
        public PlcState plcState;
        //public bool normalFlag;
        public int curBatteryi;
        private bool supportOnPosition;
        private bool probeOnPosition;
        private bool scanDown;
        private List<bool> statistic;
        private long timeStart;
        private long timeStop;
        public TimeSpan timeInterval;
        public Action<bool, string> StandardGetDown;
        System.Timers.Timer sr2000wTimer;
        LedFlash ledFlasher;
        DownLedFlash downLedFlasher;
        public Action<int> TryScanEvent;
        private Temperature temperature;
        private MesConnect mes;
        public UploadStatusThread statusUpload;
        public Action SendBatteryDownShowEvent;

        public Action<string> ErrorEventHandler;

        public Action BatteryOnPositionEvent;
        public Action KeyenceReceivedDataVerifiedEvent;
        public Action StopEvent;
        public Action<Log> LogShowInDataGridView;
        public Action<Battery> batteryInfoShowInDataGridView;
        public StreamWriter logStreamWriter = new StreamWriter(@"log.txt", false);
        public bool isTemperatureConnected;
        private XmlOperation xmlOper;
        public int curUserProductionCounter;
        private void LogDGVadd(Log myLog)
        {
            LogShowInDataGridView?.BeginInvoke(myLog, null, null);
            //DataRow row = logDataTable.NewRow();
            //DataRow row = logDataTable.NewRow();
            //row["operTime"] = myLog.operTime;
            //row["fName"] = myLog.fName;
            //row["process"] = myLog.process;
            //row["userId"] = myLog.userId;
            //row["resourceId"] = myLog.resourceId;
            //row["sn"] = myLog.sn;
            //row["result"] = myLog.result;
            //row["message"] = myLog.message;
            //row["userName"] = myLog.userName;
            //row["shop_order"] = myLog.shop_order;
            //row["tech_no"] = myLog.tech_no;
            //row["inspection_Item"] = myLog.inspection_Item;
            //row["inspection_Desc"] = myLog.inspection_Desc;
            //row["standard"] = myLog.standard;
            //row["upper_limit"] = myLog.upper_limit;
            //row["lower_limit"] = myLog.lower_limit;
            //row["flag"] = myLog.flag;
            //row["ng_code"] = myLog.ng_code;
            //row["resistance"] = myLog.resistance;
            //row["voltage"] = myLog.voltage;
            //row["status"] = myLog.status;
            //row["status_des"] = myLog.status_des;
            //row["err_code"] = myLog.err_code;
            //row["err_desc"] = myLog.err_desc;
            //logDataTable.Rows.InsertAt(row, 0);
            ////logDataTable.Rows.InsertAt(row, 0);
            //if(logDataTable.Rows.Count>curConfig.LogDisplayNum) //如果比要显示的行数多了，则踢掉最后一行
            //    logDataTable.Rows.RemoveAt(curConfig.LogDisplayNum);
        }

        private void BatteryDGVadd(Battery battery)
        {
            DataRow row = batteryDataTable.NewRow();
            row["sn"] = battery.sn;
            row["voltage"] = battery.voltage;
            row["resistance"] = battery.resistance;
            row["result"] = battery.result ? "合格" : "不合格";
            row["opertime"] = battery.operTime.ToString();
            row["errtype"] = myErrorDictionary[battery.errorType];
            if(curConfig.operation_id=="O2T")
            {
                if (battery.errorType == BatteryErrorType.OK && !battery.k_flag)  //如果电阻电压正常，而K值异常
                    row["errtype"] = "K值异常";
                else if (battery.errorType != BatteryErrorType.OK && !battery.k_flag)   //如果电阻电压不正常，而且K值异常
                    row["errtype"] += "，K值异常";
                row["K"] = battery.K.ToString();
                row["o1_voltage"] = battery.o1_voltage;
                row["o1_date"] = battery.o1_date;
            }
            row["temperature"] = battery.temperature.ToString();
            row["origin_voltage"] = battery.origin_voltage.ToString();
            row["operation_id"] = battery.operation_id;
            row["user"] = battery.user;
            row["savedflag"] = battery.MesSaved ? "上传成功" : "上传失败";
            row["verifyflag"] = battery.verifyFlag ? "校验成功" : "校验失败";
            row["techId"] = battery.techId;
            batteryDataTable.Rows.InsertAt(row, 0);
            if (batteryDataTable.Rows.Count > curConfig.BatteryDisplayNum)
                batteryDataTable.Rows.RemoveAt(curConfig.BatteryDisplayNum);
        }

        private void UploadErrorA(string error_code,string error_desc)
        {
            errorFlag = true;
            Log myLog = new Log();
            statusUpload.UploadStatus(DeviceStatus.C);
            statusUpload.UploadError(error_code, error_desc,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
        }
        public App()
        {
            VandR = new List<string>();
            bt3562 = new Bt3562();
            sr2000w = new Sr2000w();
            fx5u = new Fx5u();
            curUser = new User();
            curBattery = null;
            myMonitor = new StatiscticsMonitor();
            //读取配置文件到内存
            curConfig = SoftConfig.Load();
            batteryDataTable = new DataTable();
            logDataTable = new DataTable();
            plcState = PlcState.STOP;
            ledFlasher = new LedFlash(fx5u);
            downLedFlasher = new DownLedFlash(fx5u);
            isTemperatureConnected = false;
            mes = new MesConnect(curConfig.tech_no,curConfig.operation_id);
            temperature = new Temperature();
            statusUpload = new UploadStatusThread(mes);
            statusUpload.LogShowEventHandler = LogDGVadd;
            xmlOper = new XmlOperation();
            mut = new Mutex();
            MySns = new List<string>();
        }

        public int getDataByUser(string user)
        {
            return xmlOper.loadDataByUser(user);
        }
        private string curoperationId;
        public bool localDataUpload(DataRow row,ref string msg)
        {
            bool result = false;
            Log myLog = null;
            if((string)row["operation_id"] == "O2T")    //制作O2请求体
            {
                UploadBatteryO2TDataClass request = new UploadBatteryO2TDataClass();
                request.DATA1 = row["voltage"].ToString();
                request.DATA2 = row["resistance"].ToString();
                request.DATA03 = row["techId"].ToString();
                request.DATA4 = row["temperature"].ToString();
                request.DATA02 = row["shop_order"].ToString();
                request.DATA12 = row["origin_voltage"].ToString();
                request.MATERIAL_TYPE= row["techId"].ToString();
                request.DATA6 = row["o1_voltage"].ToString();
                request.DATA3 = row["K"].ToString();
                request.DATA16 = row["temCoeff"].ToString();
                string flag = ((string)row["result"] == "合格") ? "OK" : "NG";
                string ng_code = locaErrtype2MesErrtype(row["errtype"].ToString());
                result=mes.UploadBatteryO2TAgain(row["sn"].ToString(), flag, row["opertime"].ToString(),
                    ng_code, row["origin_voltage"].ToString(),row["operation_id"].ToString(), request, ref msg, ref myLog);
            }
            else    //制作O1请求体
            {
                UploadBatteryDataClass request = new UploadBatteryDataClass();
                request.DATA1 = row["voltage"].ToString();
                request.DATA2 = row["resistance"].ToString();
                request.DATA03 = row["techId"].ToString();
                request.DATA4 = row["temperature"].ToString();
                request.DATA02 = row["shop_order"].ToString();
                request.DATA16 = row["temCoeff"].ToString();
                request.MATERIAL_TYPE = row["techId"].ToString();
                request.DATA12 = row["origin_voltage"].ToString();
                string flag = ((string)row["result"] == "合格")?"OK":"NG";
                string ng_code = locaErrtype2MesErrtype(row["errtype"].ToString());
                result = mes.UploadBatteryAgain(row["sn"].ToString(), flag, row["opertime"].ToString(),
                    ng_code, row["origin_voltage"].ToString(),row["operation_id"].ToString(),request, ref msg, ref myLog);
            }
            if (result)  //如果请求上传成功，改变本地数据的状态
                DataManager.updateBatterySavedflag(row["sn"].ToString(),row["operation_id"].ToString());
            return result;
        }
        private string locaErrtype2MesErrtype(string localErrtype)
        {
            if (localErrtype == "正常")
                return "";
            //List<string> errList = new List<string>();
            //if (localErrtype.IndexOf("电阻值") != -1) //内阻异常
            //    errList.Add("P_OCVNG_NZ");
            //if (localErrtype.IndexOf("大于正常电压") != -1)  //高压错误
            //    errList.Add("P_OCVNG_GY");
            //if (localErrtype.IndexOf("低于正常电压") != -1)  //低压错误
            //    errList.Add("P_OCVNG_DY");
            //if (localErrtype.IndexOf("K值") != -1) //K值错误
            //    errList.Add("P_OCVNG_KZ");
            //string[] errArray=errList.ToArray<string>();
            //string mesErrtype = string.Join(",", errArray);
            //return mesErrtype;
            if (localErrtype.IndexOf("K值") != -1)
                return "P_OCVNG_KZ";
            if(localErrtype.IndexOf("大于正常电压")!=-1)
                return "P_OCVNG_GY";
            if (localErrtype.IndexOf("低于正常电压") != -1)
                return "P_OCVNG_DY";
            return "P_OCVNG_NZ";
        }
        private void GetSnsForGetStandard(string obj)
        {
            //释放计时器
            sr2000wTimer?.Stop();
            sr2000wTimer?.Dispose();
#if NOMES
            //产生随机数要用的类
            Random rd = new Random();
            //制作随机的电压电阻上下限值
            //电压上限值
            curConfig.MaxVoltage = rd.NextDouble() * 0.1 + 3.6;
            curConfig.MinVoltage = rd.NextDouble() * 0.1 + 3.5;
            curConfig.MinResistance = rd.NextDouble() * 0.01 + 0.35;
            curConfig.MaxResistance = rd.NextDouble() * 0.01 + 0.36;
            StandardGetDown.Invoke(true, "Success");
#else
            string sns = obj.Substring(0, obj.Length - 1);
            //先获取到电池型号
            string shop_order = "";
            string tech_no = "";
            string o1_voltage = "";
            string o1_date = "";
            string msg = "";
            List<GetTechnologyStandardResponseClass> techs = null;

            Log myLog = new Log();
            bool result = mes.SetupOperation(this.curoperationId, ref msg, ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog, null, null);
            result = mes.GetData(sns, true,ref msg, ref shop_order, ref tech_no,ref o1_voltage,ref o1_date,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
            if (result) //如果获得电池信息成功返回
            {
                if(this.curoperationId=="O2T"&&o1_voltage==null)        //明显不能做O2
                {
                    StandardGetDown?.BeginInvoke(false, "电池不能做O2工序", null, null);
                }
                else
                {
                    curConfig.shop_order = shop_order;
                    curConfig.tech_no = tech_no;
                    result = mes.GetTechnologyStandard(tech_no, ref msg, ref techs, ref myLog);
                    LogShowInDataGridView?.BeginInvoke(myLog, null, null);
                    if (result)
                    {
                        string prefix = this.curoperationId;
                        curConfig.operation_id = this.curoperationId;
                        //如果获得工艺标准成功返回
                        techs.ForEach(delegate (GetTechnologyStandardResponseClass technology)
                        {
                            switch (technology.INSPECTION_ITEM.Replace(prefix, ""))
                            {
                                case "_DY":
                                    curConfig.MaxVoltage = Math.Round(double.Parse(technology.UPPER_LIMIT) / VOLTAGE_UNIT,7);
                                    curConfig.MinVoltage = Math.Round(double.Parse(technology.LOWER_LIMIT) / VOLTAGE_UNIT,7);
                                    break;
                                case "_DZ":
                                    curConfig.MaxResistance = Math.Round(double.Parse(technology.UPPER_LIMIT) / RESISTANCE_UNIT,7);
                                    curConfig.MinResistance = Math.Round(double.Parse(technology.LOWER_LIMIT) / RESISTANCE_UNIT,7);
                                    break;
                                case "_WD":
                                    curConfig.maxTem = double.Parse(technology.UPPER_LIMIT);
                                    curConfig.minTem = double.Parse(technology.LOWER_LIMIT);
                                    break;
                                case "_WDBCZ":
                                    curConfig.temCoeff = double.Parse(technology.STANDARD);
                                    break;
                                case "_KZ":
                                    curConfig.maxK = double.Parse(technology.UPPER_LIMIT);
                                    curConfig.minK = double.Parse(technology.LOWER_LIMIT);
                                    break;
                            }
                        });
                        StandardGetDown?.BeginInvoke(true, msg, null, null);
                    }
                    else
                    {
                        StandardGetDown?.BeginInvoke(false, msg, null, null);
                    }
                }
            }
            else
            {
                StandardGetDown?.BeginInvoke(false, msg,null,null);
            }
#endif
            //发送电池退回指令
            fx5u.SendCmd("D136");
            //扫码的数量回到原值    
            SetScanNum(curConfig.BatteryNum, false);
            GetTechStdDown();
            //所有接口全部拆除
            fx5u.EventMeditator = null;
            sr2000w.DataReceivedEventHandler = null;
        }
        public void GetResource(Action<bool> DownCallBack)
        {
            string Msg = "";
            string resource_id = "";
            Log myLog = new Log();
            bool result = mes.GetResource(ref Msg, ref resource_id,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
            if (result) //得到了设备资源号
            {
                curConfig.resouce_id = resource_id;
                DownCallBack(true);
                statusUpload.UploadStatus(DeviceStatus.D);
                return;
            }
        }
        public void SetScanNum(int v, bool Save)
        {
            sr2000w.DataReceivedEventHandler = null;
            sr2000w.SendCmd("WP,250," + v.ToString());
            if (Save)
            {
                curConfig.BatteryNum = v;
                //SoftConfig.SetAppSettingsValue("BatteryNum", v.ToString(), false);
            }
        }
        public bool ConnectTemperature(string com="")
        {
            bool result;
            if (com == "")
            {
                result = temperature.SerialModBus(curConfig.temperatureCom);
                if (result)
                    isTemperatureConnected = true;
            }
            else
            {
                result = temperature.SerialModBus(com);
                if (result)
                {
                    isTemperatureConnected = true;
                    curConfig.temperatureCom = com;
                }
            }
            return result;
        }
        public void Init()
        {

            batteryDataTable.Columns.Add(new DataColumn("sn", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("voltage", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("resistance", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("result", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("errtype", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("user", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("savedflag", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("verifyflag", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("opertime", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("techId", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("temperature", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("origin_voltage", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("o1_voltage", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("K", typeof(double)));
            batteryDataTable.Columns.Add(new DataColumn("o1_date", typeof(string)));
            batteryDataTable.Columns.Add(new DataColumn("operation_id", typeof(string)));


            logDataTable.Columns.Add(new DataColumn("operTime", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("fName", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("process", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("userId", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("resourceId", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("sn", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("result", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("message", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("userName", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("shop_order", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("tech_no", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("inspection_Item", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("inspection_Desc", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("standard", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("upper_limit", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("lower_limit", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("flag", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("ng_code", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("resistance", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("voltage", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("status", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("status_des", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("err_code", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("err_desc", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("temperature", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("origin_voltage", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("o1_voltage", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("K", typeof(string)));
            logDataTable.Columns.Add(new DataColumn("o1_date", typeof(string)));

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
            logStreamWriter.WriteLine(DateTime.Now.ToString() + "进去初始化函数");


            

#if LOADWITHBATTERYANDLOGSHOWINMAINFORM
            //读取电池数据
            batteryDataTable= DataManager.GetBatteryInfoBylimit(20);
            //读取日志数据
#endif
        }
        public void setUpOperation()
        {
            Log myLog = new Log();
            string msg = "";
            mes.SetupOperation(curConfig.operation_id, ref msg, ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog, null, null);
        }
        public void logsave()
        {
            logStreamWriter.Close();
        }
        public bool Connect(Device device, string param, ref string errMsg)
        {

            if (device.Connect(param, curConfig, ref errMsg))
                return true;
            return false;
        }
        //使用本地配置连接某个设备
        public void Connect(Device device, Action<bool> CallBack)
        {
            if (device.IsConnected)
            {
                //创建回调参数
                bool obj = true;
                //异步回调
                CallBack?.BeginInvoke(obj, null, null);
                return;
            }
            else
            {
                //创建异步委托
                Action<SoftConfig, Action<bool>> myAsy = new Action<SoftConfig, Action<bool>>(device.Connect);
                //异步执行委托
                myAsy.BeginInvoke(curConfig, CallBack, null, null);
                return;
            }
        }

        public void FTune()
        {
            sr2000w.SendCmd("FTUNNE");
        }
        public void ConnectBt3562()
        {
            string msg = "";
            bool result=bt3562.ConnectBt3562(curConfig.Bt3562Com, ref msg);
            if(result)
            {
                //如果成功连接，还要判断是否是连接正确
                bt3562.DataReceivedEventHandler = TestBt3562Connect;
                bt3562.SendCmd("*IDN?");
            }
        }

        public void ConnectBt3562(string com,Action<string> ConnectCallBack)
        {
            string msg = "";
            bool result = bt3562.ConnectBt3562(com, ref msg);
            if (result)
            {
                //如果成功连接，还要判断是否是连接正确
                bt3562.DataReceivedEventHandler = ConnectCallBack;
                bt3562.SendCmd("*IDN?");
            }
        }
        public Action<bool> Bt3562ConDownEvent;
        private void TestBt3562Connect(string data)
        {
            if (data.StartsWith("HIOKI"))
            {
                bt3562.IsConnected = true;
                //连接正确
                Bt3562ConDownEvent?.BeginInvoke(true,null,null);
            }
        }

        //登录
        public void Login(string id, Action<bool, string> CallBack)
        {
            string Msg = "";
            string UserName = "";
            Log myLog = new Log();
            //登录
            //result 表示是否登录成功，Msg表示返回的消息
            bool result = mes.Login(id,ref Msg,ref UserName,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
            //DataManager.LogWrite(myLog);
            //如果登录成功
            if (result)
            {
                //对上一个用户的数据进行保存
                if(curUser.Id!="root")
                {
                    xmlOper.setDataByUser(curUser.Id, curUserProductionCounter);
                }
                curConfig.UserId = id;
                curConfig.UserName = UserName;
                curUser = User.SetUser(id, UserName);
                //创建登录成功日志
                //CreateUserLoginLog();
            }
            //返回回调函数
            CallBack?.BeginInvoke(result, Msg, null, null);
        }
        
        //首次开软件登录 
        public void Login(Action<bool> CallBack)
        {
            string Msg = "";
            string UserName = "";
#if NOMES
            string result = "Success";
            loginflag = true;
#else
            Log myLog = new Log();
            //登录
            bool result = mes.Login(curConfig.UserId, ref Msg, ref UserName,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
#endif
            if (result)
            {
                curUser = User.SetUser(curConfig.UserId, UserName);
                //CreateUserLoginLog();
            }
            //登录完成返回回调函数
            CallBack?.BeginInvoke(result, null, null);
        }

        private void GetTechStdDown()
        {
            //把之前的监控器地址注销掉
            fx5u.FreeDeviceStatus();
            //注册新的监控器地址
            fx5u.EntryDeviceStatus(curConfig);
        }
        public bool GetTechStdInit(string operationId,ref string errMsg)
        {
            //检查是否满足运行条件
            if (!curUser.IsLoggined || !fx5u.IsConnected || !sr2000w.IsConnected)
            {
                errMsg = "员工未登录或设备未连接";
                return false;
            }
            this.curoperationId = operationId;
            //把之前的监控器地址注销掉
            fx5u.FreeDeviceStatus();
            //注册新的监控器地址
            string strLabel = "D135";
            int iNum = 1;
            int iConMonitorCycle = 1;
            int[] arrDeviceValue = { 1 };
            fx5u.EnteryDeviceStatus(strLabel, iNum, iConMonitorCycle, ref arrDeviceValue[0]);
            //部署动作线程
            fx5u.EventMeditator = GetTechStdThread;
            //发送正常工作指令，启动上层
            fx5u.SendCmd("D134");
            return true;
        }
        public void GetTechStdThread(PlcLabel plclabel)
        {
            switch(plclabel)
            {
                //上层电池到位了
                case PlcLabel.D135:
                    //D101复位0,避免重复触发
                    fx5u.ResetRegister("D135", 0);
                    //先设置扫码数量
                    SetScanNum(1, false);
                    //等待扫码枪处理完这条信息
                    Thread.Sleep(10);
                    //拔掉fx5u的监控接口
                    fx5u.EventMeditator = null;
                    //设置扫码枪的接收函数
                    sr2000w.DataReceivedEventHandler = new Action<string>(GetSnsForGetStandard);
                    //设置定时器
                    sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut); //4秒钟
                    sr2000wTimer.Elapsed += Sr2000wGetSnTimeOut;
                    sr2000wTimer.AutoReset = false;
                    sr2000wTimer.Start();
                    //发送扫码指令
                    string sns = sr2000w.SendCmd("LON");
                    if (CheckIsSns(sns))
                    {
                        sr2000w.DataReceivedEventHandler = null;
                        GetSnsForGetStandard(sns);
                    }
                    break;
            }
        }
        //public void CreateUserLoginLog()
        //{
        //    Log myLog = PackMyLog(DateTime.Now, "员工登录");
        //    DataManager.UserLoginLogWrite(ref myLog);
        //    //LogDGVadd(myLog);
        //    LogShowInDataGridView.Invoke(myLog);
        //}
        public void AutoRunInit(bool isBatteryNotFull,Action<bool> CallBack)
        {
            logStreamWriter.WriteLine(DateTime.Now.ToString()+"启动初始化开始");
            //检查是否满足运行条件
            if (!curUser.IsLoggined|| !fx5u.IsConnected ||!sr2000w.IsConnected||
                !bt3562.IsConnected||!isTemperatureConnected/*||curConfig.tech_no==""*/)
            {
                CallBack?.BeginInvoke(false, null, null);
                return;
            }
            //设置好指令
            string cmd = PlcLabel.D1.ToString();
            string ret = fx5u.SendCmd(cmd);
            if (int.Parse(ret) != 0)
            {
                CallBack?.BeginInvoke(false, null, null);
                //打包错误信息
                string errMessage = Fx5uErr(ret);
                //ErrorEventHandler?.BeginInvoke(errMessage, "PLC", null, null);
                return;
            }
            else
            {
                errorFlag = false;
                this.isNotFullBattery = isBatteryNotFull;
                if (isBatteryNotFull)
                    sr2000w.SendCmd("WP,251,1");
                else
                    sr2000w.SendCmd("WP,251,0");
                //设置一次扫码的数量
                SetScanNum(curConfig.BatteryNum, false);
                statistic = new List<bool>(curConfig.StatisticNum);
                fx5u.ResetRegister("D101",0);
                fx5u.ResetRegister("D103", 0);
                fx5u.ResetRegister("D104", 0);
                fx5u.ResetRegister("D170", 0);
                fx5u.ResetRegister("D114", 0);
                fx5u.ResetRegister("D115", 0);
                fx5u.ResetRegister("D144", 0);
                fx5u.ResetRegister("D171", 0);
                fx5u.ResetRegister("D172", 0);
                fx5u.ResetRegister("D149", 0);

                //IO切换仪控制口复位
                fx5u.ResetRegister("D5", 0);
                fx5u.ResetRegister("D6", 0);
                fx5u.ResetRegister("D7", 0);
                fx5u.ResetRegister("D8", 0);

                fx5u.ResetRegister("D16", 0);
                fx5u.ResetRegister("D17", 0);
                fx5u.ResetRegister("D18", 0);
                fx5u.ResetRegister("D19", 0);

                #region //下位机故障报警地址清零
                string startDevice = "D150";
                int iSize = 4;
                short[] sData = new short[4] { 0, 0, 0, 0 };
                fx5u.WriteDeviceBlock(startDevice, iSize, ref sData);
                #endregion
                supportOnPosition = false;
                probeOnPosition = false;
                scanDown = false;
                measureDown = false;
                //部署自动运行线程
                fx5u.EventMeditator = new Action<PlcLabel>(AutoRun);
                //CreateSystemStartLog();
                plcState = PlcState.RUN;
                CallBack?.BeginInvoke(true, null, null);
            }
            logStreamWriter.WriteLine(DateTime.Now.ToString()+"启动初始化结束");
        }
        private bool VerifyBattery(string sn, ref string tech_no, ref string shop_order, ref string o1_voltage,ref string o1_date,ref string msg)//如果result为false,则有校验失败的电池，退出
        {
            //#if VERIFYALWAYSTRUE
            //bool verifyflag = true; //这里我弄错了
            //#else
            //            RequestBResponseRootJson responseB = null;
            //            RequestBJson requestB = new RequestBJson();//可能还要补充一些，但公司的人就说这样就OK
            //            requestB.sfc = sn;
            //            string remsg="";
            //            responseB = Mes.SendRequest(requestB,ref remsg);
            //            bool verifyflag = false;
            //            if(remsg=="Success")//正常通信成功
            //            {
            //                verifyflag = Convert.ToBoolean(responseB.result.results);
            //            }
            //            //没有通信成功默认不允许在本机器上加工
            //#endif
            //string shop_order = "";
            Log myLog = new Log();
            bool result = mes.GetData(sn, false, ref msg, ref shop_order, ref tech_no, ref o1_voltage,ref o1_date,ref myLog);
            
            //DataManager.LogWrite(myLog);
            //主界面DataGridView展示
            //LogDGVadd(myLog);
            LogShowInDataGridView?.BeginInvoke(myLog, null, null);
            if (result)
            {
                //如果正确请求了
                if (tech_no == curConfig.tech_no)   //result表示，正确请求到了，且返回的是OK，如果返的是NG，我这边就不加工。
                {
                    return true;
                }
                msg = "型号不对，不在本机加工";
                return false;
            }
            msg = "来料接口返回NG";
            return false;
        }
        
        public void productionLogSave()
        {
            xmlOper.setDataByUser(curUser.Id, curUserProductionCounter);
            xmlOper.Save();
        }
        public int getProductionNum()
        {
            return xmlOper.getTodayProduction();
        }
        //发送PLC指令函数
        private void SendFx5uCmd(PlcLabel label)
        {
            //发送PLC命令
            string cmd = label.ToString();
            string ret = fx5u.SendCmd(cmd);
            //出错
            if (int.Parse(ret) != 0)
            {
                //打包错误信息
                string errMessage=Fx5uErr(ret);
                //窗口通知
                //ErrorEventHandler?.BeginInvoke(errMessage,"PLC",null,null);
            }
        }
        //public void CreateSystemStartLog()
        //{
        //    Log myLog = PackMyLog(DateTime.Now, "启动");
        //    DataManager.SystemStartLogWrite(ref myLog);
        //    //LogDGVadd(myLog);
        //    LogShowInDataGridView.Invoke(myLog);
        //}

        //public void CreateSystemStopLog()
        //{
        //    Log myLog = PackMyLog(DateTime.Now, "停机");
        //    DataManager.SystemStopLogWrite(ref myLog);
        //    //LogDGVadd(myLog);
        //    LogShowInDataGridView.Invoke(myLog);
        //}
        //private Log PackMesLog( fName, string send, string response)
        //{
        //    Log mylog = new Log();
        //    //时间
        //    string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    mylog.operTime = time;
        //    //接口名称
        //    mylog.fName = fName;
        //    //发送内容
        //    switch(fName)
        //    {
        //        case ""
        //    }
        //    //接收内容
        //}

        //private Log PackMyLog(DateTime curTime, string operType)
        //{
        //    Log myLog = new Log();
        //    myLog.operTime = curTime;
        //    myLog.logType = (operType == "启动" || operType == "停机" || operType == "调零") ? "本地" : "MES";
        //    myLog.userId = curUser.Id;
        //    myLog.operType = operType;
        //    myLog.log = curTime.ToString() + "：" + operType;
        //    if(operType=="上传")
        //    {
        //        myLog.remark = curBattery[curBatteryi - 1].MesSaved ? "上传成功" : "上传失败";
        //    }
        //    return myLog;
        //}

        //规定的扫码时间已经过去了
        private void Sr2000wLoff(object sender, ElapsedEventArgs e)
        {
            sr2000wTimer?.Stop();
            sr2000wTimer?.Dispose();
            sr2000w.DataReceivedEventHandler = null;
            //读取扫到的结果
            string sns=sr2000w.SendCmd("LOFF");
            if(CheckIsSns(sns)) //扫到了码
            {
                sns = sns.Substring(0, sns.Length - 1);
                string[] snArray = sns.Split(',');
                curBattery = new Battery[snArray.Length];
                string msg = "";
                for (short i = 0; i < snArray.Length; i++)
                {
                    string techId = "";
                    string shop_order = "";
                    string o1_voltage = "";
                    string o1_date = "";
                    curBattery[i] = new Battery();
                    curBattery[i].sn = snArray[snArray.Length - 1 - i];
                    curBattery[i].operation_id = curConfig.operation_id;
                    myMonitor.InBatteryNum += 1;
                    curBattery[i].verifyFlag = VerifyBattery(curBattery[i].sn, ref techId,ref shop_order,ref o1_voltage,ref o1_date, ref msg);
                    curBattery[i].desc = msg;
                    curBattery[i].operTime = DateTime.Now;
                    curBattery[i].user = curUser.Id;
                    curBattery[i].techId = techId;
                    curBattery[i].shop_order = shop_order;
                    if (curBattery[i].verifyFlag)
                    {
                        if (curConfig.operation_id == "O2T")
                        {
                            curBattery[i].o1_voltage = Math.Round(double.Parse(o1_voltage) / VOLTAGE_UNIT, 7);
                            curBattery[i].o1_date = DateTime.Parse(o1_date);
                        }
                    }
                    ////如果O2T工序的话
                    //if (curConfig.operation_id=="O2T")
                    //{
                    //    curBattery[i].o1_voltage = Math.Round(double.Parse(o1_voltage) / VOLTAGE_UNIT, 7);
                    //    curBattery[i].o1_date = DateTime.Parse(o1_date);
                    //}
                    if (curBattery[i].verifyFlag)
                    {
                        DataManager.BatteryVerifyBatteryWrite(curBattery[i]);
                    }
                    //Log myLog=PackMesLog("GetData",curBattery[i].sn+","+curConfig.resouce_id,techId+","+shop_order+","+msg)
                    ////制作log
                    //Log myLog = PackMyLog(curBattery[i].operTime, "来件检验",msg);
                    //写入数据库
                    //DataManager.BatteryVerifyLogWrite(curBattery[i], ref myLog);
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "得到码" + (i + 1).ToString());
                }
                KeyenceReceivedDataVerifiedEvent.Invoke();
                List<int> ledList = new List<int>();
                //扫码完成条件置1
                scanDown = true;
                curBatteryi = 0;
                if (curLevel)
                {
                    #region
                    int iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
                    for (int itemp = 0; itemp < curBattery.Length; itemp++)
                    {
                        if (!curBattery[itemp].verifyFlag)//如果不是再本机器上生产，则亮红灯
                        {
                            ledList.Add(iStart);
                        }
                        iStart++;
                    }
                    ledFlasher.SetFlash(ledList);
                    #endregion
                    //上层
                    fx5u.ResetRegister("D123", 0);//上层不在扫码了
                    if (supportOnPosition && probeOnPosition)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            return;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4快电池，打开上层第一通道");
                        }
                        else
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(500);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送:FETCH?指令");
                    }
                }
                else
                {
                    #region
                    int iStart = (curConfig.BatteryNum == 4) ? 48 : 49;
                    for (int itemp = 0; itemp < curBattery.Length; itemp++)
                    {
                        if (!curBattery[itemp].verifyFlag)//如果不是在本机器上生产，则亮红灯
                        {
                            ledList.Add(iStart);
                        }
                        iStart++;
                    }
                    downLedFlasher.SetFlash(ledList);
                    #endregion
                    //下层托
                    fx5u.ResetRegister("D124", 0);//下层不在扫码了
                    if (supportOnPosition && probeOnPosition)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            return;
                        }
                        fx5u.SendCmd("D142");//开始测量
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        Thread.Sleep(500);
                        //检测电池
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送完:FETCH?指令");
                    }
                }
            }
            else  //报扫码超时
            {
                if (curLevel)
                    //不在扫码了
                    fx5u.ResetRegister("D123", 0);
                else
                    //不在扫码了
                    fx5u.ResetRegister("D124", 0);
                //三色灯报警
                fx5u.SendCmd("D141");
                //主界面显示错误信息
                //string errMessage = Sr2000wErr("扫码超时");
                ErrorEventHandler?.BeginInvoke("扫码枪扫码超时", null, null);
                UploadErrorA("A-001", "扫码超时");
            }
        }
        private void Sr2000wGetSns(string sns)
        {
            logStreamWriter.WriteLine(DateTime.Now.ToString() + "扫码枪扫到了码");
            //释放计时器
            sr2000wTimer?.Stop();
            sr2000wTimer?.Dispose();
            //normalFlag = true;
            //从字符串转为array
            sns = sns.Substring(0, sns.Length - 1);
            if (isNotFullBattery)
            {
                string[] snArray = sns.Split(',');
                curBattery = new Battery[snArray.Length];
                string msg = "";
                for (short i = 0; i < snArray.Length; i++)
                {
                    string techId = "";
                    string shop_order = "";
                    string o1_voltage = "";
                    string o1_date = "";
                    curBattery[i] = new Battery();
                    curBattery[i].sn = snArray[snArray.Length - 1 - i];
                    curBattery[i].operation_id = curConfig.operation_id;
                    myMonitor.InBatteryNum += 1;
                    curBattery[i].verifyFlag = VerifyBattery(curBattery[i].sn, ref techId, ref shop_order, ref o1_voltage, ref o1_date, ref msg);
                    curBattery[i].desc = msg;
                    curBattery[i].operTime = DateTime.Now;
                    curBattery[i].user = curUser.Id;
                    curBattery[i].techId = techId;
                    curBattery[i].shop_order = shop_order;

                    if (curBattery[i].verifyFlag)           //如果来料检验成功了才会返回这些信息的。
                    {
                        if (curConfig.operation_id == "O2T")
                        {
                            curBattery[i].o1_voltage = Math.Round(double.Parse(o1_voltage) / VOLTAGE_UNIT, 7);
                            curBattery[i].o1_date = DateTime.Parse(o1_date);
                        }
                    }
                    ////如果O2T工序的话
                    //if (curConfig.operation_id == "O2T")
                    //{
                    //    curBattery[i].o1_voltage =Math.Round(double.Parse(o1_voltage) / VOLTAGE_UNIT, 7);
                    //    curBattery[i].o1_date = DateTime.Parse(o1_date);
                    //}
                    if (curBattery[i].verifyFlag)
                    {
                        DataManager.BatteryVerifyBatteryWrite(curBattery[i]);
                    }
                    //curBattery[i].verifyFlag=VerifyBattery(snArray[i],ref msg);
                    ////制作log
                    //Log myLog = PackMyLog(curBattery[i].operTime, "来件检验");
                    ////写入数据库
                    //DataManager.BatteryVerifyLogWrite(curBattery[i], ref myLog);
                    ////主界面DataGridView展示
                    ////LogDGVadd(myLog);
                    //LogShowInDataGridView.Invoke(myLog);
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "得到码" + (i + 1).ToString());
                }
                KeyenceReceivedDataVerifiedEvent?.Invoke();
                List<int> ledList = new List<int>();
                //扫码完成条件置1
                scanDown = true;
                curBatteryi = 0;
                if (curLevel)
                {
                    #region
                    int iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
                    for (int itemp = 0; itemp < curBattery.Length; itemp++)
                    {
                        if (!curBattery[itemp].verifyFlag)//如果不是再本机器上生产，则亮红灯
                        {
                            ledList.Add(iStart);
                        }
                        iStart++;
                    }
                    ledFlasher.SetFlash(ledList);
                    #endregion
                    //上层
                    fx5u.ResetRegister("D123", 0);//上层不在扫码了
                    if (supportOnPosition && probeOnPosition)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            return;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4快电池，打开上层第一通道");
                        }
                        else
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(500);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送:FETCH?指令");
                    }
                }
                else
                {
                    curBatteryi = 0;
                    #region
                    int iStart = (curConfig.BatteryNum == 4) ? 48 : 49;
                    for (int itemp = 0; itemp < curBattery.Length; itemp++)
                    {
                        if (!curBattery[itemp].verifyFlag)//如果不是在本机器上生产，则亮红灯
                        {
                            ledList.Add(iStart);
                        }
                        iStart++;
                    }
                    downLedFlasher.SetFlash(ledList);
                    #endregion
                    //下层托
                    fx5u.ResetRegister("D124", 0);//下层不在扫码了
                    if (supportOnPosition && probeOnPosition)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            return;
                        }
                        fx5u.SendCmd("D142");//开始测量
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        Thread.Sleep(500);
                        //检测电池
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送完:FETCH?指令");
                    }
                }
            }
            else
            {
                MySns = new List<string>(sns.Split(','));
                if (measureDown)
                {
                    scanDown = false;
                    measureDown = false;
                    if(!CheckBatteriesErrorAlways())
                    {
                        if (curLevel)
                        {
                            fx5u.ResetRegister("D123", 0);//上层不在扫码了
                            SendFx5uCmd(PlcLabel.D108);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，下层电池进去");
                            logStreamWriter.Flush();
                        }
                        else
                        {
                            fx5u.ResetRegister("D124", 0);//下层不在扫码了
                            SendFx5uCmd(PlcLabel.D119);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，上层电池进去");
                            logStreamWriter.Flush();
                        }
                    }
                    else
                    {
                        fx5u.SendCmd("D146");//连续测量不合格
                        ErrorEventHandler?.BeginInvoke("连续测量不合格", null, null);
                        UploadErrorA("A-002", "电池连续不合格");
                    }
                    manageData();
                } else
                {
                    scanDown = true;
                }
            }
        //}
        //如果不是本单位生产
        //else
        //{
        //    int iStart;
        //    if (curLevel)
        //    {
        //        //不在扫码了
        //        fx5u.ResetRegister("D123", 0);
        //        iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
        //        for (int itemp = 0; itemp < curBattery.Length; itemp++)
        //        {
        //            if (!curBattery[itemp].verifyFlag)//如果不是再本机器上生产，则亮红灯
        //            {
        //                ledList.Add(iStart);
        //            }
        //            iStart++;
        //        }
        //        ledFlasher.SetFlash(ledList);
        //        Thread.Sleep(50);
        //        fx5u.SendCmd("D142");//开始测量
        //        Thread.Sleep(50);
        //        fx5u.SendCmd("D143");//结束测量
        //        Thread.Sleep(50);
        //        //上层电池退回
        //        SendFx5uCmd(PlcLabel.D108);
        //    }
        //    else
        //    {
        //        //不在扫码了
        //        fx5u.ResetRegister("D124", 0);
        //        iStart = (curConfig.BatteryNum == 4) ? 48 : 49;
        //        for (int itemp = 0; itemp < curBattery.Length; itemp++)
        //        {
        //            if (!curBattery[itemp].verifyFlag)//如果不是在本机器上生产，则亮红灯
        //            {
        //                ledList.Add(iStart);
        //            }
        //            iStart++;
        //        }
        //        ledFlasher.SetFlash(ledList);
        //        Thread.Sleep(50);
        //        fx5u.SendCmd("D142");
        //        Thread.Sleep(50);
        //        fx5u.SendCmd("D143");
        //        Thread.Sleep(50);
        //        //下层电池退回
        //        SendFx5uCmd(PlcLabel.D119);
        //    }
        //}
        }
        private void Sr2000wGetSnTimeOut(Object source, System.Timers.ElapsedEventArgs e)
        {
            //释放计时器
            sr2000wTimer?.Stop();
            sr2000wTimer?.Dispose();
            //取消所有扫码枪收到信息触发事件
            sr2000w.DataReceivedEventHandler = null;
            //超时结束读码
            sr2000w.SendCmd("LOFF");
            StandardGetDown.Invoke(false, "扫码超时");
            //打包错误信息
            //string errMessage=Sr2000wErr("扫码超时");
            //发送电池退回指令
            fx5u.SendCmd("D136");
            //扫码的数量回到原值
            SetScanNum(curConfig.BatteryNum, false);
            //fx5u监视软元件重新设置
            GetTechStdDown();
            //所有接口全部拆除
            fx5u.EventMeditator = null;
            sr2000w.DataReceivedEventHandler = null;
        }
        private double OperateDecimal(double decim,int precission)
        {
            double tempDouble = Math.Round(decim * Math.Pow(10, precission));
            return tempDouble / (Math.Pow(10, precission));
        }
        private void ChannelChangedCallBack(string RandV)
        {
            if(isNotFullBattery)
            {
                fx5u.ResetRegister(curIOAddrStr, 0);
                curBatteryi += 1;
                if (curBattery[curBatteryi - 1].verifyFlag) //如果该电池是在本机器上加工
                {
                    double v;
                    double r;
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "正在获取" + "第" + curBatteryi.ToString() + "块电池的电压电阻");
                    //删除掉所有的空格
                    RandV = RandV.Replace(" ", "");
                    //区分电压和电阻
                    string[] values = RandV.Split(',');
                    r = Math.Abs(double.Parse(values[0]));
                    v = Math.Abs(double.Parse(values[1]));
                    if (!CheckRVNormal(r, v))
                    {
                        //fx5u.SendCmd("D143");
                        //Thread.Sleep(10);
                        //fx5u.SendCmd("D148");
                        //ErrorEventHandler?.BeginInvoke("第" + ((curConfig.BatteryNum == 4) ? curBatteryi : curBatteryi + 1) + "通道电池检测异常",null,null);
                        //UploadErrorA("A-003", "电压电阻测试异常");
                        curBattery[curBatteryi - 1].desc = "电压电阻测量异常";
                        List<int> ledsAddr;
                        if (curLevel) //上层
                            ledsAddr = ledFlasher.getFlshAddr();
                        else
                            ledsAddr = downLedFlasher.getFlshAddr();
                        if (ledsAddr == null)//设置新的led闪烁
                        {
                            int iLabel = curLevel ? 44 : 48;
                            int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                            int iTemp = iStart + curBatteryi - 1;
                            ledsAddr = new List<int>();
                            ledsAddr.Add(iTemp);
                            if (curLevel)
                                ledFlasher.SetFlash(ledsAddr);
                            else
                                downLedFlasher.SetFlash(ledsAddr);
                        }
                        else
                        {
                            int iLabel = curLevel ? 44 : 48;
                            int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                            int iTemp = iStart + curBatteryi - 1;
                            ledsAddr.Add(iTemp);
                            if (curLevel)
                                ledFlasher.SetFlash(ledsAddr);
                            else
                                downLedFlasher.SetFlash(ledsAddr);
                        }
                        SendBatteryDownShowEvent?.Invoke();
                    }
                    else
                    {
                        //测量数加1
                        curBattery[curBatteryi - 1].operTime = DateTime.Now;
                        myMonitor.TestBatteryNum += 1;
                        //处理小数点位数：
                        r = Math.Round(r, 7);
                        curBattery[curBatteryi - 1].resistance = r;
                        double tempV = Math.Round(v, 7);
                        curBattery[curBatteryi - 1].origin_voltage = tempV;
                        double temper = 0;
                        v = temperatureCalculate(ref temper, v);
                        //处理小数点位数
                        v = Math.Round(v, 7);
                        curBattery[curBatteryi - 1].temperature = temper;
                        curBattery[curBatteryi - 1].voltage = v;
                        if (curConfig.operation_id == "O2T")
                            curBattery[curBatteryi - 1].K = CalculateK(curBattery[curBatteryi - 1].o1_voltage, v, curBattery[curBatteryi - 1].o1_date, curBattery[curBatteryi - 1].operTime);
                        //判断电池是否合格
                        JudgeBattery(curBattery[curBatteryi - 1], ref curBattery[curBatteryi - 1].result, ref curBattery[curBatteryi - 1].k_flag, ref curBattery[curBatteryi - 1].errorType);
                        //上传到MES系统
                        curBattery[curBatteryi - 1].user = curUser.Id;
                        string msg = "";
                        bool sendFlag = SendMes(curBattery[curBatteryi - 1], ref msg);
                        curBattery[curBatteryi - 1].MesSaved = sendFlag;
                        curBattery[curBatteryi - 1].tempCoeff = curConfig.temCoeff;
                        if (curBattery[curBatteryi - 1].MesSaved)
                            myMonitor.SavedBatteryNum += 1;
                        DataManager.BatterySaveBatteryWrite(curBattery[curBatteryi - 1]);

                        //Log mylog = PackMyLog(DateTime.Now, "上传");
                        //DataManager.BatterySave(curBattery[curBatteryi - 1], ref mylog);
                        ////LogDGVadd(mylog);
                        //LogShowInDataGridView.Invoke(mylog);
                        //Action<Battery> BatteryShowAsy = new Action<Battery>(BatteryDGVadd);
                        //BatteryShowAsy.BeginInvoke(curBattery[curBatteryi - 1],null,null);

                        batteryInfoShowInDataGridView?.BeginInvoke(curBattery[curBatteryi - 1], null, null);
                        SendBatteryDownShowEvent?.Invoke();
                        //先设置一下istart
                        int iLabel = curLevel ? 44 : 48;
                        int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                        int iTemp = iStart + curBatteryi - 1;
                        //如果合格则亮绿灯,绿灯为1
                        if (curBattery[curBatteryi - 1].result)
                        {
                            PlcLabel labelTemp = (PlcLabel)iTemp;
                            SendFx5uCmd(labelTemp);
                        }
                        //如果不合格，则亮红灯，红灯为1
                        //if (!curBattery[curBatteryi-1].result)
                        //{
                        //    PlcLabel labelTemp = (PlcLabel)iTemp;
                        //    SendFx5uCmd(labelTemp);
                        //}
                        statistic.Insert(0, curBattery[curBatteryi - 1].result);
                        if (statistic.Count == curConfig.StatisticNum + 1)
                            statistic.RemoveAt(curConfig.StatisticNum);
                    }
                }
                if (curBatteryi != curBattery.Length)
                {
                    //可以继续跳到下一个
                    if (curLevel)
                    {
                        //打开下一个通道
                        int add = (curConfig.BatteryNum == 4) ? 8 : 10;
                        int i_label = curBatteryi * 2 + add;
                        PlcLabel changeChannelLabel = (PlcLabel)i_label;
                        curIOAddrStr = changeChannelLabel.ToString();
                        SendFx5uCmd(changeChannelLabel);
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }
                    else
                    {
                        //打开下一个通道
                        int add = (curConfig.BatteryNum == 4) ? 24 : 26;
                        int i_label = curBatteryi * 2 + add;
                        PlcLabel changeChannelLabel = (PlcLabel)i_label;
                        curIOAddrStr = changeChannelLabel.ToString();
                        SendFx5uCmd(changeChannelLabel);
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }
                }
                else
                {
                    fx5u.ResetRegister("D143", 1);
                    if (!CheckBatteriesErrorAlways())
                    {
                        if (curLevel)
                        {
                            SendFx5uCmd(PlcLabel.D108);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，下层电池进去");
                            logStreamWriter.Flush();
                        }
                        else
                        {
                            SendFx5uCmd(PlcLabel.D119);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，上层电池进去");
                            logStreamWriter.Flush();
                        }
                    }
                    else
                    {
                        fx5u.SendCmd("D146");//连续测量不合格
                        ErrorEventHandler?.BeginInvoke("连续测量不合格", null, null);
                        UploadErrorA("A-002", "电池连续不合格");
                    }
                }
            }
            else
            {
                BatteryCount++;
                fx5u.ResetRegister(curIOAddrStr, 0);
                //删除掉所有的空格
                RandV = RandV.Replace(" ", "");
                double v, r;
                //区分电压和电阻
                string[] values = RandV.Split(',');
                r = Math.Abs(double.Parse(values[0]));
                v = Math.Abs(double.Parse(values[1]));
                //把电压和电阻放进VandR中
                if(!CheckRVNormal(r,v))
                {
                    VandR.Add("#;#");
                }else
                {
                    VandR.Add(String.Format("{0};{1}", r.ToString(), v.ToString()));
                }
                if(BatteryCount!=curConfig.BatteryNum)
                {
                    //针对不同的层，发不同的通道打开指令
                    if (curLevel)
                    {
                        //打开下一个通道
                        int add = (curConfig.BatteryNum == 4) ? 8 : 10;
                        int i_label = curBatteryi * 2 + add;
                        PlcLabel changeChannelLabel = (PlcLabel)i_label;
                        curIOAddrStr = changeChannelLabel.ToString();
                        SendFx5uCmd(changeChannelLabel);
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }
                    else
                    {
                        //打开下一个通道
                        int add = (curConfig.BatteryNum == 4) ? 24 : 26;
                        int i_label = curBatteryi * 2 + add;
                        PlcLabel changeChannelLabel = (PlcLabel)i_label;
                        curIOAddrStr = changeChannelLabel.ToString();
                        SendFx5uCmd(changeChannelLabel);
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }

                }else
                {
                    fx5u.ResetRegister("D143", 1);
                    if (scanDown)
                    {
                        scanDown = false;
                        measureDown = false;
                        if(!CheckBatteriesErrorAlways())
                        {
                            if (curLevel)
                            {
                                SendFx5uCmd(PlcLabel.D108);
                                logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，下层电池进去");
                                logStreamWriter.Flush();
                            }
                            else
                            {
                                SendFx5uCmd(PlcLabel.D119);
                                logStreamWriter.WriteLine(DateTime.Now.ToString() + "切换，上层电池进去");
                                logStreamWriter.Flush();
                            }
                        }else
                        {
                                fx5u.SendCmd("D146");//连续测量不合格
                                ErrorEventHandler?.BeginInvoke("连续测量不合格", null, null);
                                UploadErrorA("A-002", "电池连续不合格");
                        }

                        //处理数据
                        manageData();
                    }
                    else
                    {
                        measureDown = true;
                    }
                }
            }
        }

        private void manageData()
        {
            curBattery = new Battery[MySns.Count];
            string msg = "";
            for (short i = 0; i < MySns.Count; i++)
            {
                string techId = "";
                string shop_order = "";
                string o1_voltage = "";
                string o1_date = "";
                curBattery[i] = new Battery();
                curBattery[i].sn = MySns[MySns.Count - 1 - i];
                curBattery[i].operation_id = curConfig.operation_id;
                myMonitor.InBatteryNum += 1;
                curBattery[i].verifyFlag = VerifyBattery(curBattery[i].sn, ref techId, ref shop_order, ref o1_voltage, ref o1_date, ref msg);
                curBattery[i].desc = msg;
                curBattery[i].operTime = DateTime.Now;
                curBattery[i].user = curUser.Id;
                curBattery[i].techId = techId;
                curBattery[i].shop_order = shop_order;

                if (curBattery[i].verifyFlag)           //如果来料检验成功了才会返回这些信息的。
                {
                    if (curConfig.operation_id == "O2T")
                    {
                        curBattery[i].o1_voltage = Math.Round(double.Parse(o1_voltage) / VOLTAGE_UNIT, 7);
                        curBattery[i].o1_date = DateTime.Parse(o1_date);
                    }
                    DataManager.BatteryVerifyBatteryWrite(curBattery[i]);
                }
                logStreamWriter.WriteLine(DateTime.Now.ToString() + "得到码" + (i + 1).ToString());
            }
            KeyenceReceivedDataVerifiedEvent?.Invoke();
            //对验证失败的电池报红灯
            List<int> ledList = new List<int>();
            if (curLevel)
            {
                int iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
                for (int itemp = 0; itemp < curBattery.Length; itemp++)
                {
                    if (!curBattery[itemp].verifyFlag)//如果不是再本机器上生产，则亮红灯
                    {
                        ledList.Add(iStart);
                    }
                    iStart++;
                }
                ledFlasher.SetFlash(ledList);
            }
            else
            {
                #region
                int iStart = (curConfig.BatteryNum == 4) ? 48 : 49;
                for (int itemp = 0; itemp < curBattery.Length; itemp++)
                {
                    if (!curBattery[itemp].verifyFlag)//如果不是在本机器上生产，则亮红灯
                    {
                        ledList.Add(iStart);
                    }
                    iStart++;
                }
                downLedFlasher.SetFlash(ledList);
                #endregion
            }
            for (short i=0;i<MySns.Count;i++)
            {
                curBatteryi += 1;
                if(VandR[i]=="#;#")
                {
                    curBattery[curBatteryi - 1].desc = "电压电阻测量异常";
                    List<int> ledsAddr;
                    if (curLevel) //上层
                        ledsAddr = ledFlasher.getFlshAddr();
                    else
                        ledsAddr = downLedFlasher.getFlshAddr();
                    if (ledsAddr == null)//设置新的led闪烁
                    {
                        int iLabel = curLevel ? 44 : 48;
                        int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                        int iTemp = iStart + curBatteryi - 1;
                        ledsAddr = new List<int>();
                        ledsAddr.Add(iTemp);
                        if (curLevel)
                            ledFlasher.SetFlash(ledsAddr);
                        else
                            downLedFlasher.SetFlash(ledsAddr);
                    }
                    else
                    {
                        int iLabel = curLevel ? 44 : 48;
                        int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                        int iTemp = iStart + curBatteryi - 1;
                        ledsAddr.Add(iTemp);
                        if (curLevel)
                            ledFlasher.SetFlash(ledsAddr);
                        else
                            downLedFlasher.SetFlash(ledsAddr);
                    }
                    SendBatteryDownShowEvent?.Invoke();
                }
                else
                {
                    string[] RV=MySns[i].Split('#');
                    //测量数加1
                    curBattery[curBatteryi - 1].operTime = DateTime.Now;
                    myMonitor.TestBatteryNum += 1;

                    //处理小数点位数：
                    double r = Math.Round(double.Parse(RV[0]), 7);
                    curBattery[curBatteryi - 1].resistance = r;
                    double tempV = Math.Round(double.Parse(RV[1]), 7);
                    curBattery[curBatteryi - 1].origin_voltage = tempV;
                    double temper = 0;
                    double v = temperatureCalculate(ref temper, tempV);
                    //处理小数点位数
                    v = Math.Round(v, 7);
                    curBattery[curBatteryi - 1].temperature = temper;
                    curBattery[curBatteryi - 1].voltage = v;
                    if (curConfig.operation_id == "O2T")
                        curBattery[curBatteryi - 1].K = CalculateK(curBattery[curBatteryi - 1].o1_voltage, v, curBattery[curBatteryi - 1].o1_date, curBattery[curBatteryi - 1].operTime);
                    //判断电池是否合格
                    JudgeBattery(curBattery[curBatteryi - 1], ref curBattery[curBatteryi - 1].result, ref curBattery[curBatteryi - 1].k_flag, ref curBattery[curBatteryi - 1].errorType);
                    //上传到MES系统
                    curBattery[curBatteryi - 1].user = curUser.Id;

                    bool sendFlag = SendMes(curBattery[curBatteryi - 1], ref msg);
                    curBattery[curBatteryi - 1].MesSaved = sendFlag;
                    curBattery[curBatteryi - 1].tempCoeff = curConfig.temCoeff;
                    if (curBattery[curBatteryi - 1].MesSaved)
                        myMonitor.SavedBatteryNum += 1;
                    DataManager.BatterySaveBatteryWrite(curBattery[curBatteryi - 1]);

                    //Log mylog = PackMyLog(DateTime.Now, "上传");
                    //DataManager.BatterySave(curBattery[curBatteryi - 1], ref mylog);
                    ////LogDGVadd(mylog);
                    //LogShowInDataGridView.Invoke(mylog);
                    //Action<Battery> BatteryShowAsy = new Action<Battery>(BatteryDGVadd);
                    //BatteryShowAsy.BeginInvoke(curBattery[curBatteryi - 1],null,null);

                    batteryInfoShowInDataGridView?.BeginInvoke(curBattery[curBatteryi - 1], null, null);
                    SendBatteryDownShowEvent?.Invoke();
                    //先设置一下istart
                    int iLabel = curLevel ? 44 : 48;
                    int iStart = (curConfig.BatteryNum == 4) ? iLabel : iLabel + 1;
                    int iTemp = iStart + curBatteryi - 1;
                    //如果合格则亮绿灯,绿灯为1
                    if (curBattery[curBatteryi - 1].result)
                    {
                        PlcLabel labelTemp = (PlcLabel)iTemp;
                        SendFx5uCmd(labelTemp);
                    }
                    //如果不合格，则亮红灯，红灯为1
                    //if (!curBattery[curBatteryi-1].result)
                    //{
                    //    PlcLabel labelTemp = (PlcLabel)iTemp;
                    //    SendFx5uCmd(labelTemp);
                    //}
                    statistic.Insert(0, curBattery[curBatteryi - 1].result);
                    if (statistic.Count == curConfig.StatisticNum + 1)
                        statistic.RemoveAt(curConfig.StatisticNum);
                }
            }
        }

        private double CalculateK(double o1_voltage,double o2_voltage,DateTime o1_time,DateTime o2_time)
        {
            //计算K值
            double voltageInterval = (o1_voltage - o2_voltage)*VOLTAGE_UNIT;
            TimeSpan timeInterval = o2_time - o1_time;
            double hours=timeInterval.TotalHours;
            double K = voltageInterval / hours;
            K = Math.Round(K, 4);
            return K;
        }
        private double temperatureCalculate(ref double temper,double vo)
        {
            temper = GetTemperature();
            double v = 0;
            v = vo + (temper - 25) * curConfig.temCoeff/VOLTAGE_UNIT;
            return v;
        }
        private bool CheckTemperatureIsNormal()
        {
            double tem=GetTemperature();
            if (tem < curConfig.minTem)
                return false;
            if (tem > curConfig.maxTem)
                return false;
            return true;
        }
        public double GetTemperature()
        {
            double temp=0;
            temperature.ReadTemperature(ref temp);
            return temp;
        }
        private void JudgeBattery(Battery myBattery, ref bool result,ref bool K_flag, ref BatteryErrorType errorType)
        {
            int voltageJudge = (myBattery.voltage > curConfig.MaxVoltage) ? 8 : ((curConfig.MinVoltage > myBattery.voltage) ? 4 : 0);
            int resistanceJudeg = (myBattery.resistance > curConfig.MaxResistance) ? 2 : ((curConfig.MinResistance > myBattery.resistance) ? 1 : 0);
            if (curConfig.operation_id == "O1T")
            {
                result = ((voltageJudge + resistanceJudeg) == 0) ? true : false;
                
            }
            else if(curConfig.operation_id=="O2T")
            {
                //判断K值：
                if (myBattery.K > curConfig.maxK || myBattery.K < curConfig.minK)
                    K_flag = false;
                result = ((voltageJudge + resistanceJudeg) == 0 && K_flag) ? true : false;
            }
            if (result)
                myMonitor.OkBatteryNum += 1;
            errorType = (BatteryErrorType)(voltageJudge + resistanceJudeg);
        }
        public bool TestInit(ref string errMsg)
        {
            //检查是否连接
            if (!fx5u.IsConnected || !sr2000w.IsConnected || !bt3562.IsConnected)
            {
                errMsg = "请检查设备的连接情况再测试";
                return false;
            }
            fx5u.ResetRegister("D101", 0);
            fx5u.ResetRegister("D103", 0);
            fx5u.ResetRegister("D104", 0);
            fx5u.ResetRegister("D170", 0);
            fx5u.ResetRegister("D114", 0);
            fx5u.ResetRegister("D115", 0);
            fx5u.ResetRegister("D144", 0);
            probeOnPosition = false;
            supportOnPosition = false;      
            fx5u.EventMeditator = TestRun;
            SendFx5uCmd(PlcLabel.D1);
            //PLC回调函数的设置
            return true;    
        }
        private void TestRun(PlcLabel label)
        {
            switch(label)
            {   
                //上层电池到位了
                case PlcLabel.D101:
                    fx5u.ResetRegister("D101", 0);
                    timeStart = DateTime.Now.Ticks;
                    break;
                case PlcLabel.D103:
                    fx5u.ResetRegister("D103", 0);
                    supportOnPosition = true;
                    if (probeOnPosition)
                    {
                        timeStop = DateTime.Now.Ticks;
                        timeInterval = new TimeSpan(timeStop - timeStart);
                        TestUpBatteryOnPositionEvent?.BeginInvoke(null,null);
                        probeOnPosition = false;
                        supportOnPosition = false;
                    }
                    break;
                case PlcLabel.D104:
                    fx5u.ResetRegister("D104", 0);
                    probeOnPosition = true;
                    if (supportOnPosition)
                    {
                        timeStop = DateTime.Now.Ticks;
                        timeInterval = new TimeSpan(timeStop - timeStart);
                        TestUpBatteryOnPositionEvent?.BeginInvoke(null,null);
                        probeOnPosition = false;
                        supportOnPosition = false;
                    }
                    break;
                case PlcLabel.D170:
                    fx5u.ResetRegister("D170", 0);
                    probeOnPosition = false;
                    supportOnPosition = false;
                    timeStart = DateTime.Now.Ticks;
                    break;
                case PlcLabel.D114:
                    fx5u.ResetRegister("D114", 0);
                    supportOnPosition = true;
                    if(probeOnPosition)
                    {
                        timeStop = DateTime.Now.Ticks;
                        timeInterval = new TimeSpan(timeStop - timeStart);
                        TestDownBatteryOnPositionEvent?.BeginInvoke(null,null);
                        probeOnPosition = false;
                        supportOnPosition = false;
                    }
                    break;
                case PlcLabel.D115:
                    fx5u.ResetRegister("D115", 0);
                    probeOnPosition = true;
                    if (supportOnPosition)
                    {
                        timeStop = DateTime.Now.Ticks;
                        timeInterval = new TimeSpan(timeStop - timeStart);
                        TestDownBatteryOnPositionEvent?.BeginInvoke(null,null);
                        probeOnPosition = false;
                        supportOnPosition = false;
                    }
                    break;
                case PlcLabel.D144:
                    fx5u.ResetRegister("D144", 0);
                    fx5u.EventMeditator = null;
                    probeOnPosition = false;
                    supportOnPosition = false;
                    TestResetEvent();
                    break;
            }
        }
        public void CloseMySqlConnection()
        {
            DBOper.CloseMySqlConnection();
        }
        public Action TestResetEvent;
        public Action TestUpBatteryOnPositionEvent;
        public Action TestDownBatteryOnPositionEvent;
        private bool isNotFullBattery;
        private bool errorFlag;
        public Action<bool> workRunWatchEventHandler;
        public Action<string> ScannerPositionEvent;
        private int BatteryCount;
        private bool measureDown;

        public void ResetLeds()
        {
            ledFlasher.ResetFlash();
            downLedFlasher.ResetFlash();
            int iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
            for (int itemp = 0; itemp < curConfig.BatteryNum; itemp++)
            {
                PlcLabel labelTemp = (PlcLabel)iStart;
                fx5u.ResetRegister(labelTemp.ToString(), 0);
                iStart++;
            }
            int iStart_down = (curConfig.BatteryNum == 4) ? 48 : 49;
            for (int itemp = 0; itemp < curConfig.BatteryNum; itemp++)
            {
                PlcLabel labelTemp = (PlcLabel)iStart_down;
                fx5u.ResetRegister(labelTemp.ToString(), 0);
                iStart_down++;
            }
        }

        private String GetErrCode(int iErr, Battery battery)
        {
            if (battery.operation_id == "O2T")
            {
                if (iErr == 0)
                    return "P_OCVNG_KZ";
            }
            if ((iErr & 3) == 0) //电阻OK
            {
                if ((iErr & 12) == 8)
                    return "P_OCVNG_GY";
                if ((iErr & 12) == 4)
                    return "P_OCVNG_DY";
            }
            return "P_OCVNG_NZ";
        }

        private bool SendMes(Battery battery,ref string msg)
        {
#if NOMES
            Random rd = new Random();
            bool result = (rd.NextDouble() > 0.9) ? false : true;
            return result;
#else
            //这些个接口实在是太乱了
            //ng_code对应于此       P_OCVNG_KZ      P_OCVNG_NZ      P_OCVNG_GY      P_OCVNG_DY
            string ng_code = "";
            int iErr;
            if (!battery.result)
            {
                iErr = (int)battery.errorType;
                //if ((iErr & 3) != 0)
                //{
                //    ng_code += "P_OCVNG_NZ";
                //    if ((iErr & 12) == 8)
                //        ng_code += ",P_OCVNG_GY";
                //    if ((iErr & 12) == 4)
                //        ng_code += ",P_OCVNG_DY";

                //}
                //else
                //{
                //    if ((iErr & 12) == 8)
                //        ng_code += "P_OCVNG_GY";
                //    if ((iErr & 12) == 4)
                //        ng_code += "P_OCVNG_DY";
                //}
                //if(battery.operation_id=="O2T")
                //{
                //    if (iErr == 0)
                //        ng_code = "P_OCVNG_KZ";
                //    else
                //        ng_code += ",P_OCVNG_KZ";
                //}
                ng_code = GetErrCode(iErr, battery);
            }
            UploadBatteryDataClass data;
            UploadBatteryO2TDataClass dataO2T;
            string flag = "";
            Log myLog;
            bool result;
            string origin_voltage = Math.Round(battery.origin_voltage * VOLTAGE_UNIT,4).ToString();
            if (battery.operation_id == "O1T")
            {
                data = new UploadBatteryDataClass();
                data.DATA1 = Math.Round(battery.voltage * VOLTAGE_UNIT,4).ToString();
                data.DATA2 = Math.Round(battery.resistance * RESISTANCE_UNIT,4).ToString();
                data.DATA02 = battery.shop_order;
                data.DATA03 = battery.techId;
                data.MATERIAL_TYPE = battery.techId;
                data.DATA4 = battery.temperature.ToString();
                data.DATA12 = battery.origin_voltage.ToString();
                data.DATA16 = curConfig.temCoeff.ToString();

                flag = battery.result ? "OK" : "NG";
                myLog = new Log();
                //制作上传json
                result = mes.UploadBatteryData(battery.sn, flag, ng_code, origin_voltage, data, ref msg, ref myLog);
                LogShowInDataGridView?.BeginInvoke(myLog, null, null);
                return result;
            }
            else if (battery.operation_id == "O2T")
            {
                dataO2T = new UploadBatteryO2TDataClass();
                dataO2T.DATA1 = Math.Round(battery.voltage * VOLTAGE_UNIT,4).ToString();
                dataO2T.DATA2 = Math.Round(battery.resistance * RESISTANCE_UNIT,4).ToString();
                dataO2T.DATA02 = battery.shop_order;
                dataO2T.DATA03 = battery.techId;
                dataO2T.MATERIAL_TYPE = battery.techId;
                dataO2T.DATA3 = battery.K.ToString();
                dataO2T.DATA4 = battery.temperature.ToString();

                dataO2T.DATA12 = battery.origin_voltage.ToString();
                dataO2T.DATA6 = battery.o1_voltage.ToString();
                dataO2T.DATA16 = curConfig.temCoeff.ToString();
                flag = battery.result ? "OK" : "NG";
                myLog = new Log();
                //制作上传json
                result = mes.UploadBatteryO2TData(battery.sn, flag, ng_code, origin_voltage, dataO2T, ref msg, ref myLog);
                LogShowInDataGridView?.BeginInvoke(myLog, null, null);
                return result;
            }
            return false;
#endif
        }
        //如果超时了4次，才会出故障报警
        private void Sr2000wGetSnTimeOutForNomalRun(Object source, System.Timers.ElapsedEventArgs e)
        {
            //释放计时器
            sr2000wTimer?.Stop();
            sr2000wTimer?.Dispose();
            //取消所有扫码枪收到信息触发事件
            sr2000w.DataReceivedEventHandler = null;
            //超时结束读码
            sr2000w.SendCmd("LOFF");
            if(plcState==PlcState.STOP)
            {
                //if (curLevel)
                //    //不在扫码了
                //    fx5u.ResetRegister("D123", 0);
                //else
                //    //不在扫码了
                //    fx5u.ResetRegister("D124", 0);
                //三色灯报警
                fx5u.SendCmd("D141");
                //主界面显示错误信息
                //string errMessage = Sr2000wErr("扫码超时");
                ErrorEventHandler?.BeginInvoke("扫码枪扫码超时", null, null);
                UploadErrorA("A-001", "扫码超时");
                return;
            }
            if (curConfig.scaner_move_enable == "0")
            {
                scanNum += 1;
                if (scanNum < tryScanNum)
                {
                    sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                    sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                    sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                    sr2000wTimer.AutoReset = false;
                    sr2000wTimer.Start();
                    string sns = sr2000w.SendCmd("LON");
                    if (CheckIsSns(sns))//如果是立即返回了
                    {
                        sr2000w.DataReceivedEventHandler = null;
                        Sr2000wGetSns(sns);
                    }
                    TryScanEvent.BeginInvoke(scanNum, null, null);
                    return;
                }
                if (curLevel)
                    //不在扫码了
                    fx5u.ResetRegister("D123", 0);
                else
                    //不在扫码了
                    fx5u.ResetRegister("D124", 0);
                //三色灯报警
                fx5u.SendCmd("D141");
                //主界面显示错误信息
                //string errMessage = Sr2000wErr("扫码超时");
                ErrorEventHandler?.BeginInvoke("扫码枪扫码超时", null, null);
                UploadErrorA("A-001", "扫码超时");
            }
            else if(curConfig.scaner_move_enable=="1")  //设置了横移
            {
                scanNum += 1;
                if(scanNum<tryScanNum)
                {
                    //先发送扫码指令，然后发送横移指令
                    //发送扫码指令
                    sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                    sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                    sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                    sr2000wTimer.AutoReset = false;
                    sr2000wTimer.Start();
                    sr2000w.SendCmd("LON");
                    //扫码枪在原位置扫1.5s
                    Thread.Sleep(1500);
                    //发送横移指令
                    if (curConfig.scaner_position == "0")  //扫码枪在最左边
                    {
                        fx5u.SendCmd("D177");
                        curConfig.scaner_position = "2";
                        ScannerPositionEvent.Invoke("最右边");
                    }
                    else if (curConfig.scaner_position == "2") //扫码枪在最右边
                    {
                        fx5u.SendCmd("D176");
                        curConfig.scaner_position = "0";
                        ScannerPositionEvent.Invoke("最左边");
                    }
                    TryScanEvent.BeginInvoke(scanNum, null, null);
                    return;
                }
                if (curLevel)
                    //不在扫码了
                    fx5u.ResetRegister("D123", 0);
                else
                    //不在扫码了
                    fx5u.ResetRegister("D124", 0);
                //三色灯报警
                fx5u.SendCmd("D141");
                //主界面显示错误信息
                //string errMessage = Sr2000wErr("扫码超时");
                ErrorEventHandler?.BeginInvoke("扫码枪扫码超时", null, null);
                UploadErrorA("A-001", "扫码超时");
            }
        }

        private void AutoRun(PlcLabel label)
        {
            string sns = "";
            switch (label)
            {
                case PlcLabel.D101://上层电池到位
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D101", 0);
                    curLevel = true;
                    VandR.Clear();
                    MySns.Clear();
                    BatteryCount = 0;
                    //normalFlag = true;
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "上层电池到位");
                    ledFlasher.ResetFlash();
                    //上层电池指示灯全部复位
                    int iStart = (curConfig.BatteryNum == 4) ? 44 : 45;
                    for (int itemp = 0; itemp < curConfig.BatteryNum; itemp++)
                    {
                        PlcLabel labelTemp = (PlcLabel)iStart;
                        fx5u.ResetRegister(labelTemp.ToString(), 0);
                        iStart++;
                    }
                    if (curConfig.scaner_move_enable == "0")   //如果不用横移，用以前的代码可以应付
                    {
                        //statusUpload.UploadStatus(DeviceStatus.A);
                        //curLevel = true;
                        //订阅扫码枪收到信息的事件
                        sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                        BatteryOnPositionEvent?.BeginInvoke(null, null);
                        if (this.isNotFullBattery)
                        {
                            //设置计时器
                            sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut - 1000);
                            sr2000wTimer.Elapsed += Sr2000wLoff;
                        }
                        else
                        {
                            scanNum = 0;
                            sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                            sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                        }
                        sr2000wTimer.AutoReset = false;
                        sr2000wTimer.Start();
                        //扫码
                        SendFx5uCmd(PlcLabel.D123);//上层电池在扫码
                        sns = sr2000w.SendCmd("LON");
                        if (CheckIsSns(sns))//如果是立即返回了
                        {
                            sr2000w.DataReceivedEventHandler = null;
                            Sr2000wGetSns(sns);
                        }
                        fx5u.EventMeditator = AutoRun;
                    }
                    else if (curConfig.scaner_move_enable == "1")       //电机需要横移
                    {
                        //先发送扫码指令，然后发送横移指令
                        //发送扫码指令
                        scanNum = 0;
                        sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                        BatteryOnPositionEvent?.BeginInvoke(null, null);
                        sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                        sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                        sr2000wTimer.AutoReset = false;
                        sr2000wTimer.Start();
                        SendFx5uCmd(PlcLabel.D123);//上层电池在扫码
                        sns=sr2000w.SendCmd("LON");
                        fx5u.EventMeditator = AutoRun;
                        //让扫码枪在原地扫1.5s,再移动
                        Thread.Sleep(1500);
                        //发送横移指令
                        if(curConfig.scaner_position=="0")  //扫码枪在最左边
                        {
                            fx5u.SendCmd("D177");
                            curConfig.scaner_position = "2";
                            ScannerPositionEvent.Invoke("最右边");
                        }
                        else if(curConfig.scaner_position=="2") //扫码枪在最右边
                        {
                            fx5u.SendCmd("D176");
                            curConfig.scaner_position = "0";
                            ScannerPositionEvent.Invoke("最左边");
                        }
                    }
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "设置扫码结束");
                    break;
                case PlcLabel.D103: //托板上升到位1
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D103", 0);
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"托板到位");
                    supportOnPosition = true;
                    if (probeOnPosition&&isNotFullBattery && scanDown)//如果在扫码，且探针到位了
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString()+"检测4块电池，打开上层第一通道");
                        }
                        else
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString()+"检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(10);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(1000);  //让电池检测仪去充分测量数据，然后再去获取
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送了:FETCH?指令");
                    }else if(!isNotFullBattery&&probeOnPosition)
                    {
                        //开始测量
                        supportOnPosition = false;
                        probeOnPosition = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开上层第一通道");
                        }
                        else
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(10);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);  //让电池检测仪去充分测量数据，然后再去获取
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送了:FETCH?指令");
                    }
                    fx5u.EventMeditator = AutoRun;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"退出托板到位了");
                    break;
                case PlcLabel.D104://探针下降到位1
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D104", 0);
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"探针下降到位");
                    probeOnPosition = true;
                    if (scanDown && supportOnPosition&&isNotFullBattery) //如果在扫码，且托板到位了
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString()+"检测4块电池，打开上层第一通道");
                        }
                        else 
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString()+"检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(500);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送完:FETCH?指令");
                    }else if (!isNotFullBattery && probeOnPosition)
                    {
                        //开始测量
                        supportOnPosition = false;
                        probeOnPosition = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D5); //打开上层第一通道
                            curIOAddrStr = "D5";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开上层第一通道");
                        }
                        else
                        {
                            Thread.Sleep(10);
                            SendFx5uCmd(PlcLabel.D6); //打开上层第二通道电池
                            curIOAddrStr = "D6";
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开上层第二通道");
                        }
                        Thread.Sleep(10);
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);  //让电池检测仪去充分测量数据，然后再去获取
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送了:FETCH?指令");
                    }
                    fx5u.EventMeditator = AutoRun;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"退出探针下降了");
                    break;
                case PlcLabel.D170://下层电池送至检测位成功
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D170", 0);
                    VandR.Clear();
                    MySns.Clear();
                    BatteryCount = 0;
                    //normalFlag = true;
                    curLevel = false;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"下层电池到位");
                    downLedFlasher.ResetFlash();
                    //上层电池指示灯全部复位
                    int iStart_down = (curConfig.BatteryNum == 4) ? 48 : 49;
                    for (int itemp = 0; itemp < curConfig.BatteryNum; itemp++)
                    {
                        PlcLabel labelTemp = (PlcLabel)iStart_down;
                        fx5u.ResetRegister(labelTemp.ToString(), 0);
                        iStart_down++;
                    }
                    BatteryOnPositionEvent?.BeginInvoke(null,null);
                    //statusUpload.UploadStatus(DeviceStatus.A);
                    //设置计时器
                    if(curConfig.scaner_move_enable=="0")
                    {
                        if (this.isNotFullBattery)
                        {
                            //设置计时器
                            sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut - 1000);
                            sr2000wTimer.Elapsed += Sr2000wLoff;
                        }
                        else
                        {
                            scanNum = 0;
                            sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                            sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                        }
                        sr2000wTimer.AutoReset = false;
                        sr2000wTimer.Start();
                        //扫码
                        //订阅扫码枪收到信息的事件
                        sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                        //扫码
                        SendFx5uCmd(PlcLabel.D124);//下层在扫码
                        sns = sr2000w.SendCmd("LON");
                        if (CheckIsSns(sns))
                        {
                            sr2000w.DataReceivedEventHandler = null;
                            Sr2000wGetSns(sns);
                        }
                        fx5u.EventMeditator = AutoRun;
                    }
                   else if(curConfig.scaner_move_enable=="1")
                    {
                        //先发送扫码指令，然后发送横移指令
                        //发送扫码指令
                        scanNum = 0;
                        sr2000w.DataReceivedEventHandler = new Action<string>(Sr2000wGetSns);
                        sr2000wTimer = new System.Timers.Timer(curConfig.Sr2000wTimeOut);
                        sr2000wTimer.Elapsed += Sr2000wGetSnTimeOutForNomalRun;
                        sr2000wTimer.AutoReset = false;
                        sr2000wTimer.Start();
                        SendFx5uCmd(PlcLabel.D124);//上层电池在扫码
                        sns=sr2000w.SendCmd("LON");
                        fx5u.EventMeditator = AutoRun;
                        //让扫码枪在原地扫1.5s,再移动
                        Thread.Sleep(1500);
                        //发送横移指令
                        if (curConfig.scaner_position == "0")  //扫码枪在最左边
                        {
                            fx5u.SendCmd("D177");
                            curConfig.scaner_position = "2";
                            ScannerPositionEvent.Invoke("最右边");
                        }
                        else if (curConfig.scaner_position == "2") //扫码枪在最右边
                        {
                            fx5u.SendCmd("D176");
                            curConfig.scaner_position = "0";
                            ScannerPositionEvent.Invoke("最左边");
                        }
                    }
                    logStreamWriter.WriteLine(DateTime.Now.ToString() + "设置扫码结束");
                    break;
                case PlcLabel.D114://托板上升到位2
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D114", 0);
                    //SendFx5uCmd(PlcLabel.D15);//发送探针下降指令
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"托板上升到位");
                    //ledFlasher.ResetFlash();
                    supportOnPosition = true;
                    //判断是否可以开始检测
                    if (scanDown && probeOnPosition&&isNotFullBattery)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        Thread.Sleep(500);
                        //检测电池
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }else if(!isNotFullBattery&&probeOnPosition)
                    {
                        //开始测量
                        supportOnPosition = false;
                        probeOnPosition = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);  //让电池检测仪去充分测量数据，然后再去获取
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送了:FETCH?指令");
                    }
                    fx5u.EventMeditator = AutoRun;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"托板上升退出");
                    break;
                case PlcLabel.D115://探针下降到位2
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D115", 0);
                    probeOnPosition = true;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+ "探针下降到位");
                    if (scanDown && supportOnPosition&&isNotFullBattery)
                    {
                        supportOnPosition = false;
                        probeOnPosition = false;
                        scanDown = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        Thread.Sleep(500);
                        //检测电池
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                    }else if(!isNotFullBattery&&supportOnPosition)
                    {
                        //开始测量
                        supportOnPosition = false;
                        probeOnPosition = false;
                        if (!CheckTemperatureIsNormal())
                        {
                            fx5u.SendCmd("D173");
                            UploadErrorA("A-004", "温度异常，不适合测量");
                            break;
                        }
                        fx5u.SendCmd("D142");//测量使能
                        curBatteryi = 0;
                        if (curConfig.BatteryNum == 4)
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测4块电池，打开下层第一通道");
                            SendFx5uCmd(PlcLabel.D16);
                            curIOAddrStr = "D16";
                        }
                        else
                        {
                            Thread.Sleep(10);
                            logStreamWriter.WriteLine(DateTime.Now.ToString() + "检测2块电池，打开下层第二通道");
                            SendFx5uCmd(PlcLabel.D17);
                            curIOAddrStr = "D17";
                        }
                        //检测电池啊啊
                        bt3562.DataReceivedEventHandler = new Action<string>(ChannelChangedCallBack);
                        Thread.Sleep(500);  //让电池检测仪去充分测量数据，然后再去获取
                        //发送测试指令
                        bt3562.SendCmd(":FETCH?");
                        logStreamWriter.WriteLine(DateTime.Now.ToString() + "已经发送了:FETCH?指令");
                    }
                    fx5u.EventMeditator = AutoRun;
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"探针下降退出");
                    break;
                case PlcLabel.D144://复位了
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D144", 0);
                    logStreamWriter.WriteLine(DateTime.Now.ToString()+"设备复位");
                    if (!errorFlag)
                    {
                        statusUpload.UploadStatus(DeviceStatus.D);
                    }
                    sr2000w.DataReceivedEventHandler = null;
                    bt3562.DataReceivedEventHandler = null;
                    sr2000w.SendCmd("LOFF");
                    Thread.Sleep(50);
                    sr2000w.SendCmd("WP,251,0");
                    plcState = PlcState.STOP;
                    StopEvent?.Invoke();
                    break;
                case PlcLabel.D149: //下位机故障了
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D149", 0);
                    fx5u.EventMeditator = AutoRun;
                    Action errorCheckAsy = new Action(ErrorChecker);
                    errorCheckAsy.BeginInvoke(null, null);
                    break;
                //case PlcLabel.D155: //按钮状态变更了
                //    fx5u.EventMeditator = null;
                //    fx5u.ResetRegister("D155", 0);
                //    fx5u.EventMeditator = AutoRun;
                //    Action statuCheckAsy = new Action(StatusChecker);
                //    statuCheckAsy.BeginInvoke(null, null);
                //    break;
                case PlcLabel.D171: //设备启动了
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D171", 0);
                    fx5u.EventMeditator = AutoRun;
                    workRunWatchEventHandler?.BeginInvoke(true,null,null);
                    statusUpload.UploadStatus(DeviceStatus.A);
                    break;
                case PlcLabel.D172:     //设备暂停了
                    fx5u.EventMeditator = null;
                    fx5u.ResetRegister("D172", 0);
                    fx5u.EventMeditator = AutoRun;
                    workRunWatchEventHandler?.BeginInvoke(false,null,null);
                    statusUpload.UploadStatus(DeviceStatus.B);
                    break;
            }
        }
        private void ErrorChecker()
        {
            string startDevice = "D150";
            int iSize = 4;
            short[] sData = new short[4];
            fx5u.ReadDeviceBlock(startDevice, iSize, ref sData);
            if (sData[0] == 1) //电机故障
            {
                ErrorEventHandler?.BeginInvoke("电机故障",null,null);
                UploadErrorB("B-001", "电机故障");
            }
            if (sData[1] == 1)
            {
                ErrorEventHandler?.BeginInvoke("设备发生碰撞",null,null);
                UploadErrorB("B-002", "设备发生碰撞");
            }
            if (sData[2] == 1)
            {
                ErrorEventHandler?.BeginInvoke("探针挤压过大",null,null);
                UploadErrorB("B-003", "探针挤压过大");
            }
            if (sData[3] == 1)
            {
                ErrorEventHandler?.BeginInvoke("触发硬限位",null,null);
                UploadErrorB("B-004", "触发硬限位");
            }
            sData = new short[4] { 0, 0, 0, 0 };
            fx5u.WriteDeviceBlock(startDevice, iSize, ref sData);
        }
        private void UploadErrorB(string err_code, string err_desc)
        {
            Log myLog = new Log();
            statusUpload.UploadStatus(DeviceStatus.C);
            statusUpload.UploadError(err_code, err_desc,ref myLog);
            LogShowInDataGridView?.BeginInvoke(myLog,null,null);
        }
        //检查是变为了什么状态，然后发送，清零哦
        private void StatusChecker()
        {
            string startDevice = "D155";
            int iSize = 3;
            short[] sData = new short[3];
            fx5u.ReadDeviceBlock(startDevice, iSize, ref sData);
            if (sData[0] == 1) //启动
            {
                workRunWatchEventHandler(true);
                statusUpload.UploadStatus(DeviceStatus.A);
            }
            if (sData[1] == 1) //停止
            {
                workRunWatchEventHandler(false);
                statusUpload.UploadStatus(DeviceStatus.B);
            }
            if (sData[2] == 1)
            {
                workRunWatchEventHandler(false);
                statusUpload.UploadStatus(DeviceStatus.B);
            }
            sData = new short[3] { 0, 0, 0 };
            fx5u.WriteDeviceBlock(startDevice, iSize, ref sData);
        }
        public bool CheckIsSns(string sns)
        {
            if (sns == ""||sns=="ERROR")
            {
                return false;
            }
            else
                return true;
        }
        private string Sr2000wErr(string err)
        {
            //打包错误信息
            string errMessage = "SR2000W错误，错误代码" + err;
            return errMessage;
        }
        private string Fx5uErr(string err)
        {
            //打包错误信息
            string errMessage = "FX5U错误，错误代码：" + err;
            return errMessage;
        }
        public bool CheckRVNormal(double r,double v)
        {
            if (r == 10000000000||r==100000000)
                return false;
            if (v == 10000000000||r==100000000)
                return false;
            return true;
        }
        private bool CheckBatteriesErrorAlways()
        {
            if (statistic.Count < curConfig.StatisticNum)
                return false;
            int counter=0;
            foreach(bool b in statistic)
            {
                counter += (b ? 0 : 1);
            }
            if (((double)counter )/ curConfig.StatisticNum > curConfig.ErrorRate)
                return true;
            return false;
        }
    }
}

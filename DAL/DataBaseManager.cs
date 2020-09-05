using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DAL
{
    /// <summary>
    /// 这一个类有很多函数供逻辑层和表示层调用
    /// </summary>
    public class DataManager
    {
        static int VOLTAGE_UNIT = 1000;
        static int RESISTANCE_UNIT = 1000;
        static Dictionary<BatteryErrorType, string> myErrorDictionary = new Dictionary<BatteryErrorType, string>();
        static DataManager()
        {
            myErrorDictionary.Add(BatteryErrorType.OK, "正常");
            myErrorDictionary.Add(BatteryErrorType.BelowMinResistance, "低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxResistance, "大于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltage, "低于正常电压值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltageBelowMinResistance, "低于正常电压值，低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.BelowMinVoltageOverMaxResistance, "低于正常电压值，大于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltage, "大于正常电压值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltageBelowMinResistance, "大于正常电压值，低于正常电阻值");
            myErrorDictionary.Add(BatteryErrorType.OverMaxVoltageOverMaxResistance, "大于正常电压值，大于正常电阻值");
        }
        public static DataTable GetLogInfobyDay(int day,string fName)
        {
            string sql = "select operTime,fName,process,userId,resourceId," +
                "sn,result,message,userName,shop_order,tech_no,inspection_Item,inspection_Desc,standard,upper_limit,lower_limit,flag,ng_code," +
                "resistance,voltage,origin_voltage,temperature,K,o1_voltage,o1_date,status,status_des,err_code,err_desc from log " +
                "where to_days(now())-to_days(operTime)=@day";
            sql += ((fName == "") ? ";" : " and fName=@fName;");
            MySqlParameter p1 = new MySqlParameter("@day", day);
            DataTable dt;
            if (fName == "")
                dt = DBOper.GetDataTable(sql, p1);
            else
            {
                MySqlParameter p2 = new MySqlParameter("@fName", fName);
                dt = DBOper.GetDataTable(sql, p1, p2);
            }
            return dt;
        }
        public static void updateBatterySavedflag(string sn, string operation_id)
        {
            //更新数据表
            string sql = "update battery set savedflag='上传成功' where sn=@sn and operation_id=@operation_id;";
            MySqlParameter p1 = new MySqlParameter("@sn", sn);
            MySqlParameter p2 = new MySqlParameter("@operation_id", operation_id);
            DBOper.ExecuteCommand(sql, p1, p2);
        }

        public static DataTable GetBatteryInfobyDay(int day)
        {
            string sql = @"select sn,opertime,user,operation_id," +
                    "resistance,voltage,origin_voltage,temCoeff,temperature,K,o1_voltage,o1_date,result,errtype,savedflag,verifyflag,techId,shop_order from battery " +
                    "where to_days(now())-to_days(opertime)=@day;";
            MySqlParameter p1 = new MySqlParameter("@day", day);
            DataTable dt = DBOper.GetDataTable(sql, p1);
            return dt;
        }

        public static bool LoadLog2CSV(string csvpath,int day,string fName)
        {
            try
            {
                string sqlcmd = @"select * into outfile @csvpath character set GBK fields terminated by ',' lines terminated by '\n' " +
                "from (select '操作时间' as operTime,'接口' as fName,'工序' as process,'操作员工号' as userId,'设备资源编号' as resourceId,'序列号' as sn,'返回结果' as result,"+
                "'返回信息' as message,'操作员姓名' as userName,'工单' as shop_order,'型号' as tech_no,'检测项目' as inspection_Item,'检测项目描述' as inspection_Desc,"+
                "'标准值' as standard,'上限值' as upper_limit,'下限值' as lower_limit,'检测结果' as flag,'错误说明' as ng_code,'电阻' as resistance,'电压' as voltage,"+
                "'补偿前电压' as origin_voltage,'温度' as temperature,'K值' as K,'o1工序电压' as o1_voltage,'o1工序测量时间' as o1_date,'设备状态代号' as status,"+
                "'设备状态' as status_des,'故障代号' as err_code,'故障描述' as err_desc union select operTime,fName,process,userId,resourceId," +
                "sn,result,message,userName,shop_order,tech_no,inspection_Item,inspection_Desc,standard,upper_limit,lower_limit,flag,ng_code," +
                "resistance,voltage,origin_voltage,temperature,K,o1_voltage,o1_date,status,status_des,err_code,err_desc from log " +
                "where to_days(now())-to_days(operTime)<=@day";
                sqlcmd+=((fName=="")?") b;":" and fName=@fName) b;");
                MySqlParameter path = new MySqlParameter("@csvpath", csvpath);
                MySqlParameter pday = new MySqlParameter("@day", day);
                if (fName=="")
                {
                    DBOper.ExecuteCommand(sqlcmd, path, pday);
                }
                else
                {
                    MySqlParameter pfName = new MySqlParameter("@fName", fName);
                    DBOper.ExecuteCommand(sqlcmd, path, pfName, pday);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool LoadBattery2CSV(string csvpath,int day)
        {
            try
            {
                string sqlcmd = @"select * into outfile @csvpath character set gbk fields terminated by ',' lines terminated by '\n' " +
                    "from (select '序列号' as sn,'操作时间' as opertime,'操作员工' as user,'工序' as operation_id,'电阻' as resistance,'电压' as voltage,"+
                    "'补偿前电压' as origin_voltage,'温度' as temperature,'K值' as K,'O1工序电压' as o1_voltage,'O1工序测量时间' as o1_date,'检测结果' as result,"+
                    "'错误类型' as errtype,'上传状态' as savedflag,'校验结果' as verifyflag,'型号' as techId,'工单' as shop_order,'温度补偿系数' as temCoeff union select sn,opertime,user,operation_id," +
                    "resistance,voltage,origin_voltage,temperature,K,o1_voltage,o1_date,result,errtype,savedflag,verifyflag,techId,shop_order,temCoeff from battery " +
                    "where to_days(now())-to_days(opertime)<=@day) b;";
                MySqlParameter path = new MySqlParameter("@csvpath", csvpath);
                MySqlParameter pday = new MySqlParameter("@day", day);
                DBOper.ExecuteCommand(sqlcmd, path, pday);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //是否要判断一下，是否已经有这个序列的电池了，如果有就更新，没有的话就插入
        public static void BatteryVerifyBatteryWrite(Battery battery)
        {
            //先判断是否有，有则更新，无则插入
            string sqlSelectCmd = "select * from battery where sn=@sn and operation_id=@operation_id";
            MySqlParameter sn = new MySqlParameter("@sn", battery.sn);
            MySqlParameter operation_id = new MySqlParameter("@operation_id", battery.operation_id);
            MySqlDataReader reader= DBOper.GetReader(sqlSelectCmd, sn,operation_id);
            string sqlWriteCmd = "";
            MySqlParameter p1;
            MySqlParameter p2;
            MySqlParameter p3;
            MySqlParameter p4;
            MySqlParameter p5;
            MySqlParameter p6;
            MySqlParameter p7;
            MySqlParameter p8;
            MySqlParameter p9;
            bool existedFlag = reader.HasRows;
            DBOper.CloseReader();
            if (existedFlag)//如果有这个工序的电池的话
            {
                if(battery.operation_id=="O1T")
                {
                    //则更新
                    sqlWriteCmd = "update battery set opertime=@opertime,verifyflag=@verifyflag,user=@user,techid=@techid,shop_order=@shop_order where sn=@sn and operation_id=@operation_id";
                    p1 = new MySqlParameter("@sn", battery.sn);

                    p2 = new MySqlParameter("@opertime", battery.operTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    p3 = new MySqlParameter("@verifyflag", battery.verifyFlag ? "校验成功" : "校验失败");
                    p4 = new MySqlParameter("@user", battery.user);
                    p5 = new MySqlParameter("@techId", battery.techId);
                    p6 = new MySqlParameter("@operation_id", battery.operation_id);
                    p7 = new MySqlParameter("@shop_order", battery.shop_order);
                    DBOper.ExecuteCommand(sqlWriteCmd, p1, p2, p3, p4, p5, p6,p7);

                }
                else if(battery.operation_id=="O2T")
                {
                    //则更新
                    sqlWriteCmd = "update battery set opertime=@opertime,verifyflag=@verifyflag,user=@user,o1_date=@o1_date,o1_voltage=@o1_voltage,techid=@techid,shop_order=@shop_order where sn=@sn and operation_id=@operation_id";
                    p1 = new MySqlParameter("@sn", battery.sn);

                    p2 = new MySqlParameter("@opertime", battery.operTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    p3 = new MySqlParameter("@verifyflag", battery.verifyFlag ? "校验成功" : "校验失败");
                    p4 = new MySqlParameter("@user", battery.user);
                    p5 = new MySqlParameter("@techId", battery.techId);
                    p6 = new MySqlParameter("@operation_id", battery.operation_id);
                    p7 = new MySqlParameter("@o1_voltage",Math.Round(battery.o1_voltage*VOLTAGE_UNIT,4));
                    p8 = new MySqlParameter("@o1_date", battery.o1_date);
                    p9 = new MySqlParameter("@shop_order", battery.shop_order);
                    DBOper.ExecuteCommand(sqlWriteCmd, p1, p2, p3, p4, p5, p6,p7,p8,p9);
                }
            }
            else
            {
                if(battery.operation_id=="O1T")
                {
                    sqlWriteCmd = "insert into battery (sn,opertime,verifyflag,user,techId,operation_id,shop_order) values(@sn,@opertime,@verifyflag,@user,@techId,@operation_id,@shop_order)";
                    p1 = new MySqlParameter("@sn", battery.sn);
                    p2 = new MySqlParameter("@opertime", battery.operTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    p3 = new MySqlParameter("@verifyflag", battery.verifyFlag ? "校验成功" : "校验失败");
                    p4 = new MySqlParameter("@user", battery.user);
                    p5 = new MySqlParameter("@techId", battery.techId);
                    p6 = new MySqlParameter("@operation_id", battery.operation_id);
                    p7 = new MySqlParameter("@shop_order", battery.shop_order);
                    DBOper.ExecuteCommand(sqlWriteCmd, p1, p2, p3, p4, p5, p6,p7);
                }
                else if(battery.operation_id=="O2T")
                {
                    sqlWriteCmd = "insert into battery (sn,opertime,verifyflag,user,techId,operation_id,o1_voltage,o1_date,shop_order) values(@sn,@opertime,@verifyflag,@user,@techId,@operation_id,@o1_voltage,@o1_date,@shop_order)";
                    p1 = new MySqlParameter("@sn", battery.sn);
                    p2 = new MySqlParameter("@opertime", battery.operTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    p3 = new MySqlParameter("@verifyflag", battery.verifyFlag ? "校验成功" : "校验失败");
                    p4 = new MySqlParameter("@user", battery.user);
                    p5 = new MySqlParameter("@techId", battery.techId);
                    p6 = new MySqlParameter("@operation_id", battery.operation_id);
                    p7 = new MySqlParameter("@o1_voltage", Math.Round(battery.o1_voltage * VOLTAGE_UNIT,4));
                    p8 = new MySqlParameter("@o1_date", battery.o1_date);
                    p9 = new MySqlParameter("@shop_order", battery.shop_order);
                    DBOper.ExecuteCommand(sqlWriteCmd, p1, p2, p3, p4, p5, p6, p7, p8,p9);
                }
            }
        }

        public static void LogWrite(Log mylog)
        {
            string sqlWriteCmd = "insert into log(operTime,fName,process,userId,resourceId,sn,result,message,userName,shop_order,tech_no,inspection_Item," +
                 "inspection_Desc,standard,upper_limit,lower_limit,flag,ng_code,resistance,voltage,status,status_des,err_code,err_desc,o1_voltage,o1_date,K,temperature,origin_voltage) " +
                 "values(@operTime,@fName,@process,@userId,@resourceId,@sn,@result,@message,@userName,@shop_order,@tech_no,@inspection_Item," +
                 "@inspection_Desc,@standard,@upper_limit,@lower_limit,@flag,@ng_code,@resistance,@voltage,@status,@status_des,@err_code,@err_desc,@o1_voltage,@o1_date,@K,@temperature,@origin_voltage)";
            MySqlParameter p1 = new MySqlParameter("@operTime", mylog.operTime);
            MySqlParameter p2 = new MySqlParameter("@fName", mylog.fName);
            MySqlParameter p3 = new MySqlParameter("@process", mylog.process);
            MySqlParameter p4 = new MySqlParameter("@userId", mylog.userId);
            MySqlParameter p5 = new MySqlParameter("@resourceId", mylog.resourceId);
            MySqlParameter p6 = new MySqlParameter("@sn", mylog.sn);
            MySqlParameter p7 = new MySqlParameter("@result", mylog.result);
            MySqlParameter p8 = new MySqlParameter("@message", mylog.message);
            MySqlParameter p9 = new MySqlParameter("@userName", mylog.userName);
            MySqlParameter p10 = new MySqlParameter("@shop_order", mylog.shop_order);
            MySqlParameter p11 = new MySqlParameter("@tech_no", mylog.tech_no);
            MySqlParameter p12 = new MySqlParameter("@inspection_Item", mylog.inspection_Item);
            MySqlParameter p13 = new MySqlParameter("@inspection_Desc", mylog.inspection_Desc);
            MySqlParameter p14 = new MySqlParameter("@standard", mylog.standard);
            MySqlParameter p15 = new MySqlParameter("@upper_limit", mylog.upper_limit);
            MySqlParameter p16 = new MySqlParameter("@lower_limit", mylog.lower_limit);
            MySqlParameter p17 = new MySqlParameter("@flag", mylog.flag);
            MySqlParameter p18 = new MySqlParameter("@ng_code", mylog.ng_code);
            MySqlParameter p19 = new MySqlParameter("@resistance", mylog.resistance);
            MySqlParameter p20 = new MySqlParameter("@voltage", mylog.voltage);
            MySqlParameter p21 = new MySqlParameter("@status", mylog.status);
            MySqlParameter p22 = new MySqlParameter("@status_des", mylog.status_des);
            MySqlParameter p23 = new MySqlParameter("@err_code", mylog.err_code);
            MySqlParameter p24 = new MySqlParameter("@err_desc", mylog.err_desc);
            MySqlParameter p25 = new MySqlParameter("@o1_voltage", mylog.o1_voltage);
            MySqlParameter p26 = new MySqlParameter("@o1_date", mylog.o1_date);
            MySqlParameter p27 = new MySqlParameter("@K", mylog.K);
            MySqlParameter p28 = new MySqlParameter("@temperature", mylog.tempearture);
            MySqlParameter p29 = new MySqlParameter("@origin_voltage", mylog.origin_voltage);
            DBOper.ExecuteCommand(sqlWriteCmd, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24,p25,p26,p27,p28,p29);
        }
        public static void BatterySaveBatteryWrite(Battery battery)
        {
            if(battery.operation_id=="O1T")
            {
                string sqlcmd = @"update battery set opertime=@opertime, result=@result,errtype=@errtype,savedflag=@savedflag,
                        voltage=@voltage,resistance=@resistance,temperature=@temperature,origin_voltage=@origin_voltage,temCoeff=@temCoeff where sn=@sn and operation_id='O1T';";
                MySqlParameter p1 = new MySqlParameter("@opertime", battery.operTime.ToString());
                MySqlParameter p2 = new MySqlParameter("@result", battery.result ? "合格" : "不合格");
                MySqlParameter p3 = new MySqlParameter("@errtype", myErrorDictionary[battery.errorType]);
                MySqlParameter p4 = new MySqlParameter("@savedflag", battery.MesSaved ? "上传成功" : "上传失败");
                MySqlParameter p5 = new MySqlParameter("@sn", battery.sn);
                MySqlParameter p6 = new MySqlParameter("@voltage", Math.Round(battery.voltage * VOLTAGE_UNIT,4));
                MySqlParameter p7 = new MySqlParameter("@resistance", Math.Round(battery.resistance * RESISTANCE_UNIT,4));
                MySqlParameter p8 = new MySqlParameter("@origin_voltage", Math.Round(battery.origin_voltage * VOLTAGE_UNIT,4));
                MySqlParameter p9 = new MySqlParameter("@temperature", battery.temperature);
                MySqlParameter p10 = new MySqlParameter("@temCoeff", battery.tempCoeff);
                DBOper.ExecuteCommand(sqlcmd, p1, p2, p3, p4, p5, p6, p7,p8,p9,p10);
            }
            else if(battery.operation_id=="O2T")
            {
                string sqlcmd = @"update battery set opertime=@opertime, result=@result,errtype=@errtype,savedflag=@savedflag,
                        voltage=@voltage,resistance=@resistance,K=@K,temperature=@temperature,origin_voltage=@origin_voltage,temCoeff=@temCoeff where sn=@sn and operation_id='O2T';";
                MySqlParameter p1 = new MySqlParameter("@opertime", battery.operTime.ToString());
                MySqlParameter p2 = new MySqlParameter("@result", battery.result ? "合格" : "不合格");
                string errtype = myErrorDictionary[battery.errorType];
                if (battery.errorType == BatteryErrorType.OK && !battery.k_flag)  //如果电阻电压正常，而K值异常
                    errtype = "K值异常";
                else if (battery.errorType != BatteryErrorType.OK && !battery.k_flag)   //如果电阻电压不正常，而且K值异常
                    errtype += "，K值异常";
                MySqlParameter p3 = new MySqlParameter("@errtype", errtype);
                MySqlParameter p4 = new MySqlParameter("@savedflag", battery.MesSaved ? "上传成功" : "上传失败");
                MySqlParameter p5 = new MySqlParameter("@sn", battery.sn);
                MySqlParameter p6 = new MySqlParameter("@voltage", Math.Round(battery.voltage * VOLTAGE_UNIT,4));
                MySqlParameter p7 = new MySqlParameter("@resistance", Math.Round(battery.resistance * RESISTANCE_UNIT,4));
                MySqlParameter p8 = new MySqlParameter("@origin_voltage", Math.Round(battery.origin_voltage * VOLTAGE_UNIT,4));
                MySqlParameter p9 = new MySqlParameter("@temperature", battery.temperature);
                MySqlParameter p10 = new MySqlParameter("@K", battery.K);
                MySqlParameter p11 = new MySqlParameter("@temCoeff", battery.tempCoeff);
                DBOper.ExecuteCommand(sqlcmd, p1, p2, p3, p4, p5, p6, p7,p8,p9,p10,p11);
            }
        }
    }
}

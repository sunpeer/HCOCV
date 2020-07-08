using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DAL;
using Model;

namespace BLL
{
    enum FName
    {
        Login,
        GetResource,
        GetData,
        GetTechnologyStandard,
        UploadBatteryData,
        UploadDeviceStatus,
        UploadError,
        SetupOperation
    }
    public class MesConnect
    {
        string resouce_id = "";
        string user_code = "";
        //string shop_order = "";
        string tech_no = "";
        string opeation_id = "";

        MESOCV ocv = new MESOCV();
        public MesConnect(string tech_no,string operation_id)
        {
            this.tech_no = tech_no;
            this.opeation_id = operation_id;
        }
        public bool SetupOperation(string operation_id,ref string Msg,ref Log myLog)
        {
            string strRes = "";
            bool result =ocv.SetupOperation(operation_id, ref strRes);
            if(result)
            {
                UploadDataResponseClass responseClass = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRes);
                if(responseClass.result=="OK")
                {
                    this.opeation_id = operation_id;
                    SetupOperationLogWrite(operation_id, "OK", "", ref myLog);
                    return true;
                }
                Msg = responseClass.message;
                SetupOperationLogWrite(operation_id, "NG", responseClass.message, ref myLog);
                return false;
            }
            Msg = "请求不成功。";
            SetupOperationLogWrite(operation_id, "NG", Msg, ref myLog);
            return false;
        }
        private void SetupOperationLogWrite(string operation_id,string result,string message,ref Log myLog)
        {
            myLog = PackMesLog(FName.SetupOperation, operation_id, result + "&" + message);
            DataManager.LogWrite(myLog);
        }
        public bool Login(string id, ref string Msg, ref string UserName,ref Log mylog)
        {
            string strRes = "";
            bool result = ocv.UserLogin(id, ref strRes);
            //请求成功
            if (result)
            {
                //返回体解析成类
                UserLoginResponseClass responseClass = JsonConvert.DeserializeObject<UserLoginResponseClass>(strRes);
                if (responseClass.result == "OK")
                {
                    user_code = id;
                    UserName = responseClass.UserName;
                    LoginLogWrite(id, "OK", responseClass.message,UserName,ref mylog);
                    return true;
                }
                Msg = responseClass.message;
                LoginLogWrite(id, "NG", responseClass.message,UserName,ref mylog);
                return false;
            }
            Msg = "请求不成功。";
            LoginLogWrite(id, "NG", Msg, "",ref mylog);
            return false;
        }
        private void LoginLogWrite(string userId,string result,string message,string userName,ref Log myLog)
        {
            myLog=PackMesLog(FName.Login, userId, result + "&" + message + "&" + userName);
            DataManager.LogWrite(myLog);
        }

        private Log PackMesLog(FName fName, string send, string response)
        {
            Log mylog = new Log();
            string[] arrySend = send.Split('&');
            string[] arryResponse = response.Split('&');
            //时间
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mylog.operTime = time;
            //接口名称
            mylog.fName = fName.ToString();
            mylog.process = this.opeation_id;
            //发送内容
            switch (fName)
            {
                case FName.SetupOperation:
                    mylog.process = arrySend[0];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    break;
                case FName.Login:
                    mylog.userId = arrySend[0];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    mylog.userName = arryResponse[2];
                    break;
                case FName.GetResource:
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    mylog.resourceId = arryResponse[2];
                    break;
                case FName.GetData:
                    mylog.resourceId = arrySend[0];
                    mylog.sn = arrySend[1];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    mylog.shop_order = arryResponse[2];
                    mylog.tech_no = arryResponse[3];
                    mylog.o1_voltage = arryResponse[4];
                    mylog.o1_date = arryResponse[5];
                    break;
                case FName.GetTechnologyStandard:
                    mylog.resourceId = arrySend[0];
                    mylog.tech_no = arrySend[1];
                    mylog.inspection_Item = arryResponse[0];
                    mylog.inspection_Desc = arryResponse[1];
                    mylog.standard = arryResponse[2];
                    mylog.upper_limit = arryResponse[3];
                    mylog.lower_limit = arryResponse[4];
                    mylog.message = arryResponse[5];
                    break;
                case FName.UploadBatteryData:
                    mylog.resourceId = arrySend[0];
                    mylog.tech_no = arrySend[1];
                    mylog.sn = arrySend[2];
                    mylog.userId = arrySend[3];
                    mylog.flag = arrySend[4];
                    mylog.ng_code = arrySend[5];
                    mylog.resistance = arrySend[6]; //DATA1电压
                    mylog.voltage = arrySend[7];    //DATA2电阻
                    mylog.K = arrySend[8];
                    mylog.origin_voltage = arrySend[9];
                    mylog.tempearture = arrySend[10];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    
                    break;
                case FName.UploadDeviceStatus:
                    mylog.resourceId = arrySend[0];
                    mylog.status = arrySend[1];
                    mylog.status_des = arrySend[2];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    break;
                case FName.UploadError:
                    mylog.resourceId = arrySend[0];
                    mylog.err_code = arrySend[1];
                    mylog.err_desc = arrySend[2];
                    mylog.result = arryResponse[0];
                    mylog.message = arryResponse[1];
                    break;
            }
            return mylog;
        }
        private void GetResourceLogWrite(string result,string message,string resourceId,ref Log myLog)
        {
            myLog= PackMesLog(FName.GetResource, "", result + "&" + message + "&" + resourceId);
            DataManager.LogWrite(myLog);
        }
        
        public bool GetResource(ref string Msg, ref string resouce_id,ref Log myLog)
        {
            string strRe = "";
            bool result = ocv.GetResource(ref strRe);
            if (result) //请求成功
            {
                GetResourceResponseClass responseClass = JsonConvert.DeserializeObject<GetResourceResponseClass>(strRe);
                if (responseClass.result == "OK")
                {
                    this.resouce_id = responseClass.resource_id;
                    resouce_id = responseClass.resource_id;
                    GetResourceLogWrite("OK", responseClass.message, responseClass.resource_id,ref myLog);
                    return true;
                }
                Msg = responseClass.message;
                GetResourceLogWrite("NG", Msg, responseClass.resource_id,ref myLog);
                return false;
            }
            Msg = "请求不成功。";
            GetResourceLogWrite("NG", Msg, "",ref myLog);
            return false;
        }
        private void GetDataLogWrite(string resourceId,string sn,string result,string message,string shop_order,string tech_no,string voltage,string date,ref Log myLog)
        {
            myLog = PackMesLog(FName.GetData, resourceId + "&" + sn, result + "&" + message + "&" + shop_order + "&" + tech_no+"&"+voltage+"&"+date);
            DataManager.LogWrite(myLog);
        }

        public bool GetData(string barcode,bool firstFlag, ref string Msg, ref string shop_order, ref string tech_no,ref string o1_voltage,ref string o1_date,ref Log myLog)
        {
            if (this.resouce_id == "")
            {
                Msg = "没有获得设备资源号";
                return false;
            }
            string strRe = "";
            bool result = ocv.GetData(barcode, this.resouce_id, ref strRe);
            if (result)//如果请求成功
            {
                if(this.opeation_id=="O1T")
                {
                    GetDataResponseClass responseClass = JsonConvert.DeserializeObject<GetDataResponseClass>(strRe);
                    if (responseClass.result == "OK")
                    {
                        if (firstFlag)
                            this.tech_no = responseClass.tech_no;
                        shop_order = responseClass.shop_order;
                        tech_no = responseClass.tech_no;
                        GetDataLogWrite(this.resouce_id, barcode, "OK", responseClass.message, shop_order, tech_no,"","", ref myLog);
                        return true;
                    }
                    Msg = responseClass.message;
                    GetDataLogWrite(this.resouce_id, barcode, "NG", responseClass.message, responseClass.shop_order, responseClass.tech_no,"","", ref myLog);
                    return false;
                }
                else if (this.opeation_id == "O2T")
                {
                    GetDataOCV2ResponseClass responseClass = JsonConvert.DeserializeObject<GetDataOCV2ResponseClass>(strRe);
                    if (responseClass.result == "OK")
                    {
                        if (firstFlag)
                            this.tech_no = responseClass.tech_no;
                        shop_order = responseClass.shop_order;
                        tech_no = responseClass.tech_no;
                        o1_voltage = responseClass.voltage;
                        o1_date = responseClass.date;
                        GetDataLogWrite(this.resouce_id, barcode, "OK", responseClass.message, responseClass.shop_order, responseClass.tech_no, responseClass.voltage, responseClass.date, ref myLog);
                        return true;
                    }
                    Msg = responseClass.message;
                    GetDataLogWrite(this.resouce_id, barcode, "NG", responseClass.message, "", "", "", "", ref myLog);
                    return false;
                }
            }
            Msg = "请求不成功";
            GetDataLogWrite(this.resouce_id, barcode, "NG", Msg, "", "", "", "", ref myLog);
            return false;
        }
        private void GetTechnologyStandardLogWrite(string resourceId, string tech_no, string inspection_items, string inspection_desc,
            string standard, string upper_limit, string lower_limit,string msg,ref Log myLog)
        {
            myLog = PackMesLog(FName.GetTechnologyStandard, resourceId + "&" + tech_no, inspection_items + "&" + inspection_desc + "&" + standard + "&" + upper_limit + "&" + lower_limit+"&"+msg);
            DataManager.LogWrite(myLog);
        }
        //传入tech_no，返回所有请求到的工艺标准，没有OK/NG，只有有没有这么一说
        public bool GetTechnologyStandard(string tech_no, ref string Msg, ref List<GetTechnologyStandardResponseClass> technologies,ref Log myLog)
        {
            //if (this.tech_no == "")
            //{
            //    Msg = "没有获取型号";
            //    return false;
            //}
            string strRe = "";
            bool result = ocv.GetTechnologyStandard(tech_no, ref strRe);
            if (result) //请求成功
            {
                //解析返回体
                technologies = JsonConvert.DeserializeObject<List<GetTechnologyStandardResponseClass>>(strRe);
                if(technologies.Count!=0)
                {
                    int count = technologies.Count;
                    string[] inspection_items = new string[count];
                    string[] inspection_descs = new string[count];
                    string[] standards = new string[count];
                    string[] upper_limits = new string[count];
                    string[] lower_limits = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        inspection_items[i] = technologies[i].INSPECTION_ITEM;
                        inspection_descs[i] = technologies[i].INSPECTION_DESC;
                        standards[i] = technologies[i].STANDARD;
                        upper_limits[i] = technologies[i].UPPER_LIMIT;
                        lower_limits[i] = technologies[i].LOWER_LIMIT;
                    }
                    string inspection_item = string.Join(";", inspection_items);
                    string inspection_desc = string.Join(";", inspection_descs);
                    string standard = string.Join(";", standards);
                    string upper_limit = string.Join(";", upper_limits);
                    string lower_limit = string.Join(";", lower_limits);
                    GetTechnologyStandardLogWrite(this.resouce_id, tech_no, inspection_item, inspection_desc, standard,upper_limit, lower_limit,"",ref myLog);
                    return true;
                }
                else
                {
                    Msg = "没有返回工艺标准";
                    GetTechnologyStandardLogWrite(this.resouce_id, tech_no, "", "", "", "", "", Msg,ref myLog);
                    return false;
                }
            }
            Msg = "请求不成功";
            GetTechnologyStandardLogWrite(this.resouce_id, tech_no, "", "", "", "", "", Msg,ref myLog);
            return false;
        }
        private void UploadBatteryDataLogWrite(string resourceId, string tech_no, string sn, string userId, string flag, string ng_code,
            string resistance, string voltage,string K,string origin_voltage,string temperature, string result, string message,ref Log myLog)
        {
            myLog = PackMesLog(FName.UploadBatteryData, resourceId + "&" + tech_no + "&" + sn + "&" + userId + "&" + flag + "&" + 
                ng_code + "&" + resistance + "&" + voltage+"&"+K+"&"+origin_voltage+"&"+temperature,result + "&" + message);
            DataManager.LogWrite(myLog);
        }
        public bool UploadBatteryAgain(string sfc,string flag,string cz_date,string ng_code,string origin_voltage,string operation_id,UploadBatteryDataClass uploadData,ref string msg,ref Log myLog)
        {
            //做好访问的参数来
            string jsonData = JsonConvert.SerializeObject(uploadData);
            string strRe = "";
            bool result = ocv.UploadBatteryData(sfc, resouce_id, cz_date, user_code, flag, ng_code, jsonData, operation_id, ref strRe);
            if (result)  //如果请求成功
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, "", origin_voltage, uploadData.DATA4, "OK", response.message, ref myLog);
                    return true;
                }
                msg = response.message;
                UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, "", origin_voltage, uploadData.DATA4, "NG", response.message, ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, "", origin_voltage, uploadData.DATA4, "NG", msg, ref myLog);
            return false;
        }
        public bool UploadBatteryO2TAgain(string sfc, string flag, string cz_date, string ng_code, string origin_voltage,string operation_id, UploadBatteryO2TDataClass uploadData, ref string msg, ref Log myLog)
        {
            //做好访问的参数来
            string jsonData = JsonConvert.SerializeObject(uploadData);
            //string cz_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strRe = "";
            bool result = ocv.UploadBatteryData(sfc, resouce_id, cz_date, user_code, flag, ng_code, jsonData, operation_id, ref strRe);
            if (result)  //如果请求成功
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, uploadData.DATA3, origin_voltage, uploadData.DATA4, "OK", response.message, ref myLog);
                    return true;
                }
                msg = response.message;
                UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, uploadData.DATA3, origin_voltage, uploadData.DATA4, "NG", response.message, ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, uploadData.DATA3, origin_voltage, uploadData.DATA4, "NG", msg, ref myLog);
            return false;
        }
        public bool UploadBatteryData(string sfc, string flag, string ng_code,string origin_voltage, UploadBatteryDataClass uploadData, ref string msg,ref Log myLog)
        {
            //做好访问的参数来
            string jsonData = JsonConvert.SerializeObject(uploadData);
            string cz_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strRe = "";
            bool result = ocv.UploadBatteryData(sfc, resouce_id, cz_date, user_code, flag, ng_code, jsonData,this.opeation_id, ref strRe);
            if (result)  //如果请求成功
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1,"",origin_voltage, uploadData.DATA4, "OK", response.message,ref myLog);
                    return true;
                }
                msg = response.message;
                UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1,"", origin_voltage, uploadData.DATA4, "NG", response.message,ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, "", origin_voltage, uploadData.DATA4, "NG", msg,ref myLog);
            return false;
        }
        public bool UploadBatteryO2TData(string sfc, string flag, string ng_code,string origin_voltage, UploadBatteryO2TDataClass uploadData, ref string msg, ref Log myLog)
        {
            //做好访问的参数来
            string jsonData = JsonConvert.SerializeObject(uploadData);
            string cz_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strRe = "";
            bool result = ocv.UploadBatteryData(sfc, resouce_id, cz_date, user_code, flag, ng_code, jsonData, this.opeation_id, ref strRe);
            if (result)  //如果请求成功
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1,uploadData.DATA3, origin_voltage, uploadData.DATA4, "OK", response.message, ref myLog);
                    return true;
                }
                msg = response.message;
                UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, uploadData.DATA3, origin_voltage, uploadData.DATA4, "NG", response.message, ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadBatteryDataLogWrite(this.resouce_id, uploadData.DATA03, sfc, this.user_code, flag, ng_code, uploadData.DATA2, uploadData.DATA1, uploadData.DATA3, origin_voltage, uploadData.DATA4, "NG", msg, ref myLog);
            return false;
        }
        private void UploadDeviceStatusLogWrite(string resourceId,string status,string status_des,string result,string message,ref Log myLog)
        {
            myLog = PackMesLog(FName.UploadDeviceStatus, resourceId + "&" + status + "&" + status_des, result + "&" + message);
            DataManager.LogWrite(myLog);
        }
        public bool UploadDeviceStatus(string status, UploadDeviceStatusClass deviceStatus, ref string msg,ref Log myLog)
        {
            string jsonData = JsonConvert.SerializeObject(deviceStatus);
            string cz_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strRe = "";
            bool result = ocv.UploadDeviceStatusData(resouce_id, cz_date, status, jsonData, ref strRe);
            if (result)
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadDeviceStatusLogWrite(this.resouce_id, status, deviceStatus.DATA12, "OK", response.message,ref myLog);
                    return true;
                }
                msg = response.message;
                UploadDeviceStatusLogWrite(this.resouce_id, status, deviceStatus.DATA12, "NG", response.message,ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadDeviceStatusLogWrite(this.resouce_id, status, deviceStatus.DATA12, "NG", msg,ref myLog);
            return false;
        }
        private void UploadErrorLogWrite(string resourceId,string err_code,string err_desc,string result,string message,ref Log myLog)
        {
            myLog = PackMesLog(FName.UploadError, resourceId + "&" + err_code + "&" + err_desc, result + "&" + message);
            DataManager.LogWrite(myLog);
        }
        public bool UploadError(UploadErrorClass error, ref string msg,ref Log myLog)
        {
            string jsonData = JsonConvert.SerializeObject(error);
            string cz_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string strRe = "";
            if (ocv.UploadError(resouce_id, cz_date, jsonData, ref strRe))
            {
                //解析返回体
                UploadDataResponseClass response = JsonConvert.DeserializeObject<UploadDataResponseClass>(strRe);
                if (response.result == "OK")
                {
                    UploadErrorLogWrite(this.resouce_id, error.DATA11, error.DATA12, "OK", response.message,ref myLog);
                    return true;
                }
                msg = response.message;
                UploadErrorLogWrite(this.resouce_id, error.DATA11, error.DATA12, "NG", response.message,ref myLog);
                return false;
            }
            msg = "请求不成功";
            UploadErrorLogWrite(this.resouce_id, error.DATA11, error.DATA12, "NG", msg,ref myLog);
            return false;
        }
    }

    public class UploadErrorClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string DATA11 { get; set; }
        /// <summary>
        /// 表或视图不存在
        /// </summary>
        public string DATA12 { get; set; }
    }

    public class UploadDeviceStatusClass
    {
        //public string DATA01 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DATA11 { get; set; }
        /// <summary>
        /// 待料
        /// </summary>
        public string DATA12 { get; set; }
    }

    public class UploadDataResponseClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string message { get; set; }
    }
    public class UploadBatteryO2TDataClass
    {

        /// <summary>
        /// 电压
        /// </summary>
        public string DATA1 { get; set; }
        /// <summary>
        /// 电阻
        /// </summary>
        public string DATA2 { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string MATERIAL_TYPE { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        public string DATA02 { get; set; }
        ///<summary>
        ///型号
        ///</summary>
        public string DATA03 { get; set; }
        /// <summary>
        /// K值
        /// </summary>
        public string DATA3 { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string DATA4 { get; set; }
        /// <summary>
        /// O1电压
        /// </summary>
        public string DATA6 { get; set; }
        /// <summary>
        /// 温度补偿前电压
        /// </summary>
        public string DATA12 { get; set; }
        /// <summary>
        /// 温度补偿系数
        /// </summary>
        public string DATA16 { get; set; }
    }



    public class UploadBatteryDataClass
    {

        /// <summary>
        /// 电压
        /// </summary>
        public string DATA1 { get; set; }
        /// <summary>
        /// 电阻
        /// </summary>
        public string DATA2 { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string MATERIAL_TYPE { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        public string DATA02 { get; set; }
        ///<summary>
        ///型号
        ///</summary>
        public string DATA03 { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string DATA4 { get; set; }
        /// <summary>
        /// 温度补偿前电压
        /// </summary>
        public string DATA12 { get; set; }
        /// <summary>
        /// 温度补偿系数
        /// </summary>
        public string DATA16 { get; set; }
    }

    public class GetTechnologyStandardResponseClass
    {
        /// <summary>
        /// 检验项编号
        /// </summary>
        public string INSPECTION_ITEM { get; set; }
        /// <summary>
        /// 检验项名称
        /// </summary>
        public string INSPECTION_DESC { get; set; }
        /// <summary>
        /// 标准值
        /// </summary>
        public string STANDARD { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public string UPPER_LIMIT { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public string LOWER_LIMIT { get; set; }
    }

    public class GetDataResponseClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        public string shop_order { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string tech_no { get; set; }
        /// <summary>
        /// o1工序测得的电压
        /// </summary>
        //public string o1_voltage { get; set; }
        ///// <summary>
        ///// o1工序测量时间
        ///// </summary>
        //public string o1_date { get; set; }
    }
    public class GetDataOCV2ResponseClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 工单
        /// </summary>
        public string shop_order { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string tech_no { get; set; }
        /// <summary>
        /// o1工序测得的电压
        /// </summary>
        public string voltage { get; set; }
        /// <summary>
        /// o1工序测量时间
        /// </summary>
        public string date { get; set; }
    }
    public class GetResourceResponseClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 资源编号
        /// </summary>
        public string resource_id { get; set; }
    }


    public class UserLoginResponseClass
    {
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 信息描述
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string UserName { get; set; }
    }

}

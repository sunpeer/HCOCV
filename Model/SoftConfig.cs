using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Windows.Forms;


namespace Model
{
    /// <summary>
    /// 该类为一静态类，主要是做为存储在内存中的系统配置
    /// </summary>
    public class SoftConfig
    {
        public string tech_Std { get; set; }
        public int BatteryNum { get; set; }
        public int Bt3562Baudrate { get; set; }
        public string Bt3562Com { get; set; }
        public string DeviceLabels { get; set; }
        public int DeviceNumber { get; set; }
        public IEnumerable<int> DeviceValueArray { get; set; }
        public int Fx5uId { get; set; }
        public double MaxResistance { get; set; }
        public double MaxVoltage { get; set; }
        public double MinResistance { get; set; }
        public double MinVoltage { get; set; }
        public int MonitorCycle { get; set; }
        public string Sr2000wIp { get; set; }
        public string UserId { get; set; }
        //public string UserPwd { get; set; }
        public string UserName { get; set; }
        public int Sr2000wTimeOut { get; set; }
        public int StatisticNum { get; set; }
        public double ErrorRate { get; set; }
        public int LogDisplayNum { get; set; }
        public int BatteryDisplayNum { get; set; }
        public string temperatureCom { get; set; }
        public string resouce_id = "";
        public string tech_no = "";
        public double minTem;
        public double maxTem;
        public double temCoeff;
        public string shop_order = "";
        public string operation_id = "";
        public double maxK;
        public double minK;
        
        public static SoftConfig Load()
        {
            SoftConfig config = new SoftConfig();
            config.Sr2000wIp = ConfigurationManager.AppSettings["Sr2000wIp"];
            config.Bt3562Com = ConfigurationManager.AppSettings["Bt3562Com"];
            config.Bt3562Baudrate = int.Parse(ConfigurationManager.AppSettings["Bt3562Baudrate"]);
            config.Fx5uId = int.Parse(ConfigurationManager.AppSettings["Fx5uId"]);
            config.temperatureCom = ConfigurationManager.AppSettings["temperatureCom"];
            //处理双斜杆
            config.DeviceLabels = Regex.Unescape(ConfigurationManager.AppSettings["DeviceLabels"]);
            //config.DeviceLabels = "";
            //string[] arrLabels = ConfigurationManager.AppSettings["DeviceLabels"].Split('/');
            //foreach(string str in arrLabels)
            //{
            //    config.DeviceLabels += @"\"+str;
            //}
            //config.DeviceLabels.TrimStart('\\');
            config.MonitorCycle = int.Parse(ConfigurationManager.AppSettings["MonitorCycle"]);
            config.DeviceNumber = int.Parse(ConfigurationManager.AppSettings["DeviceNumber"]);
            config.DeviceValueArray = ConfigurationManager.AppSettings["DeviceValueArray"].Split(',').Select((temp => int.Parse(temp)));
            //config.UserPwd = ConfigurationManager.AppSettings["UserPwd"];
            config.UserId = ConfigurationManager.AppSettings["UserId"];
            config.UserName = ConfigurationManager.AppSettings["UserName"];
            config.BatteryNum = int.Parse(ConfigurationManager.AppSettings["BatteryNum"]);
            config.MaxResistance = double.Parse(ConfigurationManager.AppSettings["maxResistance"]);
            config.MinResistance = double.Parse(ConfigurationManager.AppSettings["minResistance"]);
            config.MaxVoltage = double.Parse(ConfigurationManager.AppSettings["maxVoltage"]);
            config.MinVoltage= double.Parse(ConfigurationManager.AppSettings["minVoltage"]);
            config.Sr2000wTimeOut = int.Parse(ConfigurationManager.AppSettings["sr2000wTimeOut"]);
            config.StatisticNum = int.Parse(ConfigurationManager.AppSettings["statisticNum"]);
            config.ErrorRate = double.Parse(ConfigurationManager.AppSettings["errorRate"]);
            config.BatteryDisplayNum = int.Parse(ConfigurationManager.AppSettings["batteryDisplayNum"]);
            config.LogDisplayNum = int.Parse(ConfigurationManager.AppSettings["logDisplayNum"]);
            config.maxTem = double.Parse(ConfigurationManager.AppSettings["maxTem"]);
            config.minTem = double.Parse(ConfigurationManager.AppSettings["minTem"]);
            config.tech_no = ConfigurationManager.AppSettings["tech_no"];
            config.operation_id = ConfigurationManager.AppSettings["operation_id"];
            config.temCoeff = double.Parse(ConfigurationManager.AppSettings["temCoeff"]);
            config.maxK = double.Parse(ConfigurationManager.AppSettings["maxK"]);
            config.minK = double.Parse(ConfigurationManager.AppSettings["minK"]);
            config.tech_Std = ConfigurationManager.AppSettings["tech_Std"];
            return config;
        }

        public static void SetAppSettingsValue(string appKey, string appValue,ref XmlNode xNode)
        {
            XmlElement xElem1 = (XmlElement)(xNode.SelectSingleNode("//add[@key='"+appKey+"']"));
            xElem1.SetAttribute("value", appValue);
        }

        public static void SaveAllConfig(SoftConfig config)
        {
            DirectoryInfo dir = new DirectoryInfo(Application.StartupPath);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(dir.FullName + @"\UIL.exe.config");
            XmlNode xNode = xDoc.SelectSingleNode("//appSettings");
            SetAppSettingsValue("Sr2000wIp", config.Sr2000wIp.ToString(), ref xNode);
            SetAppSettingsValue("Bt3562Com", config.Bt3562Com.ToString(), ref xNode);
            SetAppSettingsValue("Fx5uId", config.Fx5uId.ToString(), ref xNode);
            //SetAppSettingsValue("UserPwd", config.UserPwd.ToString(), ref xNode);
            SetAppSettingsValue("UserId", config.UserId.ToString(), ref xNode);
            SetAppSettingsValue("UserName", config.UserName, ref xNode);
            SetAppSettingsValue("BatteryNum", config.BatteryNum.ToString(), ref xNode);
            SetAppSettingsValue("maxResistance", config.MaxResistance.ToString(), ref xNode);
            SetAppSettingsValue("minResistance", config.MinResistance.ToString(), ref xNode);
            SetAppSettingsValue("maxVoltage", config.MaxVoltage.ToString(), ref xNode);
            SetAppSettingsValue("minVoltage", config.MinVoltage.ToString(), ref xNode);
            SetAppSettingsValue("sr2000wTimeOut", config.Sr2000wTimeOut.ToString(), ref xNode);
            SetAppSettingsValue("statisticNum", config.StatisticNum.ToString(), ref xNode);
            SetAppSettingsValue("errorRate", config.ErrorRate.ToString(), ref xNode);
            SetAppSettingsValue("batteryDisplayNum", config.BatteryDisplayNum.ToString(), ref xNode);
            SetAppSettingsValue("logDisplayNum", config.LogDisplayNum.ToString(), ref xNode);
            SetAppSettingsValue("temperatureCom", config.temperatureCom, ref xNode);
            SetAppSettingsValue("minTem", config.minTem.ToString(), ref xNode);
            SetAppSettingsValue("maxTem", config.maxTem.ToString(), ref xNode);
            SetAppSettingsValue("temCoeff", config.temCoeff.ToString(), ref xNode);
            SetAppSettingsValue("tech_no", config.tech_no, ref xNode);
            SetAppSettingsValue("operation_id", config.operation_id, ref xNode);
            SetAppSettingsValue("minK", config.minK.ToString(),ref xNode);
            SetAppSettingsValue("maxK", config.maxK.ToString(), ref xNode);
            SetAppSettingsValue("tech_Std", config.tech_Std, ref xNode);
            xDoc.Save(dir.FullName + @"\UIL.exe.config");
        }
    }
}

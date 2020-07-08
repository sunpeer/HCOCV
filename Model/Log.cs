using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Log
    {
        public string operTime;   //事件发生时间
        public string fName;    //接口名称
        public string process;  //工序
        public string userId;          //操作员工
        public string resourceId;       //设备资源号
        public string sn;          //如果有序列号,序列号
        public string result;       //返回结果result 如果有
        public string message;      //返回的message,如果有
        public string userName;     //员工姓名
        public string shop_order;   //工单
        public string tech_no;   //型号
        public string inspection_Item;      //检验项编号
        public string inspection_Desc;      //检验项描述
        public string standard;     //标准值
        public string upper_limit;  //最大值
        public string lower_limit; //最小值
        public string flag;     //电池判定结果
        public string ng_code; //不良代码
        public string resistance;   //电阻
        public string voltage;  //电压
        public string status;   //状态代号
        public string status_des;   //状态描述
        public string err_code;     //故障代号
        public string err_desc;     //故障描述
        public string o1_voltage;   //o1这道工序测得电压
        public string o1_date;      //o1这道工序测量的时间
        public string K;        //K值
        public string tempearture;              //温度
        public string origin_voltage;   //原始电压
        //public string logType;  //
        //public string operType;
        //public string remark; 
        public Log()
        {
            operTime = "";
            fName = "";
            process = "" ;
            userId="";
            resourceId="";
            sn="";          
            result="";
            message="";      
            userName="";     
            shop_order="";   
            tech_no="";   
            inspection_Item="";
            inspection_Desc="";
            standard="";     
            upper_limit="";  
            lower_limit=""; 
            flag="";     
            ng_code=""; 
            resistance="";   
            voltage="";  
            status="";   
            status_des="";   
            err_code="";     
            err_desc="";
            o1_voltage = "";
            o1_date = "";
            K = "";
            tempearture = "";
            origin_voltage = "";
        }
    }
}

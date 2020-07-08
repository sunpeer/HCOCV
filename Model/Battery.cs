using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    public class Battery
    {
        public string sn;
        public double voltage;
        public double resistance;
        public bool result;
        public string user;
        public BatteryErrorType errorType;
        public bool verifyFlag;
        public bool MesSaved;
        public DateTime operTime;
        public string techId;
        public string resourceId;
        public string shop_order;
        public double temperature;
        public double K;
        public double o1_voltage;
        public DateTime o1_date;
        public bool k_flag;
        public double origin_voltage;
        public string operation_id;
        public string desc;
        public double tempCoeff;
        public Battery()
        {
            sn = "";
            voltage = 0.0;
            resistance = 0.0;
            result = true;
            user = "";
            tempCoeff = 0.0;
            errorType = BatteryErrorType.OK;
            verifyFlag = true;
            MesSaved = true;
            operTime = DateTime.Now;
            techId = "";
            resourceId = "";
            shop_order = "";
            temperature = 0.0;
            K = 0.0;
            k_flag = true;
            o1_voltage = 0.0;
            o1_date=new DateTime(2000,1,1,0,0,0);
            origin_voltage = 0.0;
            operation_id = "";
            desc = "";
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VekenDLL;

namespace BLL
{
    class MESOCV
    {
        IData obj = DataAbstr.GetInstance();
        //获取设备资源编号
        //如果没有异常，则返回true,并获得返回值，如果有异常，返回false
        public bool GetResource(ref string strRe)
        {
            try
            {
                strRe = obj.GetResource();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UserLogin(string user_code, ref string strRe)
        {
            try
            {
                strRe = obj.UserLogin(user_code);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool SetupOperation(string operation_id,ref string strRe)
        {
            try
            {
                strRe = obj.SetupOperation(operation_id);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }   

        public bool GetData(string barcode, string resouce_id, ref string strRe)
        {
            try
            {
                //strRe = obj.GetData(resouce_id, barcode); //这个应该是要淘汰的
                strRe = obj.GetData(barcode);
                return true;
            }
            catch (Exception e)
            { return false; }
        }

        public bool GetTechnologyStandard(string tech_no, ref string strRe)
        {
            try
            {
                strRe = obj.GetTechnologyStandard(tech_no);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UploadBatteryData(string sfc, string resourece_id, string cz_date, string cz_user, string flag, string ng_code, string jsonData, string opeation_id, ref string strRe)
        {
            try
            {
                strRe = obj.UploadData_F(sfc, resourece_id, cz_date, cz_user, flag, ng_code, jsonData,opeation_id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UploadDeviceStatusData(string resource_id, string cz_date, string status, string jsonData, ref string strRe)
        {
            try
            {
                strRe = obj.UploadData_S(resource_id, cz_date, status, jsonData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool UploadError(string resource_id, string cz_date, string jsonData, ref string strRe)
        {
            try
            {
                strRe = obj.UploadData_W(resource_id, cz_date, jsonData);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}

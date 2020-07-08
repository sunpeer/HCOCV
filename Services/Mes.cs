#define TEST
//#define ALWAYSTRUE
#define GET
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Services
{
    public class Mes
    {
        static ASCIIEncoding asciiEncoding = new ASCIIEncoding();
        public static string Login(string id,string pwd,ref bool result)
        {
            try
            {
#if TEST
                string url = "http://192.168.2.200/weike/login/login.php";
#else
                string url = "http://192.168.0.230/j_spring_security_check";
#endif
#if ALWAYSTRUE
            string responseBody= "Success";
#endif
#if GET
                url += "?loginMode=cclient&username=" + id + "&password=" + pwd + "&siteNo=VEKEN";
                string responseBody = GetResponseByGET(url);
#elif POST

                            //制作请求
            HttpWebRequest mesRequest = (HttpWebRequest)WebRequest.Create(url);
            string requestBodystring = "loginMode=cclient&username=" + config.UserId + "&password=" + config.UserPwd + "&siteNo=VEKEN";

            string responseBody = GetResponseBodyFromRequestBody(mesRequest, requestBodystring);
#endif

                if (responseBody == "Success")
                {
                    result = true;
                    return "Success";
                }
                else
                {
                    result = false;
                    //密码错误等错误,未登录
                    return "账号密码错误或网络出错！";
                }
            }
            catch(Exception e)
            {
                result = false;
                //超时等错误,未登录
                return e.ToString();
            }
        }
        private static string GetResponseByGET(string url)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse httpWebRespones = (HttpWebResponse)httpWebRequest.GetResponse();
            Debug.WriteLine(httpWebRespones.StatusCode.ToString());
            Stream stream = httpWebRespones.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
        private static string GetResponseBodyFromRequestBody(HttpWebRequest Request, string requestBodyString)
        {
            byte[] requestBodybytes = asciiEncoding.GetBytes(requestBodyString);
            Request.Method = "POST";
            Request.ContentLength = requestBodybytes.Length;
            //using (Stream requestBodyStream = Request.GetRequestStream())
            //{
            Stream requestBodyStream = Request.GetRequestStream();
                requestBodyStream.Write(requestBodybytes, 0, requestBodybytes.Length);
                requestBodyStream.Close();
            //}
            HttpWebResponse mesResponse = (HttpWebResponse)Request.GetResponse();
            StreamReader responseBodyReader = new StreamReader(mesResponse.GetResponseStream());
            return responseBodyReader.ReadToEnd();

        }

        public static RequestCResponseRootJson SendRequest(RequestCJson requestC,ref string errMsg)//给一个发送主体给我，我返回一个回复主体
        {
#if TEST
            string url = "http://192.168.2.200/weike/send/send.php";
#else
            string url = "http://192.168.0.230/venken/http/HttpSavePubAction!doQueryPub.action";
#endif
            try
            {
#if POST
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            string requestBodyString = "dataJson=" + JsonConvert.SerializeObject(requestC);//序列化对象
            //正常通信，无错误
            string responseBodyString = GetResponseBodyFromRequestBody(request, requestBodyString);//获取返回主体
            result = "Success";
            return JsonConvert.DeserializeObject<RequestCResponseRootJson>(responseBodyString);//反序列化json
#else
                string query = "?dataJson=" + JsonConvert.SerializeObject(requestC);
                url += query;
                string responseStr = GetResponseByGET(url);
                RequestCResponseRootJson rcobject = JsonConvert.DeserializeObject<RequestCResponseRootJson>(responseStr);
                errMsg = "Success";
                return rcobject;
#endif
            }
            catch (Exception e)
            {
                errMsg = "通信错误";
                RequestCResponseRootJson rc = null;
                return rc;
            }
        }

        public static RequestAResponseRootJson SendRequest(RequestAJson requestA,ref string errMsg)//给一个发送类给我，我返回一个回复类主体
        {
#if TEST
            string url = "http://192.168.2.200/weike/requesttechstd/querytechstd.php";
#else
            string url="http://192.168.0.230/venken/http/HttpSavePubAction!doQueryPub.action";
#endif
            try
            {
#if POST
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                string requestBodyString = "dataJson=" + JsonConvert.SerializeObject(requestA);//序列化对象
                string responseBodyString = GetResponseBodyFromRequestBody(request, requestBodyString);
                errMsg="Success"
                return JsonConvert.DeserializeObject<RequestAResponseRootJson>(responseBodyString);
#else
                string query = "?dataJson=" + JsonConvert.SerializeObject(requestA);
                url += query;
                string ra = GetResponseByGET(url);
                RequestAResponseRootJson raobject = JsonConvert.DeserializeObject<RequestAResponseRootJson>(ra);
                errMsg = "Success";
                return raobject;
#endif
            }
            catch (Exception e)
            {
                errMsg = "请求错误";
                return null;
            }
        }
        public static RequestBResponseRootJson SendRequest(RequestBJson requestB,ref string result)//给一个发送体给我，我返回一个回复主体
        {
#if TEST
            string url = "http://192.168.2.200/weike/verify/verify.php";
#else
            string url = "http://192.168.0.230/venken/http/HttpSavePubAction!doQueryPub.action";
#endif
#if AlWAYSTRUE
            //制作一个RsponseB
            RequestBResponseRootJson rb = new RequestBResponseRootJson();
            rb.result.results = "true";
            result="Success";
            return rb;
#else
            try
            {
#if POST
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                string requestBodyString = "datajson=" + JsonConvert.SerializeObject(requestB);//序列化对象
                string responseBodyString = GetResponseBodyFromRequestBody(request, requestBodyString);//获取返回主体
                RequestBResponseRootJson rbobject=JsonConvert.DeserializeObject<RequestBResponseRootJson>(responseBodyString);//反序列化json
                result="Success";
                return rbobject;
#elif GET
                string query = "?dataJson=" + JsonConvert.SerializeObject(requestB);
                url += query;
                string rb = GetResponseByGET(url);
                RequestBResponseRootJson rbobject= JsonConvert.DeserializeObject<RequestBResponseRootJson>(rb);//反序列化json
                result = "Success";
                return rbobject;
#endif
        }
            catch(Exception e)
            {
                e.ToString();
                result = "网络连接错误";
                RequestBResponseRootJson rb = null;
                return rb;
            }

#endif
}
    }

    public class RequestAJson
    {
        /// <summary>
        /// H90309XAQ031039
        /// </summary>
        public string sfc { get; set; }
        /// <summary>
        /// true
        /// </summary>
        public string isfirst { get; set; }
        /// <summary>
        /// IIN
        /// </summary>
        public string operationNo { get; set; }
        /// <summary>
        /// 2
        /// </summary>
        public string interCallId { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string TECHID { get; set; }
    }



    public class RequestAResponseResultJson
    {
        /// <summary>
        /// SHOPORDER:2
        /// </summary>
        public string STATUS_ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dz_lowerLimit { get; set; }
        /// <summary>
        /// 9865B0PL-8480MAH-XA
        /// </summary>
        public string techNo { get; set; }
        /// <summary>
        /// 65943645a6e94d5db2c856cef16221e7
        /// </summary>
        public string ITEM_ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dz_upperLimit { get; set; }
        /// <summary>
        /// SFC_STATUS:DONE
        /// </summary>
        public string STATUTS_ID { get; set; }
        /// <summary>
        /// RESULT
        /// </summary>
        public int RESULT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dy_lowerLimit { get; set; }
        /// <summary>
        /// H90309XAQ031039
        /// </summary>
        public string SFC_NO { get; set; }
        /// <summary>
        /// 电芯\9865B0PL\8480\包装前电芯
        /// </summary>
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// QTY
        /// </summary>
        public int QTY { get; set; }
        /// <summary>
        /// 1040100842
        /// </summary>
        public string materielNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dy_upperLimit { get; set; }
        /// <summary>
        /// 1040100842
        /// </summary>
        public string ITEM_NO { get; set; }
        /// <summary>
        /// H190309XA-Q
        /// </summary>
        public string SHOP_ORDER_NO { get; set; }
        /// <summary>
        /// A
        /// </summary>
        public string materielVersion { get; set; }
    }

    public class RequestAResponseRootJson
    {
        /// <summary>
        /// Status
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// Result
        /// </summary>
        public RequestAResponseResultJson result { get; set; }
        /// <summary>
        /// ErrMessage
        /// </summary>
        public List<ErrMessage> errMessage { get; set; }
    }
    public class ErrMessage
    {
        /// <summary>
        /// 参数物料编号:物料编号[WK00000041]物料版本[A]不存在！
        /// </summary>
        public string message { get; set; }
    }



    public class RequestBJson
    {
        /// <summary>
        /// H61027KEQ001627
        /// </summary>
        public string sfc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isfirst { get; set; }
        /// <summary>
        /// CTT
        /// </summary>
        public string operationNo { get; set; }
        /// <summary>
        /// 6
        /// </summary>
        public string interCallId { get; set; }
        /// <summary>
        /// -1
        /// </summary>
        public string TECHID { get; set; }
        /// <summary>
        /// 2019-04-19 13:41:00
        /// </summary>
        public DateTime? uploadTime { get; set; }
    }


    public class RequestBResponseResultJson
    {
        /// <summary>
        /// SHOPORDER:2
        /// </summary>
        public string STATUS_ID { get; set; }
        /// <summary>
        /// 3357121PH-3570MAH-KE
        /// </summary>
        public string techNo { get; set; }
        /// <summary>
        /// c1548fcf871c48208a87e65b341b017e
        /// </summary>
        public string ITEM_ID { get; set; }
        /// <summary>
        /// SFC_STATUS:DONE
        /// </summary>
        public string STATUTS_ID { get; set; }
        /// <summary>
        /// RESULT
        /// </summary>
        public int RESULT { get; set; }
        /// <summary>
        /// false
        /// </summary>
        public string results { get; set; }
        /// <summary>
        /// 电压不符合
        /// </summary>
        public string errMessage { get; set; }
        /// <summary>
        /// H61027KEQ001627
        /// </summary>
        public string SFC_NO { get; set; }
        /// <summary>
        /// 3357121PH-3570
        /// </summary>
        public string ITEM_NAME { get; set; }
        /// <summary>
        /// QTY
        /// </summary>
        public int QTY { get; set; }
        /// <summary>
        /// 0300200000942
        /// </summary>
        public string materielNo { get; set; }
        /// <summary>
        /// 0300200000942
        /// </summary>
        public string ITEM_NO { get; set; }
        /// <summary>
        /// H161027KE-Q
        /// </summary>
        public string SHOP_ORDER_NO { get; set; }
        /// <summary>
        /// A
        /// </summary>
        public string materielVersion { get; set; }
    }


    public class RequestBResponseRootJson
    {
        /// <summary>
        /// Status
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// Result
        /// </summary>
        public RequestBResponseResultJson result { get; set; }
        /// <summary>
        /// ErrMessage
        /// </summary>
        public List<ErrMessage> errMessage { get; set; }
    }
    public class RequestCJson
    {
        /// <summary>
        /// 2346577878
        /// </summary>
        public string SFC { get; set; }
        /// <summary>
        /// JLJBG02
        /// </summary>
        public string RESOURCE_ID { get; set; }
        /// <summary>
        /// JLJB002
        /// </summary>
        public string OPEATION_ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DATA1 { get; set; }
        /// <summary>
        /// 123
        /// </summary>
        public string DATA2 { get; set; }
        /// <summary>
        /// 2
        /// </summary>
        public string DATA3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DATA4 { get; set; }
        /// <summary>
        /// 1
        /// </summary>
        public string DATA5 { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string DATA6 { get; set; }
        /// <summary>
        /// 20190817 13:59:00
        /// </summary>
        public string CZ_DATE { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string FLAG { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string DEMO { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string BZ { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string MATERIAL_TYPE { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string NG_CODE { get; set; }
        /// <summary>
        /// 1
        /// </summary>
        public string IS_CURRENT { get; set; }
        /// <summary>
        /// 0
        /// </summary> 
        public string IS_GAIN { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string DATA7 { get; set; }
        /// <summary>
        /// 4
        /// </summary>
        public string DATA8 { get; set; }
        /// <summary>
        /// 2
        /// </summary>
        public string tableCallId { get; set; }
    }

    public class RequestCResponseRootJson
    {
        /// <summary>
        /// Status
        /// </summary>
        public bool status { get; set; }
        /// <summary>
        /// ErrMessage
        /// </summary>
        public List<ErrMessage> errMessage { get; set; }
        /// <summary>
        /// Result
        /// </summary>
        public RequestCResponseResultJson result { get; set; }
    }
    public class RequestCResponseResultJson
    {

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Model;

namespace BLL
{
    /// <summary>
    /// 这个类用来给mES系统上传状态
    /// </summary>
    public class UploadStatusThread
    {
        MesConnect mes;
        public UploadStatusThread(MesConnect mes)
        {
            this.mes = mes;
            //实例化这个类的时候，设定定时器
            time = new System.Timers.Timer(300000);//5分钟的计时器
            time.Elapsed += UploadDeviceStatusCallback;
        }
        DeviceStatus deviceStatus;
        UploadDeviceStatusClass deviceStatusClass;
        System.Timers.Timer time;

        public Action<Log> LogShowEventHandler;

        //创建新线程5分钟传一次，指定的状态
        public void UploadStatus(DeviceStatus status)
        {
            if (status == deviceStatus)//如果状态相等，那就别动呗
                return;
            //if(status==deviceStatus)
            //{
            //    if (status != DeviceStatus.A)
            //        return;
            //    else
            //    {
            //        //要给计时器清零,从新执行
            //        alarmClock.Stop();
            //        alarmClock.Start();
            //    }
            //}
            //if(status==DeviceStatus.A)  //如果状态是A,则要开启偷懒定时器
            //{
            //    alarmClock = new System.Timers.Timer(20000);
            //    alarmClock.Elapsed += ChangeToLazy;
            //    alarmClock.AutoReset = false;
            //    alarmClock.Start();
            //}
            //设置状态呗
            deviceStatus = status;
            deviceStatusClass = new UploadDeviceStatusClass();
            //deviceStatusClass.DATA01 = deviceStatus.ToString();
            deviceStatusClass.DATA11 = deviceStatus.ToString();
            string statusDes = "";
            switch (status.ToString())
            {
                case "A":
                    statusDes = "运行";
                    break;
                case "B":
                    statusDes = "暂停";
                    break;
                case "C":
                    statusDes = "故障";
                    break;
                case "D":
                    statusDes = "不运行";
                    break;
            }
            deviceStatusClass.DATA12 = statusDes;
            Upload();
            time.Stop();
            time.Start();
        }
        //这个函数是给5分钟发送消息的计时器回调用的
        private void UploadDeviceStatusCallback(Object source, System.Timers.ElapsedEventArgs e)
        {
            Upload();
        }
        private void Upload()
        {
            //发送状态呗
            string msg = "";
            Log myLog = new Log();
            bool result = mes.UploadDeviceStatus(deviceStatus.ToString(), deviceStatusClass, ref msg,ref myLog);
            LogShowEventHandler?.BeginInvoke(myLog,null,null);
        }
        public void UploadError(string error_code, string error_desc,ref Log myLog)
        {
            string msg = "";
            UploadErrorClass error = new UploadErrorClass();
            error.DATA11 = error_code;
            error.DATA12 = error_desc;
            bool result = mes.UploadError(error, ref msg,ref myLog);
        }
    }
    public enum DeviceStatus
    {
        A,
        B,
        C,
        D
    }
}

//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Threading;
using System.IO.Ports;

namespace Services
{
    public class Bt3562:Device
    {

        //创建接受信息后调用的委托
        public Action<string> DataReceivedEventHandler;
        private SerialPort bt3562;
        
        public Bt3562()
        {
            bt3562 = new SerialPort();
            bt3562.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
            bt3562.ReceivedBytesThreshold = 1;
            bt3562.NewLine = "\r\n";
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //等待本条信息全部接受完毕
            Thread.Sleep(100);
            //读取信息
            string buffer = bt3562.ReadLine();
            //调用委托
            DataReceivedEventHandler?.BeginInvoke(buffer, null, null);
            //取消委托的所有挂载
            DataReceivedEventHandler = null;
        }
        /// <summary>
        /// 向BT3562发送指令，返回指令响应结果
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public override string SendCmd(string str)
        {
            try
            {
                bt3562.WriteLine(str);
                return "Send successful!";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }

        public override bool Connect(string conParam,SoftConfig config, ref string errMsg)
        {
            //设置连接参数
            bt3562.PortName = conParam;
            bt3562.BaudRate = 9600;
            try
            {
                bt3562.Open();
                //设置正确的工作模式
                Thread.Sleep(50);
                bt3562.WriteLine(":INIT:CONT OFF");//连续测量打开
                Thread.Sleep(50);
                bt3562.WriteLine(":TRIG:SOUR IMM");//触发源设定为外部
                IsConnected = true;
                config.Bt3562Com = conParam;
                return true;
            }
            catch(Exception e)
            {
                IsConnected = false;
                errMsg = e.ToString();
                return false;
            }
        }

        public bool ConnectBt3562(string conParam,ref string msg)
        {
            bt3562.Close();
            //设置连接参数
            bt3562.PortName = conParam;
            bt3562.BaudRate = 9600;
            try
            {
                bt3562.Open();
                //设置正确的工作模式
                Thread.Sleep(50);
                bt3562.WriteLine(":INIT:CONT ON"); //连续测量打开
                //bt3562.WriteLine(":INIT:CONT OFF");
                Thread.Sleep(50);
                bt3562.WriteLine(":TRIG:SOUR IMM");//触发源设定为内部

                //Thread.Sleep(50);
                //bt3562.WriteLine(":AUT ON");
                Thread.Sleep(50);
                bt3562.WriteLine(":SAMP:RATE FAST");    //让电池检测仪在500毫秒的时间内，检测多次
                //Thread.Sleep(50);
                //bt3562.WriteLine(":TRIG:DEL 0.1");
                //Thread.Sleep(50);
                //bt3562.WriteLine(":TRIG:DEL:STAT ON");
                return true;
            }
            catch (Exception e)
            {
                IsConnected = false;
                msg = e.ToString();
                return false;
            }
        }
    


        public override void Connect(SoftConfig config,Action<bool> CallBack)
        {
#if TEST
            //模仿真实连接
            Random rd = new Random();
            Thread.Sleep(rd.Next(10, 100));
            //构建回调参数
            bool obj = true;
            //异步回调
            CallBack.BeginInvoke(obj, null, null);
#endif
            //设置连接参数
            bt3562.PortName = config.Bt3562Com;
            bt3562.BaudRate = config.Bt3562Baudrate;
            try
            {
                bt3562.Open();
                //设置正确的工作模式
                Thread.Sleep(50);
                bt3562.WriteLine(":INIT:CONT OFF");
                Thread.Sleep(50);
                bt3562.WriteLine(":TRIG:SOUR IMM");
                IsConnected = true;
                CallBack?.BeginInvoke(true, null, null);
            }
            catch
            {
                IsConnected = false;
                CallBack?.BeginInvoke(false, null, null);
            }
        }
    }
}

//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Threading;


namespace Services
{
    public class Fx5u : Device
    {
        public Action<PlcLabel> EventMeditator;
        private ActUtlTypeLib.ActUtlTypeClass fx5u;
        public Fx5u()
        {
            //创建对象
            fx5u = new ActUtlTypeLib.ActUtlTypeClass();
            //订阅事件
            fx5u.OnDeviceStatus +=
    new ActUtlTypeLib._IActUtlTypeEvents_OnDeviceStatusEventHandler(OnDeviceStatus);
        }

        private void OnDeviceStatus(string szDevice, int lData, int lReturnCode)
        {
            PlcLabel fx5uEvent = (PlcLabel)Enum.Parse(typeof(PlcLabel), szDevice);
            //异步调用委托
            EventMeditator?.BeginInvoke(fx5uEvent, null, null);

        }


        public void ReadDeviceBlock(string startDevice, int iSize, ref short[] sData)
        {
            fx5u.ReadDeviceBlock2(startDevice, iSize, out sData[0]);
        }
        public void WriteDeviceBlock(string startDevice, int iSize, ref short[] sData)
        {
            fx5u.WriteDeviceBlock2(startDevice, iSize, ref sData[0]);
        }

        public override void Connect(SoftConfig config, Action<bool> CallBack)
        {
#if TEST
            //模仿真实连接
            Random rd = new Random();
            Thread.Sleep(rd.Next(10, 100));
            //构建回调参数
            bool obj = true;
            //异步回调
            CallBack?.BeginInvoke(obj, null, null);
#endif
            //设置连接参数
            fx5u.ActLogicalStationNumber = config.Fx5uId;
            //开始连接
            int iReturnCode = fx5u.Open();
            if (iReturnCode == 0)
            {
                //注册监控函数
                iReturnCode = EntryDeviceStatus(config);
                if (iReturnCode == 0)
                {
                    //连接成功,回调
                    IsConnected = true;
                    CallBack?.BeginInvoke(true, null, null);
                    return;
                }
            }
            IsConnected = false;
            CallBack?.BeginInvoke(false, null, null);
            return;
        }
        /// <summary>
        /// 向FX5U发送指令，返回指令响应结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string SendCmd(string cmd)
        {
            int iReturnCode = fx5u.SetDevice(cmd, 1);
            return iReturnCode.ToString();
        }
        public string ResetRegister(string cmd, int i)
        {
            int iReturnCode = fx5u.SetDevice(cmd, i);
            return iReturnCode.ToString();
        }
        public int EntryDeviceStatus(SoftConfig config)
        {
            //构造监控配置
            string szLabel = config.DeviceLabels;
            int iNumberOfData = config.DeviceNumber;
            int iConMonitorCycle = config.MonitorCycle;
            int[] arrDeviceValue = config.DeviceValueArray.ToArray();
            int ireturnCode = fx5u.EntryDeviceStatus(szLabel, iNumberOfData, iConMonitorCycle, ref arrDeviceValue[0]);
            return ireturnCode;
        }

        public int EnteryDeviceStatus(string szDeviceList, int lSize, int lMonitorCycle, ref int lplData)
        {
            return fx5u.EntryDeviceStatus(szDeviceList, lSize, lMonitorCycle, lMonitorCycle);
        }

        public int FreeDeviceStatus()
        {
            return fx5u.FreeDeviceStatus();
        }
        public override bool Connect(string conParam,SoftConfig config, ref string errMsg)
        {
            //设置连接参数
            fx5u.ActLogicalStationNumber = int.Parse(conParam);
            //开始连接
            int iReturnCode = fx5u.Open();
            if (iReturnCode == 0)
            {
                //注册监控函数
                iReturnCode = EntryDeviceStatus(config);
                if (iReturnCode == 0)
                {
                    //连接成功,回调
                    IsConnected = true;
                    config.Fx5uId = int.Parse(conParam);
                    return true;
                }
            }
            IsConnected = false;
            errMsg = "错误代码：" + iReturnCode.ToString();
            return false;
        }
    }
}

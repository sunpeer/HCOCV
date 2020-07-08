//#define TEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Model;
using Keyence.AutoID.SDK;


namespace Services
{
    public class Sr2000w:Device
    {
        public Action<string> DataReceivedEventHandler;
        private ReaderAccessor sr2000w = null;

        public string ReceivedDate { get; private set; }   

        public Sr2000w()
        {
            //创建连接对象
            sr2000w = new ReaderAccessor();
        }

        public override void Connect(SoftConfig config,Action<bool> CallBack)
        {
#if TEST
            //模仿真实连接
            Random rd = new Random();
            Thread.Sleep(rd.Next(10,100));
            //构建回调参数
            object obj = "Sr2000w连接成功";
            //异步回调
            CallBack.BeginInvoke(obj, null, null);
#endif
            sr2000w.IpAddress = config.Sr2000wIp;
            //连接，创建参数
            bool obj = sr2000w.Connect(new Action<byte[]>(
                (byte[] data) =>
                {
                    //得到返回字符串
                    ReceivedDate=Encoding.ASCII.GetString(data);
                    //异步回调委托
                    DataReceivedEventHandler?.BeginInvoke(ReceivedDate,null,null);
                    //取消所有挂载的委托
                    DataReceivedEventHandler = null;
                }));
            //状态修改
            IsConnected = obj;
            //异步回调
            CallBack?.BeginInvoke(obj, null, null);
        }
        /// <summary>
        /// 向SR2000W发送指令，返回指令响应结果
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override string SendCmd(string cmd)
        {
            string cmdResult=sr2000w.ExecCommand(cmd);
            return cmdResult;
        }

        public override bool Connect(string conParam, SoftConfig config, ref string errMsg)
        {
            sr2000w.IpAddress = conParam;
            //连接，创建参数
            bool obj = sr2000w.Connect(new Action<byte[]>(
                (byte[] data) =>
                {
                    //得到返回字符串
                    ReceivedDate = Encoding.ASCII.GetString(data);
                    //异步回调委托
                    DataReceivedEventHandler?.BeginInvoke(ReceivedDate, null, null);
                    //取消所有挂载的委托
                    DataReceivedEventHandler = null;
                }));
            //状态修改
            IsConnected = obj;
            //制作错误信息
            if(!obj)
            {
                errMsg = "请检查扫码枪连接线路和Ip地址是否正确";
            }
            else
            {
                config.Sr2000wIp = conParam;
            }
            return obj;
        }
    }
}

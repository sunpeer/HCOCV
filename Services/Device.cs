using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Services
{
    public abstract class Device
    {
        public Device()
        {
            IsConnected = false;
        }
        public bool IsConnected{ get; set; }

        public abstract void Connect(SoftConfig config,Action<bool> CallBack);
        public abstract bool Connect(string conParam,SoftConfig config,ref string errMsg);
        public abstract string SendCmd(string cmd);
    }
}

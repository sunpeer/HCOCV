using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Model
{
    public class ConnectDownEventArgs:EventArgs
    {
        //连接结果
        public object ConResult { get; set; }
        //连接设备
        public object ConType { set; get; }
        //连接完成时间
        public object DownTime { get; set; }
    }
}

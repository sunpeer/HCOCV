using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using Model;

namespace BLL
{
    //此类用来控制平台上的LED灯的闪烁
    public class DownLedFlash
    {
        Fx5u plc;
        System.Timers.Timer ledFlashTimer;
        bool flag;
        List<int> flashLed = null;
        public List<int> getFlshAddr()
        {
            return this.flashLed;
        }
        public DownLedFlash(Fx5u plc)
        {
            this.plc = plc;
        }
        public void SetFlash(List<int> values)
        {
            ResetFlash();
            flag = false;
            flashLed = new List<int>(values);
            //开始计时吧
            ledFlashTimer = new System.Timers.Timer(200);//周期为一秒闪烁
            ledFlashTimer.Elapsed += LedFlashProc;
            ledFlashTimer.Start();
        }
        public void ResetFlash()
        {
            ledFlashTimer?.Stop();
            ledFlashTimer?.Dispose();
            ledFlashTimer = null;
            flashLed = null;
        }
        private void LedFlashProc(Object source, System.Timers.ElapsedEventArgs e)
        {
            int value = flag ? 1 : 0;
            try
            {
                foreach (int i in flashLed)
                {
                    PlcLabel labelTemp = (PlcLabel)i;
                    this.plc.ResetRegister(labelTemp.ToString(), value);
                }
            }
            catch
            {

            }
            flag = !flag;
        }
    }
}


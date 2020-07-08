using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class StatiscticsMonitor: INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void EventTriger([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StatiscticsMonitor()
            {
                inBatteryNum = 0;
                testBatteryNum = 0;
                okBatteryNum = 0;
                savedBatteryNum = 0;
            }

            int inBatteryNum;
            int testBatteryNum;
            int okBatteryNum;
            int savedBatteryNum;

            public int InBatteryNum
            {
                get
                {
                    return inBatteryNum;
                }

                set
                {
                    if(inBatteryNum!=value)
                    {
                        inBatteryNum = value;
                        EventTriger();
                    }

                }
            }

            public int TestBatteryNum
            {
                get
                {
                    return testBatteryNum;
                }

                set
                {
                    testBatteryNum = value;
                    EventTriger();
                }
            }

            public int OkBatteryNum
            {
                get
                {
                    return okBatteryNum;
                }

                set
                {
                    okBatteryNum = value;
                    EventTriger();
                }
            }

            public int SavedBatteryNum
            {
                get
                {
                    return savedBatteryNum;
                }

                set
                {
                    savedBatteryNum = value;
                    EventTriger();
                }
            }
        }
}

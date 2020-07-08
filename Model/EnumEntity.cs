using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum PlcLabel
    {
        D1,     //UpBatteryToPosition,
        D101,   //UpBatteryOnPosition,
        D2,     //ReaderTrigger,
        D102,   //ReaderEndTrigger,
        D3,     //BaseUpToPosition1,
        D103,   //BaseUpOnPosition1,
        D4,     //ProbeDownToPosition1,
        D104,   //ProbeDownOnPosition1,
        D5,     //Channel1Change,
        D105,   //Channel1EndChange,
        D6,     //Channel2Change,
        D106,   //Channel2EndChange,
        D7,     //Channel3Change,
        D107,   //Channel3EndChange,
        D8,     //Channel4Change,
        D108,   //Channel4EndChange,
        D9,     //ProbeResetFromPosition1Start,
        D109,   //ProbeResetFromPosition1Down,
        D10,    //BaseResetFromPosition1Start,
        D110,    //BaseResetFromPosition1Down,
        D11,    //UpBatteryResetStart,
        D111,    //UpBatteryResetDown,
        
        D12,    //DownBatteryToPosition,
        D112,   //DownBatteryOnPosition,

        D16,    //
        D116,
        D17,
        D117,
        D18,
        D118,
        D19,
        D119,



        D14,    //BaseUpToPosition2,
        D114,   //BaseUpOnPosition2,
        D15,    //ProbeDownToPosition2,
        D115,   //ProbeDownOnPosition2,
        D20,    //ProbeResetFromPosition2Start,
        D120,   //ProbeResetFromPosition2Down,
        D21,    //BaseResetFromPosition2Start,
        D121,   //BaseResetFromPosition2Down,
        D22,    //DownBatteryResetStart,
        D122,   //DownBatteryResetDown,


        D123,   //上层读码
        D124,   //下层读码
        D125,   //上层电池1
        D126,   //上层电池2
        D127,   //上3
        D128,   //上4
        D129,   //下1
        D130,   //下2
        D131,   //下3
        D132,   //下4


        D135,   //获取首件工艺标准的时候，上层电池退回标志位


        D144,
        D149,    //下位机出故障了
        D155,
        D170,
        D171,
        D172
    }

    public enum BatteryErrorType
    {
        OK = 0,
        OverMaxVoltage = 8,
        BelowMinVoltage = 4,
        OverMaxResistance = 2,
        BelowMinResistance = 1,
        OverMaxVoltageOverMaxResistance = 10,
        OverMaxVoltageBelowMinResistance = 9,
        BelowMinVoltageOverMaxResistance = 6,
        BelowMinVoltageBelowMinResistance = 5
    }

    public enum PlcState
    {
        RUN,
        STOP
    }
}

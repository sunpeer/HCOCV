using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Modbus.Device;

namespace BLL
{
    class Temperature
    {
        IModbusMaster master;
        SerialPort myPort;
        ushort[] registerBuffer;
        //连接，设置属性，成功true，失败false
        public bool SerialModBus(string Com)
        {
            try
            {
                myPort = new SerialPort(Com, 9600, Parity.None, 8, StopBits.One);
                master = ModbusSerialMaster.CreateRtu(myPort);
                myPort.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ReadTemperature(ref double temperature)
        {
            try
            {
                byte slaveAddress = byte.Parse("1");
                ushort startAddress = ushort.Parse("0");
                ushort numberofPoint = ushort.Parse("1");
                registerBuffer = master.ReadInputRegisters(slaveAddress, startAddress, numberofPoint);
                temperature = ((double)registerBuffer[0])/10;
                return true;
            }
            catch
            {
                return false;
            }
        }
        //private double ParseResponse()
        //{
        //    ushort Htemperature = registerBuffer[3];
        //    ushort Ltemperature = registerBuffer[4];
        //    ushort rate = 0xff;
        //    int iTemperature = Htemperature * rate + Ltemperature;
        //    double temperature = (double)iTemperature;
        //    temperature = temperature / 10;
        //    return temperature;
        //}
    }
}

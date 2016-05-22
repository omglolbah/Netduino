using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace NetduinoPlusApplication1
{
    class ShiftRegister
    {
        //74HC595
        public OutputPort SerialDataInput;
        public OutputPort ShiftRegisterClock;
        public OutputPort StorageRegisterClock;
        public ShiftRegister(Cpu.Pin sdinput, Cpu.Pin shift_clock, Cpu.Pin storage_clock)
        {
            SerialDataInput = new OutputPort(sdinput, false);
            ShiftRegisterClock = new OutputPort(shift_clock, false);
            StorageRegisterClock = new OutputPort(storage_clock,false);
        }
        public void WriteMap(int x, int y)
        {
            WriteByte(y);
            WriteByte(x);
        }
        public void WriteBit(Boolean b)
        {
            SerialDataInput.Write(b);
            ShiftRegisterClock.Write(true);
            ShiftRegisterClock.Write(false);
        }
        public void ClockStorage()
        {
            StorageRegisterClock.Write(true);
            StorageRegisterClock.Write(false);
        }
        public void WriteByte(int x, Boolean Invert)
        {
            if (Invert)
            {
                x = x ^ 0xff; // invert! :D
            }
            for (int i = 128; i >= 1; i = i / 2)
            {
                //Debug.Print("i=" + i);

                if ((i & x) > 0)
                {
                    SerialDataInput.Write(true);
                }
                else
                {
                    SerialDataInput.Write(false);
                }
                ShiftRegisterClock.Write(true);
                ShiftRegisterClock.Write(false);
            }
        }
        public void WriteByte(int x)
        {
            WriteByte(x, false);
        }
    }
}

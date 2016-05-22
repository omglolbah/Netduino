using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using NetduinoPlusTesting;

namespace NetduinoPlusApplication1
{
    public class Program
    {
        

        // Example I2C sensor.
        private static BlinkM _BlinkM;
        private static DS1621 _ds1621;

        static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        static PWM pwmout = new PWM(SecretLabs.NETMF.Hardware.NetduinoPlus.Pins.GPIO_PIN_D5);

        public static void PulseLed()
        {
            for (int i = 0; i < 10; i++)
            {
                led.Write(true);
                Thread.Sleep(15);
                led.Write(false);
                Thread.Sleep(35);
            }
        }
        public static void Main()
        {
            // write your code here
            InterruptPort button = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);

            PulseLed();

            // Create a new I2C bus instance at startup.
            I2CBus i2cBus = I2CBus.GetInstance();

            _BlinkM = new BlinkM(0x09);
            _ds1621 = new DS1621(0x48);
            // write something to a register.
            //_exampleSensor.WriteSomethingToSpecificRegister();
            ShiftRegister _74HC595 = new ShiftRegister(Pins.GPIO_PIN_D2, Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1);
            //_74HC595.WriteMap(1, 255);
            _74HC595.WriteByte(0);
            _74HC595.WriteByte(0);
            _74HC595.ClockStorage();
            _74HC595.WriteByte(255);
            _74HC595.WriteByte(1);
            _74HC595.ClockStorage();
            while (true)
            {
                for (int j = 1; j <= 128; j = j * 2)
                {
                    for (int i = 1; i <= 128; i = i * 2)
                    {
                        //Debug.Print("i[" + i + "]  j[" + j);
                        _74HC595.WriteByte(0,false);
                        _74HC595.WriteByte(0,true);

                        _74HC595.WriteByte(i, false);
                        _74HC595.WriteByte(j, true);
                        _74HC595.ClockStorage();
                        Thread.Sleep(100);
                    }
                }
                /*
                for (int j = 1; j <= 128; j = j * 2)
                {
                    for (int i = 1; i <= 128; i = i * 2)
                    {
                        Debug.Print("i[" + i + "]  j[" + j);
                        _74HC595.WriteByte(i);
                        _74HC595.WriteByte(j);
                        _74HC595.ClockStorage();
                        Thread.Sleep(200);
                    }
                }
                 * */
            }
            //Bresenhams.Algorithms.Line(0,0,4,7,SetPixel);

            //Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableStaticIP("10.0.0.222", "255.255.255.0", "10.0.0.4");
            //Debug.Print("IP=" + Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            Thread.Sleep(Timeout.Infinite);
        }
        

        static void button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            led.Write(data2 == 0);
            if (data2 == 0)
            {
                byte[] data = _BlinkM.ReadSomething(0x67, 3);
                Debug.Print("data=" + data[0] + "-" + data[1] + "-" + data[2]);
                double cheese = _ds1621.ReadTempFromDevice();
                Debug.Print("temp=" + cheese);
                double d = _ds1621.ReadUberTemp();
                Debug.Print("ubertemp=" + d);
                ".".ToString();
                pwmout.SetDutyCycle(128);
            }
            else
            {
                pwmout.SetDutyCycle(0);
            }
            
            
        }

    }
}

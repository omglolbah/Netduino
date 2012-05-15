using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using NetduinoPlusTesting;

namespace NetduinoPlusApplication1
{
    /// <summary>
    /// This is an I2C sensor.
    /// </summary>
    public class BlinkM
    {
        private I2CDevice.Configuration _slaveConfig;
        private const int TransactionTimeout = 1000; // ms
        private const byte ClockRateKHz = 100;
        public byte Address { get; private set; }

        /// <summary>
        /// Example sensor constructor
        /// </summary>
        /// <param name="address">I2C device address of the example sensor</param>
        public BlinkM(byte address)
        {
            Address = address;
            _slaveConfig = new I2CDevice.Configuration(address, ClockRateKHz);
        }



        public byte[] ReadSomething(byte cmd, byte bytecount)
        {
            // write register address
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { cmd }, TransactionTimeout);

            // get MSB and LSB result
            byte[] data = new byte[bytecount];
            I2CBus.GetInstance().Read(_slaveConfig, data, TransactionTimeout);

            return data;
        }

        public byte[] ReadSomethingFromSpecificRegister()
        {
            // get MSB and LSB result
            byte[] data = new byte[2];
            I2CBus.GetInstance().ReadRegister(_slaveConfig, 0xF1, data, TransactionTimeout);

            return data;
        }

        public void WriteSomethingToSpecificRegister()
        {
            I2CBus.GetInstance().WriteRegister(_slaveConfig, 0x3C, new byte[2] { 0xF4, 0x2E }, TransactionTimeout);
        }


    }
}

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Extender.TransferAdapters;


namespace NetduinoRGBController
{
    /// <summary>
    /// This is an I2C sensor.
    /// </summary>
    public class BlinkM
    {
        
        private const byte I2CClockRateKHz = 100;
        private const int I2CTimeout = 1000;

        public byte Address { get; private set; }
        
        private I2CDevice.Configuration _i2C;
        private I2CAdapter _i2Cadapter = I2CAdapter.Instance;

        /// <summary>
        /// Example sensor constructor
        /// </summary>
        /// <param name="address">I2C device address of the example sensor</param>
        public BlinkM(byte I2CBuschipAddress)
        {
            _i2C = new I2CDevice.Configuration(I2CBuschipAddress, I2CClockRateKHz);
        }
        public void StopScript()
        {
            var data = new[] { (Byte)'o'};
            _i2Cadapter.WriteBytes(_i2C, data);
        }
    
        public void WriteColor(Byte red, Byte green, Byte blue)
        {
            // Create a new transaction to write a register value
            var data = new[] { (Byte)'n',red,green,blue };
            _i2Cadapter.WriteBytes(_i2C, data);
        }

        public byte[] ReadSomething(byte cmd, byte bytecount)
        {
            _i2Cadapter.WriteBytes(_i2C, new byte[] { cmd });
            
            var data = new Byte[bytecount];
            _i2Cadapter.ReadBytes(_i2C, data);
                       
                /*
            I2CDevice.I2CWriteTransaction write = I2CDevice.CreateWriteTransaction(new Byte[] { cmd });

            I2CDevice.I2CReadTransaction read = I2CDevice.CreateReadTransaction(data);
            var readTransaction = new I2CDevice.I2CTransaction[] { write, read };

            // Lock the _i2C bus so multiple threads don't try to access it at the same time
            lock (_i2C)
            {
                // Execute the transation
            //    _i2C.Execute(readTransaction, I2CTimeout);
            }
            */
            return data;
        }
    }
}

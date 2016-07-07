using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Netduino.Extender.TransferAdapters;


namespace NetDuinoUtils.BlinkM
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
        public void SetColor(Byte red, Byte green, Byte blue)
        {   
            // Command|Red|Green|Blue
            var data = new[] { (Byte)'n',red, green, blue };
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        public void FadeColor(Byte red, Byte green, Byte blue)
        {
            // Command|Red|Green|Blue
            var data = new[] { (Byte)'c', red, green, blue };
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        public void FadeRandomColor(Byte red, Byte green, Byte blue)
        {
            // Command|Red|Green|Blue
            var data = new[] { (Byte)'C', red, green, blue };
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        public void PlayScript(Byte scriptid, Byte repeats, Byte startline)
        {
            // Command|script number|repeat count|starting line
            var data = new[] { (Byte)'p', scriptid, repeats, startline};
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        public void SetTimeAdjust(SByte speed)
        {
            var data = new[] { (Byte)'t', (byte)speed };
            Debug.Print("Input speed["+speed+"] writing [" + data[1] +"]");
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        public void SetStartupParameters(Byte run, Byte scriptid, Byte repeats, Byte fade, SByte time)
        {
            var data = new[] { (Byte)'B', run, scriptid, repeats, fade, (byte)time };
            _i2Cadapter.WriteBytes(_i2C, data);
        }
        //{'B',playornot, scriptid, repeats, fade, time} 
        public byte[] GetColor()
        {
            return ReadFromDevice((Byte)'g', 3);
        }
        public byte[] ReadFromDevice(byte cmd, byte bytecount)
        {
            _i2Cadapter.WriteBytes(_i2C, new byte[] { cmd });
            
            var data = new Byte[bytecount];
            _i2Cadapter.ReadBytes(_i2C, data);
                       
            return data;
        }
    }
}

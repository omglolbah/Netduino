using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using NetduinoPlusTesting;
using System.Threading;

namespace NetduinoPlusApplication1
{
    public class DS1621
    {
        private const byte READ_CMD = 0xAA;
        private const byte COUNTER_CMD = 0xA8;
        private const byte SLOPE_CMD = 0xA9;
        private const byte START_CMD = 0xEE;
        private const byte STOP_CMD = 0x22;
        private const byte TH_CMD = 0xA1;
        private const byte TL_CMD = 0xA2;
        private const byte CONFIG_CMD = 0xAC;

        private I2CDevice.Configuration _slaveConfig;
        private const int TransactionTimeout = 1000; // ms
        private const byte ClockRateKHz = 100;
        public byte Address { get; private set; }

        /// <summary>
        /// Example sensor constructor
        /// </summary>
        /// <param name="address">I2C device address of the example sensor</param>
        public DS1621(byte address)
        {
            Address = address;
            _slaveConfig = new I2CDevice.Configuration(address, ClockRateKHz);

            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { CONFIG_CMD, 0x01 }, TransactionTimeout);
        }

        public Double ReadUberTemp()
        {
            #region write start
            //fire off a one-shot read
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { START_CMD }, TransactionTimeout);
            #endregion

            Thread.Sleep(1000);
            #region read data
            byte[] temp_read = new byte[1];
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { READ_CMD }, TransactionTimeout);
            I2CBus.GetInstance().Read(_slaveConfig, temp_read, TransactionTimeout);

            #endregion
            byte[] data = new byte[1];
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { SLOPE_CMD }, TransactionTimeout);
            I2CBus.GetInstance().Read(_slaveConfig, data, TransactionTimeout);
            int COUNT_PER_C = data[0];
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { COUNTER_CMD }, TransactionTimeout);
            I2CBus.GetInstance().Read(_slaveConfig, data, TransactionTimeout);
            int COUNT_REMAIN = data[0];

            double TEMP_READ = (double) temp_read[0];
            double TEMPERATURE = TEMP_READ - 0.25 + ((COUNT_PER_C - COUNT_REMAIN) / COUNT_PER_C);

            return TEMPERATURE;
        }
        public double ReadTempFromDevice()
        {

            byte[] read_buffer = new byte[2];

            #region write start
            //fire off a one-shot read
            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { START_CMD }, TransactionTimeout);
            #endregion

            Thread.Sleep(50);

            #region write stop
            //tell continous conversion to stop. Not needed if in one-shot mode.
            //I2CBus.GetInstance().Write(_slaveConfig, new byte[] { STOP_CMD }, TransactionTimeout);
            #endregion

            #region read high/low part
            read_buffer[0] = 0;
            read_buffer[1] = 0;

            I2CBus.GetInstance().Write(_slaveConfig, new byte[] { READ_CMD }, TransactionTimeout);
            I2CBus.GetInstance().Read(_slaveConfig, read_buffer, TransactionTimeout);
            
            byte[] tbuf = new byte[1];
            I2CBus.GetInstance().Read(_slaveConfig, tbuf, TransactionTimeout);
            /*
            transactions = new I2CDevice.I2CTransaction[2];
            transactions[0] = I2CDevice.CreateWriteTransaction(new byte[] { READ_CMD });
            transactions[1] = I2CDevice.CreateReadTransaction(read_buffer);
            rc = i2c.Execute(transactions, 1000);
            // Debug.Print("READ=" + rc);
            if (rc != 3) return -902;
             */
            #endregion
            //Debug.Print("buffer[0]" + read_buffer[0]);
            //Debug.Print("buffer[1]" + read_buffer[1]);
            double temp = read_buffer[0] + (read_buffer[1] == 128 ? 0.5 : 0);
            return temp;
        }
    }


}

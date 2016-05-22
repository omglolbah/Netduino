using System;
using System.Reflection;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Netduino.Extender.TransferAdapters
{
    /// <summary>
    /// Wire-up the I2C device to the Netduino as follows:
    /// SDA -> Analog Pin 4;
    /// SCL -> Analog Pin 5;
    /// GND -> GND.
    /// </summary>
    public sealed class I2CAdapter
    {
        private static readonly Object _I2CLock = new object();
        private static readonly I2CAdapter _Instance = new I2CAdapter();

        private readonly I2CDevice _I2CDevice;

        /// <summary>Creates a new <see cref="I2CAdapter"/> instance </summary>
        /// <remarks>
        /// At this time the .NET Micro Framework only supports a single I2C bus. 
        /// Therefore, creating more than one I2CAdapter instance will generate an
        /// exception. 
        /// </remarks>
        private I2CAdapter()
        {
            //Initialize the I2CDevice with a dummy configuration
            _I2CDevice = new I2CDevice(new I2CDevice.Configuration(0, 0));
        }

        public static I2CAdapter Instance
        {
            get { return _Instance; }
        }

        private I2CDevice.I2CWriteTransaction CreateInternalAddressWriteTransaction(byte[] buffer, uint internalAddress,
                                                                                    byte internalAddressSize)
        {
            I2CDevice.I2CWriteTransaction writeTransaction = I2CDevice.CreateWriteTransaction(buffer);

            ModifyToRepeatedStartTransaction(internalAddress, internalAddressSize, writeTransaction,
                                             typeof(I2CDevice.I2CWriteTransaction));

            return writeTransaction;
        }

        private I2CDevice.I2CReadTransaction CreateInternalAddressReadTransaction(byte[] buffer, uint internalAddress,
                                                                                  byte internalAddressSize)
        {
            I2CDevice.I2CReadTransaction readTransaction = I2CDevice.CreateReadTransaction(buffer);

            ModifyToRepeatedStartTransaction(internalAddress, internalAddressSize, readTransaction,
                                             typeof(I2CDevice.I2CReadTransaction));

            return readTransaction;
        }

        /// <summary>
        /// To use the new I2C InternalAddress feature (repeated start bit) in the v4.1.1 alpha,
        /// add the following method to your code...
        /// and then call it instead of built-in I2CDevice.CreateWriteTransaction/CreateReadTransaction functions when appropriate.
        /// </summary>
        /// <param name="internalAddress"></param>
        /// <param name="internalAddressSize">The InternalAddressSize parameter defines
        /// the # of bytes used to represent the InternalAddress. The InternalAddress is a set of bytes sent to a device before the
        /// buffer--often a register # or memory location to write/read. When used in an I2C ReadTransaction, an extra start bit will
        /// be sent to the I2C device after the InternalAddress (followed by the traditional buffer read/write).</param>
        /// <param name="transaction"></param>
        /// <param name="transactionType"></param>
        private static void ModifyToRepeatedStartTransaction(uint internalAddress, byte internalAddressSize,
                                                             I2CDevice.I2CTransaction transaction, Type transactionType)
        {
            FieldInfo fieldInfo = transactionType.GetField("Custom_InternalAddress",
                                                           BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(transaction, internalAddress);

            fieldInfo = transactionType.GetField("Custom_InternalAddressSize", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(transaction, internalAddressSize);
        }

        public int WriteInternalAddressBytes(I2CDevice.Configuration i2CConfiguration, byte[] bytesToWrite,
                                             uint internalAddress, byte internalAddressSize)
        {
            int bytesTransfered = ExecuteI2CTransactions(i2CConfiguration,
                                                         CreateInternalAddressWriteTransaction(bytesToWrite, internalAddress,
                                                                                               internalAddressSize));

            // I2CDevice.Execute returns the total number of bytes transfered in BOTH directions for all transactions
            if (bytesTransfered < (bytesToWrite.Length))
            {
                Debug.Print("WriteInternalAddressBytes: I2C expected + '" + bytesToWrite.Length + "' but could write + '" +
                            bytesTransfered + "'.");
            }

            return bytesTransfered;
        }

        public int ReadInternalAddressBytes(I2CDevice.Configuration i2CConfiguration, byte[] bytesToRead, uint internalAddress,
                                            byte internalAddressSize)
        {
            int bytesTransfered = ExecuteI2CTransactions(i2CConfiguration,
                                                         CreateInternalAddressReadTransaction(bytesToRead, internalAddress,
                                                                                              internalAddressSize));

            // I2CDevice.Execute returns the total number of bytes transfered in BOTH directions for all transactions
            if (bytesTransfered < (bytesToRead.Length))
            {
                Debug.Print("ReadInternalAddressBytes: I2C expected + '" + bytesToRead.Length + "' but could read + '" +
                            bytesTransfered + "'.");
            }

            return bytesTransfered;
        }

        public int WriteBytes(I2CDevice.Configuration i2CConfiguration, byte[] bytesToWrite)
        {
            int bytesTransfered = ExecuteI2CTransactions(i2CConfiguration, I2CDevice.CreateWriteTransaction(bytesToWrite));

            // I2CDevice.Execute returns the total number of bytes transfered in BOTH directions for all transactions
            if (bytesTransfered < (bytesToWrite.Length))
            {
                Debug.Print("WriteBytes: I2C expected + '" + bytesToWrite.Length + "' but could write + '" + bytesTransfered + "'.");
            }

            return bytesTransfered;
        }

        public int ReadBytes(I2CDevice.Configuration i2CConfiguration, byte[] bytesToRead)
        {
            int bytesTransfered = ExecuteI2CTransactions(i2CConfiguration, I2CDevice.CreateReadTransaction(bytesToRead));

            // I2CDevice.Execute returns the total number of bytes transfered in BOTH directions for all transactions
            if (bytesTransfered < (bytesToRead.Length))
            {
                Debug.Print("ReadBytes: I2C expected + '" + bytesToRead.Length + "' but could read + '" + bytesTransfered + "'.");
            }

            return bytesTransfered;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">Netduino needs only the 7 most significant bits for the address e.g. 0x91 >> 1 = 0x48.</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int ExecuteI2CTransactions(I2CDevice.Configuration configuration, I2CDevice.I2CTransaction transaction)
        {
            lock (_I2CLock)
            {
                _I2CDevice.Config = configuration;

                // Execute the read or write transaction, check if byte was successfully transfered
                int bytesTransfered = _I2CDevice.Execute(new[] { transaction }, 100);
                return bytesTransfered;
            }
        }
    }
}
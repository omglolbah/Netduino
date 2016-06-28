using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using NetDuinoUtils.TMP100;

namespace NetDuinoUtils.Utils
{
    public class TMP100Reader
    {
        #region Data

        private TMP100Sensor _tmp;
        private static object _lockObject = new object();

        private static double _temperature = 0;
        private static AutoResetEvent mutex = new AutoResetEvent(false);

        private static TMP100Reader _instance;

        #endregion

        #region Singleton and Constructor

        public static TMP100Reader Instance
        {
            get
            {
                lock (_lockObject)
                {
                    return _instance ?? (_instance = new TMP100Reader());
                }
            }
        }

        private TMP100Reader()
        {
            _tmp = new TMP100Sensor(0x4a);
            _tmp.Resolution = TMP100Sensor.ResolutionBits.R00625;
        }

        #endregion

        #region Writer

        public double GetTemperature()
        {
            lock (_lockObject)
            {
                _temperature = _tmp.GetTemperature();
                return _temperature;
            }
        }

        #endregion
    }
}

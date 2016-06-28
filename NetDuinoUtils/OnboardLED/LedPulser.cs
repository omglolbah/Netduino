using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NetDuinoUtils.Utils
{
    public class LedPulser
    {
        #region Data

        private OutputPort _led;
        private static object _lockObject = new object();

        private static int _ledDuration = 50;
        private static AutoResetEvent mutex = new AutoResetEvent(false);

        private static LedPulser _instance;

        #endregion

        #region Singleton and Constructor

        public static LedPulser Instance
        {
            get
            {
                lock (_lockObject)
                {
                    return _instance ?? (_instance = new LedPulser());
                }
            }
        }

        private LedPulser()
        {
            _led = new OutputPort(Pins.ONBOARD_LED, false);
            
            ThreadUtil.Start(() =>
            {
                while (true)
                {
                    lock (_lockObject)
                    {
        
                        _led.Write(true);
                        Thread.Sleep(_ledDuration);
                        _led.Write(false);
                    }
                    mutex.WaitOne();
                }
            });
        }

        #endregion

        #region Writer

        public void Pulse(int duration)
        {
            lock (_lockObject)
            {
                _ledDuration = duration;
                mutex.Set();
            }
        }

        #endregion
    }
}

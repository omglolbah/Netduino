using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using NetDuinoUtils.TMP100;
using NetDuinoUtils.BlinkM;

namespace NetDuinoUtils.Utils
{
    public class BlinkMHandler
    {
        #region Data

        private BlinkM.BlinkM _blinkm;
        private static object _lockObject = new object();

        private static AutoResetEvent mutex = new AutoResetEvent(false);

        private static BlinkMHandler _instance;

        #endregion

        #region Singleton and Constructor

        public static BlinkMHandler Instance
        {
            get
            {
                lock (_lockObject)
                {
                    return _instance ?? (_instance = new BlinkMHandler());
                }
            }
        }

        private BlinkMHandler()
        {
            _blinkm = new BlinkM.BlinkM(0x09);
        }

        #endregion

        #region Read/Write

        public void SetColor(byte r, byte g, byte b)
        {
            lock (_lockObject)
            {
                _blinkm.SetColor(r, g, b);
            }
        }
        public void FadeColor(byte r, byte g, byte b)
        {
            lock (_lockObject)
            {
                _blinkm.FadeColor(r, g, b);
            }
        }
        public void FadeRandomColor(byte r, byte g, byte b)
        {
            lock (_lockObject)
            {
                _blinkm.FadeRandomColor(r, g, b);
            }
        }
        public void PlayScript(byte scriptid, byte repeats, byte startline)
        {
            lock (_lockObject)
            {
                _blinkm.PlayScript(scriptid,repeats,startline);
            }
        }
        public void StopScript()
        {
            lock (_lockObject)
            {
                _blinkm.StopScript();
            }
        }
        public byte[] GetColor()
        {
            lock (_lockObject)
            {
                return _blinkm.GetColor();
            }
        }
        #endregion
    }
}

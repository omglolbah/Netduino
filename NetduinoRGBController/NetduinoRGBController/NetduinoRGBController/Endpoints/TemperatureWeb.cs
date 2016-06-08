using System;
using System.Collections;
using NetDuinoUtils.Utils;
using NWebREST.Web;

namespace Endpoints
{
    public class TemperaturePage : IEndPointProvider
    {
        #region Endpoint initialization

        public void Initialize() { }

        public ArrayList AvailableEndPoints()
        {
            var list = new ArrayList
                {
                    new EndPoint
                        {
                            Action = TMP100,
                            Name = "tmp100",
                            ReadOnly = true,
                            Description = "Returns the temperature."
                        }
                };
            return list;
        }

        #endregion

        #region Endpoint Execution

        private string TMP100(EndPointActionArguments misc, string[] items)
        {
            return "Temperature is: " + TMP100Reader.Instance.GetTemperature();
        }

        #endregion
    }
}

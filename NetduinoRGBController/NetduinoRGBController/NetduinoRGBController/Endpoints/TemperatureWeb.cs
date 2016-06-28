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
            double tempC = TMP100Reader.Instance.GetTemperature();
            double tempF = tempC * 1.8 + 32;
            if (misc.ReturnType == HelperClass.ReturnType.HTML)
            {
                return HTMLUtils.BuildHTML("Temperature is: " + tempC + "C / " + tempF + "F at Desk.");
            }
            else if (misc.ReturnType == HelperClass.ReturnType.JSON)
            {
                return @"
{""sensors"":[
    {""Location"":""Desk"", ""TempC"":""" + tempC + @""", ""TempF"":""" + tempF + @"""},
    {""Location"":""Dummy"", ""TempC"":"" " + tempC + @""", ""TempF"":""" + tempF + @"""}
]}";
            }
            throw new NotImplementedException("Invalid returntype: " + misc.ReturnType.ToString());
        }

        #endregion
    }
}

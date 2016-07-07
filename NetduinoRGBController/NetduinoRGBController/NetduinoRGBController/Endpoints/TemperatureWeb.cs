using System;
using System.Collections;
using NetDuinoUtils.Utils;
using NWebREST.Web;
using NetDuinoUtils.TMP100;

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
                        },
                    new EndPoint
                        {
                            Action = TMP100log,
                            Name = "tmp100log",
                            ReadOnly = true,
                            Description = "Returns the temperature log."
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
        private string TMP100log(EndPointActionArguments misc, string[] items)
        {
            if (misc.ReturnType == HelperClass.ReturnType.HTML)
            {
                return HTMLUtils.BuildHTML("bork bork bork");
            }
            else if (misc.ReturnType == HelperClass.ReturnType.JSON)
            {
                string s = @"{""Temperature History"":[";
                foreach(NetDuinoUtils.TMP100.TempData td in TMP100LoggerService.Instance.Temperatures)
                {
                    string line = @"
{""Timestamp"":"+ td.TimeStamp +@", ""TempC"":""" + td.Temperature + @"""},";
                    s += line;
                }
                s += @"
]}";
                return s;
            }
            throw new NotImplementedException("Invalid returntype: " + misc.ReturnType.ToString());
        }

        #endregion
    }
}

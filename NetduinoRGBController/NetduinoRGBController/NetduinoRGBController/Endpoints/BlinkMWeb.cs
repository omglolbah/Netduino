using System;
using System.Collections;
using NetDuinoUtils.Utils;
using NWebREST.Web;

namespace Endpoints
{
    public class BlinkMWeb : IEndPointProvider
    {
        #region Endpoint initialization

        public void Initialize() { }

        public ArrayList AvailableEndPoints()
        {
            var list = new ArrayList
                {
                    new EndPoint
                        {
                            Action = SetColor,
                            Name = "SetColor",
                            Description = "Sets the R/G/B value on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = FadeColor,
                            Name = "FadeColor",
                            Description = "Fades to the R/G/B value on the BlinkM device."
                        },
                };
            return list;
        }

        #endregion

        #region Endpoint Execution

        private string SetColor(EndPointActionArguments misc, string[] items)
        {
            byte r,g,b = 0;

            if (items != null && items.Length == 3)
            {
                r = byte.Parse(items[0]);
                g = byte.Parse(items[1]);
                b = byte.Parse(items[2]);
            }
            else
            {
                return "Missing arguments. RGB values in range 0-255 needed!";
            }

            BlinkMHandler.Instance.SetColor(r,g,b);

            return "Set color to RGB[" + r + "," + g +"," + b + "]";
        }
        private string FadeColor(EndPointActionArguments misc, string[] items)
        {
            byte r, g, b = 0;

            if (items != null && items.Length == 3)
            {
                r = byte.Parse(items[0]);
                g = byte.Parse(items[1]);
                b = byte.Parse(items[2]);
            }
            else
            {
                return "Missing arguments. RGB values in range 0-255 needed!";
            }

            BlinkMHandler.Instance.FadeColor(r, g, b);

            return "Set color to RGB[" + r + "," + g + "," + b + "]";
        }
        #endregion
    }
}

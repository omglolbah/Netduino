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
                            ReadOnly = false,
                            Description = "Sets the R/G/B value on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = FadeColor,
                            Name = "FadeColor",
                            ReadOnly = false,
                            Description = "Fades to the R/G/B value on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = FadeRandomColor,
                            Name = "FadeRandomColor",
                            ReadOnly = false,
                            Description = "Fades to a random R/G/B value on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = PlayScript,
                            Name = "PlayScript",
                            ReadOnly = false,
                            Description = "Plays a script on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = StopScript,
                            Name = "StopScript",
                            ReadOnly = false,
                            Description = "Stops any running scripts on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = SetTimeAdjust,
                            Name = "SetTimeAdjust",
                            ReadOnly = false,
                            Description = "Sets the speed of script playback on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = SetStartupParameters,
                            Name = "SetStartupParameters",
                            ReadOnly = false,
                            Description = "Sets the startup parameters on the BlinkM device."
                        },
                    new EndPoint
                        {
                            Action = GetColor,
                            Name = "GetColor",
                            ReadOnly = true,
                            Description = "Gets the current RGB value of the BlinkM device."
                        },
                };
            return list;
        }

        #endregion

        #region Endpoint Execution
        private string GetArgByName(string[] items, string name, int missingvalue)
        {
            return GetArgByName(items, name, missingvalue.ToString());
        }
        private string GetArgByName(string[] items, string name)
        {
            return GetArgByName(items, name, null);
        }
        private string GetArgByName(string[] items, string name, string missingvalue)
        {
            foreach(string s in items)
            {
                string[] a = s.Split('=');
                if(a.Length == 2 && a[0] == name)
                {
                    return a[1];
                }
            }
            return missingvalue;
        }
        private string SetColor(EndPointActionArguments misc, string[] items)
        {
            byte r,g,b = 0;

            if (items != null && items.Length > 0)
            {
                r = byte.Parse(GetArgByName(items,"r", 0));
                g = byte.Parse(GetArgByName(items,"g", 0));
                b = byte.Parse(GetArgByName(items,"b", 0));
            }
            else
            {
                return "Missing arguments. RGB values in range 0-255 needed!\n\r";
            }

            BlinkMHandler.Instance.SetColor(r,g,b);

            return "Set color to RGB[" + r + "," + g + "," + b + "]\n\r";
        }
        private string FadeColor(EndPointActionArguments misc, string[] items)
        {
            byte r, g, b = 0;

            if (items != null && items.Length > 0)
            {
                r = byte.Parse(GetArgByName(items, "r", 0));
                g = byte.Parse(GetArgByName(items, "g", 0));
                b = byte.Parse(GetArgByName(items, "b", 0));
            }
            else
            {
                return "Missing arguments. RGB values in range 0-255 needed!\n\r";
            }

            BlinkMHandler.Instance.FadeColor(r, g, b);

            return "Set color to RGB[" + r + "," + g + "," + b + "]\n\r";
        }
        private string FadeRandomColor(EndPointActionArguments misc, string[] items)
        {
            byte r, g, b = 0;

            if (items != null && items.Length > 0)
            {
                r = byte.Parse(GetArgByName(items, "r", 255));
                g = byte.Parse(GetArgByName(items, "g", 255));
                b = byte.Parse(GetArgByName(items, "b", 255));
            }
            else
            {
                return "Missing arguments. RGB values in range 0-255 needed!\n\r";
            }

            BlinkMHandler.Instance.FadeRandomColor(r, g, b);

            return "Fading to random color. Randomness variables[" + r + "," + g + "," + b + "]\n\r";
        }

        private string SetStartupParameters(EndPointActionArguments misc, string[] items)
        {
            //Byte run, Byte scriptid, Byte repeats, Byte fade, SByte time)
            byte run, scriptid, repeats,fade = 0;
            SByte time = 0;

            if (items != null && items.Length > 1)
            {
                run = byte.Parse(GetArgByName(items, "run", 15));
                scriptid = byte.Parse(GetArgByName(items, "scriptid", 0));
                repeats = byte.Parse(GetArgByName(items, "repeats", 0));
                fade = byte.Parse(GetArgByName(items, "fade", 15));
                time = SByte.Parse(GetArgByName(items, "time", 0));
            }
            else
            {
                return "run = 0/1\n\r" +
                       "scriptid = 0-18\n\r" +
                       "repeats = byte, 0 for autorepeat\n\r" +
                       "fade = 0-255 (default 15)\n\r" +
                       "time = -128-127 (default 0)";
            }

            BlinkMHandler.Instance.SetStartupParameters(run, scriptid, repeats, fade, time);

            return "Set start parameters to run[" + run + "] scriptid[" + scriptid + "] repeats[" + repeats + "] fade[" + fade + "] time[" + time + "]";
        }
        private string PlayScript(EndPointActionArguments misc, string[] items)
        {
            byte scriptid, repeats, startline = 0;

            if (items != null && items.Length > 1)
            {
                scriptid  = byte.Parse(GetArgByName(items, "scriptid" , 0));
                repeats   = byte.Parse(GetArgByName(items, "repeats"  , 1));
                startline = byte.Parse(GetArgByName(items, "startline", 0));
            }
            else
            {
                return "scriptid = 0-18\n\rrepeats = byte, 0 for autorepeat\n\rstartline = 0-xx (default 0)\n\r";
            }

            BlinkMHandler.Instance.PlayScript(scriptid, repeats, startline);

            return "Playing script ["+scriptid+"] repeating [" + repeats + "] starting at line [" + startline + "]\n\r";
        }
        private string SetTimeAdjust(EndPointActionArguments misc, string[] items)
        {
            SByte speed = 0;

            if (items != null && items.Length == 1)
            {
                speed = SByte.Parse(GetArgByName(items, "speed", 0));
            }
            else
            {
                return "speed = -128 - 127 \n\r";
            }

            BlinkMHandler.Instance.SetTimeAdjust(speed);

            return "Setting time adjust to [" + speed + "]\n\r";
        }

        private string StopScript(EndPointActionArguments misc, string[] items)
        {
            BlinkMHandler.Instance.StopScript();
            return "Script stopped.\n\r";
        }

        private string GetColor(EndPointActionArguments misc, string[] items)
        {
            byte[] color = BlinkMHandler.Instance.GetColor();
            if (color.Length == 3)
            {
                return "RGB[" + color[0] + "," + color[1] + "," + color[2] + "]\n\r";
            }
            else
            {
                return "Invalid color data. Sadpanda!\n\r";
            }
        }

        #endregion
    }
}

using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Text;

using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using PWM = SecretLabs.NETMF.Hardware.PWM;

using NWebREST.Web;
using NetDuinoUtils.Utils;
using NetDuinoUtils.TMP100;

namespace NetduinoRGBController
{
    
    
    public class Program
    {
        private static void InitializeServer()
        {
            // Initialize the network interface with a static IP
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableStaticIP("10.0.0.225", "255.255.255.0", "10.0.0.4");
            Debug.Print(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);
            NetDuinoUtils.Utils.SyncTime.Update("time.nist.gov", 1);
        }

        public static void Main()
        {
            //set static ip
            InitializeServer();

            LedPulser.Instance.Pulse(1000); 
            

            Debug.Print("Webserver initialized! [" + DateTime.Now.TimeOfDay + "]");

            WebServerWrapper.InitializeWebEndPoints(new ArrayList
                                                {
                                                    //new BasicPage(),
                                                    new Endpoints.BlinkMWeb(),
                                                    new Endpoints.TemperaturePage(),
                                                    new Endpoints.ButtonWeb()
                                                });

            WebServerWrapper.StartWebServer();

            RunUtil.KeepRunning();

                //double temp = GetTemperature();
                
                //byte[] data = _BlinkM.ReadSomething(0x67, 3);
                //Debug.Print("data=" + data[0] + "-" + data[1] + "-" + data[2]);

                //double r = temp * 6;
                //byte rb = (byte)r;
                //_BlinkM.WriteColor(rb, 0, 0);
                //Thread.Sleep((int)temp*100);

                /*
                _BlinkM.WriteColor(0, 0, 0);
                Thread.Sleep(500);
                _BlinkM.WriteColor(255, 0, 0);
                Thread.Sleep(500);
                _BlinkM.WriteColor(0, 0, 255);
                Thread.Sleep(500);
                _BlinkM.WriteColor(0, 0, 0);
                Thread.Sleep(1);
                 */

            #region RGB
            /*
            RGBOutput rgb = new RGBOutput(Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D10);
            //static PWM redChannel = new PWM(Pins.GPIO_PIN_D6);
            //static PWM greenChannel = new PWM(Pins.GPIO_PIN_D9);
            //static PWM blueChannel = new PWM(Pins.GPIO_PIN_D10);

            float redIntensity = 0.0f;
            float greenIntensity = 0.0f;
            float blueIntensity = 0.0f;
            
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            InputPort toggle = new InputPort(Pins.GPIO_PIN_D0, true, Port.ResistorMode.PullUp);
            
            int counter = 0;
            int delay = 4;
            
            while (true)
            {
                counter++;
                counter = counter % 1000;
                if(counter == 0)
                {
                    led.Write(!led.Read());
                }
                redIntensity += .1f;
                if (redIntensity > 100) redIntensity = 0.0f;

                greenIntensity += .2f;
                if (greenIntensity > 100) greenIntensity = 0.0f;

                blueIntensity += .3f;
                if (blueIntensity > 100) blueIntensity = 0.0f;
                
                

                
                if (toggle.Read())
                {
                    rgb.SetColor(0, 0, 0);
                    Thread.Sleep(250);
                    continue;
                }
                else
                {
                    rgb.SetColor((int)redIntensity, (int)greenIntensity, (int)blueIntensity);
                    Thread.Sleep(delay);
                }
            }
            */
            #endregion
        }
    }
    public class Color
    {
        public int Red {get;set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public override string ToString()
        {
            return "Color {" + Red + ":" + Green + ":" + Blue + "}";
        }
    }
    public class RGBOutput
    {
        public RGBOutput()
        {

        }
        public RGBOutput(Cpu.Pin _redpin, Cpu.Pin _greenpin, Cpu.Pin _bluepin)
        {
            SetPins(_redpin, _greenpin, _bluepin);
        }
        PWM RedPWM;
        PWM GreenPWM;
        PWM BluePWM;

        public void SetPins(Cpu.Pin _redpin, Cpu.Pin _greenpin, Cpu.Pin _bluepin)
        {
            if (RedPWM != null)
            {
                RedPWM.Dispose();
            }
            RedPWM = new PWM(_redpin);
            RedPWM.SetDutyCycle(0);
            if (GreenPWM != null)
            {
                GreenPWM.Dispose();
            }
            GreenPWM = new PWM(_greenpin);
            GreenPWM.SetDutyCycle(0);
            if (BluePWM != null)
            {
                BluePWM.Dispose();
            }
            BluePWM = new PWM(_bluepin);
            BluePWM.SetDutyCycle(0);
        }

        public void SetColor(int r, int g, int b)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }

        #region properties
        private int red;
        public int Red
        {
            get { return red; }
            set
            {
                red = value;
                if (RedPWM != null)
                {
                    red = System.Math.Min(100, System.Math.Max(0, value));
                    RedPWM.SetDutyCycle((uint)red);
                }
            }
        }

        private int green;
        public int Green
        {
            get { return green; }
            set
            {
                green = System.Math.Min(100, System.Math.Max(0, value));
                GreenPWM.SetDutyCycle((uint)green);
            }
        }

        private int blue;
        public int Blue
        {
            get { return blue; }
            set
            {
                blue = System.Math.Min(100, System.Math.Max(0, value));
                BluePWM.SetDutyCycle((uint)blue);
            }
        }
        #endregion
    }
    
}

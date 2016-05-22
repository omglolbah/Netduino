using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Text;

using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using PWM = SecretLabs.NETMF.Hardware.PWM;

namespace NetduinoRGBController
{
    
    
    public class Program
    {
        private static BlinkM _BlinkM;
        private static OutputPort OnBoardLED;
        private static TMP100 T;

        // Network communication
        private static Socket socket = null;

        private static void InitializeServer()
        {
            // Initialize the network interface with a static IP
            Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].EnableStaticIP("10.0.0.225", "255.255.255.0", "10.0.0.4");
            Debug.Print(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);

            //Initialize Socket class
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            //Request and bind to an IP
            socket.Bind(new IPEndPoint(IPAddress.Any, 12000));
            
            //Start listen for requests
            socket.Listen(10);
        }
        private static void InitializeDevices()
        {
            Debug.Print("InitializeDevices starting...");
            // BlinkM init, stop the running script
            _BlinkM = new BlinkM(0x09);
            _BlinkM.StopScript();

            // Onboard LED for indication
            OnBoardLED = new OutputPort(Pins.ONBOARD_LED, false);
            
            // TMP100 temperature sensor at address 0x4A configured to highest resolution.
            T = new TMP100(0x4A);
            T.Resolution = TMP100.ResolutionBits.R00625;
            
            Debug.Print("InitializeDevices finished.");
        }
        public static void Main()
        {
            InitializeServer();

            InitializeDevices();

            PulseOnBoardLed(1000);

            while(true)
            {
                PulseOnBoardLed(50);

                Debug.Print("Preparing to accept connections...");
                using (Socket clientSocket = socket.Accept())
                {
                    Debug.Print("Accepted connection.");
                    //Get clients IP
                    IPEndPoint clientIP = clientSocket.RemoteEndPoint as IPEndPoint;
                    EndPoint clientEndPoint = clientSocket.RemoteEndPoint;
                    Debug.Print("Remote ip:" + clientIP.ToString());

                    //int byteCount = cSocket.Available;
                    int bytesReceived = clientSocket.Available;
                    if (bytesReceived == 0)
                    {
                        string header = "Empty request. Temperature[" + T.GetTemperature() + "]";
                        clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        //Blink the onboard LED
                        PulseOnBoardLed(150);
                    }
                    else
                    {
                        //Get request to parse
                        byte[] buffer = new byte[bytesReceived];
                        int byteCount = clientSocket.Receive(buffer, bytesReceived, SocketFlags.None);
                        string request = new string(Encoding.UTF8.GetChars(buffer));
                        Debug.Print("Request[" + request + "]");

                        string header = "Parsing not supported yet!";
                        clientSocket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
                        //Blink the onboard LED
                        PulseOnBoardLed(150);
                    }
                }

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
            }


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

        private static double GetTemperature()
        {
            double temp = T.GetTemperature();
            Debug.Print("Temp:" + temp.ToString() + "C");
            return temp;
        }
        /// <summary>
        /// Simple function to pulse the on-board LED for indication
        /// </summary>
        /// <param name="milliseconds"></param>
        private static void PulseOnBoardLed(int milliseconds)
        {
            OnBoardLED.Write(true);
            Thread.Sleep(milliseconds);
            OnBoardLED.Write(false);
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

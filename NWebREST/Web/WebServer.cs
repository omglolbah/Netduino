using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using NetDuinoUtils.Utils;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace NWebREST.Web
{
    /// <summary>
    /// Simple multithreaded webserver for Netduino.
    /// Lets user register endpoints that can be called on the server.
    /// When the server receives a valid command, it fires the EndPointReceived event with details
    /// stored in the EndPoinEventArgs parameter.
    /// 
    /// Modifications By:   Kjetil Seim Haugen
    /// Contact:            netduino@omglolbah.net
    /// Latest revision:    sometime    2016
    /// 
    /// 
    /// Modifications By:   Anton Kropp
    /// Contact:            akropp@gmail.com
    /// Latest revision:    28 november 2012
    /// 
    /// Original Author:    Jasper Schuurmans
    /// Contact:            jasper@schuurmans.cc
    /// Latest revision:    06 april 2011
    /// </summary>
    internal class WebServer
    {
        private bool _cancel;
        private readonly Thread _serverThread;

        private readonly bool _enableLedStatus = true;

        #region Constructors

        /// <summary>
        /// Instantiates a new webserver.
        /// </summary>
        /// <param name="port">Port number to listen on.</param>
        /// <param name="enableLedStatus"></param>
        public WebServer(int port, bool enableLedStatus = true)
        {
            Port = port;

            _serverThread = new Thread(StartServer);

            _enableLedStatus = enableLedStatus;

            Debug.Print("WebControl started on port " + port);
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate for the EndPointReceived event.
        /// </summary>
        public delegate void EndPointReceivedHandler(object source, RequestEventArgs e);

        /// <summary>
        /// EndPointReceived event is triggered when a valid command (plus parameters) is received.
        /// Valid commands are defined in the AllowedEndPoints property.
        /// </summary>
        public event EndPointReceivedHandler RequestReceived;

        #endregion

        #region Public and private methods

        /// <summary>
        /// Initialize the multithreaded server.
        /// </summary>
        public void Start()
        {
            // List ethernet interfaces, so we can determine the server's address
            ListInterfaces();

            // start server
            _cancel = false;
            _serverThread.Start();
            Debug.Print("Started server in thread " + _serverThread.GetHashCode());
        }

        /// <summary>
        /// Parses a raw web request and filters out the command and arguments.
        /// </summary>
        /// <param name="rawData">The raw web request (including headers).</param>
        /// <returns>The parsed WebCommand if the request is valid, otherwise Null.</returns>
        private Request InterpretRequest(string rawData)
        {
            if(rawData == null)
            {
                return null;
            }
            string[] lines = rawData.Split('\n');
            for (int i = 0; i < lines.Length; i += 1)
            { 
                lines[i] = lines[i].Trim();
            }

            string reqType = lines[0].Substring(0, lines[0].IndexOf(' ')).Trim().ToUpper();
            if(reqType != "GET" && reqType != "POST")
            {
                return null; // not a handled type. Should probably fix this at some point...
            }
            int idx = lines[0].IndexOf('/')+1;
            int len = lines[0].LastIndexOf(' ')-idx;

            string reqData = lines[0].Substring(idx, len);

            string[] reqArgs = reqData.Split('/');

            string[] reqCommand = null;
            if (reqArgs.Length > 0)
            {
                // Parse first part to command
                reqCommand = reqArgs[0].Split('.');
            }
            else
            {
                //No command sent, assume root reply
                return null;
            }

            // http://url/foo/test
            // Check if this is a valid command
            Request returnRequest = new Request();
            foreach (EndPoint endPoint in _allowedEndPoints)
            {
                if (endPoint.Name.ToLower() == reqCommand[0].ToLower())
                {
                    //this feels like a kludge... blergh
                    if(endPoint.ReadOnly == true  && reqType == "GET" ||
                       endPoint.ReadOnly == false && reqType == "POST")
                    {
                        Debug.Print("reqType[" + reqType + "]");
                        Debug.Print("Endpoint[" + endPoint.Name + "]");
                        returnRequest.ReqEndPoint = endPoint;
                        if (reqCommand.Length > 1 && reqCommand[1].ToLower() == "json")
                        {
                            returnRequest.ReqReturnType = HelperClass.ReturnType.JSON;
                        }
                        else
                        {
                            returnRequest.ReqReturnType = HelperClass.ReturnType.HTML;
                        }
                        break;
                    }
                }
            }
            if (returnRequest.ReqEndPoint == null)
            {
                return null;
            }
            if (reqType == "GET") 
            { 
                var arguments = new string[reqArgs.Length - 1];
    
                for (int i = 1; i < reqArgs.Length; i++)
                {
                    arguments[i - 1] = reqArgs[i];
                }
                returnRequest.ReqEndPoint.Arguments = arguments;
            }
            else if (reqType == "POST")
            {
                string[] argString = lines[lines.Length - 1].Split('&');

                returnRequest.ReqEndPoint.Arguments = argString;
            }


            return returnRequest;
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        private void StartServer()
        {
            using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                server.Bind(new IPEndPoint(IPAddress.Any, Port));

                server.Listen(1);

                while (!_cancel)
                {
                    var connection = server.Accept();

                    if (connection.Poll(-1, SelectMode.SelectRead))
                    {
                        // Create buffer and receive raw bytes.
                        var bytes = new byte[connection.Available];

                        connection.Receive(bytes);

                        // Convert to string, will include HTTP headers.
                        var rawData = new string(Encoding.UTF8.GetChars(bytes));

                        Request request = InterpretRequest(rawData);

                        if (request != null)
                        {
                            if (_enableLedStatus)
                            {
                                PingLed();
                            }

                            // dispatch the endpoint
                            var e = new RequestEventArgs(request, connection, request.ReqReturnType);

                            if (RequestReceived != null)
                            {
                                ThreadUtil.SafeQueueWorkItem(() =>
                                {
                                    RequestReceived(null, e);

                                    if (e.ManualSent)
                                    {
                                        // the client should close the socket
                                    }
                                    else
                                    {
                                        var response = e.ReturnString;
                                        SendResponse(response, connection);
                                    }
                                });
                            }
                        }
                        else
                        {
                            SendResponse(GetApiList(), connection);
                        }
                    }

                }
            }
        }

        private string GetApiList()
        {
            try
            {
                string returnString = @"
<body>
<head>
<style type=""text/css"">"
                                      + GetStylings() +
                                      @"</style>
</head>
<div class=""container"">
	<div class=""title"">Netduino Api List</div>
    <div class=""main"">
        <ul>
";
                
                
                foreach (EndPoint endpoint in _allowedEndPoints)
                {
                    returnString +=
                        @"           <li><a href=""" + endpoint.Name + "\">" + endpoint.Name +
                        "</a><span class=\"description\">(" + endpoint.Description + ")</span></li>";
                    returnString += "\r\n";
                }
                returnString += "</ul></body>";
                return returnString;
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        private static string GetStylings()
        {
            return
                @"
body{
    background-color: #000000;
    color: #bbbbbb
}

ul { 
    list-style-type: circle; 
    font-size:20px;
} 

.container
{
	height:100%;
}                

.description{
	font-size:12px;
	padding:10px;
}	

.main{
    font-family: 'Courier New', Courier, monospace;
	height:100%;
	padding:5px;
}

.title{
    font-family: 'Courier New', Courier, monospace;
	font-size:50px;
	font-variant: small-caps;
	padding:20px;
}

a:link {color: #998700;}
a:visited {color: #998700;}
a:active {color: 998700;}
a:hover {color: #0F00B8;}
a {
    font-family: 'Courier New', Courier, monospace;
    text-decoration: underline;
    font-variant:small-caps;
}
";

        }

        private static void WriteBytes(byte[] bytes, Socket connection)
        {
            try
            {
                connection.Send(bytes, 0, bytes.Length, SocketFlags.None);
                using (connection)
                {
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private static void SendResponse(string response, Socket connection)
        {
            try
            {
                byte[] returnBytes = Encoding.UTF8.GetBytes(response);
                WriteBytes(returnBytes, connection);
            }
            catch(Exception ex)
            {
                
            }
        }
        public void PingLed()
        {
            LedPulser.Instance.Pulse(50);
        }

        private static void ListInterfaces()
        {
            NetworkInterface[] ifaces = NetworkInterface.GetAllNetworkInterfaces();
            Debug.Print("Number of Interfaces: " + ifaces.Length);
            foreach (NetworkInterface iface in ifaces)
            {
                Debug.Print("IP:  " + iface.IPAddress + "/" + iface.SubnetMask);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the port the server listens on.
        /// </summary>
        private int Port { get; set; }

        /// <summary>
        /// List of commands that can be handled by the server.
        /// </summary>
        private readonly System.Collections.ArrayList _allowedEndPoints = new System.Collections.ArrayList();

        #endregion

        public void RegisterEndPoint(EndPoint endPoint)
        {
            _allowedEndPoints.Add(endPoint);
        }
    }
    
    public class Request
    {
        public EndPoint ReqEndPoint { get; set; }
        public NetDuinoUtils.Utils.HelperClass.ReturnType ReqReturnType { get; set; }
        public Request()
        { }
        public Request(EndPoint ep, NetDuinoUtils.Utils.HelperClass.ReturnType rt)
        {
            ReqEndPoint = ep;
            ReqReturnType = rt;
        }
    }
}

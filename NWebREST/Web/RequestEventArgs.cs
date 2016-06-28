using System.Net.Sockets;

namespace NWebREST.Web
{
    /// <summary>
    /// Event arguments of an incoming web command.
    /// </summary>
    public class RequestEventArgs
    {
        /// <summary>
        /// Allows us to tell the web server that we manually replied back
        /// via the socket. If false, the server will reply back with our string response
        /// This lets us write things other than the generic response around (for example if you want
        /// to stream custom binary)
        /// </summary>
        public bool ManualSent { get; set; }

        public RequestEventArgs()
        {
        }

        public RequestEventArgs(Request command)
        {
            Command = command.ReqEndPoint;
            ReturnType = NetDuinoUtils.Utils.HelperClass.ReturnType.HTML;
        }

        public RequestEventArgs(Request command, Socket connection)
        {
            Command = command.ReqEndPoint;
            Connection = connection;
            Connection.SendTimeout = 5000;
            ReturnType = NetDuinoUtils.Utils.HelperClass.ReturnType.HTML;
        }
        public RequestEventArgs(Request command, Socket connection, NetDuinoUtils.Utils.HelperClass.ReturnType returntype)
        {
            Command = command.ReqEndPoint;
            Connection = connection;
            Connection.SendTimeout = 5000;
            ReturnType = returntype;
        }


        public EndPoint Command { get; set; }
        public string ReturnString { get; set; }
        public Socket Connection { get; set; }
        public NetDuinoUtils.Utils.HelperClass.ReturnType ReturnType { get; set; }
    }
}
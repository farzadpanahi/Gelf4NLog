using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Gelf4NLog.Target
{
    public class TcpTransportClient : ITransportClient
    {
        #region ITransportClient Members

        public void Send(byte[] bytes, int length, IPEndPoint ipEndPoint)
        {
            using (var tcpClient = new TcpClient(ipEndPoint.Address.ToString(), ipEndPoint.Port))
            {
                var stream = tcpClient.GetStream();
                stream.Write(bytes, 0, length);
                stream.Close();
            }
        }

        //public void Send(byte[] bytes, int length, IPEndPoint ipEndPoint)
        //{
        //    using (var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        //    {
        //        socket.Connect(ipEndPoint);
        //        socket.Send(bytes, 0, bytes.Length, SocketFlags.None);
        //    }
        //}

        #endregion
    }
}

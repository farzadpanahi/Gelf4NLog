using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Gelf4NLog.Target
{
    public class TcpTransport : ITransport
    {
        private readonly ITransportClient _transportClient;
        public TcpTransport(ITransportClient transportClient)
        {
            _transportClient = transportClient;
        }

        #region ITransport Members

        public void Send(string serverIpAddress, int serverPort, string message)
        {
            var ipAddress = IPAddress.Parse(serverIpAddress);
            var ipEndPoint = new IPEndPoint(ipAddress, serverPort);

            var bytes = CompressMessage(message);
            //var bytes = PrepareUncompressedMessage(message);
            //var bytes = CompressMessageZlib(message);

            _transportClient.Send(bytes, bytes.Length, ipEndPoint);
        }

        #endregion

        /// <summary>
        /// Compresses the given message using GZip algorithm
        /// </summary>
        /// <param name="message">Message to be compressed</param>
        /// <returns>Compressed message in bytes</returns>
        private static byte[] CompressMessage(String message)
        {
            var compressedMessageStream = new MemoryStream();
            using (var gzipStream = new GZipStream(compressedMessageStream, CompressionMode.Compress))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                gzipStream.Write(messageBytes, 0, messageBytes.Length);
            }

            return compressedMessageStream.ToArray();
        }

        private static byte[] CompressMessageZlib(String message)
        {
            var compressedMessageStream = new MemoryStream();
            using (var zlibStream = new Ionic.Zlib.ZlibStream(compressedMessageStream, Ionic.Zlib.CompressionMode.Compress))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                zlibStream.Write(messageBytes, 0, messageBytes.Length);
            }

            return compressedMessageStream.ToArray();
        }

        private byte[] PrepareUncompressedMessage(String message)
        {
            //var uncompressedHeader = new byte[2] { 0xff, 0xff };
            var bytes = Encoding.UTF8.GetBytes(message);
            return bytes;
            //return Combine(uncompressedHeader, bytes);
        }

        // Attribution: http://stackoverflow.com/a/415396/266659
        private byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using NLog;
using NLog.Targets;
using Newtonsoft.Json;
using System.Net.Sockets;
using System;

namespace Gelf4NLog.Target
{
    [Target("GrayLog")]
    public class NLogTarget : TargetWithLayout
    {
        [Required]
        public string HostIp { get; set; }

        [Required]
        public int HostPort { get; set; }

        public string Facility { get; set; }

        public ProtocolType Protocol { get; set; }

        public IConverter Converter { get; private set; }
        public ITransport Transport { get; private set; }

        public NLogTarget()
        {
            Protocol = ProtocolType.Udp;
            Converter = new GelfConverter();
        }

        public NLogTarget(ITransport transport, IConverter converter)
        {
            Transport = transport;
            Converter = converter;
        }

        public void WriteLogEventInfo(LogEventInfo logEvent)
        {
            Write(logEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            switch (Protocol)
            {
                case ProtocolType.Udp:
                    Transport = new UdpTransport(new UdpTransportClient());
                    break;
                case ProtocolType.Tcp:
                    Transport = new TcpTransport(new TcpTransportClient());
                    break;
                default:
                    throw new NLog.NLogConfigurationException(string.Format("Protocol '{0}' not supported.", Protocol));
            }

            var jsonObject = Converter.GetGelfJson(logEvent, Facility);
            if (jsonObject == null) return;
            Transport.Send(HostIp, HostPort, jsonObject.ToString(Formatting.None, null));
        }
    }
}

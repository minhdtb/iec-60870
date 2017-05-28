using IEC60870.Connections;
using IEC60870.Object;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace IEC60870.SAP
{
    public class ClientSAP
    {
        private readonly ConnectionSettings _settings = new ConnectionSettings();
        private Connection _connection;
        private readonly IPAddress _host;
        private readonly int _port;

        public ConnectionEventListener.NewASdu NewASdu { get; set; }
        public ConnectionEventListener.ConnectionClosed ConnectionClosed { get; set; }

        public ClientSAP(IPAddress host)
        {
            _host = host;
            _port = 2404;
        }

        public ClientSAP(IPAddress host, int port) : this(host)
        {
            _port = port;
        }

        public ClientSAP(string host, int port)
        {
            try
            {
                _host = IPAddress.Parse(host);
                _port = port;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }            
        }

        public void Connect()
        {            
            try
            {
                var remoteEp = new IPEndPoint(_host, _port);
                var socket = new Socket(_host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(remoteEp);

                _connection = new Connection(socket, _settings)
                {
                    NewASdu = NewASdu,
                    ConnectionClosed = ConnectionClosed
                };
                
                _connection.StartDataTransfer();
            }
            catch (Exception e)
            {
                throw new IOException(e.Message);
            }            
        }      

        public void SendASdu(ASdu asdu)
        {
            _connection?.Send(asdu);
        }

        public void SetMessageFragmentTimeout(int timeout)
        {
            if (timeout < 0)
            {
                throw new ArgumentException("Invalid message fragment timeout: " + timeout);
            }

            _settings.MessageFragmentTimeout = timeout;
        }

        public void SetCotFieldLength(int length)
        {
            if (length != 1 && length != 2)
            {
                throw new ArgumentException("Invalid COT length: " + length);
            }

            _settings.CotFieldLength = length;
        }

        public void SetCommonAddressFieldLength(int length)
        {
            if (length != 1 && length != 2)
            {
                throw new ArgumentException("Invalid CA length: " + length);
            }

            _settings.CommonAddressFieldLength = length;
        }

        public void SetIoaFieldLength(int length)
        {
            if (length < 1 || length > 3)
            {
                throw new ArgumentException("Invalid IOA length: " + length);
            }

            _settings.IoaFieldLength = length;
        }

        public void SetMaxTimeNoAckReceived(int time)
        {
            if (time < 1000 || time > 255000)
            {
                throw new ArgumentException("Invalid NoACK received timeout: " + time
                        + ", time must be between 1000ms and 255000ms");
            }

            _settings.MaxTimeNoAckReceived = time;
        }

        public void SetMaxTimeNoAckSent(int time)
        {
            if (time < 1000 || time > 255000)
            {
                throw new ArgumentException("Invalid NoACK sent timeout: " + time
                        + ", time must be between 1000ms and 255000ms");
            }

            _settings.MaxTimeNoAckSent = time;
        }

        public void SetMaxIdleTime(int time)
        {
            if (time < 1000 || time > 172800000)
            {
                throw new ArgumentException("Invalid idle timeout: " + time
                        + ", time must be between 1000ms and 172800000ms");
            }

            _settings.MaxIdleTime = time;
        }

        public void SetMaxUnconfirmedIPdusReceived(int maxNum)
        {
            if (maxNum < 1 || maxNum > 32767)
            {
                throw new ArgumentException("invalid maxNum: " + maxNum + ", must be a value between 1 and 32767");
            }

            _settings.MaxUnconfirmedIPdusReceived = maxNum;
        }
    }
}

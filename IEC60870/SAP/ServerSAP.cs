using IEC60870.Connections;
using IEC60870.Object;
using IEC60870.Util;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using PubSub;

namespace IEC60870.SAP
{
    class ConnectionHandler : ThreadBase
    {
        private Socket _socket;
        private ConnectionSettings _settings;
        private Connection serverConnection;
        public ConnectionHandler(Socket socket, ConnectionSettings settings): base()
        {
            _socket = socket;
            _settings = settings;

            this.Subscribe<ASdu>(asdu =>
            {
                try
                {
                    serverConnection.Send(asdu);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }               
            });
        }

        public override void Run()
        {
            serverConnection = new Connection(_socket, _settings);
            serverConnection.ConnectionClosed += (a) =>
            {
                Console.WriteLine(a);
            };

            serverConnection.WaitForStartDT(5000);
        }
    }

    class ServerThread : ThreadBase
    {
        private int _maxConnections;
        private ConnectionSettings _settings;
        private Socket _serverSocket;

        public ServerThread(Socket serverSocket, ConnectionSettings settings, int maxConnections) : base()
        {
            _maxConnections = maxConnections;
            _serverSocket = serverSocket;
            _settings = settings;
        }

        public override void Run()
        {
            try
            {
                Socket clientSocket = null;

                while(true)
                {
                    try
                    {
                        clientSocket = _serverSocket.Accept();
                        var handler = new ConnectionHandler(clientSocket, _settings);
                        handler.Start();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e);
                    }   
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        break;
                    }                
                }
            }
            catch(Exception)
            {
                Abort();
            }                     
        }
    }

    public class ServerSAP
    {
        private ConnectionSettings _settings = new ConnectionSettings();       
        private IPAddress _host;
        private int _port;
        private int _maxConnections = 10;

        public ServerSAP(IPAddress host)
        {
            _host = host;
            _port = 2404;
        }

        public ServerSAP(IPAddress host, int port) : this(host)
        {
            _port = port;
        }

        public ServerSAP(string host, int port)
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

        public void StartListen(int backlog)
        {
            var remoteEp = new IPEndPoint(_host, _port);
            var socket = new Socket(_host.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(remoteEp);
            socket.Listen(backlog);
            var serverThread = new ServerThread(socket, _settings, _maxConnections);
            serverThread.Start();
        }

        public void SendASdu(ASdu asdu)
        {
            this.Publish(asdu);
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

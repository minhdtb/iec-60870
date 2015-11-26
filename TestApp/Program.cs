using System;
using System.Net;
using System.Net.Sockets;
using IEC60870.Connection;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var socket = Connect("127.0.0.1", 2404);
                var settings = new ConnectionSettings();

                var connection = new Connection(socket, settings);

                connection.NewASdu += asdu => { Console.WriteLine("\nReceived ASDU:\n" + asdu.ToString()); };

                connection.ConnectionClosed += Console.WriteLine;

                connection.StartDataTransfer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

        private static Socket Connect(string host, int port)
        {
            var ipAddress = IPAddress.Parse(host);
            var remoteEp = new IPEndPoint(ipAddress, port);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEp);

            return socket;
        }
    }
}
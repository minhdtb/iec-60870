using IEC60870.Connection;
using System;
using System.Net;
using System.Net.Sockets;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = connect("localhost", 2404);
            ConnectionSettings settings = new ConnectionSettings();
            Connection connection = new Connection(socket, settings);
            connection.startDataTransfer();
        }

        static Socket connect(String host, int port)
        {
            IPAddress[] IPs = Dns.GetHostAddresses(host);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPs[0], port);
            return socket;
        }
    }
}

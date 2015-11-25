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
            Socket socket = connect("127.0.0.1", 2404);
            ConnectionSettings settings = new ConnectionSettings();
            try
            {
                Connection connection = new Connection(socket, settings);

                connection.newASdu += (asdu) =>
                {
                    Console.WriteLine("\nReceived ASDU:\n" + asdu.ToString());
                };

                connection.connectionClosed += (error) =>
                {
                    Console.WriteLine(error);
                };

                connection.startDataTransfer();
            }   
            catch (Exception e)
            {
                Console.WriteLine(e);
            }               
        }

        static Socket connect(String host, int port)
        {
            IPAddress ipAddress = IPAddress.Parse(host);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(remoteEP);

            return socket;
        }
    }
}

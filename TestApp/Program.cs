using System;
using IEC60870.SAP;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var client = new ClientSAP("127.0.0.1", 2404);
                client.NewASdu += asdu => {
                    Console.WriteLine(asdu);
                };

                client.Connect();                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }

    }
}
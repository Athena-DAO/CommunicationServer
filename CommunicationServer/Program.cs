using System;
using System.Threading;

namespace CommunicationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the Ip Address");
            var Ip = Console.ReadLine();
            Server server = new Server(Ip,6000);
            server.StartServer();

        }
    }   
}

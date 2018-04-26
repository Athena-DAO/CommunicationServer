using System;
using System.Threading;

namespace CommunicationServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server("192.168.1.2",6000);
            server.StartServer();

        }
    }   
}

using Microsoft.Extensions.Configuration;
using System.IO;

namespace CommunicationServer
{
    internal class Program
    {
        public static IConfiguration Configuration { get; set; }

        private static void Main(string[] args)
        {
            BuildConfiguration();
            Server server = new Server($"{Configuration["Ip"]}", int.Parse($"{Configuration["Port"]}"));
            server.StartServer();
        }

        public static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
    }
}
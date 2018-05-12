using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using Newtonsoft.Json;
using CommunicationServer.Model;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CommunicationServer
{
    class Server
    {
        private TcpListener tcpListener;
        private Dictionary<string, CommunicationBlock> Infrastructure;
        private Object addLock = new Object();
        public static IConfiguration Configuration { get; set; }
        public Server(string ip,int portNo)
        {
            BuildConfiguration();
            var Ip = ip.Split('.').Select(i => Convert.ToByte(i)).ToArray();
            Infrastructure = new Dictionary<string, CommunicationBlock>();

            try
            {
                tcpListener = new TcpListener(new IPAddress(Ip), portNo);
                tcpListener.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception {0}", e);
            }
        }
        private static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
        public void StartServer()
        {
            while (true)
            {
                Socket socket= tcpListener.AcceptSocket();
                var thread = new Thread( () => run(socket));
                thread.Start();
            }
        }


        public void run(Socket socket)
        {
            var module = new CommunicationModule
            {
                socket = socket,
                stream = new NetworkStream(socket)
            };
            var t = module.ReceiveData();
            CommunciationRequest communciationParameters = JsonConvert.DeserializeObject<CommunciationRequest>(t);

            lock (addLock)
            {
                if (!Infrastructure.ContainsKey(communciationParameters.PipelineId))
                {
                    Infrastructure[communciationParameters.PipelineId] = new CommunicationBlock();
                }

                if (communciationParameters.IsMaster)
                    Infrastructure[communciationParameters.PipelineId].Master.Add(module);
                else
                    Infrastructure[communciationParameters.PipelineId].Slaves.Add(module);
            }

            SendIpData(communciationParameters.PipelineId);
        }



        private void SendIpData(string pipelineId)
        {
            while(Infrastructure[pipelineId].Master.Count > Infrastructure[pipelineId].HolePunched && Infrastructure[pipelineId].Slaves.Count > Infrastructure[pipelineId].HolePunched)
            {
                int holePunched;
                lock (addLock)
                {
                    holePunched = Infrastructure[pipelineId].HolePunched++;
                }
                // Infrastructure[pipelineId].HolePunched;
                Infrastructure[pipelineId].Slaves[holePunched].SendData(BuildCommunicationResponseJson(Infrastructure[pipelineId].Master[holePunched].socket.RemoteEndPoint, holePunched));
                Infrastructure[pipelineId].Master[holePunched].SendData(BuildCommunicationResponseJson(Infrastructure[pipelineId].Slaves[holePunched].socket.RemoteEndPoint, holePunched));
 
            }
        }


        private string BuildCommunicationResponseJson(EndPoint endPoint , int queueNumber)
        {
            return JsonConvert.SerializeObject(new CommunicationResponse() {
                EndPoint = endPoint.ToString(),
                QueueNumber =queueNumber,
                P2P = bool.Parse($"{Configuration["P2P"]}")
            });
        }
    }
}

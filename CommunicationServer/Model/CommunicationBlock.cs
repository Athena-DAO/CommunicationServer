using System;
using System.Collections.Generic;
using System.Text;
using CommunicationServer;
namespace CommunicationServer.Model
{
    class CommunicationBlock
    {
        public int HolePunched { get; set; }
        public List<CommunicationModule> Master { get; set; }
        public List<CommunicationModule> Slaves { get; set; }

        public CommunicationBlock()
        {
            HolePunched = 0;
            Master = new List<CommunicationModule>();
            Slaves = new List<CommunicationModule>();
        }

    }
}

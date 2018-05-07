using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationServer.Model
{
    class CommunciationRequest
    {
        public string PipelineId { get; set; }
        public bool IsMaster { get; set; }

    }
}

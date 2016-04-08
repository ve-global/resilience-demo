using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResilienceDemo.Client.Components
{
    public class ServerEvent
    {
        public ServerEvent(ServerEventStatus status)
        {
            Status = status;
        }

        public ServerEventStatus Status { get; private set; }
    }

    public enum ServerEventStatus
    {
        Started,
        Completed,
        Failed,
        Retried
    }
}

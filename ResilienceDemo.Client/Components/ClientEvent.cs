using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResilienceDemo.Client.Components
{
    public class ClientEvent
    {
        public ClientEvent(ClientEventStatus status)
        {
            Status = status;
        }

        public ClientEventStatus Status { get; private set; }
    }

    public enum ClientEventStatus
    {
        Started,
        Completed,
        Failed
    }
}

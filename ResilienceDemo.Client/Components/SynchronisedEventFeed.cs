using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResilienceDemo.Client.Components
{
    public class SynchronisedEventFeed
    {
        private IObserver<ClientEvent> _clientFeed;
        private IObserver<ServerEvent> _serverFeed;

        private static object _syncRoot = new object();

        public SynchronisedEventFeed(
            IObserver<ClientEvent> clientFeed, 
            IObserver<ServerEvent> serverFeed)
        {
            _clientFeed = clientFeed;
            _serverFeed = serverFeed;
        }

        public void Write(ClientEventStatus status)
        {
            lock (_syncRoot)
            {
                _clientFeed.OnNext(new ClientEvent(status));
            }
        }

        public void Write(ServerEventStatus status)
        {
            lock (_syncRoot)
            {
                _serverFeed.OnNext(new ServerEvent(status));
            }
        }
    }
}

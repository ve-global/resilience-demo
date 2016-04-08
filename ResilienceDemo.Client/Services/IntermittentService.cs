
using ResilienceDemo.Client.Components;
using System;
using System.Threading;

namespace ResilienceDemo.Services
{
    public class IntermittentService : ITestService
    {
        private SynchronisedEventFeed _eventFeed;

        // NOTE: Was originally using timer events to toggle,
        // but these rely on the ThreadPool which is under a lot of load.
        private int _startTicks = Environment.TickCount;
        private int _windowSize = 4000;

        public IntermittentService(SynchronisedEventFeed eventFeed)
        {
            _eventFeed = eventFeed;
        }

        private bool Active
        {
            get
            {
                var dif = Environment.TickCount - _startTicks;
                var part = dif / _windowSize;
                var mod = part % 2;

                return mod == 0;
            }
        }

        public void Execute()
        {
            // Sleep to indicate connection time
            Thread.Sleep(10);

            // This logs that a connection was made to this service (Cheap)
            _eventFeed.Write(ServerEventStatus.Started);

            if (!Active)
            {
                _eventFeed.Write(ServerEventStatus.Failed);
                throw new ServiceUnavailableException();
            }

            // Sleep to indicate execution time
            Thread.Sleep(45);

            // This logs that the connection made to the service triggered an execution (Expensive)
            _eventFeed.Write(ServerEventStatus.Completed);
        }
    }
}

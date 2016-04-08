using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace ResilienceDemo.Client.Components
{
    public class Spammer
    {
        private IDisposable _scheduler;

        private SynchronisedEventFeed _eventFeed;

        private static object _syncRoot = new object();

        public Spammer(SynchronisedEventFeed eventFeed)
        {
            _eventFeed = eventFeed;
        }

        public void ExecuteApi(Action action, int callsPerSecond, int secondsDuration)
        {
            // For each second in the seconds duration
            for (int i = 0; i < secondsDuration; i++)
            {
                var baseTime = TimeSpan.FromSeconds(i);
                var subTime = baseTime;
                var increment = TimeSpan.FromMilliseconds(1000f / (float)callsPerSecond);
                
                // For each item in the calls per second
                for (int y = 0; y < callsPerSecond; y++)
                {
                    subTime += increment;

                    // Schedule the action into the event stream
                    _scheduler = Scheduler.Default.ScheduleAsync(subTime, (s, c) => Execute(action));
                }
            }
        }

        private Task Execute(Action action)
        {
            return Task.Run(() =>
            {
                _eventFeed.Write(ClientEventStatus.Started);

                try
                {
                    action();
                    _eventFeed.Write(ClientEventStatus.Completed);
                }
                catch
                {
                    _eventFeed.Write(ClientEventStatus.Failed);
                }
            });
        }
    }
}

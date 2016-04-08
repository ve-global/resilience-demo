using Polly;
using Polly.CircuitBreaker;
using ResilienceDemo.Client.Components;
using System;

namespace ResilienceDemo.Services
{
    public class RetryService : ITestService
    {
        private ITestService _next;
        private ContextualPolicy _policy;

        public RetryService(ITestService next, SynchronisedEventFeed eventFeed)
        {
            _next = next;

            _policy = Policy
                .Handle<ServiceUnavailableException>()
                .Or<BrokenCircuitException>()
                .WaitAndRetry(5,
                    retryAttempt => TimeSpan.FromSeconds(1),
                    (exception, timeSpan, context) =>
                    {
                        eventFeed.Write(ServerEventStatus.Retried);
                    });
        }

        public void Execute()
        {
            _policy.Execute(() => _next.Execute());
        }
    }
}

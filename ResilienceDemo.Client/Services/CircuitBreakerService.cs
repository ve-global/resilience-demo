using Polly;
using System;

namespace ResilienceDemo.Services
{
    public class CircuitBreakerService : ITestService
    {
        private ITestService _next;
        private Policy _policy;


        public CircuitBreakerService(ITestService next)
        {
            _next = next;

            _policy = Policy
                .Handle<ServiceUnavailableException>()
                .CircuitBreaker(2, TimeSpan.FromSeconds(0.5));
        }


        public void Execute()
        {
            _policy
                .Execute(() => _next.Execute());
        }
    }
}

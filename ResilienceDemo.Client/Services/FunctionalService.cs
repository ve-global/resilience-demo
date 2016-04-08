using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResilienceDemo.Services
{
    public class FunctionalService
    {
        public Action Bootstrap()
        {
            return new Action(
                () => RetryService(
                    () => CircuitBreakerService(
                        () => IntermittentService())));
        }

        private void IntermittentService()
        {
            // TODO: Implement intermittent behavior.
        }

        private void RetryService(Action next)
        {
            // TODO: Implement retry policy
            next();
        }

        private void CircuitBreakerService(Action next)
        {
            // TODO: Implement circuit breaker policy
            next();
        }
    }
}
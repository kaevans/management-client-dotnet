using Microsoft.Rest.Azure;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementClient.Helpers
{
    class Polly429RetryPolicy
    {
        internal static Policy SetRetryPolicy()
        {
            string retryAfterHeader = "Retry-After";
            string contextKey = "RetryAfter";

            return Policy.Handle<CloudException>(ce => ce.Response.Headers.ContainsKey(retryAfterHeader))
                        .Or<AggregateException>(ae => ((ae.InnerException as CloudException).Response.Headers.ContainsKey(retryAfterHeader)))
                        .WaitAndRetryAsync(
                            retryCount: 3,
                            sleepDurationProvider: (retryAttempt, context) =>
                            {
                                var retryAfter = TimeSpan.FromMilliseconds(10); // Should be 0? 
                                if (context == null || !context.ContainsKey(contextKey)) return retryAfter;
                                return TimeSpan.FromSeconds(double.Parse(context[contextKey].ToString()));
                            },
                            onRetryAsync: (exception, timespan, retryAttempt, context) =>
                            {
                                context[contextKey] = (exception as CloudException)?.Response.Headers[retryAfterHeader].First()
                                                        ?? (exception.InnerException as CloudException)?.Response.Headers[retryAfterHeader].First()
                                                        ?? "1"; //Just an arbitrary chosen value, never seems to be used
                                return Task.CompletedTask;
                            }
                        );
        }
    }
}

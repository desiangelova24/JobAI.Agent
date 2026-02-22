using Polly;

namespace JobAI.Core.Helpers
{
    public static class ResilienceHelper
    {
        public static void ExecuteWithRetry(Action action)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                [
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(30)
                ], (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"⚠️ Connection failed. Retry {retryCount} in {timeSpan.Seconds}s...");
                });

            retryPolicy.Execute(action);
        }
    }
}

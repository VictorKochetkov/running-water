using System;
using System.Threading.Tasks;
using RunningWater.Interfaces;

namespace RunningWater.Sources
{
    public class RetryHandlerService : IRetryHandlerService
    {
        /// <inheritdoc/>
        public Task ExecuteAsync(Func<Task> action, RetryPolicy policy = RetryPolicy.Infinite, TimeSpan? delay = null)
            => ExecuteAsync(async () =>
            {
                await action();
                return true;
            }, policy, delay);

        /// <inheritdoc/>
        public async Task<TValue> ExecuteAsync<TValue>(Func<Task<TValue>> action, RetryPolicy policy = RetryPolicy.Infinite, TimeSpan? delay = null)
        {
            while (true)
            {
                try
                {
                    return await action();
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception);
                    await Task.Delay(delay ?? TimeSpan.FromSeconds(3));
                }
            }
        }
    }
}

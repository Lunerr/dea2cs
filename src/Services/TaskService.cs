using Discord;
using DEA.Entities.Service;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class TaskService : Service
    {
        private readonly LoggingService _logger;

        public TaskService(LoggingService logger)
        {
            _logger = logger;
        }

        public Task TryRun(Func<Task> task)
        {
            Task.Run(async () =>
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, ex.ToString());
                }
            });

            return Task.CompletedTask;
        }
    }
}

using DEA.Common;
using DEA.Entities.Event;
using DEA.Services;
using DEA.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DEA.Events
{
    public sealed class Ready : Event
    {
        private readonly IServiceProvider _provider;
        private readonly LoggingService _logger;

        public Ready(IServiceProvider provider) : base(provider)
        {
            _provider = provider;
            _logger = provider.GetRequiredService<LoggingService>();

            _client.Ready += OnReadyAsync;
        }

        private Task OnReadyAsync()
            => _taskService.TryRun(async () =>
            {
                Loader.LoadTimers(_provider);

                await _client.SetGameAsync(Config.GAME);
            });
    }
}

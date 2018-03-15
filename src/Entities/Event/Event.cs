using Discord.WebSocket;
using DEA.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DEA.Entities.Event
{
    public abstract class Event
    {
        protected readonly DiscordSocketClient _client;
        protected readonly TaskService _taskService;

        public Event(IServiceProvider provider)
        {
            _client = provider.GetRequiredService<DiscordSocketClient>();
            _taskService = provider.GetRequiredService<TaskService>();
        }
    }
}

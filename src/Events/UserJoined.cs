using Discord;
using DEA.Common;
using DEA.Database.Models;
using DEA.Entities.Event;
using DEA.Extensions.Database;
using DEA.Extensions.Discord;
using DEA.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DEA.Events
{
    public sealed class UserJoined : Event
    {
        private readonly LoggingService _logger;
        private readonly SendingService _sender;
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;

        public UserJoined(IServiceProvider provider) : base(provider)
        {
            _logger = provider.GetRequiredService<LoggingService>();
            _sender = provider.GetRequiredService<SendingService>();
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();
            _dbMutes = provider.GetRequiredService<IMongoCollection<Mute>>();

            _client.UserJoined += OnUserJoinedAsync;
        }

        private Task OnUserJoinedAsync(IGuildUser guildUser)
            => _taskService.TryRun(async () =>
            {
                await _sender.TryDMAsync(guildUser, Config.HELP_MESSAGE, "Welcome to DEA", guild: guildUser.Guild);

                var dbGuild = await _dbGuilds.GetGuildAsync(guildUser.Guild.Id);

                if (!dbGuild.MutedRoleId.HasValue || !await _dbMutes.AnyMuteAsync(guildUser.Id, guildUser.GuildId))
                    return;

                var mutedRole = guildUser.Guild.GetRole(dbGuild.MutedRoleId.Value);

                if (mutedRole == null || !await mutedRole.CanUseAsync())
                    return;

                await guildUser.AddRoleAsync(mutedRole);
            });
    }
}

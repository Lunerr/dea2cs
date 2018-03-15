using DEA.Common;
using DEA.Database.Models;
using DEA.Entities.DEATimer;
using DEA.Extensions.Database;
using DEA.Extensions.Discord;
using DEA.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DEA.Timers
{
    public sealed class AutoUnmute : DEATimer
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly ModerationService _moderationService;

        public AutoUnmute(IServiceProvider provider) : base(provider, Config.AUTO_UNMUTE_TIMER)
        {
            _dbGuilds = provider.GetRequiredService<IMongoCollection<Guild>>();
            _dbMutes = provider.GetRequiredService<IMongoCollection<Mute>>();
            _moderationService = provider.GetRequiredService<ModerationService>();
        }

        protected override async Task Execute()
        {
            foreach (var guild in _client.Guilds)
            {
                var dbGuild = await _dbGuilds.GetGuildAsync(guild.Id);

                if (!dbGuild.MutedRoleId.HasValue)
                    continue;

                var mutedRole = guild.GetRole(dbGuild.MutedRoleId.Value);

                if (mutedRole == null || !await mutedRole.CanUseAsync())
                    continue;

                var mutes = await _dbMutes.WhereAsync(x => x.Active);

                foreach (var mute in mutes)
                {
                    if (mute.Timestamp.Add(mute.Length).CompareTo(DateTimeOffset.UtcNow) > 0)
                        continue;

                    await _dbMutes.UpdateManyAsync(x => x.UserId == mute.UserId && x.GuildId == mute.GuildId,
                        new UpdateDefinitionBuilder<Mute>().Set(x => x.Active, false));

                    var guildUser = guild.GetUser(mute.UserId);

                    if (guildUser == null)
                        continue;

                    await guildUser.RemoveRoleAsync(mutedRole);
                    await _moderationService.LogAutoUnmuteAsync(guild, guildUser);
                }
            }
        }
    }
}

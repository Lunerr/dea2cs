using DEA.Common;
using DEA.Database.Models;
using DEA.Entities.Service;
using DEA.Extensions.Database;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class ChatService : Service
    {
        private readonly ConcurrentDictionary<ulong, DateTimeOffset> _cooldowns = new ConcurrentDictionary<ulong, DateTimeOffset>();
        private readonly IMongoCollection<User> _dbUsers;

        public ChatService(IMongoCollection<User> dbUsers)
        {
            _dbUsers = dbUsers;
        }

        public Task ApplyAsync(Context ctx)
        {
            if (ctx.Guild == null)
                return Task.CompletedTask;
            else if (ctx.GuildUser.RoleIds.Any(x => x == ctx.DbGuild.MutedRoleId))
                return Task.CompletedTask;
            else if (ctx.Message.Content.Length < Config.MIN_CHAT_LENGTH)
                return Task.CompletedTask;

            if (!_cooldowns.TryGetValue(ctx.User.Id, out DateTimeOffset endsAt))
            {
                _cooldowns.TryAdd(ctx.User.Id, GetEndsAt());
            }
            else if (endsAt.CompareTo(DateTimeOffset.UtcNow) > 0)
            {
                return Task.CompletedTask;
            }
            else
            {
                _cooldowns.TryUpdate(ctx.User.Id, GetEndsAt(), endsAt);
            }

            return _dbUsers.ModifyCash(ctx, Config.CHAT_REWARD);
        }

        private static DateTimeOffset GetEndsAt()
            => DateTimeOffset.UtcNow.Add(Config.CHAT_SERVICE_DELAY);
    }
}

using DEA.Common;
using DEA.Database.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace DEA.Extensions.Database
{
    public static class UserCollectionExtensions
    {
        private static UpdateDefinition<User> GetFactory(ulong userId, ulong guildId)
            => new UpdateDefinitionBuilder<User>()
            .SetOnInsert(x => x.UserId, userId)
            .SetOnInsert(x => x.GuildId, guildId);

        public static Task<User> GetUserAsync(this IMongoCollection<User> collection, ulong userId, ulong guildId)
            => collection.GetAsync(x => x.UserId == userId && x.GuildId == guildId, GetFactory(userId, guildId));

        public static Task UpsertUserAsync(this IMongoCollection<User> collection, ulong userId, ulong guildId, Action<User> update)
            => collection.UpsertAsync(x => x.UserId == userId && x.GuildId == guildId, update, GetFactory(userId, guildId));

        public static Task ModifyCash(this IMongoCollection<User> collection, Context context, decimal change)
            => collection.UpsertUserAsync(context.User.Id, context.Guild.Id, x => x.Cash += change);

        public static async Task ModifyInventoryAsync(this IMongoCollection<User> collection, User DbUser, string item, int count = 1)
        {
            if (DbUser.Inventory.Contains(item))
            {
                await collection.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory[item] = count + x.Inventory[item].AsInt32);
            }
            else
            {
                await collection.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory.Add(item, count));
            }

            if (DbUser.Inventory[item] <= 0)
            {
                await collection.UpsertUserAsync(DbUser.UserId, DbUser.GuildId, x => x.Inventory.Remove(item));
            }
        }
    }
}

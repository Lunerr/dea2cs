using Discord;
using DEA.Database.Models;
using DEA.Entities.Service;
using DEA.Extensions.Database;
using DEA.Extensions.Discord;
using DEA.Extensions.System;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class LeaderboardService : Service
    {
        private readonly IMongoCollection<User> _dbUsers;

        public LeaderboardService(IMongoCollection<User> dbUsers)
        {
            _dbUsers = dbUsers;
        }
        
        public async Task<string> GetUserLbAsync<TKey>(IGuild guild, Func<User, TKey> keySelector, int quantity, bool ascending = false)
        {
            var dbGuildUsers = await _dbUsers.WhereAsync(x => x.GuildId == guild.Id);
            var ordered = ascending ? dbGuildUsers.OrderBy(keySelector) : dbGuildUsers.OrderByDescending(keySelector);
            var orderedArr = ordered.ToArray();
            var descBuilder = new StringBuilder();
            var pos = 0;

            for (int i = 0; i < orderedArr.Length; i++)
            {
                var user = await guild.GetUserAsync(orderedArr[i].UserId);

                if (user != null)
                    descBuilder.AppendFormat("{0}. {1}: ${2}\n", ++pos, user.Bold(), orderedArr[i].Cash.ToString("F2"));

                if (pos == quantity)
                    break;
            }

            return descBuilder.ToString();
        }
    }
}

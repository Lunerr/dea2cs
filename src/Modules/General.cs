using Discord;
using Discord.Commands;
using DEA.Common;
using DEA.Database.Models;
using DEA.Extensions.Database;
using DEA.Extensions.Discord;
using DEA.Preconditions.Command;
using DEA.Preconditions.Parameter;
using DEA.Services;
using MongoDB.Driver;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DEA.Extensions.System;
using DEA.Entities.Item;

namespace DEA.Modules
{
    [Name("General")]
    [Summary("The best memes in town start with these commands.")]
    [NotMuted]
    public sealed class General : ModuleBase<Context>
    {
        private IMongoCollection<User> _dbUsers;

        public General(IMongoCollection<User> dbUsers)
        {
            _dbUsers = dbUsers;
        }

        [Command("Rank")]
        [Alias("bal", "cash", "balance")]
        [Summary("Check anyones rank.")]
        public async Task RankAsync(
            [Summary("hornypenis#0083")] IUser user = null)
        {
            user = user ?? Context.User;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _dbUsers.GetUserAsync(user.Id, Context.Guild.Id);
            await Context.SendAsync($"**Balance:** {dbUser.Cash.USD()}\n**Health:** {dbUser.Health}", $"{user}'s Rank");
        }

        [Command("Inventory")]
        [Alias("inv")]
        [Summary("Check out anyones inventory.")]
        public async Task InventoryAsync(
            [Summary("johngaeh#8112")] IUser user = null)
        {
            user = user ?? Context.User;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _dbUsers.GetUserAsync(user.Id, Context.Guild.Id);

            if (dbUser.Inventory.ElementCount == 0)
            {
                await Context.ReplyErrorAsync((user.Id == Context.User.Id ? "You have nothing in your" : user + " has nothing in their") + " inventory.");
            }
            else
            {
                var description = string.Empty;

                foreach (var item in dbUser.Inventory.Elements)
                {
                    var s = item.Value.AsInt32 == 1 ? string.Empty : "s";

                    description += $"{item.Value} {item.Name}{s}\n";
                }
                await Context.SendAsync(description, $"Inventory of {user}");
            }
        }
    }
}

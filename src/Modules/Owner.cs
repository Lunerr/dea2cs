using Discord;
using Discord.Commands;
using DEA.Common;
using DEA.Database.Models;
using DEA.Extensions.Database;
using DEA.Preconditions.Command;
using DEA.Services;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using DEA.Entities.Item;
using DEA.Extensions.Discord;

namespace DEA.Modules
{
    [Name("Owner")]
    [Summary("Commands reserved for the guild owner.")]
    public sealed class Owner : ModuleBase<Context>
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<User> _dbUsers;

        public Owner(IMongoCollection<Guild> dbGuilds, IMongoCollection<User> dbUsers)
        {
            _dbGuilds = dbGuilds;
            _dbUsers = dbUsers;
        }

        [Command("SetLogChannel")]
        [Alias("setlogs", "setmodlog", "setmodlogs")]
        [Summary("Sets the log channel.")]
        public async Task SetLogChannelAsync(
            [Summary("OldManJenkins")] [Remainder] ITextChannel logChannel)
        {
            await _dbGuilds.UpsertGuildAsync(Context.Guild.Id, x => x.LogChannelId = logChannel.Id);
            await Context.ReplyAsync($"You have successfully set to log channel to {logChannel.Mention}.");
        }

        [Command("ToggleAutoMute")]
        [Alias("disableautomute", "enableautomute")]
        [Summary("Toggles the automatic mute setting.")]
        public async Task ToggleAutoMuteAsync()
        {
            await _dbGuilds.UpsertGuildAsync(Context.Guild.Id, x => x.AutoMute = !x.AutoMute);
            await Context.ReplyAsync($"You have successfully toggled the automatic mute setting.");
        }

        [Command("SetMutedRole")]
        [Alias("setmuted", "setmuterole", "setmute")]
        [Summary("Sets the muted role.")]
        public async Task SetMutedRoleAsync(
            [Summary("BarnacleBoy")] [Remainder] IRole mutedRole)
        {
            await _dbGuilds.UpsertGuildAsync(Context.Guild.Id, x => x.MutedRoleId = mutedRole.Id);
            await Context.ReplyAsync($"You have successfully set to muted role to {mutedRole.Mention}.");
        }

        [Command("ModifyInventory")]
        [Alias("ModifyInv")]
        [Summary("Modify a user's inventory.")]
        public async Task ModifyInventory([Summary("32")] int quantity, [Summary("Huntman knife")] Item item, [Summary("daddy#4200")] IUser user = null)
        {
            user = user ?? Context.User;
            var dbUser = user.Id == Context.User.Id ? Context.DbUser : await _dbUsers.GetUserAsync(user.Id, Context.Guild.Id);

            await _dbUsers.ModifyInventoryAsync(dbUser, item.Name, quantity);
            await ReplyAsync($"You have successfully modified {user.Bold()}'s inventory.");
        }
    }
}

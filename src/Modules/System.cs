using Discord;
using Discord.Commands;
using DEA.Common;
using DEA.Database.Models;
using DEA.Extensions.Discord;
using DEA.Extensions.System;
using DEA.Preconditions.Parameter;
using DEA.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEA.Modules
{
    // TODO: make less memes of the command examples!
    [Name("System")]
    [Summary("System commands directly tied with DEA's functionality.")]
    public sealed class System : ModuleBase<Context>
    {
        private readonly CommandService _commandService;
        private readonly ListService _systemService;
        private readonly CooldownService _cooldownService;
        private readonly LeaderboardService _lbService;

        public System(CommandService commandService, ListService systemService, CooldownService cooldownService, LeaderboardService lbService)
        {
            _commandService = commandService;
            _systemService = systemService;
            _cooldownService = cooldownService;
            _lbService = lbService;
        }

        [Command("Help")]
        [Alias("information", "info")]
        [Summary("Information about the bot.")]
        public async Task Help()
        {
            await Context.DmAsync(Config.HELP_MESSAGE, "DEA Information");

            if (!(Context.Channel is IDMChannel))
                await Context.ReplyAsync("You have been DMed with bot information.");
        }

        [Command("Modules")]
        [Alias("category", "categories")]
        [Summary("List of all the command modules.")]
        public Task ModulesAsync()
            => Context.SendAsync(_systemService.ListModules(_commandService.Modules), "Modules");

        [Command("Commands")]
        [Alias("cmds", "cmdlist", "commandlist")]
        [Summary("List of all the commands.")]
        public async Task CommandsAsync()
        {
            await Context.DmAsync(_systemService.ListCommands(_commandService.Commands), "Commands");

            if (!(Context.Channel is IDMChannel))
                await Context.ReplyAsync("You have been DMed with a list of all commands.");
        }

        [Command("Module")]
        [Alias("modulelist")]
        [Summary("List of all commands of a specific module.")]
        public Task ModuleAsync(
            [Summary("general")] string name)
        {
            var module = _commandService.Modules.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (module == default(ModuleInfo))
                return Context.ReplyErrorAsync("This module does not exist.");
            else
                return Context.SendAsync(_systemService.ListCommands(module.Commands), $"{module.Name}'s Commands");
        }

        [Command("Command")]
        [Alias("commandinfo", "cmd", "cmdinfo")]
        [Summary("Information about a specific command.")]
        public Task HelpAsync(
            [Summary("rep")] string name)
        {
            var cmd = _commandService.Commands.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                    x.Aliases.Any(y => y.Equals(name, StringComparison.OrdinalIgnoreCase)));

            if (cmd == default(CommandInfo))
                return Context.ReplyErrorAsync("This command does not exist.");
            else
                return Context.SendAsync($"**Description:** {cmd.Summary}\n" +
                    $"**Usage:** `{Config.PREFIX}{cmd.GetUsage()}`\n" +
                    $"**Example:** `{Config.PREFIX}{cmd.GetExample()}`", cmd.Name);
        }

        [Command("Cooldowns")]
        [Alias("cd", "cooldown", "cds")]
        [Summary("View anyone's command cooldowns.")]
        public async Task CooldownsAsync(
            [Summary("jimbo#8237")] [Remainder] IUser user = null)
        {
            user = user ?? Context.User;
            var cooldowns = await _cooldownService.GetAllCooldownsAsync(user.Id, Context.Guild.Id);

            if (cooldowns.Count() == 0)
            {
                var response = user.Id == Context.User.Id ?
                    $"{user.Bold()}, All your commands are available for use." :
                    $"All of {user.Bold()}'s commands are available for use.";

                await Context.SendAsync(response);
            }
            else
            {
                var descBuilder = new StringBuilder();
                
                foreach (var cd in cooldowns)
                    descBuilder.AppendFormat("{0}: {1}\n", cd.Command.Name.Bold(), cd.EndsAt.Subtract(DateTimeOffset.UtcNow).ToString(@"hh\:mm\:ss"));

                await Context.SendAsync(descBuilder.ToString(), $"{user}'s Cooldowns");
            }
        }
    }
}

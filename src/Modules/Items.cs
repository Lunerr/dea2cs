using Discord.Commands;
using DEA.Common;
using DEA.Preconditions.Command;
using System.Threading.Tasks;
using DEA.Entities.Item;
using System;
using DEA.Services.Static;
using DEA.Extensions.System;
using DEA.Preconditions.Parameter;
using System.Collections.Generic;
using DEA.Services;

namespace DEA.Modules
{
    [Name("Items")]
    [Summary("These commands consist with items.")]
    [NotMuted]
    public sealed class Items : ModuleBase<Context>
    {
        public readonly GameService _gameService;

        public Items(GameService gameService)
        {
            _gameService = gameService;
        }

        [Command("Item")]
        [Alias("bal", "cash", "balance")]
        [Summary("Check anyones rank.")]
        public async Task ItemAsync(
            [Summary("hornypenis#0083")] Item item)
        {
            var message = $"**Description:** {item.Description}\n";

            foreach (var property in item.GetType().GetProperties())
            {
                var name = property.Name.SplitCamelCase();

                if (name == "Description" || name == "Name")
                {
                    continue;
                }

                var value = property.GetValue(item);

                if (value is decimal newValue)
                {
                    message += $"**{name}:** {newValue.USD()}\n";
                }
                else if (name == "Item Odds")
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / 100).ToString("P")}\n";
                }
                else if (name == "Crate Odds")
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.CrateItemOdds).ToString("P")}\n";
                }
                else if (name == "Acquire Odds" && property.PropertyType == typeof(Fish))
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.FishAcquireOdds).ToString("P")}\n";
                }
                else if (name == "Acquire Odds" && property.PropertyType == typeof(Meat))
                {
                    message += $"**{name}:** {(Convert.ToSingle(value) / Data.MeatAcquireOdds).ToString("P")}\n";
                }
                else
                {
                    message += $"**{name}:** {value.ToString()}\n";
                }
            }

            await Context.SendAsync(message, item.Name);
        }


        [Command("OpenAll")]
        [Remarks("Gold Crate")]
        [Cooldown(Config.OPENALL_CD)]
        [Summary("Open a crate!")]
        public async Task OpenAll([Own] [Remainder] Crate crate)
        {
            var quantity = Context.DbUser.Inventory[crate.Name].AsInt32;

            if (quantity > Config.MAX_CRATE_OPEN)
            {
                await Context.ReplyErrorAsync($"You may not open more than {Config.MAX_CRATE_OPEN.ToString("N0")} crates.");
            }
            else
            {
                IReadOnlyDictionary<string, int> items = await _gameService.MassOpenCratesAsync(crate, quantity, Context.DbUser);

                var reply = string.Empty;
                foreach (var item in items)
                {
                    reply += $"**{item.Key}:** {item.Value}\n";
                }

                await Context.SendAsync(reply, $"Items {Context.User} has won");
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DEA.Common;
using DEA.Entities.Item;

namespace DEA.Preconditions.Parameter
{
    public sealed class Own : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo param, object value,
            IServiceProvider services)
        {
            var context = ctx as Context;
            var item = value as Item;

            if (context.DbUser.Inventory.Elements.Any(x => x.Name == item.Name))
                return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError($"You do not own the following item: {item.Name}."));
        }
    }
}

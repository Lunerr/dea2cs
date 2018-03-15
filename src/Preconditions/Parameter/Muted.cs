using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DEA.Common;

namespace DEA.Preconditions.Parameter
{
    public sealed class Muted : ParameterPreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, ParameterInfo param, object value,
            IServiceProvider services)
        {
            var context = ctx as Context;

            if (value is IGuildUser guildUser && !guildUser.RoleIds.Any(x => x == context.DbGuild.MutedRoleId))
                return Task.FromResult(PreconditionResult.FromError("This command may only be used on a muted user."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}

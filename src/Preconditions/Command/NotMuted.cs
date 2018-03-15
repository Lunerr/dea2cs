using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Common;

namespace DEA.Preconditions.Command
{
    public sealed class NotMutedAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            var context = ctx as Context;

            if (context.GuildUser.RoleIds.Any(x => x == context.DbGuild.MutedRoleId))
            {
                return Task.FromResult(PreconditionResult.FromError("You may not use this command while muted."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}

using Discord.Commands;
using DEA.Common;
using System;
using System.Threading.Tasks;

namespace DEA.Preconditions.Command
{
    public sealed class SetMutedRoleAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext ctx, CommandInfo command, IServiceProvider services)
        {
            var context = ctx as Context;

            if (!context.DbGuild.MutedRoleId.HasValue)
                return Task.FromResult(PreconditionResult.FromError(("The muted role has not been set.")));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}

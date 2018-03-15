using Discord;
using DEA.Extensions.System;

namespace DEA.Extensions.Discord
{
    public static class IEntityExtensions
    {
        public static string Bold(this IEntity<ulong> entity)
            => entity.ToString().Bold();
    }
}

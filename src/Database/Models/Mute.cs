using System;

namespace DEA.Database.Models
{
    public sealed class Mute : Entity
    {
        public Mute(ulong guildId, ulong userId, TimeSpan length)
        {
            GuildId = guildId;
            UserId = userId;
            Length = length;
        }

        public ulong UserId { get; set; }
        public TimeSpan Length { get; set; }
        public bool Active { get; set; } = true;
    }
}

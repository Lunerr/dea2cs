using Discord;
using System;

namespace DEA.Entities.Spam
{
    public sealed class SpamEntry
    {
        public SpamEntry(IUserMessage message)
        {
            FirstTimestamp = message.Timestamp;
        }

        public int Count { get; set; } = 1;
        public DateTimeOffset FirstTimestamp { get; }
    }
}

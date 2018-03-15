using System.Collections.Generic;

namespace DEA.Common
{
    public sealed class Credentials
    {
        public string Token { get; set; }
        public ulong GuildId { get; set; }
        public string DbName { get; set; }
        public string DbConnectionString { get; set; }
        public IReadOnlyList<ulong> OwnerIds { get; set; }
    }
}

namespace DEA.Database.Models
{
    public sealed class Guild : Entity
    {
        public ulong? MutedRoleId { get; set; }
        public ulong? LogChannelId { get; set; }
        public uint CaseCount { get; set; }
        public bool AutoMute { get; set; }
    }
}

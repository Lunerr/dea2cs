using DEA.Entities.ModAction;
using MongoDB.Bson;

namespace DEA.Database.Models
{
    public sealed class ModAction : Entity
    {
        public ulong UserId { get; set; }
        public uint LogCase { get; set; }
        public ModActionType Type { get; set; }
        public BsonDocument Data { get; set; }
    }
}

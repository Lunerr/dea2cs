using MongoDB.Bson;

namespace DEA.Database.Models
{
    public sealed class User : Entity
    {
        public ulong UserId { get; set; }
        public decimal Cash { get; set; } = 0;
        public int Health { get; set; } = 100;
        public BsonDocument Inventory { get; set; } = new BsonDocument();
    }
}

using MongoDB.Bson;

namespace DEA.Database.Models.Sub
{
    public sealed class UnMuteAction : Action
    {
        public ObjectId MuteId { get; set; }
    }
}

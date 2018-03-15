using Discord;
using DEA.Services;
using MongoDB.Driver;

namespace DEA.Entities.Eval
{
    // TODO: find genius way to add every service and collection to globals
    public class Globals
    {
        public Globals(IDiscordClient client, IGuild guild, IMongoDatabase database, SendingService sender)
        {
            Client = client;
            Guild = guild;
            Database = database;
            Sender = sender;
        }

        public IDiscordClient Client { get; }
        public IGuild Guild { get; }
        public IMongoDatabase Database { get; }
        public SendingService Sender { get; }
    }
}

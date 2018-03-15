using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;
using System;
using DEA.Entities.Item;

namespace DEA.TypeReaders
{
    public sealed class ItemTypeReader : TypeReader
    {
        public Type Type { get; } = typeof(Item);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                var item = Data.Items.FirstOrDefault(x => x.Name.ToLower() == input);

                if (item != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(item));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item does not exist."));
            });
        }
    }
}

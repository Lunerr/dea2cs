using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DEA.Services.Static;
using System;
using DEA.Entities.Item;

namespace DEA.Common.TypeReaders
{
    public sealed class CrateTypeReader : TypeReader
    {
        public Type Type { get; } = typeof(Crate);

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            return Task.Run(() =>
            {
                input = input.ToLower();

                input = input.EndsWith("crate") ? input : input + " crate";

                var crate = Data.Crates.FirstOrDefault(x => x.Name.ToLower() == input);

                if (crate != null)
                {
                    return Task.FromResult(TypeReaderResult.FromSuccess(crate));
                }

                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "This item either does not exist or is not a crate."));
            });
        }
    }
}

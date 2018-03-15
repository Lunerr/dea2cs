using Discord;
using DEA.Common;
using DEA.Entities.Service;
using DEA.Extensions.Discord;
using DEA.Extensions.System;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class SendingService : Service
    {
        private readonly ThreadLocal<Random> _random;

        public SendingService(ThreadLocal<Random> random)
        {
            _random = random;
        }

        public Task<IUserMessage> SendFieldsAsync(IMessageChannel channel, Color? color = null, params string[] fieldOrValue)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(Config.DEFAULT_COLORS)
            };

            for (var i = 0; i < fieldOrValue.Length; i += 2)
                builder.AddField(fieldOrValue[i], fieldOrValue[i + 1]);

            return SendEmbedAsync(channel, builder);
        }

        public Task<IUserMessage> SendFieldsErrorAsync(IMessageChannel channel, params string[] fieldOrValue)
            => SendFieldsAsync(channel, Config.ERROR_COLOR, fieldOrValue);

        public async Task<bool> TryDMAsync(IUser user, string description, string title = null, Color? color = null, IGuild guild = null)
        {
            try
            {
                await SendAsync(await user.GetOrCreateDMChannelAsync(), description, title, color, guild);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<IUserMessage> SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null, IGuild guild = null)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? _random.Value.ArrayElement(Config.DEFAULT_COLORS),
                Description = description,
                Title = title
            };

            if (guild != null)
                builder.WithFooter(guild.Name, guild.IconUrl);

            return SendEmbedAsync(channel, builder);
        }

        public Task<IUserMessage> SendEmbedAsync(IMessageChannel channel, EmbedBuilder builder)
            => channel.SendMessageAsync(string.Empty, false, builder.Build());

        public Task<IUserMessage> ReplyAsync(IUser user, IMessageChannel channel, string description, string title = null, Color? color = null)
            => SendAsync(channel, $"{user.Bold()}, {description}", title, color);

        public Task<IUserMessage> ReplyErrorAsync(IUser user, IMessageChannel channel, string description)
            => ReplyAsync(user, channel, description, null, Config.ERROR_COLOR);
    }
}

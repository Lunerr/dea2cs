using Discord;
using DEA.Common;
using DEA.Database.Models;
using DEA.Entities.Service;
using DEA.Extensions.Database;
using DEA.Extensions.Discord;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class ModerationService : Service
    {
        private readonly IMongoCollection<Guild> _dbGuilds;
        private readonly IMongoCollection<Mute> _dbMutes;
        private readonly SendingService _sender;

        public ModerationService(IMongoCollection<Guild> dbGuilds, IMongoCollection<Mute> dbMutes, SendingService sender)
        {
            _dbGuilds = dbGuilds;
            _dbMutes = dbMutes;
            _sender = sender;
        }

        public Task InformUserAsync(Context ctx, IUser subject, TimeSpan length, string reason = null)
        {
            reason = string.IsNullOrWhiteSpace(reason) ? "" : $"\n{ctx.User.Bold()} has provided the following reason:```\n{reason}```";

            // TODO: prettier solution for this long message (SAME FOR HELP ASWELL)
            // TODO: sterilize reason to remove discord markdown chars
            return _sender.TryDMAsync(subject,
                $"{ctx.User.Bold()} has muted you for **{length.TotalHours}h** for the following {reason}", guild: ctx.Guild);
        }

        public async Task CreateMute(Context ctx, IUser user, TimeSpan length, string reason = null)
        {
            await _dbMutes.InsertOneAsync(new Mute(ctx.Guild.Id, user.Id, length));
            await LogMuteAsync(ctx, user, length, reason);
            await InformUserAsync(ctx, user, length, reason);
        }

        public async Task RemoveMute(Context ctx, IUser user, string reason)
        {
            await _dbMutes.UpdateManyAsync(x => x.UserId == user.Id && x.GuildId == ctx.Guild.Id,
                        new UpdateDefinitionBuilder<Mute>().Set(x => x.Active, false));
            await LogUnmuteAsync(ctx, user, reason);
        }

        public Task LogMuteAsync(Context ctx, IUser subject, TimeSpan length, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Mute"),
                ("User", $"{subject} ({subject.Id})"),
                ("Length", $"{length.TotalHours}h")
            };

            if (!string.IsNullOrWhiteSpace(reason))
                elements.Add(("Reason", reason));

            return LogAsync(ctx.Guild, elements, Config.MUTE_COLOR, ctx.User);
        }

        public Task LogUnmuteAsync(Context ctx, IUser subject, string reason)
        {
            var elements = new(string, string)[]
            {
                ("Action", "Unmute"),
                ("User", $"{subject} ({subject.Id})"),
                ("Reason", reason)
            };

            return LogAsync(ctx.Guild, elements, Config.UNMUTE_COLOR, ctx.User);
        }

        public Task LogAutoMuteAsync(Context ctx, TimeSpan length)
            => LogAsync(ctx.Guild, new(string, string)[]
            {
                ("Action", "Automatic Mute"),
                ("User", $"{ctx.User} ({ctx.User.Id})"),
                ("Length", $"{length.TotalHours}h")
            }, Config.MUTE_COLOR);

        public Task LogAutoUnmuteAsync(IGuild guild, IUser subject)
            => LogAsync(guild, new(string, string)[]
            {
                ("Action", "Automatic Unmute"),
                ("User", $"{subject} ({subject.Id})")
            }, Config.UNMUTE_COLOR);

        public Task LogClearAsync(Context ctx, IUser subject, int quantity, string reason = null)
        {
            var elements = new List<(string, string)>
            {
                ("Action", "Clear"),
                ("User", $"{subject} ({subject.Id})"),
                ("Quantity", quantity.ToString())
            };

            if (!string.IsNullOrWhiteSpace(reason))
                elements.Add(("Reason", reason));

            return LogAsync(ctx.Guild, elements, Config.CLEAR_COLOR, ctx.User);
        }

        public async Task LogAsync(IGuild guild, IReadOnlyCollection<(string, string)> elements, Color color, IUser moderator = null)
        {
            var dbGuild = await _dbGuilds.GetGuildAsync(guild.Id);

            await _dbGuilds.UpdateAsync(dbGuild, x => x.CaseCount++);

            if (!dbGuild.LogChannelId.HasValue)
                return;

            var logChannel = await guild.GetTextChannelAsync(dbGuild.LogChannelId.Value);

            if (logChannel == null || !await logChannel.CanSendAsync())
                return;

            var descBuilder = new StringBuilder();

            foreach (var element in elements)
                descBuilder.AppendFormat("**{0}:** {1}\n", element.Item1, element.Item2);

            var builder = new EmbedBuilder()
            {
                Timestamp = DateTimeOffset.UtcNow,
                Footer = new EmbedFooterBuilder { Text = $"Case #{dbGuild.CaseCount}" },
                Description = descBuilder.ToString(),
                Color = color
            };

            if (moderator != null)
            {
                builder.WithAuthor(new EmbedAuthorBuilder
                {
                    Name = $"{moderator} ({moderator.Id})",
                    IconUrl = moderator.GetAvatarUrl()
                });
            }

            await logChannel.SendMessageAsync(string.Empty, false, builder.Build());
        }
    }
}

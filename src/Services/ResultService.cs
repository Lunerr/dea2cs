using Discord;
using Discord.Commands;
using Discord.Net;
using DEA.Common;
using DEA.Entities.Service;
using DEA.Extensions.Discord;
using DEA.Extensions.System;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class ResultService : Service
    {
        private readonly LoggingService _logger;
        private readonly CommandService _commands;
        private readonly RateLimitService _rateLimitService;
        private readonly CooldownService _cooldownService;

        public ResultService(LoggingService logger, CommandService commands, RateLimitService rateLimitService,
            CooldownService cooldownService)
        {
            _logger = logger;
            _commands = commands;
            _rateLimitService = rateLimitService;
            _cooldownService = cooldownService;
        }

        public Task HandleResultAsync(Context ctx, IResult result, int argPos)
        {
            if (result.IsSuccess)
            {
                return _cooldownService.ApplyCooldownAsync(ctx, argPos);
            }
            else
            {
                var message = result.ErrorReason;

                switch (result.Error)
                {
                    case CommandError.ParseFailed:
                        message = "You have provided an invalid number.";
                        break;
                    case CommandError.Exception:
                        return HandleExceptionAsync(ctx, ((ExecuteResult)result).Exception);
                    case CommandError.BadArgCount:
                        var cmd = _commands.GetCommand(ctx, argPos);

                        message = $"You are incorrectly using this command.\n" +
                                  $"**Usage:** `{Config.PREFIX}{cmd.GetUsage()}`\n" +
                                  $"**Example:** `{Config.PREFIX}{cmd.GetExample()}`";
                        break;
                }

                return ctx.ReplyErrorAsync(message);
            }
        }

        public async Task HandleExceptionAsync(Context ctx, Exception ex)
        {
            var last = ex.Last();
            var message = last.Message;

            if (last is HttpException httpEx)
            {
                if ((int)httpEx.HttpCode == Constants.TOO_MANY_REQUESTS)
                {
                    _rateLimitService.IgnoreUser(ctx.User.Id, Config.IGNORE_DURATION);
                    await ctx.DmAsync($"You will not be able to use commands for the next {Config.IGNORE_DURATION.TotalMinutes} minutes. " +
                        $"Please do not participate in command spam.");
                    return;
                }
                else if (!Config.DISCORD_CODES.TryGetValue(httpEx.DiscordCode.GetValueOrDefault(), out message))
                {
                    Config.HTTP_CODES.TryGetValue(httpEx.HttpCode, out message);
                }
            }
            else
            {
                await _logger.LogAsync(LogSeverity.Error, $"{ex}");
            }

            await ctx.ReplyErrorAsync(message);
        }
    }
}

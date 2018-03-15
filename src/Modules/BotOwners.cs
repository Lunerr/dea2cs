using Discord;
using Discord.Commands;
using DEA.Common;
using DEA.Entities.Eval;
using DEA.Preconditions.Command;
using DEA.Services;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DEA.Modules
{
    [BotOwner]
    [Summary("Commands reserved for the developers of the bot.")]
    public sealed class BotOwners : ModuleBase<Context>
    {
        private readonly EvalService _evalService;
        private readonly LoggingService _logger;

        public BotOwners(EvalService evalService, LoggingService logger)
        {
            _evalService = evalService;
            _logger = logger;
        }

        [Command("Reboot")]
        [Alias("restart")]
        [Summary("Reboots the bot.")]
        public async Task RebootAsync()
        {
            await Context.ReplyAsync("Rebooting...");
            Environment.Exit(0);
        }

        [Command("ErrorLogs")]
        [Alias("errorlog")]
        [ErrorLogs]
        [Summary("Sends the error logs as an attached file.")]
        public Task ErrorLogsAsync()
            => Context.Channel.SendFileAsync(_logger.LogFileName(LogSeverity.Error));

        [Command("LastErrorLogs")]
        [Alias("lasterror")]
        [ErrorLogs]
        [Summary("Sends the most recent error logs.")]
        public async Task ErrorLogsAsync(
            [Summary("15")] int lineCount = 20)
        {
            var lines = await File.ReadAllLinesAsync(_logger.LogFileName(LogSeverity.Error));
            var responseBuilder = new StringBuilder("```");

            for (int i = lineCount >= lines.Length ? 0 : lines.Length - lineCount; i < lines.Length; i++)
                responseBuilder.AppendFormat("{0}\n", lines[i]);

            responseBuilder.Append("```");

            await ReplyAsync(responseBuilder.ToString());
        }

        [Command("Eval")]
        [Summary("Evaluate C# code.")]
        public async Task EvalAsync(
            [Summary("Client.Token")] [Remainder] string code)
        {
            var script = CSharpScript.Create(code, Config.SCRIPT_OPTIONS, typeof(Globals));

            if (!_evalService.TryCompile(script, out string errorMessage))
            {
                await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Compilation Error", $"```{errorMessage}```");
            }
            else
            {
                var result = await _evalService.EvalAsync(Context.Guild, script);

                if (result.Success)
                    await Context.SendFieldsAsync(null, "Eval", $"```cs\n{code}```", "Result", $"```{result.Result}```");
                else
                    await Context.SendFieldsErrorAsync("Eval", $"```cs\n{code}```", "Runtime Error", $"```{result.Exception}```");
            }
        }
    }
}

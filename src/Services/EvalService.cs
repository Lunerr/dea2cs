using Discord;
using Discord.WebSocket;
using DEA.Entities.Eval;
using DEA.Entities.Service;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEA.Services
{
    public sealed class EvalService : Service
    {
        private readonly SendingService _sender;
        private readonly IDiscordClient _client;
        private readonly IMongoDatabase _db;

        public EvalService(SendingService sender, DiscordSocketClient client,
            IMongoDatabase db)
        {
            _sender = sender;
            _client = client;
            _db = db;
        }

        public bool TryCompile(Script script, out string errorMessage)
        {
            var diagnostics = script.Compile();
            var compilerErrors = diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);

            var errorMsgBuilder = new StringBuilder();

            foreach (var error in compilerErrors)
                errorMsgBuilder.AppendFormat("{0}\n", error.GetMessage());

            errorMessage = errorMsgBuilder.ToString();

            return errorMsgBuilder.Length == 0;
        }

        public async Task<EvalResult> EvalAsync(IGuild guild, Script script)
        {
            try
            {
                var scriptResult = await script.RunAsync(new Globals(_client, guild, _db, _sender));
                return EvalResult.FromSuccess(scriptResult.ReturnValue?.ToString() ?? "Success.");
            }
            catch (Exception ex)
            {
                return EvalResult.FromError(ex);
            }
        }
    }
}

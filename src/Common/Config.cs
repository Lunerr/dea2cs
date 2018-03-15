using Discord;
using DEA.Database.Models;
using Microsoft.CodeAnalysis.Scripting;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DEA.Common
{
    public static class Config
    {
        // Command handler
        public const string PREFIX = ";";

        // Current user
        public const string GAME = PREFIX + "help";

        // Guild
        public const string INVITE_LINK = "https://discord.gg/mpy46Mh", BOT_INVITE = "https://discordapp.com/oauth2/authorize?client_id=419920535791730688&scope=bot&permissions=8",
            HELP_MESSAGE = "DEA's goal is to provide a fully decentralized discord server by allowing the community to control " +
            "every aspect of the guild. The entire system is based around reputation. The most reputable users are the " +
            "moderators. If believe a certain user is suitable to moderate, you may enter the following: `" + PREFIX +
            "rep username#1234`. The opposite can be done with `" + PREFIX + "unrep`.\n\nIt is essential that reputation " +
            "remains in the right hands, as everything revolves around it. It is your duty as a member of this community " +
            "to ensure that if a user was unjustly punished, the culprit must pay the consequences while vindicating the " +
            "victim. You may view anyone's reputation by using `" + PREFIX + "getrep`.\n\nIf you wish to view the various " +
            "command categories, you may use `" + PREFIX + "modules`. To view all the commands of a module, you may use `" +
            PREFIX + "module general`. You may also view all commands by using `" + PREFIX + "commands`. If you wish " +
            "to view the progress of this bot, or simply support the creators, you may join the official DEA server here: "
            + INVITE_LINK + ".";

        // Chat settings
        public static readonly TimeSpan CHAT_SERVICE_DELAY = TimeSpan.FromSeconds(30);
        public const decimal CHAT_REWARD = 50;
        public const int MIN_CHAT_LENGTH = 7;

        // Moderation settings
        public const int MIN_MUTE_LENGTH = 1, CLEAR_DELETE_DELAY = 3000;

        // Spam settings
        public const decimal SPAM_CASH_PENALTY = 500;
        public const int SPAM_LIMIT = 5; 
        public static readonly TimeSpan SPAM_MUTE_LENGTH = TimeSpan.FromHours(6), SPAM_DURATION = TimeSpan.FromSeconds(4);

        // Deleted messages settings
        public const int DELETED_MESSAGES_CHARS = 150;

        // Rate limit settings
        public static readonly TimeSpan IGNORE_DURATION = TimeSpan.FromMinutes(30);

        // Base directory of the solution.
        public static readonly string MainDirectory = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("src"));

        // Maximums
        public const int MAX_LB = 30, MAX_CLEAR = 100, MAX_REASON_LENGTH = 600, MAX_DELETED_MSGS = 10;

        // Minimums
        public const int MIN_LB = 5, MIN_CLEAR = 3, MIN_DELETED_MSGS = 1;

        // Regexes
        public static readonly Regex NEW_LINE_REGEX = new Regex(@"\r\n?|\n"), NUMBER_REGEX = new Regex(@"^\d+(\.\d+)?"),
            CAMEL_CASE = new Regex("(\\B[A-Z])"), MARKDOWN_REGEX = new Regex(@"\*|`|_|~"), SPLIT_CAMEL_CASE = new Regex("(?<=[a - z])([A - Z])");

        // Defaults
        public const int CLEAR_DEFAULT = 20, LB_COUNT = 10, DELETED_MSGS = 5;

        // Cooldowns in hours
        public const double OPENALL_CD = 6;

        // Items
        public const int MAX_CRATE_OPEN = 100000;

        // Timers
        public static readonly TimeSpan AUTO_UNMUTE_TIMER = TimeSpan.FromMinutes(1);

        // Logs
        public const string LOGS_DIRECTORY = "logs/";
        
        // Discord code responses
        public static readonly IReadOnlyDictionary<int, string> DISCORD_CODES = new Dictionary<int, string>()
        {
            { 20001, "Only a user account may perform this action." },
            { 50007, "I cannot DM you. Please allow direct messages from guild users." },
            { 50013, "I do not have permission to do that." },
            { 50034, "Discord does not allow bulk deletion of messages that are more than two weeks old." }
        }.ToImmutableDictionary();

        // HTTP code responses
        public static readonly IReadOnlyDictionary<HttpStatusCode, string> HTTP_CODES = new Dictionary<HttpStatusCode, string>()
        {
            { HttpStatusCode.Forbidden, "I do not have permission to do that." },
            { HttpStatusCode.InternalServerError, "An unexpected error has occurred, please try again later." },
            { HttpStatusCode.RequestTimeout, "The request has timed out, please try again later." }
        }.ToImmutableDictionary();

        // Custom colors
        public static readonly Color ERROR_COLOR = new Color(0xFF0000), MUTE_COLOR = new Color(0xFF3E29), UNMUTE_COLOR = new Color(0x72FF65),
            CLEAR_COLOR = new Color(0x4D3DFF);

    // Default colors
    public static readonly IReadOnlyList<Color> DEFAULT_COLORS = new Color[]
    {
        new Color(0xFF269A), new Color(0x66FFCC),
        new Color(0x00FF00), new Color(0xB10DC9),
        new Color(0x00E828), new Color(0xFFFF00),
        new Color(0x08F8FF), new Color(0x03DEAB),
        new Color(0xF226FF), new Color(0xFF00BB),
        new Color(0xFF1C8E), new Color(0x00FFFF),
        new Color(0x68FF22), new Color(0x14DEA0),
        new Color(0xFFBE11), new Color(0x0FFFFF),
        new Color(0x2954FF), new Color(0x40E0D0),
        new Color(0x9624ED), new Color(0x01ADB0),
        new Color(0xA8ED00), new Color(0xBF255F)
    }.ToImmutableArray();

        // Eval imports
        public static readonly IReadOnlyList<string> EVAL_IMPORTS = new string[]
        {
            "System",
            "System.Net",
            "System.Linq",
            "System.Threading.Tasks",
            "Discord",
            "Discord.Commands",
            "Discord.WebSocket",
            "DEA.Database.Models",
            "DEA.Extensions.Database",
            "DEA.Extensions.Discord",
            "DEA.Extensions.System",
            "MongoDB.Driver"
        }.ToImmutableArray();

        // Eval script options
        public static readonly ScriptOptions SCRIPT_OPTIONS = ScriptOptions.Default
                .WithImports(EVAL_IMPORTS)
                .WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic &&
                                !string.IsNullOrWhiteSpace(x.Location)));

        // JSON serialization settings
        public static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }
}

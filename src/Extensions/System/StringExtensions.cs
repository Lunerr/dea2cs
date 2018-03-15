using DEA.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace DEA.Extensions.System
{
    public static class StringExtensions
    {
        public static string UpperFirstChar(this string input)
            => input.Any() ? input.First().ToString().ToUpper() + input.Substring(1) : string.Empty;

        public static string Bold(this string input)
            => $"**{Config.MARKDOWN_REGEX.Replace(input.ToString(), "")}**";

        // Turn regex into config regex
        public static string SplitCamelCase(this string s)
            => Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
    }
}

using Discord.Commands;
using DEA.Common;

namespace DEA.Extensions.Discord
{
    public static class ParameterInfoExtensions
    {
        public static string Format(this ParameterInfo param)
            => Config.CAMEL_CASE.Replace(param.Name, " $1").ToLower();
    }
}

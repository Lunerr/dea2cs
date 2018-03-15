using System;
using System.Globalization;

namespace DEA.Extensions.System
{
    public static class DecimalExtensions
    {
        public static readonly CultureInfo _cultureInfo = new CultureInfo("en-US");

        public static string USD(this decimal number)
            => number.ToString("C", _cultureInfo);
    }
}

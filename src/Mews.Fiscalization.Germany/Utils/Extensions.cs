using System;
using System.Text.RegularExpressions;

namespace Mews.Fiscalization.Germany.Utils
{
    internal static class Extensions
    {
        public static bool MatchesRegex(this string value, string regex)
        {
            return value != null && Regex.Match(value, regex).Success;
        }

        public static bool LengthIsInRange(this string value, int? minLength = null, int? maxLength = null)
        {
            var length = value.Length;
            var isShorterThanMinLength = minLength != null && length < minLength;
            var exceedsMaxLength = maxLength != null && length > maxLength;
            return !isShorterThanMinLength && !exceedsMaxLength;
        }

        public static DateTime FromUnixTime(this long value)
        {
            return DateTimeOffset.FromUnixTimeSeconds(value).DateTime;
        }
    }
}

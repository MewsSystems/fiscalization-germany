using System;

namespace Mews.Fiscalization.Germany.Utils
{
    internal static class Check
    {
        public static T NotNull<T>(T value, string name)
            where T : class
        {
            return value ?? throw new ArgumentNullException(name);
        }

        public static void LengthInRange(string value, int? minLength = null, int? maxLength = null)
        {
            if (minLength != null)
            {
                MinLength(value, minLength.Value);
            }
            if (maxLength != null)
            {
                MaxLength(value, maxLength.Value);
            }
        }

        public static string Regex(string value, string regex)
        {
            return value.MatchesRegex(regex)
                ? value
                : throw new ArgumentException($"Value doesn't match regular expression '{regex}'.");
        }

        private static void MinLength(string value, int minLength)
        {
            if (value.Length < minLength)
            {
                throw new ArgumentException($"Min length of string is {minLength}.");
            }
        }

        private static void MaxLength(string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new ArgumentException($"Max length of string is {maxLength}.");
            }
        }
    }
}

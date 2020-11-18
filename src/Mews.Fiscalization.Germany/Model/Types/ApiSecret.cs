using Mews.Fiscalization.Core.Model;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class ApiSecret : LimitedString
    {
        private static readonly StringLimitation Limitation = new StringLimitation(pattern: "^[0-9A-Za-z]{43}$");

        public ApiSecret(string value)
            : base(value, Limitation)
        {
        }

        public static bool IsValid(string value)
        {
            return IsValid(value, Limitation);
        }
    }
}

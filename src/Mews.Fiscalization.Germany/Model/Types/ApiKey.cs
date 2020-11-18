using Mews.Fiscalization.Core.Model;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class ApiKey : LimitedString
    {
        private static readonly StringLimitation Limitation = new StringLimitation(maxLength: 512, pattern: ".*[^\\s].*", allowEmptyOrWhiteSpace: false);

        public ApiKey(string value)
            : base(value, Limitation)
        {
        }

        public static bool IsValid(string value)
        {
            return IsValid(value, Limitation);
        }
    }
}

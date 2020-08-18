namespace Mews.Fiscalization.Germany.Model
{
    public sealed class ApiKey : ValidatedString
    {
        private static readonly string regexValidation = ".*[^\\s].*";

        public ApiKey(string value)
            : base(value, 1, 512, regexValidation)
        {
        }

        public static bool IsValid(string value)
        {
            return IsValid(value, 1, 512, regexValidation);
        }
    }
}

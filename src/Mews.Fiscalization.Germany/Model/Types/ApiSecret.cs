namespace Mews.Fiscalization.Germany.Model.Types
{
    public sealed class ApiSecret : ValidatedString
    {
        private static readonly string regexValidation = "^[0-9A-Za-z]{43}$";

        public ApiSecret(string value)
            : base(value, 43, 43, regexValidation)
        {
        }

        public static bool IsValid(string value)
        {
            return IsValid(value, 43, 43, regexValidation);
        }
    }
}

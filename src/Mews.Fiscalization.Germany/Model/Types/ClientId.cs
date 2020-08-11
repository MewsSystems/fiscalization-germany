namespace Mews.Fiscalization.Germany.Model.Types
{
    public sealed class ClientId : ValidatedString
    {
        private static readonly string regexValidation = "(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$";

        public ClientId(string value)
            : base(value, 36, 36, regexValidation)
        {
        }

        public static bool IsValid(string value)
        {
            return IsValid(value, 36, 36, regexValidation);
        }
    }
}

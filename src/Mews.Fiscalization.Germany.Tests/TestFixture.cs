using Mews.Fiscalization.Germany.Model;
using System;

namespace Mews.Fiscalization.Germany.Tests
{
    public static class TestFixture
    {
        public static readonly Guid ClientId = new Guid(Environment.GetEnvironmentVariable("client_Id") ?? "INSERT_CLIENT_ID");
        public static readonly Guid TssId = new Guid(Environment.GetEnvironmentVariable("tss_id") ?? "INSERT_TSS_ID");
        public static readonly ApiKey ApiKey = new ApiKey(Environment.GetEnvironmentVariable("api_key") ?? "INSERT_API_KEY");
        public static readonly ApiSecret ApiSecret = new ApiSecret(Environment.GetEnvironmentVariable("api_secret") ?? "INSERT_API_SECRET");

        public static FiskalyClient GetFiskalyClient()
        {
            return new FiskalyClient(ApiKey, ApiSecret);
        }
    }
}

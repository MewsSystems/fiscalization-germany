using Mews.Fiscalization.Germany.Model;
using System;

namespace Mews.Fiscalization.Germany.Tests
{
    public static class TestFixture
    {
        private static readonly Guid ClientId = new Guid(Environment.GetEnvironmentVariable("client_Id") ?? "INSERT_CLIENT_ID");
        private static readonly Guid TssId = new Guid(Environment.GetEnvironmentVariable("tss_id") ?? "INSERT_TSS_ID");
        private static readonly ApiKey ApiKey = new ApiKey(Environment.GetEnvironmentVariable("api_key") ?? "INSERT_API_KEY");
        private static readonly ApiSecret ApiSecret = new ApiSecret(Environment.GetEnvironmentVariable("api_secret") ?? "INSERT_API_SECRET");

        public static ClientData GetClientData()
        {
            return new ClientData(
                apiKey: ApiKey,
                apiSecret: ApiSecret,
                tssId: TssId,
                clientId: ClientId
            );
        }

        public static FiskalyClient GetFiskalyClient()
        {
            return new FiskalyClient(ApiKey, ApiSecret);
        }
    }

    public class ClientData
    {
        public ClientData(ApiKey apiKey, ApiSecret apiSecret, Guid tssId, Guid clientId)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            TssId = tssId;
            ClientId = clientId;
        }

        public ApiKey ApiKey { get; }

        public ApiSecret ApiSecret { get; }

        public Guid TssId { get; }

        public Guid ClientId { get; }
    }
}

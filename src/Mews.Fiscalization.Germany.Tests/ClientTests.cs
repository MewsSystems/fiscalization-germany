using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private static readonly Guid ClientId = new Guid("f84da730-8074-4d40-8e7b-65998a7cec25");
        private static readonly Guid TssId = new Guid("f84da730-8074-4d40-8e7b-65998a7cec25");
        private static readonly ApiKey ApiKey = new ApiKey("test_dgyieu2fhj1jfj5em2pak6dli_test");
        private static readonly ApiSecret ApiSecret = new ApiSecret("k3t6OZFfBtXaIfZsNPTWFn7KAFZQ3YAIjmLKZePTw16");

        [Test]
        public async Task CreateClientSucceeds()
        {
            var client = GetClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var createdClient = await client.CreateClientAsync(accessToken, TssId);

            AssertClient(createdClient.IsSuccess, createdClient.SuccessResult.Id);
        }

        [Test]
        public async Task GetClientSucceeds()
        {
            var client = GetClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var result = await client.GetClientAsync(accessToken, ClientId, TssId);

            AssertClient(result.IsSuccess, result.SuccessResult.Id);
        }

        private FiskalyClient GetClient()
        {
            return new FiskalyClient(ApiKey, ApiSecret);
        }

        private void AssertClient(bool isSuccess, object value)
        {
            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(value);
        }
    }
}

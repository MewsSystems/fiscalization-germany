using NUnit.Framework;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany.Tests
{
    [TestFixture]
    public class ClientTests
    {
        [Test]
        public async Task CreateClientSucceeds()
        {
            var clientData = TestFixture.GetClientData();
            var client = TestFixture.GetFiskalyClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var createdClient = await client.CreateClientAsync(accessToken, clientData.TssId);

            AssertClient(createdClient.IsSuccess, createdClient.SuccessResult.Id);
        }

        [Test]
        public async Task GetClientSucceeds()
        {
            var clientData = TestFixture.GetClientData();
            var client = TestFixture.GetFiskalyClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var result = await client.GetClientAsync(accessToken, clientData.ClientId, clientData.TssId);

            AssertClient(result.IsSuccess, result.SuccessResult.Id);
        }

        private void AssertClient(bool isSuccess, object value)
        {
            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(value);
        }
    }
}

using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany.Tests
{
    [TestFixture]
    public class TssTests
    {
        [Test]
        public async Task CreateTssSucceeds()
        {
            var client = TestFixture.GetFiskalyClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var createdTss = await client.CreateTssAsync(accessToken, TssState.Initialized, description: "Creating a test TSS.");

            AssertTss(createdTss.IsSuccess, createdTss.SuccessResult.Id);
        }

        [Test]
        public async Task GetTssSucceeds()
        {
            var clientData = TestFixture.GetClientData();
            var client = TestFixture.GetFiskalyClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var tss = await client.GetTssAsync(accessToken, clientData.TssId);

            AssertTss(tss.IsSuccess, tss.SuccessResult.Id);
        }

        private void AssertTss(bool isSuccess, object value)
        {
            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(value);
        }
    }
}

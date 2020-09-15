using Mews.Fiscalization.Germany;
using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mews.Fiscalization.German.Tests
{
    [TestFixture]
    public class TransactionTests
    {
        private static readonly Guid ClientId = new Guid("1b7aa38b-6817-4f95-a69d-09268b6cb651");
        private static readonly Guid TssId = new Guid("f84da730-8074-4d40-8e7b-65998a7cec25");
        private static readonly ApiKey ApiKey = new ApiKey("test_dgyieu2fhj1jfj5em2pak6dli_test");
        private static readonly ApiSecret ApiSecret = new ApiSecret("k3t6OZFfBtXaIfZsNPTWFn7KAFZQ3YAIjmLKZePTw16");

        [Test]
        public async Task StatusCheckSucceeds()
        {
            var client = GetClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var status = await client.GetClientAsync(accessToken, ClientId, TssId);

            Assert.IsTrue(status.IsSuccess);
        }

        [Test]
        public async Task StartTransactionSucceeds()
        {
            var client = GetClient();
            var accessToken = await client.GetAccessTokenAsync();
            var startedTransaction = await client.StartTransactionAsync(accessToken.SuccessResult, ClientId, TssId, Guid.NewGuid());
            var successResult = startedTransaction.SuccessResult;

            Assert.IsTrue(startedTransaction.IsSuccess);
            Assert.IsTrue(successResult.StartUtc.HasValue);
            Assert.IsNotNull(successResult.Id);
        }

        [Test]
        public async Task StartAndFinishTransactionSucceeds()
        {
            var client = GetClient();
            var accessToken = await client.GetAccessTokenAsync();
            var successAccessTokenResult = accessToken.SuccessResult;
            var startedTransaction = await client.StartTransactionAsync(successAccessTokenResult, ClientId, TssId, Guid.NewGuid());
            var endedTransaction = await client.FinishTransactionAsync(successAccessTokenResult, ClientId, TssId, GetBill(), startedTransaction.SuccessResult.Id, lastRevision: "1");
            var successResult = endedTransaction.SuccessResult;
            var signature = successResult.Signature;

            Assert.IsTrue(endedTransaction.IsSuccess);
            Assert.IsTrue(successResult.StartUtc.HasValue);
            Assert.IsTrue(successResult.EndUtc.HasValue);
            Assert.IsNotNull(signature);
            Assert.IsNotEmpty(signature.Value);
            Assert.IsNotEmpty(signature.Algorithm);
            Assert.IsNotEmpty(signature.PublicKey);
        }

        private Bill GetBill()
        {
            return new Bill(
                type: BillType.Receipt,
                payments: GetPayments(),
                items: GetItems()
            );
        }

        private List<Payment> GetPayments()
        {
            return new List<Payment>()
            {
                new Payment(25, PaymentType.Cash, "EUR"),
                new Payment(30, PaymentType.NonCash, "EUR")
            };
        }

        private List<Item> GetItems()
        {
            return new List<Item>()
            {
                new Item(20, VatRateType.Normal),
                new Item(30, VatRateType.Normal),
                new Item(5, VatRateType.Reduced)
            };
        }

        private FiskalyClient GetClient()
        {
            return new FiskalyClient(ApiKey, ApiSecret);
        }
    }
}
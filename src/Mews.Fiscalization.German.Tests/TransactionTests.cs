using Mews.Fiscalization.Germany;
using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Mews.Fiscalization.German.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Guid ClientId = new Guid("18e3147a-f314-4ed1-b229-5ae387996716");
        private static readonly Guid TssId = new Guid("f84da730-8074-4d40-8e7b-65998a7cec25");
        private static readonly ApiKey ApiKey = new ApiKey("test_dgyieu2fhj1jfj5em2pak6dli_test");
        private static readonly ApiSecret ApiSecret = new ApiSecret("k3t6OZFfBtXaIfZsNPTWFn7KAFZQ3YAIjmLKZePTw16");

        [Test]
        public void StatusCheckSucceeds()
        {
            var client = GetClient();
            var status = client.GetClient(ClientId, TssId);

            Assert.IsTrue(status.IsSuccess);
        }

        [Test]
        public void StartTransactionSucceeds()
        {
            var client = GetClient();
            var startedTransaction = client.StartTransaction(ClientId, TssId);
            var successResult = startedTransaction.SuccessResult;

            Assert.IsTrue(startedTransaction.IsSuccess);
            Assert.IsTrue(successResult.StartUtc.HasValue);
            Assert.IsNotNull(successResult.Id);
        }

        [Test]
        public void StartAndEndTransactionSucceeds()
        {
            var client = GetClient();
            var startedTransaction = client.StartTransaction(ClientId, TssId);
            var endedTransaction = client.EndTransaction(ClientId, TssId, GetBill(), startedTransaction.SuccessResult.Id, latestRevision: "1");
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
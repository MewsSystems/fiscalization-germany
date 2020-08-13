using Mews.Fiscalization.Germany;
using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Mews.Fiscalization.German.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Guid ClientId = new Guid("INSERT_CLIENT_ID");
        private static readonly Guid TssId = new Guid("INSERT_TSS_ID");

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

            Thread.Sleep(3000);
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

        [Test]
        public void EndTransactionWithInvalidLatestRevisionFails()
        {
            var client = GetClient();
            var startedTransaction = client.StartTransaction(ClientId, TssId);

            Thread.Sleep(3000);
            var endedTransaction = client.EndTransaction(ClientId, TssId, GetBill(), startedTransaction.SuccessResult.Id, latestRevision: "10");
            Assert.IsFalse(endedTransaction.IsSuccess);
            Assert.AreEqual(endedTransaction.ErrorResult.ErrorCode, ResultErrorCode.InvalidTransactionOperation);
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
            return new FiskalyClient(new ApiKey("INSERT_API_KEY"), new ApiSecret("INSERT_SECRET_KEY"));
        }
    }
}
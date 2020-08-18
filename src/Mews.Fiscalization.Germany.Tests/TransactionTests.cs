using Mews.Fiscalization.Germany;
using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mews.Fiscalization.German.Tests
{
    [TestFixture]
    public class Tests
    {
        private static readonly Guid ClientId = new Guid("INSERT_CLIENT_ID");
        private static readonly Guid TssId = new Guid("INSERT_TSS_ID");
        private static readonly ApiKey ApiKey = new ApiKey("INSERT_API_KEY");
        private static readonly ApiSecret ApiSecret = new ApiSecret("INSERT_API_Secret");

        [Test]
        public async Task StatusCheckSucceeds()
        {
            var client = GetClient();
            var accessToken = client.GetAccessToken().Result.SuccessResult;
            var status = await client.GetClient(accessToken, ClientId, TssId);

            Assert.IsTrue(status.IsSuccess);
        }

        [Test]
        public async Task StartTransactionSucceeds()
        {
            var client = GetClient();
            var accessToken = client.GetAccessToken().Result.SuccessResult;
            var startedTransaction = await client.StartTransaction(accessToken, ClientId, TssId);
            var successResult = startedTransaction.SuccessResult;

            Assert.IsTrue(startedTransaction.IsSuccess);
            Assert.IsTrue(successResult.StartUtc.HasValue);
            Assert.IsNotNull(successResult.Id);
        }

        [Test]
        public async Task StartAndFinishTransactionSucceeds()
        {
            var client = GetClient();
            var accessToken = client.GetAccessToken().Result.SuccessResult;
            var startedTransaction = client.StartTransaction(accessToken, ClientId, TssId).Result;
            var endedTransaction = await client.FinishTransaction(accessToken, ClientId, TssId, GetBill(), startedTransaction.SuccessResult.Id, lastRevision: "1");
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
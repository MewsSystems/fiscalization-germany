﻿using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private static readonly Guid ClientId = new Guid(Environment.GetEnvironmentVariable("client_Id") ?? "INSERT_CLIENT_ID");
        private static readonly Guid TssId = new Guid(Environment.GetEnvironmentVariable("tss_id") ?? "INSERT_TSS_ID");
        private static readonly ApiKey ApiKey = new ApiKey(Environment.GetEnvironmentVariable("api_key") ?? "INSERT_API_KEY");
        private static readonly ApiSecret ApiSecret = new ApiSecret(Environment.GetEnvironmentVariable("api_secret") ?? "INSERT_API_SECRET");

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

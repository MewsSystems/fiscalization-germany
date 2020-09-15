﻿using Mews.Fiscalization.Germany.Model;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany.Tests
{
    [TestFixture]
    public class TssTests
    {
        private static readonly Guid TssId = new Guid("f84da730-8074-4d40-8e7b-65998a7cec25");
        private static readonly ApiKey ApiKey = new ApiKey("test_dgyieu2fhj1jfj5em2pak6dli_test");
        private static readonly ApiSecret ApiSecret = new ApiSecret("k3t6OZFfBtXaIfZsNPTWFn7KAFZQ3YAIjmLKZePTw16");

        [Test]
        public async Task CreateTssSucceeds()
        {
            var client = GetClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var createdTss = await client.CreateTssAsync(accessToken, TssState.Initialized, description: "Creating a test TSS.");

            AssertTss(createdTss.IsSuccess, createdTss.SuccessResult.Id);
        }

        [Test]
        public async Task GetTssSucceeds()
        {
            var client = GetClient();
            var accessToken = (await client.GetAccessTokenAsync()).SuccessResult;
            var tss = await client.GetTss(accessToken, TssId);

            AssertTss(tss.IsSuccess, tss.SuccessResult.Id);
        }

        private FiskalyClient GetClient()
        {
            return new FiskalyClient(ApiKey, ApiSecret);
        }

        private void AssertTss(bool isSuccess, object value)
        {
            Assert.IsTrue(isSuccess);
            Assert.IsNotNull(value);
        }
    }
}

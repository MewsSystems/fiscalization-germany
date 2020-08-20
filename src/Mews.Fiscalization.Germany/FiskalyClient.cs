using Mews.Fiscalization.Germany.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mews.Fiscalization.Germany
{
    public sealed class FiskalyClient
    {
        public ApiKey ApiKey { get; }
        public ApiSecret ApiSecret { get; }

        private Client Client { get; }

        public FiskalyClient(ApiKey apiKey, ApiSecret apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Client = new Client();
        }

        public async Task<ResponseResult<Model.Client>> GetClientAsync(AccessToken token, Guid clientId, Guid tssId)
        {
            return await Client.ProcessRequestAsync<Dto.ClientRequest, Dto.ClientResponse, Model.Client>(
                method: HttpMethod.Get,
                endpoint: $"tss/{tssId}/client/{clientId}",
                request: null,
                successFunc: response => ModelMapper.MapClient(response),
                token: token
            ).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ResponseResult<Transaction>> StartTransactionAsync(AccessToken token, Guid clientId, Guid tssId, Guid transactionId)
        {
            var request = RequestCreator.CreateTransaction(clientId);
            return await Client.ProcessRequestAsync<Dto.TransactionRequest, Dto.TransactionResponse, Transaction>(
                method: HttpMethod.Put,
                endpoint: $"tss/{tssId}/tx/{transactionId}",
                request: request,
                successFunc: response => ModelMapper.MapTransaction(response),
                token: token
            ).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ResponseResult<Transaction>> FinishTransactionAsync(AccessToken token, Guid clientId, Guid tssId, Bill bill, Guid transactionId, string lastRevision)
        {
            var request = RequestCreator.FinishTransaction(clientId, bill);
            return await Client.ProcessRequestAsync<Dto.FinishTransactionRequest, Dto.TransactionResponse, Transaction>(
                method: HttpMethod.Put,
                endpoint: $"tss/{tssId}/tx/{transactionId}?last_revision={lastRevision}",
                request: request,
                successFunc: response => ModelMapper.MapTransaction(response),
                token: token
            ).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<ResponseResult<AccessToken>> GetAccessTokenAsync()
        {
            var request = RequestCreator.CreateAuthorizationToken(ApiKey, ApiSecret);
            return await Client.ProcessRequestAsync<Dto.AuthorizationTokenRequest, Dto.AuthorizationTokenResponse, AccessToken>(
                method: HttpMethod.Post,
                endpoint: "auth",
                request: request,
                successFunc: response => ModelMapper.MapAccessToken(response)
            ).ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}

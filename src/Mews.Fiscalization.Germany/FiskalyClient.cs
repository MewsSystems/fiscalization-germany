using Fiskaly;
using Fiskaly.Client.Models;
using Fiskaly.Errors;
using Mews.Fiscalization.Germany.Model;
using Mews.Fiscalization.Germany.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Mews.Fiscalization.Germany
{
    public sealed class FiskalyClient
    {
        private static readonly string Endpoint = "https://kassensichv.io/api/v1";

        public ApiKey ApiKey { get; }

        public ApiSecret ApiSecret { get; }

        public FiskalyHttpClient Client { get; }

        public FiskalyClient(ApiKey apiKey, ApiSecret apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            Client = new FiskalyHttpClient(apiKey.Value, apiSecret.Value, Endpoint);
        }

        public ResponseResult<Transaction> StartTransaction(Guid clientId, Guid tssId)
        {
            var startTransactionRequest = new Dto.TransactionRequest
            {
                ClientId = clientId,
                State = Dto.State.ACTIVE
            };

            var payload = JsonConvert.SerializeObject(startTransactionRequest, Formatting.None);
            var response = Send<Dto.Transaction>(tssId: tssId, method: HttpMethod.Put, path: "tx", pathValue: Guid.NewGuid().ToString(), payload: payload);
            if (response.IsSuccess)
            {
                var transaction = response.SuccessResult;
                return new ResponseResult<Transaction>(successResult: new Transaction(
                    id: transaction.Id,
                    number: transaction.Number.ToString(),
                    startUtc: transaction.TimeStart.FromUnixTime()
                ));
            }
            return ResponseResult<Transaction>.MapError(response.ErrorResult);
        }

        public ResponseResult<Transaction> EndTransaction(Guid clientId, Guid tssId, Bill bill, Guid transactionId, string latestRevision)
        {
            var payload = JsonConvert.SerializeObject(Serializer.SerializeTransaction(bill, clientId), Formatting.None);
            var response = Send<Dto.Transaction>(
                tssId: tssId,
                method: HttpMethod.Put,
                path: "tx",
                pathValue: transactionId.ToString(),
                payload: payload,
                query: new Dictionary<string, string>() { { "last_revision", latestRevision } }
            );
            if (response.IsSuccess)
            {
                var transaction = response.SuccessResult;
                var signature = transaction.Signature;
                return new ResponseResult<Transaction>(successResult: new Transaction(
                    id: transaction.Id,
                    number: transaction.Number.ToString(),
                    startUtc: transaction.TimeStart.FromUnixTime(),
                    endUtc: transaction.TimeEnd.FromUnixTime(),
                    certificateSerial: transaction.CertificateSerial,
                    signature: new Signature(
                        value: signature.Value,
                        counter: signature.Counter,
                        algorithm: signature.Algorithm,
                        publicKey: signature.PublicKey
                    ),
                    qrCodeData: transaction.QrCodeData
                ));
            }
            return ResponseResult<Transaction>.MapError(response.ErrorResult);
        }

        public ResponseResult<Client> GetClient(Guid clientId, Guid tssId)
        {
            var response = Send<Dto.Client>(tssId: tssId, method: HttpMethod.Get, path: "client", pathValue: clientId.ToString());
            if (response.IsSuccess)
            {
                var client = response.SuccessResult;
                return new ResponseResult<Client>(successResult: new Client(
                    serialNumber: client.SerialNumber,
                    created: client.TimeCreation.FromUnixTime(),
                    updated: client.TimeUpdate.FromUnixTime(),
                    tssId: client.TssId,
                    id: client.Id
                ));
            }
            return ResponseResult<Client>.MapError(response.ErrorResult);
        }

        private ResponseResult<TResult> Send<TResult>(Guid tssId, HttpMethod method, string path, string pathValue, string payload = null, Dictionary<string, string> query = null)
            where TResult : class
        {
            try
            {
                var response = Client.Request(
                    method: method.ToString(),
                    path: $"/tss/{tssId}/{path}/{pathValue}",
                    body: payload != null ? Encoding.UTF8.GetBytes(payload) : null,
                    headers: null,
                    query: query
                );

                return new ResponseResult<TResult>(successResult: JsonConvert.DeserializeObject<TResult>(Encoding.UTF8.GetString(response.Body)));
            }
            catch (FiskalyHttpError e)
            {
                return new ResponseResult<TResult>(errorResult: ErrorResult.Map(e));
            }
        }
    }
}

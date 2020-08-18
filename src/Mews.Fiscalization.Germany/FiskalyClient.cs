using Fiskaly;
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
            return Send<Dto.Transaction, Transaction>(
                tssId: tssId,
                method: HttpMethod.Put,
                path: "tx",
                pathValue: Guid.NewGuid().ToString(),
                payload: payload,
                responseMapper: transaction => new Transaction(
                    id: transaction.Id,
                    number: transaction.Number.ToString(),
                    startUtc: transaction.TimeStart.FromUnixTime()
                )
            );
        }

        public ResponseResult<Transaction> EndTransaction(Guid clientId, Guid tssId, Bill bill, Guid transactionId, string latestRevision)
        {
            var payload = JsonConvert.SerializeObject(Serializer.SerializeTransaction(bill, clientId), Formatting.None);
            return Send<Dto.Transaction, Transaction>(
                tssId: tssId,
                method: HttpMethod.Put,
                path: "tx",
                pathValue: transactionId.ToString(),
                payload: payload,
                query: new Dictionary<string, string>() { { "last_revision", latestRevision } },
                responseMapper: transaction => new Transaction(
                    id: transaction.Id,
                    number: transaction.Number.ToString(),
                    startUtc: transaction.TimeStart.FromUnixTime(),
                    endUtc: transaction.TimeEnd.FromUnixTime(),
                    certificateSerial: transaction.CertificateSerial,
                    signature: new Signature(
                        value: transaction.Signature.Value,
                        counter: transaction.Signature.Counter,
                        algorithm: transaction.Signature.Algorithm,
                        publicKey: transaction.Signature.PublicKey
                    ),
                    qrCodeData: transaction.QrCodeData
                )
            );
        }

        public ResponseResult<Client> GetClient(Guid clientId, Guid tssId)
        {
            return Send<Dto.Client, Client>(
                tssId: tssId,
                method: HttpMethod.Get,
                path: "client",
                pathValue: clientId.ToString(),
                responseMapper: client => new Client(
                    serialNumber: client.SerialNumber,
                    created: client.TimeCreation.FromUnixTime(),
                    updated: client.TimeUpdate.FromUnixTime(),
                    tssId: client.TssId,
                    id: client.Id
                )
            );
        }

        private ResponseResult<TResult> Send<TResponse, TResult>(
            Guid tssId,
            HttpMethod method,
            string path,
            string pathValue,
            Func<TResponse, TResult> responseMapper,
            string payload = null,
            Dictionary<string, string> query = null)
                where TResult : class
                where TResponse : class
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

                var deserializedResponse = JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(response.Body));
                return new ResponseResult<TResult>(successResult: responseMapper(deserializedResponse));
            }
            catch (FiskalyHttpError e)
            {
                return new ResponseResult<TResult>(errorResult: ErrorResult.Map(e));
            }
        }
    }
}

using Fiskaly;
using Fiskaly.Client.Models;
using Mews.Fiscalization.Germany.Model;
using Mews.Fiscalization.Germany.Model.Types;
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

        public ClientId ClientId { get; }

        public TssId TssId { get; }

        public FiskalyHttpClient Client { get; }

        public FiskalyClient(ApiKey apiKey, ApiSecret apiSecret, ClientId clientId, TssId tssId)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            ClientId = clientId;
            TssId = tssId;
            Client = new FiskalyHttpClient(apiKey.Value, apiSecret.Value, Endpoint);
        }

        public ResponseResult<StartTransaction, Exception> StartTransaction()
        {
            var startTransactionRequest = new Dto.TransactionRequest
            {
                ClientId = ClientId.Value,
                State = Dto.State.ACTIVE.ToString()
            };
            var payload = JsonConvert.SerializeObject(startTransactionRequest, Formatting.None);
            var response = Send(method: HttpMethod.Put, path: "tx", pathValue: Guid.NewGuid().ToString(), payload: payload);

            if (response.IsSuccess)
            {
                var transaction = JsonConvert.DeserializeObject<Dto.Transaction>(Encoding.UTF8.GetString(response.SuccessResult.Body));
                return new ResponseResult<StartTransaction, Exception>(successResult: new StartTransaction(transaction.Id, transaction.LatestRevision.ToString()));
            }
            else
            {
                return new ResponseResult<StartTransaction, Exception>(errorResult: response.ErrorResult);
            }
        }

        public ResponseResult<EndTransaction, Exception> EndTransaction(Bill bill, string transactionId, string latestRevision)
        {
            var payload = JsonConvert.SerializeObject(Serialize(bill), Formatting.None);
            var response = Send(
                method: HttpMethod.Put,
                path: "tx",
                pathValue: transactionId,
                payload: payload,
                query: new Dictionary<string, string>() 
                {
                    { 
                        "last_revision", latestRevision 
                    }
                }
            );

            if (response.IsSuccess)
            {
                var transaction = JsonConvert.DeserializeObject<Dto.Transaction>(Encoding.UTF8.GetString(response.SuccessResult.Body));
                return new ResponseResult<EndTransaction, Exception>(successResult: new EndTransaction(
                    number: transaction.Number.ToString(),
                    start: DateTimeOffset.FromUnixTimeSeconds(transaction.TimeStart).DateTime,
                    end: DateTimeOffset.FromUnixTimeSeconds(transaction.TimeEnd).DateTime,
                    certificateSerial: transaction.CertificateSerial,
                    signature: transaction.Signature.Value,
                    signatureCounter: transaction.Signature.Counter,
                    signatureAlgorithm: transaction.Signature.Algorithm,
                    signaturePublicKey: transaction.Signature.PublicKey,
                    qrCodeData: transaction.QrCodeData
                ));
            }
            else
            {
                return new ResponseResult<EndTransaction, Exception>(errorResult: response.ErrorResult);
            }
        }

        public ResponseResult<Client, Exception> GetClient()
        {
            var response = Send(HttpMethod.Get, "client", ClientId.Value);

            if (response.IsSuccess)
            {
                var client = JsonConvert.DeserializeObject<Dto.Client>(Encoding.UTF8.GetString(response.SuccessResult.Body));
                return new ResponseResult<Client, Exception>(successResult: new Client(
                    serialNumber: client.SerialNumber,
                    created: DateTimeOffset.FromUnixTimeSeconds(client.TimeCreation).DateTime,
                    updated: DateTimeOffset.FromUnixTimeSeconds(client.TimeUpdate).DateTime,
                    tssId: client.TssId.ToString(),
                    id: client.Id.ToString()
                ));
            }
            else
            {
                return new ResponseResult<Client, Exception>(errorResult: response.ErrorResult);
            }
        }

        private ResponseResult<FiskalyHttpResponse, Exception> Send(HttpMethod method, string path, string pathValue, string payload = null, Dictionary<string, string> query = null)
        {
            try
            {
                return new ResponseResult<FiskalyHttpResponse, Exception>(successResult: Client.Request(
                    method: method.ToString(),
                    path: $"/tss/{TssId.Value}/{path}/{pathValue}",
                    body: payload != null ? Encoding.UTF8.GetBytes(payload) : null,
                    headers: null,
                    query: query
                ));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Dto.EndTransaction Serialize(Bill bill)
        {
            return new Dto.EndTransaction
            {
                ClientId = ClientId.Value,
                State = Dto.State.FINISHED.ToString(),
                Schema = new Dto.Schema
                {
                    StandardV1 = new Dto.StandardV1
                    {
                        Receipt = new Dto.Receipt
                        {
                            AmountsPerPaymentType = new Dto.AmountsPerPaymentType[]
                            {
                                new Dto.AmountsPerPaymentType
                                {
                                    Amount = bill.Net.ToString(),
                                    CurrencyCode = bill.CurrencyCode,
                                    PaymentType = SerializePaymentType(bill.PaymentType).ToString()
                                }
                            },
                            AmountsPerVatRate = new Dto.AmountsPerVatRate[]
                            {
                                new Dto.AmountsPerVatRate
                                {
                                    Amount = bill.Gross.ToString(),
                                    VatRate = SerializeVatRateType(bill.VatRateType).ToString()
                                }
                            },
                            ReceiptType = SerializeReceiptType(bill.ReceiptType).ToString()
                        }
                    }
                }
            };
        }

        private Dto.ReceiptType SerializeReceiptType(ReceiptType type)
        {
            switch (type)
            {
                case ReceiptType.Invoice:
                    return Dto.ReceiptType.INVOICE;
                case ReceiptType.Receipt:
                    return Dto.ReceiptType.RECEIPT;
                default:
                    throw new NotImplementedException($"Receipt type: {type} is not implemented.");
            };
        }

        private Dto.PaymentType SerializePaymentType(PaymentType type)
        {
            switch (type)
            {
                case PaymentType.Cash:
                        return Dto.PaymentType.CASH;
                case PaymentType.NonCash:
                    return Dto.PaymentType.NON_CASH;
                default:
                    throw new NotImplementedException($"Payment type: {type} is not implemented.");
            };
        }

        private Dto.VatRateType SerializeVatRateType(VatRateType type)
        {
            switch (type)
            {
                case VatRateType.None: 
                    return Dto.VatRateType.NULL;
                case VatRateType.Normal: 
                    return Dto.VatRateType.NORMAL;
                case VatRateType.Reduced: 
                    return Dto.VatRateType.REDUCED_1;
                case VatRateType.SpecialRate1:
                    return Dto.VatRateType.SPECIAL_RATE_1;
                case VatRateType.SpecialRate2:
                    return Dto.VatRateType.SPECIAL_RATE_2;
                default:
                    throw new NotImplementedException($"Vat rate type: {type} is not implemented.");
            };
        }
    }
}

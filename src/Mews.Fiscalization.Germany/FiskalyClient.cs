using Fiskaly;
using Fiskaly.Client.Models;
using Fiskaly.Errors;
using Mews.Fiscalization.Germany.Model;
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

        public string ApiKey { get; }

        public string ApiSecret { get; }

        public string ClientId { get; }

        public string TssId { get; }

        public FiskalyHttpClient Client { get; }

        public FiskalyClient(string apiKey, string apiSecret, string clientId, string tssId)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            ClientId = clientId;
            TssId = tssId;
            Client = new FiskalyHttpClient(apiKey, apiSecret, Endpoint);
        }

        public ResponseResult<StartTransaction, Exception> StartTransaction()
        {
            var startTransactionRequest = new Dto.TransactionRequest
            {
                ClientId = ClientId,
                State = Dto.State.ACTIVE.ToString()
            };
            var payload = JsonConvert.SerializeObject(startTransactionRequest, Formatting.None);
            var response = Send(payload);

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
                payload: payload,
                transactionId: transactionId,
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

        private ResponseResult<FiskalyHttpResponse, Exception> Send(string payload, string transactionId = null, Dictionary<string, string> query = null)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(payload);
            try
            {
                return new ResponseResult<FiskalyHttpResponse, Exception>(successResult: Client.Request(
                    method: HttpMethod.Put.ToString(),
                    path: $"/tss/{TssId}/tx/{transactionId ?? Guid.NewGuid().ToString()}",
                    body: bodyBytes,
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
                ClientId = ClientId,
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

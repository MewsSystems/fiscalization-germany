using Mews.Fiscalization.Germany.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mews.Fiscalization.Germany
{
    internal static class Serializer
    {
        public static  Dto.EndTransaction SerializeTransaction(IEnumerable<Bill> bills, Guid clientId, ReceiptType receiptType)
        {
            return new Dto.EndTransaction
            {
                ClientId = clientId,
                State = Dto.State.FINISHED.ToString(),
                Schema = new Dto.Schema
                {
                    StandardV1 = new Dto.StandardV1
                    {
                        Receipt = new Dto.Receipt
                        {
                            AmountsPerPaymentType = bills.Select(b => SerializeAmountsPerPayment(b)).ToArray(),
                            AmountsPerVatRate = bills.Select(b => SerializeAmountsPerVatRate(b)).ToArray(),
                            ReceiptType = SerializeReceiptType(receiptType).ToString()
                        }
                    }
                }
            };
        }

        private static Dto.AmountsPerPaymentType SerializeAmountsPerPayment(Bill bill)
        {
            return new Dto.AmountsPerPaymentType
            {
                Amount = bill.Net.ToString(),
                CurrencyCode = bill.CurrencyCode,
                PaymentType = SerializePaymentType(bill.PaymentType).ToString()
            };
        }

        private static Dto.AmountsPerVatRate SerializeAmountsPerVatRate(Bill bill)
        {
            return new Dto.AmountsPerVatRate
            {
                Amount = bill.Gross.ToString(),
                VatRate = SerializeVatRateType(bill.VatRateType).ToString()
            };
        }

        private static Dto.ReceiptType SerializeReceiptType(ReceiptType type)
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

        private static Dto.PaymentType SerializePaymentType(PaymentType type)
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

        private static Dto.VatRateType SerializeVatRateType(VatRateType type)
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

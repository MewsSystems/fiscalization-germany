using Mews.Fiscalization.Germany.Model;
using System;
using System.Linq;

namespace Mews.Fiscalization.Germany
{
    internal static class Serializer
    {
        public static Dto.EndTransaction SerializeTransaction(Bill bill, Guid clientId)
        {
            return new Dto.EndTransaction
            {
                ClientId = clientId,
                State = Dto.State.FINISHED,
                Schema = new Dto.Schema
                {
                    StandardV1 = new Dto.StandardV1
                    {
                        Receipt = new Dto.Receipt
                        {
                            AmountsPerPaymentType = bill.Payments.GroupBy(p => p.CurrencyCode).Select(g => SerializePayment(g.First())).ToArray(),
                            AmountsPerVatRate = bill.Items.GroupBy(i => i.VatRateType).Select(g => SerializeItem(g.First())).ToArray(),
                            ReceiptType = SerializeBillType(bill.Type)
                        }
                    }
                }
            };
        }

        private static Dto.AmountsPerPaymentType SerializePayment(Payment payment)
        {
            return new Dto.AmountsPerPaymentType
            {
                Amount = (payment.Amount + 0.00m).ToString(),
                CurrencyCode = payment.CurrencyCode,
                PaymentType = SerializePaymentType(payment.Type)
            };
        }

        private static Dto.AmountsPerVatRate SerializeItem(Item item)
        {
            return new Dto.AmountsPerVatRate
            {
                Amount = (item.Amount + 0.00m).ToString(),
                VatRate = SerializeVatRateType(item.VatRateType)
            };
        }

        private static Dto.ReceiptType SerializeBillType(BillType type)
        {
            switch (type)
            {
                case BillType.Invoice:
                    return Dto.ReceiptType.INVOICE;
                case BillType.Receipt:
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

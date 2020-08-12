using System.Collections.Generic;

namespace Mews.Fiscalization.Germany.Model
{
    public enum PaymentType
    {
        Cash,
        NonCash
    }

    public enum BillType
    {
        Receipt,
        Invoice
    }

    public enum VatRateType
    {
        Normal,
        Reduced,
        SpecialRate1,
        SpecialRate2,
        None
    }

    public sealed class Bill
    {
        public Bill(BillType type, IEnumerable<Payment> payments, IEnumerable<Item> items)
        {
            Type = type;
            Payments = payments;
            Items = items;
        }

        public BillType Type { get; }

        public IEnumerable<Payment> Payments { get; }

        public IEnumerable<Item> Items { get; }
    }
}

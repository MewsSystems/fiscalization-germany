namespace Mews.Fiscalization.Germany.Model
{
    public enum PaymentType
    {
        Cash,
        NonCash
    }

    public enum ReceiptType
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
        public Bill(ReceiptType receiptType, PaymentType paymentType, VatRateType vatRateType, string currencyCode, decimal net, decimal gross)
        {
            ReceiptType = receiptType;
            PaymentType = paymentType;
            VatRateType = vatRateType;
            CurrencyCode = currencyCode;
            Net = net;
            Gross = gross;
        }

        public ReceiptType ReceiptType { get; }

        public PaymentType PaymentType { get; }

        public VatRateType VatRateType { get; }

        public string CurrencyCode { get; }

        public decimal Net { get; }

        public decimal Gross { get; }
    }
}

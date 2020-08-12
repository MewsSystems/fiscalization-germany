using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class EndTransaction
    {
        public EndTransaction(
            string number,
            DateTime startUtc,
            DateTime endUtc,
            string certificateSerial,
            Signature signature,
            string qrCodeData)
        {
            Number = number;
            StartUtc = startUtc;
            EndUtc = endUtc;
            CertificateSerial = certificateSerial;
            Signature = signature;
            QrCodeData = qrCodeData;
        }

        public string Number { get; }
        
        public DateTime StartUtc { get; }

        public DateTime EndUtc { get; }

        public string CertificateSerial { get; }

        public Signature Signature { get; }

        public string QrCodeData { get; }
    }
}

using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class EndTransaction
    {
        public EndTransaction(
            string number,
            DateTime start,
            DateTime end,
            string certificateSerial,
            string signature,
            string signatureCounter,
            string signatureAlgorithm,
            string signaturePublicKey,
            string qrCodeData)
        {
            Number = number;
            Start = start;
            End = end;
            CertificateSerial = certificateSerial;
            Signature = signature;
            SignatureCounter = signatureCounter;
            SignatureAlgorithm = signatureAlgorithm;
            SignaturePublicKey = signaturePublicKey;
            QrCodeData = qrCodeData;
        }

        public string Number { get; }
        
        public DateTime Start { get; }

        public DateTime End { get; }

        public string CertificateSerial { get; }

        public string Signature { get; }

        public string SignatureCounter { get; }

        public string SignatureAlgorithm { get; }

        public string SignaturePublicKey { get; }

        public string QrCodeData { get; }
    }
}

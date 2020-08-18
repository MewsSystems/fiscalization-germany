using Mews.Fiscalization.Germany.Model;
using Mews.Fiscalization.Germany.Utils;

namespace Mews.Fiscalization.Germany
{
    internal static class ModelMapper
    {
        internal static ResponseResult<Transaction> MapTransaction(Dto.TransactionResponse transaction)
        {
            return new ResponseResult<Transaction>(successResult: new Transaction(
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
            ));
        }

        internal static ResponseResult<AccessToken> MapAccessToken(Dto.AuthorizationTokenResponse token)
        {
            return new ResponseResult<AccessToken>(successResult: new AccessToken(
                value: token.AccessToken,
                expirationUtc: token.AccessTokenExpiresAt.FromUnixTime()
            ));
        }

        internal static ResponseResult<Model.Client> MapClient(Dto.ClientResponse client)
        {
            return new ResponseResult<Model.Client>(successResult: new Model.Client(
                serialNumber: client.SerialNumber,
                created: client.TimeCreation.FromUnixTime(),
                updated: client.TimeUpdate.FromUnixTime(),
                tssId: client.TssId,
                id: client.Id
            ));
        }
    }
}

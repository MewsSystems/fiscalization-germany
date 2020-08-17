using Fiskaly.Errors;
using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class ErrorResult
    {
        private ErrorResult(string message, string requestId, FiskalyError errorCode)
        {
            Message = message;
            RequestId = requestId;
            ErrorCode = errorCode;
        }

        public string Message { get; }

        public string RequestId { get; }

        public FiskalyError ErrorCode { get; }

        internal static ErrorResult Map(FiskalyHttpError error)
        {
            return new ErrorResult(
                message: error.Message,
                requestId: error.RequestId,
                errorCode: MapErrorCode(error)
            );
        }

        internal static FiskalyError MapErrorCode(FiskalyHttpError error)
        {
            // For some reason, when the credentials are invalid, Fiskaly returns null code but with a message.
            if (error.Code == null && error.Message.Equals("Invalid credentials"))
            {
                return FiskalyError.InvalidCredentials;
            }
            switch (error.Code)
            {
                case "E_TSS_DISABLED":
                case "E_TSS_NOT_INITIALIZED":
                case "E_TX_ILLEGAL_TYPE_CHANGE":
                case "E_TX_NO_TYPE_DEFINED":
                case "E_API_VERSION":
                case "E_TX_UPSERT":
                    throw new InvalidOperationException("Invalid request from the library.");
                case "E_CLIENT_NOT_FOUND":
                    return FiskalyError.InvalidClientId;
                case "E_TSS_NOT_FOUND":
                    return FiskalyError.InvalidTssId;
                default:
                    throw new NotImplementedException($"Error code: {error.Code} is not implemented.");
            }
        }
    }
}

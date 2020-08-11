using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class FiskalyException : Exception
    {
        public FiskalyException(string backendStatus, string proxyStatus, string smaersStatus, string message)
            : base(message)
        {
            BackendStatus = backendStatus;
            ProxyStatus = proxyStatus;
            SmaersStatus = smaersStatus;
        }

        public string BackendStatus { get; }

        public string ProxyStatus { get; }

        public string SmaersStatus { get; }

        public object GetDetails()
        {
            return new
            {
                BackendStatus = BackendStatus,
                ProxyStatus = ProxyStatus,
                SmaersStatus = SmaersStatus,
                ErrorMessage = Message
            };
        }
    }
}

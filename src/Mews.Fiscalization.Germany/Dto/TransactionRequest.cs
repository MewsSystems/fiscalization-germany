using Newtonsoft.Json;

namespace Mews.Fiscalization.Germany.Dto
{
    public partial class TransactionRequest
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }
    }
}

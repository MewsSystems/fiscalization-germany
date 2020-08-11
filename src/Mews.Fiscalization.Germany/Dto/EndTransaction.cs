using Newtonsoft.Json;

namespace Mews.Fiscalization.Germany.Dto
{
    public sealed class EndTransaction : TransactionRequest
    {
        [JsonProperty("schema")]
        public Schema Schema { get; set; }
    }
}

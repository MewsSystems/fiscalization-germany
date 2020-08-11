using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class Client
    {
        public Client(string serialNumber, DateTime created, DateTime updated, string tssId, string id)
        {
            SerialNumber = serialNumber;
            Created = created;
            Updated = updated;
            TssId = tssId;
            Id = id;
        }

        public string SerialNumber { get; }

        public DateTime Created { get; }

        public DateTime Updated { get; }

        public string TssId { get; }

        public string Id { get; }
    }
}

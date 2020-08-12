using System;

namespace Mews.Fiscalization.Germany.Model
{
    public sealed class StartTransaction
    {
        public StartTransaction(Guid id, string latestRevision)
        {
            Id = id;
            LatestRevision = latestRevision;
        }

        public Guid Id { get; }

        public string LatestRevision { get; }
    }
}

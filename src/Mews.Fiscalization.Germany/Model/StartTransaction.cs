namespace Mews.Fiscalization.Germany.Model
{
    public sealed class StartTransaction
    {
        public StartTransaction(string id, string latestRevision)
        {
            Id = id;
            LatestRevision = latestRevision;
        }

        public string Id { get; }

        public string LatestRevision { get; }
    }
}

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionState<TResource, TIndex> where TResource : class
    {
        public TransactionStatus Status { get; set; }
        public List<TIndex> ReadIndexes { get; }
        public List<TResource> PendingUpdates { get; }
        public List<TResource> PendingInserts { get; }
        public List<TIndex> PendingDeletes { get; }
        public bool IsWrite => PendingUpdates.Count > 0 || PendingDeletes.Count > 0 || PendingInserts.Count > 0;
        private readonly Func<TResource, TIndex> _indexSelector;

        public TransactionState(Func<TResource, TIndex> indexSelector)
        {
            Status = TransactionStatus.Started;
            ReadIndexes = new();
            PendingUpdates = new();
            PendingInserts = new();
            PendingDeletes = new();
            _indexSelector = indexSelector;
        }

        public IEnumerable<TIndex> GetParticipatingIndexes()
        {
            var indexes = PendingUpdates
                        .Select(_indexSelector)
                        .Union(ReadIndexes)
                        .Union(PendingDeletes);
            return indexes;
        }
    }
}

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionState<TResource, TIndex> where TResource : class
    {
        public TransactionStatus Status { get; set; }
        public List<TResource> Updates { get; }
        public List<TResource> Inserts { get; }
        public List<TIndex> Deletes { get; }
        public bool IsWrite => Updates.Count > 0 || Deletes.Count > 0 || Inserts.Count > 0;
        private readonly Func<TResource, TIndex> _indexSelector;

        public TransactionState(Func<TResource, TIndex> indexSelector)
        {
            Status = TransactionStatus.Started;
            Updates = new();
            Inserts = new();
            Deletes = new();
            _indexSelector = indexSelector;
        }

        public IEnumerable<TIndex> GetParticipatingIndexes()
        {
            var indexes = Updates
                        .Select(_indexSelector)
                        .Union(Deletes);
            return indexes;
        }
    }
}

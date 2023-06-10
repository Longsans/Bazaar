namespace Bazaar.BuildingBlocks.Transactions.Abstractions
{
    public interface IResourceManager<TResource, TIndex> where TResource : class
    {
        Dictionary<TransactionRef, TransactionState<TResource, TIndex>> OngoingTransactions { get; set; }

        void HandlePrepare(TransactionRef txn);
        void HandleCommit(TransactionRef txn);
        void HandleRollback(TransactionRef txn);
    }
}

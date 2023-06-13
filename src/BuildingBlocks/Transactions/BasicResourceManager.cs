using Bazaar.BuildingBlocks.Transactions.Abstractions;

namespace Bazaar.BuildingBlocks.Transactions
{
    public class BasicResourceManager<TResource, TIndex> : IResourceManager<TResource, TIndex>
        where TResource : class
        where TIndex : IEquatable<TIndex>
    {
        public Dictionary<TransactionRef, TransactionState<TResource, TIndex>> OngoingTransactions { get; set; }
        protected readonly LockManager<TIndex> _lockManager;
        private readonly Func<TResource, TIndex> _indexSelector;

        public BasicResourceManager(LockManager<TIndex> lockManager, Func<TResource, TIndex> indexSelector)
        {
            OngoingTransactions = new();
            _lockManager = lockManager;
            _indexSelector = indexSelector;
        }

        public TransactionState<TResource, TIndex> GetOrCreateTransactionState(TransactionRef txn)
        {
            if (!OngoingTransactions.ContainsKey(txn))
                OngoingTransactions.Add(txn, new TransactionState<TResource, TIndex>(_indexSelector));
            return OngoingTransactions[txn];
        }

        public void HandlePrepare(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn] ?? throw new InvalidOperationException();
            if (txnState.IsWrite)
            {
                var writeIndexes = txnState.GetWriteIndexes();
                _lockManager.Aquire(writeIndexes, txn, LockMode.READWRITE);
            }
            txnState.Status = TransactionStatus.Prepared;
        }

        // derived classes perform domain-specific commits first before calling base.HandleCommit()
        // ALWAYS remember to call base.HandleCommit() to free locks
        public virtual void HandleCommit(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            var participatingIndexes = txnState.GetParticipatingIndexes();
            _lockManager.Release(participatingIndexes, txn);
            OngoingTransactions.Remove(txn);
        }

        public void HandleRollback(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            if (txnState == null)
                return;
            var participatingIndexes = txnState.GetParticipatingIndexes();
            _lockManager.Release(participatingIndexes, txn);
            OngoingTransactions.Remove(txn);
        }

        public void LockReadIndex(TransactionRef txn, TIndex index)
        {
            var txnState = GetOrCreateTransactionState(txn);
            txnState.ReadIndexes.Add(index);
            _lockManager.Aquire(index, txn, LockMode.READ);
        }
    }
}

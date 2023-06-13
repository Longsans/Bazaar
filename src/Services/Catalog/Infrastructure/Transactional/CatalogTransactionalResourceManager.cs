using Bazaar.Catalog.Model;

namespace Bazaar.Catalog.Infrastructure.Transactional
{
    public class CatalogTransactionalResourceManager : IResourceManager<CatalogItem, int>
    {
        public Dictionary<TransactionRef, TransactionState<CatalogItem, int>> OngoingTransactions { get; set; }
        private readonly LockManager<int> _lockManager;
        private readonly ICatalogRepository _catalogRepo;

        public CatalogTransactionalResourceManager(LockManager<int> lockManager, ICatalogRepository catalogRepo)
        {
            _lockManager = lockManager;
            _catalogRepo = catalogRepo;
            OngoingTransactions = new();
        }

        public TransactionState<CatalogItem, int> GetOrCreateTransactionState(TransactionRef txn)
        {
            if (!OngoingTransactions.ContainsKey(txn))
                OngoingTransactions.Add(txn, new TransactionState<CatalogItem, int>(c => c.Id));
            return OngoingTransactions[txn];
        }

        public void HandlePrepare(TransactionRef txn)
        {
            if (!OngoingTransactions.ContainsKey(txn))
                throw new InvalidOperationException();
            var txnState = OngoingTransactions[txn];
            var participatingIndexes = txnState.GetParticipatingIndexes();
            if (txnState.IsWrite)
                _lockManager.Aquire(participatingIndexes, txn, LockMode.READWRITE);

            txnState.Status = TransactionStatus.Prepared;
        }

        public void HandleCommit(TransactionRef txn)
        {
            if (!OngoingTransactions.ContainsKey(txn))
                return;

            var txnState = OngoingTransactions[txn];
            foreach (var newItem in txnState.PendingInserts) _catalogRepo.Create(newItem);
            foreach (var updatedItem in txnState.PendingUpdates) _catalogRepo.Update(updatedItem);
            foreach (var deletedIndex in txnState.PendingDeletes) _catalogRepo.Delete(deletedIndex);

            var participatingIndexes = txnState.GetParticipatingIndexes();
            _lockManager.Release(participatingIndexes, txn);
            OngoingTransactions.Remove(txn);
        }

        public void HandleRollback(TransactionRef txn)
        {
            if (!OngoingTransactions.ContainsKey(txn))
                return;
            var txnState = OngoingTransactions[txn];
            var participatingIndexes = txnState.GetParticipatingIndexes();
            _lockManager.Release(participatingIndexes, txn);
            OngoingTransactions.Remove(txn);
        }
    }
}

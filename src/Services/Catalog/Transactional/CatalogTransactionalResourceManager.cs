namespace Bazaar.Catalog.Infrastructure.Transactional
{
    public class CatalogTransactionalResourceManager : BasicResourceManager<CatalogItem, int>
    {
        private readonly ICatalogRepository _catalogRepo;

        public CatalogTransactionalResourceManager(
            ICatalogRepository catalogRepo,
            LockManager<int> lockManager,
            Func<CatalogItem, int> indexSelector) : base(lockManager, indexSelector)
        {
            _catalogRepo = catalogRepo;
        }

        public override void HandleCommit(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            if (txnState == null)
                return;

            foreach (var newItem in txnState.PendingInserts) _catalogRepo.Create(newItem);
            foreach (var updatedItem in txnState.PendingUpdates) _catalogRepo.Update(updatedItem);
            foreach (var deletedIndex in txnState.PendingDeletes) _catalogRepo.Delete(deletedIndex);

            base.HandleCommit(txn);
        }
    }
}

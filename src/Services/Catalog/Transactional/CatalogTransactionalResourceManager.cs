namespace Bazaar.Catalog.Infrastructure.Transactional
{
    public class CatalogTransactionalResourceManager : BasicResourceManager<CatalogItem, int>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CatalogTransactionalResourceManager(
            IServiceScopeFactory scopeFactory,
            LockManager<int> lockManager,
            Func<CatalogItem, int> indexSelector) : base(lockManager, indexSelector)
        {
            _scopeFactory = scopeFactory;
        }

        public override void HandleCommit(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            if (txnState == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var catalogRepo = scope.ServiceProvider.GetRequiredService<ICatalogRepository>();
            foreach (var newItem in txnState.PendingInserts) catalogRepo.Create(newItem);
            foreach (var updatedItem in txnState.PendingUpdates) catalogRepo.Update(updatedItem);
            foreach (var deletedIndex in txnState.PendingDeletes) catalogRepo.Delete(deletedIndex);

            base.HandleCommit(txn);
        }
    }
}

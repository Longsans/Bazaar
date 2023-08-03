namespace Bazaar.Ordering.Transactional
{
    public class OrderTransactionalResourceManager : BasicResourceManager<Order, int>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderTransactionalResourceManager(
            IServiceScopeFactory scopeFactory,
            LockManager<int> lockManager,
            Func<Order, int> indexSelector) : base(lockManager, indexSelector)
        {
            _scopeFactory = scopeFactory;
        }

        public override void HandleCommit(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            if (txnState == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var orderRepo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            foreach (var newItem in txnState.PendingInserts)
            {
                orderRepo.CreateProcessingPaymentOrder(newItem);
            }

            foreach (var updatedItem in txnState.PendingUpdates)
            {
                orderRepo.UpdateStatus(updatedItem.Id, updatedItem.Status);
            }

            base.HandleCommit(txn);
        }
    }
}

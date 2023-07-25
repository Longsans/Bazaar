namespace Bazaar.Ordering.Transactional
{
    public class OrderTransactionalResourceManager : BasicResourceManager<Order, int>
    {
        private readonly IOrderRepository _orderRepo;

        public OrderTransactionalResourceManager(
            IOrderRepository orderRepo,
            LockManager<int> lockManager,
            Func<Order, int> indexSelector) : base(lockManager, indexSelector)
        {
            _orderRepo = orderRepo;
        }

        public override void HandleCommit(TransactionRef txn)
        {
            var txnState = OngoingTransactions[txn];
            if (txnState == null)
                return;

            foreach (var newItem in txnState.PendingInserts) _orderRepo.CreateProcessingPayment(newItem);
            foreach (var updatedItem in txnState.PendingUpdates) _orderRepo.Update(updatedItem);

            base.HandleCommit(txn);
        }
    }
}

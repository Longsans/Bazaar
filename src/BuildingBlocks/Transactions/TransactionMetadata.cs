namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionMetadata
    {
        public TransactionRef TransactionRef { get; set; }
        public List<string> Indexes { get; set; }
        public TransactionStatus Status { get; set; }

        public TransactionMetadata(TransactionRef txn)
        {
            TransactionRef = txn;
            Indexes = new();
            Status = TransactionStatus.Started;
        }
    }

    public enum TransactionStatus
    {
        Started,
        Prepared,
        Commited,
        Aborted
    }
}

namespace Bazaar.BuildingBlocks.Transactions.Abstractions
{
    public interface ITransactionClient
    {
        Task BeginTransactionIfNotInOne();
        Task Commit();
        Task Rollback();

        TransactionRef? TransactionRef { get; }
    }
}

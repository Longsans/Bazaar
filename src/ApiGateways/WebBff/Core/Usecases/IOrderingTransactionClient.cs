namespace Bazaar.ApiGateways.WebBff.Core.Usecases
{
    public interface IOrderingTransactionClient : ITransactionClient
    {
        Task<int> RetrieveProductAvailableStock(string extProductId);
        Task AdjustProductAvailableStock(string extProductId, int availableStock);
        Task<OrderQuery> CreateProcessingOrder(OrderCreateCommand command);
    }
}

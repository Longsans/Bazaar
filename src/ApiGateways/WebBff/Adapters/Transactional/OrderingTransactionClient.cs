using Bazaar.BuildingBlocks.Transactions.Utility;
using Newtonsoft.Json.Linq;

namespace Bazaar.ApiGateways.WebBff.Adapters.Transactional
{
    public class OrderingTransactionClient : TransactionClient, IOrderingTransactionClient
    {
        private readonly string CATALOG_SERVICE_URI;
        private readonly string ORDER_SERVICE_URI;

        public OrderingTransactionClient(IConfiguration config, HttpClient client)
            : base(int.Parse(config["ClusterId"]), config["CoordinatorUri"], client)
        {
            CATALOG_SERVICE_URI = config["CatalogServiceUri"];
            ORDER_SERVICE_URI = config["OrderServiceUri"];
        }

        public async Task<int> RetrieveProductAvailableStock(string extProductId)
        {
            // retrieve stock
            await BeginTransactionIfNotInOne();
            Console.WriteLine("--ORD_CLI: transaction began.");
            var response = await _httpClient.GetAsync(StockRetrievalUrl(extProductId));
            Console.WriteLine("--ORD_CLI: stock response received.");
            if (!response.IsSuccessStatusCode)
            {
                throw new KeyNotFoundException($"Product with index {extProductId} does not exist.");
            }

            var availableStock = System.Text.Json.JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());

            // add indexes to transaction
            var addIndexResult = await SendIndexToCoordinator(extProductId);
            if (!addIndexResult)
            {
                throw new InvalidOperationException("Transaction does not exist in coordinator.");
            }
            return availableStock;
        }

        public async Task AdjustProductAvailableStock(string extProductId, int availableStock)
        {
            await BeginTransactionIfNotInOne();
            var response = await _httpClient.PatchAsync(
                StockUpdateUrl(extProductId),
                TransmissionUtil.SerializeToJson(availableStock));
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Product with index {extProductId} does not exist.");
            }
        }

        public async Task<OrderQuery> CreateProcessingOrder(OrderCreateCommand command)
        {
            await BeginTransactionIfNotInOne();
            var response = await _httpClient.PostAsync(OrderCreateUrl, TransmissionUtil.SerializeToJson(command));
            if (!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Order not valid.");
            }
            var contentJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            var orderQuery = contentJson.ToObject<OrderQuery>();
            var addIndexResult = await SendIndexToCoordinator(orderQuery!.ExternalId);
            if (!addIndexResult)
            {
                throw new InvalidOperationException($"Transaction does not exist in coordinator.");
            }
            return orderQuery;
        }

        private string StockRetrievalUrl(string extProductId) => $"{CATALOG_SERVICE_URI}/catalog/{extProductId}/stock?txn={_txnRef}";
        private string StockUpdateUrl(string extProductId) => $"{CATALOG_SERVICE_URI}/txn/{_txnRef}/catalog/{extProductId}";
        private string OrderCreateUrl => $"{ORDER_SERVICE_URI}/txn/{_txnRef}/orders";
    }
}

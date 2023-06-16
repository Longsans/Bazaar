using Bazaar.BuildingBlocks.Transactions.Utility;
using Newtonsoft.Json.Linq;

namespace Bazaar.ApiGateways.WebBff.Transactional
{
    public class OrderingTransactionClient : TransactionClient
    {
        private readonly string CATALOG_SERVICE_URI;
        private readonly string ORDER_SERVICE_URI;

        public OrderingTransactionClient(IConfiguration config, HttpClient client)
            : base(int.Parse(config["ClusterId"]), config["CoordinatorUri"], client)
        {
            CATALOG_SERVICE_URI = config["CatalogServiceUri"];
            ORDER_SERVICE_URI = config["OrderServiceUri"];
        }

        public async Task<int?> RetrieveProductAvailableStock(string extProductId)
        {
            // retrieve stock
            await BeginTransactionIfNotInOne();
            Console.WriteLine("--ORD_CLI: transaction began.");
            var response = await _httpClient.GetAsync($"{CATALOG_SERVICE_URI}/txn/{_txnRef}/{extProductId}");
            Console.WriteLine("--ORD_CLI: stock response received.");
            if (!response.IsSuccessStatusCode)
                return null;

            var availableStock = System.Text.Json.JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());

            // add indexes to transaction
            var addIndexResult = await SendIndexToCoordinator(extProductId);
            if (!addIndexResult)
                throw new KeyNotFoundException($"Transaction could not be found by coordinator.");
            return availableStock;
        }

        public async Task AdjustProductAvailableStock(string extProductId, int availableStock)
        {
            await BeginTransactionIfNotInOne();
            var response = await _httpClient.PatchAsync(
                $"{CATALOG_SERVICE_URI}/txn/{_txnRef}/{extProductId}",
                TransmissionUtil.SerializeToJson(availableStock));
            if (!response.IsSuccessStatusCode)
                throw new KeyNotFoundException($"Product with index {extProductId} does not exist.");
        }

        public async Task<OrderQuery> CreateProcessingOrder(OrderCreateCommand command)
        {
            await BeginTransactionIfNotInOne();
            var response = await _httpClient.PostAsync($"{ORDER_SERVICE_URI}/txn/{_txnRef}", TransmissionUtil.SerializeToJson(command));
            var contentJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            var orderQuery = contentJson.ToObject<OrderQuery>();
            var addIndexResult = await SendIndexToCoordinator(orderQuery.ExternalId);
            if (!addIndexResult)
                throw new KeyNotFoundException($"Transaction could not be found by coordinator.");
            return orderQuery;
        }
    }
}

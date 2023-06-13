using Bazaar.BuildingBlocks.Transactions.Utility;
using System.Text.Json;

namespace Bazaar.Ordering.Infrastructure.Transactional
{
    public class OrderingTransactionClient : TransactionClient
    {
        private readonly string CATALOG_SERVICE_URI;

        public OrderingTransactionClient(IConfiguration config, HttpClient client)
            : base(int.Parse(config["ClusterId"]), config["CoordinatorUri"], client)
        {
            CATALOG_SERVICE_URI = config["CatalogServiceUri"];
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

            var availableStock = JsonSerializer.Deserialize<int>(await response.Content.ReadAsStringAsync());

            // add indexes to transaction
            response = await _httpClient.PostAsync(
                $"{COORDINATOR_URI}/transactions/{_txnRef}/indexes",
                TransmissionUtil.SerializeToJson(extProductId));
            if (!response.IsSuccessStatusCode)
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
    }
}

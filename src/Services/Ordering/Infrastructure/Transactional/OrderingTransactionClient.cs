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
            await BeginTransactionIfNotInOne();
            Console.WriteLine("--ORD_CLI: transaction began.");
            _httpClient.BaseAddress = new Uri(CATALOG_SERVICE_URI);
            var response = await _httpClient.GetAsync($"txn/{_txnRef}/{extProductId}");
            Console.WriteLine("--ORD_CLI: stock response received.");
            if (!response.IsSuccessStatusCode)
                return null;

            if (!int.TryParse(response.Content.ReadAsStringAsync().Result, out var availableStock))
                return null;

            _httpClient.BaseAddress = new Uri(_coordinatorUri);
            response = await _httpClient.PostAsync(
                $"transactions/{_txnRef}/indexes", SerializeToJson(new { index = extProductId }));
            if (!response.IsSuccessStatusCode)
                throw new KeyNotFoundException($"Product with index {extProductId} does not exist.");
            return availableStock;
        }

        public async Task AdjustProductAvailableStock(string extProductId, int availableStock)
        {
            await BeginTransactionIfNotInOne();
            _httpClient.BaseAddress = new Uri(CATALOG_SERVICE_URI);
            var response = await _httpClient.PatchAsync(
                $"txn/{_txnRef}/{extProductId}", SerializeToJson(new { availableStock }));
            if (!response.IsSuccessStatusCode)
                throw new KeyNotFoundException($"Product with index {extProductId} does not exist.");
        }
    }
}

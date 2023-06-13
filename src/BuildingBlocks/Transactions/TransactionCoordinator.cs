namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionCoordinator
    {
        private readonly Dictionary<TransactionRef, TransactionMetadata> _transactions = new();
        private readonly HttpClient _httpClient;

        public TransactionCoordinator(HttpClient client)
        {
            _httpClient = client;
        }

        public void BeginTransaction(TransactionRef txn)
        {
            var metadata = new TransactionMetadata(txn);
            _transactions.Add(txn, metadata);
        }

        public async Task<bool> CommitTransaction(TransactionRef txn)
        {
            var metadata = _transactions[txn];
            if (metadata == null)
                return true; // transaction perhaps has already been committed

            var indexTypes = metadata.Indexes.GroupBy(index => index[..4]).Select(g => g.Key).ToList();
            List<string> nodes = new(3);
            foreach (var indexType in indexTypes)
            {
                nodes.Add(indexType switch
                {
                    "PROD" => "catalog/api/catalog",
                    "CUST" => "customer/api/customer",
                    "PNER" => "contracting/api/partner",
                    // other index types + service addresses
                    _ => throw new Exception("Unhandled index type")
                });
            }

            if (nodes.Count == 0)
                return true; // no-op, since nothing was done in txn

            // prepare
            var responses = await SendCommandToParticipants(nodes, "prepare", txn);
            if (responses.Any(r => !r.IsSuccessStatusCode))
                return false;

            // commit - commit guarantees to always succeed
            // we resort to trust the commit phase for now
            // in more robust systems, methods like retry should be adopted
            responses = await SendCommandToParticipants(nodes, "commit", txn);
            return true;
        }

        private async Task<HttpResponseMessage[]> SendCommandToParticipants(IList<string> nodes, string command, TransactionRef txn)
        {
            var responseTasks = new Task<HttpResponseMessage>[3];
            for (int i = 0; i < nodes.Count; i++)
            {
                _httpClient.BaseAddress = new Uri("https://" + nodes[i]);
                responseTasks[i] = _httpClient.PostAsync($"{command}/{txn}", null);
            }
            return await Task.WhenAll(responseTasks);
        }
    }
}

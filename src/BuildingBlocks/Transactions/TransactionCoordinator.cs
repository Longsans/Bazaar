using Bazaar.BuildingBlocks.Transactions.Abstractions;

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionCoordinator
    {
        private readonly Dictionary<TransactionRef, TransactionMetadata> _transactions = new();
        private readonly HttpClient _httpClient;
        private readonly IResourceLocationResolver _uriResolver;

        public TransactionCoordinator(HttpClient client, IResourceLocationResolver uriResolver)
        {
            _httpClient = client;
            _uriResolver = uriResolver;
        }

        public void BeginTransaction(TransactionRef txn)
        {
            if (_transactions.ContainsKey(txn))
                return; // already begun
            var metadata = new TransactionMetadata(txn);
            _transactions.Add(txn, metadata);
        }

        public async Task<bool> CommitTransaction(TransactionRef txn)
        {
            var metadata = _transactions[txn];
            if (metadata == null)
                return true; // transaction perhaps has already been committed

            var nodes = _uriResolver.GetResourceNodesFromIndexes(metadata.Indexes);
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

        public void AddIndexToTransaction(TransactionRef txn, string index)
        {
            if (!_transactions.ContainsKey(txn))
                throw new InvalidOperationException("Transaction does not exist");
            var metadata = _transactions[txn];
            if (!metadata.Indexes.Contains(index))
                metadata.Indexes.Add(index);
        }

        private async Task<HttpResponseMessage[]> SendCommandToParticipants(IList<string> nodes, string command, TransactionRef txn)
        {
            var responseTasks = new Task<HttpResponseMessage>[3];
            for (int i = 0; i < nodes.Count; i++)
            {
                _httpClient.BaseAddress = new Uri(nodes[i]);
                responseTasks[i] = _httpClient.PostAsync($"{command}/{txn}", null);
            }
            return await Task.WhenAll(responseTasks);
        }
    }
}

using Bazaar.BuildingBlocks.Transactions.Abstractions;
using Bazaar.BuildingBlocks.Transactions.Utility;

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

        public void AddIndexToTransaction(TransactionRef txn, string index)
        {
            if (!_transactions.ContainsKey(txn))
                throw new KeyNotFoundException("Transaction does not exist");
            var metadata = _transactions[txn];
            if (!metadata.Indexes.Contains(index))
                metadata.Indexes.Add(index);
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
            var responses = await SendPrepareRequestToParticipants(nodes, "txn", txn);
            if (responses.Any(r => !r.IsSuccessStatusCode))
                return false;

            // commit - commit guarantees to always succeed
            // we resort to trust the commit phase for now
            // in more robust systems, methods like retry should be adopted
            responses = await SendCommitOrRollbackRequestToParticipants(nodes, $"txn/{txn}", true);
            return true;
        }

        public async Task RollbackTransaction(TransactionRef txn)
        {
            var metadata = _transactions[txn];
            if (metadata == null)
                return; // transaction perhaps has already been rolled-back

            var nodes = _uriResolver.GetResourceNodesFromIndexes(metadata.Indexes);
            if (nodes.Count == 0)
                return; // no-op, since nothing was done in txn

            var responses = await SendCommitOrRollbackRequestToParticipants(nodes, $"txn/{txn}", false);
            foreach (var r in responses)
                r.EnsureSuccessStatusCode();
        }

        private async Task<IEnumerable<HttpResponseMessage>> SendPrepareRequestToParticipants(IList<string> nodes, string endpoint, object data)
        {
            return await SendCommandToParticipants(nodes, _httpClient.PostAsync, endpoint, data);
        }

        private async Task<IEnumerable<HttpResponseMessage>> SendCommitOrRollbackRequestToParticipants(IList<string> nodes, string endpoint, bool commit)
        {
            return await SendCommandToParticipants(nodes, _httpClient.PutAsync, endpoint, commit);
        }

        private static async Task<IEnumerable<HttpResponseMessage>> SendCommandToParticipants(
            IList<string> nodes,
            Func<string, HttpContent, Task<HttpResponseMessage>> method,
            string commandEndpoint,
            object data)
        {
            var responseTasks = new List<Task<HttpResponseMessage>>();
            foreach (var node in nodes)
            {
                responseTasks.Add(method($"{node}/{commandEndpoint}", TransmissionUtil.SerializeToJson(data)));
            }
            return await Task.WhenAll(responseTasks);
        }
    }
}

using Bazaar.BuildingBlocks.Transactions.Abstractions;
using Bazaar.BuildingBlocks.Transactions.Utility;

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionClient : ITransactionClient
    {
        public TransactionRef? TransactionRef => _txnRef;
        protected TransactionRef? _txnRef;
        protected readonly HttpClient _httpClient;
        protected readonly string COORDINATOR_URI;

        protected int _requestId = 1;
        protected int _clusterId;

        public TransactionClient(int clusterId, string coordinatorUri, HttpClient client)
        {
            _clusterId = clusterId;
            COORDINATOR_URI = coordinatorUri;
            _httpClient = client;
        }

        public async Task BeginTransactionIfNotInOne()
        {
            if (_txnRef != null)
                return;

            _txnRef = new TransactionRef(_requestId++, _clusterId);
            var response = await SendPostRequestToCoordinator("transactions", _txnRef);
            response.EnsureSuccessStatusCode();
        }

        public async Task Commit()
        {
            // commit transaction, coordinator handles both prepare and commit
            var response = await SendPutRequestToCoordinator($"transactions/{_txnRef}", true);
            response.EnsureSuccessStatusCode();
            _txnRef = null;
        }

        public async Task Rollback()
        {
            var response = await SendPutRequestToCoordinator($"transactions/{_txnRef}", false);
            response.EnsureSuccessStatusCode();
            _txnRef = null;
        }

        protected async Task<HttpResponseMessage> SendPostRequestToCoordinator(string endpoint, object content)
        {
            var jsonContent = TransmissionUtil.SerializeToJson(content);
            return await _httpClient.PostAsync($"{COORDINATOR_URI}/{endpoint}", jsonContent);
        }

        protected async Task<HttpResponseMessage> SendPutRequestToCoordinator(string endpoint, object content)
        {
            var jsonContent = TransmissionUtil.SerializeToJson(content);
            return await _httpClient.PutAsync($"{COORDINATOR_URI}/{endpoint}", jsonContent);
        }

        protected async Task<bool> SendIndexToCoordinator(string index)
        {
            var response = await _httpClient.PostAsync(
                $"{COORDINATOR_URI}/transactions/{_txnRef}/indexes",
                TransmissionUtil.SerializeToJson(index));
            return response.IsSuccessStatusCode;
        }
    }
}

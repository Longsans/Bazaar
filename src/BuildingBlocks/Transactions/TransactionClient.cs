using System.Text.Json;

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionClient
    {
        public TransactionRef? TransactionRef => _txnRef;
        protected TransactionRef? _txnRef;
        protected readonly HttpClient _httpClient;
        protected string _coordinatorUri;

        protected int _requestId = 1;
        protected int _clusterId;

        public TransactionClient(int clusterId, string coordinatorUri, HttpClient client)
        {
            _clusterId = clusterId;
            _coordinatorUri = coordinatorUri;
            _httpClient = client;
        }

        public async Task BeginTransactionIfNotInOne()
        {
            if (_txnRef != null)
                return;

            _txnRef = new TransactionRef(_requestId++, _clusterId);
            _httpClient.BaseAddress = new Uri(_coordinatorUri);
            var response = await SendPostRequestToCoordinator("transactions", _txnRef);
            Console.WriteLine(_coordinatorUri);
            Console.WriteLine(response.StatusCode);
            response.EnsureSuccessStatusCode();
        }

        public async Task Commit()
        {
            // commit transaction, coordinator handles both prepare and commit
            var response = await SendPostRequestToCoordinator($"transactions/{_txnRef}", new { prepare = true });
            response.EnsureSuccessStatusCode();
            _txnRef = null;
        }

        protected async Task<HttpResponseMessage> SendPostRequestToCoordinator(string endpoint, object content)
        {
            var jsonContent = SerializeToJson(content);
            return await _httpClient.PostAsync(endpoint, jsonContent);
        }

        protected static StringContent SerializeToJson(object content)
            => new(JsonSerializer.Serialize(content), System.Text.Encoding.UTF8, "application/json");
    }
}

namespace Bazaar.BuildingBlocks.Transactions
{
    public class TransactionClient
    {
        private TransactionRef? _txnRef;
        private readonly HttpClient _httpClient;

        private int _requestId = 1;
        private int _clusterId;

        public TransactionClient(int clusterId, HttpClient client)
        {
            _httpClient = client;
            _clusterId = clusterId;
        }

        public async void BeginTransactionIfNotInOne(string coordinatorUri)
        {
            if (_txnRef != null)
                return;

            _txnRef = new TransactionRef(_requestId++, _clusterId);
            _httpClient.BaseAddress = new Uri(coordinatorUri);
            var response = await _httpClient.PostAsync($"begin/{_txnRef}", null);
            response.EnsureSuccessStatusCode();
        }

        public async void Commit()
        {
            // commit transaction, coordinator handles both prepare and commit
            var response = await _httpClient.PostAsync($"commit/{_txnRef}", null);
            response.EnsureSuccessStatusCode();
            _txnRef = null;
        }
    }
}

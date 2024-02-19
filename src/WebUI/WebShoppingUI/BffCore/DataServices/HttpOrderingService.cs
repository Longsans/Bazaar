namespace WebShoppingUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;
using OrderResult = ServiceCallResult<Order>;

public class HttpOrderingService : HttpService, IOrderingDataService
{
    private readonly ApiEndpointResolver _apiEndpoints;

    public HttpOrderingService(HttpClient httpClient, ApiEndpointResolver apiEndpoints) : base(httpClient)
    {
        _apiEndpoints = apiEndpoints;
    }

    public async Task<OrderCollectionResult> GetOrdersByBuyerId(string buyerId)
    {
        var response = await _httpClient.GetAsync(_apiEndpoints.OrdersByBuyerId(buyerId));

        if (response is null)
            return OrderCollectionResult.UntypedError("Get orders by buyer ID response null.");

        return response.IsSuccessStatusCode
            ? (await DeserializeResponse<IEnumerable<Order>>(response)).ToList()
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => OrderCollectionResult.Unauthorized,
                HttpStatusCode.BadRequest => OrderCollectionResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => OrderCollectionResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<OrderCollectionResult> GetOrdersByBuyerIdContainingProducts(string buyerId, string[] productIds)
    {
        var response = await _httpClient.GetAsync(
            _apiEndpoints.OrdersByBuyerIdAndProductIds(buyerId, productIds));

        if (response is null)
            return OrderCollectionResult.UntypedError("Get orders by buyer ID containing products response null.");

        return response.IsSuccessStatusCode
            ? (await DeserializeResponse<IEnumerable<Order>>(response)).ToList()
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => OrderCollectionResult.Unauthorized,
                HttpStatusCode.BadRequest => OrderCollectionResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => OrderCollectionResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ServiceCallResult> UpdateStatus(int orderId, OrderUpdateStatusCommand updateCommand)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(updateCommand),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PatchAsync(
            _apiEndpoints.OrderById(orderId), reqContent);

        if (response is null)
            return OrderResult.UntypedError("Update order status response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => OrderResult.Unauthorized,
                HttpStatusCode.NotFound => OrderResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.Conflict => OrderResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                var s => OrderResult.UntypedError(ErrorStatusMessage(s))
            };
        }

        return ServiceCallResult.Success;
    }
}

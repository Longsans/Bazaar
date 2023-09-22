namespace WebSellerUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;

public class HttpOrderingService : HttpService, IOrderingDataService
{
    private readonly AddressService _addressService;

    public HttpOrderingService(HttpClient httpClient, AddressService addressService) : base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<OrderCollectionResult> GetByProductIds(string productIds)
    {
        var response = await _httpClient.GetAsync(_addressService.OrdersWithProductIds(productIds));

        if (response is null)
            return OrderCollectionResult.UntypedError("Get orders by product ID's response null.");

        return response.IsSuccessStatusCode
            ? await DeserializeResponse<List<Order>>(response)
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => OrderCollectionResult.Unauthorized,
                HttpStatusCode.BadRequest => OrderCollectionResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => OrderCollectionResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ServiceCallResult> UpdateStatus(int orderId, OrderStatus status)
    {
        var reqContent = new StringContent(
            ((int)status).ToString(), Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync(
            _addressService.OrderById(orderId), reqContent);

        if (response is null)
            return ServiceCallResult.UntypedError("Update order status response null.");

        return response.IsSuccessStatusCode
            ? ServiceCallResult.Success
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult.NotFound(),
                HttpStatusCode.Conflict => ServiceCallResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                var s => ServiceCallResult.UntypedError(ErrorStatusMessage(s))
            };
    }
}

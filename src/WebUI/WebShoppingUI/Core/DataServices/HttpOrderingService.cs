namespace WebShoppingUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;
using OrderResult = ServiceCallResult<Order>;

public class HttpOrderingService : HttpService, IOrderingDataService
{
    private readonly AddressService _addressService;

    public HttpOrderingService(HttpClient httpClient, AddressService addressService) : base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<OrderCollectionResult> GetOrdersByBuyerId(string buyerId)
    {
        var response = await _httpClient.GetAsync(_addressService.OrdersByBuyerId(buyerId));

        if (response is null)
            return OrderCollectionResult.UntypedError("Get orders by buyer ID response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => OrderCollectionResult.Unauthorized,
                HttpStatusCode.BadRequest => OrderCollectionResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => OrderCollectionResult.UntypedError(ErrorStatusMessage(status))
            };
        }

        return (await DeserializeResponse<IEnumerable<Order>>(response)).ToList();
    }

    public async Task<OrderResult> UpdateStatus(int orderId, OrderStatus status)
    {
        var response = await _httpClient.PatchAsync(
            _addressService.OrderStatusById(orderId, status), null);

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

        return await DeserializeResponse<Order>(response);
    }
}

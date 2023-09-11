namespace WebSellerUI.Services;

public class OrderingService : HttpService
{
    private readonly AddressService _addressService;

    public OrderingService(
        HttpClient httpClient, IHttpContextAccessor contextAccessor, AddressService addressService)
        : base(httpClient, contextAccessor)
    {
        _addressService = addressService;
    }

    public async Task<IEnumerable<Order>?> GetByProductIds(string productIds)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(_addressService.OrdersWithProductIds(productIds)) ??
            throw new Exception("Response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception($"Response unsuccessful: {response.StatusCode}")
            };
        }

        return await DeserializeResponse<IEnumerable<Order>>(response);
    }
}

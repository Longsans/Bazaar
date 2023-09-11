namespace WebSellerUI.Services;

public class OrderingService : HttpService
{
    private readonly string ORDERING_API;

    public OrderingService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor contextAccessor)
        : base(httpClient, contextAccessor)
    {
        ORDERING_API = config["OrderingApi"]!;
    }

    public async Task<IEnumerable<Order>> GetByProductIds(string productIds)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(GetOrdersUri(productIds)) ?? throw new Exception("Response null.");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Get by product ID unsuccessful.");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<Order>>(content);
    }

    private string GetOrdersUri(string productId) => $"{ORDERING_API}/api/orders?productId={productId}";
}

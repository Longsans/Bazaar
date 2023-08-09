namespace Bazaar.ApiGateways.WebBff.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly string BASKET_URI;

    private readonly HttpClient _httpClient;
    private readonly ILogger<BasketRepository> _logger;

    public BasketRepository(string basketUri, HttpClient httpClient, ILogger<BasketRepository> logger)
    {
        BASKET_URI = basketUri;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Basket?> GetByBuyerId(string buyerId)
    {
        var getResult = await _httpClient.GetAsync(GetUri(buyerId));
        var content = await getResult.Content.ReadAsStringAsync();

        if (!getResult.IsSuccessStatusCode)
        {
            if (getResult.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError($"Status code: {getResult.StatusCode}. Content: {content}");
            return null;
        }

        return JsonConvert.DeserializeObject<Basket>(content);
    }

    private string GetUri(string buyerId) => $"{BASKET_URI}/{buyerId}";
}

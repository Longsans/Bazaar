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

    public async Task<Basket?> AddItemToBasket(string buyerId, BasketItem item)
    {
        var addItemCommand = new
        {
            item.ProductId,
            item.ProductName,
            item.UnitPrice,
            item.Quantity,
            item.ImageUrl
        };
        var addContent = new StringContent(JsonConvert.SerializeObject(addItemCommand), Encoding.UTF8, "application/json");
        var addResult = await _httpClient.PostAsync(PostItemUri(buyerId), addContent);

        if (addResult.StatusCode == HttpStatusCode.Conflict)
        {
            _logger.LogError("[AddItemToBasket] method: result indicated conflict with existing item in basket.");
            return null;
        }
        if (addResult.StatusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError("[AddItemToBasket] method: result indicated internal server error.");
            return null;
        }

        var resultContent = await addResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Basket>(resultContent);
    }

    public async Task<BasketItem?> ChangeItemQuantity(string buyerId, string productId, uint quantity)
    {
        var quantityContent = new StringContent(JsonConvert.SerializeObject(quantity.ToString()), Encoding.UTF8, "application/json");
        var changeItemResult = await _httpClient.PatchAsync(PatchItemUri(buyerId, productId), quantityContent);
        if (!changeItemResult.IsSuccessStatusCode)
        {
            return null;
        }

        var resultContent = await changeItemResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<BasketItem>(resultContent);
    }

    public async Task<Basket?> GetByBuyerId(string buyerId)
    {
        var getResult = await _httpClient.GetAsync(GetUri(buyerId));
        var content = await getResult.Content.ReadAsStringAsync();

        if (!getResult.IsSuccessStatusCode)
        {
            if (getResult.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogError($"[GetByBuyerId] Status code: {getResult.StatusCode}. Content: {content}");
            return null;
        }

        return JsonConvert.DeserializeObject<Basket>(content);
    }

    private string GetUri(string buyerId) => $"{BASKET_URI}/{buyerId}";
    private string PostItemUri(string buyerId) => $"{BASKET_URI}/{buyerId}/items";
    private string PatchItemUri(string buyerId, string productId) => $"{BASKET_URI}/{buyerId}/items/{productId}";
}

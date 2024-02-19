namespace WebShoppingUI.DataServices;

public class HttpBasketService : HttpService, IBasketDataService
{
    private readonly ApiEndpointResolver _apiEndpoints;

    public HttpBasketService(HttpClient httpClient, ApiEndpointResolver apiEndpoints) : base(httpClient)
    {
        _apiEndpoints = apiEndpoints;
    }

    public async Task<ServiceCallResult<Basket>> GetBasketByBuyerId(string buyerId)
    {
        try
        {
            var response = await _httpClient.GetAsync(_apiEndpoints.BasketByBuyerId(buyerId)) ??
                throw new Exception("Get basket by buyer ID response null.");
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => ServiceCallResult<Basket>.Unauthorized,
                    var status => ServiceCallResult<Basket>.UntypedError(ErrorStatusMessage(status))
                };
            }
            return await DeserializeResponse<Basket>(response);
        }
        catch (Exception ex)
        {
            return ServiceCallResult<Basket>.UntypedError(ex.Message);
        }
    }

    public async Task<ServiceCallResult<Basket>> AddItemToBasket(string buyerId, BasketItem basketItem)
    {
        try
        {
            var reqContent = new StringContent(
                JsonConvert.SerializeObject(basketItem),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoints.BasketItemsByBuyerId(buyerId), reqContent);
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => ServiceCallResult<Basket>.Unauthorized,
                    HttpStatusCode.Conflict => ServiceCallResult<Basket>.Conflict(
                        await response.Content.ReadAsStringAsync()),
                    var status => ServiceCallResult<Basket>.UntypedError(ErrorStatusMessage(status))
                };
            }
            return await DeserializeResponse<Basket>(response);
        }
        catch (Exception ex)
        {
            return ServiceCallResult<Basket>.UntypedError(ex.Message);
        }
    }

    public async Task<ServiceCallResult> ChangeItemQuantity(string buyerId, string productId, uint quantity)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(quantity),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PatchAsync(
            _apiEndpoints.BasketItemByBuyerAndProductId(buyerId, productId),
            reqContent);

        if (response is null)
            return ServiceCallResult.UntypedError("Change item quantity response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult.NotFound(
                    $"Item of buyer ID {buyerId} with product ID {productId} not found."),
                HttpStatusCode.BadRequest => ServiceCallResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => ServiceCallResult.UntypedError(ErrorStatusMessage(status))
            };
        }

        return ServiceCallResult.Success;
    }

    public async Task<ServiceCallResult> Checkout(BasketCheckout checkout)
    {
        try
        {
            var reqContent = new StringContent(
                JsonConvert.SerializeObject(checkout), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiEndpoints.BasketCheckouts, reqContent) ??
                throw new Exception("Basket checkout response null.");
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                    HttpStatusCode.BadRequest => ServiceCallResult.BadRequest(
                        await response.Content.ReadAsStringAsync()),
                    var status => ServiceCallResult.UntypedError(ErrorStatusMessage(status))
                };
            }

            return ServiceCallResult.Success;
        }
        catch (Exception ex)
        {
            return ServiceCallResult.UntypedError(ex.Message);
        }
    }
}

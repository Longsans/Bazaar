using System.Text;

namespace WebShoppingUI.HttpServices;

public class BasketHttpService : HttpService, IBasketHttpService
{
    private readonly AddressService _addressService;

    public BasketHttpService(HttpClient httpClient, AddressService addressService) : base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<ServiceCallResult<Basket>> GetBasketByBuyerId(string buyerId)
    {
        try
        {
            var response = await _httpClient.GetAsync(_addressService.BasketByBuyerId(buyerId)) ??
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

            var response = await _httpClient.PostAsync(_addressService.BasketItemsByBuyerId(buyerId), reqContent);
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
        try
        {
            var reqContent = new StringContent(
                JsonConvert.SerializeObject(new { quantity }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PatchAsync(
                _addressService.BasketItemByBuyerAndProductId(buyerId, productId),
                reqContent)
                ?? throw new Exception("Change item quantity response null.");

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
        catch (Exception ex)
        {
            return ServiceCallResult.UntypedError(ex.Message);
        }
    }

    public async Task<ServiceCallResult> Checkout(BasketCheckout checkout)
    {
        try
        {
            var reqContent = new StringContent(
                JsonConvert.SerializeObject(checkout), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_addressService.BasketCheckout, reqContent) ??
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

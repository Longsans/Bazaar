namespace WebShoppingUI.DataServices;

public class HttpShopperInfoService : HttpService, IShopperInfoDataService
{
    private readonly AddressService _addressService;

    public HttpShopperInfoService(HttpClient httpClient, AddressService addressService)
        : base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId)
    {
        var response = await _httpClient.GetAsync(_addressService.ShopperByExternalId(externalId));

        if (response is null)
            return ServiceCallResult<Shopper>.UntypedError("Get shopper by external ID response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult<Shopper>.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult<Shopper>.NotFound(
                    await response.Content.ReadAsStringAsync()),
                var status => ServiceCallResult<Shopper>.UntypedError(ErrorStatusMessage(status))
            };
        }

        return await DeserializeResponse<Shopper>(response);
    }
}

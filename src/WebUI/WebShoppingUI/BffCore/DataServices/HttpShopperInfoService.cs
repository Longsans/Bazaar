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
        var response = await _httpClient.GetAsync(_addressService.ShopperByExternalIdQuery(externalId));

        if (response is null)
            return ServiceCallResult<Shopper>.UntypedError("Get shopper by external ID response null.");

        return response.IsSuccessStatusCode
            ? await DeserializeResponse<Shopper>(response)
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult<Shopper>.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult<Shopper>.NotFound(
                    await response.Content.ReadAsStringAsync()),
                var status => ServiceCallResult<Shopper>.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ServiceCallResult<Shopper>> Register(ShopperWriteCommand shopperInfo)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(shopperInfo),
            Encoding.UTF8,
            "application/json");
        var response = await _httpClient.PostAsync(_addressService.Shoppers, reqContent);
        if (response is null)
        {
            return ServiceCallResult<Shopper>.UntypedError("Registration response null.");
        }

        return response.IsSuccessStatusCode
            ? await DeserializeResponse<Shopper>(response)
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult<Shopper>.Unauthorized,
                var status => ServiceCallResult<Shopper>.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ServiceCallResult> UpdateInfo(string externalId, ShopperWriteCommand updateCommand)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(updateCommand),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(
            _addressService.ShopperByExternalId(externalId), reqContent);

        if (response is null)
            return ServiceCallResult.UntypedError("Update shopper info response null.");

        return response.IsSuccessStatusCode
            ? ServiceCallResult.Success
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                var status => ServiceCallResult.UntypedError(ErrorStatusMessage(status))
            };
    }
}

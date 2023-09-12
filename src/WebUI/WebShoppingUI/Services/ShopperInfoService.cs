using System.Net;

namespace WebShoppingUI.Services;

public class ShopperInfoService : HttpService
{
    private readonly AddressService _addressService;

    public ShopperInfoService(
        HttpClient httpClient, IHttpContextAccessor contextAccessor, AddressService addressService)
        : base(httpClient, contextAccessor)
    {
        _addressService = addressService;
    }

    public async Task<Shopper?> GetByExternalId(string externalId)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(_addressService.ShopperByExternalId(externalId)) ??
            throw new Exception("Get shopper by external ID response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                var status => throw new Exception($"Request unsuccessful, status code {status}")
            };
        }

        return await DeserializeResponse<Shopper>(response);
    }
}

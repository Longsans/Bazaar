namespace WebSellerUI.Services;

public class CatalogService : HttpService
{
    private readonly AddressService _addressService;

    public CatalogService(HttpClient httpClient, IHttpContextAccessor contextAccessor, AddressService addrService)
        : base(httpClient, contextAccessor)
    {
        _addressService = addrService;
    }

    public async Task<IEnumerable<CatalogItem>?> GetBySellerId(string sellerId)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(_addressService.CatalogBySellerId(sellerId)) ??
                throw new Exception("Get by seller ID response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception("Get by seller ID threw exception.")
            };
        }

        return await DeserializeResponse<IEnumerable<CatalogItem>>(response);
    }

    public async Task Update(CatalogItem update)
    {
        await SetAccessToken();
        var reqContent = new StringContent(
                JsonConvert.SerializeObject(update), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(_addressService.Catalog, reqContent) ?? throw new Exception("Get catalog response null.");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Reponse indicated error: {response.StatusCode}");
        }
    }
}

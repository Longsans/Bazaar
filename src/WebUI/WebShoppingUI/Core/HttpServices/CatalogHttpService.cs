namespace WebShoppingUI.HttpServices;
using CatalogItemResult = ServiceCallResult<CatalogItem>;
using CatalogResult = ServiceCallResult<IEnumerable<CatalogItem>>;

public class CatalogHttpService : HttpService, ICatalogHttpService
{
    private readonly AddressService _addressService;

    public CatalogHttpService(
        HttpClient httpClient, AddressService addressService)
        : base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<CatalogResult> GetByNameSubstring(string nameSubstring)
    {
        var response = await _httpClient.GetAsync(_addressService.CatalogByNameSubstring(nameSubstring)) ??
            throw new Exception("Get catalog by name substring response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => CatalogResult.Unauthorized,
                HttpStatusCode.BadRequest => CatalogResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => CatalogResult.UntypedError(ErrorStatusMessage(status))
            };
        }

        return (await DeserializeResponse<IEnumerable<CatalogItem>>(response)).ToList();
    }

    public async Task<CatalogItemResult> GetByProductId(string productId)
    {
        try
        {
            var response = await _httpClient.GetAsync(_addressService.CatalogByProductIds(productId))
                ?? throw new Exception("Get by product ID response null.");
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => CatalogItemResult.Unauthorized,
                    HttpStatusCode.NotFound => CatalogItemResult.NotFound(
                        await response.Content.ReadAsStringAsync()),
                    HttpStatusCode.BadRequest => CatalogItemResult.BadRequest(
                        await response.Content.ReadAsStringAsync()),
                    var status => CatalogItemResult.UntypedError(ErrorStatusMessage(status))
                };
            }
            return await DeserializeResponse<CatalogItem>(response);
        }
        catch (Exception ex)
        {
            return CatalogItemResult.UntypedError(ex.Message);
        }
    }
}

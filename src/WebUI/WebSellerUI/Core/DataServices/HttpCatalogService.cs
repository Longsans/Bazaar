namespace WebSellerUI.DataServices;

using CatalogCollectionResult = ServiceCallResult<IEnumerable<CatalogItem>>;
using CatalogItemResult = ServiceCallResult<CatalogItem>;

public class HttpCatalogService : HttpService, ICatalogDataService
{
    private readonly AddressService _addressService;

    public HttpCatalogService(HttpClient httpClient, AddressService addrService)
        : base(httpClient)
    {
        _addressService = addrService;
    }

    public async Task<CatalogCollectionResult> GetBySellerId(string sellerId)
    {
        var response = await _httpClient.GetAsync(_addressService.CatalogBySellerId(sellerId));

        if (response is null)
            return CatalogCollectionResult.UntypedError("Get by seller ID response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => CatalogCollectionResult.Unauthorized,
                HttpStatusCode.BadRequest => CatalogCollectionResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => CatalogCollectionResult.UntypedError(ErrorStatusMessage(status)),
            };
        }

        return (await DeserializeResponse<IEnumerable<CatalogItem>>(response)).ToList();
    }

    public async Task<CatalogItemResult> Create(CatalogItemCreateCommand item)
    {
        var reqContent = new StringContent(
                JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_addressService.Catalog, reqContent);

        if (response is null)
            return CatalogItemResult.UntypedError("Create catalog item response null.");

        return response.IsSuccessStatusCode
            ? CatalogItemResult.Success(
                await DeserializeResponse<CatalogItem>(response))
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => CatalogItemResult.Unauthorized,
                var status => CatalogItemResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ServiceCallResult> Update(string productId, CatalogItemUpdateCommand update)
    {
        var reqContent = new StringContent(
                JsonConvert.SerializeObject(update), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(_addressService.CatalogByProductId(productId), reqContent);

        if (response is null)
            return ServiceCallResult.UntypedError("Update catalog item response null.");

        return response.IsSuccessStatusCode
            ? ServiceCallResult.Success
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult.NotFound(),
                var status => ServiceCallResult.UntypedError(ErrorStatusMessage(status))
            };
    }
}

namespace Bazaar.ApiGateways.WebBff.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly string CATALOG_URI;

    private readonly HttpClient _httpClient;

    public CatalogRepository(string catalogUri, HttpClient httpClient)
    {
        CATALOG_URI = catalogUri;
        _httpClient = httpClient;
    }

    public async Task<CatalogItem?> GetByProductId(string productId)
    {
        var getResult = await _httpClient.GetAsync(GetByProductIdUri(productId));
        if (getResult.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        var getContent = await getResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CatalogItem>(getContent);
    }

    private string GetByProductIdUri(string productId) => $"{CATALOG_URI}?productId={productId}";
}

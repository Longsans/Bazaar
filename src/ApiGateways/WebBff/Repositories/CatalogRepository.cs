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
        var getResult = await _httpClient.GetAsync(GetUri(productId));
        getResult.EnsureSuccessStatusCode();

        var getContent = await getResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CatalogItem>(getContent);
    }

    public async Task<IEnumerable<CatalogItem>> GetManyByProductId(IEnumerable<string> productIds)
    {
        var getResult = await _httpClient.GetAsync(GetManyUri(productIds));
        getResult.EnsureSuccessStatusCode();

        var getContent = await getResult.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<CatalogItem>>(getContent)!;
    }

    private string GetUri(string productId) => $"{CATALOG_URI}/{productId}";
    private string GetManyUri(IEnumerable<string> productIds) => $"{CATALOG_URI}?productIds={string.Join("&productIds=", productIds)}";
}

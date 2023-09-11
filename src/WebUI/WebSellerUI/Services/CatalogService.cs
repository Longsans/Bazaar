using System.Net;

namespace WebSellerUI.Services;

public class CatalogService : HttpService
{
    private readonly string CATALOG_API;

    public CatalogService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor contextAccessor)
        : base(httpClient, contextAccessor)
    {
        CATALOG_API = config["CatalogApi"]!;
    }

    public async Task<IEnumerable<CatalogItem>?> GetBySellerId(string sellerId)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(GetBySellerIdUri(sellerId)) ??
                throw new Exception("Get by seller ID response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception("Get by seller ID threw exception.")
            };
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<CatalogItem>>(content);
    }

    public async Task<IEnumerable<CatalogItem>?> GetAllItems()
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(CatalogUri) ?? throw new Exception("Get catalog response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception("Get catalog threw exception.")
            };
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<CatalogItem>>(content);
    }

    public async Task Update(CatalogItem update)
    {
        await SetAccessToken();
        var reqContent = new StringContent(
                JsonConvert.SerializeObject(update), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync(CatalogUri, reqContent) ?? throw new Exception("Get catalog response null.");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Reponse indicated error: {response.StatusCode}");
        }
    }

    private string GetBySellerIdUri(string sellerId) => $"{CATALOG_API}/api/catalog?sellerId={sellerId}";
    private string CatalogUri => $"{CATALOG_API}/api/orders";
}

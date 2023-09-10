using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace WebSellerUI.Services;

public class CatalogService
{
    private readonly string CATALOG_URI;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _ctxAccessor;

    public CatalogService(HttpClient httpClient, IConfiguration config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        CATALOG_URI = config["CatalogUri"]!;
        _ctxAccessor = contextAccessor;
    }

    public async Task<IEnumerable<CatalogItem>?> GetAllItems()
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync($"{CATALOG_URI}/all") ?? throw new Exception("Get catalog response null.");
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception("Get catalog threw exception.")
            };
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<IEnumerable<CatalogItem>>(content).ToList();
    }

    public async Task Update(CatalogItem update)
    {
        await SetAccessToken();
        var reqContent = new StringContent(
                JsonConvert.SerializeObject(update), System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{CATALOG_URI}", reqContent) ?? throw new Exception("Get catalog response null.");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Reponse indicated error: {response.StatusCode}");
        }
    }

    public async Task SetAccessToken()
    {
        var token = await _ctxAccessor.HttpContext.GetTokenAsync("access_token");
        if (token != null)
        {
            _httpClient.SetBearerToken(token);
        }
    }
}

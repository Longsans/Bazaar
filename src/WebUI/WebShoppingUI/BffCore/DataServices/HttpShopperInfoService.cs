namespace WebShoppingUI.DataServices;

public class HttpShopperInfoService : HttpService, IShopperInfoDataService
{
    private readonly ApiEndpointResolver _apiEndpoints;

    public HttpShopperInfoService(HttpClient httpClient, ApiEndpointResolver apiEndpoints)
        : base(httpClient)
    {
        _apiEndpoints = apiEndpoints;
    }

    public async Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId)
    {
        var response = await _httpClient.GetAsync(_apiEndpoints.ShopperByExternalIdQuery(externalId));

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

    public async Task<ServiceCallResult<Shopper>> Register(ShopperRegistration shopperInfo)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(shopperInfo),
            Encoding.UTF8,
            "application/json");
        var response = await _httpClient.PostAsync(_apiEndpoints.Shoppers, reqContent);
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

    public async Task<ServiceCallResult> UpdateInfo(string externalId, ShopperPersonalInfo shopperPersonalInfo)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(shopperPersonalInfo),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PatchAsync(
            _apiEndpoints.ShopperPersonalInfo(externalId), reqContent);

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

    public async Task<ServiceCallResult> ChangeEmailAddress(string externalId, string emailAddress)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(emailAddress),
            Encoding.UTF8,
            "application/json");
        var response = await _httpClient.PatchAsync(
            _apiEndpoints.ShopperEmailAddress(externalId), reqContent);
        if (response is null)
            return ServiceCallResult.UntypedError("Update shopper info response null.");

        return response.IsSuccessStatusCode
            ? ServiceCallResult.Success
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ServiceCallResult.Unauthorized,
                HttpStatusCode.NotFound => ServiceCallResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.Conflict => ServiceCallResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                var status => ServiceCallResult.UntypedError(ErrorStatusMessage(status))
            };
    }
}

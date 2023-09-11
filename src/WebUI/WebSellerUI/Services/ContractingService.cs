namespace WebSellerUI.Services;

public class ContractingService : HttpService
{
    private readonly AddressService _addressService;

    public ContractingService(
        HttpClient httpClient, IHttpContextAccessor contextAccessor, AddressService addressService) :
        base(httpClient, contextAccessor)
    {
        _addressService = addressService;
    }

    public async Task<IEnumerable<Contract>?> GetContractsByPartnerId(string partnerId)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(_addressService.ContractsByPartnerId(partnerId)) ??
            throw new Exception("Get contracts by partner ID response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception($"Response unsuccessful: {response.StatusCode}")
            };
        }

        return await DeserializeResponse<IEnumerable<Contract>>(response);
    }

    public async Task<Partner?> GetPartnerById(string partnerId)
    {
        await SetAccessToken();
        var response = await _httpClient.GetAsync(_addressService.PartnerById(partnerId)) ??
            throw new Exception("Get contracts by partner ID response null.");

        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => null,
                _ => throw new Exception($"Response unsuccessful: {response.StatusCode}")
            };
        }

        return await DeserializeResponse<Partner>(response);
    }
}

namespace WebShoppingUI.DataServices;

public class HttpContractingService : HttpService, IContractingService
{
    private readonly ApiEndpointResolver _apiEndpoints;

    public HttpContractingService(HttpClient httpClient, ApiEndpointResolver apiEndpoints) : base(httpClient)
    {
        _apiEndpoints = apiEndpoints;
    }

    public async Task<ServiceCallResult<ContractingClient>> GetSellerById(string sellerId)
    {
        var response = await _httpClient.GetAsync(_apiEndpoints.SellerById(sellerId));
        try
        {
            return response.IsSuccessStatusCode
                ? await DeserializeResponse<ContractingClient>(response)
                : response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => ServiceCallResult<ContractingClient>.Unauthorized,
                    HttpStatusCode.NotFound => ServiceCallResult<ContractingClient>.NotFound("Seller not found."),
                    var status => ServiceCallResult<ContractingClient>.UntypedError(ErrorStatusMessage(status))
                };
        }
        catch (Exception ex)
        {
            return ServiceCallResult<ContractingClient>.UntypedError(ex.Message);
        }
    }
}
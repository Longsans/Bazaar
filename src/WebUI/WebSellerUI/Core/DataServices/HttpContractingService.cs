namespace WebSellerUI.DataServices;

using ContractCollectionResult = ServiceCallResult<IEnumerable<Contract>>;
using ContractResult = ServiceCallResult<Contract>;
using PartnerResult = ServiceCallResult<Partner>;

public class HttpContractingService : HttpService, IContractingDataService
{
    private readonly AddressService _addressService;

    public HttpContractingService(
        HttpClient httpClient, AddressService addressService) :
        base(httpClient)
    {
        _addressService = addressService;
    }

    public async Task<ContractCollectionResult> GetContractsByPartnerId(string partnerId)
    {
        var response = await _httpClient.GetAsync(_addressService.ContractsByPartnerId(partnerId));

        if (response is null)
            return ContractCollectionResult.UntypedError("Get contracts by partner ID response null.");

        return response.IsSuccessStatusCode
            ? await DeserializeResponse<List<Contract>>(response)
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ContractCollectionResult.Unauthorized,
                var status => ContractCollectionResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<PartnerResult> GetPartnerById(string partnerId)
    {
        var response = await _httpClient.GetAsync(_addressService.PartnerById(partnerId));

        if (response is null)
            return PartnerResult.UntypedError("Get partner by ID response null.");

        return response.IsSuccessStatusCode
            ? await DeserializeResponse<Partner>(response)
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => PartnerResult.Unauthorized,
                HttpStatusCode.NotFound => PartnerResult.NotFound(),
                var status => PartnerResult.UntypedError(ErrorStatusMessage(status)),
            };
    }

    public async Task<ContractResult> SignFixedPeriodContract(
        string partnerId, FixedPeriodContractCreateCommand command)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(command),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            _addressService.PartnerFixedPeriodContracts(partnerId), reqContent);

        if (response is null)
            return ContractResult.UntypedError("Sign fixed period contract response null.");

        return response.IsSuccessStatusCode
            ? ContractResult.Success(
                await DeserializeResponse<Contract>(response))
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ContractResult.Unauthorized,
                HttpStatusCode.BadRequest => ContractResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.NotFound => ContractResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.Conflict => ContractResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                var status => ContractResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ContractResult> SignIndefiniteContract(
        string partnerId, IndefiniteContractCreateCommand command)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(command),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync(
            _addressService.PartnerIndefiniteContracts(partnerId), reqContent);

        if (response is null)
            return ContractResult.UntypedError("Sign indefinite contract response null.");

        return response.IsSuccessStatusCode
            ? ContractResult.Success(
                await DeserializeResponse<Contract>(response))
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ContractResult.Unauthorized,
                HttpStatusCode.BadRequest => ContractResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.NotFound => ContractResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.Conflict => ContractResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                var status => ContractResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ContractResult> EndCurrentIndefiniteContract(string partnerId)
    {
        var command = new IndefiniteContractEndCommand(true);
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(command),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync(
            _addressService.PartnerCurrentIndefiniteContract(partnerId),
            reqContent);

        if (response is null)
            return ContractResult.UntypedError("End current indefinite contract response null.");

        return response.IsSuccessStatusCode
            ? ContractResult.Success(
                await DeserializeResponse<Contract>(response))
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ContractResult.Unauthorized,
                HttpStatusCode.NotFound => ContractResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.BadRequest => ContractResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => ContractResult.UntypedError(ErrorStatusMessage(status))
            };
    }

    public async Task<ContractResult> ExtendCurrentFixedPeriodContract(
        string partnerId, ContractExtension extension)
    {
        var reqContent = new StringContent(
            JsonConvert.SerializeObject(extension),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PatchAsync(
            _addressService.PartnerCurrentFixedPeriodContract(partnerId), reqContent);

        if (response is null)
            return ContractResult.UntypedError("Extend current FP contract response null.");

        return response.IsSuccessStatusCode
            ? ContractResult.Success(
                await DeserializeResponse<Contract>(response))
            : response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => ContractResult.Unauthorized,
                HttpStatusCode.NotFound => ContractResult.NotFound(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.Conflict => ContractResult.Conflict(
                    await response.Content.ReadAsStringAsync()),
                HttpStatusCode.BadRequest => ContractResult.BadRequest(
                    await response.Content.ReadAsStringAsync()),
                var status => ContractResult.UntypedError(ErrorStatusMessage(status))
            };
    }
}

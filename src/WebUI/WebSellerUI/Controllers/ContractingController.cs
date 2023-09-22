using Microsoft.AspNetCore.Mvc;

namespace WebSellerUI.Controllers;

[ApiController]
[Route("api/partners/")]
public class ContractingController : ControllerBase
{
    private readonly IContractingDataService _contractingService;

    public ContractingController(IContractingDataService contractingService)
    {
        _contractingService = contractingService;
    }

    [HttpGet("/api/contracts")]
    public async Task<ActionResult<IEnumerable<Contract>>> GetContractsByPartnerId(
        string partnerId)
    {
        var contractsResult = await _contractingService.GetContractsByPartnerId(partnerId);

        return contractsResult.IsSuccess
            ? contractsResult.Result!.ToList()
            : contractsResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                _ => StatusCode(500, contractsResult.ErrorDetail)
            };
    }

    [HttpGet("{partnerId}")]
    public async Task<ActionResult<Partner>> GetPartnerById(string partnerId)
    {
        var partnerResult = await _contractingService.GetPartnerById(partnerId);

        return partnerResult.IsSuccess
            ? partnerResult.Result!
            : partnerResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                _ => StatusCode(500, partnerResult.ErrorDetail)
            };
    }

    [HttpPost("{partnerId}/fp-contracts")]
    public async Task<ActionResult<Contract>> SignFixedPeriodContract(
        string partnerId, FixedPeriodContractCreateCommand command)
    {
        var contractResult = await _contractingService
            .SignFixedPeriodContract(partnerId, command);

        return contractResult.IsSuccess
            ? contractResult.Result!
            : contractResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.BadRequest => BadRequest(contractResult.ErrorDetail),
                ServiceCallError.NotFound => NotFound(contractResult.ErrorDetail),
                ServiceCallError.Conflict => Conflict(contractResult.ErrorDetail),
                _ => StatusCode(500, contractResult.ErrorDetail)
            };
    }

    [HttpPost("{partnerId}/in-contracts")]
    public async Task<ActionResult<Contract>> SignIndefiniteContract(
        string partnerId, IndefiniteContractCreateCommand command)
    {
        var contractResult = await _contractingService
            .SignIndefiniteContract(partnerId, command);

        return contractResult.IsSuccess
            ? contractResult.Result!
            : contractResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.BadRequest => BadRequest(contractResult.ErrorDetail),
                ServiceCallError.NotFound => NotFound(contractResult.ErrorDetail),
                ServiceCallError.Conflict => Conflict(contractResult.ErrorDetail),
                _ => StatusCode(500, contractResult.ErrorDetail)
            };
    }

    [HttpPatch("{partnerId}/in-contracts/current")]
    public async Task<ActionResult<Contract>> EndCurrentIndefiniteContract(
        string partnerId, IndefiniteContractEndCommand command)
    {
        if (!command.Ended)
            return NoContent();

        var contractResult = await _contractingService
            .EndCurrentIndefiniteContract(partnerId);

        return contractResult.IsSuccess
            ? contractResult.Result!
            : contractResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.BadRequest => BadRequest(contractResult.ErrorDetail),
                ServiceCallError.NotFound => NotFound(contractResult.ErrorDetail),
                _ => StatusCode(500, contractResult.ErrorDetail)
            };
    }
}

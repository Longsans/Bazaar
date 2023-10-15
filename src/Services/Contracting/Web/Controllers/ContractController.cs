namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IContractUseCases _contractUseCases;

    public ContractController(IContractUseCases contractUseCases)
    {
        _contractUseCases = contractUseCases;
    }

    [HttpGet("{id}")]
    public ActionResult<ContractDto> GetById(int id)
    {
        var contract = _contractUseCases.GetById(id);
        if (contract == null)
        {
            return NotFound();
        }

        return new ContractDto(contract);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ContractDto>> GetByPartnerId(
        string partnerId)
    {
        return _contractUseCases
            .GetByPartnerExternalId(partnerId)
            .Select(c => new ContractDto(c))
            .ToList();
    }

    [HttpPost("/api/partners/{partnerExternalId}/fixed-period-contracts")]
    public ActionResult<ContractDto> SignPartnerForFixedPeriodContract(
        string partnerExternalId, CreateFixedPeriodContractRequest request)
    {
        var signResult = _contractUseCases.SignPartnerForFixedPeriod(
            partnerExternalId, request.SellingPlanId, request.EndDate);

        return signResult.ToActionResult(this);

        //return signResult switch
        //{
        //    ContractSuccessResult r =>
        //        CreatedAtAction(
        //            nameof(GetById),
        //            "Contract",
        //            new { id = r.Contract.Id },
        //            new ContractQuery(r.Contract)),
        //    PartnerNotFoundError => NotFound(new { partnerExternalId }),
        //    SellingPlanNotFoundError => NotFound(new { sellingPlanId = command.SellingPlanId }),
        //    PartnerUnderContractError e => Conflict(
        //        new
        //        {
        //            error = "Partner is currently already under contract.",
        //            contract = e.Contract
        //        }),
        //    ContractEndDateBeforeCurrentDate => BadRequest(
        //        new
        //        {
        //            error = "Contract end date must be after current date.",
        //            endDate = command.EndDate
        //        }),
        //    _ => StatusCode(500)
        //};
    }

    [HttpPost("/api/partners/{partnerExternalId}/indefinite-contracts")]
    public ActionResult<ContractDto> SignPartnerForIndefiniteContract(
        string partnerExternalId, CreateIndefiniteContractRequest command)
    {
        var signResult = _contractUseCases.SignPartnerIndefinitely(
            partnerExternalId, command.SellingPlanId);

        return signResult.ToActionResult(this);

        //return signResult switch
        //{
        //    ContractSuccessResult r =>
        //        CreatedAtAction(
        //            nameof(GetById),
        //            "Contract",
        //            new { id = r.Contract.Id },
        //            new ContractQuery(r.Contract)),
        //    PartnerNotFoundError => NotFound(new { partnerExternalId }),
        //    SellingPlanNotFoundError => NotFound(new { sellingPlanId = command.SellingPlanId }),
        //    PartnerUnderContractError e => Conflict(
        //        new
        //        {
        //            error = "Partner is currently already under contract.",
        //            contract = e.Contract
        //        }),
        //    _ => StatusCode(500)
        //};
    }

    [HttpPut("/api/partners/{partnerExternalId}/indefinite-contracts/current")]
    public IActionResult EndCurrentIndefiniteContract(
        string partnerExternalId, EndIndefiniteContractRequest command)
    {
        if (!command.Ended)
            return NoContent();

        var endResult = _contractUseCases
            .EndCurrentIndefiniteContractWithPartner(partnerExternalId);

        return endResult.ToActionResult(this);

        //return endResult switch
        //{
        //    ContractSuccessResult r => new ContractQuery(r.Contract!),
        //    PartnerNotFoundError => NotFound(new { partnerExternalId }),
        //    ContractNotFoundError => Conflict(
        //        new
        //        {
        //            error = "Partner is not currently under any contract.",
        //            contracts = Array.Empty<Contract>()
        //        }),
        //    ContractNotIndefiniteError =>
        //        Conflict(new { error = "Current contract is not indefinite." }),
        //    _ => StatusCode(500)
        //};
    }

    [HttpPatch("/api/partners/{partnerExternalId}/fixed-period-contracts/current")]
    public IActionResult ExtendCurrentFixedPeriodContract(
        string partnerExternalId, ContractExtensionRequest extension)
    {
        var extendResult = _contractUseCases
            .ExtendCurrentFixedPeriodContractWithPartner(
                partnerExternalId, extension.ExtendedEndDate);

        return extendResult.ToActionResult(this);

        //return extendResult switch
        //{
        //    ContractSuccessResult r => new ContractQuery(r.Contract!),
        //    PartnerNotFoundError => NotFound(new { partnerExternalId }),
        //    ContractNotFoundError => Conflict(
        //        new
        //        {
        //            error = "Partner is not currently under any contract.",
        //            contracts = Array.Empty<Contract>()
        //        }),
        //    EndDateNotAfterOldEndDateError => BadRequest(
        //        new
        //        {
        //            error = "Extended end date must be after old end date.",
        //            extendedEndDate = extension.ExtendedEndDate
        //        }),
        //    ContractNotFixedPeriodError => Conflict(
        //        new { error = "Current contract is not fixed-period." }),
        //    _ => StatusCode(500)
        //};
    }
}

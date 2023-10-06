namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly ContractManager _contractManager;

    public ContractController(ContractManager contractManager)
    {
        _contractManager = contractManager;
    }

    [HttpGet("{id}")]
    public ActionResult<ContractQuery> GetById(int id)
    {
        var contract = _contractManager.GetById(id);
        if (contract == null)
        {
            return NotFound();
        }

        return new ContractQuery(contract);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ContractQuery>> GetByPartnerId(
        string partnerId)
    {
        return _contractManager
            .GetByPartnerExternalId(partnerId)
            .Select(c => new ContractQuery(c))
            .ToList();
    }

    [HttpPost("/api/partners/{partnerExternalId}/fixed-period-contracts")]
    public ActionResult<ContractQuery> SignUpForFixedPeriodContract(
        [FromRoute] string partnerExternalId, [FromBody] FixedPeriodContractCreateCommand command)
    {
        var signResult = _contractManager.SignPartnerForFixedPeriod(
            partnerExternalId, command.SellingPlanId, command.EndDate);

        return signResult switch
        {
            ContractSuccessResult r =>
                CreatedAtAction(
                    nameof(GetById),
                    "Contract",
                    new { id = r.Contract.Id },
                    new ContractQuery(r.Contract)),
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
            ContractSellingPlanNotFoundError => NotFound(new { command.SellingPlanId }),
            PartnerUnderContractError => Conflict("Partner is currently already under contract."),
            ContractEndDateBeforeCurrentDate =>
                BadRequest(new { error = "Contract end date must be after current date." }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("/api/partners/{partnerExternalId}/indefinite-contracts")]
    public ActionResult<ContractQuery> SignUpForIndefiniteContract(
        [FromRoute] string partnerExternalId, [FromBody] IndefiniteContractCreateCommand command)
    {
        var signResult = _contractManager.SignPartnerIndefinitely(
            partnerExternalId, command.SellingPlanId);

        return signResult switch
        {
            ContractSuccessResult r =>
                CreatedAtAction(
                    nameof(GetById),
                    "Contract",
                    new { id = r.Contract.Id },
                    new ContractQuery(r.Contract)),
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
            ContractSellingPlanNotFoundError => NotFound(new { command.SellingPlanId }),
            PartnerUnderContractError => Conflict("Partner is currently already under contract."),
            _ => StatusCode(500)
        };
    }

    [HttpPut("/api/partners/{partnerExternalId}/indefinite-contracts/current")]
    public ActionResult<ContractQuery> EndCurrentIndefiniteContract(
        string partnerExternalId, IndefiniteContractEndCommand command)
    {
        if (!command.Ended)
            return NoContent();

        var endResult = _contractManager
            .EndCurrentIndefiniteContractWithPartner(partnerExternalId);

        return endResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
            PartnerNotFoundError => NotFound(new { error = "Partner not found." }),
            ContractNotFoundError => NotFound(new { error = "Partner has no contract." }),
            ContractNotIndefiniteError =>
                BadRequest(new { error = "Partner currently has a contract, but it is not indefinite." }),
            _ => StatusCode(500)
        };
    }

    [HttpPatch("/api/partners/{partnerExternalId}/fixed-period-contracts/current")]
    public ActionResult<ContractQuery> ExtendFixedPeriodContract(
        string partnerExternalId, ContractExtension extension)
    {
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                partnerExternalId, extension.ExtendedEndDate);

        return extendResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
            PartnerNotFoundError => NotFound(new { error = "Partner not found." }),
            ContractNotFoundError => NotFound(new { error = "Partner has no contract." }),
            EndDateNotAfterOldEndDateError => BadRequest(
                new { error = "Extended end date must be after old end date." }),
            ContractNotFixedPeriodError => Conflict(
                new { error = "Contract is not fixed-period." }),
            _ => StatusCode(500)
        };
    }
}

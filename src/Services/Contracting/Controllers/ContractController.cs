namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IContractManager _contractManager;

    public ContractController(IContractManager contractManager)
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
    public ActionResult<ContractQuery> SignPartnerForFixedPeriodContract(
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
            SellingPlanNotFoundError => NotFound(new { sellingPlanId = command.SellingPlanId }),
            PartnerUnderContractError e => Conflict(
                new
                {
                    error = "Partner is currently already under contract.",
                    contract = e.Contract
                }),
            ContractEndDateBeforeCurrentDate => BadRequest(
                new
                {
                    error = "Contract end date must be after current date.",
                    endDate = command.EndDate
                }),
            _ => StatusCode(500)
        };
    }

    [HttpPost("/api/partners/{partnerExternalId}/indefinite-contracts")]
    public ActionResult<ContractQuery> SignPartnerForIndefiniteContract(
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
            SellingPlanNotFoundError => NotFound(new { sellingPlanId = command.SellingPlanId }),
            PartnerUnderContractError e => Conflict(
                new
                {
                    error = "Partner is currently already under contract.",
                    contract = e.Contract
                }),
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
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
            ContractNotFoundError => Conflict(
                new
                {
                    error = "Partner is not currently under any contract.",
                    contracts = Array.Empty<Contract>()
                }),
            ContractNotIndefiniteError =>
                Conflict(new { error = "Current contract is not indefinite." }),
            _ => StatusCode(500)
        };
    }

    [HttpPatch("/api/partners/{partnerExternalId}/fixed-period-contracts/current")]
    public ActionResult<ContractQuery> ExtendCurrentFixedPeriodContract(
        string partnerExternalId, ContractExtension extension)
    {
        var extendResult = _contractManager
            .ExtendCurrentFixedPeriodContractWithPartner(
                partnerExternalId, extension.ExtendedEndDate);

        return extendResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
            ContractNotFoundError => Conflict(
                new
                {
                    error = "Partner is not currently under any contract.",
                    contracts = Array.Empty<Contract>()
                }),
            EndDateNotAfterOldEndDateError => BadRequest(
                new
                {
                    error = "Extended end date must be after old end date.",
                    extendedEndDate = extension.ExtendedEndDate
                }),
            ContractNotFixedPeriodError => Conflict(
                new { error = "Current contract is not fixed-period." }),
            _ => StatusCode(500)
        };
    }
}

namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IPartnerRepository _partnerRepo;
    private readonly IContractRepository _contractRepo;

    public ContractController(IContractRepository contractRepo, IPartnerRepository partnerRepo)
    {
        _contractRepo = contractRepo;
        _partnerRepo = partnerRepo;
    }

    [HttpGet("{id}")]
    public ActionResult<ContractQuery> GetById(int id)
    {
        var contract = _contractRepo.GetById(id);
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
        return _contractRepo
            .GetByPartnerId(partnerId)
            .Select(c => new ContractQuery(c))
            .ToList();
    }

    [HttpPost("/api/partners/{partnerExternalId}/fixed-period-contracts")]
    public ActionResult<ContractQuery> SignUpForFixedPeriodContract(
        [FromRoute] string partnerExternalId, [FromBody] FixedPeriodContractCreateCommand command)
    {
        var partner = _partnerRepo.GetByExternalId(partnerExternalId);
        if (partner == null)
            return NotFound(partnerExternalId);

        var contract = command.ToContract(partner.Id);
        var signUpResult = _contractRepo.CreateFixedPeriod(contract);

        return signUpResult switch
        {
            ContractSuccessResult =>
                CreatedAtAction(
                    nameof(GetById),
                    "Contract",
                    new { id = contract.Id },
                    new ContractQuery(contract)),
            ContractStartDateInPastOrAfterEndDateError =>
                BadRequest(new { error = "Contract end date must be after current date." }),
            SellingPlanNotFoundError => NotFound(new { command.SellingPlanId }),
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
            PartnerUnderContractError => Conflict("Partner is currently already under contract."),
            _ => StatusCode(500)
        };
    }

    [HttpPost("/api/partners/{partnerExternalId}/indefinite-contracts")]
    public ActionResult<ContractQuery> SignUpForIndefiniteContract(
        [FromRoute] string partnerExternalId, [FromBody] IndefiniteContractCreateCommand command)
    {
        var partner = _partnerRepo.GetByExternalId(partnerExternalId);
        if (partner == null)
            return NotFound(partnerExternalId);

        var contract = command.ToContract(partner.Id);
        var signUpResult = _contractRepo.CreateIndefinite(contract);

        return signUpResult switch
        {
            ContractSuccessResult =>
                CreatedAtAction(
                    nameof(GetById),
                    "Contract",
                    new { id = contract.Id },
                    new ContractQuery(contract)),
            ContractStartDateInPastOrAfterEndDateError =>
                BadRequest(new { error = "Contract start date must be from now on and must be before end date." }),
            SellingPlanNotFoundError => NotFound(new { command.SellingPlanId }),
            PartnerNotFoundError => NotFound(new { partnerExternalId }),
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

        var partner = _partnerRepo.GetByExternalId(partnerExternalId);
        if (partner is null)
            return NotFound(partnerExternalId);

        var endContractResult = _contractRepo.EndIndefiniteContract(partner.Id);
        return endContractResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
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
        var partner = _partnerRepo.GetByExternalId(partnerExternalId);
        if (partner is null)
            return NotFound(new { partnerExternalId });

        var contract = partner.CurrentContract;
        if (contract is null)
            return NotFound(new { error = "Partner has no current contract." });

        var extendResult = _contractRepo.ExtendFixedPeriodContract(
            contract.Id, extension.ExtendedEndDate);

        return extendResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
            EndDateNotAfterOldEndDateError => BadRequest(
                new { error = "Extended end date must be after old end date." }),
            ContractNotFixedPeriodError => Conflict(
                new { error = "Contract is not fixed-period." }),
            ContractEndedError => Conflict(
                new { error = "Contract ended." }),
            _ => StatusCode(500)
        };
    }
}

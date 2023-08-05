namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PartnerController : ControllerBase
{
    private readonly IPartnerRepository _partnerRepo;
    private readonly IContractRepository _contractRepo;

    public PartnerController(IPartnerRepository partnerRepo, IContractRepository contractRepo)
    {
        _partnerRepo = partnerRepo;
        _contractRepo = contractRepo;
    }

    [HttpGet("{id}")]
    public ActionResult<PartnerQuery> GetById(int id)
    {
        var partner = _partnerRepo.GetById(id);
        if (partner == null)
        {
            return NotFound();
        }
        return new PartnerQuery(partner);
    }

    [HttpGet("/api/partners-by-eid/{externalId}")]
    public ActionResult<PartnerQuery> GetByExternalId(string externalId)
    {
        var partner = _partnerRepo.GetByExternalId(externalId);
        if (partner == null)
        {
            return NotFound();
        }
        return new PartnerQuery(partner);
    }

    [HttpPost]
    public ActionResult<PartnerQuery> CreateNew(PartnerWriteCommand createCommand)
    {
        var created = _partnerRepo.Create(createCommand.ToPartnerInfo());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new PartnerQuery(created));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateInfo(int id, PartnerWriteCommand updateCommand)
    {
        var update = updateCommand.ToPartnerInfo();
        update.Id = id;

        if (!_partnerRepo.UpdateInfo(update))
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPost("{partnerId}/fixed-period-contracts")]
    public IActionResult SignUpForFixedPeriodContract(
        [FromRoute] int partnerId, [FromBody] ContractCreateFixedPeriodCommand createCommand)
    {
        var partner = _partnerRepo.GetById(partnerId);
        if (partner == null)
        {
            return NotFound(partnerId);
        }

        var contract = createCommand.ToContractInfo();
        contract.PartnerId = partnerId;
        var signUpResult = _contractRepo.CreateFixedPeriod(contract);

        return signUpResult switch
        {
            ContractSuccessResult =>
                CreatedAtAction(
                    nameof(ContractController.GetById),
                    "Contract",
                    new { id = contract.Id },
                    new ContractQuery(contract)),
            ContractStartDateInPastOrAfterEndDateError =>
                BadRequest(new { error = "Contract end date must be after current date." }),
            PartnerNotFoundError => NotFound(new { partnerId }),
            PartnerUnderContractError => Conflict("Partner is currently already under contract."),
            _ => StatusCode(500)
        };
    }

    [HttpPost("{partnerId}/indefinite-contracts")]
    public IActionResult SignUpForIndefiniteContract(
        [FromRoute] int partnerId, [FromBody] ContractCreateIndefiniteCommand createCommand)
    {
        var partner = _partnerRepo.GetById(partnerId);
        if (partner == null)
        {
            return NotFound(partnerId);
        }

        var contract = createCommand.ToContractInfo();
        contract.PartnerId = partnerId;
        var signUpResult = _contractRepo.CreateIndefinite(contract);

        return signUpResult switch
        {
            ContractSuccessResult =>
                CreatedAtAction(
                    nameof(ContractController.GetById),
                    "Contract",
                    new { id = contract.Id },
                    new ContractQuery(contract)),
            ContractStartDateInPastOrAfterEndDateError =>
                BadRequest(new { error = "Contract start date must be from now on and must be before end date." }),
            PartnerNotFoundError => NotFound(new { partnerId }),
            PartnerUnderContractError => Conflict("Partner is currently already under contract."),
            _ => StatusCode(500)
        };
    }

    [HttpPut("{partnerId}/indefinite-contracts/end-current")]
    public ActionResult<ContractQuery> EndCurrentIndefiniteContract(int partnerId)
    {
        var endContractResult = _contractRepo.EndIndefiniteContract(partnerId);
        return endContractResult switch
        {
            ContractSuccessResult r => new ContractQuery(r.Contract!),
            PartnerNotFoundError => NotFound(new { partnerId }),
            ContractNotFoundError => NotFound(new { error = "Partner has no contract." }),
            ContractNotIndefiniteError =>
                BadRequest(new { error = "Partner currently has a contract, but it is not indefinite." }),
            _ => StatusCode(500)
        };
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_partnerRepo.Delete(id))
        {
            return NotFound(id);
        }
        return Ok();
    }
}
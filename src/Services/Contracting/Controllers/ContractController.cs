namespace Bazaar.Contracting.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IContractRepository _contractRepo;

    public ContractController(IContractRepository contractRepo)
    {
        _contractRepo = contractRepo;
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
    public ActionResult<IEnumerable<ContractQuery>> GetByPartnerId([FromQuery] string partnerId)
    {
        if (string.IsNullOrWhiteSpace(partnerId))
        {
            return BadRequest("Partner ID must be specified.");
        }

        return _contractRepo
                .GetByPartnerId(partnerId)
                .Select(c => new ContractQuery(c))
                .ToList();
    }
}

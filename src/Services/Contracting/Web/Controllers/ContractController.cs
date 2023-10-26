namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IContractRepository _contractRepo;

    public ContractController(IContractRepository contractRepository)
    {
        _contractRepo = contractRepository;
    }

    [HttpGet("{id}")]
    public ActionResult<ContractResponse> GetById(int id)
    {
        var contract = _contractRepo.GetById(id);
        if (contract == null)
            return NotFound();

        return new ContractResponse(contract);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ContractResponse>> GetByClientId(
        string clientId)
    {
        return _contractRepo
            .GetByClientExternalId(clientId)
            .Select(c => new ContractResponse(c))
            .ToList();
    }
}

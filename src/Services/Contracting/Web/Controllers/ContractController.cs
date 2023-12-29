namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ContractController : ControllerBase
{
    private readonly IRepository<Contract> _contractRepo;

    public ContractController(IRepository<Contract> contractRepository)
    {
        _contractRepo = contractRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContractResponse>> GetById(int id)
    {
        var contract = await _contractRepo.GetByIdAsync(id);
        if (contract == null)
            return NotFound();

        return new ContractResponse(contract);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractResponse>>> GetByClientId(
        string clientId)
    {
        return (await _contractRepo
            .ListAsync(new ContractsByClientExternalIdSpec(clientId)))
            .Select(c => new ContractResponse(c))
            .ToList();
    }
}

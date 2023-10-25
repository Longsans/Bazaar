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
            return NotFound();

        return new ContractDto(contract);
    }

    [HttpGet]
    public ActionResult<IEnumerable<ContractDto>> GetByClientId(
        string clientId)
    {
        return _contractUseCases
            .GetByClientExternalId(clientId)
            .Select(c => new ContractDto(c))
            .ToList();
    }

    [HttpPost("/api/clients/{clientExternalId}/contracts")]
    public ActionResult<ContractDto> SignContractWithClient(
        string clientExternalId, SignContractRequest command)
    {
        var signResult = _contractUseCases.SignClient(
            clientExternalId, command.SellingPlanId);

        return signResult.ToActionResult(this);
    }

    [HttpPut("/api/clients/{clientExternalId}/contracts/current")]
    public IActionResult EndCurrentContractWithClient(
        string clientExternalId, EndContractRequest command)
    {
        if (!command.Ended)
            return NoContent();

        var endResult = _contractUseCases
            .EndCurrentContractWithClient(clientExternalId);

        return endResult.ToActionResult(this);
    }
}

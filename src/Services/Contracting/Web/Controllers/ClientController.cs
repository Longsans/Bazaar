namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientUseCases _clientUseCases;

    public ClientController(IClientUseCases clientUseCases)
    {
        _clientUseCases = clientUseCases;
    }

    [HttpGet("{id}")]
    public ActionResult<ClientResponse> GetById(int id)
    {
        var client = _clientUseCases.GetWithContractsById(id);
        if (client == null)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpGet]
    public ActionResult<ClientResponse> GetByExternalId(string externalId)
    {
        var client = _clientUseCases.GetWithContractsByExternalId(externalId);
        if (client == null)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpPost]
    public ActionResult<ClientDto> Register(RegisterClientRequest command)
    {
        var clientDto = new ClientDto()
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            EmailAddress = command.Email,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Gender = command.Gender
        };

        var registerResult = _clientUseCases.RegisterClient(clientDto);
        return registerResult.ToActionResult(this);
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdateInfo(string externalId, UpdateClientInfoRequest command)
    {
        var clientUpdateDto = new ClientDto()
        {
            ExternalId = externalId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PhoneNumber = command.PhoneNumber,
            DateOfBirth = command.DateOfBirth,
            Gender = command.Gender
        };

        var updateResult = _clientUseCases
            .UpdateClientInfoByExternalId(clientUpdateDto);

        return updateResult.ToActionResult(this);
    }

    [HttpPut("{externalId}/email-address")]
    public IActionResult UpdateEmailAddress(string externalId, string emailAddress)
    {
        return _clientUseCases
            .UpdateClientEmailAddress(externalId, emailAddress)
            .ToActionResult(this);
    }
}
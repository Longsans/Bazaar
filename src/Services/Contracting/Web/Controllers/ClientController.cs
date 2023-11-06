namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientRepository _clientRepo;
    private readonly ISellingPlanRepository _planRepo;
    private readonly IUpdateClientEmailAddressService _updateEmailAddressService;
    private readonly ICloseClientAccountService _closeClientAccountService;

    public ClientController(
        IClientRepository clientRepo,
        ISellingPlanRepository planRepo,
        IUpdateClientEmailAddressService updateEmailAddressService,
        ICloseClientAccountService closeClientAccountService)
    {
        _clientRepo = clientRepo;
        _planRepo = planRepo;
        _updateEmailAddressService = updateEmailAddressService;
        _closeClientAccountService = closeClientAccountService;
    }

    [HttpGet("{id}")]
    public ActionResult<ClientResponse> GetById(int id)
    {
        var client = _clientRepo.GetWithContractsAndPlanById(id);
        if (client == null || client.IsAccountClosed)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpGet]
    public ActionResult<ClientResponse> GetByExternalId(string externalId)
    {
        var client = _clientRepo
            .GetWithContractsAndPlanByExternalId(externalId);

        if (client == null || client.IsAccountClosed)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpPost]
    public ActionResult<ClientResponse> Register(RegisterClientRequest request)
    {
        var sellingPlan = _planRepo.GetById(request.SellingPlanId);
        if (sellingPlan == null)
            return NotFound(new { error = "Selling plan not found." });

        var existingEmailAddressOwner = _clientRepo
            .GetWithContractsAndPlanByEmailAddress(request.EmailAddress);

        if (existingEmailAddressOwner != null)
        {
            return Conflict(new
            {
                error = ClientCompliance
                    .UniqueEmailAddressNonComplianceMessage
            });
        }

        try
        {
            var client = new Client(request.FirstName, request.LastName,
                request.EmailAddress, request.PhoneNumber,
                request.DateOfBirth, request.Gender, sellingPlan.Id);

            _clientRepo.Create(client);
            return new ClientResponse(client);
        }
        catch (ClientUnderMinimumAgeException)
        {
            return BadRequest(new { error = ClientCompliance.ClientMinimumAgeStatement });
        }
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdateInfo(string externalId, UpdateClientInfoRequest request)
    {
        var client = _clientRepo
            .GetWithContractsAndPlanByExternalId(externalId);
        if (client == null || client.IsAccountClosed)
        {
            return NotFound(new { error = "Client not found." });
        }

        try
        {
            client.ChangePersonalInfo(
                request.FirstName, request.LastName,
                request.PhoneNumber, request.DateOfBirth, request.Gender);
        }
        catch (ClientUnderMinimumAgeException)
        {
            return BadRequest(new { error = ClientCompliance.ClientMinimumAgeStatement });
        }

        _clientRepo.Update(client);
        return Ok();
    }

    [HttpPut("{externalId}/email-address")]
    public IActionResult UpdateEmailAddress(string externalId, string newEmailAddress)
    {
        return _updateEmailAddressService
            .UpdateClientEmailAddress(externalId, newEmailAddress)
            .ToActionResult(this);
    }

    [HttpPut("{externalId}/selling-plan")]
    public ActionResult<ClientResponse> ChangeSellingPlan(
        string externalId, ChangeSellingPlanRequest request)
    {
        var client = _clientRepo.GetWithContractsAndPlanByExternalId(externalId);
        if (client is null || client.IsAccountClosed)
            return NotFound(new { error = "Client not found." });

        var sellingPlan = _planRepo.GetById(request.SellingPlanId);
        if (sellingPlan is null)
            return NotFound(new { error = "Selling plan not found." });

        if (sellingPlan.Id == client.SellingPlanId)
            return NoContent();

        client.ChangeSellingPlan(sellingPlan);
        _clientRepo.Update(client);

        return new ClientResponse(client);
    }

    [HttpDelete("{externalId}")]
    public IActionResult CloseAccount(string externalId)
    {
        return _closeClientAccountService.CloseAccount(externalId)
            .ToActionResult(this);
    }
}
namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IRepository<Client> _clientRepo;
    private readonly IRepository<SellingPlan> _planRepo;
    private readonly UpdateClientEmailAddressService _updateEmailAddressService;
    private readonly CloseClientAccountService _closeClientAccountService;

    public ClientController(
        IRepository<Client> clientRepo,
        IRepository<SellingPlan> planRepo,
        UpdateClientEmailAddressService updateEmailAddressService,
        CloseClientAccountService closeClientAccountService)
    {
        _clientRepo = clientRepo;
        _planRepo = planRepo;
        _updateEmailAddressService = updateEmailAddressService;
        _closeClientAccountService = closeClientAccountService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponse>> GetById(int id)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(new ClientByIdSpec(id));
        if (client == null)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpGet]
    public async Task<ActionResult<ClientResponse>> GetByExternalId(string externalId)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(new ClientByExternalIdSpec(externalId));
        if (client == null)
            return NotFound();

        return new ClientResponse(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Register(RegisterClientRequest request)
    {
        var sellingPlan = await _planRepo.GetByIdAsync(request.SellingPlanId);
        if (sellingPlan == null)
            return NotFound(new { error = "Selling plan not found." });

        var existingEmailAddressOwner = await _clientRepo
            .SingleOrDefaultAsync(new ClientByEmailAddressSpec(request.EmailAddress));
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

            await _clientRepo.AddAsync(client);
            return new ClientResponse(client);
        }
        catch (ClientUnderMinimumAgeException)
        {
            return BadRequest(new { error = ClientCompliance.ClientMinimumAgeStatement });
        }
    }

    [HttpPut("{externalId}")]
    public async Task<IActionResult> UpdateInfo(string externalId, UpdateClientInfoRequest request)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(new ClientByExternalIdSpec(externalId));
        if (client == null)
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

        await _clientRepo.UpdateAsync(client);
        return Ok();
    }

    [HttpPut("{externalId}/email-address")]
    public async Task<IActionResult> UpdateEmailAddress(string externalId, string newEmailAddress)
    {
        return (await _updateEmailAddressService
            .UpdateClientEmailAddress(externalId, newEmailAddress))
            .ToActionResult(this);
    }

    [HttpPut("{externalId}/selling-plan")]
    public async Task<ActionResult<ClientResponse>> ChangeSellingPlan(
        string externalId, ChangeSellingPlanRequest request)
    {
        var client = await _clientRepo.SingleOrDefaultAsync(new ClientByExternalIdSpec(externalId));
        if (client is null || client.IsAccountClosed)
            return NotFound(new { error = "Client not found." });

        var sellingPlan = await _planRepo.GetByIdAsync(request.SellingPlanId);
        if (sellingPlan is null)
            return NotFound(new { error = "Selling plan not found." });

        if (sellingPlan.Id == client.SellingPlanId)
            return NoContent();

        client.ChangeSellingPlan(sellingPlan);
        await _clientRepo.UpdateAsync(client);

        return new ClientResponse(client);
    }

    [HttpDelete("{externalId}")]
    public async Task<IActionResult> CloseAccount(string externalId)
    {
        return (await _closeClientAccountService.CloseAccount(externalId))
            .ToActionResult(this);
    }
}
namespace WebShoppingUI.Controllers;

[ApiController]
public class ContractingController : ControllerBase
{
    private readonly IContractingService _contractingService;

    public ContractingController(IContractingService contractingService)
    {
        _contractingService = contractingService;
    }

    [HttpGet("/api/sellers/{sellerId}")]
    public async Task<ActionResult<Seller>> GetBySellerId(string sellerId)
    {
        var clientResult = await _contractingService.GetSellerById(sellerId);
        return clientResult.Map(x => new Seller(
            x.ExternalId, x.FirstName, x.LastName, x.Email, x.PhoneNumber, x.DateOfBirth, x.Gender)).ToActionResult();
    }
}

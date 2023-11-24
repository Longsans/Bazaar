namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LotsController : ControllerBase
{
    private readonly IRemovalService _removalService;

    public LotsController(IRemovalService removalService)
    {
        _removalService = removalService;
    }

    [HttpPatch("removal-requests")]
    public IActionResult RequestRemovalForLots(LotsRemovalRequest request)
    {
        if (request.RemovalMethod == RemovalMethod.Return
            && string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return BadRequest(new { error = "Delivery address must be specified for return." });
        }

        return (request.RemovalMethod == RemovalMethod.Return
            ? _removalService.RequestReturnForLots(request.LotNumbers, request.DeliveryAddress)
            : _removalService.RequestDisposalForLots(request.LotNumbers))
            .ToActionResult(this);
    }

    [HttpPatch("unf-disposal-requests")]
    public IActionResult RequestDisposalForLotsUnfulfillableBeyondPolicyDuration(
        LotsUnfulfillableBeyondPolicyDurationDisposalRequest request)
    {
        if (request.DisposeLotsUnfulfillableBeyondPolicyDuration)
        {
            _removalService
                .RequestDisposalForLotsUnfulfillableBeyondPolicyDuration();
        }
        return NoContent();
    }
}

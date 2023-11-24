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
        return _removalService
            .RequestRemovalForLots(request.LotNumbers, request.RemovalMethod)
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

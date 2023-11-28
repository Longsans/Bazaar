namespace Bazaar.Transport.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryReturnsController : ControllerBase
{
    private readonly IInventoryReturnRepository _returnRepo;
    private readonly InventoryReturnProcessService _returnConclusionService;

    public InventoryReturnsController(IInventoryReturnRepository returnRepo,
        InventoryReturnProcessService returnConclusionService)
    {
        _returnRepo = returnRepo;
        _returnConclusionService = returnConclusionService;
    }

    [HttpGet("{id}")]
    public ActionResult<InventoryReturnResponse> GetById(int id)
    {
        var invReturn = _returnRepo.GetById(id);
        if (invReturn == null)
        {
            return NotFound();
        }

        return new InventoryReturnResponse(invReturn);
    }

    [HttpPatch("{returnId}/status")]
    public IActionResult UpdateStatus(int returnId,
        UpdateInventoryReturnStatusRequest request)
    {
        if (request.Status == DeliveryStatus.Cancelled
            && string.IsNullOrWhiteSpace(request.CancelReason))
        {
            return BadRequest(new { error = "Reason cannot be empty when cancelling return." });
        }

        return (request.Status switch
        {
            DeliveryStatus.Delivering => _returnConclusionService.StartReturnDelivery(returnId),
            DeliveryStatus.Completed => _returnConclusionService.CompleteInventoryReturn(returnId),
            DeliveryStatus.Postponed => _returnConclusionService.PostponeInventoryReturn(returnId),
            DeliveryStatus.Cancelled => _returnConclusionService
                .CancelInventoryReturn(returnId, request.CancelReason),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid status for inventory return."
            })
        }).ToActionResult(this);
    }
}

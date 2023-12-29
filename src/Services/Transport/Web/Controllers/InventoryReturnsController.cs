namespace Bazaar.Transport.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryReturnsController : ControllerBase
{
    private readonly IRepository<InventoryReturn> _returnRepo;
    private readonly InventoryReturnProcessService _returnConclusionService;

    public InventoryReturnsController(IRepository<InventoryReturn> returnRepo,
        InventoryReturnProcessService returnConclusionService)
    {
        _returnRepo = returnRepo;
        _returnConclusionService = returnConclusionService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryReturnResponse>> GetById(int id)
    {
        var invReturn = await _returnRepo
            .SingleOrDefaultAsync(new ReturnByIdSpec(id));
        if (invReturn == null)
        {
            return NotFound();
        }

        return new InventoryReturnResponse(invReturn);
    }

    [HttpPatch("{returnId}")]
    public async Task<IActionResult> UpdateStatus(int returnId,
        UpdateInventoryReturnStatusRequest request)
    {
        if (request.Status == DeliveryStatus.Cancelled
            && string.IsNullOrWhiteSpace(request.CancelReason))
        {
            return BadRequest(new { error = "Reason cannot be empty when cancelling return." });
        }

        return (request.Status switch
        {
            DeliveryStatus.Delivering => await _returnConclusionService.StartReturnDelivery(returnId),
            DeliveryStatus.Completed => await _returnConclusionService.CompleteInventoryReturn(returnId),
            DeliveryStatus.Postponed => await _returnConclusionService.PostponeInventoryReturn(returnId),
            DeliveryStatus.Cancelled => await _returnConclusionService
                .CancelInventoryReturn(returnId, request.CancelReason!),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid status for inventory return."
            })
        }).ToActionResult(this);
    }
}

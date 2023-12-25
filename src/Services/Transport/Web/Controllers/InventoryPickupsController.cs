namespace Bazaar.Transport.Web.Controllers;

[Route("api/inventory-pickups")]
[ApiController]
public class InventoryPickupsController : ControllerBase
{
    private readonly IInventoryPickupRepository _pickupRepository;
    private readonly PickupProcessService _pickupProcessService;

    public InventoryPickupsController(
        IInventoryPickupRepository pickupRepository,
        PickupProcessService pickupProcessService)
    {
        _pickupRepository = pickupRepository;
        _pickupProcessService = pickupProcessService;
    }

    [HttpPost]
    public ActionResult<InventoryPickupResponse> SchedulePickup(
        ScheduleInventoryPickupRequest request)
    {
        if (request.InventoryItems.Any(x => x.NumberOfUnits == 0))
        {
            return BadRequest(new { error = "Number of stock units cannot be 0." });
        }

        var pickupItems = request.InventoryItems
            .Select(x => new ProductInventory(x.ProductId, x.NumberOfUnits));

        return _pickupProcessService.SchedulePickup(
            request.PickupLocation, pickupItems, request.SchedulerId)
            .Map(x => new InventoryPickupResponse(x))
            .ToActionResult(this);
    }

    [HttpGet("{id}")]
    public ActionResult<InventoryPickupResponse> GetById(int id)
    {
        var pickup = _pickupRepository.GetById(id);
        if (pickup == null)
        {
            return NotFound();
        }
        return new InventoryPickupResponse(pickup);
    }

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] UpdatePickupStatusRequest request)
    {
        return request.Status switch
        {
            InventoryPickupStatus.EnRouteToPickupLocation => _pickupProcessService.DispatchPickup(id).ToActionResult(this),
            InventoryPickupStatus.DeliveringToWarehouse => _pickupProcessService.ConfirmPickupInventory(id).ToActionResult(this),
            InventoryPickupStatus.Completed => _pickupProcessService.CompletePickup(id).ToActionResult(this),
            InventoryPickupStatus.Cancelled => request.CancelReason != null
                ? _pickupProcessService.CancelPickup(id, request.CancelReason).ToActionResult(this)
                : BadRequest(new { error = "Cancel reason cannot be empty." }),
            _ => Conflict(new { error = "Invalid status for inventory pickup." }),
        };
    }

    [HttpDelete]
    public IActionResult DeleteCancelledPickups(bool cancelled)
    {
        if (cancelled)
        {
            var cancelledPickups = _pickupRepository.GetAllCancelled();
            foreach (var pickup in cancelledPickups)
            {
                _pickupRepository.Delete(pickup);
            }
        }
        return NoContent();
    }
}

namespace Bazaar.Transport.Web.Controllers;

[Route("api/inventory-pickups")]
[ApiController]
public class InventoryPickupsController : ControllerBase
{
    private readonly Repository<InventoryPickup> _pickupRepository;
    private readonly PickupProcessService _pickupProcessService;

    public InventoryPickupsController(
        Repository<InventoryPickup> pickupRepository,
        PickupProcessService pickupProcessService)
    {
        _pickupRepository = pickupRepository;
        _pickupProcessService = pickupProcessService;
    }

    [HttpPost]
    public async Task<ActionResult<InventoryPickupResponse>> SchedulePickup(
        ScheduleInventoryPickupRequest request)
    {
        if (request.InventoryItems.Any(x => x.NumberOfUnits == 0))
        {
            return BadRequest(new { error = "Number of stock units cannot be 0." });
        }

        var pickupItems = request.InventoryItems
            .Select(x => new PickupProductStock(x.ProductId, x.NumberOfUnits));

        return (await _pickupProcessService.SchedulePickup(
            request.PickupLocation, pickupItems, request.SchedulerId))
            .Map(x => new InventoryPickupResponse(x))
            .ToActionResult(this);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryPickupResponse>> GetById(int id)
    {
        var pickup = await _pickupRepository
            .SingleOrDefaultAsync(new PickupByIdSpec(id));
        if (pickup == null)
        {
            return NotFound();
        }
        return new InventoryPickupResponse(pickup);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePickupStatusRequest request)
    {
        var result = request.Status switch
        {
            InventoryPickupStatus.EnRouteToPickupLocation => await _pickupProcessService.DispatchPickup(id),
            InventoryPickupStatus.DeliveringToWarehouse => await _pickupProcessService.ConfirmPickupInventory(id),
            InventoryPickupStatus.Completed => await _pickupProcessService.CompletePickup(id),
            InventoryPickupStatus.Cancelled => request.CancelReason != null
                ? await _pickupProcessService.CancelPickup(id, request.CancelReason)
                : Result.Invalid(new ValidationError
                {
                    Identifier = nameof(request.CancelReason),
                    ErrorMessage = "Cancel reason cannot be empty."
                }),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid pickup status."
            })
        };
        return result.ToActionResult(this);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCancelledPickups(bool cancelled)
    {
        if (cancelled)
        {
            var cancelledPickups = await _pickupRepository
                .ListAsync(new PickupsCancelledSpec());
            await _pickupRepository.DeleteRangeAsync(cancelledPickups);
        }
        return NoContent();
    }
}

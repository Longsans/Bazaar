using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Transport.Web.Controllers;

[Route("api/inventory-pickups")]
[ApiController]
public class InventoryPickupController : ControllerBase
{
    private readonly IInventoryPickupRepository _pickupRepository;
    private readonly ICompleteInventoryPickupService _completePickupService;
    private readonly IEstimationService _estimationService;

    public InventoryPickupController(
        IInventoryPickupRepository pickupRepository,
        ICompleteInventoryPickupService completePickupService,
        IEstimationService estimationService)
    {
        _pickupRepository = pickupRepository;
        _completePickupService = completePickupService;
        _estimationService = estimationService;
    }

    [HttpPost]
    public ActionResult<InventoryPickupResponse> SchedulePickup(
        ScheduleInventoryPickupRequest request)
    {
        var pickupItems = request.InventoryItems
            .Select(x => new ProductInventory(x.ProductId, x.NumberOfUnits));
        var estimatedPickupTime = _estimationService
            .EstimatePickupCompletion(pickupItems);

        var pickup = new InventoryPickup(
            request.PickupLocation,
            pickupItems,
            estimatedPickupTime,
            request.SchedulerId);

        _pickupRepository.Create(pickup);
        return new InventoryPickupResponse(pickup);
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
    public IActionResult UpdateStatus(int id, [FromBody] InventoryPickupStatus status)
    {
        if (status == InventoryPickupStatus.Completed)
        {
            return _completePickupService.CompleteInventoryPickup(id)
                .ToActionResult(this);
        }

        var pickup = _pickupRepository.GetById(id);
        if (pickup == null)
        {
            return NotFound();
        }

        try
        {
            switch (status)
            {
                case InventoryPickupStatus.EnRouteToPickupLocation:
                    pickup.StartPickup();
                    break;
                case InventoryPickupStatus.DeliveringToWarehouse:
                    pickup.ConfirmInventoryPickedUp();
                    break;
                case InventoryPickupStatus.Completed:
                    pickup.Complete();
                    break;
                default:
                    throw new InvalidOperationException("Invalid status for inventory pickup.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }

        _pickupRepository.Update(pickup);
        return Ok();
    }
}

namespace Bazaar.Transport.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeliveriesController : ControllerBase
{
    private readonly Repository<Delivery> _deliveryRepository;
    private readonly DeliveryProcessService _deliveryProcessService;

    public DeliveriesController(
        Repository<Delivery> deliveryRepository, DeliveryProcessService processService)
    {
        _deliveryRepository = deliveryRepository;
        _deliveryProcessService = processService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryResponse>> GetById(int id)
    {
        var delivery = await _deliveryRepository
            .SingleOrDefaultAsync(new DeliveryByIdSpec(id));
        if (delivery == null)
        {
            return NotFound();
        }
        return new DeliveryResponse(delivery);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryResponse>> Schedule(ScheduleDeliveryRequest request)
    {
        if (request.PackageItems.Any(x => x.Quantity == 0))
        {
            return BadRequest(new { error = "Package item quantity cannot be 0." });
        }

        var packageItems = request.PackageItems
            .Select(x => new DeliveryPackageItem(x.ProductId, x.Quantity));

        return (await _deliveryProcessService
            .ScheduleDelivery(request.OrderId, request.DeliveryAddress, packageItems))
            .Map(x => new DeliveryResponse(x))
            .ToActionResult(this);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] DeliveryStatus status)
    {
        var result = status switch
        {
            DeliveryStatus.Delivering => await _deliveryProcessService.StartDelivery(id),
            DeliveryStatus.Completed => await _deliveryProcessService.CompleteDelivery(id),
            DeliveryStatus.Postponed => await _deliveryProcessService.PostponeDelivery(id),
            DeliveryStatus.Cancelled => await _deliveryProcessService.CancelDelivery(id),
            _ => throw new InvalidOperationException("Invalid status for delivery.")
        };

        return result.ToActionResult(this);
    }
}

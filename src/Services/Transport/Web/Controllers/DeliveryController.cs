namespace Bazaar.Transport.Web.Controllers;

[Route("api/deliveries")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryProcessService _deliveryProcessService;

    public DeliveryController(
        IDeliveryRepository deliveryRepository, IDeliveryProcessService processService)
    {
        _deliveryRepository = deliveryRepository;
        _deliveryProcessService = processService;
    }

    [HttpGet("{id}")]
    public ActionResult<DeliveryResponse> GetById(int id)
    {
        var delivery = _deliveryRepository.GetById(id);
        if (delivery == null)
        {
            return NotFound();
        }
        return new DeliveryResponse(delivery);
    }

    [HttpPost]
    public ActionResult<DeliveryResponse> Schedule(ScheduleDeliveryRequest request)
    {
        if (request.PackageItems.Any(x => x.Quantity == 0))
        {
            return BadRequest(new { error = "Package item quantity cannot be 0." });
        }

        var packageItems = request.PackageItems
            .Select(x => new DeliveryPackageItem(x.ProductId, x.Quantity));

        return _deliveryProcessService
            .ScheduleDelivery(request.OrderId, request.DeliveryAddress, packageItems)
            .Map(x => new DeliveryResponse(x))
            .ToActionResult(this);
    }

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] DeliveryStatus status)
    {
        var result = status switch
        {
            DeliveryStatus.Delivering => _deliveryProcessService.StartDelivery(id),
            DeliveryStatus.Completed => _deliveryProcessService.CompleteDelivery(id),
            DeliveryStatus.Postponed => _deliveryProcessService.PostponeDelivery(id),
            DeliveryStatus.Cancelled => _deliveryProcessService.CancelDelivery(id),
            _ => throw new InvalidOperationException("Invalid status for delivery.")
        };

        return result.ToActionResult(this);
    }
}

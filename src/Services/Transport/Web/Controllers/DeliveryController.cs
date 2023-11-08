using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Transport.Web.Controllers;

[Route("api/deliveries")]
[ApiController]
public class DeliveryController : ControllerBase
{
    private readonly IDeliveryRepository _deliveryRepository;

    public DeliveryController(IDeliveryRepository deliveryRepository)
    {
        _deliveryRepository = deliveryRepository;
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

    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(int id, [FromBody] DeliveryStatus status)
    {
        var delivery = _deliveryRepository.GetById(id);
        if (delivery == null)
        {
            return NotFound();
        }

        try
        {
            switch (status)
            {
                case DeliveryStatus.Delivering:
                    delivery.StartDelivery();
                    break;
                case DeliveryStatus.Completed:
                    delivery.Complete();
                    break;
                case DeliveryStatus.Postponed:
                    delivery.Postpone();
                    break;
                default:
                    throw new InvalidOperationException("Invalid status for delivery.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }

        _deliveryRepository.Update(delivery);
        return Ok();
    }
}

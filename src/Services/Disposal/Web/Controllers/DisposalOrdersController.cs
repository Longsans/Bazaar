using Ardalis.Result.AspNetCore;
using Bazaar.Disposal.Web.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Disposal.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DisposalOrdersController(
    IDisposalOrderRepository disposalOrderRepo,
    DisposalOrderConclusionService conclusionService) : ControllerBase
{
    private readonly IDisposalOrderRepository _disposalOrderRepo = disposalOrderRepo;
    private readonly DisposalOrderConclusionService _disposalConclusionService = conclusionService;

    [HttpGet("{id}")]
    public ActionResult<DisposalOrderResponse> GetById(int id)
    {
        var disposalOrder = _disposalOrderRepo.GetById(id);
        if (disposalOrder == null)
        {
            return NotFound();
        }
        return new DisposalOrderResponse(disposalOrder);
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(int id, UpdateDisposalOrderStatusRequest request)
    {
        var disposalOrder = _disposalOrderRepo.GetById(id);
        if (disposalOrder == null)
        {
            return NotFound();
        }

        if (request.Status == DisposalStatus.Processing)
        {
            try
            {
                disposalOrder.StartProcessing();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }

            _disposalOrderRepo.Update(disposalOrder);
            return NoContent();
        }

        return (request.Status switch
        {
            DisposalStatus.Completed => _disposalConclusionService.CompleteDisposal(disposalOrder.Id),
            DisposalStatus.Cancelled => _disposalConclusionService.CancelDisposal(
                disposalOrder.Id, request.CancelReason),
            _ => Result.Invalid(new ValidationError
            {
                Identifier = nameof(request.Status),
                ErrorMessage = "Invalid status for disposal order."
            })
        }).ToActionResult(this);
    }
}

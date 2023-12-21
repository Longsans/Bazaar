namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LotsController : ControllerBase
{
    private readonly StockAdjustmentService _stockAdjustmentService;
    private readonly RemovalService _removalService;

    public LotsController(StockAdjustmentService stockAdjustmentService, RemovalService removalService)
    {
        _removalService = removalService;
        _stockAdjustmentService = stockAdjustmentService;
    }

    [HttpPatch]
    public IActionResult SendLotsUnfulfillableBeyondPolicyDurationForDisposal(
        bool unfulfillableBeyondPolicyDuration,
        LotsUnfulfillableBeyondPolicyDurationDisposalRequest request)
    {
        if (!unfulfillableBeyondPolicyDuration && request.SentForDisposal)
        {
            return BadRequest();
        }
        if (request.SentForDisposal)
        {
            _removalService
                .SendLotsUnfulfillableBeyondPolicyDurationForDisposal();
        }
        return NoContent();
    }

    [HttpPatch("{lotNumber}/unfulfillable-records")]
    public IActionResult RecordLotUnitsUnfulfillable(string lotNumber,
        RecordUnfulfillableStockRequest request)
    {
        return _stockAdjustmentService
            .MoveLotUnitsIntoUnfulfillableStock(
                lotNumber, request.Quantity, request.UnfulfillableCategory)
            .ToActionResult(this);
    }

    [HttpPost("/lot-adjustments")]
    public IActionResult AdjustLotUnits(IEnumerable<LotAdjustmentQuantityDto> adjustmentQuantities)
    {
        if (adjustmentQuantities.Any(x => x.Quantity == 0))
        {
            return BadRequest(new { error = "Quantity adjusted cannot be 0." });
        }
        return _stockAdjustmentService.AdjustStockInLots(adjustmentQuantities).ToActionResult(this);
    }
}

namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SellingPlanController : ControllerBase
{
    private readonly ISellingPlanUsecases _planUsecases;

    public SellingPlanController(ISellingPlanUsecases planUsecases)
    {
        _planUsecases = planUsecases;
    }

    [HttpGet("{id}")]
    public ActionResult<SellingPlanDto> GetById(int id)
    {
        var plan = _planUsecases.GetById(id);

        if (plan == null)
            return NotFound();

        return new SellingPlanDto(plan);
    }

    [HttpPost]
    public ActionResult<SellingPlanDto> CreatePlan(SellingPlanWriteRequest request)
    {
        var planDto = new SellingPlanDto()
        {
            Name = request.Name,
            MonthlyFee = request.MonthlyFee,
            PerSaleFee = request.PerSaleFee,
            RegularPerSaleFeePercent = request.RegularPerSaleFeePercent
        };

        var createResult = _planUsecases
            .CreateSellingPlan(planDto);

        return createResult.ToActionResult(this);
    }

    [HttpPut("{id}")]
    public ActionResult<SellingPlanDto> UpdatePlan(int id, SellingPlanWriteRequest request)
    {
        var planDto = new SellingPlanDto()
        {
            Id = id,
            Name = request.Name,
            MonthlyFee = request.MonthlyFee,
            PerSaleFee = request.PerSaleFee,
            RegularPerSaleFeePercent = request.RegularPerSaleFeePercent
        };

        var updateResult = _planUsecases
            .UpdateSellingPlan(planDto);

        return updateResult.ToActionResult(this);
    }
}

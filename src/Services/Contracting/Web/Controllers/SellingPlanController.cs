namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SellingPlanController : ControllerBase
{
    private readonly ISellingPlanRepository _planRepo;

    public SellingPlanController(ISellingPlanRepository planRepository)
    {
        _planRepo = planRepository;
    }

    [HttpGet("{id}")]
    public ActionResult<SellingPlan> GetById(int id)
    {
        var plan = _planRepo.GetById(id);

        if (plan == null)
            return NotFound();

        return plan;
    }

    [HttpPost]
    public ActionResult<SellingPlan> CreatePlan(SellingPlanRequest request)
    {
        try
        {
            var plan = new SellingPlan(request.Name, request.MonthlyFee,
                request.PerSaleFee, request.RegularPerSaleFeePercent);
            _planRepo.Create(plan);
            return plan;
        }
        catch (MonthlyAndPerSaleFeesEqualZeroException)
        {
            return BadRequest(new
            {
                error = PlanRequirements
                    .MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement
            });
        }
        catch (RegularPerSaleFeePercentNotPositiveException)
        {
            return BadRequest(new
            {
                error = PlanRequirements
                    .RegularPerSaleFeePercentGreaterThanZeroStatement
            });
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePlan(int id, SellingPlanRequest request)
    {
        var plan = _planRepo.GetById(id);
        if (plan == null)
            return NotFound(new { error = "Selling plan not found." });

        plan.Name = request.Name;
        try
        {
            plan.ChangeFees(
                request.MonthlyFee,
                request.PerSaleFee,
                request.RegularPerSaleFeePercent);
        }
        catch (MonthlyAndPerSaleFeesEqualZeroException)
        {
            return BadRequest(new
            {
                error = PlanRequirements
                    .MonthlyFeeOrPerSaleFeeGreaterThanZeroStatement
            });
        }
        catch (RegularPerSaleFeePercentNotPositiveException)
        {
            return BadRequest(new
            {
                error = PlanRequirements
                    .RegularPerSaleFeePercentGreaterThanZeroStatement
            });
        }

        _planRepo.Update(plan);
        return Ok();
    }
}

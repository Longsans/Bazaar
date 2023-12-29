namespace Bazaar.Contracting.Web.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class SellingPlanController : ControllerBase
{
    private readonly IRepository<SellingPlan> _planRepo;

    public SellingPlanController(IRepository<SellingPlan> planRepository)
    {
        _planRepo = planRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SellingPlan>> GetById(int id)
    {
        var plan = await _planRepo.GetByIdAsync(id);
        if (plan == null)
            return NotFound();

        return plan;
    }

    [HttpPost]
    public async Task<ActionResult<SellingPlan>> CreatePlan(SellingPlanRequest request)
    {
        try
        {
            var plan = new SellingPlan(request.Name, request.MonthlyFee,
                request.PerSaleFee, request.RegularPerSaleFeePercent);
            await _planRepo.AddAsync(plan);
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
    public async Task<IActionResult> UpdatePlan(int id, SellingPlanRequest request)
    {
        var plan = await _planRepo.GetByIdAsync(id);
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

        await _planRepo.UpdateAsync(plan);
        return Ok();
    }
}
